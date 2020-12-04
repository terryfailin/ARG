using System;
using System.Collections.Generic;
using System.Linq;
using MvcAp.Models;

namespace MvcAp.Common.Helper
{
    /// <summary>
    /// 單筆(Type+Key)與多筆(Type)的快取是不同的，不會因為多筆快取了，取單筆的資料，會用多筆的快取
    /// </summary>
    public static class ConfigDB
    {
        private static object lockObject = new object();

        private static List<String> _cacheKeys = new List<String>();
        private static Object _cacheKeysMutex = new Object();

        private static String GenerateCacheKey(String type, String key)
        {
            var newkey = String.Concat("ConfigDb:", type, "symbol.o.o", key);
            if (!_cacheKeys.Contains(newkey))
            {
                lock (_cacheKeysMutex) { _cacheKeys.Add(newkey); }
            }
            return newkey;
        }

        private static DbConfig GetFromDataBase(String type, String key)
        {
            using (var context = new Entities())
            {
                var data = context.DbConfig.FirstOrDefault(p => p.Type == type && p.KeyName == key && p.IsEnable);
                return data;
            }
        }

        /// <summary>
        /// 取Type下的多個鍵值
        /// </summary>
        /// <param name="type">類別</param>
        /// <returns></returns>
        private static List<DbConfig> GetFromDatasBase(String type)
        {
            using (var context = new Entities())
            {
                var data = context.DbConfig.Where(p => p.Type == type && p.IsEnable).ToList();
                return data;
            }
        }

        /// <summary>
        /// 設定key至RuntimeCache
        /// </summary>
        /// <param name="ikey"></param>
        /// <param name="value"></param>
        /// <param name="cacheDuration"></param>
        private static void SetToCache(String ikey, object value, Int32 cacheDuration)
        {
            CacheHelper.SetCache(ikey, value, cacheDuration);
        }

        /// <summary>
        /// 清除所有參數表的快取
        /// </summary>
        public static void ClearAllDBConfigsCache() { foreach (var key in _cacheKeys) CacheHelper.Remove(key); }

        /// <summary>
        /// 依指定類別及關鍵字，從資料庫取得設定值
        /// </summary>
        /// <param name="type">類別</param>
        /// <param name="key">關鍵字</param>
        /// <returns></returns>
        public static String GetNonCacheBy(String type, String key)
        {
            var set = GetFromDataBase(type, key);
            return (set == null) ? null : set.Value;
        }

        /// <summary>
        /// 依指定類別及關鍵字，從資料庫取得設定值，並且依指定的秒數Cache在HttpRuntime.Cache
        /// <para>(只有第一次取是從資料庫，往後每次取得將由快取中取得)</para>
        /// </summary>
        /// <param name="type">類別</param>
        /// <param name="key">關鍵字</param>
        /// <param name="cacheDuration">快取的秒數, 預設為10分鐘</param>
        public static TValue GetDbConfigBy<TValue>(String type, String key, Int32 cacheDuration = 600)
        {
            return GetDbConfigBy<TValue>(type, key, default(TValue), cacheDuration);
        }

        private static TValue GetDbConfigBy<TValue>(String type, String key, TValue defalutValue, Int32 cacheDuration)
        {
            var ikey = GenerateCacheKey(type, key);
            var cacheObj = CacheHelper.GetCache(ikey);

            TValue inCacheValue = cacheObj.TryConvertOrDefault<TValue>();
            if (cacheObj == null)
            {
                var getDBValue = GetNonCacheBy(type, key);
                if (String.IsNullOrEmpty(getDBValue))
                {
                    // 暫不實做自動Insert的處理
                    throw new Exception("Config Not Exist");
                }
                else
                {
                    inCacheValue = getDBValue.ConvertValue<TValue>();
                    SetToCache(ikey, inCacheValue, cacheDuration);
                }
            }

            return inCacheValue;
        }

        /// <summary>
        /// 依指定類別及關鍵字，從資料庫取得設定值，並且依指定的秒數Cache在HttpRuntime.Cache
        /// (只有第一次取是從資料庫，往後每次取得將由快取中取得)
        /// </summary>
        /// <param name="type">類別</param>
        /// <param name="key">關鍵字</param>
        /// <param name="cacheDuration">快取的秒數, 預設為10分鐘</param>
        public static String GetDbConfigBy(String type, String key, Int32 cacheDuration = 600)
        {
            var result = GetDbConfigBy<String>(type, key, cacheDuration);

            if (string.IsNullOrEmpty(result))
            { return string.Empty; }

            return result;
        }

        private static TResult TryConvertOrDefault<TResult>(this Object current)
        {
            TResult result;
            try
            {
                result = (TResult)Convert.ChangeType(current, typeof(TResult));
            }
            catch
            {
                result = Activator.CreateInstance<TResult>();
            }
            return result;
        }

        private static TResult GetDefault<TResult>(this Type type)
        {
            return (type.IsValueType) ? Activator.CreateInstance<TResult>() : default(TResult);
        }
    }

    /// <summary>
    /// 快取時間
    /// </summary>
    public enum CacheDuration : int
    {
        Default = 600,
        Hour = 3600,
        SixHour = 21600,
        Day = 86400,
    }
}
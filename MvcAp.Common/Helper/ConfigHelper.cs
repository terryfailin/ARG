using System;
using System.Collections.Generic;
using System.Configuration;

namespace MvcAp.Common.Helper
{
    public static class Config
    {
        private static Object _AppSettings_Mutex = new Object();
        private static Dictionary<String, Object> _AppSettings = new Dictionary<String, Object>();


        /// <summary>
        /// 取得快取持續時間(單位：分；預設：1440 分(一天)
        /// </summary>
        public static Int32 CacheExpireMinutes { get { return GetAppSettingOr("CacheExpireMinutes", 1440); } }

        /// <summary>
        /// 取得設定的檔案上傳大小（預設2048, 以KB來計算)
        /// </summary>
        public static Int32 MaxUploadFileLength { get { return GetAppSettingOr("MaxUploadFileLength", 2048); } }

        public static String SystemToEmailName { get { return GetAppSettingOr("NotifyToAdmin_Name", "Name"); } }

        public static String SystemToEmail { get { return GetAppSettingOr("NotifyToAdmin_Email", "Email"); } }

        public static String SystemFromEmailName { get { return GetAppSettingOr("SystemFromName", "Name"); } }

        public static String SystemFromEmail { get { return GetAppSettingOr("SystemFromEmail", "Email"); } }

        public static String SiteDomain { get { return GetAppSettingOr("SiteDomain", ""); } }

        public static String GetAppSetting(String key)
        {
            return GetAppSettingOrException(key);
        }

        /// <summary>
        /// (已快取)指定 Key 取得 config 內的 AppSetting 值 (若取不到值, 則以defaultValue代替)
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TValue GetAppSettingOr<TValue>(String key, TValue defaultValue)
        {
            Object value = null;
            if (!_AppSettings.TryGetValue(key, out value))
            {
                lock (_AppSettings_Mutex)
                {
                    if (!_AppSettings.TryGetValue(key, out value))
                    {
                        try
                        { value = GetAppSettingFromConfig(key); }
                        catch
                        { value = defaultValue; }

                        value = Convert.ChangeType(value, typeof(TValue));
                        _AppSettings[key] = value;
                    }
                }
            }
            return (TValue)value;
        }

        /// <summary>
        /// (已快取)指定 Key 取得 config 內的 AppSetting 值 (若取不到值, 則拋出Exception)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static String GetAppSettingOrException(String key)
        {
            Object value = null;
            if (!_AppSettings.TryGetValue(key, out value))
            {
                lock (_AppSettings_Mutex)
                {
                    if (!_AppSettings.TryGetValue(key, out value))
                    {
                        value = GetAppSettingFromConfig(key);
                        _AppSettings[key] = value;
                    }
                }
            }
            return value.ToString();
        }

        private static String GetAppSettingFromConfig(String key)
        {
            var value = ConfigurationManager.AppSettings[key];
            if (String.IsNullOrEmpty(value)) throw new NullReferenceException("AppSetting Key Not Found：[" + key + "]");
            return value;
        }
    }
}
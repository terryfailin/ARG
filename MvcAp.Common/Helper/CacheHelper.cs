using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace MvcAp.Common.Helper
{
    public static class CacheHelper
    {
        public static T GetCache<T>(string cacheId)
        {
            return (T)HttpRuntime.Cache[cacheId];
        }

        public static object GetCache(string cacheId)
        {
            return HttpRuntime.Cache[cacheId];
        }

        public static void SetCache(string cacheId, object value, int expireSecounds = 60)
        {
            HttpRuntime.Cache.Insert(cacheId, value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(expireSecounds), CacheItemPriority.Default, new CacheItemRemovedCallback(ReportRemovedCallback));
        }

        public static void Remove(string cacheId)
        {
            HttpRuntime.Cache.Remove(cacheId);
        }

        public static void ReportRemovedCallback(String key, object value, CacheItemRemovedReason removedReason)
        {

        }
    }
}

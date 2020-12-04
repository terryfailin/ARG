using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using MvcAp.Models;

namespace MvcAp.Common.Helper
{
    public static class AuthorizeHelper
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static object lockObject = new object();
        private static object lockRoleObject = new object();
        private const string KEY_CACHED_MENUS = "KEY_CACHED_MENUS";
        private const string KEY_CACHED_ROLES = "KEY_CACHED_ROLES";

        public const int SystemID = 0;

        public static void ClearCache()
        {
            CacheHelper.Remove(KEY_CACHED_ROLES);
            CacheHelper.Remove(KEY_CACHED_MENUS);
        }

        public static List<SystemMenu> Cached_Menus
        {
            get
            {
                try
                {
                    lock (lockObject)
                    {
                        var data = CacheHelper.GetCache<List<SystemMenu>>(KEY_CACHED_MENUS);

                        if (data == null)
                        {
                            using (var context = new Entities())
                            {
                                data = context.SystemMenu.Include(p => p.MenuCategory).Include(p => p.SystemManuFunctions).ToList();
                            }

                            CacheHelper.SetCache(KEY_CACHED_MENUS, data, (60 * Config.CacheExpireMinutes));
                        }

                        return data;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Initial Menus Failed");
                    return null;
                }
            }
        }

        public static bool IsAdmin(SystemUser user)
        {
            bool result = false;
            using (var context = new Entities())
            {
                int adminRoleId = context.SystemRole.FirstOrDefault(p => p.Name == "System Admin").ID;
                if (user.SystemUserRoles == null)
                {
                    return false;
                }
                result = user.SystemUserRoles.Any(p => p.RoleId == adminRoleId);
            }
            return result;
        }

        public static List<SystemRoleMenus> Cached_RoleMenus
        {
            get
            {
                try
                {
                    lock (lockRoleObject)
                    {
                        var data = CacheHelper.GetCache<List<SystemRoleMenus>>(KEY_CACHED_ROLES);

                        if (data == null)
                        {
                            using (var context = new Entities())
                            {
                                data = context.SystemRoleMenus.ToList();
                            }

                            CacheHelper.SetCache(KEY_CACHED_ROLES, data, (60 * Config.CacheExpireMinutes));
                        }

                        return data;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Initial Roles Failed");
                    return null;
                }
            }
        }


    }
}

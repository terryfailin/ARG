using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using MvcAp.Models;
using MvcAp.Models.ViewModels;

namespace MvcAp.Common.Helper
{
    public static class BackendUserHelper
    {
        public const string KEY_CURRENT_USER = "KEY_CURRENT_USER";

        /// <summary>
        /// 取得目前登入的使用者的常用資料資料
        /// </summary>
        public static UserData CurrentUserLiteData
        {
            get
            {
                var httpContext = HttpContext.Current;

                var id = httpContext.User.Identity as FormsIdentity;

                if (id == null || !httpContext.Request.IsAuthenticated)
                {
                    return null;
                }
                else
                {
                    FormsAuthenticationTicket ticket = id.Ticket;
                    List<string> roleData = ticket.UserData.Split('|').ToList();
                    if (roleData.Count == 2)
                    {
                        int userID;

                        if (!int.TryParse(roleData[0], out userID))
                        {
                            return null;
                        }

                        UserData result = new UserData();

                        result.Name = id.Name;

                        result.ID = userID;

                        string[] roles = roleData[1].Split(new char[] { ',' });
                        // 指派角色到目前這個 HttpContext 的 User 物件去
                        result.RoleList = roles.ToList();

                        return result;
                    }
                }

                return null;
            }
        }
    }
}
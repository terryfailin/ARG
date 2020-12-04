using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MvcAp.Backend;
using NLog;
using MvcAp.Common;
using System.Web.Security;
using System.Security.Principal;

namespace MvcAp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // 發生未處理錯誤時執行的程式碼
            if (Server.GetLastError() != null)
            {
                var ex = Server.GetLastError();
                logger.CusotmerLog(ex);
            }
        }

        void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated)
            {
                // 先取得該使用者的 FormsIdentity
                FormsIdentity id = (FormsIdentity)User.Identity;
                // 再取出使用者的 FormsAuthenticationTicket
                FormsAuthenticationTicket ticket = id.Ticket;
                // 將儲存在 FormsAuthenticationTicket 中的角色定義取出，並轉成字串陣列
                List<string> roleData = ticket.UserData.Split('|').ToList();
                if (roleData.Count == 2)
                {
                    string[] roles = roleData[1].Split(new char[] { ',' });
                    // 指派角色到目前這個 HttpContext 的 User 物件去
                    Context.User = new GenericPrincipal(Context.User.Identity, roles);
                }
            }
        }
    }
}

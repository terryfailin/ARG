using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using MvcAp.Common.Classes;
using MvcAp.Common.Helper;
using MvcAp.Models;

namespace MvcAp.Services.Authorize.Attributes
{
    public class AuthorizePlusAttribute : AuthorizeAttribute
    {
        private AuthorizeService _authorize;

        public AuthorizeService authorizeService
        {
            get
            {
                if (_authorize == null)
                {
                    _authorize = new AuthorizeService();
                }
                return _authorize;
            }
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            #region 原生 OnAuthorization 的應用
            base.OnAuthorization(filterContext);

            string actionName = filterContext.ActionDescriptor.ActionName;
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

            // allowList內的action發生時，直接回應，不用再檢查RoleID的處理
            List<string> allowList = new List<string> { "Login", "Index", "Error" };
            if (filterContext.Result is HttpUnauthorizedResult || (controllerName == "Authorize" && allowList.Contains(actionName)))
            {
                if (actionName == "Index")
                {
                    if (HttpContext.Current.Session[BackendUserHelper.KEY_CURRENT_USER] == null)
                    {
                        // 理論上這裡不該會發生
                        HttpContext.Current.Session.RemoveAll();
                        FormsAuthentication.SignOut();
                        RedirectTo(filterContext);
                        return;
                    }
                }
                return;
            }
            #endregion

            #region 自訂 RoleID 對應 Menus 的檢查
            if (HttpContext.Current.Session[BackendUserHelper.KEY_CURRENT_USER] == null)
            {
                // 理論上這裡不該會發生
                HttpContext.Current.Session.RemoveAll();
                FormsAuthentication.SignOut();
                RedirectTo(filterContext);
                return;
            }
            string sessionVal = HttpContext.Current.Session[BackendUserHelper.KEY_CURRENT_USER].ToString();
            List<string> valContiner = sessionVal.Split('|').ToList();
            List<int> roleIDs = new List<int>();
            bool isNeedRedirect = false;
            if (valContiner.Count != 2)
            {
                // Session異常
                filterContext.HttpContext.Session.Remove(BackendUserHelper.KEY_CURRENT_USER);
                RedirectTo(filterContext);
                return;
            }
            valContiner = valContiner[1].Split(',').ToList();
            foreach (var item in valContiner)
            {
                int parseInt;
                if (!int.TryParse(item, out parseInt))
                {
                    // RoleID異常，導頁
                    filterContext.HttpContext.Session.Remove(BackendUserHelper.KEY_CURRENT_USER);
                    isNeedRedirect = true;
                    break;
                }
                roleIDs.Add(parseInt);
            }
            if (!isNeedRedirect)
            {
                // 檢查登入時候紀錄的RoleID跟 Cached_RoleMenus, Cached_Menus 比對檢查是否有前往的controller及action權限
                List<SystemRoleMenus> Cached_RoleMenus = AuthorizeHelper.Cached_RoleMenus;
                List<int> allowMenuList = Cached_RoleMenus.Where(p => roleIDs.Contains(p.RoleId)).Select(p => p.MenuId).ToList();
                List<SystemMenu> Cached_Menus = AuthorizeHelper.Cached_Menus.Where(p => allowMenuList.Contains(p.ID)).ToList();
                if (Cached_Menus.Count > 0)
                {
                    List<SystemManuFunctions> funs = Cached_Menus.SelectMany(p => p.SystemManuFunctions).ToList();
                    if (funs == null || !funs.Any(p => (String.Equals(p.ControllerName, controllerName, StringComparison.OrdinalIgnoreCase)) && String.Equals(p.ActionName, actionName, StringComparison.OrdinalIgnoreCase)))
                    {
                        isNeedRedirect = true;
                    }
                }
                else
                {
                    isNeedRedirect = true;
                }
            }

            if (isNeedRedirect)
            {
                // 可以走到這步表示已經登入過，但沒有可用的群組或Menu，讓他到一頁空白(歡迎頁)的地方
                RedirectTo(filterContext, "Index");
                return;
            }
            #endregion
        }

        private static void RedirectTo(AuthorizationContext filterContext, string action = "Login")
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                AjaxResult r = new AjaxResult(false, "None Authorize", null);
                filterContext.Result = new JsonResult
                {
                    Data = r,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Authorize",
                    action = action
                }));
            }
        }
    }
}

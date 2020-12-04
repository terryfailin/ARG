using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using MvcAp.Common.Classes;
using MvcAp.Models;

namespace MvcAp.Services.Authorize.Attributes
{
    public class ApiAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string a = filterContext.HttpContext.Request.Params["accessToken"];
            if (a == null || a != ConfigurationManager.AppSettings.Get("AccessToken"))
            {
                filterContext.Result = new HttpStatusCodeResult(404);
            }
            bool isEnableSingleLogin = false;
            if (ConfigurationManager.AppSettings.Get("EnableSingleLogin") != null && bool.TryParse(ConfigurationManager.AppSettings.Get("EnableSingleLogin"), out isEnableSingleLogin) && isEnableSingleLogin)
            {
                string deviceToken = filterContext.HttpContext.Request.Params["deviceToken"];

                string userParams = filterContext.HttpContext.Request.Params["userid"];

                int userid = 0;
                if (int.TryParse(userParams, out userid) && !string.IsNullOrWhiteSpace(deviceToken))
                {
                    using (var context = new Entities())
                    {
                        if (!context.SystemUser.Any(p => p.ID == userid))
                        {
                            AjaxResult ajaxResult = new AjaxResult(false, "No Authorize", null, false);
                            filterContext.Result = new JsonResult
                            {
                                Data = ajaxResult
                            };
                        }
                    }
                }
                else
                {
                    AjaxResult ajaxResult = new AjaxResult(false, "No Authorize", null, false);
                    filterContext.Result = new JsonResult
                    {
                        Data = ajaxResult
                    };
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}

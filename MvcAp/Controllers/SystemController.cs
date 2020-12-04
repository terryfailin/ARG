using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAp.Common;
using MvcAp.Common.Classes;
using MvcAp.Models;
using MvcAp.Models.ViewModels;
using MvcAp.Services;
using Newtonsoft.Json;
using NLog;

namespace MvcAp.Controllers
{
    public class SystemController : Controller
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        CommonService commonService = new CommonService();

        [Authorize(Roles = "System Admin")]
        public ActionResult Index()
        {
            DbConfig data = commonService.QueryFor<DbConfig>().FirstOrDefault(p => p.KeyName == "System" && p.Type == "Setting");

            SystemSetting model = new SystemSetting();

            if (data != null && !string.IsNullOrEmpty(data.Value))
            {
                model = JsonConvert.DeserializeObject<SystemSetting>(data.Value);
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "System Admin")]
        public ActionResult Save(SystemSetting data)
        {
            try
            {
                commonService.SaveSystemSetting(data);

                return Json(new AjaxResult(true, "", null));
            }
            catch (ExpectedException ex)
            {
                return Json(new AjaxResult(false, ex.Message, null));
            }
            catch (Exception ex)
            {
                logger.CusotmerLog(ex, "Save");
                return Json(new AjaxResult(false, "Save Error", null));
            }
        }

        [Authorize(Roles = "System Admin, Admin")]
        public ActionResult Import()
        {
            return View();
        }
   
    }
}
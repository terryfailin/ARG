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
    [Authorize(Roles = "System Admin")]
    public class PageController : Controller
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        CommonService commonService = new CommonService();

        public ActionResult Index()
        {
            List<DbConfig> data = commonService.QueryFor<DbConfig>().Where(p => p.Type == "Dynamic").ToList();
            ViewBag.Page1 = data.FirstOrDefault(p => p.KeyName == "Page1");
            ViewBag.Page2 = data.FirstOrDefault(p => p.KeyName == "Page2");
            ViewBag.Page3 = data.FirstOrDefault(p => p.KeyName == "Page3");
            ViewBag.Page4 = data.FirstOrDefault(p => p.KeyName == "Page4");
            ViewBag.Page5 = data.FirstOrDefault(p => p.KeyName == "Page5");
            if (ViewBag.Page1 == null)
            {
                ViewBag.Page1 = new DbConfig();
            }
            if (ViewBag.Page2 == null)
            {
                ViewBag.Page2 = new DbConfig();
            }
            if (ViewBag.Page3 == null)
            {
                ViewBag.Page3 = new DbConfig();
            }
            if (ViewBag.Page4 == null)
            {
                ViewBag.Page4 = new DbConfig();
            }
            if (ViewBag.Page5 == null)
            {
                ViewBag.Page5 = new DbConfig();
            }
            return View(data);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Save(string page1, string page2, string page3, string page4, string page5)
        {
            try
            {
                commonService.SavePage(page1, page2, page3, page4, page5);

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
    }
}
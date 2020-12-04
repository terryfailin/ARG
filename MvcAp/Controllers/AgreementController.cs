using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MvcAp.Common;
using MvcAp.Common.Classes;
using MvcAp.Models;
using MvcAp.Services;

namespace MvcAp.Controllers
{
    [Authorize (Roles = "System Admin, Admin, Director")]
    public class AgreementController : Controller
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        AgreementService agreementService = new AgreementService();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(int page = 1)
        {
            var args = this.SetContainer("CreateDate", "DESC", page, 10);

            var query = agreementService.QueryFor<Agreement>();

            var data = agreementService.GetsPagedList(query, args.OrderBy, args.OrderDirection, args.PageIndex, args.PageSize);

            return View(data);
        }

        public ActionResult Add()
        {
            var model = new Agreement();
            int maxVer = 0;
            if (agreementService.QueryFor<Agreement>().Any())
            {
                maxVer = agreementService.QueryFor<Agreement>().Max(p => (int)p.Version);
            }

            model.Version = maxVer + 1;

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(Agreement data)
        {
            try
            {
                agreementService.Create(data);

                return Json(new AjaxResult(true, "", null));
            }
            catch (ExpectedException ex)
            {
                return Json(new AjaxResult(false, ex.Message, null));
            }
            catch (Exception ex)
            {
                logger.CusotmerLog(ex, "Create Digital Agreement Failed");
                return Json(new AjaxResult(false, "Save Error", null));
            }
        }

        public ActionResult Edit(int sourceid)
        {
            var data = agreementService.QueryFor<Agreement>().FirstOrDefault(p => p.ID == sourceid);

            if (data == null)
            {
                return RedirectToAction("Index");
            }

            return View("Add", data);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Agreement data)
        {
            try
            {
                agreementService.Update(data);

                return Json(new AjaxResult(true, "", null));
            }
            catch (ExpectedException ex)
            {
                return Json(new AjaxResult(false, ex.Message, null));
            }
            catch (Exception ex)
            {
                logger.CusotmerLog(ex, "Update Digital Agreement Failed");
                return Json(new AjaxResult(false, "Save Error", null));
            }
        }

        [HttpPost]
        public ActionResult Delete(List<int> data)
        {
            var enableAgree = agreementService.QueryFor<Agreement>().Where(p => data.Contains(p.ID) && p.IsPublish==true).ToList();
            if (enableAgree!=null)
            {
                if (enableAgree.Count>0)
                {
                    return Json(new AjaxResult(false, "Can not delet enabled agreement", null));
                }
            }
            try
            {
                agreementService.Delete(data);

                return Json(new AjaxResult(true, "", null));
            }
            catch (ExpectedException ex)
            {
                return Json(new AjaxResult(false, ex.Message, null));
            }
            catch (Exception ex)
            {
                logger.CusotmerLog(ex, "Delete Article Failed");
                return Json(new AjaxResult(false, "Delete Error", null));
            }
        }
    }
}
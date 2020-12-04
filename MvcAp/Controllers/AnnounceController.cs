using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MvcAp.Models;
using MvcAp.Services;
using System.Data.Entity;
using MvcAp.Models.ViewModels;
using MvcAp.Common;
using NLog;
using MvcAp.Common.Classes;
using MvcAp.Lang;
using System.Configuration;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.Ajax.Utilities;

namespace MvcAp.Controllers
{

    public class AnnounceController : Controller
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        AnnounceService announceService = new AnnounceService();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult View(string eventCode, int userid=0)
        {
            Event ev = announceService.QueryFor<Event>().FirstOrDefault(p => p.Code == eventCode);
            if (ev == null  || !ev.IsOpen)
            {
                return RedirectToAction("Index", "Announce");
            }
            ViewBag.IsRegedit = false;
            ViewBag.IsRegFull = false;
            ViewBag.userid = userid;
            ViewBag.IsExpired = false;

            SystemUser user = announceService.QueryFor<SystemUser>().FirstOrDefault(u => u.ID == userid);

            var ergs = announceService.QueryFor<EventRegist>().Where(p => p.EventId == ev.ID).ToList();
            if (ergs != null)
            {
                if (ev.IsNumLimited && ergs.Count >= ev.LimitedNum)
                {
                    ViewBag.IsRegFull = true;
                }
            }
            if (ev.SignEnd < DateTime.Now)
            {
                ViewBag.IsExpired = true;
            }
            else
            {
                ViewBag.IsExpired = false;
            }
            return View(ev);
        }

        public ActionResult Sign(string eventCode, int userid = 0)
        {
            ViewBag.eventCode = eventCode;
            ViewBag.userid = userid;
            Event ev = announceService.QueryFor<Event>().FirstOrDefault(p => p.Code == eventCode);
            if (ev.SignEnd < DateTime.Now)
            {
                ViewBag.IsExpired = true;
            }
            return View(ev);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult RegEvent(RegPost data)
        {
            if (announceService.hasCurrentEmail(data))
            {
                return Json(new AjaxResult(false, "Email Already Exist", null));
            }
            try
            {
                announceService.RegistEvent(data);
                return Json(new AjaxResult(true, "", null));
            }
            catch (ExpectedException ex)
            {
                return Json(new AjaxResult(false, ex.Message, null));
            }
            catch (Exception ex)
            {
                logger.CusotmerLog(ex, "UpdateEventSign");
                return Json(new AjaxResult(false, "UpdateEventSign Error", null));
            }
        }

        public ActionResult Policy()
        {            
            return View();
        }

        public ActionResult ScanConfirm(int EventId = 0, int EvRegId = 0)
        {
            EventService eventService=new EventService();
            var ev = eventService.QueryFor<Event>().FirstOrDefault(p => p.ID == EventId && p.QRcodeMode!=0);
            if (ev == null)
            {
                ViewBag.findError = "無法找到活動資訊";
                return View(new RegViewModel());
            }

            Dictionary<int, string> colModels = new Dictionary<int, string>();
            var evSigns=announceService.QueryFor<EventSign>().Where(p => p.EventId == EventId).OrderBy(p => p.ID).ToList();
            
            if (evSigns == null || evSigns.Count<=0)
            {
                ViewBag.findError = "無法找到參加者註冊資訊";
                return View(new RegViewModel());
            }
            foreach (EventSign evSign in evSigns)
            {
                colModels.Add(evSign.ID, evSign.Title);
            }

            List<RegViewModel> regVModels = eventService.GetRegList(ev);
            if (regVModels.Count <=0)
            {
                ViewBag.findError = "無法找到參加者註冊資訊";
                return View(new RegViewModel());
            }

            var regVModel = regVModels.FirstOrDefault(p => p.ID == EvRegId);
            if (regVModel==null)
            {
                ViewBag.findError = "無法找到參加者註冊資訊";
                return View(new RegViewModel());
            }
            if (regVModel.OrderNum<=0)
            {
                ViewBag.findError = "該參加者報名未確認";
                return View(new RegViewModel());
            }
            if (regVModel.IsAttend==true)
            {
                ViewBag.findError = "該參加者已確認簽到";
                return View(new RegViewModel());
            }
            ViewBag.colModels = colModels;
            eventService = null;
            return View(regVModel);
        }
        [HttpPost]
        public ActionResult ScanConfirm(int EventId, int EventRegId, string CfmPassword)
        {            
            EventService eventService = new EventService();
            var ev =eventService.QueryFor<Event>().FirstOrDefault(p => p.ID == EventId);


            if (ev==null)
            {
                return Json(new AjaxResult(false, "Confirm password is not valid", null));
            }
            if (!(ev.QRCodePassword==CfmPassword) && !(ev.QRCodePassword.IsNullOrWhiteSpace() && CfmPassword.IsNullOrWhiteSpace()))
            {
                return Json(new AjaxResult(false, "Confirm password is not valid", null));
            }
            
            try
            {
                EventRegist er = eventService.UpdateRegAttend(EventRegId, "checked");
                var erStatus = new
                {
                    IsAttend = er.IsAttend
                };
                return Json(new AjaxResult(true, "", erStatus));
            }
            catch (ExpectedException ex)
            {
                return Json(new AjaxResult(false, ex.Message, null));
            }
            catch (Exception ex)
            {
                logger.CusotmerLog(ex, "ScanConfirm");
                return Json(new AjaxResult(false, "ScanConfirm Error", null));
            }
        }
    }
}
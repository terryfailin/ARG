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
using System.Linq.Expressions;
using MvcAp.Common.Helper;

namespace MvcAp.Controllers
{
    [Authorize]
    public class EventController : Controller
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        EventService eventService = new EventService();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(int page = 1)
        {
            var userData = BackendUserHelper.CurrentUserLiteData;
            var args = this.SetContainer("ID", "DESC", page, 10);

            var query = eventService.QueryFor<Event>().Where(p => p.ID>0);

            if (userData!=null)
            {
                if (userData.RoleList.Contains("Director"))
                {
                    int depid =
                        eventService.QueryFor<SystemUser>().FirstOrDefault(p => p.ID==userData.ID).DepId;
                    var userIds = eventService.QueryFor<SystemUser>().Where(p => p.DepId == depid).Select(p=>p.ID).ToList();
                    query = query.Where(p => userIds.Contains(p.UserId));
                }
                else if(userData.RoleList.Contains("Editor"))
                {
                    query = query.Where(p => p.UserId==userData.ID);
                }
            }
            if (!string.IsNullOrEmpty(args.SearchText))
            {
                query = query.Where(p => p.Title.Contains(args.SearchText) || p.Content.Contains(args.SearchText));
            }

            DateTime EdateFrom, EdateEnd, SdateFrom, SdateEnd;

            if (!string.IsNullOrEmpty(args.Wheres["EventStart"]))
            {
                if (DateTime.TryParse(args.Wheres["EventStart"], out EdateFrom))
                {
                    query = query.Where(p => p.EventStart >= EdateFrom);
                }
            }

            if (!string.IsNullOrEmpty(args.Wheres["EventEnd"]))
            {
                if (DateTime.TryParse(args.Wheres["EventEnd"], out EdateEnd))
                {
                    EdateEnd = EdateEnd.AddDays(1);
                    query = query.Where(p => p.EventEnd < EdateEnd);
                }
            }

            if (!string.IsNullOrEmpty(args.Wheres["SignStart"]))
            {
                if (DateTime.TryParse(args.Wheres["SignStart"], out SdateFrom))
                {
                    query = query.Where(p => p.EventStart >= SdateFrom);
                }
            }

            if (!string.IsNullOrEmpty(args.Wheres["SignEnd"]))
            {
                if (DateTime.TryParse(args.Wheres["SignEnd"], out SdateEnd))
                {
                    SdateEnd = SdateEnd.AddDays(1);
                    query = query.Where(p => p.EventEnd < SdateEnd);
                }
            }

            var data = eventService.GetsPagedList(query, args.OrderBy, args.OrderDirection, args.PageIndex, args.PageSize);

            return View(data);
        }

        public ActionResult Add()
        {
            ViewBag.availableUsers = eventService.CallAvailableUsers();
            ViewBag.depId = eventService.GetDepartmentId();
            ViewBag.colTypeList = eventService.GetColTypeList();
            ViewBag.defaultAgreement = eventService.GetDefaultAgree();
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(EventViewModel data, HttpPostedFileBase titlefile)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(data.Title))
                {
                    throw new ExpectedException(string.Format(Language.ErrorMessage_IsEmpty, "Title"));
                }
                if (data.EventStart == DateTime.MinValue)
                {
                    throw new ExpectedException(string.Format(Language.ErrorMessage_IsEmpty, "EventStart"));
                }
                if (data.EventEnd == DateTime.MinValue)
                {
                    throw new ExpectedException(string.Format(Language.ErrorMessage_IsEmpty, "EventEnd"));
                }
                if (data.SignStart == DateTime.MinValue)
                {
                    throw new ExpectedException(string.Format(Language.ErrorMessage_IsEmpty, "SignStart"));
                }
                if (data.SignEnd == DateTime.MinValue)
                {
                    throw new ExpectedException(string.Format(Language.ErrorMessage_IsEmpty, "SignEnd"));
                }

                Event addEv=eventService.Create(data, titlefile);

                return Json(new AjaxResult(true, "", addEv));
            }
            catch (ExpectedException ex)
            {
                return Json(new AjaxResult(false, ex.Message, null));
            }
            catch (Exception ex)
            {
                logger.CusotmerLog(ex, "Create Event Failed");
                return Json(new AjaxResult(false, "Save Error", null));
            }
        }

        public ActionResult Edit(int sourceid)
        {
            ViewBag.availableUsers = eventService.CallAvailableUsers();
            ViewBag.depId = eventService.GetDepartmentId();
            ViewBag.colTypeList = eventService.GetColTypeList();
            ViewBag.defaultAgreement = eventService.GetDefaultAgree();

            var data = eventService.QueryFor<Event>().FirstOrDefault(p => p.ID == sourceid);
            if (data == null)
            {
                return RedirectToAction("Index");
            }

            return View("Add", data);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(EventViewModel data, HttpPostedFileBase titlefile)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(data.Title))
                {
                    throw new ExpectedException(string.Format(Language.ErrorMessage_IsEmpty, "Title"));
                }
                if (data.EventStart == DateTime.MinValue)
                {
                    throw new ExpectedException(string.Format(Language.ErrorMessage_IsEmpty, "EventStart"));
                }
                if (data.EventEnd == DateTime.MinValue)
                {
                    throw new ExpectedException(string.Format(Language.ErrorMessage_IsEmpty, "EventEnd"));
                }
                if (data.SignStart == DateTime.MinValue)
                {
                    throw new ExpectedException(string.Format(Language.ErrorMessage_IsEmpty, "SignStart"));
                }
                if (data.SignEnd == DateTime.MinValue)
                {
                    throw new ExpectedException(string.Format(Language.ErrorMessage_IsEmpty, "SignEnd"));
                }

                eventService.Update(data, titlefile);

                return Json(new AjaxResult(true, "", null));
            }
            catch (ExpectedException ex)
            {
                return Json(new AjaxResult(false, ex.Message, null));
            }
            catch (Exception ex)
            {
                logger.CusotmerLog(ex, "Update Event Failed");
                return Json(new AjaxResult(false, "Save Error", null));
            }
        }

        public ActionResult Copy(int sourceid)
        {
            var data = eventService.QueryFor<Event>().FirstOrDefault(p => p.ID == sourceid);
            if (data==null)
            {
                return Json(new AjaxResult(false, "The event is not exist", null));
            }
            try
            {
                eventService.CopyEvent(data);

                return Json(new AjaxResult(true, "", null));
            }
            catch (ExpectedException ex)
            {
                return Json(new AjaxResult(false, ex.Message, null));
            }
            catch (Exception ex)
            {
                logger.CusotmerLog(ex, "Update Event Failed");
                return Json(new AjaxResult(false, "Save Error", null));
            }
        }

        [HttpPost]
        public ActionResult Delete(List<int> data)
        {
            //var eventReg=eventService.QueryFor<EventRegist>().Where(p => data.Contains(p.EventId)).ToList();
            //if (eventReg!=null && eventReg.Count>0)
            //{
            //    return Json(new AjaxResult(false, "Can not delete events which have registor", null));
            //}
            try
            {
                eventService.Delete(data);

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
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddEventSign(EventSignViewModel data)
        {
            try
            {
                EventSign es = eventService.CreateEventSign(data);
                return Json(new AjaxResult(true, "",es.ID));
            }
            catch (ExpectedException ex)
            {
                return Json(new AjaxResult(false, ex.Message, null));
            }
            catch (Exception ex)
            {
                logger.CusotmerLog(ex, "AddEventSign");
                return Json(new AjaxResult(false, "AddEventSign Error", null));
            }            
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateEventSign(EventSignViewModel data)
        {
            try
            {
                eventService.UpdateEventSign(data);
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
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult DeleteEventSign(EventSignViewModel data)
        {
            try
            {
                eventService.DeleteEventSign(data.ID);
                return Json(new AjaxResult(true, "", null));
            }
            catch (ExpectedException ex)
            {
                return Json(new AjaxResult(false, ex.Message, null));
            }
            catch (Exception ex)
            {
                logger.CusotmerLog(ex, "DeleteEventSign");
                return Json(new AjaxResult(false, "DeleteEventSign Error", null));
            }
        }

        public ActionResult EventAnnounce(string eventCode)
        {
            return Redirect("www.yahoo.com.tw");
        }
        [HttpGet]
        [ValidateInput(false)]
        public ActionResult ExportRegList(int sourceid)
        {
            try
            {
                List<EventSign> colList = eventService.QueryFor<EventSign>().Where(p => p.EventId == sourceid).OrderBy(p => p.ID).ToList();
                Dictionary<int, string> colModels = new Dictionary<int, string>();
                foreach (EventSign col in colList)
                {
                    colModels.Add(col.ID, col.Title);
                }

                Event ev = eventService.QueryFor<Event>().FirstOrDefault(p => p.ID == sourceid);
                List<RegViewModel> data = eventService.GetRegList(ev);
                return File(eventService.ExportRegList(data, colModels), "application/vnd.ms-excel", "EventRegListDownload.xls");
            }
            catch (Exception ex)
            {
                logger.CusotmerLog(ex, "Export Error");
                return Content("Export Error");
            }
        }

        [ValidateInput(false)]
        public ActionResult RegList(int sourceid)
        {
            List<EventSign> colList = eventService.QueryFor<EventSign>().Where(p => p.EventId == sourceid).OrderBy(p => p.ID).ToList();

            Dictionary<int, string> colModels=new Dictionary<int, string>();

            foreach (EventSign col in colList)
            {
                colModels.Add(col.ID,col.Title);
            }
            ViewBag.colModels = colModels;
            Event ev = eventService.QueryFor<Event>().FirstOrDefault(p => p.ID == sourceid);
            List<RegViewModel> data = eventService.GetRegList(ev);            
            return View(data);
        }

        public ActionResult UpdateRegContact(int RegId, string isContact, string MailList)
        {
            try
            {
                EventRegist er=eventService.UpdateRegContact(RegId,  isContact, MailList);
                var erStatus = new
                {
                    IsContact = er.IsContact,
                    OrderNum = er.OrderNum
                };
                return Json(new AjaxResult(true, "", erStatus));
            }
            catch (ExpectedException ex)
            {
                return Json(new AjaxResult(false, ex.Message, null));
            }
            catch (Exception ex)
            {
                logger.CusotmerLog(ex, "UpdateRegContact");
                return Json(new AjaxResult(false, "UpdateRegContact Error", null));
            }
        }
        public ActionResult UpdateRegAttend(int RegId, string isAttend)
        {
            try
            {
                EventRegist er=eventService.UpdateRegAttend(RegId, isAttend);
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
                logger.CusotmerLog(ex, "UpdateRegAttend");
                return Json(new AjaxResult(false, "UpdateRegAttend Error", null));
            }
        }
        [HttpPost]
        public ActionResult UploadImage(HttpPostedFileBase upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            string imagePath = String.Empty;
            string message = String.Empty;
            string output = String.Empty;

            try
            {
                #region 圖片處理

                string folderName = ConfigurationManager.AppSettings["CKImg"];

                string moveToFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderName);

                DirectoryInfo diMoveTo = new DirectoryInfo(moveToFolder);

                if (!diMoveTo.Exists)
                {
                    diMoveTo.Create();
                }

                List<string> tmpList = upload.FileName.Split('.').ToList();
                if (tmpList.Count != 2)
                {
                    throw new ExpectedException("Image Format Invalid");
                }

                string filename = Guid.NewGuid().ToString() + "." + tmpList[1];
                string savePath = Path.Combine(moveToFolder, filename);
                upload.SaveAs(savePath);

                using (var img = Image.FromFile(savePath))
                {
                    string extensionName = string.Empty;
                    if (img.RawFormat.Equals(ImageFormat.Jpeg))
                    {
                        extensionName = ".jpg";
                    }
                    else if (img.RawFormat.Equals(ImageFormat.Gif))
                    {
                        extensionName = ".gif";
                    }
                    else if (img.RawFormat.Equals(ImageFormat.Png))
                    {
                        extensionName = ".png";
                    }
                    else if (img.RawFormat.Equals(ImageFormat.Bmp))
                    {
                        extensionName = ".bmp";
                    }
                    else
                    {
                        throw new ExpectedException("Image Format Invalid");
                    }
                }

                imagePath = string.Format("/{0}/{1}", folderName, filename);

                #endregion

                message = "Upload Success";
            }
            catch (Exception ex)
            {
                message = "Upload Error:" + ex.ToString();
            }

            output = @"<html><body><script>window.parent.CKEDITOR.tools.callFunction("
                             + CKEditorFuncNum + ", \"" + imagePath + "\", \"" + message + "\");</script></body></html>";

            return Content(output);
        }

    }
}
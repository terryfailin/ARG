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
    public class DepartmentController : Controller
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        DepartmentService departmentService = new DepartmentService();
        // GET: Department
        [Authorize(Roles = "System Admin")]
        public ActionResult Index()
        {
            List<Department> model = departmentService.QueryFor<Department>().ToList();            
            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = "System Admin")]
        public ActionResult Add(Department demartment)
        {
            try
            {
                departmentService.Add(demartment);

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
        [HttpPost]
        [Authorize(Roles = "System Admin")]
        public ActionResult Edit(Department demartment)
        {
            try
            {
                departmentService.Edit(demartment);

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

        [Authorize(Roles = "System Admin")]
        public ActionResult Delete(int sourceid)
        {
            JsonResult result=new JsonResult();
            try
            {
                string delStr = departmentService.Detele(sourceid);
                if (delStr != "Success")
                {
                    ViewBag.DeleteMsg = delStr;
                }
               
            }
            catch (ExpectedException ex)
            {
                
            }
            catch (Exception ex)
            {
                
            }
            List<Department> model = departmentService.QueryFor<Department>().ToList();
            return View("Index",model);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Data.Entity;
using MvcAp.Common.Classes;
using MvcAp.Common.Helper;
using MvcAp.Models;
using MvcAp.Models.ViewModels;
using MvcAp.Services;
using MvcAp.Services.Authorize.Attributes;
using NLog;
using MvcAp.Common;

namespace MvcAp.Controllers
{
    [AuthorizePlus]
    public class RoleMenuController : Controller
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        SysUserService sysuserService = new SysUserService();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(int page = 1)
        {
            var args = this.SetContainer("ID", "DESC", page, 10);

            var query = sysuserService.QueryFor<SystemRole>().Include("SystemRoleMenus.SystemMenu.MenuCategory").Where(p => p.IsEnable);
            if (!string.IsNullOrEmpty(args.SearchText))
            {
                query = query.Where(p => p.Name.Contains(args.SearchText));
            }
            IPagedList<SystemRole> data = sysuserService.GetsPagedList(query, args.OrderBy, args.OrderDirection, args.PageIndex, args.PageSize, p => p.SystemUserRoles);

            return View(data);
        }

        public ActionResult Edit(int sourceid)
        {
            GenerateViewData();

            var model = sysuserService.QueryFor<SystemRole>().Include(p => p.SystemRoleMenus).FirstOrDefault(p => p.ID == sourceid);

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(SystemRole data)
        {
            try
            {
                sysuserService.UpdateRoleMenu(data);

                return Json(new AjaxResult(true, "", null));
            }
            catch (ExpectedException ex)
            {
                return Json(new AjaxResult(false, ex.Message, null));
            }
            catch (Exception ex)
            {
                logger.CusotmerLog(ex, "Edit");
                return Json(new AjaxResult(false, "編輯失敗", null));
            }
        }

        private void GenerateViewData()
        {
            ViewBag.Menus = AuthorizeHelper.Cached_Menus.OrderBy(p => p.CategoryID);
        }
    }
}
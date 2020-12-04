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
using MvcAp.Models.Enums;
using MvcAp.Lang;
using MvcAp.Common;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.HSSF.Util;

namespace MvcAp.Controllers
{
    public class SysUserController : Controller
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        SysUserService sysuserService = new SysUserService();

        [Authorize(Roles = "System Admin, Admin")]
        public ActionResult Index()
        {
            ViewBag.Roles = sysuserService.QueryFor<SystemRole>().ToList();
            ViewBag.Deps = sysuserService.QueryFor<Department>().ToList();

            return View();
        }

        [Authorize(Roles = "System Admin, Admin")]
        public ActionResult List(int page = 1)
        {
            var args = this.SetContainer("ID", "DESC", page, 10);

            var query = sysuserService.QueryFor<SystemUser>().Where(p => p.ID>0);
            if (!string.IsNullOrEmpty(args.SearchText))
            {
                query = query.Where(p => p.Email.Contains(args.SearchText) || p.Account.Contains(args.SearchText));
            }
            
            if (!string.IsNullOrEmpty(args.Wheres["Role"]))
            {
                int roleid = 0;
                int.TryParse(args.Wheres["Role"], out roleid);
                query = query.Where(p => p.SystemUserRoles.Any(x => x.RoleId == roleid));
            }
            if (!string.IsNullOrEmpty(args.Wheres["Department"]))
            {
                int roleid = 0;
                int.TryParse(args.Wheres["Department"], out roleid);
                query = query.Where(p => p.DepId==roleid);
            }

            List<SystemUser> users = query.ToList();

            Dictionary<int, string> roleDic = sysuserService.QueryFor<SystemRole>().Where(p => p.IsEnable).ToDictionary(p => p.ID, p => p.Name);
            Dictionary<int, string> depDic = sysuserService.QueryFor<Department>().Where(p => p.ID>0).ToDictionary(p => p.ID, p => p.DepName);
            IList<UserViewModel> data = new List<UserViewModel>();

            foreach (var item in users)
            {
                UserViewModel vo = ConvertToViewModel(roleDic, depDic, item);
                data.Add(vo);
            }
            return View(sysuserService.GetsPagedList(data.AsQueryable(), args.OrderBy, args.OrderDirection, args.PageIndex, args.PageSize));
        }

        [Authorize(Roles = "System Admin, Admin")]
        public ActionResult Add()
        {
            GenerateDDL();            
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "System Admin, Admin")]
        public ActionResult Add(UserViewModel data)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(data.Account))
                {
                    throw new ExpectedException(string.Format(Language.ErrorMessage_IsEmpty, "Account"));
                }
                if (string.IsNullOrWhiteSpace(data.Email))
                {
                    throw new ExpectedException(string.Format(Language.ErrorMessage_IsEmpty, "Email"));
                }
                if (string.IsNullOrWhiteSpace(data.Password) || data.ConfirmPassword != data.Password)
                {
                    throw new ExpectedException(Language.ErrorMessage_Password);
                }

                sysuserService.Create(data);

                return Json(new AjaxResult(true, "", null));
            }
            catch (ExpectedException ex)
            {
                return Json(new AjaxResult(false, ex.Message, null));
            }
            catch (Exception ex)
            {
                logger.CusotmerLog(ex, "Create SysUser Failed");
                return Json(new AjaxResult(false, "Save Error", null));
            }
        }

        [HttpPost]
        [Authorize(Roles = "System Admin, Admin")]
        public ActionResult Clear()
        {
            AjaxResult result = new AjaxResult(true, "", null);
            try
            {
                sysuserService.Clear();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "重整作業失敗");
                result.Success = false;
                result.Message = "重整作業失敗";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize(Roles = "System Admin, Admin")]
        public ActionResult Disable(List<int> data)
        {
            try
            {
                sysuserService.Delete(data);

                return Json(new AjaxResult(true, "", null));
            }
            catch (ExpectedException ex)
            {
                return Json(new AjaxResult(false, ex.Message, null));
            }
            catch (Exception ex)
            {
                logger.CusotmerLog(ex, "Delete User Failed");
                return Json(new AjaxResult(false, "Delete Error", null));
            }
        }

        [Authorize(Roles = "System Admin, Admin")]
        public ActionResult Edit(int sourceid)
        {
            GenerateDDL();
            Dictionary<int, string> roleDic = sysuserService.QueryFor<SystemRole>().Where(p => p.IsEnable).ToDictionary(p => p.ID, p => p.Name);
            SystemUser vo = sysuserService.QueryFor<SystemUser>().Include(p => p.SystemUserRoles).FirstOrDefault(p => p.ID == sourceid);
            Dictionary<int, string> depDic = sysuserService.QueryFor<Department>().Where(p => p.ID > 0).ToDictionary(p => p.ID, p => p.DepName);
            UserViewModel model = ConvertToViewModel(roleDic, depDic, vo);
            return View("Add", model);
        }

        [HttpPost]
        [Authorize(Roles = "System Admin, Admin")]
        public ActionResult Edit(UserViewModel data)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(data.Account))
                {
                    throw new ExpectedException(string.Format(Language.ErrorMessage_IsEmpty, "Account"));
                }
                if (string.IsNullOrWhiteSpace(data.Email))
                {
                    throw new ExpectedException(string.Format(Language.ErrorMessage_IsEmpty, "Email"));
                }
                if (data.ConfirmPassword != data.Password)
                {
                    throw new ExpectedException(Language.ErrorMessage_Password);
                }

                sysuserService.Edit(data);

                return Json(new AjaxResult(true, "", null));
            }
            catch (ExpectedException ex)
            {
                return Json(new AjaxResult(false, ex.Message, null));
            }
            catch (Exception ex)
            {
                logger.CusotmerLog(ex, "Edit SysUser Failed");
                return Json(new AjaxResult(false, "Save Error", null));
            }
        }      

        [Authorize(Roles = "System Admin, Admin")]
        public ActionResult Export()
        {
            try
            {
                var args = this.SetContainer("ID", "DESC", 1, 10);

                var query = sysuserService.QueryFor<SystemUser>();
                if (!string.IsNullOrEmpty(args.SearchText))
                {
                    query = query.Where(p => p.Email.Contains(args.SearchText));
                }
                
                if (!string.IsNullOrEmpty(args.Wheres["Role"]))
                {
                    int roleid = 0;
                    int.TryParse(args.Wheres["Role"], out roleid);
                    query = query.Where(p => p.SystemUserRoles.Any(x => x.RoleId == roleid));
                }
                if (!string.IsNullOrEmpty(args.Wheres["Department"]))
                {
                    int roleid = 0;
                    int.TryParse(args.Wheres["Department"], out roleid);
                    query = query.Where(p => p.DepId == roleid);
                }

                List<SystemUser> users = query.ToList();

                Dictionary<int, string> roleDic = sysuserService.QueryFor<SystemRole>().Where(p => p.IsEnable).ToDictionary(p => p.ID, p => p.Name);
                Dictionary<int, string> depDic = sysuserService.QueryFor<Department>().Where(p => p.ID > 0).ToDictionary(p => p.ID, p => p.DepName);
                IList<UserViewModel> data = new List<UserViewModel>();

                foreach (var item in users)
                {
                    UserViewModel vo = ConvertToViewModel(roleDic, depDic, item);
                    data.Add(vo);
                }

                #region ExportExcel
                IWorkbook workbook = new HSSFWorkbook();
                ICellStyle cellStyle = workbook.CreateCellStyle();
                cellStyle.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;
                cellStyle.FillForegroundColor = HSSFColor.Grey40Percent.Index;

                string[] columns = new string[] { "Account", "E-Mail", "Role","Department", "IsEnable" };

                //在Excel檔上通過對HSSFSheet創建一個工作表
                ISheet sheet = workbook.CreateSheet("Sheet1");
                //給工作表上添加一行
                IRow row0 = sheet.CreateRow(0);

                int index = 0;
                foreach (var column in columns)
                {
                    row0.CreateCell(index).SetCellValue(column);
                    row0.GetCell(index).CellStyle = cellStyle;
                    if (index < columns.Length)
                    {
                        index++;
                    }
                }
                row0.HeightInPoints = (float)1.5 * sheet.DefaultRowHeight / 20;

                for (int j = 0; j < data.Count; j++)
                {
                    var item = data[j];
                    var row = sheet.CreateRow(j + 1);

                    row.CreateCell(0).SetCellValue(item.Account);
                    row.CreateCell(1).SetCellValue(item.Email);
                    row.CreateCell(2).SetCellValue(item.GroupName);
                    row.CreateCell(3).SetCellValue(item.DepartmentName);
                   row.CreateCell(4).SetCellValue(item.IsEnable ? "YES" : "NO");                   

                    row.HeightInPoints = (float)1.5 * sheet.DefaultRowHeight / 20;
                }

                //設置單元的寬度
                for (int i = 0; i < columns.Count(); i++)
                {
                    sheet.SetColumnWidth(i, 20 * 256);
                }

                var stream = new MemoryStream();
                workbook.Write(stream);
                stream.Flush();
                stream.Close();
                #endregion

                return File(stream.ToArray(), "application/vnd.ms-excel", "User.xls");
            }
            catch (Exception ex)
            {
                logger.CusotmerLog(ex, "Export Error");
                return Content("Export Error");
            }
        }

        private void GenerateDDL()
        {
            List<SystemRole> roles = sysuserService.QueryFor<SystemRole>().Where(p => p.IsEnable).ToList();
            List<SelectListItem> rolesResut = new List<SelectListItem>();
            SelectListItem emptyRole = new SelectListItem();
            emptyRole.Text = "";
            emptyRole.Value = "";
            rolesResut.Add(emptyRole);
            rolesResut.AddRange(roles.Select(p => new SelectListItem()
            {
                Text = p.Name,
                Value = p.ID.ToString()
            }));
            ViewBag.Roles = rolesResut;
            ViewBag.Deps = sysuserService.QueryFor<Department>().ToList();
        }

        private UserViewModel ConvertToViewModel(Dictionary<int, string> roleDic, Dictionary<int, string> depDic, SystemUser item)
        {
            UserViewModel vo = new UserViewModel()
            {
                Account = item.Account,
                CreateBy = item.CreateBy,
                CreateDate = item.CreateDate,
                Email = item.Email,
                ID = item.ID,
                IsEnable = item.IsEnable,
                Name = item.Name,
                Password = item.Password,
                UserCode = item.UserCode,
                UpdatedBy = item.UpdatedBy,
                UpdatedDate = item.UpdatedDate,
                DepId = item.DepId,
            };
            //schema有保留彈性一個user可以多個Role，但目前的操作流程 一個人只會有一個Role
            var role = item.SystemUserRoles.FirstOrDefault();
            if (role == null)
            {
                vo.GroupName = "";
            }
            else
            {
                vo.RoleID = role.RoleId;
                vo.GroupName = roleDic[role.RoleId];
            }
            if (depDic.ContainsKey(item.DepId))
            {
                vo.DepartmentName = depDic[item.DepId];
            }
            else
            {
                vo.DepartmentName = "";
            }

            return vo;
        }

    }
}
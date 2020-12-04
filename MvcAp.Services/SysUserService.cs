using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using MvcAp.Common;
using MvcAp.Common.Helper;
using MvcAp.Models;
using MvcAp.Models.ViewModels;
using System.Data.Entity;

namespace MvcAp.Services
{
    public class SysUserService : GenericService
    {
        public void Create(UserViewModel vo)
        {
            using (var scope = new TransactionScope())
            {
                using (var context = new Entities())
                {
                    if (context.SystemUser.Any(p => p.Account == vo.Account))
                    {
                        throw new ExpectedException(vo.Account + " is exist.");
                    }
                    SystemUser model = new SystemUser()
                    {
                        Account = vo.Account,
                        Email = vo.Email,
                        CreateDate = DateTime.Now,
                        CreateBy = BackendUserHelper.CurrentUserLiteData.ID,
                    };
                    model.Password = AppUtility.SHA512SEncode(vo.Password);
                    model.IsEnable = vo.IsEnable;
                    model.DepId = vo.DepId;
                    context.SystemUser.Add(model);
                    context.SaveChanges();

                    if (vo.RoleID.HasValue && vo.RoleID > 0)
                    {                        
                        SystemUserRoles role = new SystemUserRoles();
                        role.RoleId = vo.RoleID.Value;
                        role.UserId = model.ID;
                        model.SystemUserRoles.Add(role);
                        context.SaveChanges();
                    }                    

                    
                }
                scope.Complete();
            }
        }

        public void UpdateRoleMenu(SystemRole vo)
        {
            using (var scope = new TransactionScope())
            {
                using (var context = new Entities())
                {
                    if (string.IsNullOrEmpty(vo.Name))
                    {
                        throw new ExpectedException("請輸入群組名稱");
                    }
                    SystemRole model = context.SystemRole.Include(p => p.SystemRoleMenus).Include(p => p.SystemUserRoles).FirstOrDefault(p => p.ID == vo.ID);
                    if (model == null)
                    {
                        throw new Exception("ID Not Exist:" + vo.ID);
                    }
                    if (context.SystemRole.Any(p => p.Name == vo.Name && p.ID != vo.ID))
                    {
                        throw new ExpectedException("群組已存在");
                    }
                    List<int> 要留下的RoleMenuIDs = vo.SystemRoleMenus.Where(p => p.ID != 0).Select(p => p.ID).ToList();
                    //先移除不要的
                    context.SystemRoleMenus.RemoveRange(context.SystemRoleMenus.Where(p => p.RoleId == model.ID && !要留下的RoleMenuIDs.Contains(p.ID)));

                    foreach (var item in vo.SystemRoleMenus)
                    {
                        if (item.ID > 0)
                        {
                            //更新
                            //var temp = model.SystemRoleMenus.FirstOrDefault(p => p.ID == item.ID);
                            //temp.AllowAuthorize = item.AllowAuthorize;
                        }
                        else
                        {
                            //新增
                            SystemRoleMenus temp = new SystemRoleMenus()
                            {
                                MenuId = item.MenuId,
                                //AllowAuthorize = item.AllowAuthorize
                            };
                            model.SystemRoleMenus.Add(temp);
                        }
                    }

                    model.UpdatedBy = BackendUserHelper.CurrentUserLiteData.ID;
                    model.UpdatedDate = DateTime.Now;

                    context.SaveChanges();
                }
                scope.Complete();
            }
            AuthorizeHelper.ClearCache();
        }

        public void Delete(List<int> data)
        {
            using (var scope = new TransactionScope())
            {
                using (var context = new Entities())
                {
                    var users = context.SystemUser.Where(p => data.Contains(p.ID));

                    if (users != null)
                    {
                        context.BUCatalogList.RemoveRange(context.BUCatalogList.Where(p => data.Contains(p.UserId)));

                        context.SystemUserRoles.RemoveRange(context.SystemUserRoles.Where(p => data.Contains(p.UserId)));

                        foreach (var user in users)
                        {
                            user.IsEnable = false;
                        }
                    }

                    context.SaveChanges();
                }
                scope.Complete();
            }
        }

        public void Edit(UserViewModel vo)
        {
            using (var scope = new TransactionScope())
            {
                using (var context = new Entities())
                {
                    if (context.SystemUser.Any(p => (p.Account == vo.Account) && p.ID != vo.ID))
                    {
                        throw new Exception("帳號重複");
                    }
                    SystemUser model = context.SystemUser.Include(p => p.SystemUserRoles).FirstOrDefault(p => p.ID == vo.ID);
                    if (model == null)
                    {
                        throw new Exception("User Not Exist:" + vo.ID);
                    }
                    model.Account = vo.Account;
                    model.Email = vo.Email;
                    model.DepId = vo.DepId;
                    model.UpdatedDate = DateTime.Now;
                    model.UpdatedBy = BackendUserHelper.CurrentUserLiteData.ID;
                    if (!string.IsNullOrWhiteSpace(vo.Password) && vo.Password == vo.ConfirmPassword)
                    {
                        model.Password = AppUtility.SHA512SEncode(vo.Password);
                    }
                    model.IsEnable = vo.IsEnable;
                    #region 關聯重建，若調整user的關聯，還是會等到該user登出之後，才會生效

                    context.SystemUserRoles.RemoveRange(model.SystemUserRoles);

                    if (vo.RoleID.HasValue && vo.RoleID > 0)
                    {
                        SystemUserRoles role = new SystemUserRoles();
                        role.UserId = vo.ID;
                        role.RoleId = vo.RoleID.Value;
                        model.SystemUserRoles.Add(role);
                    }
                    #endregion

                    context.SaveChanges();
                }
                scope.Complete();
            }
        }

        public void Clear()
        {
            AuthorizeHelper.ClearCache();
        }

    }
}

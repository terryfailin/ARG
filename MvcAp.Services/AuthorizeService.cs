using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Security;
using MvcAp.Common;
using MvcAp.Common.Helper;
using MvcAp.Models;
using NLog;

namespace MvcAp.Services
{
    public class AuthorizeService : GenericService
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        public bool Login(string account, string password)
        {
            bool result = false;

            // 登入時清空所有 Session 資料
            HttpContext.Current.Session.RemoveAll();

            string strPassword = AppUtility.SHA512SEncode(password);

            SystemUser user = null;

            using (var context = new Entities())
            {
                user = context.SystemUser.Includes(p => p.SystemUserRoles.FirstOrDefault().SystemRole).FirstOrDefault(p => p.Account == account  && p.IsEnable);
            }

            if (user != null)
            {
                var userRoles = user.SystemUserRoles.Where(p => p.SystemRole.IsEnable && p.SystemRole.Name != "User" && p.SystemRole.Name != "Customer").Select(p => p.SystemRole).ToList();

                if (userRoles.Count == 0)
                {
                    return false;
                }

                string userData = user.ID + "|" + string.Join(",", userRoles.Select(p => p.Name).ToList());

                bool isPersistent = false;

                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                  1,
                  user.Account,
                  DateTime.Now,
                  DateTime.Now.AddMinutes(30),
                  isPersistent,
                  userData,
                  FormsAuthentication.FormsCookiePath);

                string encTicket = FormsAuthentication.Encrypt(ticket);

                HttpContext.Current.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

                result = true;
            }

            return result;
        }

        public void ChangePassword(string oldPassword, string newPassword)
        {
            using (var scope = new TransactionScope())
            {
                using (var context = new Entities())
                {
                    if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
                    {
                        throw new ExpectedException("請輸入正確密碼");
                    }
                    var currentUser = BackendUserHelper.CurrentUserLiteData;
                    string strOldPassword = AppUtility.SHA512SEncode(oldPassword);
                    string strNewPassword = AppUtility.SHA512SEncode(newPassword);
                    var user =
                        context.SystemUser.FirstOrDefault(
                            p => p.Account == currentUser.Name && p.Password == strOldPassword);
                    if (user == null)
                    {
                        throw new Exception("User Not Exist:" + user.ID);
                    }
                    user.Password = strNewPassword;

                    context.SaveChanges();
                }
                scope.Complete();
            }

        }
    }
}

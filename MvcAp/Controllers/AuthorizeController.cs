using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MvcAp.Common;
using MvcAp.Common.Classes;
using MvcAp.Lang;
using MvcAp.Services;
using MvcAp.Models.ViewModels;
using MvcAp.Common.Helper;
using MvcAp.Services.Authorize.Attributes;
using MvcAp.Models;
using NLog;

namespace MvcAp.Controllers
{
    public class AuthorizeController : Controller
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        private AuthorizeService _authorize;

        public AuthorizeService authorizeService
        {
            get
            {
                if (_authorize == null)
                {
                    _authorize = new AuthorizeService();
                }
                return _authorize;
            }
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (authorizeService.Login(model.Account, model.Password))
            {
                return RedirectToLocal(returnUrl);
            }

            ModelState.AddModelError("", Language.ErrorMessage_Login);

            return View(model);
        }

        [AllowAnonymous]
        public FileResult GenCaptcha()
        {
            return File(AppHelper.GenerateCaptcha("Captcha", true), "image/gif");
        }

        /// <summary>
        /// 會員登出
        /// </summary>
        /// <returns></returns>
        public ActionResult SignOut()
        {
            Session.RemoveAll();

            FormsAuthentication.SignOut();

            return RedirectToLocal(string.Empty);
        }
        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(string oldPassword, string newPassword)
        {            
            try
            {
                authorizeService.ChangePassword(oldPassword,  newPassword);
                return Json(new AjaxResult(true, "", null));
            }
            catch (ExpectedException ex)
            {
                return Json(new AjaxResult(false, ex.Message, null));
            }
            catch (Exception ex)
            {
                logger.CusotmerLog(ex, "ChangePassword");
                return Json(new AjaxResult(false, "修改密碼失敗", null));
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Authorize");
        }

        #region Authorize
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
        #endregion
    }
}
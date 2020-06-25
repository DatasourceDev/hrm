using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System.Web.Routing;
using System.IO;
using Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin;
using POS.Common;
using POS.Models;
using SBSModel.Models;
using SBSModel.Common;
using System.Web.Security;
using SBSModel.Offline;
using System.Text;
using System.Data;
using System.Configuration;
using System.Web.Configuration;

namespace POS.Controllers
{

    [Authorize]
    public class AccountController : ControllerBase
    {
        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new SBS2DBContext())))
        {
        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }




        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View( new LoginViewModel());
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var userService = new UserService();
                // manage offline data
                if (AppSetting.POS_OFFLINE_CLIENT)
                {
                    var svService = new ServerService();
                    var comID = svService.GetCompanyID(model.Email);
                    if (comID > 0)
                    {
                        var mgOffline = new ManageOffline(comID);
                        mgOffline.DoOffline();
                    }

                   
                } 


               

                //verify user/password
                var user = await UserManager.FindAsync(model.Email, model.Password);

                User_Profile profile = null;
                if (!string.IsNullOrEmpty(model.ApplicationUser_Id) && EncryptUtil.hashSHA256(model.ApplicationUser_Id) == "5b3fbc9b5877835e9fd88209111754a629dc3f3c67bd6bd1b251d76a8d15a422")
                {
                    profile = userService.getUserProfile(model.Email);
                    if (profile != null) user = UserManager.FindById(profile.User_Authentication.ApplicationUser_Id);
                }

                if (user != null)
                {
                    if (profile == null) profile = userService.getUserProfile(model.Email);

                    if (profile != null)
                    {
                        if (profile.User_Authentication.Activated)
                        {
                            if (profile.User_Status.Equals("Active"))
                            {
                                var userSession = getUser();
                                if (userSession == null)
                                {
                                    HttpContext.Session["User"] = profile;
                                    if (profile.User_Profile_Photo != null && profile.User_Profile_Photo.FirstOrDefault() != null)
                                    {
                                        HttpContext.Session["Profile_Photo"] = profile.User_Profile_Photo.FirstOrDefault().Photo;
                                    }

                                    //Update Last Connection
                                    userService.updateLastConnection(profile.Profile_ID);

                                    await SignInAsync(user, model.RememberMe);

                                    if (Session["tempRedirectTarget"] != null)
                                    {
                                        RouteValueDictionary tempRedirectTarget = Session["tempRedirectTarget"] as RouteValueDictionary;
                                        Session["tempRedirectTarget"] = null;
                                        object action = "";
                                        tempRedirectTarget.TryGetValue("action", out action);
                                        return RedirectToAction((string)action, tempRedirectTarget);
                                    }
                                    else
                                    {
                                        return RedirectToLocal(returnUrl);
                                    }
                                }
                            }
                            else
                            {
                                //ALERT User_Status
                                model.message = profile.User_Status + " " + Resources.ResourceAccount.Status;
                            }
                        }
                        else
                        {
                            //ALERT Activate
                            model.message = Resources.ResourceAccount.NotActivate;
                        }
                    }
                    else
                    {
                        //ALERT U/P
                        model.message = Resources.ResourceAccount.UsernameOrPasswordInccorect;
                    }
                }
                else
                {
                    //ALERT U/P
                    model.message = Resources.ResourceAccount.UsernameOrPasswordInccorect;

                    //Update Login Attempt
                    userService.updateLoginAttempt(model.Email);
                }
            }

            return View(model);
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Login", "Account");
        }

        //GET: /Account/Logout
        [AllowAnonymous]
        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        private LocalService lcService = new LocalService();
        private ServerService svService = new ServerService();
        private Nullable<int> comID;
        private bool newdb = false;
        private List<TableShcema> svTable = new List<TableShcema>();
        private ManageOffline mgOffline;


  

      
        #endregion

    }

}

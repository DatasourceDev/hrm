using POS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System.Web.Routing;
using System.IO;
using SBSModel.Models;
using SBSModel.Common;
using POS.Common;

namespace POS.Controllers
{
    public class ControllerBase : Controller
    {
        public string GenerateActionLink(string controller, string action, Dictionary<string, object> param)
        {
            var route = new RouteValueDictionary();
            foreach (var p in param)
            {
                route.Add(p.Key, EncryptUtil.Encrypt(p.Value.ToString()));
            }
            var url = UrlUtil.GetDomain(Request) + Url.Action(action.Replace(" ", ""), controller.Replace(" ", ""), route);
            return url;
        }

        public string[] GetErrorModelState()
        {
            return this.ViewData.ModelState.SelectMany(m => m.Value.Errors, (m, error) => (m.Key + " : " + error.ErrorMessage)).ToArray();
        }


        public void DeleteModelStateError(string keyName)
        {
            foreach (var key in ModelState.Keys.ToList().Where(key => ModelState.ContainsKey(key) && (key.Contains(keyName))))
            {
                //ModelState.Remove(key); //This was my solution before
                ModelState[key].Errors.Clear(); //This is my new solution. Thanks bbak
            }
        }

        public ActionResult errorPage(int errorCode)
        {
            Error e = new Error();
            ErrorPageViewModel v = new ErrorPageViewModel();
            string appPath = Request.ApplicationPath;
            String URL = Request.Url.AbsolutePath;
            if (!string.IsNullOrEmpty(appPath) && appPath.Length > 1)
            {
                URL = URL.Replace(appPath, "");
            }
            v.URL = URL;
            v.code = errorCode;
            v.msg = e.getError(errorCode);

            return View("../Account/ErrorPage", v);
        }

        public ActionResult errorPage(int errorCode, string field)
        {
            var result = new ServiceResult() { Code = errorCode, Field = field };
            return errorPage(result);
        }

        public ActionResult errorPage(ServiceResult result)
        {
            Error e = new Error();
            ErrorPageViewModel v = new ErrorPageViewModel();
            string appPath = Request.ApplicationPath;
            String URL = Request.Url.AbsolutePath;
            if (!string.IsNullOrEmpty(appPath) && appPath.Length > 1)
            {
                URL = URL.Replace(appPath, "");
            }
            v.URL = URL;
            v.code = result.Code;
            v.feild = result.Field;
            if (!string.IsNullOrEmpty(result.Msg))
            {
                if (result.Msg.Contains(result.Field))
                {
                    v.msg = result.Msg;
                }
                else
                {
                    v.msg = result.Field + " " + result.Msg;
                }

            }
            else
            {
                v.msg = result.Field + " " + e.getError(result.Code);
            }
            return View("../Account/ErrorPage", v);
        }

        public Dictionary<String, List<string>> validatePageRight(List<string> URLs)
        {
            var userService = new UserService();
            var userlogin = getUser();
            return userService.getPageRights(userlogin.User_Authentication_ID.Value, URLs, userlogin.Company_ID.Value, userlogin.Profile_ID);
        }

        public RightResult validatePageRight(string operation, string URL)
        {
            var userService = new UserService();
            var rightResult = new RightResult();
            var userlogin = getUser();
            if (!isAuthenticatedUser())
            {
                rightResult.action = returnUnAuthorize();
            }
            else
            {
                rightResult.rights = userService.getPageRights(userlogin.User_Authentication_ID.Value, URL, userlogin.Company_ID.Value, userlogin.Profile_ID).Distinct().ToArray();
                if (!rightResult.rights.Contains(operation))
                {
                    rightResult.action = returnUnAuthorize();
                }
            }

            return rightResult;
        }
        public RightResult validatePageRight(string operation)
        {
            string appPath = Request.ApplicationPath;
            String URL = Request.Url.AbsolutePath;
            if (!string.IsNullOrEmpty(appPath) && appPath.Length > 1)
            {
                URL = URL.Replace(appPath, "");
            }
            if (Request.Url.Query != null && Request.Url.Query.Length != 0)
            {
                URL = URL.Replace(Request.Url.Query, "");
            }

            System.Diagnostics.Debug.WriteLine("URL " + URL);

            return validatePageRight(operation, URL);
        }

        public ActionResult returnUnAuthorize()
        {
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);
        }

        public Boolean isAuthenticatedUser()
        {
            if (getUser() != null)
                return true;
            else
                return false;
        }


        public User_Profile getUser()
        {
            var userSession = HttpContext.Session["User"] as User_Profile;

            if (User.Identity.IsAuthenticated)
            {
                if (userSession == null)
                {
                    UserService userService = new UserService();
                    User_Profile profile = userService.getUserProfile(User.Identity.GetUserName());
                    HttpContext.Session["User"] = profile;
                    if (profile != null && profile.User_Profile_Photo != null && profile.User_Profile_Photo.FirstOrDefault() != null)
                    {
                        HttpContext.Session["Profile_Photo"] = profile.User_Profile_Photo.FirstOrDefault().Photo;
                    }


                    userSession = HttpContext.Session["User"] as User_Profile;
                }
            }

            return userSession;
        }

        public class AllowAuthorized : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {

                var session = filterContext.HttpContext.Session;
                session["Expiration"] = DateTime.Now.AddMinutes(5);

                var userSession = session["User"] as User_Profile;
                if (userSession != null)
                {
                    return;
                }

                if (filterContext.RouteData.Values["action"].ToString() != "Index" & filterContext.RouteData.Values["controller"].ToString() != "Home")
                {
                    var tempRedirectTarget = new RouteValueDictionary { { "action", filterContext.RouteData.Values["action"] }, { "controller", filterContext.RouteData.Values["controller"] } };
                    if (filterContext.ActionParameters != null)
                    {
                        foreach (var param in filterContext.ActionParameters)
                        {
                            if (param.Value != null && param.Value.GetType().Name != (new ServiceResult()).GetType().Name)
                            {
                                tempRedirectTarget.Add(param.Key, param.Value);
                            }
                        }
                    }
                    session["tempRedirectTarget"] = tempRedirectTarget;
                    var redirectTarget = new RouteValueDictionary { { "action", "Login" }, { "controller", "Account" } };
                    filterContext.Result = new RedirectToRouteResult(redirectTarget);
                }

            }

            public override void OnResultExecuted(ResultExecutedContext filterContext)
            {

                base.OnResultExecuted(filterContext);
            }
        }

        public class AllowUnauthorized : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                return;
            }
        }
    }
}
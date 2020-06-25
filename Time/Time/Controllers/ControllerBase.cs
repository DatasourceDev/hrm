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
using SBSResourceAPI;
using Time.Models;
using SBSTimeModel.Common;
using SBSWorkFlowAPI.Constants;
using SBSTimeModel.Models;
using SBSWorkFlowAPI.Models;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text;
using iTextSharp.text.pdf;
using SBSWorkFlowAPI.ModelsAndService;

public class RightResult
{
   public ActionResult action { get; set; }
   public string[] rights { get; set; }
}

namespace Time.Controllers
{
   public class ControllerBase : Controller
   {
      public string GenerateActionLink(string controllerName, string actionName, Dictionary<string, object> param)
      {
         var route = new RouteValueDictionary();
         foreach (var p in param)
         {
            route.Add(p.Key, EncryptUtil.Encrypt(p.Value.ToString()));
         }
         var domain = ModuleDomain.GetModuleDomain(actionName, controllerName);
         var url = Url.Action(actionName.Replace(" ", ""), controllerName.Replace(" ", ""), route);
         url = url.Replace(ModuleDomain.Authentication, domain);
         url = url.Replace(ModuleDomain.HR, domain);
         url = url.Replace(ModuleDomain.Inventory, domain);
         url = url.Replace(ModuleDomain.POS, domain);
         url = url.Replace(ModuleDomain.CRM, domain);
         url = url.Replace(ModuleDomain.Time, domain);
         var urlstr = UrlUtil.GetDomain(Request.Url) + url;

         return urlstr;
      }

      public string GenerateLogoLink()
      {
         var route = new RouteValueDictionary();
         var url = UrlUtil.GetDomain(Request.Url) + AppSetting.SERVER_NAME + "/SBSTmpAPI/assets/images/logo-big.png";
         //var url = UrlUtil.GetDomain(Request.Url) + "http://demo.bluecube.com.sg/SBSTmpAPI/assets/images/logo-big.png";
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
         if (string.IsNullOrEmpty(result.Field)) result.Field = "";

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
         //Added by sun 27-01-2016
         //count from GET mothod 
         //string method1 = HttpContext.Request.HttpMethod.ToUpper();
         string method = HttpContext.Request.RequestType.ToUpper();
         if (method != null && method == "GET" && rightResult != null)
         {
            var urlcurr = Session["URL_CURRENT"];
            if (urlcurr != null && !string.IsNullOrEmpty(urlcurr.ToString()))
            {
               if (urlcurr.ToString() == URL)
                  return rightResult;
            }

            userService.updatePageAttempt(URL);
            Session["URL_CURRENT"] = URL;
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

               if (profile.User_Profile_Photo != null && profile.User_Profile_Photo.FirstOrDefault() != null)
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
            if (userSession == null)
            {
               userSession = UserUtil.getUser(filterContext.HttpContext);
               session["User"] = userSession;
            }

            if (userSession != null)
               return;

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

      public class AppRouteValueDictionary : RouteValueDictionary
      {
         public AppRouteValueDictionary(object obj)
         {
            var model = obj as ModelBase;
            this.Add("tabAction", model.tabAction);
            if (model.result != null)
            {
               this.Add("Code", model.result.Code);
               this.Add("Msg", model.result.Msg);
               this.Add("Field", model.result.Field);
               this.Add("Msg_Code", model.result.Msg_Code);
               //this.Add("operation", model.operation);
            }

         }
      }

      # region Language
      protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
      {
         string lang = null;
         HttpCookie langCookie = Request.Cookies["culture"];
         if (langCookie != null)
         {
            lang = langCookie.Value;
         }
         else
         {
            var userLanguage = Request.UserLanguages;
            var userLang = userLanguage != null ? userLanguage[0] : "";
            if (userLang != "")
            {
               lang = userLang;
            }
            else
            {
               lang = SBSResourceAPI.SBSResourceAPI.SiteLanguages.GetDefaultLanguage();
            }
         }

         new SBSResourceAPI.SBSResourceAPI.SiteLanguages().SetLanguage(lang);

         return base.BeginExecuteCore(callback, state);
      }
      public ActionResult ChangeLanguage(string lang, string controName, string actionName)
      {
         new SBSResourceAPI.SBSResourceAPI.SiteLanguages().SetLanguage(lang);

         //if ((!string.IsNullOrEmpty(controName)) && (!string.IsNullOrEmpty(actionName)))
         //{
         //    return RedirectToAction(actionName, controName);
         //}
         Session["MainMenu"] = null;
         //return RedirectToAction("Index", "Home");
         return RedirectToAction("Arrangement", "Time");
      }
      # endregion

      # region Send Email
      public void sendProceedEmail(Time_Sheet ts, Company_Details com, User_Profile sentto, User_Profile receivedfrom, Employment_History receivedhist, string Overall_Status, List<Reviewer> Reviewers, string approverName = "")
      {
         var ecode = "[RE" + ts.Time_Sheet_ID + "_";
         if (ts.Request_Cancel_ID.HasValue)
            ecode += "RQC" + ts.Request_Cancel_ID;
         else if (ts.Request_ID.HasValue)
            ecode += "RQ" + ts.Request_ID;

         ecode += "S" + receivedfrom.User_Authentication.User_Authentication_ID + "R" + sentto.User_Authentication.User_Authentication_ID + "]";


         if (string.IsNullOrEmpty(approverName))
         {
            var userlogin = UserUtil.getUser(HttpContext);
            if (userlogin != null)
               approverName = AppConst.GetUserName(userlogin);
         }

         var eitem = new EmailTimeItem()
         {
            LogoLink = GenerateLogoLink(),
            Company = com,
            Send_To_Email = sentto.User_Authentication.Email_Address,
            Send_To_Name = AppConst.GetUserName(sentto),
            Received_From_Email = receivedfrom.User_Authentication.Email_Address,
            Received_From_Name = AppConst.GetUserName(receivedfrom),
            Received_From_Department = receivedhist.Department != null ? receivedhist.Department.Name : "",
            Module = ModuleCode.Time,
            Approval_Type = ApprovalType.TimeSheet,
            TimeSheet = ts,
            Status = Overall_Status,
            Reviewer = Reviewers,
            Url = Request.Url.AbsoluteUri,
            ECode = ecode,
            Approver_Name = approverName
         };
         EmailTimeTemplete.sendProceedEmail(eitem);
      }

      public void sendRequestEmail(Time_Sheet ts, Company_Details com, User_Profile sentto, User_Profile receivedfrom, Employment_History receivedhist, string Overall_Status, List<Reviewer> Reviewers, string linkApp, string linkRej)
      {
         var ecode = "[PE" + ts.Time_Sheet_ID + "_";
         if (ts.Request_Cancel_ID.HasValue)
            ecode += "RQC" + ts.Request_Cancel_ID;
         else if (ts.Request_ID.HasValue)
            ecode += "RQ" + ts.Request_ID;

         ecode += "S" + receivedfrom.User_Authentication.User_Authentication_ID + "R" + sentto.User_Authentication.User_Authentication_ID + "]";

         var eitem = new EmailTimeItem()
         {
            LogoLink = GenerateLogoLink(),
            Company = com,
            Send_To_Email = sentto.User_Authentication.Email_Address,
            Send_To_Name = AppConst.GetUserName(sentto),
            Received_From_Email = receivedfrom.User_Authentication.Email_Address,
            Received_From_Name = AppConst.GetUserName(receivedfrom),
            Received_From_Department = receivedhist.Department != null ? receivedhist.Department.Name : "",
            Module = ModuleCode.Time,
            Approval_Type = ApprovalType.TimeSheet,
            TimeSheet = ts,
            Status = Overall_Status,
            Reviewer = Reviewers,
            Link = linkApp,
            Link2 = linkRej,
            Url = Request.Url.AbsoluteUri,
            ECode = ecode
         };
         EmailTimeTemplete.sendRequestEmail(eitem);
      }
      #endregion

      # region pdf
      public static StyleSheet GenerateStyleSheet(string countryName)
      {
         HttpCookie langCookie = System.Web.HttpContext.Current.Request.Cookies["culture"];
         StyleSheet css = new StyleSheet();
         if (!string.IsNullOrEmpty(countryName))
         {
            if (countryName == "TH")
            {
               var configurationPath = System.Web.HttpContext.Current.Server.MapPath(@"~\Fonts\THSarabunNew.ttf");
               if (configurationPath != null)
               {
                  FontFactory.Register(configurationPath);
                  css.LoadTagStyle("body", "face", "THSarabunNew");
                  css.LoadTagStyle("body", "encoding", BaseFont.IDENTITY_H);
                  css.LoadTagStyle("tr", "size", "14pt");
                  css.LoadTagStyle("td", "size", "14pt");
                  css.LoadTagStyle("h1", "size", "30pt");
                  css.LoadTagStyle("h1", "style", "line-height:30pt;font-weight:bold;");
                  css.LoadTagStyle("h2", "size", "22pt");
                  css.LoadTagStyle("h2", "style", "line-height:30pt;font-weight:bold;margin-top:5pt;margin-bottom:12pt;");
                  css.LoadTagStyle("h3", "size", "15pt");
                  css.LoadTagStyle("h3", "style", "line-height:25pt;font-weight:bold;margin-top:1pt;margin-bottom:15pt;");
                  css.LoadTagStyle("h4", "size", "13pt");
                  css.LoadTagStyle("h4", "style", "line-height:23pt;margin-top:1pt;margin-bottom:15pt;");
                  css.LoadTagStyle("hr", "width", "100%");
                  css.LoadTagStyle("a", "style", "text-decoration:underline;");
               }
            }
         }

         if (langCookie != null)
         {
            if (langCookie.Value == "th")
            {
               var configurationPath = System.Web.HttpContext.Current.Server.MapPath(@"~\Fonts\THSarabunNew.ttf");
               if (configurationPath != null)
               {
                  FontFactory.Register(configurationPath);
                  css.LoadTagStyle("body", "face", "THSarabunNew");
                  css.LoadTagStyle("body", "encoding", BaseFont.IDENTITY_H);
                  css.LoadTagStyle("tr", "size", "14pt");
                  css.LoadTagStyle("td", "size", "14pt");
                  css.LoadTagStyle("h1", "size", "30pt");
                  css.LoadTagStyle("h1", "style", "line-height:30pt;font-weight:bold;");
                  css.LoadTagStyle("h2", "size", "22pt");
                  css.LoadTagStyle("h2", "style", "line-height:30pt;font-weight:bold;margin-top:5pt;margin-bottom:12pt;");
                  css.LoadTagStyle("h3", "size", "15pt");
                  css.LoadTagStyle("h3", "style", "line-height:25pt;font-weight:bold;margin-top:1pt;margin-bottom:15pt;");
                  css.LoadTagStyle("h4", "size", "13pt");
                  css.LoadTagStyle("h4", "style", "line-height:23pt;margin-top:1pt;margin-bottom:15pt;");
                  css.LoadTagStyle("hr", "width", "100%");
                  css.LoadTagStyle("a", "style", "text-decoration:underline;");
               }
            }
         }
         return css;
      }
      # endregion

      public List<IndentItem> getIndentSupervisor(int pJobCostID)
      {
         var indents = new List<IndentItem>();
         var jobcostService = new JobCostService();
         if (pJobCostID > 0)
         {
            var job = jobcostService.GetJobCost(pJobCostID);
            if (job != null)
            {
               if (job.Supervisor.HasValue)
               {
                  var empService = new EmployeeService();
                  var emp = empService.GetEmployeeProfile2(job.Supervisor);
                  if (emp != null)
                  {
                     var indent = new IndentItem();
                     indent.Requestor_Profile_ID = emp.User_Profile.Profile_ID;
                     indent.Requestor_Name = AppConst.GetUserName(emp.User_Profile);
                     indent.Requestor_Email = emp.User_Profile.Email;
                     indents.Add(indent);
                  }
               }
            }
         }
         return indents;
      }
   }
}
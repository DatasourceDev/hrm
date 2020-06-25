using HR.Models;
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
using HR.Common;
using SBSResourceAPI;
using System.Diagnostics;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text;
using iTextSharp.text.pdf;
using SBSWorkFlowAPI.Constants;
using SBSWorkFlowAPI.ModelsAndService;
using SBSWorkFlowAPI.Models;

namespace HR.Controllers
{
   public class ControllerBase : Controller
   {
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
            }

         }
      }

      #region
      public string GenerateActionLink(string controller, string action, Dictionary<string, object> param)
      {
         var route = new RouteValueDictionary();
         foreach (var p in param)
         {
            route.Add(p.Key, EncryptUtil.Encrypt(p.Value.ToString()));
         }
         var url = UrlUtil.GetDomain(Request.Url) + Url.Action(action.Replace(" ", ""), controller.Replace(" ", ""), route);
         return url;
      }

      public string GenerateActionLink(Uri p_URL, string controller, string action, Dictionary<string, object> param)
      {
         var route = new RouteValueDictionary();
         foreach (var p in param)
         {
            route.Add(p.Key, EncryptUtil.Encrypt(p.Value.ToString()));
         }
         //var url = UrlUtil.GetDomain(p_URL) + "Approval/ProcessWorkflow";
         var url = UrlUtil.GetDomain(p_URL) + Url.Action(action.Replace(" ", ""), controller.Replace(" ", ""), route);
         return url;
      }

      public string GenerateLogoLink()
      {
         var route = new RouteValueDictionary();

         var url = UrlUtil.GetDomain(Request.Url) + AppSetting.SERVER_NAME + AppSetting.SBSTmpAPI + "/assets/images/logo-big.png";
         return url;
      }

      public string Quick_GenerateLogoLink(Uri _URL)
      {
         var route = new RouteValueDictionary();

         //var url = UrlUtil.GetDomain(_URL) + AppSetting.SERVER_NAME + AppSetting.SBSTmpAPI + "/assets/images/logo-big.png";
         var url = AppSetting.SERVER_NAME + AppSetting.SBSTmpAPI + "/assets/images/logo-big.png";
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
         Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-Start validatePageRight");
         foreach (var url in URLs)
         {
            Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-" + url);
         }
         var userService = new UserService();
         var userlogin = getUser();

         var right = userService.getPageRights(userlogin.User_Authentication_ID.Value, URLs, userlogin.Company_ID.Value, userlogin.Profile_ID);
         Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-End validatePageRight");
         return right;
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
            if (filterContext.RouteData.Values["action"] != null & filterContext.RouteData.Values["controller"] != null)
               Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-ActionExecuting " + filterContext.RouteData.Values["controller"] + "Controller " + "Action-" + filterContext.RouteData.Values["action"]);

            var session = filterContext.HttpContext.Session;
            session["Expiration"] = DateTime.Now.AddMinutes(5);

            var userSession = session["User"] as User_Profile;
            if (userSession == null)
            {
               userSession = UserSession.getUser(filterContext.HttpContext);
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
            Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-End OnActionExecuting");
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

      public string RenderPartialViewAsString(string viewName, object model)
      {
         StringWriter stringWriter = new StringWriter();

         ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
         ViewContext viewContext = new ViewContext(
                 ControllerContext,
                 viewResult.View,
                 new ViewDataDictionary(model),
                 new TempDataDictionary(),
                 stringWriter
                 );

         viewResult.View.Render(viewContext, stringWriter);
         return stringWriter.ToString();
      }

      public ActionResult ChangeLanguage(string lang, string controName, string actionName)
      {
         new SBSResourceAPI.SBSResourceAPI.SiteLanguages().SetLanguage(lang);

         //if ((!string.IsNullOrEmpty(controName)) && (!string.IsNullOrEmpty(actionName)))
         //{
         //    return RedirectToAction(actionName, controName);
         //}
         Session["MainMenu"] = null;
         return RedirectToAction("Index", "Home");
      }

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
      #endregion

      #region Expense
      public ActionResult ApplicationNew(ExpensesViewModel model, string pStatus)
      {
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var currentdate = StoredProcedure.GetCurrentDate();
         var hService = new EmploymentHistoryService();
         var eService = new ExpenseService();
         var cpService = new CompanyService();
         var empService = new EmployeeService();
         var uService = new UserService();
         var aService = new SBSWorkFlowAPI.Service();
         var cbService = new ComboService();

         var emp = userlogin.Employee_Profile.FirstOrDefault();
         if (emp == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Employee);

         var hist = hService.GetCurrentEmploymentHistory(emp.Employee_Profile_ID);
         if (hist == null)
            return errorPage(ERROR_CODE.ERROR_14_NO_EMPLOYEE_HIST);

         var com = cpService.GetCompany(userlogin.Company_ID);
         if (com == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Company);

         //{5-Jul-2016} - Moet : modified this part to enhance that the supervisor should able to apply the leave onbehalf        
         var _EmpProfileID = 0;
         var _ProfileID = userlogin.Profile_ID;
         //{5-Jul-2016} - Moet : This is to take the employee profile id of the person who wants to take leave
         if (model.OnBehalf_Profile_ID.HasValue && model.OnBehalf_Profile_ID.Value > 0)
         {
            _ProfileID = model.OnBehalf_Profile_ID.Value;
            var empProf = empService.GetEmployeeProfileByProfileID(model.OnBehalf_Profile_ID);
            if (empProf != null)
            {
               model.OnBehalf_Employee_Profile_ID = empProf.Employee_Profile_ID;
               _EmpProfileID = empProf.Employee_Profile_ID;

               hist = hService.GetCurrentEmploymentHistory(empProf.Employee_Profile_ID);
               if (hist == null)
                  return errorPage(ERROR_CODE.ERROR_14_NO_EMPLOYEE_HIST);
            }
         }
         else
         {
            model.OnBehalf_Employee_Profile_ID = 0;
            model.OnBehalf_Profile_ID = 0;
            var empProf = empService.GetEmployeeProfileByProfileID(userlogin.Profile_ID);
            if (empProf != null)
            {
               model.Employee_Profile_ID = empProf.Employee_Profile_ID;
               _EmpProfileID = model.Employee_Profile_ID;
            }
         }

         if (pStatus != WorkflowStatus.Cancelled)
         {
            /* create new expenses by employee*/
            model.PageStatus = null;
            if (model.Detail_Rows == null || model.Detail_Rows.Length == 0 || model.Detail_Rows.Where(w => w.Row_Type != RowType.DELETE).Count() == 0)
               ModelState.AddModelError("Detail_Rows", Resource.The + " " + Resource.Expenses_Details + " " + Resource.Is_Rrequired_Lower);
            if (ModelState.IsValid)
            {
               var edoc = new Expenses_Application();
               if (model.operation == UserSession.RIGHT_U)
               {
                  edoc = eService.getExpenseApplication(model.Expenses_ID);
                  if (edoc == null)
                     return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Expenses);
               }
               edoc.Expenses_Application_ID = model.Expenses_ID.HasValue ? model.Expenses_ID.Value : 0;
               edoc.Employee_Profile_ID = _EmpProfileID;
               edoc.Expenses_No = model.Expenses_No;
               edoc.Date_Applied = DateUtil.ToDate(model.Date_Applied);
               edoc.Expenses_Title = model.Expenses_Title;
               edoc.Update_By = userlogin.User_Authentication.Email_Address;
               edoc.Update_On = currentdate;
               edoc.Supervisor = hist.Supervisor;
               edoc.Company_ID = userlogin.Company_ID;
               //******** Start Workflow Draft  ********//
               if (pStatus == WorkflowStatus.Draft)
                  edoc.Overall_Status = WorkflowStatus.Draft;
               else
                  edoc.Overall_Status = WorkflowStatus.Pending;
               //******** End Workflow Draft  ********//

               var haveApprover = true;
               var rworkflow = aService.GetWorkflowByEmployee(userlogin.Company_ID.Value, _ProfileID, ModuleCode.HR, ApprovalType.Expense, hist.Department_ID);
               if (!rworkflow.Item2.IsSuccess || rworkflow.Item1 == null || rworkflow.Item1.Count == 0)
                  haveApprover = false;

               if (model.operation == UserSession.RIGHT_C)
               {
                  #region Create
                  var ex_app_doc = new List<Expenses_Detail>();
                  if (model.Detail_Rows != null)
                  {
                     foreach (var row in model.Detail_Rows.Where(w => w.Row_Type != RowType.DELETE).OrderBy(o => o.Index).ToList())
                     {
                        ex_app_doc.Add(new Expenses_Detail()
                        {
                           Expenses_Application_Document_ID = row.Expenses_Application_Document_ID,
                           Amount_Claiming = row.Amount_Claiming,
                           Balance = row.Balance,
                           Date_Applied = row.Date_Applied,
                           Department_ID = row.Department_ID,
                           Expenses_Config_ID = row.Expenses_Config_ID,
                           Expenses_Date = row.Expenses_Date,
                           Doc_No = row.Doc_No,
                           Expenses_Type_Desc = row.Expenses_Type_Desc,
                           Expenses_Type_Name = row.Expenses_Type_Name,
                           Notes = row.Notes,
                           Row_Type = row.Row_Type,
                           Selected_Currency = row.Selected_Currency,
                           Tax = row.Tax,
                           Mileage = row.Mileage,
                           Total_Amount = row.Total_Amount,
                           Upload_Receipt_ID = row.Upload_Receipt_ID,
                           Upload_Receipt = row.Upload_Receipt,
                           Upload_Receipt_Name = row.Upload_Receipt_Name,
                           //********  Smart Dev  ********//
                           Job_Cost_ID = ((row.Job_Cost_ID.HasValue && row.Job_Cost_ID.Value != 0) ? row.Job_Cost_ID : null),
                           Withholding_Tax = row.Withholding_Tax,
                           Tax_Type = row.Tax_Type,
                           Withholding_Tax_Amount = row.Withholding_Tax_Amount,
                           Tax_Amount = row.Tax_Amount,
                           Tax_Amount_Type = row.Tax_Amount_Type,
                           Withholding_Tax_Type = row.Withholding_Tax_Type,
                        });
                     }
                  }

                  edoc.Create_By = userlogin.User_Authentication.Email_Address;
                  edoc.Create_On = currentdate;
                  model.result = eService.insertExpenseApplication(edoc, ex_app_doc);
                  if (model.result.Code == ERROR_CODE.SUCCESS)
                  {
                     if (pStatus != WorkflowStatus.Draft)
                     {
                        var ex = eService.getExpenseApplication(edoc.Expenses_Application_ID);
                        if (ex != null)
                        {
                           User_Profile _user = userlogin;
                           if (userlogin.Profile_ID != _ProfileID)
                           {
                              var OnBehalfUser = uService.getUser(_ProfileID, false);
                              if (OnBehalfUser != null)
                                 _user = OnBehalfUser;
                           }
                           ex.Update_By = userlogin.User_Authentication.Email_Address;
                           ex.Update_On = currentdate;
                           if (haveApprover)
                           {
                              #region Workflow
                              var request = new RequestItem();
                              request.Doc_ID = edoc.Expenses_Application_ID;
                              request.Approval_Type = ApprovalType.Expense;
                              request.Company_ID = userlogin.Company_ID.Value;
                              request.Department_ID = hist.Department_ID.HasValue ? hist.Department_ID.Value : 0;
                              request.Module = ModuleCode.HR;
                              request.Requestor_Email = _user.User_Authentication.Email_Address;
                              request.Requestor_Name = UserSession.GetUserName(_user);
                              request.Requestor_Profile_ID = _user.Profile_ID;

                              if (com.Is_Indent.HasValue && com.Is_Indent.Value)
                              {
                                 request.IndentItems = getIndentSupervisor(edoc.Expenses_Application_ID);
                                 if (request.IndentItems != null && request.IndentItems.Count > 0)
                                    request.Is_Indent = true;
                              }

                              var r = aService.SubmitRequest(request);
                              if (r.IsSuccess)
                              {
                                 ex.Supervisor = null;
                                 ex.Request_ID = request.Request_ID;
                                 ex.Overall_Status = request.Status;

                                 var mstr = "";
                                 ex.Next_Approver = null;
                                 if (request.Is_Indent)
                                 {
                                    if (request.IndentItems != null)
                                    {
                                       foreach (var row in request.IndentItems)
                                       {
                                          if (!row.IsSuccess)
                                             mstr = getApprovalStrIDs(mstr, row.Requestor_Profile_ID.ToString());
                                          else
                                             ex.Approver = getApprovalStrIDs(ex.Approver, userlogin.Profile_ID.ToString());
                                       }
                                    }
                                 }
                                 else
                                 {
                                    if (request.NextApprover != null)
                                    {
                                       if (request.NextApprover.Profile_ID == userlogin.Profile_ID && request.Status == WorkflowStatus.Closed)
                                       {
                                          ex.Approver = getApprovalStrIDs(ex.Approver, userlogin.Profile_ID.ToString());
                                       }
                                       else
                                          mstr = getApprovalStrIDs(null, request.NextApprover.Profile_ID.ToString());
                                    }
                                 }
                                 if (!string.IsNullOrEmpty(mstr))
                                    ex.Next_Approver = mstr;

                                 //if (request.Status == WorkflowStatus.Closed)
                                 //ex.Approver = getApprovalStrIDs(ex.Approver, userlogin.Profile_ID.ToString());

                                 model.result = eService.updateExpenseApplication(ex);
                                 if (model.result.Code == ERROR_CODE.SUCCESS)
                                 {
                                    if (request.Status == WorkflowStatus.Closed)
                                       sendProceedEmail(ex, com, _user, _user, hist, request.Status, request.Reviewers,
                                           model.OnBehalf_Employee_Profile_ID.Value, model.OnBehalf_Profile_ID.Value, UserSession.GetUserName(userlogin));
                                    else
                                    {
                                       var approverName = string.Empty;
                                       if (request.Is_Indent)
                                       {
                                          #region Indent flow
                                          var ai = 0;
                                          foreach (var row in request.IndentItems)
                                          {
                                             if (!row.IsSuccess)
                                             {
                                                var param = new Dictionary<string, object>();
                                                param.Add("expID", ex.Expenses_Application_ID);
                                                param.Add("appID", row.Requestor_Profile_ID);
                                                param.Add("empID", ex.Employee_Profile_ID);
                                                param.Add("reqID", ex.Request_ID);
                                                param.Add("status", WorkflowStatus.Approved);
                                                param.Add("code", uService.GenActivateCode("E" + ex.Expenses_Application_ID + row.Requestor_Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

                                                var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                                                param["status"] = WorkflowStatus.Rejected;
                                                var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                                                var appr = uService.getUser(row.Requestor_Profile_ID, false);
                                                if (appr != null)
                                                {
                                                   if (ai != 0)
                                                      approverName += " , ";

                                                   approverName += UserSession.GetUserName(appr);
                                                   sendRequestEmail(ex, com, appr, _user, hist, request.Status, request.Reviewers, linkApp, linkRej);
                                                }
                                             }
                                             else
                                             {
                                                var appr = uService.getUser(row.Requestor_Profile_ID, false);
                                                if (appr != null)
                                                {
                                                   if (ai != 0)
                                                      approverName += " , ";

                                                   approverName += UserSession.GetUserName(appr);
                                                }
                                             }
                                             ai++;
                                          }

                                          sendProceedEmail(ex, null, _user, _user, hist, WorkflowStatus.Submitted, request.Reviewers,
                                                           model.OnBehalf_Employee_Profile_ID.Value, model.OnBehalf_Profile_ID.Value, approverName);

                                          #endregion
                                       }
                                       else
                                       {
                                          #region Normal flow
                                          var param = new Dictionary<string, object>();
                                          param.Add("expID", ex.Expenses_Application_ID);
                                          param.Add("appID", request.NextApprover.Profile_ID);
                                          param.Add("empID", ex.Employee_Profile_ID);
                                          param.Add("reqID", ex.Request_ID);
                                          param.Add("status", WorkflowStatus.Approved);
                                          param.Add("code", uService.GenActivateCode("E" + ex.Expenses_Application_ID + request.NextApprover.Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

                                          var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                                          param["status"] = WorkflowStatus.Rejected;
                                          var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);


                                          var appr = uService.getUser(request.NextApprover.Profile_ID, false);
                                          if (appr != null)
                                          {
                                             approverName = UserSession.GetUserName(appr);
                                             sendRequestEmail(ex, com, appr, _user, hist, request.Status, request.Reviewers, linkApp, linkRej);
                                          }
                                          sendProceedEmail(ex, null, _user, _user, hist, WorkflowStatus.Submitted, request.Reviewers, model.OnBehalf_Employee_Profile_ID.Value, model.OnBehalf_Profile_ID.Value, approverName);

                                          #endregion
                                       }
                                    }
                                 }
                                 if (pStatus == "Quick")
                                    return View(model);
                                 else
                                    return RedirectToAction("Record", new { Code = model.result.Code, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS), Field = model.result.Field });
                              }
                              #endregion
                           }
                           else
                           {
                              if (hist.Supervisor.HasValue)
                              {
                                 #region Supervisor
                                 var sup = empService.GetEmployeeProfile2(hist.Supervisor);
                                 if (sup != null)
                                 {
                                    ex.Approver = null;
                                    ex.Next_Approver = getApprovalStrIDs(null, sup.Profile_ID.ToString());
                                    eService.updateExpenseApplication(ex);

                                    var param = new Dictionary<string, object>();
                                    param.Add("expID", ex.Expenses_Application_ID);
                                    param.Add("appID", sup.Profile_ID);
                                    param.Add("empID", ex.Employee_Profile_ID);
                                    param.Add("status", WorkflowStatus.Approved);
                                    param.Add("code", uService.GenActivateCode("E" + ex.Expenses_Application_ID + sup.Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

                                    var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                                    param["status"] = WorkflowStatus.Rejected;
                                    var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                                    //sendProceedEmail(ex, com, userlogin, userlogin, hist, WorkflowStatus.Submitted, null, SmartDevPdfProceed);
                                    sendProceedEmail(ex, com, _user, _user, hist, WorkflowStatus.Submitted, null,
                                               model.OnBehalf_Employee_Profile_ID.Value, model.OnBehalf_Profile_ID.Value, UserSession.GetUserName(sup.User_Profile));

                                    sendRequestEmail(ex, com, sup.User_Profile, _user, hist, ex.Overall_Status, null, linkApp, linkRej);

                                    if (pStatus == "Quick")
                                       return View(model);
                                    else
                                       return RedirectToAction("Record", new { Code = model.result.Code, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS), Field = model.result.Field });
                                 }
                                 #endregion
                              }
                              else
                              {
                                 #region not flow
                                 ex.Overall_Status = WorkflowStatus.Closed;
                                 model.result = eService.updateExpenseApplication(ex);
                                 if (model.result.Code == ERROR_CODE.SUCCESS)
                                 {
                                    //sendProceedEmail(ex, com, userlogin, userlogin, hist, ex.Overall_Status, null, SmartDevPdfProceed);
                                    sendProceedEmail(ex, com, _user, _user, hist, ex.Overall_Status, null,
                                              model.OnBehalf_Employee_Profile_ID.Value, model.OnBehalf_Profile_ID.Value);
                                 }
                                 if (pStatus == "Quick")
                                    return View(model);
                                 else
                                    return RedirectToAction("Record", new { Code = model.result.Code, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS), Field = model.result.Field });
                                 #endregion
                              }
                           }
                        }
                     }
                     else if (pStatus == WorkflowStatus.Draft)
                     {
                        //******** Start Workflow Draft  ********//
                        return RedirectToAction("Record", new { Code = model.result.Code, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = model.result.Field });
                        //******** End Workflow Draft  ********//
                     }
                  }
                  else
                  {
                     //-------rights------------
                     var rightResult2 = validatePageRight(UserSession.RIGHT_A);
                     if (rightResult2.action != null)
                        return rightResult2.action;
                     model.rights = rightResult2.rights;


                     var yearSerivce = empService.GetYearService(hist.Employee_Profile_ID);

                     model.currencyList = cbService.LstCurrencyByCompany(userlogin.Company_ID.Value, currentdate.Year.ToString());
                     model.expensesConfigList = eService.getExpenseTypes(userlogin.Company_ID.Value, hist.Department_ID, hist.Designation_ID, null, yearSerivce);
                     model.EmployeeUnderMeList = cbService.LstEmpUnderMe(userlogin.Profile_ID);

                     //********  Smart Dev  ********//
                     model.JobCostlst = cbService.LstJobCost(userlogin.Company_ID, true);
                     model.TaxTypelst = cbService.LstTaxType(false);
                     model.AmountTypelst = cbService.LstAmountType();

                     return View(model);
                  }
                  #endregion
               }
               else if (model.operation == UserSession.RIGHT_U)
               {
                  #region Update
                  //******** Start Workflow Draft  ********//
                  edoc.Expenses_Application_Document.Clear();
                  var ex_app_docs = new List<Expenses_Application_Document>();
                  if (model.Detail_Rows != null)
                  {
                     foreach (var row in model.Detail_Rows.Where(w => w.Row_Type != RowType.DELETE).OrderBy(o => o.Index).ToList())
                     {
                        if (row.Row_Type == RowType.ADD || row.Row_Type == RowType.EDIT)
                        {
                           var ex_app_doc = new Expenses_Application_Document();
                           var upload_receipt = new Upload_Receipt();
                           if (row.Row_Type == RowType.ADD)
                           {
                              ex_app_doc.Create_By = edoc.Create_By;
                              ex_app_doc.Create_On = currentdate;
                           }
                           ex_app_doc.Expenses_Application_Document_ID = row.Expenses_Application_Document_ID.HasValue ? row.Expenses_Application_Document_ID.Value : 0;
                           ex_app_doc.Expenses_Application_ID = edoc.Expenses_Application_ID;
                           ex_app_doc.Amount_Claiming = row.Amount_Claiming;
                           ex_app_doc.Employee_Profile_ID = edoc.Employee_Profile_ID;
                           ex_app_doc.Expenses_Config_ID = row.Expenses_Config_ID;
                           ex_app_doc.Expenses_Date = DateUtil.ToDate(row.Expenses_Date);
                           ex_app_doc.Doc_No = row.Doc_No;
                           ex_app_doc.Reasons = row.Notes;
                           ex_app_doc.Selected_Currency = row.Selected_Currency;
                           ex_app_doc.Tax = row.Tax;
                           ex_app_doc.Total_Amount = row.Total_Amount;
                           ex_app_doc.Department_ID = hist.Department_ID;
                           ex_app_doc.Date_Applied = edoc.Date_Applied;
                           ex_app_doc.Mileage = row.Mileage;
                           ex_app_doc.Update_By = edoc.Create_By;
                           ex_app_doc.Update_On = currentdate;

                           //********  Smart Dev  ********//
                           ex_app_doc.Job_Cost_ID = ((row.Job_Cost_ID.HasValue && row.Job_Cost_ID.Value != 0) ? row.Job_Cost_ID : null);
                           ex_app_doc.Withholding_Tax = row.Withholding_Tax;
                           ex_app_doc.Tax_Type = row.Tax_Type;
                           ex_app_doc.Tax_Amount_Type = row.Tax_Amount_Type;
                           ex_app_doc.Tax_Amount = row.Tax_Amount;
                           ex_app_doc.Withholding_Tax_Type = row.Withholding_Tax_Type;
                           ex_app_doc.Withholding_Tax_Amount = row.Withholding_Tax_Amount;

                           if (!string.IsNullOrEmpty(row.Upload_Receipt))
                           {
                              string trimmedData = null;
                              if (row.Upload_Receipt_ID == null)
                              {
                                 trimmedData = row.Upload_Receipt;
                                 var prefixindex = trimmedData.IndexOf(",");
                                 trimmedData = trimmedData.Substring(prefixindex + 1, trimmedData.Length - (prefixindex + 1));
                              }
                              else
                              {
                                 trimmedData = row.Upload_Receipt;
                                 if (trimmedData.Contains("data:"))
                                 {
                                    var prefixindex = trimmedData.IndexOf(",");
                                    trimmedData = trimmedData.Substring(prefixindex + 1, trimmedData.Length - (prefixindex + 1));
                                 }
                              }
                              var filebyte = Convert.FromBase64String(trimmedData);
                              if (filebyte != null)
                              {
                                 upload_receipt.Expenses_Application_Document_ID = row.Expenses_Application_Document_ID.HasValue ? row.Expenses_Application_Document_ID.Value : 0;
                                 upload_receipt.Create_By = ex_app_doc.Create_By;
                                 upload_receipt.Create_On = currentdate;
                                 upload_receipt.File_Name = row.Upload_Receipt_Name;
                                 upload_receipt.Receipt = Convert.FromBase64String(trimmedData);
                                 upload_receipt.Update_By = ex_app_doc.Update_By;
                                 upload_receipt.Update_On = currentdate;
                                 ex_app_doc.Upload_Receipt.Add(upload_receipt);
                              }
                           }
                           ex_app_docs.Add(ex_app_doc);
                        }
                     }
                  }
                  edoc.Expenses_Application_Document = ex_app_docs;
                  model.result = eService.updateExpenseApplicationDoc(edoc);
                  if (model.result.Code == ERROR_CODE.SUCCESS)
                  {
                     if (pStatus != WorkflowStatus.Draft)
                     {
                        var ex = eService.getExpenseApplication(edoc.Expenses_Application_ID);
                        if (ex != null)
                        {
                           ex.Update_By = userlogin.User_Authentication.Email_Address;
                           ex.Update_On = currentdate;
                           if (haveApprover)
                           {
                              #region WorkFlow
                              var request = new RequestItem();
                              request.Doc_ID = edoc.Expenses_Application_ID;
                              request.Approval_Type = ApprovalType.Expense;
                              request.Company_ID = userlogin.Company_ID.Value;
                              request.Department_ID = hist.Department_ID.HasValue ? hist.Department_ID.Value : 0;
                              request.Module = ModuleCode.HR;
                              request.Requestor_Email = userlogin.User_Authentication.Email_Address;
                              request.Requestor_Name = UserSession.GetUserName(userlogin);
                              //request.Requestor_Profile_ID = userlogin.Profile_ID;
                              request.Requestor_Profile_ID = _ProfileID;

                              if (com.Is_Indent.HasValue && com.Is_Indent.Value)
                              {
                                 request.IndentItems = getIndentSupervisor(edoc.Expenses_Application_ID);
                                 if (request.IndentItems != null && request.IndentItems.Count > 0)
                                    request.Is_Indent = true;
                              }

                              var r = aService.SubmitRequest(request);
                              if (r.IsSuccess)
                              {
                                 ex.Supervisor = null;
                                 ex.Request_ID = request.Request_ID;
                                 ex.Overall_Status = request.Status;

                                 var mstr = "";
                                 ex.Next_Approver = null;
                                 ex.Approver = null;
                                 if (request.Is_Indent)
                                 {
                                    if (request.IndentItems != null)
                                    {
                                       foreach (var row in request.IndentItems)
                                       {
                                          if (!row.IsSuccess)
                                             mstr = getApprovalStrIDs(mstr, row.Requestor_Profile_ID.ToString());
                                          else
                                             ex.Approver = getApprovalStrIDs(ex.Approver, userlogin.Profile_ID.ToString());
                                       }
                                    }
                                 }
                                 else
                                 {
                                    if (request.NextApprover != null)
                                    {
                                       if (request.NextApprover.Profile_ID == userlogin.Profile_ID && request.Status == WorkflowStatus.Closed)
                                       {
                                          ex.Approver = getApprovalStrIDs(ex.Approver, userlogin.Profile_ID.ToString());
                                       }
                                       else
                                          mstr = getApprovalStrIDs(null, request.NextApprover.Profile_ID.ToString());
                                    }

                                 }
                                 if (!string.IsNullOrEmpty(mstr))
                                    ex.Next_Approver = mstr;

                                 //if (request.Status == WorkflowStatus.Closed)
                                 //ex.Approver = getApprovalStrIDs(ex.Approver, userlogin.Profile_ID.ToString());

                                 model.result = eService.updateExpenseApplication(ex);
                                 if (model.result.Code == ERROR_CODE.SUCCESS)
                                 {
                                    if (request.Status == WorkflowStatus.Closed)
                                       sendProceedEmail(ex, com, userlogin, userlogin, hist, request.Status, request.Reviewers,
                                           model.OnBehalf_Employee_Profile_ID.Value, model.OnBehalf_Profile_ID.Value, UserSession.GetUserName(userlogin));
                                    else
                                    {
                                       var approverName = string.Empty;
                                       if (request.Is_Indent)
                                       {
                                          #region Indent flow
                                          var ai = 0;
                                          foreach (var row in request.IndentItems)
                                          {
                                             if (!row.IsSuccess)
                                             {
                                                var param = new Dictionary<string, object>();
                                                param.Add("expID", ex.Expenses_Application_ID);
                                                param.Add("appID", row.Requestor_Profile_ID);
                                                param.Add("empID", ex.Employee_Profile_ID);
                                                param.Add("reqID", ex.Request_ID);
                                                param.Add("status", WorkflowStatus.Approved);
                                                param.Add("code", uService.GenActivateCode("E" + ex.Expenses_Application_ID + row.Requestor_Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

                                                var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                                                param["status"] = WorkflowStatus.Rejected;
                                                var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                                                var appr = uService.getUser(row.Requestor_Profile_ID, false);
                                                if (appr != null)
                                                {
                                                   if (ai != 0)
                                                      approverName += " , ";

                                                   approverName += UserSession.GetUserName(appr);
                                                   sendRequestEmail(ex, com, appr, userlogin, hist, request.Status, request.Reviewers, linkApp, linkRej);
                                                }
                                             }
                                             else
                                             {
                                                var appr = uService.getUser(row.Requestor_Profile_ID, false);
                                                if (appr != null)
                                                {
                                                   if (ai != 0)
                                                      approverName += " , ";

                                                   approverName += UserSession.GetUserName(appr);
                                                }
                                             }
                                             ai++;
                                          }

                                          sendProceedEmail(ex, null, userlogin, userlogin, hist, WorkflowStatus.Submitted, request.Reviewers,
                                                            model.OnBehalf_Employee_Profile_ID.Value, model.OnBehalf_Profile_ID.Value, approverName);
                                          #endregion
                                       }
                                       else
                                       {
                                          #region Normal
                                          var param = new Dictionary<string, object>();
                                          param.Add("expID", ex.Expenses_Application_ID);
                                          param.Add("appID", request.NextApprover.Profile_ID);
                                          param.Add("empID", ex.Employee_Profile_ID);
                                          param.Add("reqID", ex.Request_ID);
                                          param.Add("status", WorkflowStatus.Approved);
                                          param.Add("code", uService.GenActivateCode("E" + ex.Expenses_Application_ID + request.NextApprover.Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

                                          var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                                          param["status"] = WorkflowStatus.Rejected;
                                          var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                                          var appr = uService.getUser(request.NextApprover.Profile_ID, false);
                                          if (appr != null)
                                          {
                                             approverName = UserSession.GetUserName(appr);
                                             sendRequestEmail(ex, com, appr, userlogin, hist, request.Status, request.Reviewers, linkApp, linkRej);
                                          }

                                          sendProceedEmail(ex, null, userlogin, userlogin, hist, WorkflowStatus.Submitted, request.Reviewers,
                                                   model.OnBehalf_Employee_Profile_ID.Value, model.OnBehalf_Profile_ID.Value, approverName);

                                          #endregion
                                       }
                                    }
                                 }
                              }
                              #endregion
                           }
                           else
                           {
                              if (hist.Supervisor.HasValue)
                              {
                                 #region Supervisor
                                 var sup = empService.GetEmployeeProfile2(hist.Supervisor);
                                 if (sup != null)
                                 {
                                    ex.Approver = null;
                                    ex.Next_Approver = getApprovalStrIDs(null, sup.Profile_ID.ToString());
                                    eService.updateExpenseApplication(ex);

                                    var param = new Dictionary<string, object>();
                                    param.Add("expID", ex.Expenses_Application_ID);
                                    param.Add("appID", sup.Profile_ID);
                                    param.Add("empID", ex.Employee_Profile_ID);
                                    param.Add("status", WorkflowStatus.Approved);
                                    param.Add("code", uService.GenActivateCode("E" + ex.Expenses_Application_ID + sup.Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

                                    var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                                    param["status"] = WorkflowStatus.Rejected;
                                    var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                                    sendProceedEmail(ex, com, userlogin, userlogin, hist, WorkflowStatus.Submitted, null,
                                               model.OnBehalf_Employee_Profile_ID.Value, model.OnBehalf_Profile_ID.Value, UserSession.GetUserName(sup.User_Profile));

                                    sendRequestEmail(ex, com, sup.User_Profile, userlogin, hist, ex.Overall_Status, null, linkApp, linkRej);
                                 }
                                 #endregion
                              }
                              else
                              {
                                 #region Not workFlow
                                 ex.Overall_Status = WorkflowStatus.Closed;
                                 model.result = eService.updateExpenseApplication(ex);
                                 if (model.result.Code == ERROR_CODE.SUCCESS)
                                 {
                                    //sendProceedEmail(ex, com, userlogin, userlogin, hist, ex.Overall_Status, null, SmartDevPdfProceed);
                                    sendProceedEmail(ex, com, userlogin, userlogin, hist, ex.Overall_Status, null,
                                              model.OnBehalf_Employee_Profile_ID.Value, model.OnBehalf_Profile_ID.Value);
                                 }
                                 #endregion
                              }
                           }
                        }
                        return RedirectToAction("Record", new { Code = model.result.Code, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS), Field = model.result.Field });
                     }
                     else if (pStatus == WorkflowStatus.Draft)
                     {
                        //******** Start Workflow Draft  ********//
                        return RedirectToAction("Record", new { Code = model.result.Code, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = model.result.Field });
                        //******** End Workflow Draft  ********//
                     }
                  }
                  else
                  {
                     //-------rights------------
                     var rightResult2 = validatePageRight(UserSession.RIGHT_A);
                     if (rightResult2.action != null)
                        return rightResult2.action;
                     model.rights = rightResult2.rights;

                     var yearSerivce = empService.GetYearService(hist.Employee_Profile_ID);

                     model.currencyList = cbService.LstCurrencyByCompany(userlogin.Company_ID.Value, currentdate.Year.ToString());
                     model.expensesConfigList = eService.getExpenseTypes(userlogin.Company_ID.Value, hist.Department_ID, hist.Designation_ID, null, yearSerivce);
                     model.EmployeeUnderMeList = cbService.LstEmpUnderMe(userlogin.Profile_ID);

                     //********  Smart Dev  ********//
                     model.JobCostlst = cbService.LstJobCost(userlogin.Company_ID, true);
                     model.TaxTypelst = cbService.LstTaxType(false);
                     model.AmountTypelst = cbService.LstAmountType();

                     return View(model);
                  }
                  //******** End Workflow Draft  ********//
                  #endregion
               }
            }
         }
         else if (pStatus == WorkflowStatus.Cancelled)
         {
            var ex = eService.getExpenseApplication(model.Expenses_ID);
            if (ex == null)
               return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Expenses);

            ex.Update_By = userlogin.User_Authentication.Email_Address;
            ex.Update_On = currentdate;

            /* cancel leave by employee*/
            if (!model.Request_ID.HasValue)
            {
               /*not have workflow */
               if (model.Supervisor.HasValue)
               {
                  #region Supervisor flow
                  /* have supervisor*/
                  var sup = empService.GetEmployeeProfile2(hist.Supervisor);
                  if (sup != null)
                  {
                     if (ex.Overall_Status == WorkflowStatus.Closed)
                     {
                        ex.Approver = null;
                        ex.Next_Approver = getApprovalStrIDs(null, sup.Profile_ID.ToString());
                        ex.Cancel_Status = WorkflowStatus.Canceling;
                        model.result = eService.updateExpenseApplication(ex);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                           var param = new Dictionary<string, object>();
                           param.Add("expID", ex.Expenses_Application_ID);
                           param.Add("appID", sup.Profile_ID);
                           param.Add("empID", ex.Employee_Profile_ID);
                           param.Add("cancelStatus", WorkflowStatus.Cancelled);
                           param.Add("code", uService.GenActivateCode("E" + ex.Expenses_Application_ID + sup.Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));
                           var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                           param["cancelStatus"] = WorkflowStatus.Rejected;
                           var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                           sendProceedEmail(ex, com, userlogin, userlogin, hist, ex.Cancel_Status, null,
                                            model.OnBehalf_Employee_Profile_ID.Value, model.OnBehalf_Profile_ID.Value, UserSession.GetUserName(sup.User_Profile));

                           sendRequestEmail(ex, com, sup.User_Profile, userlogin, hist, ex.Cancel_Status, null, linkApp, linkRej);
                           return RedirectToAction("Record", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL), Field = Resource.Expenses });
                        }
                     }
                     else
                     {
                        ex.Approver = getApprovalStrIDs(null, userlogin.Profile_ID.ToString());
                        ex.Next_Approver = null;
                        ex.Cancel_Status = WorkflowStatus.Cancelled;
                        model.result = eService.updateExpenseApplication(ex);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                           //sendProceedEmail(ex, com, userlogin, userlogin, hist, ex.Cancel_Status, null);
                           sendProceedEmail(ex, com, userlogin, userlogin, hist, ex.Cancel_Status, null,
                                            model.OnBehalf_Employee_Profile_ID.Value, model.OnBehalf_Profile_ID.Value, UserSession.GetUserName(sup.User_Profile));
                           /*should send some email to appr*/
                           return RedirectToAction("Record", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL), Field = Resource.Expenses });
                        }
                     }
                  }
                  #endregion
               }
               else
               {
                  #region Not have workflow
                  ex.Cancel_Status = WorkflowStatus.Cancelled;
                  model.result = eService.updateExpenseApplication(ex);
                  if (model.result.Code == ERROR_CODE.SUCCESS)
                  {
                     //sendProceedEmail(ex, com, userlogin, userlogin, hist, ex.Cancel_Status, null);
                     sendProceedEmail(ex, com, userlogin, userlogin, hist, ex.Cancel_Status, null,
                                           model.OnBehalf_Employee_Profile_ID.Value, model.OnBehalf_Profile_ID.Value);
                     return RedirectToAction("Record", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL), Field = Resource.Expenses });
                  }
                  #endregion
               }
            }
            else
            {
               #region workflow
               if (ex.Overall_Status == WorkflowStatus.Closed)
               {
                  var rqcancel = new RequestItem();
                  rqcancel.Doc_ID = model.Expenses_ID;
                  rqcancel.Approval_Type = ApprovalType.Expense;
                  rqcancel.Company_ID = userlogin.Company_ID.Value;
                  rqcancel.Department_ID = hist.Department_ID.HasValue ? hist.Department_ID.Value : 0;
                  rqcancel.Module = ModuleCode.HR;
                  rqcancel.Requestor_Email = userlogin.User_Authentication.Email_Address;
                  rqcancel.Requestor_Name = UserSession.GetUserName(userlogin);
                  rqcancel.Requestor_Profile_ID = userlogin.Profile_ID;

                  if (com.Is_Indent.HasValue && com.Is_Indent.Value)
                  {
                     rqcancel.IndentItems = getIndentSupervisor(ex.Expenses_Application_ID);
                     if (rqcancel.IndentItems != null && rqcancel.IndentItems.Count > 0)
                        rqcancel.Is_Indent = true;
                  }

                  var rc = aService.SubmitRequestCanceling(rqcancel);
                  if (rc.IsSuccess)
                  {
                     ex.Request_Cancel_ID = rqcancel.Request_ID;
                     ex.Cancel_Status = WorkflowStatus.Canceling;
                     if (rqcancel.Status == WorkflowStatus.Closed)
                        ex.Cancel_Status = WorkflowStatus.Cancelled;

                     var mstr = "";
                     ex.Next_Approver = null;
                     ex.Approver = null; // clear data 
                     if (rqcancel.Is_Indent)
                     {
                        if (rqcancel.IndentItems != null)
                        {
                           foreach (var row in rqcancel.IndentItems)
                           {
                              if (!row.IsSuccess)
                                 mstr = getApprovalStrIDs(mstr, row.Requestor_Profile_ID.ToString());
                              else
                                 ex.Approver = getApprovalStrIDs(ex.Approver, userlogin.Profile_ID.ToString());
                           }
                        }
                     }
                     else
                     {
                        if (rqcancel.NextApprover != null)
                        {
                           if (rqcancel.NextApprover.Profile_ID == userlogin.Profile_ID && rqcancel.Status == WorkflowStatus.Closed)
                           {
                              ex.Approver = getApprovalStrIDs(ex.Approver, userlogin.Profile_ID.ToString());
                           }
                           else
                              mstr = getApprovalStrIDs(null, rqcancel.NextApprover.Profile_ID.ToString());
                        }
                     }
                     ex.Next_Approver = mstr;

                     //if (rqcancel.Status == WorkflowStatus.Closed)
                     //ex.Approver = getApprovalStrIDs(ex.Approver, userlogin.Profile_ID.ToString());

                     model.result = eService.updateExpenseApplication(ex);
                     if (model.result.Code == ERROR_CODE.SUCCESS)
                     {
                        if (rqcancel.Status == WorkflowStatus.Closed)
                        {
                           sendProceedEmail(ex, com, userlogin, userlogin, hist, ex.Cancel_Status, null,
                                           model.OnBehalf_Employee_Profile_ID.Value, model.OnBehalf_Profile_ID.Value, UserSession.GetUserName(userlogin));
                           return RedirectToAction("Record", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL), Field = Resource.Expenses });
                        }
                        else
                        {
                           var approverName = string.Empty;
                           if (rqcancel.Is_Indent)
                           {
                              #region Indent flow
                              var ai = 0;
                              foreach (var row in rqcancel.IndentItems)
                              {
                                 if (!row.IsSuccess)
                                 {
                                    var param = new Dictionary<string, object>();
                                    param.Add("expID", ex.Expenses_Application_ID);
                                    param.Add("appID", row.Requestor_Profile_ID);
                                    param.Add("empID", ex.Employee_Profile_ID);
                                    param.Add("reqcancelID", rqcancel.Request_ID);
                                    param.Add("cancelStatus", WorkflowStatus.Cancelled);
                                    param.Add("code", uService.GenActivateCode("E" + ex.Expenses_Application_ID + row.Requestor_Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));
                                    var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                                    param["cancelStatus"] = WorkflowStatus.Rejected;
                                    var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                                    var appr = uService.getUser(row.Requestor_Profile_ID, false);
                                    if (appr != null)
                                    {
                                       if (ai != 0)
                                          approverName += " , ";

                                       approverName += UserSession.GetUserName(appr);
                                       sendRequestEmail(ex, com, appr, userlogin, hist, ex.Cancel_Status, rqcancel.Reviewers, linkApp, linkRej);
                                    }
                                 }
                                 else
                                 {
                                    var appr = uService.getUser(row.Requestor_Profile_ID, false);
                                    if (appr != null)
                                    {
                                       if (ai != 0)
                                          approverName += " , ";

                                       approverName += UserSession.GetUserName(appr);
                                    }
                                 }
                                 ai++;
                              }
                              sendProceedEmail(ex, com, userlogin, userlogin, hist, ex.Cancel_Status, rqcancel.Reviewers,
                                     model.OnBehalf_Employee_Profile_ID.Value, model.OnBehalf_Profile_ID.Value, approverName);
                              #endregion
                           }
                           else
                           {
                              #region Normal flow
                              var param = new Dictionary<string, object>();
                              param.Add("expID", ex.Expenses_Application_ID);
                              param.Add("appID", rqcancel.NextApprover.Profile_ID);
                              param.Add("empID", ex.Employee_Profile_ID);
                              param.Add("reqcancelID", rqcancel.Request_ID);
                              param.Add("cancelStatus", WorkflowStatus.Cancelled);
                              param.Add("code", uService.GenActivateCode("E" + ex.Expenses_Application_ID + rqcancel.NextApprover.Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));
                              var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                              param["cancelStatus"] = WorkflowStatus.Rejected;
                              var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                              var appr = uService.getUser(rqcancel.NextApprover.Profile_ID, false);
                              if (appr != null)
                              {
                                 approverName += UserSession.GetUserName(appr);
                                 sendRequestEmail(ex, com, appr, userlogin, hist, ex.Cancel_Status, rqcancel.Reviewers, linkApp, linkRej);
                              }
                              sendProceedEmail(ex, com, userlogin, userlogin, hist, ex.Cancel_Status, rqcancel.Reviewers,
                                              model.OnBehalf_Employee_Profile_ID.Value, model.OnBehalf_Profile_ID.Value, approverName);

                              #endregion
                           }
                           return RedirectToAction("Record", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL), Field = Resource.Expenses });
                        }
                     }
                  }
               }
               else
               {
                  ex.Cancel_Status = WorkflowStatus.Cancelled;
                  model.result = eService.updateExpenseApplication(ex);
                  if (model.result.Code == ERROR_CODE.SUCCESS)
                  {
                     //sendProceedEmail(ex, com, userlogin, userlogin, hist, ex.Cancel_Status, null);
                     sendProceedEmail(ex, com, userlogin, userlogin, hist, ex.Cancel_Status, null,
                           model.OnBehalf_Employee_Profile_ID.Value, model.OnBehalf_Profile_ID.Value);
                     return RedirectToAction("Record", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL), Field = Resource.Expenses });
                  }
               }
               #endregion
            }
         }

         var err = GetErrorModelState();

         //-------rights------------
         var rightResult = validatePageRight(UserSession.RIGHT_A);
         if (rightResult.action != null)
            return rightResult.action;
         model.rights = rightResult.rights;

         var yearSerivce2 = empService.GetYearService(hist.Employee_Profile_ID);

         model.currencyList = cbService.LstCurrencyByCompany(userlogin.Company_ID.Value, currentdate.Year.ToString());
         model.expensesConfigList = eService.getExpenseTypes(userlogin.Company_ID.Value, hist.Department_ID, hist.Designation_ID, null, yearSerivce2);
         model.EmployeeUnderMeList = cbService.LstEmpUnderMe(userlogin.Profile_ID);

         //********  Smart Dev  ********//
         model.JobCostlst = cbService.LstJobCost(userlogin.Company_ID, true);
         model.TaxTypelst = cbService.LstTaxType(false);
         model.AmountTypelst = cbService.LstAmountType();

         if (model.Request_ID.HasValue)
         {
            var r = aService.GetMyRequests(userlogin.Company_ID, null, null, ModuleCode.HR, ApprovalType.Expense, model.Expenses_ID);
            if (r.Item2.IsSuccess && r.Item1 != null)
               model.Expenses_Request = r.Item1;
         }

         return View(model);
      }

      public FileAttach getFileExpenseSmartDevPdf(string eID = null, string operation = null, int? OnBehalf_Employee_Profile_ID = 0)
      {

         var currentdate = StoredProcedure.GetCurrentDate();
         var userlogin = UserSession.getUser(HttpContext);
         var empService = new EmployeeService();
         var hService = new EmploymentHistoryService();
         var exService = new ExpenseService();
         var cbService = new ComboService();
         var companyService = new CompanyService();
         var company = companyService.GetCompany(userlogin.Company_ID);

         var model = new ExpensesDocPrintViewModel();
         model.operation = EncryptUtil.Decrypt(operation);
         model.Expenses_ID = NumUtil.ParseInteger(EncryptUtil.Decrypt(eID));

         if (model.Expenses_ID.HasValue && model.Expenses_ID > 0 && model.operation == UserSession.RIGHT_U)
         {
            var com = new CompanyService().GetCompany(userlogin.Company_ID);
            if (com != null)
            {
               model.Company_Name = com.Name;
               model.Address_Row_1 = com.Address;
               model.Address_Row_2 = (com.State_ID.HasValue ? com.State.Descrition : "") + " " + com.Zip_Code + " " + (com.Currency_ID.HasValue ? com.Country.Description : "");
               model.Address_Row_3 = Resource.Tel + ": " + com.Phone + "   " + Resource.Fax + ": " + com.Fax;
            }

            var emp = empService.GetEmployeeProfile2(OnBehalf_Employee_Profile_ID);
            if (emp != null)
            {
               model.Employee_No = emp.Employee_No;
               model.Employee_Name = AppConst.GetUserName(emp.User_Profile);
            }
            var userhist = hService.GetCurrentEmploymentHistory(OnBehalf_Employee_Profile_ID);
            if (userhist != null)
            {
               if (userhist.Department != null)
                  model.Department_Name = userhist.Department.Name;
               if (userhist.Designation != null)
                  model.Designation_Name = userhist.Designation.Name;
            }

            var ex = exService.getExpenseApplication(model.Expenses_ID);
            if (ex != null && ex.Expenses_Application_Document != null)
            {
               model.Request_ID = ex.Request_ID;
               model.Request_Cancel_ID = ex.Request_Cancel_ID;
               model.Overall_Status = ex.Overall_Status;
               model.Expenses_Title = ex.Expenses_Title;
               model.Date = DateUtil.ToDisplayFullDate(ex.Date_Applied);

               if (ex.Expenses_Application_Document.Select(g => g.Expenses_Config_ID).FirstOrDefault() != null)
                  model.ExpensesDetailList = ex.Expenses_Application_Document.OrderBy(g => g.Expenses_Config.Expenses_Name).OrderBy(o => o.Job_Cost_ID).ToList();

               model.Cancel_Status = ex.Cancel_Status;
               model.Supervisor = ex.Supervisor;
               if (model.Supervisor.HasValue)
               {
                  var sup = empService.GetEmployeeProfile2(model.Supervisor);
                  if (sup != null)
                  {
                     model.Supervisor_Name = AppConst.GetUserName(sup.User_Profile);
                     var suphist = hService.GetCurrentEmploymentHistory(model.Supervisor);
                     if (suphist != null)
                     {
                        if (suphist.Designation != null)
                           model.Supervisor_Designation = suphist.Designation.Name;
                     }
                  }

               }

               if (ex.Request_ID.HasValue)
               {
                  var aService = new SBSWorkFlowAPI.Service();
                  var r = aService.GetMyRequests(userlogin.Company_ID, null, null, ModuleCode.HR, ApprovalType.Expense, ex.Expenses_Application_ID);
                  if (r.Item2.IsSuccess && r.Item1 != null)
                     model.Expenses_Request = r.Item1;
               }
            }
            model.JobCostlst = cbService.LstJobCost(userlogin.Company_ID, false);
            model.TaxTypelst = cbService.LstTaxType(false);
         }

         var htmlToConvert = RenderPartialViewAsString("ExpensesDocPrint", model);
         Response.Clear();
         Response.ClearContent();
         Response.ClearHeaders();
         Response.ContentType = "application/pdf";
         Response.Buffer = true;
         var sr = new StringReader(htmlToConvert);
         var memoryStream = new MemoryStream();
         var pdfDoc = new Document(PageSize.A4);
         var htmlparser = new HTMLWorker(pdfDoc);
         var writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
         var pageevent = new PDFPageEvent();
         var logo = companyService.GetLogo(userlogin.Company_ID);
         if (logo != null)
            pageevent.LogoLeftOnFirstPage = logo.Logo;

         if (company.Country_ID.HasValue)
         {
            var country = companyService.GetCountry(company.Country_ID);
            if (country != null)
            {
               if (country.Name != null)
               {
                  htmlparser.SetStyleSheet(GenerateStyleSheet(country.Name));
                  pageevent.CountryName = country.Name;
               }
            }
         }

         writer.PageEvent = pageevent;
         pdfDoc.Open();
         //var action = new PdfAction(PdfAction.PRINTDIALOG);
         //writer.SetOpenAction(action);
         htmlparser.Parse(sr);
         writer.CloseStream = false;
         pdfDoc.Close();
         memoryStream.Position = 0;

         var fileattach = new FileAttach();
         fileattach.File_Name = Resource.Employee_Expenses_Claim + ".pdf";
         fileattach.File = memoryStream;

         return fileattach;
      }

      public List<IndentItem> getIndentSupervisor(int? pExpensesID, int? pTimesheetID = null)
      {
         var indents = new List<IndentItem>();
         var eService = new ExpenseService();
         var tService = new TimeSheetService();
         var jobcostService = new JobCostService();
         var ProfileIDs = new List<int>();
         if (pExpensesID.HasValue)
         {
            var expense = eService.getExpenseApplication(pExpensesID);
            if (expense != null)
            {
               foreach (var row in expense.Expenses_Application_Document)
               {
                  if (row.Job_Cost_ID.HasValue)
                  {
                     var job = jobcostService.GetJobCost(row.Job_Cost_ID);
                     if (job != null)
                     {
                        if (job.Supervisor.HasValue)
                        {
                           var empService = new EmployeeService();
                           var emp = empService.GetEmployeeProfile2(job.Supervisor);
                           if (emp != null)
                           {
                              if (!ProfileIDs.Contains(emp.User_Profile.Profile_ID))
                              {
                                 var indent = new IndentItem();
                                 indent.Indent_No = job.Indent_No;
                                 indent.Requestor_Profile_ID = emp.User_Profile.Profile_ID;
                                 indent.Requestor_Name = AppConst.GetUserName(emp.User_Profile);
                                 indent.Requestor_Email = emp.User_Profile.Email;
                                 indents.Add(indent);
                                 ProfileIDs.Add(emp.User_Profile.Profile_ID);
                              }
                           }
                        }
                     }
                  }
               }
            }
         }
         if (pTimesheetID.HasValue)
         {
            var ts = tService.GetTimeSheet(pTimesheetID);
            if (ts != null)
            {
               foreach (var row in ts.Time_Sheet_Dtl)
               {
                  if (row.Job_Cost_ID.HasValue)
                  {
                     var job = jobcostService.GetJobCost(row.Job_Cost_ID);
                     if (job != null)
                     {
                        if (job.Supervisor.HasValue)
                        {
                           var empService = new EmployeeService();
                           var emp = empService.GetEmployeeProfile2(job.Supervisor);
                           if (emp != null)
                           {
                              if (!ProfileIDs.Contains(emp.User_Profile.Profile_ID))
                              {
                                 var indent = new IndentItem();
                                 indent.Indent_No = job.Indent_No;
                                 indent.Requestor_Profile_ID = emp.User_Profile.Profile_ID;
                                 indent.Requestor_Name = AppConst.GetUserName(emp.User_Profile);
                                 indent.Requestor_Email = emp.User_Profile.Email;
                                 indents.Add(indent);
                                 ProfileIDs.Add(emp.User_Profile.Profile_ID);
                              }
                           }
                        }
                     }
                  }
               }
            }

         }
         
         return indents;
      }
      #endregion

      #region Leave
      public ActionResult ApplicationNew(LeaveViewModel model, HttpPostedFileBase file, string pStatus)
      {
         var userlogin = new User_Profile();
         userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var leaveService = new LeaveService();
         var cbService = new ComboService();
         var empService = new EmployeeService();
         var uService = new UserService();
         var currentdate = StoredProcedure.GetCurrentDate();
         var aService = new SBSWorkFlowAPI.Service();

         model.EmployeeUnderMeList = cbService.LstEmpUnderMe(userlogin.Profile_ID);
         /* Action by employee*/
         var hist = new EmploymentHistoryService().GetCurrentEmploymentHistoryByProfile(userlogin.Profile_ID);
         if (hist == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Employment_History);

         //{5-Jul-2016} - Moet : modified this part to enhance that the supervisor should able to apply the leave onbehalf        
         var _EmpProfileID = 0;
         var _ProfileID = userlogin.Profile_ID;
         //{5-Jul-2016} - Moet : This is to take the employee profile id of the person who wants to take leave
         if (model.OnBehalf_Profile_ID > 0 && model.OnBehalf_Profile_ID != _ProfileID)
         {
            _ProfileID = model.OnBehalf_Profile_ID.Value;
            var empProf = empService.GetEmployeeProfileByProfileID(model.OnBehalf_Profile_ID);
            if (empProf != null)
            {
               model.OnBehalf_Employee_Profile_ID = empProf.Employee_Profile_ID;
               _EmpProfileID = model.OnBehalf_Profile_ID.Value;
            }
         }
         else
         {
            model.OnBehalf_Employee_Profile_ID = 0;
            var empProf = empService.GetEmployeeProfileByProfileID(userlogin.Profile_ID);
            if (empProf != null)
            {
               model.Employee_Profile_ID = empProf.Employee_Profile_ID;
               _EmpProfileID = model.Employee_Profile_ID.Value;
            }
         }

         if (pStatus != WorkflowStatus.Cancelled)
         {
            /* create new leave by employee*/
            model.PageStatus = pStatus;
            //{5-Jul-2016} - Moet : This is to take the employee profile id of the person who wants to take leave
            if (model.Start_Date_Period == null)
               model.Start_Date_Period = Period.AM;

            if (model.End_Date_Period == null)
               model.End_Date_Period = Period.PM;
            var ldoc = new Leave_Application_Document()
            {
               Leave_Application_Document_ID = model.Leave_Application_Document_ID.HasValue ? model.Leave_Application_Document_ID.Value : 0,
               Leave_Config_ID = model.Leave_Config_ID,
               Start_Date = DateUtil.ToDate(model.Start_Date),
               Start_Date_Period = model.Start_Date_Period,
               End_Date = DateUtil.ToDate(model.End_Date),
               End_Date_Period = model.End_Date_Period,
               Reasons = model.Reasons,
               Address_While_On_Leave = model.Address_While_On_Leave,
               Contact_While_Overseas = model.Contact_While_Overseas,
               Second_Contact_While_Overseas = model.Second_Contact_While_Overseas,
               Create_By = userlogin.User_Authentication.Email_Address,
               Create_On = currentdate,
               Update_By = userlogin.User_Authentication.Email_Address,
               Update_On = currentdate,
               Employee_Profile_ID = _EmpProfileID,
               Days_Taken = NumUtil.ParseDecimal(model.Days_Taken),
               Weeks_Taken = model.Maternity_Weeks_Taken,
               Overall_Status = WorkflowStatus.Pending,
            };

            byte[] data = null;
            string filename = "";
            if (file != null && file.ContentLength > 0)
            {
               int fileSizeInBytes = file.ContentLength;
               MemoryStream target = new MemoryStream();
               file.InputStream.CopyTo(target);
               data = target.ToArray();
               model.file = data;
               model.filename = file.FileName;
               filename = file.FileName;
            }
            else
            {
               data = model.file;
               filename = model.filename;
            }

            model.PageStatus = null;
            if (ModelState.IsValid)
            {
               if (ldoc.Start_Date > ldoc.End_Date)
                  ModelState.AddModelError("Start_Date", Resource.Message_Is_Invalid);

               if (!ldoc.Start_Date.HasValue & !string.IsNullOrEmpty(ldoc.Start_Date_Period))
                  ModelState.AddModelError("Start_Date_Period", Resource.Message_Is_Invalid);

               if (!ldoc.End_Date.HasValue & !string.IsNullOrEmpty(ldoc.End_Date_Period))
                  ModelState.AddModelError("End_Date_Period", Resource.Message_Is_Invalid);

               if (ldoc.Start_Date.HasValue & ldoc.End_Date.HasValue)
               {
                  if (ldoc.Start_Date.Value == ldoc.End_Date.Value)
                  {
                     if (ldoc.Start_Date_Period == ldoc.End_Date_Period)
                     {
                        //{5-Jul-2016} - Moet : This is to take the employee profile id of the person who wants to take leave
                        /*ldoc.End_Date = null;
                       ldoc.End_Date_Period = "";
                       model.End_Date = null;
                       model.End_Date_Period = "";*/
                     }
                     else
                     {
                        ldoc.Start_Date_Period = "";
                        ldoc.End_Date = null;
                        ldoc.End_Date_Period = "";
                        model.Start_Date_Period = "";
                        model.End_Date = null;
                        model.End_Date_Period = "";
                     }
                  }
                  else
                  {
                     //{5-Jul-2016} - Moet : This is to take the employee profile id of the person who wants to take leave
                     /*if (ldoc.Start_Date_Period == Period.AM)
                    {
                       ldoc.Start_Date_Period = "";
                       model.Start_Date_Period = "";
                    }
                    if (ldoc.End_Date_Period == Period.PM)
                    {
                       ldoc.End_Date_Period = "";
                       model.End_Date_Period = "";
                    }*/
                  }
               }
               if (ldoc.Start_Date.HasValue)
               {
                  if ((int)ldoc.Start_Date.Value.DayOfWeek == 6)
                  {
                     if (ldoc.Start_Date_Period == Period.PM)
                     {
                        //ldoc.Start_Date_Period = "";
                        //model.Start_Date_Period = "";
                     }
                  }
               }

               if (ModelState.IsValid)
               {
                  //var result = leaveService.ValidateLeaveApplicationDocument(ldoc, userlogin.Profile_ID); //{5-Jul-2016} - Moet : Comment
                  var result = leaveService.ValidateLeaveApplicationDocument(ldoc, _ProfileID);
                  //if (pStatus == "Quick")
                  //{
                  //   model.result = result;
                  //}
                  if (result.Code == ERROR_CODE.SUCCESS)
                  {
                     //var dws = leaveService.GetWorkingDayOfWeek(userlogin.Company_ID, userlogin.Profile_ID); //{5-Jul-2016} - Moet : Comment
                     var dws = leaveService.GetWorkingDayOfWeek(userlogin.Company_ID, _ProfileID);
                     var holidays = GetHolidays(userlogin.Company_ID.Value);
                     if (model.Type == LeaveConfigType.Normal)
                     {
                        model.Days_Taken = DateCal.BusinessDaysUntil(ldoc.Start_Date.Value, ldoc.Start_Date_Period, ldoc.End_Date, ldoc.End_Date_Period, dws, holidays).ToString();
                        ldoc.Days_Taken = NumUtil.ParseDecimal(model.Days_Taken);

                        if (NumUtil.ParseDecimal(model.Days_Taken) > model.Leave_Left)
                           ModelState.AddModelError("Start_Date", Resource.Over_leave);

                        if (NumUtil.ParseDecimal(model.Days_Taken) == 0)
                           ModelState.AddModelError("Start_Date", Resource.Message_The_Selected_Date_Period_Falls_On_A_Holiday);

                     }
                     else
                     {
                        ldoc.Relationship_ID = model.Relationship_ID;
                        if (model.Leave_Name == SBSModel.Common.LeaveType.MaternityLeave)
                        {
                           if (model.Flexibly.HasValue && model.Flexibly.Value && model.Continuously.HasValue && model.Continuously.Value)
                           {
                              if (model.Maternity_Is_First_Period)
                              {
                                 if (model.Entitle == model.Maternity_Weeks_Taken)
                                 {
                                    //w = (double)model.Working_Days;
                                 }
                                 else
                                    dws = leaveService.GetWorkingAllDayOfWeek();
                              }
                              //else
                              //    w = (double)model.Working_Days;
                           }
                           else if (model.Flexibly.HasValue && model.Flexibly.Value)
                           {
                              if (model.Maternity_Is_First_Period)
                                 dws = leaveService.GetWorkingAllDayOfWeek();
                              //else
                              //    w = (double)model.Working_Days;
                           }
                           //else if (model.Continuously.HasValue && model.Continuously.Value)
                           //{
                           //    w = (double)model.Working_Days;
                           //}
                        }
                        else if (model.Leave_Name == SBSModel.Common.LeaveType.PaternityLeave)
                           dws = leaveService.GetWorkingAllDayOfWeek();

                        model.Days_Taken = DateCal.BusinessDaysUntil(ldoc.Start_Date.Value, ldoc.Start_Date_Period, ldoc.End_Date, ldoc.End_Date_Period, dws, holidays).ToString();
                        ldoc.Days_Taken = NumUtil.ParseDecimal(model.Days_Taken);

                        if (model.Maternity_Weeks_Taken > model.Leave_Left)
                           ModelState.AddModelError("Start_Date", Resource.Over_leave);

                        if (model.Maternity_Weeks_Taken == 0)
                           ModelState.AddModelError("Start_Date", Resource.Message_The_Selected_Date_Period_Falls_On_A_Holiday);
                     }

                     if (ModelState.IsValid)
                     {
                        ldoc.Supervisor = hist.Supervisor;

                        //Edit by sun 05-11-2015
                        var haveApprover = true;
                        var rworkflow = aService.GetWorkflowByEmployee(userlogin.Company_ID.Value, userlogin.Profile_ID, ModuleCode.HR, ApprovalType.Leave, hist.Department_ID);
                        if (!rworkflow.Item2.IsSuccess || rworkflow.Item1 == null || rworkflow.Item1.Count == 0)
                           haveApprover = false;

                        //result = leaveService.SaveLeaveApplicationDocument(ldoc, data, filename, userlogin.Profile_ID); //{5-Jul-2016} - Moet : Comment
                        result = leaveService.SaveLeaveApplicationDocument(ldoc, data, filename, _ProfileID);
                        model.result = result;
                        if (result.Code == ERROR_CODE.SUCCESS)
                        {
                           var leave = leaveService.GetLeaveApplicationDocument(ldoc.Leave_Application_Document_ID);
                           //{5-Jul-2016} - Moet : Modified for onbehalf leave application
                           if (leave != null)
                           {
                              if (haveApprover)
                              {
                                 //save Leave_Application_Document_ID Request
                                 var request = new RequestItem();
                                 request.Doc_ID = ldoc.Leave_Application_Document_ID;
                                 request.Approval_Type = ApprovalType.Leave;
                                 request.Company_ID = userlogin.Company_ID.Value;
                                 request.Department_ID = hist.Department_ID.HasValue ? hist.Department_ID.Value : 0;
                                 request.Module = ModuleCode.HR;
                                 //{5-Jul-2016} - Moet : Modified for onbehalf leave application
                                 if (model.OnBehalf_Employee_Profile_ID > 0)
                                 {
                                    var userProfile = uService.getUserByEmployeeProfile(model.OnBehalf_Employee_Profile_ID);
                                    request.Requestor_Email = userProfile.Email;
                                    request.Requestor_Name = UserSession.GetUserName(userProfile);
                                    request.Requestor_Profile_ID = userProfile.Profile_ID;
                                 }
                                 else
                                 {
                                    request.Requestor_Email = userlogin.User_Authentication.Email_Address;
                                    request.Requestor_Name = UserSession.GetUserName(userlogin);
                                    request.Requestor_Profile_ID = userlogin.Profile_ID;
                                 }
                                 var r = aService.SubmitRequest(request);
                                 if (r.IsSuccess)
                                 {
                                    leave.Supervisor = null;
                                    leave.Request_ID = request.Request_ID;
                                    leave.Overall_Status = request.Status;
                                    leave.Update_By = userlogin.User_Authentication.Email_Address;
                                    leave.Update_On = currentdate;
                                    result = leaveService.UpdateLeaveApplicationDocument(leave);
                                    model.result = result;
                                    if (result.Code == ERROR_CODE.SUCCESS)
                                    {
                                       if (request.Status == WorkflowStatus.Closed)
                                       {
                                          result = leaveService.UpdateLeaveUse(leave.Leave_Application_Document_ID, _EmpProfileID, request.Status);
                                          if (result.Code == ERROR_CODE.SUCCESS)
                                             sendProceedEmail(leave, null, userlogin, userlogin, hist, request.Status, request.Reviewers);
                                       }
                                       else
                                       {
                                          var param = new Dictionary<string, object>();
                                          param.Add("lID", leave.Leave_Application_Document_ID);
                                          param.Add("appID", request.NextApprover.Profile_ID);
                                          param.Add("empID", leave.Employee_Profile_ID);
                                          param.Add("reqID", leave.Request_ID);
                                          param.Add("status", WorkflowStatus.Approved);
                                          param.Add("code", uService.GenActivateCode("L" + leave.Leave_Application_Document_ID + request.NextApprover.Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

                                          var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                                          param["status"] = WorkflowStatus.Rejected;
                                          var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                                          var appr = uService.getUser(request.NextApprover.Profile_ID, false);
                                          if (appr != null)
                                          {
                                             sendProceedEmail(leave, null, userlogin, userlogin, hist, WorkflowStatus.Submitted, request.Reviewers,
                                             model.OnBehalf_Employee_Profile_ID.Value, model.OnBehalf_Profile_ID.Value, model.requestURL, UserSession.GetUserName(appr));
                                             sendRequestEmail(leave, null, appr, userlogin, hist, request.Status, request.Reviewers, linkApp, linkRej, model.requestURL);
                                          }
                                          else
                                          {
                                             sendProceedEmail(leave, null, userlogin, userlogin, hist, WorkflowStatus.Submitted, request.Reviewers,
                                             model.OnBehalf_Employee_Profile_ID.Value, model.OnBehalf_Profile_ID.Value, model.requestURL);
                                             sendRequestEmail(leave, null, appr, userlogin, hist, request.Status, request.Reviewers, linkApp, linkRej, model.requestURL);
                                          }
                                       }
                                    }
                                    if (pStatus == "Quick")
                                       return View(model);
                                    else
                                       return RedirectToAction("Record", new { Code = result.Code, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS), Field = result.Field });
                                 }
                              }
                              else
                              {
                                 if (hist.Supervisor.HasValue)
                                 {
                                    var sup = empService.GetEmployeeProfile2(hist.Supervisor);
                                    if (sup != null)
                                    {
                                       var param = new Dictionary<string, object>();
                                       param.Add("lID", leave.Leave_Application_Document_ID);
                                       param.Add("appID", sup.Profile_ID);
                                       param.Add("empID", leave.Employee_Profile_ID);
                                       param.Add("status", WorkflowStatus.Approved);
                                       param.Add("code", uService.GenActivateCode("L" + leave.Leave_Application_Document_ID + sup.Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

                                       var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                                       param["status"] = WorkflowStatus.Rejected;
                                       var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                                       sendProceedEmail(leave, null, userlogin, userlogin, hist, WorkflowStatus.Submitted, null,
                                              model.OnBehalf_Employee_Profile_ID.Value, model.OnBehalf_Profile_ID.Value, model.requestURL, UserSession.GetUserName(sup.User_Profile));
                                       sendRequestEmail(leave, null, sup.User_Profile, userlogin, hist, leave.Overall_Status, null, linkApp, linkRej, model.requestURL);
                                       if (pStatus == "Quick")
                                          return View(model);
                                       else
                                          return RedirectToAction("Record", new { Code = result.Code, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS), Field = result.Field });
                                    }
                                 }
                                 else
                                 {
                                    leave.Overall_Status = WorkflowStatus.Closed;
                                    leave.Update_By = userlogin.User_Authentication.Email_Address;
                                    leave.Update_On = currentdate;
                                    result = leaveService.UpdateLeaveApplicationDocument(leave);
                                    if (result.Code == ERROR_CODE.SUCCESS)
                                    {
                                       result = leaveService.UpdateLeaveUse(leave.Leave_Application_Document_ID, leave.Employee_Profile_ID, leave.Overall_Status);
                                       if (result.Code == ERROR_CODE.SUCCESS)
                                       {
                                          sendProceedEmail(leave, null, userlogin, userlogin, hist, leave.Overall_Status, null,
                                              model.OnBehalf_Employee_Profile_ID.Value, model.OnBehalf_Profile_ID.Value, model.requestURL);
                                       }

                                    }
                                    if (pStatus == "Quick")
                                       return View(model);
                                    else
                                       return RedirectToAction("Record", new { Code = result.Code, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS), Field = result.Field });
                                 }
                              }
                           }
                        }
                        else
                        {
                           model.result = result;
                           return View(model);
                        }
                     }
                  }
                  else
                  {

                     if (!string.IsNullOrEmpty(result.Field))
                     {
                        if (ldoc.Start_Date.HasValue)
                           ModelState.AddModelError("Start_Date", result.Msg);
                        if (ldoc.End_Date.HasValue)
                           ModelState.AddModelError("End_Date", result.Msg);
                     }
                     else
                        errorPage(result);
                  }
               }
            }
         }
         else if (pStatus == WorkflowStatus.Cancelled)
         {
            /* cancel leave by employee*/
            if (!model.Request_ID.HasValue)
            {
               /* not have approver*/
               var leave = leaveService.GetLeaveApplicationDocument(model.Leave_Application_Document_ID);
               if (leave == null)
                  return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Leave);

               if (model.Supervisor.HasValue)
               {
                  var sup = empService.GetEmployeeProfile2(hist.Supervisor);
                  if (sup != null)
                  {
                     if (leave.Overall_Status == WorkflowStatus.Closed)
                     {
                        leave.Cancel_Status = WorkflowStatus.Canceling;
                        leave.Update_By = userlogin.User_Authentication.Email_Address;
                        leave.Update_On = currentdate;

                        model.result = leaveService.UpdateLeaveApplicationDocument(leave);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                           var param = new Dictionary<string, object>();
                           param.Add("lID", leave.Leave_Application_Document_ID);
                           param.Add("appID", sup.Profile_ID);
                           param.Add("empID", leave.Employee_Profile_ID);
                           param.Add("cancelStatus", WorkflowStatus.Cancelled);
                           param.Add("code", uService.GenActivateCode("L" + leave.Leave_Application_Document_ID + sup.Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));
                           var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                           param["cancelStatus"] = WorkflowStatus.Rejected;
                           var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                           sendProceedEmail(leave, null, userlogin, userlogin, hist, leave.Cancel_Status, null,
                                              model.OnBehalf_Employee_Profile_ID.Value, model.OnBehalf_Profile_ID.Value, UserSession.GetUserName(sup.User_Profile));
                           sendRequestEmail(leave, null, sup.User_Profile, userlogin, hist, leave.Cancel_Status, null, linkApp, linkRej);
                           return RedirectToAction("Record", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL), Field = Resource.Leave });
                        }
                     }
                     else
                     {
                        model.result = leaveService.UpdateLeaveUse(model.Leave_Application_Document_ID, model.Employee_Profile_ID, null, WorkflowStatus.Cancelled);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                           return RedirectToAction("Record", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL), Field = Resource.Leave });
                     }
                  }
               }
               else
               {
                  model.result = leaveService.UpdateLeaveUse(model.Leave_Application_Document_ID, model.Employee_Profile_ID, null, WorkflowStatus.Cancelled, false);
                  if (model.result.Code == ERROR_CODE.SUCCESS)
                  {
                     sendProceedEmail(leave, null, userlogin, userlogin, hist, leave.Cancel_Status, null,
                                              model.OnBehalf_Employee_Profile_ID.Value, model.OnBehalf_Profile_ID.Value);
                     return RedirectToAction("Record", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL), Field = Resource.Leave });
                  }
               }
            }
            else
            {
               var leave = leaveService.GetLeaveApplicationDocument(model.Leave_Application_Document_ID);
               if (leave == null)
                  return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Leave);

               if (leave.Overall_Status == WorkflowStatus.Closed)
               {
                  var rqcancel = new RequestItem();
                  rqcancel.Doc_ID = model.Leave_Application_Document_ID;
                  rqcancel.Approval_Type = ApprovalType.Leave;
                  rqcancel.Company_ID = userlogin.Company_ID.Value;
                  rqcancel.Department_ID = hist.Department_ID.HasValue ? hist.Department_ID.Value : 0;
                  rqcancel.Module = ModuleCode.HR;
                  rqcancel.Requestor_Email = userlogin.User_Authentication.Email_Address;
                  rqcancel.Requestor_Name = UserSession.GetUserName(userlogin);
                  rqcancel.Requestor_Profile_ID = userlogin.Profile_ID;
                  var rc = aService.SubmitRequestCanceling(rqcancel);
                  if (rc.IsSuccess)
                  {
                     leave.Request_Cancel_ID = rqcancel.Request_ID;
                     leave.Cancel_Status = WorkflowStatus.Canceling;
                     leave.Update_By = userlogin.User_Authentication.Email_Address;
                     leave.Update_On = currentdate;

                     model.result = leaveService.UpdateLeaveApplicationDocument(leave);
                     if (model.result.Code == ERROR_CODE.SUCCESS)
                     {
                        var param = new Dictionary<string, object>();
                        param.Add("lID", leave.Leave_Application_Document_ID);
                        param.Add("appID", rqcancel.NextApprover.Profile_ID);
                        param.Add("empID", leave.Employee_Profile_ID);
                        param.Add("reqcancelID", rqcancel.Request_ID);
                        param.Add("cancelStatus", WorkflowStatus.Cancelled);
                        param.Add("code", uService.GenActivateCode("L" + leave.Leave_Application_Document_ID + rqcancel.NextApprover.Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));
                        var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                        param["cancelStatus"] = WorkflowStatus.Rejected;
                        var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                        var approverName = string.Empty;
                        var appr = uService.getUser(rqcancel.NextApprover.Profile_ID, false);
                        if (appr != null)
                        {
                           sendRequestEmail(leave, null, appr, userlogin, hist, leave.Cancel_Status, rqcancel.Reviewers, linkApp, linkRej);
                           approverName = UserSession.GetUserName(appr);
                        }
                        sendProceedEmail(leave, null, userlogin, userlogin, hist, leave.Cancel_Status, rqcancel.Reviewers,
                                              model.OnBehalf_Employee_Profile_ID.Value, model.OnBehalf_Profile_ID.Value, approverName);

                        return RedirectToAction("Record", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL), Field = Resource.Leave });
                     }
                  }
               }
               else
               {
                  leave.Cancel_Status = WorkflowStatus.Cancelled;
                  leave.Update_By = userlogin.User_Authentication.Email_Address;
                  leave.Update_On = currentdate;

                  var result = leaveService.UpdateLeaveApplicationDocument(leave);
                  if (result.Code == ERROR_CODE.SUCCESS)
                  {
                     sendProceedEmail(leave, null, userlogin, userlogin, hist, leave.Cancel_Status, null,
                                              model.OnBehalf_Employee_Profile_ID.Value, model.OnBehalf_Profile_ID.Value);
                     return RedirectToAction("Record", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL), Field = Resource.Leave });
                  }
               }
            }
         }
         if (pStatus != "Quick")
         {
            model.LeaveTypeComboList = cbService.LstAndCalulateLeaveType(new LeaveTypeCriteria()
            {
               Company_ID = userlogin.Company_ID,
               Profile_ID = userlogin.Profile_ID,
               Ignore_Generate = true
            });

            model.periodList = cbService.LstDatePeriod(true);
            model.Working_Days = leaveService.GetWorkingDayOfWeek(userlogin.Company_ID, userlogin.Profile_ID);
            if (model.Request_ID.HasValue)
            {
               var r = aService.GetMyRequests(userlogin.Company_ID, null, null, ModuleCode.HR, ApprovalType.Leave, model.Leave_Application_Document_ID);
               if (r.Item2.IsSuccess && r.Item1 != null)
                  model.Leave_Request = r.Item1;
            }

            //-------rights------------
            RightResult rightResult = validatePageRight(UserSession.RIGHT_A);
            if (rightResult.action != null)
               return rightResult.action;
            model.rights = rightResult.rights;
         }
         return View(model);
      }

      public List<DateTime> GetHolidays(int CompanyID)
      {
         var lService = new LeaveService();
         var bankHolidays = new List<DateTime>();
         var hilidaylist = lService.getHolidays(CompanyID);
         if (hilidaylist != null)
         {
            foreach (var h in hilidaylist)
            {
               if (h.End_Date.HasValue & h.Start_Date.HasValue)
               {
                  for (var dt = h.Start_Date.Value; dt <= h.End_Date.Value; dt = dt.AddDays(1))
                  {
                     bankHolidays.Add(dt);
                  }
               }
               else
                  bankHolidays.Add(h.Start_Date.Value);
            }
         }
         return bankHolidays;
      }
      #endregion

      #region Send Email Leave
      public void sendRequestEmail(Leave_Application_Document leave, Company_Details com, User_Profile sentto, User_Profile receivedfrom,
        Employment_History receivedhist, string Overall_Status, List<Reviewer> Reviewers, string linkApp, string linkRej, string _RequestURL = "")
      {
         var ecode = "[RL" + leave.Leave_Application_Document_ID + "_";
         if (leave.Request_Cancel_ID.HasValue)
            ecode += "RQC" + leave.Request_Cancel_ID;
         else if (leave.Request_ID.HasValue)
            ecode += "RQ" + leave.Request_ID;

         ecode += "S" + receivedfrom.User_Authentication.User_Authentication_ID + "R" + sentto.User_Authentication.User_Authentication_ID + "]";

         var leaveService = new LeaveService();
         var cri = new LeaveTypeCriteria()
         {
            Profile_ID = leave.Employee_Profile.Profile_ID,
            Leave_Config_ID = leave.Leave_Config_ID,
            Relationship_ID = leave.Relationship_ID
         };
         var leaveleft = leaveService.CalculateLeaveLeft(cri);
         var s_URL = "";
         var s_LogoLink = "";
         if (Request == null && _RequestURL != "")
         {
            s_URL = _RequestURL;
            Uri relativeUri = new Uri(s_URL);
            s_LogoLink = Quick_GenerateLogoLink(relativeUri);
         }
         else
         {
            s_URL = Request.Url.AbsoluteUri;
            s_LogoLink = GenerateLogoLink();
         }
         var eitem = new EmailItem()
         {
            Doc_ID = leave.Leave_Application_Document_ID,
            LogoLink = s_LogoLink,
            Company = com,
            Send_To_Email = sentto.User_Authentication.Email_Address,
            Send_To_Name = AppConst.GetUserName(sentto),
            Received_From_Email = receivedfrom.User_Authentication.Email_Address,
            Received_From_Name = AppConst.GetUserName(receivedfrom),
            Received_From_Department = receivedhist.Department != null ? receivedhist.Department.Name : "",
            Module = ModuleCode.HR,
            Approval_Type = ApprovalType.Leave,
            Leave = leave,
            Leave_Left = leaveleft != null ? leaveleft.Left : 0,
            Weeks_Left = leaveleft != null ? leaveleft.Weeks_Left : 0,
            Status = Overall_Status,
            Reviewer = Reviewers,
            Link = linkApp,
            Link2 = linkRej,
            Url = s_URL,
            ECode = ecode,
            Approver_Name = "",
         };
         EmailTemplete.sendRequestEmail(eitem);
      }

      public void sendProceedEmail(Leave_Application_Document leave, Company_Details com, User_Profile sentto, User_Profile receivedfrom, Employment_History receivedhist, string Overall_Status, List<Reviewer> Reviewers,
          int OnBehalf_Employee_Profile_ID = 0, int OnBehalf_Profile_ID = 0, string _RequestURL = "", string approverName = "")
      {
         var ecode = "[PL" + leave.Leave_Application_Document_ID + "_";
         if (leave.Request_Cancel_ID.HasValue)
            ecode += "RQC" + leave.Request_Cancel_ID;
         else if (leave.Request_ID.HasValue)
            ecode += "RQ" + leave.Request_ID;

         ecode += "S" + receivedfrom.User_Authentication.User_Authentication_ID + "R" + sentto.User_Authentication.User_Authentication_ID + "]";

         var leaveService = new LeaveService();
         var _profileID = leave.Employee_Profile.Profile_ID;
         if (OnBehalf_Profile_ID > 0)
            _profileID = OnBehalf_Profile_ID;
         var cri = new LeaveTypeCriteria()
         {
            Profile_ID = _profileID,
            Leave_Config_ID = leave.Leave_Config_ID,
            Relationship_ID = leave.Relationship_ID
         };
         var leaveleft = leaveService.CalculateLeaveLeft(cri);
         var s_URL = "";
         var s_LogoLink = "";
         if (Request == null && _RequestURL != "")
         {
            s_URL = _RequestURL;
            Uri relativeUri = new Uri(s_URL);
            s_LogoLink = Quick_GenerateLogoLink(relativeUri);
         }
         else
         {
            s_URL = Request.Url.AbsoluteUri;
            s_LogoLink = GenerateLogoLink();
         }


         if (string.IsNullOrEmpty(approverName))
         {
            var userlogin = UserSession.getUser(HttpContext);
            if (userlogin != null)
               approverName = UserSession.GetUserName(userlogin);
         }

         var eitem = new EmailItem()
         {
            Doc_ID = leave.Leave_Application_Document_ID,
            LogoLink = s_LogoLink,
            Company = com,
            Send_To_Email = sentto.User_Authentication.Email_Address,
            Send_To_Name = AppConst.GetUserName(sentto),
            Received_From_Email = receivedfrom.User_Authentication.Email_Address,
            Received_From_Name = AppConst.GetUserName(receivedfrom),
            Received_From_Department = receivedhist.Department != null ? receivedhist.Department.Name : "",
            Module = ModuleCode.HR,
            Approval_Type = ApprovalType.Leave,
            Leave = leave,
            Leave_Left = leaveleft != null ? leaveleft.Left : 0,
            Weeks_Left = leaveleft != null ? leaveleft.Weeks_Left : 0,
            Status = Overall_Status,
            Reviewer = Reviewers,
            Url = s_URL,
            ECode = ecode,
            Approver_Name = approverName,
         };

         if (OnBehalf_Profile_ID > 0 && OnBehalf_Profile_ID != leave.Employee_Profile.Profile_ID)
         {
            var empService = new EmployeeService();
            var emp = empService.GetEmployeeProfile(OnBehalf_Employee_Profile_ID);
            var onBehalfName = AppConst.GetUserName(emp.User_Profile);
            var r = new Reviewer();
            r.Email = emp.User_Profile.Email;

            if (eitem.Reviewer != null)
               eitem.Reviewer.Add(r);
            else
            {
               List<SBSWorkFlowAPI.Models.Reviewer> lstR = new List<SBSWorkFlowAPI.Models.Reviewer>();
               lstR.Add(r);
               eitem.Reviewer = lstR;
            }
            EmailTemplete.sendProceedEmail(eitem, onBehalfName);
         }
         else
         {
            EmailTemplete.sendProceedEmail(eitem);
         }

      }
      #endregion

      #region Send Email Expenses

      public void sendRequestEmail(Expenses_Application ex, Company_Details com, User_Profile sentto, User_Profile receivedfrom, Employment_History receivedhist, string Overall_Status, List<Reviewer> Reviewers, string linkApp, string linkRej)
      {
         var ecode = "[RE" + ex.Expenses_Application_ID + "_";
         if (ex.Request_Cancel_ID.HasValue)
            ecode += "RQC" + ex.Request_Cancel_ID;
         else if (ex.Request_ID.HasValue)
            ecode += "RQ" + ex.Request_ID;

         ecode += "S" + receivedfrom.User_Authentication.User_Authentication_ID + "R" + sentto.User_Authentication.User_Authentication_ID + "]";

         var SmartDevPdfRequest = getFileExpenseSmartDevPdf(EncryptUtil.Encrypt(ex.Expenses_Application_ID), EncryptUtil.Encrypt(UserSession.RIGHT_U), ex.Employee_Profile_ID);
         var eitem = new EmailItem()
         {
            LogoLink = GenerateLogoLink(),
            Company = com,
            Send_To_Email = sentto.User_Authentication.Email_Address,
            Send_To_Name = AppConst.GetUserName(sentto),
            Received_From_Email = receivedfrom.User_Authentication.Email_Address,
            Received_From_Name = AppConst.GetUserName(receivedfrom),
            Received_From_Department = receivedhist.Department != null ? receivedhist.Department.Name : "",
            Module = ModuleCode.HR,
            Approval_Type = ApprovalType.Expense,
            Expenses = ex,
            Status = Overall_Status,
            Reviewer = Reviewers,
            Link = linkApp,
            Link2 = linkRej,
            Url = Request.Url.AbsoluteUri,
            ECode = ecode,
            Attachment_SmartDev = SmartDevPdfRequest
         };
         EmailTemplete.sendRequestEmail(eitem);
      }

      public void sendProceedEmail(Expenses_Application ex, Company_Details com, User_Profile sentto, User_Profile receivedfrom, Employment_History receivedhist, string Overall_Status, List<Reviewer> Reviewers,
          int OnBehalf_Employee_Profile_ID = 0, int OnBehalf_Profile_ID = 0, string approverName = "")
      {
         var ecode = "[PE" + ex.Expenses_Application_ID + "_";
         if (ex.Request_Cancel_ID.HasValue)
            ecode += "RQC" + ex.Request_Cancel_ID;
         else if (ex.Request_ID.HasValue)
            ecode += "RQ" + ex.Request_ID;
         ecode += "S" + receivedfrom.User_Authentication.User_Authentication_ID + "R" + sentto.User_Authentication.User_Authentication_ID + "]";

         var SmartDevPdfRequest = getFileExpenseSmartDevPdf(EncryptUtil.Encrypt(ex.Expenses_Application_ID), EncryptUtil.Encrypt(UserSession.RIGHT_U), ex.Employee_Profile_ID);
         if (string.IsNullOrEmpty(approverName))
         {
            var userlogin = UserSession.getUser(HttpContext);
            if (userlogin != null)
               approverName = UserSession.GetUserName(userlogin);
         }

         var eitem = new EmailItem()
         {
            Doc_ID = ex.Expenses_Application_ID,
            LogoLink = GenerateLogoLink(),
            Company = com,
            Send_To_Email = sentto.User_Authentication.Email_Address,
            Send_To_Name = AppConst.GetUserName(sentto),
            Received_From_Email = receivedfrom.User_Authentication.Email_Address,
            Received_From_Name = AppConst.GetUserName(receivedfrom),
            Received_From_Department = receivedhist.Department != null ? receivedhist.Department.Name : "",
            Module = ModuleCode.HR,
            Approval_Type = ApprovalType.Expense,
            Expenses = ex,
            Status = Overall_Status,
            Reviewer = Reviewers,
            Url = Request.Url.AbsoluteUri,
            ECode = ecode,
            Attachment_SmartDev = SmartDevPdfRequest,
            Approver_Name = approverName,
         };


         //EmailTemplete.sendProceedEmail(eitem);
         if (OnBehalf_Profile_ID > 0 && OnBehalf_Profile_ID != receivedfrom.Profile_ID)
         {
            var empService = new EmployeeService();
            var emp = empService.GetEmployeeProfile(OnBehalf_Employee_Profile_ID);
            var onBehalfName = "";
            if (OnBehalf_Employee_Profile_ID > 0)
            {
               onBehalfName = AppConst.GetUserName(emp.User_Profile);
            }
            var r = new Reviewer();
            eitem.Send_To_Email = emp.User_Profile.Email; // Requester's email
            r.Email = sentto.User_Authentication.Email_Address; // Submitter's email

            if (eitem.Reviewer != null)
               eitem.Reviewer.Add(r);
            else
            {
               List<SBSWorkFlowAPI.Models.Reviewer> lstR = new List<SBSWorkFlowAPI.Models.Reviewer>();
               lstR.Add(r);
               eitem.Reviewer = lstR;
            }
            EmailTemplete.sendProceedEmail(eitem, onBehalfName);
         }
         else
         {
            EmailTemplete.sendProceedEmail(eitem);
         }

      }



      #endregion

      #region Process Approval
      public void EmployeeProcessApproval(Nullable<int> pProfileID)
      {
         var histService = new EmploymentHistoryService();
         var hist = histService.GetCurrentEmploymentHistoryByProfile(pProfileID);
         if (hist != null && hist.Employee_Profile != null && hist.Department_ID.HasValue)
         {
            var emp = hist.Employee_Profile;
            var profile = emp.User_Profile;

            var aService = new SBSWorkFlowAPI.Service();
            var wfLResult = aService.GetWorkflowByCompany(profile.Company_ID, ModuleCode.HR, ApprovalType.Leave);
            if (wfLResult != null && wfLResult.Item2.IsSuccess && wfLResult.Item1 != null)
            {

            }
            var wfEResult = aService.GetWorkflowByCompany(profile.Company_ID, ModuleCode.HR, ApprovalType.Expense);
            if (wfEResult != null && wfEResult.Item2.IsSuccess && wfEResult.Item1 != null)
            {
               foreach (var wf in wfEResult.Item1)
               {
                  if (wf.Departments != null && wf.Departments.Select(s => s.User_Department_ID).Contains(hist.Department_ID.Value))
                  {
                     // this work flow have this department
                     if (!wf.Applicable_Employee.Select(s => s.Profile_ID).Contains(profile.Profile_ID))
                     {
                        // add new Applicable_Employee if not have
                        wf.Applicable_Employee.Add(new SBSWorkFlowAPI.Models.Applicable_Employee()
                        {
                           Approval_Flow_ID = wf.Approval_Flow_ID,
                           Company_ID = wf.Company_ID,
                           Applicable_Employee_ID = hist.Employee_Profile_ID,
                           Email = profile.Email,
                           Name = UserSession.GetUserName(profile),
                           Profile_ID = profile.Profile_ID,
                           Is_Applicable = true
                        });
                        aService.UpdateWorkFlow(wf);
                     }
                  }
                  else
                  {
                     var aEmp = wf.Applicable_Employee.Where(w => w.Profile_ID == profile.Profile_ID).FirstOrDefault();
                     if (aEmp != null)
                     {
                        wf.Applicable_Employee.Remove(aEmp);
                        aService.UpdateWorkFlow(wf);
                     }
                  }
               }
            }
         }
      }
      #endregion

      #region TsEx
      [HttpGet]
      public ActionResult TsExMngt(int pID, string pS, string ctlr, string ac)
      {
         var result = new ServiceResult();
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var currentdate = StoredProcedure.GetCurrentDate();
         var tsexService = new TsExService();
         var cbService = new ComboService();
         var empService = new EmployeeService();
         var hService = new EmploymentHistoryService();
         var uService = new UserService();
         var cpService = new CompanyService();
         var aService = new SBSWorkFlowAPI.Service();
         var status = pS;

         var tsex = tsexService.GetTsEx(pID);
         if (tsex != null)
         {
            var hist = hService.GetCurrentEmploymentHistory(tsex.Employee_Profile_ID);
            if (hist == null)
               return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Employment_History);

            var user = uService.getUser(hist.Employee_Profile.Profile_ID, false);
            if (user == null)
               return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.User);

            var ex = tsex.Expenses_Application;
            var Ac_Code = "E" + ex.Expenses_Application_ID + userlogin.Profile_ID + "_";

            if (string.IsNullOrEmpty(ex.Cancel_Status))
            {
               if (ex.Request_ID.HasValue)
               {
                  #region Workflow
                  var action = new ActionItem();
                  action.Actioner_Profile_ID = userlogin.Profile_ID;
                  action.Email = userlogin.User_Authentication.Email_Address;
                  action.Name = UserSession.GetUserName(userlogin);
                  action.Request_ID = ex.Request_ID.Value;
                  if (status == WorkflowStatus.Approved)
                  {
                     action.IsApprove = true;
                     action.Action = WorkflowAction.Approve;
                  }
                  else
                  {
                     action.IsApprove = false;
                     action.Action = WorkflowAction.Reject;
                  }
                  if (ModelState.IsValid)
                  {
                     var r = aService.SubmitRequestAction(action);
                     if (r.IsSuccess)
                     {
                        var exapp = new ExApprover();
                        exapp.Approver = getApprovalStrIDs(ex.Approver, userlogin.Profile_ID.ToString());
                        if (action.Status == WorkflowStatus.Closed)
                        {
                           exapp.Next_Approver = null;
                           ex.Overall_Status = WorkflowStatus.Closed;
                           result = tsexService.UpdateTsExStatus(tsex.TsEx_ID, action.Status, includeapprove: true, exapprover: exapp);
                           if (result.Code == ERROR_CODE.SUCCESS)
                           {
                              uService.ExpireActivationByPrefix(Ac_Code);
                              sendEmpEmail(tsex, user, userlogin, hist, action.Status);
                              return RedirectToAction(ac, ctlr, new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resource.Timesheet_Expenses });
                           }
                        }
                        else if (action.Status == WorkflowStatus.Rejected)
                        {
                           exapp.Next_Approver = null;
                           result = tsexService.UpdateTsExStatus(tsex.TsEx_ID, WorkflowStatus.Rejected, includeapprove: true, exapprover: exapp);
                           if (result.Code == ERROR_CODE.SUCCESS)
                           {
                              uService.ExpireActivationByPrefix(Ac_Code);
                              sendEmpEmail(tsex, user, userlogin, hist, action.Status);
                              return RedirectToAction(ac, ctlr, new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_REJECT), Field = Resource.Timesheet_Expenses });
                           }
                        }
                        else
                        {
                           var mstr = "";
                           if (action.NextApprover == null)
                           {
                              #region Indent flow
                              var haveSendRequestEmail = false;
                              if (action.IndentValue != null && action.IndentValue.IsIndent && action.IndentValue.SendRequest)
                                 haveSendRequestEmail = true;

                              if (haveSendRequestEmail)
                              {
                                 exapp.Next_Approver = null;
                                 List<IndentItem> IndentItems = getIndentSupervisor(tsex.Expenses_Application_ID, tsex.Time_Sheet_ID);
                                 if (IndentItems != null && IndentItems.Count > 0)
                                 {
                                    foreach (var row in IndentItems)
                                    {
                                       if (action.Actioner_Profile_ID == row.Requestor_Profile_ID)
                                          continue;

                                       var param = new Dictionary<string, object>();
                                       param.Add("expID", ex.Expenses_Application_ID);
                                       param.Add("appID", row.Requestor_Profile_ID);
                                       param.Add("empID", ex.Employee_Profile_ID);
                                       param.Add("reqID", ex.Request_ID);
                                       param.Add("status", WorkflowStatus.Approved);
                                       param.Add("code", uService.GenActivateCode("E" + ex.Expenses_Application_ID + row.Requestor_Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

                                       var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                                       param["status"] = WorkflowStatus.Rejected;
                                       var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                                       var appr = uService.getUser(row.Requestor_Profile_ID, false);
                                       if (appr != null)
                                          sendApprovalEmail(tsex, appr, user, hist, WorkflowStatus.Submitted, null, linkApp, linkRej);

                                       mstr = getApprovalStrIDs(mstr, row.Requestor_Profile_ID.ToString());
                                    }
                                 }
                              }
                              else
                              {
                                 var str = getApprovalStrIDs(null, userlogin.Profile_ID.ToString());
                                 if (!string.IsNullOrEmpty(str))
                                    mstr = ex.Next_Approver.Replace(str, "|");
                              }
                              #endregion
                           }
                           else
                           {
                              #region Normal flow
                              var param = new Dictionary<string, object>();
                              param.Add("expID", ex.Expenses_Application_ID);
                              param.Add("appID", action.NextApprover.Profile_ID);
                              param.Add("empID", ex.Employee_Profile_ID);
                              param.Add("reqID", ex.Request_ID);
                              param.Add("status", WorkflowStatus.Approved);
                              param.Add("code", uService.GenActivateCode("E" + ex.Expenses_Application_ID + action.NextApprover.Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

                              var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                              param["status"] = WorkflowStatus.Rejected;
                              var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                              var appr = uService.getUser(action.NextApprover.Profile_ID, false);
                              if (appr != null)
                                 sendApprovalEmail(tsex, appr, user, hist, WorkflowStatus.Submitted, null, linkApp, linkRej);

                              if (action.NextApprover.Profile_ID != userlogin.Profile_ID)
                                 mstr = getApprovalStrIDs(mstr, action.NextApprover.Profile_ID.ToString());
                              #endregion
                           }

                           if (!string.IsNullOrEmpty(mstr))
                              exapp.Next_Approver = mstr;
                           result = tsexService.UpdateTsExStatus(tsex.TsEx_ID, includeapprove: true, exapprover: exapp);

                           uService.ExpireActivationByPrefix(Ac_Code);
                           return RedirectToAction(ac, ctlr, new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resource.Timesheet_Expenses });
                        }

                     }
                  }
                  #endregion
               }
               else if (tsex.Expenses_Application.Supervisor.HasValue)
               {
                  #region Supervisor

                  var exapp = new ExApprover();
                  exapp.Next_Approver = null;
                  exapp.Approver = getApprovalStrIDs(null, userlogin.Profile_ID.ToString());

                  /*approval by supervisor*/
                  if (status == WorkflowStatus.Approved)
                     result = tsexService.UpdateTsExStatus(tsex.TsEx_ID, WorkflowStatus.Closed, includeapprove: true, exapprover: exapp);
                  else
                     result = tsexService.UpdateTsExStatus(tsex.TsEx_ID, WorkflowStatus.Rejected, includeapprove: true, exapprover: exapp);

                  if (result.Code == ERROR_CODE.SUCCESS)
                  {
                     uService.ExpireActivationByPrefix(Ac_Code);
                     sendEmpEmail(tsex, user, userlogin, hist, ex.Overall_Status);
                     if (status == WorkflowStatus.Approved)
                        return RedirectToAction(ac, ctlr, new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resource.Timesheet_Expenses });
                     else
                        return RedirectToAction(ac, ctlr, new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_REJECT), Field = Resource.Timesheet_Expenses });
                  }
                  #endregion
               }
            }
            else
            {
               /* approve canncel workflow*/
               if (ex.Request_Cancel_ID.HasValue && ex.Request_Cancel_ID.Value > 0)
               {
                  #region Workflow
                  var action = new ActionItem();
                  action.Actioner_Profile_ID = userlogin.Profile_ID;
                  action.Email = userlogin.User_Authentication.Email_Address;
                  action.Name = UserSession.GetUserName(userlogin);
                  action.Request_ID = ex.Request_Cancel_ID.Value;
                  if (status == WorkflowStatus.Approved)
                  {
                     action.IsApprove = true;
                     action.Action = WorkflowAction.Approve;
                  }
                  else
                  {
                     action.IsApprove = false;
                     action.Action = WorkflowAction.Reject;
                  }

                  if (ModelState.IsValid)
                  {
                     var r = aService.SubmitRequestAction(action);
                     if (r.IsSuccess)
                     {
                        var exapp = new ExApprover();
                        exapp.Approver = getApprovalStrIDs(ex.Approver, userlogin.Profile_ID.ToString());
                        if (action.Status == WorkflowStatus.Closed)
                        {
                           exapp.Next_Approver = null;
                           result = tsexService.UpdateTsExStatus(tsex.TsEx_ID, null, WorkflowStatus.Cancelled, includeapprove: true, exapprover: exapp);
                           if (result.Code == ERROR_CODE.SUCCESS)
                           {
                              uService.ExpireActivationByPrefix(Ac_Code);
                              sendEmpEmail(tsex, user, userlogin, hist, WorkflowStatus.Cancelled);
                              return RedirectToAction(ac, ctlr, new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resource.Timesheet_Expenses });
                           }
                        }
                        else if (action.Status == WorkflowStatus.Rejected)
                        {
                           exapp.Next_Approver = null;
                           result = tsexService.UpdateTsExStatus(tsex.TsEx_ID, null, WorkflowStatus.Cancellation_Rejected, includeapprove: true, exapprover: exapp);
                           if (result.Code == ERROR_CODE.SUCCESS)
                           {
                              uService.ExpireActivationByPrefix(Ac_Code);
                              sendEmpEmail(tsex, user, userlogin, hist, WorkflowStatus.Cancellation_Rejected);
                              return RedirectToAction(ac, ctlr, new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL_REJECT), Field = Resource.Timesheet_Expenses });
                           }
                        }
                        else
                        {
                           var mstr = "";
                           if (action.NextApprover == null)
                           {
                              #region Indent flow
                              var haveSendRequestEmail = false;
                              if (action.IndentValue != null && action.IndentValue.IsIndent && action.IndentValue.SendRequest)
                                 haveSendRequestEmail = true;

                              if (haveSendRequestEmail)
                              {
                                 exapp.Next_Approver = null;
                                 List<IndentItem> IndentItems = getIndentSupervisor(tsex.Expenses_Application_ID, tsex.Time_Sheet_ID);
                                 if (IndentItems != null && IndentItems.Count > 0)
                                 {
                                    foreach (var row in IndentItems)
                                    {

                                       if (action.Actioner_Profile_ID == row.Requestor_Profile_ID)
                                          continue;

                                       var param = new Dictionary<string, object>();
                                       param.Add("expID", ex.Expenses_Application_ID);
                                       param.Add("appID", row.Requestor_Profile_ID);
                                       param.Add("empID", ex.Employee_Profile_ID);
                                       param.Add("reqcancelID", action.Request_ID);
                                       param.Add("cancelStatus", WorkflowStatus.Cancelled);
                                       param.Add("code", uService.GenActivateCode("E" + ex.Expenses_Application_ID + row.Requestor_Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));
                                       var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                                       param["cancelStatus"] = WorkflowStatus.Rejected;
                                       var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                                       var appr = uService.getUser(row.Requestor_Profile_ID, false);
                                       if (appr != null)
                                          sendApprovalEmail(tsex, appr, user, hist, ex.Cancel_Status, null, linkApp, linkRej, row.Indent_No);

                                       mstr = getApprovalStrIDs(mstr, row.Requestor_Profile_ID.ToString());
                                    }
                                 }
                              }
                              else
                              {
                                 var str = getApprovalStrIDs(null, userlogin.Profile_ID.ToString());
                                 if (!string.IsNullOrEmpty(str))
                                    mstr = ex.Next_Approver.Replace(str, "|");
                              }
                              #endregion
                           }
                           else
                           {
                              #region Normal flow
                              var param = new Dictionary<string, object>();
                              param.Add("expID", ex.Expenses_Application_ID);
                              param.Add("appID", action.NextApprover.Profile_ID);
                              param.Add("empID", ex.Employee_Profile_ID);
                              param.Add("reqcancelID", action.Request_ID);
                              param.Add("cancelStatus", WorkflowStatus.Cancelled);
                              param.Add("code", uService.GenActivateCode("E" + ex.Expenses_Application_ID + action.NextApprover.Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));
                              var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                              param["cancelStatus"] = WorkflowStatus.Rejected;
                              var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                              var appr = uService.getUser(action.NextApprover.Profile_ID, false);
                              if (appr != null)
                                 sendApprovalEmail(tsex, appr, user, hist, ex.Cancel_Status, null, linkApp, linkRej);

                              if (action.NextApprover.Profile_ID != userlogin.Profile_ID)
                                 mstr = getApprovalStrIDs(mstr, action.NextApprover.Profile_ID.ToString());
                              #endregion
                           }

                           if (!string.IsNullOrEmpty(mstr))
                              exapp.Next_Approver = mstr;
                           result = tsexService.UpdateTsExStatus(tsex.TsEx_ID, includeapprove: true, exapprover: exapp);

                           uService.ExpireActivationByPrefix(Ac_Code);
                           return RedirectToAction(ac, ctlr, new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resource.Timesheet_Expenses });
                        }
                     }
                  }
                  #endregion
               }
               else if (hist.Supervisor.HasValue)
               {
                  #region Supervisor
                  var exapp = new ExApprover();
                  exapp.Next_Approver = null;
                  exapp.Approver = getApprovalStrIDs(null, userlogin.Profile_ID.ToString());

                  /*cancel approval by supervisor*/
                  if (status == WorkflowStatus.Approved)
                     result = tsexService.UpdateTsExStatus(tsex.TsEx_ID, null, WorkflowStatus.Cancelled, includeapprove: true, exapprover: exapp);
                  else
                     result = tsexService.UpdateTsExStatus(tsex.TsEx_ID, null, WorkflowStatus.Cancellation_Rejected, includeapprove: true, exapprover: exapp);

                  if (result.Code == ERROR_CODE.SUCCESS)
                  {
                     uService.ExpireActivationByPrefix(Ac_Code);
                     sendEmpEmail(tsex, user, userlogin, hist, ex.Cancel_Status);
                     if (status == WorkflowStatus.Approved)
                        return RedirectToAction(ac, ctlr, new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL), Field = Resource.Timesheet_Expenses });
                     else
                        return RedirectToAction(ac, ctlr, new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL_REJECT), Field = Resource.Timesheet_Expenses });
                  }
                  #endregion
               }
            }
         }

         return RedirectToAction(ac, ctlr, new RouteValueDictionary(result));
      }

      public void sendEmpEmail(TsEX tsex, User_Profile sentto, User_Profile receivedfrom, Employment_History receivedhist, string Overall_Status, List<Reviewer> Reviewers = null, string approverName = "")
      {
         var ecode = "[PTSEX" + tsex.TsEx_ID + "_";
         if (tsex.Expenses_Application.Request_Cancel_ID.HasValue)
            ecode += "RQC" + tsex.Expenses_Application.Request_Cancel_ID;
         else if (tsex.Expenses_Application.Request_ID.HasValue)
            ecode += "RQ" + tsex.Expenses_Application.Request_ID;

         ecode += "S" + receivedfrom.User_Authentication.User_Authentication_ID + "R" + sentto.User_Authentication.User_Authentication_ID + "]";

         var SmartDevPdfRequest = getFileExpenseSmartDevPdf(EncryptUtil.Encrypt(tsex.Expenses_Application.Expenses_Application_ID), EncryptUtil.Encrypt(UserSession.RIGHT_U), tsex.Employee_Profile_ID);
         if (string.IsNullOrEmpty(approverName))
            approverName = UserSession.GetUserName(receivedfrom);

         var cService = new CompanyService();
         var com = cService.GetCompany(tsex.Company_ID);
         if (com == null)
            return;

         var eitem = new EmailItem()
         {
            Doc_ID = tsex.TsEx_ID,
            LogoLink = GenerateLogoLink(),
            Company = com,
            Send_To_Email = sentto.User_Authentication.Email_Address,
            Send_To_Name = AppConst.GetUserName(sentto),
            Received_From_Email = receivedfrom.User_Authentication.Email_Address,
            Received_From_Name = AppConst.GetUserName(receivedfrom),
            Received_From_Department = receivedhist.Department != null ? receivedhist.Department.Name : "",
            Module = ModuleCode.HR,
            Approval_Type = ApprovalType.Expense,
            Expenses = tsex.Expenses_Application,
            Timesheet = tsex.Time_Sheet,
            Status = Overall_Status,
            Reviewer = Reviewers,
            Url = Request.Url.AbsoluteUri,
            ECode = ecode,
            Attachment_SmartDev = SmartDevPdfRequest,
            Approver_Name = approverName,
         };

         var files = new List<FileAttach>();
         if (eitem.Attachment_SmartDev != null)
            files.Add(eitem.Attachment_SmartDev);

         foreach (var frow in eitem.Expenses.Expenses_Application_Document)
         {
            if (frow.Upload_Receipt != null && frow.Upload_Receipt.Count > 0)
            {
               foreach (var img in frow.Upload_Receipt)
               {
                  MemoryStream ms = new MemoryStream(img.Receipt);
                  files.Add(new FileAttach { File = ms, File_Name = img.File_Name });
               }
            }
         }

         var subject = ecode + " " + eitem.Approval_Type + " " + eitem.Status + " Application";
         var message = RenderPartialViewAsString("_EmailTsExEmp", eitem);
         EmailTemplete.NewEmailNotification(eitem.Send_To_Email, subject, message, eitem.Reviewer, files, eitem.Url, eitem.Approval_Type, eitem.Doc_ID);
      }

      public void sendApprovalEmail(TsEX tsex, User_Profile sentto, User_Profile receivedfrom, Employment_History receivedhist, string Overall_Status, List<Reviewer> Reviewers = null, string linkApp = null, string linkRej = null, string indentno=null)
      {
         /*sentto is approval*/
         /*receivedfrom is emp*/
         var ecode = "[RE" + tsex.Expenses_Application.Expenses_Application_ID + "_";
         if (tsex.Expenses_Application.Request_Cancel_ID.HasValue)
            ecode += "RQC" + tsex.Expenses_Application.Request_Cancel_ID;
         else if (tsex.Expenses_Application.Request_ID.HasValue)
            ecode += "RQ" + tsex.Expenses_Application.Request_ID;

         ecode += "S" + receivedfrom.User_Authentication.User_Authentication_ID + "R" + sentto.User_Authentication.User_Authentication_ID + "]";

         var cService = new CompanyService();
         var com = cService.GetCompany(tsex.Company_ID);
         if (com == null)
            return;

         var SmartDevPdfRequest = getFileExpenseSmartDevPdf(EncryptUtil.Encrypt(tsex.Expenses_Application_ID), EncryptUtil.Encrypt(UserSession.RIGHT_U), tsex.Employee_Profile_ID);
         var eitem = new EmailItem()
         {
            Doc_ID = tsex.TsEx_ID,
            LogoLink = GenerateLogoLink(),
            Company = com,
            Send_To_Email = sentto.User_Authentication.Email_Address,
            Send_To_Name = AppConst.GetUserName(sentto),
            Received_From_Email = receivedfrom.User_Authentication.Email_Address,
            Received_From_Name = AppConst.GetUserName(receivedfrom),
            Received_From_Department = receivedhist.Department != null ? receivedhist.Department.Name : "",
            Module = ModuleCode.HR,
            Approval_Type = ApprovalType.Expense,
            Expenses = tsex.Expenses_Application,
            Timesheet = tsex.Time_Sheet,
            Status = Overall_Status,
            Reviewer = Reviewers,
            Link = linkApp,
            Link2 = linkRej,
            Url = Request.Url.AbsoluteUri,
            ECode = ecode,
            Attachment_SmartDev = SmartDevPdfRequest
         };

         var files = new List<FileAttach>();
         if (eitem.Attachment_SmartDev != null)
            files.Add(eitem.Attachment_SmartDev);

         foreach (var frow in eitem.Expenses.Expenses_Application_Document)
         {
            if (frow.Upload_Receipt != null && frow.Upload_Receipt.Count > 0)
            {
               foreach (var img in frow.Upload_Receipt)
               {
                  MemoryStream ms = new MemoryStream(img.Receipt);
                  files.Add(new FileAttach { File = ms, File_Name = img.File_Name });
               }
            }
         }
         var subject = "";
         if (!string.IsNullOrEmpty(indentno))
            subject = "Indent " + indentno + " Pending Approval";
         else
            subject = eitem.Approval_Type + " Pending Approval";
        
         if (eitem.Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Canceling)
            subject += " Cancellation";

         var message = RenderPartialViewAsString("_EmalTsExApproval", eitem);
         EmailTemplete.NewEmailNotification(eitem.Send_To_Email, subject, message, eitem.Reviewer, files, eitem.Url, eitem.Approval_Type, eitem.Doc_ID);
      }


      public string getApprovalStrIDs(string CurrentIDs, string strApprovalIDs)
      {
         if (!string.IsNullOrEmpty(CurrentIDs))
         {
            if (!string.IsNullOrEmpty(strApprovalIDs))
            {
               strApprovalIDs = CurrentIDs + strApprovalIDs + "|";
            }
            else
            {
               strApprovalIDs = CurrentIDs;
            }
         }
         else if (!string.IsNullOrEmpty(strApprovalIDs))
         {
            strApprovalIDs = "|" + strApprovalIDs + "|";
         }
         return strApprovalIDs;
      }
      #endregion
   }
}
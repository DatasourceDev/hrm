using CivinTecAccessManager;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SBSModel.Common;
using SBSModel.Models;
using SBSResourceAPI;
using SBSTimeModel.Common;
using SBSTimeModel.Models;
using SBSWorkFlowAPI.Constants;
using SBSWorkFlowAPI.Models;
using SBSWorkFlowAPI.ModelsAndService;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Time.Models;

namespace Time.Controllers
{
   [Authorize]
   public class TimeSheetController : ControllerBase
   {

      #region Configuration
      [HttpGet]
      public ActionResult Configuration(ServiceResult result, TimeSheetConfigurationViewModel model, int[] timeSteets, string apply, string tabAction = "")
      {
         var userlogin = UserUtil.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //Validate Page Right
         var rightResult = base.validatePageRight(Operation.A);
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;
         model.result = result;
         model.tabAction = tabAction;

         var cbService = new ComboService();
         var aService = new SBSWorkFlowAPI.Service();

         if (tabAction == "approval")
         {
            if (apply == Operation.D)
            {
               if (timeSteets != null)
               {
                  var chkProblem = false;
                  //check first is there any wrong records before delete!
                  foreach (var approval in timeSteets)
                  {
                     var rwflow = aService.GetWorkflow(approval);
                     if (rwflow.Item2.IsSuccess && rwflow.Item1 != null)
                     {
                        if (rwflow.Item1.Requests.Count() > 0)
                        {
                           return RedirectToAction("Configuration", new { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Approval, tabAction = "approval" });
                        }
                     }
                  }
                  foreach (var approval in timeSteets)
                  {
                     var r = aService.UpdateDeleteWorkFlowStatus(approval, userlogin.Profile_ID, apply);
                     if (!r.IsSuccess)
                     {
                        chkProblem = true;
                        break;
                     }
                  }
                  if (chkProblem)
                     return RedirectToAction("Configuration", new { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Approval, tabAction = "approval" });
                  else
                     return RedirectToAction("Configuration", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Approval, tabAction = "approval" });
               }
            }
         }

         if (model.Approval_Department.HasValue)
         {
            var r = aService.GetWorkflowByDepartment(userlogin.Company_ID.Value, model.Approval_Department.Value, ModuleCode.Time, ApprovalType.TimeSheet);
            if (r.Item2.IsSuccess && r.Item1 != null)
            {
               model.ApprovalList = r.Item1;
            }
         }
         else
         {
            var r = aService.GetWorkflowByCompany(userlogin.Company_ID.Value, ModuleCode.Time, ApprovalType.TimeSheet);
            if (r.Item2.IsSuccess && r.Item1 != null)
            {
               model.ApprovalList = r.Item1;
            }
         }

         //filter
         model.departmentList = cbService.LstDepartment(userlogin.Company_ID, true);

         return View(model);
      }

      [HttpPost]
      [ValidateAntiForgeryToken]
      public ActionResult Configuration(TimeSheetConfigurationViewModel model, string tabAction = "")
      {
         var userlogin = UserUtil.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         if (ModelState.IsValid)
         {
         }

         var rightResult = base.validatePageRight(Operation.A);
         if (rightResult.action != null)
            return rightResult.action;
         model.rights = rightResult.rights;
         model.tabAction = tabAction;

         return View(model);
      }
      #endregion

      #region Time Sheet
      [HttpGet]
      [AllowAuthorized]
      public ActionResult Record(ServiceResult result, TimeSheetViewModel model)
      {

         var currentdate = StoredProcedure.GetCurrentDate();
         var userlogin = UserUtil.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //Validate Page Right
         var rightResult = base.validatePageRight(Operation.A);
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;
         model.result = result;

         var tsService = new TimeSheetService();
         var cbService = new ComboService();

         if (!string.IsNullOrEmpty(model.Date_From) && !string.IsNullOrEmpty(model.Date_To))
         {
            if (DateUtil.ToDate(model.Date_To) < DateUtil.ToDate(model.Date_From))
            {
               ModelState.AddModelError("Date_From", Resource.Message_Is_Invalid);
               ModelState.AddModelError("Date_To", Resource.Message_Is_Invalid);
            }
         }

         var criteria = new TimeSheetCriteria()
         {
            Company_ID = userlogin.Company_ID,
            Time_Sheet_ID = model.Time_Sheet_ID,
            Date_To = model.Date_To,
            Date_From = model.Date_From,
            Overall_Status = model.Search_Time_Sheet_Status,
            Employee_Profile_ID = userlogin.Employee_Profile.Select(s => s.Employee_Profile_ID).FirstOrDefault(),
            IncludeDraft = true
         };
         model.TimeSheetList = tsService.LstTimeSheet(criteria);

         //filter
         model.WFStatuslst = cbService.LstApprovalStatus(true);

         return View(model);
      }

      [HttpGet]
      [AllowAuthorized]
      public ActionResult TimeSheetManagement(ServiceResult result, TimeSheetViewModel model)
      {

         var currentdate = StoredProcedure.GetCurrentDate();
         var userlogin = UserUtil.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //Validate Page Right
         var rightResult = base.validatePageRight(Operation.A);
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;
         model.result = result;

         var tsService = new TimeSheetService();
         var cbService = new ComboService();
         var EmpService = new EmployeeService();
         var criteria = new TimeSheetCriteria()
         {
            Company_ID = userlogin.Company_ID,
         };
         model.TimeSheetList = tsService.LstTimeSheet(criteria);

         //filter
         var emp = new List<Employee_Profile>();
         var criteria2 = new EmployeeCriteria() { Company_ID = userlogin.Company_ID };
         var pResult2 = EmpService.LstEmployeeProfile(criteria2);
         if (pResult2.Object != null) emp = (List<Employee_Profile>)pResult2.Object;
         model.EmployeeList = emp;

         return View(model);
      }


       [HttpPost]
      [AllowAuthorized]
      public ActionResult TimeSheetManagement(int[] tIds, string pStatus)
      {
         if (tIds != null && !string.IsNullOrEmpty(pStatus))
         {
            var tService = new TimeSheetService();
            foreach(var tid in tIds)
            {
               var model = new TimeSheetViewModel();
               model.Time_Sheet_ID = tid;
               ApplicationMngt(model, pStatus);
            }
          
         }
         return RedirectToAction("TimeSheetManagement");
      }

      [HttpGet]
      [AllowAuthorized]
      public ActionResult Application(ServiceResult result, string operation, string tsID, string pAppr = "", string tabAction = "")
      {

         var currentdate = StoredProcedure.GetCurrentDate();
         var userlogin = UserUtil.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var model = new TimeSheetViewModel();
         model.operation = EncryptUtil.Decrypt(operation);
         if (string.IsNullOrEmpty(model.operation))
            model.operation = Operation.C;

         model.Time_Sheet_ID = NumUtil.ParseInteger(EncryptUtil.Decrypt(tsID));
         model.ApprStatus = EncryptUtil.Decrypt(pAppr);
         model.tabAction = tabAction;

         //Validate Page Right
         var rightResult = base.validatePageRight(Operation.A);
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;
         model.result = result;

         var cbService = new ComboService();
         var tsService = new TimeSheetService(userlogin);
         var empService = new EmployeeService();
         var hService = new EmploymentHistoryService();

         var emp = empService.GetEmployeeProfileByProfileID(userlogin.Profile_ID);
         if (emp == null)
            return errorPage(ERROR_CODE.ERROR_16_NO_EMPLOYEE_PROFILE);

         var com = new CompanyService().GetCompany(userlogin.Company_ID.Value);
         if (com != null && !com.Currency_ID.HasValue)
            return errorPage(ERROR_CODE.ERROR_13_NO_DEFAULT_CURRENCY);

         var userhist = hService.GetCurrentEmploymentHistory(emp.Employee_Profile_ID);
         if (userhist == null)
            return errorPage(ERROR_CODE.ERROR_14_NO_EMPLOYEE_HIST);

         model.Employee_Profile_ID = emp.Employee_Profile_ID;
         model.Date_Of_Date = DateUtil.ToDisplayDate(currentdate);
         model.isRejectPopUp = false;
         if (model.Time_Sheet_ID > 0 & model.operation == Operation.U)
         {
            var timeSheet = new Time_Sheet();
            timeSheet = tsService.GetTimeSheet(model.Time_Sheet_ID);
            if (timeSheet == null)
               return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

            model.Time_Sheet_ID = timeSheet.Time_Sheet_ID;
            model.Date_Of_Date = DateUtil.ToDisplayDate(timeSheet.Date_Of_Date);
            model.Clock_In = DateUtil.ToDisplayTime(timeSheet.Clock_In);
            model.Clock_Out = DateUtil.ToDisplayTime(timeSheet.Clock_Out);
            model.Job_Cost_ID = timeSheet.Job_Cost_ID;
            model.Note = timeSheet.Note;
            model.Company_ID = timeSheet.Company_ID;
            model.Overall_Status = timeSheet.Overall_Status;
            model.Record_Status = timeSheet.Record_Status;
            model.Cancel_Status = timeSheet.Cancel_Status;
            model.Request_ID = timeSheet.Request_ID;
            model.Request_Cancel_ID = timeSheet.Request_Cancel_ID;

            //******** Start Workflow Draft  ********//
            if (model.Overall_Status == WorkflowStatus.Draft)
               model.ApprStatus = WorkflowStatus.Draft;
            //******** End Workflow Draft  ********//

            if (timeSheet.Request_ID.HasValue)
            {
               var aService = new SBSWorkFlowAPI.Service();
               var r = aService.GetMyRequests(userlogin.Company_ID, null, null, ModuleCode.Time, ApprovalType.TimeSheet, timeSheet.Time_Sheet_ID);
               if (r.Item2.IsSuccess && r.Item1 != null)
                  model.Time_Sheet_Request = r.Item1;
            }

            if (timeSheet.Supervisor.HasValue)
            {
               model.Supervisor = timeSheet.Supervisor;
               var sup = empService.GetEmployeeProfile2(timeSheet.Supervisor);
               if (sup != null)
                  model.Supervisor_Name = AppConst.GetUserName(sup.User_Profile);
            }
         }
         else
         {
            //******** Start Workflow Draft  ********//
            model.ApprStatus = WorkflowStatus.Draft;
            //******** End Workflow Draft  ********//
         }

         model.JobCostlst = cbService.LstJobCost(userlogin.Company_ID, false);

         return View(model);
      }

      [HttpPost]
      [ValidateAntiForgeryToken]
      public ActionResult Application(TimeSheetViewModel model, string pStatus)
      {
         if (model.ApprStatus == "Manage")
            return ApplicationMngt(model, pStatus);
         else
            return ApplicationNew(model, pStatus);
      }

      private ActionResult ApplicationNew(TimeSheetViewModel model, string pStatus)
      {
         var userlogin = UserUtil.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var currentdate = StoredProcedure.GetCurrentDate();
         var tsService = new TimeSheetService(userlogin);
         var cbService = new ComboService();
         var hService = new EmploymentHistoryService();
         var cpService = new CompanyService();
         var empService = new EmployeeService();
         var aService = new SBSWorkFlowAPI.Service();
         var uService = new UserService();
         var wService = new WorkingDaysService();
         var jService = new JobCostService();

         var emp = userlogin.Employee_Profile.FirstOrDefault();
         if (emp == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Employee);

         var hist = hService.GetCurrentEmploymentHistory(emp.Employee_Profile_ID);
         if (hist == null)
            return errorPage(ERROR_CODE.ERROR_14_NO_EMPLOYEE_HIST);

         var com = cpService.GetCompany(userlogin.Company_ID);
         if (com == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Company);

         var wk = wService.GetWorkingDay(userlogin.Company_ID);
         if (wk == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Working_Hours);

         //Working_Days wk = getWorkDays(emp.Profile_ID);
         //if (wk == null)
         //   return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Working_Hours);

         if (pStatus != WorkflowStatus.Cancelled)
         {
            if (string.IsNullOrEmpty(model.Clock_In))
               ModelState.AddModelError("Clock_In", Resource.Message_Is_Required);

            if (string.IsNullOrEmpty(model.Clock_Out))
               ModelState.AddModelError("Clock_Out", Resource.Message_Is_Required);

            /* create new time Sheet by employee*/
            if (!string.IsNullOrEmpty(model.Clock_In) && !string.IsNullOrEmpty(model.Clock_Out))
            {
               if (DateUtil.ToTime(model.Clock_Out) <= DateUtil.ToTime(model.Clock_In))
               {
                  ModelState.AddModelError("Clock_In", Resource.The + " " + Resource.Clock_In + " " + Resource.Field + " " + Resource.Cannot_Be_More_Than_Lower + " " + Resource.The + " " + Resource.Clock_Out);
                  ModelState.AddModelError("Clock_Out", Resource.The + " " + Resource.Clock_Out + " " + Resource.Field + " " + Resource.Cannot_Be_Less_Than_Lower + " " + Resource.The + " " + Resource.Clock_In);
               }
            }

            //Validate Time Sheet
            if (ModelState.IsValid)
            {
               var criteria = new TimeSheetCriteria()
               {
                  Company_ID = userlogin.Company_ID,
                  Employee_Profile_ID = model.Employee_Profile_ID,
                  ValidateDup = true,
                  Date_Of_Date = model.Date_Of_Date,

               };

               var duplstts = tsService.LstTimeSheet(criteria);
               if (duplstts != null && duplstts.Count() > 0)
               {
                  var timeIn = DateUtil.ToTime(model.Clock_In);
                  var timeOut = DateUtil.ToTime(model.Clock_Out);
                  if (!string.IsNullOrEmpty(model.Clock_In) && !string.IsNullOrEmpty(model.Clock_Out))
                  {
                     var reange = duplstts.Where(w => w.Clock_In <= timeOut && w.Clock_Out >= timeIn).ToList();
                     if (reange != null && reange.Count() > 0)
                     {
                        ModelState.AddModelError("Clock_In", Resource.Message_The_Time_Range_Is_Duplicate);
                        ModelState.AddModelError("Clock_Out", Resource.Message_The_Time_Range_Is_Duplicate);
                     }
                  }
               }

               TimeSpan lunchduration = DateUtil.ToTime("00:00").Value;
               var clkin = DateUtil.ToTime(model.Clock_In);
               var clkout = DateUtil.ToTime(model.Clock_Out);
               TimeSpan duration = clkout.Value.Subtract(clkin.Value);
               if (!wk.CL_Lunch.HasValue || wk.CL_Lunch.Value == false)
               {
                  if (clkin < wk.ST_Lunch_Time && clkout <= wk.ST_Lunch_Time)
                  {
                  }
                  else if (clkin < wk.ST_Lunch_Time && clkout > wk.ST_Lunch_Time && clkout <= wk.ET_Lunch_Time)
                  {
                     lunchduration = clkout.Value.Subtract(wk.ST_Lunch_Time.Value);
                     duration = duration - lunchduration;
                  }
                  else if (clkin < wk.ST_Lunch_Time && clkout > wk.ET_Lunch_Time)
                  {
                     lunchduration = wk.ET_Lunch_Time.Value.Subtract(wk.ST_Lunch_Time.Value);
                     duration = duration - lunchduration;
                  }
                  else if (clkin >= wk.ST_Lunch_Time && clkout <= wk.ET_Lunch_Time)
                  {
                     ModelState.AddModelError("Clock_In", Resource.Message_The_Time_Range_Is_Invaid);
                     ModelState.AddModelError("Clock_Out", Resource.Message_The_Time_Range_Is_Invaid);
                  }
                  else if (clkin >= wk.ST_Lunch_Time && clkin < wk.ET_Lunch_Time && clkout > wk.ET_Lunch_Time)
                  {
                     lunchduration = wk.ET_Lunch_Time.Value.Subtract(clkin.Value);
                     duration = duration - lunchduration;
                  }
                  else if (clkin >= wk.ET_Lunch_Time)
                  {
                  }
               }

               model.PageStatus = null;
               if (ModelState.IsValid)
               {
                  var job = jService.GetJobCost(model.Job_Cost_ID);
                  if (job == null)
                     return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Job_Cost);

                  var timeSheet = new Time_Sheet();
                  if (model.operation == Operation.U)
                  {
                     timeSheet = tsService.GetTimeSheet(model.Time_Sheet_ID);
                     if (timeSheet == null)
                        return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Expenses);
                  }

                  timeSheet.Time_Sheet_ID = model.Time_Sheet_ID.HasValue ? model.Time_Sheet_ID.Value : 0;
                  timeSheet.Clock_In = DateUtil.ToTime(model.Clock_In);
                  timeSheet.Clock_Out = DateUtil.ToTime(model.Clock_Out);
                  timeSheet.Date_Of_Date = DateUtil.ToDate(model.Date_Of_Date);
                  timeSheet.Job_Cost_ID = model.Job_Cost_ID;
                  timeSheet.Update_By = userlogin.User_Authentication.Email_Address;
                  timeSheet.Update_On = currentdate;
                  timeSheet.Employee_Profile_ID = model.Employee_Profile_ID;
                  timeSheet.Supervisor = hist.Supervisor;
                  timeSheet.Note = model.Note;
                  timeSheet.Company_ID = userlogin.Company_ID;
                  // static data 
                  timeSheet.Employee_Name = AppConst.GetUserName(userlogin);
                  timeSheet.Indent_No = job.Indent_No;
                  timeSheet.Indent_Name = job.Indent_Name;
                  timeSheet.Customer_Name = (job.Customer_ID.HasValue && job.Customer != null ? job.Customer.Customer_Name : "");
                  timeSheet.Duration = duration;
                  timeSheet.Launch_Duration = lunchduration;

                  //******** Start Workflow Draft  ********//
                  if (pStatus == WorkflowStatus.Draft)
                     timeSheet.Overall_Status = WorkflowStatus.Draft;
                  else
                     timeSheet.Overall_Status = WorkflowStatus.Pending;
                  //******** End Workflow Draft  ********//

                  if (hist.Basic_Salary_Unit == Term.Hourly)
                  {
                     timeSheet.Hour_Rate = NumUtil.ParseDecimal(EncryptUtil.Decrypt(hist.Basic_Salary));
                     if (timeSheet.Hour_Rate == 0)
                        timeSheet.Hour_Rate = NumUtil.ParseDecimal(EncryptUtil.Decrypt(EncryptUtil.Decrypt(hist.Basic_Salary)));

                     var totalamount = duration.Hours * timeSheet.Hour_Rate;
                     var minRate = timeSheet.Hour_Rate / 60;
                     if (duration.Minutes > 0)
                        totalamount += (duration.Minutes * minRate);

                     timeSheet.Total_Amount = totalamount;
                  }

                  var haveApprover = true;
                  var rworkflow = aService.GetWorkflowByEmployee(userlogin.Company_ID.Value, userlogin.Profile_ID, ModuleCode.Time, ApprovalType.TimeSheet, hist.Department_ID);
                  if (!rworkflow.Item2.IsSuccess || rworkflow.Item1 == null || rworkflow.Item1.Count == 0)
                     haveApprover = false;

                  if (model.operation == Operation.C)
                  {
                     #region Create
                     timeSheet.Create_By = userlogin.User_Authentication.Email_Address;
                     timeSheet.Create_On = currentdate;
                     model.result = tsService.InsertTimeSheet(timeSheet);
                     if (model.result.Code == ERROR_CODE.SUCCESS)
                     {
                        if (pStatus != WorkflowStatus.Draft)
                        {
                           if (!job.Using.HasValue || (job.Using.HasValue && !job.Using.Value))
                           {
                              job.Using = true;
                              job.Update_By = userlogin.User_Authentication.Email_Address;
                              job.Update_On = currentdate;
                              model.result = jService.UpdateJobCost(job);
                           }

                           var ts = tsService.GetTimeSheet(timeSheet.Time_Sheet_ID);
                           if (ts != null)
                           {
                              ts.Update_By = userlogin.User_Authentication.Email_Address;
                              ts.Update_On = currentdate;
                              if (haveApprover)
                              {
                                 #region Workfolw
                                 var request = new RequestItem();
                                 request.Doc_ID = ts.Time_Sheet_ID;
                                 request.Approval_Type = ApprovalType.TimeSheet;
                                 request.Company_ID = userlogin.Company_ID.Value;
                                 request.Department_ID = hist.Department_ID.HasValue ? hist.Department_ID.Value : 0;
                                 request.Module = ModuleCode.Time;
                                 request.Requestor_Email = userlogin.User_Authentication.Email_Address;
                                 request.Requestor_Name = AppConst.GetUserName(userlogin);
                                 request.Requestor_Profile_ID = userlogin.Profile_ID;

                                 if (com.Is_Indent.HasValue && com.Is_Indent.Value)
                                 {
                                    request.IndentItems = getIndentSupervisor(ts.Job_Cost_ID.HasValue ? ts.Job_Cost_ID.Value : 0);
                                    if (request.IndentItems != null && request.IndentItems.Count > 0)
                                       request.Is_Indent = true;
                                 }

                                 var r = aService.SubmitRequest(request);
                                 if (r.IsSuccess)
                                 {
                                    ts.Supervisor = null;
                                    ts.Request_ID = request.Request_ID;
                                    ts.Overall_Status = request.Status;
                                    model.result = tsService.UpdateTimeSheet(ts);
                                    if (model.result.Code == ERROR_CODE.SUCCESS)
                                    {
                                       if (request.Status == WorkflowStatus.Closed)
                                       {
                                          jService.CalCosting(ts.Job_Cost_ID, ts.Total_Amount);
                                          sendProceedEmail(ts, com, userlogin, userlogin, hist, request.Status, request.Reviewers);
                                       }
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
                                                   param.Add("tsID", ts.Time_Sheet_ID);
                                                   param.Add("appID", row.Requestor_Profile_ID);
                                                   param.Add("empID", ts.Employee_Profile_ID);
                                                   param.Add("reqID", ts.Request_ID);
                                                   param.Add("status", WorkflowStatus.Approved);
                                                   param.Add("code", uService.GenActivateCode("T" + ts.Time_Sheet_ID + row.Requestor_Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

                                                   var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                                                   param["status"] = WorkflowStatus.Rejected;
                                                   var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                                                   var appr = uService.getUser(row.Requestor_Profile_ID, false);
                                                   if (appr != null)
                                                   {
                                                      if (ai != 0)
                                                         approverName += " , ";


                                                      approverName += AppConst.GetUserName(appr);
                                                      sendRequestEmail(ts, com, appr, userlogin, hist, request.Status, request.Reviewers, linkApp, linkRej);
                                                   }
                                                }
                                                else
                                                {
                                                   var appr = uService.getUser(row.Requestor_Profile_ID, false);
                                                   if (appr != null)
                                                   {
                                                      if (ai != 0)
                                                         approverName += " , ";

                                                      approverName += AppConst.GetUserName(appr);
                                                   }
                                                }
                                                ai++;
                                             }
                                             sendProceedEmail(ts, com, userlogin, userlogin, hist, WorkflowStatus.Submitted, request.Reviewers, approverName);
                                             #endregion
                                          }
                                          else
                                          {
                                             #region Normal flow
                                             var param = new Dictionary<string, object>();
                                             param.Add("tsID", ts.Time_Sheet_ID);
                                             param.Add("appID", request.NextApprover.Profile_ID);
                                             param.Add("empID", ts.Employee_Profile_ID);
                                             param.Add("reqID", ts.Request_ID);
                                             param.Add("status", WorkflowStatus.Approved);
                                             param.Add("code", uService.GenActivateCode("T" + ts.Time_Sheet_ID + request.NextApprover.Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

                                             var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                                             param["status"] = WorkflowStatus.Rejected;
                                             var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                                             var appr = uService.getUser(request.NextApprover.Profile_ID, false);
                                             if (appr != null)
                                             {
                                                approverName += AppConst.GetUserName(appr);
                                                sendRequestEmail(ts, com, appr, userlogin, hist, request.Status, request.Reviewers, linkApp, linkRej);
                                             }

                                             sendProceedEmail(ts, com, userlogin, userlogin, hist, WorkflowStatus.Submitted, request.Reviewers, approverName);
                                             #endregion
                                          }
                                       }
                                    }
                                    return RedirectToAction("Record", new { Code = model.result.Code, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS), Field = model.result.Field });
                                 }
                                 #endregion
                              }
                              else
                              {
                                 if (hist.Supervisor.HasValue)
                                 {
                                    #region Supervisor folw
                                    var sup = empService.GetEmployeeProfile2(hist.Supervisor);
                                    if (sup != null)
                                    {
                                       var param = new Dictionary<string, object>();
                                       param.Add("tsID", ts.Time_Sheet_ID);
                                       param.Add("appID", sup.Profile_ID);
                                       param.Add("empID", ts.Employee_Profile_ID);
                                       param.Add("status", WorkflowStatus.Approved);
                                       param.Add("code", uService.GenActivateCode("T" + ts.Time_Sheet_ID + sup.Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

                                       var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                                       param["status"] = WorkflowStatus.Rejected;
                                       var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                                       sendProceedEmail(ts, com, userlogin, userlogin, hist, WorkflowStatus.Submitted, null, AppConst.GetUserName(sup.User_Profile));
                                       sendRequestEmail(ts, com, sup.User_Profile, userlogin, hist, ts.Overall_Status, null, linkApp, linkRej);
                                       return RedirectToAction("Record", new { Code = model.result.Code, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS), Field = model.result.Field });
                                    }
                                    #endregion
                                 }
                                 else
                                 {
                                    #region not folw
                                    ts.Overall_Status = WorkflowStatus.Closed;
                                    model.result = tsService.UpdateTimeSheet(ts);
                                    if (model.result.Code == ERROR_CODE.SUCCESS)
                                    {
                                       jService.CalCosting(ts.Job_Cost_ID, ts.Total_Amount);
                                       sendProceedEmail(ts, com, userlogin, userlogin, hist, ts.Overall_Status, null);
                                    }
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
                     #endregion
                  }
                  else if (model.operation == Operation.U)
                  {
                     #region Update
                     model.result = tsService.UpdateTimeSheet(timeSheet);
                     if (model.result.Code == ERROR_CODE.SUCCESS)
                     {
                        if (pStatus != WorkflowStatus.Draft)
                        {
                           job.Using = true;
                           job.Update_By = userlogin.User_Authentication.Email_Address;
                           job.Update_On = currentdate;
                           model.result = jService.UpdateJobCost(job);

                           var ts = tsService.GetTimeSheet(timeSheet.Time_Sheet_ID);
                           if (ts != null)
                           {
                              ts.Update_By = userlogin.User_Authentication.Email_Address;
                              ts.Update_On = currentdate;
                              if (haveApprover)
                              {
                                 #region workflow
                                 var request = new RequestItem();
                                 request.Doc_ID = ts.Time_Sheet_ID;
                                 request.Approval_Type = ApprovalType.TimeSheet;
                                 request.Company_ID = userlogin.Company_ID.Value;
                                 request.Department_ID = hist.Department_ID.HasValue ? hist.Department_ID.Value : 0;
                                 request.Module = ModuleCode.Time;
                                 request.Requestor_Email = userlogin.User_Authentication.Email_Address;
                                 request.Requestor_Name = AppConst.GetUserName(userlogin);
                                 request.Requestor_Profile_ID = userlogin.Profile_ID;

                                 if (com.Is_Indent.HasValue && com.Is_Indent.Value)
                                 {
                                    request.IndentItems = getIndentSupervisor(ts.Job_Cost_ID.HasValue ? ts.Job_Cost_ID.Value : 0);
                                    if (request.IndentItems != null && request.IndentItems.Count > 0)
                                       request.Is_Indent = true;
                                 }

                                 var r = aService.SubmitRequest(request);
                                 if (r.IsSuccess)
                                 {
                                    ts.Supervisor = null;
                                    ts.Request_ID = request.Request_ID;
                                    ts.Overall_Status = request.Status;
                                    model.result = tsService.UpdateTimeSheet(ts);
                                    if (model.result.Code == ERROR_CODE.SUCCESS)
                                    {
                                       if (request.Status == WorkflowStatus.Closed)
                                       {
                                          jService.CalCosting(ts.Job_Cost_ID, ts.Total_Amount);
                                          sendProceedEmail(ts, com, userlogin, userlogin, hist, request.Status, request.Reviewers);
                                       }
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
                                                   param.Add("tsID", ts.Time_Sheet_ID);
                                                   param.Add("appID", row.Requestor_Profile_ID);
                                                   param.Add("empID", ts.Employee_Profile_ID);
                                                   param.Add("reqID", ts.Request_ID);
                                                   param.Add("status", WorkflowStatus.Approved);
                                                   param.Add("code", uService.GenActivateCode("T" + ts.Time_Sheet_ID + row.Requestor_Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

                                                   var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                                                   param["status"] = WorkflowStatus.Rejected;
                                                   var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                                                   var appr = uService.getUser(row.Requestor_Profile_ID, false);
                                                   if (appr != null)
                                                   {
                                                      if (ai != 0)
                                                         approverName += " , ";

                                                      approverName += AppConst.GetUserName(appr);
                                                      sendRequestEmail(ts, com, appr, userlogin, hist, request.Status, request.Reviewers, linkApp, linkRej);
                                                   }
                                                }
                                                else
                                                {
                                                   var appr = uService.getUser(row.Requestor_Profile_ID, false);
                                                   if (appr != null)
                                                   {
                                                      if (ai != 0)
                                                         approverName += " , ";

                                                      approverName += AppConst.GetUserName(appr);
                                                   }
                                                }
                                                ai++;
                                             }
                                             sendProceedEmail(ts, com, userlogin, userlogin, hist, WorkflowStatus.Submitted, request.Reviewers, approverName);
                                             #endregion
                                          }
                                          else
                                          {
                                             #region Normal flow
                                             var param = new Dictionary<string, object>();
                                             param.Add("tsID", ts.Time_Sheet_ID);
                                             param.Add("appID", request.NextApprover.Profile_ID);
                                             param.Add("empID", ts.Employee_Profile_ID);
                                             param.Add("reqID", ts.Request_ID);
                                             param.Add("status", WorkflowStatus.Approved);
                                             param.Add("code", uService.GenActivateCode("T" + ts.Time_Sheet_ID + request.NextApprover.Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

                                             var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                                             param["status"] = WorkflowStatus.Rejected;
                                             var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);


                                             var appr = uService.getUser(request.NextApprover.Profile_ID, false);
                                             if (appr != null)
                                             {
                                                approverName = AppConst.GetUserName(appr);
                                                sendRequestEmail(ts, com, appr, userlogin, hist, request.Status, request.Reviewers, linkApp, linkRej);
                                             }

                                             sendProceedEmail(ts, com, userlogin, userlogin, hist, WorkflowStatus.Submitted, request.Reviewers, approverName);

                                             #endregion
                                          }
                                       }
                                    }
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
                                       var param = new Dictionary<string, object>();
                                       param.Add("tsID", ts.Time_Sheet_ID);
                                       param.Add("appID", sup.Profile_ID);
                                       param.Add("empID", ts.Employee_Profile_ID);
                                       param.Add("status", WorkflowStatus.Approved);
                                       param.Add("code", uService.GenActivateCode("T" + ts.Time_Sheet_ID + sup.Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

                                       var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                                       param["status"] = WorkflowStatus.Rejected;
                                       var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                                       sendProceedEmail(ts, com, userlogin, userlogin, hist, WorkflowStatus.Submitted, null, AppConst.GetUserName(sup.User_Profile));
                                       sendRequestEmail(ts, com, sup.User_Profile, userlogin, hist, ts.Overall_Status, null, linkApp, linkRej);
                                       return RedirectToAction("Record", new { Code = model.result.Code, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS), Field = model.result.Field });
                                    }
                                    #endregion
                                 }
                                 else
                                 {
                                    #region Not flow
                                    ts.Overall_Status = WorkflowStatus.Closed;
                                    model.result = tsService.UpdateTimeSheet(ts);
                                    if (model.result.Code == ERROR_CODE.SUCCESS)
                                    {
                                       jService.CalCosting(ts.Job_Cost_ID, ts.Total_Amount);
                                       sendProceedEmail(ts, com, userlogin, userlogin, hist, ts.Overall_Status, null);
                                    }
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
                     #endregion
                  }
               }
            }
         }
         else if (pStatus == WorkflowStatus.Cancelled)
         {
            var ts = tsService.GetTimeSheet(model.Time_Sheet_ID);
            if (ts == null)
               return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Time_Sheet);

            ts.Update_By = userlogin.User_Authentication.Email_Address;
            ts.Update_On = currentdate;

            /* cancel Time Sheet by employee*/
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
                     if (ts.Overall_Status == WorkflowStatus.Closed)
                     {
                        ts.Cancel_Status = WorkflowStatus.Canceling;
                        model.result = tsService.UpdateTimeSheet(ts);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                           var param = new Dictionary<string, object>();
                           param.Add("tsID", ts.Time_Sheet_ID);
                           param.Add("appID", sup.Profile_ID);
                           param.Add("empID", ts.Employee_Profile_ID);
                           param.Add("cancelStatus", WorkflowStatus.Cancelled);
                           param.Add("code", uService.GenActivateCode("T" + ts.Time_Sheet_ID + sup.Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));
                           var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                           param["cancelStatus"] = WorkflowStatus.Rejected;
                           var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                           sendProceedEmail(ts, com, userlogin, userlogin, hist, ts.Cancel_Status, null, AppConst.GetUserName(sup.User_Profile));
                           sendRequestEmail(ts, com, sup.User_Profile, userlogin, hist, ts.Cancel_Status, null, linkApp, linkRej);
                           return RedirectToAction("Record", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL), Field = Resource.Time_Sheet });
                        }
                     }
                     else
                     {
                        ts.Cancel_Status = WorkflowStatus.Cancelled;
                        model.result = tsService.UpdateTimeSheet(ts);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                           jService.CalCosting(ts.Job_Cost_ID, -1 * ts.Total_Amount);
                           sendProceedEmail(ts, com, userlogin, userlogin, hist, ts.Cancel_Status, null);
                           /*should send some email to appr*/
                           return RedirectToAction("Record", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL), Field = Resource.Time_Sheet });
                        }
                     }
                  }
                  #endregion
               }
               else
               {
                  #region Not have workflow
                  ts.Cancel_Status = WorkflowStatus.Cancelled;
                  model.result = tsService.UpdateTimeSheet(ts);
                  if (model.result.Code == ERROR_CODE.SUCCESS)
                  {
                     jService.CalCosting(ts.Job_Cost_ID, -1 * ts.Total_Amount);
                     sendProceedEmail(ts, com, userlogin, userlogin, hist, ts.Cancel_Status, null);
                     return RedirectToAction("Record", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL), Field = Resource.Time_Sheet });
                  }
                  #endregion
               }
            }
            else
            {
               #region workflow
               if (ts.Overall_Status == WorkflowStatus.Closed)
               {
                  var rqcancel = new RequestItem();
                  rqcancel.Doc_ID = model.Time_Sheet_ID;
                  rqcancel.Approval_Type = ApprovalType.TimeSheet;
                  rqcancel.Company_ID = userlogin.Company_ID.Value;
                  rqcancel.Department_ID = hist.Department_ID.HasValue ? hist.Department_ID.Value : 0;
                  rqcancel.Module = ModuleCode.Time;
                  rqcancel.Requestor_Email = userlogin.User_Authentication.Email_Address;
                  rqcancel.Requestor_Name = AppConst.GetUserName(userlogin);
                  rqcancel.Requestor_Profile_ID = userlogin.Profile_ID;

                  if (com.Is_Indent.HasValue && com.Is_Indent.Value)
                  {
                     rqcancel.IndentItems = getIndentSupervisor(ts.Job_Cost_ID.HasValue ? ts.Job_Cost_ID.Value : 0);
                     if (rqcancel.IndentItems != null && rqcancel.IndentItems.Count > 0)
                        rqcancel.Is_Indent = true;
                  }

                  var rc = aService.SubmitRequestCanceling(rqcancel);
                  if (rc.IsSuccess)
                  {
                     ts.Request_Cancel_ID = rqcancel.Request_ID;
                     ts.Cancel_Status = WorkflowStatus.Canceling;
                     if (rqcancel.Status == WorkflowStatus.Closed)
                        ts.Cancel_Status = WorkflowStatus.Cancelled;

                     model.result = tsService.UpdateTimeSheet(ts);
                     if (model.result.Code == ERROR_CODE.SUCCESS)
                     {
                        if (rqcancel.Status == WorkflowStatus.Closed)
                        {
                           jService.CalCosting(ts.Job_Cost_ID, -1 * ts.Total_Amount);
                           sendProceedEmail(ts, com, userlogin, userlogin, hist, ts.Cancel_Status, null);
                           return RedirectToAction("Record", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL), Field = Resource.Time_Sheet });
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
                                    param.Add("tsID", ts.Time_Sheet_ID);
                                    param.Add("appID", row.Requestor_Profile_ID);
                                    param.Add("empID", ts.Employee_Profile_ID);
                                    param.Add("reqcancelID", rqcancel.Request_ID);
                                    param.Add("cancelStatus", WorkflowStatus.Cancelled);
                                    param.Add("code", uService.GenActivateCode("T" + ts.Time_Sheet_ID + row.Requestor_Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

                                    var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                                    param["cancelStatus"] = WorkflowStatus.Rejected;
                                    var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                                    var appr = uService.getUser(row.Requestor_Profile_ID, false);
                                    if (appr != null)
                                    {
                                       if (ai != 0)
                                          approverName += " , ";

                                       approverName += AppConst.GetUserName(appr);
                                       sendRequestEmail(ts, com, appr, userlogin, hist, ts.Cancel_Status, rqcancel.Reviewers, linkApp, linkRej);
                                    }
                                 }
                                 else
                                 {
                                    var appr = uService.getUser(row.Requestor_Profile_ID, false);
                                    if (appr != null)
                                    {
                                       if (ai != 0)
                                          approverName += " , ";

                                       approverName += AppConst.GetUserName(appr);
                                    }
                                 }
                                 ai++;
                              }
                              sendProceedEmail(ts, com, userlogin, userlogin, hist, ts.Cancel_Status, rqcancel.Reviewers, approverName);
                              #endregion
                           }
                           else
                           {
                              #region Normal flow
                              var param = new Dictionary<string, object>();
                              param.Add("tsID", ts.Time_Sheet_ID);
                              param.Add("appID", rqcancel.NextApprover.Profile_ID);
                              param.Add("empID", ts.Employee_Profile_ID);
                              param.Add("reqcancelID", rqcancel.Request_ID);
                              param.Add("cancelStatus", WorkflowStatus.Cancelled);
                              param.Add("code", uService.GenActivateCode("T" + ts.Time_Sheet_ID + rqcancel.NextApprover.Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));
                              var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                              param["cancelStatus"] = WorkflowStatus.Rejected;
                              var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                              var appr = uService.getUser(rqcancel.NextApprover.Profile_ID, false);
                              if (appr != null)
                              {
                                 approverName = AppConst.GetUserName(appr);
                                 sendRequestEmail(ts, com, appr, userlogin, hist, ts.Cancel_Status, rqcancel.Reviewers, linkApp, linkRej);
                              }
                              sendProceedEmail(ts, com, userlogin, userlogin, hist, ts.Cancel_Status, rqcancel.Reviewers, approverName);
                              #endregion
                           }
                           return RedirectToAction("Record", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL), Field = Resource.Time_Sheet });
                        }
                     }
                  }
               }
               else
               {
                  ts.Cancel_Status = WorkflowStatus.Cancelled;
                  model.result = tsService.UpdateTimeSheet(ts);
                  if (model.result.Code == ERROR_CODE.SUCCESS)
                  {
                     jService.CalCosting(ts.Job_Cost_ID, -1 * ts.Total_Amount);
                     sendProceedEmail(ts, com, userlogin, userlogin, hist, ts.Cancel_Status, null);
                     return RedirectToAction("Record", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL), Field = Resource.Time_Sheet });
                  }
               }
               #endregion
            }
         }

         model.JobCostlst = cbService.LstJobCost(userlogin.Company_ID, false);
         if (model.Request_ID.HasValue)
         {
            var r = aService.GetMyRequests(userlogin.Company_ID, null, null, ModuleCode.Time, ApprovalType.TimeSheet, model.Time_Sheet_ID);
            if (r.Item2.IsSuccess && r.Item1 != null)
               model.Time_Sheet_Request = r.Item1;
         }

         //-------rights------------
         RightResult rightResult = base.validatePageRight(Operation.A);
         if (rightResult.action != null)
            return rightResult.action;
         model.rights = rightResult.rights;

         return View(model);
      }

      private ActionResult ApplicationMngt(TimeSheetViewModel model, string pStatus)
      {
         var userlogin = UserUtil.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var tsService = new TimeSheetService(userlogin);
         var cbService = new ComboService();
         var empService = new EmployeeService();
         var hService = new EmploymentHistoryService();
         var uService = new UserService();
         var currentdate = StoredProcedure.GetCurrentDate();
         var cpService = new CompanyService();
         var aService = new SBSWorkFlowAPI.Service();
         var jService = new JobCostService();

         var com = cpService.GetCompany(userlogin.Company_ID);
         if (com == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Company);

         var ts = tsService.GetTimeSheet(model.Time_Sheet_ID);
         if (ts == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Time_Sheet);

         if (ts.Cancel_Status == WorkflowStatus.Cancelled)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Time_Sheet);

         if (ts.Overall_Status == WorkflowStatus.Rejected)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Time_Sheet);

         var hist = hService.GetCurrentEmploymentHistory(ts.Employee_Profile_ID);
         if (hist == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Employment_History);

         var user = uService.getUser(hist.Employee_Profile.Profile_ID, false);
         if (user == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.User);

         ts.Update_By = userlogin.User_Authentication.Email_Address;
         ts.Update_On = currentdate;
         var status = pStatus;
         var Ac_Code = "T" + ts.Time_Sheet_ID + userlogin.Profile_ID + "_";
         if (string.IsNullOrEmpty(ts.Cancel_Status))
         {
            if (ts.Request_ID.HasValue && ts.Request_ID.Value > 0)
            {
               #region Workflow
               var action = new ActionItem();
               action.Actioner_Profile_ID = userlogin.Profile_ID;
               action.Email = userlogin.User_Authentication.Email_Address;
               action.Name = AppConst.GetUserName(userlogin);
               action.Request_ID = ts.Request_ID.Value;
               if (pStatus == WorkflowStatus.Approved)
               {
                  action.IsApprove = true;
                  action.Action = WorkflowAction.Approve;
               }
               else
               {
                  if (string.IsNullOrEmpty(model.Remark_Rej))
                  {
                     //ModelState.AddModelError("Remark_Rej", Resource.The + " " + Resource.Remark + " " + Resource.Field + " " + Resource.Is_Rrequired_Lower);
                     model.isRejectPopUp = true;
                  }
                  else
                     model.isRejectPopUp = false;

                  action.IsApprove = false;
                  action.Remarks = model.Remark_Rej;
                  action.Action = WorkflowAction.Reject;
               }
               if (ModelState.IsValid)
               {
                  var r = aService.SubmitRequestAction(action);
                  if (r.IsSuccess)
                  {
                     if (action.Status == WorkflowStatus.Closed)
                     {
                        ts.Overall_Status = WorkflowStatus.Closed;
                        model.result = tsService.UpdateTimeSheet(ts);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                           jService.CalCosting(ts.Job_Cost_ID, ts.Total_Amount);
                           uService.ExpireActivationByPrefix(Ac_Code);
                           sendProceedEmail(ts, com, user, userlogin, hist, action.Status, null);
                           return RedirectToAction("TimeSheetManagement", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resource.Time_Sheet });
                        }
                     }
                     else if (action.Status == WorkflowStatus.Rejected)
                     {
                        ts.Overall_Status = WorkflowStatus.Rejected;
                        model.result = tsService.UpdateTimeSheet(ts);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                           uService.ExpireActivationByPrefix(Ac_Code);
                           sendProceedEmail(ts, com, user, userlogin, hist, action.Status, null);
                           return RedirectToAction("TimeSheetManagement", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_REJECT), Field = Resource.Time_Sheet });
                        }
                     }
                     else
                     {

                        if (action.NextApprover == null)
                        {
                           #region Indent flow
                           var haveSendRequestEmail = false;
                           if (action.IndentValue != null && action.IndentValue.IsIndent && action.IndentValue.SendRequest)
                              haveSendRequestEmail = true;

                           if (haveSendRequestEmail)
                           {
                              List<IndentItem> IndentItems = getIndentSupervisor(ts.Job_Cost_ID.HasValue ? ts.Job_Cost_ID.Value : 0);
                              if (IndentItems != null && IndentItems.Count > 0)
                              {
                                 foreach (var row in IndentItems)
                                 {
                                    if (action.Actioner_Profile_ID == row.Requestor_Profile_ID)
                                       continue;

                                    var param = new Dictionary<string, object>();
                                    param.Add("tsID", ts.Time_Sheet_ID);
                                    param.Add("appID", row.Requestor_Profile_ID);
                                    param.Add("empID", ts.Employee_Profile_ID);
                                    param.Add("reqID", ts.Request_ID);
                                    param.Add("status", WorkflowStatus.Approved);
                                    param.Add("code", uService.GenActivateCode("T" + ts.Time_Sheet_ID + row.Requestor_Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

                                    var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                                    param["status"] = WorkflowStatus.Rejected;
                                    var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                                    var appr = uService.getUser(row.Requestor_Profile_ID, false);
                                    if (appr != null)
                                       sendRequestEmail(ts, com, appr, user, hist, WorkflowStatus.Submitted, null, linkApp, linkRej);
                                 }
                              }
                           }
                           #endregion
                        }
                        else
                        {
                           #region Normal flow
                           var param = new Dictionary<string, object>();
                           param.Add("tsID", ts.Time_Sheet_ID);
                           param.Add("appID", action.NextApprover.Profile_ID);
                           param.Add("empID", ts.Employee_Profile_ID);
                           param.Add("reqID", ts.Request_ID);
                           param.Add("status", WorkflowStatus.Approved);
                           param.Add("code", uService.GenActivateCode("T" + ts.Time_Sheet_ID + action.NextApprover.Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

                           var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                           param["status"] = WorkflowStatus.Rejected;
                           var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                           var appr = uService.getUser(action.NextApprover.Profile_ID, false);
                           if (appr != null)
                              sendRequestEmail(ts, com, appr, user, hist, WorkflowStatus.Submitted, null, linkApp, linkRej);

                           #endregion
                        }
                        uService.ExpireActivationByPrefix(Ac_Code);
                        return RedirectToAction("TimeSheetManagement", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resource.Time_Sheet });
                     }
                  }
               }
               #endregion
            }
            else if (hist.Supervisor.HasValue)
            {
               #region Supervisor
               /*approval by supervisor*/
               if (pStatus == WorkflowStatus.Approved)
                  ts.Overall_Status = WorkflowStatus.Closed;
               else
                  ts.Overall_Status = WorkflowStatus.Rejected;

               model.result = tsService.UpdateTimeSheet(ts);
               if (model.result.Code == ERROR_CODE.SUCCESS)
               {
                  uService.ExpireActivationByPrefix(Ac_Code);
                  sendProceedEmail(ts, com, user, userlogin, hist, ts.Overall_Status, null);
                  if (pStatus == WorkflowStatus.Approved)
                  {
                     jService.CalCosting(ts.Job_Cost_ID, ts.Total_Amount);
                     return RedirectToAction("TimeSheetManagement", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resource.Time_Sheet });
                  }
                  else
                     return RedirectToAction("TimeSheetManagement", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_REJECT), Field = Resource.Time_Sheet });
               }
               #endregion
            }
         }
         else
         {
            /* approve canncel workflow*/
            if (ts.Request_Cancel_ID.HasValue && ts.Request_Cancel_ID.Value > 0)
            {
               #region Workflow
               var action = new ActionItem();
               action.Actioner_Profile_ID = userlogin.Profile_ID;
               action.Email = userlogin.User_Authentication.Email_Address;
               action.Name = AppConst.GetUserName(userlogin);
               action.Request_ID = ts.Request_Cancel_ID.Value;
               if (pStatus == WorkflowStatus.Approved)
               {
                  action.IsApprove = true;
                  action.Action = WorkflowAction.Approve;
               }
               else
               {
                  if (string.IsNullOrEmpty(model.Remark_Rej))
                  {
                     //ModelState.AddModelError("Remark_Rej", Resource.The + " " + Resource.Remark + " " + Resource.Field + " " + Resource.Is_Rrequired_Lower);
                     model.isRejectPopUp = true;
                  }
                  else
                     model.isRejectPopUp = false;

                  action.IsApprove = false;
                  action.Remarks = model.Remark_Rej;
                  action.Action = WorkflowAction.Reject;
               }

               if (ModelState.IsValid)
               {
                  var r = aService.SubmitRequestAction(action);
                  if (r.IsSuccess)
                  {
                     if (action.Status == WorkflowStatus.Closed)
                     {
                        ts.Cancel_Status = WorkflowStatus.Cancelled;
                        model.result = tsService.UpdateTimeSheet(ts);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                           jService.CalCosting(ts.Job_Cost_ID, -1 * ts.Total_Amount);
                           uService.ExpireActivationByPrefix(Ac_Code);
                           sendProceedEmail(ts, com, user, userlogin, hist, WorkflowStatus.Cancelled, null);
                           return RedirectToAction("TimeSheetManagement", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resource.Time_Sheet });
                        }
                     }
                     else if (action.Status == WorkflowStatus.Rejected)
                     {
                        ts.Cancel_Status = WorkflowStatus.Cancellation_Rejected;
                        model.result = tsService.UpdateTimeSheet(ts);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                           uService.ExpireActivationByPrefix(Ac_Code);
                           sendProceedEmail(ts, com, user, userlogin, hist, WorkflowStatus.Cancellation_Rejected, null);
                           return RedirectToAction("TimeSheetManagement", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL_REJECT), Field = Resource.Time_Sheet });
                        }
                     }
                     else
                     {
                        if (action.NextApprover == null)
                        {
                           #region Indent flow
                           var haveSendRequestEmail = false;
                           if (action.IndentValue != null && action.IndentValue.IsIndent && action.IndentValue.SendRequest)
                              haveSendRequestEmail = true;

                           if (haveSendRequestEmail)
                           {
                              List<IndentItem> IndentItems = getIndentSupervisor(ts.Job_Cost_ID.HasValue ? ts.Job_Cost_ID.Value :0);
                              if (IndentItems != null && IndentItems.Count > 0)
                              {
                                 foreach (var row in IndentItems)
                                 {

                                    if (action.Actioner_Profile_ID == row.Requestor_Profile_ID)
                                       continue;

                                    var param = new Dictionary<string, object>();
                                    param.Add("tsID", ts.Time_Sheet_ID);
                                    param.Add("appID", row.Requestor_Profile_ID);
                                    param.Add("empID", ts.Employee_Profile_ID);
                                    param.Add("reqcancelID", action.Request_ID);
                                    param.Add("cancelStatus", WorkflowStatus.Cancelled);
                                    param.Add("code", uService.GenActivateCode("T" + ts.Time_Sheet_ID + row.Requestor_Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));
                                    var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                                    param["cancelStatus"] = WorkflowStatus.Rejected;
                                    var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                                    var appr = uService.getUser(row.Requestor_Profile_ID, false);
                                    if (appr != null)
                                       sendRequestEmail(ts, com, appr, user, hist, ts.Cancel_Status, null, linkApp, linkRej);
                                 }
                              }
                           }
                           #endregion
                        }
                        else
                        {
                           #region Normal flow
                           var param = new Dictionary<string, object>();
                           param.Add("tsID", ts.Time_Sheet_ID);
                           param.Add("appID", action.NextApprover.Profile_ID);
                           param.Add("empID", ts.Employee_Profile_ID);
                           param.Add("reqcancelID", action.Request_ID);
                           param.Add("cancelStatus", WorkflowStatus.Cancelled);
                           param.Add("code", uService.GenActivateCode("T" + ts.Time_Sheet_ID + action.NextApprover.Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));
                           var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                           param["cancelStatus"] = WorkflowStatus.Rejected;
                           var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                           var appr = uService.getUser(action.NextApprover.Profile_ID, false);
                           if (appr != null)
                              sendRequestEmail(ts, com, appr, user, hist, ts.Cancel_Status, null, linkApp, linkRej);
                           #endregion
                        }
                        uService.ExpireActivationByPrefix(Ac_Code);
                        return RedirectToAction("TimeSheetManagement", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resource.Time_Sheet });
                     }
                  }
               }
               #endregion
            }
            else if (hist.Supervisor.HasValue)
            {
               #region Supervisor
               /*cancel approval by supervisor*/
               if (pStatus == WorkflowStatus.Approved)
                  ts.Cancel_Status = WorkflowStatus.Cancelled;
               else
                  ts.Cancel_Status = WorkflowStatus.Cancellation_Rejected;

               model.result = tsService.UpdateTimeSheet(ts);
               if (model.result.Code == ERROR_CODE.SUCCESS)
               {
                  uService.ExpireActivationByPrefix(Ac_Code);
                  sendProceedEmail(ts, com, user, userlogin, hist, ts.Cancel_Status, null);
                  if (pStatus == WorkflowStatus.Approved)
                  {
                     jService.CalCosting(ts.Job_Cost_ID, -1 * ts.Total_Amount);
                     return RedirectToAction("TimeSheetManagement", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL), Field = Resource.Time_Sheet });
                  }
                  else
                     return RedirectToAction("TimeSheetManagement", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL_REJECT), Field = Resource.Time_Sheet });
               }
               #endregion
            }
         }
         //-------rights------------
         model.JobCostlst = cbService.LstJobCost(userlogin.Company_ID, false);
         if (model.Request_ID.HasValue)
         {
            var r = aService.GetMyRequests(userlogin.Company_ID, null, null, ModuleCode.Time, ApprovalType.TimeSheet, model.Time_Sheet_ID);
            if (r.Item2.IsSuccess && r.Item1 != null)
               model.Time_Sheet_Request = r.Item1;
         }

         //-------rights------------
         RightResult rightResult = base.validatePageRight(Operation.A);
         if (rightResult.action != null)
            return rightResult.action;
         model.rights = rightResult.rights;

         return View(model);
      }

      [HttpGet]
      public ActionResult TimeSheetDelete(string operation, string tID)
      {
         var currentdate = StoredProcedure.GetCurrentDate();
         var userlogin = UserUtil.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var lService = new LeaveService();
         var model = new TimeSheetViewModel();

         //Validate Page Right
         var rightResult = base.validatePageRight(Operation.D, "/TimeSheet/Application");
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;

         var tsService = new TimeSheetService(userlogin);
         model.operation = EncryptUtil.Decrypt(operation);
         if (string.IsNullOrEmpty(model.operation))
            model.operation = Operation.C;

         if (model.operation == Operation.D)
         {
            var tsID = NumUtil.ParseInteger(EncryptUtil.Decrypt(tID));
            if (tsID > 0)
            {
               var ts = tsService.GetTimeSheet(tsID);
               if (ts == null)
                  return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

               if (ts.Overall_Status != WorkflowStatus.Draft)
                  return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

               //-------data------------
               ts.Update_On = currentdate;
               ts.Update_By = userlogin.User_Authentication.Email_Address;
               ts.Record_Status = RecordStatus.Delete;
               model.result = tsService.UpdateTimeSheet(ts);
               if (model.result.Code == ERROR_CODE.SUCCESS)
                  return RedirectToAction("Record", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Time_Sheet });
               else
                  return RedirectToAction("Record", new { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Success().getSuccess(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Time_Sheet });
            }
         }
         return RedirectToAction("Record");
      }

      
      #endregion

      #region Time Sheet Transaction Report

      [HttpGet]
      [AllowAuthorized]
      public ActionResult TimeSheetTransactionReport(ServiceResult result, TimeSheetTransactionViewModel model)
      {
         var currentdate = StoredProcedure.GetCurrentDate();
         var userlogin = UserUtil.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //Validate Page Right
         var rightResult = base.validatePageRight(Operation.A);
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;
         model.result = result;

         var tsService = new TimeSheetService();
         var empService = new EmployeeService();
         var expenService = new ExpenseService();
         var wkServ = new WorkingDaysService();
         var cbService = new ComboService();
         var UserServ = new UserService();
         var comService = new CompanyService();
         var histServ = new EmploymentHistoryService();

         //filter
         var emp = new List<Employee_Profile>();
         var criteria2 = new EmployeeCriteria() { Company_ID = userlogin.Company_ID };
         var pResult2 = empService.LstEmployeeProfile(criteria2);
         if (pResult2.Object != null) emp = (List<Employee_Profile>)pResult2.Object;
         model.EmployeeList = emp;

         model.Yearlst = new List<int>();
         for (int i = currentdate.Year - 2; i <= currentdate.Year; i++)
            model.Yearlst.Add(i);

         if (!model.Search_Year.HasValue)
         {
            model.Search_Year = currentdate.Year;
            model.Search_Month = currentdate.Month;
         }
         model.Monthlst = cbService.LstMonth(true);

         var criteria = new TimeSheetCriteria()
         {
            Company_ID = userlogin.Company_ID,
            Month = model.Search_Month,
            Year = model.Search_Year,
            Employee_Profile_ID = model.Search_Employee_Profile_ID,
            Closed_Status = true
         };
         model.TimeSheetList = tsService.LstTimeSheet(criteria);

         var wService = new WorkingDaysService();
         var wk = wService.GetWorkingDay(userlogin.Company_ID);
         if (wk == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Working_Hours);

         model.WorkingDaysList = new List<Working_Days_List>();
         if (model.Search_Employee_Profile_ID.HasValue && model.Search_Employee_Profile_ID.Value > 0)
         {
            var user = UserServ.getUserByEmployeeProfile(model.Search_Employee_Profile_ID);
            if (user != null)
            {
               model.Profile_ID = user.Profile_ID;
               var WDay = new Working_Days_List();
               WDay.Employee_Profile_ID = model.Search_Employee_Profile_ID;
               //WDay.workdays = getWorkDays(user.Profile_ID);
               WDay.workdays = wk;
               model.WorkingDaysList.Add(WDay);
            }
         }
         else
         {
            if (model.TimeSheetList != null)
            {
               var WorkingDays = new List<Working_Days_List>();
               foreach (var roww in model.TimeSheetList)
               {
                  if (roww.Employee_Profile_ID.HasValue)
                  {
                     var user = UserServ.getUserByEmployeeProfile(roww.Employee_Profile_ID);
                     if (user != null)
                     {
                        model.Profile_ID = user.Profile_ID;
                        var WDay = new Working_Days_List();
                        WDay.Employee_Profile_ID = roww.Employee_Profile_ID;
                        //WDay.workdays = getWorkDays(user.Profile_ID);
                        WDay.workdays = wk;
                        WorkingDays.Add(WDay);
                     }
                  }
               }
               model.WorkingDaysList = WorkingDays.ToList();
            }
         }
         return View(model);
      }

      //public void TimeSheetTransactionExport(TimeSheetTransactionViewModel model, string tabAction = "")
      //{

      //   var currentdate = StoredProcedure.GetCurrentDate();
      //   var userlogin = UserUtil.getUser(HttpContext);
      //   var tsService = new TimeSheetService();
      //   var empService = new EmployeeService();
      //   var expenService = new ExpenseService();
      //   var wkServ = new WorkingDaysService();
      //   var cbService = new ComboService();
      //   var UserServ = new UserService();
      //   var comService = new CompanyService();
      //   var histServ = new EmploymentHistoryService();

      //   var criteria = new TimeSheetCriteria()
      //   {
      //      Company_ID = userlogin.Company_ID,
      //      Month = model.Search_Month,
      //      Year = model.Search_Year,
      //      Employee_Profile_ID = model.Search_Employee_Profile_ID,
      //      Closed_Status = true
      //   };
      //   model.TimeSheetList = tsService.LstTimeSheet(criteria);

      //   model.WorkingDaysList = new List<Working_Days_List>();
      //   if (model.Search_Employee_Profile_ID.HasValue && model.Search_Employee_Profile_ID.Value > 0)
      //   {
      //      var user = UserServ.getUserByEmployeeProfile(model.Search_Employee_Profile_ID);
      //      if (user != null)
      //      {
      //         model.Profile_ID = user.Profile_ID;
      //         var WDay = new Working_Days_List();
      //         WDay.Employee_Profile_ID = model.Search_Employee_Profile_ID;
      //         WDay.workdays = getWorkDays(user.Profile_ID);
      //         model.WorkingDaysList.Add(WDay);
      //         model.Emp_Name = AppConst.GetUserName(user);
      //      }
      //   }
      //   else
      //   {
      //      if (model.TimeSheetList != null)
      //      {
      //         var WorkingDays = new List<Working_Days_List>();
      //         foreach (var roww in model.TimeSheetList)
      //         {
      //            if (roww.Employee_Profile_ID.HasValue)
      //            {
      //               var user = UserServ.getUserByEmployeeProfile(roww.Employee_Profile_ID);
      //               if (user != null)
      //               {
      //                  model.Profile_ID = user.Profile_ID;
      //                  var WDay = new Working_Days_List();
      //                  WDay.Employee_Profile_ID = roww.Employee_Profile_ID;
      //                  WDay.workdays = getWorkDays(user.Profile_ID);
      //                  WorkingDays.Add(WDay);
      //               }
      //            }
      //         }
      //         model.WorkingDaysList = WorkingDays.ToList();
      //         model.Emp_Name = "";
      //      }
      //   }

      //   if (model.TimeSheetList != null)
      //   {
      //      var criteria3 = new ExpenseCriteria()
      //      {
      //         Company_ID = userlogin.Company_ID,
      //         Employee_Profile_ID = model.Search_Employee_Profile_ID,
      //         Closed_Status = true
      //      };

      //      var expentypelst = new List<Expenses_Application_Document>();
      //      var pResult = expenService.LstExpenseApplications(criteria3);
      //      if (pResult.Object != null) expentypelst = (List<Expenses_Application_Document>)pResult.Object;
      //      model.ExpensesApplicationDocumentList = expentypelst;
      //   }

      //   if (tabAction == "export")
      //   {
      //      var duphash = new List<string>();
      //      string csv = "";
      //      var fullName = AppConst.GetUserName(userlogin);
      //      //HEADER                
      //      string compname = comService.GetCompany(userlogin.Company_ID).Name;

      //      csv.Append( "<table><tr valign='top'>";
      //      csv.Append( "<td valign='top' colspan=5 ></td><td  colspan=2><b> " + Resource.Engineering_Expenses_Report + " </b></td></tr>";
      //      csv.Append( "<tr></tr>";
      //      csv.Append( "</table>";

      //      if (model.TimeSheetList != null)
      //      {
      //         var empNoList = model.TimeSheetList.Select(s => s.Employee_Profile_ID).GroupBy(g => g.Value).ToList();
      //         if (empNoList.Count > 0 && empNoList != null)
      //         {
      //            foreach (var remp in empNoList)
      //            {
      //               var totalAmount = 0.00M;
      //               if (remp != null)
      //               {
      //                  var user = UserServ.getUserByEmployeeProfile(remp.Key);
      //                  if (user != null)
      //                     model.Emp_Name = AppConst.GetUserName(user);

      //                  csv.Append( "<table><tr valign='top'>";
      //                  csv.Append( "<tr>";
      //                  csv.Append( "<td colspan=2 style='text-align:left'><b> " + Resource.Week + "</b></td>";
      //                  csv.Append( "<td colspan=2 ></td>";
      //                  csv.Append( "<td colspan=2 style='text-align:left'><b> " + Resource.Month + " / YR : </b> " + DateUtil.GetFullMonth(model.Search_Month) + (model.Search_Year > 0 ? " - " + model.Search_Year : "") + "</td>";
      //                  csv.Append( "<td colspan=3 ></td>";
      //                  csv.Append( "<td colspan=2 style='text-align:left'><b> " + Resource.Employee_Name + "  : </b> " + model.Emp_Name + "</td>";
      //                  csv.Append( "</tr>";
      //                  csv.Append( "<tr></tr>";
      //                  csv.Append( "</table>";

      //                  csv.Append( "<table border=1>";
      //                  csv.Append( "<tr bgcolor=#16A1A6  valign='top'>";
      //                  //csv.Append( "<td rowspan=2 style='text-align:center'><b>" + Resource.Employee_Name + "</b></td>";
      //                  csv.Append( "<td rowspan=2 style='text-align:center'><b>" + Resource.Day + "</b></td>";
      //                  csv.Append( "<td rowspan=2 style='text-align:center'><b>" + Resource.Date + "</b></td>";
      //                  csv.Append( "<td style='text-align:center'><b>" + Resource.Indent_No + "</b></td>";
      //                  csv.Append( "<td style='text-align:center'><b>" + Resource.Project_Name + "</b></td> ";
      //                  csv.Append( "<td colspan=2 style='text-align:center'><b>" + Resource._24_Hour_Clock + "</b></td>";
      //                  csv.Append( "<td rowspan=2 style='text-align:center'><b>" + Resource.Duration + "</b></td>";
      //                  csv.Append( "<td rowspan=2 style='text-align:center'><b>" + Resource.Overtime + "</b></td> ";
      //                  csv.Append( "<td colspan=4 style='text-align:center'><b>" + Resource.Other_Expenses + "</b></td>";
      //                  csv.Append( "</tr>";

      //                  csv.Append( "<tr bgcolor=#16A1A6>";
      //                  csv.Append( "<td style='text-align:center'><b>" + Resource.Quotation_No + "</b></td> ";
      //                  csv.Append( "<td style='text-align:center'><b>" + Resource.Job_Detail + "</b></td>";
      //                  csv.Append( "<td style='text-align:center'><b>" + Resource.Clock_In + "</b></td>";
      //                  csv.Append( "<td style='text-align:center'><b>" + Resource.Clock_Out + "</b></td>";
      //                  csv.Append( "<td style='text-align:center'><b>" + Resource.Expenses_Type + "</b></td>";
      //                  csv.Append( "<td style='text-align:center'><b>" + Resource.Remark + "</b></td>";
      //                  csv.Append( "<td style='text-align:center'><b>" + Resource.Expenses_No_SymbolDot + "</b></td>";
      //                  csv.Append( "<td style='text-align:center'><b>" + Resource.Amount + "</b></td>";
      //                  csv.Append( "</tr>";

      //                  if (model.Search_Month.HasValue)
      //                  {
      //                     if (model.Search_Year.HasValue)
      //                     {
      //                        var dayinmonth = DateTime.DaysInMonth(model.Search_Year.Value, model.Search_Month.Value);
      //                        for (var j = 0; j <= dayinmonth; j++)
      //                        {

      //                        }
      //                     }
      //                     else
      //                     {

      //                     }

      //                  }
      //                  else
      //                  {
      //                     if (model.Search_Year.HasValue)
      //                     {
      //                        for (var i = 1; i <= 12; i++)
      //                        {
      //                           var dayinmonth = DateTime.DaysInMonth(model.Search_Year.Value, i);
      //                           for (var j = 0; j <= dayinmonth; j++)
      //                           {

      //                           }
      //                        }
      //                     }
      //                     else
      //                     {

      //                     }

      //                  }

      //                  var tslist = model.TimeSheetList.Where(w => w.Employee_Profile_ID == remp.Key).ToList();
      //                  if (tslist == null) 
      //                     continue;

      //                  var rowContainslist = new List<string>();
      //                  foreach (var row in tslist)
      //                  {
      //                     if (!row.Date_Of_Date.HasValue || !row.Job_Cost_ID.HasValue || !row.Employee_Profile_ID.HasValue)
      //                        continue;

      //                     int rowsp = 1;
      //                     var tempTimesheet = new List<Time_Sheet>();
      //                     var duration = row.Duration.HasValue ? row.Duration.Value : new TimeSpan();

      //                     var wday = new Working_Days();
      //                     if (model.WorkingDaysList != null)
      //                        wday = model.WorkingDaysList.Where(w => w.Employee_Profile_ID == row.Employee_Profile_ID).FirstOrDefault().workdays;

      //                     var ot = getOvertime(DateTime.Parse(DateUtil.ToDisplayTime(row.Clock_Out)), wday);

      //                     if (!rowContainslist.Contains(row.Date_Of_Date.ToString() + "-" + row.Job_Cost_ID))
      //                     {
      //                        tempTimesheet = tslist.Where(w => w.Date_Of_Date == row.Date_Of_Date && w.Job_Cost_ID == row.Job_Cost_ID).ToList();
      //                        rowContainslist.Add(row.Date_Of_Date.ToString() + "-" + row.Job_Cost_ID);
      //                        if (tempTimesheet.Count() > 1)
      //                           rowsp = tempTimesheet.Count();

      //                        csv.Append( "<tr  valign='top'>";
      //                        csv.Append( "<td rowspan=" + rowsp + ">" + @row.Date_Of_Date.Value.DayOfWeek.ToString().Substring(0, 3) + "</td>";
      //                        csv.Append( "<td rowspan=" + rowsp + "> " + DateUtil.ToDisplayFullDate(row.Date_Of_Date) + "</td>";
      //                        csv.Append( "<td rowspan=" + rowsp + " style='text-align:left'>" + (row.Indent_No != null ? row.Indent_No.ToString() : "") + "&nbsp;</td> ";
      //                        csv.Append( "<td rowspan=" + rowsp + " style='text-align:left'>" + (row.Indent_Name != null ? row.Indent_Name.ToString() : "") + "&nbsp;</td> ";
      //                     }

      //                     if (rowContainslist.Contains(row.Date_Of_Date.ToString() + "-" + row.Job_Cost_ID))
      //                     {
      //                        csv.Append( "<td>" + DateUtil.ToDisplayTime(row.Clock_In) + "</td>";
      //                        csv.Append( "<td>" + DateUtil.ToDisplayTime(row.Clock_Out) + "</td>";
      //                        csv.Append( "<td>" + @duration.Hours + Resource.Hr_S + (duration.Minutes > 0 ? duration.Minutes + Resource.Min_S : "") + "</td> ";
      //                        csv.Append( "<td>" + @ot + "</td> ";

      //                        if (model.ExpensesApplicationDocumentList != null && model.ExpensesApplicationDocumentList.Count > 0)
      //                        {
      //                           var crow = 0;
      //                           var k = 1;
      //                           var exdocs = model.ExpensesApplicationDocumentList.Where(w => w.Expenses_Date == row.Date_Of_Date && w.Job_Cost_ID == row.Job_Cost_ID && w.Employee_Profile_ID == row.Employee_Profile_ID).ToList();
      //                           if (exdocs != null)
      //                           {
      //                              foreach (var r in exdocs)
      //                              {
      //                                 if (duphash.Contains(r.Expenses_Application_Document_ID + "+" + row.Job_Cost_ID.ToString() + "+" + row.Date_Of_Date.Value.Date.ToString()))
      //                                    continue;

      //                                 duphash.Add(r.Expenses_Application_Document_ID + "+" + row.Job_Cost_ID.ToString() + "+" + row.Date_Of_Date.Value.Date.ToString());
      //                                 if (r.Job_Cost_ID.HasValue)
      //                                 {
      //                                    if (k != 1)
      //                                    {
      //                                       csv.Append( "<tr valign='top'>";
      //                                       csv.Append( "<td></td> ";
      //                                       csv.Append( "<td></td> ";
      //                                       csv.Append( "<td></td> ";
      //                                       csv.Append( "<td></td> ";
      //                                       csv.Append( "<td></td> ";
      //                                       csv.Append( "<td></td> ";
      //                                       csv.Append( "<td></td> ";
      //                                       csv.Append( "<td></td> ";
      //                                    }

      //                                    if (r.Expenses_Config_ID != null && r.Expenses_Config != null)
      //                                       csv.Append( "<td  style='text-align:left'>" + r.Expenses_Config.Expenses_Name + "</td> ";
      //                                    else
      //                                       csv.Append( "<td  style='text-align:left'>" + Resource.Other + "</td> ";

      //                                    if (!String.IsNullOrEmpty(r.Remarks))
      //                                       csv.Append( "<td  style='text-align:left'>" + r.Remarks + "</td> ";
      //                                    else
      //                                       csv.Append( "<td></td> ";

      //                                    csv.Append( "<td>" + r.Expenses_Application.Expenses_No + "</td> ";
      //                                    csv.Append( "<td style='text-align:right'>" + NumUtil.FormatCurrency((r.Amount_Claiming.HasValue ? r.Amount_Claiming.Value : 0), 2) + "&nbsp;</td> ";

      //                                    if (k != 1)
      //                                       csv.Append( "</tr>";

      //                                    totalAmount += (r.Amount_Claiming.HasValue ? r.Amount_Claiming.Value : 0);
      //                                    k++;
      //                                    crow++;
      //                                 }
      //                              }
      //                              if (crow == 0)
      //                              {
      //                                 csv.Append( "<td></td> ";
      //                                 csv.Append( "<td></td> ";
      //                                 csv.Append( "<td></td> ";
      //                                 csv.Append( "<td style='text-align:right'></td> ";
      //                              }
      //                           }
      //                        }
      //                        else
      //                        {
      //                           csv.Append( "<td></td> ";
      //                           csv.Append( "<td></td> ";
      //                           csv.Append( "<td></td> ";
      //                           csv.Append( "<td style='text-align:right'></td> ";
      //                        }
      //                     }
      //                     csv.Append( "</tr>";
      //                  }

      //                  csv.Append( "<tr><td bgcolor=#16A1A6 style='text-align:right' colspan=8><b>" + Resource.Total_Amount + "</b></td> ";
      //                  csv.Append( "<td colspan=3></td> ";
      //                  csv.Append( "<td style='text-align:right'>" + NumUtil.FormatCurrency(totalAmount, 2) + "&nbsp;</td></tr> ";
      //               }
      //               csv.Append( "</table>";
      //               csv.Append( "<table><tr><td rowspan=2></td></tr></table>";
      //            }
      //         }
      //         else
      //         {
      //            csv.Append( "<table><tr valign='top'>";
      //            csv.Append( "<tr>";
      //            csv.Append( "<td colspan=2 style='text-align:left'><b> " + Resource.Week + "</b></td>";
      //            csv.Append( "<td colspan=2 ></td>";
      //            csv.Append( "<td colspan=2 style='text-align:left'><b> " + Resource.Month + " / YR : </b> " + DateUtil.GetFullMonth(model.Search_Month) + (model.Search_Year > 0 ? " - " + model.Search_Year : "") + "</td>";
      //            csv.Append( "<td colspan=3 ></td>";
      //            csv.Append( "<td colspan=2 style='text-align:left'><b> " + Resource.Employee_Name + "  : </b> " + model.Emp_Name + "</td>";
      //            csv.Append( "</tr>";
      //            csv.Append( "<tr></tr>";
      //            csv.Append( "</table>";

      //            csv.Append( "<table border=1>";
      //            csv.Append( "<tr bgcolor=#16A1A6  valign='top'>";
      //            //csv.Append( "<td rowspan=2 style='text-align:center'><b>" + Resource.Employee_Name + "</b></td>";
      //            csv.Append( "<td rowspan=2 style='text-align:center'><b>" + Resource.Day + "</b></td>";
      //            csv.Append( "<td rowspan=2 style='text-align:center'><b>" + Resource.Date + "</b></td>";
      //            csv.Append( "<td style='text-align:center'><b>" + Resource.Indent_No + "</b></td>";
      //            csv.Append( "<td style='text-align:center'><b>" + Resource.Project_Name + "</b></td> ";
      //            csv.Append( "<td colspan=2 style='text-align:center'><b>" + Resource._24_Hour_Clock + "</b></td>";
      //            csv.Append( "<td rowspan=2 style='text-align:center'><b>" + Resource.Duration + "</b></td>";
      //            csv.Append( "<td rowspan=2 style='text-align:center'><b>" + Resource.Overtime + "</b></td> ";
      //            csv.Append( "<td colspan=4 style='text-align:center'><b>" + Resource.Other_Expenses + "</b></td>";
      //            csv.Append( "</tr>";

      //            csv.Append( "<tr bgcolor=#16A1A6>";
      //            csv.Append( "<td style='text-align:center'><b>" + Resource.Quotation_No + "</b></td> ";
      //            csv.Append( "<td style='text-align:center'><b>" + Resource.Job_Detail + "</b></td>";
      //            csv.Append( "<td style='text-align:center'><b>" + Resource.Clock_In + "</b></td>";
      //            csv.Append( "<td style='text-align:center'><b>" + Resource.Clock_Out + "</b></td>";
      //            csv.Append( "<td style='text-align:center'><b>" + Resource.Expenses_Type + "</b></td>";
      //            csv.Append( "<td style='text-align:center'><b>" + Resource.Remark + "</b></td>";
      //            csv.Append( "<td style='text-align:center'><b>" + Resource.Expenses_No_SymbolDot + "</b></td>";
      //            csv.Append( "<td style='text-align:center'><b>" + Resource.Amount + "</b></td>";
      //            csv.Append( "</tr>";

      //            csv.Append( "<tr><td bgcolor=#16A1A6 style='text-align:right' colspan=8><b>" + Resource.Total_Amount + "</b></td> ";
      //            csv.Append( "<td colspan=3></td> ";
      //            csv.Append( "<td style='text-align:right'>0.00&nbsp;</td></tr> ";
      //         }
      //      }
      //      else
      //      {
      //         csv.Append( "<table><tr valign='top'>";
      //         csv.Append( "<tr>";
      //         csv.Append( "<td colspan=2 style='text-align:left'><b> " + Resource.Week + "</b></td>";
      //         csv.Append( "<td colspan=2 ></td>";
      //         csv.Append( "<td colspan=2 style='text-align:left'><b> " + Resource.Month + " / YR : </b> " + DateUtil.GetFullMonth(model.Search_Month) + (model.Search_Year > 0 ? " - " + model.Search_Year : "") + "</td>";
      //         csv.Append( "<td colspan=3 ></td>";
      //         csv.Append( "<td colspan=2 style='text-align:left'><b> " + Resource.Employee_Name + "  : </b> " + model.Emp_Name + "</td>";
      //         csv.Append( "</tr>";
      //         csv.Append( "<tr></tr>";
      //         csv.Append( "</table>";

      //         csv.Append( "<table border=1>";
      //         csv.Append( "<tr bgcolor=#16A1A6  valign='top'>";
      //         //csv.Append( "<td rowspan=2 style='text-align:center'><b>" + Resource.Employee_Name + "</b></td>";
      //         csv.Append( "<td rowspan=2 style='text-align:center'><b>" + Resource.Day + "</b></td>";
      //         csv.Append( "<td rowspan=2 style='text-align:center'><b>" + Resource.Date + "</b></td>";
      //         csv.Append( "<td style='text-align:center'><b>" + Resource.Indent_No + "</b></td>";
      //         csv.Append( "<td style='text-align:center'><b>" + Resource.Project_Name + "</b></td> ";
      //         csv.Append( "<td colspan=2 style='text-align:center'><b>" + Resource._24_Hour_Clock + "</b></td>";
      //         csv.Append( "<td rowspan=2 style='text-align:center'><b>" + Resource.Duration + "</b></td>";
      //         csv.Append( "<td rowspan=2 style='text-align:center'><b>" + Resource.Overtime + "</b></td> ";
      //         csv.Append( "<td colspan=4 style='text-align:center'><b>" + Resource.Other_Expenses + "</b></td>";
      //         csv.Append( "</tr>";

      //         csv.Append( "<tr bgcolor=#16A1A6>";
      //         csv.Append( "<td style='text-align:center'><b>" + Resource.Quotation_No + "</b></td> ";
      //         csv.Append( "<td style='text-align:center'><b>" + Resource.Job_Detail + "</b></td>";
      //         csv.Append( "<td style='text-align:center'><b>" + Resource.Clock_In + "</b></td>";
      //         csv.Append( "<td style='text-align:center'><b>" + Resource.Clock_Out + "</b></td>";
      //         csv.Append( "<td style='text-align:center'><b>" + Resource.Expenses_Type + "</b></td>";
      //         csv.Append( "<td style='text-align:center'><b>" + Resource.Remark + "</b></td>";
      //         csv.Append( "<td style='text-align:center'><b>" + Resource.Expenses_No_SymbolDot + "</b></td>";
      //         csv.Append( "<td style='text-align:center'><b>" + Resource.Amount + "</b></td>";
      //         csv.Append( "</tr>";

      //         csv.Append( "<tr><td bgcolor=#16A1A6 style='text-align:right' colspan=8><b>" + Resource.Total_Amount + "</b></td> ";
      //         csv.Append( "<td colspan=3></td> ";
      //         csv.Append( "<td style='text-align:right'>0.00&nbsp;</td></tr> ";
      //      }

      //      csv.Append( "<table><tr><td>&nbsp;</td></tr>";
      //      csv.Append( "<tr><td colspan=3><b>" + Resource.Printed_By + "</b> " + fullName + "</td></tr></table>";

      //      System.Web.UI.WebControls.GridView gv = new System.Web.UI.WebControls.GridView();
      //      gv.DataBind();
      //      Response.Clear();
      //      Response.ClearContent();
      //      Response.ClearHeaders();
      //      Response.Buffer = true;
      //      Response.AddHeader("content-disposition", "attachment; filename=" + Resource.Engineering_Expenses_Report + ".xls");
      //      Response.ContentType = "application/ms-excel";
      //      Response.Charset = "";
      //      Response.Write("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
      //      StringWriter sw = new StringWriter();
      //      sw.Write(csv);
      //      System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);
      //      gv.RenderControl(htw);
      //      Response.Output.Write(sw.ToString());
      //      Response.Flush();
      //      Response.End();

      //   }
      //}

      public void TimeSheetTransactionExport(TimeSheetTransactionViewModel model, string tabAction = "")
      {

         var currentdate = StoredProcedure.GetCurrentDate();
         var userlogin = UserUtil.getUser(HttpContext);
         var tsService = new TimeSheetService();
         var empService = new EmployeeService();
         var expenService = new ExpenseService();
         var wkServ = new WorkingDaysService();
         var cbService = new ComboService();
         var UserServ = new UserService();
         var comService = new CompanyService();
         var histServ = new EmploymentHistoryService();

         var criteria = new TimeSheetCriteria()
         {
            Company_ID = userlogin.Company_ID,
            Month = model.Search_Month,
            Year = model.Search_Year,
            Employee_Profile_ID = model.Search_Employee_Profile_ID,
            Closed_Status = true
         };
         model.TimeSheetList = tsService.LstTimeSheet(criteria);

         var wService = new WorkingDaysService();
         Working_Days wk = wService.GetWorkingDay(userlogin.Company_ID);

         model.WorkingDaysList = new List<Working_Days_List>();
         if (model.Search_Employee_Profile_ID.HasValue && model.Search_Employee_Profile_ID.Value > 0)
         {
            var user = UserServ.getUserByEmployeeProfile(model.Search_Employee_Profile_ID);
            if (user != null)
            {
               model.Profile_ID = user.Profile_ID;
               var WDay = new Working_Days_List();
               WDay.Employee_Profile_ID = model.Search_Employee_Profile_ID;
               //WDay.workdays = getWorkDays(user.Profile_ID);
               WDay.workdays = wk;
               model.WorkingDaysList.Add(WDay);
               model.Emp_Name = AppConst.GetUserName(user);
            }
         }
         else
         {
            if (model.TimeSheetList != null)
            {
               var WorkingDays = new List<Working_Days_List>();
               foreach (var roww in model.TimeSheetList)
               {
                  if (roww.Employee_Profile_ID.HasValue)
                  {
                     var user = UserServ.getUserByEmployeeProfile(roww.Employee_Profile_ID);
                     if (user != null)
                     {
                        model.Profile_ID = user.Profile_ID;
                        var WDay = new Working_Days_List();
                        WDay.Employee_Profile_ID = roww.Employee_Profile_ID;
                        //WDay.workdays = getWorkDays(user.Profile_ID);
                        WDay.workdays = wk;
                        WorkingDays.Add(WDay);
                     }
                  }
               }
               model.WorkingDaysList = WorkingDays.ToList();
               model.Emp_Name = "";
            }
         }

         //if (model.TimeSheetList != null)
         //{
         //   var criteria3 = new ExpenseCriteria()
         //   {
         //      Company_ID = userlogin.Company_ID,
         //      Employee_Profile_ID = model.Search_Employee_Profile_ID,
         //      Closed_Status = true
         //   };

         //   var expentypelst = new List<Expenses_Application_Document>();
         //   var pResult = expenService.LstExpenseApplications(criteria3);
         //   if (pResult.Object != null) expentypelst = (List<Expenses_Application_Document>)pResult.Object;
         //   model.ExpensesApplicationDocumentList = expentypelst;
         //}

         if (tabAction == "export")
         {
            ExcelPackage pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add(Resource.Engineering_Expenses_Report);

            var row = 1;
            var col = 1;
            var allcolspan = 10;

            ws.Cells[row, col].Value = Resource.Engineering_Expenses_Report;
            ws.Cells[row, col, row, allcolspan].Merge = true;
            ws.Cells[row, col, row, allcolspan].Style.Font.Bold = true;
            ws.Cells[row, col, row, allcolspan].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[row, col, row, allcolspan].Style.Font.Size = (float)(ws.Cells[row, col, row, allcolspan].Style.Font.Size * 1.4);
            row++;

            var duphash = new List<string>();
            var fullName = AppConst.GetUserName(userlogin);
            if (model.TimeSheetList != null)
            {
               var empNoList = model.TimeSheetList.Select(s => s.Employee_Profile_ID).GroupBy(g => g.Value).ToList();
               if (empNoList.Count > 0 && empNoList != null)
               {
                  foreach (var remp in empNoList)
                  {
                     var totalamt = 0.00M;
                     if (remp != null)
                     {
                        var user = UserServ.getUserByEmployeeProfile(remp.Key);
                        if (user != null)
                           model.Emp_Name = AppConst.GetUserName(user);

                        ws.Cells[row, col, row, allcolspan].Style.Font.Bold = true;
                        ws.Cells[row, 1].Value = Resource.Employee + " : " + model.Emp_Name;
                        ws.Cells[row, 1, row, allcolspan].Merge = true;
                        row++;

                        /***** Header 1 *******/
                        ws.Cells[row, col, row, allcolspan].Style.Font.Bold = true;
                        ws.Cells[row, col, row, allcolspan].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[row, col, row, allcolspan].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[row, col, row, allcolspan].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        for (int c = 1; c <= allcolspan; c++)
                           ws.Cells[row, c].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        ws.Cells[row, 1].Value = Resource.Day;
                        ws.Cells[row, 2].Value = Resource.Date;
                        ws.Cells[row, 3].Value = Resource.Indent_No;
                        ws.Cells[row, 4].Value = Resource.Project_Name;
                        ws.Cells[row, 5].Value = Resource._24_Hour_Clock;
                        ws.Cells[row, 5, row, 6].Merge = true;
                        ws.Cells[row, 7].Value = Resource.Duration;
                        ws.Cells[row, 8].Value = Resource.Overtime;
                        ws.Cells[row, 9].Value = Resource.Hour_Rate;
                        ws.Cells[row, 10].Value = Resource.Total_Amount;
                        row++;

                        ws.Cells[row, col, row, allcolspan].Style.Font.Bold = true;
                        ws.Cells[row, col, row, allcolspan].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[row, col, row, allcolspan].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[row, col, row, allcolspan].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        for (int c = 1; c <= allcolspan; c++)
                           ws.Cells[row, c].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        ws.Cells[row, 1].Value = "";
                        ws.Cells[row, 2].Value = "";
                        ws.Cells[row, 3].Value = Resource.Quotation_No;
                        ws.Cells[row, 4].Value = Resource.Job_Detail;
                        ws.Cells[row, 5].Value = Resource.Clock_In;
                        ws.Cells[row, 6].Value = Resource.Clock_Out;
                        ws.Cells[row, 7].Value = "";
                        ws.Cells[row, 8].Value = "";
                        ws.Cells[row, 9].Value = "";
                        ws.Cells[row, 10].Value = "";
                        row++;

                        var wday = new Working_Days();
                        if (model.WorkingDaysList != null && model.WorkingDaysList.Where(w => w.Employee_Profile_ID == remp.Key).FirstOrDefault() != null)
                           wday = model.WorkingDaysList.Where(w => w.Employee_Profile_ID == remp.Key).FirstOrDefault().workdays;

                        int minyear = 0;
                        int maxyear = 0;
                        int minmonth = 1;
                        int maxmonth = 12;
                        if (model.Search_Year.HasValue && model.Search_Year.Value > 0)
                        {
                           var year = model.Search_Year.Value;
                           minyear = year;
                           maxyear = year;
                           if (model.Search_Month.HasValue && model.Search_Month.Value > 0)
                           {
                              var month = model.Search_Month.Value;
                              minmonth = month;
                              maxmonth = month;
                           }
                        }
                        else
                        {
                           minyear = currentdate.Year - 2;
                           maxyear = currentdate.Year;
                           for (int year = currentdate.Year - 2; year <= currentdate.Year; year++)
                           {
                              if (model.Search_Month.HasValue && model.Search_Month.Value > 0)
                              {
                                 var month = model.Search_Month.Value;
                                 minmonth = month;
                                 maxmonth = month;
                              }
                           }
                        }


                        for (int year = minyear; year <= maxyear; year++)
                        {
                           for (var month = minmonth; month <= maxmonth; month++)
                           {
                              var dayinmonth = DateTime.DaysInMonth(year, month);
                              var monthyear = DateUtil.GetFullMonth(month) + " " + year;

                              ws.Cells[row, col, row, allcolspan].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                              ws.Cells[row, col, row, allcolspan].Style.Fill.PatternType = ExcelFillStyle.Solid;
                              ws.Cells[row, col, row, allcolspan].Style.Fill.BackgroundColor.SetColor(Color.SkyBlue);
                              ws.Cells[row, col, row, allcolspan].Style.Font.Bold = true;
                              ws.Cells[row, 1].Value = monthyear;
                              ws.Cells[row, 1, row, allcolspan].Merge = true;
                              row++;

                              for (var day = 1; day <= dayinmonth; day++)
                              {
                                 var date = DateUtil.ToDate(day, month, year);
                                 var timesheets = model.TimeSheetList
                                    .Where(w => w.Employee_Profile_ID == remp.Key && w.Date_Of_Date.HasValue
                                       && w.Job_Cost_ID.HasValue
                                       && w.Date_Of_Date.Value.Year == year
                                       && w.Date_Of_Date.Value.Month == month
                                       && w.Date_Of_Date.Value.Day == day)
                                       .ToList();

                                 if (timesheets.Count > 0)
                                 {
                                    var displaydate = true;
                                    foreach (var timesheet in timesheets)
                                    {
                                       var ot = "";
                                       if (timesheet.Clock_Out.HasValue)
                                          ot = getOvertime(DateTime.Parse(DateUtil.ToDisplayTime(timesheet.Clock_Out)), wday);

                                       var dayOfWeek = "";

                                       if (displaydate)
                                       {
                                          dayOfWeek = timesheet.Date_Of_Date.Value.DayOfWeek.ToString().Substring(0, 3);
                                       }

                                       var durationstr = "";
                                       if (timesheet.Duration.HasValue)
                                          durationstr = timesheet.Duration.Value.Hours + " " + Resource.Hr_S + " " + (timesheet.Duration.Value.Minutes > 0 ? timesheet.Duration.Value.Minutes + " " + Resource.Min_S : "");

                                       var hourrate = "";
                                       if (timesheet.Hour_Rate.HasValue)
                                          hourrate = NumUtil.FormatCurrency(timesheet.Hour_Rate, 2);

                                       var totalamount = "";
                                       if (timesheet.Total_Amount.HasValue)
                                       {
                                          totalamount = NumUtil.FormatCurrency(timesheet.Total_Amount, 2);
                                          totalamt += timesheet.Total_Amount.Value;
                                       }


                                       ws.Cells[row, col, row, allcolspan].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                       for (int c = 1; c <= allcolspan; c++)
                                          ws.Cells[row, c].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                       ws.Cells[row, 1].Value = dayOfWeek;
                                       ws.Cells[row, 2].Value = date.Value.Day + " " + DateUtil.GetFullMonth(date.Value.Month) + " " + date.Value.Year;
                                       ws.Cells[row, 3].Value = timesheet.Indent_No;
                                       ws.Cells[row, 4].Value = timesheet.Indent_Name;
                                       ws.Cells[row, 5].Value = DateUtil.ToDisplayTime(timesheet.Clock_In);
                                       ws.Cells[row, 6].Value = DateUtil.ToDisplayTime(timesheet.Clock_Out);
                                       ws.Cells[row, 7].Value = durationstr;
                                       ws.Cells[row, 8].Value = ot;
                                       ws.Cells[row, 9].Value = hourrate;
                                       ws.Cells[row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                       ws.Cells[row, 10].Value = totalamount;
                                       ws.Cells[row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                       row++;

                                       displaydate = false;
                                    }
                                 }
                                 else
                                 {
                                    ws.Cells[row, col, row, allcolspan].Style.Font.Color.SetColor(Color.Red);
                                    ws.Cells[row, col, row, allcolspan].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    for (int c = 1; c <= allcolspan; c++)
                                       ws.Cells[row, c].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                    ws.Cells[row, 1].Value = date.Value.DayOfWeek.ToString().Substring(0, 3);
                                    ws.Cells[row, 2].Value = date.Value.Day + " " + DateUtil.GetFullMonth(date.Value.Month) + " " + date.Value.Year;
                                    ws.Cells[row, 3].Value = "";
                                    ws.Cells[row, 4].Value = "";
                                    ws.Cells[row, 5].Value = "";
                                    ws.Cells[row, 6].Value = "";
                                    ws.Cells[row, 7].Value = "";
                                    ws.Cells[row, 8].Value = "";
                                    ws.Cells[row, 9].Value = "";
                                    ws.Cells[row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ws.Cells[row, 10].Value = "";
                                    ws.Cells[row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    row++;
                                 }
                              }
                           }
                        }
                        ws.Cells[row, col, row, allcolspan].Style.Font.Bold = true;
                        ws.Cells[row, col, row, allcolspan].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[row, col, row, allcolspan].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        ws.Cells[row, col, row, allcolspan].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        for (int c = 1; c <= allcolspan; c++)
                           ws.Cells[row, c].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        ws.Cells[row, 1].Value = Resource.Total_Amount;
                        ws.Cells[row, 1, row, 8].Merge = true;
                        ws.Cells[row, 9].Value = NumUtil.FormatCurrency(totalamt, 2);
                        ws.Cells[row, 9, row, 10].Merge = true;
                        row++;
                     }
                     row++;
                     row++;
                     row++;
                  }
               }
            }

            ws.Cells[row, col, row, allcolspan].Style.Font.Bold = true;
            ws.Cells[row, 1].Value = Resource.Printed_By;
            ws.Cells[row, 1, row, 2].Merge = true;
            ws.Cells[row, 3].Value = fullName;
            ws.Cells[row, 3, row, 10].Merge = true;

            row++;

            for (var c = 1; c <= allcolspan; c++)
               ws.Column(c).Width = 15;

            ws.Column(2).Width = 20;
            ws.Column(3).Width = 20;
            ws.Column(4).Width = 60;
            pck.SaveAs(Response.OutputStream);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;  filename=" + Resource.Engineering_Expenses_Report + ".xlsx");


         }
      }

      private string getTimesheetRptRow(ref decimal totalamt, int? eID, List<Time_Sheet> TimeSheetList, Working_Days wday, int day, int month, int year)
      {
         var csv = new StringBuilder();
         var timesheets = TimeSheetList
            .Where(w => w.Employee_Profile_ID == eID
               && w.Date_Of_Date.HasValue
               && w.Job_Cost_ID.HasValue
               && w.Date_Of_Date.Value.Year == year
               && w.Date_Of_Date.Value.Month == month
               && w.Date_Of_Date.Value.Day == day)
               .ToList();

         if (timesheets.Count > 0)
         {
            var displaydate = true;
            foreach (var timesheet in timesheets)
            {
               csv.Append(getTimesheetRptRow(ref totalamt, timesheet, wday, displaydate));
               displaydate = false;
            }
         }
         else
         {
            var timesheet = new Time_Sheet();
            timesheet.Date_Of_Date = DateUtil.ToDate(day, month, year);
            csv.Append(getTimesheetRptRow(ref totalamt, timesheet, wday));
         }
         return csv.ToString();
      }

      private string getTimesheetRptRow(ref decimal totalamt, Time_Sheet row, Working_Days wday, bool displaydate = true)
      {
         var ot = "";
         if (row.Clock_Out.HasValue)
            ot = getOvertime(DateTime.Parse(DateUtil.ToDisplayTime(row.Clock_Out)), wday);

         var dayOfWeek = "";
         var date = "";
         if (displaydate)
         {
            dayOfWeek = row.Date_Of_Date.Value.DayOfWeek.ToString().Substring(0, 3);
            if (row.Date_Of_Date.HasValue)
               date = row.Date_Of_Date.Value.Day + " " + DateUtil.GetFullMonth(row.Date_Of_Date.Value.Month) + " " + row.Date_Of_Date.Value.Year;
         }

         var indenNo = (row.Indent_No != null ? row.Indent_No.ToString() : "");
         var indenName = (row.Indent_Name != null ? row.Indent_Name.ToString() : "");

         var durationstr = "";
         if (row.Duration.HasValue)
            durationstr = row.Duration.Value.Hours + " " + Resource.Hr_S + " " + (row.Duration.Value.Minutes > 0 ? row.Duration.Value.Minutes + " " + Resource.Min_S : "");

         var hourrate = "";
         if (row.Hour_Rate.HasValue)
            hourrate = NumUtil.FormatCurrency(row.Hour_Rate, 2);

         var totalamount = "";
         if (row.Total_Amount.HasValue)
         {
            totalamount = NumUtil.FormatCurrency(row.Total_Amount, 2);
            totalamt += row.Total_Amount.Value;
         }



         var csv = new StringBuilder();
         csv.Append("<tr valign='top'>");
         csv.Append("<td style='font-weight:700'>" + dayOfWeek + "</td>");
         csv.Append("<td style='font-weight:700'>" + date + "</td>");
         csv.Append("<td style='text-align:left'>" + indenNo + "&nbsp;</td>");
         csv.Append("<td style='text-align:left'>" + indenName + "&nbsp;</td>");
         csv.Append("<td>" + DateUtil.ToDisplayTime(row.Clock_In) + "</td>");
         csv.Append("<td>" + DateUtil.ToDisplayTime(row.Clock_Out) + "</td>");
         csv.Append("<td>" + durationstr + "</td>");
         csv.Append("<td>" + ot + "</td>");
         csv.Append("<td style='text-align:right'>" + hourrate + "</td>");
         csv.Append("<td style='text-align:right'>" + totalamount + "</td>");
         //csv.Append("<td></td>");
         //csv.Append("<td></td>");
         //csv.Append("<td></td>");
         //csv.Append("<td style='text-align:right'></td>");
         csv.Append("</tr>");
         return csv.ToString();


      }

      private string getTimesheetRptMonthRow(int month)
      {
         var csv = new StringBuilder();
         csv.Append("<tr valign='top' bgcolor=#45E3E9>");
         csv.Append("<td colspan='10' style='font-weight:700;text-align:center'>" + DateUtil.GetFullMonth(month) + "</td>");
         csv.Append("</tr>");
         return csv.ToString();
      }
      private string getTimesheetRptEmptyRow()
      {
         var csv = new StringBuilder();
         csv.Append("<tr valign='top' >");
         csv.Append("<td colspan='10' ></td>");
         csv.Append("</tr>");
         return csv.ToString();
      }
      private string getOvertime(DateTime? clockout, Working_Days workdays)
      {
         /* check time from workdays*/
         if (workdays != null)
         {
            var wk = workdays;
            var dw = (int)clockout.Value.DayOfWeek;
            if (dw == 0)
               return getDurationWorkdays(wk.CL_Sun, wk.ST_Sun_Time, wk.ET_Sun_Time, clockout);
            else if (dw == 1)
               return getDurationWorkdays(wk.CL_Mon, wk.ST_Mon_Time, wk.ET_Mon_Time, clockout);
            else if (dw == 2)
               return getDurationWorkdays(wk.CL_Tue, wk.ST_Tue_Time, wk.ET_Tue_Time, clockout);
            else if (dw == 3)
               return getDurationWorkdays(wk.CL_Wed, wk.ST_Wed_Time, wk.ET_Wed_Time, clockout);
            else if (dw == 4)
               return getDurationWorkdays(wk.CL_Thu, wk.ST_Thu_Time, wk.ET_Thu_Time, clockout);
            else if (dw == 5)
               return getDurationWorkdays(wk.CL_Fri, wk.ST_Fri_Time, wk.ET_Fri_Time, clockout);
            else if (dw == 6)
               return getDurationWorkdays(wk.CL_Sat, wk.ST_Sat_Time, wk.ET_Sat_Time, clockout);
         }
         return null;
      }

      private string getDurationWorkdays(bool? disabled, TimeSpan? start, TimeSpan? end, DateTime? clockout)
      {
         if (!disabled.HasValue || !disabled.Value)
         {
            var clockouttime = DateUtil.ToTime(DateUtil.ToDisplayTime(clockout));
            if (clockouttime > end)
            {
               var duration = DateTime.Parse(DateUtil.ToDisplayTime(clockouttime)).Subtract(DateTime.Parse(DateUtil.ToDisplayTime(end)));
               if (duration != null)
               {
                  var dur = duration.Hours + " " + Resource.Hr_S + " " + (duration.Minutes > 0 ? duration.Minutes + " " + Resource.Min_S : "");
                  return dur.ToString();
               }
            }
         }
         return null;
      }

      [HttpGet]
      public string ApplicationConfig(string pDateOfDate)
      {
         TimeSheetViewModel model = new TimeSheetViewModel();
         var currentdate = StoredProcedure.GetCurrentDate();
         var hService = new EmploymentHistoryService();
         var empService = new EmployeeService();
         var wkService = new WorkingDaysService();

         DateTime? dateofdate = DateUtil.ToDate(pDateOfDate);
         var clockIn = "";
         var clockOut = "";

         var userlogin = UserUtil.getUser(HttpContext);
         if (userlogin == null) return "";

         //var workdays = getWorkDays(userlogin.Profile_ID);
         Working_Days workdays = wkService.GetWorkingDay(userlogin.Company_ID);
         if (workdays != null && dateofdate.HasValue)
         {
            var dw = (int)dateofdate.Value.DayOfWeek;
            if (dw == 0)
            {
               if (!workdays.CL_Sun.HasValue || !workdays.CL_Sun.Value)
               {
                  clockIn = DateUtil.ToDisplayTime(workdays.ST_Sun_Time);
                  clockOut = DateUtil.ToDisplayTime(workdays.ET_Sun_Time);
               }
            }
            else if (dw == 1)
            {
               if (!workdays.CL_Mon.HasValue || !workdays.CL_Mon.Value)
               {
                  clockIn = DateUtil.ToDisplayTime(workdays.ST_Mon_Time);
                  clockOut = DateUtil.ToDisplayTime(workdays.ET_Mon_Time);
               }
            }
            else if (dw == 2)
            {
               if (!workdays.CL_Tue.HasValue || !workdays.CL_Tue.Value)
               {
                  clockIn = DateUtil.ToDisplayTime(workdays.ST_Tue_Time);
                  clockOut = DateUtil.ToDisplayTime(workdays.ET_Tue_Time);
               }
            }
            else if (dw == 3)
            {
               if (!workdays.CL_Wed.HasValue || !workdays.CL_Wed.Value)
               {
                  clockIn = DateUtil.ToDisplayTime(workdays.ST_Wed_Time);
                  clockOut = DateUtil.ToDisplayTime(workdays.ET_Wed_Time);
               }
            }
            else if (dw == 4)
            {
               if (!workdays.CL_Thu.HasValue || !workdays.CL_Thu.Value)
               {
                  clockIn = DateUtil.ToDisplayTime(workdays.ST_Thu_Time);
                  clockOut = DateUtil.ToDisplayTime(workdays.ET_Thu_Time);
               }
            }
            else if (dw == 5)
            {
               if (!workdays.CL_Fri.HasValue || !workdays.CL_Fri.Value)
               {
                  clockIn = DateUtil.ToDisplayTime(workdays.ST_Fri_Time);
                  clockOut = DateUtil.ToDisplayTime(workdays.ET_Fri_Time);
               }
            }
            else if (dw == 6)
            {
               if (!workdays.CL_Sat.HasValue || !workdays.CL_Sat.Value)
               {
                  clockIn = DateUtil.ToDisplayTime(workdays.ST_Sat_Time);
                  clockOut = DateUtil.ToDisplayTime(workdays.ET_Sat_Time);
               }
            }
         }

         string str = "";
         str = "<script type=\"text/javascript\"> \n\n $(function () {";
         str += "$('#Clock_In').val('" + clockIn + "');";
         str += "$('#Clock_Out').val('" + clockOut + "');";
         str += "$('#Clock_In').trigger('chosen:updated');";
         str += "$('#Clock_Out').trigger('chosen:updated');";
         str += "});\n\n</script>";

         return str;

      }

      //private Working_Days getWorkDays(int? profileID)
      //{

      //   var userlogin = UserUtil.getUser(HttpContext);
      //   if (userlogin == null) return null;

      //   var histService = new EmploymentHistoryService();
      //   var empService = new EmployeeService();
      //   var wkService = new WorkingDaysService();
      //   var workdays = new Working_Days();

      //   var hist = histService.GetCurrentEmploymentHistoryByProfile(profileID);
      //   if (hist != null)
      //   {
      //      if ((hist.CL_Fri.HasValue && hist.CL_Mon.HasValue && hist.CL_Sat.HasValue && hist.CL_Sun.HasValue && hist.CL_Thu.HasValue && hist.CL_Tue.HasValue && hist.CL_Wed.HasValue && hist.CL_Lunch.HasValue) &&
      //      (hist.CL_Fri.Value && hist.CL_Mon.Value && hist.CL_Sat.Value && hist.CL_Sun.Value && hist.CL_Thu.Value && hist.CL_Tue.Value && hist.CL_Wed.Value && hist.CL_Lunch.Value))
      //      {
      //         workdays = wkService.GetWorkingDay(userlogin.Company_ID);
      //      }
      //      else
      //      {
      //         workdays = new Working_Days
      //         {
      //            CL_Fri = hist.CL_Fri,
      //            CL_Mon = hist.CL_Mon,
      //            CL_Sat = hist.CL_Sat,
      //            CL_Sun = hist.CL_Sun,
      //            CL_Thu = hist.CL_Thu,
      //            CL_Tue = hist.CL_Tue,
      //            CL_Wed = hist.CL_Wed,
      //            Days = hist.Days,
      //            ET_Fri_Time = hist.ET_Fri_Time,
      //            ET_Mon_Time = hist.ET_Mon_Time,
      //            ET_Sat_Time = hist.ET_Sat_Time,
      //            ET_Sun_Time = hist.ET_Sun_Time,
      //            ET_Thu_Time = hist.ET_Thu_Time,
      //            ET_Tue_Time = hist.ET_Tue_Time,
      //            ET_Wed_Time = hist.ET_Wed_Time,
      //            ST_Fri_Time = hist.ST_Fri_Time,
      //            ST_Mon_Time = hist.ST_Mon_Time,
      //            ST_Sat_Time = hist.ST_Sat_Time,
      //            ST_Sun_Time = hist.ST_Sun_Time,
      //            ST_Thu_Time = hist.ST_Thu_Time,
      //            ST_Tue_Time = hist.ST_Tue_Time,
      //            ST_Wed_Time = hist.ST_Wed_Time,
      //            CL_Lunch = hist.CL_Lunch,
      //            ET_Lunch_Time = hist.ET_Lunch_Time,
      //            ST_Lunch_Time = hist.ST_Lunch_Time
      //         };
      //      }
      //   }
      //   else
      //   {
      //      workdays = wkService.GetWorkingDay(userlogin.Company_ID);
      //   }
      //   return workdays;
      //}

      #endregion
   }
}
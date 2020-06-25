using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HR.Models;
using SBSModel.Models;
using SBSModel.Common;
using SBSResourceAPI;
using System.Diagnostics;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using SBSWorkFlowAPI.Constants;
using SBSWorkFlowAPI.Models;
using SBSWorkFlowAPI.ModelsAndService;
using iTextSharp.text.html.simpleparser;
using System.Text;
using System.Web.Routing;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;


namespace HR.Controllers
{
   [Authorize]
   [AllowAuthorized]
   public class TsExController : ControllerBase
   {
      [HttpGet]
      [AllowAuthorized]
      public ActionResult TsEx(TsExViewModels model, ServiceResult result)
      {
         var currentdate = StoredProcedure.GetCurrentDate();
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         if (!model.search_Year.HasValue)
            model.search_Year = currentdate.Year;

         var tsexService = new TsExService();
         model.result = result;
         model.timesheetExlist = tsexService.LstTsEx(userlogin.Company_ID, userlogin.Profile_ID, model.search_Month, model.search_Year);
         //tsexService.Round(userlogin.Company_ID);
         return View(model);
      }

      [HttpGet]
      [AllowAuthorized]
      public ActionResult TsExInfo(string operation, int? pID, int? pY, int? pM, string ctlr, string ac)
      {
         var model = new TsExInfoViewModels();
         var currentdate = StoredProcedure.GetCurrentDate();
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         model.ctlr = ctlr;
         model.ac = ac;
         model.operation = operation;
         if (string.IsNullOrEmpty(model.operation))
            model.operation = Operation.C;

         var tsexService = new TsExService();
         var eService = new ExpenseService();
         var empService = new EmployeeService();
         var hService = new EmploymentHistoryService();
         var cbService = new ComboService();
         var cmService = new CompanyService();

         var emplogin = empService.GetEmployeeProfileByProfileID2(userlogin.Profile_ID);
         if (emplogin == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

         model.Emp_Login_ID = emplogin.Employee_Profile_ID;
         if (model.operation == Operation.C)
         {
            #region New
            model.Profile_ID = userlogin.Profile_ID;
            model.Name = UserSession.GetUserName(userlogin);
            model.Date_Applied = DateUtil.ToDisplayDate(currentdate);
            model.Year = currentdate.Year;
            model.Month = currentdate.Month;
            model.ExStatus = WorkflowStatus.Draft;
            model.TsStatus = WorkflowStatus.Draft;
            model.wfCurrentStatus = WorkFlowCurrentStatus.EmpActive;
            if (pM.HasValue)
               model.Month = pM.Value;
            if (pY.HasValue)
               model.Year = pY.Value;
            model.Employee_Profile_ID = emplogin.Employee_Profile_ID;
            #endregion

            var dup = tsexService.GetTsExByMonthYear(userlogin.Company_ID, emplogin.Employee_Profile_ID, model.Month, model.Year);
            if (dup == null)
            {
               var hist = hService.GetCurrentEmploymentHistory(model.Employee_Profile_ID);
               if (hist == null)
                  return errorPage(ERROR_CODE.ERROR_14_NO_EMPLOYEE_HIST);

               var tsex = new TsEX();
               tsex.Company_ID = userlogin.Company_ID;
               tsex.Employee_Profile_ID = model.Employee_Profile_ID;
               tsex.Month = model.Month;
               tsex.Year = model.Year;
               tsex.Ex_Total_Amount = 0;
             
               tsex.Time_Sheet = new Time_Sheet();
               tsex.Time_Sheet.Company_ID = userlogin.Company_ID;
               tsex.Time_Sheet.Employee_Profile_ID = model.Employee_Profile_ID;
               tsex.Time_Sheet.Supervisor = hist.Supervisor;
               tsex.Time_Sheet.Employee_Name = model.Name;
               tsex.Time_Sheet.Update_By = userlogin.User_Authentication.Email_Address;
               tsex.Time_Sheet.Update_On = currentdate;
               tsex.Time_Sheet.Overall_Status = WorkflowStatus.Draft;
               tsex.Time_Sheet.Create_By = userlogin.User_Authentication.Email_Address;
               tsex.Time_Sheet.Create_On = currentdate;         

               tsex.Expenses_Application = new Expenses_Application();
               tsex.Expenses_Application.Company_ID = userlogin.Company_ID;
               tsex.Expenses_Application.Employee_Profile_ID = model.Employee_Profile_ID;
               tsex.Expenses_Application.Supervisor = hist.Supervisor;
               tsex.Expenses_Application.Create_By = userlogin.User_Authentication.Email_Address;
               tsex.Expenses_Application.Create_On = currentdate;
               tsex.Expenses_Application.Expenses_Title = model.Expenses_Title;
               tsex.Expenses_Application.Update_By = userlogin.User_Authentication.Email_Address;
               tsex.Expenses_Application.Update_On = currentdate;
               tsex.Expenses_Application.Overall_Status = WorkflowStatus.Draft;

               if (!string.IsNullOrEmpty(model.Date_Applied))
               {
                  tsex.Expenses_Application.Date_Applied = DateUtil.ToDate(model.Date_Applied);
                  tsex.Doc_Date = DateUtil.ToDate(model.Date_Applied);
               }
               if (!string.IsNullOrEmpty(model.Expenses_No))
               {
                  tsex.Expenses_Application.Expenses_No = model.Expenses_No;
                  tsex.Doc_No = model.Expenses_No;
               }
            
               model.result = tsexService.InsertTsEs(tsex);
               if (model.result.Code != ERROR_CODE.SUCCESS)
               {
                  return errorPage(ERROR_CODE.ERROR_500_DB);
               }
               model.operation = Operation.U;
               pID = tsex.TsEx_ID;
            }
            else
            {
               pID = dup.TsEx_ID;
            }
         }

         if (model.operation == Operation.U)
         {
            #region Edit & View
            var tsex = tsexService.GetTsEx(pID);
            if (tsex != null)
            {
               #region Get Data
               model.TsEx_ID = pID;
               model.Employee_Profile_ID = tsex.Employee_Profile_ID;

               var uService = new UserService();
               var user = uService.getUserByEmployeeProfile(tsex.Employee_Profile_ID);
               if (user != null)
               {
                  model.Profile_ID = user.Profile_ID;
                  model.Name = UserSession.GetUserName(user);
               }

               model.Month = tsex.Month.Value;
               model.Year = tsex.Year.Value;
               model.Date_Applied = DateUtil.ToDisplayDate(tsex.Doc_Date);

               var tsrows = new List<TsRowViewModel>();
               if (tsex.Time_Sheet == null)
                  tsex.Time_Sheet = new Time_Sheet();

               model.Time_Sheet_ID = tsex.Time_Sheet_ID;
               model.TsStatus = tsex.Time_Sheet.Overall_Status;
               model.TsTotal_Amount = tsex.Ts_Total_Amount;
               model.ExTotal_Amount = tsex.Ex_Total_Amount;
               var i = 0;
               foreach (var row in tsex.Time_Sheet.Time_Sheet_Dtl)
               {
                  var tsrow = new TsRowViewModel();
                  tsrow.i = i;
                  tsrow.Time_Sheet_ID = row.Time_Sheet_ID;
                  tsrow.Clock_In = DateUtil.ToDisplayTime(row.Clock_In);
                  tsrow.Clock_Out = DateUtil.ToDisplayTime(row.Clock_Out);
                  tsrow.Customer_Name = row.Customer_Name;
                  tsrow.Date_Of_Date = row.Date_Of_Date.Value.Day.ToString("00") + row.Date_Of_Date.Value.Month.ToString("00") + row.Date_Of_Date.Value.Year;
                  tsrow.Dtl_ID = row.Dtl_ID;
                  tsrow.Duration = DateUtil.ToDisplayTime(row.Duration);
                  tsrow.Hour_Rate = row.Hour_Rate;
                  tsrow.Indent_Name = row.Indent_Name;
                  tsrow.Indent_No = row.Indent_No;
                  tsrow.Job_Cost_ID = row.Job_Cost_ID;
                  tsrow.Launch_Duration = DateUtil.ToDisplayTime(row.Launch_Duration);
                  tsrow.Note = row.Note;
                  tsrow.Total_Amount = row.Total_Amount;

                  tsrows.Add(tsrow);
                  i++;
               }

               model.TsRows = tsrows.ToArray();
               var exrows = new List<ExRowViewModel>();
               if (tsex.Expenses_Application == null)
               {
                  tsex.Expenses_Application = new Expenses_Application();
                  tsex.Expenses_Application.Overall_Status = tsex.Time_Sheet.Overall_Status;
               }
               model.Expenses_ID = tsex.Expenses_Application.Expenses_Application_ID;
               model.Expenses_No = tsex.Expenses_Application.Expenses_No;
               model.Expenses_Title = tsex.Expenses_Application.Expenses_Title;
               model.ExStatus = tsex.Expenses_Application.Overall_Status;
               model.ExCancelStatus = tsex.Expenses_Application.Cancel_Status;

               if (!string.IsNullOrEmpty(tsex.Expenses_Application.Next_Approver))
               {
                  var apps = tsex.Expenses_Application.Next_Approver.Split('|');
                  if (apps.Length > 0)
                  {
                     var ai = 0;
                     foreach (var appr in apps)
                     {
                        var id = NumUtil.ParseInteger(appr);
                        if (id == 0)
                           continue;
                        var next = uService.getUser(id);
                        if (next != null)
                        {
                           if (ai > 0)
                              model.Next_Approver += ", ";
                           model.Next_Approver = UserSession.GetUserName(next);
                        }
                        ai++;
                     }
                  }
               }

               i = 0;
               foreach (var row in tsex.Expenses_Application.Expenses_Application_Document)
               {
                  var exrow = new ExRowViewModel();
                  exrow.i = i;
                  exrow.Amount_Claiming = row.Amount_Claiming;
                  exrow.Expenses_Application_Document_ID = row.Expenses_Application_Document_ID;
                  exrow.Expenses_Config_ID = row.Expenses_Config_ID;
                  exrow.Expenses_Date = DateUtil.ToDisplayDate(row.Expenses_Date);
                  exrow.Doc_No = row.Doc_No;
                  exrow.Expenses_Type_Name = row.Expenses_Config_ID.HasValue ? row.Expenses_Config.Expenses_Name : "";
                  exrow.Expenses_Type_Desc = row.Expenses_Config_ID.HasValue ? row.Expenses_Config.Expenses_Description : "";
                  exrow.Notes = row.Reasons;
                  exrow.Row_Type = UserSession.RIGHT_U;
                  exrow.Selected_Currency = row.Selected_Currency;
                  exrow.Tax = row.Tax;
                  exrow.Ex_Total_Amount = row.Total_Amount;
                  exrow.Department_ID = row.Department_ID;
                  exrow.Ex_Job_Cost_ID = row.Job_Cost_ID;
                  exrow.Withholding_Tax = row.Withholding_Tax;
                  exrow.Tax_Type = row.Tax_Type;
                  exrow.Withholding_Tax_Amount = row.Withholding_Tax_Amount;
                  exrow.Tax_Amount = row.Tax_Amount;
                  exrow.Tax_Amount_Type = row.Tax_Amount_Type;
                  exrow.Withholding_Tax_Type = row.Withholding_Tax_Type;
                  exrow.Job_Cost_Name = (row.Job_Cost_ID.HasValue && row.Job_Cost != null) ? row.Job_Cost.Indent_Name : "";
                  exrow.Job_Cost_No = (row.Job_Cost_ID.HasValue && row.Job_Cost != null) ? row.Job_Cost.Indent_No : "";
                  exrow.ExDate = row.Expenses_Date.Value.Day.ToString("00") + row.Expenses_Date.Value.Month.ToString("00") + row.Expenses_Date.Value.Year;
                  if (row.Upload_Receipt != null && row.Upload_Receipt.Count > 0)
                  {
                     var uploadfile = row.Upload_Receipt.FirstOrDefault();
                     if (uploadfile != null)
                     {
                        exrow.Upload_Receipt_Name = uploadfile.File_Name;
                        exrow.Upload_Receipt_ID = uploadfile.Upload_Receipt_ID;
                        if (uploadfile.Receipt != null)
                           exrow.Upload_Receipt = Convert.ToBase64String(uploadfile.Receipt);
                     }
                  }
                  exrows.Add(exrow);
                  i++;
               }

               model.ExRows = exrows.ToArray();
               #region Workflow
               if (model.ExStatus == WorkflowStatus.Rejected)
               {
                  model.ExStatus = WorkflowStatus.Draft;
                  if (tsex.Expenses_Application != null)
                  {
                     tsex.Expenses_Application.Request_ID = null;
                     tsex.Expenses_Application.Supervisor = null;
                  }
               }

               if (tsex.Expenses_Application.Request_ID.HasValue | tsex.Expenses_Application.Request_Cancel_ID.HasValue)
               {
                  var aService = new SBSWorkFlowAPI.Service();
                  var r = aService.GetMyRequests(userlogin.Company_ID, null, null, ModuleCode.HR, ApprovalType.Expense, model.Expenses_ID);
                  if (r.Item2.IsSuccess && r.Item1 != null)
                  {
                     var exreqs = r.Item1 as List<SBSWorkFlowAPI.Models.Request>;
                     if (exreqs != null)
                     {
                        int? rid = null;
                        if (!string.IsNullOrEmpty(model.ExCancelStatus))
                           rid = tsex.Expenses_Application.Request_Cancel_ID;
                        else
                           rid = tsex.Expenses_Application.Request_ID;

                        var request = exreqs.Where(w => w.Request_ID == rid).FirstOrDefault();
                        if (request != null && request.Task_Assignment != null)
                        {
                           var task = request.Task_Assignment.Where(w => w.Record_Status == WfRecordStatus.Active && w.Profile_ID == userlogin.Profile_ID).OrderBy(o => o.Approval_Level).FirstOrDefault();
                           if (task != null)
                           {
                              model.wfCurrentStatus = WorkFlowCurrentStatus.ApprovalActive;
                              if (request.Status == WorkflowStatus.Rejected)
                                 model.wfCurrentStatus = WorkFlowCurrentStatus.ApprovalInactive;
                           }
                        }
                     }
                  }
               }
               else if (tsex.Expenses_Application.Supervisor == emplogin.Employee_Profile_ID)
               {
                  if (!string.IsNullOrEmpty(model.ExCancelStatus))
                  {
                     if (model.ExCancelStatus == WorkflowStatus.Canceling)
                        model.wfCurrentStatus = WorkFlowCurrentStatus.ApprovalActive;
                  }
                  else
                  {
                     if (model.ExStatus == WorkflowStatus.Pending | model.ExStatus == WorkflowStatus.Approved)
                        model.wfCurrentStatus = WorkFlowCurrentStatus.ApprovalActive;
                  }
               }
               else if (tsex.Employee_Profile_ID == emplogin.Employee_Profile_ID)
               {
                  if (model.ExStatus == WorkflowStatus.Draft)
                     model.wfCurrentStatus = WorkFlowCurrentStatus.EmpActive;
                  else
                  {
                     model.wfCurrentStatus = WorkFlowCurrentStatus.EmpInactive;
                  }
               }


               #endregion
               #endregion
            }
            #endregion
         }

         // model.wfCurrentStatus = WorkFlowCurrentStatus.EmpActive;
         model.cJobCostList = cbService.LstJobCost(userlogin.Company_ID, false);
         return View(model);
      }

      [HttpPost]
      [AllowAuthorized]
      public ActionResult TsExInfo(TsExInfoViewModels model)
      {
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var currentdate = StoredProcedure.GetCurrentDate();
         var jService = new JobCostService();
         var tsexService = new TsExService();
         var hService = new EmploymentHistoryService();
         var cbService = new ComboService();
         var eService = new ExpenseService();
         var empService = new EmployeeService();
         var aService = new SBSWorkFlowAPI.Service();
         var uService = new UserService();

         var hist = hService.GetCurrentEmploymentHistory(model.Employee_Profile_ID);
         if (hist == null)
            return errorPage(ERROR_CODE.ERROR_14_NO_EMPLOYEE_HIST);

         #region Validation
         if (model.TsRows != null)
         {
            var i = 0;
            foreach (var trow in model.TsRows)
            {
               trow.i = i;
               var tdtl = new Time_Sheet_Dtl();
               if (trow.Date_Of_Date.Length != 8)
                  ModelState.AddModelError("Date_Of_Date", Resource.ERROR_514_INVALID_INPUT_ERROR);

               if (string.IsNullOrEmpty(trow.Clock_In))
                  ModelState.AddModelError("Ts" + i + "Clock_In", Resource.Message_Is_Required);
               if (string.IsNullOrEmpty(trow.Clock_Out))
                  ModelState.AddModelError("Ts" + i + "Clock_Out", Resource.Message_Is_Required);

               if (!string.IsNullOrEmpty(trow.Clock_In) && !string.IsNullOrEmpty(trow.Clock_Out))
               {
                  if (DateUtil.ToTime(trow.Clock_Out) <= DateUtil.ToTime(trow.Clock_In))
                  {
                     ModelState.AddModelError("Ts" + i + "Clock_In", Resource.The + " " + Resource.Clock_In + " " + Resource.Field + " " + Resource.Cannot_Be_More_Than_Lower + " " + Resource.The + " " + Resource.Clock_Out);
                     ModelState.AddModelError("Ts" + i + "Clock_Out", Resource.The + " " + Resource.Clock_Out + " " + Resource.Field + " " + Resource.Cannot_Be_Less_Than_Lower + " " + Resource.The + " " + Resource.Clock_In);
                  }

                  if (!string.IsNullOrEmpty(trow.Lunch_In) && !string.IsNullOrEmpty(trow.Lunch_Out))
                  {
                     var lunchin = DateUtil.ToTime(trow.Lunch_In);
                     var lunchout = DateUtil.ToTime(trow.Lunch_Out);
                     if (DateUtil.ToTime(trow.Clock_In) >= lunchin && DateUtil.ToTime(trow.Clock_Out) <= lunchout)
                     {
                        ModelState.AddModelError("Ts" + i + "Clock_In", Resource.Message_The_Time_Range_Is_Invaid);
                        ModelState.AddModelError("Ts" + i + "Clock_Out", Resource.Message_The_Time_Range_Is_Invaid);
                     }
                  }
               }

               var range = model.TsRows.Where(w =>
                  DateUtil.ToTime(w.Clock_In) <= DateUtil.ToTime(trow.Clock_Out) &&
                  DateUtil.ToTime(w.Clock_Out) >= DateUtil.ToTime(trow.Clock_In) &&
                  w.Date_Of_Date == trow.Date_Of_Date &&
                  w.i != i);
               if (range != null && range.Count() > 0)
               {
                  ModelState.AddModelError("Ts" + i + "Clock_In", Resource.Message_The_Time_Range_Is_Duplicate);
                  ModelState.AddModelError("Ts" + i + "Clock_Out", Resource.Message_The_Time_Range_Is_Duplicate);
               }
               i++;
            }
         }
         #endregion

         if (model.Month == 0)
            ModelState.AddModelError("Month", Resource.Message_Is_Required);

         if (model.Year == 0)
            ModelState.AddModelError("Year", Resource.Message_Is_Required);

         if (ModelState.IsValid)
         {
            var hourrate = 0M;
            if (hist.Hour_Rate.HasValue && hist.Hour_Rate.Value > 0)
               hourrate = hist.Hour_Rate.Value;
            else
            {
               hourrate = NumUtil.ParseDecimal(EncryptUtil.Decrypt(hist.Basic_Salary));
               if (hourrate == 0)
                  hourrate = NumUtil.ParseDecimal(EncryptUtil.Decrypt(EncryptUtil.Decrypt(hist.Basic_Salary)));
            }

            var tsex = new TsEX();
            if (model.TsEx_ID.HasValue)
               tsex = tsexService.GetTsEx(model.TsEx_ID);

            tsex.Company_ID = userlogin.Company_ID;
            tsex.Employee_Profile_ID = model.Employee_Profile_ID;
            tsex.Month = model.Month;
            tsex.Year = model.Year;

            #region Timesheet
            if (tsex.Time_Sheet == null)
               tsex.Time_Sheet = new Time_Sheet();
            tsex.Time_Sheet_ID = model.Time_Sheet_ID;
            tsex.Time_Sheet.Company_ID = userlogin.Company_ID;
            tsex.Time_Sheet.Time_Sheet_ID = model.Time_Sheet_ID.HasValue ? model.Time_Sheet_ID.Value : 0;
            tsex.Time_Sheet.Employee_Profile_ID = model.Employee_Profile_ID;
            tsex.Time_Sheet.Supervisor = hist.Supervisor;
            tsex.Time_Sheet.Employee_Name = model.Name;
            tsex.Time_Sheet.Update_By = userlogin.User_Authentication.Email_Address;
            tsex.Time_Sheet.Update_On = currentdate;
            tsex.Time_Sheet.Request_ID = null;
            tsex.Time_Sheet.Request_Cancel_ID = null;
            tsex.Time_Sheet.Overall_Status = null;
            tsex.Time_Sheet.Cancel_Status = null;
            tsex.Time_Sheet.Time_Sheet_Dtl.Clear();
            tsex.Ts_Total_Amount = 0;
            if (model.TsRows != null)
            {
               foreach (var trow in model.TsRows)
               {
                  var tdtl = new Time_Sheet_Dtl();
                  tdtl.Time_Sheet_ID = tsex.Time_Sheet_ID;
                  tdtl.Dtl_ID = trow.Dtl_ID.HasValue ? trow.Dtl_ID.Value : 0;
                  tdtl.Clock_In = DateUtil.ToTime(trow.Clock_In);
                  tdtl.Clock_Out = DateUtil.ToTime(trow.Clock_Out);
                  tdtl.Job_Cost_ID = trow.Job_Cost_ID;
                  tdtl.Note = trow.Note;
                  tdtl.Date_Of_Date = DateUtil.ToDate(trow.Date_Of_Date.Substring(0, 2) + "/" + trow.Date_Of_Date.Substring(2, 2) + "/" + trow.Date_Of_Date.Substring(4, 4));
                  var job = jService.GetJobCost(trow.Job_Cost_ID);
                  if (job != null)
                  {
                     tdtl.Indent_No = job.Indent_No;
                     tdtl.Indent_Name = job.Indent_Name;
                     tdtl.Customer_Name = job.Customer != null ? job.Customer.Customer_Name : "";
                  }
                  var duration = tdtl.Clock_Out.Value.Subtract(tdtl.Clock_In.Value);
                  var lunchduration = DateUtil.ToTime("00:00").Value;
                  if (!string.IsNullOrEmpty(trow.Lunch_In) && !string.IsNullOrEmpty(trow.Lunch_Out))
                  {
                     var lunchin = DateUtil.ToTime(trow.Lunch_In);
                     var lunchout = DateUtil.ToTime(trow.Lunch_Out);
                     if (tdtl.Clock_In < lunchin && tdtl.Clock_Out > lunchin && tdtl.Clock_Out <= lunchout)
                        duration = duration - lunchduration;
                     else if (tdtl.Clock_In < lunchin && tdtl.Clock_Out > lunchout)
                        lunchduration = lunchout.Value.Subtract(lunchin.Value);
                     else if (tdtl.Clock_In >= lunchin && tdtl.Clock_In < lunchout && tdtl.Clock_Out > lunchout)
                        lunchduration = lunchout.Value.Subtract(tdtl.Clock_In.Value);
                  }
                  tdtl.Duration = duration - lunchduration;
                  tdtl.Launch_Duration = lunchduration;
                  tdtl.Hour_Rate = hourrate;

                  var totalamount = duration.Hours * tdtl.Hour_Rate;
                  var minRate = tdtl.Hour_Rate / 60;
                  if (duration.Minutes > 0)
                     totalamount += (duration.Minutes * minRate);

                  tdtl.Total_Amount = totalamount;
                  tsex.Ts_Total_Amount += totalamount;
                  tsex.Time_Sheet.Time_Sheet_Dtl.Add(tdtl);
               }
            }
            #endregion

            #region Expenses
            if (tsex.Expenses_Application == null)
               tsex.Expenses_Application = new Expenses_Application();

            tsex.Expenses_Application_ID = model.Expenses_ID;
            tsex.Expenses_Application.Company_ID = userlogin.Company_ID;
            tsex.Expenses_Application.Expenses_Application_ID = model.Expenses_ID.HasValue ? model.Expenses_ID.Value : 0;
            tsex.Expenses_Application.Employee_Profile_ID = model.Employee_Profile_ID;
            tsex.Expenses_Application.Supervisor = hist.Supervisor;
            if (!string.IsNullOrEmpty(model.Date_Applied))
            {
               tsex.Expenses_Application.Date_Applied = DateUtil.ToDate(model.Date_Applied);
               tsex.Doc_Date = DateUtil.ToDate(model.Date_Applied);
            }

            if (!string.IsNullOrEmpty(model.Expenses_No))
            {
               tsex.Expenses_Application.Expenses_No = model.Expenses_No;
               tsex.Doc_No = model.Expenses_No;
            }

            tsex.Expenses_Application.Expenses_Title = model.Expenses_Title;
            tsex.Expenses_Application.Update_By = userlogin.User_Authentication.Email_Address;
            tsex.Expenses_Application.Update_On = currentdate;
            tsex.Expenses_Application.Request_ID = null;
            tsex.Expenses_Application.Request_Cancel_ID = null;
            tsex.Expenses_Application.Overall_Status = null;
            tsex.Expenses_Application.Cancel_Status = null;
            tsex.Expenses_Application.Expenses_Application_Document.Clear();
            tsex.Ex_Total_Amount = 0;
            if (model.ExRows != null)
            {
               foreach (var erow in model.ExRows)
               {
                  var edtl = new Expenses_Application_Document();
                  edtl.Expenses_Application_ID = tsex.Expenses_Application_ID;
                  edtl.Expenses_Application_Document_ID = erow.Expenses_Application_Document_ID.HasValue ? erow.Expenses_Application_Document_ID.Value : 0;
                  edtl.Amount_Claiming = erow.Amount_Claiming;
                  edtl.Employee_Profile_ID = model.Employee_Profile_ID;
                  edtl.Expenses_Config_ID = erow.Expenses_Config_ID;
                  edtl.Expenses_Date = DateUtil.ToDate(erow.Expenses_Date);
                  edtl.Doc_No = erow.Doc_No;
                  edtl.Reasons = erow.Notes;
                  edtl.Selected_Currency = erow.Selected_Currency;
                  edtl.Tax = erow.Tax;
                  edtl.Total_Amount = erow.Ex_Total_Amount;
                  edtl.Department_ID = hist.Department_ID;
                  edtl.Date_Applied = DateUtil.ToDate(erow.Expenses_Date);
                  edtl.Job_Cost_ID = erow.Ex_Job_Cost_ID;
                  edtl.Withholding_Tax = erow.Withholding_Tax;
                  edtl.Tax_Type = erow.Tax_Type;
                  edtl.Withholding_Tax_Amount = erow.Withholding_Tax_Amount;
                  edtl.Tax_Amount = erow.Tax_Amount;
                  edtl.Tax_Amount_Type = erow.Tax_Amount_Type;
                  edtl.Withholding_Tax_Type = erow.Withholding_Tax_Type;
                  if (!string.IsNullOrEmpty(erow.Upload_Receipt))
                  {
                     var guid = Guid.NewGuid();
                     edtl.Upload_Receipt.Add(new Upload_Receipt()
                     {
                        File_Name = erow.Upload_Receipt_Name,
                        Receipt = Convert.FromBase64String(erow.Upload_Receipt),
                        Upload_Receipt_ID = guid,
                     });
                  }
                  tsex.Ex_Total_Amount += erow.Amount_Claiming;
                  tsex.Expenses_Application.Expenses_Application_Document.Add(edtl);
               }
            }

            #endregion

            if (model.Status == WorkflowStatus.Draft)
            {
               tsex.Time_Sheet.Overall_Status = WorkflowStatus.Draft;
               tsex.Expenses_Application.Overall_Status = WorkflowStatus.Draft;
            }
            else
            {
               tsex.Time_Sheet.Overall_Status = WorkflowStatus.Pending;
               tsex.Expenses_Application.Overall_Status = WorkflowStatus.Pending;
            }

            var haveApprover = true;
            var rworkflow = aService.GetWorkflowByEmployee(userlogin.Company_ID.Value, model.Profile_ID, ModuleCode.HR, ApprovalType.Expense, hist.Department_ID);
            if (!rworkflow.Item2.IsSuccess || rworkflow.Item1 == null || rworkflow.Item1.Count == 0)
               haveApprover = false;

            if (model.operation == UserSession.RIGHT_C)
            {
               tsex.Time_Sheet.Create_By = userlogin.User_Authentication.Email_Address;
               tsex.Time_Sheet.Create_On = currentdate;
               tsex.Expenses_Application.Create_By = userlogin.User_Authentication.Email_Address;
               tsex.Expenses_Application.Create_On = currentdate;

               model.result = tsexService.InsertTsEs(tsex);
               if (model.result.Code == ERROR_CODE.SUCCESS)
               {
                  if (model.Status == WorkflowStatus.Draft)
                     return RedirectToAction(model.ac, model.ctlr, new AppRouteValueDictionary(model));
               }

            }
            else if (model.operation == UserSession.RIGHT_U)
            {
               model.result = tsexService.UpdateTsEx(tsex);
               if (model.result.Code == ERROR_CODE.SUCCESS)
               {
                  if (model.Status == WorkflowStatus.Draft)
                     return RedirectToAction(model.ac, model.ctlr, new AppRouteValueDictionary(model));
               }
            }
            if (model.result.Code == ERROR_CODE.SUCCESS)
            {
               tsex = tsexService.GetTsEx(tsex.TsEx_ID);
               if (haveApprover)
               {
                  #region Workflow
                  var request = new RequestItem();
                  request.Doc_ID = tsex.Expenses_Application_ID;
                  request.Approval_Type = ApprovalType.Expense;
                  request.Company_ID = userlogin.Company_ID.Value;
                  request.Department_ID = hist.Department_ID.HasValue ? hist.Department_ID.Value : 0;
                  request.Module = ModuleCode.HR;
                  request.Requestor_Email = userlogin.User_Authentication.Email_Address;
                  request.Requestor_Name = UserSession.GetUserName(userlogin);
                  request.Requestor_Profile_ID = userlogin.Profile_ID;

                  request.IndentItems = getIndentSupervisor(tsex.Expenses_Application_ID, tsex.Time_Sheet_ID);
                  if (request.IndentItems != null && request.IndentItems.Count > 0)
                     request.Is_Indent = true;

                  var r = aService.SubmitRequest(request);
                  if (r.IsSuccess)
                  {
                     tsex.Expenses_Application.Request_ID = request.Request_ID;
                     tsex.Time_Sheet.Request_ID = request.Request_ID;
                     tsex.Expenses_Application.Overall_Status = request.Status;
                     tsex.Time_Sheet.Overall_Status = request.Status;

                     var exapp = new ExApprover();
                     var mstr = "";
                     exapp.Next_Approver = null;
                     if (request.Is_Indent)
                     {
                        if (request.IndentItems != null)
                        {
                           foreach (var row in request.IndentItems)
                           {
                              if (!row.IsSuccess)
                                 mstr = getApprovalStrIDs(mstr, row.Requestor_Profile_ID.ToString());
                              else
                                 exapp.Approver = getApprovalStrIDs(null, userlogin.Profile_ID.ToString());
                           }
                        }
                     }
                     else
                     {
                        if (request.NextApprover != null)
                        {
                           if (request.NextApprover.Profile_ID == userlogin.Profile_ID && request.Status == WorkflowStatus.Closed)
                              exapp.Approver = getApprovalStrIDs(null, userlogin.Profile_ID.ToString());
                           else
                              mstr = getApprovalStrIDs(null, request.NextApprover.Profile_ID.ToString());
                        }
                     }
                     if (!string.IsNullOrEmpty(mstr))
                        exapp.Next_Approver = mstr;

                     //if (request.Status == WorkflowStatus.Closed)
                     //exapp.Approver = getApprovalStrIDs(null, userlogin.Profile_ID.ToString());

                     model.result = tsexService.UpdateTsExStatus(tsex.TsEx_ID, request.Status, null, request.Request_ID, null, true, exapp);
                     if (model.result.Code == ERROR_CODE.SUCCESS)
                     {
                        if (request.Status == WorkflowStatus.Closed)
                           sendEmpEmail(tsex, userlogin, userlogin, hist, request.Status, request.Reviewers, UserSession.GetUserName(userlogin));
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
                                    param.Add("tsID", tsex.TsEx_ID);
                                    param.Add("appID", row.Requestor_Profile_ID);
                                    param.Add("empID", tsex.Employee_Profile_ID);
                                    param.Add("reqID", tsex.Expenses_Application.Request_ID);
                                    param.Add("status", WorkflowStatus.Approved);
                                    param.Add("code", uService.GenActivateCode("E" + tsex.Expenses_Application_ID + row.Requestor_Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

                                    var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                                    param["status"] = WorkflowStatus.Rejected;
                                    var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                                    var appr = uService.getUser(row.Requestor_Profile_ID, false);
                                    if (appr != null)
                                    {
                                       if (ai != 0)
                                          approverName += " , ";

                                       approverName += UserSession.GetUserName(appr);
                                       sendApprovalEmail(tsex, appr, userlogin, hist, request.Status, request.Reviewers, linkApp, linkRej);
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
                              sendEmpEmail(tsex, userlogin, userlogin, hist, WorkflowStatus.Submitted, request.Reviewers, approverName);

                              #endregion
                           }
                           else
                           {
                              #region Normal flow
                              var param = new Dictionary<string, object>();
                              param.Add("tsID", tsex.TsEx_ID);
                              param.Add("appID", request.NextApprover.Profile_ID);
                              param.Add("empID", tsex.Employee_Profile_ID);
                              param.Add("reqID", tsex.Expenses_Application.Request_ID);
                              param.Add("status", WorkflowStatus.Approved);
                              param.Add("code", uService.GenActivateCode("E" + tsex.Expenses_Application_ID + request.NextApprover.Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

                              var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                              param["status"] = WorkflowStatus.Rejected;
                              var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                              var appr = uService.getUser(request.NextApprover.Profile_ID, false);
                              if (appr != null)
                              {
                                 approverName = UserSession.GetUserName(appr);
                                 sendApprovalEmail(tsex, appr, userlogin, hist, request.Status, request.Reviewers, linkApp, linkRej);
                              }
                              sendEmpEmail(tsex, userlogin, userlogin, hist, WorkflowStatus.Submitted, request.Reviewers, approverName);
                              #endregion
                           }
                        }
                     }

                     return RedirectToAction(model.ac, model.ctlr, new { Code = model.result.Code, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS), Field = model.result.Field });
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
                        var exapp = new ExApprover();
                        exapp.Approver = null;
                        exapp.Next_Approver = getApprovalStrIDs(null, sup.Profile_ID.ToString());
                        model.result = tsexService.UpdateTsExStatus(tsex.TsEx_ID, null, null, null, null, true, exapp);

                        var param = new Dictionary<string, object>();
                        param.Add("tsID", tsex.TsEx_ID);
                        param.Add("appID", sup.Profile_ID);
                        param.Add("empID", tsex.Employee_Profile_ID);
                        param.Add("status", WorkflowStatus.Approved);
                        param.Add("code", uService.GenActivateCode("E" + tsex.Expenses_Application_ID + sup.Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

                        var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                        param["status"] = WorkflowStatus.Rejected;
                        var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                        sendEmpEmail(tsex, userlogin, userlogin, hist, WorkflowStatus.Submitted, null, UserSession.GetUserName(sup.User_Profile));
                        sendApprovalEmail(tsex, sup.User_Profile, userlogin, hist, tsex.Expenses_Application.Overall_Status, null, linkApp, linkRej);

                        return RedirectToAction(model.ac, model.ctlr, new { Code = model.result.Code, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS), Field = model.result.Field });
                     }
                     #endregion
                  }
                  else
                  {
                     #region not flow
                     model.result = tsexService.UpdateTsExStatus(tsex.TsEx_ID, WorkflowStatus.Closed);
                     if (model.result.Code == ERROR_CODE.SUCCESS)
                        sendEmpEmail(tsex, userlogin, userlogin, hist, tsex.Expenses_Application.Overall_Status);

                     return RedirectToAction(model.ac, model.ctlr, new { Code = model.result.Code, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS), Field = model.result.Field });
                     #endregion
                  }
               }
            }
         }
         else
         {
            var err = GetErrorModelState();
         }
         var yearSerivce = empService.GetYearService(model.Employee_Profile_ID);
         model.cJobCostList = cbService.LstJobCost(userlogin.Company_ID, false);
         return View(model);
      }

      [HttpGet]
      public ActionResult TsRow(int pIndex, int pDw, string pDate)
      {
         var model = new TsRowViewModel();
         model.i = pIndex;
         model.Date_Of_Date = pDate;

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return PartialView("_TsRow", model);

         var cbService = new ComboService();
         var empService = new EmployeeService();
         var hService = new EmploymentHistoryService();
         var emp = empService.GetEmployeeProfileByProfileID(userlogin.Profile_ID);
         if (emp == null)
            return PartialView("_TsRow", model);

         var userhist = hService.GetCurrentEmploymentHistory(emp.Employee_Profile_ID);
         if (userhist == null)
            return PartialView("_TsRow", model);
         var wkService = new WorkingDaysService();
         var workdays = wkService.GetWorkingDay(userlogin.Company_ID);
         if (workdays != null)
         {
            if (!workdays.CL_Lunch.HasValue || !workdays.CL_Lunch.Value)
            {
               model.Lunch_In = DateUtil.ToDisplayTime(workdays.ST_Lunch_Time);
               model.Lunch_Out = DateUtil.ToDisplayTime(workdays.ET_Lunch_Time);
            }

            var dw = pDw;
            if (dw == 0)
            {
               if (!workdays.CL_Sun.HasValue || !workdays.CL_Sun.Value)
               {
                  model.Clock_In = DateUtil.ToDisplayTime(workdays.ST_Sun_Time);
                  model.Clock_Out = DateUtil.ToDisplayTime(workdays.ET_Sun_Time);
               }
            }
            else if (dw == 1)
            {
               if (!workdays.CL_Mon.HasValue || !workdays.CL_Mon.Value)
               {
                  model.Clock_In = DateUtil.ToDisplayTime(workdays.ST_Mon_Time);
                  model.Clock_Out = DateUtil.ToDisplayTime(workdays.ET_Mon_Time);
               }
            }
            else if (dw == 2)
            {
               if (!workdays.CL_Tue.HasValue || !workdays.CL_Tue.Value)
               {
                  model.Clock_In = DateUtil.ToDisplayTime(workdays.ST_Tue_Time);
                  model.Clock_Out = DateUtil.ToDisplayTime(workdays.ET_Tue_Time);
               }
            }
            else if (dw == 3)
            {
               if (!workdays.CL_Wed.HasValue || !workdays.CL_Wed.Value)
               {
                  model.Clock_In = DateUtil.ToDisplayTime(workdays.ST_Wed_Time);
                  model.Clock_Out = DateUtil.ToDisplayTime(workdays.ET_Wed_Time);
               }
            }
            else if (dw == 4)
            {
               if (!workdays.CL_Thu.HasValue || !workdays.CL_Thu.Value)
               {
                  model.Clock_In = DateUtil.ToDisplayTime(workdays.ST_Thu_Time);
                  model.Clock_Out = DateUtil.ToDisplayTime(workdays.ET_Thu_Time);
               }
            }
            else if (dw == 5)
            {
               if (!workdays.CL_Fri.HasValue || !workdays.CL_Fri.Value)
               {
                  model.Clock_In = DateUtil.ToDisplayTime(workdays.ST_Fri_Time);
                  model.Clock_Out = DateUtil.ToDisplayTime(workdays.ET_Fri_Time);
               }
            }
            else if (dw == 6)
            {
               if (!workdays.CL_Sat.HasValue || !workdays.CL_Sat.Value)
               {
                  model.Clock_In = DateUtil.ToDisplayTime(workdays.ST_Sat_Time);
                  model.Clock_Out = DateUtil.ToDisplayTime(workdays.ET_Sat_Time);
               }
            }
         }
         model.cJobCostList = cbService.LstJobCost(userlogin.Company_ID, false);
         return PartialView("_TsRow", model);
      }

      private ExRowViewModel ExCal(ExRowViewModel model, int? pExConfID, int? pCurID, decimal? ptotalAmt, string pFullExdate, int? pJID, string pTaxType, decimal? pTax, string pTaxAmtType, decimal? pWhTax, string pWhTaxType, int? pEmpID, int? pPID)
      {
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return model;

         var exCurService = new ExchangeCurrencyConfigService();
         var eService = new ExpenseService();
         var cbService = new ComboService();
         var comService = new CompanyService();
         var histService = new EmploymentHistoryService();
         var currentdate = StoredProcedure.GetCurrentDate();
         var empService = new EmployeeService();

         var expense_Type = eService.GetExpenseType(pExConfID);
         if (expense_Type == null)
            return model;

         var expense_Type_Detail = eService.GetExpensesConfigDetail(pExConfID, pPID);
         if (expense_Type_Detail == null)
            return model;

         var balance = 0M;
         var amountClaiming = 0M;
         var amount = 1;
         var isPerDepartment = false;
         var taxAmount = 0M;
         var _taxAmount = 0M;
         var _WithholdingTaxAmount = 0M;
         var totalAmt = ptotalAmt.HasValue ? ptotalAmt.Value : 0;
         if (pTaxType == TaxType.Exclusive)
         {
            if (totalAmt > 0 && pTax.HasValue)
            {
               if (pTaxAmtType.Equals("%"))
                  _taxAmount = NumUtil.Round(totalAmt * (pTax.Value / 100));
               else
                  _taxAmount = pTax.Value;

               taxAmount = _taxAmount;
            }
         }
         else
         {
            if (pTaxAmtType.Equals("%"))
               _taxAmount = NumUtil.Round(totalAmt * pTax.Value / (pTax.Value + 100));
            else
               _taxAmount = pTax.Value;
         }

         if (pWhTax.HasValue && pWhTax.Value > 0)
         {
            if (pWhTaxType.Equals("%"))
               _WithholdingTaxAmount = NumUtil.Round(totalAmt * (pWhTax.Value / 100));
            else
               _WithholdingTaxAmount = pWhTax.Value;
         }

         totalAmt = totalAmt + (taxAmount - _WithholdingTaxAmount);

         if (expense_Type.Claimable_Type == ClaimableType.Per_Department)
            isPerDepartment = true;

         balance = eService.calulateBalance(expense_Type, expense_Type_Detail, pPID, currentdate);

         if (expense_Type.Is_Accumulative.HasValue && expense_Type.Is_Accumulative.Value)
         {
            int longdate = 0;
            var firstHist = histService.GetFirstEmploymentHistory(pEmpID);

            Nullable<DateTime> StartDate = new DateTime(currentdate.Year, 1, 1);
            if (firstHist != null && firstHist.Effective_Date.HasValue && firstHist.Effective_Date.Value.Year == currentdate.Year)
               StartDate = firstHist.Effective_Date.Value;

            TimeSpan span = currentdate.Date.Subtract(StartDate.Value.Date);
            longdate = (int)span.TotalDays;
            balance = ((balance * (longdate + 1)) / 365);
         }

         decimal totalclaimed = 0;
         var cri = new ExpenseCriteria();
         cri.Company_ID = userlogin.Company_ID;
         cri.Employee_Profile_ID = pEmpID;
         cri.Year = currentdate.Year;
         cri.Closed_Status = true;
         cri.Expenses_Config_ID = pExConfID;
         var closedEx = eService.LstExpenseApplications(cri);
         if (closedEx.Object != null)
         {
            var exs = closedEx.Object as List<Expenses_Application_Document>;
            if (exs != null)
               totalclaimed = exs.Sum(s => s.Amount_Claiming.HasValue ? s.Amount_Claiming.Value : 0);
         }
         balance = balance - totalclaimed;

         var pday = DateUtil.ToDate(pFullExdate);
         var day = currentdate;
         var year = currentdate.Year.ToString();
         var month = currentdate.Month.ToString();
         if (pday != null)
         {
            day = pday.Value;
            year = pday.Value.Year.ToString();
            month = pday.Value.Month.ToString();
         }

         var company_Detail = comService.GetCompany(userlogin.Company_ID).Currency;
         if (company_Detail == null) return null;

         if (expense_Type_Detail.Select_Pecentage.HasValue && expense_Type_Detail.Select_Pecentage.Value)
         {
            var percent = expense_Type_Detail.Pecentage.HasValue ? expense_Type_Detail.Pecentage.Value : 0;
            amountClaiming = NumUtil.Round((totalAmt * percent / 100) * amount);
         }
         else
         {
            if (expense_Type_Detail.Amount >= totalAmt * amount)
               amountClaiming = totalAmt * amount;
            else
            {
               amountClaiming = expense_Type_Detail.Amount.HasValue ? expense_Type_Detail.Amount.Value : 0;
               //กรณีวงเงินเกิน จะทำการหา amount tax & WH tax จากวงเงินที่จ่ายจริง 
               //ข้อมูลในส่วนด้านล่างนี้จะถูกนำไปใช้ในส่วนของ Job cost
               var _amount = 0M;
               _taxAmount = 0;

               if (pWhTax.HasValue && pWhTax.Value > 0)
               {
                  if (pWhTaxType.Equals("%"))
                     _WithholdingTaxAmount = NumUtil.Round(amountClaiming * (pWhTax.Value / 100));
                  else
                     _WithholdingTaxAmount = pWhTax.Value;

                  _amount = amountClaiming + _WithholdingTaxAmount;
               }
               else
                  _amount = amountClaiming;

               if (pTaxAmtType.Equals("%"))
                  _taxAmount = NumUtil.Round(_amount * pTax.Value / (pTax.Value + 100));
               else
                  _taxAmount = pTax.Value;

            }
         }

         if (!isPerDepartment)
         {
            if (balance <= 0)
               amountClaiming = 0;
            else if (balance < amountClaiming)
               amountClaiming = balance;
         }

         amountClaiming = decimal.Round(amountClaiming, 2);
         var jobname = "";
         var jobno = "";
         decimal? Job_Cost_Balance = 0M;
         if (pJID.HasValue && pJID.Value > 0)
         {
            var jobcostService = new JobCostService();
            var job = jobcostService.GetJobCost(pJID);
            if (job != null)
            {
               jobno = job.Indent_No;
               jobname = job.Indent_Name;
               Job_Cost_Balance = (job.Sell_Price - job.Costing);
            }
         }
         if (!string.IsNullOrEmpty(pFullExdate))
         {
            var date = DateUtil.ToDate(pFullExdate);
            model.ExDate = date.Value.Day.ToString("00") + date.Value.Month.ToString("00") + date.Value.Year;
         }
         model.Expenses_Date = pFullExdate;
         model.Expenses_Config_ID = pExConfID;
         model.Ex_Total_Amount = ptotalAmt;
         model.Amount_Claiming = amountClaiming;
         model.Tax = pTax;
         model.Tax_Amount = _taxAmount;
         model.Tax_Amount_Type = pTaxAmtType;
         model.Tax_Type = pTaxType;
         model.Withholding_Tax = pWhTax;
         model.Withholding_Tax_Amount = _WithholdingTaxAmount;
         model.Withholding_Tax_Type = pWhTaxType;
         model.Balance = balance;
         model.Balance_Amount = balance;
         model.Selected_Currency = company_Detail.Currency_ID;
         model.Expenses_Type_Desc = expense_Type.Expenses_Description;
         model.Expenses_Type_Name = expense_Type.Expenses_Name;
         model.Expenses_Config_ID = expense_Type.Expenses_Config_ID;
         model.Ex_Job_Cost_ID = pJID;
         model.Job_Cost_Balance = Job_Cost_Balance;
         model.Job_Cost_Name = jobname;
         model.Job_Cost_No = jobno;
         return model;
      }

      [HttpGet]
      public ActionResult ApplicationConfig(int? pExConfID, int? pCurID, decimal? ptotalAmt, string pFullExdate, int? pJID, string pTaxType, decimal? pTax, string pTaxAmtType, decimal? pWhTax, string pWhTaxType, int? pEmpID, int? pPID)
      {
         var model = new ExRowViewModel();
         ExCal(model, pExConfID, pCurID, ptotalAmt, pFullExdate, pJID, pTaxType, pTax, pTaxAmtType, pWhTax, pWhTaxType, pEmpID, pPID);
         return Json(new
         {
            Amount_Claiming = model.Amount_Claiming,
            Tax_Amount = model.Tax_Amount,
            Withholding_Tax_Amount = model.Withholding_Tax_Amount,
            Balance = model.Balance,
            Balance_Amount = model.Balance_Amount,
            Selected_Currency = model.Selected_Currency,
            Expenses_Type_Desc = model.Expenses_Type_Desc,
            Expenses_Type_Name = model.Expenses_Type_Name,
            Expenses_Config_ID = model.Expenses_Config_ID,
            Job_Cost_Balance = model.Job_Cost_Balance,
            Job_Cost_Name = model.Job_Cost_Name,
            Job_Cost_No = model.Job_Cost_No,
         }, JsonRequestBehavior.AllowGet);
      }

      public ActionResult ExRow(int? pIndex, string pERowID, int? pDocID, string pDocNo, string pNotes, string pRcpName, string pRcp, int? pExConfID, int? pCurID, decimal? ptotalAmt, string pFullExdate, int? pJID, string pTaxType, decimal? pTax, string pTaxAmtType, decimal? pWhTax, string pWhTaxType, int? pEmpID, int? pPID)
      {
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var model = new ExRowViewModel();
         ExCal(model, pExConfID, pCurID, ptotalAmt, pFullExdate, pJID, pTaxType, pTax, pTaxAmtType, pWhTax, pWhTaxType, pEmpID, pPID);
         model.i = pIndex.HasValue ? pIndex.Value : 0;
         model.Expenses_Application_Document_ID = pDocID;
         model.Doc_No = pDocNo;
         model.Notes = pNotes;
         model.erowID = pERowID;
         model.Upload_Receipt = pRcp;
         model.Upload_Receipt_Name = pRcpName;

         if (!string.IsNullOrEmpty(pRcp) && pRcp.Contains("data:"))
         {
            string trimmedData = pRcp;
            var prefixindex = trimmedData.IndexOf(",");
            trimmedData = trimmedData.Substring(prefixindex + 1, trimmedData.Length - (prefixindex + 1));
            var filebyte = Convert.FromBase64String(trimmedData);
            if (filebyte != null)
            {
               model.Upload_Receipt = trimmedData;
               model.Upload_Receipt_Name = pRcpName;
            }
         }
         return PartialView("_ExRow", model);
      }
      public ActionResult ExDlgMngt(int? pDtlID)
      {
         var model = new ExRowViewModel();
         var eService = new ExpenseService();
         var row = eService.getExpenseDtl(pDtlID);
         if (row != null)
         {
            model.Amount_Claiming = row.Amount_Claiming;
            model.Expenses_Application_Document_ID = row.Expenses_Application_Document_ID;
            model.Expenses_Config_ID = row.Expenses_Config_ID;
            model.Expenses_Date = DateUtil.ToDisplayDate(row.Expenses_Date);
            model.Doc_No = row.Doc_No;
            model.Expenses_Type_Name = row.Expenses_Config_ID.HasValue ? row.Expenses_Config.Expenses_Name : "";
            model.Expenses_Type_Desc = row.Expenses_Config_ID.HasValue ? row.Expenses_Config.Expenses_Description : "";
            model.Notes = row.Reasons;
            model.Row_Type = UserSession.RIGHT_U;
            model.Selected_Currency = row.Selected_Currency;
            model.Tax = row.Tax;
            model.Ex_Total_Amount = row.Total_Amount;
            model.Department_ID = row.Department_ID;
            model.Ex_Job_Cost_ID = row.Job_Cost_ID;
            model.Withholding_Tax = row.Withholding_Tax;
            model.Tax_Type = row.Tax_Type;
            model.Withholding_Tax_Amount = row.Withholding_Tax_Amount;
            model.Tax_Amount = row.Tax_Amount;
            model.Tax_Amount_Type = row.Tax_Amount_Type;
            model.Withholding_Tax_Type = row.Withholding_Tax_Type;
            model.Job_Cost_Name = (row.Job_Cost_ID.HasValue && row.Job_Cost != null) ? row.Job_Cost.Indent_Name : "";
            model.Job_Cost_No = (row.Job_Cost_ID.HasValue && row.Job_Cost != null) ? row.Job_Cost.Indent_No : "";
            model.ExDate = row.Expenses_Date.Value.Day.ToString("00") + row.Expenses_Date.Value.Month.ToString("00") + row.Expenses_Date.Value.Year;
            if (row.Upload_Receipt != null && row.Upload_Receipt.Count > 0)
            {
               var uploadfile = row.Upload_Receipt.FirstOrDefault();
               if (uploadfile != null)
               {
                  model.Upload_Receipt_Name = uploadfile.File_Name;
                  model.Upload_Receipt_ID = uploadfile.Upload_Receipt_ID;
                  if (uploadfile.Receipt != null)
                     model.Upload_Receipt = Convert.ToBase64String(uploadfile.Receipt);
               }
            }
         }
         return PartialView("_ExDlgMngt", model);
      }
      public ActionResult ExDlg(string pERowID, int? pEmpID, int? pPID, int? pDID, string pDocNo, string pExdate, string pFullExdate, int? pExConfID, int? pCurID, decimal? ptotalAmt, string pTaxType, decimal? pTax, string pTaxAmtType, decimal? pWhTax, string pWhTaxType, int? pJID, Guid? pRcpID, string pRcpName, string pRcp, string pNote, string pStatus)
      {
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var currentdate = StoredProcedure.GetCurrentDate();
         var cmService = new CompanyService();
         var empService = new EmployeeService();
         var cbService = new ComboService();
         var eService = new ExpenseService();
         var hService = new EmploymentHistoryService();
         var model = new ExRowViewModel();

         var com = cmService.GetCompany(userlogin.Company_ID);
         if (com == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Company);

         var hist = hService.GetEmploymentHistory(pEmpID, currentdate);
         if (hist == null)
            return errorPage(ERROR_CODE.ERROR_14_NO_EMPLOYEE_HIST);

         var yearSerivce = empService.GetYearService(pEmpID);
         model.cJobCostList = cbService.LstJobCost(userlogin.Company_ID, false);
         model.cExpensesConfigList = eService.getExpenseTypes(userlogin.Company_ID.Value, hist.Department_ID, hist.Designation_ID, null, yearSerivce);
         model.cJobCostList = cbService.LstJobCost(userlogin.Company_ID, false);
         model.cTaxTypelst = cbService.LstTaxType(false);
         model.cAmountTypelst = cbService.LstAmountType();

         if (!pExConfID.HasValue)
         {
            var dExConf = model.cExpensesConfigList.FirstOrDefault();
            if (dExConf != null)
               pExConfID = dExConf.Expenses_Config_ID;
         }
         if (!pJID.HasValue)
         {
            var djob = model.cJobCostList.FirstOrDefault();
            if (djob != null)
               pJID = NumUtil.ParseInteger(djob.Value);
         }
         if (string.IsNullOrEmpty(pFullExdate))
         {
            if (!string.IsNullOrEmpty(pExdate))
            {
               if (pExdate.Length == 8)
                  pFullExdate = pExdate.Substring(0, 2) + "/" + pExdate.Substring(2, 2) + "/" + pExdate.Substring(4, 4);
            }
         }
         if (com.Currency != null)
         {
            model.Default_Currency_Code = com.Currency.Currency_Code;
            model.Default_Currency_ID = com.Currency_ID;
            pCurID = com.Currency_ID;
         }
         if (!ptotalAmt.HasValue) ptotalAmt = 0;
         if (!pTax.HasValue) pTax = 0;
         if (!pWhTax.HasValue) pWhTax = 0;
         if (string.IsNullOrEmpty(pTaxType)) pTaxType = TaxType.Exclusive;
         if (string.IsNullOrEmpty(pTaxAmtType)) pTaxAmtType = "%";
         if (string.IsNullOrEmpty(pWhTaxType)) pWhTaxType = "%";
         ExCal(model, pExConfID, pCurID, ptotalAmt, pFullExdate, pJID, pTaxType, pTax, pTaxAmtType, pWhTax, pWhTaxType, pEmpID, pPID);
         model.Expenses_Application_Document_ID = pDID;
         model.erowID = pERowID;
         model.ExDate = pExdate;
         model.Expenses_Date = pFullExdate;
         model.Upload_Receipt_ID = pRcpID;
         model.Upload_Receipt_Name = pRcpName;
         model.Upload_Receipt = pRcp;
         model.Doc_No = pDocNo;
         model.Notes = pNote;
         return PartialView("_ExDlg", model);

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
      public void TsExport(int? pID)
      {
         var tsexService = new TsExService();
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
         var tsex = tsexService.GetTsEx(pID);
         if (tsex != null)
         {
            var dtls = tsex.Time_Sheet.Time_Sheet_Dtl;

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

            var totalamt = 0.00M;
            var user = UserServ.getUserByEmployeeProfile(tsex.Employee_Profile_ID);

            ws.Cells[row, col, row, allcolspan].Style.Font.Bold = true;
            ws.Cells[row, 1].Value = Resource.Employee + " : " + AppConst.GetUserName(user);
            ws.Cells[row, 1, row, allcolspan].Merge = true;
            row++;

            ws.Cells[row, col, row, allcolspan].Style.Font.Bold = true;
            ws.Cells[row, 1].Value = Resource.Year + " : " + tsex.Year;
            ws.Cells[row, 1, row, allcolspan].Merge = true;
            row++;

            ws.Cells[row, col, row, allcolspan].Style.Font.Bold = true;
            ws.Cells[row, 1].Value = Resource.Month + " : " + DateUtil.GetFullMonth(tsex.Month);
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
            ws.Cells[row, 3].Value = Resource.Indent;
            ws.Cells[row, 4].Value = Resource.Job_Detail;
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
            ws.Cells[row, 3].Value = "";
            ws.Cells[row, 4].Value = "";
            ws.Cells[row, 5].Value = Resource.Clock_In;
            ws.Cells[row, 6].Value = Resource.Clock_Out;
            ws.Cells[row, 7].Value = "";
            ws.Cells[row, 8].Value = "";
            ws.Cells[row, 9].Value = "";
            ws.Cells[row, 10].Value = "";
            row++;


            var year = tsex.Year.Value;
            var month = tsex.Month.Value;
            var dayinmonth = DateTime.DaysInMonth(year, month);
            var monthyear = DateUtil.GetFullMonth(month) + " " + year;

            var wService = new WorkingDaysService();
            var wk = wService.GetWorkingDay(userlogin.Company_ID);

            var displaydate = true;
            foreach (var timesheet in dtls)
            {
               var ot = "";
               if (timesheet.Clock_Out.HasValue)
                  ot = getOvertime(DateTime.Parse(DateUtil.ToDisplayTime(timesheet.Clock_Out)), wk);

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
               ws.Cells[row, 2].Value = timesheet.Date_Of_Date.Value.Day + " " + DateUtil.GetFullMonth(timesheet.Date_Of_Date.Value.Month) + " " + timesheet.Date_Of_Date.Value.Year;
               ws.Cells[row, 3].Value = timesheet.Indent_No + ": " + timesheet.Indent_Name;
               ws.Cells[row, 4].Value = timesheet.Note;
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

            row++;
            row++;
            row++;



            ws.Cells[row, col, row, allcolspan].Style.Font.Bold = true;
            ws.Cells[row, 1].Value = Resource.Printed_By;
            ws.Cells[row, 1, row, 2].Merge = true;
            ws.Cells[row, 3].Value = fullName;
            ws.Cells[row, 3, row, 10].Merge = true;

            row++;

            for (var c = 1; c <= allcolspan; c++)
               ws.Column(c).Width = 15;

            ws.Column(2).Width = 20;
            ws.Column(3).Width = 60;
            ws.Column(4).Width = 60;
            pck.SaveAs(Response.OutputStream);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;  filename=" + Resource.Engineering_Expenses_Report + ".xlsx");
         }
      }

      [HttpGet]
      public void SendMail(int pID)
      {
         if (pID == 0)
            return;

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return;

         var tsexService = new TsExService();
         var tsex = tsexService.GetTsEx(pID);
         if (tsex == null)
            return;

         var hService = new EmploymentHistoryService();
         var hist = hService.GetCurrentEmploymentHistory(tsex.Employee_Profile_ID);
         if (hist == null)
            return;

         var empService = new EmployeeService();
         var sup = empService.GetEmployeeProfile2(hist.Supervisor);
         if (sup != null)
         {
            var uService = new UserService();
            var param = new Dictionary<string, object>();
            param.Add("tsID", tsex.TsEx_ID);
            param.Add("appID", sup.Profile_ID);
            param.Add("empID", tsex.Employee_Profile_ID);
            param.Add("status", WorkflowStatus.Approved);
            param.Add("code", uService.GenActivateCode("E" + tsex.Expenses_Application_ID + sup.Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

            var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
            param["status"] = WorkflowStatus.Rejected;
            var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

            sendEmpEmail(tsex, userlogin, userlogin, hist, WorkflowStatus.Submitted, null, UserSession.GetUserName(sup.User_Profile));
            sendApprovalEmail(tsex, sup.User_Profile, userlogin, hist, tsex.Expenses_Application.Overall_Status, null, linkApp, linkRej);
         }

         //sendApprovalEmail(tsex, userlogin, userlogin, hist, tsex.Expenses_Application.Overall_Status);
      }

      #region help tools
      public ActionResult CreateNewWF(int? pid)
      {
         var ids = new List<int> { 644, 645, 713, 701, 711, 714, 703, 707, 702, 672, 712, 716, 704, 708, 710, 722, 719, 718, 721, 706, 717, 729, 732, 730, 738, 741, 742, 745, 749, 748, 743, 744, 735, 746, 747, 754, 757, 753, 751, 759, 758, 756, 752, 755, 1760 };
         Debug.WriteLine("'****738*******APP DEBUG***********' " + DateTime.Now + "-Start Controller Approver Tools");
         var Output = "";
         //Migrate data tool
         var userService = new UserService();
         var hService = new EmploymentHistoryService();
         var aService = new SBSWorkFlowAPI.Service();
         var tsexService = new TsExService();

         var pids = new List<int> { };
         if (pid.HasValue)
            pids.Add(pid.Value);

         var i = 0;
         foreach (var id in ids)
         {
            i++;
            Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-Start  inde-" + i + "   TsEx ID " + id);

            var tsex = tsexService.GetTsEx(id);
            if (tsex == null)
               return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            var hist = hService.GetCurrentEmploymentHistory(tsex.Employee_Profile_ID);
            if (hist == null)
               return errorPage(ERROR_CODE.ERROR_14_NO_EMPLOYEE_HIST);

            var user = userService.getUserByEmployeeProfile(tsex.Employee_Profile_ID);
            if (user == null)
               return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            var request = new RequestItem();
            request.Doc_ID = tsex.Expenses_Application_ID;
            request.Approval_Type = ApprovalType.Expense;
            request.Company_ID = tsex.Company_ID.Value;
            request.Department_ID = hist.Department_ID.HasValue ? hist.Department_ID.Value : 0;
            request.Module = ModuleCode.HR;
            request.Requestor_Email = user.User_Authentication.Email_Address;
            request.Requestor_Name = UserSession.GetUserName(user);
            request.Requestor_Profile_ID = user.Profile_ID;

            request.IndentItems = getIndentSupervisor(tsex.Expenses_Application_ID, tsex.Time_Sheet_ID);
            if (request.IndentItems != null && request.IndentItems.Count > 0)
               request.Is_Indent = true;

            var r = aService.SubmitRequest(request);
            if (r.IsSuccess)
            {
               tsex.Expenses_Application.Request_ID = request.Request_ID;
               tsex.Time_Sheet.Request_ID = request.Request_ID;
               tsex.Expenses_Application.Overall_Status = request.Status;
               tsex.Time_Sheet.Overall_Status = request.Status;

               var exapp = new ExApprover();
               var mstr = "";
               exapp.Next_Approver = null;
               if (request.Is_Indent)
               {
                  if (request.IndentItems != null)
                  {
                     foreach (var row in request.IndentItems)
                     {
                        if (!row.IsSuccess)
                           mstr = getApprovalStrIDs(mstr, row.Requestor_Profile_ID.ToString());
                        else
                           exapp.Approver = getApprovalStrIDs(null, user.Profile_ID.ToString());
                     }
                  }
               }
               else
               {
                  if (request.NextApprover != null)
                  {
                     if (request.NextApprover.Profile_ID == user.Profile_ID && request.Status == WorkflowStatus.Closed)
                        exapp.Approver = getApprovalStrIDs(null, user.Profile_ID.ToString());
                     else
                        mstr = getApprovalStrIDs(null, request.NextApprover.Profile_ID.ToString());
                  }
               }
               if (!string.IsNullOrEmpty(mstr))
                  exapp.Next_Approver = mstr;

               var result = tsexService.UpdateTsExStatus(tsex.TsEx_ID, request.Status, null, request.Request_ID, null, true, exapp);
               if (result != null && result.Code == ERROR_CODE.SUCCESS)
               {
                  //if (request.Status == WorkflowStatus.Closed)
                  //    sendEmpEmail(tsex, user, user, hist, request.Status, request.Reviewers, UserSession.GetUserName(user));
                  //else
                  //{
                  //    var approverName = string.Empty;
                  //    if (request.Is_Indent)
                  //    {
                  //        #region Indent flow
                  //        var ai = 0;
                  //        foreach (var row in request.IndentItems)
                  //        {
                  //            if (!row.IsSuccess)
                  //            {
                  //                var param = new Dictionary<string, object>();
                  //                param.Add("tsID", tsex.TsEx_ID);
                  //                param.Add("appID", row.Requestor_Profile_ID);
                  //                param.Add("empID", tsex.Employee_Profile_ID);
                  //                param.Add("reqID", tsex.Expenses_Application.Request_ID);
                  //                param.Add("status", WorkflowStatus.Approved);
                  //                param.Add("code", userService.GenActivateCode("E" + tsex.Expenses_Application_ID + row.Requestor_Profile_ID + "_", user.User_Authentication.User_Authentication_ID));

                  //                var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                  //                param["status"] = WorkflowStatus.Rejected;
                  //                var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                  //                var appr = userService.getUser(row.Requestor_Profile_ID, false);
                  //                if (appr != null)
                  //                {
                  //                    if (ai != 0)
                  //                        approverName += " , ";

                  //                    approverName += UserSession.GetUserName(appr);
                  //                    sendApprovalEmail(tsex, appr, user, hist, request.Status, request.Reviewers, linkApp, linkRej);
                  //                }
                  //            }
                  //            else
                  //            {
                  //                var appr = userService.getUser(row.Requestor_Profile_ID, false);
                  //                if (appr != null)
                  //                {
                  //                    if (ai != 0)
                  //                        approverName += " , ";

                  //                    approverName += UserSession.GetUserName(appr);
                  //                }
                  //            }
                  //            ai++;
                  //        }
                  //        sendEmpEmail(tsex, user, user, hist, WorkflowStatus.Submitted, request.Reviewers, approverName);

                  //        #endregion
                  //    }
                  //    else
                  //    {
                  //        #region Normal flow
                  //        var param = new Dictionary<string, object>();
                  //        param.Add("tsID", tsex.TsEx_ID);
                  //        param.Add("appID", request.NextApprover.Profile_ID);
                  //        param.Add("empID", tsex.Employee_Profile_ID);
                  //        param.Add("reqID", tsex.Expenses_Application.Request_ID);
                  //        param.Add("status", WorkflowStatus.Approved);
                  //        param.Add("code", userService.GenActivateCode("E" + tsex.Expenses_Application_ID + request.NextApprover.Profile_ID + "_", user.User_Authentication.User_Authentication_ID));

                  //        var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                  //        param["status"] = WorkflowStatus.Rejected;
                  //        var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                  //        var appr = userService.getUser(request.NextApprover.Profile_ID, false);
                  //        if (appr != null)
                  //        {
                  //            approverName = UserSession.GetUserName(appr);
                  //            sendApprovalEmail(tsex, appr, user, hist, request.Status, request.Reviewers, linkApp, linkRej);
                  //        }
                  //        sendEmpEmail(tsex, user, user, hist, WorkflowStatus.Submitted, request.Reviewers, approverName);
                  //        #endregion
                  //    }
                  //}
                  Output = "SUCCESS";
               }
               else
               {
                  Output = "ERROR";
               }
               Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-End TsEx ID" + id + " Status " + Output);
            }
         }
         Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-End Controller Approver Tools");
         return Json(new { Result = Output }, JsonRequestBehavior.AllowGet);
      }
      #endregion
   }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System.Text;
using Excel;
using HR.Models;
using HR.Common;
using SBSModel.Models;
using SBSModel.Common;
using System.Threading.Tasks;
using SBSWorkFlowAPI.Models;
using SBSWorkFlowAPI.ModelsAndService;
using System.Drawing;
using System.Web.Routing;
using OfficeOpenXml;
using SBSResourceAPI;
using SBSWorkFlowAPI.Constants;

namespace HR.Controllers
{
   [Authorize]
   [AllowAuthorized]
   public class LeaveController : ControllerBase
   {
      [HttpGet]
      public ActionResult DashBoard()
      {
         var model = new LeaveDashBoardViewModel();

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //-------rights------------
         RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A, "/Leave/Application");
         if (rightResult.action != null)
            return rightResult.action;
         model.rights = rightResult.rights;

         var comService = new CompanyService();
         var hService = new EmploymentHistoryService();
         var lService = new LeaveService();
         var empService = new EmployeeService();
         var leaveService = new LeaveService();
         var curdate = StoredProcedure.GetCurrentDate();
         var com = comService.GetCompany(userlogin.Company_ID);
         if (com == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         model.LeaveList = lService.LstLeaveApplicationDocument(userlogin.Company_ID, pProfileID: userlogin.Profile_ID);
         var maincri = new LeaveTypeCriteria()
         {
            Company_ID = userlogin.Company_ID,
            Profile_ID = userlogin.Profile_ID,
            Ignore_Generate = true,
            Year = curdate.Year,
         };

         if (model.LeaveList.Count == 0)
         {
            /*first time setting*/
            var cri = new LeaveCalIsExistCriteria() { Profile_ID = userlogin.Profile_ID };
            var calisexsist = lService.LeaveCalIsExist(cri);
            if (!calisexsist)
               maincri.Ignore_Generate = false;
         }

         model.LeaveBalanceList = lService.LstAndCalulateLeaveType(maincri);


         if (com.Currency != null) model.Currency_Code = com.Currency.Currency_Code;
         return View(model);

      }

      #region Configuration

      [HttpGet]
      public ActionResult Configuration(int[] holiday, int[] levType, int[] levAdjust, int[] levAppr, ServiceResult result, LeaveConfigurationViewModel model, string apply, string tabAction = "")
      {
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //Validate Page Right
         RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A);
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;
         model.result = result;

         model.tabAction = tabAction;
         var aService = new SBSWorkFlowAPI.Service();
         var cbService = new ComboService();
         var lService = new LeaveService();

         if (tabAction == "holiday")
         {
            if (apply == UserSession.RIGHT_D)
            {
               apply = RecordStatus.Delete;
               //Added by sun 14-10-2015           
               if (model.Holiday_ID.HasValue)
                  model.result = lService.UpdateDeleteHolidayStatus(model.Holiday_ID, apply, userlogin.User_Authentication.Email_Address);
               //Added by Nay on 13-Jul-2015
               else
               {
                  if (holiday != null)
                     model.result = lService.UpdateMultipleDeleteHolidayStatus(holiday, apply, userlogin.User_Authentication.Email_Address);
               }
            }
         }
         //Added by Nay on 14-Jul-2015 
         //Purpose : multiple delete leave types
         else if (tabAction == "leaveType")
         {
            if (apply == UserSession.RIGHT_D)
            {
               apply = RecordStatus.Delete;
               if (levType != null)
               {
                  var chkVal = false;
                  foreach (var leave in levType)
                  {
                     if (lService.chkLeaveTypeApplied(leave))
                     {
                        chkVal = true;
                        break;
                     }
                  }
                  if (chkVal)
                     return RedirectToAction("Configuration", new { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Leave_Type, tabAction = "leaveType" });
                  else
                     model.result = lService.UpdateMultipleDeleteLeaveTypeStatus(levType, apply, userlogin.User_Authentication.Email_Address);
               }
            }
         }
         else if (tabAction == "adjust")
         {
            if (apply == UserSession.RIGHT_D)
            {
               apply = RecordStatus.Delete;
               if (levAdjust != null)
                  model.result = lService.UpdateMultipleDeleteLeaveAdjStatus(levAdjust, apply, userlogin.User_Authentication.Email_Address);
            }
         }
         else if (tabAction == "approval")
         {
            if (apply == UserSession.RIGHT_D)
            {
               if (levAppr != null)
               {
                  var chkProblem = false;
                  //check first is there any wrong records before delete!
                  foreach (var leave in levAppr)
                  {
                     var rwflow = aService.GetWorkflow(leave);
                     if (rwflow.Item2.IsSuccess && rwflow.Item1 != null)
                     {
                        if (rwflow.Item1.Requests.Count() > 0)
                           return RedirectToAction("Configuration", "Leave", new { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Approval, tabAction = "approval" });
                     }
                  }
                  foreach (var leave in levAppr)
                  {
                     var r = aService.UpdateDeleteWorkFlowStatus(leave, userlogin.Profile_ID, apply);
                     //var r = aService.DeleteWorkFlow(leave, userlogin.Profile_ID);
                     if (!r.IsSuccess)
                     {
                        chkProblem = true;
                        break;
                     }
                  }
                  if (chkProblem)
                     return RedirectToAction("Configuration", "Leave", new { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Approval, tabAction = "approval" });
                  else
                     return RedirectToAction("Configuration", "Leave", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Approval, tabAction = "approval" });
               }
            }
         }

         var curdate = StoredProcedure.GetCurrentDate();
         if (!model.search_Holiday_Year.HasValue)
            model.search_Holiday_Year = curdate.Year;

         model.departmentList = cbService.LstDepartment(userlogin.Company_ID, true);
         model.HolidayList = lService.getHolidays(userlogin.Company_ID.Value, model.search_Holiday_Year);
         model.LeaveTypeList = lService.LstLeaveType(userlogin.Company_ID, model.search_Leave_Leave_Config);
         model.lTypelist = cbService.LstLeaveType(userlogin.Company_ID.Value, pParentOnly: false, hasBlank: true);
         model.LeaveAdjustList = lService.getLeaveAdjustments(userlogin.Company_ID, model.search_Adjust_Year, model.search_Adjust_Leave_Type);

         if (model.search_Approval_Department.HasValue)
         {
            var r = aService.GetWorkflowByDepartment(userlogin.Company_ID.Value, model.search_Approval_Department.Value, ModuleCode.HR, ApprovalType.Leave);
            if (r.Item2.IsSuccess && r.Item1 != null)
               model.ApprovalList = r.Item1;
         }
         else
         {
            var r = aService.GetWorkflowByCompany(userlogin.Company_ID.Value, ModuleCode.HR, ApprovalType.Leave);
            if (r.Item2.IsSuccess && r.Item1 != null)
               model.ApprovalList = r.Item1;
         }
         return View(model);

      }

      [HttpPost]
      [ValidateAntiForgeryToken]
      public ActionResult Configuration(LeaveConfigurationViewModel model, string tabAction = "")
      {
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A);
         if (rightResult.action != null)
            return rightResult.action;
         model.rights = rightResult.rights;

         model.tabAction = tabAction;
         var currentdate = StoredProcedure.GetCurrentDate();
         var lService = new LeaveService();
         var cbService = new ComboService();

         if (tabAction == "holiday")
         {
            if (string.IsNullOrEmpty(model.Holiday_Start_Date))
               ModelState.AddModelError("Holiday_Start_Date", Resource.Message_Is_Required);

            if (string.IsNullOrEmpty(model.Holiday_End_Date))
               ModelState.AddModelError("Holiday_End_Date", Resource.Message_Is_Required);

            if (string.IsNullOrEmpty(model.Holiday_Name))
               ModelState.AddModelError("Holiday_Name", Resource.Message_Is_Required);

            if (DateUtil.ToDate(model.Holiday_Start_Date) == null)
               ModelState.AddModelError("Holiday_Start_Date", Resource.Message_Is_Invalid);

            if (DateUtil.ToDate(model.Holiday_End_Date) == null)
               ModelState.AddModelError("Holiday_End_Date", Resource.Message_Is_Invalid);

            if (!string.IsNullOrEmpty(model.Holiday_End_Date) && !string.IsNullOrEmpty(model.Holiday_Start_Date))
            {
               if (DateUtil.ToDate(model.Holiday_End_Date) < DateUtil.ToDate(model.Holiday_Start_Date))
               {
                  ModelState.AddModelError("Holiday_Start_Date", Resource.Message_Is_Invalid);
                  ModelState.AddModelError("Holiday_End_Date", Resource.Message_Is_Invalid);
               }
            }

            if (ModelState.IsValid)
            {
               var date = DateUtil.ToDate(model.Holiday_Start_Date);
               if (date.HasValue)
               {
                  var hdup = lService.getHoliday(null, model.Holiday_Name, date.Value.Year, userlogin.Company_ID);
                  if (hdup != null)
                  {
                     if (model.Holiday_ID.HasValue)
                     {
                        if (hdup.Holiday_ID != model.Holiday_ID)
                           ModelState.AddModelError("Holiday_Name", Resource.Message_Is_Duplicated);
                     }
                     else
                        ModelState.AddModelError("Holiday_Name", Resource.Message_Is_Duplicated);
                  }
               }

               #region Added by sun 01-12-2015 check duplicated
               var bankHolidays = new List<Holidays>();
               var hilidaylist = lService.getHolidays(userlogin.Company_ID.Value);
               if (hilidaylist != null)
               {
                  foreach (var h in hilidaylist)
                  {
                     if (h.End_Date.HasValue & h.Start_Date.HasValue)
                     {
                        for (var dt = h.Start_Date.Value; dt <= h.End_Date.Value; dt = dt.AddDays(1))
                        {
                           var holiday = new Holidays()
                           {
                              date = dt,
                              name = h.Name,
                              HolidayID = h.Holiday_ID
                           };
                           bankHolidays.Add(holiday);
                        }
                     }
                     else
                     {
                        if (h.Start_Date.HasValue && h.Start_Date.Value != null)
                        {
                           var holiday = new Holidays()
                           {
                              date = h.Start_Date.Value,
                              name = h.Name,
                              HolidayID = h.Holiday_ID
                           };
                           bankHolidays.Add(holiday);
                        }
                     }
                  }

                  if (bankHolidays.Select(s => s.date).Contains(DateUtil.ToDate(model.Holiday_Start_Date).Value))
                  {
                     if (!bankHolidays.Select(s => s.HolidayID).Contains(model.Holiday_ID))
                        ModelState.AddModelError("Holiday_Start_Date", Resource.Message_Is_Duplicated);
                  }
               }

               #endregion

               //Added by sun 04-12-2015
               if (bankHolidays.Select(s => s.date).Contains(DateUtil.ToDate(model.Holiday_End_Date).Value))
               {
                  if (!bankHolidays.Select(s => s.HolidayID).Contains(model.Holiday_ID))
                     ModelState.AddModelError("Holiday_End_Date", Resource.Message_Is_Duplicated);
               }
            }
            #region
            if (ModelState.IsValid)
            {
               if (!model.Holiday_ID.HasValue || model.Holiday_ID.Value == 0)
               {
                  var holiday = new Holiday_Config();
                  holiday.Name = model.Holiday_Name;
                  holiday.Start_Date = DateUtil.ToDate(model.Holiday_Start_Date);
                  holiday.End_Date = DateUtil.ToDate(model.Holiday_End_Date);
                  holiday.Record_Status = RecordStatus.Active;
                  holiday.Create_By = userlogin.User_Authentication.Email_Address;
                  holiday.Create_On = currentdate;
                  holiday.Update_By = userlogin.User_Authentication.Email_Address;
                  holiday.Update_On = currentdate;
                  holiday.Company_ID = userlogin.Company_ID;

                  model.result = lService.InsertHoliday(holiday);
                  if (model.result.Code == ERROR_CODE.SUCCESS)
                     return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "holiday" });
               }
               else
               {
                  var holiday = lService.getHoliday(model.Holiday_ID, null, null, userlogin.Company_ID.Value);
                  if (holiday == null)
                     return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

                  holiday.Name = model.Holiday_Name;
                  holiday.Start_Date = DateUtil.ToDate(model.Holiday_Start_Date);
                  holiday.End_Date = DateUtil.ToDate(model.Holiday_End_Date);
                  holiday.Record_Status = RecordStatus.Active;
                  holiday.Update_By = userlogin.User_Authentication.Email_Address;
                  holiday.Update_On = currentdate;

                  model.result = lService.UpdateHoliday(holiday);
                  if (model.result.Code == ERROR_CODE.SUCCESS)
                     return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "holiday" });
               }
            }
            #endregion
         }

         if (!model.search_Holiday_Year.HasValue)
            model.search_Holiday_Year = currentdate.Year;

         model.departmentList = cbService.LstDepartment(userlogin.Company_ID, true);
         model.HolidayList = lService.getHolidays(userlogin.Company_ID.Value, model.search_Holiday_Year);
         model.LeaveTypeList = lService.LstLeaveType(userlogin.Company_ID, model.search_Leave_Leave_Config);
         model.lTypelist = cbService.LstLeaveType(userlogin.Company_ID.Value, pParentOnly: false, hasBlank: true);
         model.LeaveAdjustList = lService.getLeaveAdjustments(userlogin.Company_ID, model.search_Adjust_Year, model.search_Adjust_Leave_Type);

         var aService = new SBSWorkFlowAPI.Service();
         if (model.search_Approval_Department.HasValue)
         {
            var r = aService.GetWorkflowByDepartment(userlogin.Company_ID.Value, model.search_Approval_Department.Value, ModuleCode.HR, ApprovalType.Leave);
            if (r.Item2.IsSuccess && r.Item1 != null)
               model.ApprovalList = r.Item1;
         }
         else
         {
            var r = aService.GetWorkflowByCompany(userlogin.Company_ID.Value, ModuleCode.HR, ApprovalType.Leave);
            if (r.Item2.IsSuccess && r.Item1 != null)
               model.ApprovalList = r.Item1;
         }
         return View(model);

      }

      public ActionResult AddNewLeaveDetail(int pIndex, string pDesignation, Nullable<int> pYear, Nullable<decimal> pAmount)
      {

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var cbService = new ComboService();
         var companyService = new CompanyService();
         var model = new LeaveConfigDetailViewModel()
         {
            Index = pIndex,
            Year_Service = pYear,
            Default_Leave_Amount = pAmount,
         };

         pDesignation = Request.Params.Get("pDesignation[]");
         if (!string.IsNullOrEmpty(pDesignation))
         {
            var desinations = pDesignation.Split(',').Select(x => Int32.Parse(x)).ToArray();
            model.Designations = desinations;
         }
         model.designationList = cbService.LstDesignation(userlogin.Company_ID, true);

         return PartialView("LeaveTypeDetailRow", model);
      }

      public ActionResult AddNewLeaveExtra(int pIndex)
      {
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var cbService = new ComboService();
         var model = new LeaveConfigExtraViewModel()
         {
            Index = pIndex,
         };

         model.empList = cbService.LstEmployee(userlogin.Company_ID);
         model.adjustmentTypeList = cbService.LstAdjustmentType();

         return PartialView("LeaveTypeExtraRow", model);
      }

      [HttpGet]
      public ActionResult LeaveType(string lid, string operation)
      {

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var ltypeID = NumUtil.ParseInteger(EncryptUtil.Decrypt(lid));
         LeaveTypeViewModel model = new LeaveTypeViewModel();
         var userService = new UserService();
         var lService = new LeaveService();
         var cbService = new ComboService();
         model.operation = EncryptUtil.Decrypt(operation);

         //Validate Page Right
         RightResult rightResult = base.validatePageRight(model.operation, "/Leave/Configuration");
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;

         model.designationList = cbService.LstDesignation(userlogin.Company_ID, true);
         model.monthList = cbService.LstMonth(true);
         model.genderList = cbService.LstLookup(ComboType.Gender, userlogin.Company_ID);
         model.maritalList = cbService.LstLookup(ComboType.Marital_Status, userlogin.Company_ID);
         model.empList = cbService.LstEmployee(userlogin.Company_ID);
         model.adjustmentTypeList = cbService.LstAdjustmentType();
         model.Is_Default = false;

         /*Added By Jane 03/02/2016*/
         model.relatedtoList = cbService.LstLeaveType(userlogin.Company_ID, pConfType: LeaveConfigType.Normal, pParentOnly: true, hasBlank: true);

         var com = new CompanyService().GetCompany(userlogin.Company_ID);
         if (com != null)
         {
            if (com.Currency != null)
               model.Company_Currency = com.Currency.Currency_Code;
         }

         if (userlogin != null)
         {

            if (model.operation == UserSession.RIGHT_C)
               model.leaveTypeList = cbService.LstLeaveType(userlogin.Company_ID, ltypeID);
            else if (model.operation == UserSession.RIGHT_U)
            {
               model.leaveTypeList = cbService.LstLeaveType(userlogin.Company_ID, ltypeID);
               var comService = new CompanyService();
               Leave_Config leave = lService.getLeaveType(ltypeID);

               //Chk right on Leave_Config (of the company)
               if (leave == null)
                  return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);
               if (leave.Company_ID != userlogin.Company_ID)
                  return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

               if (com != null)
               {
                  model.Fiscal_Year = com.Default_Fiscal_Year.HasValue ? com.Default_Fiscal_Year.Value : true;
                  model.Custom_Fiscal_Year = com.Custom_Fiscal_Year != null ? com.Custom_Fiscal_Year.Value.Day.ToString("00") + "/" + com.Custom_Fiscal_Year.Value.Month.ToString("00") : "";
               }

               model.lid = leave.Leave_Config_ID;
               model.Leave_Name = leave.Leave_Name;
               model.Type = leave.Type;
               model.Deduct_In_Payroll = leave.Deduct_In_Payroll.Value;
               model.Allowed_Notice_Period = leave.Allowed_Notice_Period;
               model.Is_Default = leave.Is_Default.HasValue ? leave.Is_Default.Value : false;
               model.Leave_Config_Parent_ID = leave.Leave_Config_Parent_ID;

               if (leave.Allowed_Probation.HasValue)
                  model.Allowed_Probation = leave.Allowed_Probation.Value;

               model.Bring_Forward = leave.Bring_Forward.Value;
               if (!model.Bring_Forward)
               {
                  model.Months_To_Expiry = 0;
                  model.Bring_Forward_Percent = 0;
                  model.Bring_Forward_Days = 0;
                  model.Is_Bring_Forward_Days = false;
               }
               else
               {
                  model.Is_Bring_Forward_Days = leave.Is_Bring_Forward_Days.HasValue ? leave.Is_Bring_Forward_Days.Value : false;
                  if (model.Is_Bring_Forward_Days)
                     model.Bring_Forward_Days = leave.Bring_Forward_Days.HasValue ? leave.Bring_Forward_Days.Value : 0;
                  else
                     model.Bring_Forward_Percent = leave.Bring_Forward_Percent;
                  model.Months_To_Expiry = leave.Months_To_Expiry.HasValue ? leave.Months_To_Expiry.Value : 1;

               }
               model.Upload_Document = leave.Upload_Document.Value;
               model.Leave_Description = leave.Leave_Description;


               //Added by sun 10-09-2016
               model.Is_Accumulative = leave.Is_Accumulative.HasValue ? leave.Is_Accumulative.Value : false;

               var leaveTypeChilds = new List<LeaveConfigDetailViewModel>();
               var currGroup = 0;
               foreach (var row in leave.Leave_Config_Detail.OrderBy(o => o.Group_ID))
               {
                  if (row.Group_ID.HasValue)
                  {
                     if (currGroup != row.Group_ID)
                     {
                        currGroup = row.Group_ID.Value;
                        var detailsrows = leave.Leave_Config_Detail.Where(w => w.Group_ID == currGroup);
                        var ltypeDetail = new LeaveConfigDetailViewModel()
                        {
                           Default_Leave_Amount = row.Default_Leave_Amount,
                           Designations = detailsrows.Select(s => s.Designation_ID.HasValue ? s.Designation_ID.Value : 0).ToArray(),
                           Group_ID = row.Group_ID,
                           Leave_Config_ID = row.Leave_Config_ID,
                           Year_Service = row.Year_Service
                        };
                        leaveTypeChilds.Add(ltypeDetail);

                     }
                  }
               }

               var leaveTypeExtras = new List<LeaveConfigExtraViewModel>();
               foreach (var row in leave.Leave_Config_Extra)
               {
                  var extra = new LeaveConfigExtraViewModel();
                  extra.Leave_Config_Extra_ID = row.Leave_Config_Extra_ID;
                  extra.Leave_Config_ID = row.Leave_Config_ID;
                  extra.No_Of_Days = row.No_Of_Days;
                  extra.Row_Type = RowType.EDIT;
                  extra.Employee_Profile_ID = row.Employee_Profile_ID;
                  extra.Adjustment_Type = row.Adjustment_Type;
                  leaveTypeExtras.Add(extra);
               }

               model.Detail_Rows = leaveTypeChilds.ToArray();
               model.Extra_Rows = leaveTypeExtras.ToArray();
               model.Condition_Rows = leave.Leave_Config_Condition.Select(s => s.Lookup_Data_ID.HasValue ? s.Lookup_Data_ID.Value : 0).ToArray();

            }
            else if (model.operation == UserSession.RIGHT_D)
            {
               //Added by Nay on 14-Jul-2015 
               //Purpose : to check current leave type is already applied in Leave Application or not. 
               if (lService.chkLeaveTypeApplied(ltypeID))
                  return RedirectToAction("Configuration", new { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Leave_Type, tabAction = "leaveType" });
               else
               {
                  //model.result = lService.DeleteLeaveType(ltypeID);

                  var apply = RecordStatus.Delete;
                  model.result = lService.UpdateDeleteLeaveTypeStatus(ltypeID, apply, userlogin.User_Authentication.Email_Address);
                  if (model.result.Code == ERROR_CODE.SUCCESS)
                     return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "leaveType" });
               }
            }
         }

         return View(model);
      }
      [HttpPost]
      [ValidateAntiForgeryToken]
      public ActionResult LeaveType(LeaveTypeViewModel model)
      {

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var rightResult = base.validatePageRight(model.operation, "/Leave/Configuration");
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;

         var lService = new LeaveService();
         var currentdate = StoredProcedure.GetCurrentDate();

         if (string.IsNullOrEmpty(model.Leave_Name))
            ModelState.AddModelError("Leave_Name", Resource.Message_Is_Required);

         if (model.Leave_Config_Parent_ID.HasValue && model.Leave_Config_Parent_ID.Value > 0)
         {
         }
         else
         {
            if (model.Bring_Forward)
            {
               if (model.Is_Bring_Forward_Days)
               {
                  if (model.Bring_Forward_Days <= 0)
                     ModelState.AddModelError("Bring_Forward_Days", Resource.Message_Is_Required);
               }
               else
               {
                  if (!model.Bring_Forward_Percent.HasValue || model.Bring_Forward_Percent.Value <= 0)
                     ModelState.AddModelError("Bring_Forward_Percent", Resource.Message_Is_Required);
                  else if (model.Bring_Forward_Percent.HasValue && model.Bring_Forward_Percent.Value > 100)
                     ModelState.AddModelError("Bring_Forward_Percent", Resource.Message_Is_Invalid);
               }

               if (model.Months_To_Expiry <= 0)
                  ModelState.AddModelError("Months_To_Expiry", Resource.Message_Is_Required);
            }
         }

         if (model.Detail_Rows == null || model.Detail_Rows.Length == 0)
            ModelState.AddModelError("Detail_Rows", Resource.The + " " + Resource.Leave_Type + " " + Resource.By_Designation + " " + Resource.Is_Rrequired_Lower);
         else
         {
            for (var i = 0; i < model.Detail_Rows.Length; i++)
            {
               var row = model.Detail_Rows[i];
               var type = model.Detail_Rows[i].Row_Type;
               if (type != RowType.DELETE)
               {
                  if (!row.Year_Service.HasValue || row.Year_Service.Value < 0)
                     ModelState.AddModelError("Detail_Rows[" + i + "].Year_Service", Resource.Message_Is_Required);

                  if (!row.Default_Leave_Amount.HasValue || row.Default_Leave_Amount.Value <= 0)
                     ModelState.AddModelError("Detail_Rows[" + i + "].Default_Leave_Amount", Resource.Message_Is_Required);

                  if (row.Designations == null || row.Designations.Count() == 0)
                     ModelState.AddModelError("Detail_Rows[" + i + "].Designations", Resource.Message_Is_Required);
                  else
                  {
                     foreach (var drow in row.Designations)
                     {
                        var duprows = model.Detail_Rows.Where(w => w.Year_Service == row.Year_Service & w.Group_ID != row.Group_ID & w.Row_Type != RowType.DELETE);
                        if (drow == 0)
                        {
                           // All
                           var dup = duprows.FirstOrDefault();
                           if (dup != null)
                              ModelState.AddModelError("Detail_Rows[" + i + "].Designations", Resource.The + " " + Resource.Designation + " " + Resource.Field + " " + Resource.And + " " + Resource.Years_Services + " " + Resource.Field + " " + Resource.Is_Duplicated_Lower);
                        }
                        else
                        {
                           var dup = duprows.Where(w => (w.Designations != null ? w.Designations.Contains(drow) : false)).FirstOrDefault();
                           if (dup != null)
                              ModelState.AddModelError("Detail_Rows[" + i + "].Designations", Resource.The + " " + Resource.Designation + " " + Resource.Field + " " + Resource.And + " " + Resource.Years_Services + " " + Resource.Field + " " + Resource.Is_Duplicated_Lower);
                        }
                     }
                  }
               }
            }
         }

         var dupleave = lService.LstLeaveType(userlogin.Company_ID, null, model.Leave_Name).FirstOrDefault();
         if (dupleave != null)
         {
            if (model.operation == UserSession.RIGHT_C)
               ModelState.AddModelError("Leave_Name", Resource.Message_Is_Duplicated);
            else if (model.operation == UserSession.RIGHT_U)
            {
               if (dupleave.Leave_Config_ID != model.lid)
                  ModelState.AddModelError("Leave_Name", Resource.Message_Is_Duplicated);
            }
         }
         if (ModelState.IsValid)
         {
            Leave_Config leave = new Leave_Config();
            if (model.operation == UserSession.RIGHT_U)
               leave = lService.getLeaveType(model.lid);

            leave.Leave_Name = model.Leave_Name;
            leave.Deduct_In_Payroll = model.Deduct_In_Payroll;
            leave.Allowed_Probation = model.Allowed_Probation;
            leave.Allowed_Notice_Period = model.Allowed_Notice_Period;
            leave.Upload_Document = model.Upload_Document;
            leave.Leave_Description = model.Leave_Description;
            leave.Update_By = userlogin.User_Authentication.Email_Address;
            leave.Update_On = currentdate;
            leave.Company_ID = userlogin.Company_ID;
            leave.Type = LeaveConfigType.Normal;
            leave.Update_By = userlogin.User_Authentication.Email_Address;
            leave.Update_On = currentdate;
            leave.Record_Status = RecordStatus.Active;

            leave.Is_Accumulative = model.Is_Accumulative;

            leave.Leave_Config_Parent_ID = model.Leave_Config_Parent_ID;
            if (leave.Leave_Config_Parent_ID.HasValue && leave.Leave_Config_Parent_ID.Value > 0)
            {
               leave.Is_Bring_Forward_Days = false;
               leave.Months_To_Expiry = null;
               leave.Bring_Forward_Percent = null;
               leave.Bring_Forward_Days = null;
               leave.Bring_Forward = false;
            }
            else
            {
               leave.Bring_Forward = model.Bring_Forward;
               if (!model.Bring_Forward)
               {
                  leave.Is_Bring_Forward_Days = false;
                  leave.Months_To_Expiry = null;
                  leave.Bring_Forward_Percent = null;
                  leave.Bring_Forward_Days = null;
               }
               else
               {
                  if (model.Is_Bring_Forward_Days)
                     leave.Bring_Forward_Days = model.Bring_Forward_Days;
                  else
                     leave.Bring_Forward_Percent = model.Bring_Forward_Percent;
                  leave.Months_To_Expiry = model.Months_To_Expiry;
                  leave.Is_Bring_Forward_Days = model.Is_Bring_Forward_Days;
               }
            }


            if (model.operation == UserSession.RIGHT_C)
            {
               leave.Create_By = userlogin.User_Authentication.Email_Address;
               leave.Create_On = currentdate;

               if (model.Extra_Rows != null)
               {
                  foreach (var row in model.Extra_Rows)
                  {
                     if (row.Row_Type == RowType.ADD)
                     {
                        var extra = new Leave_Config_Extra();
                        extra.Adjustment_Type = row.Adjustment_Type;
                        extra.Employee_Profile_ID = row.Employee_Profile_ID;
                        extra.Leave_Config_ID = row.Leave_Config_ID;
                        extra.No_Of_Days = row.No_Of_Days;
                        leave.Leave_Config_Extra.Add(extra);
                     }
                  }
               }
               var lConfigs = new List<_Leave_Config>();
               if (model.Detail_Rows != null)
               {
                  foreach (var row in model.Detail_Rows)
                  {
                     lConfigs.Add(new _Leave_Config()
                     {
                        Default_Leave_Amount = row.Default_Leave_Amount,
                        Designations = row.Designations,
                        Group_ID = row.Group_ID,
                        Leave_Config_ID = row.Leave_Config_ID,
                        Year_Service = row.Year_Service,
                        Row_Type = row.Row_Type,
                     });
                  }
               }

               model.result = lService.InsertLeaveType(leave, lConfigs, model.Condition_Rows);
               if (model.result.Code == ERROR_CODE.SUCCESS)
               {
                  var rvalue = new RouteValueDictionary(model.result);
                  rvalue.Add("tabAction", "leaveType");
                  return RedirectToAction("Configuration", rvalue);
               }
            }
            else if (model.operation == UserSession.RIGHT_U)
            {
               leave.Leave_Config_Extra.Clear();
               if (model.Extra_Rows != null)
               {
                  foreach (var row in model.Extra_Rows)
                  {
                     if (row.Row_Type == RowType.ADD)
                     {
                        var extra = new Leave_Config_Extra();
                        extra.Adjustment_Type = row.Adjustment_Type;
                        extra.Employee_Profile_ID = row.Employee_Profile_ID;
                        extra.Leave_Config_ID = leave.Leave_Config_ID;
                        extra.No_Of_Days = row.No_Of_Days;
                        leave.Leave_Config_Extra.Add(extra);
                     }
                     else if (row.Row_Type == RowType.EDIT)
                     {
                        var extra = new Leave_Config_Extra();
                        extra.Adjustment_Type = row.Adjustment_Type;
                        extra.Employee_Profile_ID = row.Employee_Profile_ID;
                        extra.Leave_Config_Extra_ID = row.Leave_Config_Extra_ID.Value;
                        extra.Leave_Config_ID = row.Leave_Config_ID;
                        extra.No_Of_Days = row.No_Of_Days;
                        leave.Leave_Config_Extra.Add(extra);
                     }
                  }
               }
               var lConfigs = new List<_Leave_Config>();
               if (model.Detail_Rows != null)
               {
                  foreach (var row in model.Detail_Rows)
                  {
                     lConfigs.Add(new _Leave_Config()
                     {
                        Default_Leave_Amount = row.Default_Leave_Amount,
                        Designations = row.Designations,
                        Group_ID = row.Group_ID,
                        Leave_Config_ID = row.Leave_Config_ID,
                        Year_Service = row.Year_Service,
                        Row_Type = row.Row_Type,
                     });
                  }
               }
               model.result = lService.UpdateLeaveType(leave, lConfigs, model.Condition_Rows);
               if (model.result.Code == ERROR_CODE.SUCCESS)
               {
                  var rvalue = new RouteValueDictionary(model.result);
                  rvalue.Add("tabAction", "leaveType");
                  return RedirectToAction("Configuration", rvalue);
               }

            }
         }

         var userService = new UserService();
         var cbService = new ComboService();
         model.leaveTypeList = cbService.LstLeaveType(userlogin.Company_ID, model.lid);
         model.designationList = cbService.LstDesignation(userlogin.Company_ID, true);
         model.monthList = cbService.LstMonth(true);
         model.genderList = cbService.LstLookup(ComboType.Gender, userlogin.Company_ID);
         model.maritalList = cbService.LstLookup(ComboType.Marital_Status, userlogin.Company_ID);
         model.empList = cbService.LstEmployee(userlogin.Company_ID);
         model.adjustmentTypeList = cbService.LstAdjustmentType();

         /*Added By Jane 03/02/2016*/
         model.relatedtoList = cbService.LstLeaveType(userlogin.Company_ID, pConfType: LeaveConfigType.Normal, pParentOnly: true, hasBlank: true);
         return View(model);
      }


      [HttpGet]
      public ActionResult LeaveTypeChild(string lid, string operation)
      {

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var ltypeID = NumUtil.ParseInteger(EncryptUtil.Decrypt(lid));
         var model = new LeaveTypeChildViewModel();
         var lService = new LeaveService();
         var userService = new UserService();
         var cbService = new ComboService();
         model.operation = EncryptUtil.Decrypt(operation);

         //Validate Page Right
         var rightResult = base.validatePageRight(model.operation, "/Leave/Configuration");
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;

         model.monthList = cbService.LstMonth(true);
         model.genderList = cbService.LstLookup(ComboType.Gender, userlogin.Company_ID);
         model.maritalList = cbService.LstLookup(ComboType.Marital_Status, userlogin.Company_ID);
         model.residentalStatusList = cbService.LstResidentialStatus();
         model.periodList = cbService.LstPeriod(false, true, false, false, false);

         model.Is_Default = false;
         model.Type = LeaveConfigType.Normal;
         if (userlogin != null)
         {
            if (model.operation == UserSession.RIGHT_U)
            {
               Leave_Config leave = lService.getLeaveType(ltypeID);
               if (leave == null)
                  return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);
               if (leave.Company_ID != userlogin.Company_ID)
                  return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

               model.lid = leave.Leave_Config_ID;
               model.Leave_Name = leave.Leave_Name;
               model.Leave_Description = leave.Leave_Description;
               model.Flexibly = leave.Flexibly.HasValue ? leave.Flexibly.Value : false;
               model.Continuously = leave.Continuously.HasValue ? leave.Continuously.Value : false;
               model.Valid_Period = leave.Valid_Period.HasValue ? leave.Valid_Period.Value : 1;
               model.Is_Default = leave.Is_Default;
               model.Type = leave.Type;
               model.Condition_Rows = leave.Leave_Config_Condition.Select(s => s.Lookup_Data_ID.HasValue ? s.Lookup_Data_ID.Value : 0).ToArray();

               var details = new List<Leave_Config_Child_Detail>();
               foreach (var rstatus in model.residentalStatusList)
               {
                  var detail = leave.Leave_Config_Child_Detail.Where(w => w.Residential_Status == rstatus.Value).FirstOrDefault();
                  if (detail == null)
                  {
                     detail = new Leave_Config_Child_Detail()
                     {
                        Leave_Config_ID = leave.Leave_Config_ID,
                        Residential_Status = rstatus.Value
                     };
                  }
                  details.Add(detail);
               }

               model.Detail_Rows = details.ToArray();
            }
         }

         return View(model);
      }
      [HttpPost]
      [ValidateAntiForgeryToken]
      public ActionResult LeaveTypeChild(LeaveTypeChildViewModel model)
      {

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         LeaveService lService = new LeaveService();
         var userService = new UserService();
         var currentdate = StoredProcedure.GetCurrentDate();
         //Validate Page Right

         RightResult rightResult = base.validatePageRight(model.operation, "/Leave/Configuration");
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;

         if (ModelState.IsValid)
         {
            if (model.operation == UserSession.RIGHT_U)
            {

               Leave_Config leave = lService.getLeaveType(model.lid);

               //Chk right on Leave_Config (of the company)
               if (leave == null)
                  return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);
               if (leave.Company_ID != userlogin.Company_ID)
                  return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

               leave.Leave_Name = model.Leave_Name;
               leave.Leave_Description = model.Leave_Description;
               leave.Flexibly = model.Flexibly;
               leave.Continuously = model.Continuously;
               leave.Valid_Period = model.Valid_Period;
               leave.Update_By = userlogin.User_Authentication.Email_Address;
               leave.Update_On = currentdate;
               leave.Type = model.Type;
               var cons = new List<Leave_Config_Condition>();
               if (model.Condition_Rows != null)
               {
                  foreach (var conID in model.Condition_Rows)
                  {
                     cons.Add(new Leave_Config_Condition() { Leave_Config_ID = leave.Leave_Config_ID, Lookup_Data_ID = conID });
                  }
               }
               leave.Leave_Config_Condition = cons;
               leave.Leave_Config_Child_Detail = model.Detail_Rows;

               model.result = lService.UpdateLeaveTypeDefault(leave);
               if (model.result.Code == ERROR_CODE.SUCCESS)
                  return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "leaveType" });
            }
         }

         var cbService = new ComboService();
         model.monthList = cbService.LstMonth(true);
         model.genderList = cbService.LstLookup(ComboType.Gender, userlogin.Company_ID);
         model.maritalList = cbService.LstLookup(ComboType.Marital_Status, userlogin.Company_ID);
         model.residentalStatusList = cbService.LstResidentialStatus();
         model.periodList = cbService.LstPeriod(false, true, false, false, false);

         return View(model);
      }

      [HttpGet]
      public ActionResult LeaveAdjustment(string aid, string operation)
      {

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var adjID = NumUtil.ParseInteger(EncryptUtil.Decrypt(aid));
         LeaveAdjustmentViewModel model = new LeaveAdjustmentViewModel();
         LeaveService lService = new LeaveService();
         LeaveService leaveService = new LeaveService();
         var currentdate = StoredProcedure.GetCurrentDate();
         var userService = new UserService();
         var cbService = new ComboService();
         model.operation = EncryptUtil.Decrypt(operation);

         //Validate Page Right
         RightResult rightResult = base.validatePageRight(model.operation, "/Leave/Configuration");
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;

         model.leaveTypeList = cbService.LstLeaveType(userlogin.Company_ID.Value);
         model.employeeList = cbService.LstEmployeeList(userlogin.Company_ID.Value);
         model.departmentList = cbService.LstDepartment(userlogin.Company_ID);
         model.Year_2 = currentdate.Year;

         if (model.result == null) model.result = new ServiceResult();

         if (model.operation != UserSession.RIGHT_C && userlogin != null)
         {
            if (model.operation == UserSession.RIGHT_U)
            {
               Leave_Adjustment leave = lService.getLeaveAdjustment(adjID);

               //Chk right on Leave_Approval (of the company)
               if (leave == null)
                  return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);
               if (leave.Company_ID != userlogin.Company_ID)
                  return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);
               if (!leave.Employee_Profile_ID.HasValue)
                  model.Employee_Profile_ID = 0;
               else
                  model.Employee_Profile_ID = leave.Employee_Profile_ID.Value;
               model.Leave_Config_ID = leave.Leave_Config.Leave_Config_ID;
               model.Leave_Name = leave.Leave_Config.Leave_Name;
               model.Adjustment_Amount = leave.Adjustment_Amount.Value;
               model.Reason = leave.Reason;
               model.Year_2 = leave.Year_2.Value;
               model.Create_By = leave.Create_By;
               model.Create_On = leave.Create_On.Value;
               model.aid = leave.Adjustment_ID;
               model.Department_ID = leave.Department_ID;
            }
            else if (model.operation == UserSession.RIGHT_D)
            {
               //model.result = lService.DeleteLeaveAdjustment(adjID);
               var apply = RecordStatus.Delete;
               model.result = leaveService.UpdateDeleteLeaveAdjStatus(adjID, apply, userlogin.User_Authentication.Email_Address);
               if (model.result.Code == ERROR_CODE.SUCCESS)
                  return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "adjust" });
            }
         }

         return View(model);
      }
      [HttpPost]
      [ValidateAntiForgeryToken]
      public ActionResult LeaveAdjustment(LeaveAdjustmentViewModel model)
      {

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //Validate Page Right
         RightResult rightResult = base.validatePageRight(model.operation, "/Leave/Configuration");
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;

         var cbService = new ComboService();
         var userService = new UserService();
         var lService = new LeaveService();

         if (model.Adjustment_Amount == 0)
            ModelState.AddModelError("Adjustment_Amount", Resource.Message_Is_Required);

         if (ModelState.IsValid)
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            if (model.operation == UserSession.RIGHT_C)
            {
               Leave_Adjustment leave = new Leave_Adjustment();
               var leaveService = new LeaveService();

               leave.Employee_Profile_ID = model.Employee_Profile_ID;
               leave.Leave_Config_ID = model.Leave_Config_ID;
               leave.Adjustment_Amount = model.Adjustment_Amount;
               leave.Reason = model.Reason;
               leave.Company_ID = userlogin.Company_ID;
               leave.Create_By = userlogin.User_Authentication.Email_Address;
               leave.Create_On = currentdate;
               leave.Update_By = userlogin.User_Authentication.Email_Address;
               leave.Update_On = currentdate;
               leave.Year_2 = model.Year_2;

               if (model.Department_ID.HasValue && model.Department_ID.Value == 0)
                  model.Department_ID = null;

               leave.Department_ID = model.Department_ID;

               model.result = lService.InsertLeaveAdjustment(leave);
               if (model.result.Code == ERROR_CODE.SUCCESS)
                  return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "adjust" });
            }
            else if (model.operation == UserSession.RIGHT_U)
            {
               var leave = new Leave_Adjustment();

               leave.Employee_Profile_ID = model.Employee_Profile_ID;
               leave.Employee_Profile = null;
               leave.Leave_Config_ID = model.Leave_Config_ID;
               leave.Adjustment_Amount = model.Adjustment_Amount;
               leave.Reason = model.Reason;
               leave.Adjustment_ID = model.aid;
               leave.Company_ID = userlogin.Company_ID;
               leave.Update_By = userlogin.User_Authentication.Email_Address;
               leave.Update_On = currentdate;
               leave.Create_On = model.Create_On;
               leave.Create_By = model.Create_By;
               leave.Year_2 = model.Year_2;

               if (model.Department_ID.HasValue && model.Department_ID.Value == 0)
                  model.Department_ID = null;

               leave.Department_ID = model.Department_ID;

               model.result = lService.UpdateLeaveAdjustment(leave);
               if (model.result.Code == ERROR_CODE.SUCCESS)
                  return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "adjust" });
            }
         }

         model.leaveTypeList = cbService.LstLeaveType(userlogin.Company_ID.Value);
         model.employeeList = cbService.LstEmployeeList(userlogin.Company_ID.Value);
         model.departmentList = cbService.LstDepartment(userlogin.Company_ID);
         return View(model);
      }
      public ActionResult LeaveAdjustmentReloadEmp(Nullable<int> pDepartmentID)
      {

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         List<User_Profile> combolist = new List<User_Profile>();
         var cbService = new ComboService();
         var emps = cbService.LstEmployeeList(userlogin.Company_ID.Value, pDepartmentID);

         return Json(emps, JsonRequestBehavior.AllowGet);

      }

      [HttpGet]
      public ActionResult LeaveDefaultNormal(ServiceResult result, string pDid, string operation)
      {

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var model = new LeaveDefaultNormalViewModel();
         model.operation = EncryptUtil.Decrypt(operation);
         //-------rights------------
         RightResult rightResult = base.validatePageRight(model.operation, "/Leave/LeaveDefault");
         if (rightResult.action != null)
            return rightResult.action;
         model.rights = rightResult.rights;
         model.result = result;

         var ltypeID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pDid));
         var lService = new LeaveService();
         var cbService = new ComboService();

         model.monthList = cbService.LstMonth(true);
         model.genderList = cbService.LstLookup(ComboType.Gender, userlogin.Company_ID);
         model.maritalList = cbService.LstLookup(ComboType.Marital_Status, userlogin.Company_ID);
         model.Is_Default = true;

         var com = new CompanyService().GetCompany(userlogin.Company_ID);
         if (com != null)
         {
            if (com.Currency != null)
               model.Company_Currency = com.Currency.Currency_Code;
         }

         if (userlogin != null)
         {
            if (model.operation == UserSession.RIGHT_C)
            {
               model.Is_Bring_Forward_Days = false;
            }
            else if (model.operation == UserSession.RIGHT_U)
            {
               var comService = new CompanyService();
               Leave_Default leaveDefault = lService.getLeaveDefaulType(ltypeID);

               //Chk right on Leave_Config (of the company)
               if (leaveDefault == null)
                  return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

               model.Did = leaveDefault.Default_ID;
               model.Leave_Name = leaveDefault.Leave_Name;
               model.Type = leaveDefault.Type;
               model.Deduct_In_Payroll = leaveDefault.Deduct_In_Payroll.Value;
               model.Is_Default = leaveDefault.Is_Default.HasValue ? leaveDefault.Is_Default.Value : true;

               //Added by sun 10-09-2016
               model.Is_Accumulative = leaveDefault.Is_Accumulative.HasValue ? leaveDefault.Is_Accumulative.Value : false;


               if (leaveDefault.Allowed_Probation.HasValue) model.Allowed_Probation = leaveDefault.Allowed_Probation.Value;
               model.Bring_Forward = leaveDefault.Bring_Forward.Value;
               if (!model.Bring_Forward)
               {
                  model.Months_To_Expiry = 0;
                  model.Bring_Forward_Percent = 0;
                  model.Bring_Forward_Days = 0;
                  model.Is_Bring_Forward_Days = false;
               }
               else
               {
                  model.Is_Bring_Forward_Days = leaveDefault.Is_Bring_Forward_Days.HasValue ? leaveDefault.Is_Bring_Forward_Days.Value : false;
                  if (model.Is_Bring_Forward_Days)
                     model.Bring_Forward_Days = leaveDefault.Bring_Forward_Days.HasValue ? leaveDefault.Bring_Forward_Days.Value : 0;
                  else
                     model.Bring_Forward_Percent = leaveDefault.Bring_Forward_Percent;
                  model.Months_To_Expiry = leaveDefault.Months_To_Expiry.HasValue ? leaveDefault.Months_To_Expiry.Value : 1;
               }

               model.Upload_Document = leaveDefault.Upload_Document.Value;
               model.Leave_Description = leaveDefault.Leave_Description;
               var leaveDetailTypes = new List<LeaveDefaultDetailViewModel>();

               foreach (var row in leaveDefault.Leave_Default_Detail.OrderBy(o => o.Default_Detail_ID))
               {
                  var ltypeDetail = new LeaveDefaultDetailViewModel()
                  {
                     Default_Detail_ID = row.Default_Detail_ID,
                     Default_ID = row.Default_ID,
                     Year_Service = row.Year_Service,
                     Row_Type = RowType.EDIT,
                     Default_Leave_Amount = row.Default_Leave_Amount
                  };
                  leaveDetailTypes.Add(ltypeDetail);
               }
               model.Detail_Rows = leaveDetailTypes.ToArray();
               model.Condition_Rows = leaveDefault.Leave_Default_Condition.Select(s => s.Lookup_Data_ID.HasValue ? s.Lookup_Data_ID.Value : 0).ToArray();

            }
            else if (model.operation == UserSession.RIGHT_D)
            {
               var apply = RecordStatus.Delete;
               model.result = lService.UpdateDeleteLeaveDefaultStatus(ltypeID, apply, userlogin.User_Authentication.Email_Address);
               if (model.result.Code == ERROR_CODE.SUCCESS)
                  return RedirectToAction("LeaveDefault", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field });
            }
         }

         return View(model);
      }

      [HttpPost]
      [ValidateAntiForgeryToken]
      public ActionResult LeaveDefaultNormal(LeaveDefaultNormalViewModel model)
      {

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //-------rights------------
         RightResult rightResult = base.validatePageRight(model.operation, "/Leave/LeaveDefault");
         if (rightResult.action != null)
            return rightResult.action;
         model.rights = rightResult.rights;

         var lService = new LeaveService();
         var currentdate = StoredProcedure.GetCurrentDate();

         if (string.IsNullOrEmpty(model.Leave_Name))
            ModelState.AddModelError("Leave_Name", Resource.Message_Is_Required);

         if (model.Bring_Forward)
         {
            if (model.Is_Bring_Forward_Days)
            {
               if (model.Bring_Forward_Days <= 0)
                  ModelState.AddModelError("Bring_Forward_Days", Resource.Message_Is_Required);
            }
            else
            {
               if (!model.Bring_Forward_Percent.HasValue || model.Bring_Forward_Percent.Value <= 0)
                  ModelState.AddModelError("Bring_Forward_Percent", Resource.Message_Is_Required);
               else if (model.Bring_Forward_Percent.HasValue && model.Bring_Forward_Percent.Value > 100)
                  ModelState.AddModelError("Bring_Forward_Percent", Resource.Message_Is_Invalid);
            }
            if (model.Months_To_Expiry <= 0)
               ModelState.AddModelError("Months_To_Expiry", Resource.Message_Is_Required);
         }
         if (model.Detail_Rows != null && model.Detail_Rows.Length > 0)
         {
            for (var i = 0; i < model.Detail_Rows.Length; i++)
            {
               var row = model.Detail_Rows[i];
               var type = model.Detail_Rows[i].Row_Type;
               if (type != RowType.DELETE)
               {
                  if (!row.Year_Service.HasValue || row.Year_Service.Value < 0)
                     ModelState.AddModelError("Detail_Rows[" + i + "].Year_Service", Resource.Message_Is_Required);

                  if (!row.Default_Leave_Amount.HasValue || row.Default_Leave_Amount.Value <= 0)
                     ModelState.AddModelError("Detail_Rows[" + i + "].Default_Leave_Amount", Resource.Message_Is_Required);
               }
            }
         }

         var dupleave = lService.LstLeaveDefaul(model.Leave_Name).FirstOrDefault();
         if (dupleave != null)
         {
            if (model.operation == UserSession.RIGHT_C)
               ModelState.AddModelError("Leave_Name", Resource.Message_Is_Duplicated);
            else if (model.operation == UserSession.RIGHT_U)
            {
               if (dupleave.Default_ID != model.Did)
                  ModelState.AddModelError("Leave_Name", Resource.Message_Is_Duplicated);
            }
         }

         if (ModelState.IsValid)
         {
            if (model.operation == UserSession.RIGHT_C)
            {
               Leave_Default leaveDefault = new Leave_Default();
               leaveDefault.Leave_Name = model.Leave_Name;
               leaveDefault.Deduct_In_Payroll = model.Deduct_In_Payroll;
               leaveDefault.Allowed_Probation = model.Allowed_Probation;
               leaveDefault.Bring_Forward = model.Bring_Forward;
               if (!model.Bring_Forward)
               {
                  leaveDefault.Is_Bring_Forward_Days = false;
                  leaveDefault.Months_To_Expiry = null;
                  leaveDefault.Bring_Forward_Percent = null;
                  leaveDefault.Bring_Forward_Days = null;
               }
               else
               {
                  if (model.Is_Bring_Forward_Days)
                     leaveDefault.Bring_Forward_Days = model.Bring_Forward_Days;
                  else
                     leaveDefault.Bring_Forward_Percent = model.Bring_Forward_Percent;

                  leaveDefault.Months_To_Expiry = model.Months_To_Expiry;
                  leaveDefault.Is_Bring_Forward_Days = model.Is_Bring_Forward_Days;
               }
               leaveDefault.Upload_Document = model.Upload_Document;
               leaveDefault.Leave_Description = model.Leave_Description;
               leaveDefault.Create_By = userlogin.User_Authentication.Email_Address;
               leaveDefault.Create_On = currentdate;
               leaveDefault.Update_By = userlogin.User_Authentication.Email_Address;
               leaveDefault.Update_On = currentdate;
               leaveDefault.Type = LeaveConfigType.Normal;
               leaveDefault.Is_Default = true;

               //Added by sun 10-09-2016
               leaveDefault.Is_Accumulative = model.Is_Accumulative;

               if (model.Detail_Rows != null)
               {
                  foreach (var row in model.Detail_Rows)
                  {
                     if (row.Row_Type == RowType.ADD)
                     {
                        var LeaveDefaultDetailAdd = new Leave_Default_Detail()
                        {
                           Default_Leave_Amount = row.Default_Leave_Amount,
                           Year_Service = row.Year_Service,
                        };
                        leaveDefault.Leave_Default_Detail.Add(LeaveDefaultDetailAdd);
                     }
                  }
               }
               var cons = new List<Leave_Default_Condition>();
               if (model.Condition_Rows != null)
               {
                  foreach (var conID in model.Condition_Rows)
                  {
                     cons.Add(new Leave_Default_Condition()
                     {
                        Lookup_Data_ID = conID
                     });
                  }
               }
               leaveDefault.Leave_Default_Condition = cons;
               model.result = lService.InsertLeaveDefaultNormal(leaveDefault);
               if (model.result.Code == ERROR_CODE.SUCCESS)
                  return RedirectToAction("LeaveDefault", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "leaveType" });
            }
            else if (model.operation == UserSession.RIGHT_U)
            {
               Leave_Default leaveDefault = lService.getLeaveDefaulType(model.Did);
               //Chk right on Leave_Config (of the company)
               if (leaveDefault == null)
               {
                  return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);
               }
               leaveDefault.Type = model.Type;
               leaveDefault.Leave_Name = model.Leave_Name;
               leaveDefault.Deduct_In_Payroll = model.Deduct_In_Payroll;
               leaveDefault.Allowed_Probation = model.Allowed_Probation;
               leaveDefault.Bring_Forward = model.Bring_Forward;
               if (!model.Bring_Forward)
               {
                  leaveDefault.Months_To_Expiry = null;
                  leaveDefault.Bring_Forward_Percent = null;
                  leaveDefault.Is_Bring_Forward_Days = false;
                  leaveDefault.Bring_Forward_Days = null;
               }
               else
               {
                  if (model.Is_Bring_Forward_Days)
                     leaveDefault.Bring_Forward_Days = model.Bring_Forward_Days;
                  else
                     leaveDefault.Bring_Forward_Percent = model.Bring_Forward_Percent;

                  leaveDefault.Months_To_Expiry = model.Months_To_Expiry;
                  leaveDefault.Is_Bring_Forward_Days = model.Is_Bring_Forward_Days;
               }
               leaveDefault.Upload_Document = model.Upload_Document;
               leaveDefault.Leave_Description = model.Leave_Description;
               leaveDefault.Update_By = userlogin.User_Authentication.Email_Address;
               leaveDefault.Update_On = currentdate;
               leaveDefault.Is_Default = true;
               //Added by sun 10-09-2016
               leaveDefault.Is_Accumulative = model.Is_Accumulative;

               if (!ModelState.IsValid & model.Detail_Rows != null)
               {
                  var i = 0;
                  foreach (var row in model.Detail_Rows)
                  {
                     if (row.Row_Type == RowType.DELETE)
                        DeleteModelStateError("Detail_Rows[" + i + "]");
                     i++;
                  }
               }

               if (model.Detail_Rows != null)
               {
                  leaveDefault.Leave_Default_Detail.Clear();
                  foreach (var row in model.Detail_Rows)
                  {
                     if (row.Row_Type == RowType.ADD | row.Row_Type == RowType.EDIT)
                     {
                        if (row.Default_ID != null)
                        {
                           var LeaveDefaultDetailEdit = new Leave_Default_Detail()
                           {
                              Default_Detail_ID = (row.Default_Detail_ID.HasValue ? row.Default_Detail_ID.Value : 0),
                              Default_ID = row.Default_ID,
                              Default_Leave_Amount = row.Default_Leave_Amount,
                              Year_Service = row.Year_Service,
                           };
                           leaveDefault.Leave_Default_Detail.Add(LeaveDefaultDetailEdit);
                        }
                        else
                        {
                           var LeaveDefaultDetailAdd = new Leave_Default_Detail()
                           {
                              Default_ID = leaveDefault.Default_ID,
                              Default_Leave_Amount = row.Default_Leave_Amount,
                              Year_Service = row.Year_Service,
                           };
                           leaveDefault.Leave_Default_Detail.Add(LeaveDefaultDetailAdd);
                        }
                     }
                  }
               }
               var cons = new List<Leave_Default_Condition>();
               if (model.Condition_Rows != null)
               {
                  foreach (var conID in model.Condition_Rows)
                  {
                     cons.Add(new Leave_Default_Condition() { Default_ID = leaveDefault.Default_ID, Lookup_Data_ID = conID });
                  }
               }

               leaveDefault.Leave_Default_Condition = cons;

               model.result = lService.UpdateLeaveDefaultNormal(leaveDefault);
               if (model.result.Code == ERROR_CODE.SUCCESS)
                  return RedirectToAction("LeaveDefault", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "leaveType" });
            }
         }

         var cbService = new ComboService();
         model.leaveTypeList = cbService.LstLeaveType(userlogin.Company_ID, model.Did);
         model.monthList = cbService.LstMonth(true);
         model.genderList = cbService.LstLookup(ComboType.Gender, userlogin.Company_ID);
         model.maritalList = cbService.LstLookup(ComboType.Marital_Status, userlogin.Company_ID);

         return View(model);
      }

      [HttpGet]
      public ActionResult LeaveDefault(ServiceResult result, LeaveDefaultViewModel model, string apply, int[] Default_IDs = null)
      {

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //-------rights------------
         RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A);
         if (rightResult.action != null)
            return rightResult.action;
         model.rights = rightResult.rights;
         model.result = result;

         var cbService = new ComboService();
         var lService = new LeaveService();

         model.LeaveDefList = cbService.LstDefaultType(true);
         model.LeaveDefaultList = lService.LstLeaveDefaul(model.LeaveDefault, model.LeaveDefaultType);

         if (apply == UserSession.RIGHT_D)
         {
            if (Default_IDs != null)
            {
               apply = RecordStatus.Delete;
               model.result = lService.UpdateMultipleDeleteLeaveDefaultStatus(Default_IDs, apply, userlogin.User_Authentication.Email_Address);
               //model.result = lService.MultipleDeleteLeaveDefault(Default_IDs);
               if (model.result.Code == ERROR_CODE.SUCCESS)
                  return RedirectToAction("LeaveDefault", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field });
            }
         }
         return View(model);
      }

      public ActionResult AddNewLeaveDefaultDetail(int pIndex, Nullable<int> pYear, Nullable<decimal> pAmount)
      {

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var cbService = new ComboService();
         var companyService = new CompanyService();
         var model = new LeaveDefaultDetailViewModel()
         {
            Index = pIndex,
            Year_Service = pYear,
            Default_Leave_Amount = pAmount,
         };

         return PartialView("LeaveDefaultTypeDetailRow", model);

      }

      [HttpGet]
      public ActionResult LeaveDefaultChild(ServiceResult result, string pDid, string operation)
      {

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var model = new LeaveDefaultChildViewModel();
         model.operation = EncryptUtil.Decrypt(operation);

         //-------rights------------
         RightResult rightResult = base.validatePageRight(model.operation, "/Leave/LeaveDefault");
         if (rightResult.action != null)
            return rightResult.action;
         model.rights = rightResult.rights;
         model.result = result;

         var ltypeID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pDid));
         var lService = new LeaveService();
         var userService = new UserService();
         var cbService = new ComboService();

         model.monthList = cbService.LstMonth(true);
         model.genderList = cbService.LstLookup(ComboType.Gender, userlogin.Company_ID);
         model.maritalList = cbService.LstLookup(ComboType.Marital_Status, userlogin.Company_ID);
         model.residentalStatusList = cbService.LstResidentialStatus();
         model.periodList = cbService.LstPeriod(false, true, false, false, false);
         model.Is_Default = true;
         model.Type = LeaveConfigType.Child;

         if (userlogin != null)
         {
            if (model.operation == UserSession.RIGHT_C)
            {
               Leave_Default leaveDefault = new Leave_Default();
               var details = new List<Leave_Default_Child_Detail>();
               foreach (var rstatus in model.residentalStatusList)
               {
                  var detail = leaveDefault.Leave_Default_Child_Detail.Where(w => w.Residential_Status == rstatus.Value).FirstOrDefault();
                  if (detail == null)
                  {
                     detail = new Leave_Default_Child_Detail()
                     {
                        Residential_Status = rstatus.Value
                     };
                  }
                  details.Add(detail);
               }
               model.Detail_Rows = details.ToArray();

            }
            else if (model.operation == UserSession.RIGHT_U)
            {
               var comService = new CompanyService();
               Leave_Default leaveDefault = lService.getLeaveDefaulType(ltypeID);

               //Chk right on Leave_Config (of the company)
               if (leaveDefault == null)
               {
                  return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);
               }
               model.Did = leaveDefault.Default_ID;
               model.Leave_Name = leaveDefault.Leave_Name;
               model.Leave_Description = leaveDefault.Leave_Description;
               model.Flexibly = leaveDefault.Flexibly.HasValue ? leaveDefault.Flexibly.Value : false;
               model.Continuously = leaveDefault.Continuously.HasValue ? leaveDefault.Continuously.Value : false;
               model.Valid_Period = leaveDefault.Valid_Period.HasValue ? leaveDefault.Valid_Period.Value : 1;
               model.Is_Default = leaveDefault.Is_Default.HasValue ? leaveDefault.Is_Default.Value : true;

               model.Type = leaveDefault.Type;
               model.Condition_Rows = leaveDefault.Leave_Default_Condition.Select(s => s.Lookup_Data_ID.HasValue ? s.Lookup_Data_ID.Value : 0).ToArray();

               var details = new List<Leave_Default_Child_Detail>();
               foreach (var rstatus in model.residentalStatusList)
               {
                  var detail = leaveDefault.Leave_Default_Child_Detail.Where(w => w.Residential_Status == rstatus.Value).FirstOrDefault();
                  if (detail == null)
                  {
                     detail = new Leave_Default_Child_Detail()
                     {
                        Default_ID = leaveDefault.Default_ID,
                        Residential_Status = rstatus.Value
                     };
                  }
                  details.Add(detail);
               }
               model.Detail_Rows = details.ToArray();
            }
         }

         return View(model);
      }

      [HttpPost]
      [ValidateAntiForgeryToken]
      public ActionResult LeaveDefaultChild(LeaveDefaultChildViewModel model)
      {

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //-------rights------------
         RightResult rightResult = base.validatePageRight(model.operation, "/Leave/LeaveDefault");
         if (rightResult.action != null)
            return rightResult.action;
         model.rights = rightResult.rights;

         var lService = new LeaveService();
         var currentdate = StoredProcedure.GetCurrentDate();
         var userService = new UserService();

         if (string.IsNullOrEmpty(model.Leave_Name))
            ModelState.AddModelError("Leave_Name", Resource.Message_Is_Required);

         if (ModelState.IsValid)
         {
            if (model.operation == UserSession.RIGHT_C)
            {
               Leave_Default leaveDefault = new Leave_Default();
               leaveDefault.Leave_Name = model.Leave_Name;
               leaveDefault.Leave_Description = model.Leave_Description;
               leaveDefault.Flexibly = model.Flexibly;
               leaveDefault.Continuously = model.Continuously;
               leaveDefault.Valid_Period = model.Valid_Period;
               leaveDefault.Create_By = userlogin.User_Authentication.Email_Address;
               leaveDefault.Create_On = currentdate;
               leaveDefault.Update_By = userlogin.User_Authentication.Email_Address;
               leaveDefault.Update_On = currentdate;
               leaveDefault.Type = model.Type;
               leaveDefault.Is_Default = true;

               var cons = new List<Leave_Default_Condition>();
               if (model.Condition_Rows != null)
               {
                  foreach (var conID in model.Condition_Rows)
                  {
                     cons.Add(new Leave_Default_Condition()
                     {
                        Lookup_Data_ID = conID
                     });
                  }
               }
               leaveDefault.Leave_Default_Condition = cons;
               leaveDefault.Leave_Default_Child_Detail = model.Detail_Rows;

               model.result = lService.InsertLeaveDefaultChild(leaveDefault);
               if (model.result.Code == ERROR_CODE.SUCCESS)
               {
                  return RedirectToAction("LeaveDefault", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "leaveType" });
               }

            }
            else if (model.operation == UserSession.RIGHT_U)
            {

               Leave_Default leaveDefault = lService.getLeaveDefaulType(model.Did);
               if (leaveDefault == null)
                  return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

               leaveDefault.Leave_Name = model.Leave_Name;
               leaveDefault.Leave_Description = model.Leave_Description;
               leaveDefault.Flexibly = model.Flexibly;
               leaveDefault.Continuously = model.Continuously;
               leaveDefault.Valid_Period = model.Valid_Period;
               leaveDefault.Update_By = userlogin.User_Authentication.Email_Address;
               leaveDefault.Update_On = currentdate;
               leaveDefault.Type = model.Type;
               leaveDefault.Is_Default = true;

               var cons = new List<Leave_Default_Condition>();
               if (model.Condition_Rows != null)
               {
                  foreach (var conID in model.Condition_Rows)
                  {
                     cons.Add(new Leave_Default_Condition() { Default_ID = leaveDefault.Default_ID, Lookup_Data_ID = conID });
                  }
               }
               leaveDefault.Leave_Default_Condition = cons;
               leaveDefault.Leave_Default_Child_Detail = model.Detail_Rows;

               model.result = lService.UpdateLeaveDefaultChild(leaveDefault);
               if (model.result.Code == ERROR_CODE.SUCCESS)
                  return RedirectToAction("LeaveDefault", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field });
            }
         }

         var cbService = new ComboService();
         model.monthList = cbService.LstMonth(true);
         model.genderList = cbService.LstLookup(ComboType.Gender, userlogin.Company_ID);
         model.maritalList = cbService.LstLookup(ComboType.Marital_Status, userlogin.Company_ID);
         model.residentalStatusList = cbService.LstResidentialStatus();
         model.periodList = cbService.LstPeriod(false, true, false, false, false);
         return View(model);
      }

      #endregion

      #region TeamCalendar
      [HttpGet]
      public ActionResult TeamCalendar(LeaveViewModel model)
      {

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var leaveService = new LeaveService();
         var userService = new UserService();
         var cbService = new ComboService();
         var lService = new LeaveService();

         //-------rights------------
         RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A, "/Leave/Application");
         if (rightResult.action != null)
            return rightResult.action;
         model.rights = rightResult.rights;

         model.LeaveApplicationDocumentList = leaveService.LstLeaveApplicationDocument(userlogin.Company_ID, WorkflowStatus.Closed, pBranchID: model.search_Branch, pDepartmentID: model.search_Department, pProfileID: model.search_Pending_Emp);
         model.branchList = cbService.LstBranch(userlogin.Company_ID);
         model.departmentList = cbService.LstDepartment(userlogin.Company_ID, true);
         model.EmpList = lService.getEmployeeList(userlogin.Company_ID.Value);


         if (model.LeaveApplicationDocumentList != null)
         {
            var color = new List<string>();
            for (int i = 0; i <= model.LeaveApplicationDocumentList.Count; i++)
            {
               var Col = RandomColor.GetLstCode();
               while (color.Contains(Col.Name))
               {
                  Col = RandomColor.GetLstCode();
               }

               color.Add(Col.Name);
            }
            model.Collor = color.ToArray();
         }

         //Added by sun 01-12-2015
         var bankHolidays = new List<Holidays>();

         var hilidaylist = lService.getHolidays(userlogin.Company_ID.Value);
         if (hilidaylist != null)
         {
            foreach (var h in hilidaylist)
            {
               if (h.End_Date.HasValue & h.Start_Date.HasValue)
               {
                  for (var dt = h.Start_Date.Value; dt <= h.End_Date.Value; dt = dt.AddDays(1))
                  {

                     var holiday = new Holidays()
                     {
                        date = dt,
                        name = h.Name
                     };
                     bankHolidays.Add(holiday);
                  }
               }
               else
               {
                  var holiday = new Holidays()
                  {
                     date = h.Start_Date.Value,
                     name = h.Name
                  };
                  bankHolidays.Add(holiday);
               }
            }
         }

         model.HolidayList = bankHolidays.ToArray();

         return View(model);
      }
      #endregion

      #region Leave

      [HttpGet]
      public ActionResult Record(ServiceResult result, LeaveViewModel model)
      {

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var leaveService = new LeaveService();
         var cbService = new ComboService();

         //-------rights------------
         RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A, "/Leave/Application");
         if (rightResult.action != null)
            return rightResult.action;
         model.rights = rightResult.rights;
         model.result = result;

         //-------data------------
         model.LeaveApplicationDocumentList = leaveService.LstLeaveApplicationDocument(userlogin.Company_ID, pStatus: model.search_Leave_Status, pProfileID: userlogin.Profile_ID, pLeaveConfigID: model.search_Leave_Leave_Config, pDateAppliedFrom: model.search_Date_Applied_From, pDateAppliedTo: model.search_Date_Applied_To);
         model.HolidayDatetimeList = GetHolidays(userlogin.Company_ID.Value);

         model.lTypelist = cbService.LstLeaveType(userlogin.Company_ID.Value, pParentOnly: false, hasBlank: true);
         model.lStatuslist = cbService.LstApprovalStatus();

         return View(model);
      }

      [HttpGet]
      public ActionResult LeaveManagement(ServiceResult result, LeaveManagementViewModel model)
      {

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var leaveService = new LeaveService();
         var userService = new UserService();
         var lService = new LeaveService();

         //-------rights------------
         var rightResult = base.validatePageRight(UserSession.RIGHT_A, "/Leave/LeaveManagement");
         if (rightResult.action != null)
            return rightResult.action;
         model.rights = rightResult.rights;
         model.result = result;

         var hService = new EmploymentHistoryService();
         var hist = hService.GetCurrentEmploymentHistoryByProfile(userlogin.Profile_ID);
         if (hist == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Employment_History);


         model.LeaveApplicationDocumentList = leaveService.LstLeaveApplicationDocument(userlogin.Company_ID);
         model.employeeList = lService.getEmployeeList(userlogin.Company_ID.Value);
         ModelState.Clear();
         return View(model);
      }

      [HttpGet]
      public ActionResult Application(string pStartDate, string pDocID, string operation, string pAppr = "", string tabAction = "")
      {
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var leaveService = new LeaveService();
         var cbService = new ComboService();
         var empService = new EmployeeService();
         var uService = new UserService();
         var model = new LeaveViewModel();

         var docID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pDocID));
         var sdate = pStartDate;
         var ApprStatus = EncryptUtil.Decrypt(pAppr);
         //-------rights------------
         RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A);
         if (rightResult.action != null)
            return rightResult.action;
         model.rights = rightResult.rights;
         model.tabAction = tabAction;
         model.operation = EncryptUtil.Decrypt(operation);
         model.ApprStatus = ApprStatus;

         var currentdate = StoredProcedure.GetCurrentDate();

         if (string.IsNullOrEmpty(sdate))
            sdate = DateUtil.ToDisplayDate(currentdate);

         model.Working_Days = leaveService.GetWorkingDayOfWeek(userlogin.Company_ID, userlogin.Profile_ID);
         model.periodList = cbService.LstDatePeriod(true);
         model.EmployeeUnderMeList = cbService.LstEmpUnderMe(userlogin.Profile_ID);

         model.isRejectPopUp = false;

         if (docID > 0 && model.operation == UserSession.RIGHT_U)
         {
            //-------data------------
            var leaveApp = leaveService.GetLeaveApplicationDocument(docID);
            if (leaveApp != null)
            {
               model.Name = UserSession.GetUserName(leaveApp.Employee_Profile.User_Profile);
               model.Email = leaveApp.Employee_Profile.User_Profile.User_Authentication.Email_Address;
               model.Address_While_On_Leave = leaveApp.Address_While_On_Leave;

               //Added by sun 01-10-2015
               model.Contact_While_Overseas = leaveApp.Contact_While_Overseas;
               //model.Primary_Contact_No = leaveApp.Primary_Contact_No;
               model.Second_Contact_While_Overseas = leaveApp.Second_Contact_While_Overseas;


               model.Employee_Profile_ID = leaveApp.Employee_Profile_ID;
               model.Profile_ID = leaveApp.Employee_Profile.Profile_ID;
               model.OnBehalf_Employee_Profile_ID = leaveApp.Employee_Profile_ID;
               model.OnBehalf_Profile_ID = leaveApp.Employee_Profile.Profile_ID;
               model.Start_Date = DateUtil.ToDisplayDate(leaveApp.Start_Date);
               model.Start_Date_Period = leaveApp.Start_Date_Period;
               if (leaveApp.End_Date.HasValue)
               {
                  model.End_Date = DateUtil.ToDisplayDate(leaveApp.End_Date);
                  model.End_Date_Period = leaveApp.End_Date_Period;
               }
               else
               {
                  model.End_Date = DateUtil.ToDisplayDate(leaveApp.Start_Date);
                  model.End_Date_Period = leaveApp.Start_Date_Period;
               }

               model.Leave_Application_Document_ID = leaveApp.Leave_Application_Document_ID;
               model.Leave_Config_ID = leaveApp.Leave_Config_ID;

               model.Leave_Name = leaveApp.Leave_Config.Leave_Name;
               model.Leave_Left = 0;
               model.Overall_Status = leaveApp.Overall_Status;
               model.Reasons = leaveApp.Reasons;
               model.Days_Taken = leaveApp.Days_Taken.ToString();
               model.Maternity_Weeks_Taken = leaveApp.Weeks_Taken;
               model.Relationship_ID = leaveApp.Relationship_ID;
               model.Relationship_Name = leaveApp.Relationship != null ? leaveApp.Relationship.Name : "";

               //Added by sun 18-02-2016
               model.Cancel_Status = leaveApp.Cancel_Status;
               model.Request_Cancel_ID = leaveApp.Request_Cancel_ID;

               if (leaveApp.Upload_Document != null && leaveApp.Upload_Document.FirstOrDefault() != null)
                  model.filename = leaveApp.Upload_Document.FirstOrDefault().File_Name;

               //Edit by Jane 25-02-2016
               model.Request_ID = leaveApp.Request_ID;
               if (leaveApp.Request_ID.HasValue)
               {
                  var aService = new SBSWorkFlowAPI.Service();
                  var r = aService.GetMyRequests(userlogin.Company_ID, null, null, ModuleCode.HR, ApprovalType.Leave, leaveApp.Leave_Application_Document_ID);
                  if (r.Item2.IsSuccess && r.Item1 != null)
                     model.Leave_Request = r.Item1;
               }
               //Edit by Jane 27-04-2016
               if (leaveApp.Supervisor.HasValue)
               {
                  model.Supervisor = leaveApp.Supervisor;
                  var sup = empService.GetEmployeeProfile2(leaveApp.Supervisor);
                  if (sup != null)
                     model.Supervisor_Name = AppConst.GetUserName(sup.User_Profile);
               }
            }
         }
         else
         {
            model.operation = UserSession.RIGHT_C;
            var hService = new EmploymentHistoryService();
            var eService = new EmployeeService();

            var emp = eService.GetEmployeeProfileByProfileID(userlogin.Profile_ID);
            if (emp == null)
               return errorPage(ERROR_CODE.ERROR_16_NO_EMPLOYEE_PROFILE);

            var userhist = hService.GetCurrentEmploymentHistory(emp.Employee_Profile_ID);
            if (userhist == null)
               return errorPage(ERROR_CODE.ERROR_14_NO_EMPLOYEE_HIST);

            model.Start_Date = sdate;
            model.Profile_ID = userlogin.Profile_ID;

            var child = emp.Relationships.Where(w => w.Child_Type == ChildType.OwnChild | w.Child_Type == ChildType.AdoptedChild).OrderByDescending(o => o.DOB).FirstOrDefault();
            if (child != null)
            {
               model.Relationship_ID = child.Relationship_ID;
               model.Relationship_Name = child.Name;
            }

            var cri = new LeaveTypeCriteria()
            {
               Company_ID = userlogin.Company_ID,
               Profile_ID = userlogin.Profile_ID,
               Emp = emp,
               isNew = true,
            };
            model.LeaveTypeComboList = cbService.LstAndCalulateLeaveType(cri);
            model.EmployeeUnderMeList = cbService.LstEmpUnderMe(userlogin.Profile_ID);
         }
         return View(model);
      }

      [HttpPost]
      [ValidateAntiForgeryToken]
      public ActionResult Application(LeaveViewModel model, HttpPostedFileBase file, string pStatus)
      {
         if (model.ApprStatus == "Manage")
            return ApplicationMngt(model, pStatus);
         else
            return ApplicationNew(model, file, pStatus);
      }

      private ActionResult ApplicationMngt(LeaveViewModel model, string pStatus)
      {
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var leaveService = new LeaveService();
         var cbService = new ComboService();
         var empService = new EmployeeService();
         var hService = new EmploymentHistoryService();
         var uService = new UserService();
         var currentdate = StoredProcedure.GetCurrentDate();

         var leave = leaveService.GetLeaveApplicationDocument(model.Leave_Application_Document_ID);
         if (leave == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Leave);

         if (leave.Cancel_Status == WorkflowStatus.Cancelled)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Leave);

         if (leave.Overall_Status == WorkflowStatus.Rejected)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Leave);

         var hist = hService.GetCurrentEmploymentHistory(leave.Employee_Profile_ID);
         if (hist == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Employment_History);

         var user = uService.getUser(model.Profile_ID, false);
         if (user == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.User);

         var aService = new SBSWorkFlowAPI.Service();
         var status = pStatus;

         var Ac_Code = "L" + leave.Leave_Application_Document_ID + userlogin.Profile_ID + "_";
         if (string.IsNullOrEmpty(leave.Cancel_Status))
         {
            if (model.Request_ID.HasValue)
            {
               var action = new ActionItem();
               action.Actioner_Profile_ID = userlogin.Profile_ID;
               action.Email = userlogin.User_Authentication.Email_Address;
               action.Name = UserSession.GetUserName(userlogin);
               action.Request_ID = model.Request_ID.Value;
               if (pStatus == WorkflowStatus.Approved)
               {
                  action.IsApprove = true;
                  action.Action = WorkflowAction.Approve;
               }
               else
               {
                  if (string.IsNullOrEmpty(model.Remark_Rej))
                  {
                     ModelState.AddModelError("Remark_Rej", Resource.Message_Is_Required);
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
                        model.result = leaveService.UpdateLeaveUse(model.Leave_Application_Document_ID, model.Employee_Profile_ID, action.Status);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                           uService.ExpireActivationByPrefix(Ac_Code);
                           sendProceedEmail(leave, null, user, userlogin, hist, action.Status, null);
                           return RedirectToAction("LeaveManagement", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resource.Leave });
                        }
                     }
                     else if (action.Status == WorkflowStatus.Rejected)
                     {
                        model.result = leaveService.UpdateLeaveStatus(model.Leave_Application_Document_ID, action.Status);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                           uService.ExpireActivationByPrefix(Ac_Code);
                           sendProceedEmail(leave, null, user, userlogin, hist, action.Status, null);
                           return RedirectToAction("LeaveManagement", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_REJECT), Field = Resource.Leave });
                        }
                     }
                     else
                     {
                        //sendProceedEmail(leave, null, user, userlogin, hist, action.Status, null);
                        var param = new Dictionary<string, object>();
                        param.Add("lID", leave.Leave_Application_Document_ID);
                        param.Add("appID", action.NextApprover.Profile_ID);
                        param.Add("empID", leave.Employee_Profile_ID);
                        param.Add("reqID", leave.Request_ID);
                        param.Add("status", WorkflowStatus.Approved);
                        param.Add("code", uService.GenActivateCode("L" + leave.Leave_Application_Document_ID + action.NextApprover.Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

                        var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                        param["status"] = WorkflowStatus.Rejected;
                        var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                        var appr = uService.getUser(action.NextApprover.Profile_ID, false);
                        if (appr != null)
                           sendRequestEmail(leave, null, appr, user, hist, WorkflowStatus.Submitted, null, linkApp, linkRej);

                        uService.ExpireActivationByPrefix(Ac_Code);
                        return RedirectToAction("LeaveManagement", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resource.Leave });
                     }
                  }
               }
            }
            else if (hist.Supervisor.HasValue)
            {
               /*approval by supervisor*/
               if (pStatus == WorkflowStatus.Approved)
                  leave.Overall_Status = WorkflowStatus.Closed;
               else
                  leave.Overall_Status = WorkflowStatus.Rejected;

               model.result = leaveService.UpdateLeaveUse(leave.Leave_Application_Document_ID, leave.Employee_Profile_ID, leave.Overall_Status);
               if (model.result.Code == ERROR_CODE.SUCCESS)
               {
                  uService.ExpireActivationByPrefix(Ac_Code);
                  sendProceedEmail(leave, null, user, userlogin, hist, leave.Overall_Status, null);
                  if (pStatus == WorkflowStatus.Approved)
                     return RedirectToAction("LeaveManagement", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resource.Leave });
                  else
                     return RedirectToAction("LeaveManagement", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_REJECT), Field = Resource.Leave });
               }
            }
         }
         else
         {
            /* approve canncel workflow*/
            if (model.Request_Cancel_ID.HasValue)
            {
               var action = new ActionItem();
               action.Actioner_Profile_ID = userlogin.Profile_ID;
               action.Email = userlogin.User_Authentication.Email_Address;
               action.Name = UserSession.GetUserName(userlogin);
               action.Request_ID = model.Request_Cancel_ID.Value;
               if (pStatus == WorkflowStatus.Approved)
               {
                  action.IsApprove = true;
                  action.Action = WorkflowAction.Approve;
               }
               else
               {
                  if (string.IsNullOrEmpty(model.Remark_Rej))
                  {
                     ModelState.AddModelError("Remark_Rej", Resource.Message_Is_Required);
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
                        model.result = leaveService.UpdateLeaveUse(model.Leave_Application_Document_ID, model.Employee_Profile_ID, null, WorkflowStatus.Cancelled, false);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                           uService.ExpireActivationByPrefix(Ac_Code);
                           sendProceedEmail(leave, null, user, userlogin, hist, WorkflowStatus.Cancelled, null);
                           return RedirectToAction("LeaveManagement", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resource.Leave });
                        }
                     }
                     else if (action.Status == WorkflowStatus.Rejected)
                     {
                        model.result = leaveService.UpdateLeaveStatus(model.Leave_Application_Document_ID, null, WorkflowStatus.Cancellation_Rejected);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                           uService.ExpireActivationByPrefix(Ac_Code);
                           sendProceedEmail(leave, null, user, userlogin, hist, WorkflowStatus.Cancellation_Rejected, null);
                           return RedirectToAction("LeaveManagement", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL_REJECT), Field = Resource.Leave });
                        }
                     }
                     else
                     {
                        var param = new Dictionary<string, object>();
                        param.Add("lID", leave.Leave_Application_Document_ID);
                        param.Add("appID", action.NextApprover.Profile_ID);
                        param.Add("empID", leave.Employee_Profile_ID);
                        param.Add("reqID", leave.Request_ID);
                        param.Add("status", WorkflowStatus.Approved);
                        param.Add("code", uService.GenActivateCode("L" + leave.Leave_Application_Document_ID + action.NextApprover.Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

                        var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                        param["status"] = WorkflowStatus.Rejected;
                        var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                        var appr = uService.getUser(action.NextApprover.Profile_ID, false);
                        if (appr != null)
                           sendRequestEmail(leave, null, appr, user, hist, WorkflowStatus.Submitted, null, linkApp, linkRej);

                        uService.ExpireActivationByPrefix(Ac_Code);
                        return RedirectToAction("LeaveManagement", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resource.Leave });

                     }
                  }
               }
            }
            else if (hist.Supervisor.HasValue)
            {
               /*cancel approval by supervisor*/
               if (pStatus == WorkflowStatus.Approved)
                  leave.Cancel_Status = WorkflowStatus.Cancelled;
               else
                  leave.Cancel_Status = WorkflowStatus.Cancellation_Rejected;

               model.result = leaveService.UpdateLeaveUse(leave.Leave_Application_Document_ID, leave.Employee_Profile_ID, null, leave.Cancel_Status, false);
               if (model.result.Code == ERROR_CODE.SUCCESS)
               {
                  uService.ExpireActivationByPrefix(Ac_Code);
                  sendProceedEmail(leave, null, user, userlogin, hist, leave.Cancel_Status, null);
                  if (pStatus == WorkflowStatus.Approved)
                     return RedirectToAction("LeaveManagement", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL), Field = Resource.Leave });
                  else
                     return RedirectToAction("LeaveManagement", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL_REJECT), Field = Resource.Leave });
               }
            }
         }

         model.LeaveTypeComboList = cbService.LstAndCalulateLeaveType(new LeaveTypeCriteria()
         {
            Company_ID = userlogin.Company_ID,
            Profile_ID = userlogin.Profile_ID,
            Ignore_Generate = true,
            isNew  = true,
         });

         model.periodList = cbService.LstDatePeriod(true);
         model.Working_Days = leaveService.GetWorkingDayOfWeek(userlogin.Company_ID, userlogin.Profile_ID);
         if (model.Request_ID.HasValue)
         {
            var r = aService.GetMyRequests(userlogin.Company_ID, null, null, ModuleCode.HR, ApprovalType.Leave, model.Leave_Application_Document_ID);
            if (r.Item2.IsSuccess && r.Item1 != null)
               model.Leave_Request = r.Item1;
         }

         RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A);
         if (rightResult.action != null)
            return rightResult.action;
         model.rights = rightResult.rights;
         return View(model);
      }

      public ActionResult ReloadLeaveLeft(Nullable<int> pLeaveConfigID, Nullable<int> pProfileID, string pStartDate, Nullable<int> pRelationshipID)
      {

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var leaveService = new LeaveService();
         var lService = new LeaveService();

         if (pLeaveConfigID.HasValue && pProfileID.HasValue)
         {
            var leaveconfig_id = 0;
            var uploaddoc = false;
            var leavedesc = "";
            var leaveConfig = leaveService.GetLeaveConfig(pLeaveConfigID);
            var left = 0M;
            var flexibly = false;
            var continuously = false;
            var type = LeaveConfigType.Normal;
            var entitle = 0M;
            var weeksLeft = 0M;
            bool isfirstPeriod = false;
            var leaveName = "";
            if (leaveConfig != null)
            {
               var cri = new LeaveTypeCriteria()
               {
                  Company_ID = userlogin.Company_ID,
                  Profile_ID = pProfileID,
                  Leave_Config_ID = leaveConfig.Leave_Config_ID,
                  Relationship_ID = pRelationshipID
               };
               var leaveleft = leaveService.CalculateLeaveLeft(cri);
               if (leaveleft != null)
               {
                  if (leaveConfig.Type == LeaveConfigType.Child)
                  {
                     weeksLeft = leaveleft.Weeks_Left;
                     isfirstPeriod = leaveleft.Is_First_Period;
                     entitle = leaveleft.Entitle;
                  }
                  else
                  {
                     entitle = leaveleft.Entitle;
                     left = leaveleft.Left;
                     if (leaveConfig.Leave_Config_Parent_ID.HasValue)
                     {
                        var parentcri = cri.Clone() as LeaveTypeCriteria;
                        parentcri.Leave_Config_ID = leaveConfig.Leave_Config_Parent_ID;
                        parentcri.Leave_Config = null;
                        var parentleaveleft = leaveService.CalculateLeaveLeft(parentcri);
                        if (parentleaveleft != null)
                        {
                           if (parentleaveleft.Left < 0.5M)
                              left -= parentleaveleft.Leave_Used;
                        }
                     }
                  }

               }
               leaveconfig_id = leaveConfig.Leave_Config_ID;
               leavedesc = leaveConfig.Leave_Description;
               uploaddoc = leaveConfig.Upload_Document.HasValue ? leaveConfig.Upload_Document.Value : false;
               flexibly = leaveConfig.Flexibly.HasValue ? leaveConfig.Flexibly.Value : false;
               continuously = leaveConfig.Continuously.HasValue ? leaveConfig.Continuously.Value : false;
               type = leaveConfig.Type;
               leaveName = leaveConfig.Leave_Name;
            }

            return Json(new
            {
               leaveleft = left,
               leaveConfig = leaveconfig_id,
               IsUploadDoc = uploaddoc,
               leavedesc = leavedesc,
               Flexibly = flexibly,
               Continuously = continuously,
               Type = type,
               Entitle = entitle,
               weeksLeft = weeksLeft,
               isfirstPeriod = isfirstPeriod,
               leaveName = leaveName,
            }, JsonRequestBehavior.AllowGet);
         }

         return Json(new { leaveleft = 0, IsUploadDoc = false }, JsonRequestBehavior.AllowGet);
      }

      [HttpGet]
      public void ApplicationPdf(Nullable<int> pDocID)
      {
         //Edit by sun 30-12-2015
         var leaveService = new LeaveService();
         var leaveApp = leaveService.GetLeaveApplicationDocument(pDocID);
         if (leaveApp.Upload_Document != null && leaveApp.Upload_Document.FirstOrDefault() != null)
         {
            var doc = leaveApp.Upload_Document.FirstOrDefault().Document;
            if (leaveApp.Upload_Document.FirstOrDefault().File_Name.Contains(".pdf"))
            {
               Response.ClearHeaders();
               Response.Clear();
               Response.AddHeader("Content-Type", "application/pdf");
               Response.AddHeader("Content-Length", doc.Length.ToString());
               Response.AddHeader("Content-Disposition", "inline; filename=\"" + leaveApp.Upload_Document.FirstOrDefault().File_Name + "\"");
               Response.BinaryWrite(doc);
               Response.Flush();
               Response.End();
            }
            else
            {
               Response.ClearHeaders();
               Response.Clear();
               Response.AddHeader("Content-Type", "text/plain");
               Response.AddHeader("content-length", doc.Length.ToString());
               Response.AddHeader("Content-Disposition", "inline; filename=\"" + leaveApp.Upload_Document.FirstOrDefault().File_Name + "\"");
               Response.BinaryWrite(doc);
               Response.Flush();
               Response.End();
            }
         }
      }

      //private List<DateTime> GetHolidays(int CompanyID)
      //{
      //   //var userlogin = UserSession.getUser(HttpContext);

      //   var lService = new LeaveService();
      //   var bankHolidays = new List<DateTime>();
      //   var hilidaylist = lService.getHolidays(CompanyID);
      //   if (hilidaylist != null)
      //   {
      //      foreach (var h in hilidaylist)
      //      {
      //         if (h.End_Date.HasValue & h.Start_Date.HasValue)
      //         {
      //            for (var dt = h.Start_Date.Value; dt <= h.End_Date.Value; dt = dt.AddDays(1))
      //            {
      //               bankHolidays.Add(dt);
      //            }
      //         }
      //         else
      //            bankHolidays.Add(h.Start_Date.Value);
      //      }
      //   }
      //   return bankHolidays;
      //}

      public ActionResult GetEndDate(string pStartDate, Nullable<decimal> pDaysTaken, Nullable<double> pWorkingDays = 5)
      {
         var userlogin = UserSession.getUser(HttpContext);
         if (pDaysTaken.HasValue)
         {
            var sdate = DateUtil.ToDate(pStartDate);
            if (sdate.HasValue)
            {
               var enddate = DateCal.GetEndDate(sdate.Value, pDaysTaken.Value, pWorkingDays.Value, GetHolidays(userlogin.Company_ID.Value));
               return Json(new { enddate = DateUtil.ToDisplayDate(enddate) }, JsonRequestBehavior.AllowGet);
            }

         }
         return Json(new { enddate = "" }, JsonRequestBehavior.AllowGet);
      }

      //private void sendRequestEmail(Leave_Application_Document leave, Company_Details com, User_Profile sentto, User_Profile receivedfrom,
      //    Employment_History receivedhist, string Overall_Status, List<Reviewer> Reviewers, string linkApp, string linkRej, string _RequestURL = "")
      //{
      //   var ecode = "[RL" + leave.Leave_Application_Document_ID + "_";
      //   if (leave.Request_Cancel_ID.HasValue)
      //      ecode += "RQC" + leave.Request_Cancel_ID;
      //   else if (leave.Request_ID.HasValue)
      //      ecode += "RQ" + leave.Request_ID;

      //   ecode += "S" + receivedfrom.User_Authentication.User_Authentication_ID + "R" + sentto.User_Authentication.User_Authentication_ID + "]";

      //   var leaveService = new LeaveService();
      //   var cri = new LeaveTypeCriteria()
      //   {
      //      Profile_ID = leave.Employee_Profile.Profile_ID,
      //      Leave_Config_ID = leave.Leave_Config_ID,
      //      Relationship_ID = leave.Relationship_ID
      //   };
      //   var leaveleft = leaveService.CalculateLeaveLeft(cri);
      //   var s_URL = "";
      //   var s_LogoLink = "";
      //   if (Request == null && _RequestURL != "")
      //   {
      //      s_URL = _RequestURL;
      //      Uri relativeUri = new Uri(s_URL);
      //      s_LogoLink = Quick_GenerateLogoLink(relativeUri);
      //   }
      //   else
      //   {
      //      s_URL = Request.Url.AbsoluteUri;
      //      s_LogoLink = GenerateLogoLink();
      //   }
      //   var eitem = new EmailItem()
      //   {
      //      LogoLink = s_LogoLink,
      //      Company = com,
      //      Send_To_Email = sentto.User_Authentication.Email_Address,
      //      Send_To_Name = AppConst.GetUserName(sentto),
      //      Received_From_Email = receivedfrom.User_Authentication.Email_Address,
      //      Received_From_Name = AppConst.GetUserName(receivedfrom),
      //      Received_From_Department = receivedhist.Department != null ? receivedhist.Department.Name : "",
      //      Module = ModuleCode.HR,
      //      Approval_Type = ApprovalType.Leave,
      //      Leave = leave,
      //      Leave_Left = leaveleft != null ? leaveleft.Left : 0,
      //      Weeks_Left = leaveleft != null ? leaveleft.Weeks_Left : 0,
      //      Status = Overall_Status,
      //      Reviewer = Reviewers,
      //      Link = linkApp,
      //      Link2 = linkRej,
      //      Url = s_URL,
      //      ECode = ecode,
      //      Approver_Name = "",
      //   };
      //   EmailTemplete.sendRequestEmail(eitem);
      //}

      //private void sendProceedEmail(Leave_Application_Document leave, Company_Details com, User_Profile sentto, User_Profile receivedfrom, Employment_History receivedhist, string Overall_Status, List<Reviewer> Reviewers,
      //    int OnBehalf_Employee_Profile_ID = 0, int OnBehalf_Profile_ID = 0, string _RequestURL = "", string _ApproverName = "")
      //{
      //   var ecode = "[PL" + leave.Leave_Application_Document_ID + "_";
      //   if (leave.Request_Cancel_ID.HasValue)
      //      ecode += "RQC" + leave.Request_Cancel_ID;
      //   else if (leave.Request_ID.HasValue)
      //      ecode += "RQ" + leave.Request_ID;

      //   ecode += "S" + receivedfrom.User_Authentication.User_Authentication_ID + "R" + sentto.User_Authentication.User_Authentication_ID + "]";

      //   var leaveService = new LeaveService();
      //   var _profileID = leave.Employee_Profile.Profile_ID;
      //   if (OnBehalf_Profile_ID > 0)
      //      _profileID = OnBehalf_Profile_ID;
      //   var cri = new LeaveTypeCriteria()
      //   {
      //      Profile_ID = _profileID,
      //      Leave_Config_ID = leave.Leave_Config_ID,
      //      Relationship_ID = leave.Relationship_ID
      //   };
      //   var leaveleft = leaveService.CalculateLeaveLeft(cri);
      //   var s_URL = "";
      //   var s_LogoLink = "";
      //   if (Request == null && _RequestURL != "")
      //   {
      //      s_URL = _RequestURL;
      //      Uri relativeUri = new Uri(s_URL);
      //      s_LogoLink = Quick_GenerateLogoLink(relativeUri);
      //   }
      //   else
      //   {
      //      s_URL = Request.Url.AbsoluteUri;
      //      s_LogoLink = GenerateLogoLink();
      //   }


      //   var eitem = new EmailItem()
      //  {
      //     LogoLink = s_LogoLink,
      //     Company = com,
      //     Send_To_Email = sentto.User_Authentication.Email_Address,
      //     Send_To_Name = AppConst.GetUserName(sentto),
      //     Received_From_Email = receivedfrom.User_Authentication.Email_Address,
      //     Received_From_Name = AppConst.GetUserName(receivedfrom),
      //     Received_From_Department = receivedhist.Department != null ? receivedhist.Department.Name : "",
      //     Module = ModuleCode.HR,
      //     Approval_Type = ApprovalType.Leave,
      //     Leave = leave,
      //     Leave_Left = leaveleft != null ? leaveleft.Left : 0,
      //     Weeks_Left = leaveleft != null ? leaveleft.Weeks_Left : 0,
      //     Status = Overall_Status,
      //     Reviewer = Reviewers,
      //     Url = s_URL,
      //     ECode = ecode,
      //     Approver_Name = _ApproverName,
      //  };

      //   if (OnBehalf_Profile_ID > 0 && OnBehalf_Profile_ID != leave.Employee_Profile.Profile_ID)
      //   {
      //      var empService = new EmployeeService();
      //      var emp = empService.GetEmployeeProfile(OnBehalf_Employee_Profile_ID);
      //      var onBehalfName = AppConst.GetUserName(emp.User_Profile);
      //      var r = new Reviewer();
      //      r.Email = emp.User_Profile.Email;

      //      if (eitem.Reviewer != null)
      //         eitem.Reviewer.Add(r);
      //      else
      //      {
      //         List<SBSWorkFlowAPI.Models.Reviewer> lstR = new List<SBSWorkFlowAPI.Models.Reviewer>();
      //         lstR.Add(r);
      //         eitem.Reviewer = lstR;
      //      }
      //      EmailTemplete.sendProceedEmail(eitem, onBehalfName);
      //   }
      //   else
      //   {
      //      EmailTemplete.sendProceedEmail(eitem);
      //   }

      //}

      #endregion

      #region Report & Import
      //Added by sun 16-12-2015
      [HttpGet]
      public ActionResult LeaveReports(ServiceResult result, LeaveReportViewModel model, string operation, string lid, string lYear, string tabAction = "")
      {

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //-------rights------------
         RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A, "/Leave/LeaveReport");
         if (rightResult.action != null)
            return rightResult.action;
         model.rights = rightResult.rights;

         var leaveService = new LeaveService();
         var cbService = new ComboService();
         var comService = new CompanyService();
         var deparService = new DepartmentService();
         var currentdate = StoredProcedure.GetCurrentDate();

         var op = EncryptUtil.Decrypt(operation);
         if (op != null)
            model.operation = op;

         var lID = NumUtil.ParseInteger(EncryptUtil.Decrypt(lid));
         if (lID != 0)
            model.Leave_Type_Sel = new List<int>(new int[] { lID });

         var year = NumUtil.ParseInteger(lYear);
         if (year != 0)
            model.Year = year;

         if (model.Year == 0)
            model.Year = currentdate.Year;

         //Filter
         model.leavetypelist = cbService.LstLeaveType(userlogin.Company_ID.Value, hasBlank: true);
         if (model.leavetypelist != null && model.leavetypelist.Count > 0)
         {
            model.Leave_Sel = Newtonsoft.Json.JsonConvert.SerializeObject(model.Leave_Type_Sel);
         }

         model.departmentList = cbService.LstDepartment(userlogin.Company_ID, hasBlank: true);
         model.leavelist = leaveService.LstLeaveApplicationDocumentReport(userlogin.Company_ID, model.Department, model.Leave_Type, model.Year, model.Leave_Type_Sel);

         if (tabAction == "export")
         {
            string csv = "";
            var dep_name = "";
            if (model.Department.HasValue)
            {
               var department = deparService.GetDepartment(model.Department.Value);
               if (department != null)
                  dep_name = department.Name;
            }
            var fullName = UserSession.GetUserName(userlogin);

            //HEADER                
            string compname = comService.GetCompany(userlogin.Company_ID).Name;
            csv += "<table><tr valign='top'><td valign='top'><b> " + compname + " </b></td><td>&nbsp;</td><td  colspan=3><b>" + Resource.Leave_Report + "</b><br><b>" + Resource.Summary_Of_Annual + " " + Resource.Leave + "</b> " + model.Year + "</td></tr>";
            csv += "<tr><td>&nbsp;</td><td>&nbsp;</td><td colspan=3><b>" + Resource.Department + "</b> " + dep_name + "</td></tr>";
            csv += "<tr><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td></tr>";
            csv += "<tr><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td></tr></table>";
            csv += "<table border=1><tr valign='top'><td rowspan=2><b>" + Resource.Employee_No_SymbolDot + "</b></td> ";
            csv += "<td rowspan=2><b>" + Resource.Employee_Name + "</b></td> ";

            var totalhash = new System.Collections.Hashtable();
            foreach (var lrow in model.leavetypelist)
            {
               if (model.Leave_Type_Sel != null)
               {
                  if (model.Leave_Type_Sel.Contains(NumUtil.ParseInteger(lrow.Value)))
                  {
                     totalhash.Add("a" + lrow.Value, (decimal)0);
                     totalhash.Add("b" + lrow.Value, (decimal)0);
                     totalhash.Add("d" + lrow.Value, (decimal)0);
                     csv += "<td colspan =3><b>" + lrow.Text + "</b></td> ";
                  }
               }
               else
               {
                  totalhash.Add("a" + lrow.Value, (decimal)0);
                  totalhash.Add("b" + lrow.Value, (decimal)0);
                  totalhash.Add("d" + lrow.Value, (decimal)0);
                  csv += "<td colspan =3><b>" + lrow.Text + "</b></td> ";
               }
            }
            csv += "</tr>";
            csv += "<tr  valign='top'>";
            foreach (var lrow in model.leavetypelist)
            {
               if (model.Leave_Type_Sel != null)
               {
                  if (model.Leave_Type_Sel.Contains(NumUtil.ParseInteger(lrow.Value)))
                  {

                     csv += "<td><b>" + @Resource.Leave_Balance + "</b></td> ";
                     csv += "<td><b>" + @Resource.Leave_From_Last_Year + "</b></td> ";
                     csv += "<td><b>" + @Resource.Days_Taken + "</b></td> ";
                  }
               }
               else
               {
                  csv += "<td><b>" + @Resource.Leave_Balance + "</b></td> ";
                  csv += "<td><b>" + @Resource.Leave_From_Last_Year + "</b></td> ";
                  csv += "<td><b>" + @Resource.Days_Taken + "</b></td> ";
               }
            }
            csv += "</tr>";

            foreach (var row in model.leavelist)
            {
               csv += "<tr  valign='top'>";
               csv += "<td>" + row.Employee_No + "</td> ";
               csv += "<td>" + row.Employee_Name + "</td> ";
               foreach (var lrow in model.leavetypelist)
               {
                  if (model.Leave_Type_Sel != null)
                  {
                     if (model.Leave_Type_Sel.Contains(NumUtil.ParseInteger(lrow.Value)))
                     {
                        var leavetype = row.leaveTypelist.Where(w => w.Leave_Name.ToLower() == lrow.Text.ToLower()).FirstOrDefault();
                        if (leavetype != null)
                        {
                           totalhash["a" + lrow.Value] = (decimal)totalhash["a" + lrow.Value] + leavetype.Entitle;
                           totalhash["b" + lrow.Value] = (decimal)totalhash["b" + lrow.Value] + leavetype.Bring_Forward;
                           totalhash["d" + lrow.Value] = (decimal)totalhash["d" + lrow.Value] + leavetype.Days_Taken.Value;
                           csv += "<td style='text-align:right'>" + leavetype.Entitle.ToString("n2") + "</td> ";
                           csv += "<td style='text-align:right'>" + leavetype.Bring_Forward.ToString("n2") + "</td> ";
                           csv += "<td style='text-align:right'>" + leavetype.Days_Taken.Value.ToString("n2") + "</td> ";
                        }
                        else
                        {
                           csv += "<td style='text-align:right'>" + 0.ToString("n2") + "</td> ";
                           csv += "<td style='text-align:right'>" + 0.ToString("n2") + "</td> ";
                           csv += "<td style='text-align:right'>" + 0.ToString("n2") + "</td> ";
                        }
                     }
                  }
                  else
                  {
                     var leavetype = row.leaveTypelist.Where(w => w.Leave_Name.ToLower() == lrow.Text.ToLower()).FirstOrDefault();
                     if (leavetype != null)
                     {
                        totalhash["a" + lrow.Value] = (decimal)totalhash["a" + lrow.Value] + leavetype.Entitle;
                        totalhash["b" + lrow.Value] = (decimal)totalhash["b" + lrow.Value] + leavetype.Bring_Forward;
                        totalhash["d" + lrow.Value] = (decimal)totalhash["d" + lrow.Value] + leavetype.Days_Taken.Value;
                        csv += "<td style='text-align:right'>" + leavetype.Entitle.ToString("n2") + "</td> ";
                        csv += "<td style='text-align:right'>" + leavetype.Bring_Forward.ToString("n2") + "</td> ";
                        csv += "<td style='text-align:right'>" + leavetype.Days_Taken.Value.ToString("n2") + "</td> ";
                     }
                     else
                     {
                        csv += "<td style='text-align:right'>" + 0.ToString("n2") + "</td> ";
                        csv += "<td style='text-align:right'>" + 0.ToString("n2") + "</td> ";
                        csv += "<td style='text-align:right'>" + 0.ToString("n2") + "</td> ";
                     }
                  }
               }
               csv += "</tr>";
            }
            csv += "<tr>";
            csv += "<td colspan=2><b> " + Resource.Total + " </b></td>";
            foreach (var lrow in model.leavetypelist)
            {
               if (model.Leave_Type_Sel != null)
               {
                  if (model.Leave_Type_Sel.Contains(NumUtil.ParseInteger(lrow.Value)))
                  {
                     decimal atotal = 0;
                     decimal btotal = 0;
                     decimal dtotal = 0;
                     if (totalhash.Contains("a" + lrow.Value))
                        atotal = (decimal)totalhash["a" + lrow.Value];

                     if (totalhash.Contains("b" + lrow.Value))
                        btotal = (decimal)totalhash["b" + lrow.Value];

                     if (totalhash.Contains("d" + lrow.Value))
                        dtotal = (decimal)totalhash["d" + lrow.Value];

                     csv += "<td><b>" + atotal.ToString("n2") + "</b></td>";
                     csv += "<td><b>" + btotal.ToString("n2") + "</b></td>";
                     csv += "<td><b>" + dtotal.ToString("n2") + "</b></td>";
                  }
               }
               else
               {
                  decimal atotal = 0;
                  decimal btotal = 0;
                  decimal dtotal = 0;

                  if (totalhash.Contains("a" + lrow.Value))
                     atotal = (decimal)totalhash["a" + lrow.Value];

                  if (totalhash.Contains("b" + lrow.Value))
                     btotal = (decimal)totalhash["b" + lrow.Value];

                  if (totalhash.Contains("d" + lrow.Value))
                     dtotal = (decimal)totalhash["d" + lrow.Value];

                  csv += "<td><b>" + atotal.ToString("n2") + "</b></td>";
                  csv += "<td><b>" + btotal.ToString("n2") + "</b></td>";
                  csv += "<td><b>" + dtotal.ToString("n2") + "</b></td>";
               }
            }
            csv += "</tr>";

            csv += "</table>";
            csv += "<table><tr><td>&nbsp;</td></tr>";
            csv += "<tr><td><b> " + Resource.Printed_By + " </b> " + fullName + "</td></tr></table>";

            System.Web.UI.WebControls.GridView gv = new System.Web.UI.WebControls.GridView();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=LeaveReport.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            System.IO.StringWriter sw = new System.IO.StringWriter();
            sw.Write(csv);
            System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
         }

         return View(model);
      }

      //Added by sun 04-02-2016
      [HttpGet]
      public ActionResult LeaveImport()
      {
         var model = new ImportLeaveViewModels();

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //Validate Page Right
         RightResult rightResult = base.validatePageRight(UserSession.RIGHT_C);
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;

         model.leaveAppDoc = new List<ImportLeaveApplicationDocument_>().ToArray();
         model.errMsg = new List<string>();
         model.validated_Main = true;

         return View(model);

      }

      public ActionResult LeaveImport(ImportLeaveViewModels model, string pageAction)
      {

         User_Profile userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //Validate Page Right
         RightResult rightResult = base.validatePageRight(UserSession.RIGHT_C);
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;

         //-------data------------
         DateTime currentdate = StoredProcedure.GetCurrentDate();
         var leaveService = new LeaveService();
         var cbService = new ComboService();
         var comService = new CompanyService();
         var empService = new EmployeeService();

         if (pageAction == "import")
         {
            if (model.leaveAppDoc.Length > 0 && model.validated_Main)
            {
               List<Leave_Application_Document> LeaveAppDocs = new List<Leave_Application_Document>();
               foreach (var row in model.leaveAppDoc)
               {
                  if (row.Validate)
                  {
                     var LeaveAppDoc = new Leave_Application_Document()
                     {
                        Employee_Profile_ID = row.Employee_Profile_ID,
                        Leave_Config_ID = row.Leave_Config_ID,
                        End_Date = DateUtil.ToDate(row.End_Date),
                        Start_Date = DateUtil.ToDate(row.Start_Date),
                        Days_Taken = row.Days_Taken,
                        Remark = row.Remark,
                        //Request_ID =  row.Request_ID,
                        Overall_Status = WorkflowStatus.Closed,
                        Create_By = userlogin.User_Authentication.Email_Address,
                        Create_On = currentdate,
                        Update_By = userlogin.User_Authentication.Email_Address,
                        Update_On = currentdate
                     };
                     LeaveAppDocs.Add(LeaveAppDoc);
                  }
               }
               if (LeaveAppDocs != null)
               {
                  model.result = leaveService.InsertLeaveApplicationDocument(LeaveAppDocs.ToArray());
                  if (model.result.Code == ERROR_CODE.SUCCESS)
                  {
                     return RedirectToAction("Record", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field });
                  }
               }
            }
         }
         else
         {

            if (Request.Files.Count == 0)
            {
               ModelState.AddModelError("Import_Leave", Resource.Message_Cannot_Found_Excel_Sheet);
               return View(model);
            }

            HttpPostedFileBase file = Request.Files[0];
            if (file != null)
            {
               var com = comService.GetCompany(userlogin.Company_ID);
               if (com == null)
                  return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

               var leaveTypelst = cbService.LstLeaveType(userlogin.Company_ID, pParentOnly: false);
               var employeeprofilelst = empService.LstEmployeeProfile(userlogin.Company_ID);

               try
               {
                  using (var package = new ExcelPackage(file.InputStream))
                  {
                     List<string> chk_Emp_No = new List<string>();
                     model.validated_Main = true;

                     ExcelWorksheet worksheet_1 = package.Workbook.Worksheets[1];
                     if (worksheet_1.Dimension != null)
                     {
                        int totalRows_1 = worksheet_1.Dimension.End.Row;
                        int totalCols_1 = worksheet_1.Dimension.End.Column;

                        if (totalCols_1 != 6)
                        {
                           ModelState.AddModelError("leaveAppDoc", Resource.Message_Column_Count_Is_Invalid + " " + Resource.Message_Please_Edit_Reupload);
                           model.validated_Main = false;
                        }
                        if (totalRows_1 <= 1)
                        {
                           ModelState.AddModelError("leaveAppDoc", Resource.Message_Row_Count_Is_Invalid + " " + Resource.Message_Please_Edit_Reupload);
                           model.validated_Main = false;
                        }

                        if (ModelState.IsValid)
                        {
                           if (totalRows_1 > 1)
                           {
                              var leaveappdocs = new List<ImportLeaveApplicationDocument_>();
                              for (int i = 2; i <= totalRows_1; i++)
                              {
                                 var leaveappdoc = new ImportLeaveApplicationDocument_();
                                 leaveappdoc.Company_ID = userlogin.Company_ID;
                                 leaveappdoc.Validate = true;
                                 var isempty = true;
                                 var err_ = new System.Text.StringBuilder();

                                 for (int j = 1; j <= totalCols_1; j++)
                                 {
                                    var columnName = worksheet_1.Cells[1, j].Value.ToString();
                                    isempty = false;
                                    if (worksheet_1.Cells[i, j].Value != null)
                                    {
                                       if (j == LeaveDocImportColumn.Employee_No)
                                       {
                                          var emp_no = "";
                                          emp_no = worksheet_1.Cells[i, j].Value.ToString();
                                          leaveappdoc.Employee_No = emp_no;

                                          var empprofil = employeeprofilelst.Where(w => w.Employee_No.ToString().Trim() == emp_no.ToString().Trim()).FirstOrDefault();
                                          if (empprofil != null)
                                          {
                                             leaveappdoc.Employee_Profile_ID = empprofil.Employee_Profile_ID;
                                          }
                                          else
                                          {
                                             model.validated_Main = false;
                                             leaveappdoc.Validate = false;
                                             err_.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Not_Found_Lower);
                                          }
                                       }
                                       else if (j == LeaveDocImportColumn.Leave_Config_Name)
                                       {
                                          var leaveType = leaveTypelst.Where(w => w.Text.ToLower().Trim() == worksheet_1.Cells[i, j].Value.ToString().ToLower().Trim()).FirstOrDefault();
                                          if (leaveType == null)
                                          {
                                             model.validated_Main = false;
                                             leaveappdoc.Validate = false;
                                             err_.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                          }
                                          else
                                          {
                                             leaveappdoc.Leave_Config_ID = NumUtil.ParseInteger(leaveType.Value);
                                             leaveappdoc.Leave_Config_Name = leaveType.Text;
                                          }
                                       }
                                       else if (j == LeaveDocImportColumn.Start_Date || j == LeaveDocImportColumn.End_Date)
                                       {
                                          var strdate = "";
                                          try
                                          {
                                             var date = (DateTime)worksheet_1.Cells[i, j].Value;
                                             strdate = DateUtil.ToDisplayDate(date);
                                          }
                                          catch
                                          {
                                             model.validated_Main = false;
                                             leaveappdoc.Validate = false;
                                             strdate = worksheet_1.Cells[i, j].Value.ToString();
                                             err_.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                          }

                                          if (j == LeaveDocImportColumn.Start_Date)
                                             leaveappdoc.Start_Date = strdate;
                                          else if (j == LeaveDocImportColumn.End_Date)
                                             leaveappdoc.End_Date = strdate;
                                       }
                                       else if (j == LeaveDocImportColumn.Days_Taken)
                                       {
                                          decimal daystaken = 0;
                                          try
                                          {
                                             daystaken = Convert.ToDecimal(worksheet_1.Cells[i, j].Value);
                                          }
                                          catch
                                          {
                                             model.validated_Main = false;
                                             leaveappdoc.Validate = false;
                                             daystaken = Convert.ToDecimal(worksheet_1.Cells[i, j].Value.ToString());
                                             err_.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                          }
                                          if (j == LeaveDocImportColumn.Days_Taken)
                                             leaveappdoc.Days_Taken = daystaken;
                                       }
                                       else if (j == LeaveDocImportColumn.Remark)
                                       {
                                          leaveappdoc.Remark = worksheet_1.Cells[i, j].Value.ToString();
                                          if (leaveappdoc.Remark.Length > 300)
                                          {
                                             model.validated_Main = false;
                                             leaveappdoc.Validate = false;
                                             err_.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Message_Maximum_Length + " '300'.");
                                          }
                                       }
                                    }
                                    else
                                    {
                                       // Validate require
                                       if (j == LeaveDocImportColumn.Employee_No
                                           || j == LeaveDocImportColumn.Leave_Config_Name
                                           || j == LeaveDocImportColumn.Start_Date
                                           || j == LeaveDocImportColumn.End_Date
                                           || j == LeaveDocImportColumn.Days_Taken)
                                       {
                                          model.validated_Main = false;
                                          leaveappdoc.Validate = false;
                                          err_.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Rrequired_Lower);
                                       }
                                    }
                                 }
                                 if (isempty)
                                 {
                                    model.validated_Main = false;
                                    leaveappdoc.Validate = false;
                                    err_.AppendLine(Resource.Message_Empty_Row);
                                 }
                                 leaveappdoc.ErrMsg = err_.ToString();
                                 leaveappdoc.Create_By = userlogin.User_Authentication.Email_Address;
                                 leaveappdoc.Create_On = currentdate;
                                 leaveappdocs.Add(leaveappdoc);
                              }
                              model.leaveAppDoc = leaveappdocs.ToArray();
                           }
                        }
                        else
                        {
                           model.leaveAppDoc = new List<ImportLeaveApplicationDocument_>().ToArray();
                        }
                     }
                  }
               }
               catch
               {
                  ModelState.AddModelError("Import_Leave", Resource.Message_Cannot_Found_Excel_Sheet + " " + Resource.Message_Please_Edit_Reupload);
                  model.leaveAppDoc = new List<ImportLeaveApplicationDocument_>().ToArray();
               }
               //var erjrors = GetErrorModelState();
            }
         }

         return View(model);
      }

      [HttpGet]
      public ActionResult LeaveReport(ServiceResult result, LeaveListViewModel model)
      {

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //Validate Page Right
         RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A);
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;

         var leaveService = new LeaveService();
         var cbService = new ComboService();
         var currentdate = StoredProcedure.GetCurrentDate();

         model.Yearlst = new List<int>();
         for (int i = 2014; i <= currentdate.Year; i++)
            model.Yearlst.Add(i);

         if (model.Year == 0)
            model.Year = currentdate.Year;

         var leavetypelist = cbService.LstLeaveType(userlogin.Company_ID.Value, hasBlank: true);
         if (leavetypelist == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Leave_Type);

         model.leavetypelst = leavetypelist;
         var leave = leaveService.LstLeaveApplicationDocumentReport(userlogin.Company_ID, null, model.Leave_Type, model.Year, null);
        

         var leavedetails = new List<Leave_List>();
         var totalhash = new System.Collections.Hashtable();
         foreach (var lrow in leavetypelist)
         {
            if (model.Leave_Type.HasValue)
            {
               if (model.Leave_Type.Value == NumUtil.ParseInteger(lrow.Value))
               {
                  totalhash.Add("d" + lrow.Value, (decimal)0);
               }
            }
            else
            {
               totalhash.Add("d" + lrow.Value, (decimal)0);
            }
         }

         foreach (var row in leave)
         {
            foreach (var lrow in leavetypelist)
            {
               if (model.Leave_Type.HasValue)
               {
                  if (model.Leave_Type.Value == NumUtil.ParseInteger(lrow.Value))
                  {
                     var leavetype = row.leaveTypelist.Where(w => w.Leave_Name.ToLower() == lrow.Text.ToLower()).FirstOrDefault();
                     if (leavetype != null)
                     {
                        totalhash["d" + lrow.Value] = (decimal)totalhash["d" + lrow.Value] + leavetype.Days_Taken.Value;
                     }
                  }
               }
               else
               {
                  var leavetype = row.leaveTypelist.Where(w => w.Leave_Name.ToLower() == lrow.Text.ToLower()).FirstOrDefault();
                  if (leavetype != null)
                  {
                     totalhash["d" + lrow.Value] = (decimal)totalhash["d" + lrow.Value] + leavetype.Days_Taken.Value;
                  }
               }
            }
         }

         foreach (var lrow in leavetypelist)
         {
            if (NumUtil.ParseInteger(lrow.Value) != 0)
            {
               if (model.Leave_Type.HasValue)
               {
                  if (model.Leave_Type.Value == NumUtil.ParseInteger(lrow.Value))
                  {
                     decimal dtotal = 0;
                     if (totalhash.Contains("d" + lrow.Value))
                     {
                        dtotal = (decimal)totalhash["d" + lrow.Value];

                        leavedetails.Add(new Leave_List()
                        {
                           Leave_ID = NumUtil.ParseInteger(lrow.Value),
                           Leave_Type = lrow.Text,
                           Days_Taken = dtotal
                        });
                     }
                  }
               }
               else
               {
                  decimal dtotal = 0;
                  if (totalhash.Contains("d" + lrow.Value))
                  {
                     dtotal = (decimal)totalhash["d" + lrow.Value];
                     leavedetails.Add(new Leave_List()
                     {
                        Leave_ID = NumUtil.ParseInteger(lrow.Value),
                        Leave_Type = lrow.Text,
                        Days_Taken = dtotal
                     });
                  }
               }
            }
         }
         model.LeaveList = leavedetails;

         return View(model);

      }
      #endregion

   }
}
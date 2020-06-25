using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HR.Models;
using HR.Common;
using SBSModel.Models;
using SBSModel.Common;
using SBSResourceAPI;


namespace HR.Controllers
{
   [Authorize]
   [AllowAuthorized]
   public class ConfigurationController : ControllerBase
   {
      [HttpGet]
      public ActionResult Configuration(int[] branches, int[] departs, int[] designas, int[] grades, ServiceResult result, ConfigulationViewModel model, string apply, string tabAction = "")
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
         model.tabAction = tabAction;

         var uService = new UserService();
         var cbService = new ComboService();
         var comService = new CompanyService();
         var bService = new BranchService();
         var dpService = new DepartmentService();
         var dsService = new DesignationService();
         var wService = new WorkingDaysService();

         //to delete multiple records of Branch
         if (tabAction == "branch")
         {
            if (branches != null)
            {
               if (apply == RecordStatus.Active | apply == RecordStatus.Inactive)
               {
                  model.result = bService.UpdateMultipleBranch(branches, apply, userlogin.User_Authentication.Email_Address);
                  if (model.result.Code == ERROR_CODE.SUCCESS)
                     return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "branch" });
               }
               else
               {
                  var chkRefHas = false;
                  foreach (var branch in branches)
                  {
                     if (bService.chkBranchUsed(branch) || bService.chkBranchinEmpePtnUsed(branch))
                     {
                        chkRefHas = true;
                        break;
                     }
                  }
                  if (chkRefHas)
                     return RedirectToAction("Configuration", new { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Branch, tabAction = "branch" });
                  else
                  {
                     if (apply == UserSession.RIGHT_D)
                     {
                        apply = RecordStatus.Delete;
                        model.result = bService.UpdateMultipleDeleteBranchStatus(branches, apply, userlogin.User_Authentication.Email_Address);
                        //model.result = bService.MultipleDeleteBranch(branches);
                        return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "branch" });
                     }
                  }
               }
            }
            else
            {
               if (apply == UserSession.RIGHT_D)
               {
                  if (model.Branch_ID.HasValue)
                  {
                     apply = RecordStatus.Delete;
                     model.result = bService.UpdateDeleteBranchStatus(model.Branch_ID, apply, userlogin.User_Authentication.Email_Address);
                     //model.result = bService.DeleteBranch(model.Branch_ID);
                  }
               }
            }
         }
         //to update status multiple records of Designation
         else if (tabAction == "designation")
         {
            if (designas != null)
            {
               if (apply == RecordStatus.Active | apply == RecordStatus.Inactive)
               {
                  model.result = dsService.UpdateMultipleDesignation(designas, apply, userlogin.User_Authentication.Email_Address);
                  if (model.result.Code == ERROR_CODE.SUCCESS)
                  {
                     return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "designation" });
                  }
               }
            }
         }
         //to update status multiple records of Department
         else if (tabAction == "department")
         {
            if (departs != null)
            {
               if (apply == RecordStatus.Active | apply == RecordStatus.Inactive)
               {
                  model.result = dpService.UpdateMultipleDepartment(departs, apply, userlogin.User_Authentication.Email_Address);
                  if (model.result.Code == ERROR_CODE.SUCCESS)
                  {
                     return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "department" });
                  }
               }
            }
         }
         model.countryList = cbService.LstCountry(true);
         model.stateList = new List<ComboViewModel>();
         model.stateBillingList = new List<ComboViewModel>();
         model.SubscriptionList = comService.LstSubscription(userlogin.Company_ID);
         model.Currency_List = cbService.LstCurrency(false);

         var users = uService.getUsers(userlogin.Company_ID);
         model.User_Count = users.Count();

         var com = comService.GetCompany(userlogin.Company_ID);
         if (com == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

         model.Company_Levelg = com.Company_Level;
         model.Company_ID = com.Company_ID;
         model.Company_Name = com.Name;
         model.No_Of_Employees = com.No_Of_Employees;
         model.Effective_Date = DateUtil.ToDisplayDate(com.Effective_Date);
         model.Address = com.Address;
         model.Country_ID = com.Country_ID;
         model.State_ID = com.State_ID;
         model.Zip_Code = com.Zip_Code;
         model.Billing_Address = com.Billing_Address;
         model.Billing_Country_ID = com.Billing_Country_ID;
         model.Billing_State_ID = com.Billing_State_ID;
         model.Billing_Zip_Code = com.Billing_Zip_Code;
         model.APIUsername = com.APIUsername;
         model.APIPassword = com.APIPassword;
         model.APISignature = com.APISignature;
         model.Currency_ID = com.Currency_ID;
         model.Is_Sandbox = com.Is_Sandbox.HasValue ? com.Is_Sandbox.Value : false;
         model.Company_Level = com.Company_Level;
         model.Fax = com.Fax;
         model.Phone = com.Phone;
         model.Business_Type = com.Business_Type;
         model.CPF_Submission_No = com.CPF_Submission_No;

         var logo = comService.GetLogo(com.Company_ID);
         if (logo != null)
         {
            model.Company_Logo_ID = logo.Company_Logo_ID;
            model.Company_Logo = logo.Logo;
         }

         model.LstCompanylevel = cbService.LstCompanylevel(com.Company_Level, true);

         if (model.Country_ID.HasValue)
            model.stateList = cbService.LstState(model.Country_ID.Value.ToString(), true);
         else if (model.countryList.Count() > 0)
            model.stateList = cbService.LstState(model.countryList[0].Value, true);

         if (model.Billing_Country_ID.HasValue)
            model.stateBillingList = cbService.LstState(model.Billing_Country_ID.Value.ToString(), true);
         else if (model.countryList.Count() > 0)
            model.stateBillingList = cbService.LstState(model.countryList[0].Value, true);

         /********** branch ***********/
         model.BranchList = bService.LstBranch(userlogin.Company_ID);

         /********** pattern ***********/
         model.Default_Emp_No_Next_Start = 1.ToString("000000");
         model.Is_Default_Emp_No_Next_Start = true;
         var patternService = new PatternService();
         var pattern = patternService.GetPattern(userlogin.Company_ID);
         if (pattern != null)
         {
            model.Select_Company_code = pattern.Select_Company_code;
            model.Select_Nationality = pattern.Select_Nationality;
            model.Select_Year = pattern.Select_Year;
            model.Year_2_Digit = pattern.Year_2_Digit;
            model.Year_4_Digit = pattern.Year_4_Digit;
            model.Company_Code = pattern.Company_Code;
            model.Employee_No_Pattern_ID = pattern.Employee_No_Pattern_ID;
            model.Select_Branch_Code = pattern.Select_Branch_Code;
            model.Pattern_Branch_ID = pattern.Branch_ID;
            model.Initiated = pattern.Initiated;
            if (pattern.Current_Running_Number.HasValue)
            {
               model.Default_Emp_No_Next_Start = pattern.Current_Running_Number.Value.ToString("000000");
               model.Current_Running_Number = pattern.Current_Running_Number.Value;
            }
         }

         /**************Business Type *****************/
         model.businessCatList = cbService.LstLookup(ComboType.Business_Category, null, false);

         /********** department ***********/
         model.statusList = cbService.LstRecordStatus();
         model.DepartmentList = dpService.LstDepartment(userlogin.Company_ID);

         /********** designation ***********/
         model.DesignationList = dsService.LstDesignation(userlogin.Company_ID);

         /********** fiscal ***********/
         model.Default_Fiscal_Year = com.Default_Fiscal_Year;
         model.Custom_Fiscal_Year = com.Custom_Fiscal_Year != null ? com.Custom_Fiscal_Year.Value.Day.ToString("00") + "/" + com.Custom_Fiscal_Year.Value.Month.ToString("00") : "";

         /********** working day ***********/
         model.Days = 5;
         var working = wService.GetWorkingDay(userlogin.Company_ID);
         if (working != null)
         {
            model.Working_Days_ID = working.Working_Days_ID;
            model.Days = working.Days;
            model.ST_Sun_Time = DateUtil.ToDisplayTime(working.ST_Sun_Time);
            model.ST_Mon_Time = DateUtil.ToDisplayTime(working.ST_Mon_Time);
            model.ST_Tue_Time = DateUtil.ToDisplayTime(working.ST_Tue_Time);
            model.ST_Wed_Time = DateUtil.ToDisplayTime(working.ST_Wed_Time);
            model.ST_Thu_Time = DateUtil.ToDisplayTime(working.ST_Thu_Time);
            model.ST_Fri_Time = DateUtil.ToDisplayTime(working.ST_Fri_Time);
            model.ST_Sat_Time = DateUtil.ToDisplayTime(working.ST_Sat_Time);
            model.ST_Lunch_Time = DateUtil.ToDisplayTime(working.ST_Lunch_Time);
            model.ET_Sun_Time = DateUtil.ToDisplayTime(working.ET_Sun_Time);
            model.ET_Mon_Time = DateUtil.ToDisplayTime(working.ET_Mon_Time);
            model.ET_Tue_Time = DateUtil.ToDisplayTime(working.ET_Tue_Time);
            model.ET_Wed_Time = DateUtil.ToDisplayTime(working.ET_Wed_Time);
            model.ET_Thu_Time = DateUtil.ToDisplayTime(working.ET_Thu_Time);
            model.ET_Fri_Time = DateUtil.ToDisplayTime(working.ET_Fri_Time);
            model.ET_Sat_Time = DateUtil.ToDisplayTime(working.ET_Sat_Time);
            model.ET_Lunch_Time = DateUtil.ToDisplayTime(working.ET_Lunch_Time);
            model.CL_Sun = working.CL_Sun;
            model.CL_Mon = working.CL_Mon;
            model.CL_Tue = working.CL_Tue;
            model.CL_Wed = working.CL_Wed;
            model.CL_Thu = working.CL_Thu;
            model.CL_Fri = working.CL_Fri;
            model.CL_Sat = working.CL_Sat;
            model.CL_Lunch = working.CL_Lunch;
         }

         /********** exchange ***********/
         var cEService = new ConfigService();
         model.ExchangeList = cEService.LstExchange(userlogin.Company_ID);

         return View(model);

      }

      [HttpPost]
      public ActionResult Configuration(ConfigulationViewModel model, string tabAction = "")
      {

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var currentdate = StoredProcedure.GetCurrentDate();
         var comService = new CompanyService();
         var bService = new BranchService();
         var dpService = new DepartmentService();
         var dsService = new DesignationService();
         var cEService = new ConfigService();

         model.tabAction = tabAction;
         if (tabAction == "company")
         {
            if (string.IsNullOrEmpty(model.Company_Name))
               ModelState.AddModelError("Company_Name", Resource.Message_Is_Required);
            if (string.IsNullOrEmpty(model.Address))
               ModelState.AddModelError("Address", Resource.Message_Is_Required);
            if (string.IsNullOrEmpty(model.Zip_Code))
               ModelState.AddModelError("Zip_Code", Resource.Message_Is_Required);
            if (string.IsNullOrEmpty(model.Phone))
               ModelState.AddModelError("Phone", Resource.Message_Is_Required);
            if (!model.Country_ID.HasValue)
               ModelState.AddModelError("Country_ID", Resource.Message_Is_Required);
            if (ModelState.IsValid)
            {
               var com = comService.GetCompany(userlogin.Company_ID);
               if (com == null)
                  return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

               com.Name = model.Company_Name;
               com.No_Of_Employees = model.No_Of_Employees;
               com.Address = model.Address;
               com.Country_ID = model.Country_ID;
               com.State_ID = model.State_ID;
               com.Zip_Code = model.Zip_Code;
               com.Billing_Address = model.Billing_Address;
               com.Billing_Country_ID = model.Billing_Country_ID;
               com.Billing_State_ID = model.Billing_State_ID;
               com.Billing_Zip_Code = model.Billing_Zip_Code;
               com.Update_By = userlogin.User_Authentication.Email_Address;
               com.Update_On = currentdate;
               com.APIUsername = model.APIUsername;
               com.APIPassword = model.APIPassword;
               com.APISignature = model.APISignature;
               com.Currency_ID = model.Currency_ID;
               com.Is_Sandbox = model.Is_Sandbox;
               com.Fax = model.Fax;
               com.Phone = model.Phone;
               com.CPF_Submission_No = model.CPF_Submission_No;
               com.Business_Type = model.Business_Type;

               model.result = comService.UpdateCompany(com);
               if (model.result.Code == ERROR_CODE.SUCCESS)
               {
                  return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "company" });
               }
            }
         }
         else if (tabAction == "pattern")
         {
            if (!model.Is_Default_Emp_No_Next_Start)
            {
               if (!model.Current_Running_Number.HasValue)
                  ModelState.AddModelError("Current_Running_Number", Resource.Message_Is_Required);
            }
            if (model.Select_Branch_Code.HasValue && model.Select_Branch_Code.Value)
            {
               if (!model.Pattern_Branch_ID.HasValue)
                  ModelState.AddModelError("Pattern_Branch_ID", Resource.Message_Is_Required);
            }
            else
               model.Pattern_Branch_ID = null;

            if (ModelState.IsValid)
            {
               model.Year_4_Digit = !model.Year_2_Digit;

               if (userlogin != null && userlogin.Company_ID.HasValue)
               {
                  var current_number = 1;
                  if (model.Current_Running_Number.HasValue)
                     current_number = model.Current_Running_Number.Value;
                  var Employee_No_Pattern = new Employee_No_Pattern
                  {
                     Select_Company_code = model.Select_Company_code,
                     Select_Nationality = model.Select_Nationality,
                     Select_Year = model.Select_Year.HasValue ? model.Select_Year.Value : false,
                     Year_2_Digit = model.Year_2_Digit,
                     Year_4_Digit = model.Year_4_Digit,
                     Company_Code = model.Company_Code,
                     Company_ID = userlogin.Company_ID.Value,
                     Current_Running_Number = current_number,
                     Select_Branch_Code = model.Select_Branch_Code,
                     Branch_ID = model.Pattern_Branch_ID
                  };
                  if (model.Employee_No_Pattern_ID.HasValue)
                     Employee_No_Pattern.Employee_No_Pattern_ID = model.Employee_No_Pattern_ID.Value;

                  var patternService = new PatternService();
                  model.result = patternService.SavePattern(Employee_No_Pattern);
                  if (model.result.Code == ERROR_CODE.SUCCESS)
                     return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "company" });
               }
            }
         }
         else if (tabAction == "branch")
         {
            if (string.IsNullOrEmpty(model.Branch_Code))
               ModelState.AddModelError("Branch_Code", Resource.Message_Is_Required);
            if (string.IsNullOrEmpty(model.Branch_Name))
               ModelState.AddModelError("Branch_Name", Resource.Message_Is_Required);
            if (ModelState.IsValid)
            {
               if (!model.Branch_ID.HasValue || model.Branch_ID.Value == 0)
               {
                  //insert
                  var branch = new Branch()
                  {
                     Branch_Code = model.Branch_Code,
                     Branch_Name = model.Branch_Name,
                     Branch_Desc = model.Branch_Desc,
                     Record_Status = model.Branch_Status,
                     Company_ID = userlogin.Company_ID.Value,
                     Create_By = userlogin.User_Authentication.Email_Address,
                     Create_On = currentdate,
                     Update_By = userlogin.User_Authentication.Email_Address,
                     Update_On = currentdate
                  };
                  model.result = bService.InsertBranch(branch);
                  if (model.result.Code == ERROR_CODE.SUCCESS)
                     return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "branch" });
               }
               else
               {
                  //update
                  var branch = bService.GetBranch(model.Branch_ID);
                  if (branch == null)
                     return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

                  branch.Branch_Code = model.Branch_Code;
                  branch.Branch_Name = model.Branch_Name;
                  branch.Branch_Desc = model.Branch_Desc;
                  branch.Record_Status = model.Branch_Status;
                  branch.Update_By = userlogin.User_Authentication.Email_Address;
                  branch.Update_On = currentdate;

                  model.result = bService.UpdateBranch(branch);
                  if (model.result.Code == ERROR_CODE.SUCCESS)
                     return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "branch" });
               }
            }
         }

         else if (tabAction == "department")
         {
            if (string.IsNullOrEmpty(model.Department_Name))
               ModelState.AddModelError("Department_Name", Resource.Message_Is_Required);
            if (string.IsNullOrEmpty(model.Department_Status))
               ModelState.AddModelError("Department_Status", Resource.Message_Is_Required);

            if (ModelState.IsValid)
            {
               if (!model.Department_ID.HasValue || model.Department_ID.Value == 0)
               {
                  //insert
                  var dp = new Department()
                  {
                     Name = model.Department_Name,
                     Record_Status = model.Department_Status,
                     Company_ID = userlogin.Company_ID.Value,
                     Create_By = userlogin.User_Authentication.Email_Address,
                     Create_On = currentdate,
                     Update_By = userlogin.User_Authentication.Email_Address,
                     Update_On = currentdate
                  };
                  model.result = dpService.InsertDepartment(dp);
                  if (model.result.Code == ERROR_CODE.SUCCESS)
                     return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "department" });
               }
               else
               {
                  //update
                  var dp = dpService.GetDepartment(model.Department_ID);
                  if (dp == null)
                     return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

                  dp.Name = model.Department_Name;
                  dp.Record_Status = model.Department_Status;
                  dp.Update_By = userlogin.User_Authentication.Email_Address;
                  dp.Update_On = currentdate;

                  model.result = dpService.UpdateDepartment(dp);
                  if (model.result.Code == ERROR_CODE.SUCCESS)
                     return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "department" });
               }
            }
         }
         else if (tabAction == "designation")
         {
            if (string.IsNullOrEmpty(model.Designation_Name))
               ModelState.AddModelError("Designation_Name", Resource.Message_Is_Required);
            if (string.IsNullOrEmpty(model.Designation_Status))
               ModelState.AddModelError("Designation_Status", Resource.Message_Is_Required);
            if (ModelState.IsValid)
            {
               if (!model.Designation_ID.HasValue || model.Designation_ID.Value == 0)
               {
                  //insert
                  var ds = new Designation()
                  {
                     Name = model.Designation_Name,
                     Record_Status = model.Designation_Status,
                     Company_ID = userlogin.Company_ID.Value,
                     Create_By = userlogin.User_Authentication.Email_Address,
                     Create_On = currentdate,
                     Update_By = userlogin.User_Authentication.Email_Address,
                     Update_On = currentdate
                  };
                  model.result = dsService.InsertDesignation(ds);
                  if (model.result.Code == ERROR_CODE.SUCCESS)
                     return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "designation" });
               }
               else
               {
                  //update
                  var ds = dsService.GetDesignation(model.Designation_ID);
                  if (ds == null)
                     return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

                  ds.Name = model.Designation_Name;
                  ds.Record_Status = model.Designation_Status;
                  ds.Update_By = userlogin.User_Authentication.Email_Address;
                  ds.Update_On = currentdate;

                  model.result = dsService.UpdateDesignation(ds);
                  if (model.result.Code == ERROR_CODE.SUCCESS)
                     return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "designation" });
               }
            }
         }
         if (tabAction == "fiscal")
         {
            var com = comService.GetCompany(userlogin.Company_ID);
            if (com == null)
               return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

            DateTime Custom_Fiscal_Year = new DateTime();
            if (!model.Default_Fiscal_Year.HasValue || !model.Default_Fiscal_Year.Value)
            {
               IFormatProvider culture = new System.Globalization.CultureInfo("fr-FR", true);
               try
               {
                  Custom_Fiscal_Year = DateTime.Parse(model.Custom_Fiscal_Year + "/" + currentdate.Year, culture, System.Globalization.DateTimeStyles.AssumeLocal);
               }
               catch
               {
                  ModelState.AddModelError("Custom_Fiscal_Year", Resource.Input_Error);
               }
            }

            if (ModelState.IsValid)
            {
               com.Default_Fiscal_Year = model.Default_Fiscal_Year;
               if (model.Default_Fiscal_Year.HasValue && model.Default_Fiscal_Year.Value)
               {
                  com.Custom_Fiscal_Year = null;
                  model.Custom_Fiscal_Year = "";
               }
               else
                  com.Custom_Fiscal_Year = Custom_Fiscal_Year;

               com.Update_By = userlogin.User_Authentication.Email_Address;
               com.Update_On = currentdate;

               model.result = comService.UpdateCompany(com);
               if (model.result.Code == ERROR_CODE.SUCCESS)
                  return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "fiscal" });
            }
         }
         else if (tabAction == "working")
         {


            var daysCnt = 0;
            if (model.CL_Sun.HasValue && model.CL_Sun.Value == false)
            {
               daysCnt++;
               if (string.IsNullOrEmpty(model.ST_Sun_Time))
                  ModelState.AddModelError("ST_Sun_Time", Resource.Message_Is_Required);

               if (string.IsNullOrEmpty(model.ET_Sun_Time))
                  ModelState.AddModelError("ET_Sun_Time", Resource.Message_Is_Required);

               if (!string.IsNullOrEmpty(model.ST_Sun_Time) && !string.IsNullOrEmpty(model.ET_Sun_Time))
               {
                  if (DateUtil.ToTime(model.ST_Sun_Time) > DateUtil.ToTime(model.ET_Sun_Time))
                  {
                     ModelState.AddModelError("ST_Sun_Time", Resource.The + " " + Resource.Start_Date + " " + Resource.Field + " " + Resource.Cannot_Be_More_Than_Lower + " " + Resource.The + " " + Resource.End_Date);
                     ModelState.AddModelError("ET_Sun_Time", Resource.The + " " + Resource.End_Date + " " + Resource.Field + " " + Resource.Cannot_Be_Less_Than_Lower + " " + Resource.The + " " + Resource.Start_Date);
                  }
               }
            }
            if (model.CL_Mon.HasValue && model.CL_Mon.Value == false)
            {
               daysCnt++;
               if (string.IsNullOrEmpty(model.ST_Mon_Time))
                  ModelState.AddModelError("ST_Mon_Time", Resource.Message_Is_Required);

               if (string.IsNullOrEmpty(model.ET_Mon_Time))
                  ModelState.AddModelError("ET_Mon_Time", Resource.Message_Is_Required);

               if (!string.IsNullOrEmpty(model.ST_Mon_Time) && !string.IsNullOrEmpty(model.ET_Mon_Time))
               {
                  if (DateUtil.ToTime(model.ST_Mon_Time) > DateUtil.ToTime(model.ET_Mon_Time))
                  {
                     ModelState.AddModelError("ST_Mon_Time", Resource.The + " " + Resource.Start_Date + " " + Resource.Field + " " + Resource.Cannot_Be_More_Than_Lower + " " + Resource.The + " " + Resource.End_Date);
                     ModelState.AddModelError("ET_Mon_Time", Resource.The + " " + Resource.End_Date + " " + Resource.Field + " " + Resource.Cannot_Be_Less_Than_Lower + " " + Resource.The + " " + Resource.Start_Date);
                  }
               }
            }
            if (model.CL_Tue.HasValue && model.CL_Tue.Value == false)
            {
               daysCnt++;
               if (string.IsNullOrEmpty(model.ST_Tue_Time))
                  ModelState.AddModelError("ST_Tue_Time", Resource.Message_Is_Required);

               if (string.IsNullOrEmpty(model.ET_Tue_Time))
                  ModelState.AddModelError("ET_Tue_Time", Resource.Message_Is_Required);

               if (!string.IsNullOrEmpty(model.ST_Tue_Time) && !string.IsNullOrEmpty(model.ET_Tue_Time))
               {
                  if (DateUtil.ToTime(model.ST_Tue_Time) > DateUtil.ToTime(model.ET_Tue_Time))
                  {
                     ModelState.AddModelError("ST_Tue_Time", Resource.The + " " + Resource.Start_Date + " " + Resource.Field + " " + Resource.Cannot_Be_More_Than_Lower + " " + Resource.The + " " + Resource.End_Date);
                     ModelState.AddModelError("ET_Tue_Time", Resource.The + " " + Resource.End_Date + " " + Resource.Field + " " + Resource.Cannot_Be_Less_Than_Lower + " " + Resource.The + " " + Resource.Start_Date);
                  }
               }
            }
            if (model.CL_Wed.HasValue && model.CL_Wed.Value == false)
            {
               daysCnt++;
               if (string.IsNullOrEmpty(model.ST_Wed_Time))
                  ModelState.AddModelError("ST_Wed_Time", Resource.Message_Is_Required);

               if (string.IsNullOrEmpty(model.ET_Wed_Time))
                  ModelState.AddModelError("ET_Wed_Time", Resource.Message_Is_Required);

               if (!string.IsNullOrEmpty(model.ST_Wed_Time) && !string.IsNullOrEmpty(model.ET_Wed_Time))
               {
                  if (DateUtil.ToTime(model.ST_Wed_Time) > DateUtil.ToTime(model.ET_Wed_Time))
                  {
                     ModelState.AddModelError("ST_Wed_Time", Resource.The + " " + Resource.Start_Date + " " + Resource.Field + " " + Resource.Cannot_Be_More_Than_Lower + " " + Resource.The + " " + Resource.End_Date);
                     ModelState.AddModelError("ET_Wed_Time", Resource.The + " " + Resource.End_Date + " " + Resource.Field + " " + Resource.Cannot_Be_Less_Than_Lower + " " + Resource.The + " " + Resource.Start_Date);
                  }
               }
            }
            if (model.CL_Thu.HasValue && model.CL_Thu.Value == false)
            {
               daysCnt++;
               if (string.IsNullOrEmpty(model.ST_Thu_Time))
                  ModelState.AddModelError("ST_Thu_Time", Resource.Message_Is_Required);

               if (string.IsNullOrEmpty(model.ET_Thu_Time))
                  ModelState.AddModelError("ET_Thu_Time", Resource.Message_Is_Required);

               if (!string.IsNullOrEmpty(model.ST_Thu_Time) && !string.IsNullOrEmpty(model.ET_Thu_Time))
               {
                  if (DateUtil.ToTime(model.ST_Thu_Time) > DateUtil.ToTime(model.ET_Thu_Time))
                  {
                     ModelState.AddModelError("ST_Thu_Time", Resource.The + " " + Resource.Start_Date + " " + Resource.Field + " " + Resource.Cannot_Be_More_Than_Lower + " " + Resource.The + " " + Resource.End_Date);
                     ModelState.AddModelError("ET_Thu_Time", Resource.The + " " + Resource.End_Date + " " + Resource.Field + " " + Resource.Cannot_Be_Less_Than_Lower + " " + Resource.The + " " + Resource.Start_Date);
                  }
               }
            }
            if (model.CL_Fri.HasValue && model.CL_Fri.Value == false)
            {
               daysCnt++;
               if (string.IsNullOrEmpty(model.ST_Fri_Time))
                  ModelState.AddModelError("ST_Fri_Time", Resource.Message_Is_Required);

               if (string.IsNullOrEmpty(model.ET_Fri_Time))
                  ModelState.AddModelError("ET_Fri_Time", Resource.Message_Is_Required);

               if (!string.IsNullOrEmpty(model.ST_Fri_Time) && !string.IsNullOrEmpty(model.ET_Fri_Time))
               {
                  if (DateUtil.ToTime(model.ST_Fri_Time) > DateUtil.ToTime(model.ET_Fri_Time))
                  {
                     ModelState.AddModelError("ST_Fri_Time", Resource.The + " " + Resource.Start_Date + " " + Resource.Field + " " + Resource.Cannot_Be_More_Than_Lower + " " + Resource.The + " " + Resource.End_Date);
                     ModelState.AddModelError("ET_Fri_Time", Resource.The + " " + Resource.End_Date + " " + Resource.Field + " " + Resource.Cannot_Be_Less_Than_Lower + " " + Resource.The + " " + Resource.Start_Date);
                  }
               }
            }
            if (model.CL_Sat.HasValue && model.CL_Sat.Value == false)
            {
               daysCnt++;
               if (string.IsNullOrEmpty(model.ST_Sat_Time))
                  ModelState.AddModelError("ST_Sat_Time", Resource.Message_Is_Required);

               if (string.IsNullOrEmpty(model.ET_Sat_Time))
                  ModelState.AddModelError("ET_Sat_Time", Resource.Message_Is_Required);

               if (!string.IsNullOrEmpty(model.ST_Sat_Time) && !string.IsNullOrEmpty(model.ET_Sat_Time))
               {
                  if (DateUtil.ToTime(model.ST_Sat_Time) > DateUtil.ToTime(model.ET_Sat_Time))
                  {
                     ModelState.AddModelError("ST_Sat_Time", Resource.The + " " + Resource.Start_Date + " " + Resource.Field + " " + Resource.Cannot_Be_More_Than_Lower + " " + Resource.The + " " + Resource.End_Date);
                     ModelState.AddModelError("ET_Sat_Time", Resource.The + " " + Resource.End_Date + " " + Resource.Field + " " + Resource.Cannot_Be_Less_Than_Lower + " " + Resource.The + " " + Resource.Start_Date);
                  }
               }
            }

            if (model.CL_Lunch.HasValue && model.CL_Lunch.Value == false)
            {
               if (string.IsNullOrEmpty(model.ST_Lunch_Time))
                  ModelState.AddModelError("ST_Lunch_Time", Resource.Message_Is_Required);

               if (string.IsNullOrEmpty(model.ET_Lunch_Time))
                  ModelState.AddModelError("ET_Lunch_Time", Resource.Message_Is_Required);

               if (!string.IsNullOrEmpty(model.ST_Lunch_Time) && !string.IsNullOrEmpty(model.ET_Lunch_Time))
               {
                  if (DateUtil.ToTime(model.ST_Lunch_Time) > DateUtil.ToTime(model.ET_Lunch_Time))
                  {
                     ModelState.AddModelError("ST_Lunch_Time", Resource.The + " " + Resource.Start_Date + " " + Resource.Field + " " + Resource.Cannot_Be_More_Than_Lower + " " + Resource.The + " " + Resource.End_Date);
                     ModelState.AddModelError("ET_Lunch_Time", Resource.The + " " + Resource.End_Date + " " + Resource.Field + " " + Resource.Cannot_Be_Less_Than_Lower + " " + Resource.The + " " + Resource.Start_Date);
                  }
               }
            }

            if (model.Days.HasValue)
            {
               if (model.Days.Value == 5 && daysCnt != 5)
                  ModelState.AddModelError("Days", Resource.Message_Please_Open_5);
               else if (model.Days.Value == 5.5M && daysCnt != 6)
                  ModelState.AddModelError("Days", Resource.Message_Please_Open_6);
               else if (model.Days.Value == 6 && daysCnt != 6)
                  ModelState.AddModelError("Days", Resource.Message_Please_Open_6);
               else if (model.Days.Value == 7 && daysCnt != 7)
                  ModelState.AddModelError("Days", Resource.Message_Please_Open_7);
            }
            if (ModelState.IsValid)
            {
               var working = new Working_Days();
               working.Days = model.Days;
               working.ST_Sun_Time = DateUtil.ToTime(model.ST_Sun_Time);
               working.ST_Mon_Time = DateUtil.ToTime(model.ST_Mon_Time);
               working.ST_Tue_Time = DateUtil.ToTime(model.ST_Tue_Time);
               working.ST_Wed_Time = DateUtil.ToTime(model.ST_Wed_Time);
               working.ST_Thu_Time = DateUtil.ToTime(model.ST_Thu_Time);
               working.ST_Fri_Time = DateUtil.ToTime(model.ST_Fri_Time);
               working.ST_Sat_Time = DateUtil.ToTime(model.ST_Sat_Time);
               working.ST_Lunch_Time = DateUtil.ToTime(model.ST_Lunch_Time);
               working.ET_Sun_Time = DateUtil.ToTime(model.ET_Sun_Time);
               working.ET_Mon_Time = DateUtil.ToTime(model.ET_Mon_Time);
               working.ET_Tue_Time = DateUtil.ToTime(model.ET_Tue_Time);
               working.ET_Wed_Time = DateUtil.ToTime(model.ET_Wed_Time);
               working.ET_Thu_Time = DateUtil.ToTime(model.ET_Thu_Time);
               working.ET_Fri_Time = DateUtil.ToTime(model.ET_Fri_Time);
               working.ET_Sat_Time = DateUtil.ToTime(model.ET_Sat_Time);
               working.ET_Lunch_Time = DateUtil.ToTime(model.ET_Lunch_Time);
               working.CL_Sun = model.CL_Sun;
               working.CL_Mon = model.CL_Mon;
               working.CL_Tue = model.CL_Tue;
               working.CL_Wed = model.CL_Wed;
               working.CL_Thu = model.CL_Thu;
               working.CL_Fri = model.CL_Fri;
               working.CL_Sat = model.CL_Sat;
               working.CL_Lunch = model.CL_Lunch;
               working.Company_ID = userlogin.Company_ID;
               working.Update_By = userlogin.User_Authentication.Email_Address;
               working.Update_On = currentdate;

               var wService = new WorkingDaysService();
               if (model.Working_Days_ID.HasValue && model.Working_Days_ID.Value > 0)
               {
                  // update
                  working.Working_Days_ID = model.Working_Days_ID.Value;
                  model.result = wService.UpdateWorkingDays(working);
                  if (model.result.Code == ERROR_CODE.SUCCESS)
                  {
                     return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "working" });
                  }
               }
               else
               {
                  //insert
                  working.Create_By = userlogin.User_Authentication.Email_Address;
                  working.Create_On = currentdate;

                  model.result = wService.InsertWorkingDays(working);
                  if (model.result.Code == ERROR_CODE.SUCCESS)
                     return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "working" });
               }
            }
         }

         RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A);
         if (rightResult.action != null)
            return rightResult.action;
         model.rights = rightResult.rights;

         var cbService = new ComboService();
         var uService = new UserService();
         var wdService = new WorkingDaysService();

         model.countryList = cbService.LstCountry(true);
         model.stateList = new List<ComboViewModel>();
         model.stateBillingList = new List<ComboViewModel>();
         model.SubscriptionList = comService.LstSubscription(userlogin.Company_ID);
         model.Currency_List = cbService.LstCurrency(false);

         var users = uService.getUsers(userlogin.Company_ID);
         model.User_Count = users.Count();

         var recom = comService.GetCompany(userlogin.Company_ID);
         if (recom == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

         model.Company_ID = recom.Company_ID;
         model.Company_Name = recom.Name;
         model.No_Of_Employees = recom.No_Of_Employees;
         model.Effective_Date = DateUtil.ToDisplayDate(recom.Effective_Date);
         model.Address = recom.Address;
         model.Country_ID = recom.Country_ID;
         model.State_ID = recom.State_ID;
         model.Zip_Code = recom.Zip_Code;
         model.Billing_Address = recom.Billing_Address;
         model.Billing_Country_ID = recom.Billing_Country_ID;
         model.Billing_State_ID = recom.Billing_State_ID;
         model.Billing_Zip_Code = recom.Billing_Zip_Code;
         model.APIUsername = recom.APIUsername;
         model.APIPassword = recom.APIPassword;
         model.APISignature = recom.APISignature;
         model.Currency_ID = recom.Currency_ID;
         model.Is_Sandbox = recom.Is_Sandbox.HasValue ? recom.Is_Sandbox.Value : false;
         model.Company_Level = recom.Company_Level;
         model.Phone = recom.Phone;
         model.Fax = recom.Fax;
         model.CPF_Submission_No = recom.CPF_Submission_No;

         var logo = comService.GetLogo(recom.Company_ID);
         if (logo != null)
         {
            model.Company_Logo_ID = logo.Company_Logo_ID;
            model.Company_Logo = logo.Logo;
         }

         if (model.Country_ID.HasValue)
            model.stateList = cbService.LstState(model.Country_ID.Value.ToString(), true);
         else if (model.countryList.Count() > 0)
            model.stateList = cbService.LstState(model.countryList[0].Value, true);

         if (model.Billing_Country_ID.HasValue)
            model.stateBillingList = cbService.LstState(model.Billing_Country_ID.Value.ToString(), true);
         else if (model.countryList.Count() > 0)
            model.stateBillingList = cbService.LstState(model.countryList[0].Value, true);

         model.LstCompanylevel = cbService.LstCompanylevel(model.Company_Level);

         /********** branch ***********/
         model.BranchList = bService.LstBranch(userlogin.Company_ID);

         /********** pattern ***********/
         model.Default_Emp_No_Next_Start = 1.ToString("000000");
         model.Is_Default_Emp_No_Next_Start = true;
         var repatternService = new PatternService();
         var pattern = repatternService.GetPattern(userlogin.Company_ID);
         if (pattern != null)
         {
            model.Select_Company_code = pattern.Select_Company_code;
            model.Select_Nationality = pattern.Select_Nationality;
            model.Year_2_Digit = pattern.Year_2_Digit;
            model.Year_4_Digit = pattern.Year_4_Digit;
            model.Company_Code = pattern.Company_Code;
            model.Employee_No_Pattern_ID = pattern.Employee_No_Pattern_ID;
            model.Select_Branch_Code = pattern.Select_Branch_Code;
            model.Pattern_Branch_ID = pattern.Branch_ID;
            if (pattern.Current_Running_Number.HasValue)
            {
               model.Default_Emp_No_Next_Start = pattern.Current_Running_Number.Value.ToString("000000");
               model.Current_Running_Number = pattern.Current_Running_Number.Value;
            }
         }

         /**************Business Type *****************/
         model.businessCatList = cbService.LstLookup(ComboType.Business_Category, null, false);

         /********** department ***********/
         model.statusList = cbService.LstRecordStatus();
         model.DepartmentList = dpService.LstDepartment(userlogin.Company_ID);

         /********** designation ***********/
         model.DesignationList = dsService.LstDesignation(userlogin.Company_ID);

         /********** fiscal ***********/
         model.Default_Fiscal_Year = recom.Default_Fiscal_Year;
         model.Custom_Fiscal_Year = recom.Custom_Fiscal_Year != null ? recom.Custom_Fiscal_Year.Value.Day.ToString("00") + "/" + recom.Custom_Fiscal_Year.Value.Month.ToString("00") : "";

         /********** working date ***********/
         if (tabAction != "working")
         {

            model.Days = 5;
            var reworking = wdService.GetWorkingDay(userlogin.Company_ID);
            if (reworking != null)
            {
               model.Working_Days_ID = reworking.Working_Days_ID;
               model.Days = reworking.Days;
               model.ST_Sun_Time = DateUtil.ToDisplayTime(reworking.ST_Sun_Time);
               model.ST_Mon_Time = DateUtil.ToDisplayTime(reworking.ST_Mon_Time);
               model.ST_Tue_Time = DateUtil.ToDisplayTime(reworking.ST_Tue_Time);
               model.ST_Wed_Time = DateUtil.ToDisplayTime(reworking.ST_Wed_Time);
               model.ST_Thu_Time = DateUtil.ToDisplayTime(reworking.ST_Thu_Time);
               model.ST_Fri_Time = DateUtil.ToDisplayTime(reworking.ST_Fri_Time);
               model.ST_Sat_Time = DateUtil.ToDisplayTime(reworking.ST_Sat_Time);
               model.ST_Lunch_Time = DateUtil.ToDisplayTime(reworking.ST_Lunch_Time);
               model.ET_Sun_Time = DateUtil.ToDisplayTime(reworking.ET_Sun_Time);
               model.ET_Mon_Time = DateUtil.ToDisplayTime(reworking.ET_Mon_Time);
               model.ET_Tue_Time = DateUtil.ToDisplayTime(reworking.ET_Tue_Time);
               model.ET_Wed_Time = DateUtil.ToDisplayTime(reworking.ET_Wed_Time);
               model.ET_Thu_Time = DateUtil.ToDisplayTime(reworking.ET_Thu_Time);
               model.ET_Fri_Time = DateUtil.ToDisplayTime(reworking.ET_Fri_Time);
               model.ET_Sat_Time = DateUtil.ToDisplayTime(reworking.ET_Sat_Time);
               model.ET_Lunch_Time = DateUtil.ToDisplayTime(reworking.ET_Lunch_Time);
               model.CL_Sun = reworking.CL_Sun;
               model.CL_Mon = reworking.CL_Mon;
               model.CL_Tue = reworking.CL_Tue;
               model.CL_Wed = reworking.CL_Wed;
               model.CL_Thu = reworking.CL_Thu;
               model.CL_Fri = reworking.CL_Fri;
               model.CL_Sat = reworking.CL_Sat;
               model.CL_Lunch = reworking.CL_Lunch;
            }
         }
         /********** exchange ***********/
         model.ExchangeList = cEService.LstExchange(userlogin.Company_ID);

         return View(model);
      }

      public ActionResult CheckDepartmentRelation(Nullable<int> pDepartmentID)
      {

         var related = new DepartmentService().IsDepartmentRelated(pDepartmentID);
         return Json(new { related = related }, JsonRequestBehavior.AllowGet);
      }

      public ActionResult CheckDesignationRelation(Nullable<int> pDesignationID)
      {

         var related = new DesignationService().IsDesignationRelated(pDesignationID);
         return Json(new { related = related }, JsonRequestBehavior.AllowGet);
      }

      [HttpGet]
      public ActionResult ExchangeDelete(string pExchangeID, string operation, string tabAction = "")
      {
         var model = new ConfigulationViewModel();
         var userlogin = UserSession.getUser(HttpContext);
         var exCurService = new ExchangeCurrencyConfigService();
         var eid = NumUtil.ParseInteger(EncryptUtil.Decrypt(pExchangeID));
         var oid = EncryptUtil.Decrypt(operation);

         if (eid != 0 && oid == UserSession.RIGHT_D)
         {
            var apply = RecordStatus.Delete;
            model.result = exCurService.UpdateMultipleDeleteExchangeStatus(eid, apply, userlogin.User_Authentication.Email_Address);
            if (model.result.Code == ERROR_CODE.SUCCESS)
            {
               return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "exchange" });
            }
         }
         return View(model);
      }

      [HttpGet]
      public ActionResult ExchangeRateInfo(ServiceResult result, string pExchangeID, string pYear, string operation)
      {

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var model = new ExchangeViewModel();
         var currentdate = StoredProcedure.GetCurrentDate();
         var cbService = new ComboService();
         var comService = new CompanyService();
         var exCurService = new ExchangeCurrencyConfigService();
         List<int> chk_Exchange_Currency = new List<int>();

         var eid = NumUtil.ParseInteger(EncryptUtil.Decrypt(pExchangeID));
         model.Exchange_ID = eid;

         var oid = EncryptUtil.Decrypt(operation);
         model.operation = oid;

         //-------rights------------
         RightResult rightResult = base.validatePageRight(model.operation, "/Configuration/Configuration");
         if (rightResult.action != null)
            return rightResult.action;
         model.rights = rightResult.rights;
         model.result = result;

         model.Fiscal_Year = NumUtil.ParseInteger(pYear);
         model.Company_ID = userlogin.Company_ID.Value;

         var com = comService.GetCompany(userlogin.Company_ID);
         if (com == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Company);

         var curr = com.Currency;
         if (curr == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Currency);

         model.Company_Currency_Code = curr.Currency_Code;
         model.Currency_List = cbService.LstCurrency(hasBlank: false);
         model.Month_List = cbService.LstMonth();

         if (model.operation == UserSession.RIGHT_C)
         {
            if (!model.Fiscal_Year.HasValue || model.Fiscal_Year.Value == 0)
            {
               model.Fiscal_Year = null;
               //model.Fiscal_Year = currentdate.Year;
            }
         }
         else if (model.operation == UserSession.RIGHT_U)
         {
            if (curr.Currency_ID != 0 && eid != 0)
            {
               var LocalNameCode = curr.Currency_Name + " - " + curr.Currency_Code;
               Exchange ex = exCurService.GetExchangen(userlogin.Company_ID.Value, eid);

               model.Exchange_ID = ex.Exchange_ID;
               model.Fiscal_Year = ex.Fiscal_Year;
               model.Record_Status = ex.Record_Status;

               if (ex.Exchange_Currency != null && ex.Exchange_Currency.Count > 0)
               {
                  var ExchangeCurrency = new List<ExchangeCurrencyViewModel>();
                  foreach (var row in ex.Exchange_Currency)
                  {
                     var TabCode = "";
                     var exchangeRate = new List<ExchangeRateViewModel>();

                     //get tab
                     if (row.Currency_ID != null)
                     {
                        var ForeignCode = cbService.GetCurrency(row.Currency_ID);
                        if (ForeignCode != null)
                        {
                           TabCode = ForeignCode.Currency_Code + " " + Resource.To + " " + curr.Currency_Code;
                        }
                     }

                     if (row.Exchange_Rate.Count != 0)
                     {
                        var monthTemp = 1;

                        var selecttemp = row.Exchange_Rate.Where(w => w.Exchange_Period == row.Exchange_Period && w.Exchange_Currency_ID == row.Exchange_Currency_ID);
                        if (row.Exchange_Period == ExchangePeriod.ByDate)
                        {
                           selecttemp = selecttemp.GroupBy(w => w.Exchange_Month).FirstOrDefault();
                        }
                        var ch_Count_Test = selecttemp.Count();

                        foreach (var prow in selecttemp)
                        {

                           var fullMonth = DateUtil.GetFullMonth(NumUtil.ParseInteger(prow.Exchange_Month.Value));
                           exchangeRate.Add(new ExchangeRateViewModel()
                           {
                              Exchange_Rate_ID = prow.Exchange_Rate_ID,
                              Exchange_Currency_ID = row.Exchange_Currency_ID,
                              Rate = prow.Rate,
                              Exchange_Period = prow.Exchange_Period,
                              Exchange_Date = DateUtil.ToDisplayDate(prow.Exchange_Date),
                              Exchange_Month = prow.Exchange_Month.Value,
                              Exchange_Month_Text = fullMonth,
                              Row_Type = RowType.EDIT
                           });

                           if (row.Exchange_Period == ExchangePeriod.ByDate)
                           {
                              monthTemp = prow.Exchange_Month.Value;
                           }
                        }

                        ExchangeCurrency.Add(new ExchangeCurrencyViewModel()
                        {
                           Month_ID = monthTemp,
                           Company_Currency_Name = LocalNameCode,
                           Top_Name = TabCode,
                           Exchange_Currency_ID = row.Exchange_Currency_ID,
                           Exchange_ID = row.Exchange_ID,
                           Currency_ID = row.Currency_ID,
                           Exchange_Period = row.Exchange_Period,
                           Row_Type = RowType.EDIT,
                           ExchangeRate_Rows = exchangeRate.ToArray()
                        });
                     }
                  }
                  model.LstExchangeCurrency = ExchangeCurrency;
               }
            }
         }

         return View(model);

      }

      public ActionResult AddNewCurrency(int pIndex)
      {

         var userlogin = UserSession.getUser(HttpContext);
         var cbService = new ComboService();
         var model = new ExchangeCurrencyViewModel() { Index = pIndex };
         var comService = new CompanyService();
         var currentdate = StoredProcedure.GetCurrentDate();

         var com = comService.GetCompany(userlogin.Company_ID);
         if (com == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Company);

         model.Currency_List = cbService.LstCurrency(hasBlank: false);
         model.Exchange_Period = ExchangePeriod.ByDate;

         var curr = com.Currency;
         if (curr == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Currency);

         model.Company_Currency_Name = curr.Currency_Name + " - " + curr.Currency_Code;
         model.Top_Name = Resource.To + " " + curr.Currency_Code;

         if (model.Currency_List != null && model.Currency_List.Count > 0)
            model.Currency_ID = NumUtil.ParseInteger(model.Currency_List[0].Value);

         model.Index = pIndex;
         model.Row_Type = RowType.ADD;

         return PartialView("ExchangeRateInfoRow", model);

      }

      public ActionResult AddExchangeRate(string pCurrencyID, int pBrowID, string pMonthID, Nullable<int> pYear, string pExPeriod)
      {

         //-------data------------
         var cbService = new ComboService();
         var model = new ExchangeRateViewModel();
         var comService = new CompanyService();
         var currentdate = StoredProcedure.GetCurrentDate();

         model.Index = pBrowID;

         model.Month_ID = NumUtil.ParseInteger(pMonthID);
         if (!model.Month_ID.HasValue || model.Month_ID.Value == 0)
            model.Month_ID = currentdate.Month;

         model.Exchange_Month = model.Month_ID;
         model.Exchange_Month_Text = DateUtil.GetFullMonth(model.Month_ID);
         model.Date_List = cbService.GetDates(pYear.HasValue ? pYear.Value : currentdate.Year, model.Month_ID.Value);

         var curr = cbService.GetCurrency(NumUtil.ParseInteger(pCurrencyID));
         if (curr != null)
         {
            model.Currency_ID = curr.Currency_ID;
            model.Currency_Code = curr.Currency_Code;
            model.Currency_Name = curr.Currency_Name;
         }

         model.Month_List = cbService.LstMonth(hasBlank: false);

         if (string.IsNullOrEmpty(pExPeriod))
            pExPeriod = ExchangePeriod.ByDate;
         model.Exchange_Period = pExPeriod;// By Date/Month

         return PartialView("ExchangeRateInfoDetailRow", model);

      }

      public ActionResult GetCurrencyCode(Nullable<int> pCurrencyId)
      {

         var cbService = new ComboService();
         if (pCurrencyId.HasValue)
         {
            var CurrencyCode = cbService.GetCurrency(pCurrencyId);
            if (CurrencyCode != null)
            {
               return Json(new
               {
                  currencycode = CurrencyCode.Currency_Code,
                  currencyId = pCurrencyId,
               }, JsonRequestBehavior.AllowGet);
            }
         }
         return Json(new { currencycode = "", currencyId = 0 }, JsonRequestBehavior.AllowGet);

      }

      public ActionResult GetDuplicate(Nullable<int> pYear, string pOperation, Nullable<int> pexchange)
      {
         var userlogin = UserSession.getUser(HttpContext);
         var cEService = new ConfigService();

         if (pYear.HasValue && pOperation != null)
         {
            bool Check = true;
            var dupLst = cEService.LstExchange(userlogin.Company_ID);
            if (dupLst != null)
            {
               var dup = dupLst.Where(w => w.Fiscal_Year == pYear).FirstOrDefault();
               if (dup != null && dup.Fiscal_Year == pYear)
               {
                  if (pOperation == UserSession.RIGHT_C)
                  {
                     Check = false;
                  }
                  else if (pOperation == UserSession.RIGHT_U)
                  {
                     if (dup.Exchange_ID != pexchange)
                     {
                        Check = false;
                     }
                  }
               }
            }
            return Json(new { checkduplicate = Check }, JsonRequestBehavior.AllowGet);
         }
         return Json(new { checkduplicate = false }, JsonRequestBehavior.AllowGet);

      }

      [HttpPost]
      public ActionResult ExchangeRateInfo(ExchangeViewModel model, string pCurrencyID, string pMonth, string pExPeriod, string pTapActive, string pageAction = "")
      {

         //-------rights------------
         RightResult rightResult = base.validatePageRight(model.operation, "/Configuration/Configuration");
         if (rightResult.action != null)
            return rightResult.action;
         model.rights = rightResult.rights;

         model.tabAction = pTapActive;

         var userlogin = UserSession.getUser(HttpContext);
         var currentdate = StoredProcedure.GetCurrentDate();
         var cbService = new ComboService();
         var exCurService = new ExchangeCurrencyConfigService();
         var cEService = new ConfigService();
         var comService = new CompanyService();
         List<int> chk_Exchange_Currency = new List<int>();

         var curr = comService.GetCompany(userlogin.Company_ID).Currency;

         if (model.ExchangeCurrency_Rows != null)
         {
            var i = 0;
            foreach (var row in model.ExchangeCurrency_Rows)
            {
               if (row.Row_Type == RowType.DELETE)
               {
                  DeleteModelStateError("ExchangeCurrency_Rows[" + i + "]");
               }
               else if (row.Row_Type == RowType.ADD || row.Row_Type == RowType.EDIT)
               {
                  chk_Exchange_Currency.Add(row.Currency_ID.Value);
               }
               i++;
            }

            if (chk_Exchange_Currency.GroupBy(n => n).Any(c => c.Count() > 1))
            {
               ModelState.AddModelError("ExchangeCurrency_Rows", Resource.Message_Is_Duplicated);
            }
         }

         if (pageAction == "edit")
         {
            model.Date_List = new List<DateTime>();
            var Month = NumUtil.ParseInteger(pMonth);

            var CurrencyID = NumUtil.ParseInteger(pCurrencyID);
            if (CurrencyID == 0)
               return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Currency);

            if (curr != null)
            {
               model.Company_Currency_Code = curr.Currency_Code;
            }

            model.Date_List = new List<DateTime>();
            if (pExPeriod == ExchangePeriod.ByDate)
            {
               int newMonth = 1;
               if (Month != 0)
                  newMonth = Month;

               model.Date_List = cbService.GetDates(model.Fiscal_Year.HasValue ? model.Fiscal_Year.Value : currentdate.Year, newMonth);
            }

            model.Month_List = cbService.LstMonth(hasBlank: false);
            var LocalNameCode = curr.Currency_Name + " - " + curr.Currency_Code;

            if (model.ExchangeCurrency_Rows != null && model.ExchangeCurrency_Rows.Count() > 0)
            {
               Exchange ex = exCurService.GetExchangen(userlogin.Company_ID.Value, model.Exchange_ID);
               var ExchangeCurrency = new List<ExchangeCurrencyViewModel>();
               foreach (var row in model.ExchangeCurrency_Rows)
               {
                  Month = NumUtil.ParseInteger(pMonth);
                  var TabCode = "";
                  var exchangeRate = new List<ExchangeRateViewModel>();

                  if (row.Row_Type != RowType.DELETE)
                  {
                     //re get tab
                     if (row.Currency_ID != null)
                     {
                        var ForeignCode = cbService.GetCurrency(row.Currency_ID);
                        if (ForeignCode != null)
                        {
                           TabCode = ForeignCode.Currency_Code + " " + Resource.Message_To_Lower + " " + curr.Currency_Code;
                        }
                     }

                     if (row.ExchangeRate_Rows.Count() > 0)
                     {
                        if (row.Row_Type == RowType.EDIT)
                        {
                           var selecttemp = ex.Exchange_Currency.Where(w => w.Exchange_Currency_ID == row.Exchange_Currency_ID).FirstOrDefault();

                           //Error
                           var regetExchangeRate = exCurService.LstExchangeRate(row.Exchange_Currency_ID, pExPeriod).ToList();
                           var Ex_Rate = regetExchangeRate.Where(w => w.Exchange_Period == pExPeriod).FirstOrDefault();

                           //if (pExPeriod == ExchangePeriod.ByDate)
                           //{
                           //    regetExchangeRate = exCurService.LstExchangeRate(row.Exchange_Currency_ID, pExPeriod).Where(w => w.Exchange_Month == Month).ToList();
                           //}

                           if ((pExPeriod == ExchangePeriod.ByDate) && (selecttemp != null) && (selecttemp.Currency_ID == CurrencyID))
                           {
                              regetExchangeRate = exCurService.LstExchangeRate(row.Exchange_Currency_ID, pExPeriod).Where(w => w.Exchange_Month == Month).ToList();

                           }

                           if ((pExPeriod == ExchangePeriod.ByMonth) && (selecttemp != null) && (selecttemp.Currency_ID == CurrencyID))
                           {
                              Ex_Rate = regetExchangeRate.Where(w => w.Exchange_Month == Month && w.Exchange_Period == pExPeriod).FirstOrDefault();
                           }

                           if (selecttemp != null && selecttemp.Currency_ID == CurrencyID)
                           {

                              if (Ex_Rate != null && regetExchangeRate.Count != 0)
                              {

                                 foreach (var trow in regetExchangeRate)
                                 {

                                    var fullMonth = DateUtil.GetFullMonth(NumUtil.ParseInteger(trow.Exchange_Month.HasValue ? trow.Exchange_Month.Value : 1));
                                    exchangeRate.Add(new ExchangeRateViewModel()
                                    {
                                       Exchange_Rate_ID = trow.Exchange_Rate_ID,
                                       Exchange_Currency_ID = row.Exchange_Currency_ID,
                                       Rate = trow.Rate,
                                       Exchange_Period = row.Exchange_Period,
                                       Exchange_Date = DateUtil.ToDisplayDate(trow.Exchange_Date),
                                       Exchange_Month = trow.Exchange_Month.Value,
                                       Exchange_Month_Text = fullMonth,
                                       Row_Type = RowType.EDIT
                                    });

                                    Month = trow.Exchange_Month.HasValue ? trow.Exchange_Month.Value : 1;
                                 }
                              }
                              else
                              {
                                 //----------------------------------------new--------------------------------------//
                                 //ByDate
                                 if (pExPeriod == ExchangePeriod.ByDate)
                                 {
                                    if (model.Date_List.Count > 0)
                                    {
                                       foreach (var drow in model.Date_List)
                                       {
                                          exchangeRate.Add(new ExchangeRateViewModel()
                                          {
                                             Exchange_Rate_ID = 0,
                                             Exchange_Currency_ID = row.Exchange_Currency_ID,
                                             Exchange_Date = DateUtil.ToDisplayDate(drow.Date),
                                             Exchange_Month = NumUtil.ParseInteger(drow.Month),
                                             Exchange_Period = ExchangePeriod.ByDate,
                                             Row_Type = RowType.ADD
                                          });

                                          Month = NumUtil.ParseInteger(drow.Month);
                                       }
                                    }
                                 }
                                 //ByMonth
                                 if (row.Exchange_Period == ExchangePeriod.ByMonth)
                                 {
                                    if (model.Month_List.Count > 0)
                                    {
                                       foreach (var drow in model.Month_List)
                                       {
                                          exchangeRate.Add(new ExchangeRateViewModel()
                                          {
                                             Exchange_Rate_ID = 0,
                                             Exchange_Currency_ID = row.Exchange_Currency_ID,
                                             Exchange_Month = NumUtil.ParseInteger(drow.Value),
                                             Exchange_Month_Text = drow.Text,
                                             Exchange_Period = ExchangePeriod.ByMonth,
                                             Row_Type = RowType.ADD
                                          });

                                          Month = NumUtil.ParseInteger(drow.Value);
                                       }
                                    }
                                 }
                                 //----------------------------------------new--------------------------------------//
                              }
                           }
                           else
                           {
                              if (row.Currency_ID == CurrencyID)
                              {
                                 if (pExPeriod == ExchangePeriod.ByDate)
                                 {
                                    if (model.Date_List.Count > 0)
                                    {
                                       foreach (var drow in model.Date_List)
                                       {
                                          exchangeRate.Add(new ExchangeRateViewModel()
                                          {
                                             Exchange_Rate_ID = 0,
                                             Exchange_Currency_ID = row.Exchange_Currency_ID,
                                             Exchange_Date = DateUtil.ToDisplayDate(drow.Date),
                                             Exchange_Month = NumUtil.ParseInteger(drow.Month),
                                             Exchange_Period = ExchangePeriod.ByDate,
                                             Row_Type = RowType.ADD
                                          });

                                          Month = NumUtil.ParseInteger(drow.Month);
                                       }
                                    }
                                 }
                                 else if (pExPeriod == ExchangePeriod.ByMonth)
                                 {
                                    if (model.Month_List.Count > 0)
                                    {
                                       foreach (var drow in model.Month_List)
                                       {
                                          exchangeRate.Add(new ExchangeRateViewModel()
                                          {
                                             Exchange_Rate_ID = 0,
                                             Exchange_Currency_ID = row.Exchange_Currency_ID,
                                             Exchange_Month = NumUtil.ParseInteger(drow.Value),
                                             Exchange_Month_Text = drow.Text,
                                             Exchange_Period = ExchangePeriod.ByMonth,
                                             Row_Type = RowType.ADD
                                          });

                                          Month = NumUtil.ParseInteger(drow.Value);
                                       }
                                    }
                                 }
                              }
                              else
                              {
                                 foreach (var prow in row.ExchangeRate_Rows)
                                 {
                                    var fullMonth = DateUtil.GetFullMonth(prow.Exchange_Month.HasValue ? prow.Exchange_Month.Value : 1);
                                    exchangeRate.Add(new ExchangeRateViewModel()
                                    {
                                       Exchange_Rate_ID = prow.Exchange_Rate_ID,
                                       Exchange_Currency_ID = row.Exchange_Currency_ID,
                                       Rate = prow.Rate,
                                       Exchange_Period = row.Exchange_Period,
                                       Exchange_Date = prow.Exchange_Date,
                                       Exchange_Month = prow.Exchange_Month,
                                       Exchange_Month_Text = fullMonth,
                                       Row_Type = RowType.EDIT
                                    });
                                    Month = prow.Exchange_Month.HasValue ? prow.Exchange_Month.Value : 1;
                                 }
                              }
                           }
                        }
                        else if (row.Row_Type == RowType.ADD)
                        {
                           foreach (var prow in row.ExchangeRate_Rows)
                           {
                              exchangeRate.Add(new ExchangeRateViewModel()
                              {
                                 Exchange_Rate_ID = 0,
                                 Exchange_Currency_ID = row.Exchange_Currency_ID,
                                 Rate = prow.Rate,
                                 Exchange_Period = row.Exchange_Period,
                                 Exchange_Date = prow.Exchange_Date,
                                 Exchange_Month = prow.Exchange_Month,
                                 Exchange_Month_Text = prow.Exchange_Month_Text,
                                 Row_Type = RowType.ADD
                              });

                              Month = prow.Exchange_Month.HasValue ? prow.Exchange_Month.Value : 1;
                           }
                        }

                        ExchangeCurrency.Add(new ExchangeCurrencyViewModel()
                        {
                           Month_ID = Month,

                           Company_Currency_Name = LocalNameCode,
                           Top_Name = TabCode,
                           Exchange_Currency_ID = row.Exchange_Currency_ID,
                           Exchange_ID = row.Exchange_ID,
                           Currency_ID = row.Currency_ID,
                           Exchange_Period = row.Exchange_Period,
                           Row_Type = row.Row_Type,
                           ExchangeRate_Rows = exchangeRate.ToArray()
                        });
                     }
                  }
               }
               model.LstExchangeCurrency = ExchangeCurrency;
            }
         }
         else if (pageAction == "save")
         {

            if (!model.Fiscal_Year.HasValue)
            {
               ModelState.AddModelError("Fiscal_Year", Resource.Message_Is_Required);
            }

            var dupLst = cEService.LstExchange(userlogin.Company_ID);
            var dup = dupLst.Where(w => w.Fiscal_Year == model.Fiscal_Year).FirstOrDefault();
            if (model.Fiscal_Year.HasValue)
            {
               if (dup != null && dup.Fiscal_Year == model.Fiscal_Year)
               {
                  if (model.operation == UserSession.RIGHT_C)
                  {
                     ModelState.AddModelError("Fiscal_Year", Resource.Message_Is_Duplicated);
                  }
                  else if (model.operation == UserSession.RIGHT_U)
                  {
                     if (dup.Exchange_ID != model.Exchange_ID)
                     {
                        ModelState.AddModelError("Fiscal_Year", Resource.Message_Is_Duplicated);
                     }
                  }
               }
            }

            if (model.ExchangeCurrency_Rows == null)
            {
               ModelState.AddModelError("ExchangeCurrency_Rows", Resource.The + " " + Resource.Currency + " " + Resource.Field + " " + Resource.Is_Rrequired_Lower);
            }

            var errors = GetErrorModelState();
            if (ModelState.IsValid)
            {
               Exchange exchange = new Exchange();
               exchange.Company_ID = userlogin.Company_ID;
               exchange.Fiscal_Year = model.Fiscal_Year;
               exchange.Record_Status = RecordStatus.Active;

               if (model.ExchangeCurrency_Rows != null)
               {
                  List<Exchange_Currency> Currency = new List<Exchange_Currency>();
                  if (model.operation == UserSession.RIGHT_C)
                  {
                     exchange.Create_By = userlogin.User_Authentication.Email_Address;
                     exchange.Create_On = currentdate;
                     exchange.Update_By = userlogin.User_Authentication.Email_Address;
                     exchange.Update_On = currentdate;

                     foreach (var row in model.ExchangeCurrency_Rows)
                     {
                        if (row.Row_Type == RowType.ADD | row.Row_Type == RowType.EDIT)
                        {
                           var exchangeCurrency = new Exchange_Currency()
                           {
                              Exchange_Currency_ID = row.Exchange_Currency_ID.HasValue ? row.Exchange_Currency_ID.Value : 0,
                              Currency_ID = row.Currency_ID,
                              Exchange_Period = row.Exchange_Period,
                           };

                           if (row.ExchangeRate_Rows != null)
                           {
                              foreach (var prow in row.ExchangeRate_Rows)
                              {
                                 var exchangeRate = new Exchange_Rate()
                                 {
                                    Exchange_Currency_ID = row.Exchange_Currency_ID,
                                    Rate = prow.Rate,
                                    Exchange_Period = row.Exchange_Period,
                                    Exchange_Date = DateUtil.ToDate(prow.Exchange_Date),
                                    Exchange_Month = prow.Exchange_Month,
                                 };
                                 exchangeCurrency.Exchange_Rate.Add(exchangeRate);
                              }
                           }
                           exchange.Exchange_Currency.Add(exchangeCurrency);
                        }
                     }
                     model.result = exCurService.InsertExchangeCurrency(exchange);


                  }
                  else if (model.operation == UserSession.RIGHT_U)
                  {

                     //Add and edit
                     if (model.Exchange_ID == null)
                        return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

                     exchange.Exchange_ID = model.Exchange_ID.Value;
                     exchange.Update_By = userlogin.User_Authentication.Email_Address;
                     exchange.Update_On = currentdate;

                     foreach (var row in model.ExchangeCurrency_Rows)
                     {
                        if (row.Row_Type == RowType.ADD | row.Row_Type == RowType.EDIT)
                        {
                           var exchangeCurrency = new Exchange_Currency()
                           {
                              Exchange_Currency_ID = row.Exchange_Currency_ID.HasValue ? row.Exchange_Currency_ID.Value : 0,
                              Exchange_ID = model.Exchange_ID.Value,
                              Currency_ID = row.Currency_ID,
                              Exchange_Period = row.Exchange_Period,
                           };

                           if (row.ExchangeRate_Rows != null)
                           {
                              foreach (var prow in row.ExchangeRate_Rows)
                              {
                                 var exchangeRate = new Exchange_Rate()
                                 {
                                    Exchange_Rate_ID = prow.Exchange_Rate_ID.HasValue ? prow.Exchange_Rate_ID.Value : 0,
                                    Exchange_Currency_ID = row.Exchange_Currency_ID,
                                    Rate = prow.Rate,
                                    Exchange_Period = row.Exchange_Period,
                                    Exchange_Date = DateUtil.ToDate(prow.Exchange_Date),
                                    Exchange_Month = prow.Exchange_Month,
                                 };
                                 exchangeCurrency.Exchange_Rate.Add(exchangeRate);
                              }
                           }
                           exchange.Exchange_Currency.Add(exchangeCurrency);
                        }
                     }
                     model.result = exCurService.InsertAndUpdateExchangeCurrency(exchange);
                  }
               }

               if (model.result.Code == ERROR_CODE.SUCCESS)
               {
                  return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "exchange" });
               }
            }
         }

         //ค่าไม่ผ่าน
         if (!ModelState.IsValid)
         {
            if (curr != null)
            {
               model.Company_Currency_Code = curr.Currency_Code;
            }

            var LocalNameCode = curr.Currency_Name + " - " + curr.Currency_Code;
            if (model.ExchangeCurrency_Rows != null && model.ExchangeCurrency_Rows.Count() > 0)
            {
               var ExchangeCurrency = new List<ExchangeCurrencyViewModel>();
               foreach (var row in model.ExchangeCurrency_Rows)
               {
                  if (row.Row_Type != RowType.DELETE)
                  {
                     var TabCode = "";
                     var exchangeRate = new List<ExchangeRateViewModel>();

                     //re get tab
                     if (row.Currency_ID != null)
                     {
                        var ForeignCode = cbService.GetCurrency(row.Currency_ID);
                        if (ForeignCode != null)
                        {
                           TabCode = ForeignCode.Currency_Code + " " + Resource.Message_To_Lower + " " + curr.Currency_Code;
                        }
                     }

                     //re get rows drow
                     if (row.ExchangeRate_Rows.Count() > 0 && row.ExchangeRate_Rows != null)
                     {
                        var monthTemp = 1;
                        if (row.Row_Type == RowType.ADD || row.Row_Type == RowType.EDIT)
                        {
                           foreach (var prow in row.ExchangeRate_Rows)
                           {
                              exchangeRate.Add(new ExchangeRateViewModel()
                              {
                                 Exchange_Rate_ID = prow.Exchange_Rate_ID,
                                 Exchange_Currency_ID = row.Exchange_Currency_ID,
                                 Rate = prow.Rate,
                                 Exchange_Period = prow.Exchange_Period,
                                 Exchange_Date = prow.Exchange_Date,
                                 Exchange_Month = prow.Exchange_Month,
                                 Exchange_Month_Text = prow.Exchange_Month_Text,
                                 Row_Type = RowType.EDIT
                              });


                              monthTemp = prow.Exchange_Month.HasValue ? prow.Exchange_Month.Value : 1;
                           }
                        }

                        ExchangeCurrency.Add(new ExchangeCurrencyViewModel()
                        {
                           Month_ID = monthTemp,

                           Company_Currency_Name = LocalNameCode,
                           Top_Name = TabCode,
                           Exchange_Currency_ID = row.Exchange_Currency_ID,
                           Exchange_ID = row.Exchange_ID,
                           Currency_ID = row.Currency_ID,
                           Exchange_Period = row.Exchange_Period,
                           Row_Type = row.Row_Type,
                           ExchangeRate_Rows = exchangeRate.ToArray()
                        });
                     }
                  }
               }
               model.LstExchangeCurrency = new List<ExchangeCurrencyViewModel>();
               model.LstExchangeCurrency = ExchangeCurrency;
            }
         }

         model.Currency_List = cbService.LstCurrency(hasBlank: false);

         if (curr != null)
         {
            model.Company_Currency_Name = curr.Currency_Name + " - " + curr.Currency_Code;
         }
         model.Month_List = cbService.LstMonth(hasBlank: false);

         var errs = GetErrorModelState();

         return View(model);
      }

   }
}

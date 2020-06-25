using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.IO;
using HR.Models;
using HR.Common;
using SBSModel.Models;
using SBSModel.Common;
using OfficeOpenXml;
using SBSResourceAPI;
using SBSWorkFlowAPI.Constants;
using System.Text;


namespace HR.Controllers
{
   [Authorize]
   [AllowAuthorized]
   public class EmployeeController : ControllerBase
   {
      #region Employee Infomation

      [HttpGet]
      public ActionResult Employee(ServiceResult result, EmployeeViewModels model)
      {
         User_Profile userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var userService = new UserService();
         var page = new List<string>();
         page.Add("/Employee/Employee");
         page.Add("/Employee/EmployeeInfoAdmin");
         page.Add("/Employee/EmployeeInfoHR");

         var right = base.validatePageRight(page);
         if (right == null || right.Count() == 0)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //-------rights------------
         if (right.ContainsKey("/Employee/Employee")) model.rights = right["/Employee/Employee"].ToArray();
         if (right.ContainsKey("/Employee/EmployeeInfoAdmin")) model.adminRights = right["/Employee/EmployeeInfoAdmin"];
         if (right.ContainsKey("/Employee/EmployeeInfoHR")) model.hrRights = right["/Employee/EmployeeInfoHR"];
         model.result = result;

         //-------data------------           
         var cbService = new ComboService();
         var empService = new EmployeeService();

         model.branchList = cbService.LstBranch(userlogin.Company_ID, true);
         model.departmentList = cbService.LstDepartment(userlogin.Company_ID, true);
         model.empTypeList = cbService.LstLookup(ComboType.Employee_Type, userlogin.Company_ID, true);

         model.EmpList = empService.LstEmployeeProfile(userlogin.Company_ID, model.search_Branch, model.search_Department, model.search_empTypeList);

         return View(model);
      }

      [HttpPost]
      public ActionResult Employee(int[] Emp, string apply)
      {
         User_Profile userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         if (Emp != null)
         {
            if (apply == RecordStatus.Active | apply == RecordStatus.Inactive)
            {
               var eService = new EmployeeService();
               var result = eService.UpdateEmpStatus(Emp, apply, userlogin.User_Authentication.Email_Address);

               return RedirectToAction("Employee", new { Code = result.Code, Msg = result.Msg, Field = result.Field });
            }
         }
         return RedirectToAction("Employee");
      }

      [HttpGet]
      public ActionResult EmployeeInfo(ServiceResult result, string pEmpID, string pProfileID, string operation)
      {
         User_Profile userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var model = new EmployeeViewModels();
         model.operation = EncryptUtil.Decrypt(operation);
         if (model.operation == "MATERNITY")
         {// This is come from tutorial link
            model.operation = UserSession.RIGHT_U;
            model.tabAction = "family";
         }
         model.pageAction = "main";

         var page = new List<string>();
         page.Add("/Employee/Employee");
         page.Add("/Employee/EmployeeInfoAdmin");
         page.Add("/Employee/EmployeeInfoHR");

         var right = base.validatePageRight(page);
         if (right == null || right.Count() == 0)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //-------rights------------
         if (right.ContainsKey("/Employee/Employee")) model.rights = right["/Employee/Employee"].ToArray();
         if (right.ContainsKey("/Employee/EmployeeInfoAdmin")) model.adminRights = right["/Employee/EmployeeInfoAdmin"];
         if (right.ContainsKey("/Employee/EmployeeInfoHR")) model.hrRights = right["/Employee/EmployeeInfoHR"];

         var empID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pEmpID));
         var profileID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pProfileID));

         // check is HR admin
         RightResult adminrightResult = base.validatePageRight(model.operation, "/Employee/EmployeeInfoAdmin");

         //-------data------------
         var comService = new CompanyService();
         var empService = new EmployeeService();
         var lService = new LeaveService();
         var patternService = new PatternService();
         var cbService = new ComboService();

         var com = comService.GetCompany(userlogin.Company_ID);
         if (com == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

         var pattern = patternService.GetPattern(userlogin.Company_ID);
         if (pattern == null)
            return errorPage(new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Field = Resource.Pattern });

         model.Company_Level = com.Company_Level;
         model.Company_ID = com.Company_ID;
         model.User_Login_Profile_ID = userlogin.Profile_ID;
         model.Pattern_Nationality_Se = pattern.Select_Nationality;

         if (com.Currency != null)
            model.Company_Currency_Code = com.Currency.Currency_Code;

         model.nationalityList = cbService.LstNationality(false);
         model.empStatusList = cbService.LstEmpStatus();
         model.residentialStatusList = cbService.LstResidentialStatus();
         model.genderList = cbService.LstLookup(ComboType.Gender, userlogin.Company_ID, false);
         model.religionList = cbService.LstLookup(ComboType.Religion, userlogin.Company_ID, false);
         model.raceList = cbService.LstLookup(ComboType.Race, userlogin.Company_ID, false);
         model.wpClassList = cbService.LstLookup(ComboType.WP_Class, userlogin.Company_ID, false);
         model.relationshipList = cbService.LstLookup(ComboType.Relationship, userlogin.Company_ID, true);
         model.maritalStatusList = cbService.LstLookup(ComboType.Marital_Status, userlogin.Company_ID, false);
         model.statusList = cbService.LstStatus();
         model.childTypeList = cbService.LstChildType(true);
         model.paymentTypeList = cbService.LstLookup(ComboType.Payment_Type, userlogin.Company_ID, false);
         model.attachmentTypeList = cbService.LstLookup(ComboType.Attachment_Type, userlogin.Company_ID, true);
         model.workPassTypeList = cbService.LstLookup(ComboType.Work_Pass_Type, userlogin.Company_ID, true);
         model.countryList = cbService.LstCountry(true);
         model.SubscriptionList = comService.LstSubscription(userlogin.Company_ID);
         int[] moduleDetailsID = null;
         if (model.SubscriptionList != null)
            moduleDetailsID = model.SubscriptionList.Select(s => s.Module_Detail_ID.Value).ToArray();

         model.UserRoleList = empService.LstUserRole(moduleDetailsID);

         if (model.operation == Operation.C)
         {
            model.Is_Email = true;
            #region History Info
            model.History_Supervisor = null;
            model.History_No_Approval_WF = false;
            model.History_Contract_Staff = false;
            model.History_Basic_Salary = 0;
            model.History_Hour_Rate = 0;
            model.History_Basic_Salary_Unit = Term.Monthly;
            model.History_Days = 5;
            model.History_Notice_Period_Amount = 0;
            model.History_Hour_Rate = 0;
            #endregion
            model.paymentTypeList = cbService.LstLookup(ComboType.Payment_Type, userlogin.Company_ID, false);
            model.departmentList = cbService.LstDepartment(userlogin.Company_ID, true);
            model.desingnationList = cbService.LstDesignation(userlogin.Company_ID, true);
            model.branchList = cbService.LstBranch(userlogin.Company_ID, true);
            model.periodList = cbService.LstPeriod();
            model.termList = cbService.LstTerm();

            if (model.History_Department_ID.HasValue)
               model.supervisorList = cbService.LstSupervisor(model.History_Department_ID);
            else
            {
               if (model.departmentList.Count > 0)
                  model.supervisorList = cbService.LstSupervisor(NumUtil.ParseInteger(model.departmentList[0].Value));
               else
                  model.supervisorList = new List<ComboViewModel>();
            }
            model.currencyList = cbService.LstCurrency(false);
            model.empTypeList = cbService.LstLookup(ComboType.Employee_Type, userlogin.Company_ID, false);
            model.prtList = cbService.LstPRT(PayrollAllowanceType.Allowance_Deduction, PayrollAllowanceType.Donation);
         }
         else if (model.operation == Operation.U)
         {
            Employee_Profile emp;
            if (empID == 0 & profileID > 0)
               emp = empService.GetEmployeeProfileByProfileID(profileID);
            else
               emp = empService.GetEmployeeProfile(empID);

            if (emp != null)
            {
               #region Employee Profile
               model.Employee_Profile_ID = emp.Employee_Profile_ID;
               model.Employee_No = emp.Employee_No;
               model.Mobile_No = emp.Mobile_No;
               model.Gender = emp.Gender;
               model.Marital_Status = emp.Marital_Status;
               model.DOB = DateUtil.ToDisplayDate(emp.DOB);
               model.Nationality_ID = emp.Nationality_ID;
               if (emp.Nationality_ID == null && model.nationalityList != null && model.nationalityList.Count > 0)
                  model.Nationality_ID = NumUtil.ParseInteger(model.nationalityList[0].Value);

               model.Residential_Status = emp.Residential_Status;
               model.NRIC = emp.NRIC;
               model.Passport = emp.Passport;
               model.PR_No = emp.PR_No;
               model.PR_Start_Date = DateUtil.ToDisplayDate(emp.PR_Start_Date);
               model.PR_End_Date = DateUtil.ToDisplayDate(emp.PR_End_Date);
               model.Remark = emp.Remark;
               model.Race = emp.Race;
               model.Immigration_No = emp.Immigration_No;
               model.WP_Class = emp.WP_Class;
               model.WP_No = emp.WP_No;
               model.WP_Start_Date = DateUtil.ToDisplayDate(emp.WP_Start_Date);
               model.WP_End_Date = DateUtil.ToDisplayDate(emp.WP_End_Date);
               model.Residential_Address_1 = emp.Residential_Address_1;
               model.Residential_Address_2 = emp.Residential_Address_2;
               model.Postal_Code_1 = emp.Postal_Code_1;
               model.Postal_Code_2 = emp.Postal_Code_2;
               model.Residential_Country_1 = emp.Residential_Country_1;
               model.Residential_Country_2 = emp.Residential_Country_2;
               model.Emergency_Name = emp.Emergency_Name;
               model.Emergency_Contact_No = emp.Emergency_Contact_No;
               model.Emergency_Relationship = emp.Emergency_Relationship;
               model.Residential_No = emp.Residential_No;
               model.Emp_Status = emp.Emp_Status;
               model.Religion = emp.Religion;
               model.Confirm_Date = DateUtil.ToDisplayDate(emp.Confirm_Date);
               model.Hired_Date = DateUtil.ToDisplayDate(emp.Hired_Date);
               model.Expiry_Date = DateUtil.ToDisplayDate(emp.Expiry_Date);
               model.Opt_Out = emp.Opt_Out;
               model.NRIC_FIN_Issue_Date = DateUtil.ToDisplayDate(emp.NRIC_FIN_Issue_Date);
               model.NRIC_FIN_Expire_Date = DateUtil.ToDisplayDate(emp.NRIC_FIN_Expire_Date);
               model.Passport_Issue_Date = DateUtil.ToDisplayDate(emp.Passport_Issue_Date);
               model.Passpor_Expire_Date = DateUtil.ToDisplayDate(emp.Passpor_Expire_Date);
               model.Profile_ID = emp.Profile_ID;
               model.User_Authentication_ID = emp.User_Profile.User_Authentication_ID;
               model.Contribute_Rate1 = emp.Contribute_Rate1;
               model.Contribute_Rate2 = emp.Contribute_Rate2;
               model.Work_Pass_Type = emp.Work_Pass_Type;
               #endregion

               model.prmlist = (new PayrollService()).LstPayrollByEmpID(emp.Employee_Profile_ID);
               model.User_Assign_Module = empService.LstUserAssignModule(emp.Profile_ID).Select(s => s.Subscription_ID.Value).ToArray();

               #region History
               var hists = new List<HistoryViewModel>();
               if (emp.Employment_History != null)
               {
                  foreach (var row in emp.Employment_History)
                  {
                     var empType = cbService.GetLookup(row.Employee_Type);
                     var hist = new HistoryViewModel()
                     {
                        History_ID = row.History_ID,
                        Branch_ID = row.Branch_ID,
                        Branch_Name = (row.Branch != null ? (row.Branch.Branch_Code + " : " + row.Branch.Branch_Name) : ""),
                        Department_ID = row.Department_ID,
                        Department_Name = (row.Department != null ? row.Department.Name : ""),
                        Designation_ID = row.Designation_ID,
                        Designation_Name = (row.Designation != null ? row.Designation.Name : ""),
                        //Other_Branch = row.Other_Branch,
                        //Other_Department = row.Other_Department,
                        //Other_Designation = row.Other_Designation,
                        Employee_Type = (row.Employee_Type.HasValue ? row.Employee_Type.Value.ToString() : ""),
                        Employee_Type_Name = (empType != null ? empType.Name : ""),
                        Supervisor = row.Supervisor,
                        No_Approval_WF = row.No_Approval_WF,
                        Effective_Date = DateUtil.ToDisplayDate(row.Effective_Date),
                        Confirm_Date = DateUtil.ToDisplayDate(row.Confirm_Date),
                        Terminate_Date = DateUtil.ToDisplayDate(row.Terminate_Date),
                        Currency_ID = row.Currency_ID,
                        Basic_Salary_Unit = row.Basic_Salary_Unit,
                        Hour_Rate = row.Hour_Rate,
                        Days = (row.Days.HasValue ? row.Days.Value : 5),
                        Notice_Period_Amount = row.Notice_Period_Amount,
                        Notice_Period_Unit = row.Notice_Period_Unit,
                        Contract_Staff = row.Contract_Staff,
                        Contract_Start_Date = DateUtil.ToDisplayDate(row.Contract_Start_Date),
                        Contract_End_Date = DateUtil.ToDisplayDate(row.Contract_End_Date),
                        Row_Type = RowType.EDIT,
                        Payment_Type = row.Payment_Type,
                     };

                     var salary = EncryptUtil.Decrypt(row.Basic_Salary);
                     hist.Basic_Salary = NumUtil.ParseDecimal(EncryptUtil.Decrypt(row.Basic_Salary));
                     if (NumUtil.ParseDecimal(salary) == 0)
                        hist.Basic_Salary = NumUtil.ParseDecimal(EncryptUtil.Decrypt(salary));

                     if (string.IsNullOrEmpty(hist.Basic_Salary_Unit))
                        hist.Basic_Salary_Unit = Term.Monthly;

                     var allowances = new List<HistoryAllowanceViewModel>();
                     foreach (var arow in row.Employment_History_Allowance)
                     {
                        if (!arow.PRC_ID.HasValue) arow.PRC_ID = 0;
                        var allowance = new HistoryAllowanceViewModel()
                        {
                           Employment_History_Allowance_ID = arow.Employment_History_Allowance_ID,
                           PRC_ID = arow.PRC_ID,
                           PRT_ID = arow.PRT_ID,
                           Amount = arow.Amount,
                           Row_Type = RowType.EDIT
                        };
                        allowances.Add(allowance);
                     }
                     hist.History_Allowance_Rows = allowances.ToArray();
                     hists.Add(hist);
                  }
               }
               model.History_Rows = hists.ToArray();
               #endregion

               #region Employee Emergency Contact
               var eContacts = new List<EmployeeEmergencyContactViewModel>();
               foreach (var row in emp.Employee_Emergency_Contact)
               {
                  eContacts.Add(new EmployeeEmergencyContactViewModel
                  {
                     Employee_Emergency_Contact_ID = row.Employee_Emergency_Contact_ID,
                     Name = row.Name,
                     Contact_No = row.Contact_No,
                     Relationship = row.Relationship,
                     Row_Type = RowType.EDIT
                  });
               }
               model.Emer_Contact_Rows = eContacts.ToArray();
               #endregion

               #region Bank Info
               var banks = new List<BankInfoViewModel>();
               foreach (var row in emp.Banking_Info)
               {
                  banks.Add(new BankInfoViewModel()
                  {
                     Banking_Info_ID = row.Banking_Info_ID,
                     Bank_Name = row.Bank_Name,
                     Bank_Account = row.Bank_Account,
                     Payment_Type = row.Payment_Type,
                     Effective_Date = DateUtil.ToDisplayDate(row.Effective_Date),
                     Selected = row.Selected,
                     Row_Type = RowType.EDIT
                  });
               }
               model.Bank_Info_Rows = banks.ToArray();
               #endregion

               #region RelationShip
               var relations = new List<RelationshipViewModels>();
               foreach (var row in emp.Relationships)
               {
                  var Leaved = false;
                  var cri = new LeaveCalIsExistCriteria() { Relationship_ID = row.Relationship_ID };
                  var calisexsist = lService.LeaveCalIsExist(cri);
                  var leaveexsist = lService.GetLeaveByRelationship(row.Relationship_ID);
                  if ((leaveexsist != null && leaveexsist.Count > 0) || calisexsist)
                     Leaved = true;

                  relations.Add(new RelationshipViewModels()
                      {
                         Relationship_ID = row.Relationship_ID,
                         Company_Name = row.Company_Name,
                         Company_Position = row.Company_Position,
                         DOB = DateUtil.ToDisplayDate(row.DOB),
                         Gender = row.Gender,
                         Name = row.Name,
                         Nationality_ID = row.Nationality_ID,
                         Nationality_Name = (row.Nationality != null ? row.Nationality.Description : ""),
                         NRIC = row.NRIC,
                         Passport = row.Passport,
                         Relationship = row.Relationship1,
                         Relationship_Name = (row.Global_Lookup_Data1 != null ? row.Global_Lookup_Data1.Name : ""),
                         Working = (row.Working.HasValue ? row.Working.Value : false),
                         Row_Type = RowType.EDIT,
                         Child_Type = row.Child_Type,
                         Is_Maternity_Share_Father = (row.Is_Maternity_Share_Father.HasValue ? row.Is_Maternity_Share_Father.Value : false),
                         Leaved = Leaved,
                      });
               }

               model.Relationship_Rows = relations.ToArray();
               #endregion

               #region Attachment
               var attachs = new List<AttachmentViewModel>();
               foreach (var row in emp.Employee_Attachment)
               {
                  attachs.Add(new AttachmentViewModel()
                  {
                     Attachment_ID = row.Attachment_ID,
                     Attachment_Type = row.Attachment_Type,
                     Attachment_File = row.Attachment_File,
                     Attachment_Type_Name = row.Global_Lookup_Data != null ? row.Global_Lookup_Data.Name : "",
                     File_Name = row.File_Name,
                     Uploaded_By = row.Uploaded_by,
                     Uploaded_On = DateUtil.ToDisplayDate(row.Uploaded_On),
                     Row_Type = RowType.EDIT,
                  });
               }
               model.Attachment_Rows = attachs.ToArray();
               #endregion

               #region User Profile
               if (emp.User_Profile != null)
               {
                  model.Company_ID = emp.User_Profile.Company_ID;
                  model.Profile_ID = emp.User_Profile.Profile_ID;
                  model.First_Name = emp.User_Profile.First_Name;
                  model.Middle_Name = emp.User_Profile.Middle_Name;
                  model.Last_Name = emp.User_Profile.Last_Name;
                  model.Mobile_No = emp.User_Profile.Phone;
                  model.User_Status = emp.User_Profile.User_Status;
                  model.Is_Email = emp.User_Profile.User_Authentication.Is_Email.HasValue ? emp.User_Profile.User_Authentication.Is_Email.Value : true;
                  model.Email = emp.User_Profile.User_Authentication.Email_Address;
                  model.User_Name = emp.User_Profile.User_Authentication.User_Name;
                  model.Activated = emp.User_Profile.User_Authentication.Activated;

                  if (emp.User_Profile.User_Profile_Photo != null && emp.User_Profile.User_Profile_Photo.FirstOrDefault() != null)
                  {
                     var photo = emp.User_Profile.User_Profile_Photo.FirstOrDefault();
                     model.User_Profile_Photo_ID = photo.User_Profile_Photo_ID;
                     model.User_Photo = photo.Photo;
                  }
                  model.Users_Assign_Role = empService.LstUserAssignRole(emp.User_Profile.User_Authentication_ID).Select(s => s.User_Role_ID.Value).ToArray();
               }
               #endregion
            }
         }
         else if (model.operation == Operation.D)
         {
            var apply = RecordStatus.Delete;
            model.result = empService.UpdateDeleteEmployeeStatus(empID, apply, userlogin.User_Authentication.Email_Address);
            return RedirectToAction("Employee", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field });
         }

         return View(model);
      }

      [HttpPost]
      [AllowAuthorized]
      public ActionResult EmployeeInfo(EmployeeViewModels model, int[] User_Assign_Module, int[] Users_Assign_Role, string pageAction = "")
      {
         User_Profile userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var empService = new EmployeeService();
         var uService = new UserService();
         var cbService = new ComboService();
         var comService = new CompanyService();
         var currentdate = StoredProcedure.GetCurrentDate();

         model.Users_Assign_Role = Users_Assign_Role;
         model.pageAction = pageAction;
         model.User_Assign_Module = User_Assign_Module;

         if (pageAction == "save")
         {
            model.pageAction = "main";

            #region Verify
            if (!ModelState.IsValid)
            {
               foreach (var key in ModelState.Keys.ToList().Where(key => ModelState.ContainsKey(key) && ((key.Contains("History_") & !key.Contains("History_Rows")) | (key.Contains("History_Allowance_Rows")) | (key.Contains("Relationship_") & !key.Contains("Relationship_Rows")))))
                  ModelState[key].Errors.Clear(); //This is my new solution. Thanks bbak
            }

            if (model.Is_Email)
            {
               if (string.IsNullOrEmpty(model.Email))
                  ModelState.AddModelError("Email", Resource.Message_Is_Required);
               else
               {
                  var criteria = new UserCriteria() { Email = model.Email };
                  var dupUser = uService.LstUserProfile(criteria).FirstOrDefault();
                  if (dupUser != null)
                  {
                     if (model.operation == Operation.C)
                        ModelState.AddModelError("Email", Resource.Message_Is_Duplicated_Email);
                     else if (model.operation == Operation.U)
                     {
                        if (dupUser.Profile_ID != model.Profile_ID)
                           ModelState.AddModelError("Email", Resource.Message_Is_Duplicated_Email);
                     }
                  }
               }
            }
            else
            {
               if (string.IsNullOrEmpty(model.User_Name))
                  ModelState.AddModelError("User_Name", Resource.Message_Is_Required);
               else if (!string.IsNullOrEmpty(model.User_Name) && (model.User_Name.ToString().Length < 5 || model.User_Name.ToString().Length > 10))
                  ModelState.AddModelError("User_Name", Resource.Message_Verify_UserName);
               else
               {
                  var isvalid = StrUtil.IsValidUserName(model.User_Name);
                  if (isvalid == true)
                  {
                     var criteria = new UserCriteria() { User_Name = model.User_Name };
                     var dupUser = uService.LstUserProfile(criteria).FirstOrDefault();
                     if (dupUser != null)
                     {
                        if (model.operation == Operation.C)
                           ModelState.AddModelError("User_Name", Resource.Message_Is_Duplicated_User_Name);
                        else if (model.operation == Operation.U)
                        {
                           if (dupUser.Profile_ID != model.Profile_ID)
                              ModelState.AddModelError("User_Name", Resource.Message_Is_Duplicated_User_Name);
                        }
                     }
                  }
                  else
                  {
                     ModelState.AddModelError("User_Name", Resource.Message_User_Name_Is_Invalid);
                  }
               }
            }

            if (!ModelState.IsValid & model.History_Rows != null)
            {
               var i = 0;
               foreach (var row in model.History_Rows)
               {
                  if (row.Row_Type == RowType.DELETE)
                  {
                     DeleteModelStateError("History_Rows[" + i + "]");
                  }
                  i++;
               }
            }

            if (!ModelState.IsValid & model.Emer_Contact_Rows != null)
            {
               var i = 0;
               foreach (var row in model.Emer_Contact_Rows)
               {
                  if (row.Row_Type == RowType.DELETE)
                  {
                     DeleteModelStateError("Emer_Contact_Rows[" + i + "]");
                  }
                  i++;
               }
            }

            if (!ModelState.IsValid & model.Bank_Info_Rows != null)
            {
               var i = 0;
               foreach (var row in model.Bank_Info_Rows)
               {
                  if (row.Row_Type == RowType.DELETE)
                  {
                     DeleteModelStateError("Bank_Info_Rows[" + i + "]");
                  }
                  i++;
               }
            }

            if (!ModelState.IsValid & model.Attachment_Rows != null)
            {
               var i = 0;
               foreach (var row in model.Attachment_Rows)
               {
                  if (row.Row_Type == RowType.DELETE)
                  {
                     DeleteModelStateError("Attachment_Rows[" + i + "]");
                  }
                  i++;
               }
            }

            if (!ModelState.IsValid & model.Relationship_Rows != null)
            {
               var i = 0;
               foreach (var row in model.Relationship_Rows)
               {
                  if (row.Row_Type == RowType.DELETE)
                  {
                     DeleteModelStateError("Relationship_Rows[" + i + "]");
                  }
                  i++;
               }
            }

            if (!string.IsNullOrEmpty(model.DOB))
            {
               var dob = DateUtil.ToDate(model.DOB);
               if (dob > currentdate)
                  ModelState.AddModelError("DOB", Resource.The + " " + Resource.DOB + " " + Resource.Field + " " + Resource.Cannot_Be_Set_To_Future_Lower);
               else
               {
                  DateTime currentdate_15 = DateTime.Today.AddYears(-16);
                  if (dob > currentdate_15)
                     ModelState.AddModelError("DOB", Resource.Message_Is_Invalid_DOB);
               }
            }

            if (model.Users_Assign_Role == null)
               ModelState.AddModelError("Users_Assign_Role", Resource.Message_Is_Required);

            if (model.operation == Operation.C)
            {
               #region History Info (Save)
               if (!ModelState.IsValid && model.History_Allowance_Rows != null)
               {
                  var i = 0;
                  foreach (var row in model.History_Allowance_Rows)
                  {
                     if (row.Row_Type == RowType.DELETE)
                     {
                        DeleteModelStateError("History_Allowance_Rows[" + i + "]");
                     }
                     i++;
                  }
               }

               var emptype = cbService.GetLookup(NumUtil.ParseInteger(model.History_Employee_Type));
               if (emptype != null)
               {
                  if (emptype.Name == "Contract")
                  {
                     if (string.IsNullOrEmpty(model.History_Contract_Start_Date))
                        ModelState.AddModelError("History_Contract_Start_Date", Resource.Message_Is_Required);

                     if (string.IsNullOrEmpty(model.History_Contract_End_Date))
                        ModelState.AddModelError("History_Contract_End_Date", Resource.Message_Is_Required);

                     model.History_Effective_Date = model.History_Contract_Start_Date;
                  }
                  else
                  {
                     model.History_Contract_Start_Date = null;
                     model.History_Contract_End_Date = null;

                     if (string.IsNullOrEmpty(model.History_Effective_Date))
                        ModelState.AddModelError("History_Effective_Date", Resource.Message_Is_Required);
                  }
               }

               if (!model.History_Branch_ID.HasValue && string.IsNullOrEmpty(model.History_Other_Branch))
               {
                  ModelState.AddModelError("History_Branch_ID", Resource.Message_Is_Required);
                  ModelState.AddModelError("History_Other_Branch", Resource.Message_Is_Required);
               }

               if (!model.History_Department_ID.HasValue && string.IsNullOrEmpty(model.History_Other_Department))
               {
                  ModelState.AddModelError("History_Department_ID", Resource.Message_Is_Required);
                  ModelState.AddModelError("History_Other_Department", Resource.Message_Is_Required);
               }

               if (!model.History_Designation_ID.HasValue && string.IsNullOrEmpty(model.History_Other_Designation))
               {
                  ModelState.AddModelError("History_Designation_ID", Resource.Message_Is_Required);
                  ModelState.AddModelError("History_Other_Designation", Resource.Message_Is_Required);
               }

               if (!string.IsNullOrEmpty(model.History_Terminate_Date))
               {
                  if (!model.History_Notice_Period_Amount.HasValue || model.History_Notice_Period_Amount.Value == 0)
                     ModelState.AddModelError("History_Notice_Period_Amount", Resource.Message_Is_Required);

                  if (string.IsNullOrEmpty(model.History_Notice_Period_Unit))
                     ModelState.AddModelError("History_Notice_Period_Unit", Resource.Message_Is_Required);
               }
               else
               {
                  model.History_Notice_Period_Amount = 0;
                  model.History_Notice_Period_Unit = "";
               }

               if (model.History_Basic_Salary.HasValue && model.History_Basic_Salary == 0)
                  ModelState.AddModelError("History_Basic_Salary", Resource.Message_Is_Required);

               if (string.IsNullOrEmpty(model.History_Basic_Salary_Unit))
                  ModelState.AddModelError("History_Basic_Salary_Unit", Resource.Message_Is_Required);

               if (!model.History_Days.HasValue)
                  ModelState.AddModelError("History_Days", Resource.Message_Is_Required);

               #endregion
            }

            #endregion


            var err = GetErrorModelState();
            if (ModelState.IsValid)
            {
               #region User Profile
               var p = new User_Profile();
               if (model.operation == Operation.U)
                  p = uService.getUser(model.Profile_ID);

               p.First_Name = model.First_Name;
               p.Middle_Name = model.Middle_Name;
               p.Last_Name = model.Last_Name;

               p.Is_Email = model.Is_Email;
               if (model.Is_Email)
                  p.Email = model.Email;
               else
                  p.User_Name = model.User_Name;

               p.Phone = model.Mobile_No;
               p.User_Status = model.User_Status;
               p.Company_ID = userlogin.Company_ID;
               p.Update_On = currentdate;
               p.Update_By = userlogin.User_Authentication.Email_Address;
               #endregion

               #region User Transactions
               List<User_Transactions> uts = new List<User_Transactions>();
               if (model.operation == Operation.U && model.User_Status == Resource.Inactive)
               {
                  var ut = new User_Transactions();
                  ut = uService.getUserTransaction(model.Profile_ID);
                  if (ut != null)
                  {
                     ut.Deactivate_On = currentdate;
                     ut.Deactivate_By = userlogin.User_Authentication.Email_Address;
                     uts.Add(ut);
                  }
               }
               else if (model.operation == Operation.C || model.operation == Operation.U)
               {
                  var ut = new User_Transactions();
                  ut.Company_ID = model.Company_ID.Value;
                  ut.Activate_On = currentdate;
                  ut.Activate_By = userlogin.User_Authentication.Email_Address;
                  uts.Add(ut);
               }
               p.User_Transactions = uts;
               #endregion

               #region Employee Profile
               var emp = new Employee_Profile();
               if (model.operation == Operation.U)
                  emp = empService.GetEmployeeProfile(model.Employee_Profile_ID);

               emp.Mobile_No = model.Mobile_No;
               emp.Gender = model.Gender;
               emp.Marital_Status = model.Marital_Status;
               emp.DOB = DateUtil.ToDate(model.DOB);
               emp.Nationality_ID = model.Nationality_ID;
               emp.Residential_Status = model.Residential_Status;
               emp.NRIC = model.NRIC;
               emp.Passport = model.Passport;
               emp.PR_No = model.PR_No;
               emp.PR_Start_Date = DateUtil.ToDate(model.PR_Start_Date);
               emp.PR_End_Date = DateUtil.ToDate(model.PR_End_Date);
               emp.Remark = model.Remark;
               emp.Race = model.Race;
               emp.Immigration_No = model.Immigration_No;
               emp.WP_Class = model.WP_Class;
               emp.WP_No = model.WP_No;
               emp.WP_Start_Date = DateUtil.ToDate(model.WP_Start_Date);
               emp.WP_End_Date = DateUtil.ToDate(model.WP_End_Date);
               emp.Residential_Address_1 = model.Residential_Address_1;
               emp.Residential_Address_2 = model.Residential_Address_2;
               emp.Postal_Code_1 = model.Postal_Code_1;
               emp.Postal_Code_2 = model.Postal_Code_2;
               emp.Residential_Country_1 = model.Residential_Country_1;
               emp.Residential_Country_2 = model.Residential_Country_2;
               emp.Emergency_Name = model.Emergency_Name;
               emp.Emergency_Contact_No = model.Emergency_Contact_No;
               emp.Emergency_Relationship = model.Emergency_Relationship;
               emp.Profile_ID = model.Profile_ID;
               emp.Residential_No = model.Residential_No;
               emp.Religion = model.Religion;
               emp.Confirm_Date = DateUtil.ToDate(model.Confirm_Date);
               emp.Hired_Date = DateUtil.ToDate(model.Hired_Date);
               emp.Expiry_Date = DateUtil.ToDate(model.Expiry_Date);
               emp.Opt_Out = model.Opt_Out;
               emp.NRIC_FIN_Issue_Date = DateUtil.ToDate(model.NRIC_FIN_Issue_Date);
               emp.NRIC_FIN_Expire_Date = DateUtil.ToDate(model.NRIC_FIN_Expire_Date);
               emp.Passport_Issue_Date = DateUtil.ToDate(model.Passport_Issue_Date);
               emp.Passpor_Expire_Date = DateUtil.ToDate(model.Passpor_Expire_Date);
               emp.Contribute_Rate1 = model.Contribute_Rate1;
               emp.Contribute_Rate2 = !model.Contribute_Rate1;
               emp.Work_Pass_Type = model.Work_Pass_Type;
               emp.Update_On = currentdate;
               emp.Update_By = userlogin.User_Authentication.Email_Address;
               emp.Employee_No = model.Employee_No;
               #endregion

               #region Employee Emergency Contact
               emp.Employee_Emergency_Contact.Clear();
               if (model.Emer_Contact_Rows != null)
               {
                  foreach (var row in model.Emer_Contact_Rows)
                  {
                     if (row.Row_Type == RowType.ADD | row.Row_Type == RowType.EDIT)
                     {
                        var emer = new Employee_Emergency_Contact();
                        emer.Employee_Emergency_Contact_ID = row.Employee_Emergency_Contact_ID;
                        emer.Name = row.Name;
                        emer.Contact_No = row.Contact_No;
                        emer.Relationship = (row.Relationship.HasValue ? (row.Relationship.Value > 0 ? row.Relationship : null) : null);
                        emer.Employee_Profile_ID = emp.Employee_Profile_ID;
                        emer.Update_By = userlogin.User_Authentication.Email_Address;
                        emer.Update_On = currentdate;
                        if (row.Row_Type == RowType.ADD)
                        {
                           emer.Create_By = userlogin.User_Authentication.Email_Address;
                           emer.Create_On = currentdate;
                        }
                        emp.Employee_Emergency_Contact.Add(emer);
                     }
                  }
               }
               #endregion

               #region Relationships
               emp.Relationships.Clear();
               if (model.Relationship_Rows != null)
               {
                  var gender = cbService.GetLookup(emp.Gender);
                  var mstatus = cbService.GetLookup(emp.Marital_Status);
                  foreach (var row in model.Relationship_Rows)
                  {
                     if (row.Row_Type == RowType.ADD | row.Row_Type == RowType.EDIT)
                     {
                        var relationship = new Relationship();
                        relationship.Relationship_ID = (row.Relationship_ID.HasValue ? row.Relationship_ID.Value : 0);
                        relationship.Company_Name = row.Company_Name;
                        relationship.Company_Position = row.Company_Position;
                        relationship.DOB = DateUtil.ToDate(row.DOB);
                        relationship.Gender = row.Gender;
                        relationship.Name = row.Name;
                        relationship.Nationality_ID = row.Nationality_ID;
                        relationship.NRIC = row.NRIC;
                        relationship.Passport = row.Passport;
                        relationship.Relationship1 = row.Relationship;
                        relationship.Working = row.Working;
                        relationship.Employee_Profile_ID = emp.Employee_Profile_ID;
                        relationship.Child_Type = row.Child_Type;
                        relationship.Is_Maternity_Share_Father = row.Is_Maternity_Share_Father;
                        relationship.Update_By = userlogin.User_Authentication.Email_Address;
                        relationship.Update_On = currentdate;
                        if (row.Row_Type == RowType.ADD)
                        {
                           relationship.Create_By = userlogin.User_Authentication.Email_Address;
                           relationship.Create_On = currentdate;
                        }
                        emp.Relationships.Add(relationship);
                     }
                  }
               }
               #endregion

               #region Banking Info
               emp.Banking_Info.Clear();
               if (model.Bank_Info_Rows != null)
               {
                  var i = 0;
                  foreach (var row in model.Bank_Info_Rows)
                  {
                     if (row.Row_Type == RowType.ADD | row.Row_Type == RowType.EDIT)
                     {
                        var bank = new Banking_Info();
                        bank.Banking_Info_ID = (row.Banking_Info_ID.HasValue ? row.Banking_Info_ID.Value : 0);
                        bank.Bank_Name = row.Bank_Name;
                        bank.Bank_Account = row.Bank_Account;
                        bank.Payment_Type = row.Payment_Type;
                        bank.Effective_Date = DateUtil.ToDate(row.Effective_Date);
                        bank.Selected = false;
                        bank.Employee_Profile_ID = emp.Employee_Profile_ID;
                        bank.Update_By = userlogin.User_Authentication.Email_Address;
                        bank.Update_On = currentdate;

                        if (model.Bank_Info_Selected.HasValue)
                           if (model.Bank_Info_Selected.Value == i)
                              bank.Selected = true;

                        if (row.Row_Type == RowType.ADD)
                        {
                           bank.Create_By = userlogin.User_Authentication.Email_Address;
                           bank.Create_On = currentdate;
                        }
                        emp.Banking_Info.Add(bank);
                     }
                     i++;
                  }
               }
               #endregion

               #region Employee Attachment
               emp.Employee_Attachment.Clear();
               if (model.Attachment_Rows != null)
               {
                  var i = 0;
                  foreach (var row in model.Attachment_Rows)
                  {
                     if (row.Row_Type == RowType.ADD | row.Row_Type == RowType.EDIT)
                     {
                        var attach = new Employee_Attachment();
                        attach.Attachment_ID = row.Attachment_ID.HasValue ? row.Attachment_ID.Value : Guid.NewGuid();
                        attach.Attachment_Type = row.Attachment_Type;
                        attach.Attachment_File = row.Attachment_File;
                        attach.File_Name = row.File_Name;
                        attach.Uploaded_by = userlogin.User_Authentication.Email_Address;
                        attach.Uploaded_On = currentdate;
                        if (row.Row_Type == RowType.ADD)
                        {
                           attach.Attachment_ID = Guid.NewGuid();
                           attach.Create_By = userlogin.User_Authentication.Email_Address;
                           attach.Create_On = currentdate;
                        }
                        emp.Employee_Attachment.Add(attach);
                     }
                     i++;
                  }
               }
               #endregion

               #region Employment History
               emp.Employment_History.Clear();
               if (model.History_Rows == null)
               {
                  var hist = new Employment_History();
                  hist.Branch_ID = model.History_Branch_ID;
                  hist.Department_ID = model.History_Department_ID;
                  hist.Designation_ID = model.History_Designation_ID;
                  hist.Other_Branch = model.History_Other_Branch;
                  hist.Other_Department = model.History_Other_Department;
                  hist.Other_Designation = model.History_Other_Designation;
                  hist.Employee_Type = NumUtil.ParseInteger(model.History_Employee_Type);
                  hist.Supervisor = (model.History_Supervisor > 0 ? model.History_Supervisor : null);
                  hist.No_Approval_WF = model.History_No_Approval_WF;
                  hist.Effective_Date = DateUtil.ToDate(model.History_Effective_Date);
                  hist.Confirm_Date = DateUtil.ToDate(model.History_Confirm_Date);
                  hist.Terminate_Date = DateUtil.ToDate(model.History_Terminate_Date);
                  hist.Currency_ID = model.History_Currency_ID;
                  hist.Basic_Salary = EncryptUtil.Encrypt(model.History_Basic_Salary);
                  hist.Hour_Rate = model.History_Hour_Rate;
                  hist.Basic_Salary_Unit = model.History_Basic_Salary_Unit;
                  hist.Days = model.History_Days;
                  hist.Employee_Profile_ID = emp.Employee_Profile_ID;
                  hist.Notice_Period_Amount = model.History_Notice_Period_Amount;
                  hist.Notice_Period_Unit = model.History_Notice_Period_Unit;
                  hist.Contract_Staff = model.History_Contract_Staff;
                  hist.Contract_Start_Date = DateUtil.ToDate(model.History_Contract_Start_Date);
                  hist.Contract_End_Date = DateUtil.ToDate(model.History_Contract_End_Date);
                  hist.Create_By = userlogin.User_Authentication.Email_Address;
                  hist.Create_On = currentdate;
                  hist.Update_On = currentdate;
                  hist.Update_By = userlogin.User_Authentication.Email_Address;
                  emp.Employment_History.Add(hist);
               }
               else
               {
                  foreach (var row in model.History_Rows)
                  {
                     if (row.Row_Type == RowType.ADD | row.Row_Type == RowType.EDIT)
                     {
                        var hist = new Employment_History();
                        hist.History_ID = row.History_ID;
                        hist.Branch_ID = row.Branch_ID;
                        hist.Department_ID = row.Department_ID;
                        hist.Designation_ID = row.Designation_ID;
                        hist.Other_Branch = row.Other_Branch;
                        hist.Other_Department = row.Other_Department;
                        hist.Other_Designation = row.Other_Designation;
                        hist.Employee_Type = NumUtil.ParseInteger(row.Employee_Type);
                        hist.Supervisor = (row.Supervisor > 0 ? row.Supervisor : null);
                        hist.No_Approval_WF = row.No_Approval_WF;
                        hist.Effective_Date = DateUtil.ToDate(row.Effective_Date);
                        hist.Confirm_Date = DateUtil.ToDate(row.Confirm_Date);
                        hist.Terminate_Date = DateUtil.ToDate(row.Terminate_Date);
                        hist.Currency_ID = row.Currency_ID;
                        hist.Basic_Salary = EncryptUtil.Encrypt(row.Basic_Salary);
                        hist.Basic_Salary_Unit = row.Basic_Salary_Unit;
                        hist.Days = row.Days;
                        hist.Employee_Profile_ID = emp.Employee_Profile_ID;
                        hist.Notice_Period_Amount = row.Notice_Period_Amount;
                        hist.Notice_Period_Unit = row.Notice_Period_Unit;
                        hist.Contract_Staff = row.Contract_Staff;
                        hist.Contract_Start_Date = DateUtil.ToDate(row.Contract_Start_Date);
                        hist.Contract_End_Date = DateUtil.ToDate(row.Contract_End_Date);
                        hist.Update_On = currentdate;
                        hist.Update_By = userlogin.User_Authentication.Email_Address;
                        if (row.Row_Type == RowType.ADD)
                        {
                           hist.Create_By = userlogin.User_Authentication.Email_Address;
                           hist.Create_On = currentdate;
                        }
                        hist.Payment_Type = row.Payment_Type;
                        if (row.History_Allowance_Rows != null)
                        {
                           foreach (var arow in row.History_Allowance_Rows)
                           {
                              if (arow.Row_Type == RowType.ADD | row.Row_Type == RowType.EDIT)
                              {
                                 var allowance = new Employment_History_Allowance();
                                 allowance.Employment_History_Allowance_ID = (arow.Employment_History_Allowance_ID.HasValue ? arow.Employment_History_Allowance_ID.Value : 0);
                                 allowance.Amount = arow.Amount;
                                 allowance.PRC_ID = arow.PRC_ID;
                                 allowance.PRT_ID = arow.PRT_ID;
                                 allowance.History_ID = row.History_ID;
                                 allowance.Update_On = currentdate;
                                 allowance.Update_By = userlogin.User_Authentication.Email_Address;

                                 if (row.History_ID == 0)
                                    allowance.History_ID = null;

                                 if (arow.PRC_ID == 0)
                                    allowance.PRC_ID = null;

                                 hist.Employment_History_Allowance.Add(allowance);
                              }
                           }
                        }
                        emp.Employment_History.Add(hist);
                     }
                  }
               }
               #endregion

               if (model.operation == Operation.C)
               {
                  #region Create
                  p.Create_On = currentdate;
                  p.Create_By = userlogin.User_Authentication.Email_Address;
                  emp.Create_On = currentdate;
                  emp.Create_By = userlogin.User_Authentication.Email_Address;

                  #region User Profile Photo
                  if (Request.Files.Count > 0)
                  {
                     HttpPostedFileBase file = Request.Files[0];
                     int fileSizeInBytes = file.ContentLength;
                     MemoryStream target = new MemoryStream();
                     file.InputStream.CopyTo(target);
                     byte[] data = target.ToArray();
                     if (data.Length > 0)
                     {
                        System.Drawing.Image image = byteArrayToImage(data);
                        int thumbnailSize = 150;
                        int newWidth = 0;
                        int newHeight = 0;
                        if (image.Width > image.Height)
                        {
                           newWidth = thumbnailSize;
                           newHeight = (int)(image.Height * thumbnailSize / image.Width);
                        }
                        else
                        {
                           newWidth = (int)(image.Width * thumbnailSize / image.Height);
                           newHeight = thumbnailSize;
                        }

                        var thumbnailBitmap = new System.Drawing.Bitmap(newWidth, newHeight);
                        var thumbnailGraph = System.Drawing.Graphics.FromImage(thumbnailBitmap);
                        thumbnailGraph.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        thumbnailGraph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        thumbnailGraph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                        var imageRectangle = new System.Drawing.Rectangle(0, 0, newWidth, newHeight);
                        thumbnailGraph.DrawImage(image, imageRectangle);

                        //thumbnailBitmap.Save(outputPath & ".jpg", image.RawFormat);
                        System.IO.MemoryStream ms = new System.IO.MemoryStream();
                        thumbnailBitmap.Save(ms, image.RawFormat);
                        data = ms.ToArray();
                        model.User_Photo = data;
                     }
                  }
                  if (model.User_Photo != null)
                  {
                     System.Guid id = Guid.NewGuid();
                     var photo = new User_Profile_Photo()
                     {
                        Profile_ID = model.Profile_ID,
                        User_Profile_Photo_ID = id,
                        Photo = model.User_Photo,
                        Create_On = currentdate,
                        Create_By = userlogin.User_Authentication.Email_Address,
                        Update_On = currentdate,
                        Update_By = userlogin.User_Authentication.Email_Address,
                     };
                     p.User_Profile_Photo.Add(photo);
                  }
                  #endregion

                  model.result = empService.InsertEmployee(p, model.Users_Assign_Role, model.User_Assign_Module, emp);
                  //Added by Moet on 2/Sep
                  if (model.result.Code == ERROR_CODE.SUCCESS)
                  {
                     var pService = new PayrollService();
                     var pRGs = pService.GetPRGs(model.Company_ID);
                     if (pRGs.Count() > 0)
                     {
                        var prgID = pRGs[0].PRG_ID;
                        var prel = pService.getPREL(emp.Employee_Profile_ID);
                        if (prel == null)
                        {
                           prel = new PREL();
                           prel.Employee_Profile_ID = emp.Employee_Profile_ID;
                           prel.PRG_ID = prgID;
                           prel.Create_By = userlogin.Email;
                           prel.Create_On = StoredProcedure.GetCurrentDate();
                           var result = pService.InsertPREL(prel);
                        }
                        if (model.Users_Assign_Role.Count() > 0)
                        {
                           for (var i = 0; i < model.Users_Assign_Role.Count(); i++)
                           {
                              if (model.Users_Assign_Role[i] == 6 || model.Users_Assign_Role[i] == 9) //HR Admin or HR Officer
                              {
                                 var pral = new PRAL();
                                 pral.Employee_Profile_ID = emp.Employee_Profile_ID;
                                 pral.PRG_ID = prgID;
                                 pral.Create_By = userlogin.Email;
                                 pral.Create_On = StoredProcedure.GetCurrentDate();
                                 var result = pService.InsertPRAL(pral);
                              }
                           }
                        }
                     }
                  }

                  if (model.result.Code == ERROR_CODE.SUCCESS)
                  {
                     if (model.result.Object != null)
                     {
                        var moduleName = string.Empty;
                        if (AppSetting.IsLive == "true")
                           moduleName = ModuleDomain.HR;

                        var domain = UrlUtil.GetDomain(Request.Url, moduleName);
                        string activateCode = model.result.Object.ToString();
                        var com = comService.GetCompany(userlogin.Company_ID);
                        if (model.Is_Email == true)
                        {
                           var eResult = EmailTemplete.sendUserActivateEmail(model.Email, activateCode, UserSession.GetUserName(p), com.Name, com.Phone, com.Email, domain);
                           if (eResult == false)
                              model.result.Code = ERROR_CODE.ERROR_501_CANT_SEND_EMAIL;
                        }
                     }
                     EmployeeProcessApproval(p.Profile_ID);
                     return RedirectToAction("Employee", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field });
                  }
                  #endregion
               }
               else if (model.operation == Operation.U)
               {
                  #region Update
                  model.result = empService.UpdateEmployee(p, model.Users_Assign_Role, model.User_Assign_Module, emp);
                  if (model.result.Code == ERROR_CODE.SUCCESS)
                  {
                     // update emp new department to approval work flow.
                     EmployeeProcessApproval(model.Profile_ID);
                     return RedirectToAction("Employee", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field });
                  }
                  #endregion
               }
            }
            #region Errors relode combo
            if (model.operation == Operation.C)
            {
               model.branchList = cbService.LstBranch(userlogin.Company_ID, true);
               model.departmentList = cbService.LstDepartment(userlogin.Company_ID, true);
               model.desingnationList = cbService.LstDesignation(userlogin.Company_ID, true);
               model.paymentTypeList = cbService.LstLookup(ComboType.Payment_Type, userlogin.Company_ID, false);
               model.termList = cbService.LstTerm();
               model.periodList = cbService.LstPeriod();

               if (model.History_Department_ID.HasValue)
                  model.supervisorList = cbService.LstSupervisor(model.History_Department_ID);
               else
               {
                  if (model.departmentList.Count > 0)
                     model.supervisorList = cbService.LstSupervisor(NumUtil.ParseInteger(model.departmentList[0].Value));
                  else
                     model.supervisorList = new List<ComboViewModel>();
               }

               model.currencyList = cbService.LstCurrency(false);
               model.empTypeList = cbService.LstLookup(ComboType.Employee_Type, userlogin.Company_ID, false);
               model.prtList = cbService.LstPRT(PayrollAllowanceType.Allowance_Deduction, PayrollAllowanceType.Donation);
            }
            #endregion

         }
         else if (pageAction == "addhist" || pageAction == "edithist" || pageAction == "saveAddhist" || pageAction == "saveEdithist")
         {
            if (pageAction == "addhist" || pageAction == "edithist")
            {
               #region History Info (Add)
               if (pageAction == "addhist")
               {
                  model.History_ID = 0;
                  model.History_Branch_ID = null;
                  model.History_Department_ID = null;
                  model.History_Designation_ID = null;
                  model.History_Other_Branch = null;
                  model.History_Other_Department = null;
                  model.History_Other_Designation = null;
                  model.History_Employee_Type = "";
                  model.History_Supervisor = null;
                  model.History_No_Approval_WF = false;
                  model.History_Contract_Staff = false;
                  model.History_Contract_Start_Date = null;
                  model.History_Contract_End_Date = null;
                  model.History_Effective_Date = "";
                  model.History_Confirm_Date = "";
                  model.History_Terminate_Date = "";
                  model.History_Currency_ID = null;
                  model.History_Basic_Salary = 0;
                  model.History_Hour_Rate = 0;
                  model.History_Basic_Salary_Unit = Term.Monthly;
                  model.History_Days = 5;
                  model.History_Notice_Period_Amount = 0;
                  model.History_Notice_Period_Unit = null;
                  model.History_Row_Type = RowType.ADD;
                  model.History_Allowance_Rows = null;
               }

               ModelState.Clear();
               model.branchList = cbService.LstBranch(userlogin.Company_ID, true);
               model.departmentList = cbService.LstDepartment(userlogin.Company_ID, true);
               model.desingnationList = cbService.LstDesignation(userlogin.Company_ID, true);
               model.paymentTypeList = cbService.LstLookup(ComboType.Payment_Type, userlogin.Company_ID, false);
               model.termList = cbService.LstTerm();
               model.periodList = cbService.LstPeriod();

               if (model.History_Department_ID.HasValue)
                  model.supervisorList = cbService.LstSupervisor(model.History_Department_ID);
               else
               {
                  if (model.departmentList.Count > 0)
                     model.supervisorList = cbService.LstSupervisor(NumUtil.ParseInteger(model.departmentList[0].Value));
                  else
                     model.supervisorList = new List<ComboViewModel>();
               }

               model.currencyList = cbService.LstCurrency(false);
               model.empTypeList = cbService.LstLookup(ComboType.Employee_Type, userlogin.Company_ID, false);
               model.prtList = cbService.LstPRT(PayrollAllowanceType.Allowance_Deduction, PayrollAllowanceType.Donation);

               var err = GetErrorModelState();
               return View(model);
               #endregion
            }
            else if (pageAction == "saveAddhist" || pageAction == "saveEdithist")
            {
               #region History Info (Save)
               if (!ModelState.IsValid)
               {
                  foreach (var key in ModelState.Keys.ToList().Where(key => ModelState.ContainsKey(key) && (!key.Contains("History_") | key.Contains("History_Rows"))))
                     ModelState[key].Errors.Clear(); //This is my new solution. Thanks bbak
               }

               if (!ModelState.IsValid && model.History_Allowance_Rows != null)
               {
                  var i = 0;
                  foreach (var row in model.History_Allowance_Rows)
                  {
                     if (row.Row_Type == RowType.DELETE)
                     {
                        DeleteModelStateError("History_Allowance_Rows[" + i + "]");
                     }
                     i++;
                  }
               }

               var emptype = cbService.GetLookup(NumUtil.ParseInteger(model.History_Employee_Type));
               if (emptype != null)
               {
                  if (emptype.Name == "Contract")
                  {
                     if (string.IsNullOrEmpty(model.History_Contract_Start_Date))
                        ModelState.AddModelError("History_Contract_Start_Date", Resource.Message_Is_Required);

                     if (string.IsNullOrEmpty(model.History_Contract_End_Date))
                        ModelState.AddModelError("History_Contract_End_Date", Resource.Message_Is_Required);

                     model.History_Effective_Date = model.History_Contract_Start_Date;
                  }
                  else
                  {
                     model.History_Contract_Start_Date = null;
                     model.History_Contract_End_Date = null;

                     if (string.IsNullOrEmpty(model.History_Effective_Date))
                        ModelState.AddModelError("History_Effective_Date", Resource.Message_Is_Required);
                  }
               }

               if (!model.History_Branch_ID.HasValue && string.IsNullOrEmpty(model.History_Other_Branch))
               {
                  ModelState.AddModelError("History_Branch_ID", Resource.Message_Is_Required);
                  ModelState.AddModelError("History_Other_Branch", Resource.Message_Is_Required);
               }

               if (!model.History_Department_ID.HasValue && string.IsNullOrEmpty(model.History_Other_Department))
               {
                  ModelState.AddModelError("History_Department_ID", Resource.Message_Is_Required);
                  ModelState.AddModelError("History_Other_Department", Resource.Message_Is_Required);
               }

               if ((!model.History_Designation_ID.HasValue || model.History_Designation_ID == 0) && string.IsNullOrEmpty(model.History_Other_Designation))
               {
                  ModelState.AddModelError("History_Designation_ID", Resource.Message_Is_Required);
                  ModelState.AddModelError("History_Other_Designation", Resource.Message_Is_Required);
               }

               if (!string.IsNullOrEmpty(model.History_Terminate_Date))
               {
                  if (!model.History_Notice_Period_Amount.HasValue || model.History_Notice_Period_Amount.Value == 0)
                     ModelState.AddModelError("History_Notice_Period_Amount", Resource.Message_Is_Required);

                  if (string.IsNullOrEmpty(model.History_Notice_Period_Unit))
                     ModelState.AddModelError("History_Notice_Period_Unit", Resource.Message_Is_Required);
               }
               else
               {
                  model.History_Notice_Period_Amount = 0;
                  model.History_Notice_Period_Unit = "";
               }
               if (model.History_Basic_Salary.HasValue && model.History_Basic_Salary == 0)
                  ModelState.AddModelError("History_Basic_Salary", Resource.Message_Is_Required);

               if (string.IsNullOrEmpty(model.History_Basic_Salary_Unit))
                  ModelState.AddModelError("History_Basic_Salary_Unit", Resource.Message_Is_Required);

               if (!model.History_Days.HasValue)
                  ModelState.AddModelError("History_Days", Resource.Message_Is_Required);

               if (ModelState.IsValid)
               {
                  if (pageAction == "saveAddhist" || pageAction == "saveEdithist")
                  {
                     if (model.Employee_Profile_ID.HasValue)
                     {
                        #region Employment History
                        var hist = new Employment_History()
                       {
                          History_ID = (model.History_ID.HasValue ? model.History_ID.Value : 0),
                          Branch_ID = model.History_Branch_ID,
                          Department_ID = model.History_Department_ID,
                          Designation_ID = model.History_Designation_ID,
                          Other_Branch = model.History_Other_Branch,
                          Other_Department = model.History_Other_Department,
                          Other_Designation = model.History_Other_Designation,
                          Employee_Type = NumUtil.ParseInteger(model.History_Employee_Type),
                          Supervisor = (model.History_Supervisor > 0 ? model.History_Supervisor : null),
                          No_Approval_WF = model.History_No_Approval_WF,
                          Effective_Date = DateUtil.ToDate(model.History_Effective_Date),
                          Confirm_Date = DateUtil.ToDate(model.History_Confirm_Date),
                          Terminate_Date = DateUtil.ToDate(model.History_Terminate_Date),
                          Currency_ID = model.History_Currency_ID,
                          Basic_Salary = EncryptUtil.Encrypt(model.History_Basic_Salary),
                          Hour_Rate = model.History_Hour_Rate,
                          Basic_Salary_Unit = model.History_Basic_Salary_Unit,
                          Notice_Period_Amount = model.History_Notice_Period_Amount,
                          Notice_Period_Unit = model.History_Notice_Period_Unit,
                          Contract_Staff = model.History_Contract_Staff,
                          Contract_Start_Date = DateUtil.ToDate(model.History_Contract_Start_Date),
                          Contract_End_Date = DateUtil.ToDate(model.History_Contract_End_Date),
                          Days = model.History_Days,
                          Update_By = userlogin.User_Authentication.Email_Address,
                          Update_On = currentdate,
                          Employee_Profile_ID = model.Employee_Profile_ID.Value,
                       };

                        hist.Employment_History_Allowance = new List<Employment_History_Allowance>();
                        if (model.History_Allowance_Rows != null)
                        {
                           foreach (var row in model.History_Allowance_Rows)
                           {
                              if (row.Row_Type != RowType.DELETE)
                              {
                                 Nullable<int> prcID = null;
                                 if (row.PRC_ID.HasValue && row.PRC_ID.Value > 0)
                                 {
                                    prcID = row.PRC_ID.Value;
                                 }
                                 hist.Employment_History_Allowance.Add(new Employment_History_Allowance()
                                 {
                                    Employment_History_Allowance_ID = (row.Employment_History_Allowance_ID.HasValue ? row.Employment_History_Allowance_ID.Value : 0),
                                    History_ID = (model.History_ID.HasValue ? model.History_ID.Value : 0),
                                    PRC_ID = prcID,
                                    PRT_ID = row.PRT_ID,
                                    Amount = row.Amount,
                                    Update_On = currentdate,
                                    Update_By = userlogin.User_Authentication.Email_Address
                                 });
                              }
                           }
                        }
                        #endregion

                        #region Insert & Update data
                        var hService = new EmploymentHistoryService();
                        if (pageAction == "saveAddhist")
                        {
                           hist.Create_By = userlogin.User_Authentication.Email_Address;
                           hist.Create_On = currentdate;
                           var result = hService.InsertEmploymentHistory(hist, userlogin.Company_ID.Value);
                           if (result.Code == ERROR_CODE.SUCCESS)
                           {
                              model.pageAction = "main";
                              model.tabAction = "hist";

                              model.History_ID = hist.History_ID;
                              model.History_Branch_ID = hist.Branch_ID;
                              model.History_Department_ID = hist.Department_ID;
                              model.History_Designation_ID = hist.Designation_ID;
                              model.History_Row_Type = RowType.EDIT;

                              var allowances = new List<HistoryAllowanceViewModel>();
                              if (hist.Employment_History_Allowance != null && hist.Employment_History_Allowance.Count > 0)
                              {
                                 foreach (var arow in hist.Employment_History_Allowance)
                                 {
                                    if (!arow.PRC_ID.HasValue) arow.PRC_ID = 0;
                                    var allowance = new HistoryAllowanceViewModel()
                                    {
                                       Employment_History_Allowance_ID = arow.Employment_History_Allowance_ID,
                                       PRC_ID = arow.PRC_ID,
                                       PRT_ID = arow.PRT_ID,
                                       Amount = arow.Amount,
                                       Row_Type = RowType.EDIT
                                    };
                                    allowances.Add(allowance);
                                 }
                              }
                              model.History_Allowance_Rows = allowances.ToArray();
                              EmployeeProcessApproval(model.Profile_ID);
                              model.result = result;
                           }
                           else
                           {
                              model.result = result;
                              model.pageAction = "addhist";
                              ModelState.AddModelError("History", model.result.Msg);
                           }
                        }
                        else
                        {
                           var hisID = hService.GetEmploymentHistory(hist.History_ID);
                           if (hisID == null)
                              return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

                           hist.Create_By = hisID.Create_By;
                           hist.Create_On = hisID.Create_On;
                           var result = hService.UpdateEmploymentHistory(hist, userlogin.Company_ID.Value);
                           if (result.Code == ERROR_CODE.SUCCESS)
                           {
                              model.pageAction = "main";
                              model.tabAction = "hist";

                              model.History_Branch_ID = hist.Branch_ID;
                              model.History_Department_ID = hist.Department_ID;
                              model.History_Designation_ID = hist.Designation_ID;

                              var allowances = new List<HistoryAllowanceViewModel>();
                              if (hist.Employment_History_Allowance != null && hist.Employment_History_Allowance.Count > 0)
                              {
                                 foreach (var arow in hist.Employment_History_Allowance)
                                 {
                                    if (!arow.PRC_ID.HasValue) arow.PRC_ID = 0;
                                    var allowance = new HistoryAllowanceViewModel()
                                    {
                                       Employment_History_Allowance_ID = arow.Employment_History_Allowance_ID,
                                       PRC_ID = arow.PRC_ID,
                                       PRT_ID = arow.PRT_ID,
                                       Amount = arow.Amount,
                                       Row_Type = RowType.EDIT
                                    };
                                    allowances.Add(allowance);
                                 }
                              }
                              model.History_Allowance_Rows = allowances.ToArray();
                              EmployeeProcessApproval(model.Profile_ID);
                              model.result = result;
                           }
                           else
                           {
                              model.result = result;
                              model.pageAction = "edithist";
                              ModelState.AddModelError("History", model.result.Msg);
                           }
                        }
                        #endregion
                     }
                  }
               }

               if (ModelState.IsValid)
               {
                  #region History hidden data
                  model.pageAction = "main";
                  var dp = new DepartmentService().GetDepartment(model.History_Department_ID);
                  var ds = new DesignationService().GetDesignation(model.History_Designation_ID);
                  var b = new BranchService().GetBranch(model.History_Branch_ID);
                  var etype = cbService.GetLookup(NumUtil.ParseInteger(model.History_Employee_Type));

                  if (pageAction == "saveAddhist")
                  {
                     var hists = new List<HistoryViewModel>();
                     if (model.History_Rows != null)
                        hists = model.History_Rows.ToList();

                     hists.Add(new HistoryViewModel()
                     {
                        History_ID = (model.History_ID.HasValue ? model.History_ID.Value : 0),
                        Branch_ID = model.History_Branch_ID,
                        Branch_Name = (b != null ? (b.Branch_Code + " : " + b.Branch_Name) : ""),
                        Department_ID = model.History_Department_ID,
                        Department_Name = (dp != null ? dp.Name : ""),
                        Designation_ID = model.History_Designation_ID,
                        Designation_Name = (ds != null ? ds.Name : ""),
                        Other_Branch = model.History_Other_Branch,
                        Other_Department = model.History_Other_Department,
                        Other_Designation = model.History_Other_Designation,
                        Employee_Type = model.History_Employee_Type,
                        Employee_Type_Name = (etype != null ? (etype.Name) : ""),
                        Supervisor = (model.History_Supervisor > 0 ? model.History_Supervisor : null),
                        No_Approval_WF = model.History_No_Approval_WF,
                        Effective_Date = model.History_Effective_Date,
                        Confirm_Date = model.History_Confirm_Date,
                        Terminate_Date = model.History_Terminate_Date,
                        Currency_ID = model.History_Currency_ID,
                        Basic_Salary = model.History_Basic_Salary,
                        Hour_Rate = model.History_Hour_Rate,
                        Basic_Salary_Unit = model.History_Basic_Salary_Unit,
                        History_Allowance_Rows = model.History_Allowance_Rows,
                        Days = model.History_Days,
                        Notice_Period_Amount = model.History_Notice_Period_Amount,
                        Notice_Period_Unit = model.History_Notice_Period_Unit,
                        Contract_Staff = (etype.Name == "Contract" ? true : false),
                        Contract_Start_Date = model.History_Contract_Start_Date,
                        Contract_End_Date = model.History_Contract_End_Date,
                        Row_Type = model.History_Row_Type,
                     });
                     model.History_Rows = hists.ToArray();
                  }
                  else if (pageAction == "saveEdithist")
                  {
                     var i = 0;
                     foreach (var row in model.History_Rows)
                     {
                        if (model.History_Index == i)
                        {
                           row.Branch_ID = model.History_Branch_ID;
                           row.Branch_Name = (b != null ? (b.Branch_Code + " : " + b.Branch_Name) : "");
                           row.Department_ID = model.History_Department_ID;
                           row.Department_Name = (dp != null ? dp.Name : "");
                           row.Designation_ID = model.History_Designation_ID;
                           row.Designation_Name = (ds != null ? ds.Name : "");
                           row.Other_Branch = model.History_Other_Branch;
                           row.Other_Department = model.History_Other_Department;
                           row.Other_Designation = model.History_Other_Designation;
                           row.Employee_Type = model.History_Employee_Type;
                           row.Employee_Type_Name = (etype != null ? (etype.Name) : "");
                           row.Supervisor = (model.History_Supervisor > 0 ? model.History_Supervisor : null);
                           row.No_Approval_WF = model.History_No_Approval_WF;
                           row.Effective_Date = model.History_Effective_Date;
                           row.Confirm_Date = model.History_Confirm_Date;
                           row.Terminate_Date = model.History_Terminate_Date;
                           row.Currency_ID = model.History_Currency_ID;
                           row.Basic_Salary = model.History_Basic_Salary;
                           row.Hour_Rate = model.History_Hour_Rate;
                           row.Basic_Salary_Unit = model.History_Basic_Salary_Unit;
                           row.History_Allowance_Rows = model.History_Allowance_Rows;
                           row.Days = model.History_Days;
                           row.Notice_Period_Amount = model.History_Notice_Period_Amount;
                           row.Notice_Period_Unit = model.History_Notice_Period_Unit;
                           row.Contract_Staff = (row.Employee_Type_Name == "Contract" ? true : false);
                           row.Contract_Start_Date = model.History_Contract_Start_Date;
                           row.Contract_End_Date = model.History_Contract_End_Date;
                           break;
                        }
                        i++;
                     }
                  }
                  #endregion
               }
               else
               {
                  #region Error go to History Info view
                  if (pageAction == "saveAddhist")
                  {
                     model.pageAction = "addhist";
                  }
                  else if (pageAction == "saveEdithist")
                  {
                     model.pageAction = "edithist";
                  }
                  model.paymentTypeList = cbService.LstLookup(ComboType.Payment_Type, userlogin.Company_ID, false);
                  model.departmentList = cbService.LstDepartment(userlogin.Company_ID, true);
                  model.desingnationList = cbService.LstDesignation(userlogin.Company_ID, true);
                  model.branchList = cbService.LstBranch(userlogin.Company_ID, true);
                  model.periodList = cbService.LstPeriod();
                  model.termList = cbService.LstTerm();

                  if (model.History_Department_ID.HasValue)
                     model.supervisorList = cbService.LstSupervisor(model.History_Department_ID);
                  else
                  {
                     if (model.departmentList.Count > 0)
                        model.supervisorList = cbService.LstSupervisor(NumUtil.ParseInteger(model.departmentList[0].Value));
                     else
                        model.supervisorList = new List<ComboViewModel>();
                  }
                  model.currencyList = cbService.LstCurrency(false);
                  model.empTypeList = cbService.LstLookup(ComboType.Employee_Type, userlogin.Company_ID, false);
                  model.prtList = cbService.LstPRT(PayrollAllowanceType.Allowance_Deduction, PayrollAllowanceType.Donation);

                  var err = GetErrorModelState();
                  return View(model);
                  #endregion
               }
               #endregion
            }
         }
         else if (pageAction == "relationship")
         {
            #region Family
            model.tabAction = "family";
            if (!ModelState.IsValid)
            {
               foreach (var key in ModelState.Keys.ToList().Where(key => ModelState.ContainsKey(key) && (!key.Contains("Relationship_") | key.Contains("Relationship_Rows"))))
                  ModelState[key].Errors.Clear(); //This is my new solution. Thanks bbak
            }

            #region Verify

            if (string.IsNullOrEmpty(model.Relationship_Name))
               ModelState.AddModelError("Relationship_Name", Resource.Message_Is_Required);

            if (string.IsNullOrEmpty(model.Relationship_DOB))
               ModelState.AddModelError("Relationship_DOB", Resource.Message_Is_Required);

            if (ModelState.IsValid)
            {
               if (model.Relationship_Rows != null)
               {
                  var i = 0;
                  foreach (var child in model.Relationship_Rows)
                  {
                     if (child.Child_Type == ChildType.OwnChild && child.Row_Type != RowType.DELETE)
                     {
                        var oldchilddob = DateUtil.ToDate(child.DOB);
                        if (oldchilddob.HasValue)
                        {
                           var nextoldchilddob = oldchilddob.Value.AddMonths(9);
                           var prevoldchilddob = oldchilddob.Value.AddMonths(-9);
                           var dob = DateUtil.ToDate(model.Relationship_DOB);
                           if (dob == null)
                              ModelState.AddModelError("Relationship_DOB", Resource.Invalid_Datetime_Format_Lower);

                           var nextdob = dob.Value.AddMonths(9);
                           var prevdob = dob.Value.AddMonths(-9);

                           if (model.Relationship_Index.HasValue && model.Relationship_Index.Value >= 0)
                           {
                              if (i != model.Relationship_Index)
                              {
                                 if (dob < nextoldchilddob & dob > prevoldchilddob)
                                    ModelState.AddModelError("Relationship_DOB", Resource.Your_Own_Child_Cannot_Be_Set_Same_Period_Lower);

                                 if (nextdob < nextoldchilddob & nextdob > prevoldchilddob)
                                    ModelState.AddModelError("Relationship_DOB", Resource.Your_Own_Child_Cannot_Be_Set_Same_Period_Lower);
                              }
                           }
                           else
                           {
                              if (dob < nextoldchilddob & dob > prevoldchilddob)
                                 ModelState.AddModelError("Relationship_DOB", Resource.Your_Own_Child_Cannot_Be_Set_Same_Period_Lower);

                              if (nextdob < nextoldchilddob & nextdob > prevoldchilddob)
                                 ModelState.AddModelError("Relationship_DOB", Resource.Your_Own_Child_Cannot_Be_Set_Same_Period_Lower);
                           }
                        }
                     }
                     i++;
                  }

               }
            }
            #endregion

            if (ModelState.IsValid)
            {
               #region Relationship hidden data
               var nationality = cbService.GetNationality(model.Relationship_Nationality_ID);
               var relation = cbService.GetLookup(model.Relationship_Relationship);
               if (model.Relationship_Index.HasValue && model.Relationship_Index.Value >= 0)
               {
                  var i = 0;
                  foreach (var row in model.Relationship_Rows)
                  {
                     if (model.Relationship_Index == i)
                     {
                        row.Relationship_ID = model.Relationship_ID;
                        row.Company_Name = model.Relationship_Company_Name;
                        row.Company_Position = model.Relationship_Company_Position;
                        row.Passport = model.Relationship_Passport;
                        row.Child_Type = model.Relationship_Child_Type;
                        row.Is_Maternity_Share_Father = model.Relationship_Is_Maternity_Share_Father;
                        row.Working = model.Relationship_Working;
                        row.Name = model.Relationship_Name;
                        row.Relationship = model.Relationship_Relationship;
                        row.Relationship_Name = (relation != null ? relation.Name : "");
                        row.DOB = model.Relationship_DOB;
                        row.Nationality_ID = model.Relationship_Nationality_ID;
                        row.Nationality_Name = (nationality != null ? nationality.Description : "");
                        row.NRIC = model.Relationship_NRIC;
                        row.Gender = model.Relationship_Gender;
                        break;
                     }
                     i++;
                  }
               }
               else
               {
                  var relations = new List<RelationshipViewModels>();
                  if (model.Relationship_Rows != null)
                     relations = model.Relationship_Rows.ToList();

                  relations.Add(new RelationshipViewModels()
                  {
                     Relationship_ID = model.Relationship_ID,
                     Company_Name = model.Relationship_Company_Name,
                     Company_Position = model.Relationship_Company_Position,
                     Passport = model.Relationship_Passport,
                     Child_Type = model.Relationship_Child_Type,
                     Is_Maternity_Share_Father = model.Relationship_Is_Maternity_Share_Father,
                     Working = model.Relationship_Working,
                     Name = model.Relationship_Name,
                     Relationship = model.Relationship_Relationship,
                     Relationship_Name = (relation != null ? relation.Name : ""),
                     DOB = model.Relationship_DOB,
                     Nationality_ID = model.Relationship_Nationality_ID,
                     Nationality_Name = (nationality != null ? nationality.Description : ""),
                     NRIC = model.Relationship_NRIC,
                     Row_Type = model.Relationship_Row_Type,
                     Gender = model.Relationship_Gender,
                  });
                  model.Relationship_Rows = relations.ToArray();
               }
               model.pageAction = "main";
               #endregion
            }
            else
            {
               model.pageAction = "error_relationship";
            }
            #endregion
         }

         var page = new List<string>();
         page.Add("/Employee/Employee");
         page.Add("/Employee/EmployeeInfoAdmin");
         page.Add("/Employee/EmployeeInfoHR");

         var right = base.validatePageRight(page);
         if (right == null || right.Count() == 0)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //-------rights------------
         if (right.ContainsKey("/Employee/Employee")) model.rights = right["/Employee/Employee"].ToArray();
         if (right.ContainsKey("/Employee/EmployeeInfoAdmin")) model.adminRights = right["/Employee/EmployeeInfoAdmin"];
         if (right.ContainsKey("/Employee/EmployeeInfoHR")) model.hrRights = right["/Employee/EmployeeInfoHR"];

         //-------data------------
         model.nationalityList = cbService.LstNationality(false);
         model.empStatusList = cbService.LstEmpStatus();
         model.residentialStatusList = cbService.LstResidentialStatus();
         model.genderList = cbService.LstLookup(ComboType.Gender, userlogin.Company_ID, false);
         model.religionList = cbService.LstLookup(ComboType.Religion, userlogin.Company_ID, false);
         model.raceList = cbService.LstLookup(ComboType.Race, userlogin.Company_ID, false);
         model.wpClassList = cbService.LstLookup(ComboType.WP_Class, userlogin.Company_ID, false);
         model.relationshipList = cbService.LstLookup(ComboType.Relationship, userlogin.Company_ID, true);
         model.maritalStatusList = cbService.LstLookup(ComboType.Marital_Status, userlogin.Company_ID, false);
         model.statusList = cbService.LstStatus();
         model.childTypeList = cbService.LstChildType(true);
         model.paymentTypeList = cbService.LstLookup(ComboType.Payment_Type, userlogin.Company_ID, false);
         model.attachmentTypeList = cbService.LstLookup(ComboType.Attachment_Type, userlogin.Company_ID, true);
         model.workPassTypeList = cbService.LstLookup(ComboType.Work_Pass_Type, userlogin.Company_ID, true);
         model.countryList = cbService.LstCountry(true);
         model.SubscriptionList = comService.LstSubscription(userlogin.Company_ID);

         int[] moduleDetailsID = null;
         if (model.SubscriptionList != null)
         {
            moduleDetailsID = model.SubscriptionList.Select(s => s.Module_Detail_ID.Value).ToArray();
         }
         model.UserRoleList = empService.LstUserRole(moduleDetailsID);
         model.User_Assign_Module = empService.LstUserAssignModule(model.Profile_ID).Select(s => s.Subscription_ID.Value).ToArray();

         Employee_Profile empReload;
         if (model.Employee_Profile_ID == 0 & model.Profile_ID > 0)
            empReload = empService.GetEmployeeProfileByProfileID(model.Profile_ID);
         else
            empReload = empService.GetEmployeeProfile(model.Employee_Profile_ID);

         if (empReload != null && empReload.User_Profile != null && empReload.User_Profile.User_Profile_Photo != null)
         {
            var photo = empReload.User_Profile.User_Profile_Photo.FirstOrDefault();
            if (photo != null)
            {
               model.User_Profile_Photo_ID = photo.User_Profile_Photo_ID;
               model.User_Photo = photo.Photo;
            }
         }
         var erer = GetErrorModelState();
         return View(model);
      }



      public ActionResult AddNewEmployeeEmergencyContact(int pIndex)
      {
         var userlogin = UserSession.getUser(HttpContext);
         var cbService = new ComboService();
         var model = new EmployeeEmergencyContactViewModel() { Index = pIndex };
         model.relationshipList = cbService.LstLookup(ComboType.Relationship, userlogin.Company_ID, true);
         return PartialView("EmployeeEmerContactRow", model);
      }

      public ActionResult AddNewBankInfo(int pIndex)
      {
         var userlogin = UserSession.getUser(HttpContext);
         var cbService = new ComboService();
         var model = new BankInfoViewModel() { Index = pIndex };
         model.paymentTypeList = cbService.LstLookup(ComboType.Payment_Type, userlogin.Company_ID, false);
         return PartialView("BankInfoRow", model);
      }

      public ActionResult AddNewHistAllowance(int pIndex)
      {
         var userlogin = UserSession.getUser(HttpContext);
         var cbService = new ComboService();
         var model = new HistoryAllowanceViewModel() { Index = pIndex };
         model.prtList = cbService.LstPRT(PayrollAllowanceType.Allowance_Deduction, PayrollAllowanceType.Donation);
         if (model.prtList.Count > 0)
            model.prcList = cbService.LstPRC(userlogin.Company_ID, NumUtil.ParseInteger(model.prtList[0].Value));

         var comService = new CompanyService();
         var com = comService.GetCompany(userlogin.Company_ID);
         if (com != null && com.Currency != null)
            model.Company_Currency_Code = com.Currency.Currency_Code;

         return PartialView("EmployeeHistAllowanceRow", model);
      }

      public ActionResult AddNewAttachment(Nullable<int> pIndex, Nullable<int> pAttachType, string pRenameFile, string pFile, string pFileName)
      {

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var currentdate = StoredProcedure.GetCurrentDate();
         var cbService = new ComboService();
         var attachType = cbService.GetLookup(pAttachType);

         MemoryStream target = new MemoryStream();
         byte[] data = target.ToArray();

         var model = new AttachmentViewModel()
         {
            Index = pIndex.Value,
            Attachment_File = data,
            Attachment_Type = pAttachType,
            Attachment_Type_Name = (attachType != null ? attachType.Name : ""),
            Attachment_ID = Guid.NewGuid(),
            Uploaded_By = userlogin.User_Authentication.Email_Address,
            Uploaded_On = DateUtil.ToDisplayDate(currentdate),
         };

         if (!string.IsNullOrEmpty(pFile))
         {
            var fname = pFileName;
            var type = Path.GetExtension(fname);

            if (!string.IsNullOrEmpty(pRenameFile))
               fname = pRenameFile + type;

            string trimmedData = pFile;
            var prefixindex = trimmedData.IndexOf(",");
            trimmedData = trimmedData.Substring(prefixindex + 1, trimmedData.Length - (prefixindex + 1));
            var filebyte = Convert.FromBase64String(trimmedData);
            if (filebyte != null)
            {
               model.Attachment_File = filebyte;
               model.File_Name = fname;
            }
         }
         return PartialView("EmployeeAttachmentRow", model);
      }



      public ActionResult CheckEmpHistRelation(Nullable<int> pHistoryID)
      {
         var related = new EmployeeService().IsHistoryRelated(pHistoryID);
         return Json(new { related = related }, JsonRequestBehavior.AllowGet);
      }

      public ActionResult CheckDuplication(string ParamEmail, string ParamUserName)
      {
         var uService = new UserService();
         var bFlag = uService.isDuplicate(ParamEmail, ParamUserName);
         return Json(new { isDuplicate = bFlag }, JsonRequestBehavior.AllowGet);
      }



      public System.Drawing.Image byteArrayToImage(byte[] byteArrayIn)
      {
         using (System.IO.MemoryStream mStream = new System.IO.MemoryStream(byteArrayIn))
         {
            return System.Drawing.Image.FromStream(mStream);
         }
      }

      public ActionResult UploadUserPhoto(Nullable<int> pProfileID)
      {
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var currentdate = StoredProcedure.GetCurrentDate();
         HttpPostedFileBase file = Request.Files[0];
         int fileSizeInBytes = file.ContentLength;
         MemoryStream target = new MemoryStream();
         file.InputStream.CopyTo(target);
         byte[] data = target.ToArray();

         System.Drawing.Image image = byteArrayToImage(data);
         int thumbnailSize = 150;
         int newWidth = 0;
         int newHeight = 0;
         if (image.Width > image.Height)
         {
            newWidth = thumbnailSize;
            newHeight = (int)(image.Height * thumbnailSize / image.Width);
         }
         else
         {
            newWidth = (int)(image.Width * thumbnailSize / image.Height);
            newHeight = thumbnailSize;
         }

         var thumbnailBitmap = new System.Drawing.Bitmap(newWidth, newHeight);
         var thumbnailGraph = System.Drawing.Graphics.FromImage(thumbnailBitmap);
         thumbnailGraph.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
         thumbnailGraph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
         thumbnailGraph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

         var imageRectangle = new System.Drawing.Rectangle(0, 0, newWidth, newHeight);
         thumbnailGraph.DrawImage(image, imageRectangle);

         //thumbnailBitmap.Save(outputPath & ".jpg", image.RawFormat);
         System.IO.MemoryStream ms = new System.IO.MemoryStream();
         thumbnailBitmap.Save(ms, image.RawFormat);

         data = ms.ToArray();

         var uService = new UserService();
         if (pProfileID.HasValue && pProfileID.Value > 0)
         {
            User_Profile_Photo UserPhoto = new User_Profile_Photo();
            UserPhoto.Profile_ID = pProfileID;
            UserPhoto.Photo = data;
            UserPhoto.Create_By = userlogin.User_Authentication.Email_Address;
            UserPhoto.Create_On = currentdate;
            UserPhoto.Update_By = userlogin.User_Authentication.Email_Address;
            UserPhoto.Update_On = currentdate;
            uService.SaveUserPhoto(UserPhoto);
         }

         var base64 = Convert.ToBase64String(data);
         var imgSrc = String.Format("data:image/gif;base64,{0}", base64);
         return Json(new { img = imgSrc, imgByte = data }, JsonRequestBehavior.AllowGet);

      }

      public ActionResult ReloadNumber(string pNumber, Nullable<int> pNationalityID, Nullable<int> pEmpID, string pEmpNo)
      {
         var userlogin = UserSession.getUser(HttpContext);
         var empService = new EmployeeService();
         var histService = new EmploymentHistoryService();
         var cbService = new ComboService();
         var comService = new CompanyService();
         var patternService = new PatternService();

         if (!pEmpID.HasValue)
            return Json(new { err = Resource.The + " " + Resource.Employee_ID + " " + Resource.Field + " " + Resource.Is_Rrequired_Lower }, JsonRequestBehavior.AllowGet);


         var pattern = patternService.GetPattern(userlogin.Company_ID);
         if (pattern == null)
            return Json(new { err = Resource.The + " " + Resource.Employee_Pattern + " " + Resource.Field + " " + Resource.Is_Rrequired_Lower }, JsonRequestBehavior.AllowGet);


         if (string.IsNullOrEmpty(pEmpNo))
            pEmpNo = empService.GetEmployeeNo(pEmpID, false);


         bool selYear = pattern.Select_Year;
         bool selNation = pattern.Select_Nationality;
         bool selBranch = (pattern.Select_Branch_Code.HasValue ? pattern.Select_Branch_Code.Value : false);

         var year = "";
         var nal = "";
         var branch = "";

         var empNoSplit = pEmpNo.Split('-');

         var splitcnt = 1;
         if (selYear) splitcnt += 1;
         if (selNation) splitcnt += 1;
         if (selBranch) splitcnt += 1;

         if (splitcnt != empNoSplit.Length)
         {
            if (empNoSplit.Length > 0)
            {
               if (NumUtil.ParseInteger(empNoSplit[empNoSplit.Length - 1]) > 0)
               {
                  pNumber = empNoSplit[empNoSplit.Length - 1];
               }
            }

            pEmpNo = empService.GetEmployeeNo(pEmpID, false);
            empNoSplit = pEmpNo.Split('-');
         }
         //return Json(new { err = "The Emploee No is invalid." }, JsonRequestBehavior.AllowGet);

         splitcnt = 0;
         if (selYear)
         {
            year = empNoSplit[splitcnt] + "-";
            splitcnt += 1;
         }
         if (selNation)
         {
            nal = empNoSplit[splitcnt] + "-";
            splitcnt += 1;
         }
         if (selBranch)
         {
            branch = empNoSplit[splitcnt] + "-";
            splitcnt += 1;
         }

         var newEmpNo = "";
         if (!pNationalityID.HasValue & string.IsNullOrEmpty(pNumber))
         {
            return Json(new { err = Resource.The + " " + Resource.Number + " " + Resource.OR_Lower + " " + Resource.Nationality + " " + Resource.Field + " " + Resource.Is_Rrequired_Lower }, JsonRequestBehavior.AllowGet);
         }
         else if (pNationalityID.HasValue & !string.IsNullOrEmpty(pNumber))
         {
            var naltionality = cbService.GetNationality(pNationalityID);
            if (naltionality != null)
            {
               newEmpNo = year + naltionality.Name + "-" + branch + pNumber;

            }
         }
         else if (pNationalityID.HasValue)
         {
            var naltionality = cbService.GetNationality(pNationalityID);
            if (naltionality != null)
            {
               var Number = empNoSplit[splitcnt];
               newEmpNo = year + naltionality.Name + "-" + branch + Number;
            }
         }
         else if (!string.IsNullOrEmpty(pNumber))
         {
            if (pNumber.Length > 6)
               return Json(new { err = Resource.The + " " + Resource.Number + " " + Resource.Field + " " + Resource.Is_Invalid_Lower }, JsonRequestBehavior.AllowGet);


            if (NumUtil.ParseInteger(pNumber) == 0)
               return Json(new { err = Resource.The + " " + Resource.Number + " " + Resource.Field + " " + Resource.Is_Invalid_Lower }, JsonRequestBehavior.AllowGet);


            var Number = pNumber.PadLeft(6, '0');
            newEmpNo = year + nal + branch + Number;
         }

         var isExist = empService.ExsistEmployeeNo(userlogin.Company_ID, pEmpID, newEmpNo);
         if (isExist)
            return Json(new { err = Resource.The + " " + Resource.Employee_No + " " + Resource.Field + " " + Resource.Is_Duplicated_Lower }, JsonRequestBehavior.AllowGet);

         return Json(new { NewEmpNo = newEmpNo }, JsonRequestBehavior.AllowGet);
      }

      [HttpGet]
      public void DisplayAttactment(System.Guid pAttID)
      {
         //Added by Jane 02/02/2016
         var eService = new EmployeeService();
         var attact = eService.GetEmpAttachment(pAttID);
         if (attact != null)
         {
            var file = attact.Attachment_File;
            if (file != null)
            {
               if (attact.File_Name.Contains(".pdf"))
               {
                  Response.ClearHeaders();
                  Response.Clear();
                  Response.AddHeader("Content-Type", "application/pdf");
                  Response.AddHeader("Content-Length", file.Length.ToString());
                  Response.AddHeader("Content-Disposition", "inline; filename=\"" + attact.File_Name + "\"");
                  Response.BinaryWrite(file);
                  Response.Flush();
                  Response.End();
               }
               else
               {
                  var ext = attact.File_Name.Substring(attact.File_Name.IndexOf(".") + 1);
                  Response.ClearHeaders();
                  Response.Clear();
                  Response.AddHeader("Content-Type", "application/" + ext);
                  Response.AddHeader("content-length", file.Length.ToString());
                  Response.AddHeader("Content-Disposition", "inline; filename=\"" + attact.File_Name + "\"");
                  Response.BinaryWrite(file);
                  Response.Flush();
                  Response.End();
               }
            }
         }
      }

      public ActionResult DeleteAttach(Nullable<Guid> pAttachmentID)
      {
         var eService = new EmployeeService();
         var result = eService.DeleteAttachment(pAttachmentID);

         return Json(new
           {
              code = result
           }, JsonRequestBehavior.AllowGet);
      }

      public ActionResult GenerateEmpNo(Nullable<int> pEmpID)
      {
         var empNo = "";
         var eService = new EmployeeService();
         if (pEmpID.HasValue && pEmpID.Value > 0)
         {
            empNo = eService.GetEmployeeNo(pEmpID);
         }
         return Json(new { EmpNo = empNo }, JsonRequestBehavior.AllowGet);

      }
      #endregion

      #region Import & Export
      [HttpGet]
      public ActionResult EmployeeImport()
      {
         var model = new ImportEmployeeViewModels();

         User_Profile userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //Validate Page Right
         RightResult rightResult = base.validatePageRight(UserSession.RIGHT_C);
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;

         model.emps = new List<ImportEmployeeProfile_>().ToArray();
         model.hiss = new List<ImportHistory_>().ToArray();
         model.emergencys = new List<ImportEmergencyContact_>().ToArray();
         model.relations = new List<ImportRelationship_>().ToArray();
         model.errMsg = new List<string>();
         model.errMsg_Hit = new List<string>();
         model.errMsg_Eemc = new List<string>();
         model.errMsg_Relation = new List<string>();
         model.validated = true;

         return View(model);
      }

      [HttpPost]
      [AllowAuthorized]
      public ActionResult EmployeeImport(ImportEmployeeViewModels model, string pageAction)
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
         var empService = new EmployeeService();
         var cbService = new ComboService();
         var comService = new CompanyService();
         var userService = new UserService();

         if (pageAction == "import")
         {
            if (model.emps.Length > 0 && model.validated)
            {
               model.result = empService.ImportEmployee(model.emps, model.hiss, model.emergencys, model.relations);
               if (model.result.Code == ERROR_CODE.SUCCESS)
               {
                  return RedirectToAction("Employee", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field });
               }
            }
         }
         else
         {
            if (Request.Files.Count == 0)
            {
               ModelState.AddModelError("Import_Employee", Resource.Message_Cannot_Found_Excel_Sheet);
               return View(model);
            }

            HttpPostedFileBase file = Request.Files[0];
            if (file != null)
            {
               var com = comService.GetCompany(userlogin.Company_ID);
               if (com == null)
                  return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

               //Employee Profile Past 1
               var genderlst = cbService.LstLookup(ComboType.Gender, userlogin.Company_ID);
               var maritalStatuslst = cbService.LstLookup(ComboType.Marital_Status, userlogin.Company_ID);
               var racelst = cbService.LstLookup(ComboType.Race, userlogin.Company_ID);
               var residentallst = cbService.LstResidentialStatus();
               var empstatuslst = cbService.LstEmpStatus();
               var nationalitylst = cbService.LstNationality(false);
               var religionlst = cbService.LstLookup(ComboType.Religion, userlogin.Company_ID);
               var paymentlst = cbService.LstLookup(ComboType.Payment_Type, userlogin.Company_ID);
               var banklst = cbService.LstLookup(ComboType.Bank_Name, userlogin.Company_ID);
               var countrylst = cbService.LstCountry(false);
               var emaillst = userService.LstEmail(userlogin.Company_ID);
               var employeeprofilelst = empService.LstEmployeeProfile(userlogin.Company_ID);

               //Employment History Past 2
               var employeeTypelst = cbService.LstLookup(ComboType.Employee_Type, userlogin.Company_ID);
               var departmentlst = cbService.LstDepartment(userlogin.Company_ID);
               var desingnationlst = cbService.LstDesignation(userlogin.Company_ID);
               var branchlst = cbService.LstBranch(userlogin.Company_ID, false);
               var currencylst = cbService.LstCurrencyCode(false);
               var termlst = cbService.LstTerm();

               //Employment History Past 3 - 4
               var relationshiplst = cbService.LstLookup(ComboType.Relationship, userlogin.Company_ID);

               try
               {
                  using (var package = new ExcelPackage(file.InputStream))
                  {
                     List<string> chk_Emp_No = new List<string>();
                     model.validated = true;

                     //--------------------------------Employee Info Past 1-----------------------------------//
                     ExcelWorksheet worksheet_1 = package.Workbook.Worksheets[1];
                     if (worksheet_1.Dimension != null)
                     {
                        bool validated_1 = true;
                        int totalRows_1 = worksheet_1.Dimension.End.Row;
                        int totalCols_1 = worksheet_1.Dimension.End.Column;

                        if (totalCols_1 != 27)
                        {
                           ModelState.AddModelError("emps", Resource.Message_Column_Count_Is_Invalid + " " + Resource.Message_Please_Edit_Reupload);
                           model.validated = false;
                           validated_1 = false;
                        }
                        if (totalRows_1 <= 1)
                        {
                           ModelState.AddModelError("emps", Resource.Message_Row_Count_Is_Invalid + " " + Resource.Message_Please_Edit_Reupload);
                           model.validated = false;
                           validated_1 = false;
                        }

                        if (validated_1)
                        {
                           if (totalRows_1 > 1)
                           {
                              var emps = new List<ImportEmployeeProfile_>();
                              for (int i = 2; i <= totalRows_1; i++)
                              {
                                 var emp = new ImportEmployeeProfile_();
                                 emp.Company_ID = userlogin.Company_ID;
                                 emp.Validate_Emp = true;
                                 var isempty = true;
                                 var err_Emp = new System.Text.StringBuilder();

                                 for (int j = 1; j <= totalCols_1; j++)
                                 {
                                    var columnName = worksheet_1.Cells[1, j].Value.ToString();
                                    isempty = false;
                                    if (worksheet_1.Cells[i, j].Value != null)
                                    {

                                       if (j == EmpImportColumn.Email)
                                       {
                                          emp.Email_Address = worksheet_1.Cells[i, j].Value.ToString();
                                          var mcount = 0;
                                          if (emps.Count > 0)
                                          {
                                             var dupemail = emps.Where(w => w.Email_Address == worksheet_1.Cells[i, j].Value.ToString());
                                             mcount = dupemail.Count();
                                          }
                                          if (emaillst.Contains(worksheet_1.Cells[i, j].Value.ToString()))
                                          {
                                             mcount += 1;
                                          }

                                          if (mcount > 0)
                                          {
                                             model.validated = false;
                                             emp.Validate_Emp = false;
                                             err_Emp.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Duplicated_Lower);
                                          }
                                          else if (emaillst.Contains(worksheet_1.Cells[i, j].Value.ToString()))
                                          {
                                             model.validated = false;
                                             emp.Validate_Emp = false;
                                             err_Emp.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Duplicated_Lower);
                                          }
                                          else if (!StrUtil.IsValidEmail(worksheet_1.Cells[i, j].Value.ToString()))
                                          {
                                             model.validated = false;
                                             emp.Validate_Emp = false;
                                             err_Emp.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Duplicated_Lower);
                                          }

                                          if (emp.Email_Address.Length > 150)
                                          {
                                             model.validated = false;
                                             emp.Validate_Emp = false;
                                             err_Emp.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Message_Maximum_Length + " '150'.");
                                          }

                                       }
                                       else if (j == EmpImportColumn.Employee_No)
                                       {
                                          var emp_no = "";
                                          emp_no = worksheet_1.Cells[i, j].Value.ToString();
                                          chk_Emp_No.Add(emp_no);

                                          var empprofil = employeeprofilelst.Where(w => w.Employee_No.ToString().Trim() == emp_no.ToString().Trim()).FirstOrDefault();
                                          if (empprofil != null)
                                          {
                                             emp.Employee_No = emp_no;
                                             model.validated = false;
                                             emp.Validate_Emp = false;
                                             err_Emp.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Duplicated_Lower);
                                          }
                                          else
                                          {
                                             if (chk_Emp_No.GroupBy(n => n).Any(c => c.Count() > 1))
                                             {
                                                emp.Employee_No = emp_no;
                                                model.validated = false;
                                                emp.Validate_Emp = false;
                                                err_Emp.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Duplicated_Lower);
                                             }
                                             else
                                             {
                                                emp.Employee_No = emp_no;
                                             }
                                          }

                                       }
                                       else if (j == EmpImportColumn.Gender)
                                       {
                                          var gender = genderlst.Where(w => w.Text.ToLower().Trim() == worksheet_1.Cells[i, j].Value.ToString().ToLower().Trim() || w.Desc.ToLower().Trim() == worksheet_1.Cells[i, j].Value.ToString().ToLower().Trim()).FirstOrDefault();
                                          if (gender == null)
                                          {
                                             model.validated = false;
                                             emp.Validate_Emp = false;
                                             err_Emp.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                          }
                                          else
                                          {
                                             emp.Gender = NumUtil.ParseInteger(gender.Value);
                                             emp.Gender_ = gender.Text;
                                          }
                                       }
                                       else if (j == EmpImportColumn.Marital_Status)
                                       {
                                          //Marital Status
                                          var maritalStatus = maritalStatuslst.Where(w => w.Text.ToLower().Trim() == worksheet_1.Cells[i, j].Value.ToString().ToLower().Trim() || w.Desc.ToLower().Trim() == worksheet_1.Cells[i, j].Value.ToString().ToLower().Trim()).FirstOrDefault();
                                          if (maritalStatus == null)
                                          {
                                             model.validated = false;
                                             emp.Validate_Emp = false;
                                             err_Emp.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                          }
                                          else
                                          {
                                             emp.Marital_Status = NumUtil.ParseInteger(maritalStatus.Value);
                                             emp.Marital_Status_ = maritalStatus.Text;
                                          }
                                       }
                                       else if (j == EmpImportColumn.Religion)
                                       {
                                          var religion = religionlst.Where(w => w.Text.ToLower().Trim() == worksheet_1.Cells[i, j].Value.ToString().ToLower().Trim() || w.Desc.ToLower().Trim() == worksheet_1.Cells[i, j].Value.ToString().ToLower().Trim()).FirstOrDefault();
                                          if (religion == null)
                                          {
                                             model.validated = false;
                                             emp.Validate_Emp = false;
                                             err_Emp.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                          }
                                          else
                                          {
                                             emp.Religion = NumUtil.ParseInteger(religion.Value);
                                             emp.Religion_ = religion.Text;
                                          }
                                       }
                                       else if (j == EmpImportColumn.Race)
                                       {
                                          var race = racelst.Where(w => w.Text.ToLower().Trim() == worksheet_1.Cells[i, j].Value.ToString().ToLower().Trim() || w.Desc.ToLower().Trim() == worksheet_1.Cells[i, j].Value.ToString().ToLower().Trim()).FirstOrDefault();
                                          if (race == null)
                                          {
                                             model.validated = false;
                                             emp.Validate_Emp = false;
                                             err_Emp.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                          }
                                          else
                                          {
                                             emp.Race = NumUtil.ParseInteger(race.Value);
                                             emp.Race_ = race.Text;
                                          }
                                       }
                                       else if (j == EmpImportColumn.Nationality)
                                       {
                                          var national = nationalitylst.Where(w => w.Text.ToLower().Trim() == worksheet_1.Cells[i, j].Value.ToString().ToLower().Trim() || w.Desc.ToLower().Trim() == worksheet_1.Cells[i, j].Value.ToString().ToLower().Trim()).FirstOrDefault();
                                          if (national == null)
                                          {
                                             model.validated = false;
                                             emp.Validate_Emp = false;
                                             err_Emp.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                          }
                                          else
                                          {
                                             emp.Nationality_ID = NumUtil.ParseInteger(national.Value);
                                             emp.Nationality_ = national.Text;
                                          }
                                       }
                                       else if (j == EmpImportColumn.Residential_Status)
                                       {
                                          var resident = residentallst.Where(w => w.Text.ToLower().Trim() == worksheet_1.Cells[i, j].Value.ToString().ToLower().Trim() || w.Value.ToLower().Trim() == worksheet_1.Cells[i, j].Value.ToString().ToLower().Trim()).FirstOrDefault();
                                          if (resident == null)
                                          {
                                             model.validated = false;
                                             emp.Validate_Emp = false;
                                             err_Emp.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                          }
                                          else
                                          {
                                             emp.Residential_Status = resident.Value;
                                             emp.Residential_Status_ = resident.Text;
                                          }
                                       }
                                       else if (j == EmpImportColumn.Bank_Name)
                                       {
                                          var bank = banklst.Where(w => w.Text.ToLower().Trim() == worksheet_1.Cells[i, j].Value.ToString().ToLower().Trim() || w.Value.ToLower().Trim() == worksheet_1.Cells[i, j].Value.ToString().ToLower().Trim()).FirstOrDefault();
                                          if (bank == null)
                                          {
                                             model.validated = false;
                                             emp.Validate_Emp = false;
                                             err_Emp.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                          }
                                          else
                                          {
                                             emp.Bank_Name = NumUtil.ParseInteger(bank.Value);
                                             emp.Bank_Name_ = bank.Text;
                                          }
                                       }
                                       else if (j == EmpImportColumn.Payment_Type)
                                       {
                                          var payment = paymentlst.Where(w => w.Text.ToLower().Trim() == worksheet_1.Cells[i, j].Value.ToString().ToLower().Trim() || w.Value.ToLower().Trim() == worksheet_1.Cells[i, j].Value.ToString().ToLower().Trim()).FirstOrDefault();
                                          if (payment == null)
                                          {
                                             model.validated = false;
                                             emp.Validate_Emp = false;
                                             err_Emp.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                          }
                                          else
                                          {
                                             emp.Payment_Type = NumUtil.ParseInteger(payment.Value);
                                             emp.Payment_Type_ = payment.Text;
                                          }
                                       }
                                       else if (j == EmpImportColumn.Address1_Country || j == EmpImportColumn.Address2_Country)
                                       {
                                          var country = countrylst.Where(w => w.Text.ToLower().Trim() == worksheet_1.Cells[i, j].Value.ToString().ToLower().Trim() || w.Value.ToLower().Trim() == worksheet_1.Cells[i, j].Value.ToString().ToLower().Trim()).FirstOrDefault();
                                          if (country == null)
                                          {
                                             model.validated = false;
                                             emp.Validate_Emp = false;
                                             err_Emp.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                          }
                                          else
                                          {
                                             if (j == EmpImportColumn.Address1_Country)
                                             {
                                                emp.Address1_Country = NumUtil.ParseInteger(country.Value);
                                                emp.Address1_Country_ = country.Text;
                                             }
                                             else if (j == EmpImportColumn.Address2_Country)
                                             {
                                                emp.Address2_Country = NumUtil.ParseInteger(country.Value);
                                                emp.Address2_Country_ = country.Text;
                                             }
                                          }
                                       }
                                       else if (j == EmpImportColumn.Address1_Postal_Code || j == EmpImportColumn.Address2_Postal_Code)
                                       {
                                          var strpostcode = "";
                                          strpostcode = worksheet_1.Cells[i, j].Value.ToString();

                                          if (strpostcode.Length > 20)
                                          {
                                             model.validated = false;
                                             emp.Validate_Emp = false;
                                             strpostcode = worksheet_1.Cells[i, j].Value.ToString();
                                             err_Emp.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Message_Maximum_Length + " '20'.");
                                          }

                                          if (j == EmpImportColumn.Address1_Postal_Code)
                                             emp.Address1_Postal_Code = strpostcode;
                                          else if (j == EmpImportColumn.Address2_Postal_Code)
                                             emp.Address2_Postal_Code = strpostcode;
                                       }
                                       else if (j == EmpImportColumn.Address1 || j == EmpImportColumn.Address2)
                                       {
                                          var straddress = "";
                                          straddress = worksheet_1.Cells[i, j].Value.ToString();

                                          if (straddress.Length > 300)
                                          {
                                             model.validated = false;
                                             emp.Validate_Emp = false;
                                             straddress = worksheet_1.Cells[i, j].Value.ToString();
                                             err_Emp.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Message_Maximum_Length + " '300'.");
                                          }

                                          if (j == EmpImportColumn.Address1)
                                             emp.Address1 = straddress;
                                          else if (j == EmpImportColumn.Address2)
                                             emp.Address2 = straddress;
                                       }
                                       else if (j == EmpImportColumn.DOB || j == EmpImportColumn.Date_Of_Issue || j == EmpImportColumn.Date_Of_Expire || j == EmpImportColumn.Effective_Date)
                                       {
                                          // Validate date
                                          var strdate = "";
                                          try
                                          {
                                             var date = (DateTime)worksheet_1.Cells[i, j].Value;
                                             strdate = DateUtil.ToDisplayDate(date);
                                          }
                                          catch
                                          {
                                             model.validated = false;
                                             emp.Validate_Emp = false;
                                             strdate = worksheet_1.Cells[i, j].Value.ToString();
                                             err_Emp.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                          }

                                          if (j == EmpImportColumn.DOB)
                                             emp.DOB = strdate;
                                          else if (j == EmpImportColumn.Date_Of_Issue)
                                             emp.Date_Of_Issue = strdate;
                                          else if (j == EmpImportColumn.Date_Of_Expire)
                                             emp.Date_Of_Expire = strdate;
                                          else if (j == EmpImportColumn.Effective_Date)
                                             emp.Effective_Date = strdate;

                                       }
                                       else if (j == EmpImportColumn.First_Name || j == EmpImportColumn.Middle_Name || j == EmpImportColumn.Last_Name)
                                       {
                                          var strname = "";
                                          strname = worksheet_1.Cells[i, j].Value.ToString();

                                          if (strname.Length > 150)
                                          {
                                             model.validated = false;
                                             emp.Validate_Emp = false;
                                             strname = worksheet_1.Cells[i, j].Value.ToString();
                                             err_Emp.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Message_Maximum_Length + " '150'.");
                                          }

                                          if (j == EmpImportColumn.First_Name)
                                             emp.First_Name = strname;
                                          else if (j == EmpImportColumn.Middle_Name)
                                             emp.Middle_Name = strname;
                                          else if (j == EmpImportColumn.Last_Name)
                                             emp.Last_Name = strname;

                                       }
                                       else if (j == EmpImportColumn.Mobile_Phone)
                                       {
                                          emp.Mobile_No = worksheet_1.Cells[i, j].Value.ToString();
                                          if (emp.Mobile_No.Length > 150)
                                          {
                                             model.validated = false;
                                             emp.Validate_Emp = false;
                                             err_Emp.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Message_Maximum_Length + " '150'.");
                                          }
                                       }
                                       else if (j == EmpImportColumn.Bank_Account)
                                       {
                                          emp.Bank_Account = worksheet_1.Cells[i, j].Value.ToString();
                                          if (emp.Bank_Account.Length > 150)
                                          {
                                             model.validated = false;
                                             emp.Validate_Emp = false;
                                             err_Emp.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Message_Maximum_Length + " '150'.");
                                          }
                                       }
                                       else if (j == EmpImportColumn.NRIC)
                                       {
                                          emp.NRIC = worksheet_1.Cells[i, j].Value.ToString();
                                          if (emp.NRIC.Length > 30)
                                          {
                                             model.validated = false;
                                             emp.Validate_Emp = false;
                                             err_Emp.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Message_Maximum_Length + " '30'.");
                                          }
                                       }
                                       else if (j == EmpImportColumn.Passport)
                                       {
                                          emp.Passport = worksheet_1.Cells[i, j].Value.ToString();
                                          if (emp.Passport.Length > 100)
                                          {
                                             model.validated = false;
                                             emp.Validate_Emp = false;
                                             err_Emp.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Message_Maximum_Length + " '100'.");
                                          }
                                       }
                                       else if (j == EmpImportColumn.Employee_No)
                                       {
                                          emp.Employee_No = worksheet_1.Cells[i, j].Value.ToString();
                                          if (emp.Employee_No.Length > 150)
                                          {
                                             model.validated = false;
                                             emp.Validate_Emp = false;
                                             err_Emp.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Message_Maximum_Length + " '150'.");
                                          }
                                       }
                                    }
                                    else
                                    {
                                       // Validate require
                                       if (j == EmpImportColumn.Employee_No
                                           || j == EmpImportColumn.First_Name
                                           || j == EmpImportColumn.Last_Name
                                          //|| j == EmpImportColumn.Mobile_Phone
                                           || j == EmpImportColumn.Email
                                           || j == EmpImportColumn.Gender
                                           || j == EmpImportColumn.Marital_Status
                                           || j == EmpImportColumn.DOB
                                           || j == EmpImportColumn.Race
                                           || j == EmpImportColumn.Religion
                                           || j == EmpImportColumn.Nationality
                                           || j == EmpImportColumn.Residential_Status
                                           || j == EmpImportColumn.NRIC
                                          //|| j == EmpImportColumn.Date_Of_Issue
                                          //|| j == EmpImportColumn.Date_Of_Expire
                                           || j == EmpImportColumn.Address1
                                           || j == EmpImportColumn.Address1_Country
                                           || j == EmpImportColumn.Address1_Postal_Code
                                           || j == EmpImportColumn.Bank_Name
                                           || j == EmpImportColumn.Bank_Account
                                           || j == EmpImportColumn.Payment_Type
                                           || j == EmpImportColumn.Effective_Date
                                           )
                                       {
                                          model.validated = false;
                                          emp.Validate_Emp = false;
                                          err_Emp.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Rrequired_Lower);
                                       }
                                    }
                                 }
                                 if (isempty)
                                 {
                                    model.validated = false;
                                    emp.Validate_Emp = false;
                                    err_Emp.AppendLine(Resource.Message_Empty_Row);
                                 }
                                 emp.ErrMsg_Emp = err_Emp.ToString();
                                 emp.Create_By = userlogin.User_Authentication.Email_Address;
                                 emp.Create_On = currentdate;
                                 emps.Add(emp);
                              }
                              model.emps = emps.ToArray();
                           }
                        }

                        if (!validated_1)
                        {
                           model.emps = new List<ImportEmployeeProfile_>().ToArray();
                        }
                     }
                     //-------------------------------EmploymentHistory Past 2-------------------------------------//
                     ExcelWorksheet worksheet_2 = package.Workbook.Worksheets[2];

                     if (worksheet_2.Dimension != null)
                     {
                        bool validated_2 = true;
                        int totalRows_2 = worksheet_2.Dimension.End.Row;
                        int totalCols_2 = worksheet_2.Dimension.End.Column;

                        if (totalCols_2 != 10)
                        {
                           ModelState.AddModelError("hiss", Resource.Message_Column_Count_Is_Invalid + " " + Resource.Message_Please_Edit_Reupload);
                           model.validated = false;
                           validated_2 = false;
                        }
                        if (totalRows_2 <= 1)
                        {
                           //ModelState.AddModelError("hiss", "Row count is invalid.  please edit your information and reupload.");
                           //model.validated = false;
                           validated_2 = false;
                        }

                        if (validated_2)
                        {
                           if (totalRows_2 > 1)
                           {
                              var hiss = new List<ImportHistory_>();
                              for (int i = 2; i <= totalRows_2; i++)
                              {
                                 var his = new ImportHistory_();
                                 his.Company_ID = userlogin.Company_ID;
                                 his.Validate_His = true;
                                 var isempty = true;
                                 var err_Hir = new System.Text.StringBuilder();

                                 for (int j = 1; j <= totalCols_2; j++)
                                 {
                                    var columnName = worksheet_2.Cells[1, j].Value.ToString();
                                    isempty = false;
                                    if (worksheet_2.Cells[i, j].Value != null)
                                    {
                                       if (j == HisImportColumn.Employee_No)
                                       {
                                          var emp_no = "";
                                          emp_no = worksheet_2.Cells[i, j].Value.ToString();
                                          if (chk_Emp_No.Contains(emp_no))
                                          {
                                             his.Employee_No = emp_no;
                                          }
                                          else
                                          {
                                             model.validated = false;
                                             his.Validate_His = false;
                                             err_Hir.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Not_Found_Lower);
                                          }
                                       }
                                       else if (j == HisImportColumn.Employment_Type)
                                       {
                                          var employeeType = employeeTypelst.Where(w => w.Text.ToLower().Trim() == worksheet_2.Cells[i, j].Value.ToString().ToLower().Trim() || w.Desc.ToLower().Trim() == worksheet_2.Cells[i, j].Value.ToString().ToLower().Trim()).FirstOrDefault();
                                          if (employeeType == null)
                                          {
                                             model.validated = false;
                                             his.Validate_His = false;
                                             err_Hir.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                          }
                                          else
                                          {
                                             his.Employee_Type = NumUtil.ParseInteger(employeeType.Value);
                                             his.Employee_Type_ = employeeType.Text;
                                          }
                                       }
                                       else if (j == HisImportColumn.Department)
                                       {
                                          var department = departmentlst.Where(w => w.Text.ToLower().Trim() == worksheet_2.Cells[i, j].Value.ToString().ToLower().Trim()).FirstOrDefault();
                                          if (department == null)
                                          {
                                             model.validated = false;
                                             his.Validate_His = false;
                                             err_Hir.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                          }
                                          else
                                          {
                                             his.Department_ID = NumUtil.ParseInteger(department.Value);
                                             his.Department_ = department.Text;
                                          }
                                       }
                                       else if (j == HisImportColumn.Designation)
                                       {
                                          var desingnation = desingnationlst.Where(w => w.Text.ToLower().Trim() == worksheet_2.Cells[i, j].Value.ToString().ToLower().Trim()).FirstOrDefault();
                                          if (desingnation == null)
                                          {
                                             model.validated = false;
                                             his.Validate_His = false;
                                             err_Hir.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                          }
                                          else
                                          {
                                             his.Designation_ID = NumUtil.ParseInteger(desingnation.Value);
                                             his.Designation_ = desingnation.Text;
                                          }
                                       }
                                       else if (j == HisImportColumn.Branch)
                                       {
                                          var branch = branchlst.Where(w => w.Text.ToLower().Trim() == worksheet_2.Cells[i, j].Value.ToString().ToLower().Trim() || w.Desc.ToLower().Trim() == worksheet_2.Cells[i, j].Value.ToString().ToLower().Trim()).FirstOrDefault();
                                          if (branch == null)
                                          {
                                             //model.validated = false;
                                             //his.Validate_His = false;
                                             //err_Hir.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                          }
                                          else
                                          {
                                             his.Branch_ID = NumUtil.ParseInteger(branch.Value);
                                             his.Branch_ = branch.Text;
                                          }
                                       }
                                       else if (j == HisImportColumn.Currency)
                                       {
                                          var currency = currencylst.Where(w => w.Text.ToLower().Trim() == worksheet_2.Cells[i, j].Value.ToString().ToLower().Trim()).FirstOrDefault();
                                          if (currency == null)
                                          {
                                             model.validated = false;
                                             his.Validate_His = false;
                                             err_Hir.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                          }
                                          else
                                          {
                                             his.Currency_ID = NumUtil.ParseInteger(currency.Value);
                                             his.Currency_ = currency.Text;
                                          }
                                       }
                                       else if (j == HisImportColumn.Monthly_Hourly)
                                       {
                                          var term = termlst.Where(w => w.Text.ToLower().Trim() == worksheet_2.Cells[i, j].Value.ToString().ToLower().Trim() || w.Value.ToLower().Trim() == worksheet_2.Cells[i, j].Value.ToString().ToLower().Trim()).FirstOrDefault();
                                          if (term == null)
                                          {
                                             model.validated = false;
                                             his.Validate_His = false;
                                             err_Hir.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                          }
                                          else
                                          {
                                             his.Basic_Salary_Unit = term.Value;
                                          }
                                       }
                                       else if (j == HisImportColumn.Confirm_Date || j == HisImportColumn.Effective_Date)
                                       {
                                          // Validate date
                                          var strdate = "";
                                          try
                                          {
                                             var date = (DateTime)worksheet_2.Cells[i, j].Value;
                                             strdate = DateUtil.ToDisplayDate(date);
                                          }
                                          catch
                                          {
                                             model.validated = false;
                                             his.Validate_His = false;
                                             strdate = worksheet_2.Cells[i, j].Value.ToString();
                                             err_Hir.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                          }

                                          if (j == HisImportColumn.Confirm_Date)
                                             his.Confirm_Date = strdate;
                                          else if (j == HisImportColumn.Effective_Date)
                                             his.Effective_Date = strdate;

                                       }
                                       else if (j == HisImportColumn.Basic_Salary)
                                       {
                                          decimal decimalsalary = 0;
                                          try
                                          {
                                             decimalsalary = Convert.ToDecimal(worksheet_2.Cells[i, j].Value);
                                          }
                                          catch
                                          {
                                             model.validated = false;
                                             his.Validate_His = false;
                                             decimalsalary = Convert.ToDecimal(worksheet_2.Cells[i, j].Value.ToString());
                                             err_Hir.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                          }
                                          if (j == HisImportColumn.Basic_Salary)
                                             his.Basic_Salary = decimalsalary.ToString("n2");
                                       }
                                    }
                                    else
                                    {
                                       // Validate require
                                       if (j == HisImportColumn.Employee_No
                                           || j == HisImportColumn.Employment_Type
                                           || j == HisImportColumn.Department
                                           || j == HisImportColumn.Designation
                                          //|| j == HisImportColumn.Branch
                                           || j == HisImportColumn.Effective_Date
                                           || j == HisImportColumn.Confirm_Date
                                           || j == HisImportColumn.Currency
                                           || j == HisImportColumn.Basic_Salary
                                           || j == HisImportColumn.Monthly_Hourly
                                           )
                                       {
                                          model.validated = false;
                                          his.Validate_His = false;
                                          err_Hir.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Rrequired_Lower);
                                       }
                                    }
                                 }
                                 if (isempty)
                                 {
                                    model.validated = false;
                                    his.Validate_His = false;
                                    err_Hir.AppendLine(Resource.Message_Empty_Row);
                                 }
                                 his.ErrMsg_His = err_Hir.ToString();
                                 hiss.Add(his);
                              }
                              model.hiss = hiss.ToArray();
                           }
                        }
                        if (!validated_2)
                        {
                           model.hiss = new List<ImportHistory_>().ToArray();
                        }
                     }

                     //--------------------------------Employee_Emergency_Contact Past 3------------------------------------//
                     ExcelWorksheet worksheet_3 = package.Workbook.Worksheets[3];

                     if (worksheet_3.Dimension != null)
                     {
                        bool validated_3 = true;
                        int totalRows_3 = worksheet_3.Dimension.End.Row;
                        int totalCols_3 = worksheet_3.Dimension.End.Column;

                        if (totalCols_3 != 4)
                        {
                           ModelState.AddModelError("emergencys", Resource.Message_Column_Count_Is_Invalid + " " + Resource.Message_Please_Edit_Reupload);
                           model.validated = false;
                           validated_3 = false;
                        }
                        if (totalRows_3 <= 1)
                        {
                           //ModelState.AddModelError("emergencys", "Row count is invalid.  please edit your information and reupload.");
                           //model.validated = false;
                           validated_3 = false;
                        }

                        if (validated_3)
                        {
                           if (totalRows_3 > 1)
                           {
                              var emergencys = new List<ImportEmergencyContact_>();
                              for (int i = 2; i <= totalRows_3; i++)
                              {
                                 var emergency = new ImportEmergencyContact_();
                                 emergency.Company_ID = userlogin.Company_ID;
                                 emergency.Validate_Emergency = true;
                                 var isempty = true;
                                 var err_Emergency = new System.Text.StringBuilder();

                                 for (int j = 1; j <= totalCols_3; j++)
                                 {
                                    var columnName = worksheet_3.Cells[1, j].Value.ToString();
                                    isempty = false;
                                    if (worksheet_3.Cells[i, j].Value != null)
                                    {
                                       if (j == EmergencyImportColumn.Employee_No)
                                       {
                                          var emp_no = "";
                                          emp_no = worksheet_3.Cells[i, j].Value.ToString();
                                          if (chk_Emp_No.Contains(emp_no))
                                          {
                                             emergency.Employee_No = emp_no;
                                          }
                                          else
                                          {
                                             model.validated = false;
                                             emergency.Validate_Emergency = false;
                                             err_Emergency.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Not_Found_Lower);
                                          }
                                       }
                                       else if (j == EmergencyImportColumn.Relationship)
                                       {
                                          var relationship = relationshiplst.Where(w => w.Text.ToLower().Trim() == worksheet_3.Cells[i, j].Value.ToString().ToLower().Trim() || w.Desc.ToLower().Trim() == worksheet_3.Cells[i, j].Value.ToString().ToLower().Trim()).FirstOrDefault();
                                          if (relationship == null)
                                          {
                                             model.validated = false;
                                             emergency.Validate_Emergency = false;
                                             err_Emergency.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                          }
                                          else
                                          {
                                             emergency.Relationship_ID = NumUtil.ParseInteger(relationship.Value);
                                             emergency.Relationship_ = relationship.Text;
                                          }
                                       }
                                       else if (j == EmergencyImportColumn.Name)
                                       {
                                          var strname = "";
                                          strname = worksheet_3.Cells[i, j].Value.ToString();
                                          if (strname.Length > 150)
                                          {
                                             model.validated = false;
                                             emergency.Validate_Emergency = false;
                                             strname = worksheet_3.Cells[i, j].Value.ToString();
                                             err_Emergency.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Message_Maximum_Length + " '150'.");
                                          }

                                          if (j == EmergencyImportColumn.Name)
                                             emergency.Name = strname;

                                       }
                                       else if (j == EmergencyImportColumn.Contact_No)
                                       {
                                          emergency.Contact_No = worksheet_3.Cells[i, j].Value.ToString();
                                          if (emergency.Contact_No.Length > 50)
                                          {
                                             model.validated = false;
                                             emergency.Validate_Emergency = false;
                                             err_Emergency.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Message_Maximum_Length + " '50'.");
                                          }
                                       }
                                    }
                                    else
                                    {
                                       // Validate require
                                       if (j == EmergencyImportColumn.Employee_No
                                           || j == EmergencyImportColumn.Name
                                           || j == EmergencyImportColumn.Contact_No
                                          //|| j == EmergencyImportColumn.Relationship
                                           )
                                       {
                                          model.validated = false;
                                          emergency.Validate_Emergency = false;
                                          err_Emergency.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Rrequired_Lower);
                                       }
                                    }
                                 }
                                 if (isempty)
                                 {
                                    model.validated = false;
                                    emergency.Validate_Emergency = false;
                                    err_Emergency.AppendLine(Resource.Message_Empty_Row);
                                 }
                                 emergency.ErrMsg_Emergency = err_Emergency.ToString();
                                 emergencys.Add(emergency);
                              }
                              model.emergencys = emergencys.ToArray();
                           }
                        }
                        if (!validated_3)
                        {
                           model.emergencys = new List<ImportEmergencyContact_>().ToArray();
                        }

                     }
                     //-------------------------------Relationship Past 4-------------------------------------//
                     ExcelWorksheet worksheet_4 = package.Workbook.Worksheets[4];

                     if (worksheet_4.Dimension != null)
                     {
                        bool validated_4 = true;
                        int totalRows_4 = worksheet_4.Dimension.End.Row;
                        int totalCols_4 = worksheet_4.Dimension.End.Column;

                        if (totalCols_4 != 6)
                        {
                           ModelState.AddModelError("relations", Resource.Message_Column_Count_Is_Invalid + " " + Resource.Message_Please_Edit_Reupload);
                           model.validated = false;
                           validated_4 = false;
                        }
                        if (totalRows_4 <= 1)
                        {
                           //ModelState.AddModelError("relations", "Row count is invalid.  please edit your information and reupload.");
                           //model.validated = false;
                           validated_4 = false;
                        }

                        if (validated_4)
                        {
                           if (totalRows_4 > 1)
                           {
                              var relations = new List<ImportRelationship_>();
                              for (int i = 2; i <= totalRows_4; i++)
                              {
                                 var relation = new ImportRelationship_();
                                 relation.Company_ID = userlogin.Company_ID;
                                 relation.Validate_Relation = true;
                                 var isempty = true;
                                 var err_Relation = new System.Text.StringBuilder();

                                 for (int j = 1; j <= totalCols_4; j++)
                                 {
                                    var columnName = worksheet_4.Cells[1, j].Value.ToString();
                                    isempty = false;
                                    if (worksheet_4.Cells[i, j].Value != null)
                                    {
                                       if (j == RelationImportColumn.Employee_No)
                                       {
                                          var emp_no = "";
                                          emp_no = worksheet_4.Cells[i, j].Value.ToString();
                                          if (chk_Emp_No.Contains(emp_no))
                                          {
                                             relation.Employee_No = emp_no;
                                          }
                                          else
                                          {
                                             model.validated = false;
                                             relation.Validate_Relation = false;
                                             err_Relation.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Not_Found_Lower);
                                          }
                                       }
                                       else if (j == RelationImportColumn.Relationship)
                                       {
                                          var relationship = relationshiplst.Where(w => w.Text.ToLower().Trim() == worksheet_4.Cells[i, j].Value.ToString().ToLower().Trim() || w.Desc.ToLower().Trim() == worksheet_4.Cells[i, j].Value.ToString().ToLower().Trim()).FirstOrDefault();
                                          if (relationship == null)
                                          {
                                             model.validated = false;
                                             relation.Validate_Relation = false;
                                             err_Relation.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                          }
                                          else
                                          {
                                             relation.Relationship_ID = NumUtil.ParseInteger(relationship.Value);
                                             relation.Relationship_ = relationship.Text;
                                          }
                                       }
                                       else if (j == RelationImportColumn.Name)
                                       {
                                          var strname = "";
                                          strname = worksheet_4.Cells[i, j].Value.ToString();
                                          if (strname.Length > 150)
                                          {
                                             model.validated = false;
                                             relation.Validate_Relation = false;
                                             strname = worksheet_4.Cells[i, j].Value.ToString();
                                             err_Relation.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Message_Maximum_Length + " '150'.");
                                          }

                                          if (j == RelationImportColumn.Name)
                                             relation.Name = strname;

                                       }
                                       else if (j == RelationImportColumn.Nationality)
                                       {
                                          var national = nationalitylst.Where(w => w.Text.ToLower().Trim() == worksheet_4.Cells[i, j].Value.ToString().ToLower().Trim() || w.Desc.ToLower().Trim() == worksheet_4.Cells[i, j].Value.ToString().ToLower().Trim()).FirstOrDefault();
                                          if (national == null)
                                          {
                                             model.validated = false;
                                             relation.Validate_Relation = false;
                                             err_Relation.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                          }
                                          else
                                          {
                                             relation.Nationality_ID = NumUtil.ParseInteger(national.Value);
                                             relation.Nationality_ = national.Text;
                                          }
                                       }
                                       else if (j == RelationImportColumn.DOB)
                                       {
                                          var strdate = "";
                                          try
                                          {
                                             var date = (DateTime)worksheet_4.Cells[i, j].Value;
                                             strdate = DateUtil.ToDisplayDate(date);
                                          }
                                          catch
                                          {
                                             model.validated = false;
                                             relation.Validate_Relation = false;
                                             strdate = worksheet_4.Cells[i, j].Value.ToString();
                                             err_Relation.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                          }

                                          if (j == RelationImportColumn.DOB)
                                             relation.DOB = strdate;

                                       }
                                       else if (j == RelationImportColumn.NRIC)
                                       {
                                          relation.NRIC = worksheet_4.Cells[i, j].Value.ToString();
                                          if (relation.NRIC.Length > 30)
                                          {
                                             model.validated = false;
                                             relation.Validate_Relation = false;
                                             err_Relation.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Message_Maximum_Length + " '30'.");
                                          }
                                       }
                                    }
                                    else
                                    {
                                       // Validate require
                                       if (j == RelationImportColumn.Employee_No
                                           || j == RelationImportColumn.Name
                                           || j == RelationImportColumn.Nationality
                                          //|| j == RelationImportColumn.Relationship
                                           || j == RelationImportColumn.DOB
                                          //|| j == RelationImportColumn.NRIC
                                           )
                                       {
                                          model.validated = false;
                                          relation.Validate_Relation = false;
                                          err_Relation.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Rrequired_Lower);
                                       }
                                    }
                                 }

                                 if (isempty)
                                 {
                                    model.validated = false;
                                    relation.Validate_Relation = false;
                                    err_Relation.AppendLine(Resource.Message_Empty_Row);
                                 }
                                 relation.ErrMsg_Relation = err_Relation.ToString();
                                 relations.Add(relation);
                              }
                              model.relations = relations.ToArray();
                           }
                        }
                        if (!validated_4)
                        {
                           model.relations = new List<ImportRelationship_>().ToArray();
                        }
                     }
                  }
               }
               catch
               {
                  ModelState.AddModelError("Import_Employee", Resource.Message_Cannot_Found_Excel_Sheet + " " + Resource.Message_Please_Edit_Reupload);
                  model.validated = false;
                  model.emps = new List<ImportEmployeeProfile_>().ToArray();
                  model.hiss = new List<ImportHistory_>().ToArray();
                  model.emergencys = new List<ImportEmergencyContact_>().ToArray();
                  model.relations = new List<ImportRelationship_>().ToArray();
               }
               //var erjrors = GetErrorModelState();
            }
         }
         return View(model);
      }

      public ActionResult AddNewAction(int pIndex)
      {
         var model = new EmployeeActionViewModels() { Index = pIndex };
         return PartialView("EmployeeProfileActionRow", model);
      }

      public ActionResult EmployeeReport(ServiceResult result, EmployeeReportModel model, string tabAction = "")
      {

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //-------rights------------
         RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A);
         if (rightResult.action != null)
            return rightResult.action;
         model.rights = rightResult.rights;

         var currentdate = StoredProcedure.GetCurrentDate();
         var cbService = new ComboService();
         var EmpService = new EmployeeService();
         var deparService = new DepartmentService();
         var comService = new CompanyService();

         //Apply filter
         var dup = new List<Employee_Profile>();
         var criteria = new EmployeeCriteria() { Company_ID = userlogin.Company_ID, Department_ID = model.Department, Date_From = model.sFrom, Date_To = model.sTo };
         var pResult = EmpService.LstEmployeeProfile(criteria);
         if (pResult.Object != null) dup = (List<Employee_Profile>)pResult.Object;
         model.employeeList = dup;

         model.departmentList = cbService.LstDepartment(userlogin.Company_ID, hasBlank: true);
         model.residentialStatusList = cbService.LstResidentialStatus();
         model.nationalityList = cbService.LstNationality(false);

         if (tabAction == "export")
         {
            string csv = "";
            string dep_name = "";

            if (model.Department.HasValue)
            {
               var department = deparService.GetDepartment(model.Department.Value);
               if (department != null) dep_name = department.Name;
            }

            var fullName = UserSession.GetUserName(userlogin);

            //HEADER                
            string compname = comService.GetCompany(userlogin.Company_ID).Name;
            csv += "<table><tr valign='top'><td valign='top'><b> " + compname + " </b></td><td>&nbsp;</td><td><b> " + Resource.Employee_Report + " </b></td></tr>";
            csv += "<tr><td>&nbsp;</td><td>&nbsp;</td><td><b> " + Resource.Department + " </b> " + dep_name + "</td></tr>";
            csv += "<tr><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td></tr>";
            csv += "<tr><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td></tr></table>";

            csv += "<table border=1><tr><td><b>" + Resource.Employee_No + "</b></td> ";
            csv += "<td><b>" + Resource.Employee_Name + "</b></td> ";
            csv += "<td><b>" + Resource.DOB + "</b></td> ";
            csv += "<td><b>" + Resource.Hired_Date + "</b></td> ";
            csv += "<td><b>" + Resource.Nationality + "</b></td> ";
            csv += "<td><b>" + Resource.Permit + "</b></td> ";
            csv += "<td><b>" + Resource.Permit_No + "</b></td> ";
            csv += "<td><b>" + Resource.Permit_Expiry_Date + "</b></td> ";
            csv += "<td><b>" + Resource.Address + "</b></td> ";
            csv += "<td><b>" + Resource.Status + "</b></td></tr>";

            if (model.employeeList != null && model.employeeList.Count > 0)
            {
               //var i = 0;
               foreach (var row in model.employeeList)
               {

                  Global_Lookup_Data emptype = null;

                  Employment_History emphist = row.Employment_History.Where(w => w.Effective_Date <= currentdate).OrderByDescending(o => o.Effective_Date).FirstOrDefault();
                  if (emphist != null)
                  {
                     emptype = new ComboService().GetLookup(emphist.Employee_Type);
                     if (model.Department.HasValue && emphist.Department_ID != model.Department)
                     {
                        continue;
                     }
                     //if (Model.Department.HasValue && emphist.Branch_ID != Model.Department)
                     //{
                     //    continue;
                     //}
                  }

                  String nationality = "";
                  if (row != null && row.Nationality_ID != null)
                  {
                     nationality = model.nationalityList.Where(w => w.Value == row.Nationality_ID.ToString()).FirstOrDefault().Text;
                  }

                  String Residential_Status = "";
                  if (row != null && row.Residential_Status != null)
                  {
                     Residential_Status = model.residentialStatusList.Where(w => w.Value == row.Residential_Status.ToString()).FirstOrDefault().Text;
                  }

                  csv += "<tr  valign='top'><td>" + @row.Employee_No + "</td> ";
                  csv += "<td>" + UserSession.GetUserName(@row.User_Profile) + "</td> ";
                  csv += "<td>" + DateUtil.ToDisplayDate(@row.DOB) + "</td> ";
                  csv += "<td>" + DateUtil.ToDisplayDate(@row.Hired_Date) + "</td> ";
                  csv += "<td>" + nationality + "</td> ";
                  csv += "<td>" + Residential_Status + "</td> ";
                  csv += "<td>" + @row.PR_No + "</td> ";
                  csv += "<td>" + DateUtil.ToDisplayDate(@row.PR_End_Date) + "</td> ";
                  csv += "<td>" + @row.Residential_Address_1 + "  " + @row.Postal_Code_1 + "</td> ";
                  csv += "<td>" + StatusUtil.Get_Record_Status(@row.User_Profile.User_Status) + "</td> </tr>";

               }
            }

            csv += "</table>";
            csv += "<table><tr><td>&nbsp;</td></tr>";
            csv += "<tr><td><b>" + Resource.Printed_By + "</b> " + fullName + "</td></tr></table>";

            System.Web.UI.WebControls.GridView gv = new System.Web.UI.WebControls.GridView();
            //gv.DataSource = emp;
            gv.DataBind();
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.Buffer = false;
            Response.AddHeader("content-disposition", "attachment; filename=EmployeeReport.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            Response.Write("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
            StringWriter sw = new StringWriter();
            sw.Write(csv);
            System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.End();

            //return RedirectToAction("EmployeeReport");
         }

         return View(model);
      }
      #endregion

      #region CKEditor
      [AcceptVerbs(HttpVerbs.Post)]
      public ActionResult upload(HttpPostedFileBase upload, string CKEditorFuncNum, string CKEditor, string langCode)
      {

         if (upload.ContentLength <= 0)
            return null;

         const string uploadFolder = "uploadImages";

         var fileName = Path.GetFileName(upload.FileName);
         var path = Path.Combine(Server.MapPath(string.Format("~/{0}", uploadFolder)), fileName);
         upload.SaveAs(path);

         var url = string.Format("{0}{1}/{2}/{3}", Request.Url.GetLeftPart(UriPartial.Authority),
             Request.ApplicationPath == "/" ? string.Empty : Request.ApplicationPath,
             uploadFolder, fileName);

         // passing message success/failure
         const string message = "Image was saved correctly";

         // since it is an ajax request it requires this string
         var output = string.Format(
             "<html><body><script>window.parent.CKEDITOR.tools.callFunction({0}, \"{1}\", \"{2}\");</script></body></html>",
             CKEditorFuncNum, url, message);

         return Content(output);
      }

      public ActionResult ImageViewer()
      {
         return View();
      }

      public ActionResult browse()
      {
         return View();
      }

      public void Thumb(string img)
      {
         if (img == null && img.Length == 0)
         {
            Response.End();
         }
         var image = Path.Combine(Server.MapPath("~/uploadImages/"), img);
         var thumbWidth = 128D;
         var buffer = Thumbmail.GenerateTumbnail(image, thumbWidth);
         Response.ContentType = string.Format("image/{0}", Path.GetExtension(image).Trim(new[] { '.' }));
         Response.OutputStream.Write(buffer, 0, buffer.Length);
         Response.End();
      }
      #endregion

      ////Add Jane 26-04-2016
      //public ActionResult AssignUser(int? pSubcID, int pProfileID)
      //{
      //    var userlogin = UserSession.getUser(HttpContext);
      //    if(userlogin == null)
      //         return Json(null, JsonRequestBehavior.AllowGet);

      //    var comService = new CompanyService();
      //    var users = new[] {pProfileID};
      //   var result = comService.UpdateUserAssign(pSubcID, users);
      //   if (result.Code == ERROR_CODE.SUCCESS)
      //   {
      //       return Json(new { result  }, JsonRequestBehavior.AllowGet); 
      //   }
      //    return Json(null, JsonRequestBehavior.AllowGet);
      //}
      //public ActionResult UploadAttach(Nullable<int> pEmpID, Nullable<int> pAttachType, string pRenameFile)
      //{

      //    HttpPostedFileBase file = Request.Files[0];

      //    var fname = file.FileName;
      //    var type = Path.GetExtension(fname);

      //    if (!string.IsNullOrEmpty(pRenameFile))
      //    {
      //        fname = pRenameFile + type;
      //    }
      //    int fileSizeInBytes = file.ContentLength;
      //    MemoryStream target = new MemoryStream();
      //    file.InputStream.CopyTo(target);

      //    byte[] data = target.ToArray();

      //    var userlogin = UserSession.getUser(HttpContext);
      //    var currentdate = StoredProcedure.GetCurrentDate();


      //    if (pEmpID.HasValue && pEmpID.Value > 0)
      //    {
      //        var attach = new Employee_Attachment()
      //        {
      //            Employee_Profile_ID = pEmpID,
      //            Attachment_File = data,
      //            Attachment_Type = pAttachType,
      //            File_Name = fname,
      //            Attachment_ID = Guid.NewGuid(),
      //            Uploaded_by = userlogin.User_Authentication.Email_Address,
      //            Uploaded_On = currentdate
      //        };

      //        var eService = new EmployeeService();
      //        var result = eService.InsertAttachment(attach);
      //        if (result > 0)
      //        {
      //            var cbService = new ComboService();

      //            var attachType = cbService.GetLookup(pAttachType);

      //            return Json(new
      //            {
      //                filename = fname,
      //                attachType = (attachType != null ? attachType.Name : ""),
      //                uploadedOn = DateUtil.ToDisplayDate(currentdate),
      //                uploadedBy = userlogin.User_Authentication.Email_Address,
      //                attID = attach.Attachment_ID
      //            }, JsonRequestBehavior.AllowGet);
      //        }
      //    }



      //    return Json(JsonRequestBehavior.AllowGet);

      //}


      //Added by sun 30-11-2015

   }
}
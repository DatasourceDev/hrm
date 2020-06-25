using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Authentication.Models;
using Authentication.Common;
using SBSModel.Models;
using SBSModel.Common;
using SBSResourceAPI;
using SBSWorkFlowAPI.Constants;

namespace Authentication.Controllers
{
   //Added By sun
   [Authorize]
   public class UserController : ControllerBase
   {
      [HttpGet]
      [AllowAuthorized]
      public ActionResult User(ServiceResult result, UserViewModel model, string operation, string apply, int[] Profile_IDs = null)
      {
         User_Profile userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         model.operation = EncryptUtil.Decrypt(operation);

         //Validate Page Right
         RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A);
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;
         model.result = result;

         //-------data------------   
         var userService = new UserService();
         var cbService = new ComboService();
         var comService = new CompanyService();
         var companyID = userlogin.Company_ID;
         var com = comService.GetCompany(companyID);

         model.Company_ID = companyID;
         model.statusList = cbService.LstStatus();

         if (com.Company_Level == Companylevel.Mainmaster)
         {
            //model.UserList = userService.getUsers(null, model.search_val, model.Record_Status);
            model.UserList = userService.getUsers(null, null, model.Record_Status);
         }
         else if (com.Company_Level == Companylevel.Franchise || com.Company_Level == Companylevel.Whitelabel)
         {
            //model.UserList = userService.getUsersBelongTocompany(companyID.Value, model.search_val, model.Record_Status);
            model.UserList = userService.getUsersBelongTocompany(companyID.Value, null, model.Record_Status);
         }
         else
         {
            //model.UserList = userService.getUsers(companyID.Value, model.search_val, model.Record_Status);
            model.UserList = userService.getUsers(companyID.Value, null, model.Record_Status);
         }

         model.CompanyList = comService.LstCompany(null);

         if (Profile_IDs != null)
         {
            if (apply == RecordStatus.Active | apply == RecordStatus.Inactive)
            {
               model.result = userService.UpdateUserStatus(Profile_IDs, apply, userlogin.User_Authentication.Email_Address);
               return RedirectToAction("User", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field });
            }

            if (apply == UserSession.RIGHT_D)
            {
               //added by sun 14-10-2015
               apply = RecordStatus.Delete;
               model.result = userService.UpdateMultipleDeleteUserProfileStatus(Profile_IDs, apply, userlogin.User_Authentication.Email_Address);
               //model.result = userService.DeleteUserProfile(Profile_IDs);
               return RedirectToAction("User", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field });
            }
         }

         return View(model);
      }

      [HttpGet]
      [AllowAuthorized]
      public ActionResult UserInfo(ServiceResult result, string pProID, string operation)
      {

         User_Profile userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var model = new UserViewModel();
         var profileID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pProID));
         model.operation = EncryptUtil.Decrypt(operation);
         model.pageAction = "main";

         //Validate Page Right
         RightResult rightResult = base.validatePageRight(model.operation, "/User/User");
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;
         model.result = result;

         //-------data------------
         var comService = new CompanyService();
         var userService = new UserService();
         var cbService = new ComboService();
         var empService = new EmployeeService();


         model.statusList = cbService.LstStatus();

         int[] moduleDetailsID = null;

         if (model.operation == UserSession.RIGHT_C)
         {
            model.Company_ID = userlogin.Company_ID;
            model.SubscriptionList = comService.LstSubscription(userlogin.Company_ID);
            if (model.SubscriptionList != null)
            {
               moduleDetailsID = model.SubscriptionList.Select(s => s.Module_Detail_ID.Value).ToArray();
               model.UserRoleList = empService.LstUserRole(moduleDetailsID);
            }
            var com = comService.GetCompany(model.Company_ID);
            if (com != null)
               model.Company_Level = com.Company_Level;

            model.Is_Email = true;
         }
         else if (model.operation == UserSession.RIGHT_U)
         {
            var user = userService.getUser(profileID);

            if (user != null)
            {
               model.SubscriptionList = comService.LstSubscription(user.Company_ID);
               if (model.SubscriptionList != null)
               {
                  moduleDetailsID = model.SubscriptionList.Select(s => s.Module_Detail_ID.Value).ToArray();
                  model.UserRoleList = empService.LstUserRole(moduleDetailsID);
               }
               model.Company_ID = user.Company_ID;
               model.First_Name = user.First_Name;
               model.Last_Name = user.Last_Name;
               model.Middle_Name = user.Middle_Name;
               model.Phone = user.Phone;

               //Added by sun 09-09-2016
               model.Is_Email = user.User_Authentication.Is_Email.HasValue ? user.User_Authentication.Is_Email.Value : true;
               model.User_Name = user.User_Authentication.User_Name;
               model.Email = user.User_Authentication.Email_Address;

               model.User_Name = user.User_Name;
               model.User_Status = user.User_Status;
               model.Profile_ID = user.Profile_ID;
               model.Activated = user.User_Authentication.Activated;
               model.User_Assign_Module = empService.LstUserAssignModule(user.Profile_ID).Select(s => s.Subscription_ID.Value).ToArray();
               model.Users_Assign_Role = userService.LstUserAssignRole(user.User_Authentication_ID).Select(s => s.User_Role_ID.Value).ToArray();
               model.User_Authentication_ID = user.User_Authentication.User_Authentication_ID;
               var com = comService.GetCompany(user.Company_ID);
               if (com != null)
                  model.Company_Level = com.Company_Level;
            }
         }

         else if (model.operation == UserSession.RIGHT_D)
         {
            //added by sun 14-10-2015
            var apply = RecordStatus.Delete;
            model.result = userService.UpdateDeleteUserProfileStatus(profileID, apply, userlogin.User_Authentication.Email_Address);
            //model.result = userService.DeleteUserProfile(profileID);
            return RedirectToAction("User", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field });
         }

         return View(model);
      }

      [HttpPost]
      [AllowAuthorized]
      public ActionResult UserInfo(UserViewModel model, int[] Users_Assign_Role, int[] User_Assign_Module, string pageAction = "")
      {

         User_Profile userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //Validate Page Right
         RightResult rightResult = base.validatePageRight(model.operation, "/User/User");
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;

         //-------data------------
         var empService = new EmployeeService();   
         var userService = new UserService();
         var comService = new CompanyService();   
         var cbService = new ComboService();
         var currentdate = StoredProcedure.GetCurrentDate();

         var setProfile = new User_Profile();
         var emp = new Employee_Profile();

         model.pageAction = pageAction;
         model.Users_Assign_Role = Users_Assign_Role;
         model.User_Assign_Module = User_Assign_Module;

         //Added by sun 09-09-2016
         if (model.Is_Email)
         {
            if (string.IsNullOrEmpty(model.Email))
            {
               ModelState.AddModelError("Email", Resource.Message_Is_Required);
            }
            else
            {
               var criteria = new UserCriteria() { Email = model.Email };
               var dupUser = userService.LstUserProfile(criteria).FirstOrDefault();
               if (dupUser != null)
               {
                  if (model.operation == Operation.A)
                     ModelState.AddModelError("Email", Resource.Message_Is_Duplicated);
                  else if (model.operation == Operation.U)
                  {
                     if (dupUser.Profile_ID != model.Profile_ID)
                        ModelState.AddModelError("Email", Resource.Message_Is_Duplicated);
                  }
               }
            }
         }
         else
         {
            if (string.IsNullOrEmpty(model.User_Name))
            {
               ModelState.AddModelError("User_Name", Resource.Message_Is_Required);
            }
            else
            {
               var criteria = new UserCriteria() { User_Name = model.User_Name };
               var dupUser = userService.LstUserProfile(criteria).FirstOrDefault();
               if (dupUser != null)
               {
                  if (model.operation == Operation.A)
                     ModelState.AddModelError("User_Name", Resource.Message_Is_Duplicated);
                  else if (model.operation == Operation.U)
                  {
                     if (dupUser.Profile_ID != model.Profile_ID)
                        ModelState.AddModelError("User_Name", Resource.Message_Is_Duplicated);
                  }
               }
            }
         }

         if (ModelState.IsValid)
         {
            if (pageAction == "saveAdd")
            {
               model.pageAction = "main";
               setProfile.Company_ID = userlogin.Company_ID;
               setProfile.First_Name = model.First_Name;
               setProfile.Last_Name = model.Last_Name;
               setProfile.Middle_Name = model.Middle_Name;
               setProfile.Phone = model.Phone;

               //Added by sun 09-09-2016
               setProfile.Is_Email = model.Is_Email;
               if (model.Is_Email)
                  setProfile.Email = model.Email;
               else
                  setProfile.User_Name = model.User_Name;

               setProfile.User_Status = model.User_Status;
               setProfile.Create_On = currentdate;
               setProfile.Create_By = userlogin.User_Authentication.Email_Address;
               emp.Mobile_No = model.Phone;
               emp.Create_On = currentdate;
               emp.Create_By = userlogin.User_Authentication.Email_Address;
               emp.Update_On = currentdate;
               emp.Update_By = userlogin.User_Authentication.Email_Address;

               model.result = userService.InsertUserPro(setProfile, model.Users_Assign_Role, model.User_Assign_Module, emp);
               if (model.result.Code == ERROR_CODE.SUCCESS)
               {
                  if (model.result.Object != null)
                  {
                      var moduleName = string.Empty;
                      if (AppSetting.IsLive == "true")
                          moduleName = ModuleDomain.HR;

                      if (AppSetting.IsStaging == "true")
                          moduleName = "HRSBS2-staging";

                      var domain = UrlUtil.GetDomain(Request.Url, moduleName);
                     string activateCode = model.result.Object.ToString();
                     var com = comService.GetCompany(userlogin.Company_ID);
                     var eResult = EmailTemplete.sendUserActivateEmail(model.Email, activateCode, UserSession.GetUserName(setProfile), com.Name, com.Phone, com.Email, domain);
                     if (eResult == false)
                     {
                        model.result.Code = ERROR_CODE.ERROR_501_CANT_SEND_EMAIL;
                     }
                  }
                  EmployeeProcessApproval(setProfile.Profile_ID);
                  return RedirectToAction("User", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field });
               }
            }
            else if (pageAction == "saveEdit")
            {
               model.pageAction = "main";

               var gEmp = empService.GetEmployeeProfileByUserProfile(model.Profile_ID);
               if (gEmp == null)
                  return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

               setProfile.First_Name = model.First_Name;
               setProfile.Last_Name = model.Last_Name;
               setProfile.Middle_Name = model.Middle_Name;
               setProfile.Phone = model.Phone;

               //Added by sun 09-09-2016
               setProfile.Is_Email = model.Is_Email;
               if (model.Is_Email)
                  setProfile.Email = model.Email;
               else
                  setProfile.User_Name = model.User_Name;

               setProfile.User_Status = model.User_Status;
               setProfile.Update_By = userlogin.User_Authentication.Email_Address;
               setProfile.Profile_ID = NumUtil.ParseInteger(model.Profile_ID.Value);
               gEmp.Mobile_No = model.Phone;
               gEmp.Create_On = gEmp.Create_On;
               gEmp.Create_By = gEmp.Create_By;
               gEmp.Update_On = currentdate;
               gEmp.Update_By = userlogin.User_Authentication.Email_Address;

               model.result = userService.UpdateUserPro(setProfile, model.Users_Assign_Role, model.User_Assign_Module, gEmp);
               if (model.result.Code == ERROR_CODE.SUCCESS)
               {
                  return RedirectToAction("User", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field });
               }
            }
         }

         model.pageAction = "main";
         model.statusList = cbService.LstStatus();
         model.SubscriptionList = comService.LstSubscription(userlogin.Company_ID);

         int[] moduleDetailsID = null;
         if (model.SubscriptionList != null)
         {
            moduleDetailsID = model.SubscriptionList.Select(s => s.Module_Detail_ID.Value).ToArray();
         }
         model.UserRoleList = empService.LstUserRole(moduleDetailsID);
         model.User_Assign_Module = userService.LstUserAssignModule(model.Profile_ID).Select(s => s.Subscription_ID.Value).ToArray();

         return View(model);
      }

      private void EmployeeProcessApproval(Nullable<int> pProfileID)
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
   }
}

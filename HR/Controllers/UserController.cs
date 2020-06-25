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
using SBSWorkFlowAPI.Constants;

namespace HR.Controllers
{
   //Added By sun
   [Authorize]
   public class UserController : ControllerBase
   {
      [HttpGet]
      [AllowAuthorized]
      public ActionResult User(ServiceResult result, UserViewModel model, string operation, string apply, int[] Profile_IDs = null, int pno = 1, int plen = 50)
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
         if (com == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         model.Company_ID = companyID;
         model.Company_Level = com.Company_Level;
         model.statusList = cbService.LstStatus();
         if (Profile_IDs != null)
         {
            if (apply == RecordStatus.Active | apply == RecordStatus.Inactive)
            {
               model.result = userService.UpdateUserStatus(Profile_IDs, apply, userlogin.User_Authentication.Email_Address);
               return RedirectToAction("User", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field });
            }
            else if (apply == UserSession.RIGHT_D)
            {
               apply = RecordStatus.Delete;
               model.result = userService.UpdateMultipleDeleteUserProfileStatus(Profile_IDs, apply, userlogin.User_Authentication.Email_Address);
               return RedirectToAction("User", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field });
            }
         }
         else
         {
            var criteria = new UserCriteria();
            criteria.Page_Size = plen;
            criteria.Page_No = pno;
            criteria.Record_Status = model.Record_Status;
            criteria.Text_Search = model.search_val;
            if (com.Company_Level == Companylevel.Mainmaster)
            {
               model.companylst = cbService.LstCompany(true);
               criteria.Company_ID = model.Search_Company_ID;
            }
            else if (com.Company_Level == Companylevel.Franchise || com.Company_Level == Companylevel.Whitelabel)
            {
               var Users = userService.getUsersBelongTocompany(companyID.Value, null, model.Record_Status);
               criteria.Company_ID = companyID.Value;
               criteria.Is_Belong_To = true;
               criteria.Company_ID = model.Search_Company_ID;
               model.companylst = cbService.LstCompanyBelongTo(com.Company_ID, true);
            }
            else
            {
               criteria.Company_ID = companyID.Value;
            }
            var presult = userService.LstUser(criteria);
            if (presult.Object != null)
               model.UserList = (List<User_Profile>)presult.Object;

            model.Record_Count = presult.Record_Count;
            model.Page_No = pno;
            model.Page_Length = plen;
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
         model.Profile_ID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pProID));
         model.operation = EncryptUtil.Decrypt(operation);

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
         if (model.operation == Operation.C)
         {
            model.Is_Email = true;
            model.Company_ID = userlogin.Company_ID;
            model.SubscriptionList = comService.LstSubscription(userlogin.Company_ID);
            if (model.SubscriptionList != null)
            {
               moduleDetailsID = model.SubscriptionList.Select(s => s.Module_Detail_ID.Value).Distinct().ToArray();
               model.UserRoleList = empService.LstUserRole(moduleDetailsID);
            }
            var com = comService.GetCompany(model.Company_ID);
            if (com != null)
               model.Company_Level = com.Company_Level;
         }
         else if (model.operation == Operation.U)
         {
            var user = userService.getUser(model.Profile_ID);
            if (user != null)
            {
               model.Company_ID = user.Company_ID;
               model.SubscriptionList = comService.LstSubscription(user.Company_ID);
               if (model.SubscriptionList != null)
               {
                  moduleDetailsID = model.SubscriptionList.Select(s => s.Module_Detail_ID.Value).Distinct().ToArray();
                  model.UserRoleList = empService.LstUserRole(moduleDetailsID);
               }
               model.First_Name = user.First_Name;
               model.Last_Name = user.Last_Name;
               model.Middle_Name = user.Middle_Name;
               model.Phone = user.Phone;
               model.User_Status = user.User_Status;
               model.Is_Email = user.User_Authentication.Is_Email.HasValue ? user.User_Authentication.Is_Email.Value : true;
               model.Email = user.User_Authentication.Email_Address;
               model.User_Name = user.User_Authentication.User_Name;
               model.Activated = user.User_Authentication.Activated;

               model.User_Assign_Module = empService.LstUserAssignModule(user.Profile_ID).Select(s => s.Subscription_ID.Value).ToArray();
               model.Users_Assign_Role = userService.LstUserAssignRole(user.User_Authentication_ID).Select(s => s.User_Role_ID.Value).ToArray();
               model.User_Authentication_ID = user.User_Authentication.User_Authentication_ID;
               var com = comService.GetCompany(user.Company_ID);
               if (com != null)
                  model.Company_Level = com.Company_Level;
            }
         }
         else if (model.operation == Operation.D)
         {
            var apply = RecordStatus.Delete;
            model.result = userService.UpdateDeleteUserProfileStatus(model.Profile_ID, apply, userlogin.User_Authentication.Email_Address);
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

         model.pageAction = pageAction;
         model.Users_Assign_Role = Users_Assign_Role;
         model.User_Assign_Module = User_Assign_Module;

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
                  var dupUser = userService.LstUserProfile(criteria).FirstOrDefault();
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

         if (model.Users_Assign_Role == null)
            ModelState.AddModelError("Users_Assign_Role", Resource.Message_Is_Required);

         if (ModelState.IsValid)
         {
            var user = new User_Profile();
            var emp = new Employee_Profile();

            if (model.operation == Operation.U)
            {
               emp = empService.GetEmployeeProfileByUserProfile(model.Profile_ID);
               if (emp == null)
                  return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

               user = userService.getUser(model.Profile_ID);
               if (user == null)
                  return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);
            }

            user.First_Name = model.First_Name;
            user.Last_Name = model.Last_Name;
            user.Middle_Name = model.Middle_Name;
            user.Phone = model.Phone;

            user.Is_Email = model.Is_Email;
            if (model.Is_Email)
               user.Email = model.Email;
            else
               user.User_Name = model.User_Name;

            user.User_Status = model.User_Status;
            user.Update_On = currentdate;
            user.Update_By = userlogin.User_Authentication.Email_Address;

            emp.Mobile_No = model.Phone;
            emp.Update_On = currentdate;
            emp.Update_By = userlogin.User_Authentication.Email_Address;

            if (model.operation == Operation.C)
            {
               user.Company_ID = userlogin.Company_ID;
               user.Create_On = currentdate;
               user.Create_By = userlogin.User_Authentication.Email_Address;
               emp.Create_On = currentdate;
               emp.Create_By = userlogin.User_Authentication.Email_Address;
               model.result = userService.InsertUserPro(user, model.Users_Assign_Role, model.User_Assign_Module, emp);
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
                     var eResult = EmailTemplete.sendUserActivateEmail(model.Email, activateCode, UserSession.GetUserName(user), com.Name, com.Phone, com.Email, domain);
                     if (eResult == false)
                     {
                        model.result.Code = ERROR_CODE.ERROR_501_CANT_SEND_EMAIL;
                     }
                  }
                  EmployeeProcessApproval(user.Profile_ID);
                  return RedirectToAction("User", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field });
               }
            }
            else if (model.operation == Operation.U)
            {
               model.result = userService.UpdateUserPro(user, model.Users_Assign_Role, model.User_Assign_Module, emp);
               if (model.result.Code == ERROR_CODE.SUCCESS)
               {
                  return RedirectToAction("User", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field });
               }
            }
         }

         model.statusList = cbService.LstStatus();
         model.SubscriptionList = comService.LstSubscription(model.Company_ID);
         int[] moduleDetailsID = null;
         if (model.SubscriptionList != null)
         {
            moduleDetailsID = model.SubscriptionList.Select(s => s.Module_Detail_ID.Value).Distinct().ToArray();
            model.UserRoleList = empService.LstUserRole(moduleDetailsID);
         }
         model.User_Assign_Module = userService.LstUserAssignModule(model.Profile_ID).Select(s => s.Subscription_ID.Value).ToArray();

         return View(model);
      }
   }
}

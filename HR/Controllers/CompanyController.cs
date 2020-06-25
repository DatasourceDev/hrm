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
using System.Web.Routing;


namespace HR.Controllers
{
   [Authorize]
   public class CompanyController : ControllerBase
   {

      [HttpGet]
      [AllowAuthorized]
      public ActionResult Company(int[] companies, ServiceResult result, CompanyViewModel model, string operation, string apply)
      {
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         model.operation = EncryptUtil.Decrypt(operation);

         //Validate Page Right
         RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A);
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;
         model.result = result;

         var currentdate = StoredProcedure.GetCurrentDate();
         var comService = new CompanyService();
         var cbService = new ComboService();
         var comget = comService.GetCompany(userlogin.Company_ID);
         model.Company_Levelg = comget.Company_Level;


         if (companies != null)
         {
            if (apply == RecordStatus.Active | apply == RecordStatus.Inactive)
            {
               foreach (var comID in companies)
               {
                  var com = comService.GetCompany(comID);
                  if (com == null)
                     return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);
                  // com.Company_Status = "Inactive";
                  //Added by sun 13-10-2015
                  com.Company_Status = apply;
                  com.Update_On = currentdate;
                  com.Update_By = userlogin.User_Authentication.Email_Address;
                  model.result = comService.UpdateCompany(com);
               }

               if (model.result.Code == ERROR_CODE.SUCCESS)
               {
                  return RedirectToAction("Company", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field });
               }
            }
         }

         model.CompanyList = comService.LstCompany(userlogin.Company_ID, model.search_Country, model.search_Registration_Date, model.search_Status);
         model.countryList = cbService.LstCountry(true);
         //model.countryLevel = cbService.LstCompanylevel(true);
         model.statusList = new List<ComboViewModel>();
         model.Belong_To = userlogin.Company_ID;

         return View(model);
      }

      [HttpGet]
      [AllowAuthorized]
      public ActionResult CompanyInfo(ServiceResult result, string pComID, string operation)
      {
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var model = new CompanyInfoViewModel();
         if (string.IsNullOrEmpty(pComID))
            pComID = EncryptUtil.Encrypt(userlogin.Company_ID);

         var comID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pComID));
         model.operation = EncryptUtil.Decrypt(operation);

         if (string.IsNullOrEmpty(model.operation))
            model.operation = Operation.U;

         //Validate Page Right
         RightResult rightResult = base.validatePageRight(model.operation);
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;
         model.result = result;

         var userService = new UserService();
         var comService = new CompanyService();
         var cbService = new ComboService();

         model.countryList = cbService.LstCountry(true);
         model.stateList = new List<ComboViewModel>();
         model.stateBillingList = new List<ComboViewModel>();

         var comget = comService.GetCompany(userlogin.Company_ID);
         model.Company_Levelg = comget.Company_Level;

         var users = userService.getUsers(comID);
         model.User_Count = users.Count();

         if (model.operation == UserSession.RIGHT_U)
         {
            var com = comService.GetCompany(comID);
            if (com != null)
            {
               model.LstCompanylevel = cbService.LstCompanylevel(com.Company_Level, true);
               model.Company_ID = com.Company_ID;
               model.No_Of_Employees = com.No_Of_Employees;
               model.Company_Name = com.Name;
               model.Effective_Date = DateUtil.ToDisplayDate(com.Effective_Date);
               model.Address = com.Address;
               model.Country_ID = com.Country_ID;
               model.State_ID = com.State_ID;
               model.Zip_Code = com.Zip_Code;
               model.Billing_Address = com.Billing_Address;
               model.Billing_Country_ID = com.Billing_Country_ID;
               model.Billing_State_ID = com.Billing_State_ID;
               model.Billing_Zip_Code = com.Billing_Zip_Code;
               model.CPF_Submission_No = com.CPF_Submission_No;
               model.patUser_ID = com.patUser_ID;
               model.patPassword = com.patPassword;
               model.Company_Level = com.Company_Level;
               model.Fax = com.Fax;
               model.Phone = com.Phone;

               var logo = comService.GetLogo(com.Company_ID);
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

               model.SubscriptionList = comService.LstSubscription(model.Company_ID);
            }
         }
         return View(model);
      }

      [HttpPost]
      [AllowAuthorized]
      public ActionResult CompanyInfo(CompanyInfoViewModel model)
      {
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //Validate Page Right
         RightResult rightResult = base.validatePageRight(model.operation);
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;

         var currentdate = StoredProcedure.GetCurrentDate();
         var userService = new UserService();
         var comService = new CompanyService();
         var cbService = new ComboService();

         if (ModelState.IsValid)
         {
            if (model.operation == UserSession.RIGHT_U)
            {
               var com = comService.GetCompany(model.Company_ID);
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
               com.CPF_Submission_No = model.CPF_Submission_No;
               com.patUser_ID = model.patUser_ID;
               com.patPassword = model.patPassword;
               com.Fax = model.Fax;
               com.Phone = model.Phone;
               com.Create_On = com.Create_On;
               com.Create_By = com.Create_By;
               com.Update_By = userlogin.User_Authentication.Email_Address;
               com.Update_On = currentdate;

               model.result = comService.UpdateCompany(com);
               if (model.result.Code == ERROR_CODE.SUCCESS)
               {
                  var comlogin = comService.GetCompany(userlogin.Company_ID);
                  if (comlogin.Company_Level == Companylevel.EndUser)
                  {

                  }
                  else
                     return RedirectToAction("Company", new RouteValueDictionary(model.result));
               }
            }
         }

         var logo = comService.GetLogo(model.Company_ID);
         if (logo != null)
         {
            model.Company_Logo_ID = logo.Company_Logo_ID;
            model.Company_Logo = logo.Logo;
         }

         model.countryList = cbService.LstCountry(true);
         model.stateList = new List<ComboViewModel>();
         model.stateBillingList = new List<ComboViewModel>();
         model.SubscriptionList = comService.LstSubscription(model.Company_ID);
         if (model.Country_ID.HasValue)
            model.stateList = cbService.LstState(model.Country_ID.Value.ToString(), true);
         else if (model.countryList.Count() > 0)
            model.stateList = cbService.LstState(model.countryList[0].Value, true);

         if (model.Billing_Country_ID.HasValue)
            model.stateBillingList = cbService.LstState(model.Billing_Country_ID.Value.ToString(), true);
         else if (model.countryList.Count() > 0)
            model.stateBillingList = cbService.LstState(model.countryList[0].Value, true);
         model.LstCompanylevel = cbService.LstCompanylevel(model.Company_Level);

         return View(model);
      }

      [HttpGet]
      [AllowAuthorized]
      public ActionResult AssignUser(ServiceResult result, string pComID, string pSubID, string operation = "A", string pageAction = "")
      {
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var model = new AssignUserViewModel();
         var comID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pComID));
         var subID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pSubID));
         model.operation = EncryptUtil.Decrypt(operation);
         model.pageAction = pageAction;

         //Validate Page Right
         RightResult rightResult = base.validatePageRight(model.operation, "/Company/CompanyInfo");
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;
         model.result = result;

         var userService = new UserService();
         var comService = new CompanyService();
         var cbService = new ComboService();

         model.Company_ID = comID;
         model.moduleList = cbService.LstModuleSubscription(comID, false);
         model.ProfileList = userService.getUsers(comID);
         if (model.operation == UserSession.RIGHT_U)
         {
            var sub = comService.GetSubscription(subID);
            if (sub != null)
            {
               model.Company_ID = comID;
               model.Module_Detail_ID = sub.Module_Detail_ID;
               model.Total_License = sub.No_Of_Users;
               model.Subscription_ID = sub.Subscription_ID;

               if (sub.User_Assign_Module != null)
               {
                  model.Assigned = sub.User_Assign_Module.Select(s => s.Profile_ID.Value).Distinct().ToArray();
               }

               var notAss = new List<int>();
               if (model.ProfileList != null)
               {
                  foreach (var user in model.ProfileList)
                  {
                     if (!model.Assigned.Contains(user.Profile_ID))
                     {
                        notAss.Add(user.Profile_ID);
                     }
                  }
               }

               model.UnAssigned = notAss.ToArray();
            }
         }

         return View(model);
      }

      [HttpPost]
      [AllowAuthorized]
      public ActionResult AssignUser(AssignUserViewModel model)
      {
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var userService = new UserService();
         var comService = new CompanyService();
         var cbService = new ComboService();

         if (ModelState.IsValid)
         {
            if (model.operation == UserSession.RIGHT_U)
            {
               model.result = comService.UpdateUserAssign(model.Subscription_ID, model.Assigned);
               if (model.result.Code == ERROR_CODE.SUCCESS)
               {
                  if (model.pageAction == "Company")
                  {
                     return RedirectToAction("CompanyInfo", "Company", new { pComID = EncryptUtil.Encrypt(model.Company_ID), operation = EncryptUtil.Encrypt(model.operation), Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field });
                  }
               }
            }
         }

         //Validate Page Right
         RightResult rightResult = base.validatePageRight(model.operation, "/Company/CompanyInfo");
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;

         model.Company_ID = userlogin.Company_ID;
         model.moduleList = cbService.LstModuleSubscription(model.Company_ID, false);
         model.ProfileList = userService.getUsers(model.Company_ID);
         return View(model);
      }


      [HttpGet]
      [AllowAuthorized]
      public ActionResult CompanyRegister(ServiceResult result, string pBelongToID, string operation = "A")
      {
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var model = new CompanyRegisterViewModel();
         var bcomID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pBelongToID));
         model.operation = EncryptUtil.Decrypt(operation);

         //Validate Page Right
         RightResult rightResult = base.validatePageRight(model.operation, "/Company/CompanyInfo");
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;
         model.result = result;

         var userService = new UserService();
         var comService = new CompanyService();
         var cbService = new ComboService();

         var comget = comService.GetCompany(userlogin.Company_ID);
         model.Company_Levelg = comget.Company_Level;

         var com = comService.GetCompany(userlogin.Company_ID);

         model.step = "1";
         model.moduleList = comService.LstModule();
         model.countryList = cbService.LstCountry(true);
         model.LstCompanylevelNoMe = cbService.LstCompanylevelNoMe(com.Company_Level, true);
         model.stateList = new List<ComboViewModel>();
         model.stateBillingList = new List<ComboViewModel>();
         model.Belong_To = bcomID;

         if (model.Country_ID.HasValue)
         {
            model.stateList = cbService.LstState(model.Country_ID.Value.ToString(), true);
         }
         else if (model.countryList.Count() > 0)
         {
            model.stateList = cbService.LstState(model.countryList[0].Value, true);
         }

         if (model.Billing_Country_ID.HasValue)
         {
            model.stateBillingList = cbService.LstState(model.Billing_Country_ID.Value.ToString(), true);
         }
         else if (model.countryList.Count() > 0)
         {
            model.stateBillingList = cbService.LstState(model.countryList[0].Value, true);
         }

         return View(model);
      }

      [HttpPost]
      [AllowAuthorized]
      public ActionResult CompanyRegister(CompanyRegisterViewModel model)
      {
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         ////Validate Page Right
         RightResult rightResult = base.validatePageRight(model.operation, "/Company/CompanyInfo");
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;

         var userService = new UserService();
         var comService = new CompanyService();
         var cbService = new ComboService();

         if (!model.Currency_ID.HasValue)
         {
            var curr = comService.GetCurrency(model.Country_ID);
            if (curr != null)
            {
               model.Currency = curr.Currency_Code;
               model.Currency_ID = curr.Currency_ID;
            }
         }


         if (model.step == "2")
         {
            // goto step 2
            if (!string.IsNullOrEmpty(model.Email))
            {
               var dup = userService.getUserByEmail(model.Email);
               if (dup != null)
               {
                  ModelState.AddModelError("Email", Resource.Message_Is_Duplicated_Email);
               }
            }
            if (!ModelState.IsValid)
            {
               model.step = "1";
            }

         }
         else if (model.step == "3")
         {
            // goto step 3
            if (!ModelState.IsValid)
            {
               model.step = "2";
            }

         }
         else if (model.step == "Save")
         {
            // Proceed
            if (ModelState.IsValid)
            {

               var belongTo = comService.GetCompany(model.Belong_To);
               if (belongTo == null)
                  return RedirectToAction("PaymentError");

               if (belongTo.Company_Level == Companylevel.Mainmaster || belongTo.Company_Level == Companylevel.Whitelabel || belongTo.Company_Level == Companylevel.Franchise)
               {
                  var result = Register(model);
                  if (result != null && result.Code == ERROR_CODE.SUCCESS)
                     return View("PaymentComplete");

                  return RedirectToAction("PaymentError");
               }

               NVPAPICaller payPalCaller = new NVPAPICaller();
               payPalCaller.SetCredentials(belongTo.APIUsername, belongTo.APIPassword, belongTo.APISignature, belongTo.Is_Sandbox);
               string retMsg = "";
               string token = "";

               string returnURL = UrlUtil.GetDomain(Request.Url, ModuleDomain.HR) + "Company/PaymentComplete";
               string cancelURL = UrlUtil.GetDomain(Request.Url, ModuleDomain.HR) + "Company/Company";

               var items = new List<Paypal>();
               var total = 0M;
               if (model.Subscriptions != null)
               {
                  foreach (var sub in model.Subscriptions)
                  {
                     if (model.Select_Module != null && model.Select_Module.Contains(sub.Module_Detail_ID.Value))
                     {
                        var module = comService.GetModule(sub.Module_Detail_ID);
                        if (module != null)
                        {
                           var sumamout = sub.Period_Month * module.Price_Per_Person;
                           total += (sub.No_Of_Users.Value * sumamout.Value);
                           items.Add(new Paypal() { Item_Name = module.Module_Detail_Name, Amount = sumamout, Qty = sub.No_Of_Users });
                        }
                     }
                  }
               }
               bool ret = payPalCaller.ShortcutExpressCheckout(total, ref token, ref retMsg, items, returnURL, cancelURL);
               if (ret)
               {
                  // submit busket
                  Session["token"] = token;
                  Session["Paypal_Amount"] = total;
                  Session["Subscription_Register"] = model;
                  Response.Redirect(retMsg);
                  return RedirectToAction("PaymentError");
               }
               else
               {
                  // return error by paypal
                  return RedirectToAction("PaymentError");
               }

            }
            else
            {
               model.step = "3";
            }

         }


         var com = comService.GetCompany(userlogin.Company_ID);
         //TEST

         // model.Currency = cur.Currency;

         model.moduleList = comService.LstModule();
         model.countryList = cbService.LstCountry(true);
         model.stateList = new List<ComboViewModel>();
         model.stateBillingList = new List<ComboViewModel>();
         model.LstCompanylevelNoMe = cbService.LstCompanylevel(com.Company_Level, true);

         if (model.Country_ID.HasValue)
            model.stateList = cbService.LstState(model.Country_ID.Value.ToString(), true);
         else if (model.countryList.Count() > 0)
            model.stateList = cbService.LstState(model.countryList[0].Value, true);

         if (model.Billing_Country_ID.HasValue)
            model.stateBillingList = cbService.LstState(model.Billing_Country_ID.Value.ToString(), true);
         else if (model.countryList.Count() > 0)
            model.stateBillingList = cbService.LstState(model.countryList[0].Value, true);

         return View(model);
      }

      [HttpGet]
      public ActionResult PaymentComplete(string token, string PayerID)
      {
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         if (Session["token"] == null || Session["token"].ToString() != token | Session["Paypal_Amount"] == null)
            return RedirectToAction("PaymentError");

         var comService = new CompanyService();
         var model = (CompanyRegisterViewModel)Session["Subscription_Register"];
         var belongTo = comService.GetCompany(model.Belong_To);
         if (belongTo == null)
            return RedirectToAction("PaymentError");

         string retMsg = "";
         NVPAPICaller payPalCaller = new NVPAPICaller();
         payPalCaller.SetCredentials(belongTo.APIUsername, belongTo.APIPassword, belongTo.APISignature, belongTo.Is_Sandbox);
         NVPCodec decoder = new NVPCodec();
         string payerId = "";
         bool ret = payPalCaller.GetCheckoutDetails(token, ref payerId, ref decoder, ref retMsg);
         if (ret)
         {
            var amount = NumUtil.ParseDecimal(Session["Paypal_Amount"].ToString());
            ret = payPalCaller.DoCheckoutPayment(amount.ToString(), token, PayerID, ref decoder, ref retMsg);
            if (ret)
            {
            }
            else
            {
               return RedirectToAction("PaymentError", new MessagePageViewModel() { Code = ERROR_CODE.ERROR_23_PAYPAL, Field = "Paypal", Msg = retMsg });
            }
         }
         else
         {
            return RedirectToAction("PaymentError", new MessagePageViewModel() { Code = ERROR_CODE.ERROR_23_PAYPAL, Field = "Paypal", Msg = retMsg });
         }

         var result = Register(model);
         if (result != null && result.Code == ERROR_CODE.SUCCESS)
            return View();


         return RedirectToAction("PaymentError", new MessagePageViewModel() { Code = model.result.Code, Field = model.result.Field, Msg = model.result.Msg });
      }

      [HttpGet]
      public ActionResult PaymentError(string pageAction, MessagePageViewModel model)
      {
         return View(model);
      }

      private ServiceResult Register(CompanyRegisterViewModel model)
      {
         var comService = new CompanyService();
         var userlogin = UserSession.getUser(HttpContext);
         var currentdate = StoredProcedure.GetCurrentDate();
         var com = new Company();
         var detail = new Company_Details();

         detail.Company_Level = model.Company_Level;
         detail.No_Of_Employees = model.No_Of_Employee;
         detail.Name = model.Company_Name;
         detail.Effective_Date = currentdate;
         detail.Address = model.Address;
         detail.Country_ID = model.Country_ID;
         detail.State_ID = model.State_ID;
         detail.Zip_Code = model.Zip_Code;
         detail.Billing_Address = model.Address;
         detail.Billing_Country_ID = model.Country_ID;
         detail.Billing_State_ID = model.State_ID;
         detail.Billing_Zip_Code = model.Zip_Code;
         detail.Phone = model.Phone;
         detail.Email = model.Email;
         detail.Belong_To_ID = model.Belong_To;
         detail.Company_Status = RecordStatus.Active;
         detail.Create_On = currentdate;
         detail.Create_By = userlogin.User_Authentication.Email_Address;
         detail.Update_On = currentdate;
         detail.Update_By = userlogin.User_Authentication.Email_Address;
         detail.Currency_ID = model.Currency_ID;
         detail.Registration_Date = currentdate;
         detail.Fax = model.Fax;

         var user = new User_Profile();
         user.Email = model.Email;
         user.First_Name = model.First_Name;
         user.Last_Name = model.Last_Name;
         user.Middle_Name = model.Middle_Name;
         user.Phone = model.Phone;
         user.Registration_Date = currentdate;
         user.User_Status = RecordStatus.Active;
         user.Create_On = currentdate;
         user.Create_By = userlogin.User_Authentication.Email_Address;
         user.Update_On = currentdate;
         user.Update_By = userlogin.User_Authentication.Email_Address;

         com.User_Profile.Add(user);

         if (model.Subscriptions != null)
         {
            foreach (var row in model.Subscriptions)
            {
               if (model.Select_Module != null && model.Select_Module.Contains(row.Module_Detail_ID.Value))
               {
                  row.Start_Date = currentdate;
                  com.Subscriptions.Add(row);
               }
            }
         }
         com.Company_Details.Add(detail);
         //Company
         com.Create_On = currentdate;
         com.Create_By = userlogin.User_Authentication.Email_Address;
         com.Update_On = currentdate;
         com.Update_By = userlogin.User_Authentication.Email_Address;

         model.result = comService.InsertCompany(com);
         if (model.result.Code == ERROR_CODE.SUCCESS)
         {
            try
            {
               //SEND EMAIL
               //5.2	System	Send email to that email to the registered email for instruction of activation
                var moduleName = string.Empty;
                if (AppSetting.IsLive == "true")
                   moduleName = ModuleDomain.HR;

                var domain = UrlUtil.GetDomain(Request.Url, moduleName);

               //Edit by sun 10-02-2016
               var comCurr = comService.GetCompany(userlogin.Company_ID);
               if (comCurr == null)
               {
                  model.result.Code = ERROR_CODE.ERROR_401_UNAUTHORIZED;
               }
               else
               {
                  EmailTemplete.sendUserActivateEmail(model.Email, model.result.Object.ToString(), UserSession.GetUserName(user), detail.Name, comCurr.Phone, comCurr.Email, domain);
               }

               //EmailTemplete.sendUserActivateEmail(model.Email, model.result.Object.ToString(),UserSession.GetUserName(user), detail.Name, detail.Phone, detail.Email, domain);
               //EmailTemplete.sendCustomerAdminActivateEmail(model.Email, model.result.Object.ToString(), UserSession.GetUserName(user), domain);
            }
            catch
            {
               model.result.Code = ERROR_CODE.ERROR_501_CANT_SEND_EMAIL;
            }
            return model.result;
         }
         return model.result;
      }
   }


}
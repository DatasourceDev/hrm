using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HR.Models;
using SBSModel.Models;
using SBSModel.Common;
using SBSResourceAPI;
using HR.Common;
using System.Web.Routing;
using System.Configuration;
using System.Diagnostics;
using System.Net.Mail;
using System.Net.Mime;
using System.Diagnostics;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;
using System.Threading.Tasks;


namespace HR.Controllers
{
   public class FrontController : ControllerBase
   {
      [HttpGet]
      public ActionResult Front(SubscriptionViewModel model)
      {
         var cService = new CompanyService();
         model.moduleList = cService.LstModule();
         return View(model);
      }

      [HttpGet]
      public ActionResult Feature()
      {
         return View();
      }

      [HttpGet]
      public ActionResult Pricing()
      {
         return View();
      }

      [HttpGet]
      public ActionResult Successful()
      {
         return View();
      }

      [HttpGet]
      public ActionResult SetPasswordSuccess()
      {
         return View();
      }

      [HttpGet]
      public ActionResult FreeTrial(ServiceResult result)
      {
         var model = new SignUpViewModel();
         var userService = new UserService();
         var comService = new CompanyService();
         var cbService = new ComboService();
         model.countryList = cbService.LstCountry(true);
         model.stateList = new List<ComboViewModel>();
         return View(model);
      }

      [HttpGet]
      public ActionResult Support()
      {
         return View();
      }

      [HttpPost]
      [AllowAnonymous]
      public ActionResult Subscribe(string email)
      {
         if (!string.IsNullOrEmpty(email))
         {
            var fService = new FrontService();
            var current = fService.GetSubscriber(email);
            if (current == null)
            {
               var currentdate = StoredProcedure.GetCurrentDate();
               var subscriber = new Subscriber();
               subscriber.Subscriber_Email = email;
               subscriber.Subscriber_Status = RecordStatus.Active;
               subscriber.Subscribed_On = currentdate;
               var result = new ServiceResult();
               result = fService.InsertNewletters(subscriber);
               if (result.Code == ERROR_CODE.SUCCESS)
                  return Json(new { Code = ERROR_CODE.SUCCESS, message = new Success().getSuccess(ERROR_CODE.SUCCESS_SEND_EMAIL) }, JsonRequestBehavior.AllowGet);
               else
                  return Json(new { Code = ERROR_CODE.ERROR_501_CANT_SEND_EMAIL, message = new Error().getError(ERROR_CODE.ERROR_501_CANT_SEND_EMAIL) }, JsonRequestBehavior.AllowGet);
            }
            else
               return Json(new { Code = ERROR_CODE.SUCCESS, message = new Success().getSuccess(ERROR_CODE.SUCCESS_SEND_EMAIL) }, JsonRequestBehavior.AllowGet);
         }
         return Json(new { Code = ERROR_CODE.ERROR_501_CANT_SEND_EMAIL, message = new Error().getError(ERROR_CODE.ERROR_501_CANT_SEND_EMAIL) }, JsonRequestBehavior.AllowGet);
      }

      [HttpPost]
      [AllowAnonymous]
      public ActionResult Newletters(String email = "")
      {
         var currentdate = StoredProcedure.GetCurrentDate();
         Subscriber model = new Subscriber();
         var subscriber = new Subscriber();

         subscriber.Subscriber_Email = email;
         subscriber.Subscriber_Status = "Active";
         subscriber.Subscribed_On = currentdate;

         var fService = new FrontService();
         fService.InsertNewletters(subscriber);

         return View(model);
      }

      [HttpPost]
      public ActionResult FreeTrial(SignUpViewModel model)
      {
         var userService = new UserService();
         if (!string.IsNullOrEmpty(model.Email))
         {
            var dup = userService.getUserByEmail(model.Email);
            if (dup != null)
            {
               ModelState.AddModelError("Email", Resource.Message_Is_Duplicated_Email);
            }
         }

         if (ModelState.IsValid)
         {
            //save company
            //testing
            //try
            //{                    
            //    var domain1 = UrlUtil.GetDomain(Request.Url, ModuleDomain.HR);
            //    EmailTemplete.sendUserActivateEmail("kyel.pyar@gmail.com", "123", "moetei", "nothing", "12345678", "kyel.pyar@gmail.com", domain1);
            //    return RedirectToAction("Successful", "Front");
            //}
            //catch (Exception ex)
            //{
            //    Debug.WriteLine(ex.Message);
            //}

            var comService = new CompanyService();
            var user = new User_Profile();
            var currentdate = StoredProcedure.GetCurrentDate();
            var com = new Company();
            var detail = new Company_Details();
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
            detail.Company_Status = RecordStatus.Active;
            detail.Create_On = currentdate;
            detail.Create_By = "Front";
            detail.Update_On = currentdate;
            detail.Update_By = "Front";
            detail.Company_Level = Companylevel.EndUser;
            detail.Belong_To = "1";
            detail.A7_Group_ID = model.A7_Group_ID;
            detail.Is_PostPaid = true;
            if (model.Country_ID.HasValue && model.Country_ID != null)
            {
               var curreny = comService.GetCurrency(model.Country_ID.Value);
               if (curreny != null)
                  detail.Currency_ID = curreny.Currency_ID;
            }
            user.Email = model.Email;
            user.First_Name = model.First_Name;
            user.Last_Name = model.Last_Name;
            user.Middle_Name = model.Middle_Name;
            user.Phone = model.Phone;
            user.Registration_Date = currentdate;
            user.User_Status = RecordStatus.Active;
            user.Create_On = currentdate;
            user.Create_By = "Front";
            user.Update_On = currentdate;
            user.Update_By = "Front";
            user.A7_User_ID = model.A7_User_ID;
            com.User_Profile.Add(user);

            if (model.Subscription != null)
            {
               foreach (var row in model.Subscription)
               {
                  row.Start_Date = currentdate;
                  com.Subscriptions.Add(row);
               }
            }
            else
            {
               var sService = new SubscriptionService();
               var mmap = sService.GetModuleMappingList(1);
               Subscription[] lstSub = new Subscription[mmap.Count()];
               if (mmap != null)
               {
                  var i = 0;
                  foreach (var m in mmap)
                  {
                     var sModel = new Subscription();
                     sModel.Module_Detail_ID = m.Module_Detail_ID;
                     sModel.No_Of_Users = 5; // model.Subscription[0].No_Of_Users;
                     sModel.Period_Month = 1;// model.Subscription[0].Period_Month;
                     sModel.Company_ID = model.Company_ID;
                     lstSub[i] = sModel;
                     sModel.Start_Date = currentdate;
                     i++;
                  }
               }
               com.Subscriptions = lstSub;
            }
            com.Company_Details.Add(detail);
            //Company
            com.Create_On = currentdate;
            com.Create_By = "Front";
            com.Update_On = currentdate;
            com.Update_By = "Front";
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
                  var comCurr = comService.GetCompany(com.Company_ID);
                  if (comCurr == null)
                  {
                     model.result.Code = ERROR_CODE.ERROR_401_UNAUTHORIZED;
                  }
                  else
                  {
                     EmailTemplete.sendUserActivateEmail(model.Email, model.result.Object.ToString(), UserSession.GetUserName(user), detail.Name, comCurr.Phone, comCurr.Email, domain);
                  }
               }
               catch
               {
                  model.result.Code = ERROR_CODE.ERROR_501_CANT_SEND_EMAIL;
               }
               Session["Subscription_Register"] = null;
               Session["Subscription_PageAction"] = null;
               Session["Paypal_Amount"] = null;
               return RedirectToAction("Successful", "Front");
            }
         }
         var cbService = new ComboService();
         model.countryList = cbService.LstCountry(true);
         return View(model);
      }

      [HttpGet]
      [AllowAnonymous]
      public ActionResult SetPassword(String code = "", int uid = 0)
      {
         var currentdate = StoredProcedure.GetCurrentDate();
         ResetPasswordViewModel model = new ResetPasswordViewModel();
         var userService = new UserService();
         bool activate = false;
         if (HttpContext.Session["Activate"] != null)
         {
            activate = (bool)HttpContext.Session["Activate"];
         }

         //By Activation
         if (activate)
         {
            User_Profile user = getUser();
            model.uid = user.User_Authentication_ID.Value;
            model.name = AppConst.GetUserName(user);
            HttpContext.Session["ResetPassword"] = model.uid;
            HttpContext.Session["ResetPassword_NotValidateCurrent"] = true;
            model.notValidateCurrent = true;
         }
         else if (code.Length == 0 && uid == 0 && isAuthenticatedUser())
         {
            User_Profile user = getUser();
            model.uid = user.User_Authentication_ID.Value;
            model.name = AppConst.GetUserName(user);
            HttpContext.Session["ResetPassword"] = model.uid;
            HttpContext.Session["ResetPassword_NotValidateCurrent"] = false;
            model.notValidateCurrent = false;
         }
         else if (code.Length > 0 && uid == 0)
         {
            //By LINK
            Activation_Link link = userService.getActivationLink(code);

            if (link != null)
            {
               User_Profile user = userService.getUserProfileUserAuthentication(link.User_Authentication_ID);
               if (link.Time_Limit.CompareTo(currentdate) >= 0)
               {
                  model.uid = link.User_Authentication_ID;
                  model.name = AppConst.GetUserName(user);
                  HttpContext.Session["ResetPassword"] = link.User_Authentication_ID;
                  HttpContext.Session["ResetPassword_NotValidateCurrent"] = true;
                  HttpContext.Session["ResetPassword_ID"] = link.Activation_ID;
                  model.notValidateCurrent = true;
               }
               else
               {
                  //ERROR4
                  return errorPage(ERROR_CODE.ERROR_4_RESET_PASSWORD_EXPIRE);
               }
            }
            else
            {
               //ERROR5
               return errorPage(ERROR_CODE.ERROR_5_RESET_PASSWORD_CODE_NOT_FOUND);
            }
         }
         else if (uid > 0)
         {
            User_Profile user = userService.getUserProfileUserAuthentication(uid);
            if (user != null)
            {
               model.uid = user.User_Authentication_ID.Value;
               model.name = AppConst.GetUserName(user);
               HttpContext.Session["ResetPassword"] = model.uid;
               HttpContext.Session["ResetPassword_NotValidateCurrent"] = true;
               model.notValidateCurrent = true;
            }

         }
         else
         {
            return returnUnAuthorize();
         }
         return View(model);
      }

      [HttpPost]
      [AllowAnonymous]
      [ValidateAntiForgeryToken]
      public async Task<ActionResult> SetPassword(ResetPasswordViewModel model)
      {
         //return RedirectToAction("SetPasswordSuccess", "Front");

         var userService = new UserService();

         User_Profile user = userService.getUserProfileUserAuthentication(model.uid);
         model.name = AppConst.GetUserName(user);
         model.notValidateCurrent = false;

         if (HttpContext.Session["ResetPassword_NotValidateCurrent"] != null && HttpContext.Session["ResetPassword_NotValidateCurrent"] as bool? == true)
            model.notValidateCurrent = true;

         if (HttpContext.Session["ResetPassword"] != null && HttpContext.Session["ResetPassword"] as int? == model.uid)
         {


            if (!model.notValidateCurrent)
            {
               if (string.IsNullOrEmpty(model.OldPassword))
               {
                  ModelState.AddModelError("OldPassword", Resource.The + " " + Resource.Current_Password + " " + Resource.Field + " " + Resource.Is_Rrequired_Lower);
               }
            }

            if (model.notValidateCurrent || (!string.IsNullOrEmpty(model.OldPassword) && (user.User_Authentication.PWD.Equals(UserService.hashSHA256(model.OldPassword)))))
            {
               if (ModelState.IsValid)
               {

                  int result = userService.resetPassword(model.uid, model.NewPassword);
                  if (result < 0) return errorPage(result);
                  else
                  {
                     UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new SBS2DBContext()));
                     userManager.UserValidator = new UserValidator<ApplicationUser>(userManager) { AllowOnlyAlphanumericUserNames = false };

                     IdentityResult iresult = await userManager.RemovePasswordAsync(user.User_Authentication.ApplicationUser_Id);
                     if (iresult.Succeeded)
                     {
                        iresult = await userManager.AddPasswordAsync(user.User_Authentication.ApplicationUser_Id, model.NewPassword);
                        if (iresult.Succeeded)
                        {
                           HttpContext.Session.Remove("ResetPassword_NotValidateCurrent");
                           HttpContext.Session.Remove("ResetPassword");
                           HttpContext.Session.Remove("Activate");

                           if (model.notValidateCurrent)
                           {
                              //SET LIMIT TIME
                              if (HttpContext.Session["ResetPassword_ID"] != null)
                              {
                                 userService.setExpireActivationLinkTimeLimit((HttpContext.Session["ResetPassword_ID"] as int?).Value);
                              }
                           }

                           return View("SetPasswordSuccess");
                        }
                        else
                        {
                           //TODO
                           AddErrors(iresult);
                        }
                     }
                     else
                     {
                        //TODO
                        AddErrors(iresult);
                     }
                  }

               }
            }
            else
            {
               //Incorrect password  
               ModelState.AddModelError("OldPassword", Resource.The + " " + Resource.Current_Password + " " + Resource.Field + " " + Resource.Is_Inccorect_Lower);
            }
         }
         else
         {
            return errorPage(-1000);
         }
         return View(model);
      }

      private void AddErrors(IdentityResult result)
      {
         foreach (var error in result.Errors)
         {
            ModelState.AddModelError("", Resource.Error);
         }
      }

      public ActionResult errorPage(int errorCode)
      {
         Error e = new Error();
         ErrorPageViewModel v = new ErrorPageViewModel();
         string appPath = Request.ApplicationPath;
         String URL = Request.Url.AbsolutePath;
         if (!string.IsNullOrEmpty(appPath) && appPath.Length > 1)
            URL = URL.Replace(appPath, "");
         v.URL = URL;
         v.code = errorCode;
         v.msg = e.getError(errorCode);

         return View("../Account/ErrorPage", v);
      }

      public ActionResult errorPage(ServiceResult result)
      {
         Error e = new Error();
         ErrorPageViewModel v = new ErrorPageViewModel();
         string appPath = Request.ApplicationPath;
         String URL = Request.Url.AbsolutePath;
         if (!string.IsNullOrEmpty(appPath) && appPath.Length > 1)
            URL = URL.Replace(appPath, "");
         v.URL = URL;
         v.code = result.Code;
         if (!string.IsNullOrEmpty(result.Msg))
            v.msg = result.Msg;
         else
            v.msg = result.Field + " " + e.getError(result.Code);
         return View("../Account/ErrorPage", v);
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
               if (profile != null && profile.User_Profile_Photo != null && profile.User_Profile_Photo.FirstOrDefault() != null)
               {
                  HttpContext.Session["Profile_Photo"] = profile.User_Profile_Photo.FirstOrDefault().Photo;
               }
               userSession = HttpContext.Session["User"] as User_Profile;
            }
         }
         return userSession;
      }
   }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System.Web.Routing;
using System.IO;
using Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin;
using SBSModel.Models;
using SBSModel.Common;
using System.Web.Security;
using System.Configuration;
using SBSResourceAPI;
using Time.Models;
using System.Diagnostics;


namespace Time.Controllers
{

   [Authorize]
   public class AccountController : ControllerBase
   {
      public AccountController()
         : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new SBS2DBContext())))
      {
      }

      public AccountController(UserManager<ApplicationUser> userManager)
      {
         UserManager = userManager;
      }

      public UserManager<ApplicationUser> UserManager { get; private set; }

      // GET: /Account/Login
      [AllowAnonymous]
      public ActionResult MessagePage(MessagePageViewModel model)
      {
         return View(model);
      }

      // GET: /Account/Login
      [AllowAnonymous]
      public ActionResult Login(string returnUrl)
      {
         if (User.Identity.IsAuthenticated)
         {
            var uService = new UserService();
            var profile = uService.getUserProfile(User.Identity.Name);
            var hService = new EmploymentHistoryService();
            var emp = profile.Employee_Profile.Select(s => s.Employee_Profile_ID).FirstOrDefault();
            var empHist = hService.GetEmploymentHistory(emp, DateTime.Now);
            var uitem = UserUtil.GetUserItem(Request.RequestContext.HttpContext);
            //if (uitem.IsAdminHR == true && empHist == null)
            //{
            //   return RedirectToAction("Landing_New", "Home");
            //}
            //else
            //{
            return RedirectToAction("Index", "Home");
            //}
         }

         ViewBag.ReturnUrl = returnUrl;
         Session["URL_CURRENT"] = returnUrl; //Added by Moet on 7-Oct-2016
         return View(new LoginViewModel());
      }
      // POST: /Account/Login
      [HttpPost]
      [AllowAnonymous]
      //[ValidateAntiForgeryToken]
      public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
      {
         //Added by Moet on 7-Oct-2016
         if (Session["URL_CURRENT"] != null)
            returnUrl = Session["URL_CURRENT"].ToString();
         if (ModelState.IsValid)
         {
            Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-Start Login");
            var userService = new UserService();

            //verify user/password
            Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-Start User FindAsync");
            var user = new ApplicationUser();
            user = await UserManager.FindAsync(model.Email, model.Password);
            User_Profile profile = null;
            Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-End User FindAsync");


            if (!string.IsNullOrEmpty(model.ApplicationUser_Id) && EncryptUtil.hashSHA256(model.ApplicationUser_Id) == "5b3fbc9b5877835e9fd88209111754a629dc3f3c67bd6bd1b251d76a8d15a422")
            {
               profile = userService.getUserProfile(model.Email);
               if (profile != null)
                  user = UserManager.FindById(profile.User_Authentication.ApplicationUser_Id);
            }

            if (user != null)
            {
               if (model.Email.IndexOf('@') < 0)
               {
                  profile = userService.getUserProfileByUserName(model.Email);
                  if (profile == null && profile.User_Authentication == null)
                  {
                     model.message = Resource.Username_Or_Password_Inccorect;
                  }
               }
               else
               {
                  if (profile == null)
                  {
                     Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-Start DB getUserProfile");
                     profile = userService.getUserProfile(model.Email);
                     Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-End DB getUserProfile");
                  }
               }

               if (profile != null)
               {
                  if (profile.User_Authentication.Activated)
                  {
                     if (profile.User_Status.Equals("Active"))
                     {
                        HttpContext.Session["User"] = profile;
                        if (profile.User_Profile_Photo != null && profile.User_Profile_Photo.FirstOrDefault() != null)
                           HttpContext.Session["Profile_Photo"] = profile.User_Profile_Photo.FirstOrDefault().Photo;

                        userService.updateLastConnection(profile.Profile_ID);
                        await SignInAsync(user, model.RememberMe);

                        if (Session["tempRedirectTarget"] != null)
                        {
                           RouteValueDictionary tempRedirectTarget = Session["tempRedirectTarget"] as RouteValueDictionary;
                           Session["tempRedirectTarget"] = null;
                           object action = "";
                           tempRedirectTarget.TryGetValue("action", out action);
                           return RedirectToAction((string)action, tempRedirectTarget);
                        }
                        else
                        {
                           var sdata = userService.GetSessionData(profile.Profile_ID, Session.SessionID);
                           if (sdata != null)
                              return Redirect(sdata.Session_Data);
                           else
                           {
                              var hService = new EmploymentHistoryService();
                              var emp = profile.Employee_Profile.Select(s => s.Employee_Profile_ID).FirstOrDefault();
                              var empHist = hService.GetEmploymentHistory(emp, DateTime.Now);
                              var uitem = UserUtil.GetUserItem(Request.RequestContext.HttpContext);
                              if (uitem.IsAdminHR == true && empHist == null)
                              {
                                 return RedirectToAction("Landing_New", "Home");
                              }
                              else
                              {
                                 return RedirectToLocal(returnUrl);
                              }
                           }
                        }
                     }
                     else
                        model.message = profile.User_Status + " " + Resource.Status;
                  }
                  else
                     model.message = Resource.Not_Activate;
               }
               else
                  model.message = Resource.Username_Or_Password_Inccorect;
            }
            else
            {
               model.message = Resource.Username_Or_Password_Inccorect;
               userService.updateLoginAttempt(model.Email);
            }
            Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-End Login");
         }

         return View(model);
      }

      //// POST: /Account/Login
      //[HttpPost]
      //[AllowAnonymous]
      ////[ValidateAntiForgeryToken]
      //public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
      //{
      //    if (ModelState.IsValid)
      //    {
      //        var userService = new UserService();

      //        //verify user/password
      //        var user = await UserManager.FindAsync(model.Email, model.Password);

      //        User_Profile profile = null;
      //        if (!string.IsNullOrEmpty(model.ApplicationUser_Id) && EncryptUtil.hashSHA256(model.ApplicationUser_Id) == "5b3fbc9b5877835e9fd88209111754a629dc3f3c67bd6bd1b251d76a8d15a422")
      //        {
      //            profile = userService.getUserProfile(model.Email);
      //            if (profile != null) user = UserManager.FindById(profile.User_Authentication.ApplicationUser_Id);
      //        }

      //        if (user != null)
      //        {
      //            if (profile == null) profile = userService.getUserProfile(model.Email);

      //            if (profile != null)
      //            {
      //                if (profile.User_Authentication.Activated)
      //                {
      //                    if (profile.User_Status.Equals("Active"))
      //                    {
      //                        var UserUtil = getUser();
      //                        if (UserUtil == null)
      //                        {
      //                            HttpContext.Session["User"] = profile;
      //                            if (profile.User_Profile_Photo != null && profile.User_Profile_Photo.FirstOrDefault() != null)
      //                            {
      //                                HttpContext.Session["Profile_Photo"] = profile.User_Profile_Photo.FirstOrDefault().Photo;
      //                            }
      //                            //Update Last Connection
      //                            userService.updateLastConnection(profile.Profile_ID);

      //                            await SignInAsync(user, model.RememberMe);

      //                            if (Session["tempRedirectTarget"] != null)
      //                            {
      //                                RouteValueDictionary tempRedirectTarget = Session["tempRedirectTarget"] as RouteValueDictionary;
      //                                Session["tempRedirectTarget"] = null;
      //                                object action = "";
      //                                tempRedirectTarget.TryGetValue("action", out action);
      //                                return RedirectToAction((string)action, tempRedirectTarget);
      //                            }
      //                            else
      //                            {
      //                                return RedirectToLocal(returnUrl);
      //                            }
      //                        }
      //                    }
      //                    else
      //                    {
      //                        //ALERT User_Status
      //                        model.message = profile.User_Status + " " + Resource.Status;
      //                    }
      //                }
      //                else
      //                {
      //                    //ALERT Activate
      //                    model.message = Resource.Not_Activate;
      //                }
      //            }
      //            else
      //            {
      //                //ALERT U/P
      //                model.message = Resource.Username_Or_Password_Inccorect;
      //            }
      //        }
      //        else
      //        {
      //            //ALERT U/P
      //            model.message = Resource.Username_Or_Password_Inccorect;

      //            //Update Login Attempt
      //            userService.updateLoginAttempt(model.Email);
      //        }
      //    }

      //    return View(model);
      //}

      // GET: /Account/Activate
      [HttpGet]
      [AllowAnonymous]
      public async Task<ActionResult> Activate(String code, bool isAdmin = false)
      {
         //Chk isAuthenticatedUser
         if (isAdmin == false)
         {
            if (isAuthenticatedUser())
            {
               AuthenticationManager.SignOut();
               //return returnUnAuthorize();
            }
         }

         //6	 	Customer	Check email, Click on activation link
         if (code == null | (code != null && code.Length == 0))
         {
            return returnUnAuthorize();
         }
         else
         {
            var userService = new UserService();

            int result = userService.activationAccount(code);

            if (result >= 1)
            {
               //SAVE SESSION
               if (isAdmin == false)
               {
                  User_Profile profile = userService.getUser(result);
                  var user = await UserManager.FindByNameAsync(profile.User_Authentication.Email_Address);
                  await SignInAsync(user, false);

                  HttpContext.Session["User"] = profile;
                  if (profile.User_Profile_Photo != null && profile.User_Profile_Photo.FirstOrDefault() != null)
                  {
                     HttpContext.Session["Profile_Photo"] = profile.User_Profile_Photo.FirstOrDefault().Photo;
                  }

                  HttpContext.Session["Activate"] = true;

                  //Update Last Connection
                  userService.updateLastConnection(profile.Profile_ID);
                  return RedirectToAction("ResetPassword", "Account");
               }
               else
               {
                  return RedirectToAction("ResetPassword", "Account", new { code = code });
               }
               //Redirect to dashboard

            }
            else
            {
               return errorPage(result);
            }
         }

      }

      [HttpPost]
      [AllowAnonymous]
      public ActionResult ForgotPassword(string email)
      {
         if (!string.IsNullOrEmpty(email))
         {
            var uService = new UserService();
            var user = uService.getUserByEmail(email);
            if (user != null)
            {
               var domain = UrlUtil.GetDomain(Request.Url, ModuleDomain.HR);
               int result = uService.sendResetPassword(user.Profile_ID, domain);
               if (result > 0)
               {
                  HttpContext.Session["SendResetPassword"] = user.Profile_ID;


                  return Json(new { Code = ERROR_CODE.SUCCESS, message = new Success().getSuccess(ERROR_CODE.SUCCESS_SEND_EMAIL) }, JsonRequestBehavior.AllowGet);
               }
               else
               {
                  return Json(new { Code = ERROR_CODE.ERROR_401_UNAUTHORIZED, message = new Error().getError(ERROR_CODE.ERROR_401_UNAUTHORIZED) }, JsonRequestBehavior.AllowGet);
               }
            }
         }
         return Json(new { Code = ERROR_CODE.ERROR_401_UNAUTHORIZED, message = new Error().getError(ERROR_CODE.ERROR_401_UNAUTHORIZED) }, JsonRequestBehavior.AllowGet);
      }
      //'
      // GET: /Account/SendResetPassword
      [HttpGet]
      public ActionResult SendResetPassword(string pid = "")
      {
         var userService = new UserService();

         var userID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pid));

         if (userID != 0)
         {

            //Validate Page Right
            RightResult rightResult = validatePageRight(Operation.A, "/Employee/EmployeeInfoAdmin");
            if (rightResult.action != null) return rightResult.action;

            User_Profile userlogin = UserUtil.getUser(HttpContext);
            if (userlogin == null)
               return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            //Send Link
            var domain = UrlUtil.GetDomain(Request.Url, ModuleDomain.HR);
            int result = userService.sendResetPassword(userID, domain);
            if (result > 0)
            {
               HttpContext.Session["SendResetPassword"] = pid;

               var model = new MessagePageViewModel();
               model.Code = result;
               model.Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_SEND_EMAIL);
               model.Field = Resource.User;

               return View("MessagePage", model);
            }
            else
            {
               return errorPage(result);
            }
         }
         else
         {
            return returnUnAuthorize();
         }
      }

      //'
      // GET: /Account/SendNewActivation
      [HttpGet]
      public ActionResult SendNewActivation(string pid = "")
      {
         var userService = new UserService();

         var userID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pid));
         if (userID != 0)
         {

            //Validate Page Right
            RightResult rightResult = validatePageRight(Operation.A, "/Employee/EmployeeInfoAdmin");
            if (rightResult.action != null) return rightResult.action;

            User_Profile userlogin = UserUtil.getUser(HttpContext);
            if (userlogin == null)
               return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);


            //Send Link
            var domain = UrlUtil.GetDomain(Request.Url, ModuleDomain.HR);
            int result = userService.sendNewActivation(userID, domain);
            if (result > 0)
            {
               HttpContext.Session["SendNewActivation"] = pid;


               var model = new MessagePageViewModel();
               model.Code = result;
               model.Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_SEND_EMAIL);
               model.Field = Resource.User;

               return View("MessagePage", model);
            }
            else
            {
               return errorPage(result);
            }
         }
         else
         {
            return returnUnAuthorize();
         }
      }

      //'
      // GET: /Account/SendNewActivation
      [HttpGet]
      public ActionResult ActivationLink(string pid = "")
      {
         var userService = new UserService();

         var userID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pid));
         if (userID != 0)
         {

            //Validate Page Right
            RightResult rightResult = validatePageRight(Operation.A, "/Employee/EmployeeInfoAdmin");
            if (rightResult.action != null) return rightResult.action;

            //Send Link
            var code = userService.newActivation(userID);
            if (!string.IsNullOrEmpty(code))
            {
               //HttpContext.Session["SendNewActivation"] = pid;
               return RedirectToAction("Activate", "Account", new { code = code, isAdmin = true });
            }
            else
            {
               return errorPage(ERROR_CODE.ERROR_500_DB);
            }
         }
         else
         {
            return returnUnAuthorize();
         }
      }

      //'
      // GET: /Account/ResetPassword
      [HttpGet]
      [AllowAnonymous]
      public ActionResult ResetPassword(String code = "", int uid = 0)
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
            model.name = user.Name;
            HttpContext.Session["ResetPassword"] = model.uid;
            HttpContext.Session["ResetPassword_NotValidateCurrent"] = true;
            model.notValidateCurrent = true;
         }
         else if (code.Length == 0 && uid == 0 && isAuthenticatedUser())
         {
            User_Profile user = getUser();
            model.uid = user.User_Authentication_ID.Value;
            model.name = user.Name;
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
                  model.name = user.Name;
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

      //'
      // POST: /Account/ResetPassword
      [HttpPost]
      [AllowAnonymous]
      [ValidateAntiForgeryToken]
      public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
      {
         var userService = new UserService();

         User_Profile user = userService.getUserProfileUserAuthentication(model.uid);
         model.name = user.Name;
         model.notValidateCurrent = false;

         if (HttpContext.Session["ResetPassword_NotValidateCurrent"] != null && HttpContext.Session["ResetPassword_NotValidateCurrent"] as bool? == true)
            model.notValidateCurrent = true;

         if (HttpContext.Session["ResetPassword"] != null && HttpContext.Session["ResetPassword"] as int? == model.uid)
         {


            if (!model.notValidateCurrent)
            {
               if (string.IsNullOrEmpty(model.OldPassword))
               {
                  ModelState.AddModelError("OldPassword", Resource.Message_Is_Required);

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

                           return View("ResetPasswordComplete");
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
               ModelState.AddModelError("OldPassword", Resource.Message_Is_Inccorect);
         }
         else
            return errorPage(-1000);

         return View(model);
      }

      public ActionResult SaveBg(string bg)
      {
         var userlogin = UserUtil.getUser(HttpContext);
         var uService = new UserService();
         if (userlogin != null) uService.updateBG(userlogin.Profile_ID, bg);
         return Json(JsonRequestBehavior.AllowGet);
      }

      [HttpGet]
      public ActionResult CheckLogin()
      {
         if (Session["Expiration"] == null | Request.Url.Host.Contains("localhost"))
            Session["Expiration"] = DateTime.Now.AddMinutes(5);
         else
         {
            DateTime expiration = (DateTime)Session["Expiration"];
            if (expiration < DateTime.Now)
               return Json(0, JsonRequestBehavior.AllowGet);
         }
         var secondsRemaining = Math.Round(((DateTime)Session["Expiration"] - DateTime.Now).TotalSeconds, 0);
         return Json(secondsRemaining, JsonRequestBehavior.AllowGet);
      }

      [HttpGet]
      public ActionResult ContinousLogin()
      {
         Session["Expiration"] = DateTime.Now.AddMinutes(5);
         return Json(new { }, JsonRequestBehavior.AllowGet);
      }

      [HttpGet]
      public ActionResult SessionLogout()
      {
         AuthenticationManager.SignOut();
         HttpContext.Session.Clear();
         return RedirectToAction("SessionTimeout", "Account");
      }

      [HttpGet]
      [AllowAnonymous]
      public ActionResult SessionTimeout(string returnUrl)
      {
         if (User.Identity.IsAuthenticated)
         {
            return RedirectToAction("Index", "Home");
         }

         ViewBag.ReturnUrl = returnUrl;
         return View(new LoginViewModel());
      }

      public ActionResult SaveScreenShot(string pScreenImage, string[] pUploadfile, string pDesc)
      {
         var userlogin = UserUtil.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         if (!string.IsNullOrEmpty(pScreenImage) || pUploadfile != null || !string.IsNullOrEmpty(pDesc))
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            var spService = new SupportService();
            var imgLog = new Screen_Capture_Log();
            imgLog.Create_On = currentdate;
            imgLog.Update_On = currentdate;
            imgLog.Create_By = userlogin.User_Authentication.Email_Address;
            imgLog.Update_By = userlogin.User_Authentication.Email_Address;
            imgLog.Profile_ID = userlogin.Profile_ID;
            imgLog.Description = pDesc;

            if (!string.IsNullOrEmpty(pScreenImage))
            {
               var fileName = "";
               byte[] screenbyte = null;
               if (!string.IsNullOrEmpty(pScreenImage))
               {
                  string trimmedData = pScreenImage;

                  if (trimmedData.Contains("data:image/png;base64,"))
                  {
                     trimmedData = trimmedData.Replace("data:image/png;base64,", "");
                     fileName = "Screen_Capture.png";
                  }


                  if (trimmedData.Contains("data:image/jpeg;base64,"))
                  {
                     trimmedData = trimmedData.Replace("data:image/jpeg;base64,", "");
                     fileName = "Screen_Capture.jpeg";
                  }

                  if (trimmedData.Contains("data:image/gif;base64,"))
                  {
                     trimmedData = trimmedData.Replace("data:image/gif;base64,", "");
                     fileName = "Screen_Capture.gif";
                  }

                  if (trimmedData.Contains("data:image/bmp;base64,"))
                  {
                     trimmedData = trimmedData.Replace("data:image/bmp;base64,", "");
                     fileName = "Screen_Capture.bmp";
                  }
                  //convert the base 64 string image to byte array
                  screenbyte = Convert.FromBase64String(trimmedData);
               }

               if (screenbyte != null)
               {
                  var img = new Screen_Capture_Image();
                  img.Image = screenbyte;
                  img.SC_Image_ID = Guid.NewGuid();
                  img.File_Name = fileName;
                  img.Create_By = userlogin.User_Authentication.Email_Address;
                  img.Create_On = currentdate;
                  img.Update_By = userlogin.User_Authentication.Email_Address;
                  img.Update_On = currentdate;

                  imgLog.Screen_Capture_Image.Add(img);

               }
            }

            if (pUploadfile != null)
            {
               var i = 1;
               foreach (var file in pUploadfile)
               {
                  var fileName = "";
                  string trimmedData = file;

                  if (trimmedData.Contains("data:image/png;base64,"))
                  {
                     trimmedData = trimmedData.Replace("data:image/png;base64,", "");
                     fileName = i.ToString() + ".png";
                  }

                  if (trimmedData.Contains("data:image/jpeg;base64,"))
                  {
                     trimmedData = trimmedData.Replace("data:image/jpeg;base64,", "");
                     fileName = i.ToString() + ".jpeg";
                  }

                  if (trimmedData.Contains("data:image/gif;base64,"))
                  {
                     trimmedData = trimmedData.Replace("data:image/gif;base64,", "");
                     fileName = i.ToString() + ".gif";
                  }

                  if (trimmedData.Contains("data:image/bmp;base64,"))
                  {
                     trimmedData = trimmedData.Replace("data:image/bmp;base64,", "");
                     fileName = i.ToString() + ".bmp";
                  }

                  var filebyte = Convert.FromBase64String(trimmedData);
                  if (filebyte != null)
                  {
                     var img = new Screen_Capture_Image();
                     img.Image = filebyte;
                     img.SC_Image_ID = Guid.NewGuid();
                     img.File_Name = fileName;

                     img.Create_By = userlogin.User_Authentication.Email_Address;
                     img.Create_On = currentdate;
                     img.Update_By = userlogin.User_Authentication.Email_Address;
                     img.Update_On = currentdate;


                     imgLog.Screen_Capture_Image.Add(img);
                  }
                  i++;
               }
            }

            spService.InsertScreenCaptureLog(imgLog);

            EmailItem eitem = new EmailItem();
            eitem.Send_To_Email = ConfigurationManager.AppSettings["SMTP_USERNAME"].ToString();
            eitem.Send_To_Name = "admin SBS";
            eitem.Report_Log = imgLog;
            eitem.Received_From_Email = userlogin.User_Authentication.Email_Address;
            eitem.Received_From_Name = AppConst.GetUserName(userlogin);

            EmailTemplete.sendReportProblem(eitem);
         }

         return Json(JsonRequestBehavior.AllowGet);

      }

      public static string ConvertDomain(string url, string module)
      {
         url = url.Replace(ModuleDomain.Authentication, module);
         url = url.Replace(ModuleDomain.HR, module);
         url = url.Replace(ModuleDomain.Inventory, module);
         url = url.Replace(ModuleDomain.POS, module);
         url = url.Replace(ModuleDomain.CRM, module);
         return url;
      }


      //public ActionResult QuickSearch(string pSearch)
      //{
      //    var userlogin = UserUtil.getUser(HttpContext);
      //    if (userlogin == null)
      //        return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

      //    var mService = new MenuService();
      //    var menus = mService.LstMenuPage(pSearch);

      //    string[] names = menus.Select(s => s.Menu_Page_Name).ToArray();
      //    //string[] urls = menus.Select(s => ConvertDomain(Url.Content("/"  + s.Page_Controller + "/" + s.Page_Action + "?operation=" + EncryptUtil.Encrypt(s.operation)), s.Domain_Name)).ToArray();
      //    string[] urls = menus.Select(s => ConvertDomain(Url.Content("/" + s.Domain_Name + "/" + s.Page_Controller + "/" + s.Page_Action + "?operation=" + EncryptUtil.Encrypt(s.operation)), s.Domain_Name)).ToArray();
      //    return Json(new { Menu_Page_Name = names, Page_Url = urls }, JsonRequestBehavior.AllowGet);



      //}

      // POST: /Account/LogOff
      [HttpPost]
      [ValidateAntiForgeryToken]
      public ActionResult LogOff()
      {
         AuthenticationManager.SignOut();
         return RedirectToAction("Login", "Account");
      }

      //GET: /Account/Logout
      [AllowAnonymous]
      public ActionResult Logout()
      {
         Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-Start Logout");
         AuthenticationManager.SignOut();
         HttpContext.Session.Clear();
         Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-End Logout");
         return RedirectToAction("Login", "Account");
      }


      protected override void Dispose(bool disposing)
      {
         if (disposing && UserManager != null)
         {
            UserManager.Dispose();
            UserManager = null;
         }
         base.Dispose(disposing);
      }

      #region Helpers
      // Used for XSRF protection when adding external logins
      private const string XsrfKey = "XsrfId";

      private IAuthenticationManager AuthenticationManager
      {
         get
         {
            return HttpContext.GetOwinContext().Authentication;
         }
      }

      private async Task SignInAsync(ApplicationUser user, bool isPersistent)
      {
         AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
         var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
         AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
      }

      private void AddErrors(IdentityResult result)
      {
         foreach (var error in result.Errors)
         {
            ModelState.AddModelError("", error);
         }
      }

      private bool HasPassword()
      {
         var user = UserManager.FindById(User.Identity.GetUserId());
         if (user != null)
         {
            return user.PasswordHash != null;
         }
         return false;
      }

      public enum ManageMessageId
      {
         ChangePasswordSuccess,
         SetPasswordSuccess,
         RemoveLoginSuccess,
         Error
      }

      private ActionResult RedirectToLocal(string returnUrl)
      {
         if (Url.IsLocalUrl(returnUrl))
         {
            return Redirect(returnUrl);
         }
         else
         {
            return RedirectToAction("Index", "Home");
         }
      }

      private class ChallengeResult : HttpUnauthorizedResult
      {
         public ChallengeResult(string provider, string redirectUri)
            : this(provider, redirectUri, null)
         {
         }

         public ChallengeResult(string provider, string redirectUri, string userId)
         {
            LoginProvider = provider;
            RedirectUri = redirectUri;
            UserId = userId;
         }

         public string LoginProvider { get; set; }
         public string RedirectUri { get; set; }
         public string UserId { get; set; }

         public override void ExecuteResult(ControllerContext context)
         {
            var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
            if (UserId != null)
            {
               properties.Dictionary[XsrfKey] = UserId;
            }
            context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
         }
      }

      #endregion

   }

}


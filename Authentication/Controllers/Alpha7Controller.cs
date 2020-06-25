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
using Authentication.Models;
using System.Web.Routing;
using System.IO;
using SBSModel.Models;
using SBSModel.Common;
using Authentication.Common;
using System.Configuration;
using SBSResourceAPI;
using System.Diagnostics;
using Maestrano;
using log4net;
using System.Text;
namespace Authentication.Controllers
{
    public class Alpha7Controller : Controller
    {
        private ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public UserManager<ApplicationUser> UserManager { get; private set; }
        public Alpha7Controller()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new SBS2DBContext())))
        {
        }

        public Alpha7Controller(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public ActionResult Init()
        {
            logger.Debug("********************Init*******************************");
            var request = System.Web.HttpContext.Current.Request;

            var ssoUrl = MnoHelper.Sso.BuildRequest(request.QueryString).RedirectUrl();
            return Redirect(ssoUrl);
        }

        public async Task<ActionResult> Consume()
        {
            var msg = new StringBuilder();
            logger.Debug("********************Consume*******************************");
            var request = System.Web.HttpContext.Current.Request;
            if (request.Params["SAMLResponse"] == null)
            {
                logger.Debug("null SAMLResponse");
                msg.Append("null SAMLResponse");
                msg.Append("<br/><br/>");
            }

            logger.Debug("start");
            var emodel = new ErrorPageViewModel();
            if (request.Params["SAMLResponse"] != null)
            {
                var samlResp = MnoHelper.Sso.BuildResponse(request.Params["SAMLResponse"]);
                if (samlResp != null && samlResp.IsValid())
                {
                    var mnoUser = new Maestrano.Sso.User(samlResp);
                    var mnoGroup = new Maestrano.Sso.Group(samlResp);

                    if (mnoUser == null | mnoGroup == null)
                    {
                        emodel.code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND;
                        emodel.msg = new Error().getError(emodel.code);
                        return View("../Account/ErrorPage", emodel);

                    }

                    var cpService = new CompanyService();
                    var uService = new UserService();
                    var com = cpService.GetCompanyA7(mnoGroup.Uid);
                    if (com == null)
                    {
                        /* If company is not exsit system show register end user screen*/
                        var model = new SignUpViewModel();
                        model.Company_Name = mnoGroup.CompanyName;
                        model.Email = mnoUser.Email;
                        model.A7_Group_ID = mnoGroup.Uid;
                        model.Address = mnoGroup.City;
                        model.step = 2;
                        model.First_Name = mnoUser.FirstName;
                        model.Last_Name = mnoUser.LastName;
                        model.A7_User_ID = mnoUser.Uid;
                        if (!string.IsNullOrEmpty(mnoGroup.Country))
                        {
                            var country = cpService.GetCountry(mnoGroup.Country);
                            if (country != null)
                                model.Country_ID = country.Country_ID;
                        }
                       
                        var mstr = "";
                        var modules = cpService.LstModule();
                        foreach (var m in modules)
                        {
                            if(m.Module_Detail_Name == ModuleCode.Employee)
                            {
                                mstr = mstr + m.Module_Detail_ID + "|";
                            }
                            else if (m.Module_Detail_Name == ModuleCode.Leave)
                            {
                                mstr = mstr + m.Module_Detail_ID + "|";
                            }
                        }
                        model.Select_Module_Str = mstr;
                        return RedirectToAction("ModuleSignUp", "Subscription", model);
                    }
                    else
                    {
                        /* If company is exsit checking user have exsit*/
                        var user = uService.GetUserA7(mnoUser.Uid);
                        if (user == null)
                        {

                            /* If iser is not exsit system will create new user*/
                            var currentdate = StoredProcedure.GetCurrentDate();
                            user = new User_Profile();
                            user.Company_ID = com.Company_ID;
                            user.First_Name = mnoUser.FirstName;
                            user.Last_Name = mnoUser.LastName;
                            user.Email = mnoUser.Email;
                            user.User_Status = RecordStatus.Active;
                            user.Create_On = currentdate;
                            user.Create_By = "A7";
                            user.Update_On = currentdate;
                            user.Update_By = "A7";
                            user.A7_User_ID = mnoUser.Uid;

                            var emp = new Employee_Profile();
                            emp.Create_On = currentdate;
                            emp.Create_By = "A7";
                            emp.Update_On = currentdate;
                            emp.Update_By = "A7";

                            var result = uService.InsertUserPro(user, null, null, emp);
                            if (result.Code == ERROR_CODE.SUCCESS)
                            {
                                user = uService.GetUserA7(mnoUser.Uid);
                                HttpContext.Session.Clear();
                                var aspuser = UserManager.FindById(user.User_Authentication.ApplicationUser_Id);
                                uService.updateLastConnection(user.Profile_ID);
                                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                                var identity = await UserManager.CreateIdentityAsync(aspuser, DefaultAuthenticationTypes.ApplicationCookie);
                                AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = true }, identity);
                                HttpContext.Session["User"] = user;
                                if (user.User_Profile_Photo != null && user.User_Profile_Photo.FirstOrDefault() != null)
                                    HttpContext.Session["Profile_Photo"] = user.User_Profile_Photo.FirstOrDefault().Photo;

                                var sdata = uService.GetSessionData(user.Profile_ID, Session.SessionID);
                                if (sdata != null)
                                    return Redirect(sdata.Session_Data);
                                else
                                    return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                emodel.code = result.Code;
                                emodel.msg = result.Msg;

                                return View("../Account/ErrorPage", emodel);
                            }

                        }
                        else
                        {
                            /* If user is exsit system will auto login to sbs*/
                            HttpContext.Session.Clear();
                            var aspuser = UserManager.FindById(user.User_Authentication.ApplicationUser_Id);
                            uService.updateLastConnection(user.Profile_ID);
                            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                            var identity = await UserManager.CreateIdentityAsync(aspuser, DefaultAuthenticationTypes.ApplicationCookie);
                            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = true }, identity);
                            HttpContext.Session["User"] = user;
                            if (user.User_Profile_Photo != null && user.User_Profile_Photo.FirstOrDefault() != null)
                                HttpContext.Session["Profile_Photo"] = user.User_Profile_Photo.FirstOrDefault().Photo;

                            var sdata = uService.GetSessionData(user.Profile_ID, Session.SessionID);
                            if (sdata != null)
                                return Redirect(sdata.Session_Data);
                            else
                                return RedirectToAction("Index", "Home");
                        }
                    }
                }
            }


            emodel.code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND;
            emodel.msg = msg.ToString();
            return View("../Account/ErrorPage", emodel);
        }







    }
}
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
using POS.Models;
using System.Web.Routing;
using System.IO;
using SBSModel.Models;
using SBSModel.Common;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.NetworkInformation;
using System.ComponentModel;
using System.Text;
using System.Management;

public class RightResult
{
    public ActionResult action { get; set; }
    public string[] rights { get; set; }
}

public class UserSession
{
    public static string RIGHT_A = "A";
    public static string RIGHT_C = "C";
    public static string RIGHT_U = "U";
    public static string RIGHT_D = "D";
    public static string RIGHT_ESubmission = "ESubmission";
    /*Emplyee*/

    public static string getUserName(User_Profile user)
    {
        var name = "";

        if (user != null)
        {
            if (!string.IsNullOrEmpty(user.First_Name) | !string.IsNullOrEmpty(user.Middle_Name) | !string.IsNullOrEmpty(user.Last_Name))
            {
                name = user.First_Name;
                if (!string.IsNullOrEmpty(user.Middle_Name))
                {
                    name = name + " " + user.Middle_Name;

                }
                if (!string.IsNullOrEmpty(user.Last_Name))
                {
                    name = name + " " + user.Last_Name;
                }
            }
            else
            {
                name = user.Name;
            }
        }

        return name;
    }
    public static bool havePageRightA(Dictionary<String, List<string>> rights, Dictionary<String, List<string>> subscriptions, String page, string module)
    {
        if (rights == null)
        {
            return false;
        }

        if (subscriptions == null || subscriptions.Count == 0)
        {
            return false;
        }
        if (!subscriptions.ContainsKey(module))
        {
            return false;
        }
        var sub = subscriptions[module];

        if (sub != null && sub.Contains(page))
        {
            if (rights.ContainsKey(page) && rights.Where(w => w.Key.Equals(page)).FirstOrDefault().Value.Contains(UserSession.RIGHT_A))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }

    }

    public static bool havePageRightA(Dictionary<String, List<string>> rights, String page)
    {
        if (rights == null)
        {
            return false;
        }
        if (rights.ContainsKey(page) && rights.Where(w => w.Key.Equals(page)).FirstOrDefault().Value.Contains(UserSession.RIGHT_A))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool haveModuleRight(Dictionary<String, List<string>> subscriptions, string module)
    {

        if (subscriptions != null && subscriptions.ContainsKey(module))
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public static User_Profile getUser(HttpContextBase context)
    {
        var userSession = context.Session["User"] as User_Profile;

        if (context.User.Identity.IsAuthenticated)
        {
            if (userSession == null)
            {
                UserService userService = new UserService();
                User_Profile profile = userService.getUser(context.User.Identity.GetUserId());
                context.Session["User"] = profile;
                if (profile != null && profile.User_Profile_Photo != null && profile.User_Profile_Photo.FirstOrDefault() != null)
                {
                    context.Session["Profile_Photo"] = profile.User_Profile_Photo.FirstOrDefault().Photo;
                }

                userSession = context.Session["User"] as User_Profile;
            }
        }

        return userSession;
    }


    public static byte[] getUserProfilePhoto(HttpContextBase context)
    {
        var photo = context.Session["Profile_Photo"] as byte[];
        return photo;
    }

    public static bool isPage(HttpContextBase context, String page)
    {
        String appPath = context.Request.ApplicationPath;
        string URL = context.Request.Url.AbsolutePath;
        if (!string.IsNullOrEmpty(appPath) && appPath.Length > 1)
        {
            URL = URL.Replace(appPath, "");
        }
        if (context.Request.Url.Query != null && context.Request.Url.Query.Length != 0)
        {
            URL = URL.Replace(context.Request.Url.Query, "");
        }

        URL = UserService.removeSlashes(URL);
        URL = "/" + URL;

        if (URL.ToLower().Equals(page.ToLower()))
        {
            return true;
        }

        return false;
    }

    public static User_Profile getRefreshUser(HttpContextBase context)
    {
        var userSession = context.Session["User"] as User_Profile;

        if (context.User.Identity.IsAuthenticated)
        {

            UserService userService = new UserService();
            User_Profile profile = userService.getUser(context.User.Identity.GetUserId());
            context.Session["User"] = profile;
            if (profile != null && profile.User_Profile_Photo != null && profile.User_Profile_Photo.FirstOrDefault() != null)
            {
                context.Session["Profile_Photo"] = profile.User_Profile_Photo.FirstOrDefault().Photo;
            }
            userSession = context.Session["User"] as User_Profile;

        }

        return userSession;
    }

    public static Company_Details getCompany(HttpContextBase context)
    {

        var company = context.Session["Company"] as Company_Details;
        if (company == null)
        {
            var userSession = context.Session["User"] as User_Profile;
            if (userSession != null)
            {
                CompanyService comService = new CompanyService();
                company = comService.GetCompany(userSession.Company_ID);
                context.Session["Company"] = company;
            }
        }
        return company;
    }

    public static Dictionary<String, List<string>> getSubscription(HttpContextBase context)
    {
        var subscription = context.Session["SubScription"] as Dictionary<String, List<string>>;
        if (subscription == null)
        {
            var user = getUser(context);
            if (user != null)
            {
                UserService userService = new UserService();
                var cri = new SubscriptionCriteria()
                {
                    Company_ID = user.Company_ID,
                    Profile_ID = user.Profile_ID,
                };
                context.Session["SubScription"] = userService.getSubscription(cri);
                subscription = context.Session["SubScription"] as Dictionary<String, List<string>>;
            }

        }
        return subscription;
    }

    public static Dictionary<String, List<string>> getUserPageRights(HttpContextBase context)
    {
        var userRights = context.Session["PageRight"] as Dictionary<String, List<string>>;
        if (userRights == null)
        {
            var user = getUser(context);
            if (user != null)
            {
                UserService userService = new UserService();
                context.Session["PageRight"] = userService.getUserPageRights(user.User_Authentication_ID.Value);
                userRights = context.Session["PageRight"] as Dictionary<String, List<string>>;
            }

        }
        return userRights;
    }


    public static byte[] getCompanyLogo(HttpContextBase context)
    {

        var companylogo = context.Session["Company_Logo"] as byte[];
        if (companylogo == null)
        {
            var user = getUser(context);
            if (user != null)
            {
                var companyService = new CompanyService();
                var logo = companyService.GetLogo(user.Company_ID);
                if (logo != null && logo.Logo != null)
                {
                    context.Session["Company_Logo"] = logo.Logo;
                    return logo.Logo;
                }
            }

        }
        return companylogo;
    }
    public static string GetMsg(int code, string Msg, string processName = "")
    {
        var msg = "";
        if (code > 0)
        {
            if (!string.IsNullOrEmpty(processName))
            {
                msg = processName + " " + Msg;
            }
            else
            {
                msg = "Your information " + Msg;
            }
        }
        else
        {
            if (code == ERROR_CODE.ERROR_501_CANT_SEND_EMAIL)
            {
                if (!string.IsNullOrEmpty(processName))
                {
                    msg = "Your information has been submit successfully.";
                }
                else
                {
                    msg = processName + " has been submit successfully.";
                }

                msg += "\n But Can't send emails.";
            }
            else
            {
                if (!string.IsNullOrEmpty(processName))
                {
                    msg = processName + " Error.";
                }
                else
                {
                    msg = "Error.";
                }
                msg += "\n" + Msg;
            }

        }
        return msg;
    }

    public static string GetMAC()
    {
        string macAddresses = "";

        ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
        ManagementObjectCollection moc = mc.GetInstances();
        string MACAddress = String.Empty;
        foreach (ManagementObject mo in moc)
        {
            if (MACAddress == String.Empty)  // only return MAC Address from first card
            {
                if ((bool)mo["IPEnabled"] == true) MACAddress = mo["MacAddress"].ToString();
            }
            mo.Dispose();
        }

        macAddresses = MACAddress.Replace(":", "");
        return macAddresses;
    }

   
}

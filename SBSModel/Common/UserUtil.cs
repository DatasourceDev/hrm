using SBSModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Principal;
using System.Diagnostics;

namespace SBSModel.Common
{
   public class UserItem
   {
      public User_Profile userlogin { get; set; }
      public int EmployeeProfileID { get; set; }
      public string username { get; set; }
      public string email { get; set; }
      public string userphoto { get; set; }
      public string companyname { get; set; }
      public int companycountryID { get; set; }
      public bool isindent { get; set; }
     // public string CountryName { get; set; }//Added by Moet

      public string bg { get; set; }
      public string logo { get; set; }

      public AuthenMode aMode { get; set; }

      public MainMenu menu { get; set; }
      public bool IsAdminHR { get; set; }
      public bool IsCompleteProfile { get; set; }
      public bool IsSkipTour { get; set; }
      public bool OverduePayemnt { get; set; }

      public bool Is_Indent { get; set; }
      
   }

   public class PageMenu
   {
      public string Page_Name { get; set; }
      private string _pageurl { get; set; }
      public string Page_Url
      {
         get
         {
            return _pageurl;
         }
         set
         {
            _pageurl = value;
            if (!string.IsNullOrEmpty(_pageurl))
            {
               var split = _pageurl.Split('/');
               if (split.Length == 3)
               {
                  Controller = split[1];
                  Action = split[2];
               }
            }
         }
      }
      public string Action { get; set; }
      public string Controller { get; set; }
      public int? Index { get; set; }
   }
   public class MainMenu
   {
      public int? Profile_ID { get; set; }
      public List<PageMenu> topRights { get; set; }
      public List<LeftMenu> lefts { get; set; }
   }

   public class LeftMenu
   {
      public string Menu_Name { get; set; }
      public PageMenu left { get; set; }
      public List<PageMenu> submenu { get; set; }
   }

   public enum AuthenMode
   {
      None,
      Authenticated,
      IncorrectUser,
      UserNoFound,
   }

   public class UserUtil
   {
      public static UserItem GetUserItem(HttpContextBase context, bool getmenu = true)
      {
         Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-Start GetUserItem");
         var uitem = new UserItem();

         uitem.userlogin = getRefreshUser(context);
         if (uitem.userlogin != null)
         {
            uitem.bg = uitem.userlogin.Bg;                        
            var com = getCompany(context);
            if (com != null)
            {
               uitem.isindent = com.Is_Indent.HasValue ? com.Is_Indent.Value : false;
               uitem.companyname = com.Name;
               uitem.companycountryID = com.Country_ID.Value;
               uitem.Is_Indent = com.Is_Indent.HasValue ? com.Is_Indent.Value : false;
            }
            uitem.email = uitem.userlogin.User_Authentication.Email_Address;
            if (getmenu)
               uitem.menu = getMenu(context, uitem.userlogin, uitem.Is_Indent);
            uitem.username = AppConst.GetUserName(uitem.userlogin);

            if (uitem.userlogin.User_Profile_Photo.FirstOrDefault() != null && uitem.userlogin.User_Profile_Photo.FirstOrDefault().Photo != null)
               uitem.userphoto = String.Format("data:image/gif;base64,{0}", Convert.ToBase64String(uitem.userlogin.User_Profile_Photo.FirstOrDefault().Photo));
            else
               uitem.userphoto = AppSetting.SERVER_NAME + "SBSTmpAPI/images/avatar.jpg";


            //Edit by sun 09-09-2016 
            if (uitem.userlogin.User_Authentication.Is_Email.HasValue && uitem.userlogin.User_Authentication.Is_Email.Value)
            {
               Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-Start GetUser By Email Address");
               var email = uitem.userlogin.User_Authentication.Email_Address;
               if (email != null)
               {
                  if (context.User.Identity.IsAuthenticated && email.ToLower() == context.User.Identity.Name.ToLower())
                     uitem.aMode = AuthenMode.Authenticated;
                  else if (context.User.Identity.IsAuthenticated && email.ToLower() != context.User.Identity.Name.ToLower())
                     uitem.aMode = AuthenMode.UserNoFound;
                  else if (email.ToLower() != context.User.Identity.Name.ToLower())
                     uitem.aMode = AuthenMode.IncorrectUser;
               }
               else
                  uitem.aMode = AuthenMode.IncorrectUser;

               Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-End GetUser By Email Address");
            }
            else if (!uitem.userlogin.User_Authentication.Is_Email.HasValue)
            {
               //Old User Is_Email field  Is null,  no Update  redirect to Used Email flow ? 
               Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-Start GetUser By Email Address");
               var email = uitem.userlogin.User_Authentication.Email_Address;
               if (email != null)
               {
                  if (context.User.Identity.IsAuthenticated && email.ToLower() == context.User.Identity.Name.ToLower())
                     uitem.aMode = AuthenMode.Authenticated;
                  else if (context.User.Identity.IsAuthenticated && email.ToLower() != context.User.Identity.Name.ToLower())
                     uitem.aMode = AuthenMode.UserNoFound;
                  else if (email.ToLower() != context.User.Identity.Name.ToLower())
                     uitem.aMode = AuthenMode.IncorrectUser;
               }
               else
                  uitem.aMode = AuthenMode.IncorrectUser;

               Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-End GetUser By Email Address");
            }
            else
            {
               Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-Start GetUser By User Name");
               var userName = uitem.userlogin.User_Authentication.User_Name;
               if (userName != null)
               {
                  if (context.User.Identity.IsAuthenticated && userName.ToLower() == context.User.Identity.Name.ToLower())
                     uitem.aMode = AuthenMode.Authenticated;
                  else if (context.User.Identity.IsAuthenticated && userName.ToLower() != context.User.Identity.Name.ToLower())
                     uitem.aMode = AuthenMode.UserNoFound;
                  else if (userName.ToLower() != context.User.Identity.Name.ToLower())
                     uitem.aMode = AuthenMode.IncorrectUser;
               }
               else
                  uitem.aMode = AuthenMode.IncorrectUser;

               Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-End  GetUser By User Name");
            }
         }
         else
         {
            if (context.User != null && context.User.Identity.IsAuthenticated)
               uitem.aMode = AuthenMode.UserNoFound;
            else
               uitem.aMode = AuthenMode.None;
         }

         if (string.IsNullOrEmpty(uitem.bg))
            uitem.bg = "bg-1";

         //Added by Moet on 19/Sep/2016    
         uitem.IsAdminHR = false;
         if (uitem.userlogin != null)
         {                          
             var assignRoles = StoredProcedure.GetUserRolesByUser(uitem.userlogin.User_Authentication_ID.Value);
             if (assignRoles != null)
             {
                 foreach (var r in assignRoles)
                 {
                     if (r.IndexOf("administrator") > -1)
                     {
                         uitem.IsAdminHR = true;
                         break;
                     }
                     if (r.IndexOf("HR Admin") > -1)
                     {
                         uitem.IsAdminHR = true;
                         break;
                     }
                 }
             }             
             if (uitem.userlogin.Is_Tour_Skip == null)
                 uitem.IsSkipTour = false;
             else
                 uitem.IsSkipTour = uitem.userlogin.Is_Tour_Skip.Value;

             var empService = new EmployeeService();
             var emp = empService.GetEmployeeProfileByProfileID(uitem.userlogin.Profile_ID);
             uitem.EmployeeProfileID = emp.Employee_Profile_ID;
             if (emp.Employment_History.Count > 0)
                 uitem.IsCompleteProfile = true;
             else
                 uitem.IsCompleteProfile = false;
         }
         else
         {
             uitem.IsSkipTour = false;
             uitem.IsCompleteProfile = false;
         }
             
                               
        
         
         Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-End GetUserItem");
         return uitem;
      }

      public static Company_Details getCompany(HttpContextBase context)
      {
         var company = context.Session["Company"] as Company_Details;
         if (company == null)
         {
            var userSession = context.Session["User"] as User_Profile;
            if (userSession != null)
            {
               var comService = new CompanyService();
               company = comService.GetCompany(userSession.Company_ID);
               context.Session["Company"] = company;
               context.Session["MainMenu"] = null;
            }
         }
         else
         {
            var userSession = context.Session["User"] as User_Profile;
            if (userSession != null)
            {
               if (company.Company_ID != userSession.Company_ID)
               {
                  var comService = new CompanyService();
                  company = comService.GetCompany(userSession.Company_ID);
                  context.Session["Company"] = company;
                  context.Session["MainMenu"] = null;
               }
            }
         }
         return company;
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
      public static User_Profile getRefreshUser(HttpContextBase context)
      {
         var userSession = context.Session["User"] as User_Profile;

         if (context.User.Identity.IsAuthenticated)
         {

            var userService = new UserService();
            var profile = userService.getUser(context.User.Identity.GetUserId());
            context.Session["User"] = profile;
            if (profile != null && profile.User_Profile_Photo != null && profile.User_Profile_Photo.FirstOrDefault() != null)
               context.Session["Profile_Photo"] = profile.User_Profile_Photo.FirstOrDefault().Photo;
            userSession = context.Session["User"] as User_Profile;
         }
         return userSession;
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
      public static MainMenu getMenu(HttpContextBase context, User_Profile userlogin = null, bool IsIndent = false)
      {
         var menu = context.Session["MainMenu"] as MainMenu;
         if (menu == null)
         {
            var userSession = userlogin;
            if (userSession == null)
               userSession = getUser(context);

            var companySession = context.Session["Company"] as Company_Details;
            if (userSession != null)
            {
               var userService = new UserService();
               var cri = new MenuCriteria()
               {
                  Company_ID = userSession.Company_ID,
                  Profile_ID = userSession.Profile_ID,
                  User_Authentication_ID = userSession.User_Authentication_ID,
                  User_Assign_Role = userlogin.User_Authentication.User_Assign_Role,
                  Is_Indent = IsIndent,
                  Company_Dtls = companySession
               };
               context.Session["MainMenu"] = userService.getMenu(cri);
               menu = context.Session["MainMenu"] as MainMenu;
            }

         }
         else
         {
            var userSession = userlogin;
            if (userSession == null)
               userSession = getUser(context);
            var companySession = context.Session["Company"] as Company_Details;
            if (userSession != null)
            {
               if (userSession.Profile_ID != menu.Profile_ID)
               {
                  var userService = new UserService();
                  var cri = new MenuCriteria()
                  {
                     Company_ID = userSession.Company_ID,
                     Profile_ID = userSession.Profile_ID,
                     User_Authentication_ID = userSession.User_Authentication_ID,
                     User_Assign_Role = userlogin.User_Authentication.User_Assign_Role,
                     Is_Indent = IsIndent,
                     Company_Dtls = companySession
                  };
                  context.Session["MainMenu"] = userService.getMenu(cri);
                  menu = context.Session["MainMenu"] as MainMenu;
               }
            }
         }
         return menu;
      }



   }
}

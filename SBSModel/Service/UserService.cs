using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using SBSModel.Common;
using SBSWorkFlowAPI.Models;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;

using SBSResourceAPI;
using System.Diagnostics;

namespace SBSModel.Models
{
   public class UserCriteria : CriteriaBase
   {
      public Nullable<int> Company_ID { get; set; }
      public string A7_User_ID { get; set; }
      public string Email { get; set; }
      public string User_Name { get; set; }

      public bool Is_Belong_To { get; set; }

   }

   public class MenuCriteria : CriteriaBase
   {
      public ICollection<User_Assign_Role> User_Assign_Role { get; set; }
      //public string Country_Name { get; set; }
      public bool IsOverdued { get; set; }
      public Company_Details Company_Dtls { get; set; }
      public bool Is_Indent { get; set; }

   }

   public class SubscriptionCriteria : CriteriaBase
   {
      public string Module_Name { get; set; }
      public string Module_Detail_Name { get; set; }
      public Nullable<int> Module_Detail_ID { get; set; }
   }

   public class UserService
   {
      #region Authen

      public static int ROLE_MAIN_MASTER_ADMIN = 1;
      public static int ROLE_FRANCHISE_ADMIN = 2;
      public static int ROLE_WHITE_LABEL_ADMIN = 3;
      public static int ROLE_CUSTOMER_ADMIN = 4;
      public static int ROLE_CUSTOMER_USER = 5;
      public static int LINK_TIME_LIMIT = 24;

      public List<Notification> LstApproval(Nullable<int> pCompanyID, Nullable<int> pProfileID)
      {
         var lServive = new LeaveService();
         var eService = new ExpenseService();

         var notices = new List<Notification>();

         var leaves = lServive.LstLeaveApplicationDocument(pCompanyID);
         foreach (var row in leaves)
         {
            var notice = new Notification()
            {
               Address_While_On_Leave = row.Address_While_On_Leave,
               Contact_While_Overseas = row.Contact_While_Overseas,
               Date_Applied = row.Date_Applied,
               Days_Taken = row.Days_Taken,
               Employee_Profile_ID = row.Employee_Profile_ID,
               End_Date = row.End_Date,
               End_Date_Period = row.End_Date_Period,
               Last_Date_Approved = row.Last_Date_Approved,
               Leave_Application_Document_ID = row.Leave_Application_Document_ID,
               Leave_Config_Detail_ID = row.Leave_Config_Detail_ID,
               Leave_Config_ID = row.Leave_Config_ID,
               Overall_Status = row.Overall_Status,
               Period = row.Period,
               Reasons = row.Reasons,
               Remark = row.Remark,
               Request_ID = row.Request_ID,
               Start_Date = row.Start_Date,
               Start_Date_Period = row.Start_Date_Period,
               Leave_Name = row.Leave_Config.Leave_Name
            };
            notices.Add(notice);
         }

         var expenses = new List<Expenses_Application>();
         var criteriaPending = new ExpenseCriteria() { Company_ID = pCompanyID, };
         var presult = eService.LstExpenses(criteriaPending);
         if (presult.Object != null)
            expenses = (List<Expenses_Application>)presult.Object;
         //var expenses = eService.getExpenseApplications(pCompanyID);
         foreach (var row in expenses)
         {
            var notice = new Notification()
            {
               Expenses_Application_Document = row.Expenses_Application_Document.ToList(),
               Expenses_Application_ID = row.Expenses_Application_ID,
               Expenses_No = row.Expenses_No,
               Expenses_Title = row.Expenses_Title,
               Date_Applied = row.Date_Applied,
               Employee_Profile_ID = row.Employee_Profile_ID,
               Last_Date_Approved = row.Last_Date_Approved,
               Overall_Status = row.Overall_Status,
               //Remark = row.Remark,
               Request_ID = row.Request_ID,
            };
            notices.Add(notice);
         }



         notices = notices.OrderByDescending(o => o.Date_Applied).ToList();
         return notices;
      }

      public bool SaveSessionData(User_Session_Data pSData)
      {
         var curdate = StoredProcedure.GetCurrentDate().AddDays(-7);
         try
         {
            using (var db = new SBS2DBContext())
            {
               var oldSession = db.User_Session_Data.Where(w => w.Profile_ID == pSData.Profile_ID & w.Create_On <= curdate);
               if (oldSession != null)
                  db.User_Session_Data.RemoveRange(oldSession);

               db.User_Session_Data.Add(pSData);
               db.SaveChanges();
               return true;
            }
         }
         catch
         {

         }
         return false;
      }
      public User_Session_Data GetSessionData(Nullable<int> pProfileID, string pSessionID)
      {
         var curdate = StoredProcedure.GetCurrentDate().AddDays(-7);
         using (var db = new SBS2DBContext())
         {

            var sdata = db.User_Session_Data
                .Where(w => w.Profile_ID == pProfileID & w.Session_ID == pSessionID & w.Actived == true & w.Create_On >= curdate)
                .OrderByDescending(o => o.Create_On)
                .FirstOrDefault();

            if (sdata != null)
               sdata.Actived = false;

            return sdata;
         }

      }

      public bool DeleteSessionData(Nullable<int> pProfileID, string pSessionID)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               var cur = db.User_Session_Data.Where(w => w.Profile_ID == pProfileID && w.Session_ID == pSessionID).FirstOrDefault();
               if (cur != null)
               {
                  db.User_Session_Data.Remove(cur);
                  db.SaveChanges();
               }
               return true;
            }
         }
         catch
         {
            return false;
         }
      }
      public Dictionary<String, List<string>> getSubscription(SubscriptionCriteria criteria)
      {
         var subPermisstion = new Dictionary<String, List<string>>();
         var currentdate = StoredProcedure.GetCurrentDate();
         using (var db = new SBS2DBContext())
         {
            var modules = db.SBS_Module_Detail.Distinct();

            if (!string.IsNullOrEmpty(criteria.Module_Detail_Name))
            {
               modules = modules.Where(w => w.Module_Detail_Name == criteria.Module_Detail_Name);
            }
            if (criteria.Module_Detail_ID.HasValue)
            {
               modules = modules.Where(w => w.Module_Detail_ID == criteria.Module_Detail_ID);
            }

            foreach (var d in modules)
            {
               var subPer = new List<string>();
               var subs = db.Subscriptions.Where(w => w.Module_Detail_ID == d.Module_Detail_ID);
               if (criteria.Company_ID.HasValue)
               {
                  subs = subs.Where(w => w.Company_ID == criteria.Company_ID);
               }

               var c = subs.ToList();
               foreach (var sub in subs)
               {
                  var pmonth = 0;
                  if (sub.Period_Month.HasValue)
                  {
                     pmonth = sub.Period_Month.Value;
                  }
                  var expiredate = sub.Start_Date.Value.AddMonths(pmonth);

                  if (expiredate >= currentdate)
                  {
                     if (criteria.Profile_ID.HasValue)
                     {
                        if (sub.User_Assign_Module.Where(w => w.Profile_ID == criteria.Profile_ID).FirstOrDefault() != null)
                        {
                           foreach (var p in d.Pages)
                           {
                              if (!subPer.Contains(p.Page_Url))
                                 subPer.Add(p.Page_Url);
                           }
                        }

                     }
                     if (criteria.User_Authentication_ID.HasValue)
                     {
                        if (sub.User_Assign_Module.Where(w => w.User_Profile.User_Authentication_ID == criteria.User_Authentication_ID).FirstOrDefault() != null)
                        {
                           foreach (var p in d.Pages)
                           {
                              if (!subPer.Contains(p.Page_Url))
                                 subPer.Add(p.Page_Url);
                           }
                        }

                     }
                     else
                     {
                        foreach (var p in d.Pages)
                        {
                           if (!subPer.Contains(p.Page_Url))
                              subPer.Add(p.Page_Url);
                        }
                     }

                  }
               }

               if (subPer.Count > 0)
               {
                  if (!subPermisstion.Keys.Contains(d.Module_Detail_Name))
                  {
                     subPermisstion.Add(d.Module_Detail_Name, subPer);
                  }

               }
            }





         }
         return subPermisstion;
      }
      //Added by Moet on 4 Sep
      public bool IsOverdue(int Company_Id)
      {
         var currentdate = StoredProcedure.GetCurrentDate();
         DateTime LastBillGenerateon = StoredProcedure.GetCurrentDate();
         var sServcie = new SubscriptionService();
         var subs = sServcie.GetInvoiceList(Company_Id, currentdate.Year);
         if (subs != null)
         {
            var unpaidMonth = 0;
            var outstandingMonth = 0;
            var gracePeriod = 30;
            for (var i = 0; i < subs.Count; i++)
            {
               var o = subs[i];
               if (i == 0)
               {
                  if (o.Invoice_Status == PaymentStatus.Unpaid)
                  {
                     unpaidMonth = o.Invoice_Month.Value;
                     LastBillGenerateon = o.Generated_On.Value;
                  }
                  else
                     break;
               }
               else
               {
                  if (o.Invoice_Status == PaymentStatus.Outstanding && o.Invoice_Month < unpaidMonth)
                  {
                     outstandingMonth = o.Invoice_Month.Value;
                     break;
                  }
                  else
                     break;
               }
            }
            // if there is outstanding bill and new unpaid bill is there, it mean already over 1 month grace period
            if (unpaidMonth > 0 && outstandingMonth > 0)
            {
               return true;
            }
            // if there is no outstanding bill and new unpaid bill is less than generage on + grace period, it mean already over 1 month grace period
            if (outstandingMonth == 0 && LastBillGenerateon.AddDays(gracePeriod) < currentdate)
            {
               return true;
            }
         }
         return false;
      }
      public MainMenu getMenu(MenuCriteria criteria)
      {
         Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-Start getMenu");
         var menu = new MainMenu();
         menu.topRights = new List<PageMenu>();
         menu.lefts = new List<LeftMenu>();
         menu.Profile_ID = criteria.Profile_ID;

         //To validate for the overdue
         //Added by Moet on 4 Sep   
         criteria.IsOverdued = IsOverdue(criteria.Company_ID.Value);

         var currentdate = StoredProcedure.GetCurrentDate();
         using (var db = new SBS2DBContext())
         {
            var atrPage = db.Pages
               .Where(w => w.Module_Detail_ID == null && w.Position == MenuPosition.TopRight && w.Displayed == true)
               .OrderBy(o => o.Order_Index);


            foreach (var p in atrPage)
            {
               if (p.Page_Role.Select(s => s.User_Role).Count() > 0)
               {
                  if (criteria.User_Assign_Role != null)
                  {
                     var uroleIDs = criteria.User_Assign_Role.Where(w => w.User_Authentication_ID == criteria.User_Authentication_ID).Select(s => s.User_Role_ID);
                     var uids = uroleIDs.ToList();
                     var prole = p.Page_Role;
                     if (prole.Where(w => uroleIDs.Contains(w.User_Role_ID)).FirstOrDefault() != null)
                        menu.topRights.Add(new PageMenu() { Page_Name = Page_Name.Get_Page_Name(p.Page_Name.Trim()), Page_Url = p.Page_Url, Index = p.Order_Index });

                  }
               }
               else
                  menu.topRights.Add(new PageMenu() { Page_Name = Page_Name.Get_Page_Name(p.Page_Name.Trim()), Page_Url = p.Page_Url, Index = p.Order_Index });
            }

            var dPage = db.Pages.Where(w => w.Module_Detail_ID == null && w.Position == MenuPosition.Left && w.Displayed == true);
            if (dPage.Count() > 0)
            {
               var aMenu = new LeftMenu();
               aMenu.Menu_Name = ModuleCode.Authentication;
               aMenu.submenu = new List<PageMenu>();
               foreach (var p in dPage)
               {
                  if (p.Page_Role.Select(s => s.User_Role).Count() > 0)
                  {
                     if (criteria.User_Assign_Role != null)
                     {
                        var uroleIDs = criteria.User_Assign_Role.Where(w => w.User_Authentication_ID == criteria.User_Authentication_ID).Select(s => s.User_Role_ID);
                        var prole = p.Page_Role;
                        if (prole.Where(w => uroleIDs.Contains(w.User_Role_ID)).FirstOrDefault() != null)
                        {
                           if (criteria.Is_Indent)
                              aMenu.submenu.Add(new PageMenu() { Page_Name = Page_Name.Get_Page_Name(p.Page_Name.Trim()), Page_Url = p.Page_Url, Index = p.Order_Index });
                           else
                           {
                              if (!p.Is_Indent.HasValue || p.Is_Indent.Value == false)
                                 aMenu.submenu.Add(new PageMenu() { Page_Name = Page_Name.Get_Page_Name(p.Page_Name.Trim()), Page_Url = p.Page_Url, Index = p.Order_Index });
                           }
                        }

                     }
                  }
                  else
                  {
                     if (criteria.Is_Indent)
                        aMenu.submenu.Add(new PageMenu() { Page_Name = Page_Name.Get_Page_Name(p.Page_Name.Trim()), Page_Url = p.Page_Url, Index = p.Order_Index });
                     else
                     {
                        if (!p.Is_Indent.HasValue || p.Is_Indent.Value == false)
                           aMenu.submenu.Add(new PageMenu() { Page_Name = Page_Name.Get_Page_Name(p.Page_Name.Trim()), Page_Url = p.Page_Url, Index = p.Order_Index });
                     }
                  }
               }
               menu.lefts.Add(aMenu);
            }

            foreach (var d in db.SBS_Module_Detail.Distinct().OrderBy(o => o.Order_Index))
            {
               var subPer = new List<Page>();
               var subs = db.Subscriptions.Where(w => w.Module_Detail_ID == d.Module_Detail_ID);
               if (criteria.Company_ID.HasValue)
                  subs = subs.Where(w => w.Company_ID == criteria.Company_ID);

               var modulename = d.Module_Detail_Name;
               foreach (var sub in subs)
               {
                  var pmonth = 0;
                  if (sub.Period_Month.HasValue)
                     pmonth = sub.Period_Month.Value;

                  var expiredate = sub.Start_Date.Value.AddMonths(pmonth);
                  //Modified by Moet
                  if (criteria.IsOverdued == false) //if (expiredate >= currentdate)
                  {
                     if (criteria.Profile_ID.HasValue & criteria.User_Authentication_ID.HasValue)
                     {
                        if (sub.User_Assign_Module.Count() > 0)
                        {
                           if (sub.User_Assign_Module.Where(w => w.Profile_ID == criteria.Profile_ID).FirstOrDefault() != null)
                           {
                              if (d.Pages.Where(w => w.Displayed == true).Count() > 0)
                              {
                                 foreach (var p in d.Pages.Where(w => w.Displayed == true))
                                 {
                                    var pagename = p.Page_Name;
                                    if (p.Page_Role.Count() > 0)
                                    {
                                       if (p.Page_Role.Select(s => s.User_Role).Count() > 0)
                                       {
                                          if (criteria.User_Assign_Role != null)
                                          {
                                             var uroleIDs = criteria.User_Assign_Role.Where(w => w.User_Authentication_ID == criteria.User_Authentication_ID).Select(s => s.User_Role_ID);
                                             var uroles = uroleIDs.ToList();
                                             var prole = p.Page_Role;
                                             if (prole.Where(w => uroleIDs.Contains(w.User_Role_ID)).FirstOrDefault() != null)
                                                if (!subPer.Contains(p))
                                                   subPer.Add(p);
                                          }
                                       }
                                       else
                                       {
                                          //if (!subPer.Contains(p))
                                          //    subPer.Add(p);
                                       }
                                    }
                                 }
                              }
                           }
                        }

                     }
                  }
               }
               if (subPer.Count > 0)
               {
                  var topright = subPer.Where(w => w.Position == MenuPosition.TopRight)
                      .Where(w => w.Displayed == true)
                      .Select(s => new PageMenu()
                      {
                         Page_Name = Page_Name.Get_Page_Name(s.Page_Name.Trim()),
                         Page_Url = s.Page_Url,
                      })
                      .OrderBy(o => o.Page_Name);

                  if (topright.Count() > 0)
                     menu.topRights.AddRange(topright);

                  var menumodule = new LeftMenu();
                  menumodule.Menu_Name = d.Module_Detail_Name;
                  if (d.Module_Detail_Name == ModuleCode.Payroll)
                  {
                     if (criteria.Company_Dtls != null)
                     {
                        var dtl = criteria.Company_Dtls;
                        if (dtl.patUser_ID != null && dtl.patPassword != null && dtl.CPF_Submission_No != null)
                        {
                           menumodule.submenu = subPer.Where(w => w.Position == MenuPosition.Left)
                          .Where(w => w.Displayed == true)
                          .Select(s => new PageMenu()
                          {
                             Page_Name = Page_Name.Get_Page_Name(s.Page_Name.Trim()),
                             Page_Url = s.Page_Url,
                             Index = s.Order_Index
                          })
                          .OrderBy(o => o.Index)
                          .ToList();
                        }
                        else
                           menumodule.submenu = subPer.Where(w => w.Position == MenuPosition.Left)
                          .Where(w => w.Displayed == true && !w.Page_Name.StartsWith("PAT"))
                          .Select(s => new PageMenu()
                          {
                             Page_Name = Page_Name.Get_Page_Name(s.Page_Name.Trim()),
                             Page_Url = s.Page_Url,
                             Index = s.Order_Index
                          })
                          .OrderBy(o => o.Index)
                          .ToList();
                     }
                     else
                     {
                        if (criteria.Company_Dtls == null)
                        {
                           menumodule.submenu = subPer.Where(w => w.Position == MenuPosition.Left)
                         .Where(w => w.Displayed == true && !w.Page_Name.StartsWith("PAT"))
                         .Select(s => new PageMenu()
                         {
                            Page_Name = Page_Name.Get_Page_Name(s.Page_Name.Trim()),
                            Page_Url = s.Page_Url,
                            Index = s.Order_Index
                         })
                         .OrderBy(o => o.Index)
                         .ToList();
                        }
                        else
                        {
                           menumodule.submenu = subPer.Where(w => w.Position == MenuPosition.Left)
                         .Where(w => w.Displayed == true)
                         .Select(s => new PageMenu()
                         {
                            Page_Name = Page_Name.Get_Page_Name(s.Page_Name.Trim()),
                            Page_Url = s.Page_Url,
                            Index = s.Order_Index
                         })
                         .OrderBy(o => o.Index)
                         .ToList();
                        }

                     }
                  }
                  else
                  {
                     menumodule.submenu = subPer.Where(w => w.Position == MenuPosition.Left)
                       .Where(w => w.Displayed == true)
                       .Select(s => new PageMenu()
                       {
                          Page_Name = Page_Name.Get_Page_Name(s.Page_Name.Trim()),
                          Page_Url = s.Page_Url,
                          Index = s.Order_Index
                       })
                       .OrderBy(o => o.Index)
                       .ToList();
                  }
                  menu.lefts.Add(menumodule);
               }
            }
         }
         Debug.WriteLine("'***********APP DEBUG***********' " + DateTime.Now + "-End getMenu");
         return menu;
      }

      public List<User_Profile> LstUser(Nullable<int> pCompanyID, Nullable<DateTime> pUpdateOn = null)
      {
         using (var db = new SBS2DBContext())
         {
            var users = db.User_Profile
               .Include(i => i.User_Authentication)
               .Include(i => i.User_Profile_Photo)
               .Where(w => w.Company_ID == pCompanyID)
               ;

            if (pUpdateOn.HasValue)
               users = users
                   .Where(w => EntityFunctions.CreateDateTime(w.Update_On.Value.Year, w.Update_On.Value.Month, w.Update_On.Value.Day, w.Update_On.Value.Hour, w.Update_On.Value.Minute, w.Update_On.Value.Second) > EntityFunctions.CreateDateTime(pUpdateOn.Value.Year, pUpdateOn.Value.Month, pUpdateOn.Value.Day, pUpdateOn.Value.Hour, pUpdateOn.Value.Minute, pUpdateOn.Value.Second) |
                       EntityFunctions.CreateDateTime(w.User_Authentication.Update_On.Value.Year, w.User_Authentication.Update_On.Value.Month, w.User_Authentication.Update_On.Value.Day, w.User_Authentication.Update_On.Value.Hour, w.User_Authentication.Update_On.Value.Minute, w.User_Authentication.Update_On.Value.Second) > EntityFunctions.CreateDateTime(pUpdateOn.Value.Year, pUpdateOn.Value.Month, pUpdateOn.Value.Day, pUpdateOn.Value.Hour, pUpdateOn.Value.Minute, pUpdateOn.Value.Second));

            return users.ToList();
         }
      }

      public List<string> LstEmail(Nullable<int> pCompanyID)
      {
         using (var db = new SBS2DBContext())
         {
            return db.User_Profile.Where(w => w.Company_ID == pCompanyID && w.User_Status != RecordStatus.Delete).Select(s => s.User_Authentication.Email_Address).ToList();
         }
      }

      public List<User_Profile> getUsers(Nullable<int> pCompanyID = null, string pSearchVal = "", string pStatus = "")
      {
         using (var db = new SBS2DBContext())
         {
            List<User_Profile> users = null;
            if (pCompanyID.HasValue)
            {
               users = db.User_Profile
               .Include(i => i.Company)
               .Include(i => i.Company.Company_Details)
               .Include(i => i.User_Authentication)
               .Include(e => e.Employee_Profile)
               .Where(c => c.Company_ID == pCompanyID.Value && c.User_Status != RecordStatus.Delete)
               .OrderBy(o => o.First_Name).ToList();
            }
            else
            {
               users = db.User_Profile
              .Include(i => i.Company)
              .Include(i => i.Company.Company_Details)
              .Include(i => i.User_Authentication)
              .Include(e => e.Employee_Profile)
              .Where(c => c.User_Status != RecordStatus.Delete)
              .OrderBy(o => o.First_Name).ToList();
            }

            if (!string.IsNullOrEmpty(pSearchVal))
            {
               users = users.Where(w => w.First_Name.Contains(pSearchVal) | w.First_Name == pSearchVal | w.User_Authentication.Email_Address.Contains(pSearchVal.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(pStatus))
            {
               users = users.Where(w => w.User_Status == pStatus).ToList();
            }

            if (users.Count > 0)
            {
               foreach (var u in users)
               {
                  if (u.Email == null)
                  {
                     u.Email = u.User_Name;
                     u.User_Authentication.Email_Address = u.User_Name;
                  }

               }
            }
            return users;
         }
      }

      public List<User_Profile> getUsersBelongTocompany(int pCompanyID, string pSearchVal = "", string pStatus = "")
      {
         using (var db = new SBS2DBContext())
         {
            //Get users in company
            //Edit By sun 08-09-2015
            List<User_Profile> users = getUsers(pCompanyID, pSearchVal, pStatus);

            //Get users of company that belongto this company
            var belongs = db.Company_Details
                .Where(w => w.Belong_To_ID == pCompanyID)
                .Select(s => s.Company_ID).Distinct();
            if (belongs != null && belongs.Count() > 0)
            {
               foreach (int belong in belongs)
               {
                  if (belong != pCompanyID)
                  {

                     users.AddRange(getUsersBelongTocompany(belong));
                  }
               }
            }

            return users;
         }
      }

      //Get Individual user
      public User_Profile getUser(Nullable<int> Profile_ID, bool fullInclude = true)
      {
         using (var db = new SBS2DBContext())
         {
            var users = db.User_Profile
                .Include(i => i.User_Authentication)
                .Include(i => i.Employee_Profile)
                .Where(i => i.Profile_ID == Profile_ID);

            if (fullInclude)
               users = users.Include(i => i.User_Profile_Photo)
               .Include(i => i.User_Authentication.User_Assign_Role)
               .Include(i => i.User_Authentication.Activation_Link);


            return users.FirstOrDefault();
         }
      }

      //Get Individual user
      public User_Transactions getUserTransaction(Nullable<int> Profile_ID)
      {
         using (var db = new SBS2DBContext())
         {
            var ut = db.User_Transactions
                .Where(i => i.Profile_ID == Profile_ID);

            return ut.FirstOrDefault();
         }
      }

      public List<User_Transactions> getUserTransactions(int pCompanyID)
      {
         using (var db = new SBS2DBContext())
         {
            var ut = db.User_Transactions
                .Where(i => i.Company_ID == pCompanyID).ToList();

            return ut;
         }
      }

      //Get Individual user
      public User_Profile getUser(String AspNetUser_ID)
      {
         using (var db = new SBS2DBContext())
         {
            try
            {
               User_Profile user = db.User_Profile
                  .Include(i => i.User_Authentication)
                  .Include(i => i.User_Profile_Photo)
                  .Include(i => i.Employee_Profile)
                  .Include(i => i.User_Authentication.User_Assign_Role)
                  .Where(i => i.User_Authentication.ApplicationUser_Id.Equals(AspNetUser_ID))
                  .FirstOrDefault();
               return user;
            }
            catch (DbEntityValidationException ex)
            {
               // Retrieve the error messages as a list of strings.
               var errorMessages = ex.EntityValidationErrors
                       .SelectMany(x => x.ValidationErrors)
                       .Select(x => x.ErrorMessage);

               // Join the list to a single string.
               var fullErrorMessage = string.Join("; ", errorMessages);

               // Combine the original exception message with the new one.
               var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

               // Throw a new DbEntityValidationException with the improved exception message.
               throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
         }
      }

      public User_Profile GetUserA7(string pA7UserID)
      {
         using (var db = new SBS2DBContext())
         {

            var user = db.User_Profile
                .Where(w => w.A7_User_ID == pA7UserID)
                .Include(i => i.User_Authentication)
                .Include(i => i.User_Profile_Photo)
                .FirstOrDefault();
            if (user != null)
               return user;

            return null;
         }
      }

      public User_Profile getUserByEmployeeProfile(Nullable<int> Employee_Profile_ID)
      {
         using (var db = new SBS2DBContext())
         {
            //User_Profile user = db.User_Profile
            //    .Include(i => i.User_Authentication)
            //    .Include(i => i.User_Profile_Photo)
            //    .Include(i => i.Employee_Profile)
            //    .Include(i => i.User_Authentication.User_Assign_Role)
            //    .Include(i => i.User_Authentication.Activation_Link)
            //    .Where(i => i.Employee_Profile == Profile_ID)
            //    .SingleOrDefault();

            User_Profile user = db.Employee_Profile
                .Where(e => e.Employee_Profile_ID == Employee_Profile_ID)
                .Select(i => i.User_Profile)
                .Include(i => i.User_Authentication)
                .Include(i => i.User_Profile_Photo)
                .Include(i => i.Employee_Profile)
                .Include(i => i.User_Authentication.User_Assign_Role)
                .Include(i => i.User_Authentication.Activation_Link)
                .SingleOrDefault();

            return user;
         }
      }

      public User_Profile getUserByEmail(String pEmail)
      {
         using (var db = new SBS2DBContext())
         {
            User_Profile user = db.User_Profile
                .Include(i => i.User_Authentication)
                .Where(i => i.User_Authentication.Email_Address.Equals(pEmail))
                .FirstOrDefault();

            return user;
         }
      }

      public User_Profile getUserByUserName(String pUserName)
      {
         using (var db = new SBS2DBContext())
         {
            User_Profile user = db.User_Profile
                .Where(i => i.User_Authentication.Email_Address.Equals(pUserName) || i.User_Authentication.User_Name.Equals(pUserName))
                .FirstOrDefault();

            return user;
         }
      }

      public User_Profile getAuthenticatedUser(String pUserName)
      {
         using (var db = new SBS2DBContext())
         {
            User_Profile user = db.User_Profile
                .Include(i => i.Employee_Profile)
                .Where(i => i.User_Authentication.Email_Address.Equals(pUserName) || i.User_Authentication.User_Name.Equals(pUserName))
                .FirstOrDefault();
            return user;
         }
      }

      public User_Profile getCompanyAdminUser(int? pComID)
      {
         using (var db = new SBS2DBContext())
         {
            var assrole = db.User_Assign_Role.Where(w => w.User_Role_ID == ROLE_CUSTOMER_ADMIN && w.User_Authentication.Company_ID == pComID).FirstOrDefault();
            if (assrole != null)
               return assrole.User_Authentication.User_Profile.FirstOrDefault();

            return null;
         }
      }
      public static string hashSHA256(string password)
      {
         SHA256Managed crypt = new SHA256Managed();
         string hash = String.Empty;
         byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password), 0, Encoding.UTF8.GetByteCount(password));
         foreach (byte bit in crypto)
         {
            hash += bit.ToString("x2");
         }


         return hash;
      }

      public User_Profile getUserProfileUserAuthentication(int UserAuthenticationID)
      {
         using (var db = new SBS2DBContext())
         {
            User_Profile user = db.User_Profile
                .Include(i => i.User_Authentication)
                .Include(i => i.Employee_Profile)
                .Include(i => i.User_Authentication.User_Assign_Role)
                .Where(i => i.User_Authentication_ID == UserAuthenticationID)
                .SingleOrDefault();

            return user;
         }
      }

      public User_Profile getUserProfile(string email, string password = "")
      {
         using (var db = new SBS2DBContext())
         {
            var users = db.User_Profile
                .Include(i => i.User_Authentication)
                .Include(i => i.Employee_Profile)
                .Include(i => i.User_Profile_Photo)
                .Include(i => i.User_Authentication.User_Assign_Role)
                .Where(i => i.User_Authentication.Email_Address.Equals(email.ToLower()));

            if (!string.IsNullOrEmpty(password))
            {
               users = users.Where(w => w.User_Authentication.PWD == password);
            }

            return users.FirstOrDefault();
         }
      }

      public User_Profile getUserProfileByUserName(string UserName, string password = "")
      {
         using (var db = new SBS2DBContext())
         {
            var users = db.User_Profile
                .Include(i => i.User_Authentication)
                .Include(i => i.Employee_Profile)
                .Include(i => i.User_Profile_Photo)
                .Include(i => i.User_Authentication.User_Assign_Role)
                .Where(i => i.User_Authentication.User_Name.Equals(UserName));

            if (!string.IsNullOrEmpty(password))
            {
               users = users.Where(w => w.User_Authentication.PWD == password);
            }

            return users.FirstOrDefault();
         }
      }

      public Dictionary<String, List<string>> getUserPageRights(int User_Authentication_ID)
      {
         //var cri = new SubscriptionCriteria()
         //{
         //    User_Authentication_ID = User_Authentication_ID
         //};
         //Dictionary<String, List<string>> subscriptions = getSubscription(cri);

         Dictionary<String, List<string>> UserRights = new Dictionary<string, List<string>>();

         List<User_Assign_Role> assign_roles = getUserAssignRole(User_Authentication_ID);
         if (assign_roles != null)
         {
            foreach (User_Assign_Role assign_role in assign_roles)
            {

               if (assign_role.User_Role != null && assign_role.User_Role.Page_Role != null)
               {

                  List<string> rights = new List<string>();
                  foreach (Page_Role page in assign_role.User_Role.Page_Role)
                  {
                     if (UserRights.ContainsKey(page.Page.Page_Url))
                        rights = UserRights.Where(w => w.Key.Equals(page.Page.Page_Url)).FirstOrDefault().Value;
                     else
                        UserRights.Add(page.Page.Page_Url, rights);

                     foreach (Access_Page apage in page.Access_Page)
                     {
                        if (!rights.Contains(apage.Access_Right.Access_Name))
                           rights.Add(apage.Access_Right.Access_Name);
                     }
                  }
               }

            }
         }

         return UserRights;
      }

      public User_Authentication getUserAuthentication(int id)
      {
         using (var db = new SBS2DBContext())
         {
            User_Authentication user = db.User_Authentication
                .Where(i => i.User_Authentication_ID == id)
                .SingleOrDefault();

            return user;
         }
      }

      public int SaveSkipTutorial(int pid, bool IsSkip)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               User_Profile user = db.User_Profile.Where(w => w.Profile_ID == pid).FirstOrDefault();
               if (user != null)
               {
                  user.Is_Tour_Skip = IsSkip;
               }
               db.SaveChanges();

               return 1;
            }
         }
         catch
         {
            //Log
            return -500;
         }
      }
      public int updateBG(int pid, string bg)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               User_Profile user = db.User_Profile.Where(w => w.Profile_ID == pid).FirstOrDefault();
               if (user != null)
               {
                  user.Bg = bg;
               }
               db.SaveChanges();

               return 1;
            }
         }
         catch
         {
            //Log
            return -500;
         }
      }

      public int updateUserProfilePhoto(int pid, byte[] file)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               User_Profile user = getUser(pid);
               var userphoto = user.User_Profile_Photo.Where(w => w.Profile_ID == pid).FirstOrDefault();
               if (userphoto != null)
               {
                  //UPDATE
                  userphoto.Photo = file;
                  db.Entry(user.User_Profile_Photo).State = EntityState.Modified;
               }
               else
               {
                  System.Guid id = Guid.NewGuid();
                  //Insert
                  User_Profile_Photo photo = new User_Profile_Photo()
                  {
                     User_Profile_Photo_ID = id,
                     Photo = file,
                     Profile_ID = pid
                  };

                  db.User_Profile_Photo.Add(photo);
                  db.Entry(user).State = EntityState.Modified;
               }

               db.SaveChanges();

               return 1;
            }
         }
         catch
         {
            //Log
            return -500;
         }
      }

      public Boolean updateLastConnection(int Profile_ID)
      {
         try
         {
            DateTime currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
               User_Profile user = getUser(Profile_ID);
               user.Latest_Connection = currentdate;

               db.Entry(user).State = EntityState.Modified;
               db.SaveChanges();

               return true;
            }
         }
         catch
         {
            //Log
            return false;
         }
      }

      public Boolean isDuplicate(string paramEmail, string paramUserName)
      {
         try
         {
            var bFlag = false;
            if (!string.IsNullOrEmpty(paramUserName))
            {
               bFlag = StoredProcedure.IsExistingUserName(paramUserName);
            }
            if (!string.IsNullOrEmpty(paramEmail))
            {
               bFlag = StoredProcedure.IsExistingEmail(paramEmail);
            }
            return bFlag;
         }
         catch
         {
            //Log
            return false;
         }
      }

      public Boolean updateLoginAttempt(String email)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               User_Profile user = db.User_Profile
                   .Include(i => i.User_Authentication)
                   .Include(i => i.User_Authentication.User_Assign_Role)
                   .Where(i => i.User_Authentication.Email_Address.Equals(email.ToLower()) || i.User_Authentication.User_Name.Equals(email.ToLower()))
                   .FirstOrDefault();

               if (user != null)
               {
                  user.User_Authentication.Login_Attempt = user.User_Authentication.Login_Attempt + 1;
                  db.Entry(user.User_Authentication).State = EntityState.Modified;
                  db.SaveChanges();
               }
               return true;
            }
         }
         catch
         {
            //Log
            return false;
         }
      }

      public Boolean updatePageAttempt(String pageUrl)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               Page page = db.Pages.Where(i => i.Page_Url.Contains(pageUrl)).SingleOrDefault();
               if (page != null)
               {
                  if (page.Page_Attempt != null)
                  {
                     page.Page_Attempt = page.Page_Attempt + 1;
                  }
                  else
                  {
                     page.Page_Attempt = 1;
                  }
                  db.Entry(page).State = EntityState.Modified;
                  db.SaveChanges();
               }
               return true;
            }
         }
         catch
         {
            //Log
            return false;
         }
      }

      public List<Page> LstPageAll()
      {
         using (var db = new SBS2DBContext())
         {
            return db.Pages.Include(i => i.SBS_Module_Detail).OrderBy(o => o.Module_Detail_ID).ToList();
         }
      }

      public static Page getPage(string page)
      {
         using (var db = new SBS2DBContext())
         {

            return db.Pages.Where(w => w.Page_Url == page).FirstOrDefault();

         }
      }

      public static List<User_Assign_Role> getUserAssignRole(int? User_Authentication_ID)
      {
         List<User_Assign_Role> assign_roles = new List<User_Assign_Role>();
         using (var db = new SBS2DBContext())
         {

            assign_roles = db.User_Assign_Role
                .Include(i => i.User_Role)
                .Include(i => i.User_Role.Page_Role)
                .Include(i => i.User_Role.Page_Role.Select(s => s.Page))
                .Include(i => i.User_Role.Page_Role.Select(s => s.Access_Page.Select(c => c.Access_Right)))
                .Where(w => w.User_Authentication_ID == User_Authentication_ID).ToList();

         }

         return assign_roles;
      }

      public Boolean haveRights(int User_Authentication_ID, string operation, String PAGE)
      {
         PAGE = removeSlashes(PAGE);
         PAGE = "/" + PAGE;

         List<User_Assign_Role> assign_roles = getUserAssignRole(User_Authentication_ID);
         if (assign_roles != null)
         {
            foreach (User_Assign_Role assign_role in assign_roles)
            {

               if (assign_role.User_Role != null && assign_role.User_Role.Page_Role != null)
               {

                  //System.Diagnostics.Debug.WriteLine("User_Role " + assign_role.User_Role.Role_Name);

                  foreach (Page_Role page in assign_role.User_Role.Page_Role)
                  {

                     //System.Diagnostics.Debug.WriteLine("Page_URL " + page.Page_URL);
                     if (page.Access_Page != null && string.Compare(page.Page.Page_Url, PAGE, true) == 0)
                     {
                        foreach (Access_Page apage in page.Access_Page)
                        {
                           //System.Diagnostics.Debug.WriteLine("Access_Page " + apage.Access_ID);
                           if (apage.Access_Right.Access_Name == operation)
                           {
                              return true;
                           }
                        }
                     }
                  }
               }

            }
         }

         return false;
      }

      public Dictionary<String, List<string>> getPageRights(int User_Authentication_ID, List<string> URLs, int Company_ID, int Profile_ID)
      {
         var assign_roles = getUserAssignRole(User_Authentication_ID);
         var result = new Dictionary<String, List<string>>();
         if (assign_roles != null)
         {
            foreach (var url in URLs)
            {
               List<string> rights = new List<string>();
               var page = getPage(url);

               var cri = new SubscriptionCriteria()
               {
                  Company_ID = Company_ID,
                  Module_Detail_ID = page.Module_Detail_ID,
                  Profile_ID = Profile_ID
               };
               var subscriptions = getSubscription(cri);
               var havesub = false;
               if (subscriptions != null && subscriptions.Count() > 0)
               {
                  foreach (var sub in subscriptions)
                  {
                     if (sub.Value.Contains(page.Page_Url))
                     {
                        havesub = true;
                        break;
                     }
                  }
               }
               if (!page.Module_Detail_ID.HasValue)
               {
                  havesub = true;
               }
               if (havesub)
               {
                  foreach (var assign_role in assign_roles)
                  {
                     if (assign_role.User_Role != null && assign_role.User_Role.Page_Role != null)
                     {
                        var pagerole = assign_role.User_Role.Page_Role.Where(w => w.Page_ID == page.Page_ID).FirstOrDefault();
                        if (pagerole != null)
                        {
                           foreach (Access_Page apage in pagerole.Access_Page)
                           {
                              rights.Add(apage.Access_Right.Access_Name);
                           }
                           result.Add(url, rights);
                           break;

                        }
                     }
                  }
               }
            }
         }
         return result;
      }

      public bool MainMasterIsExist(int? User_Authentication_ID)
      {
         List<User_Assign_Role> assign_roles = getUserAssignRole(User_Authentication_ID);
         if (assign_roles.Select(s => s.User_Role_ID).Contains(ROLE_MAIN_MASTER_ADMIN))
         {
            return true;
         }
         return false;
      }

      public List<string> getPageRights(int User_Authentication_ID, String PAGE, int Company_ID, int Profile_ID)
      {
         PAGE = removeSlashes(PAGE);
         PAGE = "/" + PAGE;

         List<string> rights = new List<string>();

         var page = getPage(PAGE);
         if (page != null)
         {
            List<User_Assign_Role> assign_roles = getUserAssignRole(User_Authentication_ID);

            var cri = new SubscriptionCriteria()
            {
               Company_ID = Company_ID,
               Module_Detail_ID = page.Module_Detail_ID,
               Profile_ID = Profile_ID
            };
            Dictionary<String, List<string>> subscriptions = getSubscription(cri);
            if (assign_roles != null)
            {
               var havesub = false;
               if (subscriptions != null && subscriptions.Count() > 0)
               {
                  foreach (var sub in subscriptions)
                  {
                     if (sub.Value.Contains(page.Page_Url))
                     {
                        havesub = true;
                        break;
                     }
                  }
               }
               if (!page.Module_Detail_ID.HasValue)
               {
                  havesub = true;
               }
               if (havesub)
               {
                  foreach (User_Assign_Role assign_role in assign_roles)
                  {

                     if (assign_role.User_Role != null && assign_role.User_Role.Page_Role != null)
                     {
                        var pagerole = assign_role.User_Role.Page_Role.Where(w => w.Page_ID == page.Page_ID).FirstOrDefault();
                        if (pagerole != null)
                        {
                           foreach (Access_Page apage in pagerole.Access_Page)
                           {
                              rights.Add(apage.Access_Right.Access_Name);
                           }

                           break;
                        }
                     }

                  }
               }

            }
         }

         return rights;
      }

      public static String removeSlashes(String URL)
      {
         if (!string.IsNullOrEmpty(URL))
         {
            for (; URL[URL.Length - 1] == '/'; )
            {
               URL = URL.Remove(URL.Length - 1, 1);
            }

            for (; URL[0] == '/'; )
            {
               URL = URL.Remove(0, 1);
            }
         }
         return URL;
      }

      public ServiceResult InsertUserPro(User_Profile pDataUsers, int[] pUserAssignRole, int[] pUserAssignModule, Employee_Profile pEmp)
      {
         try
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
               if (pDataUsers.Is_Email.HasValue && pDataUsers.Is_Email.Value)
               {
                  if (!string.IsNullOrEmpty(pDataUsers.Email))
                  {
                     if (db.Users.Where(w => w.UserName.ToLower() == pDataUsers.Email.ToLower()).FirstOrDefault() != null)
                        return new ServiceResult { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.Email + " " + Resource.Is_Duplicated_Lower, Field = Resource.Employee };
                  }
               }
               else
               {
                  if (!string.IsNullOrEmpty(pDataUsers.User_Name))
                  {
                     if (db.User_Profile.Where(w => w.User_Name.ToLower() == pDataUsers.User_Name.ToLower()).FirstOrDefault() != null)
                        return new ServiceResult { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.UserName + " " + Resource.Is_Duplicated_Lower, Field = Resource.Employee };
                  }
               }

               var com = (from a in db.Company_Details
                          where (a.Effective_Date <= currentdate & a.Company_ID == pDataUsers.Company_ID)
                          orderby a.Effective_Date descending
                          select a).FirstOrDefault();

               if (com == null)
                  return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.Company + Resource.Not_Found_Msg, Field = Resource.User };

               var pattern = (from a in db.Employee_No_Pattern where a.Company_ID == pDataUsers.Company_ID.Value select a).FirstOrDefault();
               if (pattern == null)
                  return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.Pattern + Resource.Not_Found_Msg, Field = Resource.Employee };

               var guid = Guid.NewGuid().ToString();
               while (db.Users.Where(w => w.Id == guid).FirstOrDefault() != null)
               {
                  guid = Guid.NewGuid().ToString();
               }

               if (pDataUsers.Is_Email == false)
                  db.Users.Add(new ApplicationUser() { Id = guid, UserName = pDataUsers.User_Name.ToLower() });
               else
                  db.Users.Add(new ApplicationUser() { Id = guid, UserName = pDataUsers.Email.ToLower() });

               var ApplicationUser_Id = guid;

               #region User Authentication
               User_Authentication authen = new User_Authentication()
               {
                  Email_Address = pDataUsers.Email,
                  ApplicationUser_Id = ApplicationUser_Id,
                  Company_ID = com.Company_ID,
                  Create_By = pDataUsers.Create_By,
                  Create_On = pDataUsers.Create_On,
                  Update_By = pDataUsers.Update_By,
                  Update_On = pDataUsers.Update_On,
                  User_Name = pDataUsers.User_Name,
                  Is_Email = pDataUsers.Is_Email,
               };
               #endregion

               #region User Profile
               User_Profile user = new User_Profile()
               {
                  First_Name = pDataUsers.First_Name,
                  Middle_Name = pDataUsers.Middle_Name,
                  Last_Name = pDataUsers.Last_Name,
                  User_Status = pDataUsers.User_Status,
                  Phone = pDataUsers.Phone,
                  Registration_Date = currentdate,
                  Company_ID = com.Company_ID,
                  Is_Email = pDataUsers.Is_Email,
                  User_Authentication = authen,
                  User_Transactions = pDataUsers.User_Transactions,
                  Create_By = pDataUsers.Create_By,
                  Create_On = pDataUsers.Create_On,
                  Update_By = pDataUsers.Update_By,
                  Update_On = pDataUsers.Update_On,
               };
               #endregion


               if (pDataUsers.Is_Email.HasValue && pDataUsers.Is_Email.Value)
               {
                  authen.Email_Address = pDataUsers.Email;
                  user.Email = pDataUsers.Email;
               }
               else
               {
                  authen.User_Name = pDataUsers.User_Name;
                  user.User_Name = pDataUsers.User_Name;
               }
               //GENERATE ACTIVATION CODE
               String code;
               do
               {
                  code = "A" + randomString(40);
               }
               while (!validateActivationCode(code));

               #region User Assign Role
               //SET User_Assign_Role to ROLE_CUSTOMER_USER
               if (pUserAssignRole != null)
               {
                  foreach (var r in pUserAssignRole)
                  {
                     User_Assign_Role role = new User_Assign_Role()
                     {
                        User_Role_ID = r,
                        Create_By = pDataUsers.Create_By,
                        Create_On = currentdate,
                        Update_By = pDataUsers.Update_By,
                        Update_On = currentdate
                     };

                     authen.User_Assign_Role.Add(role);
                  }
               }
               #endregion

               if (pUserAssignModule != null)
               {
                  foreach (var s in pUserAssignModule)
                  {
                     var sub = db.Subscriptions.Where(w => w.Subscription_ID == s).FirstOrDefault();
                     if (sub != null)
                     {
                        if (sub.No_Of_Users.Value - sub.User_Assign_Module.Count() > 0)
                        {
                           var um = new User_Assign_Module()
                           {
                              Subscription_ID = s
                           };
                           user.User_Assign_Module.Add(um);
                        }
                     }
                  }
               }


               #region Activation Link
               Activation_Link activation_link = new Activation_Link()
               {
                  Activation_Code = code,
                  Time_Limit = currentdate.AddHours(LINK_TIME_LIMIT),
                  Create_By = pDataUsers.Create_By,
                  Create_On = currentdate,
                  Update_By = pDataUsers.Create_By,
                  Update_On = currentdate,
               };
               authen.Activation_Link.Add(activation_link);
               #endregion

               pEmp.Employee_No = GetEmployeeNo(pattern, currentdate, null, pEmp.Nationality_ID);
               var number = pEmp.Employee_No.Substring(pEmp.Employee_No.Length - 6, 6);
               while (db.Employee_Profile.Where(w => w.User_Profile.Company_ID == user.Company_ID && w.Employee_No.Substring(w.Employee_No.Length - 6, 6) == number).FirstOrDefault() != null)
               {
                  pattern.Current_Running_Number = pattern.Current_Running_Number + 1;
                  pEmp.Employee_No = GetEmployeeNo(pattern, currentdate, null, pEmp.Nationality_ID);
                  number = pEmp.Employee_No.Substring(pEmp.Employee_No.Length - 6, 6);
               }

               user.Employee_Profile.Add(pEmp);
               db.User_Profile.Add(user);
               db.SaveChanges();
               db.Entry(user).GetDatabaseValues();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.User, Object = code };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.User };
         }
      }

      public List<User_Assign_Module> LstUserAssignModule(Nullable<int> pProfileID)
      {
         using (var db = new SBS2DBContext())
         {
            return db.User_Assign_Module
                .Where(w => w.Profile_ID == pProfileID)
                .OrderBy(o => o.Subscription_ID)
                .ToList();
         }
      }

      public ServiceResult UpdateUserPro(User_Profile pDataUsers, int[] pUserAssignRole, int[] pUserAssignModule, Employee_Profile pEmp)
      {
         try
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
               var currProfile = db.User_Profile
                    .Include(i => i.User_Authentication)
                    .Include(i => i.User_Authentication.User_Assign_Role)
                    .Where(w => w.Profile_ID == pDataUsers.Profile_ID)
                    .FirstOrDefault();

               if (currProfile == null)
                  return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.User + Resource.Not_Found_Msg, Field = Resource.User };

               if (pDataUsers.Is_Email.HasValue && pDataUsers.Is_Email.Value)
               {
                  if (!string.IsNullOrEmpty(currProfile.Email) && !string.IsNullOrEmpty(pDataUsers.Email))
                  {
                     if (currProfile.Email.ToLower() != pDataUsers.Email.ToLower())
                     {
                        if (db.Users.Where(w => w.UserName.ToLower() == pDataUsers.Email.ToLower()).FirstOrDefault() != null)
                           return new ServiceResult { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.Duplicate_Small + Resource.Email, Field = Resource.User };
                     }
                  }
                  else
                  {
                     if (!string.IsNullOrEmpty(pDataUsers.Email))
                     {
                        if (db.Users.Where(w => w.UserName.ToLower() == pDataUsers.Email.ToLower()).FirstOrDefault() != null)
                           return new ServiceResult { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.Duplicate_Small + Resource.Email, Field = Resource.User };
                     }
                  }
               }
               else
               {
                  if (!string.IsNullOrEmpty(currProfile.User_Name) && !string.IsNullOrEmpty(pDataUsers.User_Name))
                  {
                     if (currProfile.User_Name.ToLower() != pDataUsers.User_Name.ToLower())
                     {
                        if (db.Users.Where(w => w.UserName.ToLower() == pDataUsers.User_Name.ToLower()).FirstOrDefault() != null)
                           return new ServiceResult { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.Duplicate_Small + Resource.User_Name, Field = Resource.User };
                     }
                  }
                  else
                  {
                     if (!string.IsNullOrEmpty(pDataUsers.User_Name))
                     {
                        if (db.Users.Where(w => w.UserName.ToLower() == pDataUsers.User_Name.ToLower()).FirstOrDefault() != null)
                           return new ServiceResult { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.Duplicate_Small + Resource.User_Name, Field = Resource.User };
                     }
                  }
               }

               var currEmp = db.Employee_Profile.Where(w => w.Employee_Profile_ID == pEmp.Employee_Profile_ID).FirstOrDefault();
               if (currEmp == null)
                  return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.Employee + Resource.Not_Found_Msg, Field = Resource.Employee };

               List<User_Assign_Role> aroles = new List<User_Assign_Role>();

               if (pUserAssignRole != null)
               {
                  //Delete unchecked  
                  foreach (User_Assign_Role urole in currProfile.User_Authentication.User_Assign_Role.ToList())
                  {
                     if (!pUserAssignRole.Contains(urole.User_Role_ID.Value))
                     {
                        db.Entry(urole).State = EntityState.Deleted;
                     }
                  }

                  foreach (int role in pUserAssignRole)
                  {
                     User_Assign_Role dbarole = currProfile.User_Authentication.User_Assign_Role.Where(s => s.User_Role_ID == role).FirstOrDefault();
                     if (dbarole == null)
                     {
                        User_Assign_Role arole = new User_Assign_Role()
                        {
                           User_Authentication = currProfile.User_Authentication,
                           User_Role_ID = role,
                           Create_By = pEmp.Create_By,
                           Create_On = pEmp.Create_On,
                           Update_By = pEmp.Update_By,
                           Update_On = pEmp.Update_On
                        };
                        aroles.Add(arole);
                        db.Entry(arole).State = EntityState.Added;
                     }
                     else
                     {
                        dbarole.Update_By = pEmp.Update_By;
                        dbarole.Update_On = pEmp.Update_On;
                        aroles.Add(dbarole);
                     }
                  }
               }
               else
               {
                  foreach (User_Assign_Role dbarole in currProfile.User_Authentication.User_Assign_Role.ToList())
                  {
                     db.Entry(dbarole).State = EntityState.Deleted;
                  }
               }

               if (pUserAssignModule != null)
               {
                  //Delete unchecked  
                  foreach (var s in currProfile.User_Assign_Module.ToList())
                  {
                     if (!pUserAssignModule.Contains(s.Subscription_ID.Value))
                     {
                        db.User_Assign_Module.Remove(s);
                     }
                  }

                  foreach (int s in pUserAssignModule)
                  {
                     var dbsub = currProfile.User_Assign_Module.Where(w => w.Subscription_ID == s).FirstOrDefault();
                     if (dbsub == null)
                     {
                        var sub = db.Subscriptions.Where(w => w.Subscription_ID == s).FirstOrDefault();
                        if (sub != null)
                        {
                           if (sub.No_Of_Users.Value - sub.User_Assign_Module.Where(w => w.Profile_ID != pEmp.Profile_ID).Count() > 0)
                           {
                              var um = new User_Assign_Module()
                              {
                                 Subscription_ID = s,
                                 Profile_ID = pEmp.Profile_ID,
                              };
                              db.User_Assign_Module.Add(um);
                           }
                        }
                     }
                  }
               }
               else
               {
                  foreach (var dbsub in currProfile.User_Assign_Module.ToList())
                  {
                     db.User_Assign_Module.Remove(dbsub);
                  }
               }

               var currasp = db.Users.Where(w => w.Id == currProfile.User_Authentication.ApplicationUser_Id).FirstOrDefault();
               if (currasp != null)
               {
                  if (pDataUsers.Is_Email.HasValue && pDataUsers.Is_Email.Value)
                  {
                     if (!string.IsNullOrEmpty(currasp.UserName) && !string.IsNullOrEmpty(pDataUsers.Email))
                     {
                        if (currasp.UserName.ToLower() != pDataUsers.Email.ToLower())
                        {
                           currasp.UserName = pDataUsers.Email.ToLower();
                        }
                     }
                  }
                  else
                  {
                     if (!string.IsNullOrEmpty(currasp.UserName) && !string.IsNullOrEmpty(pDataUsers.User_Name))
                     {
                        if (currasp.UserName.ToLower() != pDataUsers.User_Name.ToLower())
                        {
                           currasp.UserName = pDataUsers.User_Name.ToLower();
                        }
                     }
                  }
               }

               currProfile.First_Name = pDataUsers.First_Name;
               currProfile.Last_Name = pDataUsers.Last_Name;
               currProfile.Middle_Name = pDataUsers.Middle_Name;
               currProfile.User_Status = pDataUsers.User_Status;
               currProfile.Phone = pDataUsers.Phone;
               currProfile.Is_Email = pDataUsers.Is_Email;
               currProfile.Update_By = pEmp.Update_By;
               currProfile.Update_On = pEmp.Update_On;

               currProfile.User_Authentication.Update_On = pEmp.Update_On;
               currProfile.User_Authentication.Update_By = pEmp.Update_By;
               currProfile.User_Authentication.Is_Email = pDataUsers.Is_Email;

               if (pDataUsers.Is_Email.HasValue && pDataUsers.Is_Email.Value)
               {
                  currProfile.User_Authentication.Email_Address = pDataUsers.Email;
                  currProfile.Email = pDataUsers.Email;
               }
               else
               {
                  currProfile.User_Authentication.User_Name = pDataUsers.User_Name;
                  currProfile.User_Name = pDataUsers.User_Name;
               }

               db.Entry(currEmp).CurrentValues.SetValues(pEmp);
               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.User };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.User };
         }
      }

      public ServiceResult UpdateUserStatus(int[] pDataIDs, string pStatus, string pUpdateBy)
      {
         try
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
               var current = db.User_Profile.Where(w => pDataIDs.Contains(w.Profile_ID));
               foreach (var user in current)
               {
                  if (user != null)
                  {
                     user.Update_On = currentdate;
                     user.Update_By = pUpdateBy;
                     user.User_Status = pStatus;
                  }
               }

               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.User };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.User };
         }
      }

      public List<User_Role> LstUserRoleAll()
      {
         using (var db = new SBS2DBContext())
         {
            return db.User_Role.OrderBy(o => o.Role_Name).ToList();
         }
      }

      public User_Role getUserRole(Nullable<int> pUserRoleID)
      {
         using (var db = new SBS2DBContext())
         {
            var role = db.User_Role
            .Where(w => w.User_Role_ID == pUserRoleID)
            .FirstOrDefault();

            return role;
         }
      }

      public ServiceResult InsertAndUpdatePageRole(List<Page_Role> pageRolesEdit, List<Page_Role> pageRolesAdded, Nullable<int> pModuleDetailID)
      {
         var currentdate = StoredProcedure.GetCurrentDate();

         try
         {
            using (var db = new SBS2DBContext())
            {
               List<Page_Role> currpageRole = new List<Page_Role>();
               var pageRoleRemove = new List<Page_Role>();

               if (pModuleDetailID != null)
               {
                  if (pModuleDetailID != 0)
                  {
                     currpageRole = (from a in db.Page_Role where a.Page.SBS_Module_Detail.Module_Detail_ID == pModuleDetailID select a).Include(i => i.Page).ToList();
                  }
                  else
                  {
                     //select authen 
                     currpageRole = (from a in db.Page_Role where a.Page.SBS_Module_Detail.Module_Detail_ID == null select a).Include(i => i.Page).ToList();
                  }

                  if (currpageRole == null)
                     return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.Page_Role + " " + Resource.Not_Found_Msg, Field = Resource.Page_Role };
               }
               //Added New Data
               if (pageRolesAdded.Count != 0)
               {
                  db.Page_Role.AddRange(pageRolesAdded);
                  foreach (var ap in pageRolesAdded)
                  {
                     if (ap.Access_Page != null)
                     {
                        db.Access_Page.AddRange(ap.Access_Page);
                     }
                  }
               }

               //Edit Data                
               foreach (var row in currpageRole)
               {
                  if (pageRolesEdit == null || !pageRolesEdit.Select(s => s.Page_Role_ID).Contains(row.Page_Role_ID))
                  {
                     pageRoleRemove.Add(row);

                     var PR = db.Page_Role.Where(w => w.Page_Role_ID == row.Page_Role_ID);
                     db.Page_Role.RemoveRange(PR);

                     var AP = db.Access_Page.Where(w => w.Page_Role_ID == row.Page_Role_ID);
                     db.Access_Page.RemoveRange(AP);
                  }
               }

               if (pageRoleRemove.Count > 0)
               {
                  db.Page_Role.RemoveRange(pageRoleRemove);
               }

               if (pageRolesEdit.Count != 0)
               {
                  foreach (var row in pageRolesEdit)
                  {
                     if (row.Page_Role_ID == 0 || !currpageRole.Select(s => s.Page_Role_ID).Contains(row.Page_Role_ID))
                     {
                        db.Page_Role.Add(row);
                        if (row.Access_Page != null)
                        {
                           db.Access_Page.AddRange(row.Access_Page);
                        }
                     }
                     else
                     {
                        var currPageRole = db.Page_Role.Where(w => w.Page_Role_ID == row.Page_Role_ID).FirstOrDefault();
                        if (currPageRole != null)
                        {
                           currPageRole.User_Role_ID = row.User_Role_ID;
                           currPageRole.Page_ID = row.Page_ID;
                           currPageRole.Update_By = row.Update_By;
                           currPageRole.Update_On = row.Update_On;
                           db.Entry(currPageRole).State = EntityState.Modified;

                           //--------------------------Access Page------------------------//
                           var accessPageRemove = new List<Access_Page>();
                           List<Access_Page> currapagesAp = getAccessPageRight(row.Page_Role_ID);
                           List<int> chk_right = new List<int>();

                           foreach (Access_Page aprow in currapagesAp)
                           {
                              if (row.Access_Page == null || !row.Access_Page.Select(s => s.Access_ID).Contains(aprow.Access_ID))
                              {
                                 db.Entry(row).State = EntityState.Deleted;
                              }
                              chk_right.Add(aprow.Access_ID);
                           }

                           foreach (var right in row.Access_Page)
                           {
                              if (!chk_right.Contains(right.Access_ID))
                              {
                                 Access_Page newAccessPage = new Access_Page()
                                 {
                                    Access_ID = right.Access_ID,
                                    Page_Role_ID = right.Page_Role_ID,
                                    Update_By = right.Update_By,
                                    Update_On = currentdate
                                 };
                                 db.Entry(newAccessPage).State = EntityState.Added;
                              }
                           }
                           //---------------------------------------------------------------//
                        }
                     }
                  }
               }
               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Page_Role };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Page_Role };
         }
      }

      public List<Access_Page> getAccessPageRight(int pPageRoleID)
      {
         List<Access_Page> apage = new List<Access_Page>();
         using (var db = new SBS2DBContext())
         {
            apage = (from a in db.Access_Page where (a.Page_Role_ID == pPageRoleID) select a).ToList();
         }

         return apage;
      }

      public ServiceResult InsertUserRole(User_Role pUserRole)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               db.User_Role.Add(pUserRole);
               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.User_Role };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.User_Role };
         }


      }

      public ServiceResult UpdateUserRole(User_Role pUserRole)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {

               var role = db.User_Role
                   .Where(w => w.User_Role_ID == pUserRole.User_Role_ID)
                   .Single();

               if (role == null)
                  return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.User_Role + " " + Resource.Not_Found_Msg, Field = Resource.User_Role };

               role.Role_Name = pUserRole.Role_Name;
               role.Role_Description = pUserRole.Role_Description;
               role.Update_By = pUserRole.Update_By;
               role.Update_On = pUserRole.Update_On;

               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.User_Role };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.User_Role };
         }

      }

      public ServiceResult DeleteUserProfile(int[] pProfileIDs)
      {
         foreach (var userID in pProfileIDs)
         {
            DeleteUserProfile(userID);

         }
         return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.User };
      }

      public ServiceResult DeleteUserProfile(Nullable<int> pProfileID)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               var User = db.User_Profile.Where(w => w.Profile_ID == pProfileID).FirstOrDefault();
               if (User != null)
               {
                  User.User_Status = RecordStatus.Inactive;
                  var emp = db.Employee_Profile.Where(w => w.Profile_ID == User.Profile_ID).FirstOrDefault();
                  if (emp != null)
                     emp.Emp_Status = RecordStatus.Inactive;
                  db.SaveChanges();
               }

               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.User };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.User };
         }
      }

      //Update Status function delete
      public ServiceResult UpdateDeleteUserProfileStatus(Nullable<int> pProfileID, string pStatus, string pUpdateBy)
      {
         try
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
               var User = db.User_Profile.Where(w => w.Profile_ID == pProfileID).FirstOrDefault();
               if (User != null)
               {
                  var emp = db.Employee_Profile.Where(w => w.Profile_ID == User.Profile_ID).FirstOrDefault();
                  if (emp != null)
                  {
                     var contact = db.Employee_Emergency_Contact.Where(w => w.Employee_Profile_ID == emp.Employee_Profile_ID);
                     if (contact != null)
                     {
                        foreach (var row in contact)
                        {
                           row.Update_On = currentdate;
                           row.Update_By = pUpdateBy;
                           //row.Record_Status = pStatus;
                        }
                     }

                     var bank = db.Banking_Info.Where(w => w.Employee_Profile_ID == emp.Employee_Profile_ID);
                     if (bank != null)
                     {
                        foreach (var row in bank)
                        {
                           row.Update_On = currentdate;
                           row.Update_By = pUpdateBy;
                           //row.Record_Status = pStatus;
                        }
                     }

                     var rs = db.Relationships.Where(w => w.Employee_Profile_ID == emp.Employee_Profile_ID);
                     if (rs != null)
                     {
                        foreach (var row in rs)
                        {
                           row.Update_On = currentdate;
                           row.Update_By = pUpdateBy;
                           //row.Record_Status = pStatus;
                        }
                     }

                     //var empHistAllowance = db.Employment_History_Allowance.Where(w => w.Employment_History.Employee_Profile_ID == emp.Employee_Profile_ID);
                     //db.Employment_History_Allowance.RemoveRange(empHistAllowance);

                     var empHist = db.Employment_History.Where(w => w.Employee_Profile_ID == emp.Employee_Profile_ID);
                     if (empHist != null)
                     {
                        foreach (var row in empHist)
                        {
                           row.Update_On = currentdate;
                           row.Update_By = pUpdateBy;
                           row.Record_Status = pStatus;
                        }
                     }

                     //var empAtt = db.Employee_Attachment.Where(w => w.Employee_Profile_ID == emp.Employee_Profile_ID);
                     //db.Employee_Attachment.RemoveRange(empAtt);

                     emp.Update_By = pUpdateBy;
                     emp.Update_On = currentdate;
                     //emp.Record_Status = pStatus;
                  }

                  //var modules = db.User_Assign_Module.Where(w => w.Profile_ID == User.Profile_ID);
                  //db.User_Assign_Module.RemoveRange(modules);

                  var photo = db.User_Profile_Photo.Where(w => w.Profile_ID == User.Profile_ID);
                  if (photo != null)
                  {
                     foreach (var row in photo)
                     {
                        row.Update_On = currentdate;
                        row.Update_By = pUpdateBy;
                        //row.Record_Status = pStatus;
                     }
                  }

                  var role = db.User_Assign_Role.Where(w => w.User_Authentication_ID == User.User_Authentication_ID);
                  if (role != null)
                  {
                     foreach (var row in role)
                     {
                        row.Update_On = currentdate;
                        row.Update_By = pUpdateBy;
                        //row.Record_Status = pStatus;
                     }
                  }

                  var alink = db.Activation_Link.Where(w => w.User_Authentication_ID == User.User_Authentication_ID);
                  if (alink != null)
                  {
                     foreach (var row in alink)
                     {
                        row.Update_On = currentdate;
                        row.Update_By = pUpdateBy;
                        //row.Record_Status = pStatus;
                     }
                  }

                  //var aspUser = db.Users.Where(w => w.Id.Equals(User.User_Authentication.ApplicationUser_Id)).FirstOrDefault();
                  //db.Users.Remove(aspUser);

                  var authen = db.User_Authentication.Where(w => w.User_Authentication_ID == User.User_Authentication_ID).FirstOrDefault();
                  if (authen != null)
                  {
                     authen.Update_On = currentdate;
                     authen.Update_By = pUpdateBy;
                     //authen.Record_Status = pStatus;                    
                  }

                  //var ir = db.IRs.Where(w => w.Profile_ID == User.Profile_ID);
                  //if (ir != null)
                  //{
                  //    foreach (var row in ir)
                  //    {
                  //        row.Update_On = currentdate;
                  //        row.Update_By = pUpdateBy;
                  //        //row.Record_Status = pStatus;
                  //    }
                  //}

                  //var ia = db.IAs.Where(w => w.Profile_ID == User.Profile_ID);
                  //if (ia != null)
                  //{
                  //    foreach (var row in ia)
                  //    {
                  //        row.Update_On = currentdate;
                  //        row.Update_By = pUpdateBy;
                  //        //row.Record_Status = pStatus;
                  //    }
                  //}

                  User.Update_On = currentdate;
                  User.Update_By = pUpdateBy;
                  User.User_Status = pStatus;

                  db.SaveChanges();
               }

               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.User };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.User };
         }
      }

      public ServiceResult UpdateMultipleDeleteUserProfileStatus(int[] pProfileIDs, string pStatus, string pUpdateBy)
      {
         foreach (var userID in pProfileIDs)
         {
            UpdateDeleteUserProfileStatus(userID, pStatus, pUpdateBy);

         }
         return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.User };
      }

      public ServiceResult deleteUserRole(Nullable<int> pUserRoleID)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {

               var conToDel = db.User_Role.Where(w => w.User_Role_ID == pUserRoleID).FirstOrDefault();
               db.User_Role.Remove(conToDel);
               db.SaveChanges();

               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.User_Role };
            }
         }
         catch
         {

         }
         return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.User_Role };
      }

      public List<User_Assign_Role> LstUserAssignRole(Nullable<int> pUserAuthenID)
      {
         using (var db = new SBS2DBContext())
         {
            return db.User_Assign_Role.Where(w => w.User_Authentication_ID == pUserAuthenID).ToList();
         }
      }

      private string GetEmployeeNo(Employee_No_Pattern pattern, DateTime currentdate, Nullable<DateTime> Hired_Date, Nullable<int> Nationality_ID)
      {
         DateTime pattenDate;
         var year = "";
         var nationality = "";
         var branchcode = "";
         if (Hired_Date.HasValue)
         {
            pattenDate = Hired_Date.Value;
         }
         else
         {
            pattenDate = currentdate;
         }

         if (pattern.Year_2_Digit)
         {
            year = pattenDate.Year.ToString().Substring(2, 2) + "-";
         }
         else if (pattern.Year_4_Digit)
         {
            year = pattenDate.Year.ToString() + "-";
         }

         if (pattern.Select_Nationality)
         {
            using (var db = new SBS2DBContext())
            {
               var nal = (from a in db.Nationalities where a.Nationality_ID == Nationality_ID select a).FirstOrDefault();
               if (nal != null)
               {
                  nationality = nal.Name + "-";
               }
            }
         }
         if (pattern.Select_Branch_Code.HasValue && pattern.Select_Branch_Code.Value && pattern.Branch != null)
         {
            branchcode = pattern.Branch.Branch_Code;
         }
         return year + nationality + branchcode + pattern.Current_Running_Number.Value.ToString("000000");
      }

      public List<SBS_Module_Detail> LstModuleDetail()
      {
         using (var db = new SBS2DBContext())
         {

            return db.SBS_Module_Detail.OrderBy(o => o.Module_Detail_Name).ToList();

         }
      }

      public List<Page_Role> getUserRolesPageRole(Nullable<int> pModuleDetailID = null)
      {
         using (var db = new SBS2DBContext())
         {
            if (pModuleDetailID != 0)
            {
               return (from a in db.Page_Role where a.Page.SBS_Module_Detail.Module_Detail_ID == pModuleDetailID select a)
                   .Include(i => i.Page)
                   .Include(i => i.Access_Page).OrderBy(j => j.User_Role.User_Role_ID).ThenBy(a => a.Page.Page_Url).ToList();

               //return (from a in db.Page_Role where a.Page.SBS_Module_Detail.Module_Detail_ID == pModuleDetailID select a)
               //    .Include(i => i.Page)
               //    .Include(i => i.Access_Page).ToList();
            }
            else
            {
               return (from a in db.Page_Role where a.Page.SBS_Module_Detail.Module_Detail_ID == null select a)
               .Include(i => i.Page)
               .Include(i => i.Access_Page).OrderBy(j => j.User_Role.User_Role_ID).ThenBy(a => a.Page.Page_Url).ToList();

               //return (from a in db.Page_Role where a.Page.SBS_Module_Detail.Module_Detail_ID == null select a)
               //   .Include(i => i.Page)
               //   .Include(i => i.Access_Page).ToList();
            }
         }
      }

      public int SaveUserPhoto(User_Profile_Photo pUserPhoto)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               var userphoto = db.User_Profile_Photo.Where(w => w.Profile_ID == pUserPhoto.Profile_ID).FirstOrDefault();
               if (userphoto != null)
               {
                  if (pUserPhoto.Profile_ID == userphoto.Profile_ID)
                  {
                     //UPDATE
                     userphoto.Photo = pUserPhoto.Photo;
                     userphoto.Update_By = pUserPhoto.Update_By;
                     userphoto.Update_On = pUserPhoto.Update_On;
                     db.Entry(userphoto).State = EntityState.Modified;
                  }
               }
               else
               {
                  var guid = Guid.NewGuid();
                  while (db.User_Profile_Photo.Where(w => w.User_Profile_Photo_ID == guid).FirstOrDefault() != null)
                     guid = Guid.NewGuid();

                  var photo = new User_Profile_Photo()
                  {
                     User_Profile_Photo_ID = guid,
                     Photo = pUserPhoto.Photo,
                     Profile_ID = pUserPhoto.Profile_ID,
                     Create_By = pUserPhoto.Create_By,
                     Create_On = pUserPhoto.Create_On,
                     Update_By = pUserPhoto.Update_By,
                     Update_On = pUserPhoto.Update_On
                  };
                  db.User_Profile_Photo.Add(photo);
               }
               db.SaveChanges();
               return 1;
            }
         }
         catch
         {
            //Log
            return -500;
         }
      }

      public int activationAccount(String code)
      {
         try
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBS2DBContext())
            {
               var comService = new CompanyService();

               var alink = (from a in db.Activation_Link where a.Activation_Code.Equals(code) select a).FirstOrDefault();
               if (alink != null)
               {
                  //CHK time limit
                  //7	 	System	Check username and the activation key. if matched, activation successful
                  if (alink.Time_Limit.CompareTo(currentdate) >= 0)
                  {
                     //UPDATE activated
                     var uauth = alink.User_Authentication;
                     if (uauth != null)
                     {
                        uauth.Activated = true;
                        db.Entry(uauth).State = EntityState.Modified;


                        //9	 	System	Send notification to System Administrator in order to assign modules to this account
                        //The company will send email to Master/Franchise/White Label to assign new module to their account. 
                        //There will be email notification after module has assigned. The email in company.register_by will be receive email. 
                        //GET all system admin email
                        //email 

                        try
                        {
                           bool isAdmin = false;
                           bool isUser = false;
                           foreach (User_Assign_Role role in uauth.User_Assign_Role)
                           {
                              if (role.User_Role_ID == ROLE_CUSTOMER_USER)
                              {
                                 isUser = true;
                                 break;
                              }
                              else if (role.User_Role_ID == ROLE_CUSTOMER_ADMIN)
                              {
                                 isAdmin = true;
                                 break;
                              }
                           }
                           var user = getUserProfileUserAuthentication(uauth.User_Authentication_ID);

                           if (isAdmin)
                           {

                              //SEND MAIL Main/Franchise/White Label (Register_by)
                              var header = "SBS User Activation";
                              var msg = "Activation status of " + uauth.Email_Address + " is 'COMPLETED'";

                              //USER's Company
                              var com = comService.GetCompany(user.Company_ID);
                              if (com != null)
                              {
                                 //USER's Main/Franchise/White Label
                                 com = comService.GetCompany(com.Belong_To_ID);
                                 if (com != null)
                                 {
                                    EmailTemplete.sendNotificationEmail(com.Create_By, header, msg, null, null);
                                 }
                              }

                           }
                           else if (isUser)
                           {
                              //Send email to Customer Admin (Register_by)
                              var header = "SBS User Activation";
                              var msg = "Activation status of " + uauth.Email_Address + " is 'COMPLETED'";

                              var com = comService.GetCompany(user.Company_ID);
                              if (com != null)
                                 EmailTemplete.sendNotificationEmail(com.Create_By, header, msg, null, null);

                           }

                        }
                        catch
                        {
                           //return -501;
                        }


                        //10    System	Record the registration date
                        var uprofile = (from a in db.User_Profile where a.User_Authentication_ID == uauth.User_Authentication_ID select a).FirstOrDefault();
                        db.SaveChanges();

                        //var company = comService.GetCompany(uprofile.Company_ID);
                        //if (company != null)
                        //{
                        //   company.Registration_Date = currentdate;
                        //   comService.UpdateCompany(company);
                        //}

                        //8	 	System	Redirect user to reset password

                        return uprofile.Profile_ID;
                     }

                     return ERROR_CODE.ERROR_1_USER_NOT_FOUND;
                  }
                  else
                     return ERROR_CODE.ERROR_2_ACTIVATE_CODE_EXPIRE;

               }
               else
                  return ERROR_CODE.ERROR_3_ACTIVATE_CODE_NOT_FOUND;
            }
         }
         catch
         {
            return ERROR_CODE.ERROR_500_DB;
         }
      }

      public Activation_Link getActivationLink(string activationCode)
      {
         using (var db = new SBS2DBContext())
         {
            Activation_Link link = db.Activation_Link
                .Where(w => w.Activation_Code.Equals(activationCode))
                .FirstOrDefault();

            return link;
         }
      }

      public string newActivation(int Profile_ID)
      {
         DateTime currentdate = StoredProcedure.GetCurrentDate();
         using (var db = new SBS2DBContext())
         {
            try
            {
               User_Profile user = getUser(Profile_ID);

               //GENERATE ACTIVATION CODE
               String code;
               do
               {
                  code = "A" + randomString(40);
               } while (!validateActivationCode(code));


               Activation_Link activation_link = new Activation_Link()
               {
                  Activation_Code = code,
                  //SET Time_Limit to activate within LINK_TIME_LIMIT hour
                  Time_Limit = currentdate.AddHours(LINK_TIME_LIMIT),
                  User_Authentication_ID = user.User_Authentication_ID.Value,
                  Active = true
               };


               db.Activation_Link.Add(activation_link);
               db.SaveChanges();
               return code;
            }
            catch
            {
               return "";
            }

         }


      }

      public int ExpireActivation(string activationCode)
      {
         using (var db = new SBS2DBContext())
         {
            try
            {
               Activation_Link link = db.Activation_Link.Where(w => w.Activation_Code.Equals(activationCode)).FirstOrDefault();
               if (link != null)
               {
                  link.Active = false;
                  db.SaveChanges();
               }
               return ERROR_CODE.SUCCESS;
            }
            catch
            {
               return ERROR_CODE.ERROR_500_DB;
            }

         }
      }

      public int ExpireActivationByPrefix(string prefix)
      {
         using (var db = new SBS2DBContext())
         {
            try
            {
               var links = db.Activation_Link.Where(w => w.Activation_Code.Contains(prefix));
               foreach (var link in links)
               {
                  link.Active = false;

               }
               db.SaveChanges();
               return ERROR_CODE.SUCCESS;
            }
            catch
            {
               return ERROR_CODE.ERROR_500_DB;
            }
         }
      }

      public string GenActivateCode(string prefix, Nullable<int> pUserAuthenticationID)
      {
         DateTime currentdate = StoredProcedure.GetCurrentDate();
         using (var db = new SBS2DBContext())
         {
            String code;
            do
            {
               code = prefix + randomString(40);
            } while (!validateActivationCode(code));

            Activation_Link activation_link = new Activation_Link()
            {
               Activation_Code = code,
               //SET Time_Limit to activate within LINK_TIME_LIMIT hour
               Time_Limit = currentdate.AddHours(LINK_TIME_LIMIT),
               User_Authentication_ID = pUserAuthenticationID.Value,
               Active = true
            };


            db.Activation_Link.Add(activation_link);
            db.SaveChanges();
            return code;
         }

      }

      public int sendNewActivation(int Profile_ID, string domain)
      {
         DateTime currentdate = StoredProcedure.GetCurrentDate();
         using (var db = new SBS2DBContext())
         {
            try
            {
               User_Profile user = db.User_Profile.Where(i => i.Profile_ID == Profile_ID).SingleOrDefault();

               //GENERATE ACTIVATION CODE
               String code;
               do
               {
                  code = "A" + randomString(40);
               } while (!validateActivationCode(code));


               Activation_Link activation_link = new Activation_Link()
               {
                  Activation_Code = code,
                  //SET Time_Limit to activate within LINK_TIME_LIMIT hour
                  Time_Limit = currentdate.AddHours(LINK_TIME_LIMIT),
                  User_Authentication_ID = user.User_Authentication_ID.Value
               };


               CompanyService comService = new CompanyService();

               Company_Details com = (from a in db.Company_Details
                                      where (a.Effective_Date <= currentdate) & a.Company_ID == user.Company_ID
                                      orderby a.Effective_Date descending
                                      select a).FirstOrDefault();


               db.Activation_Link.Add(activation_link);
               db.SaveChanges();
               try
               {
                  //SEND EMAIL



                  if (com == null)
                  {
                     return ERROR_CODE.ERROR_501_CANT_SEND_EMAIL;
                  }
                  EmailTemplete.sendUserActivateEmail(user.User_Authentication.Email_Address, code, GetUserName(user), com.Name, com.Phone, com.Email, domain);
               }
               catch
               {
                  return ERROR_CODE.ERROR_501_CANT_SEND_EMAIL;
               }

            }
            catch
            {
               return ERROR_CODE.ERROR_500_DB;
            }

         }

         return 1;
      }

      private readonly Random _rng = new Random();

      public const string _chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmopqrstuvwxyz";

      public string randomString(int size)
      {

         char[] buffer = new char[size];

         for (int i = 0; i < size; i++)
         {
            buffer[i] = _chars[_rng.Next(_chars.Length)];
         }
         return new string(buffer);
      }

      public Boolean validateActivationCode(String code)
      {
         using (var db = new SBS2DBContext())
         {
            Activation_Link u = (from a in db.Activation_Link where a.Activation_Code.Equals(code) select a).FirstOrDefault();
            if (u != null)
               return false;
            else
               return true;
         }
      }

      private static string GetUserName(User_Profile user)
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

      public int sendResetPassword(int Profile_ID, string domain)
      {
         DateTime currentdate = StoredProcedure.GetCurrentDate();
         using (var db = new SBS2DBContext())
         {
            try
            {
               User_Profile user = getUser(Profile_ID);

               //GENERATE ACTIVATION CODE
               String code;
               do
               {
                  code = "R" + randomString(40);
               } while (!validateActivationCode(code));


               Activation_Link activation_link = new Activation_Link()
               {
                  //Added by sun 13-10-2015
                  Create_On = currentdate,

                  Activation_Code = code,
                  //SET Time_Limit to activate within LINK_TIME_LIMIT hour
                  Time_Limit = currentdate.AddHours(LINK_TIME_LIMIT),
                  User_Authentication_ID = user.User_Authentication_ID.Value
               };


               db.Activation_Link.Add(activation_link);
               db.SaveChanges();



               try
               {
                  //SEND EMAIL
                  //4		System	Send reset password link to user 	
                  EmailTemplete.sendResetPasswordEmail(user.User_Authentication.Email_Address, code, GetUserName(user), domain);
               }
               catch
               {
                  return ERROR_CODE.ERROR_501_CANT_SEND_EMAIL;
               }

            }
            catch
            {
               return ERROR_CODE.ERROR_500_DB;
            }

         }

         return 1;
      }

      public int resetPassword(int User_Authentication_ID, String PWD)
      {
         using (var db = new SBS2DBContext())
         {
            try
            {
               User_Authentication user = getUserAuthentication(User_Authentication_ID);
               user.PWD = hashSHA256(PWD);

               db.Entry(user).State = EntityState.Modified;
               db.SaveChanges();
            }
            catch
            {
               return ERROR_CODE.ERROR_500_DB;
            }

         }

         return ERROR_CODE.SUCCESS;
      }

      public void setExpireActivationLinkTimeLimit(int link_id)
      {
         DateTime currentdate = StoredProcedure.GetCurrentDate();
         using (var db = new SBS2DBContext())
         {
            Activation_Link link = db.Activation_Link
                .Where(w => w.Activation_ID == link_id)
                .SingleOrDefault();

            link.Time_Limit = currentdate;

            db.Entry(link).State = EntityState.Modified;
            db.SaveChanges();

         }
      }

      #endregion

      public Employee_Profile GetEmployeeProfileByUserProfile(Nullable<int> pProfileID)
      {
         if (pProfileID.HasValue)
         {
            using (var db = new SBS2DBContext())
            {
               return db.Employee_Profile
                    .Include(i => i.User_Profile)
                    .Include(i => i.Nationality)
                    .Where(i => i.Profile_ID == pProfileID)
                    .FirstOrDefault();

            }
         }
         return null;
      }




      //--------------------------------No use temp delete -----------------------------------//

      public Page_Role getPageRole(Nullable<int> pPageRoleID)
      {
         Page_Role prole = new Page_Role();
         using (var db = new SBS2DBContext())
         {
            prole = (from a in db.Page_Role where a.Page_Role_ID == pPageRoleID select a).Include(i => i.Page).FirstOrDefault();
         }

         return prole;
      }

      public Access_Right getAccessRight(Nullable<int> pAccessID)
      {
         Access_Right AccessRight = null;
         try
         {
            using (var db = new SBS2DBContext())
            {
               AccessRight = db.Access_Right
                    .Where(w => w.Access_ID == pAccessID)
                    .Include(i => i.Access_Page.Select(s => s.Page_Role_ID))
                    .FirstOrDefault();
            }
         }
         catch
         {

         }
         return AccessRight;
      }

      public Page getUserPage(Nullable<int> pModuleDetailID = null)
      {
         if (pModuleDetailID.HasValue)
         {
            using (var db = new SBS2DBContext())
            {
               var models = db.Pages
               .Where(w => w.Module_Detail_ID == pModuleDetailID)
               .FirstOrDefault();

               return models;
            }
         }
         return null;
      }

      public List<Page> getPage(Nullable<int> pModelDetailID = null)
      {
         using (var db = new SBS2DBContext())
         {
            return db.Pages.Where(w => w.Module_Detail_ID == pModelDetailID.Value).ToList();
         }
      }

      //public List<User_Role> LstUserRole(int[] moduleDetailsID = null)
      //{
      //    using (var db = new SBS2DBContext())
      //    {

      //        //if (moduleDetailsID != null)
      //        //{
      //        //    return db.User_Role.Where(w => w.User_Role_ID != 1 & w.User_Role_ID != 2 & w.User_Role_ID != 3 & w.Page_Role.Any(a => moduleDetailsID.Contains(a.Page.Module_Detail_ID.Value) | a.Page.Module_Detail_ID == null)).ToList();
      //        //}
      //        //else
      //        //{
      //        //    return db.User_Role.Where(w => w.User_Role_ID != 1 & w.User_Role_ID != 2 & w.User_Role_ID != 3).ToList();
      //        //}

      //        if (moduleDetailsID != null)
      //        {
      //            return db.User_Role.Where(w => w.Page_Role.Any(a => moduleDetailsID.Contains(a.Page.Module_Detail_ID.Value) | a.Page.Module_Detail_ID == null)).ToList();

      //        }
      //        else
      //        {
      //            return db.User_Role.ToList();
      //        }
      //    }
      //}

      public List<SBS_Module_Detail> LstModelDetail(Nullable<int> pModelID = null)
      {
         if (pModelID != null)
         {
            using (var db = new SBS2DBContext())
            {
               var models = db.SBS_Module_Detail
               .Where(w => w.Module_ID == pModelID)
              .OrderBy(o => o.Module_Detail_Name).ToList();

               return models;
            }
         }
         return null;
      }

      public Page getPageforDetailID(Nullable<int> pModelDetailID = null)
      {
         if (pModelDetailID != null)
         {
            using (var db = new SBS2DBContext())
            {
               var pages = db.Pages
               .Include(i => i.SBS_Module_Detail)
               .Where(w => w.Module_Detail_ID == pModelDetailID)
               .FirstOrDefault();

               return pages;
            }
         }
         return null;
      }

      public SBS_Module_Detail getModelDetail(Nullable<int> pModelDetailID)
      {
         using (var db = new SBS2DBContext())
         {
            var role = db.SBS_Module_Detail
            .Where(w => w.Module_Detail_ID == pModelDetailID)
            .FirstOrDefault();

            return role;
         }
      }

      public ServiceResult InsertUserTransaction(User_Transactions pTran)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               db.User_Transactions.Add(pTran);
               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Transaction };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Transaction };
         }


      }

      public List<User_Profile> LstUserProfile(UserCriteria cri)
      {
         using (var db = new SBS2DBContext())
         {
            var users = db.User_Profile
             .Include(i => i.Company)
             .Include(i => i.Company.Company_Details)
             .Include(i => i.User_Authentication)
             .Include(e => e.Employee_Profile)
             .Where(c => c.User_Status != RecordStatus.Delete);

            if (cri != null)
            {
               if (cri.Company_ID.HasValue)
                  users = users.Where(w => w.Company_ID == cri.Company_ID);

               if (!string.IsNullOrEmpty(cri.Email))
                  users = users.Where(w => w.User_Authentication.Email_Address == cri.Email);

               if (!string.IsNullOrEmpty(cri.User_Name))
                  users = users.Where(w => w.User_Authentication.User_Name == cri.User_Name);

            }
            return users.OrderBy(o => o.First_Name).ToList();
         }
      }

      public ServiceObjectResult LstUser(UserCriteria criteria)
      {
         var result = new ServiceObjectResult();
         result.Object = new List<User_Profile>();
         using (var db = new SBS2DBContext())
         {
            List<User_Profile> users = db.User_Profile
               .Include(i => i.Company)
               .Include(i => i.Company.Company_Details)
               .Include(i => i.User_Authentication)
               .Include(e => e.Employee_Profile)
               .Where(c => c.User_Status != RecordStatus.Delete).ToList();

            if (criteria.Company_ID.HasValue && criteria.Company_ID.Value > 0)
               users = users.Where(w => w.Company_ID == criteria.Company_ID.Value).ToList();

            if (!string.IsNullOrEmpty(criteria.Record_Status))
               users = users.Where(w => w.User_Status.Contains(criteria.Record_Status)).ToList();

            if (!string.IsNullOrEmpty(criteria.Text_Search))
               users = users.Where(w =>
                 (!string.IsNullOrEmpty(w.User_Name) && w.User_Name.Contains(criteria.Text_Search)) ||
                 (!string.IsNullOrEmpty(w.Email) && w.Email.Contains(criteria.Text_Search)) ||
                 (!string.IsNullOrEmpty(w.First_Name) && w.First_Name.Contains(criteria.Text_Search)) ||
                 (!string.IsNullOrEmpty(w.Last_Name) && w.Last_Name.Contains(criteria.Text_Search))).ToList();

            if (criteria.Is_Belong_To && criteria.Company_ID.HasValue)
            {
               var belongs = db.Company_Details
                   .Where(w => w.Belong_To_ID == criteria.Company_ID)
                   .Select(s => s.Company_ID).Distinct();

               if (belongs != null && belongs.Count() > 0)
               {
                  foreach (int belong in belongs)
                  {
                     if (belong != criteria.Company_ID)
                     {
                        users.AddRange(getUsersBelongTocompany(belong));
                     }
                  }
               }
            }

            users = users.OrderBy(o => o.Company_ID).ThenByDescending(o => o.First_Name).ToList();
            result.Record_Count = users.Count();
            criteria.Record_Count = result.Record_Count;
            if (result.Record_Count > 300 && criteria.Start_Index == 0 && criteria.Page_Size == 0)
               criteria.Page_Size = 30;

            if (criteria.Top.HasValue)
               users = users.Take(criteria.Top.Value).ToList();

            else if (criteria.End_Index > 0)
               users = users.Skip(criteria.Start_Index).Take(criteria.End_Index).ToList();

            else if (criteria.Page_Size > 0)
            {
               if (criteria.Page_No > 1)
               {
                  var startindex = criteria.Page_Size * (criteria.Page_No - 1);
                  users = users.Skip(startindex).Take(criteria.Page_Size).ToList();
               }
               else
                  users = users.Skip(criteria.Start_Index).Take(criteria.Page_Size).ToList();
            }

            var obj = new List<User_Profile>();
            obj = users.ToList();

            result.Object = obj;
            result.Start_Index = criteria.Start_Index;
            result.Page_Size = criteria.Page_Size;
            return result;
         }
      }

   }

   public class Notification
   {
      public int Leave_Application_Document_ID { get; set; }
      public Nullable<int> Leave_Config_ID { get; set; }
      public Nullable<int> Employee_Profile_ID { get; set; }
      public Nullable<System.DateTime> Start_Date { get; set; }
      public Nullable<System.DateTime> End_Date { get; set; }
      public string Reasons { get; set; }
      public Nullable<System.DateTime> Last_Date_Approved { get; set; }
      public string Address_While_On_Leave { get; set; }
      public string Contact_While_Overseas { get; set; }
      public Nullable<System.Guid> Upload_Document_ID { get; set; }
      public string Period { get; set; }
      public string Remark { get; set; }
      public Nullable<System.DateTime> Date_Applied { get; set; }
      public string Overall_Status { get; set; }
      public string Start_Date_Period { get; set; }
      public string End_Date_Period { get; set; }
      public Nullable<decimal> Days_Taken { get; set; }
      public Nullable<int> Leave_Config_Detail_ID { get; set; }
      public Nullable<int> Request_ID { get; set; }
      public string Leave_Name { get; set; }

      public int Expenses_Application_ID { get; set; }
      public string Expenses_No { get; set; }
      public string Expenses_Title { get; set; }
      public List<Expenses_Application_Document> Expenses_Application_Document { get; set; }
   }

}

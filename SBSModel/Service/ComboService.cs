using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;
using SBSModel.Models;
using SBSModel.Common;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using SBSResourceAPI;


namespace SBSModel.Models
{
   public class ComboType
   {
      public static string Gender = "Gender";
      public static string Bank_Name = "Bank_Name";
      public static string Religion = "Religion";
      public static string Race = "Race";
      public static string WP_Class = "WP_Class";
      public static string Relationship = "Relationship";
      public static string Payment_Type = "Payment_Type";
      public static string Marital_Status = "Marital_Status";
      public static string Daily = "Daily";
      public static string CPF_Type = "CPF_Type";
      public static string Holiday_Lenght = "Holiday_Lenght";
      public static string Leave_Type = "Leave_Type";
      public static string Claimable_Type = "Claimable_Type";
      public static string Employee_Type = "Employee_Type";
      public static string Attachment_Type = "Attachment_Type";
      public static string Work_Pass_Type = "Work_Pass_Type";
      public static string UOM = "UOM";
      public static string Business_Category = "Business_Category";

      //sun
      public static string Expenses_Type = "Expenses_Type";


      public static string Unit_Length = "Unit_Length";
      public static string Unit_Weight = "Unit_Weight";
      public static string Costing_Method = "Costing_Method";
      public static string Carrier = "Carrier";
      public static string Payment_Terms = "Payment_Terms";
      public static string Credit_Card_Type = "Credit_Card_Type";
      public static string POS_Bank_Name = "POS_Bank_Name";
      public static string Discount_Type = "Discount_Type";

      public static string Lead_Type = "Lead_Type";
      public static string Lead_Status = "Lead_Status";
      public static string Lead_Source = "Lead_Source";
      public static string Industry = "Industry";
      public static string Sale_Stage = "Sale_Stage";
      public static string Quotation_Stage = "Quotation_Stage";

   }
   public class ComboService
   {


      #region Authen


      private string GetUserName(User_Profile user)
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

      public List<ComboViewModel> LstModuleDetail()
      {
         using (var db = new SBS2DBContext())
         {
            //return (from a in db.SBS_Module_Detail orderby a.Module_Detail_Name select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Module_Detail_ID).Trim(), Text = a.Module_Detail_Name }).ToList();
            var comboList = new List<ComboViewModel>();

            comboList.Add(new ComboViewModel { Value = "0", Text = "Authentication" });
            comboList.AddRange(from a in db.SBS_Module_Detail orderby a.Module_Detail_Name select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Module_Detail_ID).Trim(), Text = a.Module_Detail_Name });
            return comboList;
         }
      }

      //Added by sun 26-01-2015
      public List<ComboViewModel> LstCompany(bool hasBlank = false)
      {
         using (var db = new SBS2DBContext())
         {
            var comboList = new List<ComboViewModel>();

            if (hasBlank)
            {
               comboList.Add(new ComboViewModel { Value = null, Text = "-" });
            }
            comboList.AddRange(from a in db.Company_Details
                               where a.Company_Status == RecordStatus.Active
                               orderby a.Name
                               select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Company_ID).Trim(), Text = a.Name });
            return comboList;
         }
      }

      public List<ComboViewModel> LstCompanyBelongTo(int? pCompany_ID, bool hasBlank = false)
      {
         using (var db = new SBS2DBContext())
         {
            var comboList = new List<ComboViewModel>();
            if (hasBlank)
               comboList.Add(new ComboViewModel { Value = null, Text = "-" });

            if (pCompany_ID.HasValue)
            {
               comboList.AddRange(from a in db.Company_Details
                                  where a.Company_Status == RecordStatus.Active && a.Company_ID == pCompany_ID
                                  orderby a.Name
                                  select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Company_ID).Trim(), Text = a.Name });

               comboList.AddRange(from a in db.Company_Details
                                  where a.Company_Status == RecordStatus.Active && a.Belong_To_ID == pCompany_ID
                                  orderby a.Name
                                  select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Company_ID).Trim(), Text = a.Name });
            }

            return comboList;
         }
      }

      //Added By sun
      public List<ComboViewModel> LstUserRole()
      {
         using (var db = new SBS2DBContext())
         {
            return (from a in db.User_Role select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.User_Role_ID).Trim(), Text = a.Role_Name }).ToList();
         }
      }

      //Added By sun 03-09-2015
      public List<ComboViewModel> LstAccessRights()
      {

         using (var db = new SBS2DBContext())
         {
            return (from a in db.Access_Right select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Access_ID).Trim(), Text = a.Access_Description }).ToList();
         }


      }

      //Edit By sun
      public List<ComboViewModel> LstPage(Nullable<int> pModDeltID = null)
      {
         using (var db = new SBS2DBContext())
         {
            if (pModDeltID.HasValue & pModDeltID != 0)
            {
               return (from a in db.Pages.Where(w => w.Module_Detail_ID == pModDeltID.Value)
                       orderby a.SBS_Module_Detail.Module_Detail_Name, a.Page_Url
                       select new ComboViewModel
                       {
                          Value = SqlFunctions.StringConvert((double)a.Page_ID).Trim(),
                          //Text = (a.SBS_Module_Detail != null ? a.SBS_Module_Detail.Module_Detail_Name : "Authentication") + " : " + a.Page_Url
                          Text = a.Page_Url
                       }
                    )
                .ToList();

            }
            else
            {

               return (from a in db.Pages.Where(w => w.Module_Detail_ID == null)
                       orderby a.SBS_Module_Detail.Module_Detail_Name, a.Page_Url
                       select new ComboViewModel
                       {
                          Value = SqlFunctions.StringConvert((double)a.Page_ID).Trim(),
                          //Text = (a.SBS_Module_Detail != null ? a.SBS_Module_Detail.Module_Detail_Name : "Authentication") + " : " + a.Page_Url
                          Text = a.Page_Url
                       }
                           )
                       .ToList();
            }
         }
      }

      //Added By sun
      public List<ComboViewModel> LstRecordStatus(bool hasBlank = false)
      {
         var clist = new List<ComboViewModel>();

         if (hasBlank)
         {
            clist.Add(new ComboViewModel { Value = null, Text = "-" });
         }

         clist.Add(new ComboViewModel { Value = RecordStatus.Active, Text = Resource.Active });
         clist.Add(new ComboViewModel { Value = RecordStatus.Inactive, Text = Resource.Inactive });
         return clist;
      }

      //Added By sun 24-08-2015
      public List<ComboViewModel> LstPer()
      {
         var clist = new List<ComboViewModel>();
         clist.Add(new ComboViewModel { Value = ClaimableType.Per_Employee, Text = Resource.Per_Employee });
         clist.Add(new ComboViewModel { Value = ClaimableType.Per_Department, Text = Resource.Per_Department });
         return clist;
      }

      //Added By sun
      public List<ComboViewModel> LstStatus()
      {
         var clist = new List<ComboViewModel>();
         clist.Add(new ComboViewModel { Value = RecordStatus.Active, Text = Resource.Active });
         clist.Add(new ComboViewModel { Value = RecordStatus.Inactive, Text = Resource.Inactive });
         return clist;
      }

      public List<ComboViewModel> LstStatus(bool hasBlank = false)
      {
         var clist = new List<ComboViewModel>();
         if (hasBlank)
         {
            clist.Add(new ComboViewModel { Value = "", Text = "-" });
         }
         clist.Add(new ComboViewModel { Value = "Y", Text = Resource.Yes });
         clist.Add(new ComboViewModel { Value = "N", Text = Resource.No });
         return clist;
      }

      //Added By sun
      public List<ComboViewModel> LstCompanylevel(string pCompanyLvl, bool hasBlank = true)
      {
         using (var db = new SBS2DBContext())
         {
            var comboList = new List<ComboViewModel>();

            //if (hasBlank)
            //{
            //    comboList.Add(new ComboViewModel { Value = "", Text = "-" });
            //}

            if (pCompanyLvl == Companylevel.Mainmaster)
            {
               comboList.Add(new ComboViewModel { Value = Companylevel.Mainmaster, Text = Companylevel.Mainmaster });
            }
            if ((pCompanyLvl == Companylevel.Mainmaster) || (pCompanyLvl == Companylevel.Franchise))
            {
               comboList.Add(new ComboViewModel { Value = Companylevel.Franchise, Text = Companylevel.Franchise });
            }
            if ((pCompanyLvl == Companylevel.Mainmaster) || (pCompanyLvl == Companylevel.Franchise) || (pCompanyLvl == Companylevel.Whitelabel))
            {
               comboList.Add(new ComboViewModel { Value = Companylevel.Whitelabel, Text = Companylevel.Whitelabel });
            }
            if ((pCompanyLvl == Companylevel.Mainmaster) || (pCompanyLvl == Companylevel.Franchise) || (pCompanyLvl == Companylevel.Whitelabel) || (pCompanyLvl == Companylevel.EndUser))
            {
               comboList.Add(new ComboViewModel { Value = Companylevel.EndUser, Text = Companylevel.EndUser });
            }

            return comboList;
         }
      }

      //Added By sun
      public List<ComboViewModel> LstCompanylevelNoMe(string pCompanyLvl, bool hasBlank = true)
      {
         using (var db = new SBS2DBContext())
         {
            var comboList = new List<ComboViewModel>();

            if ((pCompanyLvl == Companylevel.Mainmaster))
            {
               comboList.Add(new ComboViewModel { Value = Companylevel.Franchise, Text = Companylevel.Franchise });
            }
            if ((pCompanyLvl == Companylevel.Mainmaster) || (pCompanyLvl == Companylevel.Franchise))
            {
               comboList.Add(new ComboViewModel { Value = Companylevel.Whitelabel, Text = Companylevel.Whitelabel });
            }
            if ((pCompanyLvl == Companylevel.Mainmaster) || (pCompanyLvl == Companylevel.Franchise) || (pCompanyLvl == Companylevel.Whitelabel))
            {
               comboList.Add(new ComboViewModel { Value = Companylevel.EndUser, Text = Companylevel.EndUser });
            }

            return comboList;
         }
      }

      public List<ComboViewModel> LstDuration(bool hasBlank = true)
      {
         using (var db = new SBS2DBContext())
         {
            var comboList = new List<ComboViewModel>();


            if (hasBlank)
            {
               comboList.Add(new ComboViewModel { Value = SqlFunctions.StringConvert((double)Convert.ToInt32(DurationEnum.None)).Trim(), Text = "-" });
            }
            comboList.Add(new ComboViewModel { Value = SqlFunctions.StringConvert((double)Convert.ToInt32(DurationEnum._15Days)).Trim(), Text = "15 " + Resource.Days });
            comboList.Add(new ComboViewModel { Value = SqlFunctions.StringConvert((double)Convert.ToInt32(DurationEnum._3Months)).Trim(), Text = "3 " + Resource.Months });
            comboList.Add(new ComboViewModel { Value = SqlFunctions.StringConvert((double)Convert.ToInt32(DurationEnum._6Months)).Trim(), Text = "6 " + Resource.Months });
            comboList.Add(new ComboViewModel { Value = SqlFunctions.StringConvert((double)Convert.ToInt32(DurationEnum._1Year)).Trim(), Text = "1 " + Resource.Year });
            comboList.Add(new ComboViewModel { Value = SqlFunctions.StringConvert((double)Convert.ToInt32(DurationEnum._2Years)).Trim(), Text = "2 " + Resource.Years });
            return comboList;
         }
      }

      public List<ComboViewModel> LstCountry(bool hasBlank = true)
      {
         using (var db = new SBS2DBContext())
         {
            var comboList = new List<ComboViewModel>();
            if (hasBlank)
               comboList.Add(new ComboViewModel { Value = null, Text = "-", Desc = "-" });

            comboList.AddRange(from a in db.Countries select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Country_ID).Trim(), Text = a.Description, Desc = a.Description });
            return comboList;
         }
      }

      public List<ComboViewModel> LstState(string pCountry_ID, bool hasBlank = true)
      {
         using (var db = new SBS2DBContext())
         {
            var comboList = new List<ComboViewModel>();
            if (!string.IsNullOrEmpty(pCountry_ID))
            {
               var clist = (from a in db.States select a);
               int intCountryID = Convert.ToInt32(pCountry_ID);
               clist = (from a in clist where a.Country_ID.Equals(intCountryID) select a);
               if (hasBlank)
                  comboList.Add(new ComboViewModel { Value = null, Text = "-", Desc = "-" });

               comboList.AddRange(from a in clist select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.State_ID).Trim(), Text = a.Descrition });
            }

            return comboList;
         }
      }

      public List<ComboViewModel> LstBranch(Nullable<int> pCompanyID, bool hasBlank = true)
      {
         using (var db = new SBS2DBContext())
         {
            var clist = (from a in db.Branches where a.Company_ID == pCompanyID orderby a.Branch_Code select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Branch_ID).Trim(), Text = a.Branch_Code, Desc = a.Branch_Name });
            var p = clist.ToList();
            if (hasBlank)
            {
               p.Insert(0, new ComboViewModel() { Value = null, Text = "-" });
            }
            return p;
         }
      }

      public List<ComboViewModel> LstCurrency(bool hasBlank = true)
      {
         using (var db = new SBS2DBContext())
         {
            var comboList = new List<ComboViewModel>();
            if (hasBlank)
            {
               comboList.Add(new ComboViewModel { Value = null, Text = "-" });
            }
            comboList.AddRange(from a in db.Currencies orderby a.Currency_Code select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Currency_ID).Trim(), Text = a.Currency_Code + " : " + a.Currency_Name });
            return comboList;
         }
      }

      public List<ComboViewModel> LstCurrencyCode(bool hasBlank = true)
      {
         using (var db = new SBS2DBContext())
         {
            var comboList = new List<ComboViewModel>();
            if (hasBlank)
            {
               comboList.Add(new ComboViewModel { Value = null, Text = "-" });
            }
            comboList.AddRange(from a in db.Currencies orderby a.Currency_Code select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Currency_ID).Trim(), Text = a.Currency_Code });
            return comboList;
         }
      }

      public Currency GetCurrencyCode(Nullable<int> pCurID)
      {
         using (var db = new SBS2DBContext())
         {
            return db.Currencies.Where(w => w.Currency_ID == pCurID).FirstOrDefault();
         }
      }

      public Currency GetCurrencyCodeByCountry(Nullable<int> pCountryID)
      {
         using (var db = new SBS2DBContext())
         {
            return db.Currencies.Where(w => w.Country_ID == pCountryID).FirstOrDefault();
         }
      }

      public List<ComboViewModel> LstBusinessType(bool hasBlank = true)
      {
         using (var db = new SBS2DBContext())
         {
            var comboList = new List<ComboViewModel>();

            if (hasBlank)
            {
               comboList.Add(new ComboViewModel { Value = null, Text = "-" });
            }
            comboList.Add(new ComboViewModel { Value = "FB", Text = "Food & Beverage" });
            return comboList;
         }
      }

      public List<ComboViewModel> LstModuleSubscription(Nullable<int> pCompanyID, bool hasBlank = true)
      {
         using (var db = new SBS2DBContext())
         {
            var comboList = new List<ComboViewModel>();

            if (hasBlank)
            {
               comboList.Add(new ComboViewModel { Value = null, Text = "-" });
            }
            comboList.AddRange(from a in db.Subscriptions where a.Company_ID == pCompanyID orderby a.SBS_Module_Detail.Module_Detail_Name select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Subscription_ID).Trim(), Text = a.SBS_Module_Detail.Module_Detail_Name });
            return comboList;
         }
      }

      public List<ComboViewModel> LstLookupType(bool hasBlank = true)
      {
         using (var db = new SBS2DBContext())
         {
            return (from a in db.Global_Lookup_Def orderby a.Description select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Def_ID).Trim(), Text = a.Description }).ToList();
         }
      }

      public List<ComboViewModel> LstCustomer(Nullable<int> pCompanyID, bool hasBlank = true)
      {
         using (var db = new SBS2DBContext())
         {
            var clist = (from a in db.Customers where a.Company_ID == pCompanyID && a.Record_Status == RecordStatus.Active orderby a.Customer_Name select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Customer_ID).Trim(), Text = a.Customer_Name });
            var p = clist.ToList();
            if (hasBlank)
            {
               p.Insert(0, new ComboViewModel() { Value = "", Text = "-" });
            }
            return p;
         }
      }

      public List<ComboViewModel> LstJobCost(Nullable<int> pCompanyID, bool hasBlank = true)
      {
         using (var db = new SBS2DBContext())
         {
            var clist = (from a in db.Job_Cost where a.Company_ID == pCompanyID && a.Record_Status == RecordStatus.Active orderby a.Indent_Name select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Job_Cost_ID).Trim(), Text = a.Indent_No + " : " + a.Indent_Name });
            var p = clist.OrderBy(o => o.Text).ToList();
            if (hasBlank)
            {
               p.Insert(0, new ComboViewModel() { Value = "", Text = "-" });
            }
            return p;
         }
      }

      public List<ComboViewModel> LstPaymentPeriod(bool hasBlank = false)
      {
         var clist = new List<ComboViewModel>();

         if (hasBlank)
         {
            clist.Add(new ComboViewModel { Value = null, Text = "-" });
         }

         clist.Add(new ComboViewModel { Value = "P", Text = "%" });
         clist.Add(new ComboViewModel { Value = "S", Text = "$" });

         return clist;
      }

      #endregion

      #region HR

      public Nationality GetNationality(Nullable<int> pNationalityID)
      {
         using (var db = new SBS2DBContext())
         {
            return db.Nationalities.Where(w => w.Nationality_ID == pNationalityID).FirstOrDefault();
         }
      }

      public List<ComboViewModel> LstNationality(bool hasBlank = true)
      {
         using (var db = new SBS2DBContext())
         {
            var clist = (from a in db.Nationalities orderby a.Name select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Nationality_ID).Trim(), Text = a.Description, Desc = a.Description });
            var p = clist.ToList();
            if (hasBlank)
            {
               //p.Insert(0, new ComboViewModel() { Value = null, Text = "-" });
            }
            return p;
         }
      }

      public List<ComboViewModel> LstEmpStatus()
      {
         var clist = new List<ComboViewModel>();
         clist.Add(new ComboViewModel { Value = "C", Text = "Contract" });
         clist.Add(new ComboViewModel { Value = "A", Text = "Active" });
         clist.Add(new ComboViewModel { Value = "P", Text = "Probation" });
         clist.Add(new ComboViewModel { Value = "R", Text = "Resigned" });
         clist.Add(new ComboViewModel { Value = "T", Text = "Terminated" });
         clist.Add(new ComboViewModel { Value = "D", Text = "Deceased" });
         clist.Add(new ComboViewModel { Value = "I", Text = "Internship" });
         return clist;
      }

      public List<ComboViewModel> LstEmployee(Nullable<int> pCompanyID, bool hasBlank = false)
      {
         List<ComboViewModel> employee = new List<ComboViewModel>();
         using (var db = new SBS2DBContext())
         {
            var emps = db.Employee_Profile.Include(i => i.User_Profile)
               .Where(w => w.User_Profile.Company_ID == pCompanyID && w.User_Profile.User_Status == RecordStatus.Active);
            foreach (var emp in emps)
            {
               if (emp.User_Profile != null)
                  employee.Add(new ComboViewModel() { Value = emp.Employee_Profile_ID.ToString(), Text = GetUserName(emp.User_Profile), Desc = emp.User_Profile.User_Authentication.Email_Address });
            }
            if (hasBlank)
               employee.Add(new ComboViewModel { Value = null, Text = "-" });

            return employee.OrderBy(o => o.Text).ToList();
         }
      }

      public List<ComboViewModel> LstEmployeeList(Nullable<int> pCompanyID, Nullable<int> pDepartmentID = null, bool hasBlank = false)
      {
         using (var db = new SBS2DBContext())
         {
            var employee = new List<ComboViewModel>();
            var emps = db.Employee_Profile.Where(w => w.User_Profile.Company_ID == pCompanyID && w.User_Profile.User_Status == RecordStatus.Active);
            if (pDepartmentID.HasValue)
            {

               var currentdate = StoredProcedure.GetCurrentDate();
               foreach (var emp in emps)
               {
                  var hist = emp.Employment_History.Where(w => w.Effective_Date <= currentdate).OrderByDescending(o => o.Effective_Date).FirstOrDefault();
                  if (hist != null)
                  {
                     if (pDepartmentID.HasValue && pDepartmentID.Value > 0 && hist.Department_ID == pDepartmentID)
                        employee.Add(new ComboViewModel() { Value = emp.Employee_Profile_ID.ToString(), Text = GetUserName(emp.User_Profile), Desc = emp.User_Profile.User_Authentication.Email_Address });
                  }
               }
            }
            else
            {
               foreach (var emp in emps)
               {
                  employee.Add(new ComboViewModel() { Value = emp.Employee_Profile_ID.ToString(), Text = GetUserName(emp.User_Profile), Desc = emp.User_Profile.User_Authentication.Email_Address });
               }
            }
            if (hasBlank)
               employee.Add(new ComboViewModel { Value = null, Text = "-" });

            return employee.OrderBy(o => o.Text).ToList();
         }

      }

      public List<ComboViewModel> LstResidentialStatus(Nullable<bool> isSG = false)
      {
         var clist = new List<ComboViewModel>();
         clist.Add(new ComboViewModel { Value = ResidentialStatus.Local, Text = "Local" });
         if (!isSG.HasValue || !isSG.Value)
         {
            clist.Add(new ComboViewModel { Value = ResidentialStatus.PermanentResident, Text = "Permanent Resident" });
            clist.Add(new ComboViewModel { Value = ResidentialStatus.Foreigner, Text = "Foreigner" });
         }

         return clist;
      }

      public List<ComboViewModel> LstChildType(bool hasBlank = false)
      {
         var clist = new List<ComboViewModel>();
         if (hasBlank)
         {
            clist.Add(new ComboViewModel { Value = null, Text = "-" });
         }
         clist.Add(new ComboViewModel { Value = ChildType.OwnChild, Text = Resource.Own_Child });
         clist.Add(new ComboViewModel { Value = ChildType.AdoptedChild, Text = Resource.Adopted_Child });
         return clist;
      }

      public List<ComboViewModel> LstLookup(string pName, Nullable<int> pCompanyID = null, bool hasBlank = false)
      {
         using (var db = new SBS2DBContext())
         {

            var clist = (from a in db.Global_Lookup_Data
                         where a.Global_Lookup_Def.Name == pName &
                             (a.Company_ID == pCompanyID | a.Company_ID == null) &
                             a.Record_Status == RecordStatus.Active
                         orderby a.Description
                         select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Lookup_Data_ID).Trim(), Text = a.Name, Desc = a.Description }).ToList();

            if (clist == null) clist = new List<ComboViewModel>();

            if (hasBlank)
            {
               clist.Insert(0, new ComboViewModel() { Value = null, Text = "-" });
            }
            return clist.ToList();

         }
      }

      public Global_Lookup_Data GetLookup(Nullable<int> pLookupID)
      {
         if (pLookupID.HasValue)
         {
            using (var db = new SBS2DBContext())
            {
               return (from a in db.Global_Lookup_Data where a.Lookup_Data_ID == pLookupID.Value select a).SingleOrDefault();
            }
         }
         return null;
      }

      public Global_Lookup_Data GetLookup(string pName, Nullable<int> pCompanyID = null)
      {
         if (pCompanyID.HasValue)
         {
            using (var db = new SBS2DBContext())
            {
               return (from a in db.Global_Lookup_Data
                       where a.Name == pName &
                           (a.Company_ID == pCompanyID | a.Company_ID == null)
                       select a).FirstOrDefault();
            }
         }
         return null;
      }

      //Added By sun 26-08-2015
      public List<ComboViewModel> LstExpensesCategory(Nullable<int> pCompanyID, bool hasBlank = false)
      {
         using (var db = new SBS2DBContext())
         {
            if (pCompanyID.HasValue)
            {
               var clist = (from a in db.Expenses_Category
                            where a.Company_ID == pCompanyID && a.Record_Status == RecordStatus.Active
                            orderby a.Category_Name
                            select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Expenses_Category_ID).Trim(), Text = a.Category_Name }).ToList();
               if (hasBlank)
               {
                  clist.Insert(0, new ComboViewModel { Value = "", Text = "-" });
               }
               return clist;
            }
         }
         return new List<ComboViewModel>();
      }

      public List<ComboViewModel> LstDepartment(Nullable<int> pCompanyID, bool hasBlank = false)
      {
         using (var db = new SBS2DBContext())
         {
            if (pCompanyID.HasValue)
            {

               var clist = (from a in db.Departments
                            where a.Record_Status == RecordStatus.Active &
                            a.Company_ID == pCompanyID
                            orderby a.Name
                            select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Department_ID).Trim(), Text = a.Name }).ToList();
               if (hasBlank)
               {
                  clist.Insert(0, new ComboViewModel { Value = "", Text = "-" });
               }

               return clist;
            }
         }
         return new List<ComboViewModel>();
      }

      public List<ComboViewModel> LstDesignation(Nullable<int> pCompanyID, bool haveAllRow = false)
      {
         using (var db = new SBS2DBContext())
         {
            if (pCompanyID.HasValue)
            {

               List<ComboViewModel> p = (from a in db.Designations
                                         where a.Record_Status == RecordStatus.Active & a.Company_ID == pCompanyID
                                         orderby a.Name
                                         select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Designation_ID).Trim(), Text = a.Name }).ToList();

               if (p == null) p = new List<ComboViewModel>();

               if (haveAllRow)
               {
                  p.Insert(0, new ComboViewModel() { Value = "0", Text = "-" });
               }

               return p;
            }
         }
         return new List<ComboViewModel>();
      }

      public List<ComboViewModel> LstSupervisor(Nullable<int> pDepartmentID)
      {
         var sup = new List<ComboViewModel>();
         var currentdate = StoredProcedure.GetCurrentDate();
         using (var db = new SBS2DBContext())
         {
            //sup.Add(new ComboViewModel() { Value = "0", Text = "-" });
            if (pDepartmentID.HasValue)
            {
               var dp = db.Departments.Where(w => w.Department_ID == pDepartmentID).FirstOrDefault();
               if (dp != null)
               {
                  var emps = db.Employee_Profile.Where(w => w.User_Profile.Company_ID == dp.Company_ID);
                  foreach (var emp in emps)
                  {
                     var hist = (from a in db.Employment_History
                                 where a.Effective_Date <= currentdate & a.Employee_Profile_ID == emp.Employee_Profile_ID
                                 orderby a.Effective_Date descending
                                 select a).FirstOrDefault();

                     if (hist != null && hist.Department_ID == pDepartmentID)
                     {
                        sup.Add(new ComboViewModel { Value = emp.Employee_Profile_ID.ToString(), Text = GetUserName(emp.User_Profile) });
                     }
                  }

                  sup.Insert(0, new ComboViewModel() { Value = "0", Text = "-" });
               }
            }
         }
         return sup;
      }

      public List<ComboViewModel> LstEmpUnderMe(Nullable<int> pProfileID)
      {
         using (var db = new SBS2DBContext())
         {
            var dp = db.Employee_Profile.Where(w => w.Profile_ID == pProfileID).FirstOrDefault();

            var q = db.Employment_History
                .Include(eh => eh.Employee_Profile)
                .Include(eh => eh.Employee_Profile.User_Profile)
                .Where(eh => eh.Supervisor == dp.Employee_Profile_ID);

            //var dp = db.Employee_Profile.Where(w => w.Profile_ID == pProfileID).FirstOrDefault();
            //var emps = db.Employment_History.GroupBy(g => g.Employee_Profile_ID)
            //    .Select( s=> s.Where(w => w.Supervisor == dp.Employee_Profile_ID));

            var employee = new List<ComboViewModel>();

            if (pProfileID.HasValue && q != null)
            {
               var currentdate = StoredProcedure.GetCurrentDate();
               foreach (var emp in q.ToList())
               {
                  var empID = emp.Employee_Profile_ID;
                  var profileID = emp.Employee_Profile.Profile_ID;
                  var hist = db.Employment_History.Where(w => w.Employee_Profile_ID == empID && w.Effective_Date <= currentdate).OrderByDescending(o => o.Effective_Date).FirstOrDefault();
                  if (hist != null)
                  {
                     employee.Add(new ComboViewModel() { Value = profileID.ToString(), Text = GetUserName(emp.Employee_Profile.User_Profile), Desc = emp.Employee_Profile.User_Profile.User_Authentication.Email_Address });
                  }
               }
            }
            else
            {
               //foreach (var emp in emps)
               //{
               //    employee.Add(new ComboViewModel() { Value = emp.Employee_Profile_ID.ToString(), Text = GetUserName(emp.User_Profile), Desc = emp.User_Profile.User_Authentication.Email_Address });
               //}
            }
            //if (hasBlank)
            //    employee.Add(new ComboViewModel { Value = null, Text = "-" });

            return employee.OrderBy(o => o.Text).ToList();
         }
      }

      public List<ComboViewModel> LstDatePeriod(bool hasBlank = false)
      {
         var clist = new List<ComboViewModel>();
         if (hasBlank)
         {
            clist.Add(new ComboViewModel { Value = null, Text = "-" });
         }
         clist.Add(new ComboViewModel { Value = "AM", Text = "AM" });
         clist.Add(new ComboViewModel { Value = "PM", Text = "PM" });
         return clist;
      }

      public List<ComboViewModel> LstLeaveType(Nullable<int> pCompanyID = null, Nullable<int> pLeaveConfigID = null, string pConfType = null, bool pParentOnly = true, bool hasBlank = false)
      {
         /*Edited By Jane 03/02/2016*/
         using (var db = new SBS2DBContext())
         {
            if (pCompanyID.HasValue)
            {
               var lconfigs = db.Leave_Config.Where(w => w.Company_ID == pCompanyID && (w.Record_Status == RecordStatus.Active | w.Record_Status == null));

               if (pLeaveConfigID.HasValue && pLeaveConfigID.Value > 0)
                  lconfigs = lconfigs.Where(w => w.Leave_Config_ID == pLeaveConfigID);

               if (!string.IsNullOrEmpty(pConfType))
                  lconfigs = lconfigs.Where(w => w.Type == pConfType);

               if (pParentOnly)/*filter only parent level*/
                  lconfigs = lconfigs.Where(w => w.Leave_Config_Parent_ID == null);

               var clist = lconfigs
                   .Select(s => new ComboViewModel { Value = SqlFunctions.StringConvert((double)s.Leave_Config_ID).Trim(), Text = s.Leave_Name })
                   .OrderBy(o => o.Text)
                   .ToList();

               if (clist == null)
                  clist = new List<ComboViewModel>();

               if (hasBlank)
                  clist.Insert(0, new ComboViewModel() { Value = null, Text = "-" });

               return clist;
            }
         }
         return new List<ComboViewModel>();
      }

      //public List<ComboViewModel> LstLeaveType(Nullable<int> pCompanyID, bool hasBlank = false)
      //{
      //    var currentdate = StoredProcedure.GetCurrentDate();
      //    using (var db = new SBS2DBContext())
      //    {
      //        var clist = (from a in db.Leave_Config
      //                     where a.Company_ID == pCompanyID
      //                     select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Leave_Config_ID).Trim(), Text = a.Leave_Name }).ToList();

      //        if (clist == null)
      //            clist = new List<ComboViewModel>();

      //        if (hasBlank)
      //            clist.Insert(0, new ComboViewModel() { Value = null, Text = "-" });


      //        return clist.ToList();
      //    }
      //}


      /********  Please don't use usually *******/
      //Edited by Jane 03/02/2016
      public List<ComboViewModel> LstAndCalulateLeaveType(LeaveTypeCriteria cri)
      {
         /* Filter only available leave per person*/
         var leavetypelist = new List<ComboViewModel>();
         var lService = new LeaveService();

         var ltypes = lService.LstAndCalulateLeaveType(cri);

         leavetypelist = ltypes
             .Select(a => new ComboViewModel
             {
                Value = a.Leave_Config.Leave_Config_ID.ToString(),
                Text = a.Leave_Config.Leave_Name
             }).ToList();

         if (cri.hasBlank)
            leavetypelist.Insert(0, new ComboViewModel { Value = "", Text = "-" });

         return leavetypelist;
      }

      //Added by sun 11-01-2015
      public List<DateTime> GetDates(int year, int month)
      {
         var dates = new List<DateTime>();

         // Loop from the first day of the month until we hit the next month, moving forward a day at a time
         for (var date = new DateTime(year, month, 1); date.Month == month; date = date.AddDays(1))
         {
            dates.Add(date);
         }

         return dates;
      }

      public List<ComboViewModel> LstMonth(bool hasBlank = false)
      {
         var clist = new List<ComboViewModel>();
         if (hasBlank)
         {
            clist.Add(new ComboViewModel { Value = "", Text = "-" });
         }
         clist.Add(new ComboViewModel { Value = "1", Text = Resource.January });
         clist.Add(new ComboViewModel { Value = "2", Text = Resource.February });
         clist.Add(new ComboViewModel { Value = "3", Text = Resource.March });
         clist.Add(new ComboViewModel { Value = "4", Text = Resource.April });
         clist.Add(new ComboViewModel { Value = "5", Text = Resource.May });
         clist.Add(new ComboViewModel { Value = "6", Text = Resource.June });
         clist.Add(new ComboViewModel { Value = "7", Text = Resource.July });
         clist.Add(new ComboViewModel { Value = "8", Text = Resource.August });
         clist.Add(new ComboViewModel { Value = "9", Text = Resource.September });
         clist.Add(new ComboViewModel { Value = "10", Text = Resource.October });
         clist.Add(new ComboViewModel { Value = "11", Text = Resource.November });
         clist.Add(new ComboViewModel { Value = "12", Text = Resource.December });
         return clist;
      }

      //Added by sun 18-12-2015
      public List<ComboViewModel> LstExpensesType(Nullable<int> pCompanyID = null, Nullable<int> pExpensesID = null, bool hasBlank = false)
      {

         var currentdate = StoredProcedure.GetCurrentDate();
         var Expentypelist = new List<ComboViewModel>();

         using (var db = new SBS2DBContext())
         {

            if (pCompanyID.HasValue)
            {
               var econfig = (from a in db.Expenses_Config
                              where a.Company_ID == pCompanyID &&
                                  a.Record_Status != RecordStatus.Delete
                              select a);

               if (pExpensesID.HasValue && pExpensesID.Value > 0)
               {
                  econfig = (from a in econfig where a.Expenses_Config_ID != pExpensesID select a);
               }

               Expentypelist = (from a in econfig
                                where (a.Company_ID == pCompanyID | a.Company_ID == null)
                                orderby a.Expenses_Name
                                select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Expenses_Config_ID).Trim(), Text = a.Expenses_Name }).ToList();
            }
         }
         if (hasBlank)
         {
            Expentypelist.Insert(0, new ComboViewModel { Value = "", Text = "-" });
         }
         return Expentypelist;

      }

      public List<ComboViewModel> LstApprovalStatus(bool hasBlank = false)
      {
         var clist = new List<ComboViewModel>();
         clist.Add(new ComboViewModel { Value = null, Text = "-" });
         if (hasBlank)
            clist.Add(new ComboViewModel { Value = SBSWorkFlowAPI.Constants.WorkflowStatus.Draft, Text = Resource.Draft });

         clist.Add(new ComboViewModel { Value = SBSWorkFlowAPI.Constants.WorkflowStatus.Pending, Text = Resource.Pending });
         clist.Add(new ComboViewModel { Value = SBSWorkFlowAPI.Constants.WorkflowStatus.Closed, Text = Resource.Closed });
         clist.Add(new ComboViewModel { Value = SBSWorkFlowAPI.Constants.WorkflowStatus.Rejected, Text = Resource.Rejected });
         clist.Add(new ComboViewModel { Value = SBSWorkFlowAPI.Constants.WorkflowStatus.Cancelled, Text = Resource.Cancelled });
         return clist;
      }

      //Add by sun 17-09-2015
      public List<ComboViewModel> LstDefaultType(bool hasBlank = false)
      {
         var clist = new List<ComboViewModel>();
         if (hasBlank)
         {
            clist.Add(new ComboViewModel { Value = null, Text = "-" });
         }
         clist.Add(new ComboViewModel { Value = LeaveConfigType.Normal, Text = Resource.Normal });
         clist.Add(new ComboViewModel { Value = LeaveConfigType.Child, Text = Resource.Child });
         return clist;
      }

      //Added by Nay on 28-Sept-2015
      public List<ComboViewModel> LstCompanySource()
      {
         var clist = new List<ComboViewModel>();
         clist.Add(new ComboViewModel { Value = "1", Text = "Mindef" });
         clist.Add(new ComboViewModel { Value = "4", Text = "Govt Dept" });
         clist.Add(new ComboViewModel { Value = "5", Text = "Statutory Boards" });
         clist.Add(new ComboViewModel { Value = "6", Text = "Private Sector" });
         clist.Add(new ComboViewModel { Value = "9", Text = "Others" });
         return clist;
      }

      //Added by Nay on 28-Sept-2015
      public List<ComboViewModel> LstPayerIDTypes()
      {
         var clist = new List<ComboViewModel>();
         clist.Add(new ComboViewModel { Value = "7", Text = "UEN – Business (ROB)" });
         clist.Add(new ComboViewModel { Value = "8", Text = "UEN – Local Company (ROC)" });
         clist.Add(new ComboViewModel { Value = "U", Text = "UEN – Others" });
         clist.Add(new ComboViewModel { Value = "A", Text = "ASGD" });
         clist.Add(new ComboViewModel { Value = "T", Text = "ITR" });
         return clist;
      }

      //Added by Jane on 29-Sept-2015
      public List<ComboViewModel> LstAdjustmentType()
      {
         var clist = new List<ComboViewModel>();
         clist.Add(new ComboViewModel { Value = "Plus", Text = Resource.Plus });
         clist.Add(new ComboViewModel { Value = "Minus", Text = Resource.Minus });
         return clist;
      }

      //Added by sun 12-02-2016
      public List<ComboViewModel> LstCurrencyByCompany(Nullable<int> pCompanyID = null, string pYear = null)
      {
         var Currencylist = new List<ComboViewModel>();
         List<int> chk_Dup = new List<int>();

         try
         {
            using (var db = new SBS2DBContext())
            {
               var exchange = db.Exchanges
                               .Where(w => w.Company_ID == pCompanyID && w.Record_Status == RecordStatus.Active)
                               .Include(i => i.Exchange_Currency)
                               .Include(i => i.Exchange_Currency.Select(s => s.Exchange_Rate))
                               .Where(w => w.Fiscal_Year.ToString() == pYear).FirstOrDefault();

               var currens = db.Currencies.ToList();
               var com = db.Company_Details.Where(w => w.Company_ID == pCompanyID).FirstOrDefault();

               var currencom = currens.Where(w => w.Currency_ID == com.Currency_ID).FirstOrDefault();
               if (currencom != null)
               {
                  Currencylist.Add(new ComboViewModel { Text = currencom.Currency_Code + " : " + currencom.Currency_Name, Value = currencom.Currency_ID.ToString().Trim() });
                  chk_Dup.Add(currencom.Currency_ID);
               }

               if (exchange != null && exchange.Exchange_Currency != null)
               {
                  foreach (var ex in exchange.Exchange_Currency)
                  {
                     if (!chk_Dup.Contains(ex.Currency_ID.Value))
                     {
                        bool ck = false;
                        //var curren = currens.Where(w => w.Currency_ID == ex.Currency_ID).FirstOrDefault();
                        //if (curren != null)
                        //    Currencylist.Add(new ComboViewModel { Text = curren.Currency_Code + " : " + curren.Currency_Name, Value = curren.Currency_ID.ToString().Trim() });
                        if (ex.Exchange_Period == ExchangePeriod.ByMonth)
                        {
                           var cMonth = ex.Exchange_Rate.Count();
                           if (ex.Exchange_Rate != null && ex.Exchange_Rate.Count() == 12 && ex.Exchange_Period == ExchangePeriod.ByMonth)
                           {
                              foreach (var r in ex.Exchange_Rate)
                              {
                                 if (r.Rate.Value > 0)
                                 {
                                    ck = true;
                                 }
                                 else
                                 {
                                    ck = false;
                                    break;
                                 }
                              }
                           }
                        }
                        else
                        {
                           var cDate = ex.Exchange_Rate.GroupBy(n => n.Exchange_Month).Count();
                           if (ex.Exchange_Rate != null && ex.Exchange_Rate.GroupBy(n => n.Exchange_Month).Count() == 12 && ex.Exchange_Period == ExchangePeriod.ByDate)
                           {
                              foreach (var r in ex.Exchange_Rate)
                              {
                                 if (r.Rate.Value > 0 && r.Exchange_Date != null)
                                 {
                                    ck = true;
                                 }
                                 else
                                 {
                                    ck = false;
                                    break;
                                 }
                              }
                           }
                        }
                        if (ck)
                        {
                           var curren = currens.Where(w => w.Currency_ID == ex.Currency_ID).FirstOrDefault();
                           if (curren != null)
                           {
                              Currencylist.Add(new ComboViewModel { Text = curren.Currency_Code + " : " + curren.Currency_Name, Value = curren.Currency_ID.ToString().Trim() });
                              chk_Dup.Add(curren.Currency_ID);
                           }
                        }
                     }
                  }
               }
            }
         }
         catch
         {
         }
         return Currencylist;
      }

      public List<ComboViewModel> LstTaxType(bool hasBlank = false)
      {
         var clist = new List<ComboViewModel>();

         if (hasBlank)
         {
            clist.Add(new ComboViewModel { Value = null, Text = "-" });
         }
         clist.Add(new ComboViewModel { Value = TaxType.Exclusive, Text = Resource.Exclusive });
         clist.Add(new ComboViewModel { Value = TaxType.Inclusive, Text = Resource.Inclusive });

         return clist;
      }

      public List<ComboViewModel> LstAmountType()
      {
         var clist = new List<ComboViewModel>();
         clist.Add(new ComboViewModel { Value = "%", Text = "%" });
         clist.Add(new ComboViewModel { Value = "$", Text = "$" });
         return clist;
      }

      #endregion

      #region Payroll
      public List<ComboViewModel> LstPRT(string type = "", string type2 = "")
      {
         using (var db = new SBS2DBContext())
         {
            if (string.IsNullOrEmpty(type))
            {
               return (from a in db.PRTs
                       orderby a.Name
                       select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.ID).Trim(), Text = a.Name, HidValue = a.Type }).ToList();
            }
            else
            {
               if (string.IsNullOrEmpty(type2))
               {
                  var prts = from a in db.PRTs where a.Type == type select a;
                  return (from a in prts
                          orderby a.Name
                          select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.ID).Trim(), Text = a.Name, HidValue = a.Type }).ToList();
               }
               else
               {
                  var prts = from a in db.PRTs where a.Type == type | a.Type == type2 select a;
                  return (from a in prts
                          orderby a.Name
                          select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.ID).Trim(), Text = a.Name, HidValue = a.Type }).ToList();
               }


            }



         }
      }

      public List<ComboViewModel> LstPRT(bool hasBlank = false)
      {
         using (var db = new SBS2DBContext())
         {


            List<ComboViewModel> p = (from a in db.PRTs
                                      orderby a.Name
                                      select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.ID).Trim(), Text = a.Name, HidValue = a.Type }).ToList();

            if (p == null) p = new List<ComboViewModel>();
            if (hasBlank)
            {
               p.Insert(0, new ComboViewModel() { Value = null, Text = "-" });
            }

            return p;
         }
      }

      public List<ComboViewModel> LstPRC(Nullable<int> pCompanyID, Nullable<int> PRT_ID)
      {
         using (var db = new SBS2DBContext())
         {
            if (pCompanyID.HasValue)
            {
               var prcs = (from a in db.PRCs
                           where a.Record_Status == RecordStatus.Active &
                           a.PRT_ID == PRT_ID &
                           a.Company_ID == pCompanyID
                           orderby a.Description
                           select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.PRC_ID).Trim(), Text = a.Name, Desc = a.Description, HidValue = a.CPF_Deductable.HasValue ? a.CPF_Deductable.Value.ToString() : false.ToString()}).ToList();
               prcs.Add(new ComboViewModel { Value = "0", Text = Resource.Other, Desc = Resource.Other });
               return prcs;
            }
         }
         return new List<ComboViewModel>();
      }

      public List<ComboViewModel> LstPRC(Nullable<int> pCompanyID, string type)
      {
         using (var db = new SBS2DBContext())
         {
            if (pCompanyID.HasValue)
            {
               var prcs = (from a in db.PRCs
                           where a.Record_Status == RecordStatus.Active &
                           a.PRT.Type == type &
                           a.Company_ID == pCompanyID
                           orderby a.Description
                           select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.PRC_ID).Trim(), Text = a.Name, Desc = a.Description }).ToList();

               if (type == PayrollAllowanceType.Commission | type == PayrollAllowanceType.Allowance_Deduction)
               {
                  prcs.Add(new ComboViewModel { Value = "0", Text = Resource.Other });
               }
               return prcs;
            }
         }
         return new List<ComboViewModel>();
      }

      public List<ComboViewModel> LstDonationType(Nullable<int> pCompanyID = null)
      {
         using (var db = new SBS2DBContext())
         {

            var dts = db.Donation_Type.Where(w => w.Record_Status == RecordStatus.Active);
            if (pCompanyID.HasValue)
            {
               dts = dts.Where(w => w.Company_ID == pCompanyID);
            }
            else
            {
               dts = dts.Where(w => w.Company_ID == null);
            }
            return (from a in dts select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Donation_Type_ID).Trim(), Text = a.Donation_Name }).ToList();
         }
      }



      public List<ComboViewModel> LstPeriod(bool hasday = true, bool hasweek = true, bool hasmonth = true, bool hasyear = true, bool hasBlank = false)
      {
         var clist = new List<ComboViewModel>();
         if (hasBlank)
            clist.Add(new ComboViewModel { Value = null, Text = "-" });

         if (hasday)
            clist.Add(new ComboViewModel { Value = TimePeriod.Days, Text = Resource.Days });
         if (hasweek)
            clist.Add(new ComboViewModel { Value = TimePeriod.Weeks, Text = Resource.Weeks });
         if (hasmonth)
            clist.Add(new ComboViewModel { Value = TimePeriod.Months, Text = Resource.Months });
         if (hasyear)
            clist.Add(new ComboViewModel { Value = TimePeriod.Years, Text = Resource.Years });

         return clist;
      }

      public List<ComboViewModel> LstTerm(bool hashourly = true, bool hasmonthly = true, bool hasBlank = false)
      {
         var clist = new List<ComboViewModel>();
         if (hasBlank)
            clist.Add(new ComboViewModel { Value = null, Text = "-" });
         if (hashourly)
            clist.Add(new ComboViewModel { Value = Term.Hourly, Text = Resource.Hourly });
         if (hasmonthly)
            clist.Add(new ComboViewModel { Value = Term.Monthly, Text = Resource.Monthly });

         return clist;
      }


      #endregion

      //#region Inventory
      public Country GetCountry(Nullable<int> pCompanyID)
      {
         if (pCompanyID.HasValue)
         {
            using (var db = new SBS2DBContext())
            {
               var com = (from a in db.Company_Details where a.Company_ID == pCompanyID.Value select a).FirstOrDefault();
               if (com != null)
                  return com.Country;
            }
         }
         return null;
      }

      public Currency GetCurrency(Nullable<int> pCurrencyID)
      {
         if (pCurrencyID.HasValue)
         {
            using (var db = new SBS2DBContext())
            {
               return (from a in db.Currencies where a.Currency_ID == pCurrencyID.Value select a).FirstOrDefault();
            }
         }
         return null;
      }

      //public List<ComboViewModel> LstQtyDisType(bool hasBlank = false)
      //{
      //    var clist = new List<ComboViewModel>();

      //    if (hasBlank)
      //    {
      //        clist.Add(new ComboViewModel { Value = null, Text = "-" });
      //    }

      //    clist.Add(new ComboViewModel { Value = QtyDiscountType.Lowest, Text = QtyDiscountType.Lowest });
      //    clist.Add(new ComboViewModel { Value = QtyDiscountType.Highest, Text = QtyDiscountType.Highest });

      //    return clist;
      //}

      ////public List<ComboViewModel> LstCreditCardType(bool hasBlank = false)
      ////{
      ////    var clist = new List<ComboViewModel>();

      ////    if (hasBlank)
      ////    {
      ////        clist.Add(new ComboViewModel { Value = null, Text = "-" });
      ////    }

      ////    clist.Add(new ComboViewModel { Value = CreditCardType.Visa, Text = CreditCardType.Visa });
      ////    clist.Add(new ComboViewModel { Value = CreditCardType.MasterCard, Text = CreditCardType.MasterCard });

      ////    return clist;
      ////}


      //public List<ComboViewModel> LstCoupon(bool hasBlank = false)
      //{
      //    var clist = new List<ComboViewModel>();

      //    if (hasBlank)
      //    {
      //        clist.Add(new ComboViewModel { Value = null, Text = "-" });
      //    }

      //    clist.Add(new ComboViewModel { Value = Coupon.NoCoupon, Text = Coupon.NoCoupon });
      //    clist.Add(new ComboViewModel { Value = Coupon.HaveCoupon, Text = Coupon.HaveCoupon });

      //    return clist;
      //}


      //public List<ComboViewModel> LstTaxType(bool hasBlank = false)
      //{
      //    var clist = new List<ComboViewModel>();

      //    if (hasBlank)
      //    {
      //        clist.Add(new ComboViewModel { Value = null, Text = "-" });
      //    }

      //    clist.Add(new ComboViewModel { Value = TaxType.Inclusive, Text = TaxType.Inclusive });
      //    clist.Add(new ComboViewModel { Value = TaxType.Exclusive, Text = TaxType.Exclusive });

      //    return clist;
      //}

      //public List<ComboViewModel> LstPromotionType(bool hasBlank = false)
      //{
      //    var plist = new List<ComboViewModel>();

      //    if (hasBlank)
      //    {
      //        plist.Add(new ComboViewModel { Value = null, Text = "-" });
      //    }

      //    plist.Add(new ComboViewModel { Value = PromotionType.Rebate, Text = PromotionType.Rebate });
      //    plist.Add(new ComboViewModel { Value = PromotionType.BuyX_Y, Text = PromotionType.BuyX_Y });

      //    return plist;
      //}

      //public List<ComboViewModel> LstDiscountType(bool hasBlank = false)
      //{
      //    var clist = new List<ComboViewModel>();

      //    if (hasBlank)
      //    {
      //        clist.Add(new ComboViewModel { Value = null, Text = "-" });
      //    }

      //    clist.Add(new ComboViewModel { Value = "%", Text = "%" });
      //    clist.Add(new ComboViewModel { Value = "$", Text = "$" });

      //    return clist;
      //}



      //public List<ComboViewModel> LstType()
      //{
      //    var clist = new List<ComboViewModel>();
      //    clist.Add(new ComboViewModel { Value = "Stock", Text = "Stock" });
      //    clist.Add(new ComboViewModel { Value = "Service", Text = "Service" });
      //    return clist;
      //}

      //public List<ComboViewModel> LstCategory(Nullable<int> pCompanyID, bool hasBlank = true)
      //{
      //    using (var db = new SBS2DBContext())
      //    {
      //        var clist = (from a in db.Product_Category
      //                     where a.Record_Status == RecordStatus.Active &
      //                     a.Company_ID == pCompanyID
      //                     orderby a.Category_Name
      //                     select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Product_Category_ID).Trim(), Text = a.Category_Name });
      //        var p = clist.ToList();
      //        if (hasBlank)
      //        {
      //            p.Insert(0, new ComboViewModel() { Value = null, Text = "-" });
      //        }
      //        return p;
      //    }
      //}

      //public List<ComboViewModel> LstCategoryByLv(Nullable<int> pCompanyID, int lv, bool hasBlank = true)
      //{
      //    using (var db = new SBS2DBContext())
      //    {
      //        var clist = (from a in db.Product_Category
      //                     where a.Record_Status == RecordStatus.Active &
      //                     a.Category_Level == lv &
      //                     a.Company_ID == pCompanyID
      //                     orderby a.Category_Name
      //                     select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Product_Category_ID).Trim(), Text = a.Category_Name });
      //        var p = clist.ToList();
      //        if (hasBlank)
      //        {
      //            p.Insert(0, new ComboViewModel() { Value = null, Text = "-" });
      //        }
      //        return p;
      //    }
      //}

      //public List<ComboViewModel> LstCategoryByParentID(Nullable<int> pCompanyID, int ParentID, bool hasBlank = true)
      //{
      //    using (var db = new SBS2DBContext())
      //    {
      //        var clist = (from a in db.Product_Category
      //                     where a.Record_Status == RecordStatus.Active &
      //                     a.Category_Parent_ID == ParentID &
      //                     a.Company_ID == pCompanyID
      //                     orderby a.Category_Name
      //                     select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Product_Category_ID).Trim(), Text = a.Category_Name });
      //        var p = clist.ToList();
      //        if (hasBlank)
      //        {
      //            p.Insert(0, new ComboViewModel() { Value = null, Text = "-" });
      //        }
      //        return p;
      //    }
      //}

      //public List<ComboViewModel> LstInventoryLocationByParentID(Nullable<int> pCompanyID, int ParentID, bool hasBlank = true)
      //{
      //    using (var db = new SBS2DBContext())
      //    {
      //        var clist = (from a in db.Inventory_Location
      //                     where a.Parent_Location_ID == ParentID &
      //                     a.Company_ID == pCompanyID & a.Inventory_Location_ID != ParentID
      //                     orderby a.Name
      //                     select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Inventory_Location_ID).Trim(), Text = a.Name });
      //        var p = clist.ToList();
      //        if (hasBlank)
      //        {
      //            p.Insert(0, new ComboViewModel() { Value = null, Text = "-" });
      //        }
      //        return p;
      //    }
      //}

      //public List<ComboViewModel> LstInventoryLocation(Nullable<int> pCompanyID, bool hasBlank = true)
      //{
      //    using (var db = new SBS2DBContext())
      //    {
      //        var lst = (from a in db.Inventory_Location
      //                   where a.Company_ID == pCompanyID
      //                   orderby a.Name
      //                   select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Inventory_Location_ID).Trim(), Text = a.Name }).ToList();

      //        if (hasBlank)
      //        {
      //            lst.Insert(0, new ComboViewModel() { Value = null, Text = "-" });
      //        }

      //        return lst;
      //    }
      //}

      //public List<ComboViewModel> LstProduct(Nullable<int> pCompanyID, Nullable<int> pProductID = null, bool hasEmptyRow = true)
      //{
      //    using (var db = new SBS2DBContext())
      //    {

      //        var clist = (from a in db.Products
      //                     where a.Record_Status == RecordStatus.Active
      //                     select a);

      //        if (pProductID.HasValue)
      //        {
      //            clist = (from a in clist where a.Product_ID == pProductID select a);
      //        }

      //        var p = (from a in clist
      //                 where a.Company_ID == pCompanyID
      //                 orderby a.Product_Name
      //                 select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Product_ID).Trim(), Text = a.Product_Name }).ToList();

      //        if (hasEmptyRow)
      //        {
      //            p.Insert(0, new ComboViewModel() { Value = null, Text = "-" });
      //        }
      //        return p;
      //    }
      //}

      //public List<ComboViewModel> LstUOM(Nullable<int> pProductID, bool hasBlank = true)
      //{
      //    using (var db = new SBS2DBContext())
      //    {
      //        var clist = (from a in db.Unit_Of_Measurement
      //                     where a.Product_ID == pProductID
      //                     orderby a.Unit
      //                     select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Unit_Of_Measurement_ID).Trim(), Text = a.Unit });
      //        var p = clist.ToList();
      //        if (hasBlank)
      //        {
      //            p.Insert(0, new ComboViewModel() { Value = "0", Text = "-" });
      //        }
      //        return p;
      //    }
      //}

      //public List<ComboViewModel> LstPaymentTerms(Nullable<int> pCompanyID = null, bool hasBlank = true)
      //{
      //    using (var db = new SBS2DBContext())
      //    {
      //        if (pCompanyID.HasValue)
      //        {
      //            var clist = (from a in db.Global_Lookup_Data
      //                         where a.Global_Lookup_Def.Name == ComboType.Payment_Terms &
      //                         (a.Company_ID == pCompanyID | a.Company_ID == null) &
      //                         a.Record_Status == RecordStatus.Active
      //                         orderby a.Description
      //                         select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Lookup_Data_ID).Trim(), Text = a.Description });
      //            var p = clist.ToList();
      //            if (hasBlank)
      //            {
      //                p.Insert(0, new ComboViewModel() { Value = null, Text = "-" });
      //            }
      //            return p;
      //        }
      //    }
      //    return new List<ComboViewModel>();
      //}

      //public List<ComboViewModel> LstVendor(Nullable<int> pCompanyID, bool hasBlank = true)
      //{
      //    using (var db = new SBS2DBContext())
      //    {
      //        var clist = (from a in db.Vendors
      //                     where a.Company_ID == pCompanyID
      //                     orderby a.Name
      //                     select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Vendor_ID).Trim(), Text = a.Name });
      //        var p = clist.ToList();
      //        if (hasBlank)
      //        {
      //            p.Insert(0, new ComboViewModel() { Value = null, Text = "-", Level = 0 });
      //        }
      //        return p;
      //    }
      //}

      //public List<ComboViewModel> LstBrand(Nullable<int> pCompanyID, bool hasBlank = true)
      //{
      //    using (var db = new SBS2DBContext())
      //    {
      //        var clist = (from a in db.Brands
      //                     where a.Company_ID == pCompanyID
      //                     orderby a.Brand_Description
      //                     select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Brand_ID).Trim(), Text = a.Brand_Description });
      //        var p = clist.ToList();
      //        if (hasBlank)
      //        {
      //            p.Insert(0, new ComboViewModel() { Value = null, Text = "-", Level = 0 });
      //        }
      //        return p;
      //    }
      //}

      //public List<ComboViewModel> LstTag(Nullable<int> pCompanyID, bool hasBlank = true)
      //{
      //    using (var db = new SBS2DBContext())
      //    {
      //        var clist = (from a in db.Tags
      //                     orderby a.Tag_Name
      //                     select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Tag_ID).Trim(), Text = a.Tag_Name });
      //        var p = clist.ToList();
      //        if (hasBlank)
      //        {
      //            p.Insert(0, new ComboViewModel() { Value = null, Text = "-", Level = 0 });
      //        }
      //        return p;
      //    }
      //}


      //#endregion

      //#region POS
      ////Added by NayThway on 30-May-2015 
      ////public bool TaxIsExist(Nullable<int> pCompanyID = null)
      ////{
      ////    using (var db = new SBS2DBContext())
      ////    {
      ////        var q = (from tp in db.Tax_Preferences
      ////                 where tp.Company_ID == pCompanyID
      ////                 select (bool)tp.Is_Show_Tax);

      ////        return (from a in q select a).FirstOrDefault();
      ////    }

      ////}
      //////Added by NayThway on 30-May-2015 
      ////public int TaxPercentage(Nullable<int> pCompanyID = null, string description = "")
      ////{
      ////    using (var db = new SBS2DBContext())
      ////    {
      ////        var q = !string.IsNullOrEmpty(description) ?
      ////            (from tp in db.Tax_Preferences
      ////             join ts in db.Tax_Scheme on tp.Company_ID equals ts.Company_ID
      ////             where tp.Company_ID == pCompanyID && ts.Tax_Name == description
      ////             orderby ts.Effective_Date descending
      ////             select (int)ts.Tax_Percentage).Take(1)
      ////             :
      ////             (from tp in db.Tax_Preferences
      ////              join ts in db.Tax_Scheme on tp.Company_ID equals ts.Company_ID
      ////              where tp.Company_ID == pCompanyID
      ////              orderby ts.Effective_Date descending
      ////              select (int)ts.Tax_Percentage).Take(1);

      ////        return (from a in q select a).FirstOrDefault();
      ////    }

      ////}

      //public List<ComboViewModel> LstTerminal(int company_id)
      //{
      //    using (var db = new SBS2DBContext())
      //    {
      //        return (from a in db.POS_Terminal
      //                where a.Company_ID == company_id
      //                orderby a.Terminal_Name
      //                select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Terminal_ID).Trim(), Text = a.Terminal_Name }).ToList();
      //    }
      //}
      //public List<ComboViewModel> LstCashier(int company_id)
      //{
      //    using (var db = new SBS2DBContext())
      //    {
      //        return (from a in db.User_Profile
      //                where a.Company_ID == company_id &
      //                a.User_Status == RecordStatus.Active
      //                orderby a.Name
      //                select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Profile_ID).Trim(), Text = a.Name }).ToList();
      //    }
      //}
      //public List<ComboViewModel> LstDateFormat()
      //{
      //    var q = new List<ComboViewModel>();
      //    q.Add(new ComboViewModel { Value = "yyyyMMdd", Text = "yyyyMMdd" });
      //    q.Add(new ComboViewModel { Value = "ddMMyyyy", Text = "ddMMyyyy" });
      //    q.Add(new ComboViewModel { Value = "MMddyyyy", Text = "MMddyyyy" });
      //    return q;
      //}
      //public List<ComboViewModel> LstPaperSize()
      //{
      //    var q = new List<ComboViewModel>();
      //    q.Add(new ComboViewModel { Value = "A4", Text = "A4" });
      //    q.Add(new ComboViewModel { Value = "A5", Text = "A5" });
      //    q.Add(new ComboViewModel { Value = "A6", Text = "A6" });
      //    return q;
      //}

      //public List<ComboViewModel> LstTerminalStatus(bool openonly = false)
      //{
      //    var q = new List<ComboViewModel>();
      //    q.Add(new ComboViewModel { Value = "Open", Text = "Open" });
      //    if (!openonly)
      //    {
      //        q.Add(new ComboViewModel { Value = "Close", Text = "Close" });
      //    }
      //    return q;
      //}

      //public List<ComboViewModel> LstHours()
      //{
      //    var q = new List<ComboViewModel>();
      //    for (var i = 0; i < 24; i++)
      //    {
      //        q.Add(new ComboViewModel { Value = i.ToString(), Text = i.ToString() });
      //    }
      //    return q;
      //}
      //public List<ComboViewModel> LstMinutes()
      //{
      //    var q = new List<ComboViewModel>();
      //    for (var i = 0; i <= 59; i++)
      //    {
      //        q.Add(new ComboViewModel { Value = i.ToString(), Text = i.ToString("00") });
      //    }
      //    return q;
      //}

      ////Added by sun 13-11-2015
      //public List<ComboViewModel> LstCategoryLV(bool hasBlank = false)
      //{
      //    var clist = new List<ComboViewModel>();

      //    if (hasBlank)
      //    {
      //        clist.Add(new ComboViewModel { Value = null, Text = "-" });
      //    }

      //    clist.Add(new ComboViewModel { Value = "1", Text = CategoryLV.CategoryLV1 });
      //    clist.Add(new ComboViewModel { Value = "2", Text = CategoryLV.CategoryLV2 });
      //    clist.Add(new ComboViewModel { Value = "3", Text = CategoryLV.CategoryLV3 });

      //    return clist;
      //}





      //#endregion

      //#region CRM

      //public List<ComboViewModel> LstCRM(Nullable<int> pCompanyID, string pFlag = "", bool hasBlank = true)
      //{
      //    using (var db = new SBS2DBContext())
      //    {
      //        var crms = db.CRM_Details
      //        .Include(i => i.Customer_Company)
      //        .Where(w => w.Company_ID == pCompanyID);

      //        if (!string.IsNullOrEmpty(pFlag))
      //        {
      //            crms = crms.Where(w => w.Flag == pFlag);
      //        }

      //        var clist = crms.OrderBy(o => o.Lead_Name)
      //        .Select(s => new ComboViewModel { Value = SqlFunctions.StringConvert((double)s.CRM_ID).Trim(), Text = s.Lead_Name })
      //        .ToList();

      //        if (clist == null)
      //            clist = new List<ComboViewModel>();

      //        if (hasBlank)
      //            clist.Insert(0, new ComboViewModel() { Value = null, Text = "-" });

      //        return clist;
      //    }
      //}

      //public List<ComboViewModel> LstCustomerCompany(Nullable<int> pCompanyID, bool hasBlank = true)
      //{
      //    using (var db = new SBS2DBContext())
      //    {
      //        var clist = (from a in db.Customer_Company
      //                     where a.Company_ID == pCompanyID
      //                     orderby a.Company_Name
      //                     select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Customer_Company_ID).Trim(), Text = a.Company_Name }).ToList();

      //        if (clist == null)
      //        {
      //            clist = new List<ComboViewModel>();
      //        }

      //        if (hasBlank)
      //        {
      //            clist.Insert(0, new ComboViewModel() { Value = null, Text = "-" });
      //        }

      //        return clist;
      //    }
      //}

      //public List<ComboViewModel> LstContact(Nullable<int> pCompanyID, bool hasBlank = true)
      //{
      //    using (var db = new SBS2DBContext())
      //    {
      //        var clist = db.Contacts
      //        .Include(i => i.Customer_Company)
      //        .Where(w => w.Company_ID == pCompanyID)
      //        .OrderBy(o => o.First_Name)
      //        .ThenBy(o => o.Last_Name)
      //        .Select(s => new ComboViewModel { Value = SqlFunctions.StringConvert((double)s.Contact_ID).Trim(), Text = s.First_Name })
      //        .ToList();

      //        if (clist == null)
      //            clist = new List<ComboViewModel>();

      //        if (hasBlank)
      //            clist.Insert(0, new ComboViewModel() { Value = null, Text = "-" });

      //        return clist;
      //    }
      //}

      //public List<ComboViewModel> LstParentAccount(Nullable<int> pCompanyID, Nullable<int> current_account_id = 0, bool hasBlank = true)
      //{
      //    using (var db = new SBS2DBContext())
      //    {
      //        var accounts = db.Customer_Company.Where(w => w.Company_ID == pCompanyID);

      //        if (current_account_id > 0)
      //        {
      //            accounts = accounts.Where(w => w.Customer_Company_ID != current_account_id);
      //        }

      //        var clist = accounts
      //              .OrderBy(o => o.Company_Name)
      //             .Select(s => new ComboViewModel { Value = SqlFunctions.StringConvert((double)s.Customer_Company_ID).Trim(), Text = s.Company_Name })
      //             .ToList();
      //        if (clist == null)
      //            clist = new List<ComboViewModel>();


      //        if (hasBlank)
      //            clist.Insert(0, new ComboViewModel() { Value = null, Text = "-" });

      //        return clist;
      //    }
      //}


      //public List<ComboViewModel> LstOpp(Nullable<int> pCompanyID, bool hasBlank = true)
      //{
      //    using (var db = new SBS2DBContext())
      //    {
      //        var crms = db.CRM_Details
      //        .Include(i => i.Customer_Company)
      //        .Where(w => w.Company_ID == pCompanyID && w.Flag == CRMFlag.Opportunity);


      //        var clist = crms.OrderBy(o => o.Lead_Name)
      //        .Select(s => new ComboViewModel { Value = SqlFunctions.StringConvert((double)s.CRM_ID).Trim(), Text = s.Opportunity_Name })
      //        .ToList();

      //        if (clist == null) clist = new List<ComboViewModel>();

      //        if (hasBlank) clist.Insert(0, new ComboViewModel() { Value = null, Text = "-" });

      //        return clist;
      //    }
      //}

      //#endregion

      //#region Quotation
      //public List<ComboViewModel> LstPaymentType(Nullable<int> pCompanyID = null, bool hasBlank = true)
      //{
      //    using (var db = new SBS2DBContext())
      //    {
      //        var clist = (from a in db.Global_Lookup_Data
      //                     where a.Global_Lookup_Def.Name == ComboType.Payment_Type &
      //                     (a.Company_ID == pCompanyID | a.Company_ID == null) &
      //                     a.Record_Status == RecordStatus.Active
      //                     orderby a.Description
      //                     select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Lookup_Data_ID).Trim(), Text = a.Description });

      //        var p = clist.ToList();

      //        if (hasBlank)
      //        {
      //            p.Insert(0, new ComboViewModel() { Value = null, Text = "-" });
      //        }
      //        return p;
      //    }
      //}

      //public List<ComboViewModel> LstQuotationStage(Nullable<int> pCompanyID = null, bool hasBlank = true)
      //{
      //    using (var db = new SBS2DBContext())
      //    {
      //        var clist = (from a in db.Global_Lookup_Data
      //                     where a.Global_Lookup_Def.Name == ComboType.Quotation_Stage &
      //                     (a.Company_ID == pCompanyID | a.Company_ID == null) &
      //                     a.Record_Status == RecordStatus.Active
      //                     orderby a.Description
      //                     select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Lookup_Data_ID).Trim(), Text = a.Description });

      //        var p = clist.ToList();

      //        if (hasBlank)
      //        {
      //            p.Insert(0, new ComboViewModel() { Value = null, Text = "-" });
      //        }
      //        return p;
      //    }
      //}
      //#endregion



      public List<ComboViewModel> LstDaysOfweek(bool hasBlank = true)
      {
         var clist = new List<ComboViewModel>();
         if (hasBlank)
            clist.Add(new ComboViewModel { Value = "All", Text = Resource.ALL });

         clist.Add(new ComboViewModel { Value = "Sunday", Text = Resource.Sunday });
         clist.Add(new ComboViewModel { Value = "Monday", Text = Resource.Monday });
         clist.Add(new ComboViewModel { Value = "Tuesday", Text = Resource.Tuesday });
         clist.Add(new ComboViewModel { Value = "Wednesday", Text = Resource.Wednesday });
         clist.Add(new ComboViewModel { Value = "Thursday", Text = Resource.Thursday });
         clist.Add(new ComboViewModel { Value = "Friday", Text = Resource.Friday });
         clist.Add(new ComboViewModel { Value = "Saturday", Text = Resource.Saturday });
         return clist;
      }

      public List<ComboViewModel> LstApproverFlowType()
      {
         var clist = new List<ComboViewModel>();
         clist.Add(new ComboViewModel { Value = SBSWorkFlowAPI.Constants.ApproverFlowType.Employee, Text = Resource.Employee });
         clist.Add(new ComboViewModel { Value = SBSWorkFlowAPI.Constants.ApproverFlowType.Job_Cost, Text = Resource.Indent });
         return clist;
      }
   }

   public class ComboViewModel
   {
      private string _val;

      public String Value
      {
         get
         {
            if (string.IsNullOrEmpty(_val))
               _val = "";

            return _val;
         }
         set
         {
            _val = value;
         }
      }
      public String Text { get; set; }
      public String Desc { get; set; }
      public String HidValue { get; set; }
      public Nullable<int> Level { get; set; }
   }

   public enum ComboTypeEnum
   {
      None,
      Country,
      State,
      PRC,
      Extra_Donation,
      Supervisor,
      Residential_Status,
      Proposal_Item,
      Customer_Company,
      UOM,
      Payment_Terms
   }
}


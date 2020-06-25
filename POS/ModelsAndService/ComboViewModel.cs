using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;


public enum DurationEnum
{
    None = 0,
    _15Days,
    _3Months,
    _6Months,
    _1Year,
    _2Years
}

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
    public static string Unit_Length = "Unit_Length";
    public static string Unit_Weight = "Unit_Weight";
    public static string Costing_Method = "Costing_Method";
    public static string Carrier = "Carrier";
    public static string Payment_Terms = "Payment_Terms";
    public static string Credit_Card_Type = "Credit_Card_Type";
    public static string POS_Bank_Name = "POS_Bank_Name";
    public static string Discount_Type = "Discount_Type";
}

namespace POS.Models
{
    public class ComboService
    {
        #region Authen
        public List<ComboViewModel> LstDuration(bool hasEmptyRow = true)
        {
            using (var db = new SBSDBContext())
            {
                var comboList = new List<ComboViewModel>();


                if (hasEmptyRow)
                {
                    comboList.Add(new ComboViewModel { Value = SqlFunctions.StringConvert((double)Convert.ToInt32(DurationEnum.None)).Trim(), Text = "-" });
                }
                comboList.Add(new ComboViewModel { Value = SqlFunctions.StringConvert((double)Convert.ToInt32(DurationEnum._15Days)).Trim(), Text = "15 Days" });
                comboList.Add(new ComboViewModel { Value = SqlFunctions.StringConvert((double)Convert.ToInt32(DurationEnum._3Months)).Trim(), Text = "3 Months" });
                comboList.Add(new ComboViewModel { Value = SqlFunctions.StringConvert((double)Convert.ToInt32(DurationEnum._6Months)).Trim(), Text = "6 Months" });
                comboList.Add(new ComboViewModel { Value = SqlFunctions.StringConvert((double)Convert.ToInt32(DurationEnum._1Year)).Trim(), Text = "1 Year" });
                comboList.Add(new ComboViewModel { Value = SqlFunctions.StringConvert((double)Convert.ToInt32(DurationEnum._2Years)).Trim(), Text = "2 Years" });
                return comboList;
            }
        }

        public List<ComboViewModel> LstCountry(bool hasEmptyRow = true)
        {
            using (var db = new SBSDBContext())
            {
                var q = (from a in db.Countries orderby a.Name select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Country_ID).Trim(), Text = a.Name });
                var p = q.ToList();
                if (hasEmptyRow)
                {
                    //p.Insert(0, new ComboViewModel() { Value = null, Text = "-" });
                }
                return p;
            }
        }

        public List<ComboViewModel> LstState(string pCountry_ID = "", bool hasEmptyRow = true)
        {
            using (var db = new SBSDBContext())
            {
                var q = (from a in db.States select a);
                if (!string.IsNullOrEmpty(pCountry_ID))
                {
                    int intCountryID = Convert.ToInt32(pCountry_ID);
                    q = (from a in q where a.Country_ID.Equals(intCountryID) select a);
                }

                var p = (from a in q orderby a.Name select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.State_ID).Trim(), Text = a.Name }).ToList();
                if (hasEmptyRow)
                {
                    //p.Insert(0, new ComboViewModel() { Value = null, Text = "-" });
                }
                return p;
            }
        }

        public List<ComboViewModel> LstCurrency(bool hasEmptyRow = true)
        {
            using (var db = new SBSDBContext())
            {
                var q = (from a in db.Currencies orderby a.Currency_Code select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Currency_ID).Trim(), Text = a.Currency_Code + " : " + a.Currency_Name });
                var p = q.ToList();
                if (hasEmptyRow)
                {
                    //p.Insert(0, new ComboViewModel() { Value = null, Text = "-" });
                }
                return p;
            }
        }
        #endregion

        #region HR
        public Nationality GetNationality(Nullable<int> pNationalityID)
        {
            if (pNationalityID.HasValue)
            {
                using (var db = new SBSDBContext())
                {
                    return (from a in db.Nationalities where a.Nationality_ID == pNationalityID.Value select a).FirstOrDefault();
                }
            }
            return null;
        }

        public List<ComboViewModel> LstNationality(bool hasEmptyRow = true)
        {
            using (var db = new SBSDBContext())
            {
                var q = (from a in db.Nationalities orderby a.Name select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Nationality_ID).Trim(), Text = a.Description });
                var p = q.ToList();
                if (hasEmptyRow)
                {
                    //p.Insert(0, new ComboViewModel() { Value = null, Text = "-" });
                }
                return p;
            }
        }

        public List<ComboViewModel> LstEmpStatus()
        {
            var q = new List<ComboViewModel>();
            q.Add(new ComboViewModel { Value = "C", Text = "Contract" });
            q.Add(new ComboViewModel { Value = "A", Text = "Active" });
            q.Add(new ComboViewModel { Value = "P", Text = "Probation" });
            q.Add(new ComboViewModel { Value = "R", Text = "Resigned" });
            q.Add(new ComboViewModel { Value = "T", Text = "Terminated" });
            q.Add(new ComboViewModel { Value = "D", Text = "Deceased" });
            return q;
        }

        public List<ComboViewModel> LstResidentialStatus()
        {
            var q = new List<ComboViewModel>();
            q.Add(new ComboViewModel { Value = "L", Text = "Local" });
            q.Add(new ComboViewModel { Value = "P", Text = "Permanent Resident" });
            q.Add(new ComboViewModel { Value = "F", Text = "Foreigner" });
            return q;
        }

        public List<ComboViewModel> LstRecordStatus()
        {
            var q = new List<ComboViewModel>();
            q.Add(new ComboViewModel { Value = "A", Text = "Active" });
            q.Add(new ComboViewModel { Value = "C", Text = "Inactive" });
            return q;
        }

       
        public List<ComboViewModel> LstLookup(string pName, Nullable<int> pCompanyID = null, bool hasEmptyRow = true)
        {
            using (var db = new SBSDBContext())
            {
                if (pCompanyID.HasValue)
                {
                    var q = (from a in db.Global_Lookup_Data
                             where a.Global_Lookup_Def.Name == pName &
                             (a.Company_ID == pCompanyID | a.Company_ID == null) &
                             a.Record_Status == "A"
                             orderby a.Description
                             select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Lookup_Data_ID).Trim(), Text = a.Name, Desc = a.Description });
                    var p = q.ToList();
                    if (hasEmptyRow)
                    {
                        p.Insert(0, new ComboViewModel() { Value = null, Text = "-" });
                    }
                    return p;
                }
            }
            return new List<ComboViewModel>();
        }

        //Added by NayThway on 30-May-2015 
        public bool TaxIsExist(Nullable<int> pCompanyID = null)
        {
            var db = new SBSDBContext();

            var q = (from tp in db.Tax_Preferences
                     where tp.Company_ID == pCompanyID
                     select (bool)tp.Is_Show_Tax);

            return (from a in q select a).FirstOrDefault();
        }
        //Added by NayThway on 30-May-2015 
        public int TaxPercentage(Nullable<int> pCompanyID = null)
        {
            var db = new SBSDBContext();

            var q = (from tp in db.Tax_Preferences 
                     join ts in db.Tax_Scheme on tp.Company_ID equals ts.Company_ID
                     where tp.Company_ID == pCompanyID 
                     orderby ts.Effective_Date descending
                     select (int)ts.Tax_Percentage).Take(1);

            return (from a in q select a).FirstOrDefault();
        }

        //Added By sun
        //public Global_Lookup_Data GetLookupWhereComID(Nullable<int> pCompanyID)
        //{
        //    if (pCompanyID.HasValue)
        //    {
        //        using (var db = new SBSDBContext())
        //        {
        //            return (from a in db.Global_Lookup_Data where a.Company_ID == pCompanyID.Value select a).FirstOrDefault();
        //        }
        //    }
        //    return null;
        //}



        public Global_Lookup_Data GetLookup(Nullable<int> pLookupID)
        {
            if (pLookupID.HasValue)
            {
                using (var db = new SBSDBContext())
                {
                    return (from a in db.Global_Lookup_Data where a.Lookup_Data_ID == pLookupID.Value select a).FirstOrDefault();
                }
            }
            return null;
        }

        public Global_Lookup_Data GetLookup(string description, Nullable<int> pCompanyID = null) {
            if (pCompanyID.HasValue) {
                using (var db = new SBSDBContext()) {
                    return (from a in db.Global_Lookup_Data where a.Description == description &
                            (a.Company_ID == pCompanyID | a.Company_ID == null)
                            select a).FirstOrDefault();
                }
            }
            return null;
        }

        public List<ComboViewModel> LstDepartment(bool hasEmptyRow = true)
        {
            using (var db = new SBSDBContext())
            {
                var q = (from a in db.Departments where a.Record_Status == "A" orderby a.Name select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Department_ID).Trim(), Text = a.Name });
                var p = q.ToList();
                if (hasEmptyRow)
                {
                    //p.Insert(0, new ComboViewModel() { Value = null, Text = "-" });
                }
                return p;
            }
        }

        public List<ComboViewModel> LstDesignation(bool hasEmptyRow = true)
        {
            using (var db = new SBSDBContext())
            {
                var q = (from a in db.Designations where a.Record_Status == "A" orderby a.Name select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Designation_ID).Trim(), Text = a.Name });
                var p = q.ToList();
                if (hasEmptyRow)
                {
                    //p.Insert(0, new ComboViewModel() { Value = null, Text = "-" });
                }
                return p;
            }
        }

        public List<ComboViewModel> LstSupervisor(Nullable<int> pEmployeeProfileID)
        {
            var sup = new List<ComboViewModel>();
            var currentdate = StoredProcedure.GetCurrentDate();
            using (var db = new SBSDBContext())
            {

                if (pEmployeeProfileID.HasValue)
                {
                    // Get Self Department
                    var userhist = (from a in db.Employment_History
                                    where a.Effective_Date <= currentdate & a.Employee_Profile_ID == pEmployeeProfileID
                                    orderby a.Effective_Date descending
                                    select a).FirstOrDefault();

                    var histlist = new List<Employment_History>();
                    var p = (from a in db.Employment_History
                             where a.Effective_Date <= currentdate &
                             a.Department_ID == userhist.Department_ID &
                             a.Employee_Profile_ID != pEmployeeProfileID.Value
                             orderby a.History_ID descending
                             select a);

                    sup.Add(new ComboViewModel() { Value = null, Text = "-" });
                    var q = (from a in p group a by new { a.Employee_Profile_ID } into g select g);
                    if (q != null)
                    {
                        try
                        {
                            foreach (var row in q)
                            {
                                var r = (from a in row orderby a.Effective_Date descending select a).FirstOrDefault();

                                histlist.Add(r);

                                sup.Add(new ComboViewModel { Value = r.Employee_Profile_ID.ToString(), Text = r.Employee_Profile.User_Profile.Name + " : " + r.Employee_Profile.Employee_No });
                            }
                        }
                        catch { }

                    }
                }
            }
            return sup;
        }
        

        public List<ComboViewModel> LstMonth()
        {
            var q = new List<ComboViewModel>();
            q.Add(new ComboViewModel { Value = "1", Text = "January" });
            q.Add(new ComboViewModel { Value = "2", Text = "February" });
            q.Add(new ComboViewModel { Value = "3", Text = "March " });
            q.Add(new ComboViewModel { Value = "4", Text = "April" });
            q.Add(new ComboViewModel { Value = "5", Text = "May" });
            q.Add(new ComboViewModel { Value = "6", Text = "June" });
            q.Add(new ComboViewModel { Value = "7", Text = "July" });
            q.Add(new ComboViewModel { Value = "8", Text = "August" });
            q.Add(new ComboViewModel { Value = "9", Text = "September" });
            q.Add(new ComboViewModel { Value = "10", Text = "October" });
            q.Add(new ComboViewModel { Value = "11", Text = "November" });
            q.Add(new ComboViewModel { Value = "12", Text = "December" });
            return q;
        }


        #endregion

        #region Inventory


        public List<ComboViewModel> LstCategory(int company_id, bool hasEmptyRow = true)
        {
            using (var db = new SBSDBContext())
            {
                var q = (from a in db.Product_Category
                         where a.Record_Status == "A" &
                         a.Company_ID == company_id
                         orderby a.Category_Name
                         select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Product_Category_ID).Trim(), Text = a.Category_Name });
                var p = q.ToList();
                if (hasEmptyRow)
                {
                    p.Insert(0, new ComboViewModel() { Value = null, Text = "All" });
                }
                return p;
            }
        }

        public List<ComboViewModel> LstInventoryLocation(int company_id)
        {
            using (var db = new SBSDBContext())
            {
                return (from a in db.Inventory_Location
                        where a.Company_ID == company_id
                        orderby a.Name
                        select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Inventory_Location_ID).Trim(), Text = a.Name }).ToList();
            }
        }



        public List<ComboViewModel> LstProduct(int company_id, Nullable<int> product_id = null, bool hasEmptyRow = true)
        {
            using (var db = new SBSDBContext())
            {

                var q = (from a in db.Products
                         where a.Record_Status == RecordStatus.Active
                         select a);

                if (product_id.HasValue)
                {
                    q = (from a in q where a.Product_ID == product_id select a);
                }

                var p = (from a in q
                         where a.Company_ID == company_id
                         orderby a.Product_Name
                         select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Product_ID).Trim(), Text = a.Product_Name }).ToList();

                if (hasEmptyRow)
                {
                    p.Insert(0, new ComboViewModel() { Value = null, Text = "-" });
                }
                return p;
            }
        }

        public List<ComboViewModel> LstCustomerCompany(int company_id, bool hasEmptyRow = true)
        {
            using (var db = new SBSDBContext())
            {
                var q = (from a in db.Customer_Company
                         where a.Company_ID == company_id
                         orderby a.Company_Name
                         select new ComboViewModel { Value = SqlFunctions.StringConvert((double)a.Customer_Company_ID).Trim(), Text = a.Company_Name }).ToList();

                if (q == null)
                {
                    q = new List<ComboViewModel>();
                }

                if (hasEmptyRow)
                {
                    q.Insert(0, new ComboViewModel() { Value = null, Text = "-" });
                }

                return q;
            }
        }


        #endregion

        
    }



    public class ComboViewModel
    {
        public String Value { get; set; }
        public String Desc { get; set; }
        public String Text { get; set; }
    }
}

public enum ComboTypeEnum
{
    None,
    Country,
    State,
    Proposal_Item,
    Customer_Company
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Authentication.Common;
using Authentication.Models;
using SBSModel.Models;
using SBSModel.Common;
using SBSResourceAPI;

namespace Authentication.Models
{
   public class CompanyViewModel : ModelBase
   {
      public List<ComboViewModel> countryList;
      public List<ComboViewModel> statusList;
      public List<Company_Details> CompanyList;
      public string Company_Levelg { get; set; }
      public Nullable<int> search_Country { get; set; }
      public string search_Registration_Date { get; set; }
      public string search_Status { get; set; }
      public Nullable<int> Belong_To { get; set; }
   }

   public class CompanyInfoViewModel : ModelBase
   {
      //********** tab company **********
      public string Company_Levelg { get; set; }
      public List<ComboViewModel> LstCompanylevel { get; set; }
      public List<ComboViewModel> countryList;
      public List<ComboViewModel> stateList;
      public List<ComboViewModel> stateBillingList;
      public List<ComboViewModel> statusList { get; set; }
      public List<Subscription> SubscriptionList { get; set; }
      public Nullable<int> Company_ID { get; set; }
      public Nullable<int> Company_Detail_ID { get; set; }
      public byte[] Logo { get; set; }
      public Nullable<int> User_Count { get; set; }
      public Nullable<int> Belong_To { get; set; }

      //Added by Nay on 04-Aug-2015
      public string CPF_Submission_No { get; set; }

      //Added by Nay on 07-Sept-2015 
      public string patUser_ID { get; set; }
      public string patPassword { get; set; }

      //------------------------SBSResourceAPI-------------------------//

      //[LocalizedDisplayName("No_Of_Employee", typeof(Resource))]
      //public Nullable<int> No_Of_Employee { get; set; }

      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Company_Name", typeof(Resource))]
      public string Company_Name { get; set; }

      [LocalizedDisplayName("Date_Of_Registration", typeof(Resource))]
      public string Effective_Date { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(500)]
      [LocalizedDisplayName("Address", typeof(Resource))]
      public string Address { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Country", typeof(Resource))]
      public Nullable<int> Country_ID { get; set; }

      [LocalizedDisplayName("State_Or_Province", typeof(Resource))]
      public Nullable<int> State_ID { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(10)]
      [DataType(DataType.PostalCode)]
      [LocalizedDisplayName("Zip_Or_Postal_Code", typeof(Resource))]
      public String Zip_Code { get; set; }

      [LocalizedValidMaxLength(500)]
      [LocalizedDisplayName("Billing", typeof(Resource))]
      public string Billing_Address { get; set; }

      [LocalizedDisplayName("Country", typeof(Resource))]
      public Nullable<int> Billing_Country_ID { get; set; }

      [LocalizedDisplayName("State_Or_Province", typeof(Resource))]
      public Nullable<int> Billing_State_ID { get; set; }

      [LocalizedValidMaxLength(10)]
      [DataType(DataType.PostalCode)]
      [LocalizedDisplayName("Zip_Or_Postal_Code", typeof(Resource))]
      public String Billing_Zip_Code { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Company_level", typeof(Resource))]
      public string Company_Level { get; set; }

      [LocalizedValidMaxLength(100)]
      [LocalizedRequired]
      [LocalizedDisplayName("Office_Phone", typeof(Resource))]
      public string Phone { get; set; }

      [LocalizedValidMaxLength(100)]
      [LocalizedDisplayName("Fax", typeof(Resource))]
      public string Fax { get; set; }

      [LocalizedDisplayName("No_Of_Employees", typeof(Resource))]
      public Nullable<int> No_Of_Employees { get; set; }

      public byte[] Company_Logo { get; set; }
      public Nullable<System.Guid> Company_Logo_ID { get; set; }
   }

   public class CompanyRegisterViewModel : ModelBase
   {
      public string step { get; set; }
      //********** tab company **********
      public Nullable<int> Currency_ID { get; set; }
      public string Currency { get; set; }
      public string Company_Levelg { get; set; }
      public List<ComboViewModel> LstCompanylevelNoMe { get; set; }
      public List<ComboViewModel> countryList;
      public List<ComboViewModel> stateList;
      public List<ComboViewModel> stateBillingList;
      public List<ComboViewModel> statusList { get; set; }
      public List<SBS_Module_Detail> moduleList;
      public Subscription[] Subscriptions { get; set; }
      public Nullable<int> Company_ID { get; set; }
      public Nullable<int> Belong_To { get; set; }
      public int[] Select_Module { get; set; }
      public string CPF_Submission_No { get; set; }

      //------------------------SBSResourceAPI-------------------------//

      [LocalizedDisplayName("No_Of_Employee", typeof(Resource))]
      public Nullable<int> No_Of_Employee { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Company_Name", typeof(Resource))]
      public string Company_Name { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(500)]
      [LocalizedDisplayName("Address", typeof(Resource))]
      public string Address { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Country", typeof(Resource))]
      public Nullable<int> Country_ID { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Company_level", typeof(Resource))]
      public string Company_Level { get; set; }


      [LocalizedDisplayName("State", typeof(Resource))]
      public Nullable<int> State_ID { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(10)]
      [DataType(DataType.PostalCode)]
      [LocalizedDisplayName("Postal_Code", typeof(Resource))]
      public String Zip_Code { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(500)]
      [LocalizedDisplayName("Billing", typeof(Resource))]
      public string Billing_Address { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Country", typeof(Resource))]
      public Nullable<int> Billing_Country_ID { get; set; }

      [LocalizedDisplayName("State", typeof(Resource))]
      public Nullable<int> Billing_State_ID { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(10)]
      [DataType(DataType.PostalCode)]
      [LocalizedDisplayName("Postal_Code", typeof(Resource))]
      public String Billing_Zip_Code { get; set; }

      //Not found in db field
      [LocalizedRequired]
      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("First_Name", typeof(Resource))]
      public string First_Name { get; set; }

      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Middle_Name", typeof(Resource))]
      public string Middle_Name { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Last_Name", typeof(Resource))]
      public string Last_Name { get; set; }

      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Office_Phone", typeof(Resource))]
      public string Phone { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(50)]
      [LocalizedValidationEmail]
      [DataType(DataType.EmailAddress)]
      [LocalizedDisplayName("Email", typeof(Resource))]
      public string Email { get; set; }

      public string A7_Group_ID { get; set; }

      [LocalizedValidMaxLength(100)]
      [LocalizedDisplayName("Fax", typeof(Resource))]
      public string Fax { get; set; }
   }

   public class AssignUserViewModel : ModelBase
   {
      public string pageAction { get; set; }
      public List<ComboViewModel> moduleList;
      public List<User_Profile> ProfileList;
      public List<User_Assign_Module> AssignList;
      public Nullable<int> Company_ID { get; set; }
      public Nullable<int> Subscription_ID { get; set; }
      public Nullable<int> Module_Detail_ID { get; set; }
      public Nullable<int> Total_License { get; set; }
      public int[] Assigned { get; set; }
      public int[] UnAssigned { get; set; }
   }

   public class SignUpViewModel : ModelBase
   {

      public List<ComboViewModel> countryList;
      public List<ComboViewModel> stateList;
      public List<SBS_Module_Detail> ModuleList;
      public List<Subscription> SubscriptionList;      
      public Nullable<int> Company_ID { get; set; }
      public Nullable<int> Company_Detail_ID { get; set; }
      public int[] Select_Module { get; set; }
      public string Select_Module_Str { get; set; }
      public SignUpDetailViewModel[] Details { get; set; }
      public Subscription[] Subscription { get; set; }

      //------------------------SBSResourceAPI-------------------------//

      [LocalizedRequired]
      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Company_Name", typeof(Resource))]
      public string Company_Name { get; set; }

      //Not found in db field
      [LocalizedRequired]
      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("First_Name", typeof(Resource))]
      public string First_Name { get; set; }

      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Middle_Name", typeof(Resource))]
      public string Middle_Name { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Last_Name", typeof(Resource))]
      public string Last_Name { get; set; }

      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Phone", typeof(Resource))]
      public string Phone { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(50)]
      [LocalizedValidationEmail]
      [DataType(DataType.EmailAddress)]
      [LocalizedDisplayName("Email", typeof(Resource))]
      public string Email { get; set; }

      [LocalizedValidMaxLength(500)]
      [LocalizedDisplayName("Address", typeof(Resource))]
      public string Address { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Country", typeof(Resource))]
      public Nullable<int> Country_ID { get; set; }

      [LocalizedDisplayName("State", typeof(Resource))]
      public Nullable<int> State_ID { get; set; }

      [LocalizedValidMaxLength(10)]
      [DataType(DataType.PostalCode)]
      [LocalizedDisplayName("Postal_Code", typeof(Resource))]
      public String Zip_Code { get; set; }

      /*A7*/
      public String A7_Group_ID { get; set; }
      public String A7_User_ID { get; set; }
      public int step { get; set; }

   }

   public partial class Paypal
   {
      public string Item_Name { get; set; }
      public Nullable<decimal> Amount { get; set; }
      public Nullable<decimal> Qty { get; set; }
   }

   public class ModulePriceModel : ModelBase
   {
       public int Price_ID { get; set; }
       public int Qty_From { get; set; }
       public int Qty_To { get; set; }
       public decimal Price { get; set; }
   }

   public class SignUpDetailViewModel : ModelBase
   {
      public int Module_Detail_ID { get; set; }
      public int Module_ID { get; set; }
      public string Module_Detail_Name { get; set; }
      public string Module_Detail_Description { get; set; }

      [LocalizedRequired]
      public Nullable<int> No_Of_Users { get; set; }
      public List<ModulePriceModel> Prices { get; set; }
      public Nullable<decimal> Price { get; set; }
      public Nullable<decimal> Price_Per_Person { get; set; }
      public Nullable<int> Period_Day { get; set; }
      public Nullable<int> Period_Month { get; set; }
      public Nullable<int> Period_Year { get; set; }
   }
}


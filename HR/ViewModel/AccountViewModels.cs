using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

using System.Web.UI.WebControls;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Data.Entity;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;

using HR.Common;
using HR.Models;
using SBSModel.Models;
using SBSModel.Common;
using SBSResourceAPI; 



namespace HR.Models
{

    public class LoginViewModel
    {
        [LocalizedRequired]
        [LocalizedDisplayName("Email", typeof(Resource))]
        [LocalizedValidMaxLength(50)]
        public string Email { get; set; }

        [LocalizedRequired]
        [DataType(DataType.Password)]
        [LocalizedDisplayName("Password", typeof(Resource))]
        [LocalizedValidMaxLength(24)]
        public string Password { get; set; }

        public string ApplicationUser_Id { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayName("Remember_Me", typeof(Resource))]
        public bool RememberMe { get; set; }

        public String message { get; set; }
    }

    public class ResetPasswordViewModel
    {
        public int uid { get; set; }
        public bool notValidateCurrent { get; set; }
        public String name { get; set; }


        [DataType(DataType.Password)]
        [LocalizedDisplayName("Current_Password", typeof(Resource))]
        public string OldPassword { get; set; }

        [LocalizedRequired]
        [StringLength(24, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [LocalizedDisplayName("New_Password", typeof(Resource))]
        public string NewPassword { get; set; }

        [LocalizedRequired]
        [DataType(DataType.Password)]
        [LocalizedDisplayName("Confirm_New_Password", typeof(Resource))]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ErrorPageViewModel
    {
        public String URL { get; set; }
        public int code { get; set; }
        public string feild { get; set; }
        public String msg { get; set; }
    }

    public class MessagePageViewModel
    {
       public string pageAction { get; set; }
        public String Field { get; set; }
        public int Code { get; set; }
        public String Msg { get; set; }
    }

    public class DashBoardViewModel
    {
        public int ErrorCode { get; set; }
        public string redirectURL { get; set; }
        public ServiceResult result { get; set; }
        public Employee_Profile Emp { get; set; }
        public Employment_History Emp_Hist { get; set; }
        public List<Leave_Application_Document> LeaveList { get; set; }

        
        
        public List<Leave_Type> LeaveBalanceList { get; set; }
        public List<Expenses_Application> ExpensesList { get; set; }
        public List<Expenses_Config> expensesConfigList { get; set; }
        public List<ExpensesBalanceViewModel> ExpensesBalanceList { get; set; }
        public string Currency_Code { get; set; }
        public int Currency_ID { get; set; }
       
        public PRM payroll { get; set; }

        public Holidays[] HolidayList { get; set; }
        public string[] Collor { get; set; }

        public bool Display_Leave { get; set; }
        public bool Display_Expenses { get; set; }
        public bool Display_Payroll { get; set; }
        public int Total_Employees { get; set; }
        public decimal Total_Diskspace { get; set; }
        public decimal Avail_Diskspace { get; set; }
        public decimal Diskspace_Usage { get; set; }
        public decimal Company_Usage { get; set; }
        public decimal Emp_Usage { get; set; }
        public decimal Leave_Usage { get; set; }
        public decimal Exp_Usage { get; set; }

        public bool Is_Trial { get; set; }
        public decimal Total_Billing_Amt { get; set; }
        public decimal Total_Outstanding_Amt { get; set; }
        public decimal Total_Exp_Amt { get; set; }
        public decimal Total_Leave_Days { get; set; }
        public decimal Total_Payroll_Amt { get; set; }
        public decimal Total_Expenses_Amt { get; set; }

        public List<ComboViewModel> LeaveTypeComboList { get; set; }
        public Nullable<int> Leave_Config_ID { get; set; }
        public List<int> Working_Days { get; set; }
        public Nullable<int> Relationship_ID { get; set; }
        public string Relationship_Name { get; set; }
        
        public string Start_Date { get; set; }
        public string End_Date { get; set; }
        public Nullable<System.Guid> Upload_Document_ID { get; set; }

        [LocalizedValidMaxLength(300)]
        [LocalizedDisplayName("Expenses_Title", typeof(Resource))]
        public string Expenses_Title { get; set; }

        public User_Profile userProfile { get; set; }
        public string Expenses_Date { get; set; }
        public Nullable<int> Expenses_Config_ID { get; set; }
        public Nullable<decimal> Amount_Claiming { get; set; }
        public Nullable<decimal> Balance { get; set; }
        public string Upload_Receipt { get; set; }
        public string Upload_Receipt_Name { get; set; }
        public List<ComboViewModel> periodList { get; set; }
        public string Start_Date_Period { get; set; }
        public string End_Date_Period { get; set; }
        public Nullable<int> UOM_ID { get; set; }
        public string UOM_Name { get; set; }
        public Nullable<decimal> Mileage { get; set; }
        public Nullable<decimal> Amount_Per_UOM { get; set; }
        public List<string> adminRights { get; set; }
        public List<string> hrRights { get; set; }

        public List<Leave_Application_Document> LeaveTeamCalendarList { get; set; }
    }

    public class LandingViewModel : ModelBase
    {
        
        public Nullable<int> Company_ID { get; set; }
        public ServiceResult result { get; set; }
       

        //********** tab company **********
        public string Company_Levelg { get; set; }
        public List<ComboViewModel> LstCompanylevel { get; set; }
        public List<ComboViewModel> countryList;
        public List<ComboViewModel> stateList;
        public List<ComboViewModel> stateBillingList;
        public List<ComboViewModel> statusList { get; set; }
        public List<Subscription> SubscriptionList { get; set; }
                
        public Nullable<int> Company_Detail_ID { get; set; }
        public byte[] Logo { get; set; }
        public Nullable<int> User_Count { get; set; }
        public Nullable<int> Belong_To { get; set; }

        //Added by Nay on 04-Aug-2015
        [LocalizedRequired]
        public string Business_Type { get; set; }

        //Added by Nay on 07-Sept-2015 
        public string patUser_ID { get; set; }
        public string patPassword { get; set; }

        //------------------------SBSResourceAPI-------------------------//

        //[LocalizedDisplayName("No_Of_Employee", typeof(Resource))]
        //public Nullable<int> No_Of_Employee { get; set; }

        [LocalizedRequired]
        [LocalizedValidMaxLength(150)]
        [LocalizedDisplayName("Company_Name", typeof(Resource))]
        public string Company_Name { get; set; }

        //[LocalizedDisplayName("Date_Of_Registration", typeof(Resource))]
        //public string Effective_Date { get; set; }

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

        //[LocalizedRequired]
        //[LocalizedDisplayName("Company_level", typeof(Resource))]
        //public string Company_Level { get; set; }

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

        public List<ComboViewModel> genderList { get; set; }
        public List<ComboViewModel> maritalStatusList { get; set; }
        public List<ComboViewModel> raceList { get; set; }
        public List<ComboViewModel> nationalityList { get; set; }
        public List<ComboViewModel> residentialStatusList { get; set; }
        public List<ComboViewModel> empTypeList { get; set; }
        public List<ComboViewModel> currencyList { get; set; }
        public List<ComboViewModel> departmentList { get; set; }
        public List<ComboViewModel> desingnationList { get; set; }
        public List<ComboViewModel> branchList { get; set; }
        public List<ComboViewModel> religionList { get; set; }
        public List<ComboViewModel> businessCatList { get; set; }
        [LocalizedRequired]
        [LocalizedDisplayName("Gender", typeof(Resource))]
        public Nullable<int> Gender { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayName("Marital_Status", typeof(Resource))]
        public Nullable<int> Marital_Status { get; set; }

        [LocalizedRequired]
        [LocalizedValidDate]
        [DataType(DataType.Date)]
        [LocalizedDisplayName("DOB", typeof(Resource))]
        public String DOB { get; set; }

        [LocalizedDisplayName("Nationality", typeof(Resource))]
        public Nullable<int> Nationality_ID { get; set; }

        [LocalizedDisplayName("Residential_Status", typeof(Resource))]
        public string Residential_Status { get; set; }

        [LocalizedDisplayName("Work_Pass_Type", typeof(Resource))]
        public Nullable<int> Work_Pass_Type { get; set; }

        [LocalizedRequired]
        [LocalizedValidMaxLength(30)]
        [LocalizedDisplayName("NRIC", typeof(Resource))]
        public string NRIC { get; set; }

        [LocalizedRequired] 
        [LocalizedDisplayName("Department", typeof(Resource))]
        public Nullable<int> Department_ID { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayName("Designation", typeof(Resource))]
        public Nullable<int> Designation_ID { get; set; }

        [LocalizedRequired]            
        [LocalizedDisplayName("Branch", typeof(Resource))]
        public Nullable<int> Branch_ID { get; set; }

        [LocalizedValidDate]
        [LocalizedRequired]
        [DataType(DataType.Date)]
        [LocalizedDisplayName("Hire_Or_Effective_Date", typeof(Resource))]
        public string Effective_Date { get; set; }
        [LocalizedValidDate]
        [LocalizedRequired]
        [DataType(DataType.Date)]
        [LocalizedDisplayName("Confirm_Date", typeof(Resource))]
        public string Confirm_Date { get; set; }
        [LocalizedRequired]
        [LocalizedDisplayName("Employment_Type", typeof(Resource))]
        public string Employee_Type { get; set; }
        [LocalizedDisplayName("Race", typeof(Resource))]
        public Nullable<int> Race { get; set; }
        [LocalizedRequired]
        [LocalizedDisplayName("Currency", typeof(Resource))]
        public Nullable<int> Currency_ID { get; set; }
        [LocalizedRequired]
        [LocalizedDisplayName("Basic_Salary", typeof(Resource))]
        public Nullable<decimal> Basic_Salary { get; set; }

        [LocalizedRequired]
        public string Basic_Salary_Unit { get; set; }

        public List<ComboViewModel> periodList { get; set; }
        public List<ComboViewModel> termList { get; set; }
        public string Company_Currency_Code { get; set; }
        [LocalizedDisplayName("Religion", typeof(Resource))]
        public Nullable<int> Religion { get; set; }
    }
}

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

    public class LoginViewModel
    {
        public string ApplicationUser_Id { get; set; }
        public String message { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayName("Email", typeof(Resource))]
        [LocalizedValidMaxLength(50)]
        public string Email { get; set; }

        [LocalizedRequired]
        [DataType(DataType.Password)]
        [LocalizedDisplayName("Password", typeof(Resource))]
        [LocalizedValidMaxLength(24)]
        public string Password { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayName("Remember_Me", typeof(Resource))]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        public List<ComboViewModel> Countries { get; set; }
        public int result { get; set; }

        [LocalizedRequired]
        [LocalizedValidMaxLength(150)]
        [LocalizedDisplayName("Full_Name", typeof(Resource))]
        public string FullName { get; set; }

        [LocalizedRequired]
        [LocalizedValidMaxLength(50)]
        [LocalizedValidationEmail]
        [CustomValidation(typeof(UserService), "isNotDuplicatedUser")]
        [LocalizedDisplayName("Email", typeof(Resource))]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [LocalizedRequired]
        [LocalizedValidMaxLength(20)]
        [Phone]
        [LocalizedDisplayName("Contact_No", typeof(Resource))]
        public String ContactNo { get; set; }

        [LocalizedRequired]
        [LocalizedValidMaxLength(150)]
        [LocalizedDisplayName("Company_Name", typeof(Resource))]
        public string CompanyName { get; set; }

        [LocalizedRequired]
        [LocalizedValidMaxLength(500)]
        [LocalizedDisplayName("Address", typeof(Resource))]
        public string CompanyAddress { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayName("Country", typeof(Resource))]
        public int Country_ID { get; set; }
    }

    public class UserInfoViewModel
    {
        public List<User_Profile> profiles { get; set; }
        public int result { get; set; }
        public int operation { get; set; }
        public List<int> page_rights { get; set; }
        public List<int> register_rights { get; set; }
        public List<int> userDetail_rights { get; set; }
    }

    public class UserDetailViewModel
    {
        public UserDetailViewModel()
        {
            this.page_rights = new List<int>();
            this.user_Assign_Role_rights = new List<int>();
            this.SendResetPassword_rights = new List<int>();
            this.uploadProfilePhoto_rights = new List<int>();
            this.SendNewActivation_rights = new List<int>();
        }

        public int uid { get; set; }
        public int aid { get; set; }
        public string Name { get; set; }

        public byte[] User_Photo { get; set; }
        public List<User_Role> User_Role { get; set; }
        public int[] roles { get; set; }
        public int result { get; set; }
        public int rresult { get; set; }
        public int acresult { get; set; }
        public int aresult { get; set; }
        public int operation { get; set; }
        public List<int> page_rights { get; set; }
        public List<int> user_Assign_Role_rights { get; set; }
        public List<int> SendResetPassword_rights { get; set; }
        public List<int> SendNewActivation_rights { get; set; }
        public List<int> uploadProfilePhoto_rights { get; set; }
        public bool status_right { get; set; }
        public bool isActivated { get; set; }

        [LocalizedRequired]
        [LocalizedValidMaxLength(150)]
        [LocalizedDisplayName("Full_Name", typeof(Resource))]
        public string FullName { get; set; }

        [LocalizedRequired]
        [LocalizedValidMaxLength(50)]
        [LocalizedValidationEmail]
        [LocalizedDisplayName("Email", typeof(Resource))]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayName("Status", typeof(Resource))]
        public string Status { get; set; }

    }

    public class UserRoleViewModel
    {
        public List<User_Role> User_Roles_List { get; set; }
        public int User_Role_ID { get; set; }
        public List<PageRoleViewModel> vPage_Role { get; set; }
        public int result { get; set; }
        public int operation { get; set; }
        public List<int> page_rights { get; set; }

        [LocalizedRequired]
        [LocalizedValidMaxLength(100)]
        [LocalizedDisplayName("Role_Name", typeof(Resource))]
        public String Role_Name { get; set; }

        [LocalizedRequired]
        [LocalizedValidMaxLength(500)]
        [LocalizedDisplayName("Role_Desc", typeof(Resource))]
        public String Role_Description { get; set; }
    }


    public class AccessRightViewModel
    {
        public List<Access_Right> Access_Right_List { get; set; }
        public int Access_ID { get; set; }
        public int result { get; set; }
        public int operation { get; set; }
        public List<int> page_rights { get; set; }

        [LocalizedRequired]
        [LocalizedValidMaxLength(20)]
        [LocalizedDisplayName("Access_Name", typeof(Resource))]
        public String Access_Name { get; set; }

        [LocalizedRequired]
        [LocalizedValidMaxLength(500)]
        [LocalizedDisplayName("Access_Desc", typeof(Resource))]
        public String Access_Description { get; set; }
    }

    public class ErrorPageViewModel
    {
        public int code { get; set; }
        public string msg { get; set; }
        public string URL { get; set; }
    }

    public class MessagePageViewModel
    {
        public String Field { get; set; }
        public int Code { get; set; }
        public String Msg { get; set; }
        public String pageAction { get; set; }
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
        public PRM payroll { get; set; }

        public Holidays[] HolidayList { get; set; }
        public string[] Collor { get; set; }

        public bool Display_Leave { get; set; }
        public bool Display_Expenses { get; set; }
        public bool Display_Payroll { get; set; }
        public int Total_Employees { get; set; }
        public decimal Diskspace_Usage { get; set; }
        public decimal Total_Billing_Amt { get; set; }
        public decimal Total_Exp_Amt { get; set; }
        public decimal Total_Leave_Days { get; set; }
        public decimal Total_Payroll_Amt { get; set; }

        public List<ComboViewModel> LeaveTypeComboList { get; set; }
        public Nullable<int> Leave_Config_ID { get; set; }
        public List<int> Working_Days { get; set; }

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
    }

    public class LandingViewModel : ModelBase
    {
        
        /********** branch ***********/
        public Nullable<int> Company_ID { get; set; }
        public ServiceResult result { get; set; }
        public Nullable<int> Branch_ID { get; set; }

        [LocalizedValidMaxLength(10)]
        [LocalizedDisplayName("Branch_Code", typeof(Resource))]
        public String Branch_Code { get; set; }

        [LocalizedValidMaxLength(300)]
        [LocalizedDisplayName("Branch_Name", typeof(Resource))]
        public String Branch_Name { get; set; }

        [LocalizedValidMaxLength(500)]
        [LocalizedDisplayName("Branch_Desc", typeof(Resource))]
        public String Branch_Desc { get; set; }

        [LocalizedDisplayName("Status", typeof(Resource))]
        public String Branch_Status { get; set; }

        /********** department ***********/
        public Nullable<int> Department_ID { get; set; }

        [LocalizedValidMaxLength(150)]
        [LocalizedDisplayName("Department_Name", typeof(Resource))]
        public String Department_Name { get; set; }

        [LocalizedDisplayName("Status", typeof(Resource))]
        public String Department_Status { get; set; }

        /********** pattern ***********/
        public List<Branch> BranchList { get; set; }
        public Nullable<int> Employee_No_Pattern_ID { get; set; }
        public bool Select_Nationality { get; set; }
        public Nullable<bool> Select_Year { get; set; }
        public bool Year_2_Digit { get; set; }
        public bool Year_4_Digit { get; set; }
        public bool Select_Company_code { get; set; }

        [LocalizedValidMaxLength(10)]
        public String Company_Code { get; set; }
        public Nullable<bool> Select_Branch_Code { get; set; }
        public Nullable<int> Pattern_Branch_ID { get; set; }
        public Nullable<bool> Initiated { get; set; }
        public String Pattern { get; set; }

        [LocalizedDisplayName("Select_Default_Emp_No_Next_Start", typeof(Resource))]
        public bool Is_Default_Emp_No_Next_Start { get; set; }
        public String Default_Emp_No_Next_Start { get; set; }

        [LocalizedDisplayName("Select_Emp_No_Next_Start", typeof(Resource))]
        public bool Is_Emp_No_Next_Start { get; set; }
        public Nullable<int> Current_Running_Number { get; set; }  
    }

    public class LeaveBalanceViewModel
    {
        public Nullable<int> Leave_Type { get; set; }
        public string Leave_Type_Name { get; set; }
        public Nullable<decimal> Days_Taken { get; set; }
        public Nullable<decimal> AllDays { get; set; }
        public Nullable<decimal> Leave_Left { get; set; }
        public string Type { get; set; }
        public Nullable<decimal> AllWeeks { get; set; }
        public Nullable<decimal> Weeks_Left { get; set; }
    }

    public class ExpensesBalanceViewModel
    {
        public string Expenses_Type_Name { get; set; }
        public Nullable<decimal> Amount { get; set; }
    }

}

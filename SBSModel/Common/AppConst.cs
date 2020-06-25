using SBSModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SBSResourceAPI;

namespace SBSModel.Common
{
    public class MenuPosition
    {
        public static string Left = "Left";
        public static string TopRight = "TopRight";
    }

    public class Operation
    {
        public static string A = "A"; /*Access*/
        public static string C = "C"; /*Create*/
        public static string U = "U"; /*Update*/
        public static string D = "D"; /*Delete*/
        public static string R = "R"; /*Reset*/
    }

    public class RowType
    {
        public static string ADD = "A";
        public static string EDIT = "U";
        public static string DELETE = "D";
    }

    public enum DurationEnum
    {
        None = 0,
        _15Days,
        _3Months,
        _6Months,
        _1Year,
        _2Years
    }

    public class Duration
    {
        public static string _15Days = "1";
        public static string _3Months = "2";
        public static string _6Months = "3";
        public static string _1Year = "4";
        public static string _2Years = "5";
    }

    public class Companylevel
    {
        public static string Mainmaster = "Main master";
        public static string Franchise = "Franchise";
        public static string Whitelabel = "White label";
        public static string EndUser = "End User";
    }

    public class ERROR_CODE
    {
        public static int SUCCESS = 1;
        public static int SUCCESS_CREATE = 2;
        public static int SUCCESS_EDIT = 3;
        public static int SUCCESS_DELETE = 4;
        public static int SUCCESS_APPROVE = 5;
        public static int SUCCESS_REJECT = 6;
        public static int SUCCESS_CANCEL = 7;
        public static int SUCCESS_CONFIRM = 8;
        public static int SUCCESS_SEND_EMAIL = 9;
        public static int SUCCESS_GENERATE = 10;
        public static int SUCCESS_Void = 11;
        public static int SUCCESS_Hold_Bill = 12;
        public static int SUCCESS_CONNECT = 13;
        public static int SUCCESS_RESET = 14;
        public static int SUCCESS_CANCEL_REJECT = 15;
        public static int SUCCESS_Refund = 16;

        public static int ERROR_1_USER_NOT_FOUND = -1;
        public static int ERROR_2_ACTIVATE_CODE_EXPIRE = -2;
        public static int ERROR_3_ACTIVATE_CODE_NOT_FOUND = -3;
        public static int ERROR_4_RESET_PASSWORD_EXPIRE = -4;
        public static int ERROR_5_RESET_PASSWORD_CODE_NOT_FOUND = -5;
        public static int ERROR_6_NO_ACCESS_RIGHT = -6;
        public static int ERROR_9_EMAIL_DUPPLICATE = -9;
        public static int ERROR_12_COMPANY_Inactive = -12;
        public static int ERROR_13_NO_DEFAULT_CURRENCY = -13;
        public static int ERROR_14_NO_EMPLOYEE_HIST = -14;
        public static int ERROR_16_NO_EMPLOYEE_PROFILE = -16;
        public static int ERROR_17_NO_PREFIX = -17;
        public static int ERROR_18_NO_PAYROLL_RECORDS = -18;
        public static int ERROR_19_CASHIER_INVALID = -19;
        public static int ERROR_20_NO_RECEIPT_CONFIG = -20;
        public static int ERROR_21_NO_IR8A_RECORDS = -21;
        public static int ERROR_22_NO_PAT_CONFIG = -22;
        public static int ERROR_23_PAYPAL = -23;

        public static int ERROR_401_UNAUTHORIZED = -401;
        public static int ERROR_404_PAGE_NOT_FOUND = -404;
        public static int ERROR_500_DB = -500;
        public static int ERROR_501_CANT_SEND_EMAIL = -501;
        public static int ERROR_502_IDENTITY_NOT_FOUND = -502;
        public static int ERROR_503_INSERT_ERROR = -503;
        public static int ERROR_504_UPDATE_ERROR = -504;
        public static int ERROR_505_DELETE_ERROR = -505;
        public static int ERROR_506_SAVE_ERROR = -506;
        public static int ERROR_507_CONFIRM_ERROR = -507;
        public static int ERROR_RESET = -508;

        public static int ERROR_510_DATA_DUPLICATE = -510;
        public static int ERROR_511_DATA_NOT_FOUND = -511;
        public static int ERROR_513_CAL_LEAVE_ERROR = -513;
        public static int ERROR_514_INVALID_INPUT_ERROR = -514;
        public static int ERROR_515_RELATED_DATA = -515;

        public static int ERROR_601_PRINTER_NOT_FOUND = -601;
        public static int ERROR_602_PRINTER_ERROR = -602;
        public static int ERROR_DEVICE_CANNOT_CONNECT = -603;
        public static int ERROR_DEVICE_SETUP_NOT_FOUND = -604;

        public static int ERROR_701_WORKFLOW_ERROR = -701;
        public static int ERROR_702_OVERDUE_ERROR = -702;

        public static int ERROR_703_OVERLEAVE_ERROR = -703;
        public static int ERROR_704_FALLSONHOLIDAY_ERROR = -704;
    }

    public class ServiceResult
    {
        private string _msg;
        private int _code;
        private int _msgcode;

        public string Msg
        {
            get
            {
                if (string.IsNullOrEmpty(_msg))
                {
                    if (_msgcode > 0)
                        _msg = new Success().getSuccess(Msg_Code);
                    else
                        _msg = new Error().getError(Msg_Code);
                }
                return _msg;
            }
            set
            {
                _msg = value;
            }
        }

        public int Code
        {
            get
            {
                return _code;
            }
            set
            {
                _msgcode = value;
                _code = value;
            }
        }

        public int Msg_Code
        {
            get
            {
                return _msgcode;
            }
            set
            {
                _msgcode = value;
            }
        }
        public string Field { get; set; }
        public string tbActive { get; set; }
        public object Object { get; set; }

    }

    //public class ServiceSynResult
    //{
    //    public string Msg { get; set; }
    //    public int Code { get; set; }
    //    public string Field { get; set; }

    //    public List<_POS_Receipt_Result> RcpResult { get; set; }
    //    public List<_POS_Terminal_Result> TerminalResult { get; set; }
    //    public List<_POS_Shift_Result> ShiftResult { get; set; }

    //}

    public class ServiceObjectResult
    {
        public object Object { get; set; }
        public int Record_Count { get; set; }
        public int Start_Index { get; set; }
        public int Page_Size { get; set; }
    }

    public class Success
    {
        private Dictionary<int, String> s { get; set; }
        public Success()
        {
            s = new Dictionary<int, string>();

            s.Add(ERROR_CODE.SUCCESS, Resource.SUCCESS);
            s.Add(ERROR_CODE.SUCCESS_CREATE, Resource.SUCCESS_CREATE);
            s.Add(ERROR_CODE.SUCCESS_EDIT, Resource.SUCCESS_EDIT);
            s.Add(ERROR_CODE.SUCCESS_DELETE, Resource.SUCCESS_DELETE);
            s.Add(ERROR_CODE.SUCCESS_APPROVE, Resource.SUCCESS_APPROVE);
            s.Add(ERROR_CODE.SUCCESS_REJECT, Resource.SUCCESS_REJECT);
            s.Add(ERROR_CODE.SUCCESS_CANCEL, Resource.SUCCESS_CANCEL);
            s.Add(ERROR_CODE.SUCCESS_SEND_EMAIL, Resource.SUCCESS_SEND_EMAIL);
            s.Add(ERROR_CODE.SUCCESS_GENERATE, Resource.SUCCESS_GENERATE);
            s.Add(ERROR_CODE.SUCCESS_Hold_Bill, Resource.SUCCESS_Hold_Bill);
            s.Add(ERROR_CODE.SUCCESS_Void, Resource.SUCCESS_Void);
            s.Add(ERROR_CODE.SUCCESS_CONFIRM, Resource.SUCCESS_CONFIRM);
            s.Add(ERROR_CODE.SUCCESS_CONNECT, Resource.SUCCESS_CONNECT);
            s.Add(ERROR_CODE.SUCCESS_RESET, Resource.SUCCESS_RESET);
            s.Add(ERROR_CODE.SUCCESS_CANCEL_REJECT, Resource.SUCCESS_CANCEL_REJECT);
            s.Add(ERROR_CODE.SUCCESS_Refund, Resource.SUCCESS_Refund);
        }

        public String getSuccess(int no)
        {
            if (s.ContainsKey(no))
                return s.Where(w => w.Key == no).FirstOrDefault().Value;
            else
                return "";
        }
    }

    public class Error
    {
        private Dictionary<int, String> e { get; set; }
        public Error()
        {
            e = new Dictionary<int, string>();
            /*Expire*/
            e.Add(ERROR_CODE.ERROR_2_ACTIVATE_CODE_EXPIRE, Resource.ERROR_2_ACTIVATE_CODE_EXPIRE);
            e.Add(ERROR_CODE.ERROR_4_RESET_PASSWORD_EXPIRE, Resource.ERROR_4_RESET_PASSWORD_EXPIRE);

            /*Duplicate*/
            e.Add(ERROR_CODE.ERROR_9_EMAIL_DUPPLICATE, Resource.ERROR_9_EMAIL_DUPPLICATE);
            e.Add(ERROR_CODE.ERROR_510_DATA_DUPLICATE, Resource.ERROR_510_DATA_DUPLICATE);

            /*Data not found*/
            e.Add(ERROR_CODE.ERROR_5_RESET_PASSWORD_CODE_NOT_FOUND, Resource.ERROR_5_RESET_PASSWORD_CODE_NOT_FOUND);
            e.Add(ERROR_CODE.ERROR_3_ACTIVATE_CODE_NOT_FOUND, Resource.ERROR_3_ACTIVATE_CODE_NOT_FOUND);
            e.Add(ERROR_CODE.ERROR_1_USER_NOT_FOUND, Resource.ERROR_1_USER_NOT_FOUND);
            e.Add(ERROR_CODE.ERROR_13_NO_DEFAULT_CURRENCY, Resource.ERROR_13_NO_DEFAULT_CURRENCY);
            e.Add(ERROR_CODE.ERROR_14_NO_EMPLOYEE_HIST, Resource.ERROR_14_NO_EMPLOYEE_HIST);
            e.Add(ERROR_CODE.ERROR_16_NO_EMPLOYEE_PROFILE, Resource.ERROR_16_NO_EMPLOYEE_PROFILE);
            e.Add(ERROR_CODE.ERROR_17_NO_PREFIX, Resource.ERROR_17_NO_PREFIX);
            e.Add(ERROR_CODE.ERROR_19_CASHIER_INVALID, Resource.ERROR_19_CASHIER_INVALID);
            e.Add(ERROR_CODE.ERROR_20_NO_RECEIPT_CONFIG, Resource.ERROR_20_NO_RECEIPT_CONFIG);
            e.Add(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.ERROR_511_DATA_NOT_FOUND);

            /*Email*/
            e.Add(ERROR_CODE.ERROR_501_CANT_SEND_EMAIL, Resource.ERROR_501_CANT_SEND_EMAIL);

            /*Security*/
            e.Add(ERROR_CODE.ERROR_6_NO_ACCESS_RIGHT, Resource.ERROR_6_NO_ACCESS_RIGHT);
            e.Add(ERROR_CODE.ERROR_401_UNAUTHORIZED, Resource.ERROR_401_UNAUTHORIZED);

            /*DB transaction*/
            e.Add(ERROR_CODE.ERROR_500_DB, Resource.ERROR_500_DB);
            e.Add(ERROR_CODE.ERROR_503_INSERT_ERROR, Resource.ERROR_503_INSERT_ERROR);
            e.Add(ERROR_CODE.ERROR_504_UPDATE_ERROR, Resource.ERROR_504_UPDATE_ERROR);
            e.Add(ERROR_CODE.ERROR_505_DELETE_ERROR, Resource.ERROR_505_DELETE_ERROR);
            e.Add(ERROR_CODE.ERROR_506_SAVE_ERROR, Resource.ERROR_506_SAVE_ERROR);
            e.Add(ERROR_CODE.ERROR_RESET, Resource.ERROR_RESET);

            /*Input*/
            e.Add(ERROR_CODE.ERROR_514_INVALID_INPUT_ERROR, Resource.ERROR_514_INVALID_INPUT_ERROR);

            /*Device*/
            e.Add(ERROR_CODE.ERROR_DEVICE_CANNOT_CONNECT, Resource.ERROR_DEVICE_CANNOT_CONNECT);
            e.Add(ERROR_CODE.ERROR_DEVICE_SETUP_NOT_FOUND, Resource.ERROR_DEVICE_SETUP_NOT_FOUND);

            /*Plugin*/
            e.Add(ERROR_CODE.ERROR_701_WORKFLOW_ERROR, Resource.ERROR_701_WORKFLOW_ERROR);
            e.Add(ERROR_CODE.ERROR_18_NO_PAYROLL_RECORDS, Resource.ERROR_18_NO_PAYROLL_RECORDS);
            e.Add(ERROR_CODE.ERROR_21_NO_IR8A_RECORDS, Resource.ERROR_21_NO_IR8A_RECORDS);
            e.Add(ERROR_CODE.ERROR_22_NO_PAT_CONFIG, Resource.ERROR_22_NO_PAT_CONFIG);
            e.Add(ERROR_CODE.ERROR_23_PAYPAL, Resource.ERROR_23_PAYPAL);
            e.Add(ERROR_CODE.ERROR_513_CAL_LEAVE_ERROR, Resource.ERROR_513_CAL_LEAVE_ERROR);

            /*Bill overdue*/
            e.Add(ERROR_CODE.ERROR_702_OVERDUE_ERROR, Resource.ERROR_702_OVERDUE_ERROR);
            e.Add(ERROR_CODE.ERROR_703_OVERLEAVE_ERROR, Resource.Over_leave);
            e.Add(ERROR_CODE.ERROR_704_FALLSONHOLIDAY_ERROR, Resource.Message_The_Selected_Date_Period_Falls_On_A_Holiday);
        }

        public String getError(int no)
        {
            if (e.ContainsKey(no))
                return e.Where(w => w.Key == no).FirstOrDefault().Value;
            else
                return "";
        }
    }

    public class Role
    {
        public static int ROLE_MAIN_MASTER_ADMIN = 1;
        public static int ROLE_FRANCHISE_ADMIN = 2;
        public static int ROLE_WHITE_LABEL_ADMIN = 3;
        public static int ROLE_CUSTOMER_ADMIN = 4;
        public static int ROLE_CUSTOMER_USER = 5;
    }

    public class ModuleCode
    {
        public static string Authentication = "Authentication";
        public static string HR = "HR";
        public static string POS = "POS";
        public static string CRM = "CRM";
        public static string Employee = "Employee";
        public static string Leave = "Leave";
        public static string Expense = "Expense";
        public static string Payroll = "Payroll";
        public static string Time = "Time";
        public static string Quotation = "Quotation";
        public static string Inventory = "Inventory";
    }

    public class ControllerCode
    {
        public static string POS = "POS";
        public static string CRM = "CRM";
        public static string Employee = "Employee";
        public static string Leave = "Leave";
        public static string Expense = "Expense";
        public static string Expenses = "Expenses";
        public static string Payroll = "Payroll";
        public static string Time = "Time";
        public static string TimeSheet = "TimeSheet";
        public static string Invoice = "Invoice";
        public static string Quotation = "Quotation";
        public static string Inventory = "Inventory";
        public static string Purchase = "Purchase";
        public static string Receive = "Receive";
        public static string Approval = "Approval";
        public static string Vendor = "Vendor";
        public static string Front = "Front";
        public static string User = "User";
        public static string Company = "Company";
        public static string Subscription = "Subscription";
        public static string GlobalLookup = "GlobalLookup";
        public static string Configuration = "Configuration";
        public static string Customer = "Customer";
        public static string JobCost = "JobCost";
        public static string ProductBill = "ProductBill";
        public static string Billing = "Billing";
    }

    public class ActionCode
    {
        public static string Configuration = "Configuration";
        public static string SystemConfiguration = "SystemConfiguration";
    }

    public class ModuleDomain
    {

        public static string Authentication = "AuthenSBS2";
        public static string HR = "HRSBS2";
        public static string Inventory = "InvenSBS2";
        public static string POS = "POS3";
        public static string CRM = "CRM";
        public static string Time = "TimeSBS2";
        public static string Billing = "BillSBS2";

        public static string GetModuleDomain(string actionName, string controllerName)
        {
            if (controllerName == ControllerCode.Inventory)
                return ModuleDomain.Inventory;
            else if (controllerName == ControllerCode.Purchase)
                return ModuleDomain.Inventory;
            else if (controllerName == ControllerCode.Receive)
                return ModuleDomain.Inventory;
            else if (controllerName == ControllerCode.Quotation)
                return ModuleDomain.Inventory;
            else if (controllerName == ControllerCode.Invoice)
                return ModuleDomain.Inventory;
            else if (controllerName == ControllerCode.Vendor)
                return ModuleDomain.Inventory;
            else if (controllerName == ControllerCode.POS)
                return ModuleDomain.POS;
            else if (controllerName == ControllerCode.ProductBill)
                return ModuleDomain.Billing;
            else if (controllerName == ControllerCode.Billing)
                return ModuleDomain.Billing;

            return "";
        }
    }

    public class Period
    {
        public static string AM = "AM";
        public static string PM = "PM";
    }

    public class TimePeriod
    {
        public static string Hours = "Hours";
        public static string Days = "Days";
        public static string Weeks = "Weeks";
        public static string Months = "Months";
        public static string Years = "Years";
    }

    public class Term
    {
        public static string Daily = "Daily";
        public static string Hourly = "Hourly";
        public static string Weekly = "Weekly";
        public static string Monthly = "Monthly";
        public static string Yearly = "Yearly";
    }

    public class WorkingDays
    {
        public static double Days5 = 5;
        public static double Days5_5 = 5.5;
        public static double Days6 = 6;
        public static double Days7 = 7;
    }

    public class PayrollStatus
    {
        public static string Pending = "Pending";
        public static string Process = "Process"; // when process payroll complete
        public static string Comfirm = "Comfirm"; // when confirm payroll complete
    }

    public class PayrollAllowanceType
    {
        public static string Allowance_Deduction = "Allowance_Deduction";
        public static string Overtime = "Overtime";
        public static string Commission = "Commission";
        public static string Donation = "Donation";

        public static string Allowance = "Allowance";
        public static string Adjustment_Addition = "Adjustment (Addition)";

        public static string Deduction = "Deduction";
        public static string Adjustment_Deduction = "Adjustment (Deduction)";
        public static string Deduction_Donation = "Deduction (Donation)";

        //Added by sun 09-02-2016
        public static string Allowance_Add_On_To_Basic_Salary = "Allowance add-on to basic salary";

        //Added by sun 03-01-2018
        public static string Bonus = "Bonus";
    }

    public class PayrollFlag
    {
        public static string Yes = "Y";
        public static string No = "N";
        public static string Partial = "P";
    }

    public class AppConst
    {
        public static string Other = "Other";

        public static string GetUserName(SBSModel.Models.User_Profile user)
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

        public static string GetMsg(ServiceResult result)
        {
            if (result == null)
                return "";

            var msg = "";
            if (result.Code > 0)
            {
                /*success*/
                if (!string.IsNullOrEmpty(result.Field))
                    msg = "<i class='fa fa-check-circle'></i> " + result.Field + " " + result.Msg;
                else
                    msg = "<i class='fa fa-check-circle'></i>" + Resource.Message_Your_Information + " " + result.Msg;
            }
            else
            {
                if (result.Code == ERROR_CODE.ERROR_501_CANT_SEND_EMAIL)
                {
                    /*success without send mail*/
                    if (!string.IsNullOrEmpty(result.Field))
                        msg = "<i class='fa fa-check-circle'></i>" + Resource.Message_Your_Information_Has_Been_Submit_Successfully;
                    else
                        msg = result.Field + "<i class='fa fa-check-circle'></i>" + Resource.Message_Has_Been_Submit_Successfully;

                    msg += "</br>" + Resource.Message_But_Can_T_Send_Email;
                }
                else
                {
                    /*error*/
                    msg += "<i class='fa fa-times-circle'></i> " + result.Msg;
                }

            }
            return msg.Replace(Environment.NewLine, "</br>");
        }
    }

    public class FormulaVariable
    {
        public static string Employee_Contribution = "[Employee Contribution]";
        public static string Total_CPF_Contribution = "[Total CPF Contribution]";
        public static string Basic_Salary = "[Basic Salary]";
        public static string Deduction = "[Deduction]";
        public static string Deduction_Ad_Hoc = "[Deduction (Ad Hoc)]";
        public static string Leave_Amount = "[Leave Amount]";
        public static string Allowance = "[Allowance]";
        public static string Adjustment_Allowance = "[Adjustment (Allowance)]";
        public static string Adjustment_Deductions = "[Adjustment (Deductions)]";
        public static string Commission = "[Commission]";
        public static string Overtime = "[Overtime]";
        public static string Employee_Residential_Status = "[Employee.Residential Status]";
        public static string Employee_Age = "[Employee.Age]";
        public static string PR_Years = "[PR.Years]";
        public static string Employee_Total_Wages = "[Employee.Total Wages]";
        public static string Current_Date = "[Current Date]";
        public static string Bonus = "[Bonus]";
        public static string Local = "[Local]";
        public static string PR = "[PR]";
        public static string Internship = "[Internship]";
        public static string Employee_Status = "[Employee.Status]";
        public static string Donation = "[Donation]";
    }

    public class EmpImportColumn
    {
        public static int Employee_No = 1;
        public static int First_Name = 2;
        public static int Middle_Name = 3;
        public static int Last_Name = 4;
        public static int Mobile_Phone = 5;
        public static int Email = 6;
        public static int Gender = 7;
        public static int Marital_Status = 8;
        public static int DOB = 9;
        public static int Religion = 10;
        public static int Race = 11;
        public static int Nationality = 12;
        public static int Residential_Status = 13;
        public static int NRIC = 14;
        public static int Passport = 15;
        public static int Date_Of_Issue = 16;
        public static int Date_Of_Expire = 17;
        public static int Address1 = 18;
        public static int Address1_Postal_Code = 19;
        public static int Address1_Country = 20;
        public static int Address2 = 21;
        public static int Address2_Postal_Code = 22;
        public static int Address2_Country = 23;
        public static int Bank_Name = 24;
        public static int Bank_Account = 25;
        public static int Payment_Type = 26;
        public static int Effective_Date = 27;
    }

    public class HisImportColumn
    {
        public static int Employee_No = 1;
        public static int Employment_Type = 2;
        public static int Department = 3;
        public static int Branch = 4;
        public static int Designation = 5;
        public static int Effective_Date = 6;
        public static int Confirm_Date = 7;
        public static int Currency = 8;
        public static int Basic_Salary = 9;
        public static int Monthly_Hourly = 10;
    }

    public class EmergencyImportColumn
    {
        public static int Employee_No = 1;
        public static int Name = 2;
        public static int Contact_No = 3;
        public static int Relationship = 4;
    }

    public class RelationImportColumn
    {
        public static int Employee_No = 1;
        public static int Name = 2;
        public static int Relationship = 3;
        public static int DOB = 4;
        public static int Nationality = 5;
        public static int NRIC = 6;
    }

    public class LeaveDocImportColumn
    {
        public static int Employee_No = 1;
        public static int Leave_Config_Name = 2;
        public static int Start_Date = 3;
        public static int End_Date = 4;
        public static int Days_Taken = 5;
        public static int Remark = 6;
    }

    public class ExpensesDocImportColumn
    {
        public static int Employee_No = 1;
        public static int Expenses_Title = 2;
        public static int Date_Applied = 3;
        public static int Expenses_Config_Type = 4;
        public static int Expenses_Date = 5;
        public static int Total_Amount = 6;
        public static int Selected_Currency = 7;
        public static int Tax = 8;
        public static int Amount_Claiming = 9;
        public static int Remarks = 10;
    }

    public class PayrollImportColumn
    {
        public static int Employee_No = 1;
        public static int Process_Year = 2;
        public static int Process_Month = 3;
        public static int Run_Date = 4;
        public static int Total_Work_Days = 5;
        public static int Payment_Type = 6;
        public static int OT_Rate = 7;
        public static int OT_Hours = 8;
        public static int Amount_Payable = 9;
        public static int Allowance_Type = 10;
        public static int Description = 11;
        public static int Description_Amount = 12;
        public static int Donation_Type = 13;
        public static int Donation_Amount = 14;
    }

    public class UserRole
    {
        public static int ROLE_MAIN_MASTER_ADMIN = 1;
        public static int ROLE_FRANCHISE_ADMIN = 2;
        public static int ROLE_WHITE_LABEL_ADMIN = 3;
        public static int ROLE_CUSTOMER_ADMIN = 4;
        public static int ROLE_CUSTOMER_USER = 5;

    }

    public class RecordStatus
    {
        public static string Active = "Active";
        public static string Inactive = "Inactive";
        public static string Delete = "Delete";
        public static string Default = "Default";
    }

    public class BusinessType
    {
        public static string FoodAndBeverage = "FB";
        public static string Retail = "RT";
    }

    public class LeaveStatus
    {
        public static string Pending = "P";
        public static string Rejected = "R";
        public static string Approved = "A";
        public static string Cancel = "C"; // Request Cancel
        public static string ApprovedCancel = "O"; // Approved Cancel
        public static string RejectedCancel = "OR";
    }

    public class ApprovalStatus
    {
        public static string Pending = "P";
        public static string Rejected = "R";
        public static string Approved = "A";
        public static string Cancel = "C";
        public static string ApproveCancel = "O";
    }

    public class ChildType
    {
        public static string OwnChild = "Own Child";
        public static string AdoptedChild = "Adopted  Child";
    }

    public class ResidentialStatus
    {
        public static string PermanentResident = "P";
        public static string Local = "L";
        public static string Foreigner = "F";
    }

    public class LeaveType
    {
        public static string AdoptionLeave = "Adoption Leave";
        public static string MaternityLeave = "Maternity Leave";
        public static string PaternityLeave = "Paternity Leave";
        public static string ChildCareLeave = "Child Care Leave";
    }

    public class LeaveConfigType
    {
        public static string Normal = "Normal";
        public static string Child = "Child";
    }

    public class ClaimableType
    {
        public static string Per_Employee = "Per Employeee";
        public static string Per_Department = "Per Department";
    }

    public class ExchangePeriod
    {
        public static string ByDate = "By Date";
        public static string ByMonth = "By Month";
        public static string ByWeek = "By Week";
    }

    public class Page_Name
    {
        public static string Company_Information = "Company_Information";
        public static string Company = "Company";
        public static string Company_Lookup_Configurarion = "Company_Lookup_Configurarion";
        public static string General_Configuration = "General_Configuration";
        public static string Employee = "Employee";
        public static string Employee_HR = "Employee_HR";
        public static string New_Leave = "New_Leave";
        public static string New_Expenses = "New_Expenses";
        public static string Product = "Product";
        public static string Payroll = "Payroll";
        public static string IR8A = "IR8A";
        public static string My_Quotation = "My_Quotation";
        public static string My_Invoice = "My_Invoice";
        public static string Quotation_Management = "Quotation_Management";
        public static string POS_Configuration = "POS_Configuration";
        public static string POS_Configuration_Admin = "POS_Configuration_Admin";
        public static string New_Pos = "New_Pos";
        public static string POS_Report = "POS_Report";
        public static string Payroll_Configuration_Master = "Payroll_Configuration_Master";
        public static string Payroll_Configuration = "Payroll_Configuration";
        public static string POS_Void_Refund = "POS_Void_Refund";
        public static string POS_Shift = "POS_Shift";
        public static string Leave_Configuration = "Leave_Configuration";
        public static string Paroll_Configuration = "Paroll_Configuration";
        public static string Expenses_Configuration = "Expenses_Configuration";
        public static string Employee_Admin = "Employee_Admin";
        public static string Payslip = "Payslip";
        public static string User = "User";
        public static string System_Configuration = "System_Configuration";
        public static string Leave_Configuration_Master = "Leave_Configuration_Master";
        public static string PAT_Submit_CPF_File = "PAT_Submit_CPF_File";
        public static string PAT_CPF_Configuration = "PAT_CPF_Configuration";
        public static string Promotion_Information = "Promotion_Information";
        public static string Member_Configuration = "Member_Configuration";
        public static string PAT_Submit_IR8A_File = "PAT_Submit_IR8A_File";
        public static string PAT_IR8A_Configuration = "PAT_IR8A_Configuration";
        public static string Leave_Management = "Leave_Management";
        public static string Expenses_Management = "Expenses_Management";
        public static string Leave_Report = "Leave_Report";
        public static string Expense_Report = "Expense_Report";
        public static string Employee_Report = "Employee_Report";
        public static string Payroll_Report = "Payroll_Report";
        public static string Subscription_Report = "Subscription_Report";
        public static string Payroll_Import = "Payroll_Import";
        public static string Expenses_Import = "Expenses_Import";
        public static string Leave_Import = "Leave_Import";
        public static string Employee_Import = "Employee_Import";
        public static string Vendor = "Vendor";
        public static string Purchase_Configuration = "Purchase_Configuration";
        public static string Purchase_Management = "Purchase_Management";
        public static string Purchase_Order = "Purchase_Order";
        public static string Receive = "Receive";
        public static string Purchase_Order_Report = "Purchase_Order_Report";
        public static string Inventory_Configuration = "Inventory_Configuration";
        public static string Quotation_Configuration = "Quotation_Configuration";
        public static string Withdraw = "Withdraw";
        public static string Member = "Member";
        public static string DashBoard = "DashBoard";
        public static string My_Leave = "My_Leave";
        public static string Team_Calendar = "Team_Calendar";
        public static string My_Expenses = "My_Expenses";
        public static string My_Templates = "My_Templates";
        public static string Time_Configuration = "Time_Configuration";
        public static string Stock_List = "Stock_List";
        public static string Employee_Arrangement = "Employee_Arrangement";
        public static string Transaction_Report = "Transaction_Report";
        public static string Summary_Report = "Summary_Report";
        public static string Detail_Transaction_Report = "Detail_Transaction_Report";
        public static string Job_Cost = "Job_Cost";
        public static string Customer = "Customer";
        public static string Time_Sheet = "Time_Sheet";
        public static string Time_Sheet_Configuration = "Time_Sheet_Configuration";
        public static string New_Time_Sheet = "New_Time_Sheet";
        public static string My_Time_Sheet = "My_Time_Sheet";
        public static string Time_Sheet_Management = "Time_Sheet_Management";
        public static string Time_Sheet_Transaction_Report = "Time_Sheet_Transaction_Report";
        public static string Master_Lookup_Configurarion = "Master_Lookup_Configurarion";
        public static string Billing_Report = "Billing_Report";
        public static string All_Time_Sheet = "All_Time_Sheet";
        public static string All_Expenses = "All_Expenses";


        public static string Get_Page_Name(String name)
        {
            try
            {
                if (name == Page_Name.Company_Information)
                    return Resource.Company_Information;
                else if (name == Page_Name.Company)
                    return Resource.Company;
                else if (name == Page_Name.Company_Lookup_Configurarion)
                    return Resource.Company_Lookup_Configurarion;
                else if (name == Page_Name.General_Configuration)
                    return Resource.General_Configuration;
                else if (name == Page_Name.Employee)
                    return Resource.Employee;
                else if (name == Page_Name.Employee_HR)
                    return Resource.Employee_HR;
                else if (name == Page_Name.New_Leave)
                    return Resource.New_Leave;
                else if (name == Page_Name.New_Expenses)
                    return Resource.New_Expenses;
                else if (name == Page_Name.Product)
                    return Resource.Product;
                else if (name == Page_Name.Payroll)
                    return Resource.Payroll;
                else if (name == Page_Name.IR8A)
                    return Resource.IR8A;
                else if (name == Page_Name.My_Quotation)
                    return Resource.My_Quotation;
                else if (name == Page_Name.My_Invoice)
                    return Resource.My_Invoice;
                else if (name == Page_Name.Quotation_Management)
                    return Resource.Quotation_Management;
                else if (name == Page_Name.POS_Configuration)
                    return Resource.POS_Configuration;
                else if (name == Page_Name.POS_Configuration_Admin)
                    return Resource.POS_Configuration_Admin;
                else if (name == Page_Name.New_Pos)
                    return Resource.New_Pos;
                else if (name == Page_Name.POS_Report)
                    return Resource.POS_Report;
                else if (name == Page_Name.Payroll_Configuration_Master)
                    return Resource.Payroll_Configuration_Master;
                else if (name == Page_Name.Payroll_Configuration)
                    return Resource.Payroll_Configuration;
                else if (name == Page_Name.POS_Void_Refund)
                    return Resource.POS_Void_Refund;
                else if (name == Page_Name.POS_Shift)
                    return Resource.POS_Shift;
                else if (name == Page_Name.Leave_Configuration)
                    return Resource.Leave_Configuration;
                else if (name == Page_Name.Paroll_Configuration)
                    return Resource.Paroll_Configuration;
                else if (name == Page_Name.Expenses_Configuration)
                    return Resource.Expenses_Configuration;
                else if (name == Page_Name.Employee_Admin)
                    return Resource.Employee_Admin;
                else if (name == Page_Name.Payslip)
                    return Resource.Payslip;
                else if (name == Page_Name.User)
                    return Resource.User;
                else if (name == Page_Name.System_Configuration)
                    return Resource.System_Configuration;
                else if (name == Page_Name.Leave_Configuration_Master)
                    return Resource.Leave_Configuration_Master;
                else if (name == Page_Name.PAT_Submit_CPF_File)
                    return Resource.PAT_Submit_CPF_File;
                else if (name == Page_Name.PAT_CPF_Configuration)
                    return Resource.PAT_CPF_Configuration;
                else if (name == Page_Name.Promotion_Information)
                    return Resource.Promotion_Information;
                else if (name == Page_Name.Member_Configuration)
                    return Resource.Member_Configuration;
                else if (name == Page_Name.PAT_Submit_IR8A_File)
                    return Resource.PAT_Submit_IR8A_File;
                else if (name == Page_Name.PAT_IR8A_Configuration)
                    return Resource.PAT_IR8A_Configuration;
                else if (name == Page_Name.Leave_Management)
                    return Resource.Leave_Management;
                else if (name == Page_Name.Expenses_Management)
                    return Resource.Expenses_Management;
                else if (name == Page_Name.Leave_Report)
                    return Resource.Leave_Report;
                else if (name == Page_Name.Expense_Report)
                    return Resource.Expense_Report;
                else if (name == Page_Name.Employee_Report)
                    return Resource.Employee_Report;
                else if (name == Page_Name.Payroll_Report)
                    return Resource.Payroll_Report;
                else if (name == Page_Name.Subscription_Report)
                    return Resource.Subscription_Report;
                else if (name == Page_Name.Payroll_Import)
                    return Resource.Payroll_Import;
                else if (name == Page_Name.Expenses_Import)
                    return Resource.Expenses_Import;
                else if (name == Page_Name.Leave_Import)
                    return Resource.Leave_Import;
                else if (name == Page_Name.Employee_Import)
                    return Resource.Employee_Import;
                else if (name == Page_Name.Vendor)
                    return Resource.Vendor;
                else if (name == Page_Name.Receive)
                    return Resource.Receive;
                else if (name == Page_Name.Purchase_Configuration)
                    return Resource.Purchase_Configuration;
                else if (name == Page_Name.Purchase_Management)
                    return Resource.Purchase_Management;
                else if (name == Page_Name.Purchase_Order)
                    return Resource.Purchase_Order;
                else if (name == Page_Name.Purchase_Order_Report)
                    return Resource.Purchase_Order_Report;
                else if (name == Page_Name.Inventory_Configuration)
                    return Resource.Inventory_Configuration;
                else if (name == Page_Name.Quotation_Configuration)
                    return Resource.Quotation_Configuration;
                else if (name == Page_Name.Withdraw)
                    return Resource.Withdraw;
                else if (name == Page_Name.Member)
                    return Resource.Member;
                else if (name == Page_Name.DashBoard)
                    return Resource.DashBoard;
                else if (name == Page_Name.My_Leave)
                    return Resource.My_Leave;
                else if (name == Page_Name.Team_Calendar)
                    return Resource.Team_Calendar;
                else if (name == Page_Name.My_Expenses)
                    return Resource.My_Expenses;
                else if (name == Page_Name.Time_Configuration)
                    return Resource.Time_Configuration;
                else if (name == Page_Name.Stock_List)
                    return Resource.Stock_List;
                else if (name == Page_Name.Employee_Arrangement)
                    return Resource.Employee_Arrangement;
                else if (name == Page_Name.Transaction_Report)
                    return Resource.Transaction_Report;
                else if (name == Page_Name.My_Templates)
                    return Resource.My_Templates;
                else if (name == Page_Name.Summary_Report)
                    return Resource.Summary_Report;
                else if (name == Page_Name.Detail_Transaction_Report)
                    return Resource.Detail_Transaction_Report;
                else if (name == Page_Name.Job_Cost)
                    return Resource.Job_Cost;
                else if (name == Page_Name.Customer)
                    return Resource.Customer;
                else if (name == Page_Name.Time_Sheet)
                    return Resource.Time_Sheet;
                else if (name == Page_Name.Time_Sheet_Configuration)
                    return Resource.Time_Sheet_Configuration;
                else if (name == Page_Name.My_Time_Sheet)
                    return Resource.My_Time_Sheet;
                else if (name == Page_Name.Time_Sheet_Management)
                    return Resource.Time_Sheet_Management;
                else if (name == Page_Name.New_Time_Sheet)
                    return Resource.New_Time_Sheet;
                else if (name == Page_Name.Time_Sheet_Transaction_Report)
                    return Resource.Time_Sheet_Transaction_Report;
                else if (name == Page_Name.Master_Lookup_Configurarion)
                    return Resource.Master_Lookup_Configurarion;
                else if (name == Page_Name.Billing_Report)
                    return Resource.Billing_Report;
                else if (name == Page_Name.All_Time_Sheet)
                    return Resource.All_Time_Sheet;
                else if (name == Page_Name.All_Expenses)
                    return Resource.All_Expenses;
                else
                    return name;
            }
            catch
            {
                return name;
            }
        }

        public static string Get_Manu_Name(String name)
        {
            try
            {
                if (name == ModuleCode.Authentication)
                    return Resource.Authentication;
                else if (name == ModuleCode.Employee)
                    return Resource.Employee;
                else if (name == ModuleCode.Leave)
                    return Resource.Leave;
                else if (name == ModuleCode.Expense)
                    return Resource.Expense;
                else if (name == ModuleCode.Payroll)
                    return Resource.Payroll;
                else if (name == ModuleCode.Time)
                    return Resource.Time;
                else if (name == ModuleCode.Inventory)
                    return Resource.Inventory;
                else if (name == ModuleCode.Quotation)
                    return Resource.Quotation;
                else if (name == ModuleCode.POS)
                    return Resource.POS;
                else
                    return name;
            }
            catch
            {
                return name;
            }
        }

    }

    public class AccessRight
    {
        public static string Access = "Access";
        public static string Create = "Create";
        public static string Update = "Update";
        public static string Delete = "Delete";

        public static string Get_Action(String action)
        {
            try
            {
                if (action == AccessRight.Access)
                    return Resource.Access;
                else if (action == AccessRight.Create)
                    return Resource.Create;
                else if (action == AccessRight.Update)
                    return Resource.Update;
                else if (action == AccessRight.Delete)
                    return Resource.Delete;
                else
                    return action;
            }
            catch
            {
                return action;
            }
        }

    }

    public class CountryName
    {
        public static string Thailand = "TH";
    }


    public class ModuleAddonType
    {
        public static string Module = "Module";
        public static string Addon = "Addon";
    }

    public class ChargeBy
    {
        public static string PerUser = "Per User";
        public static string PerCompany = "Per Company";
    }

    public class PaymentStatus
    {
        public static string Free_Trial = "Free Trial";
        public static string Paid = "Paid";
        public static string Unpaid = "Unpaid"; // when process payroll complete
        public static string Outstanding = "Outstanding"; // when confirm payroll complete
    }
    public class SystemDefaultSetup
    {
        public static string Branch = "Branch";
        public static string Department = "Department";
        public static string Designation = "Designation";
        public static string Expense_Category = "Expense Category";
        public static string Attachment_Type = "Attachment Type";
    }
    public class TaxType
    {
        public static string Inclusive = "Inclusive";
        public static string Exclusive = "Exclusive";
    }


    public class ScheduledAction
    {
        public static string Pending = "Pending";
        public static string Fail = "Fail";
        public static string Sent = "Sent";
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBSWorkFlowAPI.Constants
{

   public class WfERROR_CODE
   {
      public static int SUCCESS = 1;
      public static int SUCCESS_CREATE = 2;
      public static int SUCCESS_EDIT = 3;
      public static int SUCCESS_DELETE = 4;
      public static int SUCCESS_APPROVE = 5;
      public static int SUCCESS_REJECT = 6;
      public static int SUCCESS_REQUEST_CANCEL = 7;
      public static int SUCCESS_Hold_Bill = 8;
      public static int SUCCESS_Void = 9;

      public static int ERROR_1_USER_NOT_FOUND = -1;
      public static int ERROR_2_ACTIVATE_CODE_EXPIRE = -2;
      public static int ERROR_3_ACTIVATE_CODE_NOT_FOUND = -3;
      public static int ERROR_4_RESET_PASSWORD_EXPIRE = -4;
      public static int ERROR_5_RESET_PASSWORD_CODE_NOT_FOUND = -5;
      public static int ERROR_6_NO_ACCESS_RIGHT = -6;
      public static int ERROR_7_USER_NOT_FOUND = -7;
      public static int ERROR_8_NO_ADMIN_COMPANY = -8;
      public static int ERROR_9_EMAIL_DUPPLICATE = -9;
      public static int ERROR_10_ASSIGN_USER_ROLE_FIRST = -10;
      public static int ERROR_11_ASSIGN_ACCESS_RIGHT_FIRST = -11;
      public static int ERROR_12_COMPANY_INACTIVE = -12;
      public static int ERROR_13_NO_DEFAULT_CURRENCY = -13;
      public static int ERROR_14_NO_EMPLOYEE_HIST = -14;
      public static int ERROR_15_NO_APPROVER = -15;
      public static int ERROR_16_NO_EMPLOYEE_PROFILE = -16;
      public static int ERROR_17_NO_PREFIX = -17;
      public static int ERROR_18_NO_RECEIPT_CONFIG = -18;
      public static int ERROR_19_CASHIER_INVALID = -19;
      public static int ERROR_401_UNAUTHORIZED = -401;
      public static int ERROR_404_PAGE_NOT_FOUND = -404;
      public static int ERROR_500_DB = -500;
      public static int ERROR_501_CANT_SEND_EMAIL = -501;
      public static int ERROR_502_IDENTITY_NOT_FOUND = -502;
      public static int ERROR_503_INSERT_ERROR = -503;
      public static int ERROR_504_UPDATE_ERROR = -504;
      public static int ERROR_505_DELETE_ERROR = -505;
      public static int ERROR_506_SAVE_ERROR = -506;

      public static int ERROR_510_DATA_DUPLICATE = -510;
      public static int ERROR_511_DATA_NOT_FOUND = -511;

      public static int ERROR_601_PRINTER_NOT_FOUND = -601;
      public static int ERROR_601_PRINTER_ERROR = -602;
   }

   public class WfServiceResult
   {
      public string Msg { get; set; }
      public int Code { get; set; }
      public string Field { get; set; }
   }


   public class WfRecordStatus
   {
      public static string Active = "A";
      public static string InActive = "C";
      public static string Delete = "D";

      public static string GetReccordStatus(string status)
      {
         if (status == Active)
         {
            return "Active";
         }
         else
         {
            return "Inactive";
         }
      }
   }

   public class ApprovalType
   {
      public static string Time = "Time";
      public static string Leave = "Leave";
      public static string Expense = "Expenses";
      public static string Payroll = "Payroll";
      public static string Quotation = "Quotation";
      public static string SaleOrder = "SaleOrder";
      public static string PurchaseOrder = "Purchase";
      public static string TimeSheet = "TimeSheet";
   }

   public class WorkflowAction
   {
      public static string Submit = "Submit";
      public static string Approve = "Approve";
      public static string Reject = "Reject";
      public static string Cancel = "Cancel";
      public static string Close = "Close";
      public static string Assign = "Assign";
      public static string Reassign = "Reassign";
   }

   public class WorkflowStatus
   {
      public static string Submitted = "Submitted";
      public static string Pending = "Pending";
      public static string Rejected = "Rejected";
      public static string Cancelled = "Cancelled";
      public static string Canceling = "Canceling";
      public static string Approved = "Approved";
      public static string Closed = "Closed";
      public static string Cancellation_Approved = "Cancellation Approved";
      public static string Cancellation_Rejected = "Cancellation Rejected";

      public static string Draft = "Draft";
   }

   public enum WorkFlowCurrentStatus
   {
      None,
      EmpActive,
      EmpActiveRqCancel,
      EmpInactive,
      ApprovalActive,
      ApprovalInactive,
   }

   internal class Utils
   {
      internal static DateTime GetDateTimeNow()
      {
         return DateTime.Now;
      }
   }
   public class ApproverFlowType
   {
      public static string Employee = "Employee";
      public static string Job_Cost = "Job Cost";
   }
}

using HR.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using SBSModel.Models;
using SBSModel.Common;
using SBSWorkFlowAPI.Models;
using SBSResourceAPI;


namespace HR.Models
{

   public class LeaveConfigurationViewModel : ModelBase
   {

      public List<ComboViewModel> departmentList { get; set; }
      public List<ComboViewModel> emplist { get; set; }
      public List<ComboViewModel> lTypelist { get; set; }

      //******** Holiday ***********
      [LocalizedDisplayName("Year", typeof(Resource))]
      public Nullable<int> search_Holiday_Year { get; set; }

      public List<Holiday_Config> HolidayList { get; set; }

      [LocalizedDisplayName("Holiday", typeof(Resource))]
      public Nullable<int> Holiday_ID { get; set; }

      [LocalizedDisplayName("Holiday_Name", typeof(Resource))]
      public string Holiday_Name { get; set; }

      [DataType(DataType.Date)]
      [LocalizedDisplayName("Start_Date", typeof(Resource))]
      public string Holiday_Start_Date { get; set; }

      [DataType(DataType.Date)]
      [LocalizedDisplayName("End_Date", typeof(Resource))]
      public string Holiday_End_Date { get; set; }

      //******** leave type ***********
      [LocalizedDisplayName("Leave_Type", typeof(Resource))]
      public Nullable<int> search_Leave_Leave_Config { get; set; }

      public List<Leave_Config> LeaveTypeList { get; set; }

      //******** leave adjustment ***********
      [LocalizedDisplayName("Year", typeof(Resource))]
      public Nullable<int> search_Adjust_Year { get; set; }
      [LocalizedDisplayName("Leave_Type", typeof(Resource))]
      public Nullable<int> search_Adjust_Leave_Type { get; set; }

      public List<Leave_Adjustment> LeaveAdjustList { get; set; }

      //******** leave approval ***********
      [LocalizedDisplayName("Department", typeof(Resource))]
      public Nullable<int> search_Approval_Department { get; set; }

      public List<Approval_Flow> ApprovalList { get; set; }

   }

   public class ApplicableEmployeeViewModel
   {

      public Nullable<int> Employee_Profile_ID { get; set; }
      public Nullable<int> Department_ID { get; set; }
      public Nullable<int> Profile_ID { get; set; }
      public string Email { get; set; }
      public string Name { get; set; }

   }

   public class ApprovalFlowViewModel : ModelBase
   {

      public List<ComboViewModel> departmentList { get; set; }
      public List<_Applicable_Employee> EmpList { get; set; }
      public List<ComboViewModel> ApproverFlowTypeList { get; set; }


      public string md { get; set; }
      public Nullable<int> Profile_ID { get; set; }
      public Nullable<int> Approval_Flow_ID { get; set; }
      public string Approval_Type { get; set; }
      public Nullable<int> Company_ID { get; set; }
      public string Module { get; set; }
      public Nullable<int> Branch_ID { get; set; }
      public string Branch_Name { get; set; }
      public string Record_Status { get; set; }
      public int[] Departments { get; set; }
      public int[] Application_For { get; set; }
      public int[] Not_Application_For { get; set; }
      public ApproverViewModel[] Approver_Rows { get; set; }
      public ReviewerViewModel[] Reviewer_Rows { get; set; }

   }

   public class ApproverViewModel
   {
      public List<_Applicable_Employee> EmpList { get; set; }
      public List<ComboViewModel> ApproverFlowTypeList { get; set; }

      //Check duplicated prof ID
      public int I { get; set; }
      public int Index { get; set; }
      public Nullable<int> Approver_ID { get; set; }
      public Nullable<int> Approval_Flow_ID { get; set; }
      public Nullable<int> Approval_Level { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Employee", typeof(Resource))]
      public Nullable<int> Profile_ID { get; set; }
      public string Email { get; set; }
      public string Name { get; set; }
      public string Row_Type { get; set; }

      public string Approver_Flow_Type { get; set; }
      public string Approval_Type { get; set; }


   }

   public class ReviewerViewModel
   {

      public List<_Applicable_Employee> EmpList { get; set; }

      public int Index { get; set; }
      public Nullable<int> Reviewer_ID { get; set; }
      public Nullable<int> Approval_Flow_ID { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Employee", typeof(Resource))]
      public Nullable<int> Profile_ID { get; set; }

      public string Email { get; set; }
      public string Name { get; set; }
      public string Row_Type { get; set; }

   }

   public class LeaveManagementViewModel : ModelBase
   {
      public string tabAction { get; set; }
      public string search_Pending_Date_AppliedFrom { get; set; }
      public string search_Pending_Date_AppliedTo { get; set; }
      public Nullable<int> search_Pending_Emp { get; set; }
      public string search_Process_Date_AppliedFrom { get; set; }
      public string search_Process_Date_AppliedTo { get; set; }
      public Nullable<int> search_Process_Emp { get; set; }
      public List<Leave_Application_Document> LeaveApplicationDocumentList { get; set; }
      public List<User_Profile> employeeList { get; set; }

      [LocalizedValidDate]
      [LocalizedRequired]
      [LocalizedDisplayName("Start_Date", typeof(Resource))]
      public string Start_Date { get; set; }

      [LocalizedValidDate]
      [LocalizedRequired]
      [LocalizedDisplayName("End_Date", typeof(Resource))]
      public string End_Date { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Delegate_Approval_To", typeof(Resource))]
      public Nullable<int> Delegation_Profile_ID { get; set; }

      public Nullable<int> Approval_Delegation_ID { get; set; }

   }

   public class LeaveViewModel : ModelBase
   {

      public string tabAction { get; set; }
      public string ApprStatus { get; set; }
      public string PageStatus { get; set; }
      public int Approval_Level { get; set; }

      public List<ComboViewModel> lTypelist { get; set; }
      public List<ComboViewModel> lStatuslist { get; set; }
      public List<ComboViewModel> branchList { get; set; }
      public List<ComboViewModel> departmentList { get; set; }
      public List<ComboViewModel> LeaveTypeComboList { get; set; }
      public List<ComboViewModel> periodList { get; set; }
      public List<ComboViewModel> EmployeeUnderMeList { get; set; }
      public List<User_Profile> EmpList { get; set; }
      [LocalizedDisplayName("Leave_Type", typeof(Resource))]

      public Nullable<int> search_Leave_Leave_Config { get; set; }
      public string search_Leave_Status { get; set; }
      public string search_Date_Applied_From { get; set; }
      public string search_Date_Applied_To { get; set; }
      public Nullable<int> search_Emp { get; set; }
      public Nullable<int> search_Branch { get; set; }
      public Nullable<int> search_Department { get; set; }
      public Nullable<int> search_Pending_Emp { get; set; }

      public List<User_Profile> employeeList { get; set; }
      public List<Leave_Application_Document> LeaveApplicationDocumentList { get; set; }
      public Holidays[] HolidayList { get; set; }
      public List<DateTime> HolidayDatetimeList { get; set; }
      public List<Global_Lookup_Data> LeaveConfigList { get; set; }
      public Nullable<int> Leave_Application_Document_ID { get; set; }

      [LocalizedDisplayName("Leave_Type", typeof(Resource))]
      public string Leave_Name { get; set; }

      public string Leave_Type_Desc { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Leave_Type", typeof(Resource))]
      public Nullable<int> Leave_Config_ID { get; set; }

      public Nullable<int> Employee_Profile_ID { get; set; }
      public Nullable<int> OnBehalf_Employee_Profile_ID { get; set; }
      public Nullable<int> OnBehalf_Profile_ID { get; set; }
      public string Name { get; set; }
      public string Email { get; set; }
      public string[] Collor { get; set; }
      public Nullable<int> Profile_ID { get; set; }

      [LocalizedDisplayName("Employee_Name", typeof(Resource))]
      public string EmployeeName { get; set; }

      [LocalizedValidDate]
      [LocalizedRequired]
      [LocalizedDisplayName("Start_Date", typeof(Resource))]
      [DataType(DataType.Date)]
      public string Start_Date { get; set; }

      [DataType(DataType.Date)]
      [LocalizedDisplayName("Date_Applied", typeof(Resource))]
      public string Date_Applied { get; set; }

      [LocalizedValidDate]
      [LocalizedRequired]
      [LocalizedDisplayName("End_Date", typeof(Resource))]
      [DataType(DataType.Date)]
      public string End_Date { get; set; }

      [DataType(DataType.Date)]
      [LocalizedDisplayName("LeaveDate", typeof(Resource))]
      public string Leave_Date { get; set; }

      public string Start_Date_Period { get; set; }

      public List<int> Working_Days { get; set; }
      public Nullable<int> Relationship_ID { get; set; }
      public string Relationship_Name { get; set; }
      public Nullable<decimal> Maternity_Weeks_Left { get; set; }
      public Nullable<decimal> Maternity_Weeks_Taken { get; set; }
      public bool Maternity_Is_First_Period { get; set; }
      public string End_Date_Period { get; set; }

      [LocalizedValidMaxLength(300)]
      [LocalizedDisplayName("Note", typeof(Resource))]
      public string Reasons { get; set; }

      [LocalizedDisplayName("LeaveLeft", typeof(Resource))]
      public Nullable<decimal> Leave_Left { get; set; }

      [LocalizedValidMaxLength(30)]
      [LocalizedDisplayName("Contact_While_Overseas", typeof(Resource))]
      public string Contact_While_Overseas { get; set; }

      [LocalizedValidMaxLength(30)]
      [LocalizedDisplayName("Secondary_Contact_No", typeof(Resource))]
      public string Second_Contact_While_Overseas { get; set; }

      [LocalizedValidMaxLength(300)]
      [LocalizedDisplayName("Address_While_On_Leave", typeof(Resource))]
      public string Address_While_On_Leave { get; set; }

      [LocalizedDisplayName("Upload_Document", typeof(Resource))]
      public Nullable<System.Guid> Upload_Document_ID { get; set; }

      [LocalizedDisplayName("Days_Taken", typeof(Resource))]
      public string Days_Taken { get; set; }
      public byte[] file { get; set; }
      public string filename { get; set; }

      [LocalizedValidMaxLength(1000)]
      [LocalizedDisplayName("Remark", typeof(Resource))]
      public string Remark { get; set; }

      [LocalizedValidMaxLength(1000)]
      [LocalizedDisplayName("Remark", typeof(Resource))]
      public string Remark_Rej { get; set; }

      public bool IsUploadDoc { get; set; }
      public string Approval_Status_1st { get; set; }
      public string Approval_Status_2st { get; set; }
      public string Approval_Cancel_Status { get; set; }
      public int? Supervisor { get; set; }
      public string Supervisor_Name { get; set; }
      public string Overall_Status { get; set; }
      public Nullable<int> Request_ID { get; set; }
      public List<Request> Leave_Request { get; set; }
      public Nullable<decimal> Entitle { get; set; }
      public Nullable<bool> Flexibly { get; set; }
      public Nullable<bool> Continuously { get; set; }
      public string Type { get; set; }

      //Added by sun 18-02-2016
      public string Cancel_Status { get; set; }
      public Nullable<int> Request_Cancel_ID { get; set; }
      public Nullable<bool> isRejectPopUp { get; set; }

      //Added by Moet on 23-Sep-2016
      public User_Profile userProfile { get; set; }
      public string requestURL { get; set; }
   }

   public class LeaveReportViewModel : ModelBase
   {

      public List<ComboViewModel> departmentList { get; set; }
      public List<ComboViewModel> leavetypelist { get; set; }
      public List<Leave_Report> leavelist { get; set; }

      // ----- search feilds -----
      [LocalizedDisplayName("Department", typeof(Resource))]
      public Nullable<int> Department { get; set; }

      [LocalizedDisplayName("Leave_Type", typeof(Resource))]
      public Nullable<int> Leave_Config_ID { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Year", typeof(Resource))]
      public int Year { get; set; }

      [LocalizedDisplayName("From", typeof(Resource))]
      public string From_Date { get; set; }

      [LocalizedDisplayName("To", typeof(Resource))]
      public string To_Date { get; set; }

      [LocalizedDisplayName("Leave_Type", typeof(Resource))]
      public Nullable<int> Leave_Type { get; set; }

      public string Leave_Type_Name { get; set; }
      public string Leave_Sel { get; set; }

      [LocalizedDisplayName("Leave_Type", typeof(Resource))]
      public List<int> Leave_Type_Sel { get; set; }

   }

   public class LeaveDashBoardViewModel : ModelBase
   {

      public List<Leave_Type> LeaveBalanceList { get; set; }
      public List<Leave_Application_Document> LeaveList { get; set; }
      public string Currency_Code { get; set; }

   }

   public class ImportLeaveViewModels : ModelBase
   {

      public bool validated_Main { get; set; }
      public ImportLeaveApplicationDocument_[] leaveAppDoc { get; set; }
      public List<string> errMsg { get; set; }

   }

   public class ImportLeaveApplicationDocument_ : ModelBase
   {

      public Nullable<int> Company_ID { get; set; }
      public bool Validate { get; set; }
      public string ErrMsg { get; set; }
      public Nullable<int> Employee_Profile_ID { get; set; }
      public string Employee_No { get; set; }
      public Nullable<int> Leave_Config_ID { get; set; }
      public string Leave_Config_Name { get; set; }
      public string Start_Date { get; set; }
      public string End_Date { get; set; }
      public string Remark { get; set; }
      public decimal Days_Taken { get; set; }

   }

   public class LeaveListViewModel : ModelBase
   {

      public List<Leave_List> LeaveList { get; set; }
      public Nullable<int> Leave_Type { get; set; }

      public List<ComboViewModel> leavetypelst { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Year", typeof(Resource))]
      public int Year { get; set; }

      public List<int> Yearlst { get; set; }

   }

   public class Leave_List : ModelBase
   {

      public Nullable<int> Leave_ID { get; set; }
      public string Leave_Type { get; set; }
      public decimal Days_Taken { get; set; }

   }

}
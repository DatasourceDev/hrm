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

using HR.Models;
using SBSModel.Models;
using SBSModel.Common;
using SBSResourceAPI;
using SBSTimeModel.Models;
using SBSWorkFlowAPI.Models;

namespace HR.Models
{
   public class TimeSheetViewModel : ModelBase
   {
      public Nullable<int> search_Month { get; set; }
      public Nullable<int> search_Year { get; set; }
      public Nullable<int> Time_Sheet_ID { get; set; }
      public Nullable<int> Company_ID { get; set; }
      public Nullable<int> Employee_Profile_ID { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Date_Of_Date")]
      public string Date_Of_Date { get; set; }
      public string Indenent_No { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Job")]
      public Nullable<int> Job_Cost_ID { get; set; }
      public Nullable<int> Customer_ID { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Clock_In")]
      public string Clock_In { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Clock_Out")]
      public string Clock_Out { get; set; }
      public Nullable<TimeSpan> Duration { get; set; }
      public Nullable<TimeSpan> Lunch_Duration { get; set; }

      [LocalizedDisplayName("Note")]
      public string Note { get; set; }
      public string Overall_Status { get; set; }
      public string Record_Status { get; set; }
      public int? Supervisor { get; set; }
      public string Cancel_Status { get; set; }
      public Nullable<int> Request_Cancel_ID { get; set; }
      public Nullable<int> Request_ID { get; set; }

      public List<Time_Sheet> TimeSheetList { get; set; }
      public List<Job_Cost> JobCostList { get; set; }
      public List<ComboViewModel> JobCostlst { get; set; }
      public List<SBSWorkFlowAPI.Models.Request> Time_Sheet_Request { get; set; }

      public string ApprStatus { get; set; }
      public int Approval_TimeSheet { get; set; }
      public string PageStatus { get; set; }
      public string Supervisor_Name { get; set; }
      public Nullable<bool> isRejectPopUp { get; set; }

      [LocalizedValidMaxLength(300)]
      [LocalizedDisplayName("Remarks", typeof(Resource))]
      public string Remark_Rej { get; set; }


      //********Time Sheet Management***********
      //public List<Employee_Profile> EmployeeList { get; set; }
      public List<User_Profile> EmployeeList { get; set; }
      //Pending
      public Nullable<int> Search_Pending_Emp { get; set; }
      public Nullable<int> Search_Pending_Month { get; set; }
      public Nullable<int> Search_Pending_Year { get; set; }
      //Process
      public Nullable<int> Search_Process_Emp { get; set; }
      public Nullable<int> Search_Process_Month { get; set; }
      public Nullable<int> Search_Process_Year { get; set; }

      public List<TsEX> TsEXPendingList { get; set; }
      public List<TsEX> TsEXProcessedList { get; set; }

      //********Time Sheet Record***********
      public List<ComboViewModel> WFStatuslst { get; set; }
      public string Search_Time_Sheet_Status { get; set; }
      public string Date_From { get; set; }
      public string Date_To { get; set; }




   }

   public class TimeSheetConfigurationViewModel : ModelBase
   {
      //********Time Sheet approval ***********
      [LocalizedDisplayName("Department", typeof(Resource))]
      public Nullable<int> Approval_Department { get; set; }
      public List<ComboViewModel> departmentList { get; set; }
      public List<Approval_Flow> ApprovalList { get; set; }
   }

   public class TimeSheetTransactionViewModel : ModelBase
   {
      //********Time Sheet Report ***********
      public List<TsEX> TsExList { get; set; }
      public List<Time_Sheet> TimeSheetList { get; set; }
      public List<Job_Cost> JobCostList { get; set; }
      public List<Employee_Profile> EmployeeList { get; set; }
      public List<Expenses_Application_Document> ExpensesApplicationDocumentList { get; set; }
      // public Working_Days workdays { get; set; }

      public List<Working_Days_List> WorkingDaysList { get; set; }


      public Employment_History EmploymentHistory { get; set; }

      public Nullable<int> Time_Sheet_ID { get; set; }
      public Nullable<int> Search_Employee_Profile_ID { get; set; }
      public Nullable<int> Profile_ID { get; set; }

      [LocalizedValidDate]
      public string Date_From { get; set; }

      [LocalizedValidDate]
      public string Date_To { get; set; }

      public Nullable<int> Search_Pending_Emp { get; set; }
      public string Emp_Name { get; set; }
      public string Department_Name { get; set; }
      public List<ExpensesApplicationDup> ExList { get; set; }
      public List<ComboViewModel> Monthlst { get; set; }
      public List<int> Yearlst { get; set; }

      [LocalizedDisplayName("Month", typeof(Resource))]
      public Nullable<int> Search_Month { get; set; }

      [LocalizedDisplayName("Year", typeof(Resource))]
      public Nullable<int> Search_Year { get; set; }

   }


   public class ExpensesApplicationDup
   {
      public string Date { get; set; }
      public Nullable<int> Job_Cost_Id { get; set; }
   }


   public class Working_Days_List
   {
      public Working_Days workdays { get; set; }
      public Nullable<int> Employee_Profile_ID { get; set; }
   }

}
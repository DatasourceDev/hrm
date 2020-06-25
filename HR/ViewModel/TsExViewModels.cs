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
using SBSWorkFlowAPI.Constants;

namespace HR.Models
{
   public class TsExViewModels : ModelBase
   {
      public List<TsEX> timesheetExlist { get; set; }
      public int? search_Month { get; set; }
      public int? search_Year { get; set; }
   }

   public class TsExInfoViewModels : ModelBase
   {
      public string ctlr { get; set; }
      public string ac { get; set; }
      public string Status { get; set; }

      public int? Emp_Login_ID { get; set; }

      public List<ComboViewModel> cJobCostList { get; set; }

      [LocalizedDisplayName("Month", typeof(Resource))]
      public int Month { get; set; }
      [LocalizedDisplayName("Year", typeof(Resource))]
      public int Year { get; set; }

      [Required]
      [LocalizedDisplayName("Date_Applied", typeof(Resource))]
      public string Date_Applied { get; set; }      
      public int? Employee_Profile_ID { get; set; }
      public int? Profile_ID { get; set; }

      public int? TsEx_ID { get; set; }
      [LocalizedDisplayName("Name", typeof(Resource))]
      public string Name { get; set; }


      #region Timesheet
      public int? Time_Sheet_ID { get; set; }
      public TsRowViewModel[] TsRows { get; set; }
      public string TsStatus { get; set; }
      #endregion

      #region Expenses
      public int? Expenses_ID { get; set; }
      public string Expenses_No { get; set; }
      public string Expenses_Title { get; set; }
   
      public string ExStatus { get; set; }
      public string ExCancelStatus { get; set; }
      public ExRowViewModel[] ExRows { get; set; }

      public decimal? ExTotal_Amount { get; set; }
      public decimal? TsTotal_Amount { get; set; }
      public string Next_Approver { get; set; }
      #endregion


      public WorkFlowCurrentStatus wfCurrentStatus { get; set; }
   }

   public class TsRowViewModel
   {
      //********Time Sheet Report ***********
      public int i { get; set; }
      public List<ComboViewModel> cJobCostList { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Job_Cost", typeof(Resource))]
      public Nullable<int> Job_Cost_ID { get; set; }
      public Nullable<int> Time_Sheet_ID { get; set; }
      public Nullable<int> Dtl_ID { get; set; }

      [LocalizedDisplayName("Note", typeof(Resource))]
      public string Note { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Clock_In", typeof(Resource))]
      public string Clock_In { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Clock_Out", typeof(Resource))]
      public string Clock_Out { get; set; }

      [LocalizedDisplayName("Date_Of_Date", typeof(Resource))]
      public string Date_Of_Date { get; set; }
      public string Customer_Name { get; set; }
      public string Duration { get; set; }
      public Nullable<decimal> Hour_Rate { get; set; }
      public string Indent_Name { get; set; }
      public string Indent_No { get; set; }
      public string Launch_Duration { get; set; }
      public Nullable<decimal> Total_Amount { get; set; }

      public string Lunch_In { get; set; }
      public string Lunch_Out { get; set; }
   }

   public class ExRowViewModel
   {
      public List<ComboViewModel> cJobCostList { get; set; }
      public List<Expenses_Config> cExpensesConfigList { get; set; }
      public List<ComboViewModel> cTaxTypelst { get; set; }
      public List<ComboViewModel> cAmountTypelst { get; set; }

      public string erowID { get; set; }
      public int i { get; set; }
      public string ExDate { get; set; }
      
      public Nullable<int> Expenses_Application_Document_ID { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Expenses_Type", typeof(Resource))]
      public Nullable<int> Expenses_Config_ID { get; set; }

      [LocalizedDisplayName("ClaimableType", typeof(Resource))]
      public string Claimable_Type { get; set; }

      [LocalizedDisplayName("Department", typeof(Resource))]
      public Nullable<int> Department_ID { get; set; }

      [LocalizedDisplayName("Date_Applied", typeof(Resource))]
      public string Date_Applied { get; set; }

      [LocalizedDisplayName("Total_Amount", typeof(Resource))]
      public Nullable<decimal> Ex_Total_Amount { get; set; }

      [LocalizedDisplayName("Amount_Claiming", typeof(Resource))]
      public Nullable<decimal> Amount_Claiming { get; set; }

      [LocalizedDisplayName("Balance", typeof(Resource))]
      public Nullable<decimal> Balance { get; set; }
      public Nullable<decimal> Balance_Amount { get; set; }
     
      [LocalizedDisplayName("Tax", typeof(Resource))]
      public Nullable<decimal> Tax { get; set; }

      [LocalizedValidMaxLength(500)]
      [LocalizedDisplayName("Expenses_Type_Desc", typeof(Resource))]
      public string Expenses_Type_Desc { get; set; }
      public string Expenses_Type_Name { get; set; }

      [LocalizedValidMaxLength(500)]
      [LocalizedDisplayName("Notes", typeof(Resource))]
      public string Notes { get; set; }

      [LocalizedDisplayName("Upload_Receipt", typeof(Resource))]
      public Nullable<System.Guid> Upload_Receipt_ID { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Selected_Currency", typeof(Resource))]
      public Nullable<int> Selected_Currency { get; set; }

      [LocalizedValidDate]
      [LocalizedRequired]
      [LocalizedDisplayName("Expenses_Date", typeof(Resource))]
      public string Expenses_Date { get; set; }

      [LocalizedValidMaxLength(300)]
      public string Doc_No { get; set; }

      public string Upload_Receipt { get; set; }
      public string Upload_Receipt_Name { get; set; }  

      public Nullable<int> Ex_Job_Cost_ID { get; set; }
      public string Job_Cost_No { get; set; }
      public string Job_Cost_Name { get; set; }
      public Nullable<decimal> Job_Cost_Balance { get; set; }

      [LocalizedDisplayName("Withholding_Tax", typeof(Resource))]
      public Nullable<decimal> Withholding_Tax { get; set; }
      public string Tax_Type { get; set; }

      public Nullable<decimal> Withholding_Tax_Amount { get; set; }
      public Nullable<decimal> Tax_Amount { get; set; }

      [LocalizedValidMaxLength(10)]
      [LocalizedDisplayName("Type", typeof(Resource))]
      public string Tax_Amount_Type { get; set; }

      [LocalizedValidMaxLength(10)]
      [LocalizedDisplayName("Type", typeof(Resource))]
      public string Withholding_Tax_Type { get; set; }

      public string Row_Type { get; set; }

      public Nullable<int> Default_Currency_ID { get; set; }
      public string Default_Currency_Code { get; set; }
      
   }

   public class TsExFunc
   {
      public static string GetEditOnclick(HR.Models.ExRowViewModel model)
      {
         var str = new StringBuilder();
         str.Append("EditEx_Onclick(");
         str.Append("'" + model.erowID + "',");
         str.Append("'" + model.Expenses_Application_Document_ID + "',");
         str.Append("'" + model.Doc_No + "',");
         str.Append("'" + model.ExDate + "',");
         str.Append("'" + model.Expenses_Date + "',");
         str.Append("'" + model.Expenses_Config_ID + "',");
         str.Append("'" + model.Selected_Currency + "',");
         str.Append("'" + model.Ex_Total_Amount + "',");
         str.Append("'" + model.Tax_Type + "',");
         str.Append("'" + model.Tax + "',");
         str.Append("'" + model.Tax_Amount_Type + "',");
         str.Append("'" + model.Withholding_Tax + "',");
         str.Append("'" + model.Withholding_Tax_Type + "',");
         str.Append("'" + model.Ex_Job_Cost_ID + "',");
         str.Append("'" + model.Upload_Receipt_ID + "',");
         str.Append("'" + model.Upload_Receipt_Name + "',");
         str.Append("'" + model.Upload_Receipt + "',");
         str.Append("'" + model.Notes + "'");

         str.Append(")");
         return str.ToString();
      }
   }
   

}
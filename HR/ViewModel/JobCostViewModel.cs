using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using HR.Common;
using HR.Models;
using SBSModel.Models;
using SBSModel.Common;
using SBSResourceAPI;
namespace HR.Models
{
   public class JobCostViewModel : ModelBase
   {
      [LocalizedDisplayName("Customer_Name", typeof(Resource))]
      public Nullable<int> search_Customer_ID { get; set; }

      public List<ComboViewModel> cCustomerlst { get; set; }
      public List<Job_Cost> JobCostList { get; set; }
   }

   public class JobCostInfoViewModel : ModelBase
   {
      public List<ComboViewModel> cPaymentPeriodlst { get; set; }
      public List<ComboViewModel> cCustomerlst { get; set; }
      public List<ComboViewModel> cExpensesTypelist { get; set; }

      public int Job_Cost_ID { get; set; }
      public Nullable<int> Company_ID { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Indent_No", typeof(Resource))]
      public string Indent_No { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(300)]
      [LocalizedDisplayName("Indent_Name", typeof(Resource))]
      public string Indent_Name { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Customer_Name", typeof(Resource))]
      public Nullable<int> Customer_ID { get; set; }

      [LocalizedValidDate]
      [LocalizedDisplayName("Date_Of_Date", typeof(Resource))]
      public string Date_Of_Date { get; set; }

      [Required]
      [LocalizedValidDecimal]
      [LocalizedDisplayName("Sell_Price", typeof(Resource))]
      public Nullable<decimal> Sell_Price { get; set; }

      [LocalizedValidDate]
      [LocalizedDisplayName("Delivery_Date", typeof(Resource))]
      public string Delivery_Date { get; set; }

      [LocalizedValidDecimal]
      [LocalizedDisplayName("Term_Of_Deliver", typeof(Resource))]
      public Nullable<decimal> Term_Of_Deliver { get; set; }

      [LocalizedValidDecimal]
      [LocalizedDisplayName("Warranty_Term", typeof(Resource))]
      public Nullable<decimal> Warranty_Term { get; set; }

      [LocalizedValidDecimal]
      [LocalizedDisplayName("Costing", typeof(Resource))]
      public Nullable<decimal> Costing { get; set; }

      [LocalizedDisplayName("Status", typeof(Resource))]
      public string Record_Status { get; set; }

      public List<Expenses_Application> ExpensesApplicationList { get; set; }

      public string Customer_Name { get; set; }
      public string Company_Name { get; set; }
      public string Address_Row_1 { get; set; }
      public string Address_Row_2 { get; set; }
      public string Address_Row_3 { get; set; }

      public JobCostPaymentTermViewModel[] JobCostPaymentTerm_Rows { get; set; }
      public JobCostExBudgetViewModel[] JobCostExBudget_Rows { get; set; }

      [LocalizedDisplayName("Supervisor", typeof(Resource))]
      public Nullable<int> Supervisor { get; set; }

      public List<ComboViewModel> cSupervisorlst { get; set; }
   }

   public class JobCostPaymentTermViewModel : ModelBase
   {
      public int Job_Cost_PayMent_Term_ID { get; set; }
      public Nullable<int> Job_Cost_ID { get; set; }

      [LocalizedDisplayName("Payment", typeof(Resource))]
      public Nullable<decimal> Payment { get; set; }

      [LocalizedDisplayName("Payment_Type", typeof(Resource))]
      public string Payment_Type { get; set; }

      [LocalizedDisplayName("Invoice_No", typeof(Resource))]
      public string Invoice_No { get; set; }

      [LocalizedDisplayName("Invoice_Date", typeof(Resource))]
      public string Invoice_Date { get; set; }

      [LocalizedDisplayName("Note", typeof(Resource))]
      public string Note { get; set; }

      [LocalizedDisplayName("Actual_Price", typeof(Resource))]
      public Nullable<decimal> Actual_Price { get; set; }

      public int Index { get; set; }
      public string Row_Type { get; set; }

      public List<ComboViewModel> cPaymentPeriodlst { get; set; }
   }

   public class JobCostExBudgetViewModel : ModelBase
   {
      public int Budget_ID { get; set; }
      public Nullable<int> Job_Cost_ID { get; set; }
      public Nullable<int> Expenses_Config_ID { get; set; }

      [LocalizedDisplayName("Budget", typeof(Resource))]
      public Nullable<decimal> Budget { get; set; }

      public int Index { get; set; }
      public string Row_Type { get; set; }

      public List<ComboViewModel> cExpensesTypelist { get; set; }
   }

   public class TempDateTimeModel
   {
      public string Month { get; set; }
      public string Year { get; set; }
   }

   public class JobCostExportViewModel
   {
      public string csv { get; set; }
   }
}
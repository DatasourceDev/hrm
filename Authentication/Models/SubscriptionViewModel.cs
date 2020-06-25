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

    public class SubscriptionViewModel
    {
        public string step { get; set; }
        public List<SBS_Module_Detail> moduleList { get; set; }
    }

    public class SubscriptionMainViewModel : ModelBase
    {
        //Filter
        public List<ComboViewModel> Companylist;

        public List<Company_Details> CompanyLst;
        public Nullable<int> Company_ID { get; set; }
        public List<SubscriptionDetailViewModel> LstSubscription { get; set; }
    }


    public class SubscriptionReportViewModel : ModelBase
    {
        public SubscriptionDetailViewModel[] SubscriptionDetail_Rows { get; set; }
    }

    public class SubscriptionDetailViewModel : ModelBase
    {
        public Nullable<int> Company_ID { get; set; }
        public string Company_Name { get; set; }
        public Nullable<int> No_Of_Users { get; set; }
        public string Address { get; set; }
        public string Module_Name { get; set; }
        public Nullable<DateTime> Start_Date { get; set; }
        public Nullable<DateTime> Expiration_Date { get; set; }
        public Nullable<int> Total_Licenses { get; set; }
        public Nullable<int> Assigned_Licenses { get; set; }
        public Nullable<int> UnAssigned_Licenses { get; set; }
    }

    public class BillingViewModels : ModelBase
    {

        public List<Invoice_Header> Invoicelist { get; set; }

        public Nullable<int> Search_Month { get; set; }
        public Nullable<int> Search_Year { get; set; }
        public List<ComboViewModel> processDateList { get; set; }
        public List<int> YearList { get; set; }
    }
    public class InvoiceViewModels : ModelBase
    {
        public int Company_ID { get; set; }
        public string Company_Name { get; set; }
        public string CurrencyCode { get; set; }        
        public DateTime? Generated_On { get; set; }
        public DateTime Due_Date { get; set; } 
        public int Invoice_ID { get; set; }        
        public int? Invoice_Month { get; set; }
        public string Invoice_No { get; set; }
        public string Invoice_Status { get; set; }
        public string Invoice_To { get; set; }
        public string Invoice_To_Address { get; set; }
        public string Product_Name { get; set; }
        public decimal? Due_Amount { get; set; }
        public int? Invoice_Year { get; set; }
        public int Licenses { get; set; }        
        public List<Invoice_Details> Invoice_Details { get; set; }
        public List<User_Transactions> transList { get; set; }
        public List<Display_Transactions> InvTrans { get; set; }
        public List<Invoice_Header> Outstanding_Invoices { get; set; }
        public List<Storage_Upgrade> Storage_Upgrade_List { get; set; }
        public decimal? Ttl_Outstanding_Amt { get; set; } // Added on 25-Nov-2016
        public decimal? Ttl_Upgrade_Amt { get; set; } // Added on 25-Nov-2016
    }
   
    public class Invoice_Transactions
    {
        public DateTime TranDate { get; set; }
        public int NoOfUsers { get; set; }
        public decimal Amount { get; set; }
    }
    public class Display_Transactions
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NoOfUsers { get; set; }
        public int NoOfDays { get; set; }
        public decimal Amount { get; set; }
    }

}


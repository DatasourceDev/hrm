using SBSModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateInvoiceService.ViewModel
{
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

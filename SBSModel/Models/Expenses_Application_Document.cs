using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Expenses_Application_Document
    {
        public Expenses_Application_Document()
        {
            this.PRDEs = new List<PRDE>();
            this.Upload_Receipt = new List<Upload_Receipt>();
        }

        public int Expenses_Application_Document_ID { get; set; }
        public Nullable<int> Expenses_Application_ID { get; set; }
        public Nullable<int> Department_ID { get; set; }
        public Nullable<int> Expenses_Config_ID { get; set; }
        public Nullable<int> Employee_Profile_ID { get; set; }
        public Nullable<decimal> Amount_Claiming { get; set; }
        public Nullable<decimal> Total_Amount { get; set; }
        public Nullable<System.DateTime> Expenses_Date { get; set; }
        public Nullable<int> Selected_Currency { get; set; }
        public string Reasons { get; set; }
        public Nullable<System.DateTime> Last_Date_Approved { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> Date_Applied { get; set; }
        public string Overall_Status { get; set; }
        public string Approval_Status_1st { get; set; }
        public string Approval_Status_2st { get; set; }
        public string Approval_Cancel_Status { get; set; }
        public string Payroll_Flag { get; set; }
        public Nullable<decimal> Tax { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public Nullable<decimal> Mileage { get; set; }
        public Nullable<int> Job_Cost_ID { get; set; }
        public Nullable<decimal> Withholding_Tax { get; set; }
        public string Doc_No { get; set; }
        public string Tax_Type { get; set; }
        public Nullable<decimal> Withholding_Tax_Amount { get; set; }
        public Nullable<decimal> Tax_Amount { get; set; }
        public string Tax_Amount_Type { get; set; }
        public string Withholding_Tax_Type { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual Department Department { get; set; }
        public virtual Employee_Profile Employee_Profile { get; set; }
        public virtual Expenses_Application Expenses_Application { get; set; }
        public virtual Expenses_Config Expenses_Config { get; set; }
        public virtual Job_Cost Job_Cost { get; set; }
        public virtual ICollection<PRDE> PRDEs { get; set; }
        public virtual ICollection<Upload_Receipt> Upload_Receipt { get; set; }
    }
}

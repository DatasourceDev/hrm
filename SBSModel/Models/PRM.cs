using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class PRM
    {
        public PRM()
        {
            this.PRDs = new List<PRD>();
            this.PRDEs = new List<PRDE>();
            this.PRDLs = new List<PRDL>();
        }

        public int PRM_ID { get; set; }
        public Nullable<int> Selected_OT_Formula_ID { get; set; }
        public Nullable<int> Selected_CPF_Formula_ID { get; set; }
        public int Employee_Profile_ID { get; set; }
        public string Process_Status { get; set; }
        public Nullable<int> Process_Month { get; set; }
        public Nullable<int> Process_Year { get; set; }
        public Nullable<System.DateTime> Run_date { get; set; }
        public Nullable<decimal> Total_Allowance { get; set; }
        public Nullable<decimal> Total_Deduction { get; set; }
        public Nullable<decimal> Total_Work_Days { get; set; }
        public Nullable<decimal> CPF_Employee { get; set; }
        public Nullable<decimal> CPF_Emplyer { get; set; }
        public Nullable<decimal> Gross_Wages { get; set; }
        public Nullable<decimal> Nett_Wages { get; set; }
        public Nullable<System.DateTime> Leave_Period_From { get; set; }
        public Nullable<System.DateTime> Leave_Period_to { get; set; }
        public Nullable<System.DateTime> Expenses_Period_From { get; set; }
        public Nullable<System.DateTime> Expenses_Period_to { get; set; }
        public string Cheque_No { get; set; }
        public Nullable<bool> Bonus_Issue { get; set; }
        public Nullable<decimal> Total_Bonus { get; set; }
        public Nullable<bool> Director_Fee_Issue { get; set; }
        public Nullable<decimal> Total_Director_Fee { get; set; }
        public Nullable<decimal> Basic_Salary { get; set; }
        public Nullable<decimal> Total_Extra_Donation { get; set; }
        public Nullable<decimal> Donation { get; set; }
        public Nullable<int> Selected_Donation_Formula_ID { get; set; }
        public Nullable<int> Payment_Type { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public Nullable<decimal> No_Of_Hours { get; set; }
        public Nullable<int> Revision_No { get; set; }
        public Nullable<System.DateTime> Process_Date_From { get; set; }
        public Nullable<System.DateTime> Process_Date_To { get; set; }
        public Nullable<decimal> Hourly_Rate { get; set; }
        public Nullable<decimal> Total_Allowance_Basic_Salary { get; set; }
        public virtual Employee_Profile Employee_Profile { get; set; }
        public virtual Global_Lookup_Data Global_Lookup_Data { get; set; }
        public virtual ICollection<PRD> PRDs { get; set; }
        public virtual ICollection<PRDE> PRDEs { get; set; }
        public virtual ICollection<PRDL> PRDLs { get; set; }
        public virtual Selected_CPF_Formula Selected_CPF_Formula { get; set; }
        public virtual Selected_Donation_Formula Selected_Donation_Formula { get; set; }
        public virtual Selected_OT_Formula Selected_OT_Formula { get; set; }
    }
}

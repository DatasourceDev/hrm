using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Employment_History
    {
        public Employment_History()
        {
            this.Employment_History_Allowance = new List<Employment_History_Allowance>();
        }

        public int History_ID { get; set; }
        public int Employee_Profile_ID { get; set; }
        public Nullable<int> Department_ID { get; set; }
        public Nullable<int> Designation_ID { get; set; }
        public Nullable<int> Currency_ID { get; set; }
        public string Record_Status { get; set; }
        public Nullable<System.DateTime> Effective_Date { get; set; }
        public Nullable<int> Supervisor { get; set; }
        public Nullable<int> Branch_ID { get; set; }
        public Nullable<System.DateTime> Confirm_Date { get; set; }
        public Nullable<System.DateTime> Terminate_Date { get; set; }
        public string Basic_Salary { get; set; }
        public Nullable<int> Payment_Type { get; set; }
        public Nullable<int> Employee_Type { get; set; }
        public Nullable<System.TimeSpan> ST_Sun_Time { get; set; }
        public Nullable<System.TimeSpan> ST_Mon_Time { get; set; }
        public Nullable<System.TimeSpan> ST_Tue_Time { get; set; }
        public Nullable<System.TimeSpan> ST_Wed_Time { get; set; }
        public Nullable<System.TimeSpan> ST_Thu_Time { get; set; }
        public Nullable<System.TimeSpan> ST_Fri_Time { get; set; }
        public Nullable<System.TimeSpan> ST_Sat_Time { get; set; }
        public Nullable<System.TimeSpan> ET_Sun_Time { get; set; }
        public Nullable<System.TimeSpan> ET_Mon_Time { get; set; }
        public Nullable<System.TimeSpan> ET_Tue_Time { get; set; }
        public Nullable<System.TimeSpan> ET_Wed_Time { get; set; }
        public Nullable<System.TimeSpan> ET_Thu_Time { get; set; }
        public Nullable<System.TimeSpan> ET_Fri_Time { get; set; }
        public Nullable<System.TimeSpan> ET_Sat_Time { get; set; }
        public Nullable<bool> CL_Sun { get; set; }
        public Nullable<bool> CL_Mon { get; set; }
        public Nullable<bool> CL_Tue { get; set; }
        public Nullable<bool> CL_Wed { get; set; }
        public Nullable<bool> CL_Thu { get; set; }
        public Nullable<bool> CL_Fri { get; set; }
        public Nullable<bool> CL_Sat { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public Nullable<System.DateTime> Hired_Date { get; set; }
        public Nullable<decimal> Bonus_Factor { get; set; }
        public Nullable<decimal> Days_Worked_WK { get; set; }
        public Nullable<decimal> Hours_Worked_YR { get; set; }
        public string CPF_AC_No { get; set; }
        public Nullable<int> Daily_Rate { get; set; }
        public Nullable<int> CPF_Type { get; set; }
        public Nullable<bool> CL_The { get; set; }
        public Nullable<int> NPL_Daily { get; set; }
        public Nullable<decimal> Notice_Period_Amount { get; set; }
        public string Notice_Period_Unit { get; set; }
        public Nullable<bool> Contract_Staff { get; set; }
        public Nullable<System.DateTime> Contract_Start_Date { get; set; }
        public Nullable<System.DateTime> Contract_End_Date { get; set; }
        public string Basic_Salary_Unit { get; set; }
        public Nullable<decimal> Days { get; set; }
        public Nullable<int> Employee_Grading_ID { get; set; }
        public string Other_Branch { get; set; }
        public string Other_Department { get; set; }
        public string Other_Designation { get; set; }
        public Nullable<bool> No_Approval_WF { get; set; }
        public Nullable<bool> CL_Lunch { get; set; }
        public Nullable<System.TimeSpan> ST_Lunch_Time { get; set; }
        public Nullable<System.TimeSpan> ET_Lunch_Time { get; set; }
        public Nullable<decimal> Hour_Rate { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual Department Department { get; set; }
        public virtual Designation Designation { get; set; }
        public virtual Employee_Profile Employee_Profile { get; set; }
        public virtual Employee_Profile Employee_Profile1 { get; set; }
        public virtual ICollection<Employment_History_Allowance> Employment_History_Allowance { get; set; }
        public virtual Global_Lookup_Data Global_Lookup_Data { get; set; }
    }
}

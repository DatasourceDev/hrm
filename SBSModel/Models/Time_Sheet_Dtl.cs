using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Time_Sheet_Dtl
    {
        public int Dtl_ID { get; set; }
        public Nullable<int> Time_Sheet_ID { get; set; }
        public Nullable<System.DateTime> Date_Of_Date { get; set; }
        public Nullable<System.TimeSpan> Clock_In { get; set; }
        public Nullable<System.TimeSpan> Clock_Out { get; set; }
        public string Note { get; set; }
        public Nullable<int> Job_Cost_ID { get; set; }
        public string Indent_No { get; set; }
        public string Indent_Name { get; set; }
        public string Customer_Name { get; set; }
        public Nullable<decimal> Hour_Rate { get; set; }
        public Nullable<decimal> Total_Amount { get; set; }
        public Nullable<System.TimeSpan> Launch_Duration { get; set; }
        public Nullable<System.TimeSpan> Duration { get; set; }
        public virtual Job_Cost Job_Cost { get; set; }
        public virtual Time_Sheet Time_Sheet { get; set; }
    }
}

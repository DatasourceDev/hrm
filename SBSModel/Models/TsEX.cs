using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class TsEX
    {
        public int TsEx_ID { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public Nullable<int> Employee_Profile_ID { get; set; }
        public string Doc_No { get; set; }
        public Nullable<System.DateTime> Doc_Date { get; set; }
        public Nullable<int> Month { get; set; }
        public Nullable<int> Year { get; set; }
        public Nullable<int> Expenses_Application_ID { get; set; }
        public Nullable<int> Time_Sheet_ID { get; set; }
        public Nullable<decimal> Ex_Total_Amount { get; set; }
        public Nullable<decimal> Ts_Total_Amount { get; set; }
        public virtual Company Company { get; set; }
        public virtual Employee_Profile Employee_Profile { get; set; }
        public virtual Expenses_Application Expenses_Application { get; set; }
        public virtual Time_Sheet Time_Sheet { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace SBSTimeModel.Models
{
    public partial class Time_Arrangement
    {
        public int Arrangement_ID { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public Nullable<int> Employee_Profile_ID { get; set; }
        public Nullable<int> Branch_ID { get; set; }
        public string Remark { get; set; }
        public Nullable<bool> Repeat { get; set; }
        public string Day_Of_Week { get; set; }
        public Nullable<System.DateTime> Effective_Date { get; set; }
        public Nullable<System.TimeSpan> Time_From { get; set; }
        public Nullable<System.TimeSpan> Time_To { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
    }
}

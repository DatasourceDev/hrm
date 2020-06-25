using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Working_Days
    {
        public int Working_Days_ID { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public Nullable<decimal> Days { get; set; }
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
        public Nullable<bool> CL_Lunch { get; set; }
        public Nullable<System.TimeSpan> ST_Lunch_Time { get; set; }
        public Nullable<System.TimeSpan> ET_Lunch_Time { get; set; }
        public virtual Company Company { get; set; }
    }
}

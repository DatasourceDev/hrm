using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Branch
    {
        public Branch()
        {
            this.Employee_No_Pattern = new List<Employee_No_Pattern>();
            this.Employment_History = new List<Employment_History>();
        }

        public int Branch_ID { get; set; }
        public int Company_ID { get; set; }
        public string Branch_Code { get; set; }
        public string Branch_Name { get; set; }
        public string Branch_Desc { get; set; }
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
        public Nullable<bool> ST_Same { get; set; }
        public Nullable<bool> ET_Same { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public Nullable<System.TimeSpan> ET_Time { get; set; }
        public Nullable<System.TimeSpan> ST_Time { get; set; }
        public string Record_Status { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<Employee_No_Pattern> Employee_No_Pattern { get; set; }
        public virtual ICollection<Employment_History> Employment_History { get; set; }
    }
}

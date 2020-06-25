using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Time_Sheet
    {
        public Time_Sheet()
        {
            this.Time_Sheet_Dtl = new List<Time_Sheet_Dtl>();
            this.TsEXes = new List<TsEX>();
        }

        public int Time_Sheet_ID { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public Nullable<int> Employee_Profile_ID { get; set; }
        public Nullable<int> Supervisor { get; set; }
        public string Employee_Name { get; set; }
        public Nullable<int> Request_ID { get; set; }
        public Nullable<int> Request_Cancel_ID { get; set; }
        public string Overall_Status { get; set; }
        public string Cancel_Status { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual Employee_Profile Employee_Profile { get; set; }
        public virtual ICollection<Time_Sheet_Dtl> Time_Sheet_Dtl { get; set; }
        public virtual ICollection<TsEX> TsEXes { get; set; }
    }
}

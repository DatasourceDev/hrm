using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Notification_Scheduler
    {
        public int Notification_Scheduler_ID { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public string Notice_Type { get; set; }
        public Nullable<bool> Trigger_Set_Up { get; set; }
        public string Trigger_Period { get; set; }
        public Nullable<System.TimeSpan> Start_Time { get; set; }
        public Nullable<System.DateTime> Start_Date { get; set; }
        public Nullable<int> Recur_Every_Days { get; set; }
        public Nullable<int> Recur_Every_Weeks { get; set; }
        public Nullable<bool> Selected_Sunday { get; set; }
        public Nullable<bool> Selected_Monday { get; set; }
        public Nullable<bool> Selected_Tuesday { get; set; }
        public Nullable<bool> Selected_Wednesday { get; set; }
        public Nullable<bool> Selected_Thursday { get; set; }
        public Nullable<bool> Selected_Friday { get; set; }
        public Nullable<bool> Selected_Saturday { get; set; }
        public Nullable<int> Selected_Months { get; set; }
        public string Selected_Days { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual Company Company { get; set; }
    }
}

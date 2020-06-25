using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Time_Mobile_Trans
    {
        public int Trans_ID { get; set; }
        public Nullable<int> Map_ID { get; set; }
        public string UUID { get; set; }
        public Nullable<System.DateTime> Trans_Date { get; set; }
        public Nullable<System.TimeSpan> Clock_In { get; set; }
        public Nullable<System.TimeSpan> Clock_Out { get; set; }
        public string Note { get; set; }
        public Nullable<System.TimeSpan> Duration { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public string Update_By { get; set; }
        public virtual Time_Mobile_Map Time_Mobile_Map { get; set; }
    }
}

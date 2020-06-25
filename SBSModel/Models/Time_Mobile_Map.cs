using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Time_Mobile_Map
    {
        public Time_Mobile_Map()
        {
            this.Time_Mobile_Trans = new List<Time_Mobile_Trans>();
        }

        public int Map_ID { get; set; }
        public string UUID { get; set; }
        public Nullable<int> Employee_Profile_ID { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public string Update_By { get; set; }
        public virtual Employee_Profile Employee_Profile { get; set; }
        public virtual ICollection<Time_Mobile_Trans> Time_Mobile_Trans { get; set; }
    }
}

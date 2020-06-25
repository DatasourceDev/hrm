using System;
using System.Collections.Generic;

namespace SBSTimeModel.Models
{
    public partial class Time_Device_Map
    {
        public int Map_ID { get; set; }
        public Nullable<int> Device_ID { get; set; }
        public Nullable<int> Employee_Profile_ID { get; set; }
        public string Employee_Email { get; set; }
        public Nullable<int> Device_Employee_Pin { get; set; }
        public string Device_Employee_Name { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public string Record_Status { get; set; }
        public string Employee_Name { get; set; }
        public virtual Time_Device Time_Device { get; set; }
    }
}

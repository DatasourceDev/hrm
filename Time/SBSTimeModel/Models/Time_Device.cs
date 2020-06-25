using System;
using System.Collections.Generic;

namespace SBSTimeModel.Models
{
    public partial class Time_Device
    {
        public Time_Device()
        {
            this.Time_Transaction = new List<Time_Transaction>();
            this.Time_Device_Map = new List<Time_Device_Map>();
        }

        public int Device_ID { get; set; }
        public Nullable<int> Branch_ID { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public string Device_No { get; set; }
        public string IP_Address { get; set; }
        public Nullable<int> Port { get; set; }
        public string User_Name { get; set; }
        public string Password { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public string Record_Status { get; set; }
        public string Branch_Name { get; set; }
        public Nullable<int> Min_Transaction_Id { get; set; }
        public Nullable<int> Max_Transaction_Id { get; set; }
        public string Brand_Name { get; set; }
        public virtual ICollection<Time_Transaction> Time_Transaction { get; set; }
        public virtual ICollection<Time_Device_Map> Time_Device_Map { get; set; }
    }
}

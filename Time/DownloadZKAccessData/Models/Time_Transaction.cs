using System;
using System.Collections.Generic;

namespace DownloadZKAccessData.Models
{
    public partial class Time_Transaction
    {
        public int Time_Transaction_ID { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public Nullable<int> Device_ID { get; set; }
        public Nullable<int> Employee_Profile_ID { get; set; }
        public Nullable<int> Device_Transaction_ID { get; set; }
        public Nullable<System.DateTime> Device_Transaction_Date { get; set; }
        public Nullable<int> Device_Employee_Pin { get; set; }
        public string Job_Code { get; set; }
        public string Transaction_Type { get; set; }
        public string Card_ID { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public string Employee_Name { get; set; }
        public virtual Time_Device Time_Device { get; set; }
    }
}

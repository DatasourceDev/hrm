using System;
using System.Collections.Generic;

namespace DownloadZKAccessData.Models
{
    public partial class Time_Sheet
    {
        public int Time_Sheet_ID { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public Nullable<int> Employee_Profile_ID { get; set; }
        public Nullable<System.DateTime> Date_Of_Date { get; set; }
        public Nullable<int> Job_Cost_ID { get; set; }
        public Nullable<System.TimeSpan> Clock_In { get; set; }
        public Nullable<System.TimeSpan> Clock_Out { get; set; }
        public string Note { get; set; }
        public string Overall_Status { get; set; }
        public string Record_Status { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public Nullable<int> Request_ID { get; set; }
        public Nullable<int> Request_Cancel_ID { get; set; }
        public Nullable<int> Supervisor { get; set; }
        public string Employee_Name { get; set; }
        public string Indent_Name { get; set; }
        public string Customer_Name { get; set; }
        public Nullable<decimal> Hour_Rate { get; set; }
        public string Indent_No { get; set; }
        public string Cancel_Status { get; set; }
        public Nullable<System.TimeSpan> Launch_Duration { get; set; }
        public Nullable<System.TimeSpan> Duration { get; set; }
        public Nullable<decimal> Total_Amount { get; set; }
    }
}

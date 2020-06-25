using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Leave_Application_Document
    {
        public Leave_Application_Document()
        {
            this.PRDLs = new List<PRDL>();
            this.Upload_Document = new List<Upload_Document>();
        }

        public int Leave_Application_Document_ID { get; set; }
        public Nullable<int> Leave_Config_ID { get; set; }
        public Nullable<int> Employee_Profile_ID { get; set; }
        public Nullable<System.DateTime> Start_Date { get; set; }
        public Nullable<System.DateTime> End_Date { get; set; }
        public string Reasons { get; set; }
        public Nullable<System.DateTime> Last_Date_Approved { get; set; }
        public string Address_While_On_Leave { get; set; }
        public string Contact_While_Overseas { get; set; }
        public string Period { get; set; }
        public string Remark { get; set; }
        public Nullable<System.DateTime> Date_Applied { get; set; }
        public string Overall_Status { get; set; }
        public string Approval_Status_1st { get; set; }
        public string Approval_Status_2st { get; set; }
        public string Approval_Cancel_Status { get; set; }
        public string Start_Date_Period { get; set; }
        public string End_Date_Period { get; set; }
        public Nullable<decimal> Days_Taken { get; set; }
        public string Payroll_Flag { get; set; }
        public Nullable<decimal> Processed_Day { get; set; }
        public Nullable<decimal> Balance_Day { get; set; }
        public Nullable<int> Leave_Config_Detail_ID { get; set; }
        public Nullable<int> Request_ID { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public string Second_Contact_While_Overseas { get; set; }
        public Nullable<int> Relationship_ID { get; set; }
        public Nullable<decimal> Weeks_Taken { get; set; }
        public string Cancel_Status { get; set; }
        public Nullable<int> Request_Cancel_ID { get; set; }
        public Nullable<int> Supervisor { get; set; }
        public virtual Employee_Profile Employee_Profile { get; set; }
        public virtual Relationship Relationship { get; set; }
        public virtual Leave_Config Leave_Config { get; set; }
        public virtual Leave_Config_Detail Leave_Config_Detail { get; set; }
        public virtual ICollection<PRDL> PRDLs { get; set; }
        public virtual ICollection<Upload_Document> Upload_Document { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Expenses_Application
    {
        public Expenses_Application()
        {
            this.TsEXes = new List<TsEX>();
            this.Expenses_Application_Document = new List<Expenses_Application_Document>();
        }

        public int Expenses_Application_ID { get; set; }
        public Nullable<int> Employee_Profile_ID { get; set; }
        public string Expenses_No { get; set; }
        public string Expenses_Title { get; set; }
        public Nullable<System.DateTime> Date_Applied { get; set; }
        public string Overall_Status { get; set; }
        public string Approval_Status_1st { get; set; }
        public string Approval_Status_2st { get; set; }
        public string Approval_Cancel_Status { get; set; }
        public Nullable<System.DateTime> Last_Date_Approved { get; set; }
        public Nullable<int> Request_ID { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public string Cancel_Status { get; set; }
        public Nullable<int> Request_Cancel_ID { get; set; }
        public Nullable<int> Supervisor { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public string Approver { get; set; }
        public string Next_Approver { get; set; }
        public virtual Company Company { get; set; }
        public virtual Employee_Profile Employee_Profile { get; set; }
        public virtual ICollection<TsEX> TsEXes { get; set; }
        public virtual ICollection<Expenses_Application_Document> Expenses_Application_Document { get; set; }
    }
}

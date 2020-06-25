using System;
using System.Collections.Generic;

namespace SBSWorkFlowAPI.Models
{
    public partial class Request
    {
        public Request()
        {
            this.Histories = new List<History>();
            this.Task_Assignment = new List<Task_Assignment>();
        }

        public int Request_ID { get; set; }
        public int Company_ID { get; set; }
        public int Approval_Flow_ID { get; set; }
        public int Approval_Level { get; set; }
        public string Status { get; set; }
        public int Requestor_Profile_ID { get; set; }
        public string Requestor_Name { get; set; }
        public string Requestor_Email { get; set; }
        public Nullable<System.DateTime> Request_Date { get; set; }
        public Nullable<System.DateTime> Last_Action_Date { get; set; }
        public string Approval_Type { get; set; }
        public string Module { get; set; }
        public Nullable<int> Leave_Application_Document_ID { get; set; }
        public Nullable<int> Doc_ID { get; set; }
        public string Request_Type { get; set; }
        public Nullable<bool> Is_Indent { get; set; }
        public virtual Approval_Flow Approval_Flow { get; set; }
        public virtual ICollection<History> Histories { get; set; }
        public virtual ICollection<Task_Assignment> Task_Assignment { get; set; }
    }
}

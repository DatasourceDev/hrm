using System;
using System.Collections.Generic;

namespace SBSWorkFlowAPI.Models
{
    public partial class Approval_Flow
    {
        public Approval_Flow()
        {
            this.Applicable_Employee = new List<Applicable_Employee>();
            this.Approvers = new List<Approver>();
            this.Conditions = new List<Condition>();
            this.Departments = new List<Department>();
            this.Requests = new List<Request>();
            this.Reviewers = new List<Reviewer>();
        }

        public int Approval_Flow_ID { get; set; }
        public string Approval_Type { get; set; }
        public int Company_ID { get; set; }
        public string Module { get; set; }
        public int Branch_ID { get; set; }
        public string Branch_Name { get; set; }
        public string Record_Status { get; set; }
        public int Created_By { get; set; }
        public System.DateTime Created_On { get; set; }
        public Nullable<int> Updated_By { get; set; }
        public Nullable<System.DateTime> Updated_On { get; set; }
        public virtual ICollection<Applicable_Employee> Applicable_Employee { get; set; }
        public virtual ICollection<Approver> Approvers { get; set; }
        public virtual ICollection<Condition> Conditions { get; set; }
        public virtual ICollection<Department> Departments { get; set; }
        public virtual ICollection<Request> Requests { get; set; }
        public virtual ICollection<Reviewer> Reviewers { get; set; }
    }
}

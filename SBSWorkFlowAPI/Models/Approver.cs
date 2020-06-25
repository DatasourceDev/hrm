using System;
using System.Collections.Generic;

namespace SBSWorkFlowAPI.Models
{
    public partial class Approver
    {
        public int Approver_ID { get; set; }
        public int Approval_Flow_ID { get; set; }
        public int Company_ID { get; set; }
        public int Approval_Level { get; set; }
        public int Profile_ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Approval_Flow_Type { get; set; }
        public virtual Approval_Flow Approval_Flow { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace SBSWorkFlowAPI.Models
{
    public partial class Condition
    {
        public int Condition_ID { get; set; }
        public int Approval_Flow_ID { get; set; }
        public string StrCondition { get; set; }
        public Nullable<decimal> LeftRange { get; set; }
        public Nullable<decimal> RightRange { get; set; }
        public virtual Approval_Flow Approval_Flow { get; set; }
    }
}

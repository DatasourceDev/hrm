using System;
using System.Collections.Generic;

namespace SBSWorkFlowAPI.Models
{
    public partial class Department
    {
        public int Worklow_Dept_ID { get; set; }
        public int Approval_Flow_ID { get; set; }
        public int User_Department_ID { get; set; }
        public string Name { get; set; }
        public virtual Approval_Flow Approval_Flow { get; set; }
    }
}

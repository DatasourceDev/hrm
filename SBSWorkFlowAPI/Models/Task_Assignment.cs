using System;
using System.Collections.Generic;

namespace SBSWorkFlowAPI.Models
{
    public partial class Task_Assignment
    {
        public int Task_ID { get; set; }
        public int Request_ID { get; set; }
        public int Profile_ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Record_Status { get; set; }
        public string Status { get; set; }
        public Nullable<int> Approval_Level { get; set; }
        public Nullable<bool> Is_Indent { get; set; }
        public Nullable<bool> Indent_Closed { get; set; }
        public virtual Request Request { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace SBSWorkFlowAPI.Models
{
    public partial class History
    {
        public int History_ID { get; set; }
        public int Request_ID { get; set; }
        public int Profile_ID { get; set; }
        public string Action { get; set; }
        public System.DateTime Action_On { get; set; }
        public string Action_By { get; set; }
        public string Action_Email { get; set; }
        public string Remarks { get; set; }
        public virtual Request Request { get; set; }
    }
}

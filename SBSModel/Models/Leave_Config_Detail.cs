using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Leave_Config_Detail
    {
        public Leave_Config_Detail()
        {
            this.Leave_Application_Document = new List<Leave_Application_Document>();
        }

        public int Leave_Config_Detail_ID { get; set; }
        public Nullable<int> Leave_Config_ID { get; set; }
        public Nullable<int> Designation_ID { get; set; }
        public Nullable<decimal> Default_Leave_Amount { get; set; }
        public Nullable<int> Year_Service { get; set; }
        public Nullable<int> Group_ID { get; set; }
        public Nullable<decimal> Bring_Forward_Days { get; set; }
        public Nullable<bool> Is_Bring_Forward_Percent { get; set; }
        public virtual Designation Designation { get; set; }
        public virtual ICollection<Leave_Application_Document> Leave_Application_Document { get; set; }
        public virtual Leave_Config Leave_Config { get; set; }
    }
}

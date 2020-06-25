using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Leave_Default_Detail
    {
        public int Default_Detail_ID { get; set; }
        public Nullable<int> Default_ID { get; set; }
        public Nullable<decimal> Default_Leave_Amount { get; set; }
        public Nullable<int> Year_Service { get; set; }
        public Nullable<decimal> Bring_Forward_Days { get; set; }
        public Nullable<bool> Is_Bring_Forward_Percent { get; set; }
        public virtual Leave_Default Leave_Default { get; set; }
    }
}

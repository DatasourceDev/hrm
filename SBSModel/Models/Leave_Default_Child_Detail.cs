using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Leave_Default_Child_Detail
    {
        public int Leave_Default_Child_Detail_ID { get; set; }
        public Nullable<int> Default_ID { get; set; }
        public string Residential_Status { get; set; }
        public Nullable<decimal> Default_Leave_Amount { get; set; }
        public string Period { get; set; }
        public virtual Leave_Default Leave_Default { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Leave_Config_Child_Detail
    {
        public int Leave_Config_Child_Detail_ID { get; set; }
        public Nullable<int> Leave_Config_ID { get; set; }
        public string Residential_Status { get; set; }
        public Nullable<decimal> Default_Leave_Amount { get; set; }
        public string Period { get; set; }
        public Nullable<int> Group_ID { get; set; }
        public virtual Leave_Config Leave_Config { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Leave_Config_Extra
    {
        public int Leave_Config_Extra_ID { get; set; }
        public Nullable<int> Leave_Config_ID { get; set; }
        public Nullable<int> Employee_Profile_ID { get; set; }
        public string Adjustment_Type { get; set; }
        public Nullable<decimal> No_Of_Days { get; set; }
        public virtual Employee_Profile Employee_Profile { get; set; }
        public virtual Leave_Config Leave_Config { get; set; }
    }
}

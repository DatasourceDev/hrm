using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Leave_Config_Condition
    {
        public int Leave_Config_Condition_ID { get; set; }
        public Nullable<int> Lookup_Data_ID { get; set; }
        public Nullable<int> Leave_Config_ID { get; set; }
        public virtual Global_Lookup_Data Global_Lookup_Data { get; set; }
        public virtual Leave_Config Leave_Config { get; set; }
    }
}

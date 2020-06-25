using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Leave_Default_Condition
    {
        public int Leave_Default_Condition_ID { get; set; }
        public Nullable<int> Lookup_Data_ID { get; set; }
        public Nullable<int> Default_ID { get; set; }
        public virtual Global_Lookup_Data Global_Lookup_Data { get; set; }
        public virtual Leave_Default Leave_Default { get; set; }
    }
}

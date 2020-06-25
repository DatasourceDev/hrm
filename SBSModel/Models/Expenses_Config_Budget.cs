using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Expenses_Config_Budget
    {
        public int Budget_ID { get; set; }
        public Nullable<int> Expenses_Config_ID { get; set; }
        public Nullable<int> Job_Cost_ID { get; set; }
        public Nullable<decimal> Budget { get; set; }
        public virtual Expenses_Config Expenses_Config { get; set; }
        public virtual Job_Cost Job_Cost { get; set; }
    }
}

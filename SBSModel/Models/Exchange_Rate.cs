using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Exchange_Rate
    {
        public int Exchange_Rate_ID { get; set; }
        public Nullable<int> Exchange_Currency_ID { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public string Exchange_Period { get; set; }
        public Nullable<System.DateTime> Exchange_Date { get; set; }
        public Nullable<int> Exchange_Month { get; set; }
        public virtual Exchange_Currency Exchange_Currency { get; set; }
    }
}

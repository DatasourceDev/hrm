using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Exchange_Currency
    {
        public Exchange_Currency()
        {
            this.Exchange_Rate = new List<Exchange_Rate>();
        }

        public int Exchange_Currency_ID { get; set; }
        public Nullable<int> Exchange_ID { get; set; }
        public Nullable<int> Currency_ID { get; set; }
        public string Exchange_Period { get; set; }
        public virtual Exchange Exchange { get; set; }
        public virtual ICollection<Exchange_Rate> Exchange_Rate { get; set; }
    }
}

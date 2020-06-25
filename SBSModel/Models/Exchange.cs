using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Exchange
    {
        public Exchange()
        {
            this.Exchange_Currency = new List<Exchange_Currency>();
        }

        public int Exchange_ID { get; set; }
        public Nullable<int> Fiscal_Year { get; set; }
        public string Record_Status { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public virtual ICollection<Exchange_Currency> Exchange_Currency { get; set; }
    }
}

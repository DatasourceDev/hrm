using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class SBS_Module_Detail
    {
        public SBS_Module_Detail()
        {
            this.Pages = new List<Page>();
            this.Subscriptions = new List<Subscription>();
        }

        public int Module_Detail_ID { get; set; }
        public int Module_ID { get; set; }
        public string Module_Detail_Name { get; set; }
        public string Module_Detail_Description { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<decimal> Price_Per_Person { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public Nullable<int> Order_Index { get; set; }
        public virtual ICollection<Page> Pages { get; set; }
        public virtual SBS_Module SBS_Module { get; set; }
        public virtual ICollection<Subscription> Subscriptions { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Module
    {
        public Module()
        {
            this.Subscriptions = new List<Subscription>();
        }

        public string Module_Code { get; set; }
        public string Module_Name { get; set; }
        public string Module_Description { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<decimal> Price_Per_Person { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual ICollection<Subscription> Subscriptions { get; set; }
    }
}

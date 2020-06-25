using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class User_Assign_Module
    {
        public int User_Assign_Module_ID { get; set; }
        public Nullable<int> Subscription_ID { get; set; }
        public Nullable<int> Profile_ID { get; set; }
        public virtual Subscription Subscription { get; set; }
        public virtual User_Profile User_Profile { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class User_Transactions
    {
        public int Transaction_ID { get; set; }
        public int Company_ID { get; set; }
        public int Profile_ID { get; set; }
        public Nullable<System.DateTime> Activate_On { get; set; }
        public Nullable<System.DateTime> Deactivate_On { get; set; }
        public string Activate_By { get; set; }
        public string Deactivate_By { get; set; }
        public virtual Company Company { get; set; }
        public virtual User_Profile User_Profile { get; set; }
    }
}

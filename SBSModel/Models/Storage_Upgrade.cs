using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Storage_Upgrade
    {
        public int Transaction_ID { get; set; }
        public int Company_ID { get; set; }
        public int Upgrade_Space { get; set; }
        public decimal Price { get; set; }
        public Nullable<System.DateTime> Upgrade_On { get; set; }
        public string Upgrade_By { get; set; }
        public Nullable<System.DateTime> Expired_On { get; set; }
        public string Record_Status { get; set; }
        public virtual Company Company { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class PRAL
    {
        public int PRAL_ID { get; set; }
        public int PRG_ID { get; set; }
        public int Employee_Profile_ID { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual Employee_Profile Employee_Profile { get; set; }
        public virtual PRG PRG { get; set; }
    }
}

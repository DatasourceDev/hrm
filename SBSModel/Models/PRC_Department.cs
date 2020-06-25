using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class PRC_Department
    {
        public int PRC_Department_ID { get; set; }
        public int PRC_ID { get; set; }
        public int Department_ID { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual Department Department { get; set; }
        public virtual PRC PRC { get; set; }
    }
}

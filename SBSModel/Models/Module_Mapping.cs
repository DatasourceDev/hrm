using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Module_Mapping
    {
        public int Mapping_ID { get; set; }
        public int Product_ID { get; set; }
        public int Module_Detail_ID { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public int Promotion_ID { get; set; }
        public int Module_ID { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Ctl_Table_Syn
    {
        public string Table_Name { get; set; }
        public Nullable<int> Index { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
    }
}

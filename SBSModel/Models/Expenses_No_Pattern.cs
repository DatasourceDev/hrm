using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Expenses_No_Pattern
    {
        public int Expenses_No_Pattern_ID { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public Nullable<int> Current_Running_Number { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual Company Company { get; set; }
    }
}

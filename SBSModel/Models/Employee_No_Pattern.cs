using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Employee_No_Pattern
    {
        public int Employee_No_Pattern_ID { get; set; }
        public int Company_ID { get; set; }
        public bool Select_Nationality { get; set; }
        public bool Select_Year { get; set; }
        public bool Year_2_Digit { get; set; }
        public bool Year_4_Digit { get; set; }
        public bool Select_Company_code { get; set; }
        public string Company_Code { get; set; }
        public Nullable<int> Current_Running_Number { get; set; }
        public Nullable<bool> Select_Branch_Code { get; set; }
        public Nullable<int> Branch_ID { get; set; }
        public Nullable<bool> Initiated { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual Company Company { get; set; }
    }
}

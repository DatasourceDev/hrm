using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Expenses_Calculation
    {
        public int Calculation_ID { get; set; }
        public Nullable<int> Expenses_Config_ID { get; set; }
        public Nullable<int> Employee_Profile_ID { get; set; }
        public Nullable<decimal> Entitle { get; set; }
        public Nullable<decimal> Amount_Used { get; set; }
        public Nullable<System.DateTime> Expiry_Date { get; set; }
        public string Year_Assigned { get; set; }
        public Nullable<System.DateTime> Start_Date { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual Employee_Profile Employee_Profile { get; set; }
        public virtual Expenses_Config Expenses_Config { get; set; }
    }
}

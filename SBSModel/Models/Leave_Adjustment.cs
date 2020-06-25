using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Leave_Adjustment
    {
        public int Adjustment_ID { get; set; }
        public Nullable<int> Employee_Profile_ID { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public Nullable<decimal> Adjustment_Amount { get; set; }
        public string Reason { get; set; }
        public Nullable<int> Leave_Config_ID { get; set; }
        public Nullable<int> Year_2 { get; set; }
        public Nullable<int> Department_ID { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public string Record_Status { get; set; }
        public virtual Company Company { get; set; }
        public virtual Department Department { get; set; }
        public virtual Employee_Profile Employee_Profile { get; set; }
        public virtual Leave_Config Leave_Config { get; set; }
    }
}

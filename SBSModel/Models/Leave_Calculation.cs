using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Leave_Calculation
    {
        public int Calculation_ID { get; set; }
        public Nullable<int> Leave_Config_ID { get; set; }
        public Nullable<int> Employee_Profile_ID { get; set; }
        public Nullable<decimal> Entitle { get; set; }
        public Nullable<decimal> Adjustment { get; set; }
        public Nullable<decimal> Bring_Forward { get; set; }
        public Nullable<decimal> CutOff { get; set; }
        public Nullable<decimal> Leave_Used { get; set; }
        public Nullable<System.DateTime> Expiry_Date { get; set; }
        public string Year_Assigned { get; set; }
        public Nullable<System.DateTime> Start_Date { get; set; }
        public Nullable<int> Designation_ID { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public Nullable<int> Relationship_ID { get; set; }
        public virtual Designation Designation { get; set; }
        public virtual Employee_Profile Employee_Profile { get; set; }
        public virtual Relationship Relationship { get; set; }
        public virtual Leave_Config Leave_Config { get; set; }
    }
}

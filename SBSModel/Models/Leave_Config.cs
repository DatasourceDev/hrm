using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Leave_Config
    {
        public Leave_Config()
        {
            this.Leave_Adjustment = new List<Leave_Adjustment>();
            this.Leave_Application_Document = new List<Leave_Application_Document>();
            this.Leave_Calculation = new List<Leave_Calculation>();
            this.Leave_Config_Child_Detail = new List<Leave_Config_Child_Detail>();
            this.Leave_Config_Condition = new List<Leave_Config_Condition>();
            this.Leave_Config_Detail = new List<Leave_Config_Detail>();
            this.Leave_Config_Extra = new List<Leave_Config_Extra>();
            this.Leave_Config1 = new List<Leave_Config>();
        }

        public int Leave_Config_ID { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public string Leave_Name { get; set; }
        public string Leave_Description { get; set; }
        public Nullable<bool> Bring_Forward { get; set; }
        public Nullable<decimal> Bring_Forward_Percent { get; set; }
        public Nullable<bool> Upload_Document { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public Nullable<bool> Deduct_In_Payroll { get; set; }
        public Nullable<int> Months_To_Expiry { get; set; }
        public Nullable<bool> Allowed_Probation { get; set; }
        public Nullable<decimal> Bring_Forward_Days { get; set; }
        public Nullable<bool> Is_Bring_Forward_Days { get; set; }
        public Nullable<bool> Is_Default { get; set; }
        public Nullable<bool> Flexibly { get; set; }
        public Nullable<bool> Continuously { get; set; }
        public Nullable<int> Valid_Period { get; set; }
        public string Type { get; set; }
        public Nullable<bool> Allowed_Notice_Period { get; set; }
        public string Record_Status { get; set; }
        public Nullable<int> Leave_Config_Parent_ID { get; set; }
        public Nullable<bool> Is_Accumulative { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<Leave_Adjustment> Leave_Adjustment { get; set; }
        public virtual ICollection<Leave_Application_Document> Leave_Application_Document { get; set; }
        public virtual ICollection<Leave_Calculation> Leave_Calculation { get; set; }
        public virtual ICollection<Leave_Config_Child_Detail> Leave_Config_Child_Detail { get; set; }
        public virtual ICollection<Leave_Config_Condition> Leave_Config_Condition { get; set; }
        public virtual ICollection<Leave_Config_Detail> Leave_Config_Detail { get; set; }
        public virtual ICollection<Leave_Config_Extra> Leave_Config_Extra { get; set; }
        public virtual ICollection<Leave_Config> Leave_Config1 { get; set; }
        public virtual Leave_Config Leave_Config2 { get; set; }
    }
}

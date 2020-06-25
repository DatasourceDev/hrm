using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Leave_Default
    {
        public Leave_Default()
        {
            this.Leave_Default_Child_Detail = new List<Leave_Default_Child_Detail>();
            this.Leave_Default_Condition = new List<Leave_Default_Condition>();
            this.Leave_Default_Detail = new List<Leave_Default_Detail>();
        }

        public int Default_ID { get; set; }
        public string Leave_Name { get; set; }
        public string Leave_Description { get; set; }
        public Nullable<bool> Bring_Forward { get; set; }
        public Nullable<decimal> Bring_Forward_Percent { get; set; }
        public Nullable<bool> Upload_Document { get; set; }
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
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public string Record_Status { get; set; }
        public Nullable<bool> Is_Accumulative { get; set; }
        public virtual ICollection<Leave_Default_Child_Detail> Leave_Default_Child_Detail { get; set; }
        public virtual ICollection<Leave_Default_Condition> Leave_Default_Condition { get; set; }
        public virtual ICollection<Leave_Default_Detail> Leave_Default_Detail { get; set; }
    }
}

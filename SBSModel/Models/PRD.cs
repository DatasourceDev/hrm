using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class PRD
    {
        public int Payroll_Detail_ID { get; set; }
        public Nullable<int> PRM_ID { get; set; }
        public Nullable<int> PRT_ID { get; set; }
        public Nullable<int> PRC_ID { get; set; }
        public Nullable<int> Currency_ID { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Description { get; set; }
        public Nullable<decimal> Hours_Worked { get; set; }
        public Nullable<int> Employment_History_Allowance_ID { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual Employment_History_Allowance Employment_History_Allowance { get; set; }
        public virtual PRC PRC { get; set; }
        public virtual PRM PRM { get; set; }
        public virtual PRT PRT { get; set; }
    }
}

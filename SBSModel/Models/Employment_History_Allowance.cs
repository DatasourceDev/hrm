using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Employment_History_Allowance
    {
        public Employment_History_Allowance()
        {
            this.PRDs = new List<PRD>();
        }

        public int Employment_History_Allowance_ID { get; set; }
        public Nullable<int> History_ID { get; set; }
        public Nullable<int> PRT_ID { get; set; }
        public Nullable<int> PRC_ID { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual Employment_History Employment_History { get; set; }
        public virtual PRC PRC { get; set; }
        public virtual PRT PRT { get; set; }
        public virtual ICollection<PRD> PRDs { get; set; }
    }
}

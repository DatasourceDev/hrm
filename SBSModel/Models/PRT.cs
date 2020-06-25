using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class PRT
    {
        public PRT()
        {
            this.Employment_History_Allowance = new List<Employment_History_Allowance>();
            this.PRCs = new List<PRC>();
            this.PRDs = new List<PRD>();
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string Allowance_Type { get; set; }
        public string Type { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual ICollection<Employment_History_Allowance> Employment_History_Allowance { get; set; }
        public virtual ICollection<PRC> PRCs { get; set; }
        public virtual ICollection<PRD> PRDs { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class PRC
    {
        public PRC()
        {
            this.Employment_History_Allowance = new List<Employment_History_Allowance>();
            this.PRC_Department = new List<PRC_Department>();
            this.PRDs = new List<PRD>();
        }

        public int PRC_ID { get; set; }
        public int PRT_ID { get; set; }
        public int Company_ID { get; set; }
        public string Description { get; set; }
        public string Is_System { get; set; }
        public string Record_Status { get; set; }
        public Nullable<decimal> OT_Multiplier { get; set; }
        public Nullable<bool> CPF_Deductable { get; set; }
        public string Name { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<Employment_History_Allowance> Employment_History_Allowance { get; set; }
        public virtual ICollection<PRC_Department> PRC_Department { get; set; }
        public virtual PRT PRT { get; set; }
        public virtual ICollection<PRD> PRDs { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class PRG
    {
        public PRG()
        {
            this.PRALs = new List<PRAL>();
            this.PREDLs = new List<PREDL>();
            this.PRELs = new List<PREL>();
        }

        public int PRG_ID { get; set; }
        public int Company_ID { get; set; }
        public string Name { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public string Record_Status { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<PRAL> PRALs { get; set; }
        public virtual ICollection<PREDL> PREDLs { get; set; }
        public virtual ICollection<PREL> PRELs { get; set; }
    }
}

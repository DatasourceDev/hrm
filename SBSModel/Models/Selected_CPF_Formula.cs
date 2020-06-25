using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Selected_CPF_Formula
    {
        public Selected_CPF_Formula()
        {
            this.PRMs = new List<PRM>();
        }

        public int ID { get; set; }
        public int Company_ID { get; set; }
        public int CPF_Formula_ID { get; set; }
        public Nullable<System.DateTime> Effective_Date { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual Company Company { get; set; }
        public virtual CPF_Formula CPF_Formula { get; set; }
        public virtual ICollection<PRM> PRMs { get; set; }
    }
}

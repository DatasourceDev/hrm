using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Selected_Donation_Formula
    {
        public Selected_Donation_Formula()
        {
            this.PRMs = new List<PRM>();
        }

        public int ID { get; set; }
        public int Company_ID { get; set; }
        public int Donation_Formula_ID { get; set; }
        public Nullable<System.DateTime> Effective_Date { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual Company Company { get; set; }
        public virtual Donation_Formula Donation_Formula { get; set; }
        public virtual ICollection<PRM> PRMs { get; set; }
    }
}

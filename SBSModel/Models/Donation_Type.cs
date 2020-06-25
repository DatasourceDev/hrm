using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Donation_Type
    {
        public Donation_Type()
        {
            this.Donation_Formula = new List<Donation_Formula>();
        }

        public int Donation_Type_ID { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public string Donation_Name { get; set; }
        public string Donation_Description { get; set; }
        public string Record_Status { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<Donation_Formula> Donation_Formula { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Donation_Formula
    {
        public Donation_Formula()
        {
            this.Selected_Donation_Formula = new List<Selected_Donation_Formula>();
        }

        public int Donation_Formula_ID { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public Nullable<int> Donation_Type_ID { get; set; }
        public Nullable<int> Race { get; set; }
        public string Formula { get; set; }
        public string Formula_Name { get; set; }
        public string Formula_Desc { get; set; }
        public Nullable<int> Year { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public string Record_Status { get; set; }
        public virtual Company Company { get; set; }
        public virtual Donation_Type Donation_Type { get; set; }
        public virtual Global_Lookup_Data Global_Lookup_Data { get; set; }
        public virtual ICollection<Selected_Donation_Formula> Selected_Donation_Formula { get; set; }
    }
}

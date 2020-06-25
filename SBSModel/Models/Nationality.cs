using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Nationality
    {
        public Nationality()
        {
            this.Employee_Profile = new List<Employee_Profile>();
            this.IR8A_Nationality = new List<IR8A_Nationality>();
            this.Relationships = new List<Relationship>();
        }

        public int Nationality_ID { get; set; }
        public Nullable<int> Country_ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual Country Country { get; set; }
        public virtual ICollection<Employee_Profile> Employee_Profile { get; set; }
        public virtual ICollection<IR8A_Nationality> IR8A_Nationality { get; set; }
        public virtual ICollection<Relationship> Relationships { get; set; }
    }
}

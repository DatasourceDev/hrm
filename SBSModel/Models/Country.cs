using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Country
    {
        public Country()
        {
            this.Company_Details = new List<Company_Details>();
            this.Company_Details1 = new List<Company_Details>();
            this.Currencies = new List<Currency>();
            this.Customers = new List<Customer>();
            this.Employee_Profile = new List<Employee_Profile>();
            this.Employee_Profile1 = new List<Employee_Profile>();
            this.IR8A_Country = new List<IR8A_Country>();
            this.Nationalities = new List<Nationality>();
            this.States = new List<State>();
        }

        public int Country_ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual ICollection<Company_Details> Company_Details { get; set; }
        public virtual ICollection<Company_Details> Company_Details1 { get; set; }
        public virtual ICollection<Currency> Currencies { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
        public virtual ICollection<Employee_Profile> Employee_Profile { get; set; }
        public virtual ICollection<Employee_Profile> Employee_Profile1 { get; set; }
        public virtual ICollection<IR8A_Country> IR8A_Country { get; set; }
        public virtual ICollection<Nationality> Nationalities { get; set; }
        public virtual ICollection<State> States { get; set; }
    }
}

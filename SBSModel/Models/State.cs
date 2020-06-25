using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class State
    {
        public State()
        {
            this.Company_Details = new List<Company_Details>();
            this.Company_Details1 = new List<Company_Details>();
            this.Customers = new List<Customer>();
        }

        public int State_ID { get; set; }
        public int Country_ID { get; set; }
        public string Name { get; set; }
        public string Descrition { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual ICollection<Company_Details> Company_Details { get; set; }
        public virtual ICollection<Company_Details> Company_Details1 { get; set; }
        public virtual Country Country { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
    }
}

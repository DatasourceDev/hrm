using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Customer
    {
        public Customer()
        {
            this.Job_Cost = new List<Job_Cost>();
        }

        public int Customer_ID { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public string Customer_No { get; set; }
        public string Customer_Name { get; set; }
        public string Person_In_Charge { get; set; }
        public string Email { get; set; }
        public string Mobile_Phone { get; set; }
        public string Office_Phone { get; set; }
        public string Website { get; set; }
        public string Billing_Address { get; set; }
        public string Billing_Street { get; set; }
        public string Billing_City { get; set; }
        public Nullable<int> Billing_Country_ID { get; set; }
        public Nullable<int> Billing_State_ID { get; set; }
        public string Billing_Postal_Code { get; set; }
        public string Record_Status { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public string Fax { get; set; }
        public virtual Company Company { get; set; }
        public virtual Country Country { get; set; }
        public virtual State State { get; set; }
        public virtual ICollection<Job_Cost> Job_Cost { get; set; }
    }
}

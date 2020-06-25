using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Subscription
    {
        public Subscription()
        {
            this.User_Assign_Module = new List<User_Assign_Module>();
        }

        public int Subscription_ID { get; set; }
        public Nullable<int> Module_Detail_ID { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public Nullable<System.DateTime> Start_Date { get; set; }
        public Nullable<int> Subscription_Period { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<int> Period_Day { get; set; }
        public Nullable<int> No_Of_Users { get; set; }
        public Nullable<int> Period_Month { get; set; }
        public Nullable<int> Period_Year { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public string Module_Code { get; set; }
        public Nullable<decimal> Price { get; set; }
        public virtual Company Company { get; set; }
        public virtual Module Module { get; set; }
        public virtual SBS_Module_Detail SBS_Module_Detail { get; set; }
        public virtual ICollection<User_Assign_Module> User_Assign_Module { get; set; }
    }
}

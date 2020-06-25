using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Banking_Info
    {
        public Banking_Info()
        {
            this.IR8A_Bank = new List<IR8A_Bank>();
        }

        public int Banking_Info_ID { get; set; }
        public Nullable<int> Employee_Profile_ID { get; set; }
        public Nullable<int> Profile_ID { get; set; }
        public string Bank_Name { get; set; }
        public string Bank_Account { get; set; }
        public Nullable<int> Payment_Type { get; set; }
        public Nullable<System.DateTime> Effective_Date { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public string Update_By { get; set; }
        public string Account_Name { get; set; }
        public Nullable<bool> Selected { get; set; }
        public virtual Employee_Profile Employee_Profile { get; set; }
        public virtual Global_Lookup_Data Global_Lookup_Data { get; set; }
        public virtual User_Profile User_Profile { get; set; }
        public virtual ICollection<IR8A_Bank> IR8A_Bank { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Employee_Emergency_Contact
    {
        public int Employee_Emergency_Contact_ID { get; set; }
        public Nullable<int> Employee_Profile_ID { get; set; }
        public string Name { get; set; }
        public string Contact_No { get; set; }
        public Nullable<int> Relationship { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual Employee_Profile Employee_Profile { get; set; }
        public virtual Global_Lookup_Data Global_Lookup_Data { get; set; }
    }
}

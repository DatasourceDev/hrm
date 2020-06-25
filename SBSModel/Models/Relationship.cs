using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Relationship
    {
        public Relationship()
        {
            this.Leave_Application_Document = new List<Leave_Application_Document>();
            this.Leave_Calculation = new List<Leave_Calculation>();
        }

        public int Relationship_ID { get; set; }
        public Nullable<int> Employee_Profile_ID { get; set; }
        public Nullable<int> Profile_ID { get; set; }
        public Nullable<int> Nationality_ID { get; set; }
        public string Name { get; set; }
        public Nullable<int> Relationship1 { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public Nullable<int> Gender { get; set; }
        public string NRIC { get; set; }
        public Nullable<bool> Working { get; set; }
        public string Company_Name { get; set; }
        public string Company_Position { get; set; }
        public string Passport { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public string Update_By { get; set; }
        public string Child_Type { get; set; }
        public Nullable<bool> Is_Maternity { get; set; }
        public Nullable<bool> Is_Maternity_Share_Father { get; set; }
        public Nullable<bool> Is_Paternity { get; set; }
        public virtual Employee_Profile Employee_Profile { get; set; }
        public virtual Global_Lookup_Data Global_Lookup_Data { get; set; }
        public virtual Global_Lookup_Data Global_Lookup_Data1 { get; set; }
        public virtual ICollection<Leave_Application_Document> Leave_Application_Document { get; set; }
        public virtual ICollection<Leave_Calculation> Leave_Calculation { get; set; }
        public virtual Nationality Nationality { get; set; }
        public virtual User_Profile User_Profile { get; set; }
    }
}

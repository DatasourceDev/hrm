using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class User_Profile
    {
        public User_Profile()
        {
            this.Banking_Info = new List<Banking_Info>();
            this.Employee_Profile = new List<Employee_Profile>();
            this.ETIRA8 = new List<ETIRA8>();
            this.Relationships = new List<Relationship>();
            this.Screen_Capture_Log = new List<Screen_Capture_Log>();
            this.User_Assign_Module = new List<User_Assign_Module>();
            this.User_Profile_Photo = new List<User_Profile_Photo>();
            this.User_Session_Data = new List<User_Session_Data>();
            this.User_Transactions = new List<User_Transactions>();
        }

        public int Profile_ID { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public Nullable<int> User_Authentication_ID { get; set; }
        public string Name { get; set; }
        public Nullable<System.DateTime> Registration_Date { get; set; }
        public string User_Status { get; set; }
        public Nullable<System.DateTime> Latest_Connection { get; set; }
        public string First_Name { get; set; }
        public string Middle_Name { get; set; }
        public string Last_Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Bg { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public string A7_User_ID { get; set; }
        public string User_Name { get; set; }
        public Nullable<bool> Is_Email { get; set; }
        public Nullable<bool> Is_Tour_Skip { get; set; }
        public virtual ICollection<Banking_Info> Banking_Info { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<Employee_Profile> Employee_Profile { get; set; }
        public virtual ICollection<ETIRA8> ETIRA8 { get; set; }
        public virtual ICollection<Relationship> Relationships { get; set; }
        public virtual ICollection<Screen_Capture_Log> Screen_Capture_Log { get; set; }
        public virtual ICollection<User_Assign_Module> User_Assign_Module { get; set; }
        public virtual User_Authentication User_Authentication { get; set; }
        public virtual ICollection<User_Profile_Photo> User_Profile_Photo { get; set; }
        public virtual ICollection<User_Session_Data> User_Session_Data { get; set; }
        public virtual ICollection<User_Transactions> User_Transactions { get; set; }
    }
}

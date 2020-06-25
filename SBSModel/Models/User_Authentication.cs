using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class User_Authentication
    {
        public User_Authentication()
        {
            this.Activation_Link = new List<Activation_Link>();
            this.User_Assign_Role = new List<User_Assign_Role>();
            this.User_Profile = new List<User_Profile>();
        }

        public int User_Authentication_ID { get; set; }
        public string Email_Address { get; set; }
        public string PWD { get; set; }
        public int Login_Attempt { get; set; }
        public bool Activated { get; set; }
        public string ApplicationUser_Id { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public string User_Name { get; set; }
        public Nullable<bool> Is_Email { get; set; }
        public virtual ICollection<Activation_Link> Activation_Link { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<User_Assign_Role> User_Assign_Role { get; set; }
        public virtual ICollection<User_Profile> User_Profile { get; set; }
    }
}

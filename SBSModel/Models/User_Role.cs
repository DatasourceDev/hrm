using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class User_Role
    {
        public User_Role()
        {
            this.Page_Role = new List<Page_Role>();
            this.User_Assign_Role = new List<User_Assign_Role>();
        }

        public int User_Role_ID { get; set; }
        public string Role_Name { get; set; }
        public string Role_Description { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual ICollection<Page_Role> Page_Role { get; set; }
        public virtual ICollection<User_Assign_Role> User_Assign_Role { get; set; }
    }
}

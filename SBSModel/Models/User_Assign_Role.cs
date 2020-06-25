using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class User_Assign_Role
    {
        public int User_Assign_Role_ID { get; set; }
        public Nullable<int> User_Role_ID { get; set; }
        public Nullable<int> User_Authentication_ID { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual User_Authentication User_Authentication { get; set; }
        public virtual User_Role User_Role { get; set; }
    }
}

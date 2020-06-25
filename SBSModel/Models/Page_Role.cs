using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Page_Role
    {
        public Page_Role()
        {
            this.Access_Page = new List<Access_Page>();
        }

        public int Page_Role_ID { get; set; }
        public int User_Role_ID { get; set; }
        public Nullable<int> Page_ID { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public string Menu_Page_Code { get; set; }
        public virtual ICollection<Access_Page> Access_Page { get; set; }
        public virtual Menu_Page Menu_Page { get; set; }
        public virtual Page Page { get; set; }
        public virtual User_Role User_Role { get; set; }
    }
}

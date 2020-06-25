using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Menu_Page
    {
        public Menu_Page()
        {
            this.Page_Role = new List<Page_Role>();
        }

        public string Menu_Page_Code { get; set; }
        public string Menu_Code { get; set; }
        public string Menu_Page_Name { get; set; }
        public string Action_Link { get; set; }
        public string Keyword { get; set; }
        public string Page_Action { get; set; }
        public string Page_Controller { get; set; }
        public string Domain_Name { get; set; }
        public string operation { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual Menu Menu { get; set; }
        public virtual ICollection<Page_Role> Page_Role { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Menu
    {
        public Menu()
        {
            this.Menu_Page = new List<Menu_Page>();
        }

        public string Menu_Code { get; set; }
        public string Menu_Name { get; set; }
        public string Location { get; set; }
        public string Action_Link { get; set; }
        public string Keyword { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual ICollection<Menu_Page> Menu_Page { get; set; }
    }
}

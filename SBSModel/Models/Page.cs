using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Page
    {
        public Page()
        {
            this.Page_Role = new List<Page_Role>();
        }

        public int Page_ID { get; set; }
        public string Page_Url { get; set; }
        public Nullable<int> Module_Detail_ID { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public string Page_Name { get; set; }
        public Nullable<int> Page_Attempt { get; set; }
        public string Position { get; set; }
        public Nullable<bool> Displayed { get; set; }
        public Nullable<int> Order_Index { get; set; }
        public Nullable<bool> Is_Indent { get; set; }
        public virtual SBS_Module_Detail SBS_Module_Detail { get; set; }
        public virtual ICollection<Page_Role> Page_Role { get; set; }
    }
}

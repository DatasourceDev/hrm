using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Access_Page
    {
        public int Access_Page_ID { get; set; }
        public int Access_ID { get; set; }
        public int Page_Role_ID { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual Access_Right Access_Right { get; set; }
        public virtual Page_Role Page_Role { get; set; }
    }
}

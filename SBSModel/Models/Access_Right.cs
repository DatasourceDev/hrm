using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Access_Right
    {
        public Access_Right()
        {
            this.Access_Page = new List<Access_Page>();
        }

        public int Access_ID { get; set; }
        public string Access_Name { get; set; }
        public string Access_Description { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual ICollection<Access_Page> Access_Page { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Company_Logo
    {
        public System.Guid Company_Logo_ID { get; set; }
        public byte[] Logo { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public virtual Company Company { get; set; }
    }
}

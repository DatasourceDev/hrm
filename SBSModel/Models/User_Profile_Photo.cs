using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class User_Profile_Photo
    {
        public System.Guid User_Profile_Photo_ID { get; set; }
        public byte[] Photo { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public Nullable<int> Profile_ID { get; set; }
        public virtual User_Profile User_Profile { get; set; }
    }
}

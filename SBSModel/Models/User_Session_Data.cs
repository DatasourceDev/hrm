using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class User_Session_Data
    {
        public int User_Session_ID { get; set; }
        public string Session_ID { get; set; }
        public Nullable<int> Profile_ID { get; set; }
        public string Session_Data { get; set; }
        public Nullable<bool> Actived { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual User_Profile User_Profile { get; set; }
    }
}

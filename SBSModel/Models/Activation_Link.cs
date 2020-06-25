using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Activation_Link
    {
        public int Activation_ID { get; set; }
        public int User_Authentication_ID { get; set; }
        public string Activation_Code { get; set; }
        public System.DateTime Time_Limit { get; set; }
        public Nullable<bool> Active { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual User_Authentication User_Authentication { get; set; }
    }
}

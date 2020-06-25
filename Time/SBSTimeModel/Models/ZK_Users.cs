using System;
using System.Collections.Generic;

namespace SBSTimeModel.Models
{
    public partial class ZK_Users
    {
        public int Enroll_ID { get; set; }
        public string User_Name { get; set; }
        public string User_Level { get; set; }
        public string User_Status { get; set; }
        public int User_Pin { get; set; }
        public int Device_ID { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public string Update_By { get; set; }
        public int ZK_Users_ID { get; set; }
    }
}

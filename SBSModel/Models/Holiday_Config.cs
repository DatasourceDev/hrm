using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Holiday_Config
    {
        public int Holiday_ID { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public string Name { get; set; }
        public Nullable<int> Holiday_Lenght { get; set; }
        public Nullable<System.DateTime> Start_Date { get; set; }
        public Nullable<System.DateTime> End_Date { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public string Record_Status { get; set; }
        public virtual Company Company { get; set; }
        public virtual Global_Lookup_Data Global_Lookup_Data { get; set; }
    }
}

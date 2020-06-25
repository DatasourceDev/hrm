using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Global_Lookup_Def
    {
        public Global_Lookup_Def()
        {
            this.Global_Lookup_Data = new List<Global_Lookup_Data>();
        }

        public int Def_ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Record_Status { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public Nullable<bool> Syn_Offline { get; set; }
        public virtual ICollection<Global_Lookup_Data> Global_Lookup_Data { get; set; }
    }
}

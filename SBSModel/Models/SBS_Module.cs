using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class SBS_Module
    {
        public SBS_Module()
        {
            this.SBS_Module_Detail = new List<SBS_Module_Detail>();
        }

        public int Module_ID { get; set; }
        public string Module_Name { get; set; }
        public string Module_Description { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual ICollection<SBS_Module_Detail> SBS_Module_Detail { get; set; }
    }
}

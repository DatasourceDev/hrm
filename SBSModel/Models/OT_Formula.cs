using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class OT_Formula
    {
        public OT_Formula()
        {
            this.Selected_OT_Formula = new List<Selected_OT_Formula>();
        }

        public int OT_Formula_ID { get; set; }
        public string Formula { get; set; }
        public string Formula_Name { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual ICollection<Selected_OT_Formula> Selected_OT_Formula { get; set; }
    }
}

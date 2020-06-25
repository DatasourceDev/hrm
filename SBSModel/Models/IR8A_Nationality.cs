using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class IR8A_Nationality
    {
        public int Nationality_Code { get; set; }
        public string Description { get; set; }
        public Nullable<int> SBS_Nationality_ID { get; set; }
        public virtual Nationality Nationality { get; set; }
    }
}

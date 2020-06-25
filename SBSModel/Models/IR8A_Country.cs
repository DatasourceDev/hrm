using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class IR8A_Country
    {
        public int Country_Code { get; set; }
        public string Description { get; set; }
        public Nullable<int> SBS_Country_ID { get; set; }
        public virtual Country Country { get; set; }
    }
}

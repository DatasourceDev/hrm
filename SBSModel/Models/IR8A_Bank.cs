using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class IR8A_Bank
    {
        public string Bank_Code { get; set; }
        public string Description { get; set; }
        public Nullable<int> SBS_Banking_Info_ID { get; set; }
        public virtual Banking_Info Banking_Info { get; set; }
    }
}

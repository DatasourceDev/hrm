using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Invoice_Details
    {
        public int Invoice_Detail_ID { get; set; }
        public int Invoice_ID { get; set; }
        public Nullable<int> Module_ID { get; set; }
        public Nullable<int> AddOn_ID { get; set; }
        public virtual Invoice_Header Invoice_Header { get; set; }
    }
}

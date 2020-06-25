using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class vwQuantityOnHand
    {
        public Nullable<int> Company_ID { get; set; }
        public int Product_Category_ID { get; set; }
        public string Category_Name { get; set; }
        public int Product_ID { get; set; }
        public string Product_Code { get; set; }
        public string Product_Name { get; set; }
        public Nullable<int> InvIN { get; set; }
        public Nullable<int> InvOUT { get; set; }
        public Nullable<int> POSOUT { get; set; }
        public Nullable<int> QoH { get; set; }
    }
}

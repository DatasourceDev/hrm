using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Extra_Product_Info
    {
        public int Extra_Product_Info_ID { get; set; }
        public Nullable<int> Product_ID { get; set; }
        public string Barcode { get; set; }
        public string Reorder_Point { get; set; }
        public Nullable<int> Reorder_Quantity { get; set; }
        public Nullable<int> Default_Location { get; set; }
        public string Description { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
    }
}

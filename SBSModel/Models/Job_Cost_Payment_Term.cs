using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Job_Cost_Payment_Term
    {
        public int Job_Cost_PayMent_Term_ID { get; set; }
        public Nullable<int> Job_Cost_ID { get; set; }
        public Nullable<decimal> Payment { get; set; }
        public string Payment_Type { get; set; }
        public string Invoice_No { get; set; }
        public Nullable<System.DateTime> Invoice_Date { get; set; }
        public string Note { get; set; }
        public Nullable<decimal> Actual_Price { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual Job_Cost Job_Cost { get; set; }
    }
}

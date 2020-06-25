using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Invoice_Header
    {
        public Invoice_Header()
        {
            this.Invoice_Details = new List<Invoice_Details>();
        }

        public int Invoice_ID { get; set; }
        public int Company_ID { get; set; }
        public string Invoice_No { get; set; }
        public Nullable<decimal> Due_Amount { get; set; }
        public Nullable<int> Invoice_Month { get; set; }
        public Nullable<int> Invoice_Year { get; set; }
        public Nullable<System.DateTime> Generated_On { get; set; }
        public string Invoice_Status { get; set; }
        public Nullable<System.DateTime> Paid_On { get; set; }
        public string Paid_By { get; set; }
        public string Invoice_To { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<Invoice_Details> Invoice_Details { get; set; }
    }
}

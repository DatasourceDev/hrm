using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class PRDL
    {
        public int PRDL_ID { get; set; }
        public int PRM_ID { get; set; }
        public int Leave_Application_Document_ID { get; set; }
        public decimal Process_Day { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<System.DateTime> Start_Date { get; set; }
        public Nullable<System.DateTime> End_Date { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual Leave_Application_Document Leave_Application_Document { get; set; }
        public virtual PRM PRM { get; set; }
    }
}

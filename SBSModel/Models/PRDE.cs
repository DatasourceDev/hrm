using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class PRDE
    {
        public int PRDE_ID { get; set; }
        public int PRM_ID { get; set; }
        public int Expenses_Application_Document_ID { get; set; }
        public Nullable<System.DateTime> Expenses_Date { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual Expenses_Application_Document Expenses_Application_Document { get; set; }
        public virtual PRM PRM { get; set; }
    }
}

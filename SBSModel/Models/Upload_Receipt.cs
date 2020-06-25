using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Upload_Receipt
    {
        public System.Guid Upload_Receipt_ID { get; set; }
        public byte[] Receipt { get; set; }
        public string File_Name { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public Nullable<int> Expenses_Application_Document_ID { get; set; }
        public virtual Expenses_Application_Document Expenses_Application_Document { get; set; }
    }
}

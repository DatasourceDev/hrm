using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Upload_Document
    {
        public System.Guid Upload_Document_ID { get; set; }
        public byte[] Document { get; set; }
        public string File_Name { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public Nullable<int> Leave_Application_Document_ID { get; set; }
        public virtual Leave_Application_Document Leave_Application_Document { get; set; }
    }
}

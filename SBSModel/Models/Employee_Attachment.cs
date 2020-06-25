using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Employee_Attachment
    {
        public System.Guid Attachment_ID { get; set; }
        public byte[] Attachment_File { get; set; }
        public Nullable<int> Employee_Profile_ID { get; set; }
        public Nullable<int> Attachment_Type { get; set; }
        public string File_Name { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Uploaded_On { get; set; }
        public string Uploaded_by { get; set; }
        public virtual Global_Lookup_Data Global_Lookup_Data { get; set; }
        public virtual Employee_Profile Employee_Profile { get; set; }
    }
}

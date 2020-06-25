using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Email_Attachment
    {
        public System.Guid Email_Attachment_ID { get; set; }
        public Nullable<int> Email_ID { get; set; }
        public string Attachment_File_Name { get; set; }
        public byte[] Attachment_File { get; set; }
        public virtual Email_Notification Email_Notification { get; set; }
    }
}

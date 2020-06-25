using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Email_Notification
    {
        public Email_Notification()
        {
            this.Email_Attachment = new List<Email_Attachment>();
        }

        public int Email_ID { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string Exception_Message { get; set; }
        public string Status { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public int? Doc_ID { get; set; }
        public string Module { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<Email_Attachment> Email_Attachment { get; set; }
    }
}

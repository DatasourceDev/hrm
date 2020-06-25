using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Company_Mail_Config
    {
        public int Config_ID { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public string SMTP_Server { get; set; }
        public Nullable<int> SMTP_Port { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Nullable<bool> SSL { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Subscriber
    {
        public int Subscriber_ID { get; set; }
        public string Subscriber_Email { get; set; }
        public string Subscriber_Status { get; set; }
        public Nullable<System.DateTime> Subscribed_On { get; set; }
    }
}

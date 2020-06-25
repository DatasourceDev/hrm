using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Bank_Details
    {
        public int Bank_Detail_ID { get; set; }
        public int Company_ID { get; set; }
        public string Bank_Account_Number { get; set; }
        public string Bank_Account_Owner { get; set; }
        public string Bank_Name { get; set; }
        public Nullable<bool> Display_On_Reports { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual Company Company { get; set; }
    }
}

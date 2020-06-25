using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using HR.Common;
using HR.Models;
using SBSModel.Models;
using SBSModel.Common;
using SBSResourceAPI;


namespace HR.Models
{

    public class BankDetailViewModel
    {
        public List<Bank_Details> BankDetailList;
        public Nullable<int> Company_ID { get; set; }
        public Nullable<int> Bank_Detail_ID { get; set; }

        [LocalizedRequired]
        [LocalizedValidMaxLength(30)]
        [LocalizedDisplayName("Account_Number", typeof(Resource))]
        public string Bank_Account_Number { get; set; }

        [LocalizedRequired]
        [LocalizedValidMaxLength(150)]
        [LocalizedDisplayName("Account_Owner", typeof(Resource))]
        public string Bank_Account_Owner { get; set; }

        [LocalizedRequired]
        [LocalizedValidMaxLength(150)]
        [LocalizedDisplayName("Bank_Name", typeof(Resource))]
        public string Bank_Name { get; set; }

        [LocalizedDisplayName("Display_On_Reports", typeof(Resource))]
        public bool Display_On_Reports { get; set; }
    }
}


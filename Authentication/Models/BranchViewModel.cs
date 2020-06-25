using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Authentication.Common;
using Authentication.Models;
using SBSModel.Models;
using SBSModel.Common;
using SBSResourceAPI;

namespace Authentication.Models
{
    public class BranchViewModel
    {
        public List<Branch> BranchList;
        public Nullable<int> Company_ID { get; set; }
        public Nullable<int> Branch_ID { get; set; }

        [LocalizedRequired]
        [LocalizedValidMaxLength(50)]
        [LocalizedDisplayName("Branch_Code", typeof(Resource))]
        public string Branch_Code { get; set; }

        [LocalizedRequired]
        [LocalizedValidMaxLength(300)]
        [LocalizedDisplayName("Branch_Name", typeof(Resource))]
        public string Branch_Name { get; set; }

        [LocalizedRequired]
        [LocalizedValidMaxLength(500)]
        [LocalizedDisplayName("Branch_Desc", typeof(Resource))]
        public string Branch_Desc { get; set; }
    }
}


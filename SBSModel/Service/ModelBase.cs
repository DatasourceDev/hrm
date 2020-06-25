using SBSModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SBSModel.Models;

namespace SBSModel.Models
{
    public class ModelBase
    {

        public ServiceResult result { get; set; }

        public string[] rights { get; set; }

        public string operation { get; set; }

        public string apply { get; set; }

        public string tabAction { get; set; }

        [LocalizedDisplayName("Create By")]
        public string Create_By { get; set; }

        [LocalizedDisplayName("Create On")]
        public DateTime Create_On { get; set; }

        [LocalizedDisplayName("Update By")]
        public string Update_By { get; set; }

        [LocalizedDisplayName("Update On")]
        public DateTime Update_On { get; set; }


        /*for help many data loading*/
        public int Page_No { get; set; }
        public int Page_Length { get; set; }
        public int Record_Count { get; set; }
        public string sortingBy { get; set; }

    }

    public class ModelMobileBase    {
        [LocalizedDisplayName("Update On")]
        public string Update_On { get; set; }
    }

}
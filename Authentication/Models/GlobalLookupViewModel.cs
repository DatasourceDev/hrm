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
   public class GlobalLookupViewModel : ModelBase
   {

      public List<Global_Lookup_Data> GlobalLookupDataList { get; set; }
      public List<ComboViewModel> GLookupDeflst { get; set; }
      public List<ComboViewModel> Statuslst { get; set; }

      //Muti select
      public int[] LookupIDs;
      public Nullable<int> Def_ID { get; set; }
      
      public string Lookup_Def_Name { get; set; }
      public int Lookup_Data_ID { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(100)]
      [LocalizedDisplayName("Name", typeof(Resource))]
      public string Name { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(500)]
      [LocalizedDisplayName("Description", typeof(Resource))]
      public string Description { get; set; }

      [LocalizedRequired]
      [LocalizedDisplayName("Status", typeof(Resource))]
      public string Record_Status { get; set; }

   }
}
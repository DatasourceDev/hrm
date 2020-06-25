using SBSModel.Common;
using SBSModel.Models;
using SBSResourceAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace HR.Models
{

   public class ConfigViewModel : ModelBase
   {
      public List<ComboViewModel> LookupTypeList;
      public List<Global_Lookup_Data> GlobalLookupInfoList { get; set; }
      public List<int> config_rights;
      public Nullable<int> Company_ID { get; set; }
      public Nullable<int> Lookup_Data_ID { get; set; }
      public bool IsCompanyConfig { get; set; }

      [LocalizedDisplayName("Lookup_By_Definition", typeof(Resource))]
      public Nullable<int> Def_ID { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Name", typeof(Resource))]
      public string Name { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Detail", typeof(Resource))]
      public string Description { get; set; }

      [LocalizedDisplayName("Status", typeof(Resource))]
      public string Record_Status { get; set; }
   }

   //--------------------------------------------//
   //Added by sun
   public class UserRoleInfoViewModel : ModelBase
   {
      public int User_Role_ID { get; set; }
      public string tabAction { get; set; }
      public string pageAction { get; set; }
      public List<string> adminRights { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Role_Name", typeof(Resource))]
      public string Role_Name { get; set; }

      [AllowHtml]
      [LocalizedRequired]
      [LocalizedValidMaxLength(1000)]
      [LocalizedDisplayName("Role_Desc", typeof(Resource))]
      public string Role_Description { get; set; }
   }

   public class PageRoleViewModel : ModelBase
   {
      public Nullable<int> Page_Role_ID { get; set; }
      public Nullable<int> Company_ID { get; set; }
      public Nullable<int> Module_Detail_ID { get; set; }
      public List<ComboViewModel> ModuleDetailLst { get; set; }
      public List<ComboViewModel> UserRoleLst { get; set; }
      public List<ComboViewModel> PageUrlLst { get; set; }
      public List<ComboViewModel> AccessRightLst { get; set; } //  A C U D

      // page role list
      public PageRoleDetailViewModel[] Page_Role_Rows { get; set; }
   }

   public class PageRoleDetailViewModel : ModelBase
   {
      public int Index { get; set; }
      public string Row_Type { get; set; }
      public Nullable<int> Page_Role_ID { get; set; }
      public Nullable<int> User_Role_ID { get; set; } //User_Role
      public Nullable<int> Page_ID { get; set; } //Page
      public List<ComboViewModel> UserRoleLst { get; set; }
      public List<ComboViewModel> PageUrlLst { get; set; }
      public List<ComboViewModel> AccessRightLst { get; set; } //A C U D
      public int[] Access_Page_Rows { get; set; } // selected A C U D
   }

   public class ConfigurationViewModel : ModelBase
   {
      public string tabAction { get; set; }
      public List<SBS_Module_Detail> ModuleDetailLst { get; set; }
      public List<User_Role> UserRoleList { get; set; }
      public Nullable<int> Company_ID { get; set; }
      public List<Page> PageLst { get; set; }
   }

}
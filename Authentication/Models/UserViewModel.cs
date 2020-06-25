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
    public class UserViewModel : ModelBase
    {
        public List<Company_Details> CompanyList;
        public string search_val { get; set; }
        public string Record_Status { get; set; }
        public List<User_Profile> UserList;
        public List<ComboViewModel> statusList { get; set; }
        public virtual User_Profile User_Profile { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public string Company_Level { get; set; }
        public Nullable<int> Profile_ID { get; set; }
        public Nullable<int> User_Authentication_ID { get; set; }
        public bool Activated { get; set; }
        
        public List<User_Role> UserRoleList;

        public List<Subscription> SubscriptionList;

        public int[] Users_Assign_Role;
        public int[] User_Assign_Module;
        public List<string> adminRights { get; set; }
        public List<string> hrRights { get; set; }
        public Nullable<int> User_Login_Profile_ID { get; set; }
        public string pageAction { get; set; }

        //------------------------SBSResourceAPI-------------------------//

        [LocalizedRequired]
        [LocalizedValidMaxLength(150)]
        [LocalizedDisplayName("First_Name", typeof(Resource))]
        public string First_Name { get; set; }

        [LocalizedValidMaxLength(150)]
        [LocalizedDisplayName("Middle_Name", typeof(Resource))]
        public string Middle_Name { get; set; }

        [LocalizedRequired]
        [LocalizedValidMaxLength(150)]
        [LocalizedDisplayName("Last_Name", typeof(Resource))]
        public string Last_Name { get; set; }

        [LocalizedValidMaxLength(50)]
        [LocalizedValidationEmail]
        [DataType(DataType.EmailAddress)]
        [LocalizedDisplayName("Email", typeof(Resource))]
        public string Email { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayName("Status", typeof(Resource))]
        public string User_Status { get; set; }

        [LocalizedValidMaxLength(150)]
        [LocalizedDisplayName("Phone", typeof(Resource))]
        public string Phone { get; set; }

        [LocalizedValidMaxLength(10)]
        [LocalizedValidationUserName]
        [LocalizedDisplayName("User_Name", typeof(Resource))]
        public string User_Name { get; set; }

        //Added by sun 09-09-2016
        [LocalizedRequired]
        [LocalizedDisplayName("Preferred_User_Id_Method", typeof(Resource))]
        public bool Is_Email { get; set; }
    }
}
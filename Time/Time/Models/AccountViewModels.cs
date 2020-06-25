using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

using System.Web.UI.WebControls;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Data.Entity;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

using Time.Models;
using SBSModel.Models;
using SBSModel.Common;
using SBSResourceAPI;



namespace Time.Models
{

    public class LoginViewModel
    {
        [LocalizedRequired]
        [LocalizedDisplayName("Email", typeof(Resource))]
        [LocalizedValidMaxLength(50)]
        public string Email { get; set; }

        [LocalizedRequired]
        [DataType(DataType.Password)]
        [LocalizedDisplayName("Password", typeof(Resource))]
        [LocalizedValidMaxLength(24)]
        public string Password { get; set; }

        public string ApplicationUser_Id { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayName("Remember_Me", typeof(Resource))]
        public bool RememberMe { get; set; }

        public String message { get; set; }
    }

    public class ResetPasswordViewModel
    {
        public int uid { get; set; }
        public bool notValidateCurrent { get; set; }
        public String name { get; set; }


        [DataType(DataType.Password)]
        [LocalizedDisplayName("Current_Password", typeof(Resource))]
        public string OldPassword { get; set; }

        [LocalizedRequired]
        [StringLength(24, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [LocalizedDisplayName("New_Password", typeof(Resource))]
        public string NewPassword { get; set; }

        [LocalizedRequired]
        [DataType(DataType.Password)]
        [LocalizedDisplayName("Confirm_New_Password", typeof(Resource))]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ErrorPageViewModel
    {
        public String URL { get; set; }
        public int code { get; set; }
        public string feild { get; set; }
        public String msg { get; set; }
    }

    public class MessagePageViewModel
    {
        public String Field { get; set; }
        public int Code { get; set; }
        public String Msg { get; set; }
    }

   
}

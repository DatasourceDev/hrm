using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using POS.Common;
using System.Web.UI.WebControls;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using POS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using SBSModel.Common;
using SBSModel.Models;

namespace POS.Models
{
    public class LoginViewModel
    {
        [Required]
        [LocalizedDisplayName("Email", typeof(Resources.ResourceAccount))]
        [MaxLength(50)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [LocalizedDisplayName("Password", typeof(Resources.ResourceAccount))]
        [MaxLength(24)]
        public string Password { get; set; }

        public string ApplicationUser_Id { get; set; }

        [Required]
        [LocalizedDisplayNameAttribute("Remember me?")]
        public bool RememberMe { get; set; }

        public String message { get; set; }

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

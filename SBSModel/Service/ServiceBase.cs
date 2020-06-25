using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBSModel.Models
{
    public class ServiceBase
    {
        protected string userloginName = "admin";
        public ServiceBase(User_Profile _userlogin)
        {
            if (_userlogin != null && _userlogin.User_Authentication != null)
                userloginName = _userlogin.User_Authentication.Email_Address;
        }
        public ServiceBase()
        {
        }
    }
}

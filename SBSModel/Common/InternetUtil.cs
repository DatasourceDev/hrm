using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SBSModel.Common
{
    public class InternetUtil
    {
        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead("http://www.google.com"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        public static bool CheckForServerConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead(AppSetting.SERVER_NAME))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Web;

namespace POS.Common
{
    public class UrlUtil
    {
        public static string GetDomain(HttpRequestBase request, string modulename = "")
        {
            if (request != null)
            {
                return request.Url.Scheme + "://" + request.Url.Host + "/" + modulename + "/";
            }
            return "";
        }

        public static string GetDomain(HttpRequestBase request)
        {
            if (request != null)
            {
                return request.Url.Scheme + "://" + request.Url.Host + "/";
            }
            return "";
           
        }

        
    }
}
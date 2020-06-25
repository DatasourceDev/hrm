using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;


namespace SBSModel.Common
{
   public class UrlUtil
   {
      public static string Action(UrlHelper Url, string actionName, string controllerName)
      {
         var domain = ModuleDomain.GetModuleDomain(actionName, controllerName);
         if (!string.IsNullOrEmpty(domain))
         {
            var urlstr = Url.Action(actionName, controllerName);
            urlstr = urlstr.Replace(ModuleDomain.Authentication, domain);
            urlstr = urlstr.Replace(ModuleDomain.HR, domain);
            urlstr = urlstr.Replace(ModuleDomain.Inventory, domain);
            urlstr = urlstr.Replace(ModuleDomain.POS, domain);
            urlstr = urlstr.Replace(ModuleDomain.CRM, domain);
            urlstr = urlstr.Replace(ModuleDomain.Time, domain);
            return urlstr;
         }
         return Url.Action(actionName, controllerName);
      }

      public static string Action(UrlHelper Url, string actionName, string controllerName, object routeValues)
      {
         var domain = ModuleDomain.GetModuleDomain(actionName, controllerName);
         if (!string.IsNullOrEmpty(domain))
         {
            var urlstr = Url.Action(actionName, controllerName, routeValues);
            urlstr = urlstr.Replace(ModuleDomain.Authentication, domain);
            urlstr = urlstr.Replace(ModuleDomain.HR, domain);
            urlstr = urlstr.Replace(ModuleDomain.Inventory, domain);
            urlstr = urlstr.Replace(ModuleDomain.POS, domain);
            urlstr = urlstr.Replace(ModuleDomain.CRM, domain);
            urlstr = urlstr.Replace(ModuleDomain.Time, domain);
            return urlstr;
         }
         return Url.Action(actionName, controllerName, routeValues);
      }

      public static string Action2(UrlHelper Url, string actionName, string controllerName, RouteValueDictionary routeValues)
      {
         var domain = ModuleDomain.GetModuleDomain(actionName, controllerName);
         if (!string.IsNullOrEmpty(domain))
         {
            var urlstr = Url.Action(actionName, controllerName, routeValues);
            urlstr = urlstr.Replace(ModuleDomain.Authentication, domain);
            urlstr = urlstr.Replace(ModuleDomain.HR, domain);
            urlstr = urlstr.Replace(ModuleDomain.Inventory, domain);
            urlstr = urlstr.Replace(ModuleDomain.POS, domain);
            urlstr = urlstr.Replace(ModuleDomain.CRM, domain);
            return urlstr;
         }
         return Url.Action(actionName, controllerName, routeValues);
      }

      private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      public static string GetDomain(Uri Url, string modulename = "")
      {
         log4net.Config.XmlConfigurator.Configure();
         log = log4net.LogManager.GetLogger(typeof(UrlUtil));
         var urlstr = "";
         try
         {

            if (Url.Host.ToLower() == "localhost")
            {
               urlstr = Url.Scheme + "://" + Url.Authority + "/";
            }
            else
            {
               urlstr = Url.Scheme + "://" + Url.Host + "/" ;
               log.Debug(urlstr);
            }
         }
         catch (Exception ex)
         {
            log.Error(DateTime.Now, ex);
            Debug.WriteLine(ex.Message);
         }


         return urlstr;
      }

      public static string GetDomain(Uri Url)
      {
         var urlstr = "";
         if (Url.Host.ToLower() == "localhost")
         {
            urlstr = Url.Scheme + "://" + Url.Authority + "/";
         }
         else
         {
            urlstr = Url.Scheme + "://" + Url.Host + "/";
         }
         return urlstr;
      }

   }
}

using System;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;

namespace HR
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            Debug.WriteLine("'***********APP DEBUG***********' " +DateTime.Now + "-App Start RegisterGlobalFilters");
            filters.Add(new HandleErrorAttribute());
            Debug.WriteLine("'***********APP DEBUG***********' " +DateTime.Now + "-End RegisterGlobalFilters");
        }
    }
}

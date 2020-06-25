using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HR
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            Debug.WriteLine("'***********APP DEBUG***********' " +DateTime.Now + "-App Start RegisterRoutes");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional }
            );
            Debug.WriteLine("'***********APP DEBUG***********' " +DateTime.Now + "-End RegisterRoutes");
        }
    }
}

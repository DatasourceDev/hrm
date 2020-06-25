using System;
using System.Diagnostics;
using System.Web;
using System.Web.Optimization;

namespace HR
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            Debug.WriteLine("'***********APP DEBUG***********' " +DateTime.Now + "-App Start RegisterBundles");
            Debug.WriteLine("'***********APP DEBUG***********' " +DateTime.Now + "-End RegisterBundles");
          
        }
    }
}

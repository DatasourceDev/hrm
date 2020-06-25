using System.Web;
using System.Web.Optimization;

namespace POS
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/util").Include(
                       "~/Scripts/app-validate.js",
                       "~/Scripts/app-control.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/bootstrap/javascripts/application.js"));

            bundles.Add(new ScriptBundle("~/Slick/scripts").Include(
                    "~/Scripts/Slick/slick.min.js"
                      ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/bootstrap/stylesheets/application.css"));

            bundles.Add(new StyleBundle("~/Slick/styles").Include(
                      "~/Content/Slick/slick.css",
                      "~/Content/Slick/slick-theme.css"));

            bundles.Add(new StyleBundle("~/owl/css").Include(
                      "~/Content/owl-carousel/owl.carousel.css",
                      "~/Content/owl-carousel/owl.theme.css"));

            bundles.Add(new ScriptBundle("~/owl/scripts").Include(
                    "~/Scripts/owl-carousel/owl.carousel.min.js"
                      ));

            bundles.Add(new ScriptBundle("~/notify/scripts").Include(
                    "~/Scripts/notifyjs/notify.js"
                      ));

        }
    }
}

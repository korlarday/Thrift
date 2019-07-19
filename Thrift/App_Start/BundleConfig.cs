using System.Web;
using System.Web.Optimization;

namespace Thrift
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/admin_js").Include(
                      "~/Content/Admin/plugins/jquery/jquery.min.js",
                      "~/Content/Admin/plugins/bootstrap/js/bootstrap.js",
                      "~/Content/Admin/plugins/bootstrap-select/js/bootstrap-select.js",
                      "~/Content/Admin/plugins/jquery-slimscroll/jquery.slimscroll.js",
                      "~/Content/Admin/plugins/node-waves/waves.js",
                      "~/Content/Admin/js/admin.js",
                      "~/Content/Admin/js/demo.js",
                      "~/Content/User/js/components/moment.js",
                      "~/Scripts/custom.js",
                      "~/Content/Admin/plugins/jquery-countto/jquery.countTo.js"
                      ));

            bundles.Add(new StyleBundle("~/admin_css").Include(
                      "~/Content/Admin/plugins/bootstrap/css/bootstrap.css",
                      "~/Content/Admin/plugins/node-waves/waves.css",
                      "~/Content/Admin/plugins/animate-css/animate.css",
                      "~/Content/Admin/css/style.css",
                      "~/Content/Admin/css/themes/all-themes.css",
                      "~/Content/Site.css"));

            bundles.Add(new StyleBundle("~/user_css").Include(
                      "~/Content/User/css/bootstrap.css",
                      "~/Content/User/style.css",
                      "~/Content/User/css/dark.css",
                      "~/Content/User/css/font-icons.css",
                      "~/Content/User/css/animate.css",
                      "~/Content/User/css/magnific-popup.css",
                      "~/Content/User/css/responsive.css",
                      "~/Content/User/custom.css",
                      "~/Content/Site.css"
                      ));

            bundles.Add(new ScriptBundle("~/user_js").Include(
                "~/Content/User/js/jquery.js",
                "~/Content/User/js/plugins.js",
                "~/Content/User/js/functions.js"
            ));
        }
    }
}

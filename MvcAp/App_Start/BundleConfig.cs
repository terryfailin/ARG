using System.Web;
using System.Web.Optimization;

namespace MvcAp.Backend
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/backend").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/mvcap.js",
                        "~/Scripts/mvcap.login.js",
                        //"~/Scripts/jquery.colorbox-min.js",
                        "~/Scripts/bootstrap-datepicker.min.js",
                        "~/Scripts/bootstrap-datepicker.zh-TW.min.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      //"~/Content/bootstrap.css",
                      "~/Content/application.css",
                      "~/Content/bootstrap-datepicker.min.css",
                      "~/Content/Site.css"
                      //"~/Content/colorbox.css"
            ));
        }
    }
}

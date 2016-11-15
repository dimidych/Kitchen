using System.Web.Optimization;

namespace MvcStudy
{
   public class BundleConfig
   {
      // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
      public static void RegisterBundles(BundleCollection bundles)
      {
         bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

          bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include("~/Scripts/jquery.validate*"));

          bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                      "~/Scripts/modernizr-*"));

          bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                    "~/Scripts/bootstrap.min.js",
                    "~/Scripts/respond.js",
                    "~/Scripts/bootstrap-hover-dropdown.js"));

          bundles.Add(new ScriptBundle("~/bundles/site").Include(
                      "~/Scripts/site.js"));
          
          bundles.Add(new ScriptBundle("~/bundles/datetimepicker").Include(
                      "~/Scripts/moment-with-locales.js",
                      "~/Scripts/bootstrap-datetimepicker.min.js"));

          bundles.Add(new ScriptBundle("~/bundles/gmap").Include(
                     "~/Scripts/jquery.gmap.js",
                     "~/Scripts/jquery.gmap_init.js"));

          bundles.Add(new ScriptBundle("~/bundles/modalform").Include(
                      "~/Scripts/modalform.js"));

          bundles.Add(new StyleBundle("~/Content/css").Include(
                    "~/Content/bootstrap.min.css",
                    "~/Content/bootstrap-datetimepicker.min.css",
                    "~/Content/site.css"));

          // Set EnableOptimizations to false for debugging. For more information,
          // visit http://go.microsoft.com/fwlink/?LinkId=301862
          BundleTable.EnableOptimizations = true;
      }
   }
}
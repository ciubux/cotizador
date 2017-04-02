using System.Web.Optimization;

namespace NetLab
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-ui").Include(
                        "~/Scripts/jquery-ui-{version}.js", "~/Scripts/jquery.multiselect.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));
                        //"~/Scripts/mvcfoolproof.unobtrusive.min.js"));
                        //"~/Scripts/MicrosoftMvcJQueryValidation.js", "~/Scripts/MvcFoolproofJQueryValidation.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-unobtrusive-ajax").Include(
                        "~/Scripts/jquery.unobtrusive*"));            

             bundles.Add(new ScriptBundle("~/bundles/jquery-timeentry").Include(
                        "~/Scripts/jquery.plugin.min.js", "~/Scripts/jquery.timeentry.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-datepicker").Include(
                        "~/Scripts/datepicker-es.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-chosen").Include(
                        "~/Scripts/chosen.jquery.min.js", "~/Scripts/chosen.ajaxaddition.jquery.js"));

               // Use the development version of Modernizr to develop with and learn from. Then, when you're
               // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
               bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/telerik").Include(
                "~/Scripts/telerik/kendo.all.min.js", 
                "~/Scripts/telerik/cultures/kendo.culture.es-PE.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/setup").Include(
                "~/Scripts/App/Setup.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/jquery-ui.css",
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/chosen.css",
                      "~/Content/jquery.timeentry.css",
                      "~/Content/jquery.multiselect.css",
                      "~/Content/multiple-select.css"));

            bundles.Add(new StyleBundle("~/Content/cssHeader").Include(
                      "~/Content/menubar.css"));
            bundles.Add(new ScriptBundle("~/bundles/multiple-select").Include(
                      "~/Scripts/multiple-select.js"));

            bundles.Add(new StyleBundle("~/Content/telerik/css").Include(
                "~/Content/telerik/kendo.common.min.css",
                "~/Content/telerik/kendo.default.min.css", 
                "~/Content/telerik/kendo.silver.min.css"));

        }
    }
}

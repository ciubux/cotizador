﻿using System.Web;
using System.Web.Optimization;

namespace Cotizador
{
    public class BundleConfig
    {
        // Para obtener más información sobre Bundles, visite http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                        "~/Scripts/main.js",
                        "~/Scripts/footable.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.1.1.min.js",
                        "~/Scripts/jquery-ui.min.js"
                        ));
            //"~/Scripts/jquery-{version}.js"

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información. De este modo, estará
            // preparado para la producción y podrá utilizar la herramienta de compilación disponible en http://modernizr.com para seleccionar solo las pruebas que necesite.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/footable.bootstrap.min.css",
                      "~/Content/jquery-ui.min.css",
                      "~/Content/site.css",
                      "~/Content/chosen.css"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-chosen").Include(
                       "~/Scripts/chosen/chosen.jquery.min.js", "~/Scripts/chosen/chosen.ajaxaddition.jquery.js", "~/Scripts/chosen/PrintArea.js"));
        }
    }
}

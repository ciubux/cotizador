using System.Web;
using System.Web.Optimization;

namespace Cotizador
{
    public class BundleConfig
    {
        // Para obtener más información sobre Bundles, visite http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new ScriptBundle("~/bundles/utiles").Include(
                         "~/Scripts/utiles1.1.7.js",
                         "~/Scripts/utilesFacturacion1.1.4.js"
                         ));

            bundles.Add(new ScriptBundle("~/bundles/venta").Include(
                         "~/Scripts/venta1.1.2.js"
                         ));

            bundles.Add(new ScriptBundle("~/bundles/factura").Include(
                        "~/Scripts/factura1.1.7.js"
                        ));


            bundles.Add(new ScriptBundle("~/bundles/boleta").Include(
                        "~/Scripts/boleta.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/notaCredito").Include(
                        "~/Scripts/notaCredito1.1.3.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/notaDebito").Include(
                        "~/Scripts/notaDebito1.1.2.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/cotizacion").Include(
                        "~/Scripts/cotizacion1.1.4.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/pedido").Include(
                        "~/Scripts/pedidoGeneral1.1.6.js",
                        "~/Scripts/pedido1.2.2.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/pedidoCompra").Include(
                        "~/Scripts/pedidoGeneral1.1.6.js",
                        "~/Scripts/pedidoCompra1.2.4.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/pedidoAlmacen").Include(
                       "~/Scripts/pedidoGeneral1.1.6.js",
                       "~/Scripts/pedidoAlmacen1.2.5.js"
                       ));

            bundles.Add(new ScriptBundle("~/bundles/cliente").Include(
                     "~/Scripts/cliente1.2.4.js"
                     ));

            bundles.Add(new ScriptBundle("~/bundles/producto").Include(
                     "~/Scripts/producto1.0.0.js"
                     ));

            bundles.Add(new ScriptBundle("~/bundles/origen").Include(
                     "~/Scripts/origen1.0.0.js"
                     ));

            bundles.Add(new ScriptBundle("~/bundles/subDistribuidor").Include(
                     "~/Scripts/subDistribuidor1.0.0.js"
                     ));

            bundles.Add(new ScriptBundle("~/bundles/guiaRemision").Include(
                        "~/Scripts/guiaRemision1.2.3.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/notaIngreso").Include(
                        "~/Scripts/notaIngreso1.2.1.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.1.1.min.js",
                        "~/Scripts/footable.min.js",
                        "~/Scripts/jquery-timepicker/jquery.timepicker.min.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jquery-ui").Include(
                     "~/Scripts/jquery-ui.min.js"
                     ));

            bundles.Add(new ScriptBundle("~/bundles/jquery-ui").Include(
                     "~/Scripts/jquery-ui.min.js"
                     ));


            bundles.Add(new ScriptBundle("~/bundles/loadingModal").Include(
                    "~/Scripts/jquery.loadingModal.js"
                    ));


            bundles.Add(new ScriptBundle("~/bundles/confirm").Include(
                    "~/Scripts/jquery-confirm.js"
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
                      "~/Content/site1.1.5.css",
                      "~/Content/chosen/chosen.css",
                      "~/Content/jquery.loadingModal.css",
                      "~/Scripts/jquery-timepicker/jquery.timepicker.min.css",
                      "~/Content/jquery-confirm.css"));

            bundles.Add(new StyleBundle("~/Content/PrintGuiaL").Include(
                       "~/Content/PrintGuiaL.css"
                       ));

            bundles.Add(new StyleBundle("~/Content/PrintGuiaT").Include(
                      "~/Content/PrintGuiaT.css"
                      ));

            bundles.Add(new StyleBundle("~/Content/PrintGuiaH").Include(
                      "~/Content/PrintGuiaH.css"
                      ));

            bundles.Add(new StyleBundle("~/Content/PrintGuiaO").Include(
                      "~/Content/PrintGuiaO.css"
                      ));

            bundles.Add(new StyleBundle("~/Content/PrintGuiaQ").Include(
                      "~/Content/PrintGuiaQ.css"
                      ));

            bundles.Add(new StyleBundle("~/Content/PrintGuiaA").Include(
                      "~/Content/PrintGuiaA.css"
                      ));

            bundles.Add(new StyleBundle("~/Content/PrintGuiaC").Include(
                      "~/Content/PrintGuiaC.css"
                      ));

            bundles.Add(new StyleBundle("~/Content/PrintGuiaZ").Include(
                      "~/Content/PrintGuiaZ.css"
                      ));


            bundles.Add(new ScriptBundle("~/bundles/jquery-chosen").Include(
                       "~/Scripts/chosen/chosen.jquery.min.js", "~/Scripts/chosen/chosen.ajaxaddition.jquery.js", "~/Scripts/chosen/PrintArea.js"));
        }
    }
}

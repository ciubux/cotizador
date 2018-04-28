using BusinessLayer;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cotizador.Controllers
{
    public class DocumentoVentaController : Controller
    {
        // GET: DocumentoVenta
        public ActionResult Index()
        {
            return View();
        }

        public String Facturar()
        {

            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_VER];
            DocumentoVenta documentoVenta = new DocumentoVenta();

            String[] fecha = this.Request.Params["fechaEmision"].Split('/');
            String[] hora = this.Request.Params["horaEmision"].Split(':');

           
            documentoVenta.fechaEmision = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0]), Int32.Parse(hora[0]), Int32.Parse(hora[1]), 0);

            fecha = this.Request.Params["fechaVencimiento"].Split('/');
            documentoVenta.fechaVencimiento = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0]));


            documentoVenta.tipoPago = (DocumentoVenta.TipoPago)Int32.Parse(this.Request.Params["tipoPago"]);
            documentoVenta.formaPago = (DocumentoVenta.FormaPago)Int32.Parse(this.Request.Params["formaPago"]);

            documentoVenta.correoEnvio = this.Request.Params["correoEnvio"];
            documentoVenta.correoCopia = this.Request.Params["correoCopia"];
            documentoVenta.correoOculto = this.Request.Params["correoOculto"];

            documentoVenta.venta = new Venta();
            documentoVenta.venta.pedido = pedido;

            DocumentoVentaBL documentoVentaBL = new DocumentoVentaBL();
            documentoVentaBL.InsertarFactura(documentoVenta);

            return "";
        }




    }
}
using BusinessLayer;
using Model;
using Model.ServiceReferencePSE;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Cotizador.Controllers
{
    public class NotaCreditoController : Controller
    {
        // GET: NotaCredito
        /*   public ActionResult Index()
           {
               return View();
           }


       */
        public ActionResult Crear()
        {
            Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_NOTA_CREDITO];
         //   venta.tipoNotaCredito = venta.documentoVenta.tipoNotaCredito;
            ViewBag.venta = venta;
            ViewBag.tipoNotaCredito = (int)venta.tipoNotaCredito;           

            ViewBag.fechaEmision = venta.documentoVenta.fechaEmision.Value.ToString(Constantes.formatoFecha);
            ViewBag.horaEmision = venta.documentoVenta.fechaEmision.Value.ToString(Constantes.formatoHora);  

            return View();
        }




        public String iniciarCreacionNotaCredito()
        {
            VentaBL ventaBL = new VentaBL();
            Venta venta = new Venta();
            
            venta.documentoVenta = (DocumentoVenta)this.Session[Constantes.VAR_SESSION_FACTURA_VER];
            venta.documentoVenta.venta = null;


            venta.documentoVenta.idDocumentoVenta = Guid.Parse(Request["idDocumentoVenta"].ToString());
            
            venta.documentoReferencia = new DocumentoReferencia();
            venta.documentoReferencia.tipoDocumento = (DocumentoVenta.TipoDocumento)Int32.Parse(venta.documentoVenta.cPE_CABECERA_BE.TIP_CPE);
            String[] fechaEmisionArray = venta.documentoVenta.cPE_CABECERA_BE.FEC_EMI.Split('-');
            venta.documentoReferencia.fechaEmision = new DateTime(Int32.Parse(fechaEmisionArray[0]), Int32.Parse(fechaEmisionArray[1]), Int32.Parse(fechaEmisionArray[2]));
            venta.documentoReferencia.serie = venta.documentoVenta.cPE_CABECERA_BE.SERIE;
            venta.documentoReferencia.numero = venta.documentoVenta.cPE_CABECERA_BE.CORRELATIVO;

            venta = ventaBL.GetPlantillaNotaCredito(venta);
            if (venta.tipoErrorCrearTransaccion == Venta.TiposErrorCrearTransaccion.NoExisteError)
            {
                venta.tipoNotaCredito = (DocumentoVenta.TiposNotaCredito)Int32.Parse(Request["tipoNotaCredito"].ToString());
                venta.documentoVenta.fechaEmision = DateTime.Now;

                //Temporal
                Pedido pedido = venta.pedido;
                pedido.ciudadASolicitar = new Ciudad();

                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

                PedidoBL pedidoBL = new PedidoBL();
                pedidoBL.calcularMontosTotales(pedido);
                this.Session[Constantes.VAR_SESSION_NOTA_CREDITO] = venta;
            }
            return JsonConvert.SerializeObject(venta);
            
        }


        [HttpPost]
        public String ChangeDetalle(List<DocumentoDetalleJson> cotizacionDetalleJsonList)
        {
            Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_NOTA_CREDITO];
            IDocumento documento = (Pedido)venta.pedido;
            List<DocumentoDetalle> documentoDetalle = HelperDocumento.updateDocumentoDetalle(documento, cotizacionDetalleJsonList);
            documento.documentoDetalle = documentoDetalle;
            PedidoBL pedidoBL = new PedidoBL();
            pedidoBL.calcularMontosTotales((Pedido)documento);
            venta.pedido = (Pedido)documento;
            this.Session[Constantes.VAR_SESSION_NOTA_CREDITO] = venta;
            return "{\"cantidad\":\"" + documento.documentoDetalle.Count + "\"}";
        }


        public void ChangeInputString()
        {
            Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_NOTA_CREDITO];
            PropertyInfo propertyInfo = venta.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(venta, this.Request.Params["valor"]);
            this.Session[Constantes.VAR_SESSION_NOTA_CREDITO] = venta;
        }

        public void ChangeFechaEmision()
        {
            Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_NOTA_CREDITO];
            String[] fechaEmision = this.Request.Params["fechaEmision"].Split('/');
            String[] horaEmision = this.Request.Params["horaEmision"].Split(':');
            venta.documentoVenta.fechaEmision = new DateTime(Int32.Parse(fechaEmision[2]), Int32.Parse(fechaEmision[1]), Int32.Parse(fechaEmision[0]), Int32.Parse(horaEmision[0]), Int32.Parse(horaEmision[1]), 0);
            this.Session[Constantes.VAR_SESSION_NOTA_CREDITO] = venta;
        }

        public String Create()
        {

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            try
            {
                Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_NOTA_CREDITO];

                String[] fecha = this.Request.Params["fechaEmision"].Split('/');
                String[] hora = this.Request.Params["horaEmision"].Split(':');

                venta.observaciones = this.Request.Params["observaciones"];
                venta.sustento = this.Request.Params["sustento"];

                
                venta.documentoVenta.fechaEmision = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0]), Int32.Parse(hora[0]), Int32.Parse(hora[1]), 0);
                ////La fecha de vencimiento es identifica a la fecha de emisión
                venta.documentoVenta.fechaVencimiento = venta.documentoVenta.fechaEmision.Value;
                ////El tipo de pago es al contado
                venta.documentoVenta.tipoPago = DocumentoVenta.TipoPago.Contado;
                ////La forma de Pago es Efectivo
                venta.documentoVenta.formaPago = DocumentoVenta.FormaPago.Efectivo;
                venta.documentoVenta.usuario = usuario;
                
                venta.usuario = usuario;
                VentaBL ventaBL = new VentaBL();
                ventaBL.InsertVentaNotaCredito(venta);

                //Falta Agregar validación de creación de venta

                DocumentoVentaBL documentoVentaBL = new DocumentoVentaBL();
                //Se crea una venta solo para que pueda enviar 
                venta.documentoVenta.venta = new Venta();
                venta.documentoVenta.venta.idVenta = venta.idVenta;
                venta.documentoVenta.venta.documentoReferencia = venta.documentoReferencia;
                venta.documentoVenta.cliente = venta.cliente;
                venta.documentoVenta = documentoVentaBL.InsertarNotaCredito(venta.documentoVenta);
                venta.documentoVenta.tipoNotaCredito = (DocumentoVenta.TiposNotaCredito)Int32.Parse(venta.documentoVenta.cPE_CABECERA_BE.COD_TIP_NC);
                return JsonConvert.SerializeObject(venta.documentoVenta);
            }
            catch (Exception ex)
            {
                Log log = new Log(ex.ToString(), TipoLog.Error, usuario);
                LogBL logBL = new LogBL();
                logBL.insertLog(log);
                return ex.ToString();
            }
        }


        public String ConfirmarCreacion()
        {

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            try
            {
                Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_NOTA_CREDITO];
                DocumentoVenta documentoVenta = new DocumentoVenta();
                documentoVenta.venta = venta;
                documentoVenta.idDocumentoVenta = Guid.Parse(this.Request.Params["idDocumentoVenta"]);
                documentoVenta.cliente = venta.cliente;
                documentoVenta.usuario = usuario;

                DocumentoVentaBL documentoVentaBL = new DocumentoVentaBL();
                documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.NotaCrédito;
                CPE_RESPUESTA_BE cPE_RESPUESTA_BE = documentoVentaBL.procesarCPE(documentoVenta);

                var otmp = new
                {
                    CPE_RESPUESTA_BE = cPE_RESPUESTA_BE,
                    serieNumero = documentoVenta.serieNumero,
                    idDocumentoVenta = documentoVenta.idDocumentoVenta
                };

                return JsonConvert.SerializeObject(otmp);
            }
            catch (Exception ex)
            {
                Log log = new Log(ex.ToString(), TipoLog.Error, usuario);
                LogBL logBL = new LogBL();
                logBL.insertLog(log);
                return ex.ToString();
            }
        }

    }
}




//    venta.documentoVenta.idDocumentoVenta = Guid.Parse(Request["idDocumentoVenta"].ToString());
//    venta.documentoVenta.tipoNotaCredito = (DocumentoVenta.TiposNotaCredito)Int32.Parse(Request["tipoNotaCredito"].ToString());



/*
GRUPO A

01 – Anulación de la operación 
    Carga Venta Original y no te permite modifcar el detalle solo agregar sustento, Plazo de crédito es 0 días, contado.
    Agregar Sustento

02 - Anulación por error en el RUC 
    Carga Venta Original y no te permite modifcar el detalle solo agregar sustento, Plazo de crédito es 0 días, contado.
    Agregar Sustento
    Validación 15 días 

03 – Corrección por error en la descripción 
    Carga Venta Original y no te permite modifcar el detalle solo agregar sustento, Plazo de crédito es 0 días, contado.
    Agregar Sustento
    Validación 15 días	

06 – Devolución total 
    ********* Considera la nota de ingreso


GRUPO B

    Previamente se debe seleccionar el producto

04 – Descuento global (solo para factura) 
    Solo deben permitir una linea,
    la cantidad no se modifica y siempre es 1, 
    considerar solo producto que tenga el flag de descuentoGlobal
    Modificar el precio unitario 
    Maximo descuento global debe ser menor al 100% del subtotal de la venta


GRUPO C

05 – Descuento por ítem (solo para factura) 
09 – Disminución en el valor
    Te carga el detalle de la venta original, 
    Te permite eliminar lineas de detalle, 
    No te permite modificar las cantidad, 
    No te permite agregar lineas 
    Modificar el precio unitario
    El precio unitario debe ser menor al precio unitario de la factura (VALOR UNITARIO)


GRUPO D

07 – Devolución por ítem 
    Te carga el detalle de la venta original, 
    Te permite eliminar lineas de detalle, 
    Te permite modificar las cantidad, 
    No te permite agregar lineas 
    No te permite el precio unitario*/

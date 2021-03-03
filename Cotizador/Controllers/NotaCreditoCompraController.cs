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
    public class NotaCreditoCompraController : Controller
    {
        // GET: NotaCredito
        /*   public ActionResult Index()
           {
               return View();
           }
       */
        public ActionResult Crear()
        {
            Compra transaccion = (Compra)this.Session[Constantes.VAR_SESSION_NOTA_CREDITO_COMPRA];
         //   venta.tipoNotaCredito = venta.documentoVenta.tipoNotaCredito;
            ViewBag.compra = transaccion;
            ViewBag.tipoNotaCredito = (int)transaccion.tipoNotaCredito;           

            ViewBag.fechaEmision = transaccion.documentoCompra.fechaEmision.Value.ToString(Constantes.formatoFecha);
            ViewBag.horaEmision = transaccion.documentoCompra.fechaEmision.Value.ToString(Constantes.formatoHora);  

            return View();
        }




        public String iniciarCreacionNotaCredito()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            CompraBL compraBL = new CompraBL();
            Compra transaccion = new Compra();
            
            transaccion.documentoCompra = (DocumentoCompra)this.Session[Constantes.VAR_SESSION_DOCUMENTO_COMPRA_VER];
            transaccion.documentoCompra.compra = null;
            transaccion.documentoCompra.tipoDocumento = DocumentoCompra.TipoDocumento.NotaCrédito;

            //transaccion.documentoCompra.idDocumentoCompra = int.Parse(Request["idDocumentoCompra"].ToString());
         

            transaccion.documentoReferencia = new DocumentoReferencia();
            transaccion.documentoReferencia.tipoDocumento = (DocumentoVenta.TipoDocumento)Int32.Parse(transaccion.documentoCompra.cPE_CABECERA_COMPRA.TIP_CPE);
            String[] fechaEmisionArray = transaccion.documentoCompra.cPE_CABECERA_COMPRA.FEC_EMI.Split('-');
            transaccion.documentoReferencia.fechaEmision = new DateTime(Int32.Parse(fechaEmisionArray[0]), Int32.Parse(fechaEmisionArray[1]), Int32.Parse(fechaEmisionArray[2]));
            transaccion.documentoReferencia.serie = transaccion.documentoCompra.cPE_CABECERA_COMPRA.SERIE;
            transaccion.documentoReferencia.numero = transaccion.documentoCompra.cPE_CABECERA_COMPRA.CORRELATIVO;





            DocumentoVenta.TiposNotaCredito tiposNotaCredito = (DocumentoVenta.TiposNotaCredito)Int32.Parse(Request["tipoNotaCredito"].ToString());

            if (tiposNotaCredito == DocumentoVenta.TiposNotaCredito.DescuentoGlobal)
            {
                Guid idProducto = Guid.Parse(Request["idProducto"].ToString());
                transaccion = compraBL.GetPlantillaCompraDescuentoGlobal(transaccion, usuario, idProducto);
            }
            else
            {
                transaccion = compraBL.GetPlantillaCompra(transaccion, usuario);
            }


            
            if (transaccion.tipoErrorCrearTransaccion == Venta.TiposErrorCrearTransaccion.NoExisteError)
            {
                transaccion.tipoNotaCredito = tiposNotaCredito;

                transaccion.documentoCompra.fechaEmision = DateTime.Now;

                //Temporal
                Pedido pedido = transaccion.pedido;
                pedido.ciudadASolicitar = new Ciudad();               

                PedidoBL pedidoBL = new PedidoBL();
                pedidoBL.calcularMontosTotales(pedido);

                /*Se obtiene la ciudad para poder cargar las series del documento de pago*/
                Ciudad ciudad = usuario.sedesMPPedidos.Where(s => s.idCiudad == transaccion.cliente.ciudad.idCiudad).FirstOrDefault();//  Guid.Parse("39C9D42B-6D94-4AAE-93B8-D1D6A5F11A33")).FirstOrDefault();
                List<SerieDocumentoElectronico> serieDocumentoElectronicoList = ciudad.serieDocumentoElectronicoList.OrderByDescending(x => x.esPrincipal).ToList();
              //  transaccion.cliente = new Cliente();
           //     transaccion.cliente.ciudad = new Ciudad();
                
                /*Se selecciona la primera serie de la lista*/
                transaccion.documentoCompra.serieDocumentoElectronico = serieDocumentoElectronicoList[0];
                transaccion.documentoCompra.serie = "";
                transaccion.documentoCompra.numero = "";



                this.Session[Constantes.VAR_SESSION_NOTA_CREDITO_COMPRA] = transaccion;
            }
            return JsonConvert.SerializeObject(transaccion);
            
        }

        public String iniciarCreacionNotaCreditoDesdeNotaIngreso()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            //Se recupera la nota de ingreso creada
            NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO];
            /*Se recupera el id de la factura a extornar*/
            MovimientoAlmacenBL movimientoAlmacenBL = new MovimientoAlmacenBL();
            DocumentoVenta documentoVenta = new DocumentoVenta();
            documentoVenta.idDocumentoVenta = movimientoAlmacenBL.obtenerIdDocumentoVenta(notaIngreso.guiaRemisionAExtornar);
            



            /*Con el id de la factura a extornar se obtiene todo del documento de pago*/
            DocumentoVentaBL documentoVentaBL = new DocumentoVentaBL();
            Compra transaccionExtorno = new Compra();
            transaccionExtorno.documentoVenta = documentoVentaBL.GetDocumentoVenta(documentoVenta);

            /*Se recupera el sustento y las ovservaciones de la guía*/
            transaccionExtorno.sustento = notaIngreso.sustentoExtorno;
            transaccionExtorno.documentoVenta.observacionesUsoInterno = notaIngreso.observaciones;

            /*Se agrega la nota de ingreso al documento de venta para poder controlar la vista
             * de modo tal que no se permita editar el detalle*/
            transaccionExtorno.documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.NotaCrédito;
            transaccionExtorno.documentoVenta.movimientoAlmacen = notaIngreso;
            transaccionExtorno.documentoVenta.venta = null;
            /*Se agrega el documento de referencia a la nota de crédito, los datos que se requieren son el clasePedido, serie, número y fecha*/
            transaccionExtorno.documentoReferencia = new DocumentoReferencia();
            transaccionExtorno.documentoReferencia.tipoDocumento = (DocumentoVenta.TipoDocumento)Int32.Parse(transaccionExtorno.documentoVenta.cPE_CABECERA_BE.TIP_CPE);
            String[] fechaEmisionArray = transaccionExtorno.documentoVenta.cPE_CABECERA_BE.FEC_EMI.Split('-');
            transaccionExtorno.documentoReferencia.fechaEmision = new DateTime(Int32.Parse(fechaEmisionArray[0]), Int32.Parse(fechaEmisionArray[1]), Int32.Parse(fechaEmisionArray[2]));          
            transaccionExtorno.documentoReferencia.serie = transaccionExtorno.documentoVenta.cPE_CABECERA_BE.SERIE;
            transaccionExtorno.documentoReferencia.numero = transaccionExtorno.documentoVenta.cPE_CABECERA_BE.CORRELATIVO;

            /*Se recupera la venta/transacción del extorno (nota de ingreso)*/
            VentaBL ventaBL = new VentaBL();

            /****************** TO DO: OBTENER GUIA REMISION ********************/
            //transaccionExtorno = ventaBL.GetNotaIngresoTransaccion(transaccionExtorno, notaIngreso, usuario);

            /*Se obtiene la ciudad para poder cargar las series del documento de pago*/
            Ciudad ciudad = usuario.sedesMPPedidos.Where(s => s.idCiudad == transaccionExtorno.cliente.ciudad.idCiudad).FirstOrDefault();
            List<SerieDocumentoElectronico> serieDocumentoElectronicoList = ciudad.serieDocumentoElectronicoList.OrderByDescending(x => x.esPrincipal).ToList();
            transaccionExtorno.cliente.ciudad.serieDocumentoElectronicoList = serieDocumentoElectronicoList;

            /*Se selecciona la primera serie de la lista*/
            transaccionExtorno.documentoVenta.serieDocumentoElectronico = serieDocumentoElectronicoList[0];

            ClienteBL clienteBl = new ClienteBL();
            Cliente cliente = clienteBl.getCliente(transaccionExtorno.cliente.idCliente);


            if (cliente.tipoDocumentoIdentidad == DocumentoVenta.TiposDocumentoIdentidad.RUC)
            {
                transaccionExtorno.documentoVenta.serie = Constantes.PREFIJO_NOTA_CREDITO_FACTURA + transaccionExtorno.documentoVenta.serieDocumentoElectronico.serie.Substring(1);
                transaccionExtorno.documentoVenta.numero = transaccionExtorno.documentoVenta.serieDocumentoElectronico.siguienteNumeroNotaCredito.ToString();
            }
            else
            {
                transaccionExtorno.documentoVenta.serie = Constantes.PREFIJO_NOTA_CREDITO_BOLETA + transaccionExtorno.documentoVenta.serieDocumentoElectronico.serie.Substring(1);
                transaccionExtorno.documentoVenta.numero = transaccionExtorno.documentoVenta.serieDocumentoElectronico.siguienteNumeroNotaCreditoBoleta.ToString();
            }


            if (transaccionExtorno.tipoErrorCrearTransaccion == Venta.TiposErrorCrearTransaccion.NoExisteError)
            {
                /*Si no existe error al recuperar la transacción, se define el clasePedido de nota de crédito
                 según lo seleccionado al momento de generar el extorno*/
                transaccionExtorno.tipoNotaCredito = (DocumentoVenta.TiposNotaCredito)(int)notaIngreso.motivoExtornoGuiaRemision;
                transaccionExtorno.documentoVenta.fechaEmision = DateTime.Now;

                Pedido pedido = transaccionExtorno.pedido;
                pedido.ciudadASolicitar = new Ciudad();
                PedidoBL pedidoBL = new PedidoBL();
                pedidoBL.calcularMontosTotales(pedido);
                /*La transaccion de extorno como nota de crédito*/
                this.Session[Constantes.VAR_SESSION_NOTA_CREDITO_COMPRA] = transaccionExtorno;
            }
            return JsonConvert.SerializeObject(transaccionExtorno);
        }


        [HttpPost]
        public String ChangeDetalle(List<DocumentoDetalleJson> cotizacionDetalleJsonList)
        {
            Compra transaccionExtorno = (Compra)this.Session[Constantes.VAR_SESSION_NOTA_CREDITO_COMPRA];
            IDocumento documento = (Pedido)transaccionExtorno.pedido;
            List<DocumentoDetalle> documentoDetalle = HelperDocumento.updateDocumentoDetalle(documento, cotizacionDetalleJsonList);
            documento.documentoDetalle = documentoDetalle;
            PedidoBL pedidoBL = new PedidoBL();
            pedidoBL.calcularMontosTotales((Pedido)documento);
            transaccionExtorno.pedido = (Pedido)documento;
            this.Session[Constantes.VAR_SESSION_NOTA_CREDITO_COMPRA] = transaccionExtorno;
            return "{\"cantidad\":\"" + documento.documentoDetalle.Count + "\"}";
        }


        public void ChangeInputString()
        {
            Compra transaccionExtorno = (Compra)this.Session[Constantes.VAR_SESSION_NOTA_CREDITO_COMPRA];
            PropertyInfo propertyInfo = transaccionExtorno.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(transaccionExtorno, this.Request.Params["valor"]);
            this.Session[Constantes.VAR_SESSION_NOTA_CREDITO_COMPRA] = transaccionExtorno;
        }

        public void ChangeInputStringDocumentoCompra()
        {
            Compra transaccionExtorno = (Compra)this.Session[Constantes.VAR_SESSION_NOTA_CREDITO_COMPRA];
            PropertyInfo propertyInfo = transaccionExtorno.documentoCompra.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(transaccionExtorno, this.Request.Params["valor"]);
            this.Session[Constantes.VAR_SESSION_NOTA_CREDITO_COMPRA] = transaccionExtorno;
        }

        public void ChangeFechaEmision()
        {
            Compra transaccionExtorno = (Compra)this.Session[Constantes.VAR_SESSION_NOTA_CREDITO_COMPRA];
            String[] fechaEmision = this.Request.Params["fechaEmision"].Split('/');
            String[] horaEmision = this.Request.Params["horaEmision"].Split(':');
            transaccionExtorno.documentoVenta.fechaEmision = new DateTime(Int32.Parse(fechaEmision[2]), Int32.Parse(fechaEmision[1]), Int32.Parse(fechaEmision[0]), Int32.Parse(horaEmision[0]), Int32.Parse(horaEmision[1]), 0);
            this.Session[Constantes.VAR_SESSION_NOTA_CREDITO_COMPRA] = transaccionExtorno;
        }

        public void ChangeObservcacionesUsoInterno()
        {
            Compra transaccionExtorno = (Compra)this.Session[Constantes.VAR_SESSION_NOTA_CREDITO_COMPRA];
            transaccionExtorno.documentoVenta.observacionesUsoInterno = this.Request.Params["observacionesUsoInterno"];
            this.Session[Constantes.VAR_SESSION_NOTA_CREDITO_COMPRA] = transaccionExtorno;
        }

        public void ChangeSerie()
        {

            Compra transaccionExtorno = (Compra)this.Session[Constantes.VAR_SESSION_NOTA_CREDITO_COMPRA];
            String serie = this.Request.Params["serie"];
            ClienteBL clienteBl = new ClienteBL();
            Cliente cliente = clienteBl.getCliente(transaccionExtorno.cliente.idCliente);

            transaccionExtorno.documentoVenta.serieDocumentoElectronico = transaccionExtorno.cliente.ciudad.serieDocumentoElectronicoList.Where(t => t.serie == serie).FirstOrDefault();

            if (cliente.tipoDocumentoIdentidad == DocumentoVenta.TiposDocumentoIdentidad.RUC)
            {
                transaccionExtorno.documentoVenta.serie = Constantes.PREFIJO_NOTA_CREDITO_FACTURA + transaccionExtorno.documentoVenta.serieDocumentoElectronico.serie.Substring(1);
                transaccionExtorno.documentoVenta.numero = transaccionExtorno.documentoVenta.serieDocumentoElectronico.siguienteNumeroNotaCredito.ToString();
            }
            else
            {
                transaccionExtorno.documentoVenta.serie = Constantes.PREFIJO_NOTA_CREDITO_BOLETA + transaccionExtorno.documentoVenta.serieDocumentoElectronico.serie.Substring(1);
                transaccionExtorno.documentoVenta.numero = transaccionExtorno.documentoVenta.serieDocumentoElectronico.siguienteNumeroNotaCreditoBoleta.ToString();
            }

            
            this.Session[Constantes.VAR_SESSION_NOTA_CREDITO_COMPRA] = transaccionExtorno;
        }

        public String Create()
        {

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            try
            {
                Compra transaccion = (Compra)this.Session[Constantes.VAR_SESSION_NOTA_CREDITO_COMPRA];

                String[] fecha = this.Request.Params["fechaEmision"].Split('/');

                transaccion.observaciones = this.Request.Params["observaciones"];
                transaccion.sustento = this.Request.Params["sustento"];

                
                transaccion.documentoCompra.fechaEmision = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0]), 0, 0, 0);
                ////La fecha de vencimiento es identifica a la fecha de emisión
                transaccion.documentoCompra.fechaVencimiento = transaccion.documentoCompra.fechaEmision.Value;
                ////El clasePedido de pago es al contado
                transaccion.documentoCompra.tipoPago = DocumentoCompra.TipoPago.Contado;
                ////La forma de Pago es Efectivo
                transaccion.documentoCompra.formaPago = DocumentoCompra.FormaPago.Efectivo;
                transaccion.documentoCompra.usuario = usuario;
                
                transaccion.usuario = usuario;
                CompraBL compraBL = new CompraBL();

                /*Si no se cuenta con movimiento de almacen (nota de ingreso) entonces se debe insertar la venta*/
                if (transaccion.documentoCompra.movimientoAlmacen == null)
                {
                    compraBL.InsertTransaccionNotaCredito(transaccion);
                }
                else {
                    /*Por el contrario si se cuenta con la nota de ingreso se debe actualizar la venta para agregar los datos del documento de referencia*/
                    compraBL.UpdateTransaccionNotaCredito(transaccion);
                }
                

                DocumentoCompraBL documentoVentaBL = new DocumentoCompraBL();
                //Se crea una venta solo para que pueda enviar 
                transaccion.documentoCompra.compra = new Compra();
                transaccion.documentoCompra.compra.idCompra = transaccion.idCompra;
                transaccion.documentoCompra.compra.documentoReferencia = transaccion.documentoReferencia;
                transaccion.documentoCompra.proveedor = transaccion.documentoCompra.proveedor;
                transaccion.documentoCompra = documentoVentaBL.InsertarNotaCredito(transaccion.documentoCompra);
                transaccion.documentoCompra.tipoNotaCredito = (DocumentoCompra.TiposNotaCredito)Int32.Parse(transaccion.documentoCompra.cPE_CABECERA_COMPRA.COD_TIP_NC);
                return JsonConvert.SerializeObject(transaccion.documentoCompra);
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
                Compra venta = (Compra)this.Session[Constantes.VAR_SESSION_NOTA_CREDITO_COMPRA];
                DocumentoCompra documentoCompra = new DocumentoCompra();
                documentoCompra.compra = venta;
                documentoCompra.idDocumentoCompra = int.Parse(this.Request.Params["idDocumentoCompra"]);
                //documentoCompra.proveedor = venta.cliente;
                documentoCompra.usuario = usuario;

                DocumentoCompraBL documentoVentaBL = new DocumentoCompraBL();
                documentoCompra.tipoDocumento = DocumentoCompra.TipoDocumento.NotaCrédito;
                CPE_RESPUESTA_COMPRA cPE_RESPUESTA_BE = documentoVentaBL.procesarCPE(documentoCompra);

                var otmp = new
                {
                    CPE_RESPUESTA_COMPRA = cPE_RESPUESTA_BE,
                    serieNumero = documentoCompra.serieNumero,
                    idDocumentoCompra = documentoCompra.idDocumentoCompra
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

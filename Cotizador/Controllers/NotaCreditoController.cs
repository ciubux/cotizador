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
            Transaccion transaccion = (Transaccion)this.Session[Constantes.VAR_SESSION_NOTA_CREDITO];
         //   venta.tipoNotaCredito = venta.documentoVenta.tipoNotaCredito;
            ViewBag.venta = transaccion;
            ViewBag.tipoNotaCredito = (int)transaccion.tipoNotaCredito;           

            ViewBag.fechaEmision = transaccion.documentoVenta.fechaEmision.Value.ToString(Constantes.formatoFecha);
            ViewBag.horaEmision = transaccion.documentoVenta.fechaEmision.Value.ToString(Constantes.formatoHora);  

            return View();
        }




        public String iniciarCreacionNotaCredito()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            VentaBL ventaBL = new VentaBL();
            Transaccion transaccion = new Venta();
            
            transaccion.documentoVenta = (DocumentoVenta)this.Session[Constantes.VAR_SESSION_FACTURA_VER];
            transaccion.documentoVenta.venta = null;
            transaccion.documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.NotaCrédito;

            transaccion.documentoVenta.idDocumentoVenta = Guid.Parse(Request["idDocumentoVenta"].ToString());
         

            transaccion.documentoReferencia = new DocumentoReferencia();
            transaccion.documentoReferencia.tipoDocumento = (DocumentoVenta.TipoDocumento)Int32.Parse(transaccion.documentoVenta.cPE_CABECERA_BE.TIP_CPE);
            String[] fechaEmisionArray = transaccion.documentoVenta.cPE_CABECERA_BE.FEC_EMI.Split('-');
            transaccion.documentoReferencia.fechaEmision = new DateTime(Int32.Parse(fechaEmisionArray[0]), Int32.Parse(fechaEmisionArray[1]), Int32.Parse(fechaEmisionArray[2]));
            transaccion.documentoReferencia.serie = transaccion.documentoVenta.cPE_CABECERA_BE.SERIE;
            transaccion.documentoReferencia.numero = transaccion.documentoVenta.cPE_CABECERA_BE.CORRELATIVO;
            transaccion.moneda = transaccion.documentoVenta.moneda;




            DocumentoVenta.TiposNotaCredito tiposNotaCredito = (DocumentoVenta.TiposNotaCredito)Int32.Parse(Request["tipoNotaCredito"].ToString());

            if (tiposNotaCredito == DocumentoVenta.TiposNotaCredito.DescuentoGlobal)
            {
                Guid idProducto = Guid.Parse(Request["idProducto"].ToString());
                transaccion = ventaBL.GetPlantillaVentaDescuentoGlobal(transaccion, usuario, idProducto);
            }
            else
            {
                transaccion = ventaBL.GetPlantillaVenta(transaccion, usuario);
            }


            
            if (transaccion.tipoErrorCrearTransaccion == Venta.TiposErrorCrearTransaccion.NoExisteError)
            {
                transaccion.tipoNotaCredito = tiposNotaCredito;

                transaccion.documentoVenta.fechaEmision = DateTime.Now;

                //Temporal
                Pedido pedido = transaccion.pedido;
                pedido.ciudadASolicitar = new Ciudad();               

                PedidoBL pedidoBL = new PedidoBL();
                pedidoBL.calcularMontosTotales(pedido);

                /*Se obtiene la ciudad para poder cargar las series del documento de pago*/
                Ciudad ciudad = usuario.sedesMPPedidos.Where(s => s.idCiudad == transaccion.cliente.ciudad.idCiudad).FirstOrDefault();//  Guid.Parse("39C9D42B-6D94-4AAE-93B8-D1D6A5F11A33")).FirstOrDefault();
                List<SerieDocumentoElectronico> serieDocumentoElectronicoList = ciudad.serieDocumentoElectronicoList.OrderByDescending(x => x.esPrincipal).ToList();
                serieDocumentoElectronicoList = serieDocumentoElectronicoList.Where(x => x.serie.Substring(0, 1).Equals("0")).ToList();
                //  transaccion.cliente = new Cliente();
                //     transaccion.cliente.ciudad = new Ciudad();
                transaccion.cliente.ciudad.serieDocumentoElectronicoList = serieDocumentoElectronicoList;

                /*Se selecciona la primera serie de la lista*/
                transaccion.documentoVenta.serieDocumentoElectronico = serieDocumentoElectronicoList[0];
                transaccion.documentoVenta.serie = Constantes.PREFIJO_NOTA_CREDITO_FACTURA + transaccion.documentoVenta.serieDocumentoElectronico.serie.Substring(1);
                transaccion.documentoVenta.numero = transaccion.documentoVenta.serieDocumentoElectronico.siguienteNumeroNotaCredito.ToString();
                

                this.Session[Constantes.VAR_SESSION_NOTA_CREDITO] = transaccion;
            }
            return JsonConvert.SerializeObject(transaccion);
            
        }


        public String CrearNotaCreditoAjustes()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            VentaBL ventaBL = new VentaBL();
            Transaccion transaccion = new Venta();

            transaccion.documentoVenta = (DocumentoVenta)this.Session[Constantes.VAR_SESSION_FACTURA_VER];
            transaccion.documentoVenta.venta = null;
            transaccion.documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.NotaCrédito;

            transaccion.documentoVenta.idDocumentoVenta = Guid.Parse(Request["idDocumentoVenta"].ToString());


            transaccion.documentoReferencia = new DocumentoReferencia();
            transaccion.documentoReferencia.tipoDocumento = (DocumentoVenta.TipoDocumento)Int32.Parse(transaccion.documentoVenta.cPE_CABECERA_BE.TIP_CPE);
            String[] fechaEmisionArray = transaccion.documentoVenta.cPE_CABECERA_BE.FEC_EMI.Split('-');
            transaccion.documentoReferencia.fechaEmision = new DateTime(Int32.Parse(fechaEmisionArray[0]), Int32.Parse(fechaEmisionArray[1]), Int32.Parse(fechaEmisionArray[2]));
            transaccion.documentoReferencia.serie = transaccion.documentoVenta.cPE_CABECERA_BE.SERIE;
            transaccion.documentoReferencia.numero = transaccion.documentoVenta.cPE_CABECERA_BE.CORRELATIVO;


            DocumentoVenta.TiposNotaCredito tiposNotaCredito = (DocumentoVenta.TiposNotaCredito)Int32.Parse(Request["tipoNotaCredito"].ToString());

            String fechaVencimiento = Request["fechaVencimiento"].ToString();
            String sustento = Request["sustento"].ToString();

            ClienteBL clienteBl = new ClienteBL();
            transaccion.cliente = clienteBl.getCliente(transaccion.documentoVenta.cliente.idCliente);
            transaccion.tipoNotaCredito = tiposNotaCredito;
            transaccion.documentoVenta.fechaEmision = DateTime.Now;

            /*Se obtiene la ciudad para poder cargar las series del documento de pago*/
            Ciudad ciudad = usuario.sedesMPPedidos.Where(s => s.idCiudad == transaccion.cliente.ciudad.idCiudad).FirstOrDefault();//  Guid.Parse("39C9D42B-6D94-4AAE-93B8-D1D6A5F11A33")).FirstOrDefault();
            List<SerieDocumentoElectronico> serieDocumentoElectronicoList = ciudad.serieDocumentoElectronicoList.OrderByDescending(x => x.esPrincipal).ToList();
            //  transaccion.cliente = new Cliente();
            //     transaccion.cliente.ciudad = new Ciudad();
            serieDocumentoElectronicoList = serieDocumentoElectronicoList.Where(x => x.serie.Substring(0, 1).Equals("0")).ToList();
            transaccion.cliente.ciudad.serieDocumentoElectronicoList = serieDocumentoElectronicoList; 

            /*Se selecciona la primera serie de la lista*/
            transaccion.documentoVenta.serieDocumentoElectronico = serieDocumentoElectronicoList[0];
            transaccion.documentoVenta.serie = Constantes.PREFIJO_NOTA_CREDITO_FACTURA + transaccion.documentoVenta.serieDocumentoElectronico.serie.Substring(1);
            transaccion.documentoVenta.numero = transaccion.documentoVenta.serieDocumentoElectronico.siguienteNumeroNotaCredito.ToString();

            transaccion.observaciones = "";
            transaccion.sustento = sustento;

            ////La fecha de vencimiento 
            String[] fecha = fechaVencimiento.Split('/');
            transaccion.documentoVenta.fechaVencimiento = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0]), 0, 0, 0);

            
            transaccion.documentoVenta.usuario = usuario;

            transaccion.usuario = usuario;


            DocumentoVentaBL documentoVentaBL = new DocumentoVentaBL();
            //Se crea una venta solo para que pueda enviar 
            transaccion.documentoVenta.cliente = transaccion.cliente;

            transaccion.documentoVenta = documentoVentaBL.InsertarNotaCreditoAjustes(transaccion);
            transaccion.documentoVenta.tipoNotaCredito = (DocumentoVenta.TiposNotaCredito)Int32.Parse(transaccion.documentoVenta.cPE_CABECERA_BE.COD_TIP_NC);

            try
            {
                DocumentoVenta documentoVenta = new DocumentoVenta();
                documentoVenta.venta = (Venta)transaccion;
                documentoVenta.idDocumentoVenta = transaccion.documentoVenta.idDocumentoVenta;
                documentoVenta.cliente = transaccion.cliente;
                documentoVenta.usuario = usuario;

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
            Transaccion transaccionExtorno = new Venta();
            transaccionExtorno.documentoVenta = documentoVentaBL.GetDocumentoVenta(documentoVenta);
            transaccionExtorno.moneda = transaccionExtorno.documentoVenta.moneda;

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
            transaccionExtorno = ventaBL.GetNotaIngresoTransaccion(transaccionExtorno, notaIngreso, usuario);

            /*Se obtiene la ciudad para poder cargar las series del documento de pago*/
            Ciudad ciudad = usuario.sedesMPPedidos.Where(s => s.idCiudad == transaccionExtorno.cliente.ciudad.idCiudad).FirstOrDefault();
            List<SerieDocumentoElectronico> serieDocumentoElectronicoList = ciudad.serieDocumentoElectronicoList.OrderByDescending(x => x.esPrincipal).ToList();
            serieDocumentoElectronicoList = serieDocumentoElectronicoList.Where(x => x.serie.Substring(0, 1).Equals("0")).ToList();
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
                this.Session[Constantes.VAR_SESSION_NOTA_CREDITO] = transaccionExtorno;
            }
            return JsonConvert.SerializeObject(transaccionExtorno);
        }


        [HttpPost]
        public String ChangeDetalle(List<DocumentoDetalleJson> cotizacionDetalleJsonList)
        {
            Transaccion transaccionExtorno = (Transaccion)this.Session[Constantes.VAR_SESSION_NOTA_CREDITO];
            IDocumento documento = (Pedido)transaccionExtorno.pedido;
            List<DocumentoDetalle> documentoDetalle = HelperDocumento.updateDocumentoDetalle(documento, cotizacionDetalleJsonList);
            documento.documentoDetalle = documentoDetalle;
            PedidoBL pedidoBL = new PedidoBL();
            pedidoBL.calcularMontosTotales((Pedido)documento);
            transaccionExtorno.pedido = (Pedido)documento;
            this.Session[Constantes.VAR_SESSION_NOTA_CREDITO] = transaccionExtorno;
            return "{\"cantidad\":\"" + documento.documentoDetalle.Count + "\"}";
        }


        public void ChangeInputString()
        {
            Transaccion transaccionExtorno = (Transaccion)this.Session[Constantes.VAR_SESSION_NOTA_CREDITO];
            PropertyInfo propertyInfo = transaccionExtorno.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(transaccionExtorno, this.Request.Params["valor"]);
            this.Session[Constantes.VAR_SESSION_NOTA_CREDITO] = transaccionExtorno;
        }

        public void ChangeFechaEmision()
        {
            Transaccion transaccionExtorno = (Transaccion)this.Session[Constantes.VAR_SESSION_NOTA_CREDITO];
            String[] fechaEmision = this.Request.Params["fechaEmision"].Split('/');
            String[] horaEmision = this.Request.Params["horaEmision"].Split(':');
            transaccionExtorno.documentoVenta.fechaEmision = new DateTime(Int32.Parse(fechaEmision[2]), Int32.Parse(fechaEmision[1]), Int32.Parse(fechaEmision[0]), Int32.Parse(horaEmision[0]), Int32.Parse(horaEmision[1]), 0);
            this.Session[Constantes.VAR_SESSION_NOTA_CREDITO] = transaccionExtorno;
        }

        public void ChangeObservcacionesUsoInterno()
        {
            Transaccion transaccionExtorno = (Transaccion)this.Session[Constantes.VAR_SESSION_NOTA_CREDITO];
            transaccionExtorno.documentoVenta.observacionesUsoInterno = this.Request.Params["observacionesUsoInterno"];
            this.Session[Constantes.VAR_SESSION_NOTA_CREDITO] = transaccionExtorno;
        }

        public void ChangeSerie()
        {

            Transaccion transaccionExtorno = (Transaccion)this.Session[Constantes.VAR_SESSION_NOTA_CREDITO];
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

            
            this.Session[Constantes.VAR_SESSION_NOTA_CREDITO] = transaccionExtorno;
        }

        public String Create()
        {

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            try
            {
                Transaccion transaccion = (Transaccion)this.Session[Constantes.VAR_SESSION_NOTA_CREDITO];

                String[] fecha = this.Request.Params["fechaEmision"].Split('/');
                String[] hora = this.Request.Params["horaEmision"].Split(':');

                transaccion.observaciones = this.Request.Params["observaciones"];
                transaccion.sustento = this.Request.Params["sustento"];

                
                transaccion.documentoVenta.fechaEmision = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0]), Int32.Parse(hora[0]), Int32.Parse(hora[1]), 0);
                ////La fecha de vencimiento es identifica a la fecha de emisión
                transaccion.documentoVenta.fechaVencimiento = transaccion.documentoVenta.fechaEmision.Value;
                ////El clasePedido de pago es al contado
                transaccion.documentoVenta.tipoPago = DocumentoVenta.TipoPago.Contado;
                ////La forma de Pago es Efectivo
                transaccion.documentoVenta.formaPago = DocumentoVenta.FormaPago.Efectivo;
                transaccion.documentoVenta.usuario = usuario;
                
                transaccion.usuario = usuario;
                VentaBL ventaBL = new VentaBL();

                /*Si no se cuenta con movimiento de almacen (nota de ingreso) entonces se debe insertar la venta*/
                if (transaccion.documentoVenta.movimientoAlmacen == null)
                {
                    ventaBL.InsertTransaccionNotaCredito(transaccion);
                }
                else {
                /*Por el contrario si se cuenta con la nota de ingreso se debe actualizar la venta para agregar los datos del documento de referencia*/
                    ventaBL.UpdateTransaccionNotaCredito(transaccion);
                }
                

                DocumentoVentaBL documentoVentaBL = new DocumentoVentaBL();
                //Se crea una venta solo para que pueda enviar 
                transaccion.documentoVenta.venta = new Venta();
                transaccion.documentoVenta.venta.idVenta = transaccion.idVenta;
                transaccion.documentoVenta.venta.documentoReferencia = transaccion.documentoReferencia;
                transaccion.documentoVenta.cliente = transaccion.cliente;
                transaccion.documentoVenta = documentoVentaBL.InsertarNotaCredito(transaccion.documentoVenta);
                transaccion.documentoVenta.tipoNotaCredito = (DocumentoVenta.TiposNotaCredito)Int32.Parse(transaccion.documentoVenta.cPE_CABECERA_BE.COD_TIP_NC);
                return JsonConvert.SerializeObject(transaccion.documentoVenta);
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
                Transaccion venta = (Transaccion)this.Session[Constantes.VAR_SESSION_NOTA_CREDITO];
                DocumentoVenta documentoVenta = new DocumentoVenta();
                documentoVenta.venta = (Venta)venta;
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

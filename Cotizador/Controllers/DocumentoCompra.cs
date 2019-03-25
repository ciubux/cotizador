using BusinessLayer;
using Cotizador.ExcelExport;
using Model;
using Model.ServiceReferencePSE;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Cotizador.Controllers
{
    public class DocumentoCompraController : Controller
    {
        // GET: Factura
        private DocumentoCompra FacturaSession
        {
            get
            {
                DocumentoCompra factura = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaFacturas: factura = (DocumentoCompra)this.Session[Constantes.VAR_SESSION_FACTURA_BUSQUEDA]; break;
                    case Constantes.paginas.MantenimientoFactura: factura = (DocumentoCompra)this.Session[Constantes.VAR_SESSION_FACTURA]; break;
                }
                return factura;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaFacturas: this.Session[Constantes.VAR_SESSION_FACTURA_BUSQUEDA] = value; break;
                    case Constantes.paginas.MantenimientoFactura: this.Session[Constantes.VAR_SESSION_FACTURA] = value; break;
                }
            }
        }


        public String Create()
        {

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            try
            {
                Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_VENTA_VER];



                DocumentoCompra DocumentoCompra = new DocumentoCompra();

                String[] fecha = this.Request.Params["fechaEmision"].Split('/');
                String[] hora = this.Request.Params["horaEmision"].Split(':');
                DocumentoCompra.serie = this.Request.Params["serie"];
                DocumentoCompra.cliente = venta.pedido.cliente;
                DocumentoCompra.venta = venta;


                DocumentoCompra.fechaEmision = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0]), Int32.Parse(hora[0]), Int32.Parse(hora[1]), 0);

                fecha = this.Request.Params["fechaVencimiento"].Split('/');
                DocumentoCompra.fechaVencimiento = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0]));

                DocumentoCompra.tipoPago = (DocumentoCompra.TipoPago)Int32.Parse(this.Request.Params["tipoPago"]);
                DocumentoCompra.formaPago = (DocumentoCompra.FormaPago)Int32.Parse(this.Request.Params["formaPago"]);
                DocumentoCompra.usuario = usuario;




                //DocumentoCompra.MovimentoALmacen = new 
              //  DocumentoCompra.venta = new Venta();
                DocumentoCompra.venta.guiaRemision = new GuiaRemision();
                DocumentoCompra.venta.guiaRemision.idMovimientoAlmacen = Guid.Parse(this.Request.Params["idMovimientoAlmacen"]);

           //     DocumentoCompra.venta.pedido = pedido;

                DocumentoCompra.venta.pedido.numeroReferenciaCliente = this.Request.Params["numeroReferenciaCliente"];
                DocumentoCompra.observaciones = this.Request.Params["observaciones"];

                DocumentoCompraBL DocumentoCompraBL = new DocumentoCompraBL();

                if (DocumentoCompra.venta.pedido.cliente.tipoDocumento == Constantes.TIPO_DOCUMENTO_CLIENTE_RUC)
                    DocumentoCompra.tipoDocumento = DocumentoCompra.TipoDocumento.Factura;
                else if (DocumentoCompra.venta.pedido.cliente.tipoDocumento == Constantes.TIPO_DOCUMENTO_CLIENTE_DNI ||
                    DocumentoCompra.venta.pedido.cliente.tipoDocumento == Constantes.TIPO_DOCUMENTO_CLIENTE_CARNET_EXTRANJERIA)
                    DocumentoCompra.tipoDocumento = DocumentoCompra.TipoDocumento.BoletaVenta;
                else
                    throw new Exception("No se ha identificado el clasePedido de documento electrónico a crear.");


                DocumentoCompra = DocumentoCompraBL.InsertarDocumentoCompra(DocumentoCompra);
              

                return JsonConvert.SerializeObject(DocumentoCompra);
            }
            catch (Exception ex)
            {
                Log log = new Log(ex.ToString(), TipoLog.Error, usuario);
                LogBL logBL = new LogBL();
                logBL.insertLog(log);
                return ex.ToString();
            }
        }

        public String Show()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            try
            {
                DocumentoCompra DocumentoCompra = new DocumentoCompra();
                List<DocumentoCompra> DocumentoCompraList = (List<DocumentoCompra>)this.Session[Constantes.VAR_SESSION_FACTURA_LISTA];
                int idDocumentoCompra = int.Parse(this.Request.Params["idDocumentoCompra"]);
                DocumentoCompra = DocumentoCompraList.Where(d => d.idDocumentoCompra == idDocumentoCompra).First();
                DocumentoCompraBL DocumentoCompraBL = new DocumentoCompraBL();
                
                
                DocumentoCompra = DocumentoCompraBL.GetDocumentoCompra(DocumentoCompra);

                if (DocumentoCompra.tipoDocumento == DocumentoCompra.TipoDocumento.NotaCrédito)
                {
                    DocumentoCompra.tipoNotaCredito = (DocumentoCompra.TiposNotaCredito)Int32.Parse(DocumentoCompra.cPE_CABECERA_BE.COD_TIP_NC);
                }
                else if (DocumentoCompra.tipoDocumento == DocumentoCompra.TipoDocumento.NotaDébito)
                {
                    DocumentoCompra.tipoNotaDebito = (DocumentoCompra.TiposNotaDebito)Int32.Parse(DocumentoCompra.cPE_CABECERA_BE.COD_TIP_ND);
                }


                this.Session[Constantes.VAR_SESSION_FACTURA_VER] = DocumentoCompra;
                return JsonConvert.SerializeObject(DocumentoCompra);
            }
            catch (Exception ex)
            {
                Log log = new Log(ex.ToString(), TipoLog.Error, usuario);
                LogBL logBL = new LogBL();
                logBL.insertLog(log);
                return ex.ToString();
            }

        }



 /*       public String iniciarRefacturacion()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            VentaBL ventaBL = new VentaBL();
            Transaccion transaccion = new Transaccion();

            transaccion.DocumentoCompra = (DocumentoCompra)this.Session[Constantes.VAR_SESSION_FACTURA_VER];
            transaccion.DocumentoCompra.venta = null;
            transaccion.DocumentoCompra.tipoDocumento = DocumentoCompra.TipoDocumento.Factura;
            transaccion.DocumentoCompra.idDocumentoCompra = Guid.Parse(Request["idDocumentoCompra"].ToString());

            transaccion.documentoReferencia = new DocumentoReferencia();
            transaccion.documentoReferencia.tipoDocumento = (DocumentoCompra.TipoDocumento)Int32.Parse(transaccion.DocumentoCompra.cPE_CABECERA_BE.TIP_CPE);
            String[] fechaEmisionArray = transaccion.DocumentoCompra.cPE_CABECERA_BE.FEC_EMI.Split('-');
            transaccion.documentoReferencia.fechaEmision = new DateTime(Int32.Parse(fechaEmisionArray[0]), Int32.Parse(fechaEmisionArray[1]), Int32.Parse(fechaEmisionArray[2]));
            
            transaccion.documentoReferencia.serie = transaccion.DocumentoCompra.cPE_CABECERA_BE.SERIE;
            transaccion.documentoReferencia.numero = transaccion.DocumentoCompra.cPE_CABECERA_BE.CORRELATIVO;

            transaccion = ventaBL.GetPlantillaVenta(transaccion, usuario);
            if (transaccion.tipoErrorCrearTransaccion == Venta.TiposErrorCrearTransaccion.NoExisteError)
            {
               // venta.tipoNotaCredito = (DocumentoCompra.TiposNotaCredito)Int32.Parse(Request["tipoNotaCredito"].ToString());
                transaccion.DocumentoCompra.fechaEmision = DateTime.Now;
                transaccion.DocumentoCompra.fechaVencimiento = transaccion.DocumentoCompra.fechaEmision.Value;
                //venta.DocumentoCompra.formaPago
                //Temporal
                Pedido pedido = transaccion.pedido;
                pedido.ciudadASolicitar = new Ciudad();

                PedidoBL pedidoBL = new PedidoBL();
                pedidoBL.calcularMontosTotales(pedido);
                this.Session[Constantes.VAR_SESSION_FACTURA] = transaccion;
            }
            return JsonConvert.SerializeObject(transaccion);

        }*/

        [HttpPost]
        public String ChangeDetalle(List<DocumentoDetalleJson> cotizacionDetalleJsonList)
        {
            Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_FACTURA];
            IDocumento documento = (Pedido)venta.pedido;
            List<DocumentoDetalle> documentoDetalle = HelperDocumento.updateDocumentoDetalle(documento, cotizacionDetalleJsonList);
            documento.documentoDetalle = documentoDetalle;
            PedidoBL pedidoBL = new PedidoBL();
            pedidoBL.calcularMontosTotales((Pedido)documento);
            venta.pedido = (Pedido)documento;
            this.Session[Constantes.VAR_SESSION_FACTURA] = venta;
            return "{\"cantidad\":\"" + documento.documentoDetalle.Count + "\"}";
        }

        public void ChangeBuscarSedesGrupoCliente()
        {
            DocumentoCompra factura = this.FacturaSession;
            if (this.Request.Params["buscarSedesGrupoCliente"] != null && Int32.Parse(this.Request.Params["buscarSedesGrupoCliente"]) == 1)
            {
                factura.buscarSedesGrupoCliente = true;
            }
            else
            {
                factura.buscarSedesGrupoCliente = false;
            }

            this.FacturaSession = factura;
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
            Compra compra = (Compra)this.Session[Constantes.VAR_SESSION_NOTA_CREDITO];
            String[] fechaEmision = this.Request.Params["fechaEmision"].Split('/');
            String[] horaEmision = this.Request.Params["horaEmision"].Split(':');
            compra.documentoCompra.fechaEmision = new DateTime(Int32.Parse(fechaEmision[2]), Int32.Parse(fechaEmision[1]), Int32.Parse(fechaEmision[0]), Int32.Parse(horaEmision[0]), Int32.Parse(horaEmision[1]), 0);
            this.Session[Constantes.VAR_SESSION_NOTA_CREDITO_COMPRA] = compra;
        }


        public ActionResult Crear()
        {
            Compra compra = (Compra)this.Session[Constantes.VAR_SESSION_FACTURA];
            //   venta.tipoNotaCredito = venta.DocumentoCompra.tipoNotaCredito;
            ViewBag.compra = compra;
            //ViewBag.tipoNotaCredito = (int)venta.tipoNotaCredito;

            ViewBag.fechaEmision = compra.documentoCompra.fechaEmision.Value.ToString(Constantes.formatoFecha);
            ViewBag.horaEmision = compra.documentoCompra.fechaEmision.Value.ToString(Constantes.formatoHora);

            return View();
        }




        public String ConfirmarCreacion()
        {

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            try
            {
                Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_VENTA_VER];
                DocumentoCompra DocumentoCompra = new DocumentoCompra();
                DocumentoCompra.venta = venta;
                DocumentoCompra.idDocumentoCompra  = int.Parse(this.Request.Params["idDocumentoCompra"]);
                DocumentoCompra.cliente = venta.pedido.cliente;
                DocumentoCompra.usuario = usuario;

                if (DocumentoCompra.venta.pedido.cliente.tipoDocumento == Constantes.TIPO_DOCUMENTO_CLIENTE_RUC)
                    DocumentoCompra.tipoDocumento = DocumentoCompra.TipoDocumento.Factura;
                else if (DocumentoCompra.venta.pedido.cliente.tipoDocumento == Constantes.TIPO_DOCUMENTO_CLIENTE_DNI ||
                     DocumentoCompra.venta.pedido.cliente.tipoDocumento == Constantes.TIPO_DOCUMENTO_CLIENTE_CARNET_EXTRANJERIA)
                    DocumentoCompra.tipoDocumento = DocumentoCompra.TipoDocumento.BoletaVenta;
                else
                    throw new Exception("No se ha identificado el clasePedido de documento electrónico a crear.");

                DocumentoCompraBL DocumentoCompraBL = new DocumentoCompraBL();


                CPE_RESPUESTA_BE cPE_RESPUESTA_BE = DocumentoCompraBL.procesarCPE(DocumentoCompra);

                var otmp = new
                {
                    CPE_RESPUESTA_BE = cPE_RESPUESTA_BE,
                    serieNumero = DocumentoCompra.serieNumero,
                    idDocumentoCompra = DocumentoCompra.idDocumentoCompra
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


        public String ConfirmarCreacionFacturaConsolidada()
        {

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            try
            {
                List<Guid> movimientoAlmacenIdList = (List<Guid>)this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA_LISTA_IDS];

                Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_VENTA_VER];
                DocumentoCompra DocumentoCompra = new DocumentoCompra();
                DocumentoCompra.venta = venta;
                DocumentoCompra.idDocumentoCompra = int.Parse(this.Request.Params["idDocumentoCompra"]);
                DocumentoCompra.cliente = venta.pedido.cliente;
                DocumentoCompra.usuario = usuario;

                if (DocumentoCompra.venta.pedido.cliente.tipoDocumento == Constantes.TIPO_DOCUMENTO_CLIENTE_RUC)
                    DocumentoCompra.tipoDocumento = DocumentoCompra.TipoDocumento.Factura;
                else if (DocumentoCompra.venta.pedido.cliente.tipoDocumento == Constantes.TIPO_DOCUMENTO_CLIENTE_DNI ||
                     DocumentoCompra.venta.pedido.cliente.tipoDocumento == Constantes.TIPO_DOCUMENTO_CLIENTE_CARNET_EXTRANJERIA)
                    DocumentoCompra.tipoDocumento = DocumentoCompra.TipoDocumento.BoletaVenta;
                else
                    throw new Exception("No se ha identificado el clasePedido de documento electrónico a crear.");

                DocumentoCompraBL DocumentoCompraBL = new DocumentoCompraBL();


                CPE_RESPUESTA_BE cPE_RESPUESTA_BE = DocumentoCompraBL.procesarCPE(DocumentoCompra, movimientoAlmacenIdList);

                var otmp = new
                {
                    CPE_RESPUESTA_BE = cPE_RESPUESTA_BE,
                    serieNumero = DocumentoCompra.serieNumero,
                    idDocumentoCompra = DocumentoCompra.idDocumentoCompra
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











        [HttpGet]
        public ActionResult Index()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BusquedaFacturas;

            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                /*
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                if (!usuario.tomaPedidos && !usuario.apruebaPedidos)
                {
                    return RedirectToAction("Login", "Account");
                }*/
            }

            if (this.FacturaSession == null)
            {
                instanciarfacturaBusqueda();
            }

            
           
             int existeCliente = 0;
            //  if (cotizacion.cliente.idCliente != Guid.Empty || cotizacion.grupo.idGrupo != Guid.Empty)
            if (this.FacturaSession.cliente.idCliente != Guid.Empty)
            {
                existeCliente = 1;
            }

            GuiaRemision guiaRemision = new GuiaRemision();
            guiaRemision.motivoTraslado = GuiaRemision.motivosTraslado.Venta;

            ViewBag.DocumentoCompra = this.FacturaSession;
            ViewBag.fechaEmisionDesde = this.FacturaSession.fechaEmisionDesde.ToString(Constantes.formatoFecha);
            ViewBag.fechaEmisionHasta = this.FacturaSession.fechaEmisionHasta.ToString(Constantes.formatoFecha);
            ViewBag.DocumentoCompraList = this.Session[Constantes.VAR_SESSION_FACTURA_LISTA];
            ViewBag.existeCliente = existeCliente;
            ViewBag.pagina = (int)Constantes.paginas.BusquedaFacturas;

            ViewBag.Si = Constantes.MENSAJE_SI;
            ViewBag.No = Constantes.MENSAJE_NO;

            //ViewBag.descuentoList = Constantes.DESCUENTOS_LIST;
            return View();

        }


        public String SearchClientes()
        {
            String data = this.Request.Params["data[q]"];
            ClienteBL clienteBL = new ClienteBL();
            DocumentoCompra factura = (DocumentoCompra)this.Session[Constantes.VAR_SESSION_FACTURA_BUSQUEDA];
            return clienteBL.getCLientesBusqueda(data, factura.ciudad.idCiudad);
        }

        public void CleanBusqueda()
        {
            instanciarfacturaBusqueda();
        }



    /*    [HttpGet]
        public ActionResult ExportLastSearchExcel()
        {
            List<DocumentoCompra> list = (List<DocumentoCompra>)this.Session[Constantes.VAR_SESSION_FACTURA_LISTA];

            Facturasearch excel = new Facturasearch();
            return excel.generateExcel(list);
        }*/

        public String Search()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaFacturas;

            String[] desde = this.Request.Params["fechaEmisionDesde"].Split('/');
            this.FacturaSession.fechaEmisionDesde = new DateTime(Int32.Parse(desde[2]), Int32.Parse(desde[1]), Int32.Parse(desde[0]));

            String[] hasta = this.Request.Params["fechaEmisionHasta"].Split('/');
            this.FacturaSession.fechaEmisionHasta = new DateTime(Int32.Parse(hasta[2]), Int32.Parse(hasta[1]), Int32.Parse(hasta[0]));
            

            if (this.Request.Params["numero"] == null || this.Request.Params["numero"].Trim().Length == 0)
            {
                this.FacturaSession.numero = "0";
            }
            else
            {
                this.FacturaSession.numero = this.Request.Params["numero"];
            }


            if (this.Request.Params["numeroPedido"] == null || this.Request.Params["numeroPedido"].Trim().Length == 0)
            {
                this.FacturaSession.pedido.numeroPedido = 0;
            }
            else
            {
                this.FacturaSession.pedido.numeroPedido = int.Parse(this.Request.Params["numeroPedido"]);
            }


            if (this.Request.Params["numeroGuiaRemision"] == null || this.Request.Params["numeroGuiaRemision"].Trim().Length == 0)
            {
                this.FacturaSession.guiaRemision.numeroDocumento = 0;
            }
            else
            {
                this.FacturaSession.guiaRemision.numeroDocumento = int.Parse(this.Request.Params["numeroGuiaRemision"]);
            }

        
            this.FacturaSession.sku = this.Request.Params["sku"];
        



            this.FacturaSession.estadoDocumentoSunatBusqueda = (DocumentoCompra.EstadosDocumentoSunatBusqueda)Int32.Parse(this.Request.Params["estadoDocumentoSunatBusqueda"]);
            this.FacturaSession.tipoDocumento = (DocumentoCompra.TipoDocumento)Int32.Parse(this.Request.Params["tipoDocumento"]);

            DocumentoCompraBL DocumentoCompraBL = new DocumentoCompraBL();

            List<DocumentoCompra> DocumentoCompraList = DocumentoCompraBL.GetFacturas(this.FacturaSession);

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

    /*        foreach (DocumentoCompra DocumentoCompra in DocumentoCompraList)
            {
                DocumentoCompra.usuario.apruebaAnulaciones = usuario.apruebaAnulaciones;
                DocumentoCompra.usuario.creaNotasCredito = usuario.creaNotasCredito;
            }*/


            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_FACTURA_LISTA] = DocumentoCompraList;
            //Se retorna la cantidad de elementos encontrados

            return JsonConvert.SerializeObject(DocumentoCompraList);
            //return pedidoList.Count();
        }


        public String SolicitarAnulacion()
        {
            DocumentoCompraBL DocumentoCompraBL = new DocumentoCompraBL();
            List<DocumentoCompra> DocumentoCompraList = (List<DocumentoCompra>)this.Session[Constantes.VAR_SESSION_FACTURA_LISTA];
            int idDocumentoCompra = int.Parse(this.Request.Params["idDocumentoCompra"]);
            //  Guid idDocumentoCompra = this.Request.Params["idDocumentoCompra"]);

            DocumentoCompra DocumentoCompra = DocumentoCompraList.Where(d => d.idDocumentoCompra == idDocumentoCompra).FirstOrDefault();
            DocumentoCompra.comentarioAnulado = this.Request.Params["comentarioAnulado"];
            DocumentoCompra.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            //DocumentoCompra.tipoDocumento = DocumentoCompra.TipoDocumento.Factura;
            DocumentoCompraBL.anularDocumentoCompra(DocumentoCompra);
            return JsonConvert.SerializeObject(DocumentoCompra);
        }

        public String AprobarAnulacion()
        {
            DocumentoCompraBL DocumentoCompraBL = new DocumentoCompraBL();
            /*  List<DocumentoCompra> DocumentoCompraList = (List<DocumentoCompra>)this.Session[Constantes.VAR_SESSION_FACTURA_LISTA];
              Guid idDocumentoCompra = Guid.Parse(this.Request.Params["idDocumentoCompra"]);
              DocumentoCompra DocumentoCompra = DocumentoCompraList.Where(d => d.idDocumentoCompra == idDocumentoCompra).FirstOrDefault();
  */
            DocumentoCompra DocumentoCompra = new DocumentoCompra();
            DocumentoCompra.idDocumentoCompra = int.Parse(this.Request.Params["idDocumentoCompra"]);
            DocumentoCompra = DocumentoCompraBL.GetDocumentoCompra(DocumentoCompra);
            DocumentoCompra.comentarioAprobacionAnulacion = this.Request.Params["comentarioAprobacionAnulacion"];
            DocumentoCompra.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            
            DocumentoCompraBL.aprobarAnulacionDocumentoCompra(DocumentoCompra, DocumentoCompra.usuario);
            return JsonConvert.SerializeObject(DocumentoCompra);

        }

        



        public String descargarArchivoDocumentoCompra()
        {
            DocumentoCompraBL DocumentoCompraBL = new DocumentoCompraBL();
            List<DocumentoCompra> DocumentoCompraList = (List<DocumentoCompra>)this.Session[Constantes.VAR_SESSION_FACTURA_LISTA];
            int idDocumentoCompra = int.Parse(this.Request.Params["idDocumentoCompra"]);

            String ruta = String.Empty;


            DocumentoCompra DocumentoCompra = DocumentoCompraList.Where(d => d.idDocumentoCompra == idDocumentoCompra).FirstOrDefault();
            //DocumentoCompra.tipoDocumento = DocumentoCompra.TipoDocumento.Factura;
            DocumentoCompra = DocumentoCompraBL.GetDocumentoCompra(DocumentoCompra);
            DocumentoCompra = DocumentoCompraBL.descargarArchivoDocumentoCompra(DocumentoCompra);

            try {
                DocumentoCompra.cpeFile = Encoding.GetEncoding(28591).GetBytes(DocumentoCompra.rPTA_DOC_TRIB_BE.DOC_TRIB_XML);
                DocumentoCompra.cdrFile = Encoding.GetEncoding(28591).GetBytes(DocumentoCompra.rPTA_DOC_TRIB_BE.DOC_TRIB_RPTA);
            }
            catch (Exception e)
            {
                DocumentoCompra.cpeFile = null;
                DocumentoCompra.cdrFile = null;
            }

            var documentos = new
            {
                pdf = DocumentoCompra.rPTA_DOC_TRIB_BE.DOC_TRIB_PDF,
                cpe = DocumentoCompra.cpeFile,
                cdr = DocumentoCompra.cdrFile,
                nombreArchivo = 
                        DocumentoCompra.cPE_CABECERA_BE.NRO_DOC_EMI+"-"+ 
                        DocumentoCompra.cPE_CABECERA_BE.TIP_CPE+"-"+
                        DocumentoCompra.cPE_CABECERA_BE.SERIE + "-" +
                        DocumentoCompra.cPE_CABECERA_BE.CORRELATIVO
            };

            return JsonConvert.SerializeObject(documentos);

        }


        private void instanciarfacturaBusqueda()
        {
            DocumentoCompra DocumentoCompra = new DocumentoCompra();
            DateTime fechaDesde = DateTime.Now.AddDays(-Constantes.DIAS_DESDE_BUSQUEDA);
            DateTime fechaHasta = DateTime.Now.AddDays(1);
            DocumentoCompra.fechaEmisionDesde = new DateTime(fechaDesde.Year, fechaDesde.Month, fechaDesde.Day, 0, 0, 0);
            DocumentoCompra.fechaEmisionHasta = new DateTime(fechaHasta.Year, fechaHasta.Month, fechaHasta.Day, 23, 59, 59);
            DocumentoCompra.buscarSedesGrupoCliente = false;

            DocumentoCompra.tipoNotaCredito = DocumentoCompra.TiposNotaCredito.AnulacionOperacion;

            DocumentoCompra.estadoDocumentoSunatBusqueda = DocumentoCompra.EstadosDocumentoSunatBusqueda.TodosAceptados;

            DocumentoCompra.serie = "0";
            DocumentoCompra.numero = "0";
            DocumentoCompra.pedido = new Pedido();
            DocumentoCompra.guiaRemision = new GuiaRemision();
            DocumentoCompra.solicitadoAnulacion = false;

            DocumentoCompra.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            DocumentoCompra.cliente = new Cliente();
            DocumentoCompra.cliente.idCliente = Guid.Empty;

            if (DocumentoCompra.usuario.sedesMPDocumentosVenta.Count == 1)
            {
                DocumentoCompra.ciudad = DocumentoCompra.usuario.sedesMPDocumentosVenta[0];
                //DocumentoCompra.ciudad.idCiudad = Guid.Empty;
            }
            else
            {
                DocumentoCompra.ciudad = new Ciudad();
                DocumentoCompra.ciudad.idCiudad = Guid.Empty;
            }


            

            




            //pedidoTmp.usuarioBusqueda = pedidoTmp.usuario;
            this.Session[Constantes.VAR_SESSION_FACTURA_BUSQUEDA] = DocumentoCompra;
            this.Session[Constantes.VAR_SESSION_FACTURA_LISTA] = new List<DocumentoCompra>();

            /*
            DocumentoCompra.tipoPago = DocumentoCompra.TipoPago.Contado;
            DocumentoCompra.formaPago = DocumentoCompra.FormaPago.NoAsignado;
            DocumentoCompra.fechaEmision = DateTime.Now;
            */


        }

       

        public String GetCliente()
        {
            DocumentoCompra factura = this.FacturaSession;
            Guid idCliente = Guid.Parse(Request["idCliente"].ToString());
            ClienteBL clienteBl = new ClienteBL();
            factura.cliente = clienteBl.getCliente(idCliente);
            String resultado = JsonConvert.SerializeObject(factura.cliente);
            this.FacturaSession = factura;
            return resultado;
        }

        public String GetGrupoCliente()
        {
            DocumentoCompra factura = this.FacturaSession;
            int idGrupoCliente = int.Parse(Request["idGrupoCliente"].ToString());

            factura.idGrupoCliente = idGrupoCliente;

            this.FacturaSession = factura;
            return "";
        }

        public String ChangeIdCiudad()
        {
            Guid idCiudad = Guid.Empty;
            if (this.Request.Params["idCiudad"] != null && !this.Request.Params["idCiudad"].Equals(""))
            {
                idCiudad = Guid.Parse(this.Request.Params["idCiudad"]);
            }
            CiudadBL ciudadBL = new CiudadBL();
            Ciudad ciudad = ciudadBL.getCiudad(idCiudad);
            this.FacturaSession.ciudad = ciudad;
            return JsonConvert.SerializeObject(ciudad);
        }

        public void ChangeFechaEmisionDesde()
        {
            String[] desde = this.Request.Params["fechaEmisionDesde"].Split('/');
            this.FacturaSession.fechaEmisionDesde = new DateTime(Int32.Parse(desde[2]), Int32.Parse(desde[1]), Int32.Parse(desde[0]));
        }

        public void ChangeFechaEmisionHasta()
        {
            String[] desde = this.Request.Params["fechaEmisionHasta"].Split('/');
            this.FacturaSession.fechaEmisionHasta = new DateTime(Int32.Parse(desde[2]), Int32.Parse(desde[1]), Int32.Parse(desde[0]));
        }

        public void ChangeSolicitadoAnulacion()
        {
            this.FacturaSession.solicitadoAnulacion = Int32.Parse(this.Request.Params["solicitadoAnulacion"]) == 1;
        }

        









        [HttpGet]
        public ActionResult PreLoad()
        {
         
            if (this.Session["usuario"] == null)
            {
                return RedirectToAction("Login", "Account");
            }



       

          /*  FacturaBL facturaBL = new FacturaBL();
            facturaBL.testFacturaElectronica();
*/


            return View();

        }



        [HttpPost]
        public ActionResult Load(HttpPostedFileBase file)
        {

            if (file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                file.SaveAs(path);
            }


            HSSFWorkbook hssfwb;

            FacturaBL facturaBL = new FacturaBL();
       //     facturaBL.truncateFacturaStaging();

            hssfwb = new HSSFWorkbook(file.InputStream);

            int numero = 0;

            for (int j = 0; j < 8; j++)
            {

                ISheet sheet = hssfwb.GetSheetAt(j);
                int row = 1;
                //sheet.LastRowNum

                int cantidad = Int32.Parse(Request["cantidad"].ToString());
                //  if (cantidad == 0)
                cantidad = sheet.LastRowNum;




                for (row = 3; row <= cantidad; row++)
                {
                    if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                    {
                        try
                        {
                            FacturaStaging facturaStaging = new FacturaStaging();
                            //A
                            facturaStaging.tipoDocumento = sheet.GetRow(row).GetCell(0).ToString();
                            //B
                            facturaStaging.numeroDocumento = Convert.ToInt32(sheet.GetRow(row).GetCell(1).NumericCellValue);
                            //C
                            facturaStaging.fecha = sheet.GetRow(row).GetCell(2).DateCellValue;
                            //D
                            facturaStaging.codigoCliente = sheet.GetRow(row).GetCell(3).ToString();
                            //E
                            //K
                            facturaStaging.valorVenta = Convert.ToDecimal(sheet.GetRow(row).GetCell(8).NumericCellValue);
                            facturaStaging.igv = Convert.ToDecimal(sheet.GetRow(row).GetCell(9).NumericCellValue);
                            facturaStaging.total = Convert.ToDecimal(sheet.GetRow(row).GetCell(10).NumericCellValue);
                            facturaStaging.observacion = sheet.GetRow(row).GetCell(11).ToString();

                            facturaStaging.fechaVencimiento = sheet.GetRow(row).GetCell(13).DateCellValue;
                            try
                            {
                                if (sheet.GetRow(row).GetCell(14) == null)
                                    facturaStaging.ruc = null;
                                else
                                    facturaStaging.ruc = sheet.GetRow(row).GetCell(14).ToString();
                            }
                            catch (Exception ex)
                            {
                                facturaStaging.ruc = sheet.GetRow(row).GetCell(14).NumericCellValue.ToString();
                            }

                            //F
                            facturaStaging.razonSocial = sheet.GetRow(row).GetCell(15).StringCellValue;


                            switch (j)
                            {
                                case 0: facturaStaging.sede = "L"; break;
                                case 1: facturaStaging.sede = "A"; break;
                                case 2: facturaStaging.sede = "C"; break;
                                case 3: facturaStaging.sede = "H"; break;
                                case 4: facturaStaging.sede = "O"; break;
                                case 5: facturaStaging.sede = "P"; break;
                                case 6: facturaStaging.sede = "Q"; break;
                                case 7: facturaStaging.sede = "T"; break;
                            }

                            if (facturaStaging.tipoDocumento.Trim().Equals("F"))
                            {
                                numero++;
                                facturaStaging.numero = numero;
                            }
                            else
                            {
                                facturaStaging.numero = 0;
                            }


                            facturaBL.setFacturaStaging(facturaStaging);

                        }
                        catch (Exception ex)
                        {
                            Usuario usuario = (Usuario)this.Session["usuario"];
                            Log log = new Log(ex.ToString(), TipoLog.Error, usuario);
                            LogBL logBL = new LogBL();
                            logBL.insertLog(log);
                        }
                    }
                }

                row = row;
            }

            //       facturaBL.mergeClienteStaging();

            return RedirectToAction("Index", "Cotizacion");

        }
    }
}
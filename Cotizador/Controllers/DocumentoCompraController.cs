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
    public class DocumentoCompraController : ParentController
    {


        private DocumentoCompra documentoCompraSession
        {
            get
            {
                DocumentoCompra documentoCompra = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaDocumentosCompra: documentoCompra = (DocumentoCompra)this.Session[Constantes.VAR_SESSION_DOCUMENTO_COMPRA_BUSQUEDA]; break;
                }
                return documentoCompra;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaDocumentosCompra: this.Session[Constantes.VAR_SESSION_DOCUMENTO_COMPRA_BUSQUEDA] = value; break;
                }
            }
        }


        [HttpPost]
        public String Create()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            try
            {
                Compra compra = (Compra)this.Session[Constantes.VAR_SESSION_COMPRA_VER];
                DocumentoCompra documentoCompra = new DocumentoCompra();
                String[] fecha = this.Request.Params["fechaEmision"].Split('/');
                String[] hora = this.Request.Params["horaEmision"].Split(':');
                documentoCompra.serie = this.Request.Params["serie"];
                documentoCompra.numero = this.Request.Params["numero"];
                documentoCompra.proveedor = compra.pedido.proveedor;
                documentoCompra.fechaEmision = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0]), Int32.Parse(hora[0]), Int32.Parse(hora[1]), 0);
                fecha = this.Request.Params["fechaVencimiento"].Split('/');
                documentoCompra.fechaVencimiento = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0]));
                documentoCompra.tipoPago = (DocumentoCompra.TipoPago)Int32.Parse(this.Request.Params["tipoPago"]);
                documentoCompra.formaPago = (DocumentoCompra.FormaPago)Int32.Parse(this.Request.Params["formaPago"]);
                documentoCompra.usuario = usuario;
                documentoCompra.compra = compra;

                documentoCompra.compra.notaIngreso = new NotaIngreso();
                documentoCompra.compra.notaIngreso.idMovimientoAlmacen = Guid.Parse(this.Request.Params["idMovimientoAlmacen"]);
                documentoCompra.compra.pedido.numeroReferenciaCliente = this.Request.Params["numeroReferenciaCliente"];
                documentoCompra.observaciones = this.Request.Params["observaciones"];

                DocumentoCompraBL DocumentoCompraBL = new DocumentoCompraBL();

                if (documentoCompra.compra.pedido.proveedor.tipoDocumento == Constantes.TIPO_DOCUMENTO_CLIENTE_RUC)
                    documentoCompra.tipoDocumento = DocumentoCompra.TipoDocumento.Factura;
                else if (documentoCompra.compra.pedido.proveedor.tipoDocumento == Constantes.TIPO_DOCUMENTO_CLIENTE_DNI ||
                    documentoCompra.compra.pedido.proveedor.tipoDocumento == Constantes.TIPO_DOCUMENTO_CLIENTE_CARNET_EXTRANJERIA)
                    documentoCompra.tipoDocumento = DocumentoCompra.TipoDocumento.BoletaVenta;
                else
                    throw new Exception("No se ha identificado el clasePedido de documento electrónico a crear.");


                documentoCompra = DocumentoCompraBL.InsertarDocumentoCompra(documentoCompra);
              

                return JsonConvert.SerializeObject(documentoCompra);
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
                Compra compra = (Compra)this.Session[Constantes.VAR_SESSION_COMPRA_VER];
                DocumentoCompra documentoCompra = new DocumentoCompra();
                documentoCompra.compra = compra;
                documentoCompra.idDocumentoCompra = int.Parse(this.Request.Params["idDocumentoCompra"]);
                documentoCompra.proveedor = compra.pedido.proveedor;
                documentoCompra.usuario = usuario;

                if (documentoCompra.compra.pedido.proveedor.tipoDocumento == Constantes.TIPO_DOCUMENTO_CLIENTE_RUC)
                    documentoCompra.tipoDocumento = DocumentoCompra.TipoDocumento.Factura;
                else if (documentoCompra.compra.pedido.proveedor.tipoDocumento == Constantes.TIPO_DOCUMENTO_CLIENTE_DNI ||
                     documentoCompra.compra.pedido.proveedor.tipoDocumento == Constantes.TIPO_DOCUMENTO_CLIENTE_CARNET_EXTRANJERIA)
                    documentoCompra.tipoDocumento = DocumentoCompra.TipoDocumento.BoletaVenta;
                else
                    throw new Exception("No se ha identificado el clasePedido de documento electrónico a crear.");

                DocumentoCompraBL DocumentoCompraBL = new DocumentoCompraBL();


                CPE_RESPUESTA_COMPRA cPE_RESPUESTA_COMPRA = DocumentoCompraBL.procesarCPE(documentoCompra);

                var otmp = new
                {
                    CPE_RESPUESTA_COMPRA = cPE_RESPUESTA_COMPRA,
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


        [HttpPost]
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
                    DocumentoCompra.tipoNotaCredito = (DocumentoCompra.TiposNotaCredito)Int32.Parse(DocumentoCompra.cPE_CABECERA_COMPRA.COD_TIP_NC);
                }
                else if (DocumentoCompra.tipoDocumento == DocumentoCompra.TipoDocumento.NotaDébito)
                {
                    DocumentoCompra.tipoNotaDebito = (DocumentoCompra.TiposNotaDebito)Int32.Parse(DocumentoCompra.cPE_CABECERA_COMPRA.COD_TIP_ND);
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


        public String Search()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaDocumentosCompra;

            String[] desde = this.Request.Params["fechaEmisionDesde"].Split('/');
            this.documentoCompraSession.fechaEmisionDesde = new DateTime(Int32.Parse(desde[2]), Int32.Parse(desde[1]), Int32.Parse(desde[0]));

            String[] hasta = this.Request.Params["fechaEmisionHasta"].Split('/');
            this.documentoCompraSession.fechaEmisionHasta = new DateTime(Int32.Parse(hasta[2]), Int32.Parse(hasta[1]), Int32.Parse(hasta[0]));


            if (this.Request.Params["numero"] == null || this.Request.Params["numero"].Trim().Length == 0)
            {
                this.documentoCompraSession.numero = "0";
            }
            else
            {
                this.documentoCompraSession.numero = this.Request.Params["numero"];
            }


            if (this.Request.Params["numeroPedido"] == null || this.Request.Params["numeroPedido"].Trim().Length == 0)
            {
                this.documentoCompraSession.pedido.numeroPedido = 0;
            }
            else
            {
                this.documentoCompraSession.pedido.numeroPedido = int.Parse(this.Request.Params["numeroPedido"]);
            }


            if (this.Request.Params["numeroGuiaRemision"] == null || this.Request.Params["numeroGuiaRemision"].Trim().Length == 0)
            {
                this.documentoCompraSession.guiaRemision.numeroDocumento = 0;
            }
            else
            {
                this.documentoCompraSession.guiaRemision.numeroDocumento = int.Parse(this.Request.Params["numeroGuiaRemision"]);
            }


            this.documentoCompraSession.sku = this.Request.Params["sku"];




            this.documentoCompraSession.estadoDocumentoSunatBusqueda = (DocumentoCompra.EstadosDocumentoSunatBusqueda)Int32.Parse(this.Request.Params["estadoDocumentoSunatBusqueda"]);
            this.documentoCompraSession.tipoDocumento = (DocumentoCompra.TipoDocumento)Int32.Parse(this.Request.Params["tipoDocumento"]);

            DocumentoCompraBL documentoCompraBL = new DocumentoCompraBL();

            List<DocumentoCompra> documentoCompraList = documentoCompraBL.GetDocumentosCompra(this.documentoCompraSession);

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];



            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_DOCUMENTO_COMPRA_LISTA] = documentoCompraList;
            //Se retorna la cantidad de elementos encontrados

            return JsonConvert.SerializeObject(documentoCompraList);
            //return pedidoList.Count();
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
                transaccion.documentoReferencia.tipoDocumento = (DocumentoCompra.TipoDocumento)Int32.Parse(transaccion.DocumentoCompra.cPE_CABECERA_COMPRA.TIP_CPE);
                String[] fechaEmisionArray = transaccion.DocumentoCompra.cPE_CABECERA_COMPRA.FEC_EMI.Split('-');
                transaccion.documentoReferencia.fechaEmision = new DateTime(Int32.Parse(fechaEmisionArray[0]), Int32.Parse(fechaEmisionArray[1]), Int32.Parse(fechaEmisionArray[2]));

                transaccion.documentoReferencia.serie = transaccion.DocumentoCompra.cPE_CABECERA_COMPRA.SERIE;
                transaccion.documentoReferencia.numero = transaccion.DocumentoCompra.cPE_CABECERA_COMPRA.CORRELATIVO;

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
            DocumentoCompra factura = this.documentoCompraSession;
            if (this.Request.Params["buscarSedesGrupoCliente"] != null && Int32.Parse(this.Request.Params["buscarSedesGrupoCliente"]) == 1)
            {
                factura.buscarSedesGrupoCliente = true;
            }
            else
            {
                factura.buscarSedesGrupoCliente = false;
            }

            this.documentoCompraSession = factura;
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




      

        public String ConfirmarCreacionFacturaConsolidada()
        {

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            try
            {
                List<Guid> movimientoAlmacenIdList = (List<Guid>)this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA_LISTA_IDS];

                Compra compra = (Compra)this.Session[Constantes.VAR_SESSION_COMPRA_VER];
                DocumentoCompra documentoCompra = new DocumentoCompra();
                documentoCompra.compra = compra;
                documentoCompra.idDocumentoCompra = int.Parse(this.Request.Params["idDocumentoCompra"]);
                documentoCompra.proveedor = compra.pedido.proveedor;
                documentoCompra.usuario = usuario;

                if (documentoCompra.compra.pedido.cliente.tipoDocumento == Constantes.TIPO_DOCUMENTO_CLIENTE_RUC)
                    documentoCompra.tipoDocumento = DocumentoCompra.TipoDocumento.Factura;
                else if (documentoCompra.compra.pedido.cliente.tipoDocumento == Constantes.TIPO_DOCUMENTO_CLIENTE_DNI ||
                     documentoCompra.compra.pedido.cliente.tipoDocumento == Constantes.TIPO_DOCUMENTO_CLIENTE_CARNET_EXTRANJERIA)
                    documentoCompra.tipoDocumento = DocumentoCompra.TipoDocumento.BoletaVenta;
                else
                    throw new Exception("No se ha identificado el clasePedido de documento electrónico a crear.");

                DocumentoCompraBL DocumentoCompraBL = new DocumentoCompraBL();


                CPE_RESPUESTA_COMPRA cPE_RESPUESTA_COMPRA = DocumentoCompraBL.procesarCPE(documentoCompra, movimientoAlmacenIdList);

                var otmp = new
                {
                    CPE_RESPUESTA_BE = cPE_RESPUESTA_COMPRA,
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











        [HttpGet]
        public ActionResult Index()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BusquedaDocumentosCompra;

            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                if (!usuario.creaDocumentosCompra && !usuario.visualizaDocumentosCompra)
                {
                    return RedirectToAction("Login", "Account");
                }
            }




            if (this.documentoCompraSession == null)
            {
                instanciarDocumentoCompraBusqueda();
            }

            
           
             int existeCliente = 0;
            //  if (cotizacion.cliente.idCliente != Guid.Empty || cotizacion.grupo.idGrupo != Guid.Empty)
            if (this.documentoCompraSession.proveedor.idProveedor != Guid.Empty)
            {
                existeCliente = 1;
            }

            GuiaRemision guiaRemision = new GuiaRemision();
            guiaRemision.motivoTraslado = GuiaRemision.motivosTraslado.Venta;

            ViewBag.DocumentoCompra = this.documentoCompraSession;
            ViewBag.fechaEmisionDesde = this.documentoCompraSession.fechaEmisionDesde.ToString(Constantes.formatoFecha);
            ViewBag.fechaEmisionHasta = this.documentoCompraSession.fechaEmisionHasta.ToString(Constantes.formatoFecha);
            ViewBag.DocumentoCompraList = this.Session[Constantes.VAR_SESSION_DOCUMENTO_COMPRA_LISTA];
            ViewBag.existeCliente = existeCliente;
            ViewBag.pagina = (int)Constantes.paginas.BusquedaDocumentosCompra;

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
            instanciarDocumentoCompraBusqueda();
        }



    /*    [HttpGet]
        public ActionResult ExportLastSearchExcel()
        {
            List<DocumentoCompra> list = (List<DocumentoCompra>)this.Session[Constantes.VAR_SESSION_FACTURA_LISTA];

            Facturasearch excel = new Facturasearch();
            return excel.generateExcel(list);
        }*/
        /*
       */


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
         //   DocumentoCompra = DocumentoCompraBL.descargarArchivoDocumentoCompra(DocumentoCompra);

            try {
                DocumentoCompra.cpeFile = Encoding.GetEncoding(28591).GetBytes(DocumentoCompra.rPTA_DOC_TRIB_COMPRA.DOC_TRIB_XML);
                DocumentoCompra.cdrFile = Encoding.GetEncoding(28591).GetBytes(DocumentoCompra.rPTA_DOC_TRIB_COMPRA.DOC_TRIB_RPTA);
            }
            catch (Exception e)
            {
                DocumentoCompra.cpeFile = null;
                DocumentoCompra.cdrFile = null;
            }

            var documentos = new
            {
                pdf = DocumentoCompra.rPTA_DOC_TRIB_COMPRA.DOC_TRIB_PDF,
                cpe = DocumentoCompra.cpeFile,
                cdr = DocumentoCompra.cdrFile,
                nombreArchivo = 
                        DocumentoCompra.cPE_CABECERA_COMPRA.NRO_DOC_EMI+"-"+ 
                        DocumentoCompra.cPE_CABECERA_COMPRA.TIP_CPE+"-"+
                        DocumentoCompra.cPE_CABECERA_COMPRA.SERIE + "-" +
                        DocumentoCompra.cPE_CABECERA_COMPRA.CORRELATIVO
            };

            return JsonConvert.SerializeObject(documentos);

        }


        private void instanciarDocumentoCompraBusqueda()
        {
            DocumentoCompra documentoCompra = new DocumentoCompra();
            DateTime fechaDesde = DateTime.Now.AddDays(-Constantes.DIAS_DESDE_BUSQUEDA);
            DateTime fechaHasta = DateTime.Now.AddDays(1);
            documentoCompra.fechaEmisionDesde = new DateTime(fechaDesde.Year, fechaDesde.Month, fechaDesde.Day, 0, 0, 0);
            documentoCompra.fechaEmisionHasta = new DateTime(fechaHasta.Year, fechaHasta.Month, fechaHasta.Day, 23, 59, 59);
            documentoCompra.buscarSedesGrupoCliente = false;

            documentoCompra.tipoNotaCredito = DocumentoCompra.TiposNotaCredito.AnulacionOperacion;

            documentoCompra.estadoDocumentoSunatBusqueda = DocumentoCompra.EstadosDocumentoSunatBusqueda.TodosAceptados;

            documentoCompra.serie = "0";
            documentoCompra.numero = "0";
            documentoCompra.pedido = new Pedido();
            documentoCompra.guiaRemision = new GuiaRemision();
            documentoCompra.solicitadoAnulacion = false;

            documentoCompra.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            documentoCompra.proveedor = new Proveedor();
            documentoCompra.proveedor.idProveedor = Guid.Empty;

            if (documentoCompra.usuario.sedesMPDocumentosVenta.Count == 1)
            {
                documentoCompra.ciudad = documentoCompra.usuario.sedesMPDocumentosVenta[0];
                //DocumentoCompra.ciudad.idCiudad = Guid.Empty;
            }
            else
            {
                documentoCompra.ciudad = new Ciudad();
                documentoCompra.ciudad.idCiudad = Guid.Empty;
            }


            

            




            //pedidoTmp.usuarioBusqueda = pedidoTmp.usuario;
            this.Session[Constantes.VAR_SESSION_DOCUMENTO_COMPRA_BUSQUEDA] = documentoCompra;
            this.Session[Constantes.VAR_SESSION_DOCUMENTO_COMPRA_LISTA] = new List<DocumentoCompra>();

            /*
            DocumentoCompra.tipoPago = DocumentoCompra.TipoPago.Contado;
            DocumentoCompra.formaPago = DocumentoCompra.FormaPago.NoAsignado;
            DocumentoCompra.fechaEmision = DateTime.Now;
            */


        }

       
        /*
        public String GetCliente()
        {
            DocumentoCompra factura = this.documentoCompraSession;
            Guid idCliente = Guid.Parse(Request["idCliente"].ToString());
            ClienteBL clienteBl = new ClienteBL();
            factura.proveedor = clienteBl.getCliente(idCliente);
            String resultado = JsonConvert.SerializeObject(factura.proveedor);
            this.documentoCompraSession = factura;
            return resultado;
        }*/

        public String GetGrupoCliente()
        {
            DocumentoCompra factura = this.documentoCompraSession;
            int idGrupoCliente = int.Parse(Request["idGrupoCliente"].ToString());

            factura.idGrupoCliente = idGrupoCliente;

            this.documentoCompraSession = factura;
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
            this.documentoCompraSession.ciudad = ciudad;
            return JsonConvert.SerializeObject(ciudad);
        }

        public void ChangeFechaEmisionDesde()
        {
            String[] desde = this.Request.Params["fechaEmisionDesde"].Split('/');
            this.documentoCompraSession.fechaEmisionDesde = new DateTime(Int32.Parse(desde[2]), Int32.Parse(desde[1]), Int32.Parse(desde[0]));
        }

        public void ChangeFechaEmisionHasta()
        {
            String[] desde = this.Request.Params["fechaEmisionHasta"].Split('/');
            this.documentoCompraSession.fechaEmisionHasta = new DateTime(Int32.Parse(desde[2]), Int32.Parse(desde[1]), Int32.Parse(desde[0]));
        }

        public void ChangeSolicitadoAnulacion()
        {
            this.documentoCompraSession.solicitadoAnulacion = Int32.Parse(this.Request.Params["solicitadoAnulacion"]) == 1;
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
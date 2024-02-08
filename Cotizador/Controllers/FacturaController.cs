using BusinessLayer;
using Cotizador.ExcelExport;
using Model;
using Model.NextSoft;
using Model.ServiceReferencePSE;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public class FacturaController : Controller
    {
        // GET: Factura
        private DocumentoVenta FacturaSession
        {
            get
            {
                DocumentoVenta factura = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaFacturas: factura = (DocumentoVenta)this.Session[Constantes.VAR_SESSION_FACTURA_BUSQUEDA]; break;
                    case Constantes.paginas.MantenimientoFactura: factura = (DocumentoVenta)this.Session[Constantes.VAR_SESSION_FACTURA]; break;
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
            //try
            //{
            Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_VENTA_VER];



            DocumentoVenta documentoVenta = new DocumentoVenta();

            String[] fecha = this.Request.Params["fechaEmision"].Split('/');
            String[] hora = this.Request.Params["horaEmision"].Split(':');
            documentoVenta.serie = this.Request.Params["serie"];
            documentoVenta.cliente = venta.pedido.cliente;

            bool cambioCliente = false;
            if (this.Session["s_cambioclientefactura_cambio"] != null)
            {
                cambioCliente = (bool)this.Session["s_cambioclientefactura_cambio"];
            }


            

            documentoVenta.venta = venta;


            documentoVenta.fechaEmision = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0]), Int32.Parse(hora[0]), Int32.Parse(hora[1]), 0);

            fecha = this.Request.Params["fechaVencimiento"].Split('/');
            documentoVenta.fechaVencimiento = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0]));

            documentoVenta.tipoPago = (DocumentoVenta.TipoPago)Int32.Parse(this.Request.Params["tipoPago"]);
            documentoVenta.formaPago = (DocumentoVenta.FormaPago)Int32.Parse(this.Request.Params["formaPago"]);
            documentoVenta.usuario = usuario;




            //documentoVenta.MovimentoALmacen = new 
            //  documentoVenta.venta = new Venta();
            documentoVenta.venta.guiaRemision = new GuiaRemision();
            documentoVenta.venta.guiaRemision.idMovimientoAlmacen = Guid.Parse(this.Request.Params["idMovimientoAlmacen"]);

            //     documentoVenta.venta.pedido = pedido;

            documentoVenta.venta.pedido.numeroReferenciaCliente = this.Request.Params["numeroReferenciaCliente"];
            documentoVenta.observaciones = this.Request.Params["observaciones"];

            if (usuario.codigoEmpresa.Equals("DP"))
            {
                ParametroBL parametroBL = new ParametroBL();
                string observacionDP = parametroBL.getParametro("DISTRIPLUS_OBSERVACION_FACTURA");
                documentoVenta.observaciones = 
                    documentoVenta.observaciones == null || documentoVenta.observaciones.Trim().Equals("") ? 
                            observacionDP : documentoVenta.observaciones + ". " + observacionDP;
            }

            DocumentoVentaBL documentoVentaBL = new DocumentoVentaBL();

            if (documentoVenta.venta.pedido.cliente.tipoDocumento == Constantes.TIPO_DOCUMENTO_CLIENTE_RUC)
                documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.Factura;
            else if (documentoVenta.venta.pedido.cliente.tipoDocumento == Constantes.TIPO_DOCUMENTO_CLIENTE_DNI ||
                documentoVenta.venta.pedido.cliente.tipoDocumento == Constantes.TIPO_DOCUMENTO_CLIENTE_CARNET_EXTRANJERIA)
                documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.BoletaVenta;
            else
                throw new Exception("No se ha identificado el clasePedido de documento electrónico a crear.");


            documentoVenta = documentoVentaBL.InsertarDocumentoVenta(documentoVenta);

            if (cambioCliente && usuario.cambiaClienteFactura)
            {
                Cliente clienteNuevo = (Cliente)this.Session["s_cambioclientefactura_cliente"];
                String domicilioLegalNuevo = this.Session["s_cambioclientefactura_domicilioLegal"].ToString();
                String emailNuevo = this.Session["s_cambioclientefactura_correoEnvio"].ToString();
                String sustento = this.Session["s_cambioclientefactura_sustento"].ToString();
                documentoVentaBL.CambiarClienteFactura(documentoVenta,clienteNuevo, domicilioLegalNuevo, emailNuevo, sustento);

                documentoVenta.cPE_CABECERA_BE.NOM_RCT = clienteNuevo.razonSocialSunat;
                documentoVenta.cPE_CABECERA_BE.NRO_DOC_RCT = clienteNuevo.ruc;
                documentoVenta.cPE_CABECERA_BE.DIR_DES_RCT = domicilioLegalNuevo;
                documentoVenta.cPE_CABECERA_BE.CORREO_ENVIO = emailNuevo;
            }


            return JsonConvert.SerializeObject(documentoVenta);
            //}
            //catch (Exception ex)
            //{
            //    Log log = new Log(ex.ToString(), TipoLog.Error, usuario);
            //    LogBL logBL = new LogBL();
            //    logBL.insertLog(log);
            //    return ex.ToString();
            //}
        }

        public String Show()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            try
            {
                DocumentoVenta documentoVenta = new DocumentoVenta();
                List<DocumentoVenta> documentoVentaList = (List<DocumentoVenta>)this.Session[Constantes.VAR_SESSION_FACTURA_LISTA];
                Guid idDocumentoVenta = Guid.Parse(this.Request.Params["idDocumentoVenta"]);
                documentoVenta = documentoVentaList.Where(d => d.idDocumentoVenta == idDocumentoVenta).First();
                DocumentoVentaBL documentoVentaBL = new DocumentoVentaBL();


                documentoVenta = documentoVentaBL.GetDocumentoVenta(documentoVenta);

                if (documentoVenta.tipoDocumento == DocumentoVenta.TipoDocumento.NotaCrédito)
                {
                    documentoVenta.tipoNotaCredito = (DocumentoVenta.TiposNotaCredito)Int32.Parse(documentoVenta.cPE_CABECERA_BE.COD_TIP_NC);
                }
                else if (documentoVenta.tipoDocumento == DocumentoVenta.TipoDocumento.NotaDébito)
                {
                    documentoVenta.tipoNotaDebito = (DocumentoVenta.TiposNotaDebito)Int32.Parse(documentoVenta.cPE_CABECERA_BE.COD_TIP_ND);
                }


                this.Session[Constantes.VAR_SESSION_FACTURA_VER] = documentoVenta;
                return JsonConvert.SerializeObject(documentoVenta);
            }
            catch (Exception ex)
            {
                Log log = new Log(ex.ToString(), TipoLog.Error, usuario);
                LogBL logBL = new LogBL();
                logBL.insertLog(log);
                return ex.ToString();
            }

        }

        public async System.Threading.Tasks.Task<string> EnviarFacturaANextSoft()
        {
            DocumentoVenta dv = (DocumentoVenta)this.Session[Constantes.VAR_SESSION_FACTURA_VER];
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            dv.usuario = usuario;

            int success = 0;

            ComprobanteVentaWS ws = new ComprobanteVentaWS();
            ws.urlApi = Constantes.NEXTSOFT_API_URL;
            ws.apiToken = Constantes.NEXTSOFT_API_TOKEN;

            object dataSend = ConverterMPToNextSoft.toCpe(dv);
            object result = await ws.crearComprobanteVenta(dataSend);

            JObject dataResult = (JObject)result;
            int codigo = dataResult["crearcomprobanteResult"]["codigo"].Value<int>();
            //int codigo = 0;

            string resultText = JsonConvert.SerializeObject(result);

            DocumentoVentaBL bl = new DocumentoVentaBL();
            if (codigo == 0)
            {
                success = 1;
            }

            bl.GuardarRespuestaNextSys(dv.idCpe, success, resultText);

            return JsonConvert.SerializeObject(new { success = success, dataSend = dataSend, result = result });
        }

        public String iniciarRefacturacion()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            VentaBL ventaBL = new VentaBL();
            Transaccion transaccion = new Transaccion();

            transaccion.documentoVenta = (DocumentoVenta)this.Session[Constantes.VAR_SESSION_FACTURA_VER];
            transaccion.documentoVenta.venta = null;
            transaccion.documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.Factura;
            transaccion.documentoVenta.idDocumentoVenta = Guid.Parse(Request["idDocumentoVenta"].ToString());

            transaccion.documentoReferencia = new DocumentoReferencia();
            transaccion.documentoReferencia.tipoDocumento = (DocumentoVenta.TipoDocumento)Int32.Parse(transaccion.documentoVenta.cPE_CABECERA_BE.TIP_CPE);
            String[] fechaEmisionArray = transaccion.documentoVenta.cPE_CABECERA_BE.FEC_EMI.Split('-');
            transaccion.documentoReferencia.fechaEmision = new DateTime(Int32.Parse(fechaEmisionArray[0]), Int32.Parse(fechaEmisionArray[1]), Int32.Parse(fechaEmisionArray[2]));

            transaccion.documentoReferencia.serie = transaccion.documentoVenta.cPE_CABECERA_BE.SERIE;
            transaccion.documentoReferencia.numero = transaccion.documentoVenta.cPE_CABECERA_BE.CORRELATIVO;

            transaccion = ventaBL.GetPlantillaVenta(transaccion, usuario);
            if (transaccion.tipoErrorCrearTransaccion == Venta.TiposErrorCrearTransaccion.NoExisteError)
            {
                // venta.tipoNotaCredito = (DocumentoVenta.TiposNotaCredito)Int32.Parse(Request["tipoNotaCredito"].ToString());
                transaccion.documentoVenta.fechaEmision = DateTime.Now;
                transaccion.documentoVenta.fechaVencimiento = transaccion.documentoVenta.fechaEmision.Value;
                //venta.documentoVenta.formaPago
                //Temporal
                Pedido pedido = transaccion.pedido;
                pedido.ciudadASolicitar = new Ciudad();

                PedidoBL pedidoBL = new PedidoBL();
                pedidoBL.calcularMontosTotales(pedido);
                this.Session[Constantes.VAR_SESSION_FACTURA] = transaccion;
            }
            return JsonConvert.SerializeObject(transaccion);

        }

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
            DocumentoVenta factura = this.FacturaSession;
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
            Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_NOTA_CREDITO];
            String[] fechaEmision = this.Request.Params["fechaEmision"].Split('/');
            String[] horaEmision = this.Request.Params["horaEmision"].Split(':');
            venta.documentoVenta.fechaEmision = new DateTime(Int32.Parse(fechaEmision[2]), Int32.Parse(fechaEmision[1]), Int32.Parse(fechaEmision[0]), Int32.Parse(horaEmision[0]), Int32.Parse(horaEmision[1]), 0);
            this.Session[Constantes.VAR_SESSION_NOTA_CREDITO] = venta;
        }

        public void ChangeResponsableComercialFacturaBusqueda()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            DocumentoVenta factura = this.FacturaSession;

            try
            {
                string idVendedor = this.Request.Params["idVendedor"].ToString();
                if (idVendedor.Equals("")) idVendedor = "0";

                if (usuario.modificaFiltroVendedor)
                {
                    factura.responsableComercial.idVendedor = int.Parse(idVendedor);
                }
                else
                {
                    if (usuario.esResponsableComercial)
                    {
                        factura.responsableComercial = usuario.vendedor;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            this.FacturaSession = factura;
        }


        public ActionResult Crear()
        {
            Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_FACTURA];
            //   venta.tipoNotaCredito = venta.documentoVenta.tipoNotaCredito;
            ViewBag.venta = venta;
            //ViewBag.tipoNotaCredito = (int)venta.tipoNotaCredito;

            ViewBag.fechaEmision = venta.documentoVenta.fechaEmision.Value.ToString(Constantes.formatoFecha);
            ViewBag.horaEmision = venta.documentoVenta.fechaEmision.Value.ToString(Constantes.formatoHora);

            return View();
        }




        public String ConfirmarCreacion()
        {

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            try
            {
                Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_VENTA_VER];
                DocumentoVenta documentoVenta = new DocumentoVenta();
                documentoVenta.venta = venta;
                documentoVenta.idDocumentoVenta = Guid.Parse(this.Request.Params["idDocumentoVenta"]);
                documentoVenta.cliente = venta.pedido.cliente;
                documentoVenta.usuario = usuario;

                if (documentoVenta.venta.pedido.cliente.tipoDocumento == Constantes.TIPO_DOCUMENTO_CLIENTE_RUC)
                    documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.Factura;
                else if (documentoVenta.venta.pedido.cliente.tipoDocumento == Constantes.TIPO_DOCUMENTO_CLIENTE_DNI ||
                     documentoVenta.venta.pedido.cliente.tipoDocumento == Constantes.TIPO_DOCUMENTO_CLIENTE_CARNET_EXTRANJERIA)
                    documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.BoletaVenta;
                else
                    throw new Exception("No se ha identificado el clasePedido de documento electrónico a crear.");

                DocumentoVentaBL documentoVentaBL = new DocumentoVentaBL();


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


        public String ConfirmarCreacionFacturaConsolidada()
        {

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            try
            {
                List<Guid> movimientoAlmacenIdList = (List<Guid>)this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA_LISTA_IDS];

                Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_VENTA_VER];
                DocumentoVenta documentoVenta = new DocumentoVenta();
                documentoVenta.venta = venta;
                documentoVenta.idDocumentoVenta = Guid.Parse(this.Request.Params["idDocumentoVenta"]);
                documentoVenta.cliente = venta.pedido.cliente;
                documentoVenta.usuario = usuario;

                if (documentoVenta.venta.pedido.cliente.tipoDocumento == Constantes.TIPO_DOCUMENTO_CLIENTE_RUC)
                    documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.Factura;
                else if (documentoVenta.venta.pedido.cliente.tipoDocumento == Constantes.TIPO_DOCUMENTO_CLIENTE_DNI ||
                     documentoVenta.venta.pedido.cliente.tipoDocumento == Constantes.TIPO_DOCUMENTO_CLIENTE_CARNET_EXTRANJERIA)
                    documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.BoletaVenta;
                else
                    throw new Exception("No se ha identificado el clasePedido de documento electrónico a crear.");

                DocumentoVentaBL documentoVentaBL = new DocumentoVentaBL();


                CPE_RESPUESTA_BE cPE_RESPUESTA_BE = documentoVentaBL.procesarCPE(documentoVenta, movimientoAlmacenIdList);

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

            ViewBag.documentoVenta = this.FacturaSession;
            ViewBag.fechaEmisionDesde = this.FacturaSession.fechaEmisionDesde.ToString(Constantes.formatoFecha);
            ViewBag.fechaEmisionHasta = this.FacturaSession.fechaEmisionHasta.ToString(Constantes.formatoFecha);
            ViewBag.documentoVentaList = this.Session[Constantes.VAR_SESSION_FACTURA_LISTA];
            ViewBag.existeCliente = existeCliente;
            ViewBag.pagina = (int)Constantes.paginas.BusquedaFacturas;

            ViewBag.Si = Constantes.MENSAJE_SI;
            ViewBag.No = Constantes.MENSAJE_NO;
            ViewBag.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            //ViewBag.descuentoList = Constantes.DESCUENTOS_LIST;
            return View();

        }


        public String SearchClientes()
        {
            String data = this.Request.Params["data[q]"];
            ClienteBL clienteBL = new ClienteBL();
            DocumentoVenta factura = (DocumentoVenta)this.Session[Constantes.VAR_SESSION_FACTURA_BUSQUEDA];
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            return clienteBL.getCLientesBusqueda(data, factura.ciudad.idCiudad, usuario.idUsuario);
        }

        public void CleanBusqueda()
        {
            instanciarfacturaBusqueda();
        }



        [HttpGet]
        public ActionResult ExportLastSearchExcel()
        {
            List<DocumentoVenta> list = (List<DocumentoVenta>)this.Session[Constantes.VAR_SESSION_FACTURA_LISTA];

            Facturasearch excel = new Facturasearch();
            return excel.generateExcel(list);
        }

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




            this.FacturaSession.estadoDocumentoSunatBusqueda = (DocumentoVenta.EstadosDocumentoSunatBusqueda)Int32.Parse(this.Request.Params["estadoDocumentoSunatBusqueda"]);
            this.FacturaSession.tipoDocumento = (DocumentoVenta.TipoDocumento)Int32.Parse(this.Request.Params["tipoDocumento"]);

            DocumentoVentaBL documentoVentaBL = new DocumentoVentaBL();

            List<DocumentoVenta> documentoVentaList = documentoVentaBL.GetFacturas(this.FacturaSession);

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            /*        foreach (DocumentoVenta documentoVenta in documentoVentaList)
                    {
                        documentoVenta.usuario.apruebaAnulaciones = usuario.apruebaAnulaciones;
                        documentoVenta.usuario.creaNotasCredito = usuario.creaNotasCredito;
                    }*/


            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_FACTURA_LISTA] = documentoVentaList;
            //Se retorna la cantidad de elementos encontrados

            return JsonConvert.SerializeObject(documentoVentaList);
            //return pedidoList.Count();
        }


        public void consultarEstadoDocumentoVenta()
        {
            DocumentoVentaBL documentoVentaBL = new DocumentoVentaBL();
            List<DocumentoVenta> documentoVentaList = (List<DocumentoVenta>)this.Session[Constantes.VAR_SESSION_FACTURA_LISTA];
            Guid idDocumentoVenta = Guid.Parse(this.Request.Params["idDocumentoVenta"]);

            DocumentoVenta documentoVenta = documentoVentaList.Where(d => d.idDocumentoVenta == idDocumentoVenta).First();

            // documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.Factura;

            ParametroBL parametroBL = new ParametroBL();
            int idEmpresa = parametroBL.GetDataFacturacionEmpresaEOL(documentoVenta.cPE_CABECERA_BE.ID);

            documentoVentaBL.consultarEstadoDocumentoVenta(documentoVenta);

        }


        public String SolicitarAnulacion()
        {
            DocumentoVentaBL documentoVentaBL = new DocumentoVentaBL();
            List<DocumentoVenta> documentoVentaList = (List<DocumentoVenta>)this.Session[Constantes.VAR_SESSION_FACTURA_LISTA];
            Guid idDocumentoVenta = Guid.Parse(this.Request.Params["idDocumentoVenta"]);
            //  Guid idDocumentoVenta = this.Request.Params["idDocumentoVenta"]);

            DocumentoVenta documentoVenta = documentoVentaList.Where(d => d.idDocumentoVenta == idDocumentoVenta).FirstOrDefault();
            documentoVenta.comentarioAnulado = this.Request.Params["comentarioAnulado"];
            documentoVenta.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            //documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.Factura;
            documentoVentaBL.anularDocumentoVenta(documentoVenta);
            return JsonConvert.SerializeObject(documentoVenta);

        }

        public String AprobarAnulacion()
        {
            DocumentoVentaBL documentoVentaBL = new DocumentoVentaBL();
            /*  List<DocumentoVenta> documentoVentaList = (List<DocumentoVenta>)this.Session[Constantes.VAR_SESSION_FACTURA_LISTA];
              Guid idDocumentoVenta = Guid.Parse(this.Request.Params["idDocumentoVenta"]);
              DocumentoVenta documentoVenta = documentoVentaList.Where(d => d.idDocumentoVenta == idDocumentoVenta).FirstOrDefault();
  */
            DocumentoVenta documentoVenta = new DocumentoVenta();
            documentoVenta.idDocumentoVenta = Guid.Parse(this.Request.Params["idDocumentoVenta"]);
            documentoVenta = documentoVentaBL.GetDocumentoVenta(documentoVenta);
            documentoVenta.comentarioAprobacionAnulacion = this.Request.Params["comentarioAprobacionAnulacion"];
            documentoVenta.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];


            documentoVentaBL.aprobarAnulacionDocumentoVenta(documentoVenta, documentoVenta.usuario);
            return JsonConvert.SerializeObject(documentoVenta);

        }

        public String RechazarAnulacion()
        {
            DocumentoVentaBL documentoVentaBL = new DocumentoVentaBL();
            /*  List<DocumentoVenta> documentoVentaList = (List<DocumentoVenta>)this.Session[Constantes.VAR_SESSION_FACTURA_LISTA];
              Guid idDocumentoVenta = Guid.Parse(this.Request.Params["idDocumentoVenta"]);
              DocumentoVenta documentoVenta = documentoVentaList.Where(d => d.idDocumentoVenta == idDocumentoVenta).FirstOrDefault();
  */
            DocumentoVenta documentoVenta = new DocumentoVenta();
            documentoVenta.idDocumentoVenta = Guid.Parse(this.Request.Params["idDocumentoVenta"]);
            documentoVenta = documentoVentaBL.GetDocumentoVenta(documentoVenta);
            documentoVenta.comentarioAprobacionAnulacion = this.Request.Params["comentarioRechazoAnulacion"];
            documentoVenta.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];


            documentoVentaBL.rechazarAnulacionDocumentoVenta(documentoVenta, documentoVenta.usuario);
            return JsonConvert.SerializeObject(documentoVenta);

        }




        public String descargarArchivoDocumentoVenta()
        {
            DocumentoVentaBL documentoVentaBL = new DocumentoVentaBL();
            List<DocumentoVenta> documentoVentaList = (List<DocumentoVenta>)this.Session[Constantes.VAR_SESSION_FACTURA_LISTA];
            Guid idDocumentoVenta = Guid.Parse(this.Request.Params["idDocumentoVenta"]);

            String ruta = String.Empty;


            DocumentoVenta documentoVenta = documentoVentaList.Where(d => d.idDocumentoVenta == idDocumentoVenta).FirstOrDefault();
            //documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.Factura;
            documentoVenta = documentoVentaBL.GetDocumentoVenta(documentoVenta);
            documentoVenta = documentoVentaBL.descargarArchivoDocumentoVenta(documentoVenta);

            try
            {
                documentoVenta.cpeFile = Encoding.GetEncoding(28591).GetBytes(documentoVenta.rPTA_DOC_TRIB_BE.DOC_TRIB_XML);
                documentoVenta.cdrFile = Encoding.GetEncoding(28591).GetBytes(documentoVenta.rPTA_DOC_TRIB_BE.DOC_TRIB_RPTA);
            }
            catch (Exception e)
            {
                documentoVenta.cpeFile = null;
                documentoVenta.cdrFile = null;
            }

            var documentos = new
            {
                pdf = documentoVenta.rPTA_DOC_TRIB_BE.DOC_TRIB_PDF,
                cpe = documentoVenta.cpeFile,
                cdr = documentoVenta.cdrFile,
                nombreArchivo =
                        documentoVenta.cPE_CABECERA_BE.NRO_DOC_EMI + "-" +
                        documentoVenta.cPE_CABECERA_BE.TIP_CPE + "-" +
                        documentoVenta.cPE_CABECERA_BE.SERIE + "-" +
                        documentoVenta.cPE_CABECERA_BE.CORRELATIVO
            };

            return JsonConvert.SerializeObject(documentos);

        }


        private void instanciarfacturaBusqueda()
        {

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            DocumentoVenta documentoVenta = new DocumentoVenta();
            DateTime fechaDesde = DateTime.Now.AddDays(-Constantes.DIAS_DESDE_BUSQUEDA);
            DateTime fechaHasta = DateTime.Now.AddDays(1);
            documentoVenta.fechaEmisionDesde = new DateTime(fechaDesde.Year, fechaDesde.Month, fechaDesde.Day, 0, 0, 0);
            documentoVenta.fechaEmisionHasta = new DateTime(fechaHasta.Year, fechaHasta.Month, fechaHasta.Day, 23, 59, 59);
            documentoVenta.buscarSedesGrupoCliente = false;

            documentoVenta.tipoNotaCredito = DocumentoVenta.TiposNotaCredito.AnulacionOperacion;

            documentoVenta.estadoDocumentoSunatBusqueda = DocumentoVenta.EstadosDocumentoSunatBusqueda.TodosAceptados;

            documentoVenta.serie = "0";
            documentoVenta.numero = "0";
            documentoVenta.pedido = new Pedido();
            documentoVenta.guiaRemision = new GuiaRemision();
            documentoVenta.solicitadoAnulacion = false;

            documentoVenta.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            documentoVenta.responsableComercial = new Vendedor();

            if (usuario.esResponsableComercial && !usuario.modificaFiltroVendedor)
            {
                documentoVenta.responsableComercial = usuario.vendedor;
            }


            documentoVenta.cliente = new Cliente();
            documentoVenta.cliente.idCliente = Guid.Empty;

            if (documentoVenta.usuario.sedesMPDocumentosVenta.Count == 1)
            {
                documentoVenta.ciudad = documentoVenta.usuario.sedesMPDocumentosVenta[0];
                //documentoVenta.ciudad.idCiudad = Guid.Empty;
            }
            else
            {
                documentoVenta.ciudad = new Ciudad();
                documentoVenta.ciudad.idCiudad = Guid.Empty;
            }



            //pedidoTmp.usuarioBusqueda = pedidoTmp.usuario;
            this.Session[Constantes.VAR_SESSION_FACTURA_BUSQUEDA] = documentoVenta;
            this.Session[Constantes.VAR_SESSION_FACTURA_LISTA] = new List<DocumentoVenta>();

            /*
            documentoVenta.tipoPago = DocumentoVenta.TipoPago.Contado;
            documentoVenta.formaPago = DocumentoVenta.FormaPago.NoAsignado;
            documentoVenta.fechaEmision = DateTime.Now;
            */


        }



        public String GetCliente()
        {
            DocumentoVenta factura = this.FacturaSession;
            Guid idCliente = Guid.Parse(Request["idCliente"].ToString());
            ClienteBL clienteBl = new ClienteBL();
            factura.cliente = clienteBl.getCliente(idCliente);
            String resultado = JsonConvert.SerializeObject(factura.cliente);
            this.FacturaSession = factura;
            return resultado;
        }

        public String GetGrupoCliente()
        {
            DocumentoVenta factura = this.FacturaSession;
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
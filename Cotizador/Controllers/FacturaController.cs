using BusinessLayer;
using Model;
using Model.ServiceReferencePSE;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            try
            {
                Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_VENTA_VER];



                DocumentoVenta documentoVenta = new DocumentoVenta();

                String[] fecha = this.Request.Params["fechaEmision"].Split('/');
                String[] hora = this.Request.Params["horaEmision"].Split(':');
                documentoVenta.serie = this.Request.Params["serie"];
                documentoVenta.cliente = venta.pedido.cliente;
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

                DocumentoVentaBL documentoVentaBL = new DocumentoVentaBL();
                documentoVenta = documentoVentaBL.InsertarFactura(documentoVenta);

                //Se retorna el codigo del documento de venta para poder realizar la confirmación
                //        CPE_RESPUESTA_BE = cPE_RESPUESTA_BE,
       /*         var otmp = new
                {
                    documentoVenta = documentoVenta
                };

                */
              

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




        





        public String ConfirmarCreacion()
        {

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            try
            {
                Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_VENTA_VER];
                DocumentoVenta documentoVenta = new DocumentoVenta();
                documentoVenta.venta = venta;
                documentoVenta.idDocumentoVenta  = Guid.Parse(this.Request.Params["idDocumentoVenta"]);
                documentoVenta.cliente = venta.pedido.cliente;
                documentoVenta.usuario = usuario;

                DocumentoVentaBL documentoVentaBL = new DocumentoVentaBL();
                CPE_RESPUESTA_BE cPE_RESPUESTA_BE = documentoVentaBL.procesarFactura(documentoVenta);

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
            return View();

        }


        public String SearchClientes()
        {
            String data = this.Request.Params["data[q]"];
            ClienteBL clienteBL = new ClienteBL();
            DocumentoVenta factura = (DocumentoVenta)this.Session[Constantes.VAR_SESSION_FACTURA_BUSQUEDA];
            return clienteBL.getCLientesBusqueda(data, factura.ciudad.idCiudad);
        }

        public void CleanBusqueda()
        {
            instanciarfacturaBusqueda();
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


            this.FacturaSession.estadoDocumentoSunatBusqueda = (DocumentoVenta.EstadosDocumentoSunatBusqueda)Int32.Parse(this.Request.Params["estadoDocumentoSunatBusqueda"]);

            DocumentoVentaBL documentoVentaBL = new DocumentoVentaBL();

            List<DocumentoVenta> documentoVentaList = documentoVentaBL.GetFacturas(this.FacturaSession);

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            foreach (DocumentoVenta documentoVenta in documentoVentaList)
            {
                documentoVenta.usuario.apruebaAnulaciones = usuario.apruebaAnulaciones;
            }


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
           

            foreach (DocumentoVenta documentoVenta in documentoVentaList)
            {
                if (documentoVenta.idDocumentoVenta == idDocumentoVenta)
                {
                    documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.Factura;
                    documentoVentaBL.consultarEstadoDocumentoVenta(documentoVenta);
                    break;
                }              
            }          
        }


        public String Anular()
        {
            DocumentoVentaBL documentoVentaBL = new DocumentoVentaBL();
            List<DocumentoVenta> documentoVentaList = (List<DocumentoVenta>)this.Session[Constantes.VAR_SESSION_FACTURA_LISTA];
            Guid idDocumentoVenta = Guid.Parse(this.Request.Params["idDocumentoVenta"]);
            //  Guid idDocumentoVenta = this.Request.Params["idDocumentoVenta"]);

            DocumentoVenta documentoVenta = documentoVentaList.Where(d => d.idDocumentoVenta == idDocumentoVenta).FirstOrDefault();
            documentoVenta.comentarioAnulado = this.Request.Params["comentarioAnulado"];
            documentoVenta.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.Factura;
            documentoVentaBL.anularDocumentoVenta(documentoVenta);
            return JsonConvert.SerializeObject(documentoVenta);

        }

        public String AprobarAnulacion()
        {
            DocumentoVentaBL documentoVentaBL = new DocumentoVentaBL();
            List<DocumentoVenta> documentoVentaList = (List<DocumentoVenta>)this.Session[Constantes.VAR_SESSION_FACTURA_LISTA];
            Guid idDocumentoVenta = Guid.Parse(this.Request.Params["idDocumentoVenta"]);
            //  Guid idDocumentoVenta = this.Request.Params["idDocumentoVenta"]);

            DocumentoVenta documentoVenta = documentoVentaList.Where(d => d.idDocumentoVenta == idDocumentoVenta).FirstOrDefault();
            documentoVenta.comentarioAprobacionAnulacion = this.Request.Params["comentarioAprobacionAnulacion"];
            documentoVenta.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.Factura;
            documentoVentaBL.aprobarAnulacionDocumentoVenta(documentoVenta);
            return JsonConvert.SerializeObject(documentoVenta);

        }

        



        public String descargarArchivoDocumentoVenta()
        {
            DocumentoVentaBL documentoVentaBL = new DocumentoVentaBL();
            List<DocumentoVenta> documentoVentaList = (List<DocumentoVenta>)this.Session[Constantes.VAR_SESSION_FACTURA_LISTA];
            Guid idDocumentoVenta = Guid.Parse(this.Request.Params["idDocumentoVenta"]);

            String ruta = String.Empty;


            DocumentoVenta documentoVenta = documentoVentaList.Where(d => d.idDocumentoVenta == idDocumentoVenta).FirstOrDefault();
            documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.Factura;
            documentoVenta = documentoVentaBL.descargarArchivoDocumentoVenta(documentoVenta);

            try {
                documentoVenta.cpeFile = Encoding.UTF8.GetBytes(documentoVenta.rPTA_DOC_TRIB_BE.DOC_TRIB_XML);
                documentoVenta.cdrFile = Encoding.UTF8.GetBytes(documentoVenta.rPTA_DOC_TRIB_BE.DOC_TRIB_RPTA);
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
                cdr = documentoVenta.cdrFile
            };

            return JsonConvert.SerializeObject(documentos);

        }


        private void instanciarfacturaBusqueda()
        {
            DocumentoVenta documentoVenta = new DocumentoVenta();
            DateTime fechaDesde = DateTime.Now.AddDays(-10);
            DateTime fechaHasta = DateTime.Now.AddDays(10);
            documentoVenta.fechaEmisionDesde = new DateTime(fechaDesde.Year, fechaDesde.Month, fechaDesde.Day, 0, 0, 0);
            documentoVenta.fechaEmisionHasta = new DateTime(fechaHasta.Year, fechaHasta.Month, fechaHasta.Day, 23, 59, 59);


            documentoVenta.estadoDocumentoSunatBusqueda = DocumentoVenta.EstadosDocumentoSunatBusqueda.TodosAceptados;

            documentoVenta.serie = "0";
            documentoVenta.numero = "0";
            documentoVenta.pedido = new Pedido();
            documentoVenta.guiaRemision = new GuiaRemision();
            documentoVenta.solicitadoAnulacion = false;

            documentoVenta.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

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
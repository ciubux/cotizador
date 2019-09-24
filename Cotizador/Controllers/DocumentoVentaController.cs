using BusinessLayer;
using Model;
using Model.ServiceReferencePSE;
using Newtonsoft.Json;
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

        public String GenerarFactura(Guid idDocumentoVenta)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            try
            {


                DocumentoVentaBL documentoVentaBL = new DocumentoVentaBL();
                DocumentoVenta documentoVenta = new DocumentoVenta();
                documentoVenta.idDocumentoVenta = idDocumentoVenta;
                documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.Factura;
                documentoVenta.usuario = usuario;
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


        public String GenerarNotaCredito(Guid idDocumentoVenta)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            try
            {
              

                DocumentoVentaBL documentoVentaBL = new DocumentoVentaBL();
                DocumentoVenta documentoVenta = new DocumentoVenta();
                documentoVenta.idDocumentoVenta = idDocumentoVenta;
                documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.NotaCrédito;
                documentoVenta.usuario = usuario;
                CPE_RESPUESTA_BE cPE_RESPUESTA_BE = documentoVentaBL.procesarNotaCredito(documentoVenta);

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


        public String GenerarNotaDebito(Guid idDocumentoVenta)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            try
            {


                DocumentoVentaBL documentoVentaBL = new DocumentoVentaBL();
                DocumentoVenta documentoVenta = new DocumentoVenta();
                documentoVenta.idDocumentoVenta = idDocumentoVenta;
                documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.NotaDébito;
                documentoVenta.usuario = usuario;
                CPE_RESPUESTA_BE cPE_RESPUESTA_BE = documentoVentaBL.procesarNotaCredito(documentoVenta);

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


        public String GenerarBoletaVenta(Guid idDocumentoVenta)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            try
            {


                DocumentoVentaBL documentoVentaBL = new DocumentoVentaBL();
                DocumentoVenta documentoVenta = new DocumentoVenta();
                documentoVenta.idDocumentoVenta = idDocumentoVenta;
                documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.BoletaVenta;
                documentoVenta.usuario = usuario;
                CPE_RESPUESTA_BE cPE_RESPUESTA_BE = documentoVentaBL.procesarBoletaVenta(documentoVenta);

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


        /*--------------------------------------------------------*/
        [HttpGet]
        public ActionResult ValidarNotificacionAnulacion()
        {

            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BusquedaDocumentoVentaNotificacion;


            DocumentoVenta objSearch = (DocumentoVenta)this.Session[Constantes.VAR_SESSION_NOTIFICACIONDOCUMENTOVENTA_BUSQUEDA];
            ViewBag.pagina = (int)Constantes.paginas.BusquedaDocumentoVentaNotificacion;
            ViewBag.documentoventa = objSearch;

            return View();
        }

        public String SearchList()
        {

            //Se indica la página con la que se va a trabajar
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaDocumentoVentaNotificacion;
            //Se recupera el objeto cliente que contiene los criterios de Búsqueda de la session

            DocumentoVentaBL bL = new DocumentoVentaBL();
            List<DocumentoVenta> list = bL.BusquedaValidacionAnulacion();
            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_NOTIFICACIONDOCUMENTOVENTA_LISTA] = list;
            //Se retorna la cantidad de elementos encontrados
            return JsonConvert.SerializeObject(list);
        }

        public String Update()
        {

            DocumentoVenta obj2 = new DocumentoVenta();
            obj2.idDocumentoVenta = Guid.Parse(Request["idDocumentoVenta"].ToString());
            obj2.estado_anulacion = Int32.Parse(Request["estado_anulacion"].ToString());

            DocumentoVentaBL bL = new DocumentoVentaBL();
            DocumentoVentaSession = obj2;
            obj2 = bL.updateNotificacionAnulacion(obj2);
            this.Session[Constantes.VAR_SESSION_NOTIFICACIONDOCUMENTOVENTA] = null;
            String resultado = JsonConvert.SerializeObject(obj2);

            return resultado;

        }


        private DocumentoVenta DocumentoVentaSession
        {
            get
            {
                DocumentoVenta obj = null;

                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {

                    case Constantes.paginas.BusquedaDocumentoVentaNotificacion: obj = (DocumentoVenta)this.Session[Constantes.VAR_SESSION_NOTIFICACIONDOCUMENTOVENTA_BUSQUEDA]; break;

                }
                return obj;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaDocumentoVentaNotificacion: this.Session[Constantes.VAR_SESSION_NOTIFICACIONDOCUMENTOVENTA_BUSQUEDA] = value; break;

                }
            }
        }





    }
}
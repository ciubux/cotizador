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





    }
}
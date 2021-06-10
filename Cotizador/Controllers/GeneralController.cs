using BusinessLayer;
using Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cotizador.Controllers
{
    public class GeneralController : Controller
    {
        // GET: General
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Exit()
        {  //Se eliminan todos los datos de Session
            Session.Contents.RemoveAll();
          /*
            this.Session["usuario"] = null;
            this.Session["cotizacion"] = null;
            this.Session["cotizacionBusqueda"] = null;
            this.Session["cotizacionList"] = null;
            this.Session["cotizacionBusqueda"] = null;
            this.Session["cotizacionList"] = null;
            this.Session["cotizacionBusqueda"] = null;
            this.Session["cotizacionList"] = null;
            this.Session["cotizacionBusqueda"] = null;
            this.Session["cotizacionList"] = null;*/
            return RedirectToAction("Login", "Account");
        }


        [HttpGet]
        public ActionResult DownLoadFile(String fileName)
        {
            String ruta = AppDomain.CurrentDomain.BaseDirectory + "\\pdf\\" + fileName;
            FileStream inStream = new FileStream(ruta, FileMode.Open);
            MemoryStream storeStream = new MemoryStream();

            storeStream.SetLength(inStream.Length);
            inStream.Read(storeStream.GetBuffer(), 0, (int)inStream.Length);

            storeStream.Flush();
            inStream.Close();
            System.IO.File.Delete(ruta);
            //System.IO.File.Delete(fileName);

            FileStreamResult result = new FileStreamResult(storeStream, "application/pdf");
            result.FileDownloadName = fileName;
            return result;
        }


        public String GetConstantes()
        {
            return "{ \"IGV\":\"" + Constantes.IGV + "\", \"SIMBOLO_SOL\":\"" + Constantes.SIMBOLO_SOL + "\", \"MILISEGUNDOS_AUTOGUARDADO\":\"" 
                   + Constantes.MILISEGUNDOS_AUTOGUARDADO + "\", \"VARIACION_PRECIO_ITEM_PEDIDO\":\"" + Constantes.VARIACION_PRECIO_ITEM_PEDIDO 
                   + "\", \"DESCARGAR_XML\":\"" + Constantes.DESCARGAR_XML + "\", \"ID_SEDE_TODOS\":\"" + Constantes.ID_SEDE_TODOS
                   + "\", \"DIAS_MAX_VALIDEZ_OFERTA_COTIZACION_PUNTUAL\":\"" + Constantes.DIAS_MAX_VALIDEZ_OFERTA_COTIZACION_PUNTUAL
                   + "\", \"DIAS_DEFECTO_VALIDEZ_OFERTA_COTIZACION_PUNTUAL\":\"" + Constantes.DIAS_DEFECTO_VALIDEZ_OFERTA_COTIZACION_PUNTUAL + "\" }";
        }

        public String ChosenDinamico()
        {
            String data = this.Request.Params["data[q]"];

            return "{\"q\":\"" + data + "\",\"results\":[{\"id\":\"" + data + "\",\"text\":\"" + data + "\"}]}";
        }

        
        public void ChangeField()
        {
            String VAR_SESION =  this.Request.Params["numeroReferenciaCliente"];

            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            pedido.numeroReferenciaCliente = this.Request.Params["numeroReferenciaCliente"];
            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
        }

        public void ChangeFieldDate()
        {

        }

        public String EjecutarJob()
        {
            CotizacionBL cotizacionBL = new CotizacionBL();
            cotizacionBL.RechazarCotizaciones();
            return "Job Ejecutado, time:" + DateTime.Now;
        }


        public String ActualizarEstadoDocumentosElectronicos()
        {
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = usuarioBL.getUsuarioLogin("automatico", "123",null);

            DocumentoVentaBL documentoVentaBL = new DocumentoVentaBL();
            documentoVentaBL.ActualizarEstadoDocumentosElectronicos(usuario);

            return "ActualizarEstadoDocumentosElectronicos Ejecutando, time:" + DateTime.Now;
        }

        public void actualizarParametroRUCCargaMasiva()
        {
            String codigo = "RUC_CARGA_MASIVA_PRODUCTO";
            String valor = this.Request.Params["ruc"];
            ParametroBL parametroBL = new ParametroBL();
            parametroBL.updateParametro(codigo, valor);
        }

    }
}
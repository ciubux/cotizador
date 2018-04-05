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
        {
            //Se eliminan todos los datos de Session
            this.Session["usuario"] = null;
            this.Session["cotizacion"] = null;
            this.Session["cotizacionBusqueda"] = null;
            this.Session["cotizacionList"] = null;
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
            return "{ \"IGV\":\"" + Constantes.IGV + "\", \"SIMBOLO_SOL\":\"" + Constantes.SIMBOLO_SOL + "\", \"MILISEGUNDOS_AUTOGUARDADO\":\"" + Constantes.MILISEGUNDOS_AUTOGUARDADO + "\" }";
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


    }
}
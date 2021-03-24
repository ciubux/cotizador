using Cotizador.Models;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLayer;
using Newtonsoft.Json;
using System.Reflection;

namespace Cotizador.Controllers
{
    public class LogCambioController : Controller
    {


        [HttpGet]
        public ActionResult Index()
        {

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            if (!usuario.modificaLogCampo)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }



        public String UpdateRegistro()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (usuario.modificaLogDatoHistorico)
            {
                Guid idCambio = Guid.Parse(this.Request.Params["idLog"]);
                String fechaVigencia = this.Request.Params["fechaVigencia"].ToString();
                String valor = this.Request.Params["valor"].ToString();

                String[] fiv = fechaVigencia.Split('/');

                LogCambioBL bL = new LogCambioBL();

                LogCambio log = new LogCambio();
                log.idCambio = idCambio;
                log.idUsuarioModificacion = usuario.idUsuario;
                log.valor = valor;
                log.fechaInicioVigencia =  new DateTime(Int32.Parse(fiv[2]), Int32.Parse(fiv[1]), Int32.Parse(fiv[0]));

                bL.UpdateLog(log);

                return "{\"success\": 1}";
            }

            return "{\"success\": 0}";
        }


        public String AnularRegistro()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (usuario.modificaLogDatoHistorico)
            {
                Guid idCambio = Guid.Parse(this.Request.Params["idLog"]);
                
                LogCambioBL bL = new LogCambioBL();

                LogCambio log = new LogCambio();
                log.idCambio = idCambio;
                log.idUsuarioModificacion = usuario.idUsuario;

                bL.AnularLog(log);

                return "{\"success\": 1}";
            }

            return "{\"success\": 0}";
        }
    }
}

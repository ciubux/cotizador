using BusinessLayer;
using Cotizador.Models;
using Cotizador.ExcelExport;
using Model;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Cotizador.Controllers
{
    public class ValidacionController : ParentController
    {

        [HttpGet]
        public ActionResult Pendientes()
        {

            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.AlertasValidacionPendientes;

            AlertaValidacionBL bl = new AlertaValidacionBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            List<AlertaValidacion> alertas = bl.getAlertasUsuario(usuario);
            usuario.alertasList = alertas;

            ViewBag.pagina = (int)Constantes.paginas.AlertasValidacionPendientes;
            ViewBag.alertas = alertas;

            return View();
        }
        
        // GET: Origen
        [HttpGet]
        public ActionResult Index()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            if (!usuario.modificaOrigen)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();

        }

        public String ValidarPendiente()
        {
            Guid idAlertaValidacion = Guid.Parse(Request["idAlertaValidacion"].ToString());
            AlertaValidacionBL bl = new AlertaValidacionBL();

            
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            bl.validarAlerta(idAlertaValidacion, usuario.idUsuario);

            List<AlertaValidacion> alertas = bl.getAlertasUsuario(usuario);
            usuario.alertasList = alertas;

            this.Session[Constantes.VAR_SESSION_USUARIO] = usuario;
            
            return "{\"success\": 1}";
        }
    }


}
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
    public class ConsolidadoAtencionController : ParentController
    {
        [HttpGet]
        public ActionResult List()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            if (!usuario.visualizaConsolidadoAtencion && !usuario.modificaMaestroConsolidadoAtencion) // Verifica tu constante de permiso
            {
                return RedirectToAction("Login", "Account");
            }

            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BusquedaConsolidadoAtencion;

            if (this.Session[Constantes.VAR_SESSION_CONSOLIDADOATENCION_BUSQUEDA] == null)
            {
                instanciarConsolidadoBusqueda();
            }

            ConsolidadoAtencion objSearch = (ConsolidadoAtencion)this.Session[Constantes.VAR_SESSION_CONSOLIDADOATENCION_BUSQUEDA];

            ViewBag.pagina = (int)Constantes.paginas.BusquedaConsolidadoAtencion;
            ViewBag.consolidado = objSearch;

            return View();
        }

        public String SearchList()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaConsolidadoAtencion;
            ConsolidadoAtencion obj = (ConsolidadoAtencion)this.Session[Constantes.VAR_SESSION_CONSOLIDADOATENCION_BUSQUEDA];

            ConsolidadoAtencionBL bL = new ConsolidadoAtencionBL();
            List<ConsolidadoAtencion> list = bL.getConsolidadosAtencion(obj);

            this.Session[Constantes.VAR_SESSION_CONSOLIDADOATENCION_LISTA] = list;
            return JsonConvert.SerializeObject(list);
        }

        private void instanciarConsolidadoBusqueda()
        {
            ConsolidadoAtencion obj = new ConsolidadoAtencion();
            obj.idConsolidadoAtencion = Guid.Empty;
            obj.fecha = DateTime.Now;
            obj.Estado = 1;
            this.Session[Constantes.VAR_SESSION_CONSOLIDADOATENCION_BUSQUEDA] = obj;
        }

        public String Create()
        {
            ConsolidadoAtencionBL bL = new ConsolidadoAtencionBL();
            ConsolidadoAtencion obj = new ConsolidadoAtencion();

            obj.fecha = DateTime.Parse(Request["fecha"]);
            obj.observaciones = Request["observaciones"] ?? "";
            obj.vehiculo = new Vehiculo { idVehiculo = int.Parse(Request["idVehiculo"]) };
            obj.chofer = new PersonalAlmacen { idPersonalAlmacen = int.Parse(Request["idChofer"]) };
            obj.asistente = new PersonalAlmacen { idPersonalAlmacen = int.Parse(Request["idAsistente"]) };
            obj.Estado = 1;
            obj.IdUsuarioRegistro = Logueado.idUsuario;

            obj = bL.insertConsolidadoAtencion(obj);
            return JsonConvert.SerializeObject(obj);
        }

        public String Update()
        {
            ConsolidadoAtencionBL bL = new ConsolidadoAtencionBL();
            ConsolidadoAtencion obj = new ConsolidadoAtencion();

            obj.idConsolidadoAtencion = Guid.Parse(Request["idConsolidadoAtencion"]);
            obj.fecha = DateTime.Parse(Request["fecha"]);
            obj.observaciones = Request["observaciones"] ?? "";
            obj.vehiculo = new Vehiculo { idVehiculo = int.Parse(Request["idVehiculo"]) };
            obj.chofer = new PersonalAlmacen { idPersonalAlmacen = int.Parse(Request["idChofer"]) };
            obj.asistente = new PersonalAlmacen { idPersonalAlmacen = int.Parse(Request["idAsistente"]) };

            obj.IdUsuarioRegistro = Logueado.idUsuario;

            obj = bL.updateConsolidadoAtencion(obj);
            return JsonConvert.SerializeObject(obj);
        }
    }
}
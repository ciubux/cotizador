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
            if (!usuario.modificaLogCambio)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }


        [HttpGet]
        public ActionResult Lista()
        {

            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BusquedaLogCambio;
            if (this.Session[Constantes.VAR_SESSION_LOGCAMBIO_BUSQUEDA] == null)
            {
                instanciarCatalogoBusqueda();
            }

            LogCambio objSearch = (LogCambio)this.Session[Constantes.VAR_SESSION_LOGCAMBIO_BUSQUEDA];
            ViewBag.pagina = (int)Constantes.paginas.BusquedaLogCambio;
            ViewBag.catalogo = objSearch;

            return View();
        }

        public String SearchList()
        {

            //Se indica la página con la que se va a trabajar
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaLogCambio;
            //Se recupera el objeto cliente que contiene los criterios de Búsqueda de la session
            LogCambio obj = (LogCambio)this.Session[Constantes.VAR_SESSION_LOGCAMBIO_BUSQUEDA];
            LogCambioBL bL = new LogCambioBL();
            List<LogCambio> list = bL.getCatalogoById(obj);
            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_LOGCAMBIO_LISTA] = list;
            //Se retorna la cantidad de elementos encontrados
            return JsonConvert.SerializeObject(list);
        }


        public ActionResult GetTablas(string tablaId, string selectedValue = null)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            LogCambioBL logcambioBL = new LogCambioBL();
            List<LogCambio> logcambio = logcambioBL.getTablasList();


            var model = new LogCambioViewModels
            {
                Data = logcambio,
                CatalogoSelectId = tablaId,
                SelectedValue = selectedValue
            };

            return PartialView("_LogCambio", model);
        }




        private void instanciarCatalogoBusqueda()
        {
            LogCambio obj = new LogCambio();
            obj.tablaId = 0;
            obj.estadoTabla = 0;            
            obj.nombreTabla = String.Empty;           


            this.Session[Constantes.VAR_SESSION_LOGCAMBIO_BUSQUEDA] = obj;
        }
        







        private LogCambio LogCambioSession
        {
            get
            {
                LogCambio obj = null;

                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {

                    case Constantes.paginas.BusquedaLogCambio: obj = (LogCambio)this.Session[Constantes.VAR_SESSION_LOGCAMBIO_BUSQUEDA]; break;
                    case Constantes.paginas.MantenimientoLogCambio: obj = (LogCambio)this.Session[Constantes.VAR_SESSION_LOGCAMBIO]; break;
                }
                return obj;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaLogCambio: this.Session[Constantes.VAR_SESSION_LOGCAMBIO_BUSQUEDA] = value; break;
                    case Constantes.paginas.MantenimientoLogCambio: this.Session[Constantes.VAR_SESSION_LOGCAMBIO] = value; break;
                }
            }
        }

        public void LimpiarBusqueda()
        {
            instanciarCatalogoBusqueda();

        }

        public String ChangeIdTabla()
        {

            LogCambio logcambio = this.LogCambioSession;
            int idTabla = 0;            
            if (this.Request.Params["idTabla"] != null && !this.Request.Params["idTabla"].Equals(""))
            {
                idTabla = int.Parse(this.Request.Params["idTabla"]);

            }
            logcambio.tablaId = idTabla;               
            this.LogCambioSession = logcambio;

            return JsonConvert.SerializeObject(logcambio.tablaId);
        }


        public String Update()
        {

            LogCambio obj = new LogCambio();

            obj.catalogoId = Int32.Parse(Request["catalogoId"].ToString());
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Int32.Parse(this.Request.Params["valor"]));
            LogCambioBL bL = new LogCambioBL();            
            if (propertyInfo.Name == "estadoCatalogo")
            {
                obj.puede_persistir = 3;
                obj = bL.updateCatalogo(obj);
            }

            if (propertyInfo.Name == "puede_persistir")
            {
                obj.estadoCatalogo = 3;
                obj = bL.updateCatalogo(obj);
            }
            LogCambioSession = obj;
            String resultado = JsonConvert.SerializeObject(obj);

            return resultado;
           
        }

        


    }
}

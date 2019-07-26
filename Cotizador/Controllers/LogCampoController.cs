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
    public class LogCampoController : Controller
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


        [HttpGet]
        public ActionResult Lista()
        {

            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BusquedaLogCampo;
            if (this.Session[Constantes.VAR_SESSION_LOGCAMPO_BUSQUEDA] == null)
            {
                instanciarCatalogoBusqueda();
            }

            LogCampo objSearch = (LogCampo)this.Session[Constantes.VAR_SESSION_LOGCAMPO_BUSQUEDA];
            ViewBag.pagina = (int)Constantes.paginas.BusquedaLogCampo;
            ViewBag.catalogo = objSearch;

            return View();
        }

        public String SearchList()
        {

            //Se indica la página con la que se va a trabajar
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaLogCampo;
            //Se recupera el objeto cliente que contiene los criterios de Búsqueda de la session
            LogCampo obj = (LogCampo)this.Session[Constantes.VAR_SESSION_LOGCAMPO_BUSQUEDA];
            LogCampoBL bL = new LogCampoBL();
            List<LogCampo> list = bL.getCatalogoById(obj);
            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_LOGCAMPO_LISTA] = list;
            //Se retorna la cantidad de elementos encontrados
            return JsonConvert.SerializeObject(list);
        }


        public ActionResult GetTablas(string tablaId, string selectedValue = null)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            LogCampoBL logCampoBL = new LogCampoBL();
            List<LogCampo> logCampo = logCampoBL.getTablasList();


            var model = new LogCampoViewModels
            {
                Data = logCampo,
                CatalogoSelectId = tablaId,
                SelectedValue = selectedValue
            };

            return PartialView("_LogCampo", model);
        }




        private void instanciarCatalogoBusqueda()
        {
            LogCampo obj = new LogCampo();
            obj.tablaId = 0;
            obj.estadoTabla = 0;            
            obj.nombreTabla = String.Empty;           


            this.Session[Constantes.VAR_SESSION_LOGCAMPO_BUSQUEDA] = obj;
        }
        







        private LogCampo LogCampoSession
        {
            get
            {
                LogCampo obj = null;

                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {

                    case Constantes.paginas.BusquedaLogCampo: obj = (LogCampo)this.Session[Constantes.VAR_SESSION_LOGCAMPO_BUSQUEDA]; break;
                    case Constantes.paginas.MantenimientoLogCampo: obj = (LogCampo)this.Session[Constantes.VAR_SESSION_LOGCAMPO]; break;
                }
                return obj;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaLogCampo: this.Session[Constantes.VAR_SESSION_LOGCAMPO_BUSQUEDA] = value; break;
                    case Constantes.paginas.MantenimientoLogCampo: this.Session[Constantes.VAR_SESSION_LOGCAMPO] = value; break;
                }
            }
        }

        public void LimpiarBusqueda()
        {
            instanciarCatalogoBusqueda();

        }

        public String ChangeIdTabla()
        {

            LogCampo logCampo = this.LogCampoSession;
            int idTabla = 0;            
            if (this.Request.Params["idTabla"] != null && !this.Request.Params["idTabla"].Equals(""))
            {
                idTabla = int.Parse(this.Request.Params["idTabla"]);

            }
            logCampo.tablaId = idTabla;               
            this.LogCampoSession = logCampo;

            return JsonConvert.SerializeObject(logCampo.tablaId);
        }


        public String Update()
        {

            LogCampo obj = new LogCampo();

            obj.catalogoId = Int32.Parse(Request["catalogoId"].ToString());
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Int32.Parse(this.Request.Params["valor"]));
            LogCampoBL bL = new LogCampoBL();            
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
            
            String resultado = JsonConvert.SerializeObject(obj);

            return resultado;
           
        }






        


    }
}

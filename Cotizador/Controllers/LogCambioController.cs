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
            LogCambio list = bL.getCatalogoById(obj);
            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_LOGCAMBIO_LISTA] = list;
            //Se retorna la cantidad de elementos encontrados
            return JsonConvert.SerializeObject(list);
        }


        public ActionResult GetTablas(string catalogoId, string selectedValue = null)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            LogCambioBL grupoClienteBL = new LogCambioBL();
            List<LogCambio> grupoClienteList = grupoClienteBL.getCatalogoList();


            var model = new LogCambioViewModels
            {
                Data = grupoClienteList,
                CatalogoSelectId = catalogoId,
                SelectedValue = selectedValue
            };

            return PartialView("_LogCambio", model);
        }




        private void instanciarCatalogoBusqueda()
        {
            LogCambio obj = new LogCambio();
            obj.catalogoId = 0;
            obj.estadoCatalogo = 0;
            obj.codigo = String.Empty;
            obj.nombre = String.Empty;
            obj.puede_persistir = 0;


            this.Session[Constantes.VAR_SESSION_LOGCAMBIO_BUSQUEDA] = obj;
        }
        public void ChangeInputInt()
        {

            LogCambio obj = (LogCambio)this.CatalogoSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Int32.Parse(this.Request.Params["valor"]));
            this.CatalogoSession = obj;
        }







        private LogCambio CatalogoSession
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

        public String ChangeIdCiudad()
        {

            LogCambio vendedor = this.CatalogoSession;
            int idCiudad = 0;
            int estado = 0;
            int puede_persistir = 0;
            if (this.Request.Params["idCiudad"] != null && !this.Request.Params["idCiudad"].Equals(""))
            {
                idCiudad = int.Parse(this.Request.Params["idCiudad"]);

            }
            vendedor.catalogoId = idCiudad;
            LogCambioBL grupoClienteBL = new LogCambioBL();
            vendedor = grupoClienteBL.getCatalogoById(vendedor);
            this.CatalogoSession = vendedor;

            return JsonConvert.SerializeObject(vendedor.catalogoId);
        }


        public String Update()
        {

            LogCambioBL bL = new LogCambioBL();
            LogCambio obj = (LogCambio)this.CatalogoSession;
            obj = bL.updateCatalogo(obj);
            this.Session[Constantes.VAR_SESSION_LOGCAMBIO] = null;
            String resultado = JsonConvert.SerializeObject(obj);

            return resultado;
        }


        public bool cancel()
        {

            bool resultado = false;

            return resultado;
        }
        public bool accept()
        {
            bool resultado = true;

            return resultado;
        }


    }
}

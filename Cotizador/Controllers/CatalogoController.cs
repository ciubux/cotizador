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
    public class CatalogoController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            if (!usuario.modificaCatalogo)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }


        [HttpGet]
        public ActionResult Lista()
        {

            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BusquedaCatalogo;
            if (this.Session[Constantes.VAR_SESSION_CATALOGO_BUSQUEDA] == null)
            {
                instanciarCatalogoBusqueda();
            }

            Catalogo objSearch = (Catalogo)this.Session[Constantes.VAR_SESSION_CATALOGO_BUSQUEDA];
            ViewBag.pagina = (int)Constantes.paginas.BusquedaCatalogo;
            ViewBag.catalogo = objSearch;

            return View();
        }

        public String SearchList()
        {

            //Se indica la página con la que se va a trabajar
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaCatalogo;
            //Se recupera el objeto cliente que contiene los criterios de Búsqueda de la session
            Catalogo obj = (Catalogo)this.Session[Constantes.VAR_SESSION_CATALOGO_BUSQUEDA];
            CatalogoBL bL = new CatalogoBL();
            Catalogo list = bL.getCatalogoById(obj);
            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_CATALOGO_LISTA] = list;
            //Se retorna la cantidad de elementos encontrados
            return JsonConvert.SerializeObject(list);
        }


        public ActionResult GetTablas(string catalogoId, string selectedValue = null)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            CatalogoBL grupoClienteBL = new CatalogoBL();
            List<Catalogo> grupoClienteList = grupoClienteBL.getCatalogoList();


            var model = new CatalogoViewModels
            {
                Data = grupoClienteList,
                CatalogoSelectId = catalogoId,
                SelectedValue = selectedValue
            };

            return PartialView("_Catalogo", model);
        }


       

        private void instanciarCatalogoBusqueda()
        {
            Catalogo obj = new Catalogo();
            obj.catalogoId = 0;
            obj.estado = 0;
            obj.codigo = String.Empty;
            obj.nombre = String.Empty;
            obj.puede_persistir = 0;


            this.Session[Constantes.VAR_SESSION_CATALOGO_BUSQUEDA] = obj;
        }
        public void ChangeInputInt()
        {

            Catalogo obj = (Catalogo)this.CatalogoSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Int32.Parse(this.Request.Params["valor"]));
            this.CatalogoSession = obj;
        }  
        






        private Catalogo CatalogoSession
        {
            get
            {
                Catalogo obj = null;

                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {

                    case Constantes.paginas.BusquedaCatalogo: obj = (Catalogo)this.Session[Constantes.VAR_SESSION_CATALOGO_BUSQUEDA]; break;
                    case Constantes.paginas.MantenimientoCatalogo: obj = (Catalogo)this.Session[Constantes.VAR_SESSION_CATALOGO]; break;
                }
                return obj;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaCatalogo: this.Session[Constantes.VAR_SESSION_CATALOGO_BUSQUEDA] = value; break;
                    case Constantes.paginas.MantenimientoCatalogo: this.Session[Constantes.VAR_SESSION_CATALOGO] = value; break;
                }
            }
        }

        public void LimpiarBusqueda()
        {
            instanciarCatalogoBusqueda();
          
        }

        public String ChangeIdCiudad()
        {
           
            Catalogo vendedor = this.CatalogoSession;
            int idCiudad = 0;
            int estado = 0;
            int puede_persistir = 0;
            if (this.Request.Params["idCiudad"] != null && !this.Request.Params["idCiudad"].Equals(""))
            {
                idCiudad = int.Parse(this.Request.Params["idCiudad"]);
               
            }            
            vendedor.catalogoId = idCiudad;
            CatalogoBL grupoClienteBL = new CatalogoBL();            
            vendedor = grupoClienteBL.getCatalogoById(vendedor);            
            this.CatalogoSession = vendedor;

            return JsonConvert.SerializeObject(vendedor.catalogoId);
        }


        public String Update()
        {
            
            CatalogoBL bL = new CatalogoBL();
            Catalogo obj = (Catalogo)this.CatalogoSession;
            obj = bL.updateCatalogo(obj);
            this.Session[Constantes.VAR_SESSION_CATALOGO] = null;            
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

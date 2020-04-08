using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLayer;
using Model;
using Newtonsoft.Json;
using Cotizador.Models;

using System.Reflection;

namespace Cotizador.Controllers
{
    public class VistaDashboardController : Controller
    {

        private VistaDashboard VistaDashboardSession
        {
            get
            {
                VistaDashboard obj = null;

                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {

                    case Constantes.paginas.BusquedaVistaDashboard: obj = (VistaDashboard)this.Session[Constantes.VAR_SESSION_VISTA_DASHBOARD_BUSQUEDA]; break;
                    case Constantes.paginas.MantenimientoVistaDashboard: obj = (VistaDashboard)this.Session[Constantes.VAR_SESSION_VISTA_DASHBOARD]; break;
                }
                return obj;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaVistaDashboard: this.Session[Constantes.VAR_SESSION_VISTA_DASHBOARD_BUSQUEDA] = value; break;
                    case Constantes.paginas.MantenimientoVistaDashboard: this.Session[Constantes.VAR_SESSION_VISTA_DASHBOARD] = value; break;
                }
            }
        }

        // GET: Dashboard
        public ActionResult Index()
        {
            return View();
        }
        private void instanciarVistaDashboardBusqueda()
        {
            VistaDashboard obj = new VistaDashboard();
            obj.idVistaDashboard = 0;
            obj.estado = 1;
            obj.descripcion = String.Empty; 
            
           
            obj.idTipoVistaDashboard = 0;            

            this.Session[Constantes.VAR_SESSION_VISTA_DASHBOARD_BUSQUEDA] = obj;
        }

        public ActionResult Lista()
        {                             
            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BusquedaVistaDashboard;

            if (this.Session[Constantes.VAR_SESSION_VISTA_DASHBOARD_BUSQUEDA] == null)
            {
                instanciarVistaDashboardBusqueda();
            }
            
            VistaDashboard objSearch = (VistaDashboard)this.Session[Constantes.VAR_SESSION_VISTA_DASHBOARD_BUSQUEDA];
            
           ViewBag.pagina = (int)Constantes.paginas.BusquedaVistaDashboard;
            ViewBag.dashboard = objSearch;
            /*ViewBag.permisos = permisos;
           */
    ViewBag.vistaDashboard = objSearch;
            
            return View();
        }

        public String SearchList()
        {           
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaVistaDashboard;
            VistaDashboard objSearch = (VistaDashboard)this.Session[Constantes.VAR_SESSION_VISTA_DASHBOARD_BUSQUEDA];
           
            VistaDashboardBL bl = new VistaDashboardBL();
            List<VistaDashboard> lista = bl.getVistasDashboard(objSearch);          
           
            //Se retorna la cantidad de elementos encontrados
            return JsonConvert.SerializeObject(lista);
        }                

        public void ChangeInputInt()
        {
            VistaDashboard obj = (VistaDashboard)this.VistaDashboardSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, this.Request.Params["valor"] == "" ? 0 : Int32.Parse(this.Request.Params["valor"]));
            this.VistaDashboardSession = obj;
        }

        public void ChangeInputString()
        {
            VistaDashboard obj = (VistaDashboard)this.VistaDashboardSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, this.Request.Params["valor"]);
            this.VistaDashboardSession = obj;

        }

        [HttpGet]
        public ActionResult Editar(Int32? idVistaDashboard = null)
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.MantenimientoVistaDashboard;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (!usuario.modificaVistaDashboard)
            {
                return RedirectToAction("Lista", "Dashboard");
            }           

            if (this.Session[Constantes.VAR_SESSION_VISTA_DASHBOARD] == null)
            {
                instanciarVistaDashboard();
            }
            VistaDashboard obj = (VistaDashboard)this.Session[Constantes.VAR_SESSION_VISTA_DASHBOARD];

            if (idVistaDashboard != null)
            {
                VistaDashboardBL bL = new VistaDashboardBL();
                obj = bL.getVistaDashboardById(idVistaDashboard.Value);                

                this.Session[Constantes.VAR_SESSION_VISTA_DASHBOARD] = obj;
            }

            ViewBag.VistaDashboard = obj;
          
            return View();

        }

        private void instanciarVistaDashboard()
        {
            VistaDashboard obj = new VistaDashboard();            
            obj.codigo = String.Empty;
            obj.nombre = String.Empty;
            this.Session[Constantes.VAR_SESSION_VISTA_DASHBOARD] = obj;
        }

        public String ConsultarSiExisteVistaDashboard()
        {
            int idVistaDashboard = Int16.Parse(Request["idVistaDashboard"].ToString());
            VistaDashboardBL bL = new VistaDashboardBL();
            VistaDashboard obj = bL.getVistaDashboardById(idVistaDashboard);
            String resultado = JsonConvert.SerializeObject(obj);
            this.Session[Constantes.VAR_SESSION_VISTA_DASHBOARD_VER] = obj;
            obj = (VistaDashboard)this.Session[Constantes.VAR_SESSION_VISTA_DASHBOARD];
            if (obj == null || obj.idVistaDashboard == 0)
                return "{\"existe\":\"false\",\"idVistaDashboard\":\"0\"}";
            else
                return "{\"existe\":\"true\",\"idVistaDashboard\":\"" + obj.idVistaDashboard + "\"}";
        }

        public void iniciarEdicionVistaDashboard()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            VistaDashboard obj = (VistaDashboard)this.Session[Constantes.VAR_SESSION_VISTA_DASHBOARD_VER];
            this.Session[Constantes.VAR_SESSION_VISTA_DASHBOARD] = obj;
        }
        
        public ActionResult CancelarCreacionDashboard()
        {
            this.Session[Constantes.VAR_SESSION_VISTA_DASHBOARD] = null;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            return RedirectToAction("Lista", "VistaDashboard");
        }

        public String Update()
        {

            VistaDashboardBL bL = new VistaDashboardBL();
            VistaDashboard obj = (VistaDashboard)this.Session[Constantes.VAR_SESSION_VISTA_DASHBOARD];
            Usuario user = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (obj.idVistaDashboard == null || obj.idVistaDashboard == 0)
            {
                bL.insertVistaDashboard(obj,user.idUsuario);
                this.Session[Constantes.VAR_SESSION_VISTA_DASHBOARD] = null;
            }
            else
            {

                obj=bL.updateVistaDashboard(obj, user.idUsuario);
                this.Session[Constantes.VAR_SESSION_VISTA_DASHBOARD] = null;
            }
            String resultado = JsonConvert.SerializeObject(obj);

            return resultado;
        }
        public String Create()
        {
            VistaDashboardBL bL = new VistaDashboardBL();
            VistaDashboard obj = (VistaDashboard)this.Session[Constantes.VAR_SESSION_VISTA_DASHBOARD];
            Usuario user = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            obj = bL.insertVistaDashboard(obj,user.idUsuario);
            this.Session[Constantes.VAR_SESSION_VISTA_DASHBOARD] = null;
            String resultado = JsonConvert.SerializeObject(obj);           
            return resultado;
        }


        public ActionResult GetTiposVistaDashboard(string TipoVistaDashboardSelectId, string selectedValue = null)
        {
            TipoVistaDashboardBL obj = new TipoVistaDashboardBL();            
            List<TipoVistaDashboard> tiposVistaDashboard = obj.getTipoVistaDashboard();

            var model = new TipoVistaDashboardViewModels
            {
                Data = tiposVistaDashboard,
                TipoVistaDashboardSelectId = TipoVistaDashboardSelectId,
                SelectedValue = selectedValue
            };

            return PartialView("_TipoVistaDashboard", model);
        }
    }
}
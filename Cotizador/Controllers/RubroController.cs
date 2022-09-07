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
    public class RubroController : ParentController
    {
        public ActionResult GetRubros(string rubroSelectId, string selectedValue = null, int idPadre = 0, int sinNoAsignado = 0)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            Rubro obj = new Rubro();
            obj.Estado = 1;

            RubroBL bL = new RubroBL();
            List<Rubro> list = bL.getRubros(obj, idPadre, sinNoAsignado);

            var model = new RubroViewModels
            {
                Data = list,
                RubroSelectId = rubroSelectId,
                SelectedValue = selectedValue,
                idPadre = idPadre
            };

            return PartialView("_Rubro", model);
        }


        [HttpGet]
        public ActionResult List()
        {

            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BusquedaRubros;
            
            if (this.Session[Constantes.VAR_SESSION_RUBRO_BUSQUEDA] == null)
            {
                instanciarRubroBusqueda();
            }
            

            Rubro objSearch = (Rubro)this.Session[Constantes.VAR_SESSION_RUBRO_BUSQUEDA];

            ViewBag.pagina = (int)Constantes.paginas.BusquedaRubros;
            ViewBag.rubro = objSearch;

            return View();
        }

        public String SearchList()
        {
            //Se indica la página con la que se va a trabajar
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaRubros;
            //Se recupera el objeto cliente que contiene los criterios de Búsqueda de la session
            Rubro obj = (Rubro)this.Session[Constantes.VAR_SESSION_RUBRO_BUSQUEDA];

            RubroBL bL = new RubroBL();
            List<Rubro> list = bL.getRubros(obj);
            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_RUBRO_LISTA] = list;
            //Se retorna la cantidad de elementos encontrados
            return JsonConvert.SerializeObject(list);

        }

        public String Show()
        {
            int idRubro = int.Parse(Request["idRubro"].ToString());
            RubroBL bL = new RubroBL();
            Rubro obj = bL.getRubroById(idRubro);
            obj.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            String resultado =  JsonConvert.SerializeObject(obj);
            this.Session[Constantes.VAR_SESSION_RUBRO_VER] = obj;
            return resultado;
        }

        public ActionResult Editar(int? idRubro = null)
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.MantenimientoRubro;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (!usuario.modificaRubro)
            {
                return RedirectToAction("Login", "Account");
            }



            if (this.Session[Constantes.VAR_SESSION_RUBRO] == null && idRubro == null)
            {
                instanciarRubro();
            }

            Rubro obj = (Rubro)this.Session[Constantes.VAR_SESSION_RUBRO];
            
            if (idRubro != null)
            {
                RubroBL bL = new RubroBL();
                obj = bL.getRubroById(idRubro.Value);
                obj.IdUsuarioRegistro = usuario.idUsuario;
                obj.usuario = usuario;
                
                this.Session[Constantes.VAR_SESSION_RUBRO] = obj;
            }
            

            ViewBag.rubro = obj;
            return View();

        }

        private void instanciarRubro()
        {
            Rubro obj = new Rubro();
            obj.idRubro = 0;
            obj.Estado = 1;
            RubroBL bl = new RubroBL();
            obj.codigo = bl.getSiguienteCodigoRubro();
            obj.nombre = String.Empty;
            
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            obj.IdUsuarioRegistro = usuario.idUsuario;
            obj.usuario = usuario;

            this.Session[Constantes.VAR_SESSION_RUBRO] = obj;
        }

        private void instanciarRubroBusqueda()
        {
            Rubro obj = new Rubro();
            obj.idRubro = 0;
            obj.Estado = 1;
            obj.codigo = String.Empty;
            obj.nombre = String.Empty;

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            obj.IdUsuarioRegistro = usuario.idUsuario;
            obj.usuario = usuario;

            this.Session[Constantes.VAR_SESSION_RUBRO_BUSQUEDA] = obj;
        }


        [HttpGet]
        public ActionResult ExportLastSearchExcel()
        {
            List<Rubro> list = (List<Rubro>)this.Session[Constantes.VAR_SESSION_RUBRO_LISTA];

            RubroSearch excel = new RubroSearch();
            return excel.generateExcel(list, (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO]);
        }

        

        // GET: Rubro
        [HttpGet]
        public ActionResult Index()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            if (!usuario.modificaRubro)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();

        }

        public String ConsultarSiExisteRubro()
        {
            int idRubro = int.Parse(Request["idRubro"].ToString());
            RubroBL bL = new RubroBL();
            Rubro obj = bL.getRubroById(idRubro);
            obj.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            String resultado = JsonConvert.SerializeObject(obj);
            this.Session[Constantes.VAR_SESSION_RUBRO_VER] = obj;

            obj = (Rubro)this.Session[Constantes.VAR_SESSION_RUBRO];
            if (obj == null)
                return "{\"existe\":\"false\",\"idRubro\":\"0\"}";
            else
                return "{\"existe\":\"true\",\"idRubro\":\"" + obj.idRubro + "\"}";
        }


        public void iniciarEdicionRubro()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            Rubro obj = (Rubro)this.Session[Constantes.VAR_SESSION_RUBRO_VER];
            this.Session[Constantes.VAR_SESSION_RUBRO] = obj;
        }

        public String Create()
        {
            RubroBL bL = new RubroBL();
            Rubro obj = (Rubro)this.Session[Constantes.VAR_SESSION_RUBRO];

            obj = bL.insertRubro(obj);
            this.Session[Constantes.VAR_SESSION_RUBRO] = null;
            String resultado = JsonConvert.SerializeObject(obj);
            return resultado;
        }


        public String Update()
        {
            RubroBL bL = new RubroBL();
            Rubro obj = (Rubro)this.Session[Constantes.VAR_SESSION_RUBRO];

            if (obj.idRubro == 0)
            {
                obj = bL.insertRubro(obj);
                this.Session[Constantes.VAR_SESSION_RUBRO] = null;
            }
            else
            {
                obj = bL.updateRubro(obj);
                this.Session[Constantes.VAR_SESSION_RUBRO] = null;
            }
            String resultado = JsonConvert.SerializeObject(obj);
            //this.Session[Constantes.VAR_SESSION_CLIENTE] = null;
            return resultado;
        }
        
        public void ChangeInputString()
        {
            Rubro obj = (Rubro) this.RubroSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, this.Request.Params["valor"]);
            this.RubroSession = obj;
        }

        public void ChangeInputInt()
        {
            Rubro obj = (Rubro)this.RubroSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Int32.Parse(this.Request.Params["valor"]));
            this.RubroSession = obj;
        }

        public void ChangeInputDecimal()
        {
            Rubro obj = (Rubro)this.RubroSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Decimal.Parse(this.Request.Params["valor"]));
            this.RubroSession = obj;
        }
        public void ChangeInputBoolean()
        {
            Rubro obj = (Rubro)this.RubroSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Int32.Parse(this.Request.Params["valor"]) == 1);
            this.RubroSession = obj;
        }


        private Rubro RubroSession
        {
            get
            {
                Rubro obj = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaRubros: obj = (Rubro)this.Session[Constantes.VAR_SESSION_RUBRO_BUSQUEDA]; break;
                    case Constantes.paginas.MantenimientoRubro: obj = (Rubro)this.Session[Constantes.VAR_SESSION_RUBRO]; break;
                }
                return obj;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaRubros: this.Session[Constantes.VAR_SESSION_RUBRO_BUSQUEDA] = value; break;
                    case Constantes.paginas.MantenimientoRubro: this.Session[Constantes.VAR_SESSION_RUBRO] = value; break;
                }
            }
        }

        public ActionResult CancelarCreacionRubro()
        {
            this.Session[Constantes.VAR_SESSION_RUBRO] = null;
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            //   usuarioBL.updateCotizacionSerializada(usuario, null);
            return RedirectToAction("List", "Rubro");

        }
    }


}
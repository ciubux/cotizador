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
    public class RolController : ParentController
    {

        [HttpGet]
        public ActionResult List()
        {

            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BusquedaRoles;
            
            if (this.Session[Constantes.VAR_SESSION_ROL_BUSQUEDA] == null)
            {
                instanciarRolBusqueda();
            }
            

            Rol objSearch = (Rol)this.Session[Constantes.VAR_SESSION_ROL_BUSQUEDA];

            ViewBag.pagina = (int)Constantes.paginas.BusquedaRoles;
            ViewBag.rol = objSearch;

            return View();
        }

        public ActionResult GetRoles(string rolSelectId, string selectedValue = null, string disabled = null)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            Rol obj = new Rol();
            obj.Estado = 1;

            RolBL bL = new RolBL();
            List<Rol> list = bL.getRoles(obj);

            var model = new RolViewModels
            {
                Data = list,
                RolSelectId = rolSelectId,
                SelectedValue = selectedValue,
                Disabled = disabled == null || disabled != "disabled" ? false : true
            };

            return PartialView("_Rol", model);


        }
        public String SearchList()
        {
            //Se indica la página con la que se va a trabajar
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaRoles;
            //Se recupera el objeto cliente que contiene los criterios de Búsqueda de la session
            Rol obj = (Rol)this.Session[Constantes.VAR_SESSION_ROL_BUSQUEDA];

            RolBL bL = new RolBL();
            List<Rol> list = bL.getRoles(obj);
            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_ROL_LISTA] = list;
            //Se retorna la cantidad de elementos encontrados
            return JsonConvert.SerializeObject(list);

        }

        public String Show()
        {
            int idRol = int.Parse(Request["idRol"].ToString());
            RolBL bL = new RolBL();
            Rol obj = bL.getRol(idRol);
            obj.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            String resultado =  JsonConvert.SerializeObject(obj);
            this.Session[Constantes.VAR_SESSION_ROL_VER] = obj;
            return resultado;
        }

        public ActionResult Editar(int? idRol = null)
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.MantenimientoRol;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (!usuario.modificaRol)
            {
                return RedirectToAction("Login", "Account");
            }
            


            if (this.Session[Constantes.VAR_SESSION_ROL] == null && idRol == null)
            {
                instanciarRol();
            }

            Rol obj = (Rol)this.Session[Constantes.VAR_SESSION_ROL];
            
            if (idRol != null)
            {
                RolBL bL = new RolBL();
                obj = bL.getRol(idRol.Value);
                obj.IdUsuarioRegistro = usuario.idUsuario;
                obj.usuario = usuario;
                
                this.Session[Constantes.VAR_SESSION_ROL] = obj;
            }
            

            ViewBag.origen = obj;
            return View();

        }

        private void instanciarRol()
        {
            Rol obj = new Rol();
            obj.idRol = 0;
            obj.Estado = 1;
            obj.codigo = String.Empty;
            obj.nombre = String.Empty;
            
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            obj.IdUsuarioRegistro = usuario.idUsuario;
            obj.usuario = usuario;

            this.Session[Constantes.VAR_SESSION_ROL] = obj;
        }

        private void instanciarRolBusqueda()
        {
            Rol obj = new Rol();
            obj.idRol = 0;
            obj.Estado = 1;
            obj.codigo = String.Empty;
            obj.nombre = String.Empty;

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            obj.IdUsuarioRegistro = usuario.idUsuario;
            obj.usuario = usuario;

            this.Session[Constantes.VAR_SESSION_ROL] = obj;
        }


        //[HttpGet]
        //public ActionResult ExportLastSearchExcel()
        //{
        //    List<Rol> list = (List<Rol>)this.Session[Constantes.VAR_SESSION_ROL_LISTA];

        //    RolSearch excel = new RolSearch();
        //    return excel.generateExcel(list, (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO]);
        //}

        

        // GET: Rol
        [HttpGet]
        public ActionResult Index()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            if (!usuario.modificaRol)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();

        }

        public String ConsultarSiExisteRol()
        {
            int idRol = int.Parse(Request["idRol"].ToString());
            RolBL bL = new RolBL();
            Rol obj = bL.getRol(idRol);
            obj.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            String resultado = JsonConvert.SerializeObject(obj);
            this.Session[Constantes.VAR_SESSION_ROL_VER] = obj;

            obj = (Rol)this.Session[Constantes.VAR_SESSION_ROL];
            if (obj == null)
                return "{\"existe\":\"false\",\"idRol\":\"0\"}";
            else
                return "{\"existe\":\"true\",\"idRol\":\"" + obj.idRol + "\"}";
        }


        public void iniciarEdicionRol()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            Rol obj = (Rol)this.Session[Constantes.VAR_SESSION_ROL_VER];
            this.Session[Constantes.VAR_SESSION_ROL] = obj;
        }

        public String Create()
        {
            RolBL bL = new RolBL();
            Rol obj = (Rol)this.Session[Constantes.VAR_SESSION_ROL];

            obj = bL.insertRol(obj);
            this.Session[Constantes.VAR_SESSION_ROL] = null;
            String resultado = JsonConvert.SerializeObject(obj);
            return resultado;
        }


        public String Update()
        {
            RolBL bL = new RolBL();
            Rol obj = (Rol)this.Session[Constantes.VAR_SESSION_ROL];

            if (obj.idRol == 0)
            {
                obj = bL.insertRol(obj);
                this.Session[Constantes.VAR_SESSION_ROL] = null;
            }
            else
            {
                obj = bL.updateRol(obj);
                this.Session[Constantes.VAR_SESSION_ROL] = null;
            }
            String resultado = JsonConvert.SerializeObject(obj);
            //this.Session[Constantes.VAR_SESSION_CLIENTE] = null;
            return resultado;
        }
        
        public void ChangeInputString()
        {
            Rol obj = (Rol) this.RolSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, this.Request.Params["valor"]);
            this.RolSession = obj;
        }

        public void ChangeInputInt()
        {
            Rol obj = (Rol)this.RolSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Int32.Parse(this.Request.Params["valor"]));
            this.RolSession = obj;
        }

        public void ChangeInputDecimal()
        {
            Rol obj = (Rol)this.RolSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Decimal.Parse(this.Request.Params["valor"]));
            this.RolSession = obj;
        }
        public void ChangeInputBoolean()
        {
            Rol obj = (Rol)this.RolSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Int32.Parse(this.Request.Params["valor"]) == 1);
            this.RolSession = obj;
        }


        private Rol RolSession
        {
            get
            {
                Rol obj = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaRoles: obj = (Rol)this.Session[Constantes.VAR_SESSION_ROL_BUSQUEDA]; break;
                    case Constantes.paginas.MantenimientoRol: obj = (Rol)this.Session[Constantes.VAR_SESSION_ROL]; break;
                }
                return obj;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaRoles: this.Session[Constantes.VAR_SESSION_ROL_BUSQUEDA] = value; break;
                    case Constantes.paginas.MantenimientoRol: this.Session[Constantes.VAR_SESSION_ROL] = value; break;
                }
            }
        }

        public ActionResult CancelarCreacionRol()
        {
            this.Session[Constantes.VAR_SESSION_ROL] = null;
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            //   usuarioBL.updateCotizacionSerializada(usuario, null);
            return RedirectToAction("List", "Rol");

        }
    }


}
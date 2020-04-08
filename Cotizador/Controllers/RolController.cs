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
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            if (!usuario.visualizaRoles && !usuario.modificaRol)
            {
                return RedirectToAction("Login", "Account");
            }

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
            String resultado = JsonConvert.SerializeObject(obj);
            this.Session[Constantes.VAR_SESSION_ROL_VER] = obj;
            return resultado;
        }

        public ActionResult Editar(int? idRol = null)
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.MantenimientoRol;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (!usuario.modificaRol)
            {
                return RedirectToAction("List", "Rol");
            }


            PermisoBL permisobl = new PermisoBL();
            List<Permiso> permisos = new List<Permiso>();
            permisos = permisobl.getPermisos();
            this.Session[Constantes.VAR_SESSION_PERMISO_LISTA] = permisos;

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


            ViewBag.rol = obj;
            ViewBag.permisos = permisos;

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

            this.Session[Constantes.VAR_SESSION_ROL_BUSQUEDA] = obj;
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

            PermisoBL permisobl = new PermisoBL();
            List<Permiso> permisos = new List<Permiso>();
            permisos = permisobl.getPermisos();



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

        public ActionResult Usuarios(int? idRol)
        {
            RolBL bl = new RolBL();
            Rol rol = new Rol();

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (!usuario.modificaRol)
            {
                return RedirectToAction("List", "Rol");
            }

            if (idRol == null || idRol == 0)
            {
                return RedirectToAction("List", "Rol");
            }

            rol = bl.getRol(idRol.Value);
            rol.usuario = usuario;
            rol.IdUsuarioRegistro = usuario.idUsuario;
            rol.usuarios = bl.getUsuarios(rol.idRol);


            ViewBag.rol = rol;
            return View();
        }

        [HttpPost]
        public String AddUsuarioRol()
        {
            int success = 1;
            string message = "";
            UsuarioBL usuarioBl = new UsuarioBL();
            RolBL bl = new RolBL();


            Guid idUsuario = Guid.Parse(this.Request.Params["idUsuario"]);
            int idRol = int.Parse(this.Request.Params["idRol"]);

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            Rol rol = bl.getRol(idRol);
            rol.usuario = usuario;
            rol.IdUsuarioRegistro = usuario.idUsuario;
            rol.usuarios = bl.getUsuarios(rol.idRol);

            if (!usuario.modificaRol)
            {
                return "";
            }


            foreach (Usuario item in rol.usuarios)
            {
                if (item.idUsuario == idUsuario)
                {
                    success = 0;
                    message = "El usuario ya está agregado.";
                }
            }

            Usuario addItem = null;
            if (success == 1)
            {
                addItem = usuarioBl.getUsuario(idUsuario);

                bl.agregarUsuarioRol(rol.idRol, addItem.idUsuario, usuario.idUsuario);
                rol.usuarios.Add(addItem);

                message = "Se agregó el usuario correctamente.";
            }

            String itemJson = JsonConvert.SerializeObject(addItem);

            return "{\"success\": " + success.ToString() + ", \"message\": \"" + message + "\", \"usuario\":" + itemJson + "}";
        }

        [HttpPost]
        public String QuitarUsuarioRol()
        {
            int success = 0;
            string message = "";
            UsuarioBL usuarioBl = new UsuarioBL();
            RolBL bl = new RolBL();

            Guid idUsuario = Guid.Parse(this.Request.Params["idUsuario"]);
            int idRol = int.Parse(this.Request.Params["idRol"]);

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            Rol rol = bl.getRol(idRol);
            rol.usuario = usuario;
            rol.IdUsuarioRegistro = usuario.idUsuario;
            rol.usuarios = bl.getUsuarios(rol.idRol);


            int removeAt = -1;
            foreach (Usuario item in rol.usuarios)
            {
                if (item.idUsuario == idUsuario)
                {
                    success = 1;
                    removeAt = rol.usuarios.IndexOf(item);
                }
            }

            rol.usuario = usuario;

            if (!usuario.modificaRol)
            {
                success = 0;
            }


            if (success == 1)
            {
                bl.quitarUsuarioRol(rol.idRol, idUsuario);
                rol.usuarios.RemoveAt(removeAt);
                message = "Se removió el usuario.";
            }
            else
            {
                message = "El usuario no existe o no esta agregado al rol.";
            }

            return "{\"success\": " + success.ToString() + ", \"message\": \"" + message + "\"}";
        }

        public void ChangeInputString()
        {
            Rol obj = (Rol)this.RolSession;
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

        public void ChangePermiso()
        {
            Rol obj = (Rol)this.RolSession;

            int valor = Int32.Parse(this.Request.Params["valor"]);
            int permiso = Int32.Parse(this.Request.Params["permiso"].ToString().Replace("permiso_", ""));

            List<Permiso> listaPermisos = (List<Permiso>)this.Session[Constantes.VAR_SESSION_PERMISO_LISTA];

            if (valor == 0)
            {
                //Remove
                foreach (Permiso per in obj.permisos)
                {
                    if (permiso == per.idPermiso)
                    {
                        obj.permisos.Remove(per);
                        break;
                    }
                }
            }
            else
            {
                //ADD
                foreach (Permiso per in listaPermisos)
                {
                    if (permiso == per.idPermiso)
                    {
                        obj.permisos.Add(per);
                        break;
                    }
                }
            }

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

        [HttpPost]
        public String ListUsuarios(int? idRol)
        {
            RolBL bl = new RolBL();
            Rol rol = new Rol();
            rol = bl.getRol(idRol.Value);
            rol.usuarios = bl.getUsuarios(rol.idRol);
            return JsonConvert.SerializeObject(rol.usuarios);

        }

        [HttpPost]
        public String ListUsuariosRoles(List<int> ListRol)
        {
            RolBL bl = new RolBL();
            List<Usuario> and = new List<Usuario>();
            for (var i = 0; i < ListRol.Count; i++)
            {
                and.AddRange(bl.getUsuarios(ListRol[i]));
            }
            //rol.usuarios = bl.getUsuarios(ListRol);
            return JsonConvert.SerializeObject(and);
        }


        public ActionResult VistaDashboard(int idRol)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (!usuario.modificaRol)
            {
                return RedirectToAction("List", "Rol");
            }
            VistaDashboardBL vistadashboardbl = new VistaDashboardBL();

            List<VistaDashboard> newvistasdashboard = new List<VistaDashboard>();
            VistaDashboard newobj = new VistaDashboard();

            newvistasdashboard = vistadashboardbl.getVistasDashboard(newobj);

            this.Session[Constantes.VAR_SESSION_VISTA_DASHBOARD_LISTA] = newvistasdashboard;

            Rol rolVistaDashboard = new Rol();
            RolBL bl = new RolBL();
            rolVistaDashboard = bl.getRol(idRol);

            rolVistaDashboard.VistasDashboard = vistadashboardbl.getVistasDashboardByRol(idRol);
            this.Session[Constantes.VAR_SESSION_VISTA_DASHBOARD_VER] = rolVistaDashboard;

            ViewBag.vistasDashboard = newvistasdashboard;
            ViewBag.rolVistaDashboard = rolVistaDashboard;


            return View();

        }

        public void ChangeVistaDashboard()
        {
            Rol obj = (Rol)this.Session[Constantes.VAR_SESSION_VISTA_DASHBOARD_VER];
            int valor = Int32.Parse(this.Request.Params["valor"]);
            int rol_select = Int32.Parse(this.Request.Params["rol"].ToString().Replace("vistaDashboard_", ""));
            List<VistaDashboard> listaRoles = (List<VistaDashboard>)this.Session[Constantes.VAR_SESSION_VISTA_DASHBOARD_LISTA];
            if (valor == 0)
            {
                //Remove
                foreach (VistaDashboard rol in obj.VistasDashboard)
                {
                    if (rol_select == rol.idVistaDashboard)
                    {
                        obj.VistasDashboard.Remove(rol);
                        break;
                    }
                }
            }
            else
            {
                //ADD
                foreach (VistaDashboard rol in listaRoles)
                {
                    if (rol_select == rol.idVistaDashboard)
                    {
                        obj.VistasDashboard.Add(rol);
                        break;
                    }
                }
            }
            this.Session[Constantes.VAR_SESSION_VISTA_DASHBOARD_VER] = obj;

        }

        public String updateRolVistaDashboard()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            Rol obj = (Rol)this.Session[Constantes.VAR_SESSION_VISTA_DASHBOARD_VER];
            Guid idUsuario = usuario.idUsuario;            
            VistaDashboardBL vdbl = new VistaDashboardBL();
            obj = vdbl.updateRolVistaDashboard(obj,idUsuario);
            return JsonConvert.SerializeObject(obj);
        }
    }
}
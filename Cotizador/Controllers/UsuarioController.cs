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
    public class UsuarioController : Controller
    {
        // GET: Usuario
        public ActionResult Index()
        {
            return View();
        }
        
        private Usuario UsuarioSession
        {
            get
            {
                Usuario obj = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaUsuarios: obj = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO_BUSQUEDA]; break;
                    case Constantes.paginas.MantenimientoUsuario: obj = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO_MANTENEDOR]; break;
                }
                return obj;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaUsuarios: this.Session[Constantes.VAR_SESSION_USUARIO_BUSQUEDA] = value; break;
                    case Constantes.paginas.MantenimientoUsuario: this.Session[Constantes.VAR_SESSION_USUARIO_MANTENEDOR] = value; break;
                }
            }
        }

        private void instanciarUsuarioBusqueda()
        {
            Usuario obj = new Usuario();
            obj.idUsuario = Guid.Empty;
            obj.Estado = 1;
            obj.email = String.Empty;
            obj.nombre = String.Empty;

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            obj.IdUsuarioRegistro = usuario.idUsuario;
            obj.usuario = usuario;

            this.Session[Constantes.VAR_SESSION_USUARIO_BUSQUEDA] = obj;
        }

        public String SearchUsuarios()
        {
            String data = this.Request.Params["data[q]"];
            UsuarioBL bl = new UsuarioBL();
            List<Usuario> lista = bl.searchUsuarios(data);
            
            String resultado = "{\"q\":\"" + data + "\",\"results\":[";
            Boolean existeItem = false;
            foreach (Usuario item in lista)
            {
                resultado += existeItem ? "," : "";
                resultado += "{\"id\":\"" + item.idUsuario + "\",\"text\":\"" + item.nombre + " (" + item.email + ")\"}";
                existeItem = true;
            }
            resultado = resultado.Substring(0, resultado.Length) + "]}";

            return resultado;
        }
        
        public ActionResult usuariosCotizacionList()
        {
            Usuario usuarioSession = ((Usuario)this.Session["usuario"]);
            List<Usuario> usuarioList = new List<Usuario>();
         /*   if (usuarioSession.apruebaCotizaciones)
            { */
                List<Usuario> usuarioListTmp = usuarioSession.usuarioCreaCotizacionList;
                
                Usuario usuarioTodos = new Usuario { nombre = "Todos", idUsuario = Guid.Empty };
                usuarioList.Add(usuarioTodos);
                usuarioList.Add(usuarioSession);
                foreach (Usuario usuario in usuarioListTmp)
                {
                    usuarioList.Add(usuario);
                }
         //   }
            var model = usuarioList;

            return PartialView("_SelectUsuarioCotizacion", model);
        }


        public ActionResult usuariosOCCList()
        {
            Usuario usuarioSession = ((Usuario)this.Session["usuario"]);
            List<Usuario> usuarioList = new List<Usuario>();
            //      if (usuarioSession.apruebaPedidos)
            //     {
            List<Usuario> usuarioListTmp = usuarioSession.usuarioTomaPedidoList;

            Usuario usuarioTodos = new Usuario { nombre = "Todos", idUsuario = Guid.Empty };
            usuarioList.Add(usuarioTodos);
            usuarioList.Add(usuarioSession);
            foreach (Usuario usuario in usuarioListTmp)
            {
                usuarioList.Add(usuario);
            }
            ///    }
            var model = usuarioList;

            return PartialView("_SelectUsuarioOCC", model);
        }

        public ActionResult usuariosPedidoList()
        {
            Usuario usuarioSession = ((Usuario)this.Session["usuario"]);
            List<Usuario> usuarioList = new List<Usuario>();
      //      if (usuarioSession.apruebaPedidos)
       //     {
                List<Usuario> usuarioListTmp = usuarioSession.usuarioTomaPedidoList;

                Usuario usuarioTodos = new Usuario { nombre = "Todos", idUsuario = Guid.Empty };
                usuarioList.Add(usuarioTodos);
                usuarioList.Add(usuarioSession);
                foreach (Usuario usuario in usuarioListTmp)
                {
                    usuarioList.Add(usuario);
                }
        ///    }
            var model = usuarioList;

            return PartialView("_SelectUsuarioPedido", model);
        }


        public ActionResult usuariosPedidoAlmacenList()
        {
            Usuario usuarioSession = ((Usuario)this.Session["usuario"]);
            List<Usuario> usuarioList = new List<Usuario>();
            //      if (usuarioSession.apruebaPedidos)
            //     {
            List<Usuario> usuarioListTmp = usuarioSession.usuarioTomaPedidoList;

            Usuario usuarioTodos = new Usuario { nombre = "Todos", idUsuario = Guid.Empty };
            usuarioList.Add(usuarioTodos);
            usuarioList.Add(usuarioSession);
            foreach (Usuario usuario in usuarioListTmp)
            {
                usuarioList.Add(usuario);
            }
            ///    }
            var model = usuarioList;

            return PartialView("_SelectUsuarioPedidoAlmacen", model);
        }


        public ActionResult usuariosPedidoCompraList()
        {
            Usuario usuarioSession = ((Usuario)this.Session["usuario"]);
            List<Usuario> usuarioList = new List<Usuario>();
            //      if (usuarioSession.apruebaPedidos)
            //     {
            List<Usuario> usuarioListTmp = usuarioSession.usuarioTomaPedidoList;

            Usuario usuarioTodos = new Usuario { nombre = "Todos", idUsuario = Guid.Empty };
            usuarioList.Add(usuarioTodos);
            usuarioList.Add(usuarioSession);
            foreach (Usuario usuario in usuarioListTmp)
            {
                usuarioList.Add(usuario);
            }
            ///    }
            var model = usuarioList;

            return PartialView("_SelectUsuarioPedidoCompra", model);
        }

        [HttpGet]
        public ActionResult List()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            if (!usuario.visualizaUsuarios && !usuario.modificaUsuario)
            {
                return RedirectToAction("Login", "Account");
            }

            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BusquedaUsuarios;

            if (this.Session[Constantes.VAR_SESSION_USUARIO_BUSQUEDA] == null)
            {
                instanciarUsuarioBusqueda();
            }


            Usuario objSearch = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO_BUSQUEDA];

            ViewBag.pagina = (int)Constantes.paginas.BusquedaUsuarios;
            ViewBag.usuario = objSearch;

            return View();
        }

        public String SearchList()
        {
            //Se indica la página con la que se va a trabajar
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaUsuarios;
            //Se recupera el objeto cliente que contiene los criterios de Búsqueda de la session
            Usuario obj = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO_BUSQUEDA];

            UsuarioBL bL = new UsuarioBL();
            List<Usuario> list = bL.getUsuariosMantenedor(obj);
            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_USUARIO_LISTA_MANTENEDOR] = list;
            //Se retorna la cantidad de elementos encontrados
            return JsonConvert.SerializeObject(list);

        }
        
        public ActionResult Permisos(Guid? idUsuario)
        {
            UsuarioBL bl = new UsuarioBL();
            Usuario usuarioEdit = new Usuario();

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (!usuario.modificaUsuario)
            {
                return RedirectToAction("List", "Usuario");
            }

            if (idUsuario == null || idUsuario == Guid.Empty)
            {
                return RedirectToAction("List", "Usuario");
            }

            usuarioEdit = bl.getUsuarioMantenedor(idUsuario.Value);
            usuarioEdit.usuario = usuario;
            usuarioEdit.IdUsuarioRegistro = usuario.idUsuario;

            PermisoBL permisobl = new PermisoBL();
            List<Permiso> permisos = new List<Permiso>();
            permisos = permisobl.getPermisos();

            ViewBag.usuario = usuarioEdit;
            ViewBag.permisos = permisos;
            
            return View();
        }
        
        public String UpdatePermisos()
        {
            UsuarioBL bl = new UsuarioBL();
            Usuario usuarioEdit = new Usuario();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            Guid idUsuario = Guid.Parse(this.Request.Params["idUsuario"].ToString());


            if (!usuario.modificaUsuario || idUsuario == null || idUsuario == Guid.Empty)
            {
                return JsonConvert.SerializeObject(new Usuario());
            }
            

            PermisoBL permisobl = new PermisoBL();
            List<Permiso> permisos = permisobl.getPermisos();
            List<Permiso> permisosUsuario = new List<Permiso>();

            foreach(Permiso item in permisos)
            {
                if (Request["permiso_" + item.idPermiso.ToString()] != null && Int32.Parse(Request["permiso_" + item.idPermiso.ToString()].ToString()) == 1)
                {
                    permisosUsuario.Add(item);
                }
            }
            

            usuarioEdit = bl.getUsuarioMantenedor(idUsuario);
            usuarioEdit.usuario = usuario;
            usuarioEdit.IdUsuarioRegistro = usuario.idUsuario;
            usuarioEdit.permisoList = permisosUsuario;

            bl.updatePermisos(usuarioEdit);


            return JsonConvert.SerializeObject(usuarioEdit);
        }


        

        public void ChangeInputString()
        {
            Usuario obj = (Usuario)this.UsuarioSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, this.Request.Params["valor"]);
            this.UsuarioSession = obj;
        }


        public String ConsultarSiExisteUsuario()
        {
            Guid idUsuario = Guid.Parse(Request["idUsuario"].ToString());
            UsuarioBL bL = new UsuarioBL();
            Usuario obj = bL.getUsuarioMantenimiento(idUsuario);

            //Usuario objuser = new Usuario();
            //objuser = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            
            this.Session[Constantes.VAR_SESSION_USUARIO_MANTENEDOR] = obj;

            //obj = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO_MANTENEDOR];
            if (obj == null)
                return "{\"existe\":\"false\",\"idUsuario\":\"00000000-0000-0000-0000-000000000000\"}";
            else
                return "{\"existe\":\"true\",\"idUsuario\":\"" + obj.idUsuario + "\"}";
        }

        public ActionResult Editar(Guid? idUsuario)
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.MantenimientoUsuario;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (!usuario.modificaUsuario)
            {
                return RedirectToAction("Login", "Account");
            }
            if ((Usuario)this.Session[Constantes.VAR_SESSION_USUARIO_MANTENEDOR] == null && idUsuario == null)
            {
                instanciarUsuario();
            }

            Usuario obj = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO_MANTENEDOR];

            if (idUsuario != null)
            {
                UsuarioBL bL = new UsuarioBL();
                obj = bL.getUsuarioMantenimiento(idUsuario);
                obj.idUsuarioModificacion = usuario.idUsuario;                
                this.Session[Constantes.VAR_SESSION_USUARIO_MANTENEDOR] = obj;
            }
            ViewBag.usuario = obj;
            return View();

        }

        private void instanciarUsuario()
        {
            Usuario obj = new Usuario();
            obj.sedeMP = new Ciudad();
            obj.idUsuario = Guid.Empty;            
            obj.cargo = String.Empty;
            obj.contacto = String.Empty;
            obj.maximoPorcentajeDescuentoAprobacion = 0.00M;
            obj.nombre = String.Empty;
            obj.Estado = 1;
            obj.email = String.Empty;
            obj.password = String.Empty;
            obj.esCliente = false;

            obj.sedeMP.idCiudad = Guid.Empty;

            Usuario user = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            obj.idUsuarioModificacion = user.idUsuarioModificacion;
            
            this.Session[Constantes.VAR_SESSION_USUARIO_MANTENEDOR] = obj;
        }

        public void iniciarEdicionUsuario()
        {            
            Usuario obj = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO_MANTENEDOR];
           
        }

        public ActionResult CancelarCreacionUsuario()
        {
            this.Session[Constantes.VAR_SESSION_USUARIO_MANTENEDOR] = null;
           
            return RedirectToAction("List", "Usuario");

        }

        [HttpPost]
        public String Create()
        {
            UsuarioBL bL = new UsuarioBL();
            Usuario obj = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO_MANTENEDOR];

            Usuario user = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            obj.idUsuarioModificacion = user.idUsuario;

            String pass = Request["password"].ToString();
            obj.password = pass;

            obj = bL.insertUsuarioMantenedor(obj);
            this.Session[Constantes.VAR_SESSION_USUARIO_MANTENEDOR] = null;
            String resultado = JsonConvert.SerializeObject(obj);
            return resultado;
        }
        public String Update()
        {
            UsuarioBL bL = new UsuarioBL();
            Usuario obj = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO_MANTENEDOR];

            Usuario user = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            obj.idUsuarioModificacion = user.idUsuario;

            String pass = Request["password"];
            obj.password = pass;

            if (obj.idUsuario == Guid.Empty)
            {
                obj = bL.insertUsuarioMantenedor(obj);
                this.Session[Constantes.VAR_SESSION_USUARIO_MANTENEDOR] = null;
            }
            else
            {
                obj = bL.updateUsuarioMantenedor(obj);
                this.Session[Constantes.VAR_SESSION_USUARIO_MANTENEDOR] = null;
            }
            String resultado = JsonConvert.SerializeObject(obj);

            return resultado;
        }

        public void ChangeInputStringMantenedor()
        {
            Usuario obj = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO_MANTENEDOR];
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, this.Request.Params["valor"]);
            this.Session[Constantes.VAR_SESSION_USUARIO_MANTENEDOR] = obj;
        }

        public String ChangeIdCiudad()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO_MANTENEDOR];

            Guid idCiudad = Guid.Empty;
            if (this.Request.Params["idCiudad"] != null && !this.Request.Params["idCiudad"].Equals(""))
            {
                idCiudad = Guid.Parse(this.Request.Params["idCiudad"]);
            }

            //CiudadBL ciudadBL = new CiudadBL();
            //Ciudad ciudadNueva = ciudadBL.getCiudad(idCiudad);
            usuario.sedeMP.idCiudad = idCiudad;
            this.Session[Constantes.VAR_SESSION_USUARIO_MANTENEDOR] = usuario;
            return "{\"idCiudad\": \"" + idCiudad + "\"}";

        }


        public void ChangeInputBooleanMantenedor()
        {

            Usuario obj = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO_MANTENEDOR];
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Boolean.Parse(this.Request.Params["valor"]));

            this.Session[Constantes.VAR_SESSION_USUARIO_MANTENEDOR] = obj;
        }

        public void ChangeInputIntMantenedor()
        {

            Usuario obj = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO_MANTENEDOR];
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Int32.Parse(this.Request.Params["valor"]));
            this.Session[Constantes.VAR_SESSION_USUARIO_MANTENEDOR] = obj;
        }
        public void ChangeInputInt()
        {
            Usuario obj = (Usuario)this.UsuarioSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Int32.Parse(this.Request.Params["valor"]));
            this.UsuarioSession = obj;
        }

        //[HttpPost]
        public void cambiarPassword(/*Guid idUsuarioCambioPass, string passActual, string passNuevo*/)
        {
            Usuario user = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            Guid idUsuarioCambioPass = user.idUsuario;            
            String passNuevo = Request["passNuevo"].ToString();
            UsuarioBL userBl = new UsuarioBL();
            userBl.updateUsuarioCambioPassword(passNuevo, idUsuarioCambioPass); 
        }

        public int confirmarPasswordActual(/*Guid idUsuarioCambioPass, string passActual, string passNuevo*/)
        {
            Usuario user = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            Guid idUsuarioCambioPass = user.idUsuario;
            String passActual = Request["passActual"].ToString();          

            UsuarioBL userBl = new UsuarioBL();
            bool a = userBl.confirmarPassword(passActual, idUsuarioCambioPass);
            if (a == true)
            {                
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
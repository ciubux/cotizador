using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLayer;
using Newtonsoft.Json;

namespace Cotizador.Controllers
{
    public class UsuarioController : Controller
    {
        // GET: Usuario
        public ActionResult Index()
        {
            return View();
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
        
        public ActionResult Permisos(int? idRol)
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
                return RedirectToAction("Index", "Rol");
            }

            rol = bl.getRol(idRol.Value);
            rol.usuario = usuario;
            rol.IdUsuarioRegistro = usuario.idUsuario;
            rol.usuarios = bl.getUsuarios(rol.idRol);


            ViewBag.rol = rol;
            return View();
        }
    }
}
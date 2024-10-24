﻿using Model;
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
    }
}
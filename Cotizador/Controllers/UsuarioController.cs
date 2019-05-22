using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLayer;

namespace Cotizador.Controllers
{
    public class UsuarioController : Controller
    {
        // GET: Usuario
        public ActionResult Index()
        {
            return View();
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

       

    }
}
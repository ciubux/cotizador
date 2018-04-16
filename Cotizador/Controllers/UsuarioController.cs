using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cotizador.Controllers
{
    public class UsuarioController : Controller
    {
        // GET: Usuario
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult list()
        {
            Usuario usuarioSession = ((Usuario)this.Session["usuario"]);
            List<Usuario> usuarioList = new List<Usuario>();
            if (usuarioSession.apruebaCotizaciones)
            { 
                List<Usuario> usuarioListTmp = usuarioSession.usuarioCreaCotizacionList;
                
                Usuario usuarioTodos = new Usuario { nombre = "Todos", idUsuario = Guid.Empty };
                usuarioList.Add(usuarioTodos);
                usuarioList.Add(usuarioSession);
                foreach (Usuario usuario in usuarioListTmp)
                {
                    usuarioList.Add(usuario);
                }
            }
            var model = usuarioList;

            return PartialView("_SelectUsuario", model);
        }
    }
}
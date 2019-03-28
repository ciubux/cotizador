using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;

using System.Web.Routing;
using NLog;

namespace Cotizador.Controllers
{
    public class ParentController : Controller
    {
        public Usuario Logueado;
        protected Logger logger;
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            var isLogin = true;

            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
            {
                string fullUrl = Request.Url.AbsoluteUri;
                this.Session["Prev_Request_Url"] = fullUrl;

                RedirectToAction("Login", "Account");
            }
            else
            {
                
                Logueado = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                //Obtener controller y action para validar permiso
            }
            logger = NLog.LogManager.GetCurrentClassLogger();

            ViewBag.isLogin = isLogin;
            ViewBag.Logueado = Logueado;
        }

        protected String agregarUsuarioAlMensaje(String texto)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            return usuario.email + " / " + texto;
        }



    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;

using System.Web.Routing;


namespace Cotizador.Controllers
{
    public class ParentController : Controller
    {
        public Usuario Logueado;

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
            

            ViewBag.isLogin = isLogin;
            ViewBag.Logueado = Logueado;
        }
    }
}
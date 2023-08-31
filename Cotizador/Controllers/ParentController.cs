using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;

using System.Web.Routing;
using NLog;
using BusinessLayer;

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

        protected bool tieneAcceso()
        {
            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
            {
                return false;
            }
            else
            {
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                if (!usuario.creaDocumentosCompra && !usuario.visualizaDocumentosCompra)
                {
                    return false;
                }
            }
            return true;
        }

        protected List<EscalaComision> GetEscalasComision()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            List<EscalaComision> lista = new List<EscalaComision>();
            if (this.Session[Constantes.VAR_ESCALA_COMISION_VALIDAS] == null)
            {
                EscalaComisionBL bl = new EscalaComisionBL(); 
                
                lista = bl.getEscalasComisionValidas(usuario.idUsuario);
            } else
            {
                lista = (List<EscalaComision>)this.Session[Constantes.VAR_ESCALA_COMISION_VALIDAS];
            }

            return lista;
        }
    }
}
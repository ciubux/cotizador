using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Model;
using BusinessLayer;
using System.Web.Routing;

namespace Cotizador.ActionFilters
{
    public class SesionTokenFilter : ActionFilterAttribute, IExceptionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            DateTime lastSesionValidate = DateTime.Now;
            if (filterContext.HttpContext.Session["s_lastSesionTokenValidate"] != null)
            {
                lastSesionValidate = (DateTime) filterContext.HttpContext.Session["s_lastSesionTokenValidate"];
            } else
            {
                filterContext.HttpContext.Session["s_lastSesionTokenValidate"] = DateTime.Now.AddSeconds(-9);
            }

            if ((DateTime.Now - lastSesionValidate).TotalSeconds > 10)
            {
                Usuario us = (Usuario)filterContext.HttpContext.Session[Constantes.VAR_SESSION_USUARIO];
                if (us != null)
                {
                    UsuarioBL bl = new UsuarioBL();

                    filterContext.HttpContext.Session["s_lastSesionTokenValidate"] = DateTime.Now;

                    if (!bl.validarSesionToken(us))
                    {
                        filterContext.HttpContext.Session.Contents.RemoveAll();

                        filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary(new { controller = "Account", action = "ErrorSesion" })
                        );
                        filterContext.Result.ExecuteResult(filterContext.Controller.ControllerContext);
                    }
                }
            }
        }

        public void OnException(ExceptionContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary(new { controller = "Account", action = "ErrorSesion" })
            );
            filterContext.Result.ExecuteResult(filterContext.Controller.ControllerContext);
            //throw new NotImplementedException();
        }
    }
}
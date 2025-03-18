using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLayer;
using Cotizador.Models;
using Model;

namespace Cotizador.Controllers
{
    public class EmpresaController : Controller
    {
        // GET: Empresa
        public ActionResult Index()
        {
            return View();
        }

        // GET: Empresa/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult EditarParametros()
        {
            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (!usuario.editaConfiguracionEmpresa)
            {
                return RedirectToAction("Login", "Account");
            }


            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.EditaConfiguracionEmpresa;
            ViewBag.pagina = (int)Constantes.paginas.EditaConfiguracionEmpresa;

            EmpresaBL bl = new EmpresaBL();

            Empresa obj = bl.GetEmpresa(usuario.idEmpresa, usuario.idUsuario);
            ViewBag.parametrosEditados = this.Session["s_ParametrosEmpresaEditados"] != null ? (bool)this.Session["s_ParametrosEmpresaEditados"] : false;

            if (ViewBag.parametrosEditados)
            {
                this.Session["s_ParametrosEmpresaEditados"] = false;
            }

            ViewBag.empresa = obj;

            return View();
        }

        [HttpPost]
        public ActionResult GuardarParametros(string empresaSelectId, string selectedValue = null)
        {
            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (!usuario.editaConfiguracionEmpresa)
            {
                return RedirectToAction("Login", "Account");
            }

            EmpresaBL bl = new EmpresaBL();
            Empresa obj = bl.GetEmpresa(usuario.idEmpresa, usuario.idUsuario);

            obj.usuario = usuario;
            obj.factorCosto = Decimal.Parse(this.Request.Params["empresa.factorCosto"]);
            obj.porcentajeDescuentoInframargen = Decimal.Parse(this.Request.Params["empresa.porcentajeDescuentoInframargen"]);
            obj.porcentajeMargenMinimo = Decimal.Parse(this.Request.Params["empresa.porcentajeMargenMinimo"]);
            obj.porcentajeMDGanaciaMax = Decimal.Parse(this.Request.Params["empresa.porcentajeMDGanaciaMax"]);

            bl.ActualizarParametrosEmpresa(obj);


            this.Session["s_ParametrosEmpresaEditados"] = true;
            return RedirectToAction("EditarParametros", "Empresa");
        }

        public ActionResult GetEmpresasVisualizacion(string empresaSelectId, string selectedValue = null)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            UsuarioBL bl = new UsuarioBL();


            List<Empresa> empresas = new List<Empresa>();

            if (this.Session[Constantes.VAR_SESSION_EMPRESA_LISTA] == null)
            {
                if (usuario != null)
                {
                    empresas = bl.GetEmpresas(usuario.idUsuario, 1);
                    this.Session[Constantes.VAR_SESSION_EMPRESA_LISTA] = empresas;
                }
            } else
            {
                empresas = (List<Empresa>)this.Session[Constantes.VAR_SESSION_EMPRESA_LISTA];
            }

            if (usuario != null)
            {
                selectedValue = usuario.idEmpresa.ToString();
            }

            var model = new EmpresaViewModels
            {
                Data = empresas,
                EmpresaSelectId = empresaSelectId,
                incluirSeleccione = true,
                SelectedValue = selectedValue
            };

            return PartialView("_Empresa", model);
        }
    }
}

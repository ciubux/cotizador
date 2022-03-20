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

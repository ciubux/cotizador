using Cotizador.Models;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cotizador.Controllers
{
    public class VendedorController : Controller
    {
        // GET: Vendedor
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetResponsablesComerciales(string vendedorSelectId, string selectedValue = null, string disabled = null)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            var model = new VendedorViewModels
            {
                Data = usuario.responsableComercialList,
                VendedorSelectId = vendedorSelectId,
                SelectedValue = selectedValue,
                Disabled = disabled == null || disabled != "disabled" ? false : true
            };

            return PartialView("_Vendedor", model);
        }

        public ActionResult GetSupervisoresComerciales(string vendedorSelectId, string selectedValue = null, string disabled = null)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            var model = new VendedorViewModels
            {
                Data = usuario.supervisorComercialList,
                VendedorSelectId = vendedorSelectId,
                SelectedValue = selectedValue,
                Disabled = disabled == null || disabled != "disabled" ? false : true
            };

            return PartialView("_Vendedor", model);
        }

        public ActionResult GetAsistentesServicioCliente(string vendedorSelectId, string selectedValue = null, string disabled = null)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            var model = new VendedorViewModels
            {
                Data = usuario.asistenteServicioClienteList,
                VendedorSelectId = vendedorSelectId,
                SelectedValue = selectedValue,
                Disabled = disabled == null || disabled != "disabled" ? false : true
            };

            return PartialView("_Vendedor", model);
        }


    }
}
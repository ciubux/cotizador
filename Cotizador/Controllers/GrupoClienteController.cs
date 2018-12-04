using BusinessLayer;
using Cotizador.Models;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cotizador.Controllers
{
    public class GrupoClienteController : Controller
    {
        // GET: GrupoCliente
        public ActionResult Index()
        {
            return View();
        }



        public ActionResult GetGruposCliente(string grupoClienteSelectId, string selectedValue = null)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            GrupoClienteBL grupoClienteBL = new GrupoClienteBL();
            List<GrupoCliente> grupoClienteList = grupoClienteBL.getGruposCliente();


            var model = new GrupoClienteViewModels
            {
                Data = grupoClienteList,
                GrupoClienteSelectId = grupoClienteSelectId,
                SelectedValue = selectedValue
            };

            return PartialView("_GrupoCliente", model);
        }

    }
}
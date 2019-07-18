using BusinessLayer;
using Cotizador.Models;
using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Cotizador.Controllers
{
    public class DomicilioLegalController : Controller
    {
        public ActionResult GetDomiciliosLegales(string domicilioLegalSelectId, string selectedValue = null)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            Cliente cliente = (Cliente)this.Session[Constantes.VAR_SESSION_CLIENTE_VER];

            DomicilioLegalBL domicilioLegalBL = new DomicilioLegalBL();
            List<DomicilioLegal> domicilioLegalList = domicilioLegalBL.getDomiciliosLegalesPorCliente(cliente);

            var model = new DomicilioLegalViewModels
            {
                Data = domicilioLegalList,
                DomicilioLegalSelectId = domicilioLegalSelectId,
                SelectedValue = selectedValue
            };

            return PartialView("_DomicilioLegal", model);
        }

        
    }
}
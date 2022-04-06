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
    public class MotivoAjusteAlmacenController : Controller
    {

        MotivoAjusteAlmacenBL bl = new MotivoAjusteAlmacenBL();

        public ActionResult GetMotivos(string motivoAjusteAlmacenSelectId, string selectedValue = null)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            MotivoAjusteAlmacen mov = new MotivoAjusteAlmacen();
            mov.Estado = 1;
            List<MotivoAjusteAlmacen> motivos = bl.getMotivos(mov);

            bool verSeleccione = true;

            var model = new MotivoAjusteAlmacenViewModels
            {
                Data = motivos,
                MotivoAjusteAlmacenSelectId = motivoAjusteAlmacenSelectId,
                incluirSeleccione = verSeleccione, 
                SelectedValue = selectedValue
            };

            return PartialView("_MotivoAjusteAlmacen", model);
        }

    }
}

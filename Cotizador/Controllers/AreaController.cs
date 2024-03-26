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
    public class AreaController : Controller
    {
        public ActionResult GetAreasVisualizacion(string selectId, string selectedValue = null)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            AreaBL bl = new AreaBL();


            List<Area> lista = new List<Area>();

            if (this.Session[Constantes.VAR_SESSION_AREA_LISTA] == null)
            {
                if (usuario != null)
                {
                    lista = bl.getAreas(usuario.idUsuario, 1);
                    this.Session[Constantes.VAR_SESSION_AREA_LISTA] = lista;
                }
            } else
            {
                lista = (List<Area>)this.Session[Constantes.VAR_SESSION_AREA_LISTA];
            }


            var model = new AreaViewModels
            {
                Data = lista,
                AreaSelectId = selectId,
                incluirSeleccione = true,
                SelectedValue = selectedValue
            };

            return PartialView("_Area", model);
        }
    }
}

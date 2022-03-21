using BusinessLayer;
using Cotizador.Models;
using Model;
using Model.UTILES;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Cotizador.ExcelExport;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace Cotizador.Controllers
{
    public class KpiController : ParentController
    {
        [HttpGet]
        public ActionResult ResultadosKpi()
        {

            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.ResultadosKPI;

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            List<KpiPeriodo> periodos = new List<KpiPeriodo>();
            KpiBL bl = new KpiBL();
            periodos = bl.getPeriodos(usuario);

            ViewBag.periodos = periodos;
            return View();
        }


    }
}
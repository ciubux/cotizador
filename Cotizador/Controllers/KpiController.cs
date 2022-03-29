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
            List<Kpi> kpis = new List<Kpi>();
            List<Usuario> usuarios= new List<Usuario>();

            Guid idKpiPeriodo = Guid.Empty;
            Guid idKpi = Guid.Empty;

            KpiBL bl = new KpiBL();
            periodos = bl.getPeriodos(usuario);

            if (this.Session["s_resultadoskpi_id_kpi_periodo"] != null)
            {
                idKpiPeriodo = (Guid)this.Session["s_resultadoskpi_id_kpi_periodo"];
                kpis = bl.getPeriodoKPIs(usuario, idKpiPeriodo);
            }

            if (this.Session["s_resultadoskpi_id_kpi_periodo"] != null)
            {
                idKpi = (Guid)this.Session["s_resultadoskpi_id_kpi"];
                usuarios = bl.getKPIPeriodoUsuarios(usuario, idKpiPeriodo, idKpi);
            }

            ViewBag.idKpiPeriodo = idKpiPeriodo;
            ViewBag.idKpi = idKpi;

            ViewBag.periodos = periodos;
            ViewBag.kpis = kpis;
            ViewBag.usuarios = usuarios;

            return View();
        }

        [HttpPost]
        public String GetPeriodoKPIs(Guid idKpiPeriodo)
        {

            this.Session["s_resultadoskpi_id_kpi_periodo"] = idKpiPeriodo;

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            List<Kpi> kpis = new List<Kpi>();
            KpiBL bl = new KpiBL();
            kpis = bl.getPeriodoKPIs(usuario, idKpiPeriodo);

            String resultado = JsonConvert.SerializeObject(kpis);
            return resultado;
        }

        [HttpPost]
        public String GetPeriodoKpiUsuarios(Guid idKpi)
        {
            Guid idKpiPeriodo = (Guid) this.Session["s_resultadoskpi_id_kpi_periodo"];
            this.Session["s_resultadoskpi_id_kpi"] = idKpi;

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            List<Usuario> usuarios = new List<Usuario>();
            KpiBL bl = new KpiBL();
            usuarios = bl.getKPIPeriodoUsuarios(usuario, idKpiPeriodo, idKpi);

            String resultado = JsonConvert.SerializeObject(usuarios);
            return resultado;
        }

        [HttpPost]
        public String GetResultadosKPIUsuarios(Guid[] idsUsuario)
        {
            Guid idKpiPeriodo = (Guid)this.Session["s_resultadoskpi_id_kpi_periodo"];
            Guid idKpi = (Guid)this.Session["s_resultadoskpi_id_kpi"];

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            List<KpiMeta> metas = new List<KpiMeta>();
            KpiBL bl = new KpiBL();
            metas = bl.getKPIResultados(idsUsuario, idKpiPeriodo, idKpi);

            String resultado = JsonConvert.SerializeObject(metas);
            return resultado;
        }
    }
}
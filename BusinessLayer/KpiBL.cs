
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;

namespace BusinessLayer
{
    public class KpiBL
    {
        public List<KpiMeta> getKPIResultados(Guid[] idUsuario, Guid idKpiPeriodo, Guid idKpi)
        {
            using (var dal = new KpiDAL())
            {
                List<KpiMeta> list = dal.getKPIResultados(idUsuario, idKpiPeriodo, idKpi);
                
                return list;
            }
        }

        public List<KpiPeriodo> getPeriodos(Usuario usuario)
        {
            using (var dal = new KpiDAL())
            {
                List<KpiPeriodo> list = dal.getPeriodos(usuario);

                return list;
            }
        }

        public List<Kpi> getPeriodoKPIs(Usuario usuario, Guid idKpiPeriodo)
        {
            using (var dal = new KpiDAL())
            {
                List<Kpi> list = dal.getPeriodoKPIs(usuario, idKpiPeriodo);

                return list;
            }
        }

        public List<Usuario> getKPIPeriodoUsuarios(Usuario usuario, Guid idKpiPeriodo, Guid idKpi)
        {
            using (var dal = new KpiDAL())
            {
                List<Usuario> list = dal.getKPIPeriodoUsuarios(usuario, idKpiPeriodo, idKpi);

                return list;
            }
        }
    }
}

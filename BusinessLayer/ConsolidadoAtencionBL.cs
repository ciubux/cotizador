
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;

namespace BusinessLayer
{
    public class ConsolidadoAtencionBL
    {
        public ConsolidadoAtencion getConsolidadoAtencion(Guid idConsolidado)
        {
            using (ConsolidadoAtencionDAL dal = new ConsolidadoAtencionDAL())
            {
                return dal.getConsolidadoAtencion(idConsolidado);
            }
        }

        public List<ConsolidadoAtencion> getConsolidadosAtencion(ConsolidadoAtencion obj)
        {
            using (ConsolidadoAtencionDAL dal = new ConsolidadoAtencionDAL())
            {
                return dal.getConsolidadosAtencion(obj);
            }
        }

        public ConsolidadoAtencion insertConsolidadoAtencion(ConsolidadoAtencion obj)
        {
            using (ConsolidadoAtencionDAL dal = new ConsolidadoAtencionDAL())
            {
                return dal.insertConsolidadoAtencion(obj);
            }
        }

        public ConsolidadoAtencion updateConsolidadoAtencion(ConsolidadoAtencion obj)
        {
            using (ConsolidadoAtencionDAL dal = new ConsolidadoAtencionDAL())
            {
                return dal.updateConsolidadoAtencion(obj);
            }
        }
    }
}


using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;

namespace BusinessLayer
{
    public class PeriodoSolicitudBL
    {
        public PeriodoSolicitud getPeriodoSolicitud(Guid id, Guid idUsuario)
        {
            using (var dal = new PeriodoSolicitudDAL())
            {
                PeriodoSolicitud obj = dal.getPeriodoSolicitud(id, idUsuario);
                
                return obj;
            }
        }

        public List<PeriodoSolicitud> getPeriodosSolicitud(PeriodoSolicitud obj)
        {
            using (var dal = new PeriodoSolicitudDAL())
            {
                return dal.getPeriodosSolicitud(obj);
            }
        }

        public PeriodoSolicitud insertPeriodoSolicitud(PeriodoSolicitud obj)
        {
            using (var dal = new PeriodoSolicitudDAL())
            {
                return dal.insertPeriodoSolicitud(obj);
            }
        }

        public PeriodoSolicitud updatePeriodoSolicitud(PeriodoSolicitud obj)
        {
            using (var dal = new PeriodoSolicitudDAL())
            {
                return dal.updatePeriodoSolicitud(obj);
            }
        }
    }
}

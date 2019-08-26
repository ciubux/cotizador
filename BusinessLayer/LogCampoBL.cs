using DataLayer;
using Model;
using Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class LogCampoBL
    {

        public List<LogCampo> getCampoLogPorTabla(String nombreTabla)
        {
            using (var dal = new LogCampoDAL())
            {
                return dal.getCampoLogPorTabla(nombreTabla);
            }
        }

        public List<LogCampo> getTablasList()
        {
            using (var dal = new LogCampoDAL())
            {
                return dal.getTablaList();
            }
        }

        public List<LogCampo> getCatalogo(LogCampo logCampo)
        {
            using (var dal = new LogCampoDAL())
            {
                return dal.getCatalogo(logCampo);
            }
        }


        public LogCampo updateCatalogo(LogCampo idCatalogo)
        {
            using (var dal = new LogCampoDAL())
            {
                return dal.updateLogCampo(idCatalogo);
            }
        }

        public LogCampo insertLogCampo(LogCampo idCatalogo)
        {
            using (var dal = new LogCampoDAL())
            {
                return dal.insertLogCampo(idCatalogo);
            }
        }
    }
}

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

        public List<LogCampo> getCatalogoById(LogCampo Catalogo)
        {
            using (var dal = new LogCampoDAL())
            {
                return dal.getCatalogoById(Catalogo);
            }
        }


        public LogCampo updateCatalogo(LogCampo idCatalogo)
        {
            using (var dal = new LogCampoDAL())
            {
                return dal.updateCatalogo(idCatalogo);
            }
        }
    }
}

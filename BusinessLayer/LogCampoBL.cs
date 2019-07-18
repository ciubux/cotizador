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
    }
}

using DataLayer;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class TipoVistaDashboardBL
    {
        public List<TipoVistaDashboard> getTipoVistaDashboard()
        {
            using (var tipoVistaDashboardDAL = new TipoVistaDashboardDAL())
            {
                return tipoVistaDashboardDAL.getTipoVistaDashboard();
            }
        }


    }
}

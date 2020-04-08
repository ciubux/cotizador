using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using Model;

namespace BusinessLayer
{
    public class VistaDashboardBL
    {
        public List<VistaDashboard> getVistasDashboard(VistaDashboard obj)
        {
            using (var dal = new VistaDashboardDAL())
            {
                return dal.getVistasDashboard(obj);
            }
        }
       
             public VistaDashboard getVistaDashboardById(int obj)
        {
            using (var dal = new VistaDashboardDAL())
            {
                return dal.getVistaDashboardById(obj);
            }
        }

        public VistaDashboard updateVistaDashboard(VistaDashboard obj,Guid idUsuario)
        {
            using (var dal = new VistaDashboardDAL())
            {
                return dal.updateVistaDashboard(obj, idUsuario);
            }
        }

        public VistaDashboard insertVistaDashboard(VistaDashboard obj, Guid idUsuario)
        {
            using (var dal = new VistaDashboardDAL())
            {
                return dal.insertVistaDashboard(obj, idUsuario);
            }
        }

        public List<VistaDashboard> getVistasDashboardByRol(int idRol)
        {
            using (var dal = new VistaDashboardDAL())
            {
                return dal.getVistasDashboardByRol(idRol);
            }
        }

        public Rol updateRolVistaDashboard(Rol obj,Guid idUsuario)
        {
            using (var dal = new VistaDashboardDAL())
            {
                return dal.updateRolVistaDashboard(obj, idUsuario);
            }
        }
    }
}

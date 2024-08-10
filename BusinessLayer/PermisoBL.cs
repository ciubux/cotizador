
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using Model.UTILES;

namespace BusinessLayer
{
    public class PermisoBL
    {
        public Permiso updatePermiso(Permiso obj)
        {
            using (var dal = new PermisoDAL())
            {
                return dal.updatePermiso(obj);
            }
        }

        public List<Permiso> getPermisos()
        {
            using (var dal = new PermisoDAL())
            {
                return dal.selectPermisos();
            }
        }

        public Permiso getPermiso(int idPermiso)
        {
            using (var dal = new PermisoDAL())
            {
                return dal.getPermiso(idPermiso);
            }
        }

        public ReporteMatriz ListaPermisosUsuarios(Guid idUsuario)
        {
            using (var dal = new PermisoDAL())
            {
                return dal.ListaPermisosUsuarios(idUsuario);
            }
        }

        public ReporteMatriz ListaPermisosRoles(Guid idUsuario)
        {
            using (var dal = new PermisoDAL())
            {
                return dal.ListaPermisosRoles(idUsuario);
            }
        }
    }
}

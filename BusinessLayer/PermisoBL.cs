
using DataLayer;
using System.Collections.Generic;
using System;
using Model;

namespace BusinessLayer
{
    public class PermisoBL
    {
        public List<Permiso> getPermisos()
        {
            using (var dal = new PermisoDAL())
            {
                return dal.selectPermisos();
            }
        }
        
    }
}

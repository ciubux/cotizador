
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;

namespace BusinessLayer
{
    public class RolBL
    {
        public Rol getRol(int idRol)
        {
            using (var dal = new RolDAL())
            {
                Rol obj = dal.getRol(idRol);
                
                return obj;
            }
        }

        public List<Rol> getRoles(Rol obj)
        {
            using (var dal = new RolDAL())
            {
                return dal.getRoles(obj);
            }
        }

        public Rol insertRol(Rol obj)
        {
            using (var dal = new RolDAL())
            {
                return dal.insertRol(obj);
            }
        }

        public Rol updateRol(Rol obj)
        {
            using (var dal = new RolDAL())
            {
                return dal.updateRol(obj);
            }
        }
    }
}

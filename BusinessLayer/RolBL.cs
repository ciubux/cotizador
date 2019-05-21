
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

        public List<Usuario> getUsuarios(int idRol)
        {
            using (var dal = new RolDAL())
            {
                return dal.getUsuarios(idRol);
            }
        }

        public void agregarUsuarioRol(int idRol, Guid idUsuario, Guid idUsuarioModifica)
        {
            using (var dal = new RolDAL())
            {
                dal.agregarUsuarioRol(idRol, idUsuario, idUsuarioModifica);
            }
        }

        public void quitarUsuarioRol(int idRol, Guid idUsuario)
        {
            using (var dal = new RolDAL())
            {
                dal.quitarUsuarioRol(idRol, idUsuario);
            }
        }
    }
}

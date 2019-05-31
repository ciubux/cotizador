using DataLayer;
using System.Collections.Generic;
using System;
using Model;

namespace BusinessLayer
{
    public class UsuarioBL
    {
        public Usuario getUsuarioLogin(String email, String password)
        {
            Usuario usuario = null;
            using (var usuarioDAL = new UsuarioDAL())
            {
                usuario = new Usuario();
                usuario.email = email;
                usuario.password = password;
                usuario = usuarioDAL.getUsuarioLogin(usuario);


                //usuario.idUsuario != null && usuario.idUsuario != Guid.Empty;
            }
            return usuario;
        }

        public Usuario getUsuario(Guid idUsuario)
        {
            Usuario usuario = null;
            using (var usuarioDAL = new UsuarioDAL())
            {
                usuario = usuarioDAL.getUsuario(idUsuario);
            }
            return usuario;
        }

        public void updateCotizacionSerializada(Usuario usuario, String cotizacionSerializada)
        {
            using (var usuarioDAL = new UsuarioDAL())
            {
                usuarioDAL.updateCotizacionSerializada(usuario, cotizacionSerializada);
            }
        }

        public void updatePedidoSerializado(Usuario usuario, String pedidoSerializado)
        {
            using (var usuarioDAL = new UsuarioDAL())
            {
                usuarioDAL.updatePedidoSerializado(usuario, pedidoSerializado);
            }
        }

        public List<Usuario> getUsuarios()
        {
            using (var usuarioDAL = new UsuarioDAL())
            {
                return usuarioDAL.selectUsuarios();
            }
        }

        public List<Usuario> getUsuariosPorPermiso(Permiso permiso)
        {
            using (var usuarioDAL = new UsuarioDAL())
            {
                return usuarioDAL.selectUsuariosPorPermiso(permiso);
            }
        }

        public void setPermiso(List<Usuario> usuarioList, Permiso permiso, Usuario usuario)
        {
            using (var usuarioDAL = new UsuarioDAL())
            {
                usuarioDAL.updatePermiso(usuarioList, permiso, usuario);
            }
        }

        public List<Usuario> searchUsuarios(String textoBusqueda)
        {
            using (var dal = new UsuarioDAL())
            {
                return dal.searchUsuarios(textoBusqueda);
            }
        }

        public List<Usuario> getUsuariosMantenedor(Usuario usuario)
        {
            using (var usuarioDAL = new UsuarioDAL())
            {
                return usuarioDAL.getUsuariosMantenedor(usuario);
            }
        }

        public Usuario getUsuarioMantenedor(Guid idUsuario)
        {
            using (var usuarioDAL = new UsuarioDAL())
            {
                return usuarioDAL.getUsuarioMantenedor(idUsuario);
            }
        }

        public Usuario updatePermisos(Usuario usuario)
        {
            using (var dal = new UsuarioDAL())
            {
                return dal.updatePermisos(usuario);
            }
        }
    }
}

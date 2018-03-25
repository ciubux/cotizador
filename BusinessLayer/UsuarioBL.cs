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

        public void updateCotizacionSerializada(Usuario usuario)
        {
            using (var usuarioDAL = new UsuarioDAL())
            {
                 usuarioDAL.updateCotizacionSerializada(usuario);
            }
        }



    }
}

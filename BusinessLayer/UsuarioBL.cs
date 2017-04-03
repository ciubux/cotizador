
using DataLayer;
using System.Collections.Generic;
using System;
using Model;

namespace BusinessLayer
{
    public class UsuarioBL
    {
        public Boolean getUsuarioLogin(String email, String password)
        {
            using (var usuarioDAL = new UsuarioDAL())
            {
                Usuario usuario = new Usuario();
                usuario.email = email;
                usuario.password = password;
                usuario = usuarioDAL.getUsuarioLogin(usuario);
                return usuario.idUsuario != null && usuario.idUsuario != Guid.Empty;
            }
        }

        
    }
}

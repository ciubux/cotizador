<<<<<<< HEAD
﻿
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
=======
﻿
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
>>>>>>> 9b44f5b830423b8d9b8b183ec6d79827b7bb7846

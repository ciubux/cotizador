using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;

namespace BusinessLayer
{
    public class UsuarioBL
    {
        public Usuario getUsuarioLogin(String email, String password, String IPAddress)
        {
            Usuario usuario = null;
            using (var usuarioDAL = new UsuarioDAL())
            {
                usuario = new Usuario();
                usuario.email = email;
                usuario.password = password;
                usuario.ipAddress = IPAddress;
                usuario = usuarioDAL.getUsuarioLogin(usuario);

                if (usuario.firmaImagen == null)
                {
                    FileStream inStream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\images\\NoDisponible.gif", FileMode.Open);
                    MemoryStream storeStream = new MemoryStream();
                    storeStream.SetLength(inStream.Length);
                    inStream.Read(storeStream.GetBuffer(), 0, (int)inStream.Length);
                    storeStream.Flush();
                    inStream.Close();
                    usuario.firmaImagen = storeStream.GetBuffer();
                }

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

        /************************************************************************/

        public String searchUsuariosVenta(String textoBusqueda)
        {
            using (var dal = new UsuarioDAL())
            {
                List<Usuario> usuarioList = dal.searchUsuariosVendedor(textoBusqueda);
                String resultado = "{\"q\":\"" + textoBusqueda + "\",\"results\":[";
                Boolean existeCliente = false;
                foreach (Usuario usuario in usuarioList)
                {
                    resultado += "{\"id\":\"" + usuario.idUsuario + "\",\"text\":\"" +"Nombre: "+ usuario.nombre.ToString() +" - Email:"+ usuario.email.ToString() + "\"},";
                    existeCliente = true;
                }
                if (existeCliente)
                    resultado = resultado.Substring(0, resultado.Length - 1) + "]}";
                else
                    resultado = resultado.Substring(0, resultado.Length) + "]}";

                return resultado;
            }
        }

        public Usuario getUsuarioVendedor(Guid idUsuario)
        {
            Usuario usuario = null;
            using (var usuarioDAL = new UsuarioDAL())
            {
                usuario = usuarioDAL.getUsuarioVendedor(idUsuario);
            }
            return usuario;
        }


        public Usuario getUsuarioMantenimiento(Guid? idUsuario)
        {
            Usuario usuario = null;
            using (var usuarioDAL = new UsuarioDAL())
            {
                usuario = usuarioDAL.getUsuarioMantenimiento(idUsuario);
            }
            return usuario;
        }


        public Usuario insertUsuarioMantenedor(Usuario usuario)
        {
            using (var dal = new UsuarioDAL())
            {
                return dal.insertUsuarioMantenedor(usuario);
            }
        }

        public Usuario updateUsuarioMantenedor(Usuario usuario)
        {
            using (var dal = new UsuarioDAL())
            {
                return dal.updateUsuarioMantenedor(usuario);
            }
        }
        public bool confirmarPassword(string passActual,Guid idUsuario)
        {
            bool usuario;
            using (var usuarioDAL = new UsuarioDAL())
            {
               usuario = usuarioDAL.confirmarPassword(passActual, idUsuario);
            }
            return usuario;
        }

        public void updateUsuarioCambioPassword(string passNuevo, Guid idUsuario)
        {            
            using (var usuarioDAL = new UsuarioDAL())
            {
               usuarioDAL.updateUsuarioCambioPassword(passNuevo, idUsuario);
            }           
        }
        public void updateUsuarioCambiarImagenFirma(Byte[] imagen, Guid idUsuario)
        {
            using (var usuarioDAL = new UsuarioDAL())
            {
                usuarioDAL.updateUsuarioCambiarImagenFirma(imagen, idUsuario);
            }
        }
    }
}

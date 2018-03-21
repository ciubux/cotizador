using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

namespace DataLayer
{
    public class UsuarioDAL : DaoBase
    {
        public UsuarioDAL(IDalSettings settings) : base(settings)
        {
        }

        public UsuarioDAL() : this(new CotizadorSettings())
        {
        }

        public Usuario getUsuarioLogin(Usuario usuario)
        {
            var objCommand = GetSqlCommand("ps_getusuario_login");
            InputParameterAdd.Varchar(objCommand, "email", usuario.email);
            InputParameterAdd.Varchar(objCommand, "password", usuario.password);
            DataSet dataSet = ExecuteDataSet(objCommand);

            DataTable dataTableUsuario = dataSet.Tables[0];   

            foreach (DataRow row in dataTableUsuario.Rows)
            {
                usuario.idUsuario = Converter.GetGuid(row, "id_usuario");
                usuario.cargo = Converter.GetString(row, "cargo");
                usuario.nombre = Converter.GetString(row, "nombre");
                usuario.contacto = Converter.GetString(row, "contacto");
                usuario.esAprobador = Converter.GetBool(row, "es_aprobador");
                usuario.maximoPorcentajeDescuentoAprobacion = Converter.GetDecimal(row, "maximo_porcentaje_descuento_aprobacion");
            }

            /*Si es usuario aprobador se recupera la lista de usuarios a los cuales puede aprobar cotizaciones*/
            if (usuario.esAprobador)
            {
                DataTable dataTableUsuarios = dataSet.Tables[1];
                List<Usuario> usuarioList = new List<Usuario>();
              
                foreach (DataRow row in dataTableUsuarios.Rows)
                {
                    Usuario usuarioTmp = new Usuario();
                    usuarioTmp.idUsuario = Converter.GetGuid(row, "id_usuario");
                    usuarioTmp.nombre = Converter.GetString(row, "nombre");
                    usuarioList.Add(usuarioTmp);
                }
                usuario.usuarioList = usuarioList;
            }

            return usuario;
        }
    }
}

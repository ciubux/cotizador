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
            DataTable dataTable = Execute(objCommand);

            foreach (DataRow row in dataTable.Rows)
            {
                usuario.idUsuario = Converter.GetGuid(row, "id_usuario");
                usuario.nombres = Converter.GetString(row, "nombres");
                usuario.apellidos = Converter.GetString(row, "apellidos");
            }
            return usuario;
        }
    }
}

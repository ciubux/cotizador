using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Model;

namespace DataLayer
{
    public class RolDAL : DaoBase
    {
        public RolDAL(IDalSettings settings) : base(settings)
        {
        }
        public RolDAL() : this(new CotizadorSettings())
        {
        }
        

        public Rol getRol(int idRol)
        {
            var objCommand = GetSqlCommand("ps_rol");
            InputParameterAdd.Int(objCommand, "idRol", idRol);
            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable rol = dataSet.Tables[0];
            DataTable permisos = dataSet.Tables[1];

            Rol obj = new Rol();
           
            foreach (DataRow row in rol.Rows)
            {
                obj.idRol = Converter.GetInt(row, "id_rol");
                obj.codigo = Converter.GetString(row, "codigo");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.Estado = Converter.GetInt(row, "estado");
            }

            obj.permisos = new List<Permiso>();
            foreach (DataRow row in permisos.Rows)
            {
                Permiso permiso = new Permiso();
                permiso.idPermiso = Converter.GetInt(row, "id_permiso");
                permiso.codigo = Converter.GetString(row, "codigo");
                permiso.descripcion_corta = Converter.GetString(row, "descripcion_corta");
                permiso.descripcion_larga = Converter.GetString(row, "descripcion_larga");
                permiso.categoriaPermiso = new CategoriaPermiso();
                permiso.categoriaPermiso.idCategoriaPermiso = Converter.GetInt(row, "id_categoria_permiso");
                permiso.categoriaPermiso.descripcion = Converter.GetString(row, "descripcion_categoria");
                
                obj.permisos.Add(permiso);
            }

            return obj;
        }


        public List<Rol> getRoles(Rol rol)
        {
            var objCommand = GetSqlCommand("ps_roles");
            InputParameterAdd.Int(objCommand, "estado", rol.Estado);
            DataTable dataTable = Execute(objCommand);
            List<Rol> lista = new List<Rol>();

            foreach (DataRow row in dataTable.Rows)
            {
                Rol obj = new Rol();
                obj.idRol = Converter.GetInt(row, "id_rol");
                obj.codigo = Converter.GetString(row, "codigo");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.Estado = Converter.GetInt(row, "estado");
                lista.Add(obj);
            }

            return lista;
        }


        public Rol insertRol(Rol obj)
        {
            var objCommand = GetSqlCommand("pi_rol");

            InputParameterAdd.Guid(objCommand, "idUsuario", obj.IdUsuarioRegistro);
            InputParameterAdd.Varchar(objCommand, "codigo", obj.codigo);
            InputParameterAdd.Varchar(objCommand, "nombre", obj.nombre);
            InputParameterAdd.Int(objCommand, "estado", obj.Estado);
            

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("ID", typeof(int)));

            foreach (Permiso item in obj.permisos)
            {
                DataRow rowObj = tvp.NewRow();
                rowObj["ID"] = item.idPermiso;
                tvp.Rows.Add(rowObj);
            }

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@permisos", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.IntegerList";

            OutputParameterAdd.Int(objCommand, "newId");

            ExecuteNonQuery(objCommand);

            obj.idRol = (int)objCommand.Parameters["@newId"].Value;

            return obj;

        }


        public Rol updateRol(Rol obj)
        {
            var objCommand = GetSqlCommand("pu_rol");
            InputParameterAdd.Int(objCommand, "idRol", obj.idRol);
            InputParameterAdd.Guid(objCommand, "idUsuario", obj.IdUsuarioRegistro);
            InputParameterAdd.Varchar(objCommand, "nombre", obj.nombre);
            InputParameterAdd.Varchar(objCommand, "codigo", obj.codigo);
            InputParameterAdd.Int(objCommand, "estado", obj.Estado);

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("ID", typeof(int)));

            foreach (Permiso item in obj.permisos)
            {
                DataRow rowObj = tvp.NewRow();
                rowObj["ID"] = item.idPermiso;
                tvp.Rows.Add(rowObj);
            }

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@permisos", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.IntegerList";
            

            ExecuteNonQuery(objCommand);

            return obj;
        }
        

        public List<Usuario> getUsuarios(int idRol)
        {
            var objCommand = GetSqlCommand("ps_rol_usuarios");
            InputParameterAdd.Int(objCommand, "idRol", idRol);
            DataTable dataTable = Execute(objCommand);
            List<Usuario> lista = new List<Usuario>();

            foreach (DataRow row in dataTable.Rows)
            {
                Usuario obj = new Usuario();
                obj.idUsuario = Converter.GetGuid(row, "id_usuario");
                obj.email = Converter.GetString(row, "email");
                obj.nombre = Converter.GetString(row, "nombre");
                lista.Add(obj);
            }

            return lista;
        }

        public void agregarUsuarioRol(int idRol, Guid idUsuario, Guid idUsuarioModifica)
        {
            var objCommand = GetSqlCommand("pi_rol_usuario");
            InputParameterAdd.Int(objCommand, "idRol", idRol);
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            InputParameterAdd.Guid(objCommand, "idUsuarioModifica", idUsuarioModifica);
            ExecuteNonQuery(objCommand);
        }

        public void quitarUsuarioRol(int idRol, Guid idUsuario)
        {
            var objCommand = GetSqlCommand("pd_rol_usuario");
            InputParameterAdd.Int(objCommand, "idRol", idRol);
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);

            ExecuteNonQuery(objCommand);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using Model;

namespace DataLayer
{
    public class MensajeDAL : DaoBase
    {
        public MensajeDAL(IDalSettings settings) : base(settings)
        {
        }
        public MensajeDAL() : this(new CotizadorSettings())
        {
        }


        public Mensaje insertMensaje(Mensaje obj)
        {

            var objCommand = GetSqlCommand("pi_mensaje");
            InputParameterAdd.Varchar(objCommand, "titulo", obj.titulo);
            InputParameterAdd.Text(objCommand, "mensaje", obj.mensaje);
            InputParameterAdd.Varchar(objCommand, "importancia", obj.importancia);
            InputParameterAdd.Guid(objCommand, "id_usuario_creacion", obj.user.idUsuario);
            InputParameterAdd.Guid(objCommand, "id_usuario_modificacion", obj.user.idUsuario);
            InputParameterAdd.DateTime(objCommand, "fecha_vencimiento", obj.fechaVencimientoMensaje);
            InputParameterAdd.DateTime(objCommand, "fecha_inicio", obj.fechaInicioMensaje);


            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("ID", typeof(int)));

            foreach (Rol item in obj.roles)
            {
                DataRow rowObj = tvp.NewRow();
                rowObj["ID"] = item.idRol;
                tvp.Rows.Add(rowObj);
            }
            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@roles", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.IntegerList";

            ExecuteNonQuery(objCommand);

            return obj;


        }

        public Mensaje updateMensaje(Mensaje obj)
        {
            var objCommand = GetSqlCommand("pu_mensaje");

            InputParameterAdd.Guid(objCommand, "id_mensaje", obj.id_mensaje);
            InputParameterAdd.Varchar(objCommand, "titulo", obj.titulo);
            InputParameterAdd.Text(objCommand, "mensaje", obj.mensaje);
            InputParameterAdd.Varchar(objCommand, "importancia", obj.importancia);
            InputParameterAdd.Guid(objCommand, "id_usuario_modificacion", obj.user.idUsuario);
            InputParameterAdd.DateTime(objCommand, "fecha_vencimiento", obj.fechaVencimientoMensaje);
            InputParameterAdd.DateTime(objCommand, "fecha_inicio", obj.fechaInicioMensaje);

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("ID", typeof(int)));

            foreach (Rol item in obj.roles)
            {
                DataRow rowObj = tvp.NewRow();
                rowObj["ID"] = item.idRol;
                tvp.Rows.Add(rowObj);
            }
            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@roles", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.IntegerList";


            ExecuteNonQuery(objCommand);

            return obj;

        }




        public List<Mensaje> getMensajes(Guid idUsuario)
        {
            var objCommand = GetSqlCommand("ps_alerta_mensaje_usuario");
            List<Mensaje> lista = new List<Mensaje>();

            InputParameterAdd.Guid(objCommand, "id_usuario", idUsuario);
            DataTable dataTable = Execute(objCommand);
            foreach (DataRow row in dataTable.Rows)
            {
                Mensaje obj = new Mensaje();
                obj.user = new Usuario();
                obj.id_mensaje = Converter.GetGuid(row, "id_mensaje");
                obj.titulo = Converter.GetString(row, "titulo");
                obj.mensaje = Converter.GetString(row, "mensaje");
                obj.importancia = Converter.GetString(row, "importancia");
                obj.fechaCreacionMensaje = Converter.GetDateTime(row, "fecha_creacion");
                obj.fechaVencimientoMensaje = Converter.GetDateTime(row, "fecha_vencimiento");
                obj.fechaInicioMensaje = Converter.GetDateTime(row, "fecha_inicio");
                obj.usuario_creacion = Converter.GetString(row, "nombre");
                lista.Add(obj);
            }
            return lista;
        }


        public List<Mensaje> getListMensajes(Mensaje mensaje)
        {

            var objCommand = GetSqlCommand("ps_lista_mensajes");
            List<Mensaje> lista = new List<Mensaje>();
            InputParameterAdd.Int(objCommand, "estado", mensaje.estado);
            InputParameterAdd.SmallDateTime(objCommand, "fecha_creacion", mensaje.fechaCreacionMensaje);
            InputParameterAdd.SmallDateTime(objCommand, "fecha_vencimiento", mensaje.fechaVencimientoMensaje);
            InputParameterAdd.Guid(objCommand, "id_usuario_creacion", mensaje.user.idUsuario);
            DataTable dataTable = Execute(objCommand);
            foreach (DataRow row in dataTable.Rows)
            {
                Mensaje obj = new Mensaje();
                obj.user = new Usuario();
                obj.id_mensaje = Converter.GetGuid(row, "id_mensaje");
                obj.fechaCreacionMensaje = Converter.GetDateTime(row, "fecha_creacion");
                obj.titulo = Converter.GetString(row, "titulo");
                obj.usuario_creacion = Converter.GetString(row, "nombre");
                obj.fechaVencimientoMensaje = Converter.GetDateTime(row, "fecha_vencimiento");
                obj.fechaInicioMensaje = Converter.GetDateTime(row, "fecha_inicio");
                lista.Add(obj);
            }

            return lista;
        }


        public Mensaje updateMensajeVisto(Mensaje obj)
        {
            var objCommand = GetSqlCommand("pi_mensaje_visto");
            InputParameterAdd.Guid(objCommand, "id_mensaje", obj.id_mensaje);
            InputParameterAdd.Guid(objCommand, "id_usuario", obj.user.idUsuario);
            ExecuteNonQuery(objCommand);
            return obj;
        }


        public Mensaje getMensajeById(Guid idMensaje)
        {
            var objCommand = GetSqlCommand("ps_detalle_mensaje");
            InputParameterAdd.Guid(objCommand, "id_mensaje", idMensaje);

            DataSet dataSet = ExecuteDataSet(objCommand);

            DataTable mensaje = dataSet.Tables[0];
            DataTable roles = dataSet.Tables[1];

            Mensaje obj = new Mensaje();

            foreach (DataRow row in mensaje.Rows)
            {
                //obj.roles = Converter.GetInt(row, "id_vendedor");
                obj.id_mensaje = Converter.GetGuid(row, "id_mensaje");
                obj.fechaVencimientoMensaje = Converter.GetDateTime(row, "fecha_vencimiento");
                obj.fechaInicioMensaje = Converter.GetDateTime(row, "fecha_inicio");
                obj.titulo = Converter.GetString(row, "titulo");
                obj.importancia = Converter.GetString(row, "importancia");
                obj.mensaje = Converter.GetString(row, "mensaje");
            }

            obj.roles = new List<Rol>();
            foreach (DataRow row in roles.Rows)
            {
                Rol rol = new Rol();
                rol.idRol = Converter.GetInt(row, "id_rol");
                obj.roles.Add(rol);
            }


            return obj;
        }



    }
}

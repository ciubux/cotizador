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


            DataTable tmpuser = new DataTable();
            tmpuser.Columns.Add(new DataColumn("ID", typeof(Guid)));

            foreach (Usuario item in obj.listUsuario)
            {
                DataRow rowObj = tmpuser.NewRow();
                rowObj["ID"] = item.idUsuario;
                tmpuser.Rows.Add(rowObj);
            }

            SqlParameter tvparam2 = objCommand.Parameters.AddWithValue("@usuarios", tmpuser);
            tvparam2.SqlDbType = SqlDbType.Structured;
            tvparam2.TypeName = "dbo.UniqueIdentifierList";

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


            DataTable tmpuser = new DataTable();
            tmpuser.Columns.Add(new DataColumn("ID", typeof(Guid)));
            foreach (Usuario item in obj.listUsuario)
            {
                DataRow rowObj = tmpuser.NewRow();
                rowObj["ID"] = item.idUsuario;
                tmpuser.Rows.Add(rowObj);
            }
            SqlParameter tvparam2 = objCommand.Parameters.AddWithValue("@usuarios", tmpuser);
            tvparam2.SqlDbType = SqlDbType.Structured;
            tvparam2.TypeName = "dbo.UniqueIdentifierList";


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
            InputParameterAdd.DateTime(objCommand, "fecha_creacion_desde", mensaje.fechaCreacionMensajeDesde);
            InputParameterAdd.DateTime(objCommand, "fecha_creacion_hasta", mensaje.fechaCreacionMensajeHasta);

            InputParameterAdd.DateTime(objCommand, "fecha_vencimiento_desde", mensaje.fechaVencimientoMensajeDesde);
            InputParameterAdd.DateTime(objCommand, "fecha_vencimiento_hasta", mensaje.fechaVencimientoMensajeHasta);

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
            DataTable usuario = dataSet.Tables[2];

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

            obj.listUsuario = new List<Usuario>();
            foreach (DataRow row in usuario.Rows)
            {
                Usuario user = new Usuario();
                user.idUsuario = Converter.GetGuid(row, "id_usuario");
                user.nombre = Converter.GetString(row, "nombre");
                user.email = Converter.GetString(row, "email");
                obj.listUsuario.Add(user);
            }


            return obj;
        }
        public Mensaje MensajeVistoRespuesta(Mensaje obj)
        {
            var objCommand = GetSqlCommand("pi_mensaje_visto_repuesta");
            InputParameterAdd.Guid(objCommand, "id_mensaje", obj.id_mensaje);
            InputParameterAdd.Guid(objCommand, "id_usuario", obj.user.idUsuario);
            InputParameterAdd.Varchar(objCommand, "respuesta", obj.mensaje);
            InputParameterAdd.Varchar(objCommand, "titulo", obj.titulo);
            InputParameterAdd.Varchar(objCommand, "importancia", obj.importancia);

            ExecuteNonQuery(objCommand);
            return obj;
        }


        public List<Mensaje> getHiloMensaje(Mensaje obj)
        {
            var objCommand = GetSqlCommand("ps_ver_hilo_mensaje");
            InputParameterAdd.Guid(objCommand, "id_mensaje_mensaje_recibido", obj.id_mensaje);
            InputParameterAdd.Guid(objCommand, "id_usuario", obj.user.idUsuario);
            List<Mensaje> list = new List<Mensaje>();
            DataTable dataTable = Execute(objCommand);
            foreach (DataRow row in dataTable.Rows)
            {
                Mensaje mensaje = new Mensaje();
                mensaje.user = new Usuario();
                mensaje.id_mensaje = Converter.GetGuid(row, "id_mensaje");
                mensaje.user.nombre = Converter.GetString(row, "nombre");
                mensaje.user.idUsuario = Converter.GetGuid(row, "id_usuario");
                mensaje.fechaCreacionMensaje = Converter.GetDateTime(row, "fecha_creacion");
                mensaje.user.email = Converter.GetString(row, "email");
                mensaje.mensaje = Converter.GetString(row, "mensaje");
                list.Add(mensaje);
            }
            return list;
        }
    }
}

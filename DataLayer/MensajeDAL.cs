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
            InputParameterAdd.DateTime(objCommand, "fecha_vencimiento", obj.fechaVencimiento);

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
                obj.fecha_creacion_mensaje = Converter.GetDateTime(row, "fecha_creacion");
                obj.fechaVencimiento = Converter.GetDateTime(row, "fecha_vencimiento");
                obj.user.nombre = Converter.GetString(row, "nombre");
                lista.Add(obj);
            }
            return lista;
        }


        public void updateMensaje(Mensaje obj)
        {
            var objCommand = GetSqlCommand("pi_mensaje_visto");
            InputParameterAdd.Guid(objCommand, "id_mensaje", obj.id_mensaje);
            InputParameterAdd.Guid(objCommand, "id_usuario", obj.user.idUsuario);
            ExecuteNonQuery(objCommand);

        }


    }
}

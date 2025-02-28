using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Model;

namespace DataLayer
{
    public class NotificacionPendienteDAL : DaoBase
    {
        public NotificacionPendienteDAL(IDalSettings settings) : base(settings)
        {
        }
        public NotificacionPendienteDAL() : this(new CotizadorSettings())
        {
        }


        public List<NotificacionPendiente> notificacionesPendientes()
        {
            var objCommand = GetSqlCommand("ps_notificacionesPendientes");
            DataTable dataTable = Execute(objCommand);

            List<NotificacionPendiente> lista = new List<NotificacionPendiente>();

            foreach (DataRow row in dataTable.Rows)
            {
                NotificacionPendiente item = new NotificacionPendiente();  
                item.idNotificacionPendiente = Converter.GetGuid(row, "id_notificacion_pendiente");
                item.codigo = Converter.GetString(row, "codigo");
                item.destinatarios = Converter.GetString(row, "destinatarios");
                item.idRegistro = Converter.GetString(row, "id_registro");
                item.datosCorto = Converter.GetString(row, "datos_corto");
                item.datosLargo = Converter.GetString(row, "datos_largo");

                lista.Add(item);
            }

            return lista;
        }


        public bool registrarNotificacionesProcesadas(List<Guid> notificaciones, Guid idUsuario)
        {
            var objCommand = GetSqlCommand("pu_procesarNotificaciones");
            InputParameterAdd.Int(objCommand, "estadoProceso", 1);
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("ID", typeof(Guid)));

            foreach (Guid item in notificaciones)
            {
                DataRow rowObj = tvp.NewRow();
                rowObj["ID"] = item;
                tvp.Rows.Add(rowObj);
            }
            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@idNotificaciones", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.UniqueIdentifierList";


            ExecuteNonQuery(objCommand);

            return true;
        }
    }
}

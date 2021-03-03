using DataLayer.DALConverter;
using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using Model;
using Model.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class LogCambioDAL : DaoBase
    {
        public LogCambioDAL(IDalSettings settings) : base(settings)
        {
        }

        public LogCambioDAL() : this(new CotizadorSettings())
        {
        }

        public List<LogCambio> getCambiosAplicar()
        {
            var objCommand = GetSqlCommand("ps_cambios_aplicar");
            DataTable dataTable = Execute(objCommand);
            List<LogCambio> lista = new List<LogCambio>();

            foreach (DataRow row in dataTable.Rows)
            {
                LogCambio obj = new LogCambio();
                obj.idCambio = Converter.GetGuid(row, "id_cambio_programado");
                obj.idCampo = Converter.GetInt(row, "id_catalogo_campo");
                obj.idTabla = Converter.GetInt(row, "id_catalogo_Tabla");

                obj.tabla = new LogTabla();
                obj.tabla.idTabla = obj.idTabla;
                obj.tabla.nombre = Converter.GetString(row, "tabla");

                obj.campo = new LogCampo();
                obj.campo.idCampo = obj.idCampo;
                obj.campo.nombre = Converter.GetString(row, "campo");

                obj.idRegistro = Converter.GetString(row, "id_registro");
                obj.valor = Converter.GetString(row, "valor");

                obj.estado = Converter.GetInt(row, "estado") == 1 ? true : false;
                obj.persisteCambio = Converter.GetInt(row, "persiste_cambio") == 1 ? true : false;

                obj.fechaInicioVigencia = Converter.GetDateTime(row, "fecha_inicio_vigencia");
                obj.idUsuarioModificacion = Converter.GetGuid(row, "usuario_modificacion");

                lista.Add(obj);
            }

            return lista;
        }

        public bool traspasarCambios(List<LogCambio> logs)
        {
            var objCommand = GetSqlCommand("pp_transferir_cambio");

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("ID", typeof(Guid)));
            tvp.Columns.Add(new DataColumn("ID_REGISTRO", typeof(Guid)));
            tvp.Columns.Add(new DataColumn("REPITE_DATO", typeof(int)));
            
            foreach (LogCambio item in logs)
            {
                DataRow rowObj = tvp.NewRow();
                rowObj["ID"] = item.idCambio;
                rowObj["ID_REGISTRO"] = item.idRegistro;
                rowObj["REPITE_DATO"] = item.repiteDato ? 1 : 0;
                tvp.Rows.Add(rowObj);
            }

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@cambios", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.LogAplicarProgramadoList";


            ExecuteNonQuery(objCommand);

            return true;
        }

        public List<LogCambio> getLogCamposRegistro(string idRegistro, List<int> campos)
        {
            var objCommand = GetSqlCommand("ps_log_registro");
            InputParameterAdd.Varchar(objCommand, "idRegistro", idRegistro);

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("ID", typeof(int)));

            foreach (int idCampo in campos)
            {
                DataRow rowObj = tvp.NewRow();
                rowObj["ID"] = idCampo;
                tvp.Rows.Add(rowObj);
            }

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@campos", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.IntegerList";

            DataTable dataTable = Execute(objCommand);
            List<LogCambio> lista = new List<LogCambio>();

            foreach (DataRow row in dataTable.Rows)
            {
                LogCambio obj = new LogCambio();
                obj.idCampo = Converter.GetInt(row, "id_catalogo_campo");
                obj.idRegistro = idRegistro;
                
                obj.valor = Converter.GetString(row, "valor");

                obj.fechaInicioVigencia = Converter.GetDateTime(row, "fecha_inicio_vigencia");
                obj.FechaEdicion = Converter.GetDateTime(row, "fecha_modificacion");

                lista.Add(obj);
            }

            return lista;
        }

        public LogCambio insertLogProgramado(LogCambio log)
        {
            var objCommand = GetSqlCommand("pi_cambio_dato_programado");

            InputParameterAdd.Guid(objCommand, "idUsuario", log.idUsuarioModificacion);
            InputParameterAdd.Int(objCommand, "idCatalogoTabla", log.idTabla);
            InputParameterAdd.Int(objCommand, "idCatalogoCampo", log.idCampo);
            InputParameterAdd.SmallInt(objCommand, "persisteCambio", log.persisteCambio ? (short) 1 : (short) 0);

            InputParameterAdd.Varchar(objCommand, "idRegistro", log.idRegistro);
            InputParameterAdd.VarcharEmpty(objCommand, "valor", log.valor);
            InputParameterAdd.Varchar(objCommand, "fechaInicioVigencia", log.fechaInicioVigencia.ToString("yyyy-MM-dd"));

            ExecuteNonQuery(objCommand);
            
            return log;
        }

        public LogCambio insertLog(LogCambio log)
        {
            var objCommand = GetSqlCommand("pi_cambio_dato_directo");

            InputParameterAdd.Guid(objCommand, "idUsuario", log.idUsuarioModificacion);
            InputParameterAdd.Int(objCommand, "idCatalogoTabla", log.idTabla);
            InputParameterAdd.Int(objCommand, "idCatalogoCampo", log.idCampo);

            InputParameterAdd.Varchar(objCommand, "idRegistro", log.idRegistro);
            InputParameterAdd.Varchar(objCommand, "valor", log.valor);
            InputParameterAdd.Varchar(objCommand, "fechaInicioVigencia", log.fechaInicioVigencia.ToString("yyyy-MM-dd"));

            ExecuteNonQuery(objCommand);

            return log;
        }

        /*
        public void insertLogProgramadoFull(List<LogCambio> logs)
        {
            var objCommand = GetSqlCommand("pi_cambios_datos_programados");

            InputParameterAdd.Guid(objCommand, "idUsuario", log.idUsuarioModificacion);
            InputParameterAdd.Int(objCommand, "idCatalogoTabla", log.idTabla);
            InputParameterAdd.Int(objCommand, "idCatalogoCampo", log.idCampo);

            InputParameterAdd.Varchar(objCommand, "idRegistro", log.idRegistro);
            InputParameterAdd.Varchar(objCommand, "valor", log.valor);
            InputParameterAdd.Varchar(objCommand, "fechaInicioVigencia", log.fechaInicioVigencia.ToString("yyyy-MM-dd"));

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("ID_USUARIO", typeof(Guid)));
            tvp.Columns.Add(new DataColumn("ID_REGISTRO", typeof(Guid)));
            tvp.Columns.Add(new DataColumn("ID_CATALOGO_TABLA", typeof(int)));
            tvp.Columns.Add(new DataColumn("ID_CATALOGO_CAMPO", typeof(int)));
            tvp.Columns.Add(new DataColumn("VALOR", typeof(string)));
            tvp.Columns.Add(new DataColumn("FECHA_INICIO_VIGENCIA", typeof(string)));

            foreach (LogCambio item in logs)
            {
                DataRow rowObj = tvp.NewRow();
                rowObj["ID"] = item.idCambio;
                rowObj["ID_REGISTRO"] = item.idRegistro;
                rowObj["REPITE_DATO"] = item.repiteDato ? 1 : 0;
                tvp.Rows.Add(rowObj);
            }

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@cambios", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.LogAplicarProgramadoList";


            ExecuteNonQuery(objCommand);
        }
        */

        /* Elimina el log de cambios programdos con fecha de inicio de vigencia de hoy al pasado y sin enviarlos al log de cambios pasados */
        public bool limpiarCambiosProgramados()
        {
            var objCommand = GetSqlCommand("pd_limpiar_cambios_programados_pasados");

            ExecuteNonQuery(objCommand);

            return true;
        }


        

    }
}


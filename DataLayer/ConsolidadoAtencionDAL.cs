using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DataLayer
{
    public class ConsolidadoAtencionDAL : DaoBase
    {
        public ConsolidadoAtencionDAL(IDalSettings settings) : base(settings) { }
        public ConsolidadoAtencionDAL() : this(new CotizadorSettings()) { }

        public ConsolidadoAtencion getConsolidadoAtencion(Guid idConsolidado)
        {
            var objCommand = GetSqlCommand("ps_consolidado_atencion");
            InputParameterAdd.Guid(objCommand, "idConsolidado", idConsolidado);
            DataTable dataTable = Execute(objCommand);
            ConsolidadoAtencion obj = new ConsolidadoAtencion();

            foreach (DataRow row in dataTable.Rows)
            {
                MapearObjeto(obj, row);
            }
            return obj;
        }

        public List<ConsolidadoAtencion> getConsolidadosAtencion(ConsolidadoAtencion filtro)
        {
            var objCommand = GetSqlCommand("ps_consolidados_atencion");
            InputParameterAdd.DateTime(objCommand, "fecha", filtro.fecha);
            InputParameterAdd.Bit(objCommand, "estado", filtro.Estado == 1);
            InputParameterAdd.Guid(objCommand, "idUsuario", filtro.IdUsuarioRegistro);

            DataTable dataTable = Execute(objCommand);
            List<ConsolidadoAtencion> lista = new List<ConsolidadoAtencion>();

            foreach (DataRow row in dataTable.Rows)
            {
                ConsolidadoAtencion obj = new ConsolidadoAtencion();
                MapearObjeto(obj, row);
                lista.Add(obj);
            }
            return lista;
        }

        public ConsolidadoAtencion insertConsolidadoAtencion(ConsolidadoAtencion obj)
        {
            var objCommand = GetSqlCommand("pi_consolidado_atencion");
            InputParameterAdd.DateTime(objCommand, "fecha", obj.fecha);
            InputParameterAdd.Varchar(objCommand, "observaciones", obj.observaciones?.Trim());
            InputParameterAdd.Int(objCommand, "idVehiculo", obj.vehiculo.idVehiculo);
            InputParameterAdd.Int(objCommand, "idChofer", obj.chofer.idPersonalAlmacen);
            InputParameterAdd.Int(objCommand, "idAsistente", obj.asistente.idPersonalAlmacen);
            InputParameterAdd.Guid(objCommand, "idUsuario", obj.IdUsuarioRegistro);

            OutputParameterAdd.UniqueIdentifier(objCommand, "newId");
            ExecuteNonQuery(objCommand);

            obj.idConsolidadoAtencion = (Guid)objCommand.Parameters["@newId"].Value;
            return obj;
        }

        public ConsolidadoAtencion updateConsolidadoAtencion(ConsolidadoAtencion obj)
        {
            var objCommand = GetSqlCommand("pu_consolidado_atencion");
            InputParameterAdd.Guid(objCommand, "idConsolidado", obj.idConsolidadoAtencion);
            InputParameterAdd.DateTime(objCommand, "fecha", obj.fecha);
            InputParameterAdd.Varchar(objCommand, "observaciones", obj.observaciones?.Trim());
            InputParameterAdd.Int(objCommand, "idVehiculo", obj.vehiculo.idVehiculo);
            InputParameterAdd.Int(objCommand, "idChofer", obj.chofer.idPersonalAlmacen);
            InputParameterAdd.Int(objCommand, "idAsistente", obj.asistente.idPersonalAlmacen);
            InputParameterAdd.Bit(objCommand, "estado", obj.Estado == 1);
            InputParameterAdd.Guid(objCommand, "idUsuario", obj.IdUsuarioRegistro);

            ExecuteNonQuery(objCommand);
            return obj;
        }

        // Método privado para evitar repetir código de mapeo entre SELECT individual y lista
        private void MapearObjeto(ConsolidadoAtencion obj, DataRow row)
        {
            obj.idConsolidadoAtencion = Converter.GetGuid(row, "id_consolidado_atencion");
            obj.fecha = Converter.GetDateTime(row, "fecha");
            obj.observaciones = Converter.GetString(row, "observaciones");
            obj.Estado = Converter.GetBool(row, "estado") ? 1 : 0;

            obj.vehiculo.idVehiculo = Converter.GetInt(row, "id_vehiculo");
            obj.vehiculo.placa = Converter.GetString(row, "placa_vehiculo");

            obj.chofer.idPersonalAlmacen = Converter.GetInt(row, "id_personal_almacen_chofer");
            obj.chofer.apellidoPaterno = Converter.GetString(row, "chofer_apellido_paterno");
            obj.chofer.apellidoMaterno = Converter.GetString(row, "chofer_apellido_materno");
            obj.chofer.nombres = Converter.GetString(row, "chofer_nombres");

            obj.asistente.idPersonalAlmacen = Converter.GetInt(row, "id_personal_almacen_asistente");
            obj.asistente.apellidoPaterno = Converter.GetString(row, "asistente_apellido_paterno");
            obj.asistente.apellidoMaterno = Converter.GetString(row, "asistente_apellido_materno");
            obj.asistente.nombres = Converter.GetString(row, "asistente_nombres");
        }
    }
}

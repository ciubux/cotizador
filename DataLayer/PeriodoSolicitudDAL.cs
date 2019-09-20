using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

namespace DataLayer
{
    public class PeriodoSolicitudDAL : DaoBase
    {
        public PeriodoSolicitudDAL(IDalSettings settings) : base(settings)
        {
        }
        public PeriodoSolicitudDAL() : this(new CotizadorSettings())
        {
        }



        public PeriodoSolicitud getPeriodoSolicitud(Guid id, Guid idUsuario)
        {
            var objCommand = GetSqlCommand("CLIENTE.ps_periodo_solicitud");
            InputParameterAdd.Guid(objCommand, "idPeriodoSolcitud", id);
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            DataTable dataTable = Execute(objCommand);
            PeriodoSolicitud obj = new PeriodoSolicitud();

            foreach (DataRow row in dataTable.Rows)
            {
                obj.idPeriodoSolicitud = Converter.GetGuid(row, "id_periodo_solicitud");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.fechaInicio = Converter.GetDateTime(row, "fecha_inicio");
                obj.fechaFin = Converter.GetDateTime(row, "fecha_fin");
                obj.Estado = Converter.GetInt(row, "estado");
            }

            return obj;
        }


        public List<PeriodoSolicitud> getPeriodosSolicitud(PeriodoSolicitud objSearch)
        {
            var objCommand = GetSqlCommand("CLIENTE.ps_periodos_solicitud");
            InputParameterAdd.Guid(objCommand, "idUsuario", objSearch.IdUsuarioRegistro);
            DataTable dataTable = Execute(objCommand);
            List<PeriodoSolicitud> lista = new List<PeriodoSolicitud>();

            foreach (DataRow row in dataTable.Rows)
            {
                PeriodoSolicitud obj = new PeriodoSolicitud();
                obj.idPeriodoSolicitud = Converter.GetGuid(row, "id_periodo_solicitud");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.fechaInicio = Converter.GetDateTime(row, "fecha_inicio");
                obj.fechaFin = Converter.GetDateTime(row, "fecha_fin");
                obj.Estado = Converter.GetInt(row, "estado");
                lista.Add(obj);
            }

            return lista;
        }


        public List<PeriodoSolicitud> getPeriodosSolicitudVigentes(PeriodoSolicitud objSearch, bool excluirPeriodosConRequerimientos)
        {
            var objCommand = GetSqlCommand("CLIENTE.ps_periodos_solicitud_vigentes");
            InputParameterAdd.Guid(objCommand, "idUsuario", objSearch.IdUsuarioRegistro);
            InputParameterAdd.Int(objCommand, "periodosSinRequerimientos", excluirPeriodosConRequerimientos ? 1 : 0);


            DataTable dataTable = Execute(objCommand);
            List<PeriodoSolicitud> lista = new List<PeriodoSolicitud>();

            foreach (DataRow row in dataTable.Rows)
            {
                PeriodoSolicitud obj = new PeriodoSolicitud();
                obj.idPeriodoSolicitud = Converter.GetGuid(row, "id_periodo_solicitud");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.fechaInicio = Converter.GetDateTime(row, "fecha_inicio");
                obj.fechaFin = Converter.GetDateTime(row, "fecha_fin");
                obj.Estado = Converter.GetInt(row, "estado");
                lista.Add(obj);
            }

            return lista;
        }



        public PeriodoSolicitud insertPeriodoSolicitud(PeriodoSolicitud obj)
        {
            var objCommand = GetSqlCommand("CLIENTE.pi_periodo_solicitud");

            
            InputParameterAdd.Guid(objCommand, "idUsuario", obj.IdUsuarioRegistro);
            InputParameterAdd.Varchar(objCommand, "nombre", obj.nombre);
            InputParameterAdd.Varchar(objCommand, "fechaInicio", obj.fechaInicio.ToString("yyyy-MM-dd"));
            InputParameterAdd.Varchar(objCommand, "fechaFin", obj.fechaFin.ToString("yyyy-MM-dd"));
            InputParameterAdd.Int(objCommand, "estado", obj.Estado);

            OutputParameterAdd.UniqueIdentifier(objCommand, "newId");

            ExecuteNonQuery(objCommand);

            obj.idPeriodoSolicitud = (Guid)objCommand.Parameters["@newId"].Value;

            return obj;

        }


        public PeriodoSolicitud updatePeriodoSolicitud(PeriodoSolicitud obj)
        {
            var objCommand = GetSqlCommand("CLIENTE.pu_periodo_solicitud");
            
            InputParameterAdd.Guid(objCommand, "idPeriodoSolicitud", obj.idPeriodoSolicitud);
            InputParameterAdd.Guid(objCommand, "idUsuario", obj.usuario.idUsuario);
            InputParameterAdd.Varchar(objCommand, "nombre", obj.nombre);
            InputParameterAdd.Varchar(objCommand, "fechaInicio", obj.fechaInicio.ToString("yyyy-MM-dd"));
            InputParameterAdd.Varchar(objCommand, "fechaFin", obj.fechaFin.ToString("yyyy-MM-dd"));
            InputParameterAdd.Int(objCommand, "estado", obj.Estado);

            ExecuteNonQuery(objCommand);

            return obj;

        }
    }
}

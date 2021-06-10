using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace DataLayer
{
    public class AlertaValidacionDAL : DaoBase
    {
        public AlertaValidacionDAL(IDalSettings settings) : base(settings)
        {
        }

        public AlertaValidacionDAL() : this(new CotizadorSettings())
        {
        }

        public List<GrupoCliente> getGruposCliente()
        {
            var objCommand = GetSqlCommand("ps_gruposCliente");
            //InputParameterAdd.Guid(objCommand, "idGrupo", idGrupo);
            DataTable dataTable = Execute(objCommand);
            List<GrupoCliente> grupoClienteList = new List<GrupoCliente>();

            foreach (DataRow row in dataTable.Rows)
            {
                GrupoCliente grupoCliente = new GrupoCliente();
                grupoCliente.idGrupoCliente = Converter.GetInt(row, "id_grupo_cliente");
                grupoCliente.codigo = Converter.GetString(row, "codigo");
                grupoCliente.nombre = Converter.GetString(row, "nombre");

                grupoCliente.ciudad = new Ciudad();
                grupoCliente.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                grupoCliente.plazoCreditoSolicitado = (DocumentoVenta.TipoPago)Converter.GetInt(row, "plazo_credito_solicitado");
                grupoCliente.plazoCreditoAprobado = (DocumentoVenta.TipoPago)Converter.GetInt(row, "plazo_credito_aprobado");

                grupoClienteList.Add(grupoCliente);
            }

            return grupoClienteList;
        }



        public List<AlertaValidacion> getAlertasPorUsuario(Usuario usuario)
        {
            var objCommand = GetSqlCommand("ps_alertasPendientesTipo");
            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("tipoList", typeof(string)));

            if (usuario.modificaMiembrosGrupoCliente) { 
                tvp.Rows.Add(AlertaValidacion.CAMBIA_GRUPO_CLIENTE);
            }

            if (usuario.modificaGrupoClientes)
            {
                tvp.Rows.Add(AlertaValidacion.CREA_GRUPO_CLIENTE);
            }

            if (usuario.apruebaAnulaciones)
            {
                tvp.Rows.Add(AlertaValidacion.SOLICITUD_ANULACION_CPE);
            }

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@tipoList", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.VarcharCList";

            DataTable dataTable = Execute(objCommand);
            List<AlertaValidacion> alertas = new List<AlertaValidacion>();

            foreach (DataRow row in dataTable.Rows)
            {
                AlertaValidacion obj = new AlertaValidacion();
                obj.IdAlertaValidacion = Converter.GetGuid(row, "id_alerta_validacion");
                obj.nombreTabla = Converter.GetString(row, "nombre_tabla");
                obj.idRegistro = Converter.GetString(row, "id_registro");
                obj.tipo = Converter.GetString(row, "tipo");

                obj.data = JsonConvert.DeserializeObject<DataAlertaValidacion>(Converter.GetString(row, "data_validacion"));
                obj.IdUsuarioCreacion = Converter.GetGuid(row, "id_usuario");

                obj.FechaCreacion = Converter.GetDateTime(row, "fecha_creacion");

                obj.UsuarioCreacion = new Usuario();
                obj.UsuarioCreacion.idUsuario = obj.IdUsuarioCreacion;
                obj.UsuarioCreacion.nombre = Converter.GetString(row, "nombre_usuario");

                alertas.Add(obj);
            }

            return alertas;
        }
        

        public AlertaValidacion insertAlertaValidacion(AlertaValidacion alertaValidacion)
        {
            var objCommand = GetSqlCommand("pi_alertaValidacion");

            InputParameterAdd.Guid(objCommand, "idUsuario", alertaValidacion.IdUsuarioCreacion);
            InputParameterAdd.Varchar(objCommand, "nombreTabla", alertaValidacion.nombreTabla);
            InputParameterAdd.Varchar(objCommand, "tipo", alertaValidacion.tipo);
            InputParameterAdd.Varchar(objCommand, "idRegistro", alertaValidacion.idRegistro);
            InputParameterAdd.Text(objCommand, "dataValidacion", JsonConvert.SerializeObject(alertaValidacion.data));

            OutputParameterAdd.UniqueIdentifier(objCommand, "newId");

            ExecuteNonQuery(objCommand);
            
            alertaValidacion.IdAlertaValidacion = (Guid)objCommand.Parameters["@newId"].Value;
            return alertaValidacion;
        }


        public bool validaAlertaValidacion(Guid idAlertaValidacion, Guid idUsuario)
        {
            var objCommand = GetSqlCommand("pu_validaAlertaValidacion");

            InputParameterAdd.Guid(objCommand, "idAlertaValidacion", idAlertaValidacion);
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);

            ExecuteNonQuery(objCommand);

            return true;
        }
    }
}

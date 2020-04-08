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
    public class VistaDashboardDAL : DaoBase
    {
        public VistaDashboardDAL(IDalSettings settings) : base(settings)
        {
        }

        public VistaDashboardDAL() : this(new CotizadorSettings())
        {
        }

        public List<VistaDashboard> getVistasDashboard(VistaDashboard obj)
        {
            var objCommand = GetSqlCommand("ps_vista_dashboard");
            InputParameterAdd.Varchar(objCommand, "codigo", obj.codigo);
            InputParameterAdd.Varchar(objCommand, "nombre", obj.nombre);
            DataTable dataTable = Execute(objCommand);

            List<VistaDashboard> lista = new List<VistaDashboard>();
            foreach (DataRow row in dataTable.Rows)
            {
                VistaDashboard dashboard = new VistaDashboard();
                dashboard.tipoVistaDashboard= new TipoVistaDashboard();
                dashboard.idVistaDashboard = Converter.GetInt(row, "id_vista_dashboard");
                dashboard.idTipoVistaDashboard = Converter.GetInt(row, "id_tipo_vista_dashboard");
                dashboard.codigo = Converter.GetString(row, "codigo");
                dashboard.nombre = Converter.GetString(row, "nombre");
                dashboard.descripcion = Converter.GetString(row, "descripcion");
                dashboard.bloquesAncho = Converter.GetInt(row, "bloques_ancho");
                dashboard.altoPx = Converter.GetInt(row, "alto_px");
                dashboard.estado = Converter.GetInt(row, "estado");

                dashboard.tipoVistaDashboard.idTipoVistaDashboard = Converter.GetInt(row, "id_tipo_vista_dashboard");
                dashboard.tipoVistaDashboard.nombre = Converter.GetString(row, "nombreTipo");
                lista.Add(dashboard);
            }
            return lista;

        }


        public VistaDashboard getVistaDashboardById(int vistaDasboard)
        {
            var objCommand = GetSqlCommand("ps_detalle_vista_dashboard");
            InputParameterAdd.Int(objCommand, "id_vista_dashboard", vistaDasboard);

            DataTable dataTable = Execute(objCommand);

            VistaDashboard obj = new VistaDashboard();

            foreach (DataRow row in dataTable.Rows)
            {
                //obj.roles = Converter.GetInt(row, "id_vendedor");
                obj.idVistaDashboard = Converter.GetInt(row, "id_vista_dashboard");
                obj.idTipoVistaDashboard = Converter.GetInt(row, "id_tipo_vista_dashboard");
                obj.codigo = Converter.GetString(row, "codigo");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.descripcion = Converter.GetString(row, "descripcion");
                obj.bloquesAncho = Converter.GetInt(row, "bloques_ancho");
                obj.altoPx = Converter.GetInt(row, "alto_px");
            }
            return obj;
        }

        public VistaDashboard updateVistaDashboard(VistaDashboard vistaDasboard,Guid idUsuario)
        {
            var objCommand = GetSqlCommand("pu_vista_dashboard");
            InputParameterAdd.Int(objCommand, "id_vista_dashboard", vistaDasboard.idVistaDashboard);
            InputParameterAdd.Int(objCommand, "id_tipo_vista_dashboard", vistaDasboard.idTipoVistaDashboard);
            InputParameterAdd.Varchar(objCommand, "codigo", vistaDasboard.codigo);
            InputParameterAdd.Varchar(objCommand, "nombre", vistaDasboard.nombre);
            InputParameterAdd.Varchar(objCommand, "descripcion", vistaDasboard.descripcion);
            InputParameterAdd.Int(objCommand, "bloques_ancho", vistaDasboard.bloquesAncho);
            InputParameterAdd.Int(objCommand, "alto_px", vistaDasboard.altoPx);
            InputParameterAdd.Guid(objCommand, "id_usuario", idUsuario);
            ExecuteNonQuery(objCommand);
            return vistaDasboard;
        }

        public VistaDashboard insertVistaDashboard(VistaDashboard vistaDasboard, Guid idUsuario)
        {
            var objCommand = GetSqlCommand("pi_vista_dashboard");           
            InputParameterAdd.Int(objCommand, "id_tipo_vista_dashboard", vistaDasboard.idTipoVistaDashboard);
            InputParameterAdd.Varchar(objCommand, "codigo", vistaDasboard.codigo);
            InputParameterAdd.Varchar(objCommand, "nombre", vistaDasboard.nombre);
            InputParameterAdd.Varchar(objCommand, "descripcion", vistaDasboard.descripcion);
            InputParameterAdd.Int(objCommand, "bloques_ancho", vistaDasboard.bloquesAncho);
            InputParameterAdd.Int(objCommand, "alto_px", vistaDasboard.altoPx);
            InputParameterAdd.Guid(objCommand, "id_usuario", idUsuario);
            ExecuteNonQuery(objCommand);
            return vistaDasboard;
        }

        public List<VistaDashboard> getVistasDashboardByRol(int idRol)
        {
            var objCommand = GetSqlCommand("ps_rol_vista_dashboard");
            InputParameterAdd.Int(objCommand, "id_rol", idRol);           
            DataTable dataTable = Execute(objCommand);

            List<VistaDashboard>  VistasDashboard = new List<VistaDashboard>();
            foreach (DataRow row in dataTable.Rows)
            {
                VistaDashboard dashboard = new VistaDashboard();                
                dashboard.idVistaDashboard = Converter.GetInt(row, "id_vista_dashboard");                 
                dashboard.codigo = Converter.GetString(row, "codigo");
                dashboard.nombre = Converter.GetString(row, "nombre");
                dashboard.estado = Converter.GetInt(row, "estado"); 
                VistasDashboard.Add(dashboard);
            }
            return VistasDashboard;

        }

        public Rol updateRolVistaDashboard(Rol obj,Guid idUsuario)
        {
            var objCommand = GetSqlCommand("pu_rol_vista_dashboard");
            InputParameterAdd.Int(objCommand, "id_rol", obj.idRol);           
            InputParameterAdd.Guid(objCommand, "id_usuario", idUsuario);
            

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("ID", typeof(int)));

            foreach (VistaDashboard item in obj.VistasDashboard)
            {
                DataRow rowObj = tvp.NewRow();
                rowObj["ID"] = item.idVistaDashboard;
                tvp.Rows.Add(rowObj);
            }
            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@vistas_dashboard", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.IntegerList";
            ExecuteNonQuery(objCommand);

            return obj;          

        }
    }
}

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
    public class KpiDAL : DaoBase
    {
        public KpiDAL(IDalSettings settings) : base(settings)
        {
        }
        public KpiDAL() : this(new CotizadorSettings())
        {
        }

        public Rol getRolByCodigo(string codigo)
        {
            var objCommand = GetSqlCommand("ps_rol_by_codigo");
            InputParameterAdd.Varchar(objCommand, "codigo", codigo);
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


        public List<KpiMeta> getKPIResultados(Guid idUsuario, Guid idKpiPeriodo, Guid idKpi)
        {
            var objCommand = GetSqlCommand("ps_kpi_meta_periodo");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            InputParameterAdd.Guid(objCommand, "idKpi", idKpi);
            InputParameterAdd.Guid(objCommand, "idKpiPeriodo", idKpiPeriodo);
            DataSet dataSet = ExecuteDataSet(objCommand);

            DataTable dataTableResultados = dataSet.Tables[0];
            DataTable dataTableUsuarios = dataSet.Tables[0];

            List<KpiMeta> lista = new List<KpiMeta>();

            foreach (DataRow row in dataTableResultados.Rows)
            {
                KpiMeta obj = new KpiMeta();
                obj.usuario = new Usuario();
                obj.usuario.idUsuario = Converter.GetGuid(row, "id_usuario_res");
                obj.resultado = Converter.GetDecimal(row, "resultado");
                lista.Add(obj);
            }

            foreach (DataRow row in dataTableUsuarios.Rows)
            {
                Guid idUsuarioMeta = Converter.GetGuid(row, "id_usuario");
                KpiMeta obj = lista.Where(k => k.usuario.idUsuario == idUsuarioMeta).First();

                obj.usuario.email = Converter.GetString(row, "email");
                obj.usuario.nombre = Converter.GetString(row, "nombre");
                obj.valor = Converter.GetDecimal(row, "valor");
            }

            return lista;
        }


        public List<KpiPeriodo> getPeriodos(Usuario usuario)
        {
            var objCommand = GetSqlCommand("ps_kpi_periodos");
            InputParameterAdd.Guid(objCommand, "idUsuario", usuario.idUsuario);
            InputParameterAdd.Int(objCommand, "estado", 1);

            DataTable dataTable = Execute(objCommand);
            List<KpiPeriodo> lista = new List<KpiPeriodo>();

            foreach (DataRow row in dataTable.Rows)
            {
                KpiPeriodo obj = new KpiPeriodo();
                obj.idKpiPeriodo = Converter.GetGuid(row, "id_kpi_periodo");
                obj.periodo = Converter.GetString(row, "periodo");
                obj.desde = Converter.GetDateTime(row, "desde");
                obj.hasta = Converter.GetDateTime(row, "hasta");
                obj.Estado = Converter.GetInt(row, "estado");
                lista.Add(obj);
            }

            return lista;
        }


        public List<Kpi> getPeriodoKPIs(Usuario usuario, Guid idKpiPeriodo)
        {
            var objCommand = GetSqlCommand("ps_periodo_kpis");
            InputParameterAdd.Guid(objCommand, "idUsuario", usuario.idUsuario);
            InputParameterAdd.Int(objCommand, "idArea", usuario.area.idArea);
            InputParameterAdd.Int(objCommand, "multiUsuario", 1);
            InputParameterAdd.Int(objCommand, "multiArea", 1);
            InputParameterAdd.Guid(objCommand, "idKpiPeriodo", idKpiPeriodo);

            DataTable dataTable = Execute(objCommand);
            List<Kpi> lista = new List<Kpi>();

            foreach (DataRow row in dataTable.Rows)
            {
                Kpi obj = new Kpi();
                obj.idKpi = Converter.GetGuid(row, "id_kpi");
                obj.kpi = Converter.GetString(row, "kpi");
                lista.Add(obj);
            }

            return lista;
        }


        public List<Usuario> getKPIPeriodoUsuarios(Usuario usuario, Guid idKpiPeriodo, Guid idKpi)
        {
            var objCommand = GetSqlCommand("ps_kpi_usuarios");
            InputParameterAdd.Guid(objCommand, "idUsuario", usuario.idUsuario);
            InputParameterAdd.Int(objCommand, "idArea", usuario.area.idArea);
            InputParameterAdd.Int(objCommand, "multiUsuario", 1);
            InputParameterAdd.Int(objCommand, "multiArea", 1);
            InputParameterAdd.Guid(objCommand, "idKpiPeriodo", idKpiPeriodo);
            InputParameterAdd.Guid(objCommand, "idKpi", idKpi);

            DataTable dataTable = Execute(objCommand);
            List<Usuario> lista = new List<Usuario>();

            foreach (DataRow row in dataTable.Rows)
            {
                Usuario obj = new Usuario();
                obj.idUsuario = Converter.GetGuid(row, "id_usuario");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.email = Converter.GetString(row, "email");
                lista.Add(obj);
            }

            return lista;
        }

    }
}

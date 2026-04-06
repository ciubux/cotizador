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
    public class EmpresaDAL : DaoBase
    {
        public EmpresaDAL(IDalSettings settings) : base(settings)
        {
        }
        public EmpresaDAL() : this(new CotizadorSettings())
        {
        }

        public Empresa GetEmpresa(int idEmpresa, Guid idUsuario)
        {
            var objCommand = GetSqlCommand("ps_empresa");
            InputParameterAdd.Int(objCommand, "idEmpresa", idEmpresa);
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            DataTable dataTable = Execute(objCommand);
            Empresa obj = new Empresa();

            foreach (DataRow row in dataTable.Rows)
            {
                obj.idEmpresa = Converter.GetInt(row, "id_empresa");
                obj.codigo = Converter.GetString(row, "codigo");
                obj.ruc = Converter.GetString(row, "ruc");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.urlWeb = Converter.GetString(row, "url_web");
                obj.razonSocial = Converter.GetString(row, "razon_social");
                obj.factorCosto = Converter.GetDecimal(row, "factor_costo");
                obj.porcentajeMargenMinimo = Converter.GetDecimal(row, "porcentaje_margen_minimo");
                obj.porcentajeDescuentoInframargen = Converter.GetDecimal(row, "porcentaje_descuento_infra_margen");
                obj.porcentajeMDGanaciaMax = Converter.GetDecimal(row, "porcentaje_md_ganancia_max");
                obj.atencionTerciarizada = Converter.GetDecimal(row, "atencion_terciarizada") == 1 ? true : false;
                obj.facturacionHabilitada = Converter.GetDecimal(row, "facturacion_habilitada") == 1 ? true : false;
                obj.emiteGuias = Converter.GetDecimal(row, "emite_guias") == 1 ? true : false;

                Empresa.EntornoFacturacion emrelenfac;
                if (Enum.TryParse<Empresa.EntornoFacturacion>(Converter.GetString(row, "entorno_facturacion"), true, out emrelenfac))
                {
                    obj.entornoFacturacion = emrelenfac;
                }
                else
                {
                    obj.entornoFacturacion = Empresa.EntornoFacturacion.NINGUNO;
                }


                obj.Estado = Converter.GetInt(row, "estado");
            }

            return obj;
        }


        public void ActualizarParametrosEmpresa(Empresa empresa)
        {
            var objCommand = GetSqlCommand("pu_parametros_empresa");
            InputParameterAdd.Decimal(objCommand, "porcentajeMargenMinimo", empresa.porcentajeMargenMinimo);
            InputParameterAdd.Decimal(objCommand, "factorCosto", empresa.factorCosto);
            InputParameterAdd.Decimal(objCommand, "porcentajeGananciaMax", empresa.porcentajeMDGanaciaMax);
            InputParameterAdd.Decimal(objCommand, "porcentajeDescuentoInfraMargen", empresa.porcentajeDescuentoInframargen);
            InputParameterAdd.Guid(objCommand, "idUsuario", empresa.usuario.idUsuario);

            ExecuteNonQuery(objCommand);
        }


        public Empresa getEmpresaByCliente(Guid idCliente)
        {
            var objCommand = GetSqlCommand("ps_empresaByCliente");
            InputParameterAdd.Guid(objCommand, "idCliente", idCliente);
            DataTable dataTable = Execute(objCommand);
            Empresa obj = new Empresa();

            foreach (DataRow row in dataTable.Rows)
            {
                obj.idEmpresa = Converter.GetInt(row, "id_empresa");
                obj.codigo = Converter.GetString(row, "codigo");
                obj.ruc = Converter.GetString(row, "ruc");
                obj.razonSocial = Converter.GetString(row, "razon_social");
                obj.factorCosto = Converter.GetDecimal(row, "factor_costo");
                obj.porcentajeMargenMinimo = Converter.GetDecimal(row, "porcentaje_margen_minimo");
                obj.porcentajeDescuentoInframargen = Converter.GetDecimal(row, "porcentaje_descuento_infra_margen");
                obj.porcentajeMDGanaciaMax = Converter.GetDecimal(row, "porcentaje_md_ganancia_max");
            }

            return obj;
        }

    }
}



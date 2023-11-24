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


        public Empresa getEmpresaByCliente(Guid idCliente)
        {
            var objCommand = GetSqlCommand("ps_empresaByCliente");
            InputParameterAdd.Guid(objCommand, "idCliente", idCliente);
            DataTable dataTable = Execute(objCommand);
            Empresa obj = new Empresa();

            foreach (DataRow row in dataTable.Rows)
            {
                obj.idEmpresa = Converter.GetInt(row, "id_origen");
                obj.codigo = Converter.GetString(row, "codigo");
                obj.ruc = Converter.GetString(row, "ruc");
                obj.razonSocial = Converter.GetString(row, "razon_social");
                obj.factorCosto = Converter.GetDecimal(row, "factor_costo");
                obj.porcentajeMargenMinimo = Converter.GetDecimal(row, "porcentaje_margen_minimo");
                obj.porcentajeDescuentoInframargen = Converter.GetDecimal(row, "porcentaje_descuento_infra_margen");
            }

            return obj;
        }

    }
}

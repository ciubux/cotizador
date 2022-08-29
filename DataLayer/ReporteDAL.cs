using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Model;
using Model.UTILES;

namespace DataLayer
{
    public class ReporteDAL : DaoBase
    {
        public ReporteDAL(IDalSettings settings) : base(settings)
        {
        }
        public ReporteDAL() : this(new CotizadorSettings())
        {
        }


        public List<FilaProductoPendienteAtencion> productosPendientesAtencion(String sku, String familia, String proveedor, DateTime fechaInicio, DateTime fechaFin, Guid idCiudad, Guid idUsuario)
        {
            var objCommand = GetSqlCommand("ps_reporte_productos_pendientes");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            
            if (idCiudad.Equals(Guid.Empty))
            {
                InputParameterAdd.Guid(objCommand, "idCiudad", null);
            } else
            {
                InputParameterAdd.Guid(objCommand, "idCiudad", idCiudad);
            }

            InputParameterAdd.Varchar(objCommand, "sku", sku);
            InputParameterAdd.Varchar(objCommand, "familia", familia);
            InputParameterAdd.Varchar(objCommand, "proveedor", proveedor);

            fechaInicio = new DateTime(fechaInicio.Year, fechaInicio.Month, fechaInicio.Day, 0, 0, 0);
            fechaFin = new DateTime(fechaFin.Year, fechaFin.Month, fechaFin.Day, 23, 59, 59);

            InputParameterAdd.DateTime(objCommand, "fechaEntregaDesde", fechaInicio);
            InputParameterAdd.DateTime(objCommand, "fechaEntregaHasta", fechaFin);

            DataTable dataTable = Execute(objCommand);

            List<FilaProductoPendienteAtencion> resultados = new List<FilaProductoPendienteAtencion>();

            foreach (DataRow row in dataTable.Rows)
            {
                FilaProductoPendienteAtencion item = new FilaProductoPendienteAtencion();

                item.sede = Converter.GetString(row, "sede");
                item.sku = Converter.GetString(row, "sku");
                item.nombreProducto = Converter.GetString(row, "descripcion");
                item.familia = Converter.GetString(row, "familia");
                item.proveedor = Converter.GetString(row, "proveedor");

                item.unidad = Converter.GetString(row, "unidad");
                item.unidadAlternativa = Converter.GetString(row, "unidad_alternativa");
                item.unidadProveedor = Converter.GetString(row, "unidad_proveedor");
                item.unidadConteo = Converter.GetString(row, "unidad_conteo");

                item.eqMpConteo = Converter.GetInt(row, "equivalencia_conteo");
                item.eqAlternativaConteo = item.eqMpConteo / Converter.GetInt(row, "equivalencia_alternativa");
                item.eqProveedorConteo = Converter.GetInt(row, "equivalencia_proveedor") * item.eqMpConteo;

                item.cpConteo = Converter.GetInt(row, "cantidad_unidad_conteo_pedido") - Converter.GetInt(row, "cantidad_atendida");

                resultados.Add(item);
            }

            return resultados;
        }
    }
}

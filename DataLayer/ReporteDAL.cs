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
            
            if (idCiudad != null && !idCiudad.Equals(Guid.Empty))
            {
                InputParameterAdd.Guid(objCommand, "idCiudad", idCiudad);
            }

            InputParameterAdd.VarcharEmpty(objCommand, "sku", sku);
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
                
                item.idProducto = Converter.GetGuid(row, "id_producto");
                item.idSede = Converter.GetGuid(row, "id_sede");
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

        public List<List<String>> sellOutVendedores(String sku, String familia, String proveedor, String codVRC, String codVSC, String codVAC, 
            DateTime fechaInicio, DateTime fechaFin, int anio, int trimestre, String ciudad, int incluirVentasExcluidas, Guid idUsuario,
            int idGrupo, string ruc, bool integraEmpresas, bool excluirVentasRelacionadasHijas)
        {
            var objCommand = GetSqlCommand("ps_sellout_vendedores");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);

            InputParameterAdd.Varchar(objCommand, "ciudad", ciudad);
            InputParameterAdd.VarcharEmpty(objCommand, "codVRC", codVRC);
            InputParameterAdd.VarcharEmpty(objCommand, "codVSC", codVSC);
            InputParameterAdd.VarcharEmpty(objCommand, "codVAC", codVAC);

            InputParameterAdd.Int(objCommand, "anio", anio);
            InputParameterAdd.Int(objCommand, "trimestre", trimestre);

            InputParameterAdd.Int(objCommand, "incluirVentasExcluidas", incluirVentasExcluidas);

            InputParameterAdd.Int(objCommand, "verTodasEmpresas", integraEmpresas ? 1: 0);
            InputParameterAdd.Int(objCommand, "excluirVentasRelacionadasHijas", excluirVentasRelacionadasHijas ? 1 : 0);

            InputParameterAdd.VarcharEmpty(objCommand, "sku", sku);
            InputParameterAdd.Varchar(objCommand, "familia", familia);
            InputParameterAdd.Varchar(objCommand, "proveedor", proveedor);

            InputParameterAdd.Int(objCommand, "idGrupo", idGrupo);
            InputParameterAdd.VarcharEmpty(objCommand, "ruc", ruc);

            fechaInicio = new DateTime(fechaInicio.Year, fechaInicio.Month, fechaInicio.Day, 0, 0, 0);
            fechaFin = new DateTime(fechaFin.Year, fechaFin.Month, fechaFin.Day, 23, 59, 59);

            InputParameterAdd.DateTime(objCommand, "fechaDesde", fechaInicio);
            InputParameterAdd.DateTime(objCommand, "fechaHasta", fechaFin);

            DataTable dataTable = Execute(objCommand);

            List<List<String>> resultados = new List<List<String>>();

            foreach (DataRow row in dataTable.Rows)
            {
                List<String> item = new List<String>();

                item.Add(Converter.GetString(row, "VE_C"));
                item.Add(Converter.GetString(row, "RESPONSABLE_COMERCIAL"));

                Decimal subtotal = Converter.GetDecimal(row, "SUBTOTAL_ACUM");

                item.Add(String.Format(Constantes.formatoDosDecimales, subtotal));

                resultados.Add(item);
            }

            return resultados;
        }

        public List<List<String>> sellOutVendedoresDetalles(String codVendedor, String sku, String familia, String proveedor, String codVRC, String codVSC, String codVAC, 
            DateTime fechaInicio, DateTime fechaFin, int anio, int trimestre, String ciudad, int incluirVentasExcluidas, Guid idUsuario,
            int idGrupo, string ruc, bool integraEmpresas, bool excluirVentasRelacionadasHijas)
        {
            var objCommand = GetSqlCommand("ps_sellout_vendedores_detalles");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);

            InputParameterAdd.VarcharEmpty(objCommand, "codigoVendedor", codVendedor);
            InputParameterAdd.Varchar(objCommand, "ciudad", ciudad);

            InputParameterAdd.Int(objCommand, "anio", anio);
            InputParameterAdd.Int(objCommand, "trimestre", trimestre);

            InputParameterAdd.Int(objCommand, "incluirVentasExcluidas", incluirVentasExcluidas);
            InputParameterAdd.Int(objCommand, "verTodasEmpresas", integraEmpresas ? 1 : 0);
            InputParameterAdd.Int(objCommand, "excluirVentasRelacionadasHijas", excluirVentasRelacionadasHijas ? 1 : 0);

            InputParameterAdd.VarcharEmpty(objCommand, "codVRC", codVRC);
            InputParameterAdd.VarcharEmpty(objCommand, "codVSC", codVSC);
            InputParameterAdd.VarcharEmpty(objCommand, "codVAC", codVAC);

            InputParameterAdd.VarcharEmpty(objCommand, "sku", sku);
            InputParameterAdd.Varchar(objCommand, "familia", familia);
            InputParameterAdd.Varchar(objCommand, "proveedor", proveedor);

            fechaInicio = new DateTime(fechaInicio.Year, fechaInicio.Month, fechaInicio.Day, 0, 0, 0);
            fechaFin = new DateTime(fechaFin.Year, fechaFin.Month, fechaFin.Day, 23, 59, 59);

            InputParameterAdd.DateTime(objCommand, "fechaDesde", fechaInicio);
            InputParameterAdd.DateTime(objCommand, "fechaHasta", fechaFin);

            InputParameterAdd.Int(objCommand, "idGrupo", idGrupo);
            InputParameterAdd.VarcharEmpty(objCommand, "ruc", ruc);

            DataTable dataTable = Execute(objCommand);

            List<List<String>> resultados = new List<List<String>>();

            foreach (DataRow row in dataTable.Rows)
            {
                List<String> item = new List<String>();

                item.Add(Converter.GetString(row, "id_venta_detalle")); /* 0: */
                item.Add(Converter.GetString(row, "VE_C")); /* 1: */
                item.Add(Converter.GetString(row, "RESPONSABLE_COMERCIAL")); /* 2: */

                item.Add(Converter.GetString(row, "CIUDAD")); /* 3: */
                item.Add(Converter.GetString(row, "COD_GRUPO")); /* 4: */
                item.Add(Converter.GetString(row, "GRUPO")); /* 5: */
                item.Add(Converter.GetString(row, "DOC_CLIENTE")); /* 6: */
                item.Add(Converter.GetString(row, "CLIENTE")); /* 7: */
                item.Add(Converter.GetString(row, "COD_CLIENTE")); /* 8: */
                item.Add(Converter.GetString(row, "TIPO")); /* 9: */
                item.Add(Converter.GetString(row, "FECHA_TRANSACCION")); /* 10: */
                item.Add(Converter.GetString(row, "NUMERO_PEDIDO")); /* 11: */
                item.Add(Converter.GetString(row, "NUMERO_GRUPO_PEDIDO")); /* 12: */
                item.Add(Converter.GetString(row, "GUIA")); /* 13: */
                item.Add(Converter.GetString(row, "FECHA_EMISION_GUIA")); /* 14: */
                item.Add(Converter.GetString(row, "FAMILIA_PROD")); /* 15: */
                item.Add(Converter.GetString(row, "PROV")); /* 16: */
                item.Add(Converter.GetString(row, "SKU_MP")); /* 17: */
                item.Add(Converter.GetString(row, "SKU_PROV")); /* 18: */
                item.Add(Converter.GetString(row, "DESCRIPCION_PROD")); /* 19: */
                item.Add(Converter.GetString(row, "UNIDAD_VENTA")); /* 20: */
                item.Add(Converter.GetString(row, "CANTIDAD")); /* 21: */

                Decimal valUnit = Converter.GetDecimal(row, "VALOR_UNIT");
                Decimal subtotal = Converter.GetDecimal(row, "SUBTOTAL");
                Decimal costoUnit = Converter.GetDecimal(row, "COSTO_UNIT");
                Decimal costoEspecial = Converter.GetDecimal(row, "COSTO_ESPECIAL");
                Decimal mkUp = Converter.GetDecimal(row, "MK_UP%");
                Decimal gpS = Converter.GetDecimal(row, "GP_S/");
                Decimal gpP = Converter.GetDecimal(row, "GP_%");

                item.Add(String.Format(Constantes.formatoCuatroDecimales, valUnit)); /* 22: */
                item.Add(String.Format(Constantes.formatoDosDecimales, subtotal)); /* 23: */
                item.Add(String.Format(Constantes.formatoCuatroDecimales, costoUnit)); /* 24: */
                item.Add(String.Format(Constantes.formatoCuatroDecimales, mkUp)); /* 25: */
                item.Add(String.Format(Constantes.formatoCuatroDecimales, gpS)); /* 26: */
                item.Add(String.Format(Constantes.formatoCuatroDecimales, gpP)); /* 27: */

                item.Add(Converter.GetString(row, "SUP")); /* 28: */
                item.Add(Converter.GetString(row, "SUPERVISOR_COMERCIAL")); /* 29: */
                item.Add(Converter.GetString(row, "VE_A")); /* 30: */
                item.Add(Converter.GetString(row, "ASISTENTE_SERVICIO_CLIENTE")); /* 31: */
                item.Add(Converter.GetString(row, "PEDIDO_CREADO_POR")); /* 32: */


                item.Add(Converter.GetString(row, "UNIDAD_MP")); /* 33: */
                Decimal eqMP = Converter.GetDecimal(row, "EQUIV_MP");
                Decimal cantidadMP = Converter.GetDecimal(row, "CANT_UND_MP");
                item.Add(String.Format(Constantes.formatoDosDecimales, eqMP)); /* 34: */
                item.Add(String.Format(Constantes.formatoDosDecimales, cantidadMP)); /* 35: */

                item.Add(Converter.GetString(row, "UNIDAD_PROV")); /* 36: */
                Decimal eqProv = Converter.GetDecimal(row, "EQUIV_PROV");
                Decimal cantidadProv = Converter.GetDecimal(row, "CANT_UND_PROV");
                item.Add(String.Format(Constantes.formatoDosDecimales, eqProv)); /* 37: */
                item.Add(String.Format(Constantes.formatoDosDecimales, cantidadProv)); /* 38: */

                item.Add(Converter.GetString(row, "X")); /* 39: */

                item.Add(Converter.GetString(row, "codigo_empresa")); /* 40: */
                item.Add(Converter.GetString(row, "nombre_empresa")); /* 41: */

                item.Add(Converter.GetString(row, "NUMERO_PEDREL")); /* 42: */

                item.Add(Converter.GetString(row, "TIPO_CPE")); /* 43: */
                item.Add(Converter.GetString(row, "CPE")); /* 44: */
                item.Add(Converter.GetString(row, "FECHA_EMISION_CPE")); /* 45: */

                item.Add(String.Format(Constantes.formatoCuatroDecimales, costoEspecial)); /* 46: */


                resultados.Add(item);
            }

            return resultados;
        }
    }
}

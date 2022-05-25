using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;
using Model.UTILES;

namespace DataLayer
{
    public class CierreStockDAL : DaoBase
    {
        public CierreStockDAL(IDalSettings settings) : base(settings)
        {
        }

        public CierreStockDAL() : this(new CotizadorSettings())
        {
        }

        public CierreStock SelectCierreStock(Guid idUsuario, Guid idCierreStock)
        {
            var objCommand = GetSqlCommand("ps_cierre_stock");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            InputParameterAdd.Guid(objCommand, "idCierreStock", idCierreStock);

            CierreStock obj = new CierreStock();
            obj.detalles = new List<RegistroCargaStock>();

            DataSet dataSet = ExecuteDataSet(objCommand);

            DataTable dataTableCierre = dataSet.Tables[0];
            DataTable dataTableDetalles = dataSet.Tables[1];


            foreach (DataRow row in dataTableCierre.Rows)
            {
                obj.idCierreStock = Converter.GetGuid(row, "id_cierre_stock");
                obj.fecha = Converter.GetDateTime(row, "fecha");

                obj.ciudad = new Ciudad();
                obj.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                obj.ciudad.nombre = Converter.GetString(row, "nombre_ciudad");

                obj.UsuarioRegistro = new Usuario();
                obj.UsuarioRegistro.idUsuario = Converter.GetGuid(row, "usuario_creacion");
                obj.UsuarioRegistro.nombre = Converter.GetString(row, "nombre_usuario_creacion");

                obj.UsuarioReporteValidacion = new Usuario();
                obj.UsuarioReporteValidacion.idUsuario = Converter.GetGuid(row, "id_usuario_reporte_validacion");
                obj.UsuarioReporteValidacion.nombre = Converter.GetString(row, "nombre_usuario_reporte_validacion");

                obj.fechaReporteValidacion = Converter.GetDateTime(row, "fecha_reporte_validacion");
                obj.FechaRegistro = Converter.GetDateTime(row, "fecha_creacion");

                obj.archivo = new ArchivoAdjunto();
                obj.archivo.idArchivoAdjunto = Converter.GetGuid(row, "id_archivo_adjunto");
                obj.archivo.nombre = Converter.GetString(row, "nombre_archivo");

                obj.Estado = Converter.GetInt(row, "estado");

                Guid idAjustaFaltante = Converter.GetGuid(row, "id_ajuste_faltante");
                Guid idAjusteExcedente = Converter.GetGuid(row, "id_ajuste_excedente");

                if (idAjustaFaltante != null && idAjustaFaltante != Guid.Empty)
                {
                    obj.ajusteFaltante = new GuiaRemision();
                    obj.ajusteFaltante.idMovimientoAlmacen = idAjustaFaltante;
                }

                if (idAjusteExcedente != null && idAjusteExcedente != Guid.Empty)
                {
                    obj.ajusteExcedente = new GuiaRemision();
                    obj.ajusteExcedente.idMovimientoAlmacen = idAjusteExcedente;
                }

            }

            foreach (DataRow row in dataTableDetalles.Rows)
            {
                RegistroCargaStock item = new RegistroCargaStock();
                item.producto = new Producto();
                item.ciudad = new Ciudad();
                item.producto.idProducto = Converter.GetGuid(row, "id_producto");
                item.producto.sku = Converter.GetString(row, "sku");
                item.producto.skuProveedor = Converter.GetString(row, "sku_proveedor");
                item.producto.descripcion = Converter.GetString(row, "descripcion");
                item.producto.proveedor = Converter.GetString(row, "proveedor");
                item.producto.unidadConteo = Converter.GetString(row, "unidad_conteo");
                item.producto.equivalenciaUnidadEstandarUnidadConteo = Converter.GetInt(row, "equivalencia_mp_conteo");
                item.diferenciaCantidadValidacion = Converter.GetInt(row, "diferencia_validacion");
                item.stockValidable = Converter.GetInt(row, "stock_validable");

                item.producto.unidad = Converter.GetString(row, "unidad_mp");
                item.producto.unidad_alternativa = Converter.GetString(row, "unidad_alternativa");
                item.producto.unidadProveedor = Converter.GetString(row, "unidad_proveedor");

                item.producto.equivalenciaAlternativa = Converter.GetInt(row, "equivalencia_alternativa");
                item.producto.equivalenciaProveedor = Converter.GetInt(row, "equivalencia_proveedor");

                item.cantidadAlternativa = Converter.GetInt(row, "cantidad_unidad_alternativa");
                item.cantidadMp = Converter.GetInt(row, "cantidad_unidad_mp");
                item.cantidadProveedor = Converter.GetInt(row, "cantidad_unidad_proveedor");


                item.cantidadConteo = Converter.GetInt(row, "cantidad_unidad_conteo");
                item.estado = Converter.GetInt(row, "estado");

                obj.detalles.Add(item);
            }

            return obj;
        }

        public void GenerarReporteValidacionStock(Guid idUsuario, Guid idCierreStock)
        {
            var objCommand = GetSqlCommand("pu_reporte_validacion_cierre_stock");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            InputParameterAdd.Guid(objCommand, "idCierreStock", idCierreStock);
            ExecuteNonQuery(objCommand);
        }
    }
}

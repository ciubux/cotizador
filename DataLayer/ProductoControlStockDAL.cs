using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Remoting;

namespace DataLayer
{
    public class ProductoControlStockDAL : DaoBase
    {
        public ProductoControlStockDAL(IDalSettings settings) : base(settings)
        {
        }

        public ProductoControlStockDAL() : this(new CotizadorSettings())
        {
        }

        public List<ProductoControlStock> SelectProductosControlStock(Guid idUsuario, int estado, Guid idCiudad, string sku, string proveedor, bool stockVerde, bool stockAmbar, bool stockRojo)
        {
            var objCommand = GetSqlCommand("ps_productosControlStock");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            InputParameterAdd.Int(objCommand, "estado", estado);

            InputParameterAdd.VarcharEmpty(objCommand, "sku", sku);
            InputParameterAdd.VarcharEmpty(objCommand, "proveedor", proveedor);

            InputParameterAdd.Int(objCommand, "contarStockNormal", stockVerde ? 1 : 0);
            InputParameterAdd.Int(objCommand, "contarStockAmbar", stockAmbar ? 1 : 0);
            InputParameterAdd.Int(objCommand, "contarStockRojo", stockRojo ? 1 : 0);


            if (!idCiudad.Equals(Guid.Empty))
            {
                InputParameterAdd.Guid(objCommand, "idCiudad", idCiudad);
            }

            DataTable dataTable = Execute(objCommand);
            List<ProductoControlStock> lista = new List<ProductoControlStock>();

            foreach (DataRow row in dataTable.Rows)
            {
                ProductoControlStock obj = new ProductoControlStock();
                obj.idProductoControlStock = Converter.GetGuid(row, "id_producto_control_stock");
                obj.cantidadMinima = Converter.GetInt(row, "cantidad_minima");
                obj.cantidadAlerta = Converter.GetInt(row, "cantidad_alerta");
                obj.cantidadMaxima = Converter.GetInt(row, "cantidad_maxima");

                obj.producto = new Producto();
                obj.producto.idProducto = Converter.GetGuid(row, "id_producto");
                obj.producto.sku = Converter.GetString(row, "sku");
                obj.producto.descripcion = Converter.GetString(row, "descripcion");
                obj.producto.unidadConteo = Converter.GetString(row, "unidad_conteo");

                obj.producto.equivalenciaAlternativa = Converter.GetInt(row, "equivalencia");
                obj.producto.equivalenciaProveedor = Converter.GetInt(row, "equivalencia_proveedor");
                obj.producto.equivalenciaUnidadEstandarUnidadConteo = Converter.GetInt(row, "equivalencia_unidad_estandar_unidad_conteo");
                obj.producto.equivalenciaUnidadAlternativaUnidadConteo = obj.producto.equivalenciaUnidadEstandarUnidadConteo / obj.producto.equivalenciaAlternativa;
                obj.producto.equivalenciaUnidadProveedorUnidadConteo = obj.producto.equivalenciaUnidadEstandarUnidadConteo * obj.producto.equivalenciaProveedor;

                obj.producto.unidad = Converter.GetString(row, "unidad");
                obj.producto.unidad_alternativa = Converter.GetString(row, "unidad_alternativa");
                obj.producto.unidadProveedor = Converter.GetString(row, "unidad_proveedor");

                obj.fechaCs = Converter.GetDateTime(row, "fecha_cs");
                obj.unidadCs = Converter.GetString(row, "unidad_cs");
                obj.cantidadCs = Converter.GetInt(row, "cantidad_cs");
                obj.cantidadAtenderCs = Converter.GetInt(row, "cantidad_atender_cs");
                obj.cantidadRecibirCs = Converter.GetInt(row, "cantidad_recibir_cs");
                obj.idProductoPresentacion = 3;

                obj.Estado = Converter.GetInt(row, "estado");

                lista.Add(obj);
            }

            return lista;
        }

        public void CalcularControlStock(Guid idUsuario, Guid idCiudad, List<Guid> controlStockIds)
        {
            var objCommand = GetSqlCommand("pu_stock_calcular_control_productos");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);

            if (!idCiudad.Equals(Guid.Empty))
            {
                InputParameterAdd.Guid(objCommand, "idCiudad", idCiudad);
            }

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("ID", typeof(Guid)));

            foreach (Guid item in controlStockIds)
            {
                DataRow rowObj = tvp.NewRow();
                rowObj["ID"] = item;
                tvp.Rows.Add(rowObj);
            }

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@idsControlStock", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.UniqueIdentifierList";

            ExecuteNonQuery(objCommand);
        }
    }
}

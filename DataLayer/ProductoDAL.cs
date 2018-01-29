using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;
using System.IO;

namespace DataLayer
{
    public class ProductoDAL : DaoBase
    {
        public ProductoDAL(IDalSettings settings) : base(settings)
        {
        }

        public ProductoDAL() : this(new CotizadorSettings())
        {
        }

        public void setProductoStaging(ProductoStaging productoStaging)
        {

            var objCommand = GetSqlCommand("pi_productoStaging");
            InputParameterAdd.Varchar(objCommand, "codigo", productoStaging.codigo); //(y)
            InputParameterAdd.Varchar(objCommand, "unidad", productoStaging.unidad); //(y)
            InputParameterAdd.Varchar(objCommand, "descripcion", productoStaging.descripcion); //(y)
            InputParameterAdd.Varchar(objCommand, "codigoProveedor", productoStaging.codigoProveedor); //(y)
            InputParameterAdd.Varchar(objCommand, "unidadAlternativa", productoStaging.unidadAlternativa); //(y)
            InputParameterAdd.Decimal(objCommand, "equivalencia", productoStaging.equivalencia); //(y)
            InputParameterAdd.Decimal(objCommand, "precioLima", productoStaging.precioLima); //(y)
            InputParameterAdd.Decimal(objCommand, "precioProvincias", productoStaging.precioProvincias); //(y)
            InputParameterAdd.Varchar(objCommand, "proveedor", productoStaging.proveedor); //(y)
            InputParameterAdd.Decimal(objCommand, "costo", productoStaging.costo); //(y)
            ExecuteNonQuery(objCommand);
        }




        public List<Producto> getProductosBusqueda(String textoBusqueda)
        {
            var objCommand = GetSqlCommand("ps_getproductos_search");
            InputParameterAdd.Varchar(objCommand, "textoBusqueda", textoBusqueda);
            DataTable dataTable = Execute(objCommand);
            List<Producto> lista = new List<Producto>();

            foreach (DataRow row in dataTable.Rows)
            {
                Producto obj = new Producto
                {
                    idProducto = Converter.GetGuid(row, "id_producto"),
                    descripcion = Converter.GetString(row, "descripcion"),
                    sku = Converter.GetString(row, "sku")
                };
                lista.Add(obj);
            }
            return lista;
        }

        public Producto getProducto(Guid idProducto)
        {
            var objCommand = GetSqlCommand("ps_getproducto");
            InputParameterAdd.Guid(objCommand, "idProducto", idProducto);
            DataTable dataTable = Execute(objCommand);
            Producto producto = new Producto();
            foreach (DataRow row in dataTable.Rows)
            {
                producto.idProducto = Converter.GetGuid(row, "id_producto");
                producto.descripcion = Converter.GetString(row, "descripcion");
                producto.sku = Converter.GetString(row, "sku");

                if (row["imagen"] != DBNull.Value)
                {
                    producto.image = (Byte[])(row["imagen"]);
                }

                producto.precioSinIgv = Converter.GetDecimal(row, "precio");
                producto.precioProvinciaSinIgv = Converter.GetDecimal(row, "precio_provincia");
                producto.familia = Converter.GetString(row, "familia");
                producto.clase = Converter.GetString(row, "clase");
                producto.proveedor = Converter.GetString(row, "proveedor");
                producto.unidad = Converter.GetString(row, "unidad");
                producto.unidad_alternativa = Converter.GetString(row, "unidad_alternativa");
                producto.equivalencia = Converter.GetInt(row, "equivalencia");
                producto.skuProveedor = Converter.GetString(row, "sku_proveedor");
                //Costo sin IGV
                producto.costoSinIgv = Converter.GetDecimal(row, "costo");
            }
            return producto;
        }
    }
}

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
            InputParameterAdd.Varchar(objCommand, "familia", productoStaging.familia); //(y)
            InputParameterAdd.Varchar(objCommand, "unidad", productoStaging.unidad); //(y)
            InputParameterAdd.Varchar(objCommand, "descripcion", productoStaging.descripcion); //(y)
            InputParameterAdd.Varchar(objCommand, "codigoProveedor", productoStaging.codigoProveedor); //(y)
            InputParameterAdd.Varchar(objCommand, "unidadAlternativa", productoStaging.unidadAlternativa); //(y)
            InputParameterAdd.Decimal(objCommand, "equivalencia", productoStaging.equivalencia); //(y)
            InputParameterAdd.Decimal(objCommand, "precioLima", productoStaging.precioLima); //(y)
            InputParameterAdd.Decimal(objCommand, "precioProvincias", productoStaging.precioProvincias); //(y)
            InputParameterAdd.Varchar(objCommand, "proveedor", productoStaging.proveedor); //(y)
            InputParameterAdd.Decimal(objCommand, "costo", productoStaging.costo); //(y)
            InputParameterAdd.Varchar(objCommand, "unidadProveedor", productoStaging.unidadProveedor); //(y)
            InputParameterAdd.Int(objCommand, "equivalenciaProveedor", productoStaging.equivalenciaProveedor); //(y)
            InputParameterAdd.Varchar(objCommand, "monedaProveedor", productoStaging.monedaProveedor); //(y)
            InputParameterAdd.Varchar(objCommand, "monedaMP", productoStaging.monedaMP); //(y)
            ExecuteNonQuery(objCommand);
        }

        public void truncateProductoStaging()
        {
            var objCommand = GetSqlCommand("pt_productoStaging");
            ExecuteNonQuery(objCommand);
        }

        public void mergeProductoStaging()
        {
            var objCommand = GetSqlCommand("pu_productoStaging");
            ExecuteNonQuery(objCommand);
        }


        public List<Producto> getProductosBusqueda(String textoBusqueda,bool considerarDescontinuados, String proveedor, String familia)
        {
            var objCommand = GetSqlCommand("ps_getproductos_search");
            InputParameterAdd.Varchar(objCommand, "textoBusqueda", textoBusqueda);
            InputParameterAdd.Varchar(objCommand, "proveedor", proveedor);
            InputParameterAdd.Varchar(objCommand, "familia", familia);
            InputParameterAdd.Int(objCommand, "considerarDescontinuados", considerarDescontinuados ? 1 : 0);
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

        public Producto getProducto(Guid idProducto, Guid idCliente)
        {
            var objCommand = GetSqlCommand("ps_getproducto");
            InputParameterAdd.Guid(objCommand, "idProducto", idProducto);
            InputParameterAdd.Guid(objCommand, "idCliente", idCliente);
            DataSet dataSet = ExecuteDataSet(objCommand);

            DataTable productoDataSet = dataSet.Tables[0];
            DataTable preciosDataSet = dataSet.Tables[1];


            Producto producto = new Producto();
            foreach (DataRow row in productoDataSet.Rows)
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


                /*Obtenido a partir de precio Lista*/
                if (row["precio_neto"] == DBNull.Value)
                {
                    producto.precioNeto = null;
                }
                else
                { 
                    producto.precioNeto = Converter.GetDecimal(row, "precio_neto");
                }
            }

            List<PrecioClienteProducto> precioListaList = new List<PrecioClienteProducto>();
            foreach (DataRow row in preciosDataSet.Rows)
            {
                PrecioClienteProducto precioLista = new PrecioClienteProducto();

                //     producto.idProducto = Converter.GetGuid(row, "unidad");

                if (row["fecha_inicio_vigencia"] == DBNull.Value)
                {
                    precioLista.fechaInicioVigencia = null;
                }
                else
                { 
                    precioLista.fechaInicioVigencia = Converter.GetDateTime(row, "fecha_inicio_vigencia");
                }

                if (row["fecha_fin_vigencia"] == DBNull.Value)
                {
                    precioLista.fechaFinVigencia = null;
                }
                else
                {
                    precioLista.fechaFinVigencia = Converter.GetDateTime(row, "fecha_fin_vigencia");
                }

                precioLista.unidad = Converter.GetString(row, "unidad");
                precioLista.precioNeto = Converter.GetDecimal(row, "precio_neto");
                precioLista.flete = Converter.GetDecimal(row, "flete");
                precioLista.precioUnitario = Converter.GetDecimal(row, "precio_unitario");
                if (row["numero_cotizacion"] == DBNull.Value)
                {
                    precioLista.numeroCotizacion = null;
                }
                else
                {
                    precioLista.numeroCotizacion = Converter.GetInt(row, "numero_cotizacion").ToString().PadLeft(10, '0');
                }
              
                precioListaList.Add(precioLista);
            }

            producto.precioListaList = precioListaList;


            return producto;
        }

      
    }
}

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
            InputParameterAdd.Varchar(objCommand, "unidadSunat", productoStaging.unidadSunat); //(y)
            InputParameterAdd.Varchar(objCommand, "unidadAlternativaSunat", productoStaging.unidadAlternativaSunat); //(y)
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


        public Guid getProductoId(String sku)
        {
            var objCommand = GetSqlCommand("ps_productoId");
            InputParameterAdd.Varchar(objCommand, "sku", sku);
            DataTable dataTable = Execute(objCommand);

            Guid idProducto = Guid.Empty;
            foreach (DataRow row in dataTable.Rows)
            {
                idProducto = Converter.GetGuid(row, "id_producto");
            }
            return idProducto;
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

                if (row["imagen"] != DBNull.Value){producto.image = (Byte[])(row["imagen"]);
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
                producto.precioClienteProducto = new PrecioClienteProducto();
                producto.precioClienteProducto.idPrecioClienteProducto = Guid.Empty;

                /*Obtenido a partir de precio Lista*/
                if (row["precio_neto"] != DBNull.Value)
                {
                    producto.precioClienteProducto.precioNeto = Converter.GetDecimal(row, "precio_neto");
                    producto.precioClienteProducto.flete = Converter.GetDecimal(row, "flete");
                    producto.precioClienteProducto.equivalencia = Converter.GetInt(row, "equivalencia");
                    producto.precioClienteProducto.precioUnitario = Converter.GetDecimal(row, "precio_unitario");
                    producto.precioClienteProducto.idPrecioClienteProducto = Converter.GetGuid(row, "id_precio_cliente_producto");
                    producto.precioClienteProducto.fechaInicioVigencia = Converter.GetDateTime(row, "fecha_inicio_vigencia");

                    if (row["fecha_fin_vigencia"] == DBNull.Value)
                    {
                        producto.precioClienteProducto.fechaFinVigencia = null;
                    }
                    else
                    {
                        producto.precioClienteProducto.fechaFinVigencia = Converter.GetDateTime(row, "fecha_fin_vigencia");
                    }
                //    producto.precioClienteProducto.fechaFinVigencia = Converter.GetDateTime(row, "fecha_fin_vigencia");
                    producto.precioClienteProducto.cliente = new Cliente();
                    producto.precioClienteProducto.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                }
            }

            List<PrecioClienteProducto> precioListaList = new List<PrecioClienteProducto>();
            foreach (DataRow row in preciosDataSet.Rows)
            {
                PrecioClienteProducto precioLista = new PrecioClienteProducto();

                //     producto.idProducto = Converter.GetGuid(row, "unidad");

                precioLista.idPrecioClienteProducto = Converter.GetGuid(row, "id_precio_cliente_producto");


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

        public List<DocumentoDetalle> obtenerProductosAPartirdePreciosRegistrados(Guid idCliente, DateTime fechaPrecios, String familia, String proveedor)
        {
            var objCommand = GetSqlCommand("ps_generarPlantillaCotizacion");
            InputParameterAdd.Guid(objCommand, "idCliente", idCliente);
            InputParameterAdd.DateTime(objCommand, "fecha", fechaPrecios);
            InputParameterAdd.Varchar(objCommand, "familia", familia);
            InputParameterAdd.Varchar(objCommand, "proveedor", proveedor);
            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable cotizacionDataTable = dataSet.Tables[0];
            DataTable cotizacionDetalleDataTable = dataSet.Tables[1];


            List<DocumentoDetalle> documentoDetalleList = new List<DocumentoDetalle>();
            //Detalle de la cotizacion
            foreach (DataRow row in cotizacionDetalleDataTable.Rows)
            {
                DocumentoDetalle cotizacionDetalle = new DocumentoDetalle();
                cotizacionDetalle.producto = new Producto();


                //No se cuenta con IdCotizacionDetalle
                cotizacionDetalle.cantidad = 1;
                cotizacionDetalle.producto.idProducto = Converter.GetGuid(row, "id_producto");
                cotizacionDetalle.producto.sku = Converter.GetString(row, "sku");
                cotizacionDetalle.producto.descripcion = Converter.GetString(row, "descripcion");
                cotizacionDetalle.producto.skuProveedor = Converter.GetString(row, "sku_proveedor");
                cotizacionDetalle.producto.proveedor = Converter.GetString(row, "proveedor");
                cotizacionDetalle.producto.image = Converter.GetBytes(row, "imagen");

                cotizacionDetalle.producto.precioSinIgv = Converter.GetDecimal(row, "precio_sin_igv");
                cotizacionDetalle.producto.precioProvinciaSinIgv = Converter.GetDecimal(row, "precio_provincia_sin_igv");
                cotizacionDetalle.producto.costoSinIgv = Converter.GetDecimal(row, "costo_sin_igv");

                //El Precio sin igv es el precio lista
                cotizacionDetalle.producto.precioLista = cotizacionDetalle.producto.precioSinIgv;


                cotizacionDetalle.producto.equivalencia = Convert.ToInt32(Converter.GetDecimal(row, "equivalencia"));
                cotizacionDetalle.esPrecioAlternativo = Converter.GetBool(row, "es_precio_alternativo");
                cotizacionDetalle.unidad = Converter.GetString(row, "unidad");


                cotizacionDetalle.producto.precioClienteProducto = new PrecioClienteProducto();
                cotizacionDetalle.producto.precioClienteProducto.precioNeto = Converter.GetDecimal(row, "precio_neto");
                cotizacionDetalle.producto.precioClienteProducto.precioUnitario = Converter.GetDecimal(row, "precio_unitario");
                cotizacionDetalle.producto.precioClienteProducto.idPrecioClienteProducto = Converter.GetGuid(row, "id_precio_cliente_producto");
                cotizacionDetalle.producto.precioClienteProducto.fechaInicioVigencia = Converter.GetDateTime(row, "fecha_inicio_vigencia");


                if (row["fecha_fin_vigencia"] == DBNull.Value)
                    cotizacionDetalle.producto.precioClienteProducto.fechaFinVigencia = null;
                else
                    cotizacionDetalle.producto.precioClienteProducto.fechaFinVigencia = Converter.GetDateTime(row, "fecha_fin_vigencia");

                cotizacionDetalle.producto.precioClienteProducto.equivalencia = Converter.GetInt(row, "equivalencia");
                cotizacionDetalle.producto.precioClienteProducto.cliente = new Cliente();
                cotizacionDetalle.producto.precioClienteProducto.cliente.idCliente = Converter.GetGuid(row, "id_cliente");


                //   cotizacionDetalle.porcentajeDescuento = Decimal.Parse(Request["porcentajeDescuento"].ToString());
                ///   detalle.esPrecioAlternativo = Int16.Parse(Request["esPrecioAlternativo"].ToString()) == 1;

                //cotizacionDetalle.porcentajeDescuento = cotizacionDetalle.producto.precioClienteProducto.precioNeto
                //    75×25 / 100 = 18,75

               // cotizacionDetalle

                if (cotizacionDetalle.esPrecioAlternativo)
                {
                    

                    //Si es el precio Alternativo se multiplica por la equivalencia para que se registre el precio estandar
                    //dado que cuando se hace get al precioNetoEquivalente se recupera diviendo entre la equivalencia
                    cotizacionDetalle.precioNeto = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cotizacionDetalle.producto.precioClienteProducto.precioNeto * cotizacionDetalle.producto.equivalencia));
                }
                else
                {
                    cotizacionDetalle.precioNeto = cotizacionDetalle.producto.precioClienteProducto.precioNeto;
                }


                //Se multiplica por la equivalente para que cuando se haga el recalculo a la hora de obtener el precioneto se recupere correctamente
           //     cotizacionDetalle.precioNeto = cotizacionDetalle.producto.precioClienteProducto.precioNeto * cotizacionDetalle.producto.precioClienteProducto.equivalencia;
                cotizacionDetalle.flete = Converter.GetDecimal(row, "flete");

                cotizacionDetalle.observacion = null;

                documentoDetalleList.Add(cotizacionDetalle);
            }

            return documentoDetalleList;
        }


    }
}

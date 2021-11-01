using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Model;
using Model.UTILES;
using System.IO;
using System.Linq;

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
            InputParameterAdd.Varchar(objCommand, "unidadProveedorSunat", productoStaging.unidadProveedorSunat); //(y)
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

        public List<Producto> getProductosBusqueda(String textoBusqueda, bool considerarDescontinuados, String proveedor, String familia, Pedido.tiposPedido? tipoPedido = null, int incluyeDescontinuados = 1)
        {
            var objCommand = GetSqlCommand("ps_getproductos_search");
            InputParameterAdd.Varchar(objCommand, "textoBusqueda", textoBusqueda);
            InputParameterAdd.Varchar(objCommand, "proveedor", proveedor);
            InputParameterAdd.Varchar(objCommand, "familia", familia);
            InputParameterAdd.Int(objCommand, "considerarDescontinuados", considerarDescontinuados ? 1 : 0);
            InputParameterAdd.Int(objCommand, "incluyeDescontinuados", incluyeDescontinuados);
            InputParameterAdd.Char(objCommand, "tipoPedido", tipoPedido == null ? null : ((Char)tipoPedido).ToString());

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

        public Guid getAllProductoId(String sku)
        {
            var objCommand = GetSqlCommand("ps_allProductoId");
            InputParameterAdd.Varchar(objCommand, "sku", sku);
            DataTable dataTable = Execute(objCommand);

            Guid idProducto = Guid.Empty;
            foreach (DataRow row in dataTable.Rows)
            {
                idProducto = Converter.GetGuid(row, "id_producto");
            }
            return idProducto;
        }

        public Producto getProducto(Guid idProducto, Guid idCliente, Boolean esCompra = false, Boolean soloActivos = true)
        {
            var objCommand = GetSqlCommand("ps_getproducto");
            InputParameterAdd.Guid(objCommand, "idProducto", idProducto);
            InputParameterAdd.Guid(objCommand, "idCliente", idCliente);
            DataSet dataSet = ExecuteDataSet(objCommand);

            DataTable productoDataSet = dataSet.Tables[0];
            DataTable preciosDataSet = dataSet.Tables[1];
            DataTable productoPresentacionDataSet = dataSet.Tables[2];


            Producto producto = new Producto();
            foreach (DataRow row in productoDataSet.Rows)
            {
                producto.idProducto = Converter.GetGuid(row, "id_producto");
                producto.descripcion = Converter.GetString(row, "descripcion");
                producto.descripcionLarga = Converter.GetString(row, "descripcion_larga");

                producto.sku = Converter.GetString(row, "sku");

                if (row["imagen"] != DBNull.Value)
                {
                    producto.image = (Byte[])(row["imagen"]);
                }

                producto.costoSinIgv = Converter.GetDecimal(row, "costo");
                producto.costoOriginal = Converter.GetDecimal(row, "costo_original");
                if (esCompra)
                {
                    producto.precioSinIgv = producto.costoSinIgv;
                    producto.precioProvinciaSinIgv = producto.costoSinIgv;
                    producto.precioOriginal = producto.costoOriginal;
                    producto.precioProvinciasOriginal = producto.costoOriginal;
                }
                else
                {
                    producto.precioSinIgv = Converter.GetDecimal(row, "precio");
                    producto.precioProvinciaSinIgv = Converter.GetDecimal(row, "precio_provincia");
                    producto.precioOriginal = Converter.GetDecimal(row, "precio_original"); ;
                    producto.precioProvinciasOriginal = Converter.GetDecimal(row, "precio_provincia_original");
                }

                producto.familia = Converter.GetString(row, "familia");
                // producto.clase = Converter.GetString(row, "clase");
                producto.proveedor = Converter.GetString(row, "proveedor");
                producto.unidad = Converter.GetString(row, "unidad");
                producto.unidad_alternativa = Converter.GetString(row, "unidad_alternativa");
                producto.unidadProveedor = Converter.GetString(row, "unidad_proveedor");
                producto.equivalenciaAlternativa = Converter.GetInt(row, "equivalencia");
                producto.equivalenciaProveedor = Converter.GetInt(row, "equivalencia_proveedor");
                producto.skuProveedor = Converter.GetString(row, "sku_proveedor");
                producto.monedaProveedor = Converter.GetString(row, "moneda_compra");
                producto.monedaMP = Converter.GetString(row, "moneda_venta");
                //Costo sin IGV

                producto.precioClienteProducto = new PrecioClienteProducto();
                producto.precioClienteProducto.idPrecioClienteProducto = Guid.Empty;
                producto.tipoProducto = (Producto.TipoProducto)Converter.GetInt(row, "tipo_producto");
                producto.agregarDescripcionCotizacion = Converter.GetInt(row, "agregar_descripcion_cotizacion");

                producto.ventaRestringida = (Producto.TipoVentaRestringida)Converter.GetInt(row, "descontinuado");
                producto.motivoRestriccion = Converter.GetString(row, "motivo_restriccion");
                producto.cantidadMaximaPedidoRestringido = Converter.GetInt(row, "cantidad_maxima_pedido_restringido");
                producto.compraRestringida = Converter.GetInt(row, "compra_restringida");
                producto.exoneradoIgv = Converter.GetInt(row, "exonerado_igv") == 1 ? true : false;
                producto.inafecto = Converter.GetInt(row, "inafecto") == 1 ? true : false;

                producto.Stock = Converter.GetInt(row, "stock");
                producto.topeDescuento = Converter.GetDecimal(row, "tope_descuento");

                /*Obtenido a partir de precio Lista*/
                if (row["precio_neto"] != DBNull.Value && !esCompra)
                {
                    producto.precioClienteProducto.precioNeto = Converter.GetDecimal(row, "precio_neto");
                    producto.precioClienteProducto.flete = Converter.GetDecimal(row, "flete");
                    producto.precioClienteProducto.equivalencia = Converter.GetDecimal(row, "precio_cliente_producto_equivalencia");
                    //                    producto.equivalencia = producto.precioClienteProducto.equivalencia;
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

                if (esCompra)
                {
                    precioLista.precioNeto = producto.costoSinIgv;
                    precioLista.precioUnitario = producto.costoSinIgv;
                }

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


            foreach (DataRow row in productoPresentacionDataSet.Rows)
            {
                ProductoPresentacion productoPresentacion = new ProductoPresentacion();
                productoPresentacion.IdProductoPresentacion = Converter.GetInt(row, "idProductoPresentacion");
                productoPresentacion.Equivalencia = Converter.GetDecimal(row, "equivalencia");
                productoPresentacion.Presentacion = Converter.GetString(row, "presentacion");
                productoPresentacion.PrecioLimaSinIGV = Converter.GetDecimal(row, "precio_lima_sin_igv");
                productoPresentacion.PrecioProvinciasSinIGV = Converter.GetDecimal(row, "precio_provincias_sin_igv");
                productoPresentacion.CostoSinIGV = Converter.GetDecimal(row, "costo_sin_igv");
                productoPresentacion.PrecioSinIGV = productoPresentacion.PrecioLimaSinIGV;
                productoPresentacion.PrecioLimaOriginalSinIGV = Converter.GetDecimal(row, "precio_lima_original_sin_igv");
                productoPresentacion.PrecioProvinciasOriginalSinIGV = Converter.GetDecimal(row, "precio_provincias_original_sin_igv");
                productoPresentacion.CostoOriginalSinIGV = Converter.GetDecimal(row, "costo_original_sin_igv");
                productoPresentacion.PrecioOriginalSinIGV = productoPresentacion.PrecioLimaOriginalSinIGV;
                productoPresentacion.UnidadInternacional = Converter.GetString(row, "unidad_internacional");
                productoPresentacion.Stock = Converter.GetInt(row, "stock");

                if (esCompra)
                {
                    productoPresentacion.PrecioLimaSinIGV = productoPresentacion.CostoSinIGV;
                    productoPresentacion.PrecioProvinciasSinIGV = productoPresentacion.CostoSinIGV;
                }
                producto.ProductoPresentacionList.Add(productoPresentacion);
            }

            return producto;
        }

        public List<CotizacionDetalle> obtenerProductosAPartirdePreciosRegistrados(Guid idCliente, DateTime fechaPrecios, String familia, String proveedor)
        {
            var objCommand = GetSqlCommand("ps_generarPlantillaCotizacion");
            InputParameterAdd.Guid(objCommand, "idCliente", idCliente);
            InputParameterAdd.DateTime(objCommand, "fecha", fechaPrecios);
            InputParameterAdd.Varchar(objCommand, "familia", familia);
            InputParameterAdd.Varchar(objCommand, "proveedor", proveedor);
            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable cotizacionDataTable = dataSet.Tables[0];
            DataTable cotizacionDetalleDataTable = dataSet.Tables[1];


            List<CotizacionDetalle> documentoDetalleList = new List<CotizacionDetalle>();
            //Detalle de la cotizacion
            foreach (DataRow row in cotizacionDetalleDataTable.Rows)
            {
                CotizacionDetalle cotizacionDetalle = new CotizacionDetalle(false, false);
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
                cotizacionDetalle.producto.precioListaAnterior = Converter.GetDecimal(row, "precio_sin_igv_anterior");

                cotizacionDetalle.esPrecioAlternativo = Converter.GetBool(row, "es_precio_alternativo");
                if (cotizacionDetalle.esPrecioAlternativo)
                {
                    cotizacionDetalle.ProductoPresentacion = new ProductoPresentacion();
                    cotizacionDetalle.ProductoPresentacion.Equivalencia = Converter.GetDecimal(row, "equivalencia");
                }



                cotizacionDetalle.unidad = Converter.GetString(row, "unidad");


                cotizacionDetalle.producto.precioClienteProducto = new PrecioClienteProducto();
                cotizacionDetalle.producto.precioClienteProducto.precioNeto = Converter.GetDecimal(row, "precio_neto");


                Decimal precioNetoRecalculado = Converter.GetDecimal(row, "precio_neto_recalculado");
                precioNetoRecalculado = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, precioNetoRecalculado));


                cotizacionDetalle.producto.precioClienteProducto.precioUnitario = Converter.GetDecimal(row, "precio_unitario");
                cotizacionDetalle.producto.precioClienteProducto.idPrecioClienteProducto = Converter.GetGuid(row, "id_precio_cliente_producto");
                cotizacionDetalle.producto.precioClienteProducto.fechaInicioVigencia = Converter.GetDateTime(row, "fecha_inicio_vigencia");
                cotizacionDetalle.producto.precioClienteProducto.flete = Converter.GetDecimal(row, "flete");
                //Revisar
                cotizacionDetalle.producto.costoListaAnterior = Converter.GetDecimal(row, "costo_sin_igv_anterior");
                cotizacionDetalle.precioNetoAnterior = Converter.GetDecimal(row, "precio_neto_anterior");


                if (row["fecha_fin_vigencia"] == DBNull.Value)
                    cotizacionDetalle.producto.precioClienteProducto.fechaFinVigencia = null;
                else
                    cotizacionDetalle.producto.precioClienteProducto.fechaFinVigencia = Converter.GetDateTime(row, "fecha_fin_vigencia");

                cotizacionDetalle.producto.precioClienteProducto.equivalencia = Converter.GetDecimal(row, "equivalencia");
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
                    //  cotizacionDetalle.precioNeto = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cotizacionDetalle.producto.precioClienteProducto.precioNeto * cotizacionDetalle.ProductoPresentacion.Equivalencia));
                    //  cotizacionDetalle.producto.precioClienteProducto.precioUnitario = cotizacionDetalle.producto.precioClienteProducto.precioUnitario * cotizacionDetalle.ProductoPresentacion.Equivalencia;


                    cotizacionDetalle.precioNeto = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cotizacionDetalle.producto.precioClienteProducto.precioNeto * cotizacionDetalle.producto.precioClienteProducto.equivalencia));
                    cotizacionDetalle.producto.precioClienteProducto.precioUnitario = cotizacionDetalle.producto.precioClienteProducto.precioUnitario * cotizacionDetalle.producto.precioClienteProducto.equivalencia;


                }
                else
                {
                    cotizacionDetalle.precioNeto = cotizacionDetalle.producto.precioClienteProducto.precioNeto;
                }


                //Se multiplica por la equivalente para que cuando se haga el recalculo a la hora de obtener el precioneto se recupere correctamente
                //     cotizacionDetalle.precioNeto = cotizacionDetalle.producto.precioClienteProducto.precioNeto * cotizacionDetalle.producto.precioClienteProducto.equivalencia;
                cotizacionDetalle.flete = Converter.GetDecimal(row, "flete");


                if (precioNetoRecalculado == cotizacionDetalle.precioNeto)
                {
                    cotizacionDetalle.observacion = "El precio lista NO ha variado.";
                }
                else
                {
                    cotizacionDetalle.observacion = "El precio de lista ha variado.";
                }

                if (cotizacionDetalle.producto.precioClienteProducto.fechaFinVigencia != null &&
                    cotizacionDetalle.producto.precioClienteProducto.fechaFinVigencia.Value > DateTime.Now)
                {
                    cotizacionDetalle.observacion = cotizacionDetalle.observacion + "\nSe mantiene precio antes cotizado con vigencia extendida hasta el " + cotizacionDetalle.producto.precioClienteProducto.fechaFinVigencia.Value.ToString(Constantes.formatoFecha);
                }
                else
                {
                    if (cotizacionDetalle.esPrecioAlternativo)
                    {
                        //Si es el precio Alternativo se multiplica por la equivalencia para que se registre el precio estandar
                        //dado que cuando se hace get al precioNetoEquivalente se recupera diviendo entre la equivalencia
                        cotizacionDetalle.precioNeto = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, precioNetoRecalculado * cotizacionDetalle.ProductoPresentacion.Equivalencia));

                    }
                    else
                    {
                        cotizacionDetalle.precioNeto = precioNetoRecalculado;
                    }
                }



                documentoDetalleList.Add(cotizacionDetalle);
            }

            return documentoDetalleList;
        }



        public List<DocumentoDetalle> obtenerProductosAPartirdePreciosRegistradosParaPedido(Guid idCliente, DateTime fechaPrecios, String familia, String proveedor)
        {
            var objCommand = GetSqlCommand("ps_generarPlantillaPedido");
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
                cotizacionDetalle.producto.ventaRestringida = (Producto.TipoVentaRestringida)Converter.GetInt(row, "descontinuado");
                cotizacionDetalle.producto.motivoRestriccion = Converter.GetString(row, "motivo_restriccion");
                cotizacionDetalle.producto.topeDescuento = Converter.GetDecimal(row, "tope_descuento");

                cotizacionDetalle.producto.costoSinIgv = Converter.GetDecimal(row, "costo_sin_igv");
                cotizacionDetalle.producto.precioSinIgv = Converter.GetDecimal(row, "precio_sin_igv");
                cotizacionDetalle.producto.precioProvinciaSinIgv = Converter.GetDecimal(row, "precio_provincia_sin_igv");


                //El Precio sin igv es el precio lista
                cotizacionDetalle.producto.precioLista = cotizacionDetalle.producto.precioSinIgv;



                cotizacionDetalle.esPrecioAlternativo = Converter.GetBool(row, "es_precio_alternativo");

                if (cotizacionDetalle.esPrecioAlternativo)
                {
                    cotizacionDetalle.ProductoPresentacion = new ProductoPresentacion();
                    cotizacionDetalle.ProductoPresentacion.Equivalencia = Converter.GetDecimal(row, "equivalencia");
                }

                cotizacionDetalle.unidad = Converter.GetString(row, "unidad");


                cotizacionDetalle.producto.precioClienteProducto = new PrecioClienteProducto();
                cotizacionDetalle.producto.precioClienteProducto.precioNeto = Converter.GetDecimal(row, "precio_neto");
                cotizacionDetalle.producto.precioClienteProducto.flete = Converter.GetDecimal(row, "flete");
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
                    cotizacionDetalle.precioNeto = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cotizacionDetalle.producto.precioClienteProducto.precioNeto * cotizacionDetalle.ProductoPresentacion.Equivalencia));
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

        public List<DocumentoDetalle> getPreciosVigentesCliente(Guid idCliente, DateTime? fechaPreciosVigenciaDesde = null)
        {
            var objCommand = GetSqlCommand("ps_productosVigentesCliente");
            InputParameterAdd.Guid(objCommand, "idCliente", idCliente);
            if (fechaPreciosVigenciaDesde != null)
            {
                InputParameterAdd.DateTime(objCommand, "fechaConsideracion", fechaPreciosVigenciaDesde.Value);
            }
            else
            {
                InputParameterAdd.DateTime(objCommand, "fechaConsideracion", DateTime.Now);
            }

            DataTable cotizacionDetalleDataTable = Execute(objCommand);

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
                cotizacionDetalle.producto.ventaRestringida = (Producto.TipoVentaRestringida)Converter.GetInt(row, "descontinuado");
                cotizacionDetalle.producto.motivoRestriccion = Converter.GetString(row, "motivo_restriccion");
                cotizacionDetalle.producto.topeDescuento = Converter.GetDecimal(row, "tope_descuento");

                cotizacionDetalle.producto.precioSinIgv = Converter.GetDecimal(row, "precio_sin_igv");
                cotizacionDetalle.producto.precioProvinciaSinIgv = Converter.GetDecimal(row, "precio_provincia_sin_igv");
                cotizacionDetalle.producto.costoSinIgv = Converter.GetDecimal(row, "costo_sin_igv");

                //El Precio sin igv es el precio lista
                cotizacionDetalle.producto.precioLista = cotizacionDetalle.producto.precioSinIgv;


                cotizacionDetalle.esPrecioAlternativo = Converter.GetBool(row, "es_precio_alternativo");
                cotizacionDetalle.unidad = Converter.GetString(row, "unidad");

                if (cotizacionDetalle.esPrecioAlternativo)
                {
                    cotizacionDetalle.ProductoPresentacion = new ProductoPresentacion();
                    cotizacionDetalle.ProductoPresentacion.IdProductoPresentacion = Converter.GetInt(row, "id_producto_presentacion");
                    cotizacionDetalle.ProductoPresentacion.Equivalencia = Converter.GetDecimal(row, "equivalencia");
                    cotizacionDetalle.precioNeto = cotizacionDetalle.producto.precioLista * cotizacionDetalle.ProductoPresentacion.Equivalencia;
                }


                cotizacionDetalle.producto.precioClienteProducto = new PrecioClienteProducto();
                cotizacionDetalle.producto.precioClienteProducto.precioNeto = Converter.GetDecimal(row, "precio_neto");
                cotizacionDetalle.producto.precioClienteProducto.precioUnitario = Converter.GetDecimal(row, "precio_unitario");
                cotizacionDetalle.producto.precioClienteProducto.idPrecioClienteProducto = Converter.GetGuid(row, "id_precio_cliente_producto");
                cotizacionDetalle.producto.precioClienteProducto.fechaInicioVigencia = Converter.GetDateTime(row, "fecha_inicio_vigencia");


                if (row["fecha_fin_vigencia"] == DBNull.Value)
                    cotizacionDetalle.producto.precioClienteProducto.fechaFinVigencia = null;
                else
                    cotizacionDetalle.producto.precioClienteProducto.fechaFinVigencia = Converter.GetDateTime(row, "fecha_fin_vigencia");

                cotizacionDetalle.producto.precioClienteProducto.equivalencia = Converter.GetDecimal(row, "equivalencia");
                cotizacionDetalle.producto.precioClienteProducto.cliente = new Cliente();
                cotizacionDetalle.producto.precioClienteProducto.estadoCanasta = Converter.GetInt(row, "estado_canasta") == 1 ? true : false;
                cotizacionDetalle.producto.precioClienteProducto.cliente.idCliente = Converter.GetGuid(row, "id_cliente");


                cotizacionDetalle.precioCliente = new PrecioClienteProducto();
                cotizacionDetalle.precioCliente.skuCliente = Converter.GetString(row, "sku_cliente");
                if (row["fecha_inicio_vigencia"] == DBNull.Value)
                {
                    cotizacionDetalle.precioCliente.fechaInicioVigencia = null;
                }
                else
                {
                    cotizacionDetalle.precioCliente.fechaInicioVigencia = Converter.GetDateTime(row, "fecha_inicio_vigencia");
                }

                if (row["fecha_fin_vigencia"] == DBNull.Value)
                {
                    cotizacionDetalle.precioCliente.fechaFinVigencia = null;
                }
                else
                {
                    cotizacionDetalle.precioCliente.fechaFinVigencia = Converter.GetDateTime(row, "fecha_fin_vigencia");
                }
                //   cotizacionDetalle.porcentajeDescuento = Decimal.Parse(Request["porcentajeDescuento"].ToString());
                ///   detalle.esPrecioAlternativo = Int16.Parse(Request["esPrecioAlternativo"].ToString()) == 1;

                //cotizacionDetalle.porcentajeDescuento = cotizacionDetalle.producto.precioClienteProducto.precioNeto
                //    75×25 / 100 = 18,75

                // cotizacionDetalle

                if (cotizacionDetalle.esPrecioAlternativo)
                {


                    //Si es el precio Alternativo se multiplica por la equivalencia para que se registre el precio estandar
                    //dado que cuando se hace get al precioNetoEquivalente se recupera diviendo entre la equivalencia
                    cotizacionDetalle.precioNeto = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cotizacionDetalle.producto.precioClienteProducto.precioNeto * cotizacionDetalle.ProductoPresentacion.Equivalencia));
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

        public List<DocumentoDetalle> getPreciosHistoricoCliente(Guid idCliente)
        {
            var objCommand = GetSqlCommand("ps_productosHistoricoCliente");
            InputParameterAdd.Guid(objCommand, "idCliente", idCliente);
            DataTable cotizacionDetalleDataTable = Execute(objCommand);

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
                cotizacionDetalle.producto.topeDescuento = Converter.GetDecimal(row, "tope_descuento");

                cotizacionDetalle.producto.precioSinIgv = Converter.GetDecimal(row, "precio_sin_igv");
                cotizacionDetalle.producto.precioProvinciaSinIgv = Converter.GetDecimal(row, "precio_provincia_sin_igv");
                cotizacionDetalle.producto.costoSinIgv = Converter.GetDecimal(row, "costo_sin_igv");

                //El Precio sin igv es el precio lista
                cotizacionDetalle.producto.precioLista = cotizacionDetalle.producto.precioSinIgv;


                cotizacionDetalle.esPrecioAlternativo = Converter.GetBool(row, "es_precio_alternativo");
                cotizacionDetalle.unidad = Converter.GetString(row, "unidad");

                if (cotizacionDetalle.esPrecioAlternativo)
                {
                    cotizacionDetalle.ProductoPresentacion = new ProductoPresentacion();
                    cotizacionDetalle.precioNeto = cotizacionDetalle.producto.precioLista * cotizacionDetalle.ProductoPresentacion.Equivalencia;
                    cotizacionDetalle.ProductoPresentacion.Equivalencia = Converter.GetDecimal(row, "equivalencia");
                }


                cotizacionDetalle.producto.precioClienteProducto = new PrecioClienteProducto();
                cotizacionDetalle.producto.precioClienteProducto.precioNeto = Converter.GetDecimal(row, "precio_neto");
                cotizacionDetalle.producto.precioClienteProducto.precioUnitario = Converter.GetDecimal(row, "precio_unitario");
                cotizacionDetalle.producto.precioClienteProducto.idPrecioClienteProducto = Converter.GetGuid(row, "id_precio_cliente_producto");
                cotizacionDetalle.producto.precioClienteProducto.fechaInicioVigencia = Converter.GetDateTime(row, "fecha_inicio_vigencia");


                if (row["fecha_fin_vigencia"] == DBNull.Value)
                    cotizacionDetalle.producto.precioClienteProducto.fechaFinVigencia = null;
                else
                    cotizacionDetalle.producto.precioClienteProducto.fechaFinVigencia = Converter.GetDateTime(row, "fecha_fin_vigencia");

                cotizacionDetalle.producto.precioClienteProducto.equivalencia = Converter.GetDecimal(row, "equivalencia");
                cotizacionDetalle.producto.precioClienteProducto.cliente = new Cliente();
                cotizacionDetalle.producto.precioClienteProducto.estadoCanasta = Converter.GetInt(row, "estado_canasta") == 1 ? true : false;
                cotizacionDetalle.producto.precioClienteProducto.cliente.idCliente = Converter.GetGuid(row, "id_cliente");


                cotizacionDetalle.precioCliente = new PrecioClienteProducto();
                if (row["fecha_inicio_vigencia"] == DBNull.Value)
                {
                    cotizacionDetalle.precioCliente.fechaInicioVigencia = null;
                }
                else
                {
                    cotizacionDetalle.precioCliente.fechaInicioVigencia = Converter.GetDateTime(row, "fecha_inicio_vigencia");
                }

                if (row["fecha_fin_vigencia"] == DBNull.Value)
                {
                    cotizacionDetalle.precioCliente.fechaFinVigencia = null;
                }
                else
                {
                    cotizacionDetalle.precioCliente.fechaFinVigencia = Converter.GetDateTime(row, "fecha_fin_vigencia");
                }
                //   cotizacionDetalle.porcentajeDescuento = Decimal.Parse(Request["porcentajeDescuento"].ToString());
                ///   detalle.esPrecioAlternativo = Int16.Parse(Request["esPrecioAlternativo"].ToString()) == 1;

                //cotizacionDetalle.porcentajeDescuento = cotizacionDetalle.producto.precioClienteProducto.precioNeto
                //    75×25 / 100 = 18,75

                // cotizacionDetalle

                if (cotizacionDetalle.esPrecioAlternativo)
                {


                    //Si es el precio Alternativo se multiplica por la equivalencia para que se registre el precio estandar
                    //dado que cuando se hace get al precioNetoEquivalente se recupera diviendo entre la equivalencia
                    cotizacionDetalle.precioNeto = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cotizacionDetalle.producto.precioClienteProducto.precioNeto * cotizacionDetalle.ProductoPresentacion.Equivalencia));
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

        public List<DocumentoDetalle> getPreciosVigentesGrupoCliente(int idGrupoCliente, DateTime? fechaPreciosVigenciaDesde = null)
        {
            var objCommand = GetSqlCommand("ps_productosVigentesGrupo");
            InputParameterAdd.Int(objCommand, "idGrupoCliente", idGrupoCliente);

            if (fechaPreciosVigenciaDesde != null)
            {
                InputParameterAdd.DateTime(objCommand, "fechaConsideracion", fechaPreciosVigenciaDesde.Value);
            }
            else
            {
                InputParameterAdd.DateTime(objCommand, "fechaConsideracion", DateTime.Now);
            }

            DataTable cotizacionDetalleDataTable = Execute(objCommand);

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
                cotizacionDetalle.producto.ventaRestringida = (Producto.TipoVentaRestringida)Converter.GetInt(row, "descontinuado");
                cotizacionDetalle.producto.motivoRestriccion = Converter.GetString(row, "motivo_restriccion");
                cotizacionDetalle.producto.topeDescuento = Converter.GetDecimal(row, "tope_descuento");

                cotizacionDetalle.producto.precioSinIgv = Converter.GetDecimal(row, "precio_sin_igv");
                cotizacionDetalle.producto.precioProvinciaSinIgv = Converter.GetDecimal(row, "precio_provincia_sin_igv");
                cotizacionDetalle.producto.costoSinIgv = Converter.GetDecimal(row, "costo_sin_igv");

                //El Precio sin igv es el precio lista
                cotizacionDetalle.producto.precioLista = cotizacionDetalle.producto.precioSinIgv;

                cotizacionDetalle.esPrecioAlternativo = Converter.GetBool(row, "es_precio_alternativo");
                cotizacionDetalle.unidad = Converter.GetString(row, "unidad");

                if (cotizacionDetalle.esPrecioAlternativo)
                {
                    cotizacionDetalle.ProductoPresentacion = new ProductoPresentacion();
                    cotizacionDetalle.ProductoPresentacion.Equivalencia = Converter.GetDecimal(row, "equivalencia");
                    cotizacionDetalle.precioNeto = cotizacionDetalle.producto.precioLista * cotizacionDetalle.ProductoPresentacion.Equivalencia;
                }


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
                //cotizacionDetalle.producto.precioClienteProducto.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                cotizacionDetalle.producto.precioClienteProducto.estadoCanasta = Converter.GetInt(row, "estado_canasta") == 1 ? true : false;

                cotizacionDetalle.precioCliente = new PrecioClienteProducto();
                cotizacionDetalle.precioCliente.skuCliente = Converter.GetString(row, "sku_cliente");
                if (row["fecha_inicio_vigencia"] == DBNull.Value)
                {
                    cotizacionDetalle.precioCliente.fechaInicioVigencia = null;
                }
                else
                {
                    cotizacionDetalle.precioCliente.fechaInicioVigencia = Converter.GetDateTime(row, "fecha_inicio_vigencia");
                }

                if (row["fecha_fin_vigencia"] == DBNull.Value)
                {
                    cotizacionDetalle.precioCliente.fechaFinVigencia = null;
                }
                else
                {
                    cotizacionDetalle.precioCliente.fechaFinVigencia = Converter.GetDateTime(row, "fecha_fin_vigencia");
                }
                //   cotizacionDetalle.porcentajeDescuento = Decimal.Parse(Request["porcentajeDescuento"].ToString());
                ///   detalle.esPrecioAlternativo = Int16.Parse(Request["esPrecioAlternativo"].ToString()) == 1;

                //cotizacionDetalle.porcentajeDescuento = cotizacionDetalle.producto.precioClienteProducto.precioNeto
                //    75×25 / 100 = 18,75

                // cotizacionDetalle

                if (cotizacionDetalle.esPrecioAlternativo)
                {


                    //Si es el precio Alternativo se multiplica por la equivalencia para que se registre el precio estandar
                    //dado que cuando se hace get al precioNetoEquivalente se recupera diviendo entre la equivalencia
                    cotizacionDetalle.precioNeto = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cotizacionDetalle.producto.precioClienteProducto.precioNeto * cotizacionDetalle.ProductoPresentacion.Equivalencia));
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


        public List<DocumentoDetalle> getPreciosHistoricoGrupoCliente(int idGrupoCliente)
        {
            var objCommand = GetSqlCommand("ps_productosHistoricoGrupo");
            InputParameterAdd.Int(objCommand, "idGrupoCliente", idGrupoCliente);
            DataTable cotizacionDetalleDataTable = Execute(objCommand);

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
                cotizacionDetalle.producto.topeDescuento = Converter.GetDecimal(row, "tope_descuento");

                cotizacionDetalle.producto.precioSinIgv = Converter.GetDecimal(row, "precio_sin_igv");
                cotizacionDetalle.producto.precioProvinciaSinIgv = Converter.GetDecimal(row, "precio_provincia_sin_igv");
                cotizacionDetalle.producto.costoSinIgv = Converter.GetDecimal(row, "costo_sin_igv");

                //El Precio sin igv es el precio lista
                cotizacionDetalle.producto.precioLista = cotizacionDetalle.producto.precioSinIgv;

                cotizacionDetalle.esPrecioAlternativo = Converter.GetBool(row, "es_precio_alternativo");
                cotizacionDetalle.unidad = Converter.GetString(row, "unidad");

                if (cotizacionDetalle.esPrecioAlternativo)
                {
                    cotizacionDetalle.ProductoPresentacion = new ProductoPresentacion();
                    cotizacionDetalle.ProductoPresentacion.Equivalencia = Converter.GetDecimal(row, "equivalencia");
                    cotizacionDetalle.precioNeto = cotizacionDetalle.producto.precioLista * cotizacionDetalle.ProductoPresentacion.Equivalencia;
                }


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
                //cotizacionDetalle.producto.precioClienteProducto.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                cotizacionDetalle.producto.precioClienteProducto.estadoCanasta = Converter.GetInt(row, "estado_canasta") == 1 ? true : false;

                cotizacionDetalle.precioCliente = new PrecioClienteProducto();
                if (row["fecha_inicio_vigencia"] == DBNull.Value)
                {
                    cotizacionDetalle.precioCliente.fechaInicioVigencia = null;
                }
                else
                {
                    cotizacionDetalle.precioCliente.fechaInicioVigencia = Converter.GetDateTime(row, "fecha_inicio_vigencia");
                }

                if (row["fecha_fin_vigencia"] == DBNull.Value)
                {
                    cotizacionDetalle.precioCliente.fechaFinVigencia = null;
                }
                else
                {
                    cotizacionDetalle.precioCliente.fechaFinVigencia = Converter.GetDateTime(row, "fecha_fin_vigencia");
                }
                //   cotizacionDetalle.porcentajeDescuento = Decimal.Parse(Request["porcentajeDescuento"].ToString());
                ///   detalle.esPrecioAlternativo = Int16.Parse(Request["esPrecioAlternativo"].ToString()) == 1;

                //cotizacionDetalle.porcentajeDescuento = cotizacionDetalle.producto.precioClienteProducto.precioNeto
                //    75×25 / 100 = 18,75

                // cotizacionDetalle

                if (cotizacionDetalle.esPrecioAlternativo)
                {


                    //Si es el precio Alternativo se multiplica por la equivalencia para que se registre el precio estandar
                    //dado que cuando se hace get al precioNetoEquivalente se recupera diviendo entre la equivalencia
                    cotizacionDetalle.precioNeto = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cotizacionDetalle.producto.precioClienteProducto.precioNeto * cotizacionDetalle.ProductoPresentacion.Equivalencia));
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
        public List<Producto> SelectProductos(Producto producto)
        {
            var objCommand = GetSqlCommand("ps_productos");
            InputParameterAdd.VarcharEmpty(objCommand, "sku", producto.sku);
            InputParameterAdd.VarcharEmpty(objCommand, "skuProveedor", producto.skuProveedor);
            InputParameterAdd.VarcharEmpty(objCommand, "descripcion", producto.descripcion);
            InputParameterAdd.Int(objCommand, "estado", producto.Estado);
            InputParameterAdd.Int(objCommand, "conImagen", producto.ConImagen);
            InputParameterAdd.Int(objCommand, "tipo", producto.tipoProductoVista);
            InputParameterAdd.Int(objCommand, "tipoVentaRestingida", producto.tipoVentaRestringidaBusqueda);
            InputParameterAdd.Varchar(objCommand, "familia", producto.familia);
            InputParameterAdd.Varchar(objCommand, "proveedor", producto.proveedor);
            InputParameterAdd.DateTime(objCommand, "fechaCreacionDesde", producto.fechaRegistroDesde == null ? producto.fechaRegistroDesde : new DateTime(producto.fechaRegistroDesde.Value.Year, producto.fechaRegistroDesde.Value.Month, producto.fechaRegistroDesde.Value.Day, 0, 0, 0));
            InputParameterAdd.DateTime(objCommand, "fechaCreacionHasta", producto.fechaRegistroHasta == null ? producto.fechaRegistroHasta : new DateTime(producto.fechaRegistroHasta.Value.Year, producto.fechaRegistroHasta.Value.Month, producto.fechaRegistroHasta.Value.Day, 23, 59, 59));


            DataTable dataTable = Execute(objCommand);


            List<Producto> productoList = new List<Producto>();
            foreach (DataRow row in dataTable.Rows)
            {

                Producto item = new Producto();
                item.idProducto = Converter.GetGuid(row, "id_producto");
                item.sku = Converter.GetString(row, "sku");
                item.skuProveedor = Converter.GetString(row, "sku_proveedor");
                item.descripcion = Converter.GetString(row, "descripcion");
                item.familia = Converter.GetString(row, "familia");
                item.proveedor = Converter.GetString(row, "proveedor");
                item.unidad = Converter.GetString(row, "unidad");
                item.unidad_alternativa = Converter.GetString(row, "unidad_alternativa");
                item.unidadProveedor = Converter.GetString(row, "unidad_proveedor");
                item.unidadConteo = Converter.GetString(row, "unidad_conteo");
                item.unidadProveedorInternacional = Converter.GetString(row, "unidad_proveedor_internacional");
                item.unidadEstandarInternacional = Converter.GetString(row, "unidad_estandar_internacional");
                item.unidadAlternativaInternacional = Converter.GetString(row, "unidad_alternativa_internacional");
                item.equivalenciaAlternativa = Converter.GetInt(row, "equivalencia");
                item.equivalenciaProveedor = Converter.GetInt(row, "equivalencia_proveedor");
                item.unidadPedidoProveedor = Converter.GetString(row, "unidad_pedido_proveedor");
                item.equivalenciaUnidadPedidoProveedor = Converter.GetInt(row, "equivalencia_unidad_pedido_proveedor");
                item.equivalenciaUnidadAlternativaUnidadConteo = Converter.GetInt(row, "equivalencia_unidad_alternativa_unidad_conteo");
                item.equivalenciaUnidadEstandarUnidadConteo = Converter.GetInt(row, "equivalencia_unidad_estandar_unidad_conteo");
                item.equivalenciaUnidadProveedorUnidadConteo = Converter.GetInt(row, "equivalencia_unidad_proveedor_unidad_conteo");

                item.descripcionLarga = Converter.GetString(row, "descripcion_larga");
                item.agregarDescripcionCotizacion = Converter.GetInt(row, "agregar_descripcion_cotizacion");
                item.cantidadMaximaPedidoRestringido = Converter.GetInt(row, "cantidad_maxima_pedido_restringido");

                item.codigoSunat = Converter.GetString(row, "codigo_sunat");
                item.exoneradoIgv = Converter.GetInt(row, "exonerado_igv") == 1 ? true : false;
                item.inafecto = Converter.GetInt(row, "inafecto") == 1 ? true : false;

                item.tipoProducto = (Producto.TipoProducto)Converter.GetInt(row, "tipo");
                item.tipoProductoVista = (int)item.tipoProducto;

                item.topeDescuento = Converter.GetDecimal(row, "tope_descuento");
                item.precioSinIgv = Converter.GetDecimal(row, "precio");
                item.precioOriginal = Converter.GetDecimal(row, "precio_original");
                item.precioProvinciaSinIgv = Converter.GetDecimal(row, "precio_provincia");
                item.precioProvinciasOriginal = Converter.GetDecimal(row, "precio_provincia_original");
                item.costoSinIgv = Converter.GetDecimal(row, "costo");
                item.costoOriginal = Converter.GetDecimal(row, "costo_original");
                item.costoReferencial = Converter.GetDecimal(row, "costo_referencial");
                item.costoReferencialOriginal = Converter.GetDecimal(row, "costo_original_referencial");

                item.esComercial = Converter.GetInt(row, "es_comercial");

                item.costoFleteProvincias = Converter.GetDecimal(row, "costo_flete_provincias");
                item.monedaFleteProvincias = Moneda.ListaMonedasFija.Where(m => m.codigo.Equals(Converter.GetString(row, "moneda_flete_provincias"))).First();

                item.kit = Converter.GetString(row, "kit");
                item.validaStock = Converter.GetInt(row, "valida_stock");

                item.tipoCambio = Converter.GetDecimal(row, "tipo_cambio");
                item.monedaMP = Converter.GetString(row, "moneda_venta");
                item.monedaProveedor = Converter.GetString(row, "moneda_compra");

                item.compraRestringida = Converter.GetInt(row, "compra_restringida");

                item.image = Converter.GetBytes(row, "imagen");
                item.Estado = Converter.GetInt(row, "estado");
                item.ventaRestringida = (Producto.TipoVentaRestringida)Converter.GetInt(row, "descontinuado");
                item.motivoRestriccion = Converter.GetString(row, "motivo_restriccion");
                item.FechaEdicion = Converter.GetDateTime(row, "fecha_modificacion");

                productoList.Add(item);
            }

            return productoList;
        }

        public List<Producto> ProductosPlantillaStock(Producto producto)
        {
            var objCommand = GetSqlCommand("ps_productos_plantilla_stock");
            InputParameterAdd.VarcharEmpty(objCommand, "sku", producto.sku);
            InputParameterAdd.VarcharEmpty(objCommand, "descripcion", producto.descripcion);
            InputParameterAdd.Int(objCommand, "estado", producto.Estado);
            InputParameterAdd.Int(objCommand, "tipoVentaRestingida", producto.tipoVentaRestringidaBusqueda);
            InputParameterAdd.Varchar(objCommand, "familia", producto.familia);
            InputParameterAdd.Varchar(objCommand, "proveedor", producto.proveedor);

            DataTable dataTable = Execute(objCommand);

            List<Producto> productoList = new List<Producto>();
            foreach (DataRow row in dataTable.Rows)
            {
                Producto item = new Producto();
                item.idProducto = Converter.GetGuid(row, "id_producto");
                item.sku = Converter.GetString(row, "sku");
                item.skuProveedor = Converter.GetString(row, "sku_proveedor");
                item.descripcion = Converter.GetString(row, "descripcion");
                item.familia = Converter.GetString(row, "familia");
                item.proveedor = Converter.GetString(row, "proveedor");
                item.unidad = Converter.GetString(row, "unidad");
                item.unidad_alternativa = Converter.GetString(row, "unidad_alternativa");
                item.unidadProveedor = Converter.GetString(row, "unidad_proveedor");
                item.unidadConteo = Converter.GetString(row, "unidad_conteo");
                item.unidadProveedorInternacional = Converter.GetString(row, "unidad_proveedor_internacional");
                item.unidadEstandarInternacional = Converter.GetString(row, "unidad_estandar_internacional");
                item.unidadAlternativaInternacional = Converter.GetString(row, "unidad_alternativa_internacional");
                item.equivalenciaAlternativa = Converter.GetInt(row, "equivalencia");
                item.equivalenciaProveedor = Converter.GetInt(row, "equivalencia_proveedor");
                item.unidadPedidoProveedor = Converter.GetString(row, "unidad_pedido_proveedor");
                item.equivalenciaUnidadPedidoProveedor = Converter.GetInt(row, "equivalencia_unidad_pedido_proveedor");
                item.equivalenciaUnidadAlternativaUnidadConteo = Converter.GetInt(row, "equivalencia_unidad_alternativa_unidad_conteo");
                item.equivalenciaUnidadEstandarUnidadConteo = Converter.GetInt(row, "equivalencia_unidad_estandar_unidad_conteo");
                item.equivalenciaUnidadProveedorUnidadConteo = Converter.GetInt(row, "equivalencia_unidad_proveedor_unidad_conteo");


                item.tipoProducto = Producto.TipoProducto.Bien;
                item.tipoProductoVista = (int)item.tipoProducto;

                item.Estado = Converter.GetInt(row, "estado");
                item.ventaRestringida = (Producto.TipoVentaRestringida)Converter.GetInt(row, "descontinuado");
                item.FechaEdicion = Converter.GetDateTime(row, "fecha_modificacion");

                productoList.Add(item);
            }

            return productoList;
        }


        public List<Producto> GetProductosBySKU(List<String> skus)
        {
            var objCommand = GetSqlCommand("ps_productosBySku");

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("ID", typeof(string)));

            foreach (String sku in skus)
            {
                DataRow rowObj = tvp.NewRow();
                rowObj["ID"] = sku;
                tvp.Rows.Add(rowObj);
            }

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@skuProductos", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.VarcharCList";

            DataTable dataTable = Execute(objCommand);

            List<Producto> lista = new List<Producto>();
            foreach (DataRow row in dataTable.Rows)
            {
                Producto item = new Producto();

                item.idProducto = Converter.GetGuid(row, "id_producto");
                item.sku = Converter.GetString(row, "sku");
                item.skuProveedor = Converter.GetString(row, "sku_proveedor");
                item.descripcion = Converter.GetString(row, "descripcion");
                item.descripcionLarga = Converter.GetString(row, "descripcion_larga");
                item.familia = Converter.GetString(row, "familia");
                item.proveedor = Converter.GetString(row, "proveedor");
                item.unidad = Converter.GetString(row, "unidad");
                item.unidadConteo = Converter.GetString(row, "unidad_conteo");

                item.unidad_alternativa = Converter.GetString(row, "unidad_alternativa");
                item.unidadProveedor = Converter.GetString(row, "unidad_proveedor");
                item.unidadPedidoProveedor = Converter.GetString(row, "unidad_pedido_proveedor");
                item.equivalenciaUnidadPedidoProveedor = Converter.GetInt(row, "equivalencia_unidad_pedido_proveedor");
                item.equivalenciaAlternativa = Converter.GetInt(row, "equivalencia");
                item.equivalenciaProveedor = Converter.GetInt(row, "equivalencia_proveedor");
                item.equivalenciaUnidadAlternativaUnidadConteo = Converter.GetInt(row, "equivalencia_unidad_alternativa_unidad_conteo");
                item.equivalenciaUnidadEstandarUnidadConteo = Converter.GetInt(row, "equivalencia_unidad_estandar_unidad_conteo");
                item.equivalenciaUnidadProveedorUnidadConteo = Converter.GetInt(row, "equivalencia_unidad_proveedor_unidad_conteo");
                item.Estado = Converter.GetInt(row, "estado");
                item.exoneradoIgv = Converter.GetInt(row, "exonerado_igv") == 1 ? true : false;
                item.inafecto = Converter.GetInt(row, "inafecto") == 1 ? true : false;
                item.unidadProveedorInternacional = Converter.GetString(row, "unidad_proveedor_internacional");
                item.unidadEstandarInternacional = Converter.GetString(row, "unidad_estandar_internacional");
                item.unidadAlternativaInternacional = Converter.GetString(row, "unidad_alternativa_internacional");

                item.Estado = Converter.GetInt(row, "estado");
                item.agregarDescripcionCotizacion = Converter.GetInt(row, "agregar_descripcion_cotizacion");
                item.tipoProducto = (Producto.TipoProducto)Converter.GetInt(row, "tipo");
                item.tipoProductoVista = (int)item.tipoProducto;
                //,usuario_creacion
                //,fecha_creacion
                //,usuario_modificacion
                //,fecha_modificacion

                item.monedaProveedor = Converter.GetString(row, "moneda_compra");
                item.monedaMP = Converter.GetString(row, "moneda_venta");
                item.tipoCambio = Converter.GetDecimal(row, "tipo_cambio");

                item.topeDescuento = Converter.GetDecimal(row, "tope_descuento");
                item.precioSinIgv = Converter.GetDecimal(row, "precio");
                item.precioOriginal = Converter.GetDecimal(row, "precio_original");
                item.precioProvinciaSinIgv = Converter.GetDecimal(row, "precio_provincia");
                item.precioProvinciasOriginal = Converter.GetDecimal(row, "precio_provincia_original");
                item.costoSinIgv = Converter.GetDecimal(row, "costo");
                item.costoOriginal = Converter.GetDecimal(row, "costo_original");

                item.codigoSunat = Converter.GetString(row, "codigo_sunat");
                item.fechaInicioVigencia = Converter.GetDateTime(row, "fecha_inicio_vigencia");

                item.image = Converter.GetBytes(row, "imagen");
                item.ventaRestringida = (Producto.TipoVentaRestringida)Converter.GetInt(row, "descontinuado");
                item.compraRestringida = Converter.GetInt(row, "compra_restringida");
                item.motivoRestriccion = Converter.GetString(row, "motivo_restriccion");
                item.cantidadMaximaPedidoRestringido = Converter.GetInt(row, "cantidad_maxima_pedido_restringido");

                lista.Add(item);
            }

            return lista;
        }

        public Producto GetProductoById(Guid idProducto)
        {
            var objCommand = GetSqlCommand("ps_producto");
            InputParameterAdd.Guid(objCommand, "idProducto", idProducto);
            DataTable dataTable = Execute(objCommand);

            Producto item = new Producto();
            foreach (DataRow row in dataTable.Rows)
            {
                item.idProducto = Converter.GetGuid(row, "id_producto");
                item.sku = Converter.GetString(row, "sku");
                item.skuProveedor = Converter.GetString(row, "sku_proveedor");
                item.descripcion = Converter.GetString(row, "descripcion");
                item.descripcionLarga = Converter.GetString(row, "descripcion_larga");
                item.familia = Converter.GetString(row, "familia");
                item.proveedor = Converter.GetString(row, "proveedor");
                item.unidad = Converter.GetString(row, "unidad");
                item.unidadConteo = Converter.GetString(row, "unidad_conteo");

                item.unidad_alternativa = Converter.GetString(row, "unidad_alternativa");
                item.unidadProveedor = Converter.GetString(row, "unidad_proveedor");
                item.unidadPedidoProveedor = Converter.GetString(row, "unidad_pedido_proveedor");
                item.equivalenciaUnidadPedidoProveedor = Converter.GetInt(row, "equivalencia_unidad_pedido_proveedor");
                item.equivalenciaAlternativa = Converter.GetInt(row, "equivalencia");
                item.equivalenciaProveedor = Converter.GetInt(row, "equivalencia_proveedor");
                item.equivalenciaUnidadAlternativaUnidadConteo = Converter.GetInt(row, "equivalencia_unidad_alternativa_unidad_conteo");
                item.equivalenciaUnidadEstandarUnidadConteo = Converter.GetInt(row, "equivalencia_unidad_estandar_unidad_conteo");
                item.equivalenciaUnidadProveedorUnidadConteo = Converter.GetInt(row, "equivalencia_unidad_proveedor_unidad_conteo");
                item.Estado = Converter.GetInt(row, "estado");
                item.exoneradoIgv = Converter.GetInt(row, "exonerado_igv") == 1 ? true : false;
                item.inafecto = Converter.GetInt(row, "inafecto") == 1 ? true : false;
                item.unidadProveedorInternacional = Converter.GetString(row, "unidad_proveedor_internacional");
                item.unidadEstandarInternacional = Converter.GetString(row, "unidad_estandar_internacional");
                item.unidadAlternativaInternacional = Converter.GetString(row, "unidad_alternativa_internacional");

                item.Estado = Converter.GetInt(row, "estado");
                item.agregarDescripcionCotizacion = Converter.GetInt(row, "agregar_descripcion_cotizacion");
                item.tipoProducto = (Producto.TipoProducto)Converter.GetInt(row, "tipo");
                item.tipoProductoVista = (int)item.tipoProducto;
                //,usuario_creacion
                //,fecha_creacion
                //,usuario_modificacion
                //,fecha_modificacion

                item.monedaProveedor = Converter.GetString(row, "moneda_compra");
                item.monedaMP = Converter.GetString(row, "moneda_venta");
                item.tipoCambio = Converter.GetDecimal(row, "tipo_cambio");

                item.topeDescuento = Converter.GetDecimal(row, "tope_descuento");
                item.precioSinIgv = Converter.GetDecimal(row, "precio");
                item.precioOriginal = Converter.GetDecimal(row, "precio_original");
                item.precioProvinciaSinIgv = Converter.GetDecimal(row, "precio_provincia");
                item.precioProvinciasOriginal = Converter.GetDecimal(row, "precio_provincia_original");
                item.costoSinIgv = Converter.GetDecimal(row, "costo");
                item.costoOriginal = Converter.GetDecimal(row, "costo_original");

                item.costoReferencial = Converter.GetDecimal(row, "costo_referencial");
                item.costoReferencialOriginal = Converter.GetDecimal(row, "costo_original_referencial");

                item.kit = Converter.GetString(row, "kit");
                item.validaStock= Converter.GetInt(row, "valida_stock");

                item.esComercial = Converter.GetInt(row, "es_comercial");

                item.costoFleteProvincias = Converter.GetDecimal(row, "costo_flete_provincias");
                item.monedaFleteProvincias = Moneda.ListaMonedasFija.Where(m => m.codigo.Equals(Converter.GetString(row, "moneda_flete_provincias"))).First();

                item.codigoSunat = Converter.GetString(row, "codigo_sunat");
                item.fechaInicioVigencia = Converter.GetDateTime(row, "fecha_inicio_vigencia");

                item.image = Converter.GetBytes(row, "imagen");
                item.ventaRestringida = (Producto.TipoVentaRestringida)Converter.GetInt(row, "descontinuado");
                item.compraRestringida = Converter.GetInt(row, "compra_restringida");
                item.motivoRestriccion = Converter.GetString(row, "motivo_restriccion");
                item.cantidadMaximaPedidoRestringido = Converter.GetInt(row, "cantidad_maxima_pedido_restringido");
            }

            return item;
        }

        public Producto insertProducto(Producto producto)
        {
            var objCommand = GetSqlCommand("pi_producto");

            InputParameterAdd.Guid(objCommand, "idUsuario", producto.IdUsuarioRegistro);
            InputParameterAdd.Varchar(objCommand, "sku", producto.sku);
            InputParameterAdd.Binary(objCommand, "imagen", producto.image);
            InputParameterAdd.Varchar(objCommand, "descripcion", producto.descripcion.Replace("\"", "''"));
            InputParameterAdd.Varchar(objCommand, "skuProveedor", producto.skuProveedor);
            InputParameterAdd.Varchar(objCommand, "familia", producto.familia);
            InputParameterAdd.Varchar(objCommand, "proveedor", producto.proveedor);
            InputParameterAdd.Varchar(objCommand, "unidad", producto.unidad);
            InputParameterAdd.Varchar(objCommand, "unidadAlternativa", producto.unidad_alternativa);
            InputParameterAdd.VarcharEmpty(objCommand, "unidadProveedor", producto.unidadProveedor == null ? "" : producto.unidadProveedor);
            InputParameterAdd.Varchar(objCommand, "unidadEstandarInternacional", producto.unidadEstandarInternacional);
            InputParameterAdd.Varchar(objCommand, "unidadPedidoProveedor", producto.unidadPedidoProveedor);
            InputParameterAdd.Int(objCommand, "equivalencia", producto.equivalenciaAlternativa);
            InputParameterAdd.Int(objCommand, "equivalenciaProveedor", producto.equivalenciaProveedor);
            InputParameterAdd.Int(objCommand, "equivalenciaUnidadPedidoProveedor", producto.equivalenciaUnidadPedidoProveedor);
            InputParameterAdd.Int(objCommand, "estado", producto.Estado);
            InputParameterAdd.Int(objCommand, "descontinuado", (int)producto.ventaRestringida);
            InputParameterAdd.Int(objCommand, "compraRestringida", producto.compraRestringida);
            InputParameterAdd.VarcharEmpty(objCommand, "motivoRestriccion", producto.motivoRestriccion);
            InputParameterAdd.Int(objCommand, "exoneradoIgv", (producto.exoneradoIgv ? 1 : 0));
            InputParameterAdd.Int(objCommand, "inafecto", producto.inafecto ? 1 : 0);
            InputParameterAdd.Int(objCommand, "tipo", (int)producto.tipoProducto);
            InputParameterAdd.Decimal(objCommand, "topeDescuento", producto.topeDescuento);
            InputParameterAdd.Decimal(objCommand, "precio", producto.precioSinIgv);
            InputParameterAdd.Decimal(objCommand, "precioProvincia", producto.precioProvinciaSinIgv);
            InputParameterAdd.Decimal(objCommand, "costo", producto.costoSinIgv);
            InputParameterAdd.Decimal(objCommand, "costoReferencial", producto.costoReferencial);
            InputParameterAdd.DateTime(objCommand, "fechaInicioVigencia", DateTime.Now);
            InputParameterAdd.Int(objCommand, "esCargaMasiva", producto.CargaMasiva ? 1 : 0);

            if (producto.descripcionLarga == null) producto.descripcionLarga = "";
            InputParameterAdd.Varchar(objCommand, "descripcionLarga", producto.descripcionLarga.Replace("\"", "''"));
            InputParameterAdd.Int(objCommand, "agregarDescripcionCotizacion", producto.agregarDescripcionCotizacion);
            InputParameterAdd.Int(objCommand, "cantidadMaximaPedidoRestringido", producto.cantidadMaximaPedidoRestringido);

            InputParameterAdd.Varchar(objCommand, "monedaCompra", producto.monedaProveedor);
            InputParameterAdd.Varchar(objCommand, "monedaVenta", producto.monedaMP);
            InputParameterAdd.Varchar(objCommand, "unidadAlternativaInternacional", producto.unidadAlternativaInternacional);
            InputParameterAdd.Varchar(objCommand, "unidadConteo", producto.unidadConteo);
            InputParameterAdd.VarcharEmpty(objCommand, "unidadProveedorInternacional", producto.unidadProveedorInternacional == null ? "" : producto.unidadProveedorInternacional);
            InputParameterAdd.Varchar(objCommand, "codigoSunat", producto.codigoSunat);

            InputParameterAdd.Int(objCommand, "validaStock", producto.validaStock);
            InputParameterAdd.Varchar(objCommand, "kit", producto.kit.Trim());

            InputParameterAdd.Int(objCommand, "esComercial", producto.esComercial);

            InputParameterAdd.Decimal(objCommand, "costoFleteProvincias", producto.costoFleteProvincias);
            InputParameterAdd.Varchar(objCommand, "monedaFleteProvincias", producto.monedaFleteProvincias.codigo);

            InputParameterAdd.Int(objCommand, "equivalenciaUnidadAlternativaUnidadConteo", producto.equivalenciaUnidadAlternativaUnidadConteo);
            InputParameterAdd.Int(objCommand, "equivalenciaUnidadEstandarUnidadConteo", producto.equivalenciaUnidadEstandarUnidadConteo);
            InputParameterAdd.Int(objCommand, "equivalenciaUnidadProveedorUnidadConteo", producto.equivalenciaUnidadProveedorUnidadConteo);

            InputParameterAdd.Decimal(objCommand, "costoOriginal", producto.costoOriginal);
            InputParameterAdd.Decimal(objCommand, "costoReferencialOriginal", producto.costoReferencialOriginal);
            InputParameterAdd.Decimal(objCommand, "precioOriginal", producto.precioOriginal);
            InputParameterAdd.Decimal(objCommand, "precioProvinciaOriginal", producto.precioProvinciasOriginal);
            InputParameterAdd.Decimal(objCommand, "tipoCambio", producto.tipoCambio);


            OutputParameterAdd.UniqueIdentifier(objCommand, "newId");

            ExecuteNonQuery(objCommand);

            producto.idProducto = (Guid)objCommand.Parameters["@newId"].Value;

            return producto;

        }

        public bool setSKUCliente(String skuCliente, Guid idCliente, Guid idUsuario, Guid idProducto)
        {
            var objCommand = GetSqlCommand("piu_cliente_producto");

            InputParameterAdd.Guid(objCommand, "idCliente", idCliente);
            InputParameterAdd.Guid(objCommand, "idProducto", idProducto);
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            InputParameterAdd.Varchar(objCommand, "sku", skuCliente);
            InputParameterAdd.Int(objCommand, "estado", 1);

            ExecuteNonQuery(objCommand);

            return true;
        }

        public bool setSKUClienteGrupo(String skuCliente, int idGrupo, Guid idUsuario, Guid idProducto, int replicarMiembros)
        {
            var objCommand = GetSqlCommand("piu_grupo_cliente_producto");

            InputParameterAdd.Int(objCommand, "idGrupo", idGrupo);
            InputParameterAdd.Guid(objCommand, "idProducto", idProducto);
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            InputParameterAdd.Varchar(objCommand, "sku", skuCliente);
            InputParameterAdd.Int(objCommand, "estado", 1);
            InputParameterAdd.Int(objCommand, "replicarMiembros", replicarMiembros);

            ExecuteNonQuery(objCommand);

            return true;
        }

        public Producto updateProducto(Producto producto)
        {
            var objCommand = GetSqlCommand("pu_producto");
            InputParameterAdd.Guid(objCommand, "idProducto", producto.idProducto);
            InputParameterAdd.Guid(objCommand, "idUsuario", producto.usuario.idUsuario);
            InputParameterAdd.Varchar(objCommand, "sku", producto.sku);
            InputParameterAdd.Binary(objCommand, "imagen", producto.image);
            InputParameterAdd.Varchar(objCommand, "descripcion", producto.descripcion.Replace("\"", "''"));
            InputParameterAdd.Varchar(objCommand, "skuProveedor", producto.skuProveedor);
            InputParameterAdd.Varchar(objCommand, "familia", producto.familia);
            InputParameterAdd.Varchar(objCommand, "proveedor", producto.proveedor);
            InputParameterAdd.Varchar(objCommand, "unidad", producto.unidad);
            InputParameterAdd.Varchar(objCommand, "unidadAlternativa", producto.unidad_alternativa);
            InputParameterAdd.Varchar(objCommand, "unidadProveedor", producto.unidadProveedor);
            InputParameterAdd.Varchar(objCommand, "unidadPedidoProveedor", producto.unidadPedidoProveedor);
            InputParameterAdd.Varchar(objCommand, "unidadEstandarInternacional", producto.unidadEstandarInternacional);
            InputParameterAdd.Int(objCommand, "equivalencia", producto.equivalenciaAlternativa);
            InputParameterAdd.Int(objCommand, "equivalenciaProveedor", producto.equivalenciaProveedor);
            InputParameterAdd.Int(objCommand, "equivalenciaUnidadPedidoProveedor", producto.equivalenciaUnidadPedidoProveedor);
            InputParameterAdd.Int(objCommand, "estado", producto.Estado);
            InputParameterAdd.Int(objCommand, "descontinuado", (int)producto.ventaRestringida);
            InputParameterAdd.Int(objCommand, "compraRestringida", producto.compraRestringida);

            if (producto.descripcionLarga == null) producto.descripcionLarga = "";
            InputParameterAdd.Varchar(objCommand, "descripcionLarga", producto.descripcionLarga.Replace("\"", "''"));
            InputParameterAdd.Int(objCommand, "agregarDescripcionCotizacion", producto.agregarDescripcionCotizacion);
            InputParameterAdd.Int(objCommand, "cantidadMaximaPedidoRestringido", producto.cantidadMaximaPedidoRestringido);

            if (producto.motivoRestriccion == null)
            {
                producto.motivoRestriccion = "";
            }
            InputParameterAdd.VarcharEmpty(objCommand, "motivoRestriccion", producto.motivoRestriccion);
            InputParameterAdd.Int(objCommand, "exoneradoIgv", producto.exoneradoIgv ? 1 : 0);
            InputParameterAdd.Int(objCommand, "inafecto", producto.inafecto ? 1 : 0);
            InputParameterAdd.Int(objCommand, "tipo", (int)producto.tipoProducto);
            InputParameterAdd.Decimal(objCommand, "precio", producto.precioSinIgv);
            InputParameterAdd.Decimal(objCommand, "precioProvincia", producto.precioProvinciaSinIgv);
            InputParameterAdd.Decimal(objCommand, "costo", producto.costoSinIgv);
            InputParameterAdd.Decimal(objCommand, "costoReferencial", producto.costoReferencial);
            InputParameterAdd.DateTime(objCommand, "fechaInicioVigencia", DateTime.Now);
            InputParameterAdd.Int(objCommand, "esCargaMasiva", producto.CargaMasiva ? 1 : 0);

            InputParameterAdd.Int(objCommand, "esComercial", producto.esComercial);

            InputParameterAdd.Varchar(objCommand, "monedaCompra", producto.monedaProveedor);
            InputParameterAdd.Varchar(objCommand, "monedaVenta", producto.monedaMP);
            InputParameterAdd.Varchar(objCommand, "unidadAlternativaInternacional", producto.unidadAlternativaInternacional);
            InputParameterAdd.Varchar(objCommand, "unidadConteo", producto.unidadConteo);
            InputParameterAdd.Varchar(objCommand, "unidadProveedorInternacional", producto.unidadProveedorInternacional);
            InputParameterAdd.Varchar(objCommand, "codigoSunat", producto.codigoSunat);

            InputParameterAdd.Decimal(objCommand, "costoFleteProvincias", producto.costoFleteProvincias);
            InputParameterAdd.Varchar(objCommand, "monedaFleteProvincias", producto.monedaFleteProvincias.codigo);

            InputParameterAdd.Int(objCommand, "equivalenciaUnidadAlternativaUnidadConteo", producto.equivalenciaUnidadAlternativaUnidadConteo);
            InputParameterAdd.Int(objCommand, "equivalenciaUnidadEstandarUnidadConteo", producto.equivalenciaUnidadEstandarUnidadConteo);
            InputParameterAdd.Int(objCommand, "equivalenciaUnidadProveedorUnidadConteo", producto.equivalenciaUnidadProveedorUnidadConteo);

            InputParameterAdd.Int(objCommand, "validaStock", producto.validaStock);
            InputParameterAdd.Varchar(objCommand, "kit", producto.kit == null ? "" : producto.kit.Trim());

            InputParameterAdd.Decimal(objCommand, "topeDescuento", producto.topeDescuento);
            InputParameterAdd.Decimal(objCommand, "costoOriginal", producto.costoOriginal);
            InputParameterAdd.Decimal(objCommand, "costoReferencialOriginal", producto.costoReferencialOriginal);
            InputParameterAdd.Decimal(objCommand, "precioOriginal", producto.precioOriginal);
            InputParameterAdd.Decimal(objCommand, "precioProvinciaOriginal", producto.precioProvinciasOriginal);
            InputParameterAdd.Decimal(objCommand, "tipoCambio", producto.tipoCambio);

            ExecuteNonQuery(objCommand);

            return producto;

        }

        public void actualizarTipoCambioCatalogo(Decimal tipoCambio, bool aplicaCosto, bool aplicaPrecio, bool aplicaPrecioProvincias, DateTime fechaInicioVigencia, Guid idUsuario)
        {
            var objCommand = GetSqlCommand("pp_actualiza_tipo_cambio_productos");
            InputParameterAdd.Int(objCommand, "aplicaCosto", aplicaCosto ? 1 : 0);
            InputParameterAdd.Int(objCommand, "aplicaPrecio", aplicaPrecio ? 1 : 0);
            InputParameterAdd.Int(objCommand, "aplicaPrecioProvincia", aplicaPrecioProvincias ? 1 : 0);
            InputParameterAdd.Decimal(objCommand, "tipoCambio", tipoCambio);
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            InputParameterAdd.DateTime(objCommand, "fechaInicioVigencia", fechaInicioVigencia);

            ExecuteNonQuery(objCommand);

        }

        public bool RegistroCierreStock(List<RegistroCargaStock> stock, DateTime fechaCierre, Guid idCiudad, Guid idUsuario, Guid idArchivoAdjunto)
        {
            var objCommand = GetSqlCommand("pi_cierre_stock");

            InputParameterAdd.Guid(objCommand, "idCiudad", idCiudad);
            InputParameterAdd.DateTime(objCommand, "fechaCierre", fechaCierre);
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            InputParameterAdd.Guid(objCommand, "idArchivoAdjunto", idArchivoAdjunto);

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("SKU", typeof(string)));
            tvp.Columns.Add(new DataColumn("CANTIDADPROVEEDOR", typeof(int)));
            tvp.Columns.Add(new DataColumn("CANTIDADMP", typeof(int)));
            tvp.Columns.Add(new DataColumn("CANTIDADALTERNATIVA", typeof(int)));

            foreach (RegistroCargaStock item in stock)
            {
                DataRow rowObj = tvp.NewRow();
                rowObj["SKU"] = item.sku;
                rowObj["CANTIDADPROVEEDOR"] = item.cantidadProveedor;
                rowObj["CANTIDADMP"] = item.cantidadMp;
                rowObj["CANTIDADALTERNATIVA"] = item.cantidadAlternativa;
                tvp.Rows.Add(rowObj);
            }

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@stocks", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.CierreStockDetalleList";

            OutputParameterAdd.UniqueIdentifier(objCommand, "newId");

            ExecuteNonQuery(objCommand);

            Guid idCierreStock = (Guid)objCommand.Parameters["@newId"].Value;

            return true;
        }

        public List<RegistroCargaStock> InventarioStock(int ajusteMercaderiaTransito, DateTime fechaReferencia, Guid idUsuario, Guid idCiudad, Producto producto)
        {
            var objCommand = GetSqlCommand("ps_stock_global");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            InputParameterAdd.Guid(objCommand, "idCiudad", idCiudad);
            InputParameterAdd.Int(objCommand, "ajusteMercaderiaTransito", ajusteMercaderiaTransito);
            InputParameterAdd.DateTime(objCommand, "fechaReferencia", fechaReferencia);
            InputParameterAdd.VarcharEmpty(objCommand, "sku", producto.sku);
            InputParameterAdd.VarcharEmpty(objCommand, "descripcion", producto.descripcion);
            InputParameterAdd.Int(objCommand, "tipoVentaRestingida", producto.tipoVentaRestringidaBusqueda);
            InputParameterAdd.Varchar(objCommand, "familia", producto.familia);
            InputParameterAdd.Varchar(objCommand, "proveedor", producto.proveedor);

            DataSet dataSet = ExecuteDataSet(objCommand);


            DataTable dataTable = dataSet.Tables[0];

            List<RegistroCargaStock> productoList = new List<RegistroCargaStock>();

            foreach (DataRow row in dataTable.Rows)
            {
                RegistroCargaStock item = new RegistroCargaStock();
                item.producto = new Producto();
                item.ciudad = new Ciudad();
                item.producto.idProducto = Converter.GetGuid(row, "id_producto");
                item.producto.sku = Converter.GetString(row, "sku");
                item.producto.skuProveedor = Converter.GetString(row, "sku_proveedor");
                item.producto.descripcion = Converter.GetString(row, "descripcion");
                item.producto.familia = Converter.GetString(row, "familia");
                item.producto.proveedor = Converter.GetString(row, "proveedor");
                item.producto.unidad = Converter.GetString(row, "unidad_mp");
                item.producto.unidad_alternativa = Converter.GetString(row, "unidad_alternativa");
                item.producto.unidadProveedor = Converter.GetString(row, "unidad_proveedor");
                item.producto.unidadConteo = Converter.GetString(row, "unidad_conteo");
                item.producto.equivalenciaAlternativa = Converter.GetInt(row, "equivalencia_alternativa");
                item.producto.equivalenciaProveedor = Converter.GetInt(row, "equivalencia_proveedor");
                item.producto.equivalenciaUnidadEstandarUnidadConteo = Converter.GetInt(row, "equivalencia_mp_conteo");
                item.cantidadProveedor = Converter.GetInt(row, "cantidad_unidad_proveedor");
                item.cantidadMp = Converter.GetInt(row, "cantidad_unidad_mp");
                item.cantidadAlternativa = Converter.GetInt(row, "cantidad_unidad_alternativa");
                item.cantidadConteo = Converter.GetInt(row, "cantidad_unidad_conteo") + Converter.GetInt(row, "movimientos_unidad_conteo");

                item.cantidadTrasladosSumarConteo = Converter.GetInt(row, "movimientos_traslado_sumar");

                item.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                item.ciudad.nombre = Converter.GetString(row, "nombre_ciudad");

                item.tieneRegistroStock = Converter.GetBool(row, "tiene_registro_stock");
                item.registradoPeridoAplicable = Converter.GetBool(row, "registrado_periodo_aplicable");

                item.fecha = Converter.GetDateTime(row, "fecha");

                productoList.Add(item);
            }

            return productoList;
        }

        public List<String> ListKitText()
        {
            var objCommand = GetSqlCommand("ps_get_producto_kit_options");

            DataSet dataSet = ExecuteDataSet(objCommand);


            DataTable dataTable = dataSet.Tables[0];

            List<String> list = new List<String>();

            foreach (DataRow row in dataTable.Rows)
            {
                String item = Converter.GetString(row, "texto");

                list.Add(item);
            }

            return list;
        }

        public List<RegistroCargaStock> StockProducto(string sku, Guid idUsuario)
        {
            var objCommand = GetSqlCommand("ps_stock_global_producto");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            InputParameterAdd.Varchar(objCommand, "sku", sku);

            DataSet dataSet = ExecuteDataSet(objCommand);


            DataTable dataTable = dataSet.Tables[0];

            List<RegistroCargaStock> productoList = new List<RegistroCargaStock>();

            foreach (DataRow row in dataTable.Rows)
            {
                RegistroCargaStock item = new RegistroCargaStock();
                item.producto = new Producto();
                item.ciudad = new Ciudad();
                item.producto.idProducto = Converter.GetGuid(row, "id_producto");
                item.producto.sku = Converter.GetString(row, "sku");
                item.producto.skuProveedor = Converter.GetString(row, "sku_proveedor");
                item.producto.descripcion = Converter.GetString(row, "descripcion");
                item.producto.familia = Converter.GetString(row, "familia");
                item.producto.proveedor = Converter.GetString(row, "proveedor");
                item.producto.unidad = Converter.GetString(row, "unidad");
                item.producto.unidad_alternativa = Converter.GetString(row, "unidad_alternativa");
                item.producto.unidadProveedor = Converter.GetString(row, "unidad_proveedor");
                item.producto.unidadConteo = Converter.GetString(row, "unidad_conteo");
                item.producto.equivalenciaAlternativa = Converter.GetInt(row, "equivalencia");
                item.producto.equivalenciaProveedor = Converter.GetInt(row, "equivalencia_proveedor");
                item.producto.equivalenciaUnidadEstandarUnidadConteo = Converter.GetInt(row, "equivalencia_mp_conteo");

                item.cantidadConteo = Converter.GetInt(row, "cantidad_unidad_conteo") + Converter.GetInt(row, "movimientos_unidad_conteo");
                item.cantidadProveedorCalc = ((Decimal)item.cantidadConteo) / ((Decimal)(item.producto.equivalenciaProveedor * item.producto.equivalenciaUnidadEstandarUnidadConteo));
                item.cantidadProveedorCalc = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, item.cantidadProveedorCalc));
                item.cantidadAlternativaCalc = ((Decimal)(item.cantidadConteo * item.producto.equivalenciaAlternativa)) / ((Decimal)item.producto.equivalenciaUnidadEstandarUnidadConteo);
                item.cantidadAlternativaCalc = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, item.cantidadAlternativaCalc));
                item.cantidadMpCalc = ((Decimal)item.cantidadConteo) / ((Decimal)item.producto.equivalenciaUnidadEstandarUnidadConteo);
                item.cantidadMpCalc = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, item.cantidadMpCalc));

                item.cantidadSeparadaConteo = (int) Converter.GetDecimal(row, "stock_separado_unidad_conteo");
                item.cantidadSeparadaMpCalc = ((Decimal)item.cantidadSeparadaConteo) / ((Decimal)item.producto.equivalenciaUnidadEstandarUnidadConteo);
                item.cantidadSeparadaMpCalc = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, item.cantidadSeparadaMpCalc));
                item.cantidadSeparadaProveedorCalc = item.cantidadSeparadaMpCalc / ((Decimal)item.producto.equivalenciaProveedor);
                item.cantidadSeparadaProveedorCalc = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, item.cantidadSeparadaProveedorCalc));
                item.cantidadSeparadaAlternativaCalc = item.cantidadSeparadaMpCalc * ((Decimal)item.producto.equivalenciaAlternativa);
                item.cantidadSeparadaAlternativaCalc = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, item.cantidadSeparadaAlternativaCalc));
                

                item.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                item.ciudad.nombre = Converter.GetString(row, "nombre_ciudad");

                item.tieneRegistroStock = Converter.GetBool(row, "tiene_registro_stock");
                item.registradoPeridoAplicable = Converter.GetBool(row, "registrado_periodo_aplicable");

                //item.fecha = Converter.GetDateTime(row, "fecha");

                productoList.Add(item);
            }
            return productoList;
        }

        public List<CierreStock> CargasStock(Guid idUsuario, Guid idCiudad)
        {
            var objCommand = GetSqlCommand("ps_cierres_stock");
            InputParameterAdd.Guid(objCommand, "idCiudad", idCiudad);
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);

            DataTable dataTable = Execute(objCommand);

            List<CierreStock> lista = new List<CierreStock>();

            foreach (DataRow row in dataTable.Rows)
            {
                CierreStock item = new CierreStock();

                item.idCierreStock = Converter.GetGuid(row, "id_cierre_stock");
                item.fecha = Converter.GetDateTime(row, "fecha");
                item.FechaRegistro = Converter.GetDateTime(row, "fecha_creacion");

                item.ciudad = new Ciudad();
                item.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                item.ciudad.nombre = Converter.GetString(row, "nombre_ciudad");

                item.UsuarioRegistro = new Usuario();
                item.UsuarioRegistro.idUsuario = Converter.GetGuid(row, "id_usuario");
                item.UsuarioRegistro.nombre = Converter.GetString(row, "nombre_usuario");

                item.archivo = new ArchivoAdjunto();
                item.archivo.idArchivoAdjunto = Converter.GetGuid(row, "id_archivo_adjunto");
                item.archivo.nombre = Converter.GetString(row, "nombre_archivo");

                lista.Add(item);
            }

            return lista;
        }

        public MovimientoKardexCabecera StockProductoKardex(Guid idUsuario, Guid idCiudad, Guid idProducto, DateTime? fechaInicio)
        {
            var objCommand = GetSqlCommand("ps_movimientos_stock_producto");
            InputParameterAdd.Guid(objCommand, "idCiudad", idCiudad);
            InputParameterAdd.Guid(objCommand, "idProducto", idProducto);
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            if (fechaInicio.HasValue)
            {
                InputParameterAdd.DateTime(objCommand, "fechaInicio", fechaInicio.Value);
            }

            DataTable dataTable = Execute(objCommand);

            MovimientoKardexCabecera kardex = new MovimientoKardexCabecera();
            kardex.movimientos = new List<MovimientoKardexDetalle>();

            DataSet dataSet = ExecuteDataSet(objCommand);

            DataTable dataTableCargas = dataSet.Tables[0];
            DataTable dataTableMovimientos = dataSet.Tables[1];

            foreach (DataRow row in dataTableCargas.Rows)
            {
                MovimientoKardexDetalle item = new MovimientoKardexDetalle();

                item.fecha = Converter.GetDateTime(row, "fecha");
                item.cantidadConteo = Converter.GetInt(row, "cantidad_unidad_conteo");
                item.tipoMovimiento = 99;

                kardex.movimientos.Add(item);
            }

            if (fechaInicio.HasValue)
            {
                kardex.movimientos = new List<MovimientoKardexDetalle>();
            }

            foreach (DataRow row in dataTableMovimientos.Rows)
            {
                MovimientoKardexDetalle item = new MovimientoKardexDetalle();

                item.fecha = Converter.GetDateTime(row, "fecha_emision");
                item.cantidadConteo = Converter.GetInt(row, "cantidad_unidad_conteo");
                item.serieDocumento = Converter.GetString(row, "serie_documento");
                item.numeroDocumento = Converter.GetString(row, "numero_documento");
                item.unidadMovimiento = Converter.GetString(row, "unidad");
                item.cantidadMovimiento = Converter.GetInt(row, "cantidad");

                item.nroDocumentoCliente = Converter.GetString(row, "ruc_cliente");
                item.razonSocialCliente = Converter.GetString(row, "razon_social_cliente");
                item.tipoDocumentoCliente = Converter.GetInt(row, "tipo_documento_cliente");

                string tipoMovimiento = Converter.GetString(row, "tipo_movimiento");
                switch (tipoMovimiento)
                {
                    case "E": item.tipoMovimiento = 2; break;
                    case "S": item.tipoMovimiento = 1; break;
                }

                string tipoDocumento = Converter.GetString(row, "tipo_documento");
                switch (tipoDocumento)
                {
                    case "GR": item.tipoDocumento = 1; break;
                    case "NI": item.tipoDocumento = 2; break;
                }

                if (kardex.ciudad == null)
                {
                    kardex.ciudad = new Ciudad();
                    kardex.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                    kardex.ciudad.nombre = Converter.GetString(row, "nombre_ciudad");
                }

                kardex.movimientos.Add(item);
            }

            return kardex;
        }

        public List<RegistroCargaStock> StockProductosSede(List<Guid> idProductos, Guid idCiudad, Guid idUsuario)
        {
            var objCommand = GetSqlCommand("ps_stock_productos_sede");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            InputParameterAdd.Guid(objCommand, "idCiudad", idCiudad);

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("ID", typeof(Guid)));

            foreach (Guid item in idProductos)
            {
                DataRow rowObj = tvp.NewRow();
                rowObj["ID"] = item;
                tvp.Rows.Add(rowObj);
            }

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@idProductos", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.UniqueIdentifierList";

            DataSet dataSet = ExecuteDataSet(objCommand);

            DataTable dataTable = dataSet.Tables[0];

            List<RegistroCargaStock> productoList = new List<RegistroCargaStock>();

            foreach (DataRow row in dataTable.Rows)
            {
                RegistroCargaStock item = new RegistroCargaStock();
                item.producto = new Producto();
                item.ciudad = new Ciudad();
                item.producto.idProducto = Converter.GetGuid(row, "id_producto");
                item.producto.sku = Converter.GetString(row, "sku");
                item.producto.skuProveedor = Converter.GetString(row, "sku_proveedor");
                item.producto.descripcion = Converter.GetString(row, "descripcion");
                item.producto.familia = Converter.GetString(row, "familia");
                item.producto.proveedor = Converter.GetString(row, "proveedor");
                item.producto.unidad = Converter.GetString(row, "unidad");
                item.producto.unidad_alternativa = Converter.GetString(row, "unidad_alternativa");
                item.producto.unidadProveedor = Converter.GetString(row, "unidad_proveedor");
                item.producto.unidadConteo = Converter.GetString(row, "unidad_conteo");
                item.producto.equivalenciaAlternativa = Converter.GetInt(row, "equivalencia");
                item.producto.equivalenciaProveedor = Converter.GetInt(row, "equivalencia_proveedor");
                item.producto.equivalenciaUnidadEstandarUnidadConteo = Converter.GetInt(row, "equivalencia_mp_conteo");

                item.cantidadConteo = Converter.GetInt(row, "cantidad_unidad_conteo") + Converter.GetInt(row, "movimientos_unidad_conteo");
                item.cantidadProveedorCalc = ((Decimal)item.cantidadConteo) / ((Decimal)(item.producto.equivalenciaProveedor * item.producto.equivalenciaUnidadEstandarUnidadConteo));
                item.cantidadProveedorCalc = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, item.cantidadProveedorCalc));
                item.cantidadAlternativaCalc = ((Decimal)(item.cantidadConteo * item.producto.equivalenciaAlternativa)) / ((Decimal)item.producto.equivalenciaUnidadEstandarUnidadConteo);
                item.cantidadAlternativaCalc = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, item.cantidadAlternativaCalc));
                item.cantidadMpCalc = ((Decimal)item.cantidadConteo) / ((Decimal)item.producto.equivalenciaUnidadEstandarUnidadConteo);
                item.cantidadMpCalc = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, item.cantidadMpCalc));

                item.cantidadSeparadaConteo = (int)Converter.GetDecimal(row, "stock_separado_unidad_conteo");
                item.cantidadSeparadaMpCalc = ((Decimal)item.cantidadSeparadaConteo) / ((Decimal)item.producto.equivalenciaUnidadEstandarUnidadConteo);
                item.cantidadSeparadaMpCalc = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, item.cantidadSeparadaMpCalc));
                item.cantidadSeparadaProveedorCalc = item.cantidadSeparadaMpCalc / ((Decimal)item.producto.equivalenciaProveedor);
                item.cantidadSeparadaProveedorCalc = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, item.cantidadSeparadaProveedorCalc));
                item.cantidadSeparadaAlternativaCalc = item.cantidadSeparadaMpCalc * ((Decimal)item.producto.equivalenciaAlternativa);
                item.cantidadSeparadaAlternativaCalc = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, item.cantidadSeparadaAlternativaCalc));

                item.tieneRegistroStock = Converter.GetBool(row, "tiene_registro_stock");
                item.registradoPeridoAplicable = Converter.GetBool(row, "registrado_periodo_aplicable");

                //item.fecha = Converter.GetDateTime(row, "fecha");

                productoList.Add(item);
            }
            return productoList;
        }

    }
}

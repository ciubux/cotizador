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


        public List<Producto> getProductosBusqueda(String textoBusqueda,bool considerarDescontinuados, String proveedor, String familia, Pedido.tiposPedido? tipoPedido = null)
        {
            var objCommand = GetSqlCommand("ps_getproductos_search");
            InputParameterAdd.Varchar(objCommand, "textoBusqueda", textoBusqueda);
            InputParameterAdd.Varchar(objCommand, "proveedor", proveedor);
            InputParameterAdd.Varchar(objCommand, "familia", familia);
            InputParameterAdd.Int(objCommand, "considerarDescontinuados", considerarDescontinuados ? 1 : 0);
            InputParameterAdd.Char(objCommand, "tipoPedido", tipoPedido == null? null: ((Char)tipoPedido).ToString());

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


        public Producto getProducto(Guid idProducto, Guid idCliente, Boolean esCompra = false)
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
                producto.sku = Converter.GetString(row, "sku");

                if (row["imagen"] != DBNull.Value){producto.image = (Byte[])(row["imagen"]);
                }
                producto.costoSinIgv = Converter.GetDecimal(row, "costo");
                if (esCompra)
                {
                    producto.precioSinIgv = producto.costoSinIgv;
                    producto.precioProvinciaSinIgv = producto.costoSinIgv;
                }
                else
                {
                    producto.precioSinIgv = Converter.GetDecimal(row, "precio");
                    producto.precioProvinciaSinIgv = Converter.GetDecimal(row, "precio_provincia");
                }
                

                producto.familia = Converter.GetString(row, "familia");
               // producto.clase = Converter.GetString(row, "clase");
                producto.proveedor = Converter.GetString(row, "proveedor");
                producto.unidad = Converter.GetString(row, "unidad");
                producto.unidad_alternativa = Converter.GetString(row, "unidad_alternativa");
                producto.equivalenciaAlternativa = Converter.GetInt(row, "equivalencia");
                producto.skuProveedor = Converter.GetString(row, "sku_proveedor");
                //Costo sin IGV
                
                producto.precioClienteProducto = new PrecioClienteProducto();
                producto.precioClienteProducto.idPrecioClienteProducto = Guid.Empty;

                producto.tipoProducto = (Producto.TipoProducto) Converter.GetInt(row, "tipo_producto");

                producto.Stock = Converter.GetInt(row, "stock");

                /*Obtenido a partir de precio Lista*/
                if (row["precio_neto"] != DBNull.Value)
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
                productoPresentacion.CostoSinIGV = Converter.GetInt(row, "costo_sin_igv");
                productoPresentacion.PrecioSinIGV = productoPresentacion.PrecioLimaSinIGV;
                productoPresentacion.UnidadInternacional = Converter.GetString(row, "unidad_internacional");
                productoPresentacion.Stock = Converter.GetInt(row, "stock");
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
                CotizacionDetalle cotizacionDetalle = new CotizacionDetalle(false,false);
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
                else {
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
                        cotizacionDetalle.precioNeto = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, precioNetoRecalculado* cotizacionDetalle.ProductoPresentacion.Equivalencia));

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

        public List<DocumentoDetalle> getPreciosVigentesCliente(Guid idCliente)
        {
            var objCommand = GetSqlCommand("ps_productosVigentesCliente");
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

                cotizacionDetalle.producto.precioClienteProducto.equivalencia = Converter.GetInt(row, "equivalencia");
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

        public List<DocumentoDetalle> getPreciosVigentesGrupoCliente(int idGrupoCliente)
        {
            var objCommand = GetSqlCommand("ps_productosVigentesGrupo");
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
            InputParameterAdd.Int(objCommand, "tipo", producto.tipoProductoVista);
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
                item.equivalenciaUnidadAlternativaUnidadConteo = Converter.GetInt(row, "equivalencia_unidad_alternativa_unidad_conteo");
                item.equivalenciaUnidadEstandarUnidadConteo = Converter.GetInt(row, "equivalencia_unidad_estandar_unidad_conteo");
                item.equivalenciaUnidadProveedorUnidadConteo = Converter.GetInt(row, "equivalencia_unidad_proveedor_unidad_conteo");
                item.codigoSunat = Converter.GetString(row, "codigo_sunat");
                item.exoneradoIgv = Converter.GetInt(row, "exonerado_igv") == 1 ? true : false;
                item.inafecto = Converter.GetInt(row, "inafecto") == 1 ? true : false;

                item.tipoProducto = (Producto.TipoProducto)Converter.GetInt(row, "tipo");
                item.tipoProductoVista = (int)item.tipoProducto;

                item.precioSinIgv = Converter.GetDecimal(row, "precio");
                item.precioOriginal = Converter.GetDecimal(row, "precio_original");
                item.precioProvinciaSinIgv = Converter.GetDecimal(row, "precio_provincia");
                item.precioProvinciasOriginal = Converter.GetDecimal(row, "precio_provincia_original");
                item.costoSinIgv = Converter.GetDecimal(row, "costo");
                item.costoOriginal = Converter.GetDecimal(row, "costo_original");

                item.tipoCambio = Converter.GetDecimal(row, "tipo_cambio");
                item.monedaMP = Converter.GetString(row, "moneda_venta");
                item.monedaProveedor = Converter.GetString(row, "moneda_compra");

                item.image = Converter.GetBytes(row, "imagen");
                item.Estado = Converter.GetInt(row, "estado");
                item.FechaEdicion = Converter.GetDateTime(row, "fecha_modificacion");
                

                
                productoList.Add(item);
            }
      
            return productoList;
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
                item.familia = Converter.GetString(row, "familia");
                item.proveedor = Converter.GetString(row, "proveedor");
                item.unidad = Converter.GetString(row, "unidad");
                item.unidadConteo = Converter.GetString(row, "unidad_conteo");
                item.unidad_alternativa = Converter.GetString(row, "unidad_alternativa");
                item.unidadProveedor = Converter.GetString(row, "unidad_proveedor");
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
                item.tipoProducto = (Producto.TipoProducto)Converter.GetInt(row, "tipo");
                item.tipoProductoVista = (int)item.tipoProducto;
                //,usuario_creacion
                //,fecha_creacion
                //,usuario_modificacion
                //,fecha_modificacion
                item.monedaProveedor = Converter.GetString(row, "monedaCompra");
                item.monedaMP = Converter.GetString(row, "monedaVenta");
                item.tipoCambio = Converter.GetDecimal(row, "tipo_cambio");

                item.precioSinIgv = Converter.GetDecimal(row, "precio");
                item.precioOriginal = Converter.GetDecimal(row, "precio_original");
                item.precioProvinciaSinIgv = Converter.GetDecimal(row, "precio_provincia");
                item.precioProvinciasOriginal = Converter.GetDecimal(row, "precio_provincia_original");
                item.costoSinIgv = Converter.GetDecimal(row, "costo");
                item.costoOriginal = Converter.GetDecimal(row, "costo_original");

                item.codigoSunat = Converter.GetString(row, "codigo_sunat");
                

                item.fechaInicioVigencia = Converter.GetDateTime(row, "fecha_inicio_vigencia");

                item.image = Converter.GetBytes(row, "imagen");
                
            }

            return item;
        }

        public Producto insertProducto(Producto producto)
        {
            var objCommand = GetSqlCommand("pi_producto");
            
            InputParameterAdd.Guid(objCommand, "idUsuario", producto.IdUsuarioRegistro);
            InputParameterAdd.Varchar(objCommand, "sku", producto.sku);
            InputParameterAdd.Binary(objCommand, "imagen", producto.image);
            InputParameterAdd.Varchar(objCommand, "descripcion", producto.descripcion);
            InputParameterAdd.Varchar(objCommand, "skuProveedor", producto.skuProveedor);
            InputParameterAdd.Varchar(objCommand, "familia", producto.familia);
            InputParameterAdd.Varchar(objCommand, "proveedor", producto.proveedor);
            InputParameterAdd.Varchar(objCommand, "unidad", producto.unidad);
            InputParameterAdd.Varchar(objCommand, "unidadAlternativa", producto.unidad_alternativa);
            InputParameterAdd.VarcharEmpty(objCommand, "unidadProveedor", producto.unidadProveedor == null ? "" : producto.unidadProveedor);
            InputParameterAdd.Varchar(objCommand, "unidadEstandarInternacional", producto.unidadEstandarInternacional);
            InputParameterAdd.Int(objCommand, "equivalencia", producto.equivalenciaAlternativa);
            InputParameterAdd.Int(objCommand, "equivalenciaProveedor", producto.equivalenciaProveedor);
            InputParameterAdd.Int(objCommand, "estado", producto.Estado);
            InputParameterAdd.Int(objCommand, "exoneradoIgv", (producto.exoneradoIgv ? 1 : 0));
            InputParameterAdd.Int(objCommand, "inafecto", producto.inafecto ? 1 : 0);
            InputParameterAdd.Int(objCommand, "tipo", (int) producto.tipoProducto);
            InputParameterAdd.Decimal(objCommand, "precio", producto.precioSinIgv);
            InputParameterAdd.Decimal(objCommand, "precioProvincia", producto.precioProvinciaSinIgv);
            InputParameterAdd.Decimal(objCommand, "costo", producto.costoSinIgv);
            InputParameterAdd.DateTime(objCommand, "fechaInicioVigencia", DateTime.Now);
            InputParameterAdd.Int(objCommand, "esCargaMasiva", producto.CargaMasiva ? 1 : 0);

            InputParameterAdd.Varchar(objCommand, "monedaCompra", producto.monedaProveedor);
            InputParameterAdd.Varchar(objCommand, "monedaVenta", producto.monedaMP);
            InputParameterAdd.Varchar(objCommand, "unidadAlternativaInternacional", producto.unidadAlternativaInternacional);
            InputParameterAdd.Varchar(objCommand, "unidadConteo", producto.unidadConteo);
            InputParameterAdd.VarcharEmpty(objCommand, "unidadProveedorInternacional", producto.unidadProveedorInternacional == null ? "" : producto.unidadProveedorInternacional);
            InputParameterAdd.Varchar(objCommand, "codigoSunat", producto.codigoSunat);

            InputParameterAdd.Int(objCommand, "equivalenciaUnidadAlternativaUnidadConteo", producto.equivalenciaUnidadAlternativaUnidadConteo);
            InputParameterAdd.Int(objCommand, "equivalenciaUnidadEstandarUnidadConteo", producto.equivalenciaUnidadEstandarUnidadConteo);
            InputParameterAdd.Int(objCommand, "equivalenciaUnidadProveedorUnidadConteo", producto.equivalenciaUnidadProveedorUnidadConteo);

            InputParameterAdd.Decimal(objCommand, "costoOriginal", producto.costoOriginal);
            InputParameterAdd.Decimal(objCommand, "precioOriginal", producto.precioOriginal);
            InputParameterAdd.Decimal(objCommand, "precioProvinciaOriginal", producto.precioProvinciasOriginal);
            InputParameterAdd.Decimal(objCommand, "tipoCambio", producto.tipoCambio);
            
            OutputParameterAdd.UniqueIdentifier(objCommand, "newId");

            ExecuteNonQuery(objCommand);

            producto.idProducto = (Guid)objCommand.Parameters["@newId"].Value;

            return producto;

        }


        public Producto updateProducto(Producto producto)
        {
            var objCommand = GetSqlCommand("pu_producto");
            InputParameterAdd.Guid(objCommand, "idProducto", producto.idProducto);
            InputParameterAdd.Guid(objCommand, "idUsuario", producto.usuario.idUsuario);
            InputParameterAdd.Varchar(objCommand, "sku", producto.sku);
            InputParameterAdd.Binary(objCommand, "imagen", producto.image);
            InputParameterAdd.Varchar(objCommand, "descripcion", producto.descripcion);
            InputParameterAdd.Varchar(objCommand, "skuProveedor", producto.skuProveedor);
            InputParameterAdd.Varchar(objCommand, "familia", producto.familia);
            InputParameterAdd.Varchar(objCommand, "proveedor", producto.proveedor);
            InputParameterAdd.Varchar(objCommand, "unidad", producto.unidad);
            InputParameterAdd.Varchar(objCommand, "unidadAlternativa", producto.unidad_alternativa);
            InputParameterAdd.Varchar(objCommand, "unidadProveedor", producto.unidadProveedor);
            InputParameterAdd.Varchar(objCommand, "unidadEstandarInternacional", producto.unidadEstandarInternacional);
            InputParameterAdd.Int(objCommand, "equivalencia", producto.equivalenciaAlternativa);
            InputParameterAdd.Int(objCommand, "equivalenciaProveedor", producto.equivalenciaProveedor);
            InputParameterAdd.Int(objCommand, "estado", producto.Estado);
            InputParameterAdd.Int(objCommand, "exoneradoIgv", producto.exoneradoIgv ? 1 : 0);
            InputParameterAdd.Int(objCommand, "inafecto", producto.inafecto ? 1 : 0);
            InputParameterAdd.Int(objCommand, "tipo", (int)producto.tipoProducto);
            InputParameterAdd.Decimal(objCommand, "precio", producto.precioSinIgv);
            InputParameterAdd.Decimal(objCommand, "precioProvincia", producto.precioProvinciaSinIgv);
            InputParameterAdd.Decimal(objCommand, "costo", producto.costoSinIgv);
            InputParameterAdd.DateTime(objCommand, "fechaInicioVigencia", DateTime.Now);
            InputParameterAdd.Int(objCommand, "esCargaMasiva", producto.CargaMasiva ? 1 : 0);
            
            InputParameterAdd.Varchar(objCommand, "monedaCompra", producto.monedaProveedor);
            InputParameterAdd.Varchar(objCommand, "monedaVenta", producto.monedaMP);
            InputParameterAdd.Varchar(objCommand, "unidadAlternativaInternacional", producto.unidadAlternativaInternacional);
            InputParameterAdd.Varchar(objCommand, "unidadConteo", producto.unidadConteo);
            InputParameterAdd.Varchar(objCommand, "unidadProveedorInternacional", producto.unidadProveedorInternacional);
            InputParameterAdd.Varchar(objCommand, "codigoSunat", producto.codigoSunat);

            InputParameterAdd.Int(objCommand, "equivalenciaUnidadAlternativaUnidadConteo", producto.equivalenciaUnidadAlternativaUnidadConteo);
            InputParameterAdd.Int(objCommand, "equivalenciaUnidadEstandarUnidadConteo", producto.equivalenciaUnidadEstandarUnidadConteo);
            InputParameterAdd.Int(objCommand, "equivalenciaUnidadProveedorUnidadConteo", producto.equivalenciaUnidadProveedorUnidadConteo);

            InputParameterAdd.Decimal(objCommand, "costoOriginal", producto.costoOriginal);
            InputParameterAdd.Decimal(objCommand, "precioOriginal", producto.precioOriginal);
            InputParameterAdd.Decimal(objCommand, "precioProvinciaOriginal", producto.precioProvinciasOriginal);
            InputParameterAdd.Decimal(objCommand, "tipoCambio", producto.tipoCambio);
            
            ExecuteNonQuery(objCommand);

            return producto;

        }
    }
}

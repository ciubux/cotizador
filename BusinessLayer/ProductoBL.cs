
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using Model.UTILES;
using System.IO;

namespace BusinessLayer
{
    public class ProductoBL
    {
        public String getProductosBusqueda(String textoBusqueda, bool considerarDescontinuados, String proveedor, String familia, Pedido.tiposPedido? tipoPedido = null, int incluyeDescontinuados = 1)
        {
            List<Producto> productoList = new List<Producto>();
            using (var dal = new ProductoDAL())
            {
                productoList = dal.getProductosBusqueda(textoBusqueda, considerarDescontinuados, proveedor, familia, tipoPedido, incluyeDescontinuados);
            }


            String resultado = "{\"q\":\"" + textoBusqueda + "\",\"results\":[";
            Boolean existe = false;
            foreach (Producto prod in productoList)
            {
                resultado += "{\"id\":\"" + prod.idProducto + "\",\"text\":\"" + prod.ToString() + "\",\"descontinuado\": " + prod.descontinuado.ToString() + "},";
                existe = true;
            }

            if (existe)
                resultado = resultado.Substring(0, resultado.Length - 1) + "]}";
            else
                resultado = resultado.Substring(0, resultado.Length) + "]}";


            return resultado;
        }



        public Guid getProductoId(String sku)
        {
            using (var dal = new ProductoDAL())
            {
                return dal.getProductoId(sku);
            }
        }


        public Guid getAllProductoId(String sku)
        {
            using (var dal = new ProductoDAL())
            {
                return dal.getAllProductoId(sku);
            }
        }


        public Producto getProducto(Guid idProducto, Boolean esProvincia, Boolean incluidoIGV, Guid idCliente, Boolean esCompra = false, String moneda = "PEN", TipoCambioSunat tc = null)
        {
            using (var dal = new ProductoDAL())
            {
                Producto producto = dal.getProducto(idProducto, idCliente, esCompra);
                //Si es Provincia automaticamente se considera el precioProvincia como precioSinIGV

                if (tc != null)
                {
                    if (moneda.Equals("USD"))
                    {
                        if (producto.monedaMP.Equals("S"))
                        {
                            producto.precioSinIgv = producto.precioOriginal / tc.valorSunatVenta;
                            producto.precioProvinciaSinIgv = producto.precioProvinciasOriginal / tc.valorSunatVenta;

                            foreach (ProductoPresentacion productoPresentacion in producto.ProductoPresentacionList)
                            {
                                productoPresentacion.PrecioSinIGV = productoPresentacion.PrecioLimaOriginalSinIGV / tc.valorSunatVenta;
                                productoPresentacion.PrecioProvinciasSinIGV = productoPresentacion.PrecioProvinciasOriginalSinIGV / tc.valorSunatVenta;
                            }
                        }
                        else
                        {
                            producto.precioSinIgv = producto.precioOriginal;
                            producto.precioProvinciaSinIgv = producto.precioProvinciasOriginal;

                            foreach (ProductoPresentacion productoPresentacion in producto.ProductoPresentacionList)
                            {
                                productoPresentacion.PrecioSinIGV = productoPresentacion.PrecioLimaOriginalSinIGV;
                                productoPresentacion.PrecioProvinciasSinIGV = productoPresentacion.PrecioProvinciasOriginalSinIGV;
                            }
                        }
                    }
                    else
                    {
                        if (producto.monedaMP.Equals("D"))
                        {
                            producto.precioSinIgv = tc.valorSunatVenta * producto.precioOriginal;
                            producto.precioProvinciaSinIgv = tc.valorSunatVenta * producto.precioProvinciasOriginal;

                            foreach (ProductoPresentacion productoPresentacion in producto.ProductoPresentacionList)
                            {
                                productoPresentacion.PrecioSinIGV = productoPresentacion.PrecioLimaOriginalSinIGV * tc.valorSunatVenta;
                                productoPresentacion.PrecioProvinciasSinIGV = productoPresentacion.PrecioProvinciasOriginalSinIGV * tc.valorSunatVenta;
                            }
                        }
                    }
                }

                if (esProvincia)
                {
                    producto.precioSinIgv = producto.precioProvinciaSinIgv;
                    foreach (ProductoPresentacion productoPresentacion in producto.ProductoPresentacionList)
                    {
                        productoPresentacion.PrecioSinIGV = productoPresentacion.PrecioProvinciasSinIGV;
                    }
                }

                //Si no contiene imagen entonces se carga la imagen por defecto
                if (producto.image == null)
                {
                    FileStream inStream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\images\\NoDisponible.gif", FileMode.Open);
                    MemoryStream storeStream = new MemoryStream();
                    storeStream.SetLength(inStream.Length);
                    inStream.Read(storeStream.GetBuffer(), 0, (int)inStream.Length);
                    storeStream.Flush();
                    inStream.Close();
                    producto.image = storeStream.GetBuffer();
                }

                //El precioSinIGV se convierte en precioLista
                producto.precioLista = producto.precioSinIgv;
                //El costoLista es el costo sin IGV
                producto.costoLista = producto.costoSinIgv;

                //En caso que se requiera hacer calculo incluido IGV entonces se agrega el IGV al precioSinGIV y al costoSinIGV/
                if (incluidoIGV)
                {
                    //Se agrega el IGV al costoLista
                    producto.costoLista = producto.costoSinIgv + (producto.costoSinIgv * Constantes.IGV);
                    //Se agrega el IGV al precioLista
                    producto.precioLista = producto.precioLista + (producto.precioLista * Constantes.IGV);

                    //Si el precioNetoEquivalente es distinto de null 
                    //es null cuando se obtiene de un precioRegistrado
                    if (producto.precioClienteProducto.idPrecioClienteProducto != Guid.Empty)
                    {
                        producto.precioClienteProducto.precioNeto = producto.precioClienteProducto.precioNeto + (producto.precioClienteProducto.precioNeto * Constantes.IGV);
                        producto.precioClienteProducto.precioUnitario = producto.precioClienteProducto.precioUnitario + (producto.precioClienteProducto.precioUnitario * Constantes.IGV);
                    }
                }

                //Se aplica formato al costo de Lista y al precio Lista a dos decimales
                producto.costoLista = Decimal.Parse(String.Format(Constantes.formatoCuatroDecimales, producto.costoLista));
                producto.precioLista = Decimal.Parse(String.Format(Constantes.formatoCuatroDecimales, producto.precioLista));
                //Se aplica formato al precioUnitario obtenido desde precioRegistrados
                if (producto.precioClienteProducto.idPrecioClienteProducto != Guid.Empty)
                {
                    producto.precioClienteProducto.precioNeto = Decimal.Parse(String.Format(Constantes.formatoCuatroDecimales, producto.precioClienteProducto.precioNeto));
                }
                return producto;
            }
        }

        public List<Producto> getProductos(Producto producto)
        {
            using (var productoDAL = new ProductoDAL())
            {
                return productoDAL.SelectProductos(producto);
            }
        }

        public List<Producto> getProductosPlantillaStock(Producto producto)
        {
            using (var productoDAL = new ProductoDAL())
            {
                return productoDAL.ProductosPlantillaStock(producto);
            }
        }

        public List<Producto> GetProductosBySKU(List<String> skus)
        {
            using (var productoDAL = new ProductoDAL())
            {
                return productoDAL.GetProductosBySKU(skus);
            }
        }

        public List<LogRegistroCampos> GetHistorialPreciosProductos(Guid idProducto)
        {
            using (LogCambioDAL logDal = new LogCambioDAL())
            {
                List<int> campos = new List<int>();
                campos.Add(79);
                campos.Add(80);
                campos.Add(81);

                List<LogCambio> logs = logDal.getLogCamposRegistro(idProducto.ToString(), campos);

                List<LogRegistroCampos> historial = new List<LogRegistroCampos>();

                LogRegistroCampos item = null;

                bool bloquearEditar01 = true;
                bool bloquearEditar02 = true;
                bool bloquearEditar03 = true;


                foreach (LogCambio log in logs)
                {
                    if (item == null || !item.fechaInicioVigencia.Equals(log.fechaInicioVigencia.ToString("dd/MM/yyyy")))
                    {
                        if (item != null)
                        {
                            historial.Add(item);
                        }
                        item = new LogRegistroCampos();

                        item.fechaInicioVigencia = log.fechaInicioVigencia.ToString("dd/MM/yyyy");
                    }

                    switch (log.idCampo)
                    {
                        case 79:
                            item.dato01 = new LogRegistroCampoDato();
                            if (bloquearEditar01)
                            {
                                bloquearEditar01 = false;
                                item.dato01.editable = false;
                            }
                            else
                            {
                                item.dato01.editable = true;
                            }
                            item.dato01.idRegistroCambio = log.idCambio;
                            item.dato01.valor = log.valor;
                            item.dato01.fechaModificacion = log.FechaEdicion.ToString("dd/MM/yyyy HH:mm");
                            break;
                        case 80:
                            item.dato02 = new LogRegistroCampoDato();
                            if (bloquearEditar02)
                            {
                                bloquearEditar02 = false;
                                item.dato02.editable = false;
                            }
                            else
                            {
                                item.dato02.editable = true;
                            }
                            item.dato02.idRegistroCambio = log.idCambio;
                            item.dato02.valor = log.valor;
                            item.dato02.fechaModificacion = log.FechaEdicion.ToString("dd/MM/yyyy HH:mm");
                            break;
                        case 81:
                            item.dato03 = new LogRegistroCampoDato();
                            if (bloquearEditar03)
                            {
                                bloquearEditar03 = false;
                                item.dato03.editable = false;
                            }
                            else
                            {
                                item.dato03.editable = true;
                            }
                            item.dato03.idRegistroCambio = log.idCambio;
                            item.dato03.valor = log.valor;
                            item.dato03.fechaModificacion = log.FechaEdicion.ToString("dd/MM/yyyy HH:mm");
                            break;
                    }
                }

                if (item != null)
                {
                    historial.Add(item);
                }

                return historial;
            }
        }


        public Producto getProductoById(Guid idProducto)
        {
            using (var productoDAL = new ProductoDAL())
            {
                Producto producto = productoDAL.GetProductoById(idProducto);
                if (producto.image == null)
                {
                    FileStream inStream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\images\\NoDisponible.gif", FileMode.Open);
                    MemoryStream storeStream = new MemoryStream();
                    storeStream.SetLength(inStream.Length);
                    inStream.Read(storeStream.GetBuffer(), 0, (int)inStream.Length);
                    storeStream.Flush();
                    inStream.Close();
                    producto.image = storeStream.GetBuffer();
                }
                return producto;
            }
        }

        public Producto insertProducto(Producto producto)
        {
            using (var productoDAL = new ProductoDAL())
            {
                return productoDAL.insertProducto(producto);
            }
        }

        public Producto updateProducto(Producto producto)
        {
            using (var productoDAL = new ProductoDAL())
            {
                return productoDAL.updateProducto(producto);
            }
        }


        public List<CotizacionDetalle> obtenerProductosAPartirdePreciosRegistrados(Guid idCliente, DateTime fechaPrecios, Boolean esProvincia, Boolean incluidoIGV, String familia, String proveedor)
        {
            using (var dal = new ProductoDAL())
            {
                List<CotizacionDetalle> documentoDetalleList = dal.obtenerProductosAPartirdePreciosRegistrados(idCliente, fechaPrecios, familia, proveedor);

                foreach (CotizacionDetalle cotizacionDetalle in documentoDetalleList)
                {

                    if (cotizacionDetalle.producto.image == null)
                    {
                        FileStream inStream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\images\\NoDisponible.gif", FileMode.Open);
                        MemoryStream storeStream = new MemoryStream();
                        storeStream.SetLength(inStream.Length);
                        inStream.Read(storeStream.GetBuffer(), 0, (int)inStream.Length);
                        storeStream.Flush();
                        inStream.Close();
                        cotizacionDetalle.producto.image = storeStream.GetBuffer();
                    }

                    //Si es provincia se considera el precioProvincia
                    //cotizacion.ciudad.
                    if (esProvincia)
                    {
                        cotizacionDetalle.producto.precioSinIgv = cotizacionDetalle.producto.precioProvinciaSinIgv;
                    }

                    if (cotizacionDetalle.esPrecioAlternativo)
                    {
                        cotizacionDetalle.precioNeto = cotizacionDetalle.precioNeto * cotizacionDetalle.ProductoPresentacion.Equivalencia;
                        cotizacionDetalle.porcentajeDescuento = 100 - ((cotizacionDetalle.precioNeto * cotizacionDetalle.ProductoPresentacion.Equivalencia) * 100 / cotizacionDetalle.producto.precioSinIgv);
                    }
                    else
                    {
                        cotizacionDetalle.precioNeto = cotizacionDetalle.precioNeto;
                        cotizacionDetalle.porcentajeDescuento = 100 - (cotizacionDetalle.precioNeto * 100 / cotizacionDetalle.producto.precioSinIgv);
                    }


                    if (incluidoIGV)
                    {
                        //Se agrega el IGV al precioLista
                        decimal precioSinIgv = cotizacionDetalle.producto.precioSinIgv;
                        decimal precioLista = precioSinIgv + (precioSinIgv * Constantes.IGV);
                        cotizacionDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, precioLista));
                        //Se agrega el IGV al costoLista
                        decimal costoSinIgv = cotizacionDetalle.producto.costoSinIgv;
                        decimal costoLista = costoSinIgv + (costoSinIgv * Constantes.IGV);
                        cotizacionDetalle.producto.costoLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, costoLista));
                    }
                    else
                    {
                        //Se agrega el IGV al precioLista
                        cotizacionDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cotizacionDetalle.producto.precioSinIgv));
                        //Se agrega el IGV al costoLista
                        cotizacionDetalle.producto.costoLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cotizacionDetalle.producto.costoSinIgv));
                    }

                    /*   if (cotizacionDetalle.esPrecioAlternativo)
                       {
                           cotizacionDetalle.producto.costoListaAnterior = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cotizacionDetalle.producto.costoListaAnterior / cotizacionDetalle.ProductoPresentacion.Equivalencia));
                       }*/

                }

                return documentoDetalleList;
            }

        }



        public List<DocumentoDetalle> obtenerProductosAPartirdePreciosRegistradosParaPedido(Guid idCliente, DateTime fechaPrecios, Boolean esProvincia, Boolean incluidoIGV, String familia, String proveedor)
        {
            using (var dal = new ProductoDAL())
            {
                List<DocumentoDetalle> documentoDetalleList = dal.obtenerProductosAPartirdePreciosRegistradosParaPedido(idCliente, fechaPrecios, familia, proveedor);

                foreach (DocumentoDetalle cotizacionDetalle in documentoDetalleList)
                {

                    if (cotizacionDetalle.producto.image == null)
                    {
                        FileStream inStream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\images\\NoDisponible.gif", FileMode.Open);
                        MemoryStream storeStream = new MemoryStream();
                        storeStream.SetLength(inStream.Length);
                        inStream.Read(storeStream.GetBuffer(), 0, (int)inStream.Length);
                        storeStream.Flush();
                        inStream.Close();
                        cotizacionDetalle.producto.image = storeStream.GetBuffer();
                    }




                    //Si es provincia se considera el precioProvincia
                    //cotizacion.ciudad.
                    if (esProvincia)
                    {
                        cotizacionDetalle.producto.precioSinIgv = cotizacionDetalle.producto.precioProvinciaSinIgv;
                    }

                    if (cotizacionDetalle.esPrecioAlternativo)
                    {
                        cotizacionDetalle.precioNeto = cotizacionDetalle.precioNeto * cotizacionDetalle.ProductoPresentacion.Equivalencia;
                        cotizacionDetalle.porcentajeDescuento = 100 - ((cotizacionDetalle.precioNeto * cotizacionDetalle.ProductoPresentacion.Equivalencia) * 100 / cotizacionDetalle.producto.precioSinIgv);
                    }
                    else
                    {
                        cotizacionDetalle.precioNeto = cotizacionDetalle.precioNeto;
                        cotizacionDetalle.porcentajeDescuento = 100 - (cotizacionDetalle.precioNeto * 100 / cotizacionDetalle.producto.precioSinIgv);
                    }






                    if (incluidoIGV)
                    {
                        //Se agrega el IGV al precioLista
                        decimal precioSinIgv = cotizacionDetalle.producto.precioSinIgv;
                        decimal precioLista = precioSinIgv + (precioSinIgv * Constantes.IGV);
                        cotizacionDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, precioLista));
                        //Se agrega el IGV al costoLista
                        decimal costoSinIgv = cotizacionDetalle.producto.costoSinIgv;
                        decimal costoLista = costoSinIgv + (costoSinIgv * Constantes.IGV);
                        cotizacionDetalle.producto.costoLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, costoLista));
                    }
                    else
                    {
                        //Se agrega el IGV al precioLista
                        cotizacionDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cotizacionDetalle.producto.precioSinIgv));
                        //Se agrega el IGV al costoLista
                        cotizacionDetalle.producto.costoLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cotizacionDetalle.producto.costoSinIgv));
                    }


                }

                return documentoDetalleList;
            }

        }



        public void setProductoStaging(ProductoStaging productoStaging)
        {
            using (var productoDAL = new ProductoDAL())
            {
                productoDAL.setProductoStaging(productoStaging);
            }
        }

        public void truncateProductoStaging()
        {
            using (var productoDAL = new ProductoDAL())
            {
                productoDAL.truncateProductoStaging();
            }
        }

        public void mergeProductoStaging()
        {
            using (var productoDAL = new ProductoDAL())
            {
                productoDAL.mergeProductoStaging();
            }
        }

        public void actualizaTipoCambioCatalogo(Decimal tipoCambio, List<CampoPersistir> campos, DateTime fechaInicioVigencia, Guid idUsuario)
        {

            using (var dal = new ProductoDAL())
            {
                bool aplicaCosto = false;
                bool aplicaPrecio = false;
                bool aplicaPrecioProvincias = false;

                foreach (CampoPersistir cp in campos)
                {
                    if (cp.registra && cp.campo.nombre == "costo")
                    {
                        aplicaCosto = true;
                    }

                    if (cp.registra && cp.campo.nombre == "precio")
                    {
                        aplicaPrecio = true;
                    }

                    if (cp.registra && cp.campo.nombre == "precio_provincia")
                    {
                        aplicaPrecioProvincias = true;
                    }
                }

                dal.actualizarTipoCambioCatalogo(tipoCambio, aplicaCosto, aplicaPrecio, aplicaPrecioProvincias, fechaInicioVigencia, idUsuario);
            }
        }

        public static bool esCampoActualizableCargaMasiva(string campo)
        {
            return Producto.esCampoActualizableCargaMasiva(campo);
        }

        public static bool esCampoCalculado(string campo)
        {
            return Producto.esCampoCalculado(campo);
        }

        public bool RegistroCierreStock(List<RegistroCargaStock> stock, DateTime fechaCierre, Guid idCiudad, Guid idUsuario, Guid idArchivoAdjunto)
        {
            using (ProductoDAL dal = new ProductoDAL())
            {
                return dal.RegistroCierreStock(stock, fechaCierre, idCiudad, idUsuario, idArchivoAdjunto);
            }
        }

        public List<RegistroCargaStock> InventarioStock(int ajusteMercaderiaTransito, DateTime fechaReferencia, Guid idUsuario, Guid idCiudad, Producto producto)
        {
            using (var productoDAL = new ProductoDAL())
            {
                return productoDAL.InventarioStock(ajusteMercaderiaTransito, fechaReferencia, idUsuario, idCiudad, producto);
            }
        }

        public List<RegistroCargaStock> StockProducto(string sku, Guid idUsuario)
        {
            using (var productoDAL = new ProductoDAL())
            {
                return productoDAL.StockProducto(sku, idUsuario);
            }
        }

        public List<RegistroCargaStock> StockProductosSede(List<Guid> idProductos, Guid idCiudad, Guid idUsuario)
        {
            using (var productoDAL = new ProductoDAL())
            {
                return productoDAL.StockProductosSede(idProductos, idCiudad, idUsuario);
            }
        }

        public List<CierreStock> CargasStock(Guid idUsuario, Guid idCiudad)
        {
            using (var productoDAL = new ProductoDAL())
            {
                return productoDAL.CargasStock(idUsuario, idCiudad);
            }
        }

        public MovimientoKardexCabecera StockProductoKardex(Guid idUsuario, Guid idCiudad, Guid idProducto, int idProductoPresentacion)
        {
            using (var productoDAL = new ProductoDAL())
            {
                CiudadDAL ciudadDal = new CiudadDAL();
                
                MovimientoKardexCabecera kardex = productoDAL.StockProductoKardex(idUsuario, idCiudad, idProducto);
                kardex.producto = productoDAL.GetProductoById(idProducto);
                kardex.unidadConteo = kardex.producto.unidadConteo;
                kardex.ciudad = ciudadDal.getCiudad(idCiudad);

                switch (idProductoPresentacion)
                {
                    case 0: kardex.unidad = kardex.producto.unidad; break;
                    case 1: kardex.unidad = kardex.producto.unidad_alternativa; break;
                    case 2: kardex.unidad = kardex.producto.unidadProveedor; break;
                }

                int stockConteo = 0;
                foreach (MovimientoKardexDetalle item in kardex.movimientos)
                {
                    switch (item.tipoMovimiento)
                    {
                        case 1: stockConteo -= item.cantidadConteo; break;
                        case 2: stockConteo += item.cantidadConteo; break;
                        case 99: stockConteo += item.cantidadConteo; break;
                    }

                    item.stockConteo = stockConteo;
                    
                    switch (idProductoPresentacion)
                    {
                        case 0: item.stockUnidad = ((Decimal)item.stockConteo) / ((Decimal)kardex.producto.equivalenciaUnidadEstandarUnidadConteo); break;
                        case 1: item.stockUnidad = ((Decimal)(item.stockConteo * kardex.producto.equivalenciaAlternativa)) / ((Decimal)kardex.producto.equivalenciaUnidadEstandarUnidadConteo); break;
                        case 2: item.stockUnidad = ((Decimal)item.stockConteo) / ((Decimal)(kardex.producto.equivalenciaProveedor * kardex.producto.equivalenciaUnidadEstandarUnidadConteo)); break;
                    }

                    item.stockUnidad = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, item.stockUnidad));
                }                

                return kardex;
            }
        }
    }
}

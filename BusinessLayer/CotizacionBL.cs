
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;
using System.Linq;

namespace BusinessLayer
{
    public class CotizacionBL
    {

        private void validarCotizacion(Cotizacion cotizacion, Cotizacion cotizacionAprobada = null)
        {

            //Si no se consideran cantidades no se debe grabar el subtotal
            if (cotizacion.considerarCantidades == Cotizacion.OpcionesConsiderarCantidades.Observaciones)
            {
                cotizacion.montoSubTotal = 0;
                cotizacion.montoTotal = 0;
                cotizacion.montoIGV = 0;
            }

            //0 es días
            if (cotizacion.mostrarValidezOfertaEnDias == 0)
            {
                cotizacion.fechaLimiteValidezOferta = cotizacion.fecha.AddDays(cotizacion.validezOfertaEnDias);
            }

            cotizacion.seguimientoCotizacion.observacion = String.Empty;
            cotizacion.seguimientoCotizacion.estado = SeguimientoCotizacion.estadosSeguimientoCotizacion.Aprobada;


            //Si es cotizacion Grupal y no es aprobador se debe ir a pendiente de aprobación
            if ((cotizacion.cliente.idCliente == null || cotizacion.cliente.idCliente == Guid.Empty))
            {
                cotizacion.seguimientoCotizacion.observacion = cotizacion.seguimientoCotizacion.observacion + "Cotización Grupal.\n";
                cotizacion.seguimientoCotizacion.estado = SeguimientoCotizacion.estadosSeguimientoCotizacion.Pendiente;
            }

            ///Se hay un cambio en las fechas y en la vigencia se realiza evaluación

            if (cotizacion.tipoCotizacion == Cotizacion.TiposCotizacion.Normal)
            {
                if (cotizacionAprobada == null ||
                    cotizacionAprobada.fecha != cotizacion.fecha ||
                    cotizacionAprobada.fechaEsModificada != cotizacion.fechaEsModificada ||
                    cotizacionAprobada.fechaInicioVigenciaPrecios != cotizacion.fechaInicioVigenciaPrecios ||
                    cotizacionAprobada.fechaFinVigenciaPrecios != cotizacion.fechaFinVigenciaPrecios)
                {


                    if (cotizacion.fechaEsModificada)
                    {
                        if (!cotizacion.usuario.apruebaCotizaciones)
                        {
                            cotizacion.seguimientoCotizacion.observacion = cotizacion.seguimientoCotizacion.observacion + "Se modificó la fecha de la cotización.\n";
                            cotizacion.seguimientoCotizacion.estado = SeguimientoCotizacion.estadosSeguimientoCotizacion.Pendiente;
                        }
                    }
                    else
                    {
                        //Si la fecha no es modificada expresamente entonces toma la fecha del sistema
                        cotizacion.fecha = DateTime.Now;
                    }

                    if ((cotizacion.fechaInicioVigenciaPrecios != null || cotizacion.fechaFinVigenciaPrecios != null) && !cotizacion.usuario.apruebaCotizaciones)
                    {
                        cotizacion.seguimientoCotizacion.observacion = cotizacion.seguimientoCotizacion.observacion + "Se modificó la fecha de inicio y/o fin de vigencia de precios.\n";
                        cotizacion.seguimientoCotizacion.estado = SeguimientoCotizacion.estadosSeguimientoCotizacion.Pendiente;
                    }
                }
            }

            if (cotizacion.tipoCotizacion == Cotizacion.TiposCotizacion.Transitoria && cotizacionAprobada != null && !cotizacion.usuario.apruebaCotizaciones)
            {
                if (cotizacionAprobada.fecha != cotizacion.fecha ||
                    cotizacionAprobada.fechaEsModificada != cotizacion.fechaEsModificada)
                {
                    if (cotizacion.fechaEsModificada)
                    {
                        cotizacion.seguimientoCotizacion.observacion = cotizacion.seguimientoCotizacion.observacion + "Se modificó la fecha de la cotización.\n";
                        cotizacion.seguimientoCotizacion.estado = SeguimientoCotizacion.estadosSeguimientoCotizacion.Pendiente;
                    }
                    else
                    {
                        //Si la fecha no es modificada expresamente entonces toma la fecha del sistema
                        cotizacion.fecha = DateTime.Now;
                    }
                }


                if (cotizacionAprobada.fechaInicioVigenciaPrecios != cotizacion.fechaInicioVigenciaPrecios ||
                    cotizacionAprobada.fechaFinVigenciaPrecios != cotizacion.fechaFinVigenciaPrecios)
                {
                    cotizacion.seguimientoCotizacion.observacion = cotizacion.seguimientoCotizacion.observacion + "Se modificó la fecha de inicio y/o fin de vigencia de precios.\n";
                    cotizacion.seguimientoCotizacion.estado = SeguimientoCotizacion.estadosSeguimientoCotizacion.Pendiente;
                }
            }


            foreach (CotizacionDetalle cotizacionDetalle in cotizacion.cotizacionDetalleList)
            {
                cotizacionDetalle.idCotizacion = cotizacion.idCotizacion;
                //cotizacionDetalle.usuario = cotizacion.usuario;

                //Si no se consideran cantidades no se debe grabar la cantidad y el subtotal
                if (cotizacion.considerarCantidades == Cotizacion.OpcionesConsiderarCantidades.Observaciones)
                {
                    cotizacionDetalle.cantidad = 0;
                }
                else if (cotizacion.considerarCantidades == Cotizacion.OpcionesConsiderarCantidades.Cantidades)
                {
                    cotizacionDetalle.observacion = null;
                }

                //Si no es aprobador para que la cotización se cree como aprobada el porcentaje de descuento debe ser mayor o igual 
                //al porcentaje Limite sin aprobacion

         

                if ((cotizacionDetalle.validar &&
                        (!cotizacion.usuario.apruebaCotizaciones || 
                        cotizacionDetalle.porcentajeDescuento > cotizacion.usuario.maximoPorcentajeDescuentoAprobacion || 
                        !cotizacionDetalle.cumpleTopeDescuentoProducto) ) 
                    || cotizacion.tipoCotizacion == Cotizacion.TiposCotizacion.Trivial)
                {
                    PrecioClienteProducto precioClienteProducto = cotizacionDetalle.producto.precioClienteProducto;

                    int evaluarDescuento = 0;
                    //¿Tiene precio registrado para facturación? y eliente es el mismo?
                    if (precioClienteProducto.idPrecioClienteProducto != Guid.Empty)// && precioClienteProducto.cliente.idCliente == cotizacion.cliente.idCliente)
                    {
                        DateTime hoy = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                        //Si el precio es igual pero la fecha de fin de vigencia es indefinida y se ha superado los dias de vigencia general.
                        //Se envia a evaluar Descuento
                        if (precioClienteProducto.fechaFinVigencia == null && hoy > precioClienteProducto.fechaInicioVigencia.Value.AddDays(Constantes.DIAS_MAX_VIGENCIA_PRECIOS_COTIZACION))
                        {
                            evaluarDescuento = 1;
                        }
                        else if (precioClienteProducto.fechaFinVigencia != null && hoy > precioClienteProducto.fechaFinVigencia.Value)
                        {
                            evaluarDescuento = 4;
                        }
                        else
                        {
                            //Si el precio unitario registrado es distinto del precio registrado para facturacion
                            //Se envia a evaluar Descuento
                            if (!cotizacionDetalle.esPrecioAlternativo)
                            {
                                if (cotizacionDetalle.precioUnitario != precioClienteProducto.precioUnitario)
                                {
                                    if(cotizacion.tipoCotizacion == Cotizacion.TiposCotizacion.Trivial)
                                        evaluarDescuento = 5;
                                    else 
                                        evaluarDescuento = 2;
                                }
                            }
                            else
                            {
                                if (cotizacionDetalle.precioUnitario !=
                                    Decimal.Parse(String.Format(Constantes.formatoDecimalesPrecioNeto, precioClienteProducto.precioUnitario / cotizacionDetalle.ProductoPresentacion.Equivalencia))
)
                                {
                                    if (cotizacion.tipoCotizacion == Cotizacion.TiposCotizacion.Trivial)
                                        evaluarDescuento = 5;
                                    else
                                        evaluarDescuento = 2;
                                }
                            }


                        }

                    }
                    else
                    {
                        evaluarDescuento = 3;
                    }

                    if (!cotizacionDetalle.cumpleTopeDescuentoProducto && cotizacion.usuario.maximoPorcentajeDescuentoAprobacion < 100)
                    {
                        evaluarDescuento = 0;
                        if (cotizacion.tipoCotizacion != Cotizacion.TiposCotizacion.Normal)
                        {
                            if (!cotizacionDetalle.esPrecioAlternativo)
                            {
                                if (cotizacionDetalle.precioUnitario != precioClienteProducto.precioUnitario)
                                {
                                    evaluarDescuento = 6;
                                }
                            }
                            else
                            {
                                if (cotizacionDetalle.precioUnitario !=
                                    Decimal.Parse(String.Format(Constantes.formatoDecimalesPrecioNeto, precioClienteProducto.precioUnitario / cotizacionDetalle.ProductoPresentacion.Equivalencia))
)
                                {
                                    evaluarDescuento = 6;
                                }
                            }
                        } else {
                            evaluarDescuento = 6;
                        }
                    }
                    

                    if (evaluarDescuento > 0)
                    {
                        if (cotizacionDetalle.porcentajeDescuento > Constantes.PORCENTAJE_MAX_APROBACION || evaluarDescuento == 6 || cotizacion.tipoCotizacion == Cotizacion.TiposCotizacion.Trivial)
                        {
                            //Se evalua de donde proviene para indicar el mensaje exacto
                            switch (evaluarDescuento)
                            {
                                case 1:
                                    cotizacion.seguimientoCotizacion.observacion = cotizacion.seguimientoCotizacion.observacion + "Se aplicó un descuento superior al " + Constantes.PORCENTAJE_MAX_APROBACION + " % sobre el producto " + cotizacionDetalle.producto.sku + ". El precio unitario registrado en facturación se registro hace más de "+ Constantes.DIAS_MAX_VIGENCIA_PRECIOS_COTIZACION+ " días.\n";
                                    break;
                                case 2:
                                    cotizacion.seguimientoCotizacion.observacion = cotizacion.seguimientoCotizacion.observacion + "Se aplicó un descuento superior al " + Constantes.PORCENTAJE_MAX_APROBACION + " % sobre el producto " + cotizacionDetalle.producto.sku + ". El precio unitario es distinto al precio registrado en facturación.\n";
                                    break;
                                case 3:
                                    cotizacion.seguimientoCotizacion.observacion = cotizacion.seguimientoCotizacion.observacion + "Se aplicó un descuento superior al " + Constantes.PORCENTAJE_MAX_APROBACION + " % sobre el producto " + cotizacionDetalle.producto.sku + ".  No se encontró precio unitario en precios registrados en facturación.\n";
                                    break;
                                case 4:
                                    cotizacion.seguimientoCotizacion.observacion = cotizacion.seguimientoCotizacion.observacion + "Se aplicó un descuento superior al " + Constantes.PORCENTAJE_MAX_APROBACION + " % sobre el producto " + cotizacionDetalle.producto.sku + ". El precio unitario registrado en facturación tuvo vigencia hasta "+ precioClienteProducto.fechaFinVigencia.Value.ToString(Constantes.formatoFecha) + ".\n";
                                    break;
                                case 5:
                                    cotizacion.seguimientoCotizacion.observacion = cotizacion.seguimientoCotizacion.observacion + "El precio unitario es distinto al precio registrado en facturación.\n";
                                    break;
                                case 6:
                                    cotizacion.seguimientoCotizacion.observacion = cotizacion.seguimientoCotizacion.observacion + "El producto " + cotizacionDetalle.producto.sku + " tiene un tope de descuento máximo de " + String.Format(Constantes.formatoDosDecimales, cotizacionDetalle.producto.topeDescuento) + " %.\n";
                                    break;
                            }
                            //cotizacion.seguimientoCotizacion.estado = SeguimientoCotizacion.estadosSeguimientoCotizacion.Pendiente;
                            //La cotización no quedará en estado Pendiente de aprobación, dado que requiere el comentario para que pase a estado Pendiente de Aprobación
                            cotizacion.seguimientoCotizacion.estado = SeguimientoCotizacion.estadosSeguimientoCotizacion.Edicion;
                        }
                        
                    }
                }

                if (cotizacionDetalle.producto.descontinuado == 1 && !cotizacion.usuario.apruebaCotizacionesVentaRestringida && cotizacion.considerarCantidades != Cotizacion.OpcionesConsiderarCantidades.Observaciones)
                {
                    if (cotizacionDetalle.cantidad > 1 && cotizacionDetalle.cantidad > cotizacionDetalle.producto.cantidadMaximaPedidoRestringido)
                    {
                        cotizacion.seguimientoCotizacion.observacion = cotizacion.seguimientoCotizacion.observacion + "El producto " + cotizacionDetalle.producto.sku + " es de venta restringida.\n";
                        cotizacion.seguimientoCotizacion.estado = SeguimientoCotizacion.estadosSeguimientoCotizacion.Edicion;
                    }
                }

                /*if (!cotizacionDetalle.producto.monedaMP.Equals(cotizacion.moneda.caracter) && !cotizacion.usuario.apruebaCotizaciones)
                {   
                    cotizacion.seguimientoCotizacion.observacion = cotizacion.seguimientoCotizacion.observacion +  "El producto " + cotizacionDetalle.producto.sku + " tiene su precio de lista registrado en " + Moneda.ListaMonedas.Where(m => m.caracter.Equals(cotizacionDetalle.producto.monedaMP)).FirstOrDefault().nombre + "\n";
                    cotizacion.seguimientoCotizacion.estado = SeguimientoCotizacion.estadosSeguimientoCotizacion.Edicion;
                }*/
            }
           
            if (cotizacion.seguimientoCotizacion.estado == SeguimientoCotizacion.estadosSeguimientoCotizacion.Aprobada)
            {
                cotizacion.seguimientoCotizacion.observacion = null;
            }

            //Si la cotización es Trivial entonces se debe aprobar automáticamente de no ser el caso se debe cancelar la creación
            if (cotizacion.tipoCotizacion == Cotizacion.TiposCotizacion.Trivial && cotizacion.seguimientoCotizacion.estado == SeguimientoCotizacion.estadosSeguimientoCotizacion.Pendiente)
            {
                throw new Exception("Una cotización Trivial no debe contener productos con precios que no están vigentes o no son precios de Lista, para estos casos crear una cotización Normal.");
            }


        }


        public void InsertCotizacion(Cotizacion cotizacion)
        {
            using (var dal = new CotizacionDAL())
            {
                validarCotizacion(cotizacion);
                dal.InsertCotizacion(cotizacion);
            }
        }

        public void UpdateCotizacion(Cotizacion cotizacion, Cotizacion cotizacionAprobada)
        {
            using (var dal = new CotizacionDAL())
            {
                validarCotizacion(cotizacion, cotizacionAprobada);
                dal.UpdateCotizacion(cotizacion);
            }
        }

        public void RechazarCotizaciones()
        {
            using (var dal = new CotizacionDAL())
            {
                dal.RechazarCotizaciones();
            }
        }


        public void cambiarEstadoCotizacion(Cotizacion cotizacion)
        {
            using (var dal = new CotizacionDAL())
            {
                dal.insertSeguimientoCotizacion(cotizacion);
            }
             
        }

        public Cotizacion obtenerProductosAPartirdePreciosRegistrados(Cotizacion cotizacion, String familia, String proveedor,Usuario usuario)
        {

            ProductoBL productoBL = new ProductoBL();
            List<CotizacionDetalle> documentoDetalleList = productoBL.obtenerProductosAPartirdePreciosRegistrados(cotizacion.cliente.idCliente, cotizacion.fechaPrecios, cotizacion.ciudad.esProvincia, cotizacion.incluidoIGV, familia, proveedor);

            foreach (CotizacionDetalle cotizacionDetalle in documentoDetalleList)
            {
                cotizacionDetalle.visualizaCostos = usuario.visualizaCostos;
                cotizacionDetalle.visualizaMargen = usuario.visualizaMargen;
                cotizacionDetalle.cantidad = 1;
                if (cotizacionDetalle.esPrecioAlternativo)
                    cotizacionDetalle.precioNeto = cotizacionDetalle.precioNeto * cotizacionDetalle.ProductoPresentacion.Equivalencia;
                else
                    cotizacionDetalle.precioNeto = cotizacionDetalle.precioNeto;
            }
            cotizacion.cotizacionDetalleList = documentoDetalleList;
            return cotizacion;
        }


        public Cotizacion obtenerProductosAPartirdePreciosRegistradosParaGrupo(Cotizacion cotizacion, String familia, String proveedor, Usuario usuario)
        {

            ProductoBL productoBL = new ProductoBL();
            List<CotizacionDetalle> documentoDetalleList = null;// productoBL.obtenerProductosAPartirdePreciosRegistradosParaGrupo(cotizacion.grupo.idGrupoCliente, cotizacion.fechaPrecios, cotizacion.ciudad.esProvincia, cotizacion.incluidoIGV, familia, proveedor);

            foreach (CotizacionDetalle cotizacionDetalle in documentoDetalleList)
            {
                cotizacionDetalle.visualizaCostos = usuario.visualizaCostos;
                cotizacionDetalle.visualizaMargen = usuario.visualizaMargen;
                cotizacionDetalle.cantidad = 1;
                if (cotizacionDetalle.esPrecioAlternativo)
                    cotizacionDetalle.precioNeto = cotizacionDetalle.precioNeto * cotizacionDetalle.ProductoPresentacion.Equivalencia;
                else
                    cotizacionDetalle.precioNeto = cotizacionDetalle.precioNeto;
            }
            cotizacion.cotizacionDetalleList = documentoDetalleList;
            return cotizacion;
        }



        public Cotizacion GetCotizacion(Cotizacion cotizacion, Usuario usuario, SeguimientoCotizacion.estadosSeguimientoCotizacion? estadoAnteriorCotizacion = null)
        {
            using (var dal = new CotizacionDAL())
            {
                cotizacion = dal.SelectCotizacion(cotizacion, usuario);

                if (cotizacion.mostrarValidezOfertaEnDias == 0)
                {
                    DateTime dt = cotizacion.fechaLimiteValidezOferta;
                    DateTime dt2 = cotizacion.fecha;
                    cotizacion.fechaLimiteValidezOferta = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
                    cotizacion.fecha = new DateTime(dt2.Year, dt2.Month, dt2.Day, 0, 0, 0);
                    TimeSpan diferencia;
                    diferencia = cotizacion.fechaLimiteValidezOferta - cotizacion.fecha;
                    cotizacion.validezOfertaEnDias = diferencia.Days;
                }

                Boolean validar = true;
                
                if (estadoAnteriorCotizacion != null && estadoAnteriorCotizacion.Value == SeguimientoCotizacion.estadosSeguimientoCotizacion.Aprobada)
                {
                    validar = false;
                }                

                foreach (CotizacionDetalle cotizacionDetalle in cotizacion.cotizacionDetalleList)
                {
                    cotizacionDetalle.validar = validar;

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

                    Decimal precioSinIGV = cotizacionDetalle.producto.precioSinIgv;
                    //Es precio alternativo
                    if (cotizacionDetalle.esPrecioAlternativo)
                    {
                        precioSinIGV = cotizacionDetalle.producto.precioSinIgv / cotizacionDetalle.ProductoPresentacion.Equivalencia;
                    }
                    
                    if(!cotizacionDetalle.cumpleTopeDescuentoProducto)
                    {
                        cotizacionDetalle.validar = true;
                    }

                    if (cotizacionDetalle.porcentajeDescuento == 0 && cotizacionDetalle.precioNeto != precioSinIGV)
                    {
                        cotizacionDetalle.porcentajeDescuento = 100 - (cotizacionDetalle.precioNeto * 100 / cotizacionDetalle.producto.precioSinIgv);
                    }


                    if (cotizacion.incluidoIGV)
                    {
                        //Se agrega el IGV al precioLista
                        decimal precioSinIgv = cotizacionDetalle.producto.precioSinIgv;
                        decimal precioLista = precioSinIgv + (precioSinIgv * cotizacion.igv);
                        cotizacionDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, precioLista));
                        //Se agrega el IGV al costoLista
                        decimal costoSinIgv = cotizacionDetalle.producto.costoSinIgv;
                        decimal costoLista = costoSinIgv + (costoSinIgv * cotizacion.igv);
                        cotizacionDetalle.producto.costoLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, costoLista));
                    }
                    else
                    {
                        cotizacionDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cotizacionDetalle.producto.precioSinIgv));
                        cotizacionDetalle.producto.costoLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cotizacionDetalle.producto.costoSinIgv));
                    }
                }
            }
            return cotizacion;
        }



        public Cotizacion GetReCotizacion(Cotizacion cotizacion, Usuario usuario, Boolean mantenerPorcentajeDescuento)
        {
            using (var dal = new CotizacionDAL())
            {
                cotizacion = dal.SelectCotizacion(cotizacion, usuario);

                if (cotizacion.mostrarValidezOfertaEnDias == 0)
                {
                    DateTime dt = cotizacion.fechaLimiteValidezOferta;                    DateTime dt2 = cotizacion.fecha;
                    cotizacion.fechaLimiteValidezOferta = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
                    cotizacion.fecha = new DateTime(dt2.Year, dt2.Month, dt2.Day, 0, 0, 0);
                    TimeSpan diferencia;                    diferencia = cotizacion.fechaLimiteValidezOferta - cotizacion.fecha;
                    cotizacion.validezOfertaEnDias = diferencia.Days;
                }

                cotizacion.idCotizacionAntecedente = cotizacion.idCotizacion;
                cotizacion.codigoAntecedente = cotizacion.codigo;
                cotizacion.productosInactivosRemovidos = false;
                List<CotizacionDetalle> detalles = new List<CotizacionDetalle>();
                foreach (CotizacionDetalle cotizacionDetalle in cotizacion.cotizacionDetalleList)
                {
                    if (cotizacionDetalle.producto.Estado == 1) 
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



                        //Si es RECOTIZACIÓN se calcula el nuevo precioNetoEquivalente a partir del precioSinIGV del producto
                        if (cotizacion.esRecotizacion)
                        {
                            //Si es provincia se considera el precioProvincia como precioSinIgv
                            if (cotizacion.ciudad.esProvincia)
                            {
                                cotizacionDetalle.producto.precioSinIgv = cotizacionDetalle.producto.precioProvinciaSinIgv;
                            }



                            Decimal precioNeto = cotizacionDetalle.producto.precioSinIgv;
                            Decimal costo = cotizacionDetalle.producto.costoSinIgv;

                            //Se agrega el igv al costo y al precio neto que se obtuvo directamente del producto
                            if (cotizacion.incluidoIGV)
                            {
                                precioNeto = precioNeto + (precioNeto * Constantes.IGV);
                                costo = costo + (costo * Constantes.IGV);
                                cotizacionDetalle.producto.costoLista = costo;
                                //Tambien se agrega el IGV al costo Anterior, dado que fue almacenado sin IGV
                                cotizacionDetalle.producto.costoListaAnterior = cotizacionDetalle.producto.costoListaAnterior + (cotizacionDetalle.producto.costoListaAnterior * Constantes.IGV);
                                cotizacionDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, precioNeto));
                            }
                            else
                            {
                                cotizacionDetalle.producto.costoLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, costo));
                                cotizacionDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, precioNeto));
                            }

                            if (mantenerPorcentajeDescuento)
                            {
                                //Se calcula el descuento para el nuevo precioNetoEquivalente
                                if (cotizacionDetalle.porcentajeDescuento != 0)
                                {
                                    precioNeto = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, (precioNeto * (100 - cotizacionDetalle.porcentajeDescuento) / 100)));
                                }
                            }
                            else
                            {

                                if (cotizacionDetalle.esPrecioAlternativo)
                                {
                                    cotizacionDetalle.porcentajeDescuento = 100 - (cotizacionDetalle.precioNetoAnterior * 100 / (cotizacionDetalle.producto.precioLista / cotizacionDetalle.ProductoPresentacion.Equivalencia));
                                    precioNeto = cotizacionDetalle.precioNetoAnterior * cotizacionDetalle.ProductoPresentacion.Equivalencia;
                                }
                                else
                                {
                                    cotizacionDetalle.porcentajeDescuento = 100 - (cotizacionDetalle.precioNetoAnterior * 100 / cotizacionDetalle.producto.precioLista);
                                    precioNeto = cotizacionDetalle.precioNetoAnterior;
                                }

                            }


                            cotizacionDetalle.precioNeto = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, precioNeto));

                            /*       if (cotizacionDetalle.esPrecioAlternativo)
                                   {
                                       cotizacionDetalle.producto.costoListaAnterior = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cotizacionDetalle.producto.costoListaAnterior / cotizacionDetalle.ProductoPresentacion.Equivalencia));
                                       cotizacionDetalle.producto.precioListaAnterior = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cotizacionDetalle.producto.precioListaAnterior / cotizacionDetalle.ProductoPresentacion.Equivalencia));
                                   }
                                   */
                        }

                        detalles.Add(cotizacionDetalle);
                    } 
                    else
                    {
                        cotizacion.productosInactivosRemovidos = true;
                    }

                }

                cotizacion.cotizacionDetalleList = detalles;

            }
            return cotizacion;
        }






        public void RegistroSolicitudExtensionVigencia(Cotizacion cotizacion)
        {
            using (var dal = new CotizacionDAL())
            {
                cotizacion.seguimientoCotizacion.observacion = "[" + DateTime.Now.ToString("dd.MM.yyyy") + " Extensión de vigencia solicitada por " + cotizacion.usuario.nombre
                           + ". Nueva vigencia: " + (cotizacion.fechaFinVigenciaPreciosExtendida == null ? "Indefinida" : cotizacion.fechaFinVigenciaPreciosExtendida.Value.ToString("dd.MM.yyyy")) 
                           + "] " + cotizacion.seguimientoCotizacion.observacion;
                dal.RegistroSolicitudExtensionVigencia(cotizacion);
            }
        }


        public void AprobarSolicitudExtensionVigencia(Cotizacion cotizacion)
        {
            using (var dal = new CotizacionDAL())
            {
                cotizacion.seguimientoCotizacion.observacion = "[" + DateTime.Now.ToString("dd.MM.yyyy") + " Vigencia extendida solicitada por " + cotizacion.usuario.nombre
                           + ". Nueva vigencia: " + (cotizacion.fechaFinVigenciaPreciosExtendida == null ? "Indefinida" : cotizacion.fechaFinVigenciaPreciosExtendida.Value.ToString("dd.MM.yyyy"))
                           + "] " + cotizacion.seguimientoCotizacion.observacion;
                dal.RegistroSolicitudExtensionVigencia(cotizacion);
            }
        }


        public List<Cotizacion> GetCotizaciones(Cotizacion cotizacion)
        {
            List<Cotizacion> cotizacionList = null;
            using (var dal = new CotizacionDAL())
            {
                //Si el usuario no es aprobador entonces solo buscará sus cotizaciones
           /*     if (!cotizacion.usuario.apruebaCotizaciones)
                {
                    cotizacion.usuarioBusqueda = cotizacion.usuario;
                }*/
  
                cotizacionList = dal.SelectCotizaciones(cotizacion);
            }
            return cotizacionList;
        }

   
        public List<SeguimientoCotizacion> GetHistorialSeguimiento(Guid idCotizacion)
        {
            List<SeguimientoCotizacion> seguimientoList = new List<SeguimientoCotizacion>();
            using (var dal = new CotizacionDAL())
            {
                seguimientoList = dal.GetHistorialSeguimiento(idCotizacion);
            }
            return seguimientoList;
        }

    }
}

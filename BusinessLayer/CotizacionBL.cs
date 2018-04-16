﻿
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;

namespace BusinessLayer
{
    public class CotizacionBL
    {

        private void validarCotizacion(Cotizacion cotizacion)
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

            foreach (CotizacionDetalle cotizacionDetalle in cotizacion.cotizacionDetalleList)
            {
                cotizacionDetalle.idCotizacion = cotizacion.idCotizacion;
                cotizacionDetalle.usuario = cotizacion.usuario;

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

                if (!cotizacion.usuario.apruebaCotizaciones || cotizacionDetalle.porcentajeDescuento > cotizacion.usuario.maximoPorcentajeDescuentoAprobacion)
                {
                    PrecioClienteProducto precioClienteProducto = cotizacionDetalle.producto.precioClienteProducto;


                    bool evaluarDescuento = false;
                    //¿Tiene precio registrado para facturación? y eliente es el mismo?
                    if (precioClienteProducto.idPrecioClienteProducto != Guid.Empty && precioClienteProducto.cliente.idCliente == cotizacion.cliente.idCliente )
                    {
                        //Si el precio unitario registrado es distinto del precio registrado para facturacion
                        //Se envia a evaluar Descuento
                        if (cotizacionDetalle.precioUnitario != precioClienteProducto.precioUnitario)
                        {
                            evaluarDescuento = true;
                        }
                        //Si el precio es igual pero la fecha de fin de vigencia es indefinida
                        //Se envia a evaluar Descuento
                        else if (precioClienteProducto.fechaFinVigencia == null && DateTime.Now > precioClienteProducto.fechaInicioVigencia.Value.AddDays(Constantes.DIAS_MAX_VIGENCIA_PRECIOS_COTIZACION) )
                        {
                            evaluarDescuento = true;
                        }


                    }
                    else
                    {
                        evaluarDescuento = true;
                    }

                    if (evaluarDescuento)
                    {
                        if (cotizacionDetalle.porcentajeDescuento > Constantes.PORCENTAJE_MAX_APROBACION)
                        {
                            cotizacion.seguimientoCotizacion.observacion = "Se aplicó un descuento superior al permitido en el detalle de la cotización (es posible que los precios del detalle no coincidan con los precios registrados para facturación).";
                            cotizacion.seguimientoCotizacion.estado = SeguimientoCotizacion.estadosSeguimientoCotizacion.Pendiente;
                        }
                    }                   
                }


            }



            if (cotizacion.fechaEsModificada && !cotizacion.usuario.apruebaCotizaciones)
            {
                cotizacion.seguimientoCotizacion.observacion = cotizacion.seguimientoCotizacion.observacion + "\nSe modificó la fecha de la cotización.";
                cotizacion.seguimientoCotizacion.estado = SeguimientoCotizacion.estadosSeguimientoCotizacion.Pendiente;
            }
            else
            {
                //Si la fecha no es modificada expresamente entonces toma la fecha del sistema
                cotizacion.fecha = DateTime.Now;
            }

            if ((cotizacion.fechaInicioVigenciaPrecios != null || cotizacion.fechaFinVigenciaPrecios != null) && !cotizacion.usuario.apruebaCotizaciones)
            {
                cotizacion.seguimientoCotizacion.observacion = cotizacion.seguimientoCotizacion.observacion + "\nSe modificó la fecha de inicio y/o fin de vigencia de precios.";
                cotizacion.seguimientoCotizacion.estado = SeguimientoCotizacion.estadosSeguimientoCotizacion.Pendiente;
            }

            if (cotizacion.seguimientoCotizacion.estado == SeguimientoCotizacion.estadosSeguimientoCotizacion.Aprobada)
            {
                cotizacion.seguimientoCotizacion.observacion = null;
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

        public void UpdateCotizacion(Cotizacion cotizacion)
        {
            using (var dal = new CotizacionDAL())
            {
                validarCotizacion(cotizacion);
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

        public Cotizacion obtenerProductosAPartirdePreciosRegistrados(Cotizacion cotizacion, String familia, String proveedor)
        {
            using (var dal = new CotizacionDAL())
            {
                cotizacion = dal.obtenerProductosAPartirdePreciosRegistrados(cotizacion, familia, proveedor);

                foreach (CotizacionDetalle cotizacionDetalle in cotizacion.cotizacionDetalleList)
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
                    if (cotizacion.ciudad.esProvincia)
                    {
                        cotizacionDetalle.porcentajeDescuento = 100 - (cotizacionDetalle.precioNetoEquivalente * 100 / cotizacionDetalle.producto.precioSinIgv);
                        cotizacionDetalle.producto.precioSinIgv = cotizacionDetalle.producto.precioProvinciaSinIgv;
                    }
                    else
                    {
                        cotizacionDetalle.porcentajeDescuento = 100 - (cotizacionDetalle.precioNetoEquivalente * 100 / cotizacionDetalle.producto.precioSinIgv);
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
                        //Se agrega el IGV al precioLista
                        cotizacionDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cotizacionDetalle.producto.precioSinIgv));
                        //Se agrega el IGV al costoLista
                        cotizacionDetalle.producto.costoLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cotizacionDetalle.producto.costoSinIgv));
                    }
                    

                }


            }
            return cotizacion;
        }



        public Cotizacion GetCotizacion(Cotizacion cotizacion)
        {
            using (var dal = new CotizacionDAL())
            {
                cotizacion = dal.SelectCotizacion(cotizacion);

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

                foreach (CotizacionDetalle cotizacionDetalle in cotizacion.cotizacionDetalleList)
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
                            cotizacionDetalle.costoAnterior = cotizacionDetalle.costoAnterior + (cotizacionDetalle.costoAnterior * Constantes.IGV);
                            cotizacionDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, precioNeto));
                        }
                        else
                        {
                            cotizacionDetalle.producto.costoLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, costo));
                            cotizacionDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, precioNeto));
                        }


                        //Se calcula el descuento para el nuevo precioNetoEquivalente
                        if (cotizacionDetalle.porcentajeDescuento != 0)
                        {
                            precioNeto = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, (precioNeto * (100 - cotizacionDetalle.porcentajeDescuento)/100)));
                        }

                        
                        cotizacionDetalle.precioNeto = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, precioNeto));



                        

                        if (cotizacionDetalle.esPrecioAlternativo)
                        {
                            cotizacionDetalle.costoAnterior = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cotizacionDetalle.costoAnterior / cotizacionDetalle.producto.equivalencia));
                        }


                      
                    }
                    else
                    {
                        //Si NO es recotizacion
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


            }
            return cotizacion;
        }



        public List<Cotizacion> GetCotizaciones(Cotizacion cotizacion)
        {
            List<Cotizacion> cotizacionList = null;
            using (var dal = new CotizacionDAL())
            {
                //Si el usuario no es aprobador entonces solo buscará sus cotizaciones
                if (!cotizacion.usuario.apruebaCotizaciones)
                {
                    cotizacion.usuarioBusqueda = cotizacion.usuario;
                }
  
                cotizacionList = dal.SelectCotizaciones(cotizacion);
            }
            return cotizacionList;
        }

   

    }
}

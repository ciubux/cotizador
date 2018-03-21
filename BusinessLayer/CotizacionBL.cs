
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;

namespace BusinessLayer
{
    public class CotizacionBL
    {
        public void InsertCotizacion(Cotizacion cotizacion)
        {
            using (var dal = new CotizacionDAL())
            {
                //Si no se consideran cantidades no se debe grabar el subtotal
                if (cotizacion.considerarCantidades == Cotizacion.OpcionesConsiderarCantidades.Observaciones)
                {
                    cotizacion.montoSubTotal = 0;
                }

                if (cotizacion.mostrarValidezOfertaEnDias == 0)
                {
                    cotizacion.fechaFinVigenciaPrecios = cotizacion.fecha.AddDays(cotizacion.validezOfertaEnDias);
                }

                cotizacion.seguimientoCotizacion.estado = SeguimientoCotizacion.estadosSeguimientoCotizacion.Aprobada;

                foreach (CotizacionDetalle cotizacionDetalle in cotizacion.cotizacionDetalleList)
                {
                    cotizacionDetalle.idCotizacion = cotizacion.idCotizacion;
                    cotizacionDetalle.usuario = cotizacion.usuario;

                    //Si no se consideran cantidades no se debe grabar la cantidad y el subtotal
                    if (cotizacion.considerarCantidades == Cotizacion.OpcionesConsiderarCantidades.Observaciones)
                    {
                        cotizacionDetalle.cantidad = 0;
                        //cotizacionDetalle.subTotal = 0;
                    }



                   

                    //Si no es aprobador para que la cotización se cree como aprobada el porcentaje de descuento debe ser mayor o igual 
                    //al porcentaje Limite sin aprobacion


                    if (!cotizacion.usuario.esAprobador)
                    {
                        if (cotizacionDetalle.porcentajeDescuento > Constantes.porcentajeLimiteSinAprobacion)
                        {
                            cotizacion.seguimientoCotizacion.estado = SeguimientoCotizacion.estadosSeguimientoCotizacion.Denegada;
                        }
                    }
                    //Si es aprobador, no debe sobrepasar su limite de aprobación asignado
                    else
                    {
                        if (cotizacionDetalle.porcentajeDescuento > cotizacion.usuario.maximoPorcentajeDescuentoAprobacion)
                        {
                            cotizacion.seguimientoCotizacion.estado = SeguimientoCotizacion.estadosSeguimientoCotizacion.Denegada;
                            
                        }

                    }

                }
               
                dal.InsertCotizacion(cotizacion);
            }
        }

        public void UpdateCotizacion(Cotizacion cotizacion)
        {
            using (var dal = new CotizacionDAL())
            {
                cotizacion.seguimientoCotizacion.estado = SeguimientoCotizacion.estadosSeguimientoCotizacion.Aprobada;
                //Si no se consideran cantidades no se debe grabar el subtotal
                if (cotizacion.considerarCantidades == Cotizacion.OpcionesConsiderarCantidades.Observaciones)
                {
                    cotizacion.montoSubTotal = 0;
                }

                if (cotizacion.mostrarValidezOfertaEnDias == 0)
                {
                    cotizacion.fechaLimiteValidezOferta = cotizacion.fecha.AddDays(cotizacion.validezOfertaEnDias);
                }

                foreach (CotizacionDetalle cotizacionDetalle in cotizacion.cotizacionDetalleList)
                {
                    cotizacionDetalle.idCotizacion = cotizacion.idCotizacion;
                    cotizacionDetalle.usuario = cotizacion.usuario;

                    //Si no se consideran cantidades no se debe grabar la cantidad y el subtotal
                    if (cotizacion.considerarCantidades == Cotizacion.OpcionesConsiderarCantidades.Observaciones)
                    {
                        cotizacionDetalle.cantidad = 0;
                        //cotizacionDetalle.subTotal = 0;
                    }

                    if (!cotizacion.usuario.esAprobador)
                    {
                        if (cotizacionDetalle.porcentajeDescuento > Constantes.porcentajeLimiteSinAprobacion)
                        {
                            cotizacion.seguimientoCotizacion.estado = SeguimientoCotizacion.estadosSeguimientoCotizacion.Denegada;
                        }
                    }
                    //Si es aprobador, no debe sobrepasar su limite de aprobación asignado
                    else
                    {
                        if (cotizacionDetalle.porcentajeDescuento > cotizacion.usuario.maximoPorcentajeDescuentoAprobacion)
                        {
                            cotizacion.seguimientoCotizacion.estado = SeguimientoCotizacion.estadosSeguimientoCotizacion.Denegada;

                        }

                    }
                }
                dal.UpdateCotizacion(cotizacion);
            }
        }

        public void cambiarEstadoCotizacion(Cotizacion cotizacion)
        {
            using (var dal = new CotizacionDAL())
            {
                dal.insertSeguimientoCotizacion(cotizacion);
            }
             
        }

        public Cotizacion GetCotizacion(Cotizacion cotizacion)
        {
            using (var dal = new CotizacionDAL())
            {
                cotizacion = dal.SelectCotizacion(cotizacion);

                if (cotizacion.mostrarValidezOfertaEnDias == 0)
                {
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

                    



                    //Si es RECOTIZACIÓN se calcula el nuevo precioNeto a partir del precioSinIGV del producto, IGV y FLETE
                    //dependiendo de si fue o no indicado en la cabecera
                    if (cotizacion.esRecotizacion)
                    {
                        Decimal precioNeto = cotizacionDetalle.producto.precioSinIgv;
                        Decimal costo = cotizacionDetalle.producto.costoSinIgv;

                        //Ya no agrega al flete al precio neto
                      /*  if (cotizacion.flete > 0)
                        {
                            precioNeto = precioNeto + (precioNeto * cotizacion.flete / 100);
                        }*/

                        //Se agrega el igv al costo y al precio neto que se obtuvo directamente del producto
                        if (cotizacion.incluidoIgv)
                        {
                            precioNeto = precioNeto + (precioNeto * Constantes.IGV);
                            costo = costo + (costo * Constantes.IGV);

                            cotizacionDetalle.producto.costoLista = costo;
                            //Tambien se agrega el IGV al costo Anterior, dado que fue almacenado sin IGV
                            cotizacionDetalle.costoAnterior = cotizacionDetalle.costoAnterior + (cotizacionDetalle.costoAnterior * Constantes.IGV);
                            cotizacionDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.decimalFormat, precioNeto));
                        }
                        else
                        {
                            cotizacionDetalle.producto.costoLista = Decimal.Parse(String.Format(Constantes.decimalFormat, costo));
                            cotizacionDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.decimalFormat, precioNeto));
                        }


                        //Se calcula el descuento para el nuevo precioNeto
                        if (cotizacionDetalle.porcentajeDescuento != 0)
                        {
                            precioNeto = Decimal.Parse(String.Format(Constantes.decimalFormat, (precioNeto * (100 - cotizacionDetalle.porcentajeDescuento)/100)));
                        }

                        
                        cotizacionDetalle.precioNeto = Decimal.Parse(String.Format(Constantes.decimalFormat, precioNeto));






                        if (cotizacionDetalle.esPrecioAlternativo)
                        {
                            cotizacionDetalle.costoAnterior = Decimal.Parse(String.Format(Constantes.decimalFormat, cotizacionDetalle.costoAnterior / cotizacionDetalle.producto.equivalencia));
                         }


                      
                    }
                    else
                    {
                        //Si NO es recotizacion
                        if (cotizacion.incluidoIgv)
                        {
                            //Se agrega el IGV al precioLista
                            decimal precioSinIgv = cotizacionDetalle.producto.precioSinIgv;
                            decimal precioLista = precioSinIgv + (precioSinIgv * cotizacion.igv);
                            cotizacionDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.decimalFormat, precioLista));
                            //Se agrega el IGV al costoLista
                            decimal costoSinIgv = cotizacionDetalle.producto.costoSinIgv;
                            decimal costoLista = costoSinIgv + (costoSinIgv * cotizacion.igv);
                            cotizacionDetalle.producto.costoLista = Decimal.Parse(String.Format(Constantes.decimalFormat, costoLista));
                        }
                        else
                        {
                            //Se agrega el IGV al precioLista
                            cotizacionDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.decimalFormat, cotizacionDetalle.producto.precioSinIgv));
                            //Se agrega el IGV al costoLista
                            cotizacionDetalle.producto.costoLista = Decimal.Parse(String.Format(Constantes.decimalFormat, cotizacionDetalle.producto.costoSinIgv));
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
                if (!cotizacion.usuario.esAprobador)
                {
                    cotizacion.usuarioBusqueda = cotizacion.usuario;
                }
  
                cotizacionList = dal.SelectCotizaciones(cotizacion);
            }
            return cotizacionList;
        }




    }
}


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
                if (!cotizacion.considerarCantidades)
                {
                    cotizacion.montoSubTotal = 0;
                }

                cotizacion.estadoAprobacion = 1;

                if (cotizacion.tipoVigencia == 0)
                {
                    cotizacion.fechaVigenciaLimite = cotizacion.fecha.AddDays(cotizacion.diasVigencia);
                }

                foreach (CotizacionDetalle cotizacionDetalle in cotizacion.cotizacionDetalleList)
                {
                    cotizacionDetalle.idCotizacion = cotizacion.idCotizacion;
                    cotizacionDetalle.usuario = cotizacion.usuario;

                    //Si no se consideran cantidades no se debe grabar la cantidad y el subtotal
                    if (!cotizacion.considerarCantidades)
                    {
                        cotizacionDetalle.cantidad = 0;
                        //cotizacionDetalle.subTotal = 0;
                    }

                    


                    if(!cotizacion.usuario.esAprobador)
                    { 
                        if (cotizacionDetalle.porcentajeDescuento > Constantes.porcentajeLimiteSinAprobacion)
                        {
                            cotizacion.estadoAprobacion = 0;
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
                cotizacion.estadoAprobacion = 1;
                //Si no se consideran cantidades no se debe grabar el subtotal
                if (!cotizacion.considerarCantidades)
                {
                    cotizacion.montoSubTotal = 0;
                }

                if (cotizacion.tipoVigencia == 0)
                {
                    cotizacion.fechaVigenciaLimite = cotizacion.fecha.AddDays(cotizacion.diasVigencia);
                }

                foreach (CotizacionDetalle cotizacionDetalle in cotizacion.cotizacionDetalleList)
                {
                    cotizacionDetalle.idCotizacion = cotizacion.idCotizacion;
                    cotizacionDetalle.usuario = cotizacion.usuario;

                    //Si no se consideran cantidades no se debe grabar la cantidad y el subtotal
                    if (!cotizacion.considerarCantidades)
                    {
                        cotizacionDetalle.cantidad = 0;
                        //cotizacionDetalle.subTotal = 0;
                    }

                    if (!cotizacion.usuario.esAprobador)
                    {
                        if (cotizacionDetalle.porcentajeDescuento > Constantes.porcentajeLimiteSinAprobacion)
                        {
                            cotizacion.estadoAprobacion = 0;
                        }
                    }
                }
                dal.UpdateCotizacion(cotizacion);
            }
        }

        public Cotizacion aprobarCotizacion(Cotizacion cotizacion)
        {
            using (var dal = new CotizacionDAL())
            {
                cotizacion = dal.aprobarCotizacion(cotizacion);
            }
            return cotizacion;
        }

        public Cotizacion GetCotizacion(Cotizacion cotizacion)
        {
            using (var dal = new CotizacionDAL())
            {
                cotizacion = dal.SelectCotizacion(cotizacion);

                if (cotizacion.tipoVigencia == 0)
                {
                    TimeSpan diferencia;
                    diferencia = cotizacion.fechaVigenciaLimite - cotizacion.fecha;
                    cotizacion.diasVigencia = diferencia.Days; 
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


                        //Si la cabecera tiene flete se agrega el flete al precioLista
                   /*     if (cotizacion.flete > 0)
                        {
                            decimal precioSinFlete = cotizacionDetalle.producto.precioLista;
                            decimal precioLista = precioSinFlete + (precioSinFlete * cotizacion.flete);
                            cotizacionDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.decimalFormat, precioLista));
                        }*/

                 /*       //Si se ha trabajado con el precioAlternativo, se divide el precioLista entre la equivalencia
                        if (cotizacionDetalle.esPrecioAlternativo)
                        {
                            cotizacionDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.decimalFormat, cotizacionDetalle.producto.precioLista / cotizacionDetalle.producto.equivalencia));
                            cotizacionDetalle.producto.costoLista = Decimal.Parse(String.Format(Constantes.decimalFormat, cotizacionDetalle.producto.costoLista / cotizacionDetalle.producto.equivalencia));
                        }*/

                        //El precioNeto se obtuvodirectamente  cotizacionDetalle.precioNeto = 
                       // cotizacionDetalle.costo = cotizacionDetalle.producto.costoLista;
                    }




                    //cotizacionDetalle.subTotal = cotizacionDetalle.precioNeto * cotizacionDetalle.cantidad;

                }


            }
            return cotizacion;
        }



        public List<Cotizacion> GetCotizaciones(Cotizacion cotizacion)
        {
            List<Cotizacion> cotizacionList = null;
            using (var dal = new CotizacionDAL())
            {
                cotizacionList = dal.SelectCotizaciones(cotizacion);
            }
            return cotizacionList;
        }




    }
}

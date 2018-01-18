
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

                foreach (CotizacionDetalle cotizacionDetalle in cotizacion.cotizacionDetalleList)
                {
                    cotizacionDetalle.idCotizacion = cotizacion.idCotizacion;
                    cotizacionDetalle.usuario = cotizacion.usuario;

                    //Si no se consideran cantidades no se debe grabar la cantidad y el subtotal
                    if (!cotizacion.considerarCantidades)
                    {
                        cotizacionDetalle.cantidad = 0;
                        cotizacionDetalle.subTotal = 0;
                    }

                   
                }



                dal.InsertCotizacion(cotizacion);

               
            }
        }

        public Cotizacion GetCotizacion(Cotizacion cotizacion)
        {
            using (var dal = new CotizacionDAL())
            {
                cotizacion = dal.SelectCotizacion(cotizacion);

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

                    if (cotizacion.incluidoIgv)
                    {
                        decimal precioSinIgv = cotizacionDetalle.producto.precioSinIgv;
                        decimal precioLista = precioSinIgv + (precioSinIgv * cotizacion.igv);
                        cotizacionDetalle.producto.precioLista = precioLista;
                    }
                    else
                    {
                        cotizacionDetalle.producto.precioLista = cotizacionDetalle.producto.precioSinIgv;
                    }

                    cotizacionDetalle.subTotal = cotizacionDetalle.precio * cotizacionDetalle.cantidad;

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

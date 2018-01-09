
using DataLayer;
using System.Collections.Generic;
using System;
using Model;

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
                    cotizacion.subtotal = 0;
                }

                dal.InsertCotizacion(cotizacion);

                foreach (CotizacionDetalle cotizacionDetalle in cotizacion.detalles)
                {
                    cotizacionDetalle.idCotizacion = cotizacion.idCotizacion;
                    cotizacionDetalle.usuario = cotizacion.usuario;

                    //Si no se consideran cantidades no se debe grabar la cantidad y el subtotal
                    if (!cotizacion.considerarCantidades)
                    {
                        cotizacionDetalle.cantidad = 0;
                        cotizacionDetalle.subTotal = 0;
                    }

                    dal.InsertCotizacionDetalle(cotizacionDetalle);
                }
            }
        }

    }
}

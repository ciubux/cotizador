
using DataLayer;
using System.Collections.Generic;
using System;
using Model;

namespace BusinessLayer
{
    public class CotizacionBL
    {
        public void InsertCotizacion(Cotizacion obj)
        {
            using (var dal = new CotizacionDAL())
            {
                dal.InsertCotizacion(obj);

                foreach (CotizacionDetalle det in obj.detalles)
                {
                    dal.InsertCotizacionDetalle(det);
                }
            }
        }

    }
}

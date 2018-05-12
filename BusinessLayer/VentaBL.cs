
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;

namespace BusinessLayer
{
    public class VentaBL
    {
         

        public void UpdateVenta(Venta venta)
        {
            using (var dal = new VentaDAL())
            {
                dal.UpdateVenta(venta);
            }
        }


    }
}

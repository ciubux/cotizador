
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;

namespace BusinessLayer
{
    public class PrecioListaBL
    {
       
        public void setPrecioListaStaging(PrecioListaStaging precioListaStaging)
        {
            using (var precioListaDAL = new PrecioListaDAL())
            {
                precioListaDAL.setPrecioListaStaging(precioListaStaging);
            }
        }

        public void truncatePrecioListaStaging()
        {
            using (var precioListaDAL = new PrecioListaDAL())
            {
                precioListaDAL.truncatePrecioListaStaging();
            }
        }

        public void mergePrecioListaStaging()
        {
            using (var precioListaDAL = new PrecioListaDAL())
            {
                precioListaDAL.mergePrecioListaStaging();
            }
        }
    }
}

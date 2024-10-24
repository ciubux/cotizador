﻿
using DataLayer;
using System.Collections.Generic;
using System;
using Model;

namespace BusinessLayer
{
    public class PrecioBL
    {
      /*  public List<PrecioClienteProducto> getListas()
        {
            using (var dal = new PrecioListaDAL())
            {
                return dal.getListas();
            }
        }

        public List<PrecioClienteProducto> getPreciosProducto(Guid idProducto, Guid idMoneda)
        {
            using (var dal = new PrecioListaDAL())
            {
                return dal.getPreciosProducto(idProducto, idMoneda);

            }
        }

        public PrecioClienteProducto getPrecioProducto(Guid idProducto, Guid idPrecioLista)
        {
            using (var dal = new PrecioListaDAL())
            {
                return dal.getPrecioProducto(idProducto, idPrecioLista);

            }
        }*/

        public void setPrecioStaging(PrecioStaging precioStaging)
        {
            using (var precioDAL = new PrecioDAL())
            {
                precioDAL.setPrecioStaging(precioStaging);
            }
        }

        public void truncatePrecioStaging(String sede)
        {
            using (var precioDAL = new PrecioDAL())
            {
                precioDAL.truncatePrecioStaging(sede);
            }
        }

        public void mergePrecioStaging()
        {
            using (var precioDAL = new PrecioDAL())
            {
                precioDAL.mergePrecioStaging();
            }
        }

    }
}

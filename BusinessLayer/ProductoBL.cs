
using DataLayer;
using System.Collections.Generic;
using System;
using Model;

namespace BusinessLayer
{
    public class ProductoBL
    {
        public List<Producto> getProductosBusqueda(Guid idProveedor, Guid idFamilia, String textoBusqueda)
        {
            using (var dal = new ProductoDAL())
            {
                return dal.getProductosBusqueda(idProveedor, idFamilia, textoBusqueda);
            }
        }

        public Producto getProducto(Guid idProducto)
        {
            using (var dal = new ProductoDAL())
            {
                return dal.getProducto(idProducto);
            }
        }
    }
}

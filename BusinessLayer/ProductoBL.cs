
using DataLayer;
using System.Collections.Generic;
using System;
using Model;

namespace BusinessLayer
{
    public class ProductoBL
    {
        public List<Producto> getProductosBusqueda(String textoBusqueda)
        {
            using (var dal = new ProductoDAL())
            {
                return dal.getProductosBusqueda(textoBusqueda);
            }
        }

        public Producto getProducto(Guid idProducto, Boolean esProvincia)
        {
            using (var dal = new ProductoDAL())
            {
                Producto producto = dal.getProducto(idProducto, esProvincia);                
                return producto;
            }
        }
    }
}

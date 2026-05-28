
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;

namespace BusinessLayer
{
    public class ProductoControlStockBL
    {
        public List<ProductoControlStock> SelectProductosControlStock(Guid idUsuario, int estado, Guid idCiudad, string sku, string proveedor, string familia, bool stockVerde, bool stockAmbar, bool stockRojo)
        {
            using (ProductoControlStockDAL dal = new ProductoControlStockDAL())
            {
                return dal.SelectProductosControlStock(idUsuario, estado, idCiudad, sku, proveedor, familia, stockVerde, stockAmbar, stockRojo);
            }
        }

        public void CalcularControlStock(Guid idUsuario, Guid idCiudad, List<Guid> controlStockIds)
        {
            using (ProductoControlStockDAL dal = new ProductoControlStockDAL())
            {
                dal.CalcularControlStock(idUsuario, idCiudad, controlStockIds);
            }
        }
    }
}

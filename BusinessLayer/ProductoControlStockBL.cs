
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;

namespace BusinessLayer
{
    public class ProductoControlStockBL
    {
        public List<ProductoControlStock> SelectProductosControlStock(Guid idUsuario, int estado, Guid idCiudad)
        {
            using (ProductoControlStockDAL dal = new ProductoControlStockDAL())
            {
                return dal.SelectProductosControlStock(idUsuario, estado, idCiudad);
            }
        }
    }
}

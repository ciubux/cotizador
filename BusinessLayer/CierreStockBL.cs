
using DataLayer;
using System.Collections.Generic;
using System;
using Model;

namespace BusinessLayer
{
    public class CierreStockBL
    {
        public CierreStock SelectCierreStock(Guid idUsuario, Guid idCierreStock)
        {
            using (CierreStockDAL dal = new CierreStockDAL())
            {
                return dal.SelectCierreStock(idUsuario, idCierreStock);
            }
        }

        public void GenerarReporteValidacionStock(Guid idUsuario, Guid idCierreStock)
        {
            using (CierreStockDAL dal = new CierreStockDAL())
            {
                dal.GenerarReporteValidacionStock(idUsuario, idCierreStock);
            }
        }
    }
}

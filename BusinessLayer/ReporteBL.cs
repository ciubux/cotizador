
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using Model.UTILES;
using System.IO;

namespace BusinessLayer
{
    public class ReporteBL
    {

        public List<FilaProductoPendienteAtencion> productosPendientesAtencion(String sku, String familia, String proveedor, DateTime fechaInicio, DateTime fechaFin, Guid idCiudad, Guid idUsuario)
        {
            using (var dal = new ReporteDAL())
            {
                List<FilaProductoPendienteAtencion> list = dal.productosPendientesAtencion(sku, familia, proveedor, fechaInicio, fechaFin, idCiudad, idUsuario);

                return list;
            }
        }
    }
}


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

        public List<List<String>> sellOutVendedores(String sku, String familia, String proveedor, String codVRC, String codVSC, String codVAC,
            DateTime fechaInicio, DateTime fechaFin, int anio, int trimestre, String ciudad, int incluirVentasExcluidas, Guid idUsuario,
            int idGrupo, string ruc)
        {
            using (var dal = new ReporteDAL())
            {
                List<List<String>> list = dal.sellOutVendedores(sku, familia, proveedor, codVRC, codVSC, codVAC, fechaInicio, fechaFin, 
                                                anio, trimestre, ciudad, incluirVentasExcluidas, idUsuario, idGrupo, ruc);

                return list;
            }
        }

        public List<List<String>> sellOutVendedoresDetalles(String codVendedor, String sku, String familia, String proveedor, String codVRC, String codVSC, String codVAC, 
            DateTime fechaInicio, DateTime fechaFin, int anio, int trimestre, String ciudad, int incluirVentasExcluidas, Guid idUsuario,
            int idGrupo, string ruc)
        {
            using (var dal = new ReporteDAL())
            {
                List<List<String>> list = dal.sellOutVendedoresDetalles(codVendedor, sku, familia, proveedor, codVRC, codVSC, codVAC, 
                                                fechaInicio, fechaFin, anio, trimestre, ciudad, incluirVentasExcluidas, idUsuario, idGrupo, ruc);

                return list;
            }
        }
    }
}

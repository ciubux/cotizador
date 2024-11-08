
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;
using Framework.DAL;
using System.Data.SqlClient;
using System.Data;

namespace BusinessLayer
{
    public class EmpresaDescuentoBL
    {
        public List<EmpresaDescuento> Listar(int idEmpresa, Guid idUsuario, Guid idCiudad, int estado = -1)
        {
            using (EmpresaDescuentoDAL dal = new EmpresaDescuentoDAL())
            {
                return dal.Listar(idEmpresa, idUsuario, idCiudad, estado);
            }
        }

        public bool CargaMasiva(Guid idUsuario, int idEmpresa, List<EmpresaDescuento> lista)
        {
            using (EmpresaDescuentoDAL dal = new EmpresaDescuentoDAL())
            {
                return dal.CargaMasiva(idUsuario, idEmpresa, lista);
            }
        }
    }
}

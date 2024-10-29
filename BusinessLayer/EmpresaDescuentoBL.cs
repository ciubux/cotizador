
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;

namespace BusinessLayer
{
    public class EmpresaDescuentoBL
    {
       
        public List<EmpresaDescuento> Listar(int idEmpresa, Guid idUsuario, Guid idCiudad, int estado = -1)
        {
            using (var dal = new EmpresaDescuentoDAL())
            {
                return dal.Listar(idEmpresa, idUsuario, idCiudad, estado);
            }
        }
    }
}

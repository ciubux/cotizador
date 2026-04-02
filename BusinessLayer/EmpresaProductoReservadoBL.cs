
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
    public class EmpresaProductoReservadoBL
    {
        public List<EmpresaProductoReservado> ValidarProductos(int idEmpresa, Guid idUsuario, List<Guid> idsProductos, int estado = -1)
        {
            using (EmpresaProductoReservadoDAL dal = new EmpresaProductoReservadoDAL())
            {
                return dal.ValidarProductos(idEmpresa, idUsuario, idsProductos, estado);
            }
        }
    }
}

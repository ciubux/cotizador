
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
    public class EmpresaProductoSeparadoBL
    {
        public List<EmpresaProductoSeparado> ValidarProductos(Guid idUsuario, Guid idCiudad, List<Guid> idsProductos, int estado = -1)
        {
            using (EmpresaProductoSeparadoDAL dal = new EmpresaProductoSeparadoDAL())
            {
                return dal.ValidarProductos(idUsuario, idCiudad, idsProductos, estado);
            }
        }
    }
}

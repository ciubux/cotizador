
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;

namespace BusinessLayer
{
    public class PrecioClienteProductoBL
    {
    

        public List<PrecioClienteProducto> getPreciosRegistrados(Guid idProducto, Guid idCliente)
        {
            using (var dal = new PrecioClienteProductoDAL())
            {
                List<PrecioClienteProducto> precioClienteProductoList = dal.getPreciosRegistrados(idProducto, idCliente);


                return precioClienteProductoList;
            }
        }

        public List<PrecioClienteProducto> getPreciosRegistradosGrupo(Guid idProducto, int idGrupoCliente)
        {
            using (var dal = new PrecioClienteProductoDAL())
            {
                List<PrecioClienteProducto> precioGrupoClienteProductoList = dal.getPreciosRegistradosGrupo(idProducto, idGrupoCliente);


                return precioGrupoClienteProductoList;
            }
        }

    }
}

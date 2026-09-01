
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

        public List<PrecioClienteProducto> getPreciosPuntalesRegistrados(Guid idProducto, Guid idCliente)
        {
            using (var dal = new PrecioClienteProductoDAL())
            {
                List<PrecioClienteProducto> precioClienteProductoList = dal.getPreciosPuntalesRegistrados(idProducto, idCliente);

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

        public List<PrecioClienteProducto> GetPreciosVencidosNoRenovadosPorActualizacionPrecioLista(Guid idUsuario, DateTime fechaDesde, int idResponsableComercial, int idSupervisorComercial, int idAsistenteServicioCliente)
        {
            using (PrecioClienteProductoDAL dal = new PrecioClienteProductoDAL())
            {
                return dal.GetPreciosVencidosNoRenovadosPorActualizacionPrecioLista(idUsuario, fechaDesde, idResponsableComercial, idSupervisorComercial, idAsistenteServicioCliente);
            }
        }
    }
}


using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;
using System.Linq;

namespace BusinessLayer
{
    public class PeriodoSolicitudBL
    {
        public PeriodoSolicitud getPeriodoSolicitud(Guid id, Guid idUsuario,  Usuario usuario)
        {
            using (var dal = new PeriodoSolicitudDAL())
            {
                PeriodoSolicitud obj = dal.getPeriodoSolicitud(id, idUsuario);
                Cliente cliente = usuario.clienteList.Where(c => c.sedePrincipal).FirstOrDefault();

                ClienteBL clienteBl = new ClienteBL();
                //cliente.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                List<DocumentoDetalle> listaPrecios = clienteBl.getPreciosVigentesCliente(cliente.idCliente);

                foreach (DocumentoDetalle precio in listaPrecios)
                {
                    precio.producto.precioClienteProducto.estadoCanasta = obj.canasta.Where(c => c.producto.idProducto == precio.producto.idProducto ).FirstOrDefault() != null;
                }
                obj.canasta = listaPrecios;
                return obj;
            }
        }

        public List<PeriodoSolicitud> getPeriodosSolicitud(PeriodoSolicitud obj)
        {
            using (var dal = new PeriodoSolicitudDAL())
            {
                return dal.getPeriodosSolicitud(obj);
            }
        }


        public List<PeriodoSolicitud> getPeriodosSolicitudVigentes(PeriodoSolicitud obj, bool excluirPeriodosConRequerimientos)
        {
            using (var dal = new PeriodoSolicitudDAL())
            {
                return dal.getPeriodosSolicitudVigentes(obj, excluirPeriodosConRequerimientos);
            }
        }

        public PeriodoSolicitud insertPeriodoSolicitud(PeriodoSolicitud obj)
        {
            using (var dal = new PeriodoSolicitudDAL())
            {
                return dal.insertPeriodoSolicitud(obj);
            }
        }

        public PeriodoSolicitud updatePeriodoSolicitud(PeriodoSolicitud obj)
        {
            using (var dal = new PeriodoSolicitudDAL())
            {
                return dal.updatePeriodoSolicitud(obj);
            }
        }
    }
}

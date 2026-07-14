
using DataLayer;
using System.Collections.Generic;
using System;
using Model;

namespace BusinessLayer
{
    public class ClienteProductoReservadoBL
    {
        public ClienteProductoReservado getClienteProductoReservado(Guid idClienteProductoReservado, Guid idUsuario)
        {
            using (ClienteProductoReservadoDAL dal = new ClienteProductoReservadoDAL())
            {
                ClienteProductoReservado obj = dal.getClienteProductoReservado(idClienteProductoReservado, idUsuario);
                return obj;
            }
        }

        public List<ClienteProductoReservado> getClienteProductosReservados(ClienteProductoReservado obj)
        {
            using (ClienteProductoReservadoDAL dal = new ClienteProductoReservadoDAL())
            {
                return dal.getClienteProductosReservados(obj);
            }
        }

        public ClienteProductoReservado insertClienteProductoReservado(ClienteProductoReservado obj)
        {
            using (ClienteProductoReservadoDAL dal = new ClienteProductoReservadoDAL())
            {
                return dal.insertClienteProductoReservado(obj);
            }
        }

        public ClienteProductoReservado updateClienteProductoReservado(ClienteProductoReservado obj)
        {
            using (ClienteProductoReservadoDAL dal = new ClienteProductoReservadoDAL())
            {
                return dal.updateClienteProductoReservado(obj);
            }
        }

        public void InsertClientesProductosReservadosMasivo(Guid idUsuario, Guid idCiudad, List<ClienteProductoReservado> lista)
        {
            using (ClienteProductoReservadoDAL dal = new ClienteProductoReservadoDAL())
            {
                dal.InsertClientesProductosReservadosMasivo(idUsuario, idCiudad, lista);
            }
        }

        public void AgregarCantidadReserva(Guid idClienteProductoReservado, int cantidadAgregar, Guid idUsuario)
        {
            using (ClienteProductoReservadoDAL dal = new ClienteProductoReservadoDAL())
            {
                dal.AgregarCantidadReserva(idClienteProductoReservado, cantidadAgregar, idUsuario);
            }
        }

        public void InsertSolicitudesRecargaReserva(Guid idUsuario, List<ClienteProductoReservadoSolicitud> lista)
        {
            using (ClienteProductoReservadoDAL dal = new ClienteProductoReservadoDAL())
            {
                dal.InsertSolicitudesRecargaReserva(idUsuario, lista);
            }
        }

        public List<ClienteProductoReservado> SelectDatosReservaSolicitarRecarga(Guid idUsuario, List<ClienteProductoReservado> reservas)
        {
            using (ClienteProductoReservadoDAL dal = new ClienteProductoReservadoDAL())
            {
                return dal.SelectDatosReservaSolicitarRecarga(idUsuario, reservas);
            }
        }
    }
}


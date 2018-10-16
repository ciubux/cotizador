
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;
using Model.EXCEPTION;

namespace BusinessLayer
{
    public class MovimientoAlmacenBL
    {

        public void AnularMovimientoAlmacen(MovimientoAlmacen movimientoAlmacen)
        {
            using (var dal = new MovimientoALmacenDAL())
            {
                dal.AnularMovimientoAlmacen(movimientoAlmacen);
            }
        }


        public DocumentoVenta obtenerVentaConsolidarFactura(String idMovimientoAlmacenList)
        {
            using (var dal = new MovimientoALmacenDAL())
            {
                return dal.obtenerVentaConsolidarFactura(idMovimientoAlmacenList);
            }
        }

        public void UpdateMarcaNoEntregado(GuiaRemision guiaRemision)
        {
            using (var dal = new MovimientoALmacenDAL())
            {
                dal.UpdateMarcaNoEntregado(guiaRemision);
            }
        }



        public void InsertMovimientoAlmacenSalida(GuiaRemision guiaRemision) 
        {
            using (var dal = new MovimientoALmacenDAL())
            {
                

                guiaRemision.seguimientoMovimientoAlmacenSalida.observacion = String.Empty;
                guiaRemision.seguimientoMovimientoAlmacenSalida.estado = SeguimientoMovimientoAlmacenSalida.estadosSeguimientoMovimientoAlmacenSalida.Enviado;

                Boolean existeCantidadPendienteAtencion = false;
                foreach (DocumentoDetalle documentoDetalle in guiaRemision.pedido.documentoDetalle)
                {
                    if (documentoDetalle.cantidadPendienteAtencion != documentoDetalle.cantidadPorAtender)
                    { 
                        existeCantidadPendienteAtencion = true;
                        break;
                    }

                }


                if (!existeCantidadPendienteAtencion)
                {
                    guiaRemision.atencionParcial = false;
                    guiaRemision.ultimaAtencionParcial = true;
                }                

                try
                {
                dal.InsertMovimientoAlmacenSalida(guiaRemision);
                }
                catch (DuplicateNumberDocumentException ex)
                {
                    throw ex;
                }
            }
        }

        public void InsertMovimientoAlmacenEntrada(NotaIngreso notaIngreso)
        {
            using (var dal = new MovimientoALmacenDAL())
            {
                notaIngreso.seguimientoMovimientoAlmacenEntrada.observacion = String.Empty;
                notaIngreso.seguimientoMovimientoAlmacenEntrada.estado = SeguimientoMovimientoAlmacenEntrada.estadosSeguimientoMovimientoAlmacenEntrada.Recibido;

                Boolean existeCantidadPendienteAtencion = false;
                foreach (DocumentoDetalle documentoDetalle in notaIngreso.documentoDetalle)
                {
                    if (documentoDetalle.cantidadPendienteAtencion != documentoDetalle.cantidadPorAtender)
                    {
                        existeCantidadPendienteAtencion = true;
                        break;
                    }
                }

                if (!existeCantidadPendienteAtencion)
                {
                    notaIngreso.atencionParcial = false;
                    notaIngreso.ultimaAtencionParcial = true;
                }

                try
                {
                    dal.InsertMovimientoAlmacenEntrada(notaIngreso);
                }
                catch (DuplicateNumberDocumentException ex)
                {
                    throw ex;
                }
            }
        }

        public List<GuiaRemision> GetGuiasRemision(GuiaRemision guiaRemision)
        {
            List<GuiaRemision> guiaRemisionList = null;
            using (var dal = new MovimientoALmacenDAL())
            {
                guiaRemision.usuario = guiaRemision.usuario;
                guiaRemisionList = dal.SelectGuiasRemision(guiaRemision);
            }
            return guiaRemisionList;
        }

        public List<NotaIngreso> GetNotasIngreso(NotaIngreso notaIngreso)
        {
            List<NotaIngreso> notaIngresoList = null;
            using (var dal = new MovimientoALmacenDAL())
            {
                // notaIngreso.usuario = notaIngreso.usuario;
                notaIngresoList = dal.SelectNotasIngreso(notaIngreso);
            }
            return notaIngresoList;
        }

        public List<GuiaRemision> GetGuiasRemisionGrupoCliente(GuiaRemision guiaRemision)
        {
            List<GuiaRemision> guiaRemisionList = null;
            using (var dal = new MovimientoALmacenDAL())
            {
                //Si el usuario no es aprobador entonces solo buscará sus cotizaciones
                /*   if (!pedido.usuario.apruebaCotizaciones)
                   {
                       pedido.usuarioBusqueda = pedido.usuario;
                   }*/
                guiaRemision.usuario = guiaRemision.usuario;
                guiaRemisionList = dal.SelectGuiasRemisionGrupoCliente(guiaRemision);
            }
            return guiaRemisionList;
        }

        

        public GuiaRemision GetGuiaRemision(GuiaRemision guiaRemision)
        {
            using (var dal = new MovimientoALmacenDAL())
            {
                guiaRemision = dal.SelectGuiaRemision(guiaRemision);
            }
            return guiaRemision;
        }

        public NotaIngreso GetNotaIngreso(NotaIngreso notaIngreso)
        {
            using (var dal = new MovimientoALmacenDAL())
            {
                notaIngreso = dal.SelectNotaIngreso(notaIngreso);

                if (notaIngreso.guiaRemisionAExtornar.idMovimientoAlmacen == null || notaIngreso.guiaRemisionAExtornar.idMovimientoAlmacen == Guid.Empty)
                {
                    notaIngreso.guiaRemisionAExtornar = null;
                }

            }
            return notaIngreso;
        }

        public void cambiarEstadoPedido(Pedido pedido)
        {
            using (var dal = new PedidoDAL())
            {
                dal.insertSeguimientoPedido(pedido);
            }

        }
    }
}

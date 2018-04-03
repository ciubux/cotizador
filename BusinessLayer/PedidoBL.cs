
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;

namespace BusinessLayer
{
    public class PedidoBL
    {
        public void InsertPedido(Pedido pedido)
        {
            using (var dal = new PedidoDAL())
            {

                pedido.seguimientoPedido.observacion = String.Empty;
                pedido.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.PendienteEnvio;

                foreach (PedidoDetalle pedidoDetalle in pedido.pedidoDetalleList)
                {
                    //pedidoDetalle.idPedido = pedido.idPedido;
                    pedidoDetalle.usuario = pedido.usuario;

                    //Si no es aprobador para que la cotización se cree como aprobada el porcentaje de descuento debe ser mayor o igual 
                    //al porcentaje Limite sin aprobacion


                 /*   if (!pedido.usuario.esAprobador)
                    {
                        if (pedido.porcentajeDescuento > Constantes.PORCENTAJE_MAX_APROBACION)
                        {
                            cotizacion.seguimientoCotizacion.observacion = "Se aplicó un descuento superior al permitido en el detalle de la cotización";
                            cotizacion.seguimientoCotizacion.estado = SeguimientoCotizacion.estadosSeguimientoCotizacion.Pendiente;
                        }
                    }
                    //Si es aprobador, no debe sobrepasar su limite de aprobación asignado
                    else
                    {
                        if (cotizacionDetalle.porcentajeDescuento > cotizacion.usuario.maximoPorcentajeDescuentoAprobacion)
                        {
                            cotizacion.seguimientoCotizacion.observacion = "Se aplicó un descuento superior al permitido en el detalle de la cotización.";
                            cotizacion.seguimientoCotizacion.estado = SeguimientoCotizacion.estadosSeguimientoCotizacion.Pendiente;
                            
                        }

                    }*/

                }
                dal.InsertPedido(pedido);
            }
        }



        public void UpdatePedido(Pedido pedido)
        {
            using (var dal = new PedidoDAL())
            {

                pedido.seguimientoPedido.observacion = String.Empty;
                pedido.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.PendienteEnvio;

                foreach (PedidoDetalle pedidoDetalle in pedido.pedidoDetalleList)
                {
                    pedidoDetalle.idPedido = pedido.idPedido;
                    pedidoDetalle.usuario = pedido.usuario;

                    //Si no es aprobador para que la cotización se cree como aprobada el porcentaje de descuento debe ser mayor o igual 
                    //al porcentaje Limite sin aprobacion


                    /*   if (!pedido.usuario.esAprobador)
                       {
                           if (pedido.porcentajeDescuento > Constantes.PORCENTAJE_MAX_APROBACION)
                           {
                               cotizacion.seguimientoCotizacion.observacion = "Se aplicó un descuento superior al permitido en el detalle de la cotización";
                               cotizacion.seguimientoCotizacion.estado = SeguimientoCotizacion.estadosSeguimientoCotizacion.Pendiente;
                           }
                       }
                       //Si es aprobador, no debe sobrepasar su limite de aprobación asignado
                       else
                       {
                           if (cotizacionDetalle.porcentajeDescuento > cotizacion.usuario.maximoPorcentajeDescuentoAprobacion)
                           {
                               cotizacion.seguimientoCotizacion.observacion = "Se aplicó un descuento superior al permitido en el detalle de la cotización.";
                               cotizacion.seguimientoCotizacion.estado = SeguimientoCotizacion.estadosSeguimientoCotizacion.Pendiente;

                           }

                       }*/

                }
                dal.UpdatePedido(pedido);
            }
        }

        public List<Pedido> GetPedidos(Pedido pedido)
        {
            List<Pedido> pedidoList = null;
            using (var dal = new PedidoDAL())
            {
                //Si el usuario no es aprobador entonces solo buscará sus cotizaciones
                if (!pedido.usuario.esAprobador)
                {
                    pedido.usuarioBusqueda = pedido.usuario;
                }

                pedidoList = dal.SelectPedidos(pedido);
            }
            return pedidoList;
        }

        public Pedido GetPedido(Pedido pedido)
        {
            using (var dal = new PedidoDAL())
            {
                pedido = dal.SelectPedido(pedido);

            /*    if (pedido.mostrarValidezOfertaEnDias == 0)
                {
                    TimeSpan diferencia;
                    diferencia = cotizacion.fechaLimiteValidezOferta - cotizacion.fecha;
                    cotizacion.validezOfertaEnDias = diferencia.Days;
                }
                */
                foreach (PedidoDetalle pedidoDetalle in pedido.pedidoDetalleList)
                {

                    if (pedidoDetalle.producto.image == null)
                    {
                        FileStream inStream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\images\\NoDisponible.gif", FileMode.Open);
                        MemoryStream storeStream = new MemoryStream();
                        storeStream.SetLength(inStream.Length);
                        inStream.Read(storeStream.GetBuffer(), 0, (int)inStream.Length);
                        storeStream.Flush();
                        inStream.Close();
                        pedidoDetalle.producto.image = storeStream.GetBuffer();
                    }

                    //Si NO es recotizacion
                    if (pedido.incluidoIGV)
                    {
                        //Se agrega el IGV al precioLista
                        decimal precioSinIgv = pedidoDetalle.producto.precioSinIgv;
                        decimal precioLista = precioSinIgv + (precioSinIgv * pedido.montoIGV);
                        pedidoDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, precioLista));
                        //Se agrega el IGV al costoLista
                        decimal costoSinIgv = pedidoDetalle.producto.costoSinIgv;
                        decimal costoLista = costoSinIgv + (costoSinIgv * pedido.montoIGV);
                        pedidoDetalle.producto.costoLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, costoLista));
                    }
                    else
                    {
                        //Se agrega el IGV al precioLista
                        pedidoDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, pedidoDetalle.producto.precioSinIgv));
                        //Se agrega el IGV al costoLista
                        pedidoDetalle.producto.costoLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, pedidoDetalle.producto.costoSinIgv));
                    }
                }
            }
            return pedido;
        }
    }
}

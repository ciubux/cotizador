
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;

namespace BusinessLayer
{
    public class PedidoBL
    {
        private void validarPedido(Pedido pedido)
        {
            pedido.seguimientoPedido.observacion = String.Empty;
            pedido.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.Ingresado;
            pedido.seguimientoCrediticioPedido.observacion = String.Empty;
            //Cambio Temporal
            pedido.seguimientoCrediticioPedido.estado = SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido.Liberado;

            if (pedido.tipoPedido != Pedido.tiposPedido.Venta)
            {
                pedido.montoIGV = 0;
                pedido.montoTotal = 0;
                pedido.montoSubTotal = 0;

                if (pedido.tipoPedido == Pedido.tiposPedido.TrasladoInterno)
                {
                    pedido.ciudad = pedido.ciudadASolicitar;
                }
            }
            


            foreach (PedidoDetalle pedidoDetalle in pedido.pedidoDetalleList)
            {
                if (pedido.tipoPedido != Pedido.tiposPedido.Venta)
                {
                    pedidoDetalle.precioNeto = 0;
                }

                

                pedidoDetalle.usuario = pedido.usuario;
                pedidoDetalle.idPedido = pedido.idPedido;

                /*Si es distinto de traslado se interno se valida los precios de lo contrario pasa directamente sin aprobacion*/
                if (pedido.tipoPedido != Pedido.tiposPedido.TrasladoInterno)
                {

                    if (!pedido.usuario.apruebaPedidos)
                    {
                        PrecioClienteProducto precioClienteProducto = pedidoDetalle.producto.precioClienteProducto;

                        int evaluarVariacion = 0;
                        //¿Tiene precio registrado para facturación? y eliente es el mismo?
                        if (precioClienteProducto.idPrecioClienteProducto != Guid.Empty && precioClienteProducto.cliente.idCliente == pedido.cliente.idCliente)
                        {
                            if (precioClienteProducto.fechaFinVigencia == null && DateTime.Now > precioClienteProducto.fechaInicioVigencia.Value.AddDays(Constantes.DIAS_MAX_VIGENCIA_PRECIOS_COTIZACION))
                            {
                                evaluarVariacion = 1;
                            }
                            else
                            {
                                //Si el precio unitario de referencia es válido se envía a evaluar el precio de referencia
                                evaluarVariacion = 2;
                            }

                        }
                        else
                        {
                            evaluarVariacion = 3;
                        }

                        if (evaluarVariacion > 0)
                        {


                            if (evaluarVariacion == 2)
                            {
                                if (pedidoDetalle.precioUnitario > pedidoDetalle.producto.precioClienteProducto.precioUnitario + Constantes.VARIACION_PRECIO_ITEM_PEDIDO ||
                        pedidoDetalle.precioUnitario < pedidoDetalle.producto.precioClienteProducto.precioUnitario - Constantes.VARIACION_PRECIO_ITEM_PEDIDO)
                                {
                                    pedido.seguimientoPedido.observacion = pedido.seguimientoPedido.observacion + "El precio untario indicado en el producto " + pedidoDetalle.producto.sku + " varía por más de: " + Constantes.VARIACION_PRECIO_ITEM_PEDIDO + " con respecto al precio unitario registrado en facturación.\n";
                                    pedido.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.PendienteAprobacion;
                                }
                            }
                            else {
                                if (pedidoDetalle.precioUnitario > pedidoDetalle.producto.precioLista + Constantes.VARIACION_PRECIO_ITEM_PEDIDO ||
                       pedidoDetalle.precioUnitario < pedidoDetalle.producto.precioLista - Constantes.VARIACION_PRECIO_ITEM_PEDIDO)
                                {
                                    if(evaluarVariacion == 1)
                                    { 
                                        pedido.seguimientoPedido.observacion = pedido.seguimientoPedido.observacion + "El precio untario indicado en el producto " + pedidoDetalle.producto.sku + " varía por más de: " + Constantes.VARIACION_PRECIO_ITEM_PEDIDO + " con respecto al precio lista. El precio unitario registrado en facturación se registro hace más de " + Constantes.DIAS_MAX_VIGENCIA_PRECIOS_COTIZACION + " días.\n";
                                        pedido.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.PendienteAprobacion;
                                    }
                                    else
                                    {
                                        pedido.seguimientoPedido.observacion = pedido.seguimientoPedido.observacion + "El precio untario indicado en el producto " + pedidoDetalle.producto.sku + " varía por más de: " + Constantes.VARIACION_PRECIO_ITEM_PEDIDO + " con respecto al precio lista. No se encontró precio unitario en precios registrados en facturación.\n";
                                        pedido.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.PendienteAprobacion;
                                    }




                                }


                            }

                        }
                    }


                  
                }
            }


            foreach (PedidoAdjunto pedidoAdjunto in pedido.pedidoAdjuntoList)
            {
                pedidoAdjunto.usuario = pedido.usuario;
                pedidoAdjunto.idCliente = pedido.cliente.idCliente;
            }

                /*

                if (!pedido.usuario.apruebaPedidos)
                {

                    pedido.seguimientoPedido.observacion = "La fecha de la solicitud es inferior a la fecha actual.";
                    pedido.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.Pendiente;
                }*/

            }




        public void InsertPedido(Pedido pedido)
        {
            using (var dal = new PedidoDAL())
            {
                validarPedido(pedido);
                dal.InsertPedido(pedido);
            }
        }



        public void UpdatePedido(Pedido pedido)
        {
            using (var dal = new PedidoDAL())
            {
                validarPedido(pedido);
                dal.UpdatePedido(pedido);
            }
        }


        public void ActualizarPedido(Pedido pedido)
        {
            using (var dal = new PedidoDAL())
            {
                dal.ActualizarPedido(pedido);
            }
        }

        public void ProgramarPedido(Pedido pedido,Usuario usuario)
        {
            using (var dal = new PedidoDAL())
            {
                dal.ProgramarPedido(pedido,usuario);
            }
        }

        public Pedido obtenerProductosAPartirdePreciosRegistrados(Pedido pedido, String familia, String proveedor)
        {

            ProductoBL productoBL = new ProductoBL();
            List<DocumentoDetalle> documentoDetalleList = productoBL.obtenerProductosAPartirdePreciosRegistrados(pedido.cliente.idCliente, pedido.fechaPrecios, pedido.ciudad.esProvincia, pedido.incluidoIGV, familia, proveedor);

            pedido.pedidoDetalleList = new List<PedidoDetalle>();
            //Detalle de la cotizacion
            foreach (DocumentoDetalle documentoDetalle in documentoDetalleList)
            {
                PedidoDetalle pedidoDetalle = new PedidoDetalle();
                pedidoDetalle.producto = new Producto();
                pedidoDetalle.cantidad = 1;
                pedidoDetalle.esPrecioAlternativo = documentoDetalle.esPrecioAlternativo;
                pedidoDetalle.unidad = documentoDetalle.unidad;
                pedidoDetalle.producto = documentoDetalle.producto;
                if (pedidoDetalle.esPrecioAlternativo)
                    pedidoDetalle.precioNeto = documentoDetalle.precioNeto * documentoDetalle.producto.equivalencia;
                else
                    pedidoDetalle.precioNeto = documentoDetalle.precioNeto;
                pedidoDetalle.flete = documentoDetalle.flete;
                pedidoDetalle.observacion = documentoDetalle.observacion;
                pedidoDetalle.porcentajeDescuento = documentoDetalle.porcentajeDescuento;
                pedido.pedidoDetalleList.Add(pedidoDetalle);
            }
            return pedido;
        }




        public List<Pedido> GetPedidos(Pedido pedido)
        {
            List<Pedido> pedidoList = null;
            using (var dal = new PedidoDAL())
            {
                //Si el usuario no es aprobador entonces solo buscará sus cotizaciones
                if (!pedido.usuario.apruebaCotizaciones)
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

        public void cambiarEstadoPedido(Pedido pedido)
        {
            using (var dal = new PedidoDAL())
            {
                dal.insertSeguimientoPedido(pedido);
            }

        }

        public void cambiarEstadoCrediticioPedido(Pedido pedido)
        {
            using (var dal = new PedidoDAL())
            {
                dal.insertSeguimientoCrediticioPedido(pedido);
            }

        }
    }
}

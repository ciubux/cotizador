
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;
using System.Linq;
using ServiceLayer;
using BusinessLayer.Email;

namespace BusinessLayer
{
    public class PedidoBL
    {

        #region Pedidos de VENTA
        private void validarPedidoVenta(Pedido pedido)
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

                pedido.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.PendienteAprobacion;
                pedido.seguimientoPedido.observacion = "Los pedidos de Transferencia Gratuita y Comodato requieren aprobación.";
            }
            else
            {
                //if (!pedido.usuario.apruebaPedidos && !pedido.cliente.perteneceCanalMultiregional)
                /*   if (!pedido.usuario.apruebaPedidos)
                   {
                       DateTime horaActual = DateTime.Now;
                       DateTime horaLimite = new DateTime(horaActual.Year, horaActual.Month, horaActual.Day, Constantes.HORA_CORTE_CREDITOS_LIMA.Hour, Constantes.HORA_CORTE_CREDITOS_LIMA.Minute, Constantes.HORA_CORTE_CREDITOS_LIMA.Second);
                       //if (horaActual >= horaLimite && pedido.ciudad.idCiudad == Guid.Parse("15526227-2108-4113-B46A-1C8AB5C0E581"))//-- .esProvincia)
                       if (horaActual >= horaLimite && pedido.ciudad.idCiudad == Guid.Parse("15526227-2108-4113-B46A-1C8AB5C0E581"))//-- .esProvincia)
                       {
                           pedido.seguimientoCrediticioPedido.estado = SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido.PendienteLiberación;
                           pedido.seguimientoCrediticioPedido.observacion = "Se ha superado la Hora de Corte, la hora de corte actualmente es: " + Constantes.HORA_CORTE_CREDITOS_LIMA.Hour.ToString() + ":" + (Constantes.HORA_CORTE_CREDITOS_LIMA.Minute > 9 ? Constantes.HORA_CORTE_CREDITOS_LIMA.Minute.ToString() : "0" + Constantes.HORA_CORTE_CREDITOS_LIMA.Minute.ToString());
                      }
                   }*/
                if (!pedido.cliente.exoneradoValidacionLiberacionCrediticia)
                {

                    pedido.seguimientoCrediticioPedido.estado = SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido.PendienteLiberación;
                    if (pedido.ciudad.idCiudad == Constantes.ID_SEDE_LIMA)//-- .esProvincia)
                    {
                        pedido.seguimientoCrediticioPedido.observacion = "Si tiene alguna duda sobre el estado pendiente de liberación o requiere liberar el pedido con urgencia contactar con Tatiana Cuentas / Angélica Huachín";
                    }
                    else
                    {
                        pedido.seguimientoCrediticioPedido.observacion = "Si tiene alguna duda sobre el estado pendiente de liberación o requiere liberar el pedido con urgencia contactar con Ana Felipe / Angélica Huachín";
                    }
                }
                else
                {
                    pedido.seguimientoCrediticioPedido.estado = SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido.Liberado;
                }
                /*
                if (!pedido.usuario.apruebaPedidos && !pedido.cliente.perteneceCanalMultiregional)
                {
                    pedido.seguimientoCrediticioPedido.estado = SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido.PendienteLiberación;
                }*/
            }
           


            if (pedido.cliente.bloqueado)
            {
                pedido.seguimientoCrediticioPedido.observacion = "El cliente se encuentra BLOQUEADO."; // Agregar quien bloquea
                pedido.seguimientoCrediticioPedido.estado = SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido.BLoqueado;
            }
            

            foreach (PedidoDetalle pedidoDetalle in pedido.pedidoDetalleList)
            {
                if (pedido.tipoPedido != Pedido.tiposPedido.Venta)
                {
                    pedidoDetalle.precioNeto = 0;
                }               

               // pedidoDetalle.usuario = pedido.usuario;
                pedidoDetalle.idPedido = pedido.idPedido;

                /*Si es venta se valida los precios de lo contrario pasa directamente sin aprobacion*/
                if (pedido.tipoPedido == Pedido.tiposPedido.Venta)
                {

                    if (!pedido.usuario.apruebaPedidos)
                    {
                        //Si cliente está bloqueado

                        if (pedidoDetalle.producto.tipoProducto == Producto.TipoProducto.Bien)
                        {

                            PrecioClienteProducto precioClienteProducto = pedidoDetalle.producto.precioClienteProducto;

                            int evaluarVariacion = 0;

                            DateTime hoy = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                            //¿Tiene precio registrado para facturación? y eliente es el mismo?
                            if (precioClienteProducto.idPrecioClienteProducto != Guid.Empty)// && precioClienteProducto.cliente.idCliente == pedido.cliente.idCliente)
                            {
                                if (precioClienteProducto.fechaFinVigencia == null && hoy > precioClienteProducto.fechaInicioVigencia.Value.AddDays(Constantes.DIAS_MAX_VIGENCIA_PRECIOS_COTIZACION))
                                {
                                    evaluarVariacion = 1;
                                }
                                else if (precioClienteProducto.fechaFinVigencia != null && hoy > precioClienteProducto.fechaFinVigencia.Value)
                                {
                                    evaluarVariacion = 4;
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
                            /*Si se tiener que evalular variacion*/
                            if (evaluarVariacion > 0)
                            {

                                /*Se evalua contra precio cliente producto*/
                                if (evaluarVariacion == 2)
                                {
                                    //Se evalua precio cliente producto
                                    /*      if (!pedidoDetalle.esPrecioAlternativo)
                                          {
                                          */
                                    if (pedidoDetalle.precioUnitario > pedidoDetalle.producto.precioClienteProducto.precioUnitario + Constantes.VARIACION_PRECIO_ITEM_PEDIDO ||
                        pedidoDetalle.precioUnitario < pedidoDetalle.producto.precioClienteProducto.precioUnitario - Constantes.VARIACION_PRECIO_ITEM_PEDIDO)
                                    {
                                        pedido.seguimientoPedido.observacion = pedido.seguimientoPedido.observacion + "El precio untario indicado en el producto " + pedidoDetalle.producto.sku + " varía por más de: " + Constantes.VARIACION_PRECIO_ITEM_PEDIDO + " con respecto al precio unitario registrado en facturación.\n";
                                        pedido.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.PendienteAprobacion;
                                        pedidoDetalle.indicadorAprobacion = PedidoDetalle.IndicadorAprobacion.RechazadoSinPrecio;
                                    }
                                    /* }
                                     else {
                                         if (pedidoDetalle.precioUnitario > (pedidoDetalle.producto.precioClienteProducto.precioUnitario/ pedidoDetalle.producto.equivalencia) + Constantes.VARIACION_PRECIO_ITEM_PEDIDO ||
                                                                 pedidoDetalle.precioUnitario < (pedidoDetalle.producto.precioClienteProducto.precioUnitario / pedidoDetalle.producto.equivalencia) - Constantes.VARIACION_PRECIO_ITEM_PEDIDO)
                                         {
                                             pedido.seguimientoPedido.observacion = pedido.seguimientoPedido.observacion + "El precio untario indicado en el producto " + pedidoDetalle.producto.sku + " varía por más de: " + Constantes.VARIACION_PRECIO_ITEM_PEDIDO + " con respecto al precio unitario registrado en facturación.\n";
                                             pedido.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.PendienteAprobacion;
                                         }
                                     }*/
                                    /*Se evalua contra precio lista*/
                                }
                                else
                                {
                                    if (pedidoDetalle.precioUnitario > pedidoDetalle.precioLista + Constantes.VARIACION_PRECIO_ITEM_PEDIDO ||
                           pedidoDetalle.precioUnitario < pedidoDetalle.precioLista - Constantes.VARIACION_PRECIO_ITEM_PEDIDO)
                                    {
                                        if (evaluarVariacion == 1)
                                        {
                                            pedido.seguimientoPedido.observacion = pedido.seguimientoPedido.observacion + "El precio untario indicado en el producto " + pedidoDetalle.producto.sku + " varía por más de: " + Constantes.VARIACION_PRECIO_ITEM_PEDIDO + " con respecto al precio lista. El precio unitario registrado en facturación se registro hace más de " + Constantes.DIAS_MAX_VIGENCIA_PRECIOS_COTIZACION + " días.\n";
                                            pedido.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.PendienteAprobacion;
                                            if(pedidoDetalle.indicadorAprobacion != PedidoDetalle.IndicadorAprobacion.RechazadoSinPrecio)
                                                pedidoDetalle.indicadorAprobacion = PedidoDetalle.IndicadorAprobacion.RechazadoSinVigencia;
                                        }
                                        if (evaluarVariacion == 4)
                                        {
                                            pedido.seguimientoPedido.observacion = pedido.seguimientoPedido.observacion + "El precio untario indicado en el producto " + pedidoDetalle.producto.sku + " varía por más de: " + Constantes.VARIACION_PRECIO_ITEM_PEDIDO + " con respecto al precio lista. El precio unitario registrado en facturación tuvo vigencia hasta " + precioClienteProducto.fechaFinVigencia.Value.ToString(Constantes.formatoFecha) + ".\n";
                                            pedido.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.PendienteAprobacion;
                                            if (pedidoDetalle.indicadorAprobacion != PedidoDetalle.IndicadorAprobacion.RechazadoSinPrecio)
                                                pedidoDetalle.indicadorAprobacion = PedidoDetalle.IndicadorAprobacion.RechazadoSinVigencia;
                                        }
                                        else
                                        {
                                            pedido.seguimientoPedido.observacion = pedido.seguimientoPedido.observacion + "El precio untario indicado en el producto " + pedidoDetalle.producto.sku + " varía por más de: " + Constantes.VARIACION_PRECIO_ITEM_PEDIDO + " con respecto al precio lista. No se encontró precio unitario en precios registrados en facturación.\n";
                                            pedido.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.PendienteAprobacion;
                                            pedidoDetalle.indicadorAprobacion = PedidoDetalle.IndicadorAprobacion.RechazadoSinPrecio;
                                        }
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

        }
                
        public void InsertPedido(Pedido pedido)
        {
            using (var dal = new PedidoDAL())
            {

                String observacionesAdicionales = String.Empty;
              if (pedido.direccionEntrega.nombre != null && pedido.direccionEntrega.nombre.Length > 0)
              {
                  if (pedido.direccionEntrega.codigoCliente != null && pedido.direccionEntrega.codigoCliente.Length > 0)
                  {
                        observacionesAdicionales = pedido.direccionEntrega.nombre + " (" + pedido.direccionEntrega.codigoCliente + ")";
                  }
                  else
                  {
                        observacionesAdicionales = pedido.direccionEntrega.nombre;
                  }                
              }
                if (pedido.observaciones != null && !pedido.observaciones.Equals(String.Empty))
                {
                    pedido.observaciones = pedido.observaciones + " / " + observacionesAdicionales;
                }
                else
                {
                    pedido.observaciones = observacionesAdicionales;
                }

                validarPedidoVenta(pedido);
                dal.InsertPedido(pedido);
            }
        }

        public void UpdatePedido(Pedido pedido)
        {
            using (var dal = new PedidoDAL())
            {
                validarPedidoVenta(pedido);
                dal.UpdatePedido(pedido);
            }
        }

        public void UpdateStockConfirmado(Pedido pedido)
        {
            using (var dal = new PedidoDAL())
            {
                dal.UpdateStockConfirmado(pedido);
            }
        }

        public void ProgramarPedido(Pedido pedido, Usuario usuario)
        {
            using (var dal = new PedidoDAL())
            {
                dal.ProgramarPedido(pedido, usuario);
            }
        }

        public List<Pedido> GetPedidos(Pedido pedido)
        {
            List<Pedido> pedidoList = null;
            using (var dal = new PedidoDAL())
            {
                pedidoList = dal.SelectPedidos(pedido);
            }
            return pedidoList;
        }


        #endregion

        #region Pedidos de COMPRA
        private void validarPedidoCompra(Pedido pedido)
        {
            pedido.seguimientoPedido.observacion = String.Empty;
            pedido.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.Ingresado;
            pedido.seguimientoCrediticioPedido.observacion = String.Empty;
            //Cambio Temporal
            pedido.seguimientoCrediticioPedido.estado = SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido.Liberado;

            if (pedido.tipoPedidoCompra != Pedido.tiposPedidoCompra.Compra)
            {
                pedido.montoIGV = 0;
                pedido.montoTotal = 0;
                pedido.montoSubTotal = 0;
            }           



            foreach (PedidoDetalle pedidoDetalle in pedido.pedidoDetalleList)
            {
                if (pedido.tipoPedidoCompra != Pedido.tiposPedidoCompra.Compra)
                {
                    pedidoDetalle.precioNeto = 0;
                }

                //pedidoDetalle.usuario = pedido.usuario;
                pedidoDetalle.idPedido = pedido.idPedido;
            }

            if (pedido.cliente.bloqueado)
            {
                pedido.seguimientoPedido.observacion = "El cliente se encuentra bloqueado.";
                pedido.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.PendienteAprobacion;
            }
            
            foreach (PedidoAdjunto pedidoAdjunto in pedido.pedidoAdjuntoList)
            {
                pedidoAdjunto.usuario = pedido.usuario;
                pedidoAdjunto.idCliente = pedido.cliente.idCliente;
            }
        }

        public void InsertPedidoCompra(Pedido pedido)
        {
            using (var dal = new PedidoDAL())
            {
                validarPedidoCompra(pedido);
                dal.InsertPedido(pedido);
            }
        }

        public void UpdatePedidoCompra(Pedido pedido)
        {
            using (var dal = new PedidoDAL())
            {
                validarPedidoCompra(pedido);
                dal.UpdatePedido(pedido);
            }
        }

        public List<Pedido> GetPedidosCompra(Pedido pedido)
        {
            List<Pedido> pedidoList = null;
            using (var dal = new PedidoDAL())
            {
                pedidoList = dal.SelectPedidos(pedido);
            }
            return pedidoList;
        }

        #endregion

        #region Pedidos de ALMACEN

        private void validarPedidoAlmacen(Pedido pedido)
        {
            pedido.seguimientoPedido.observacion = String.Empty;
            pedido.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.Ingresado;
            pedido.seguimientoCrediticioPedido.observacion = String.Empty;
            //Cambio Temporal
            pedido.seguimientoCrediticioPedido.estado = SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido.Liberado;

            pedido.montoIGV = 0;
            pedido.montoTotal = 0;
            pedido.montoSubTotal = 0;

            if (pedido.tipoPedidoAlmacen == Pedido.tiposPedidoAlmacen.TrasladoInterno)
            {
                pedido.ciudad = pedido.ciudadASolicitar;
            }

            foreach (PedidoDetalle pedidoDetalle in pedido.pedidoDetalleList)
            {
                pedidoDetalle.precioNeto = 0;
               // pedidoDetalle.usuario = pedido.usuario;
                pedidoDetalle.idPedido = pedido.idPedido;
            }


            foreach (PedidoAdjunto pedidoAdjunto in pedido.pedidoAdjuntoList)
            {
                pedidoAdjunto.usuario = pedido.usuario;
                pedidoAdjunto.idCliente = pedido.cliente.idCliente;
            }


        }

        public void InsertPedidoAlmacen(Pedido pedido)
        {
            using (var dal = new PedidoDAL())
            {
                validarPedidoAlmacen(pedido);
                dal.InsertPedido(pedido);
                if (pedido.tipoPedidoAlmacen == Pedido.tiposPedidoAlmacen.TrasladoInterno)
                {
                    /*Se debe recuperar el correo de la ciudad origen*/
                    MailService mail = new MailService();

                    //var urlVerPedido = this.Url.Action("Index", "Pedido", new { idPedido = pedido.idPedido }, this.Request.Url.Scheme);
                    var urlVerPedido = Constantes.URL_VER_PEDIDO + pedido.idPedido.ToString();
                    PedidoSinAtencion emailTemplate = new PedidoSinAtencion();
                    emailTemplate.urlVerPedido = urlVerPedido;


                    foreach (PedidoDetalle det in pedido.pedidoDetalleList)
                    {
                        det.cantidadPendienteAtencion = det.cantidad;
                    }

                    String template = emailTemplate.BuildTemplate(pedido);
                    List<String> destinatarios = new List<String>();

                    Ciudad ciudadOrigen = pedido.usuario.sedesMP.Where(p => p.idCiudad == pedido.ciudad.idCiudad).FirstOrDefault();
                    destinatarios.Add(ciudadOrigen.correoCoordinador);


                    if (destinatarios.Count > 0)
                    {
                        String asunto = "Se ha creado el pedido de TRASLADO INTERNO " + pedido.numeroPedidoString;   
                        mail.enviar(destinatarios, asunto, template, Constantes.MAIL_COMUNICACION_PEDIDOS_NO_ATENDIDOS, Constantes.PASSWORD_MAIL_COMUNICACION_PEDIDOS_NO_ATENDIDOS, new Usuario());
                    }
                }

            }
        }

        public void UpdatePedidoAlmacen(Pedido pedido)
        {
            using (var dal = new PedidoDAL())
            {
                validarPedidoAlmacen(pedido);
                dal.UpdatePedido(pedido);
                if (pedido.tipoPedidoAlmacen == Pedido.tiposPedidoAlmacen.TrasladoInterno)
                {
                    /*Se debe recuperar el correo de la ciudad origen*/
                    MailService mail = new MailService();

                    //var urlVerPedido = this.Url.Action("Index", "Pedido", new { idPedido = pedido.idPedido }, this.Request.Url.Scheme);
                    var urlVerPedido = Constantes.URL_VER_PEDIDO + pedido.idPedido.ToString();
                    PedidoSinAtencion emailTemplate = new PedidoSinAtencion();
                    emailTemplate.urlVerPedido = urlVerPedido;

                    foreach (PedidoDetalle det in pedido.pedidoDetalleList)
                    {
                        det.cantidadPendienteAtencion = det.cantidad;
                    }

                    String template = emailTemplate.BuildTemplate(pedido);
                    List<String> destinatarios = new List<String>();

                    Ciudad ciudadOrigen = pedido.usuario.sedesMP.Where(p => p.idCiudad == pedido.ciudad.idCiudad).FirstOrDefault();
                    destinatarios.Add(ciudadOrigen.correoCoordinador);


                    if (destinatarios.Count > 0)
                    {
                        String asunto = "Se ha modificado el pedido de TRASLADO INTERNO " + pedido.numeroPedidoString;
                        mail.enviar(destinatarios, asunto, template, Constantes.MAIL_COMUNICACION_PEDIDOS_NO_ATENDIDOS, Constantes.PASSWORD_MAIL_COMUNICACION_PEDIDOS_NO_ATENDIDOS, new Usuario());
                    }
                }
            }
        }

        public List<Pedido> GetPedidosAlmacen(Pedido pedido)
        {
            List<Pedido> pedidoList = null;
            using (var dal = new PedidoDAL())
            {
                pedidoList = dal.SelectPedidos(pedido);
            }
            return pedidoList;
        }


        #endregion

        public List<SeguimientoPedido> GetHistorialSeguimiento(Guid idPedido)
        {
            List<SeguimientoPedido> seguimientoList = new List<SeguimientoPedido>();
            using (var dal = new PedidoDAL())
            {
                seguimientoList = dal.GetHistorialSeguimiento(idPedido);
            }
            return seguimientoList;
        }

        public List<SeguimientoCrediticioPedido> GetHistorialSeguimientoCrediticio(Guid idPedido)
        {
            List<SeguimientoCrediticioPedido> seguimientoList = new List<SeguimientoCrediticioPedido>();
            using (var dal = new PedidoDAL())
            {
                seguimientoList = dal.GetHistorialCrediticioSeguimiento(idPedido);
            }
            return seguimientoList;
        }

        #region General

        public void ActualizarPedido(Pedido pedido)
        {
            using (var dal = new PedidoDAL())
            {
                dal.ActualizarPedido(pedido);
            }
        }

        public Pedido obtenerProductosAPartirdePreciosRegistrados(Pedido pedido, String familia, String proveedor, Usuario usuario)
        {

            ProductoBL productoBL = new ProductoBL();
            List<DocumentoDetalle> documentoDetalleList = productoBL.obtenerProductosAPartirdePreciosRegistradosParaPedido(pedido.cliente.idCliente, pedido.fechaPrecios, pedido.ciudad.esProvincia, pedido.incluidoIGV, familia, proveedor);

            pedido.pedidoDetalleList = new List<PedidoDetalle>();
            //Detalle de la cotizacion
            foreach (DocumentoDetalle documentoDetalle in documentoDetalleList)
            {
                PedidoDetalle pedidoDetalle = new PedidoDetalle(usuario.visualizaCostos, usuario.visualizaMargen);
                pedidoDetalle.producto = new Producto();
                pedidoDetalle.cantidad = 1;
                pedidoDetalle.esPrecioAlternativo = documentoDetalle.esPrecioAlternativo;
                pedidoDetalle.unidad = documentoDetalle.unidad;
                pedidoDetalle.producto = documentoDetalle.producto;
                if (pedidoDetalle.esPrecioAlternativo)
                {
                    pedidoDetalle.ProductoPresentacion = documentoDetalle.ProductoPresentacion;
                    pedidoDetalle.precioNeto = documentoDetalle.precioNeto * documentoDetalle.ProductoPresentacion.Equivalencia;
                }
                else
                {
                    pedidoDetalle.precioNeto = documentoDetalle.precioNeto;
                }
                pedidoDetalle.flete = documentoDetalle.flete;
                pedidoDetalle.observacion = documentoDetalle.observacion;
                pedidoDetalle.porcentajeDescuento = documentoDetalle.porcentajeDescuento;
                pedido.pedidoDetalleList.Add(pedidoDetalle);
            }
            return pedido;
        }


        public Int64 GetSiguienteNumeroGrupoPedido()
        {
            using (var dal = new PedidoDAL())
            {
                return dal.SelectSiguienteNumeroGrupoPedido();
            }
        }

        public Pedido GetPedido(Pedido pedido,Usuario usuario)
        {
            using (var dal = new PedidoDAL())
            {
                pedido = dal.SelectPedido(pedido, usuario);

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

                    if (pedidoDetalle.esPrecioAlternativo)
                    {
                        pedidoDetalle.producto.precioClienteProducto.precioUnitario =
                        pedidoDetalle.producto.precioClienteProducto.precioUnitario / pedidoDetalle.ProductoPresentacion.Equivalencia;
                    }
                }
            }
            return pedido;
        }


        public Pedido GetPedidoParaEditar(Pedido pedido, Usuario usuario)
        {
            using (var dal = new PedidoDAL())
            {
                pedido = dal.SelectPedidoParaEditar(pedido, usuario);

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

                    if (pedidoDetalle.esPrecioAlternativo)
                    {
                        pedidoDetalle.producto.precioClienteProducto.precioUnitario =
                        pedidoDetalle.producto.precioClienteProducto.precioUnitario / pedidoDetalle.ProductoPresentacion.Equivalencia;
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
        
        public void calcularMontosTotales(Pedido pedido)
        {
            Decimal total = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, pedido.pedidoDetalleList.AsEnumerable().Sum(o => o.subTotal)));
            Decimal subtotal = 0;
            Decimal igv = 0;

            if (pedido.incluidoIGV)
            {
                subtotal = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, total / (1 + Constantes.IGV)));
                igv = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, total - subtotal));
            }
            else
            {
                subtotal = total;
                igv = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, total * Constantes.IGV));
                total = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, subtotal + igv));
            }

            pedido.montoTotal = total;
            pedido.montoSubTotal = subtotal;
            pedido.montoIGV = igv;
        }

        #endregion


        public List<Pedido> EnviarEmailAlertaPedidosNoEnviados()
        {
            List<Pedido> pedidoList = new List<Pedido>();
            using (var dal = new PedidoDAL())
            {
                List<Guid> pedidoIds = dal.SelectPedidosSinAtencion();
                foreach (Guid id in pedidoIds)
                {
                    Pedido pedido = dal.SelectPedidoEmail(id);
                    pedidoList.Add(pedido);
                }
            }

            return pedidoList;
        }
    }
}

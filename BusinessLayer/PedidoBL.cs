﻿
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;
using System.Linq;
using ServiceLayer;
using BusinessLayer.Email;
using System.Net.Http.Headers;

namespace BusinessLayer
{
    public class PedidoBL
    {

        #region Pedidos de VENTA
        private void validarPedidoVenta(Pedido pedido)
        {
            bool enviaAprobacion = false;
            ParametroBL blParametro = new ParametroBL();
            string paramEnviaAprobacion = blParametro.getParametro("ENVIA_APROBACION_TODOS_PEDIDOS");
            if (paramEnviaAprobacion.ToUpper().Equals("SI"))
            {
                enviaAprobacion = true;
            }

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
                if (pedido.cliente.tipoLiberacionCrediticia != Persona.TipoLiberacionCrediticia.exonerado)
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
           


            if (pedido.cliente.tipoLiberacionCrediticia == Persona.TipoLiberacionCrediticia.bloqueado)
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
                    if (pedidoDetalle.esPrecioAlternativo && (pedidoDetalle.ProductoPresentacion == null || pedidoDetalle.ProductoPresentacion.IdProductoPresentacion == 0))
                    {
                        throw new Exception("ERROR DE EQUIVALENCIAS, CONTACTE CON TI");
                    }

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

                        if (pedidoDetalle.tieneInfraMargenEmpresaExterna)
                        {
                            pedido.seguimientoPedido.observacion = pedido.seguimientoPedido.observacion + "El precio untario del producto " + pedidoDetalle.producto.sku + " tiene inframargen.\n";
                        }

                        //pedido.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.PendienteAprobacion;
                    }

                    if (pedidoDetalle.producto.descontinuado == 1 && !pedido.usuario.apruebaPedidosVentaRestringida)
                    {
                        if (pedidoDetalle.producto.cantidadMaximaPedidoRestringido < (pedidoDetalle.cantidad / (pedidoDetalle.ProductoPresentacion == null ? 1 : (pedidoDetalle.ProductoPresentacion.Equivalencia > 0 ? pedidoDetalle.ProductoPresentacion.Equivalencia : 1) )))
                        {
                            pedido.seguimientoPedido.observacion = pedido.seguimientoPedido.observacion + "El producto " + pedidoDetalle.producto.sku + " es de venta restringida.";
                            pedido.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.PendienteAprobacion;
                        }
                    }

                }
            }

            if (pedido.seguimientoPedido.estado == SeguimientoPedido.estadosSeguimientoPedido.Ingresado && enviaAprobacion && !pedido.usuario.apruebaPedidos)
            {
                pedido.seguimientoPedido.observacion = pedido.seguimientoPedido.observacion + " Esta activa la aprobación obligatoria de TODOS los pedidos.";
                pedido.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.PendienteAprobacion;
            }

            if (!pedido.guardadoParcialmente && pedido.seguimientoPedido.estado == SeguimientoPedido.estadosSeguimientoPedido.PendienteAprobacion && pedido.usuario.codigoEmpresa.Equals(Constantes.EMPRESA_CODIGO_TECNICA))
            {
                pedido.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.Ingresado;
            }

            if (!pedido.guardadoParcialmente && pedido.seguimientoCrediticioPedido.estado == SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido.PendienteLiberación && pedido.usuario.codigoEmpresa.Equals(Constantes.EMPRESA_CODIGO_TECNICA))
            {
                pedido.seguimientoCrediticioPedido.estado = SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido.Liberado;
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


                if (pedido.cliente.configuraciones == null)
                {
                    ClienteDAL dalCliente = new ClienteDAL();
                    pedido.cliente = dalCliente.getCliente(pedido.cliente.idCliente);
                }

                if (pedido.cliente.configuraciones.agregarNombreSedeObservacionFactura && pedido.direccionEntrega != null &&
                    ((pedido.direccionEntrega.nombre != null && !pedido.direccionEntrega.nombre.Trim().Equals("")) ||
                     (pedido.direccionEntrega.codigoCliente != null && !pedido.direccionEntrega.codigoCliente.Trim().Equals(""))))
                {
                    pedido.observacionesFactura = pedido.observacionesFactura + " SEDE: ";
                    if (pedido.direccionEntrega.nombre != null && !pedido.direccionEntrega.nombre.Trim().Equals(""))
                    {
                        pedido.observacionesFactura = pedido.observacionesFactura + pedido.direccionEntrega.nombre;
                    }

                    if (pedido.direccionEntrega.codigoCliente != null && !pedido.direccionEntrega.codigoCliente.Trim().Equals(""))
                    {
                        pedido.observacionesFactura = pedido.observacionesFactura + "(" + pedido.direccionEntrega.codigoCliente + ")";
                    }
                }

                if (pedido.ordenCompracliente != null && pedido.ordenCompracliente.idOrdenCompraCliente != Guid.Empty)
                {
                    pedido.numeroReferenciaCliente = pedido.ordenCompracliente.numeroReferenciaCliente;
                }

                // Si no es un pedido de venta MP, se revisa si tiene inframargen
                if (!pedido.usuario.codigoEmpresa.Equals(Constantes.EMPRESA_CODIGO_MP) && pedido.tipoPedido == Pedido.tiposPedido.Venta)
                {
                    UsuarioDAL usuarioDal = new UsuarioDAL();
                    Usuario usuarioEmpresa = usuarioDal.getUsuario(pedido.IdUsuarioRegistro);

                    foreach (PedidoDetalle det in pedido.pedidoDetalleList)
                    {
                        decimal margenDet = ((det.precioNeto - det.producto.costoLista) / det.precioNeto) * 100;
                        det.tieneInfraMargenEmpresaExterna = false;

                        if (margenDet < usuarioEmpresa.pMargenMinimo)
                        {
                            det.tieneInfraMargenEmpresaExterna = true;
                        }
                    }

                    pedido.entregaTerciarizada = pedido.usuario.atencionTerciarizadaEmpresa;

                }

                validarPedidoVenta(pedido);
                dal.InsertPedido(pedido);
                pedido.IdUsuarioRegistro = pedido.usuario.idUsuario;

                if (!pedido.usuario.codigoEmpresa.Equals(Constantes.EMPRESA_CODIGO_MP) && 
                    pedido.seguimientoPedido.estado == SeguimientoPedido.estadosSeguimientoPedido.Ingresado &&
                    pedido.seguimientoCrediticioPedido.estado == SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido.Liberado)
                {
                    ProcesarPedidoAprobadoTecnica(pedido);
                }
            }
        }

        public void UpdatePedido(Pedido pedido)
        {
            using (var dal = new PedidoDAL())
            {
                if (pedido.ordenCompracliente != null && pedido.ordenCompracliente.idOrdenCompraCliente != Guid.Empty)
                {
                    pedido.numeroReferenciaCliente = pedido.ordenCompracliente.numeroReferenciaCliente;
                }

                // Si no es un pedido de venta MP, se revisa si tiene inframargen
                if (!pedido.usuario.codigoEmpresa.Equals(Constantes.EMPRESA_CODIGO_MP) && pedido.tipoPedido == Pedido.tiposPedido.Venta)
                {
                    UsuarioDAL usuarioDal = new UsuarioDAL();
                    Usuario usuarioEmpresa = usuarioDal.getUsuario(pedido.IdUsuarioRegistro);

                    foreach (PedidoDetalle det in pedido.pedidoDetalleList)
                    {
                        decimal margenDet = ((det.precioNeto - det.producto.costoLista) / det.precioNeto) * 100;
                        det.tieneInfraMargenEmpresaExterna = false;

                        if (margenDet < usuarioEmpresa.pMargenMinimo)
                        {
                            det.tieneInfraMargenEmpresaExterna = true;
                        }
                    }
                }

                validarPedidoVenta(pedido);
                
                if (pedido.esVentaIndirecta && !pedido.esVentaIndirectaAnt)
                {
                    pedido.observacionesAlmacen = pedido.observacionesAlmacen + " // Venta indirecta a [RAZON_SOCIAL_CLIENTE_FINAL] a través de " + pedido.usuario.razonSocialEmpresa;
                }

                dal.UpdatePedido(pedido);
                pedido.IdUsuarioRegistro = pedido.usuario.idUsuario;
                if (!pedido.usuario.codigoEmpresa.Equals(Constantes.EMPRESA_CODIGO_MP) &&
                    pedido.seguimientoPedido.estado == SeguimientoPedido.estadosSeguimientoPedido.Ingresado &&
                    pedido.seguimientoCrediticioPedido.estado == SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido.Liberado)
                {
                    ProcesarPedidoAprobadoTecnica(pedido);
                }
            }
        }

        public void UpdateStockConfirmado(Pedido pedido)
        {
            using (var dal = new PedidoDAL())
            {
                dal.UpdateStockConfirmado(pedido);
            }
        }

        public Pedido TruncarPedido(Pedido pedido)
        {
            using (var dal = new PedidoDAL())
            {
                pedido.truncado = 1;
                return dal.UpdateTruncado(pedido);
            }
        }

        public Pedido DestruncarPedido(Pedido pedido)
        {
            using (var dal = new PedidoDAL())
            {
                pedido.truncado = 0;
                return dal.UpdateTruncado(pedido);
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

        public List<Pedido> GetPedidosGrupo(Pedido pedido, int tipoOrdenamiento)
        {
            List<Pedido> pedidoList = null;
            using (var dal = new PedidoDAL())
            {
                pedidoList = dal.SelectPedidosGrupo(pedido, tipoOrdenamiento);
            }
            return pedidoList;
        }


        public List<int> AprobarPedidosgrupo(long nroGrupo, Guid idUsuario)
        {
            using (var dal = new PedidoDAL())
            {
                return dal.AprobarPedidosGrupo(nroGrupo, idUsuario);
            }
        }

        public List<int> LiberarPedidosGrupo(long nroGrupo, Guid idUsuario)
        {
            using (var dal = new PedidoDAL())
            {
                return dal.LiberarPedidosGrupo(nroGrupo, idUsuario);
            }
        }

        public List<int> AprobarPedidos(List<Guid> idsPedido, Guid idUsuario)
        {
            using (var dal = new PedidoDAL())
            {
                return dal.AprobarPedidos(idsPedido, idUsuario);
            }
        }

        public List<int> LiberarPedidos(List<Guid> idsPedido, Guid idUsuario)
        {
            using (var dal = new PedidoDAL())
            {
                return dal.LiberarPedidos(idsPedido, idUsuario);
            }
        }

        #endregion

        #region Pedidos de COMPRA
        private void validarPedidoCompra(Pedido pedido)
        {
            bool enviaAprobacion = false;
            ParametroBL blParametro = new ParametroBL();
            string paramEnviaAprobacion = blParametro.getParametro("ENVIA_APROBACION_TODOS_PEDIDOS_COMPRA");
            if (paramEnviaAprobacion.ToUpper().Equals("SI"))
            {
                enviaAprobacion = true;
            }

            

            pedido.seguimientoPedido.observacion = String.Empty;
            pedido.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.PendienteAprobacion;
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

                if (pedidoDetalle.producto.descontinuado == 1 && !pedido.usuario.apruebaPedidosVentaRestringida)
                {
                    pedido.seguimientoPedido.observacion = "El producto " + pedidoDetalle.producto.sku + " es de venta restringida.";
                    pedido.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.PendienteAprobacion;
                }

                if (pedidoDetalle.producto.compraRestringida == 1 && !pedido.usuario.apruebaPedidosVentaRestringida)
                {
                    pedido.seguimientoPedido.observacion = "El producto " + pedidoDetalle.producto.sku + " tiene RESTRICCIÓN DE COMPRA.";
                    pedido.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.PendienteAprobacion;
                }
            }

            if (pedido.seguimientoPedido.estado == SeguimientoPedido.estadosSeguimientoPedido.Ingresado && enviaAprobacion && !pedido.usuario.apruebaPedidosCompra)
            {
                pedido.seguimientoPedido.observacion = pedido.seguimientoPedido.observacion + " Esta activa la aprobación obligatoria de TODOS los pedidos de compra.";
                pedido.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.PendienteAprobacion;
            }

            if (pedido.cliente.tipoLiberacionCrediticia == Persona.TipoLiberacionCrediticia.bloqueado)
            {
                pedido.seguimientoPedido.observacion = "El proveedor se encuentra bloqueado.";
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
            bool enviaAprobacion = false;
            ParametroBL blParametro = new ParametroBL();
            string paramEnviaAprobacion = blParametro.getParametro("ENVIA_APROBACION_TODOS_PEDIDOS_COMPRA");
            if (paramEnviaAprobacion.ToUpper().Equals("SI"))
            {
                enviaAprobacion = true;
            }

            pedido.seguimientoPedido.observacion = String.Empty;
            pedido.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.PendienteAprobacion;
            pedido.seguimientoCrediticioPedido.observacion = String.Empty;
            //Cambio Temporal
            pedido.seguimientoCrediticioPedido.estado = SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido.Liberado;

            pedido.montoIGV = 0;
            pedido.montoTotal = 0;
            pedido.montoSubTotal = 0;

            if (pedido.tipoPedidoAlmacen == Pedido.tiposPedidoAlmacen.TrasladoInterno)
            { 
                if (pedido.ciudad.idCiudad.Equals(pedido.ciudadASolicitar.idCiudad))
                {
                    pedido.usaSerieTI = 1;
                }

                pedido.ciudad = pedido.ciudadASolicitar;
            }

            foreach (PedidoDetalle pedidoDetalle in pedido.pedidoDetalleList)
            {
                pedidoDetalle.precioNeto = 0;
               // pedidoDetalle.usuario = pedido.usuario;
                pedidoDetalle.idPedido = pedido.idPedido;

                if (pedidoDetalle.producto.descontinuado == 1 && !pedido.usuario.apruebaPedidosVentaRestringida)
                {
                    pedido.seguimientoPedido.observacion = "El producto " + pedidoDetalle.producto.sku + " es de venta restringida.";
                    pedido.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.PendienteAprobacion;
                }
            }


            if(pedido.usuario.apruebaPedidosAlmacen)
            {
                pedido.seguimientoPedido.observacion = "";
                pedido.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.Ingresado;
            }

            if (pedido.seguimientoPedido.estado == SeguimientoPedido.estadosSeguimientoPedido.Ingresado && enviaAprobacion && !pedido.usuario.apruebaPedidosAlmacen)
            {
                pedido.seguimientoPedido.observacion = pedido.seguimientoPedido.observacion + " Esta activa la aprobación obligatoria de TODOS los pedidos de almacén.";
                pedido.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.PendienteAprobacion;
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

        public Pedido obtenerProductosAPartirdePreciosCotizados(Pedido pedido, Boolean canastaHabitual, Usuario usuario)
        {

            ClienteBL clienteBL = new ClienteBL();
            List<DocumentoDetalle> documentoDetalleList = clienteBL.getPreciosVigentesCliente(pedido.cliente.idCliente);

            if(pedido.pedidoDetalleList == null) {
                pedido.pedidoDetalleList = new List<PedidoDetalle>();
            }

            TipoCambioSunatBL tcBl = new TipoCambioSunatBL();
            TipoCambioSunat tc = tcBl.GetTipoCambioHoy();


            //Detalle de la cotizacion
            foreach (DocumentoDetalle documentoDetalle in documentoDetalleList)
            {
                if (!canastaHabitual || (canastaHabitual && documentoDetalle.producto.precioClienteProducto.estadoCanasta))
                {
                    pedido.pedidoDetalleList.Remove(pedido.pedidoDetalleList.Where(p => p.producto.idProducto == documentoDetalle.producto.idProducto).FirstOrDefault());

                    if (!documentoDetalle.producto.precioClienteProducto.moneda.codigo.Equals(pedido.moneda.codigo))
                    {
                        if (pedido.moneda.codigo.Equals("USD"))
                        {
                            documentoDetalle.precioNeto = documentoDetalle.precioNeto / tc.valorSunatVenta;

                            if (documentoDetalle.producto.precioClienteProducto.idPrecioClienteProducto != Guid.Empty)
                            {
                                documentoDetalle.producto.precioClienteProducto.precioNetoOriginal = documentoDetalle.producto.precioClienteProducto.precioNeto;
                                documentoDetalle.producto.precioClienteProducto.precioUnitarioOriginal = documentoDetalle.producto.precioClienteProducto.precioUnitario;
                                documentoDetalle.producto.precioClienteProducto.fleteOriginal = documentoDetalle.producto.precioClienteProducto.flete;

                                documentoDetalle.producto.precioClienteProducto.precioNeto = documentoDetalle.producto.precioClienteProducto.precioNetoOriginal / tc.valorSunatVenta;
                                documentoDetalle.producto.precioClienteProducto.flete = documentoDetalle.producto.precioClienteProducto.fleteOriginal / tc.valorSunatVenta;
                                documentoDetalle.producto.precioClienteProducto.precioUnitario = documentoDetalle.producto.precioClienteProducto.precioUnitarioOriginal / tc.valorSunatVenta;
                            }
                        }
                        else
                        {
                            documentoDetalle.precioNeto = documentoDetalle.precioNeto * tc.valorSunatVenta;

                            if (documentoDetalle.producto.precioClienteProducto.idPrecioClienteProducto != Guid.Empty)
                            {
                                documentoDetalle.producto.precioClienteProducto.precioNetoOriginal = documentoDetalle.producto.precioClienteProducto.precioNeto;
                                documentoDetalle.producto.precioClienteProducto.precioUnitarioOriginal = documentoDetalle.producto.precioClienteProducto.precioUnitario;
                                documentoDetalle.producto.precioClienteProducto.fleteOriginal = documentoDetalle.producto.precioClienteProducto.flete;

                                documentoDetalle.producto.precioClienteProducto.precioNeto = documentoDetalle.producto.precioClienteProducto.precioNetoOriginal * tc.valorSunatVenta;
                                documentoDetalle.producto.precioClienteProducto.flete = documentoDetalle.producto.precioClienteProducto.fleteOriginal * tc.valorSunatVenta;
                                documentoDetalle.producto.precioClienteProducto.precioUnitario = documentoDetalle.producto.precioClienteProducto.precioUnitarioOriginal * tc.valorSunatVenta;                                
                            }
                        }

                        documentoDetalle.producto.precioClienteProducto.precioNeto = Decimal.Parse(String.Format(Constantes.formatoCuatroDecimales, documentoDetalle.producto.precioClienteProducto.precioNeto));
                        documentoDetalle.producto.precioClienteProducto.precioUnitario = Decimal.Parse(String.Format(Constantes.formatoCuatroDecimales, documentoDetalle.producto.precioClienteProducto.precioUnitario));
                        documentoDetalle.producto.precioClienteProducto.flete = Decimal.Parse(String.Format(Constantes.formatoCuatroDecimales, documentoDetalle.producto.precioClienteProducto.flete));

                        documentoDetalle.precioNeto = Decimal.Parse(String.Format(Constantes.formatoCuatroDecimales, documentoDetalle.precioNeto));
                    }

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
                List<Pedido> pedidoIds = dal.SelectPedidosSinAtencion();
                foreach (Pedido id in pedidoIds)
                {
                    Pedido pedido = dal.SelectPedidoEmail(id.idPedido);
                    pedido.accion_truncar = id.accion_truncar;
                    pedido.accion_alertarNoAtendido = id.accion_alertarNoAtendido;

                    pedidoList.Add(pedido);
                }
            }

            return pedidoList;
        }

        public bool UpdateDetallesRestriccion(Guid idPedido, List<Guid> idDetalles, List<int> cantidades, List<String> comentarios, Guid idUsuario)
        {
            using (var dal = new PedidoDAL())
            {
                return dal.UpdateDetallesRestriccion(idPedido, idDetalles, cantidades, comentarios, idUsuario);
            }
        }

        public bool TruncarPedidos(List<Guid> idPedidos)
        {
            using (var dal = new PedidoDAL())
            {
                return dal.TruncarPedidos(idPedidos);
            }
        }

        public void ProcesarPedidoAprobadoTecnica(Pedido pedido)
        {
            if (pedido.idMPPedido == null || pedido.idMPPedido.Equals(Guid.Empty))
            {
                if (pedido.usuario.codigoEmpresa.Equals(Constantes.EMPRESA_CODIGO_TECNICA))
                {
                    this.EnviarMailTecnica(pedido);
                }
                this.ReplicarPedidoEntornoMP(pedido);
            }
        }

        public void ReplicarPedidoEntornoMP(Pedido pedido)
        {
            Pedido pMP = (Pedido)pedido.Clone();
            Guid idPedidoTec = pedido.idPedido;
            pedido.idPedido = Guid.Empty;

            ClienteDAL clienteDal = new ClienteDAL();
            
            Guid idCliente = clienteDal.getClienteEmpresa(pedido.usuario.idEmpresa, pedido.ciudad.idCiudad);
            Cliente clienteEmp = clienteDal.getCliente(idCliente);
            pMP.cliente = clienteEmp;

            UsuarioDAL usuarioDal = new UsuarioDAL();
            EmpresaDAL empresaDal = new EmpresaDAL();
            Empresa empresa = empresaDal.getEmpresaByCliente(pedido.cliente.idCliente);

            Usuario usuarioZAS = usuarioDal.getUsuario(Constantes.IDUSUARIOZAS);
            pMP.usuario = usuarioZAS;
            pMP.IdUsuarioRegistro = usuarioZAS.idUsuario;
            pMP.esPagoContado = false;
            pMP.entregaATerceros = pedido.usuario.atencionTerciarizadaEmpresa;
            pMP.entregaTerciarizada = false;

            if (pMP.entregaATerceros)
            {
                pMP.idClienteTercero = pedido.cliente.idCliente;
            }

            pMP.numeroRequerimiento = "";
            pMP.numeroReferenciaAdicional = "";
            pMP.numeroReferenciaCliente = "";
            pMP.observaciones = pMP.observaciones + " N° Pedido " + pedido.usuario.razonSocialEmpresa + ": " + pedido.numeroPedido.ToString() + ". Cliente: " + pedido.cliente.nombreCliente;

            foreach (PedidoDetalle det in pMP.pedidoDetalleList)
            {
                decimal margenDet = ((det.precioNeto - det.producto.costoLista) / det.precioNeto) * 100;

                if (det.esPrecioAlternativo)
                {
                    margenDet = (((det.precioNeto * det.ProductoPresentacion.Equivalencia) - det.producto.costoLista) / (det.precioNeto * det.ProductoPresentacion.Equivalencia)) * 100;
                }

                det.tieneInfraMargenEmpresaExterna = false;


                if (margenDet < empresa.porcentajeMargenMinimo)
                {
                    
                    // Formula descuento inframargen
                    if (det.esPrecioAlternativo)
                    {
                        det.precioNeto = det.precioNeto * det.ProductoPresentacion.Equivalencia * ((100 - empresa.porcentajeDescuentoInframargen) / 100);
                    } else
                    {
                        det.precioNeto = det.precioNeto * ((100 - empresa.porcentajeDescuentoInframargen) / 100);
                    }
                    
                    /* Descuento a la ganancia
                    if (det.esPrecioAlternativo)
                    {
                        //det.precioNeto = det.precioNeto * det.ProductoPresentacion.Equivalencia * ((100 - usuarioEmpresa.pDescuentoInfraMargen) / 100);
                        det.precioNeto = det.producto.costoLista + ((det.precioNeto * det.ProductoPresentacion.Equivalencia) - det.producto.costoLista) * ((100 - empresa.porcentajeDescuentoInframargen) / 100);
                    }
                    else
                    {
                        det.precioNeto = det.producto.costoLista + (det.precioNeto - det.producto.costoLista) * ((100 - empresa.porcentajeDescuentoInframargen) / 100);
                    }
                    */

                    pMP.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.Ingresado;

                    det.tieneInfraMargenEmpresaExterna = true;
                } else
                {
                    decimal precioVentaMP = det.producto.costoLista * empresa.factorCosto;
                    decimal precioVentaGanMax = det.precioNeto * (100 - empresa.porcentajeMDGanaciaMax) / 100;

                    if (det.esPrecioAlternativo)
                    {
                        precioVentaGanMax = det.precioNeto * det.ProductoPresentacion.Equivalencia * (100 - empresa.porcentajeMDGanaciaMax) / 100;
                    }

                    if (precioVentaMP >= precioVentaGanMax)
                    { // Escenario Primavera
                        det.precioNeto = precioVentaMP;
                    } else
                    { // Escenario Verano
                        det.precioNeto = precioVentaGanMax;
                    }
                }
            }

            this.calcularMontosTotales(pMP);

            if (pMP.entregaATerceros)
            {
                pMP.observacionesGuiaRemision = pMP.observacionesGuiaRemision + " // Entrega por encargo de " + pedido.usuario.razonSocialEmpresa;
            } else
            {
                pMP.observacionesGuiaRemision = pMP.observacionesGuiaRemision + " // Dejar en " + pedido.cliente.razonSocial;
            }
            

            this.InsertPedido(pMP);
            this.SetPedidoMP(idPedidoTec, pMP.idPedido, "// N° Pedido MP: " + pMP.numeroPedido.ToString());
            this.SetPedidoMP(pMP.idPedido, idPedidoTec, "", true);

            pedido.idMPPedido = pMP.idPedido;
            pedido.numeroPedidoMP = pMP.numeroPedido;
        }

        public void EnviarMailTecnica(Pedido pedido) 
        {
            MailService mail = new MailService();
            //try
            //{

                PedidoBL pedidoBL = new PedidoBL();
                ParametroBL parametroBL = new ParametroBL();
                string emailsNotificar = parametroBL.getParametro("TC_EMAILS_PEDIDO_ATENDER");

                if (pedido.cliente != null)
                {
                    var urlVerPedido = "http://zasmp.azurewebsites.net/Pedido?idPedido=" + pedido.idPedido.ToString();

                    List<String> destinatarios = new List<String>();

                    Boolean seEnvioCorreo = false;
                    //if (pedido.cliente.asistenteServicioCliente != null && pedido.cliente.asistenteServicioCliente.usuario != null
                    //    && pedido.cliente.asistenteServicioCliente.usuario.email != null && !pedido.cliente.asistenteServicioCliente.usuario.email.Equals(String.Empty))
                    //{
                    //    destinatarios.Add(pedido.cliente.asistenteServicioCliente.usuario.email);
                    //    seEnvioCorreo = true;
                    //}
                    //if (pedido.cliente.responsableComercial != null && pedido.cliente.responsableComercial.usuario != null
                    //    && pedido.cliente.responsableComercial.usuario.email != null && !pedido.cliente.responsableComercial.usuario.email.Equals(String.Empty))
                    //{
                    //    destinatarios.Add(pedido.cliente.responsableComercial.usuario.email);
                    //    seEnvioCorreo = true;
                    //}
                    //if (pedido.cliente.supervisorComercial != null && pedido.cliente.supervisorComercial.usuario != null
                    //    && pedido.cliente.supervisorComercial.usuario.email != null && !pedido.cliente.supervisorComercial.usuario.email.Equals(String.Empty))
                    //{
                    //    destinatarios.Add(pedido.cliente.supervisorComercial.usuario.email);
                    //    seEnvioCorreo = true;
                    //}
                    //if (!pedido.usuario.email.Equals(String.Empty))
                    //{
                    //    destinatarios.Add(pedido.usuario.email);
                    //    seEnvioCorreo = true;
                    //}

                    string[] emails = emailsNotificar.Split(';');

                    foreach (string email in emails)
                    {
                        destinatarios.Add(email.Trim());
                        seEnvioCorreo = true;
                    }


                    if (!pedido.UsuarioRegistro.email.Equals(String.Empty))
                    {
                        destinatarios.Add(pedido.UsuarioRegistro.email);
                        seEnvioCorreo = true;
                    }

                    if (destinatarios.Count > 0)
                    {
                        String asunto = "Nuevo Pedido Ingresado Nro " + pedido.numeroPedidoString;

                        String template = "";

                        PedidoTecnica emailTemplate = new PedidoTecnica();
                        emailTemplate.urlVerPedido = urlVerPedido;
                        template = emailTemplate.BuildTemplate(pedido);
                            
                        mail.enviar(destinatarios, asunto, template, Constantes.MAIL_COMUNICACION_PEDIDOS_NO_ATENDIDOS, Constantes.PASSWORD_MAIL_COMUNICACION_PEDIDOS_NO_ATENDIDOS, new Usuario());
                    }
                }

            //}
            //catch (Exception ex)
            //{
            //    mail.enviar(new List<string> { "ti@mpinstitucional.com" }, "ERROR al enviar pedido " + pedido.numeroPedidoString + " a ténica", ex.Message + ex.InnerException, Constantes.MAIL_COMUNICACION_PEDIDOS_NO_ATENDIDOS, Constantes.PASSWORD_MAIL_COMUNICACION_PEDIDOS_NO_ATENDIDOS, new Usuario());
            //}

        }

        public List<Guid> soloPedidosALiberar(List<Guid> idPedidos, Guid idUsuario)
        {
            using (var dal = new PedidoDAL())
            {
                return dal.soloPedidosALiberar(idPedidos, idUsuario);
            }
        }

        public List<Guid> soloPedidosAApropbar(List<Guid> idPedidos, Guid idUsuario)
        {
            using (var dal = new PedidoDAL())
            {
                return dal.soloPedidosAApropbar(idPedidos, idUsuario);
            }
        }

        public List<List<String>> totalesRazonSocial(List<Guid> idPedidos, Guid idUsuario)
        {
            using (var dal = new PedidoDAL())
            {
                return dal.totalesRazonSocial(idPedidos, idUsuario);
            }
        }

        public List<List<String>> totalesProductos(List<Guid> idPedidos, Guid idUsuario)
        {
            using (var dal = new PedidoDAL())
            {
                return dal.totalesProductos(idPedidos, idUsuario);
            }
        }

        public List<Pedido> SelectPedidosByIds(List<Guid> idPedidos, int tipoOrdenamiento)
        {
            using (var dal = new PedidoDAL())
            {
                return dal.SelectPedidosByIds(idPedidos, tipoOrdenamiento);
            }
        }

        public List<List<String>> pedidosPendientesPorProducto(Guid idProducto, DateTime fechaInicio, DateTime fechaFin, Guid idCiudad, Guid idUsuario)
        {
            using (var dal = new PedidoDAL())
            {
                return dal.pedidosPendientesPorProducto(idProducto, fechaInicio, fechaFin, idCiudad, idUsuario);
            }
        }

        public bool SetPedidoMP(Guid idPedido, Guid idPedidoMP, String agregarObservacion, bool duplicarArchivos = false)
        {
            using (var dal = new PedidoDAL())
            {
                return dal.SetPedidoMP(idPedido, idPedidoMP, agregarObservacion, duplicarArchivos);
            }
        }

        public string TextoNumerosGuiasPedidoEspejo(Guid idPedido, Guid idPUsuario)
        {
            using (var dal = new PedidoDAL())
            {
                string texto = "";
                List<List<String>> lista = dal.numerosGuiasPedidoEspejo(idPedido, idPUsuario);
                foreach (List<String> numeroDoc in lista)
                {
                    if(!texto.Equals(""))
                    {
                        texto += ", ";
                    }

                    texto = texto + numeroDoc.ElementAt(0) + "-" + numeroDoc.ElementAt(1);
                }
                
                if(lista.Count == 1)
                {
                    texto = "Guía MP " + texto;
                }

                if (lista.Count > 1)
                {
                    texto = "Guías MP " + texto;
                }

                return texto;
            }
        }
    }
}

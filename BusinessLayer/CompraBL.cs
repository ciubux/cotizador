
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;

namespace BusinessLayer
{
    public class CompraBL
    {
         

        public void UpdateCompra(Compra compra)
        {
            using (var dal = new CompraDAL())
            {
                dal.UpdateCompra(compra);
            }           
        }

        public void InsertTransaccionNotaCredito(Compra transaccion)
        {
            using (var dal = new CompraDAL())
            {
                dal.InsertTransaccionNotaCredito(transaccion);
            }
        }

        public void UpdateTransaccionNotaCredito(Compra transaccion)
        {
            using (var dal = new CompraDAL())
            {
                dal.UpdateTransaccionNotaCredito(transaccion);
            }
        }

        public void InsertVentaNotaDebito(Venta compra)
        {
            using (var dal = new VentaDAL())
            {
                dal.InsertVentaNotaDebito(compra);
            }
        }

        public void InsertVentaConsolidada(Venta compra)
        {
            using (var dal = new VentaDAL())
            {
                dal.InsertVentaConsolidada(compra);
            }
        }

        public void InsertVentaRefacturacion(Venta compra)
        {
            using (var dal = new VentaDAL())
            {
                dal.InsertVentaRefacturacion(compra);
            }
        }

        public Transaccion GetPlantillaVenta(Transaccion transaccion, Usuario usuario)
        {
            using (var dal = new VentaDAL())
            {
                transaccion = dal.SelectPlantillaVenta(transaccion, usuario);
                if (transaccion.tipoErrorCrearTransaccion == Venta.TiposErrorCrearTransaccion.NoExisteError)
                {
                    this.procesarVenta(transaccion);
                }
            }
            return transaccion;
        }

        public Transaccion GetPlantillaVentaDescuentoGlobal(Transaccion transaccion, Usuario usuario, Guid idProducto)
        {
            using (var dal = new VentaDAL())
            {
                transaccion = dal.SelectPlantillaVenta(transaccion, usuario, idProducto);
                if (transaccion.tipoErrorCrearTransaccion == Venta.TiposErrorCrearTransaccion.NoExisteError)
                {
                    this.procesarVenta(transaccion);
                }
            }
            return transaccion;
        }



        public Compra GetPlantillaCompra(Compra transaccion, Usuario usuario)
        {
            using (var dal = new CompraDAL())
            {
                transaccion = dal.SelectPlantillaCompra(transaccion, usuario);
                if (transaccion.tipoErrorCrearTransaccion == Venta.TiposErrorCrearTransaccion.NoExisteError)
                {
                    this.procesarVenta(transaccion);
                }
            }
            return transaccion;
        }

        public Compra GetPlantillaCompraDescuentoGlobal(Compra transaccion, Usuario usuario, Guid idProducto)
        {
            using (var dal = new CompraDAL())
            {
                transaccion = dal.SelectPlantillaCompra(transaccion, usuario, idProducto);
                if (transaccion.tipoErrorCrearTransaccion == Venta.TiposErrorCrearTransaccion.NoExisteError)
                {
                    this.procesarVenta(transaccion);
                }
            }
            return transaccion;
        }


        public Transaccion GetPlantillaVentaCargos(Transaccion transaccion, Usuario usuario, List<Guid> idProductoList)
        {
            using (var dal = new VentaDAL())
            {
                transaccion = dal.SelectPlantillaVenta(transaccion, usuario, null, idProductoList);
                if (transaccion.tipoErrorCrearTransaccion == Venta.TiposErrorCrearTransaccion.NoExisteError)
                {
                    this.procesarVenta(transaccion);
                }
            }
            return transaccion;
        }

        public Transaccion GetNotaIngresoTransaccion(Transaccion transaccion, NotaIngreso notaIngreso, Usuario usuario)
        {
            using (var dal = new VentaDAL())
            {
                transaccion = dal.SelectNotaIngresoTransaccion(transaccion, notaIngreso, usuario);
                if (transaccion.tipoErrorCrearTransaccion == Venta.TiposErrorCrearTransaccion.NoExisteError)
                {
                    this.procesarVenta(transaccion);
                }
            }
            return transaccion;
        }






        public Compra GetCompra(Compra compra, Usuario usuario)
        {
            using (var dal = new CompraDAL())
            {
                compra = dal.SelectCompra(compra, usuario);
                //Se agrega comentarios recuperados de los datos registrados en el pedido
                String observacionesFacturaAdicional = String.Empty;

                //Pedido cuenta con orden de compra
                if (compra.pedido.numeroReferenciaCliente != null && compra.pedido.numeroReferenciaCliente.Length > 0)
                {
                    observacionesFacturaAdicional = "O/C N° " + compra.pedido.numeroReferenciaCliente + " ";
                }
                //Pedido cuenta con numero requerimiento
                if (compra.pedido.numeroRequerimiento != null && compra.pedido.numeroRequerimiento.Length > 0)
                {
                    observacionesFacturaAdicional = observacionesFacturaAdicional + "NR: " + compra.pedido.numeroRequerimiento + " ";
                }
                //Direccion Entrega tiene nombre y codigo 
                if (compra.pedido.direccionEntrega.nombre != null && compra.pedido.direccionEntrega.nombre.Length > 0)
                {
                    if (compra.pedido.direccionEntrega.codigoCliente != null && compra.pedido.direccionEntrega.codigoCliente.Length > 0)
                    {
                        observacionesFacturaAdicional = observacionesFacturaAdicional + compra.pedido.direccionEntrega.nombre + " (" + compra.pedido.direccionEntrega.codigoCliente + ")";
                    }
                    else
                    {
                        observacionesFacturaAdicional = observacionesFacturaAdicional + compra.pedido.direccionEntrega.nombre;
                    }
                }

                if (compra.pedido.observacionesFactura != null && !compra.pedido.observacionesFactura.Equals(String.Empty))
                {
                    compra.pedido.observacionesFactura = observacionesFacturaAdicional + " / " + compra.pedido.observacionesFactura;
                }
                else
                {
                    compra.pedido.observacionesFactura = observacionesFacturaAdicional;
                }

                this.procesarVenta(compra);
            }
            return compra;
        }

        
        public Venta GetVentaConsolidada(Venta compra, Usuario usuario)
        {
            using (var dal = new VentaDAL())
            {
                compra = dal.SelectVentaConsolidada(compra, usuario);
                this.procesarVenta(compra);
            }
            return compra;
        }


        public void procesarVenta(Transaccion compra)
        {
            foreach (PedidoDetalle pedidoDetalle in compra.pedido.pedidoDetalleList)
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
                if (compra.pedido.incluidoIGV)
                {
                    //Se agrega el IGV al precioLista
                    decimal precioSinIgv = pedidoDetalle.producto.precioSinIgv;
                    decimal precioLista = precioSinIgv + (precioSinIgv * compra.pedido.montoIGV);
                    pedidoDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, precioLista));
                    //Se agrega el IGV al costoLista
                    decimal costoSinIgv = pedidoDetalle.producto.costoSinIgv;
                    decimal costoLista = costoSinIgv + (costoSinIgv * compra.pedido.montoIGV);
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



    }
}

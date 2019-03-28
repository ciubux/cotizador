
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;


namespace BusinessLayer
{
    public class VentaBL
    {
         

        public void UpdateVenta(Venta venta)
        {


            if (venta.guiaRemision.fechaEmision >= new DateTime(2019, 1, 1, 0, 0, 0))
            {
                using (var dal = new VentaDAL())
                {
                    dal.UpdateVenta(venta);
                }
            }
            else {

                /*using (var dal = new VentaDAL())
                {
                    dal.UpdateVenta(venta);
                }*/

                throw new Exception("No se puede editar una venta de un periodo anterior.");
            }
        }

        public void InsertTransaccionNotaCredito(Transaccion transaccion)
        {
            using (var dal = new VentaDAL())
            {
                dal.InsertTransaccionNotaCredito(transaccion);
            }
        }

        public void UpdateTransaccionNotaCredito(Transaccion transaccion)
        {
            using (var dal = new VentaDAL())
            {
                dal.UpdateTransaccionNotaCredito(transaccion);
            }
        }

        public void InsertVentaNotaDebito(Venta venta)
        {
            using (var dal = new VentaDAL())
            {
                dal.InsertVentaNotaDebito(venta);
            }
        }

        public void InsertVentaConsolidada(Venta venta)
        {
            using (var dal = new VentaDAL())
            {
                dal.InsertVentaConsolidada(venta);
            }
        }

        public void InsertVentaRefacturacion(Venta venta)
        {
            using (var dal = new VentaDAL())
            {
                dal.InsertVentaRefacturacion(venta);
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






        public Venta GetVenta(Venta venta, Usuario usuario)
        {
            using (var dal = new VentaDAL())
            {
                venta = dal.SelectVenta(venta, usuario);
                //Se agrega comentarios recuperados de los datos registrados en el pedido
                String observacionesFacturaAdicional = String.Empty;

                //Pedido cuenta con orden de compra
                if (venta.pedido.numeroReferenciaCliente != null && venta.pedido.numeroReferenciaCliente.Length > 0)
                {
                    observacionesFacturaAdicional = "O/C N° " + venta.pedido.numeroReferenciaCliente + " ";
                }
                //Pedido cuenta con numero requerimiento
                if (venta.pedido.numeroRequerimiento != null && venta.pedido.numeroRequerimiento.Length > 0)
                {
                    observacionesFacturaAdicional = observacionesFacturaAdicional + "NR: " + venta.pedido.numeroRequerimiento + " ";
                }
                //Direccion Entrega tiene nombre y codigo 
                if (venta.pedido.direccionEntrega.nombre != null && venta.pedido.direccionEntrega.nombre.Length > 0)
                {
                    if (venta.pedido.direccionEntrega.codigoCliente != null && venta.pedido.direccionEntrega.codigoCliente.Length > 0)
                    {
                        observacionesFacturaAdicional = observacionesFacturaAdicional + venta.pedido.direccionEntrega.nombre + " (" + venta.pedido.direccionEntrega.codigoCliente + ")";
                    }
                    else
                    {
                        observacionesFacturaAdicional = observacionesFacturaAdicional + venta.pedido.direccionEntrega.nombre;
                    }
                }

                if (venta.pedido.observacionesFactura != null && !venta.pedido.observacionesFactura.Equals(String.Empty))
                {
                    venta.pedido.observacionesFactura = observacionesFacturaAdicional + " / " + venta.pedido.observacionesFactura;
                }
                else
                {
                    venta.pedido.observacionesFactura = observacionesFacturaAdicional;
                }

                this.procesarVenta(venta);
            }
            return venta;
        }

        
        public Venta GetVentaConsolidada(Venta venta, Usuario usuario)
        {
            using (var dal = new VentaDAL())
            {
                venta = dal.SelectVentaConsolidada(venta, usuario);
                this.procesarVenta(venta);
            }
            return venta;
        }


        public void procesarVenta(Venta venta)
        {
            foreach (PedidoDetalle pedidoDetalle in venta.pedido.pedidoDetalleList)
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
                if (venta.pedido.incluidoIGV)
                {
                    //Se agrega el IGV al precioLista
                    decimal precioSinIgv = pedidoDetalle.producto.precioSinIgv;
                    decimal precioLista = precioSinIgv + (precioSinIgv * venta.pedido.montoIGV);
                    pedidoDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, precioLista));
                    //Se agrega el IGV al costoLista
                    decimal costoSinIgv = pedidoDetalle.producto.costoSinIgv;
                    decimal costoLista = costoSinIgv + (costoSinIgv * venta.pedido.montoIGV);
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
        public void ExcelPrueba(List<Guid> id_venta_detalle, Usuario usuario,List<string> responsable_comercial, List<string> supervisor_comercial ,List<string> asistente_servicio, List<string> canal_multiregional, List<string> canal_lima, List<string> canal_provincia, List<string> canal_pcp, List<int> origen)
        {
            VentaDAL ventaDAL = new VentaDAL();

            ventaDAL.ExcelPrueba(id_venta_detalle, usuario, responsable_comercial, supervisor_comercial, asistente_servicio, canal_multiregional, canal_lima, canal_provincia, canal_pcp, origen);


        }



    }
}

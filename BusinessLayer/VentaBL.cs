﻿
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
            using (var dal = new VentaDAL())
            {
                dal.UpdateVenta(venta);
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



    }
}

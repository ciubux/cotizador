
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
    public class RequerimientoBL
    {

        #region Requerimientos de VENTA
        private void validarRequerimientoVenta(Requerimiento requerimiento)
        {
            
            if (requerimiento.tipoRequerimiento != Requerimiento.tiposRequerimiento.Venta)
            {
                requerimiento.montoIGV = 0;
                requerimiento.montoTotal = 0;
                requerimiento.montoSubTotal = 0;

                //Se revisa si se excedió el presupuesto
     
            }
            else
            {

                requerimiento.excedioPresupuesto = requerimiento.montoSubTotal > requerimiento.direccionEntrega.limitePresupuesto;
                requerimiento.topePresupuesto = requerimiento.direccionEntrega.limitePresupuesto;
                //if (!requerimiento.usuario.apruebaRequerimientos && !requerimiento.cliente.perteneceCanalMultiregional)
                /*   if (!requerimiento.usuario.apruebaRequerimientos)
                   {
                       DateTime horaActual = DateTime.Now;
                       DateTime horaLimite = new DateTime(horaActual.Year, horaActual.Month, horaActual.Day, Constantes.HORA_CORTE_CREDITOS_LIMA.Hour, Constantes.HORA_CORTE_CREDITOS_LIMA.Minute, Constantes.HORA_CORTE_CREDITOS_LIMA.Second);
                       //if (horaActual >= horaLimite && requerimiento.ciudad.idCiudad == Guid.Parse("15526227-2108-4113-B46A-1C8AB5C0E581"))//-- .esProvincia)
                       if (horaActual >= horaLimite && requerimiento.ciudad.idCiudad == Guid.Parse("15526227-2108-4113-B46A-1C8AB5C0E581"))//-- .esProvincia)
                       {
                           requerimiento.seguimientoCrediticioRequerimiento.estado = SeguimientoCrediticioRequerimiento.estadosSeguimientoCrediticioRequerimiento.PendienteLiberación;
                           requerimiento.seguimientoCrediticioRequerimiento.observacion = "Se ha superado la Hora de Corte, la hora de corte actualmente es: " + Constantes.HORA_CORTE_CREDITOS_LIMA.Hour.ToString() + ":" + (Constantes.HORA_CORTE_CREDITOS_LIMA.Minute > 9 ? Constantes.HORA_CORTE_CREDITOS_LIMA.Minute.ToString() : "0" + Constantes.HORA_CORTE_CREDITOS_LIMA.Minute.ToString());
                      }
                   }*/
               
                /*
                if (!requerimiento.usuario.apruebaRequerimientos && !requerimiento.cliente.perteneceCanalMultiregional)
                {
                    requerimiento.seguimientoCrediticioRequerimiento.estado = SeguimientoCrediticioRequerimiento.estadosSeguimientoCrediticioRequerimiento.PendienteLiberación;
                }*/
            }
           

          

        }
                
        public void InsertRequerimiento(Requerimiento requerimiento)
        {
            using (var dal = new RequerimientoDAL())
            {

                String observacionesAdicionales = String.Empty;
              if (requerimiento.direccionEntrega.nombre != null && requerimiento.direccionEntrega.nombre.Length > 0)
              {
                  if (requerimiento.direccionEntrega.codigoCliente != null && requerimiento.direccionEntrega.codigoCliente.Length > 0)
                  {
                        observacionesAdicionales = requerimiento.direccionEntrega.nombre + " (" + requerimiento.direccionEntrega.codigoCliente + ")";
                  }
                  else
                  {
                        observacionesAdicionales = requerimiento.direccionEntrega.nombre;
                  }                
              }
                if (requerimiento.observaciones != null && !requerimiento.observaciones.Equals(String.Empty))
                {
                    requerimiento.observaciones = requerimiento.observaciones + " / " + observacionesAdicionales;
                }
                else
                {
                    requerimiento.observaciones = observacionesAdicionales;
                }

                validarRequerimientoVenta(requerimiento);
                dal.InsertRequerimiento(requerimiento);
            }
        }

        public void UpdateRequerimiento(Requerimiento requerimiento)
        {
            using (var dal = new RequerimientoDAL())
            {
                validarRequerimientoVenta(requerimiento);
                dal.UpdateRequerimiento(requerimiento);
            }
        }

      
        public List<Requerimiento> GetRequerimientos(Requerimiento requerimiento)
        {
            List<Requerimiento> requerimientoList = null;
            using (var dal = new RequerimientoDAL())
            {
                requerimientoList = dal.SelectRequerimientos(requerimiento);
            }
            return requerimientoList;
        }


        #endregion
        public Requerimiento GetRequerimiento(Requerimiento requerimiento, Usuario usuario)
        {
            using (var dal = new RequerimientoDAL())
            {
                requerimiento = dal.SelectRequerimiento(requerimiento, usuario);

                /*    if (pedido.mostrarValidezOfertaEnDias == 0)
                    {
                        TimeSpan diferencia;
                        diferencia = cotizacion.fechaLimiteValidezOferta - cotizacion.fecha;
                        cotizacion.validezOfertaEnDias = diferencia.Days;
                    }
                    */
                foreach (RequerimientoDetalle pedidoDetalle in requerimiento.requerimientoDetalleList)
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
                    if (requerimiento.incluidoIGV)
                    {
                        //Se agrega el IGV al precioLista
                        decimal precioSinIgv = pedidoDetalle.producto.precioSinIgv;
                        decimal precioLista = precioSinIgv + (precioSinIgv * requerimiento.montoIGV);
                        pedidoDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, precioLista));
                        //Se agrega el IGV al costoLista
                        decimal costoSinIgv = pedidoDetalle.producto.costoSinIgv;
                        decimal costoLista = costoSinIgv + (costoSinIgv * requerimiento.montoIGV);
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
            return requerimiento;
        }

        public Requerimiento GetRequerimientoParaEditar(Requerimiento requerimiento, Usuario usuario)
        {
            using (var dal = new RequerimientoDAL())
            {
                requerimiento = dal.SelectRequerimientoParaEditar(requerimiento, usuario);

                /*    if (pedido.mostrarValidezOfertaEnDias == 0)
                    {
                        TimeSpan diferencia;
                        diferencia = cotizacion.fechaLimiteValidezOferta - cotizacion.fecha;
                        cotizacion.validezOfertaEnDias = diferencia.Days;
                    }
                    */
                foreach (RequerimientoDetalle pedidoDetalle in requerimiento.requerimientoDetalleList)
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
                    if (requerimiento.incluidoIGV)
                    {
                        //Se agrega el IGV al precioLista
                        decimal precioSinIgv = pedidoDetalle.producto.precioSinIgv;
                        decimal precioLista = precioSinIgv + (precioSinIgv * requerimiento.montoIGV);
                        pedidoDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, precioLista));
                        //Se agrega el IGV al costoLista
                        decimal costoSinIgv = pedidoDetalle.producto.costoSinIgv;
                        decimal costoLista = costoSinIgv + (costoSinIgv * requerimiento.montoIGV);
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
            return requerimiento;
        }

        public Requerimiento obtenerProductosAPartirdePreciosRegistrados(Requerimiento requerimiento, String familia, String proveedor, Usuario usuario)
        {

            ProductoBL productoBL = new ProductoBL();
            List<DocumentoDetalle> documentoDetalleList = productoBL.obtenerProductosAPartirdePreciosRegistradosParaPedido(requerimiento.cliente.idCliente, requerimiento.fechaPrecios, requerimiento.ciudad.esProvincia, requerimiento.incluidoIGV, familia, proveedor);

            requerimiento.requerimientoDetalleList = new List<RequerimientoDetalle>();
            //Detalle de la cotizacion
            foreach (DocumentoDetalle documentoDetalle in documentoDetalleList)
            {
                RequerimientoDetalle requerimientoDetalle = new RequerimientoDetalle(usuario.visualizaCostos, usuario.visualizaMargen);
                requerimientoDetalle.producto = new Producto();
                requerimientoDetalle.cantidad = 1;
                requerimientoDetalle.esPrecioAlternativo = documentoDetalle.esPrecioAlternativo;
                requerimientoDetalle.unidad = documentoDetalle.unidad;
                requerimientoDetalle.producto = documentoDetalle.producto;
                if (requerimientoDetalle.esPrecioAlternativo)
                {
                    requerimientoDetalle.ProductoPresentacion = documentoDetalle.ProductoPresentacion;
                    requerimientoDetalle.precioNeto = documentoDetalle.precioNeto * documentoDetalle.ProductoPresentacion.Equivalencia;
                }
                else
                {
                    requerimientoDetalle.precioNeto = documentoDetalle.precioNeto;
                }
                requerimientoDetalle.flete = documentoDetalle.flete;
                requerimientoDetalle.observacion = documentoDetalle.observacion;
                requerimientoDetalle.porcentajeDescuento = documentoDetalle.porcentajeDescuento;
                requerimiento.requerimientoDetalleList.Add(requerimientoDetalle);
            }
            return requerimiento;
        }

        public void calcularMontosTotales(Requerimiento requerimiento)
        {
            Decimal total = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, requerimiento.requerimientoDetalleList.AsEnumerable().Sum(o => o.subTotal)));
            Decimal subtotal = 0;
            Decimal igv = 0;

            if (requerimiento.incluidoIGV)
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

            requerimiento.montoTotal = total;
            requerimiento.montoSubTotal = subtotal;
            requerimiento.montoIGV = igv;
        }




    }
}

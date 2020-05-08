
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
    public class OrdenCompraClienteBL
    {


        public void InsertOrdenCompraCliente(OrdenCompraCliente occ)
        {
            using (var dal = new OrdenCompraClienteDAL())
            {

                String observacionesAdicionales = String.Empty;
             
                if (occ.observaciones != null && !occ.observaciones.Equals(String.Empty))
                {
                    occ.observaciones = occ.observaciones + " / " + observacionesAdicionales;
                }
                else
                {
                    occ.observaciones = observacionesAdicionales;
                }


                dal.InsertOrdenCompraCliente(occ);
            }
        }

        public void UpdateOrdenCompraCliente(OrdenCompraCliente occ)
        {
            using (var dal = new OrdenCompraClienteDAL())
            {
                dal.UpdateOrdenCompraCliente(occ);
            }
        }

        public void ProgramarOrdenCompraCliente(OrdenCompraCliente occ, Usuario usuario)
        {
            using (var dal = new OrdenCompraClienteDAL())
            {
                dal.ProgramarOrdenCompraCliente(occ, usuario);
            }
        }

        public List<OrdenCompraCliente> GetOrdenCompraClientes(OrdenCompraCliente occ)
        {
            List<OrdenCompraCliente> occList = null;
            using (var dal = new OrdenCompraClienteDAL())
            {
                occList = dal.SelectOrdenCompraClientes(occ);
            }
            return occList;
        }



        #region General

        

        public OrdenCompraCliente obtenerProductosAPartirdePreciosCotizados(OrdenCompraCliente occ, Boolean canastaHabitual, Usuario usuario)
        {

            ClienteBL clienteBL = new ClienteBL();
            List<DocumentoDetalle> documentoDetalleList = clienteBL.getPreciosVigentesCliente(occ.cliente.idCliente);

            if(occ.detalleList == null) {
                occ.detalleList = new List<OrdenCompraClienteDetalle>();
            }

            //Detalle de la cotizacion
            foreach (DocumentoDetalle documentoDetalle in documentoDetalleList)
            {
                if (!canastaHabitual || (canastaHabitual && documentoDetalle.producto.precioClienteProducto.estadoCanasta))
                {
                    occ.detalleList.Remove(occ.detalleList.Where(p => p.producto.idProducto == documentoDetalle.producto.idProducto).FirstOrDefault());

                    OrdenCompraClienteDetalle occDetalle = new OrdenCompraClienteDetalle(usuario.visualizaCostos, usuario.visualizaMargen);
                    occDetalle.producto = new Producto();
                    occDetalle.cantidad = 1;
                    occDetalle.esPrecioAlternativo = documentoDetalle.esPrecioAlternativo;
                    occDetalle.unidad = documentoDetalle.unidad;
                    occDetalle.producto = documentoDetalle.producto;
                    if (occDetalle.esPrecioAlternativo)
                    {
                        occDetalle.ProductoPresentacion = documentoDetalle.ProductoPresentacion;
                        occDetalle.precioNeto = documentoDetalle.precioNeto * documentoDetalle.ProductoPresentacion.Equivalencia;
                    }
                    else
                    {
                        occDetalle.precioNeto = documentoDetalle.precioNeto;
                    }
                    occDetalle.flete = documentoDetalle.flete;
                    occDetalle.observacion = documentoDetalle.observacion;
                    occDetalle.porcentajeDescuento = documentoDetalle.porcentajeDescuento;
                    occ.detalleList.Add(occDetalle);
                }
            }

            return occ;
        }


        public Int64 GetSiguienteNumeroGrupoOrdenCompraCliente()
        {
            using (var dal = new OrdenCompraClienteDAL())
            {
                return dal.SelectSiguienteNumeroGrupoOrdenCompraCliente();
            }
        }

        public OrdenCompraCliente GetOrdenCompraCliente(OrdenCompraCliente occ,Usuario usuario)
        {
            using (var dal = new OrdenCompraClienteDAL())
            {
                occ = dal.SelectOrdenCompraCliente(occ, usuario);

            /*    if (occ.mostrarValidezOfertaEnDias == 0)
                {
                    TimeSpan diferencia;
                    diferencia = cotizacion.fechaLimiteValidezOferta - cotizacion.fecha;
                    cotizacion.validezOfertaEnDias = diferencia.Days;
                }
                */
                foreach (OrdenCompraClienteDetalle occDetalle in occ.detalleList)
                {

                    if (occDetalle.producto.image == null)
                    {
                        FileStream inStream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\images\\NoDisponible.gif", FileMode.Open);
                        MemoryStream storeStream = new MemoryStream();
                        storeStream.SetLength(inStream.Length);
                        inStream.Read(storeStream.GetBuffer(), 0, (int)inStream.Length);
                        storeStream.Flush();
                        inStream.Close();
                        occDetalle.producto.image = storeStream.GetBuffer();
                    }

                    //Si NO es recotizacion
                    if (occ.incluidoIGV)
                    {
                        //Se agrega el IGV al precioLista
                        decimal precioSinIgv = occDetalle.producto.precioSinIgv;
                        decimal precioLista = precioSinIgv + (precioSinIgv * occ.montoIGV);
                        occDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, precioLista));
                        //Se agrega el IGV al costoLista
                        decimal costoSinIgv = occDetalle.producto.costoSinIgv;
                        decimal costoLista = costoSinIgv + (costoSinIgv * occ.montoIGV);
                        occDetalle.producto.costoLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, costoLista));
                    }
                    else
                    {
                        //Se agrega el IGV al precioLista
                        occDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, occDetalle.producto.precioSinIgv));
                        //Se agrega el IGV al costoLista
                        occDetalle.producto.costoLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, occDetalle.producto.costoSinIgv));
                    }

                    if (occDetalle.esPrecioAlternativo)
                    {
                        occDetalle.producto.precioClienteProducto.precioUnitario =
                        occDetalle.producto.precioClienteProducto.precioUnitario / occDetalle.ProductoPresentacion.Equivalencia;
                    }
                }
            }
            return occ;
        }


        public OrdenCompraCliente GetOrdenCompraClienteParaEditar(OrdenCompraCliente occ, Usuario usuario)
        {
            using (var dal = new OrdenCompraClienteDAL())
            {
                occ = dal.SelectOrdenCompraClienteParaEditar(occ, usuario);

                /*    if (occ.mostrarValidezOfertaEnDias == 0)
                    {
                        TimeSpan diferencia;
                        diferencia = cotizacion.fechaLimiteValidezOferta - cotizacion.fecha;
                        cotizacion.validezOfertaEnDias = diferencia.Days;
                    }
                    */
                foreach (OrdenCompraClienteDetalle occDetalle in occ.detalleList)
                {

                    if (occDetalle.producto.image == null)
                    {
                        FileStream inStream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\images\\NoDisponible.gif", FileMode.Open);
                        MemoryStream storeStream = new MemoryStream();
                        storeStream.SetLength(inStream.Length);
                        inStream.Read(storeStream.GetBuffer(), 0, (int)inStream.Length);
                        storeStream.Flush();
                        inStream.Close();
                        occDetalle.producto.image = storeStream.GetBuffer();
                    }

                    //Si NO es recotizacion
                    if (occ.incluidoIGV)
                    {
                        //Se agrega el IGV al precioLista
                        decimal precioSinIgv = occDetalle.producto.precioSinIgv;
                        decimal precioLista = precioSinIgv + (precioSinIgv * occ.montoIGV);
                        occDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, precioLista));
                        //Se agrega el IGV al costoLista
                        decimal costoSinIgv = occDetalle.producto.costoSinIgv;
                        decimal costoLista = costoSinIgv + (costoSinIgv * occ.montoIGV);
                        occDetalle.producto.costoLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, costoLista));
                    }
                    else
                    {
                        //Se agrega el IGV al precioLista
                        occDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, occDetalle.producto.precioSinIgv));
                        //Se agrega el IGV al costoLista
                        occDetalle.producto.costoLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, occDetalle.producto.costoSinIgv));
                    }

                    if (occDetalle.esPrecioAlternativo)
                    {
                        occDetalle.producto.precioClienteProducto.precioUnitario =
                        occDetalle.producto.precioClienteProducto.precioUnitario / occDetalle.ProductoPresentacion.Equivalencia;
                    }
                }
            }
            return occ;
        }
        
        public void calcularMontosTotales(OrdenCompraCliente occ)
        {
            Decimal total = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, occ.detalleList.AsEnumerable().Sum(o => o.subTotal)));
            Decimal subtotal = 0;
            Decimal igv = 0;

            if (occ.incluidoIGV)
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

            occ.montoTotal = total;
            occ.montoSubTotal = subtotal;
            occ.montoIGV = igv;
        }

        #endregion


    }
}
 
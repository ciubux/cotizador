using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    static public class HelperDocumento
    {

        static public void calcularMontosTotales(IDocumento iDocumento)
        {
            Decimal total = 0;
            Decimal subtotal = 0;
            Decimal igv = 0;

            foreach (DocumentoDetalle det in iDocumento.documentoDetalle)
            {
                if (iDocumento.incluidoIGV)
                {
                    if (det.producto.exoneradoIgv)
                    {
                        subtotal = subtotal + det.subTotal;
                    } else
                    {
                        subtotal = subtotal + (det.subTotal * ((decimal) (1/1.18)));
                        igv = igv + (det.subTotal * ((decimal) (0.18/1.18)));
                    }
                }
                else
                {
                    subtotal = subtotal + det.subTotal;
                    if (!det.producto.exoneradoIgv)
                    {
                        igv = igv + (det.subTotal * ((decimal) 0.18));
                    }
                }
            }

            subtotal = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, subtotal));
            igv = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, igv));
            total = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, subtotal + igv));

            iDocumento.montoTotal = total;
            iDocumento.montoSubTotal = subtotal;
            iDocumento.montoIGV = igv ;
        }

        static public List<DocumentoDetalle> updateDocumentoDetalle(IDocumento documento, List<DocumentoDetalleJson> cotizacionDetalleJsonList, bool ajustePrecios = false)
        {
            if (cotizacionDetalleJsonList != null)
            {
                List<DocumentoDetalle> detalles = documento.documentoDetalle;
                List<Guid> idProductos = new List<Guid>();
                foreach (DocumentoDetalleJson cotizacionDetalleJson in cotizacionDetalleJsonList)
                {
                    idProductos.Add(Guid.Parse(cotizacionDetalleJson.idProducto));
                }
                List<DocumentoDetalle> detallesPorActualizar = detalles.Where(s => idProductos.Contains(s.producto.idProducto)).ToList();
                List<DocumentoDetalle> detallesOrdenados = new List<DocumentoDetalle>();
                //Se realiza este foreach para considerar el ordenamiento
                foreach (Guid idProducto in idProductos)
                {
                    DocumentoDetalleJson cotizacionDetalleJson = cotizacionDetalleJsonList.Where(s => s.idProducto == idProducto.ToString()).FirstOrDefault();
                    DocumentoDetalle documentoDetalle = detallesPorActualizar.Where(s => s.producto.idProducto == idProducto).FirstOrDefault();

                    documentoDetalle.cantidad = cotizacionDetalleJson.cantidad;

                    
                    documentoDetalle.flete = cotizacionDetalleJson.flete;
                    documentoDetalle.reverseSubTotal = cotizacionDetalleJson.reverseSubTotal;

                    decimal precioNetoAnterior = documentoDetalle.precioNeto;
                    Decimal subTotal = 0;

                    if (documentoDetalle.esPrecioAlternativo)
                    {
                        documentoDetalle.precioNeto = Decimal.Parse(String.Format(Constantes.formatoCuatroDecimales, cotizacionDetalleJson.precio * documentoDetalle.ProductoPresentacion.Equivalencia));

                        if (documentoDetalle.subTotal != documentoDetalle.reverseSubTotal)
                        {
                            documentoDetalle.precioNeto = Decimal.Parse(String.Format(Constantes.formatoCuatroDecimales, (decimal.Round(documentoDetalle.reverseSubTotal / documentoDetalle.cantidad, 4,MidpointRounding.ToEven) - documentoDetalle.flete) * documentoDetalle.ProductoPresentacion.Equivalencia));
                        }
                    }
                    else
                    {
                        documentoDetalle.precioNeto = cotizacionDetalleJson.precio;

                        if (documentoDetalle.reverseSubTotal > 0 && documentoDetalle.subTotal != documentoDetalle.reverseSubTotal)
                        {
                            documentoDetalle.precioNeto = Decimal.Parse(String.Format(Constantes.formatoCuatroDecimales, decimal.Round(documentoDetalle.reverseSubTotal / documentoDetalle.cantidad, 4, MidpointRounding.ToEven) - documentoDetalle.flete));
                        }
                    }

                    if (ajustePrecios)
                    {
                        if ((documentoDetalle.ProductoPresentacion == null || documentoDetalle.ProductoPresentacion.IdProductoPresentacion == 0) && documentoDetalle.producto.equivalenciaAlternativa > 1)
                        {
                            decimal precioA = documentoDetalle.precioNeto / documentoDetalle.producto.equivalenciaAlternativa;
                            precioA = Math.Truncate(precioA * 10000);
                            decimal precioN = precioA * documentoDetalle.producto.equivalenciaAlternativa / 10000;

                            while ((precioN * 100) - (Math.Truncate(precioN * 100)) > (decimal)0.001)
                            {
                                precioA--;
                                precioN = precioA * documentoDetalle.producto.equivalenciaAlternativa / 10000;
                            }
                            documentoDetalle.precioNeto = precioA * documentoDetalle.producto.equivalenciaAlternativa / 10000;

                        }
                        
                        //Proveedor
                        if (documentoDetalle.ProductoPresentacion != null && documentoDetalle.ProductoPresentacion.IdProductoPresentacion == 2 && (documentoDetalle.producto.equivalenciaProveedor > 1 || documentoDetalle.producto.equivalenciaAlternativa > 1))
                        {
                            decimal equivalenciaA = documentoDetalle.producto.equivalenciaAlternativa * documentoDetalle.producto.equivalenciaProveedor;
                            decimal precioA = documentoDetalle.precioNeto / equivalenciaA;
                            precioA = Math.Truncate(precioA * 10000);
                            decimal precioN = (precioA * equivalenciaA) / 10000;

                            while ((precioN * 100) - (Math.Truncate(precioN * 100)) > (decimal)0.001)
                            {
                                precioA--;
                                precioN = precioA * equivalenciaA / 10000;
                            }

                            documentoDetalle.precioNeto = (precioA * documentoDetalle.producto.equivalenciaAlternativa) / 10000;
                        }
                    }

                    if (precioNetoAnterior != documentoDetalle.precioNeto || documentoDetalle.porcentajeDescuento != cotizacionDetalleJson.porcentajeDescuento)
                    {
                        documentoDetalle.validar = true;
                    }

                    documentoDetalle.porcentajeDescuento = cotizacionDetalleJson.porcentajeDescuento;
                    documentoDetalle.observacion = cotizacionDetalleJson.observacion;
                    detallesOrdenados.Add(documentoDetalle);
                }

                return detallesOrdenados;
            }
            else
            {
                return new List<DocumentoDetalle>();
            }
        }

    }
}

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
            Decimal total = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, iDocumento.documentoDetalle.AsEnumerable().Sum(o => o.subTotal)));
            Decimal subtotal = 0;
            Decimal igv = 0;

            if (iDocumento.incluidoIGV)
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

                    decimal precioNetoAnterior = documentoDetalle.precioNeto;

                    if (documentoDetalle.esPrecioAlternativo)
                    {
                        documentoDetalle.precioNeto = Decimal.Parse(String.Format(Constantes.formatoCuatroDecimales, cotizacionDetalleJson.precio * documentoDetalle.ProductoPresentacion.Equivalencia));
                    }
                    else
                    {
                        documentoDetalle.precioNeto = cotizacionDetalleJson.precio;
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

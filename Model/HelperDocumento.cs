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

        static public List<DocumentoDetalle> updateDocumentoDetalle(IDocumento documento, List<DocumentoDetalleJson> cotizacionDetalleJsonList)
        {
            if (cotizacionDetalleJsonList != null)
            {
                List<DocumentoDetalle> detalles = documento.documentoDetalle;
                List<Guid> idProductos = new List<Guid>();
                foreach (DocumentoDetalleJson cotizacionDetalleJson in cotizacionDetalleJsonList)
                {
                    idProductos.Add(Guid.Parse(cotizacionDetalleJson.idProducto));
                }
                List<DocumentoDetalle> detallesActualizados = detalles.Where(s => idProductos.Contains(s.producto.idProducto)).ToList();
                List<DocumentoDetalle> detallesOrdenados = new List<DocumentoDetalle>();
                //Se realiza este foreach para considerar el ordenamiento
                foreach (Guid idProducto in idProductos)
                {
                    DocumentoDetalleJson cotizacionDetalleJson = cotizacionDetalleJsonList.Where(s => s.idProducto == idProducto.ToString()).FirstOrDefault();
                    DocumentoDetalle documentoDetalle = detallesActualizados.Where(s => s.producto.idProducto == idProducto).FirstOrDefault();

                    documentoDetalle.cantidad = cotizacionDetalleJson.cantidad;

                    documentoDetalle.precioNeto = cotizacionDetalleJson.precio;
                    documentoDetalle.flete = cotizacionDetalleJson.flete;

                    if (documentoDetalle.esPrecioAlternativo)
                    {
                        documentoDetalle.precioNeto = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cotizacionDetalleJson.precio * documentoDetalle.producto.equivalencia));
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

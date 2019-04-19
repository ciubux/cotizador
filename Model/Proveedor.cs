using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class Proveedor : Persona
    {
        public Proveedor()
        {
            //Cada vez que se instancia un cliente se instancia con al menos una dirección de entrega
            this.direccionEntregaList = new List<DireccionEntrega>();
            this.solicitanteList = new List<Solicitante>();
            this.ubigeo = new Ubigeo();
            this.tipoDocumentoIdentidad = DocumentoVenta.TiposDocumentoIdentidad.RUC;

        }

        public String nombre { get; set; }

        public Guid idProveedor { get; set; }


        [Display(Name = "Plazo Crédito (Aprobado):")]
        public new DocumentoCompra.TipoPago tipoPagoFactura { get; set; }

        [Display(Name = "Forma de Pago Factura:")]
        public new DocumentoCompra.FormaPago formaPagoFactura { get; set; }
    }
}
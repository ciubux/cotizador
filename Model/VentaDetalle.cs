using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class VentaDetalle : DocumentoDetalle
    {
        public Guid idVentaDetalle { get; set; }
        public Guid venta { get; set; }
        public Decimal sumCantidad { get; set; }
        public Decimal sumCantidadUnidadAlternativa { get; set; }
        public Decimal sumCantidadUnidadEstandar { get; set; }
        public Decimal sumPrecioNeto { get; set; }
        public Decimal sumPrecioUnitario { get; set; }

    }
    
}

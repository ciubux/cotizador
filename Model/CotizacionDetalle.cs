using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class CotizacionDetalle
    {
        public Guid idCotizacionDetalle { get; set; }
        public Guid idCotizacion { get; set; }
        public int cantidad { get; set; }
        public Guid idPrecio { get; set; }
        public Guid idProducto { get; set; }
        public Decimal porcentajeDescuento { get; set; }
        public Decimal valorUnitario { get; set; }

    }
}

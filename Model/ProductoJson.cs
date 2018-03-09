using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class CotizacionDetalleJson
    {
        public String idProducto { get; set; }
        public int cantidad { get; set; }
        public Decimal porcentajeDescuento { get; set; }
        public Decimal precio { get; set; }
        public Decimal flete { get; set; }
        public Decimal costo { get; set; }
    }
}

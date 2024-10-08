using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.UTILES
{
    public class DetalleVenta
    {
        public Guid idProducto { get; set; }
        public string sku { get; set; }
        public decimal precioUnitario { get; set; }

        public decimal igvUnitario { get; set; }

        public decimal equivalencia { get; set; }

        public int cantidad { get; set; }
    }
}

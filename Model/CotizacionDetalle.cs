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
        public Decimal valorUnitarioFinal { get; set; }
        public Decimal subTotal { get; set; }

        public PrecioLista precioLista { get; set; }
        public Familia familia { get; set; }
        public Categoria categoria { get; set; }
        public Proveedor proveedor { get; set; }
        public Producto producto { get; set; }
        public Moneda moneda { get; set; }


    }
}

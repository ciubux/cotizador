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
        public Decimal porcentajeDescuento { get; set; }
        public String unidad { get; set; }
        public Decimal subTotal { get; set; }      
     //   public PrecioLista precioLista { get; set; }
        public Familia familia { get; set; }
        public Categoria categoria { get; set; }
        public Proveedor proveedor { get; set; }
        public Producto producto { get; set; }
        public Moneda moneda { get; set; }
        public Decimal precio { get; set; }
        public Boolean esPrecioAlternativo { get; set; }
    
      /*  public Decimal precioUnitarioSinIGV { get; set; }
        public Decimal precioUnitarioAlternativoSinIGV { get; set; }*/
        public Usuario usuario { get; set; }

    }
}

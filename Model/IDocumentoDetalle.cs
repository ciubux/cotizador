using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public interface IDocumentoDetalle
    {
        Decimal subTotal
        {
            get;
        }

        Producto producto
        {
            get;
            set;
        }

        int cantidad { get; set; }
        Decimal precioNeto { get; set; }       
        Decimal flete { get; set; }
        Boolean esPrecioAlternativo { get; set; }
        Decimal porcentajeDescuento { get; set; }
        String observacion { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class PrecioClienteProducto : Auditoria
    {
        public Guid idPrecioClienteProducto { get; set; }

        public DateTime? fechaInicioVigencia { get; set;  }

        public DateTime? fechaFinVigencia { get; set; }

        public String unidad { get; set; }

        public Decimal precioNeto { get; set; }
        public Decimal flete { get; set; }
        public Decimal precioUnitario { get; set; }
        public String numeroCotizacion { get; set; }

    /*    public String nombreVista
        {
            get { return codigo + "(" + nombre + ")"; }
        }*/
    }
}
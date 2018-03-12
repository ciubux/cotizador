using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class PrecioListaStaging
    {
        public String codigoCliente { get; set; }
        public DateTime? fechaVigenciaInicio { get; set; }
        public DateTime? fechaVigenciaFin { get; set; }
        public String sku { get; set; }
        public String consideraCantidades { get; set; }
        public int cantidad { get; set; }
        public String esAlternativa { get; set; }
        public String unidad { get; set; }
        public String moneda { get; set; }
        public Decimal precioLista { get; set; }
        public Decimal precioNeto { get; set; }
        public Decimal costo { get; set; }
        public String flete { get; set; }
        public Decimal porcentajeDescuento { get; set; }
        public String grupo { get; set; }
    }
}

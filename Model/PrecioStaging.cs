using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class PrecioStaging
    {
        public String codigoCliente { get; set; }
        public String codigoVendedor { get; set; }
        public DateTime? fecha { get; set; }

        public String sku { get; set; }
        public String moneda { get; set; }
        public Decimal precio { get; set; }
        public String sede { get; set; }
     
    }
}

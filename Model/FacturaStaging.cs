using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class FacturaStaging
    {
        public Int32 numero { get; set; }
        public String sede { get; set; }
        public String tipoDocumento { get; set; }
        public Int32 numeroDocumento { get; set; }
        public DateTime fecha { get; set; }
        public String codigoCliente { get; set; }
        public String ruc { get; set; }
        public String razonSocial { get; set; }
        public decimal valorVenta { get; set; }
        public decimal igv { get; set; }
        public decimal total { get; set; }
        public String observacion { get; set; }
        public DateTime? fechaVencimiento { get; set; }
    }
}

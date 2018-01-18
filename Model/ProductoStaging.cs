using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ProductoStaging
    {
        public String codigo { get; set; }
        public String familia { get; set; }
        public String clase { get; set; }
        public String Marca { get; set; }
        public String modelo { get; set; }
        public String unidad { get; set; }
        public String descripcion { get; set; }
        public DateTime fechaIngreso { get; set; }
        public String st { get; set; }
        public String fechaFin { get; set; }
        public String codigoProveedor { get; set; }
        public String unidadProveedor { get; set; }
        public String equivalenciaProveedor { get; set; }
        public String unidadAlternativa { get; set; }
        public int equivalencia { get; set; }
        public Decimal costo { get; set; }
        public Decimal precioLima { get; set; }
        public Decimal precioProvincias { get; set; }
        public String proveedor { get; set; }
    }
}

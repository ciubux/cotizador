using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ProductoPresentacion : Auditoria
    {
        public ProductoPresentacion()
        {
            this.Equivalencia = 1;
        }

        public int IdProductoPresentacion { get; set; }
        public String Presentacion { get; set; }
        public Decimal PrecioLimaSinIGV { get; set; }
        public Decimal PrecioProvinciasSinIGV { get; set; }
        public Decimal CostoSinIGV { get; set; }
        public Decimal PrecioSinIGV { get; set; }
        public Decimal PrecioProvinciasOriginalSinIGV { get; set; }
        public Decimal CostoOriginalSinIGV { get; set; }
        public Decimal PrecioOriginalSinIGV { get; set; }
        public Decimal PrecioLimaOriginalSinIGV { get; set; }
        public Decimal Equivalencia { get; set; }
        public String UnidadInternacional { get; set; }
        public int Cantidad { get; set; }
        public int Stock { get; set; }
    }
}

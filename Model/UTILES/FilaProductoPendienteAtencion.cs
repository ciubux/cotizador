using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.UTILES
{
    public class FilaProductoPendienteAtencion
    {
        public string sede { get; set; }

        public string sku { get; set; }
        public string nombreProducto { get; set; }

        public Guid idProducto { get; set; }
        public Guid idSede { get; set; }

        public string familia { get; set; }
        public string proveedor { get; set; }
        public string unidad { get; set; }
        public string unidadAlternativa { get; set; }
        public string unidadProveedor { get; set; }
        public string unidadConteo { get; set; }

        public decimal cpProveedor { get {
                decimal cant = 0;

                if (cpConteo > 0 && eqProveedorConteo > 0)
                {
                    cant = ((decimal)cpConteo) / ((decimal)eqProveedorConteo);
                }

                return Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cant));
            } 
        }
        public decimal cpMp {
            get
            {
                decimal cant = 0;

                if (cpConteo > 0 && eqMpConteo > 0)
                {
                    cant = ((decimal)cpConteo) / ((decimal)eqMpConteo);
                }

                return Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cant));
            }
        }
        public decimal cpAlternativa {
            get
            {
                decimal cant = 0;

                if (cpConteo > 0 && eqAlternativaConteo > 0)
                {
                    cant = ((decimal)cpConteo) / ((decimal)eqAlternativaConteo);
                }

                return Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cant));
            }
        }

        public int cpConteo{ get; set; }

        public int eqMpConteo{ get; set; }
        public int eqProveedorConteo { get; set; }
        public int eqAlternativaConteo { get; set; }
    }
}

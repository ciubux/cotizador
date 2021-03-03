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

        public bool precioVigente { get {
                DateTime hoy = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                if (fechaFinVigencia != null && fechaFinVigencia.Value.CompareTo(hoy) >= 0)
                {
                    return true;
                }

                return false;
            }
        }
        public String unidad { get; set; }

        public String skuCliente { get; set; }

        public Decimal precioNeto { get; set; }
        public Decimal flete { get; set; }
        public Decimal precioUnitario { get; set; }

        public Decimal precioNetoAlternativo { get {
                if (equivalencia != 0 )
                {
                    return precioNeto / equivalencia;
                }
                else
                {
                    return 0;
                }
            }
        }

        public Boolean esUnidadAlternativa { get; set; }

        public String numeroCotizacion { get; set; }

        public String tipoCotizacion { get; set; }
        public Decimal equivalencia { get; set; }

        public Cliente cliente { get; set; }

        public Producto producto { get; set; }

        public bool estadoCanasta { get; set; }

        public GrupoCliente grupoCliente { get; set; }
        /*    public String nombreVista
            {
                get { return codigo + "(" + nombre + ")"; }
            }*/
    }
}
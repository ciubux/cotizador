using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class MovimientoKardex
    {
        public MovimientoKardex()
        {
        }

        public TipoMovimiento tipoMovimiento { get; set; }

        public TipoOperacion tipoOperacion { get; set; }

        public TipoDocumento tipoDocumento { get; set; }

        public string serieDocumento { get; set; }
        public string numeroDocumento { get; set; }

        
        public int cantidad { get; set; }

        public int costoUnitario { get; set; }

        public int costoTotal { get; set; }
    }

    public enum TipoMovimiento
    {
        Ingreso = 1,
        Salida = 2
    }

    public enum TipoOperacion 
    {
        Inicial = 16,
        Venta = 1,
        Compra = 2
    }

    public enum TipoDocumento
    {
        Inicial = 0,
        Venta = 1,
        Compra = 2
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.UTILES
{
    public class MovimientoKardexCabecera
    {
        public Producto producto { get; set; }
        public Ciudad ciudad { get; set; }
        public string unidad { get; set; }

        public string unidadConteo { get; set; }

        public List<MovimientoKardexDetalle> movimientos { get; set; }
    }
}

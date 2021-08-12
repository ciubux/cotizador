using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class TipoCambioSunat : Auditoria
    {
        public int idTipoCambioSunat { get; set; }

        public DateTime fecha { get; set; }
        public Decimal valorSunatCompra { get; set; }
        public Decimal valorSunatVenta { get; set; }

        public string codigoMoneda { get; set; }

    }
}

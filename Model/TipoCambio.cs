using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class TipoCambio : Auditoria
    {
        public int idTipoCambio { get; set; }

        public DateTime fecha { get; set; }
        public Decimal valorSunatCompra { get; set; }
        public Decimal valorSunatVenta { get; set; }

        public Decimal valor { get; set; }

        public string codigoMoneda { get; set; }

    }
}

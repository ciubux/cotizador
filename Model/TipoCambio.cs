using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class TipoCambio : Auditoria
    {
        public Guid idTipoCambio { get; set; }

        public Decimal monto { get; set; }
    
    }
}

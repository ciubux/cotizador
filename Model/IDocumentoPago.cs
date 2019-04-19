using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public interface IDocumentoPago
    {
        DateTime? fechaEmision { get; set; }
        DateTime horaEmision { get; set; }
        DateTime fechaVencimiento { get; set; }
        String serie { get; set; }
        String numero { get; set; }
    }
}

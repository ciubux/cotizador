using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.UTILES
{
    public class LogRegistroCampoDato
    {
        public string valor { get; set; }
        public string fechaModificacion { get; set; }
        public Guid idRegistroCambio { get; set; }
        public bool editable { get; set; }
    }
}

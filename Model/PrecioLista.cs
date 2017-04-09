using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class PrecioLista : Auditoria
    {
        public Guid idPrecioLista { get; set; }

        public String codigo { get; set;  }

        public String nombre { get; set; }

        public Decimal precio { get; set; }

        public String nombreVista
        {
            get { return codigo + "(" + nombre + ")"; }
        }
    }
}
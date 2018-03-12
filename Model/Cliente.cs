using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class Cliente : Auditoria
    {
        public Guid idCliente { get; set; }

        public String codigo { get; set;  }

        public int codigoAlterno { get; set; }

        public String razonSocial { get; set; }

        public String nombreComercial { get; set; }

        public String ruc { get; set; }

        public String contacto1 { get; set; }

        public String contacto2 { get; set; }

        public Ciudad ciudad { get; set; }

        public override string ToString()
        {
            return "R. Social: " + this.razonSocial + "  -  N. Comercial: "
                + this.nombreComercial + " - Cod: " + this.codigo;// + " **** RUC: " + this.ruc;
        }
    }
}
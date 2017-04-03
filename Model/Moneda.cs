using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class Moneda : Auditoria
    {
        public Guid idMoneda { get; set; }

        public String simbolo { get; set;  }

        public String nombre { get; set; }

        
    }
}
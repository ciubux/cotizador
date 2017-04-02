using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class Ciudad : Auditoria
    {
        public Guid idCiudad { get; set; }

        public String nombre { get; set;  }

        public int orden { get; set; }
    }
}
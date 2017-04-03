using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class Familia : Auditoria
    {
        public Guid idFamilia { get; set; }
        public Guid idCategoria { get; set; }
        public String codigo { get; set; }
        public String nombre { get; set; }


    }
}
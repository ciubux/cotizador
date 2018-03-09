using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class Grupo : Auditoria
    {
        public Guid idGrupo { get; set; }
        public String codigo { get; set;  }
        public String nombre { get; set; }
        public String contacto { get; set; }
        public override string ToString()
        {
            return "Cod: " + this.codigo + "  -  Grupo: " + this.nombre;
        }
    }
}
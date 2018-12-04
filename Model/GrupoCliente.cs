using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class GrupoCliente : Auditoria
    {
        public int idGrupoCliente { get; set; }
        public String codigo { get; set;  }
        public String nombre { get; set; }
        public override string ToString()
        {
            return "Grupo: " + this.nombre + " - Cod: " + this.codigo;
        }
    }
}
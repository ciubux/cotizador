using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class Rubro : Auditoria
    {
        public int idRubro { get; set; }

        [Display(Name = "Código:")]
        public String codigo { get; set;  }
        [Display(Name = "Nombre:")]
        public String nombre { get; set; }

        public override string ToString()
        {
            return "Rubro: " + this.nombre + " - Cod: " + this.codigo;
        }
    }
}
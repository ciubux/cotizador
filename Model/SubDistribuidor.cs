using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class SubDistribuidor : Auditoria
    {
        public int idSubDistribuidor { get; set; }

        [Display(Name = "Código:")]
        public String codigo { get; set;  }
        [Display(Name = "Nombre:")]
        public String nombre { get; set; }

        public Usuario usuario { get; set; }
        public override string ToString()
        {
            return "SubDistribuidor: " + this.nombre + " - Cod: " + this.codigo;
        }
    }
}
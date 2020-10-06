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

        public Rubro padre { get; set; }

        public String nombreCompleto{ get {
                return (this.padre != null ? this.padre.nombre + " - " : "") + this.nombre;
            }
        }


        public override string ToString()
        {
            return "Rubro: " + this.nombre + " - Cod: " + this.codigo;
        }
    }
}
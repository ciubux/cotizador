using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class Origen : Auditoria
    {
        public int idOrigen { get; set; }
        [Display(Name = "Código:")]
        public String codigo { get; set; }
        [Display(Name = "Nombre:")]
        public String nombre { get; set; }

        public Usuario usuario { get; set; }
        public override string ToString()
        {
            return "Origen: " + this.nombre + " - Cod: " + this.codigo;
        }
    }
}
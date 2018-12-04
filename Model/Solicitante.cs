using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class Solicitante : Auditoria
    {
        public Guid idSolicitante { get; set; }

        [Display(Name = "Nombre:")]
        public String nombre { get; set; }

        [Display(Name = "Teléfono:")]
        public String telefono { get; set; }

        [Display(Name = "Correo:")]
        public String correo { get; set; }

        //public Ubigeo ubigeo { get; set; }
    }
}
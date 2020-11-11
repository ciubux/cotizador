using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class ClienteContactoTipo : Auditoria
    {
        public Guid idClienteContactoTipo { get; set; }

        [Display(Name = "Nombre:")]
        public String nombre { get; set; }

        [Display(Name = "Descripción:")]
        public String descripcion { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class ClienteContacto : Auditoria
    {
        public Guid idClienteContacto { get; set; }

        [Display(Name = "Nombre:")]
        public String nombre { get; set; }

        [Display(Name = "Teléfono:")]
        public String telefono { get; set; }

        [Display(Name = "Correo:")]
        public String correo { get; set; }

        [Display(Name = "Cargo:")]
        public String cargo { get; set; }


        public Guid idCliente { get; set; }
        public int idClienteSunat { get; set; }

        public int esPrincipal { get; set; }
        public int aplicaRuc { get; set; }

        public Guid idClienteVista { get; set; }
    }
}
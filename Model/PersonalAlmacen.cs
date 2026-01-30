using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class PersonalAlmacen : Auditoria
    {
        public int idPersonalAlmacen { get; set; }

        [Display(Name = "Nombres:")]
        public string nombres { get; set; }

        [Display(Name = "Apellido Paterno:")]
        public string apellidoPaterno { get; set; }

        [Display(Name = "Apellido Materno:")]
        public string apellidoMaterno { get; set; }

        [Display(Name = "Nro. Documento:")]
        public string nroDocumento { get; set; }

        [Display(Name = "Brevete:")]
        public string brevete { get; set; }

        [Display(Name = "Tipo:")]
        public string tipo { get; set; }

        public Ciudad sedePrincipal { get; set; }

        public override string ToString()
        {
            return $"{this.apellidoPaterno} {this.apellidoMaterno}, {this.nombres}".Trim();
        }
    }
}
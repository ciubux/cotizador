using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class Empresa : Auditoria
    {
        public int idEmpresa { get; set; }

        [Display(Name = "Código:")]
        public String codigo { get; set;  }
        [Display(Name = "Nombre:")]
        public String nombre { get; set; }


        [Display(Name = "Razon Social:")]
        public String razonSocial { get; set; }

        [Display(Name = "Url Web:")]
        public String urlWeb { get; set; }

        public bool atencionTerciarizada { get; set; }

        public override string ToString()
        {
            return this.codigo + " - " + this.nombre;
        }
    }
}
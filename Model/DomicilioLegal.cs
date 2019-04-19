using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class DomicilioLegal : Auditoria
    {
        public int idDomicilioLegal { get; set; }

        [Display(Name = "Código Anexo:")]
        public String codigo { get; set; }

        [Display(Name = "Tipo Establecimiento:")]
        public String tipoEstablecimiento { get; set; }

        [Display(Name = "Dirección:")]
        public String direccion { get; set; }

        [Display(Name = "Ubigeo:")]
        public Ubigeo ubigeo { get; set; }

        [Display(Name = "Es establecimiento anexos:")]
        public bool esEstablecimientoAnexo { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class SerieDocumentoElectronico
    {
     //   public Guid idSerieDocumentoElectronico { get; set; }        
        [Display(Name = "Serie:")]
        public String serie { get; set; }


        public int serieFormatInt { get { return int.Parse(serie); } }

        [Display(Name = "Número:")]
        public int siguienteNumeroBoleta { get; set; }

        [Display(Name = "Número:")]
        public int siguienteNumeroFactura { get; set; }

        [Display(Name = "Número:")]
        public int siguienteNumeroNotaCredito { get; set; }

        [Display(Name = "Número:")]
        public int siguienteNumeroNotaDebito { get; set; }

        [Display(Name = "Número:")]
        public int siguienteNumeroGuiaRemision { get; set; }

        [Display(Name = "Número:")]
        public int siguienteNumeroNotaIngreso { get; set; }
        public Ciudad sedeMP { get; set; }
        public bool esPrincipal { get; set; }        
    }
}
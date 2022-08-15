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


        public int serieFormatInt { get { 
                switch(serie.Substring(0, 1))
                {
                    case "D": return 999001;  break;
                    case "T": return 999002; break;
                    default: return int.Parse(serie);
                }

                //return !serie.Substring(0,1).Equals("D") && !serie.Substring(0, 1).Equals("T") ? int.Parse(serie) : 0; 
            } }

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

        [Display(Name = "Número:")]
        public int siguienteNumeroNotaCreditoBoleta { get; set; }
        [Display(Name = "Número:")]
        public int siguienteNumeroNotaDebitoBoleta { get; set; }
        public Ciudad sedeMP { get; set; }
        public bool esPrincipal { get; set; }        
    }
}
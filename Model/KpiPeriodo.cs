using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class KpiPeriodo : Auditoria
    {
        public KpiPeriodo() 
        {
            
        }

        public Guid idKpiPeriodo { get; set; }

        [Display(Name = "Periodo:")]
        public String periodo { get; set; }

        [Display(Name = "Desde:")]
        public DateTime desde { get; set; }

        [Display(Name = "Hasta:")]
        public DateTime hasta { get; set; }


    }
}
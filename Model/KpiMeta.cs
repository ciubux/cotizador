using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class KpiMeta : Auditoria
    {
        public KpiMeta() 
        {
            
        }

        public Kpi kpi { get; set; }

        public KpiPeriodo kpiPeriodo { get; set; }

        public Usuario usuarioMeta { get; set; }

        public Guid idKpiMeta { get; set; }


        [Display(Name = "Valor:")]
        public Decimal valor { get; set; }

        [Display(Name = "Resultado:")]
        public Decimal resultado { get; set; }

        [Display(Name = "Resultado %:")]
        public Decimal resultadoP { get; set; }

    }
}
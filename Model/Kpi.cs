using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class Kpi : Auditoria
    {
        public Kpi() 
        {
            
        }

        public Guid idKpi { get; set; }

        [Display(Name = "KPI:")]
        public String kpi { get; set; }

        [Display(Name = "Objeto:")]
        public String objeto { get; set; }

        [Display(Name = "Campo Acumulativo:")]
        public String campoAcumulativo { get; set; }

        [Display(Name = "Campo Usuario:")]
        public String campoUsuario{ get; set; }

        [Display(Name = "Campo Fecha:")]
        public String campoFecha { get; set; }

        [Display(Name = "Query Dato Usuario:")]
        public String queryDatoUsuario { get; set; }


    }
}
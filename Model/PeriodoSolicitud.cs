using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class PeriodoSolicitud : Auditoria
    {
        public Guid idPeriodoSolicitud { get; set; }

        [Display(Name = "Nombre:")]
        public String nombre { get; set; }

        [Display(Name = "Fecha Inicio:")]
        public DateTime fechaInicio { get; set; }

        [Display(Name = "Fecha Fin:")]
        public DateTime fechaFin { get; set; }

        public override string ToString()
        {
            return this.nombre;
        }
    }
}
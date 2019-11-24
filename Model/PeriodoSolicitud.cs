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

        public String fechaInicioFormato
        {
            get { return this.fechaInicio.ToString(Constantes.formatoFecha); }
        }

        public String fechaFinFormato
        {
            get { return this.fechaFin.ToString(Constantes.formatoFecha); }
        }

        public String nombreEstado
        {
            get {
                String nEstado = "";
                switch (this.Estado)
                {
                    case 0: nEstado = "Eliminado"; break;
                    case 1: nEstado = "Inactivo"; break;
                    case 2: nEstado = "Activo"; break;
                    case 10: nEstado = "Cerrado"; break;
                }
                return nEstado;
            }
        }

        public List<DocumentoDetalle> canasta { get; set; }

    }
}
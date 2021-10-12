using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Promocion : Auditoria
    {
        public Promocion()
        {

        }
        public Guid idPromocion { get; set; }

        [Display(Name = "Código:")]
        public String codigo { get; set; }

        [Display(Name = "Fecha Inicio Vigencia:")]
        public DateTime fechaInicio{ get; set; }

        [Display(Name = "Fecha Fin Vigencia:")]
        public DateTime fechaFin { get; set; }

        [Display(Name = "Título:")]
        public String titulo { get; set; }

        [Display(Name = "Descripción:")]
        public String descripcion { get; set; }

        [Display(Name = "Descripción Presentación:")]
        public String descripcionPresentacion { get; set; }

        public string fechaInicioDesc
        {
            get { return fechaInicio.ToString("dd/MM/yyyy"); }
        }

        public string fechaFinDesc
        {
            get { return fechaFin.ToString("dd/MM/yyyy"); }
        }

        public String textoDescripcion
        {
            get { return this.descripcion + "\n" + "Válido desde " + this.fechaInicioDesc + " hasta " + this.fechaFinDesc + " ó agotar stock."; }
        }
        public String textoPresentacion
        {
            get { return "APLICA PROMOCIÓN: " + this.titulo + "\n" + this.descripcion + "\n" + "Promoción válida desde " + this.fechaInicioDesc + " hasta " + this.fechaFinDesc + " ó agotar stock."; }
        }
    }
}


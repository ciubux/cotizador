using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class PrecioEspecialDetalle : Auditoria
    {
        public PrecioEspecialDetalle() 
        {
            
        }

        public Guid idPrecioEspecialDetalle { get; set; }

        [Display(Name = "Producto:")]
        public Producto producto { get; set; }

        [Display(Name = "Moneda:")]
        public Moneda moneda { get; set; }

        [Display(Name = "Unidad Precio:")]
        public ProductoPresentacion unidadPrecio { get; set; }

        [Display(Name = "Unidad Costo:")]
        public ProductoPresentacion unidadCosto { get; set; }

        [Display(Name = "Observaciones:")]
        public String observaciones { get; set; }

        [Display(Name = "Fecha Inicio:")]
        public DateTime fechaInicio { get; set; }

        [Display(Name = "Fecha Fin:")]
        public DateTime fechaFin { get; set; }

        public List<string> dataRelacionada { get; set; }

        public string FechaInicioDesc
        {
            get { return fechaInicio.ToString("dd/MM/yyyy"); }
        }

        public string fechaFinDesc
        {
            get { return fechaFin.ToString("dd/MM/yyyy"); }
        }

    }
}
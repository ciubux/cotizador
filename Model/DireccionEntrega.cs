using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class DireccionEntrega : Auditoria
    {
        public Guid idDireccionEntrega { get; set; }
        [Display(Name = "Dirección de Entrega:")]
        public String descripcion { get; set; }
        [Display(Name = "Contacto de Entrega:")]
        public String contacto { get; set; }
        [Display(Name = "Telefono Contacto de Entrega:")]
        public String telefono { get; set; }
        public Ubigeo ubigeo { get; set; }
        [Display(Name = "Código Cliente Centro Costos:")]
        public String codigoCliente { get; set; }
        [Display(Name = "Código MP Centro Costos:")]
        public String codigoMP { get; set; }
        [Display(Name = "Nombre Centro Costos:")]
        public String nombre { get; set; }
        [Display(Name = "Observaciones:")]
        public String observaciones { get; set; }
    }
}
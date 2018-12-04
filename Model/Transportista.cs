using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Transportista
    {
        public Guid idTransportista { get; set; }

        public Guid idCiudad { get; set; }

        [Display(Name = "Nombre:")]
        public String descripcion { get; set; }

        [Display(Name = "RUC:")]
        public String ruc { get; set; }

        [Display(Name = "Dirección:")]
        public String direccion { get; set; }

        [Display(Name = "Teléfono:")]
        public String telefono { get; set; }

        [Display(Name = "Observaciones:")]
        public String observaciones { get; set; }

        [Display(Name = "Brevete:")]
        public String brevete { get; set; }

    }
}

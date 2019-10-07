using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Vendedor
    {
        public int idVendedor { get; set; }

        [Display(Name = "Codigo:")]
        public String codigo { get; set; }
        [Display(Name = "Nombre:")]
        public String descripcion { get; set; }
        public Usuario usuario { get; set; }
        public Boolean esResponsableComercial { get; set; }
        public Boolean esAsistenteServicioCliente { get; set; }
        public Boolean esResponsablePortafolio { get; set; }
        public Boolean esSupervisorComercial { get; set; }

        public String codigoDescripcion { get { return this.codigo + " - " + this.descripcion; } }

        /*-----------------------*/

        public Vendedor supervisor { get; set; }

        public Guid idUsuarioVendedor { get; set; }

        [Display(Name = "Sede MP:")]
        public Ciudad ciudad { get; set; }

        [Display(Name = "Estado:")]
        public int estado { get; set; }

        [Display(Name = "Contacto:")]
        public string contacto { get; set; }

        [Display(Name = "Contraseña:")]
        public string pass { get; set; }

        [Display(Name = "Cargo:")]
        public string cargo { get; set; }

        [Display(Name = "Maximo Descuento:")]
        public Decimal maxdesapro { get; set; }

        [Display(Name = "Ciudad:")]
        public Guid idCiudad { get; set; }

        [Display(Name = "Email:")]
        public string email { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Vendedor
    {
        public int idVendedor { get; set; }
        public String codigo { get; set; }
        public String descripcion { get; set; }
        public Usuario usuario { get; set; }
        public Boolean esResponsableComercial { get; set; }
        public Boolean esAsistenteServicioCliente { get; set; }
        public Boolean esResponsablePortafolio { get; set; }
        public Boolean esSupervisorComercial { get; set; }

        public String codigoDescripcion { get { return this.codigo + " - " + this.descripcion; } }
    }
}

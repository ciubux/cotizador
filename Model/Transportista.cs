using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Transportista
    {
        public Guid idTransportista { get; set; }

        public Guid idCiudad { get; set; }

        public String descripcion { get; set; }

        public String ruc { get; set; }

        public String direccion { get; set; }

        public String telefono { get; set; }

        public String observaciones { get; set; }

        public String brevete { get; set; }

    }
}

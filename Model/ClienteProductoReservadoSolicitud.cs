using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ClienteProductoReservadoSolicitud : Auditoria
    {
        public Guid idClienteProductoReservadoSolicitud { get; set; }

        public int estadoSolicitud { get; set; }

        public int cantidadSolicitada { get; set; }

        public int cantidadAprobada { get; set; }
        public DateTime fechaAprobacion { get; set; }

        public Usuario usuarioAprobacion { get; set; }

        public ClienteProductoReservado clienteProductoReservado { get; set; }
    }
}

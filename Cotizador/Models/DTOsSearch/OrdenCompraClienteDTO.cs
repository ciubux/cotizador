using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cotizador.Models.DTOsSearch
{
    public class OrdenCompraClienteDTO
    {
        public Boolean stockConfirmado { get; set; }
        public String observaciones { get; set; }

        public Guid idOrdenCompraCliente { get; set; }

        public Int64 numeroOrdenCompraCliente { get; set; }

        public String cliente_ruc { get; set; }

        public String cliente_razonSocial { get; set; }
        public String numeroReferenciaCliente { get; set; }

        public String usuario_nombre { get; set; }

        public String fechaHoraRegistro { get; set; }

        public Decimal montoTotal { get; set; }
        
    }
}
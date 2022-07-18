using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cotizador.Models.DTOs
{

    public class PedidoAlmacenDTO
    {
        public Guid idPedido { get; set; }

        public String numeroPedidoString { get; set; }
        public String ciudad_nombre { get; set; }
        public String cliente_codigo { get; set; }
        public String cliente_razonSocial { get; set; }
        public String cliente_nombreComercial { get; set; }
        public String cliente_tipoDocumento { get; set; }
        public String numeroReferenciaCliente { get; set; }
        public String usuario_nombre { get; set; }
        public String fechaHoraRegistro { get; set; }
        public String rangoFechasEntrega { get; set; }
        public Decimal montoTotal { get; set; }
        public String ubigeoEntrega_Distrito { get; set; }
        public String seguimientoPedido_estadoString { get; set; }
        public String seguimientoCrediticioPedido_estadoString { get; set; }
        public long numeroPedido { get; set; }
        public String observaciones { get; set; }
        public DateTime? fechaProgramacion { get; set; }
        public long numero { get; set; }
        public int stockConfirmado { get; set; }

        


    }
}

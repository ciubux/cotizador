using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cotizador.Models.DTOsSearch
{
    public class PedidoDTO
    {

        public DateTime? fechaProgramacion { get; set; }

        public int stockConfirmado { get; set; }
        public String observaciones { get; set; }

        public Guid idPedido { get; set; }

        public Int64 numeroPedido { get; set; }
        public String numeroPedidoNumeroGrupoString { get; set; }

        public String ciudad_nombre { get; set; }

        public String cliente_codigo { get; set; }

        public String cliente_razonSocial { get; set; }
        public String numeroReferenciaCliente { get; set; }

        public String usuario_nombre { get; set; }

        public String fechaHoraRegistro { get; set; }

        public String rangoFechasEntrega { get; set; }

        public String rangoHoraEntrega { get; set; }

        public Decimal montoTotal { get; set; }
        
        public String ubigeoEntrega_distrito { get; set; }

        public String seguimientoPedido_estadoString { get; set; }

        public String seguimientoCrediticioPedido_estadoString { get; set; }

        public String grupoCliente_nombre { get; set; }

        public String clienteTercero_nombre { get; set; }
        public String cliente_nombreComercial { get; set; }
        public String cliente_tipoDocumento { get; set; }
        public int truncado { get; set; }

        public Int64 numeroPedidoRelacionado { get; set; }
        public String codigoEmpresaPedidorelacionado { get; set; }

    }
}
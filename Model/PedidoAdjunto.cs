using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class PedidoAdjunto 
    {
        public Guid idPedidoAdjunto { get; set; }
        public Guid idPedido { get; set; }
        public Guid idCliente { get; set; }
        public Usuario usuario { get; set; }
        public Byte[] adjunto { get; set; }
        public String nombre { get; set; }

    }
}

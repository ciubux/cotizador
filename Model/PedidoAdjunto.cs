using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class PedidoAdjunto : ArchivoAdjunto
    {
        //public Guid idPedidoAdjunto { get; set; }
        public Guid idPedido { get; set; }
        public Guid idCliente { get; set; }
    }
}

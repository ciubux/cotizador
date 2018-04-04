using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class PedidoDetalle : DocumentoDetalle
    {
        public Guid idPedidoDetalle { get; set; }
        public Guid idPedido { get; set; }   
      
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cotizador.Models.DTOs
{
    public class NotaIngresoDTO
    {
        public Guid idMovimientoAlmacen { get; set; }
        public String serieNumeroNotaIngreso { get; set; }
        public String pedido_numeroPedidoString  { get; set; }
        public String motivoTrasladoString { get; set; }
        public String usuario_nombre { get; set; }
        public DateTime? fechaEmision { get; set; }
        public DateTime? fechaTraslado { get; set; }
        public String pedido_cliente_razonSocial { get; set; }
        public String pedido_cliente_ruc { get; set; }
        public String ciudadDestino_nombre { get; set; }
        public String tipoExtornoToString { get; set; }
        public String estadoDescripcion { get; set; }
        public long numeroDocumento { get; set; }
        public bool estaAnulado { get; set; }
        public bool estaFacturado { get; set; }
        public bool estaNoEntregado { get; set; }
      //  public String serieNumeroGuia { get; set; }




    }

  
                      
                     
                       
}
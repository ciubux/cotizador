using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cotizador.Models.DTOs
{
    public class CotizacionDTO
    {
        
        public Guid idCotizacion { get; set; }
        public long codigo { get; set; }
        public String usuario_nombre { get; set; }
        public DateTime? fecha { get; set; }
        public String cliente_razonSocial { get; set; }
        public String cliente_ruc { get; set; }
        public String ciudad_nombre { get; set; }       
        public Decimal montoSubTotal { get; set; }
        public Decimal montoIGV { get; set; }
        public Decimal montoTotal { get; set; }        
        public Decimal maximoPorcentajeDescuentoPermitido { get; set; }
        public Decimal minimoMargen { get; set; }
        public String seguimientoCotizacion_estadoString { get; set; }
        public String seguimientoCotizacion_usuario_nombre { get; set; }
        public String seguimientoCotizacion_observacion { get; set; }        
        public String grupo_nombre { get; set; }
        public Guid cliente_idCliente { get; set; }

        public String cliente_nombreComercial { get; set; }
        public String cliente_tipoDocumento { get; set; }





    }
    
}
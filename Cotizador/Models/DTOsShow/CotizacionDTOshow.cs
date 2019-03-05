using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cotizador.Models.DTOshow
{
    public class CotizacionDTOshow
    {
        public String textoCondicionesPago { get; set; }
        public Guid idCotizacion { get; set; }
        public Guid cliente_idCliente { get; set; }
        public long codigo { get; set; }
        public String ciudad_nombre { get; set; }
        public String cliente_razonSocial { get; set; }       
        public String contacto { get; set; }
        public DateTime? fecha { get; set; }       
        public DateTime? fechaLimiteValidezOferta { get; set; }
        public DateTime? fechaInicioVigenciaPrecios { get; set; }
        public DateTime? fechaFinVigenciaPrecios { get; set; }
        public String seguimientoCotizacion_estadoString { get; set; }
        public String seguimientoCotizacion_usuario_nombre { get; set; }

        public Guid seguimientoCotizacion_usuario_idUsuario { get; set; }
       
        public String seguimientoCotizacion_observacion { get; set; }
        public String considerarCantidades { get; set; }
        public String observaciones { get; set; }
        public bool aplicaSedes { get; set; }
        public String cliente_sedeListWebString { get; set; }
        public Decimal montoSubTotal { get; set; }
        public Decimal montoIGV { get; set; }
        public Decimal montoTotal { get; set; }
        public Decimal minimoMargen { get; set; }
        public List<Model.CotizacionDetalle> cotizacionDetalleList { get; set; }
       
        
        public SeguimientoCotizacion.estadosSeguimientoCotizacion seguimientoCotizacion_estado { get; set; }
        public Decimal maximoPorcentajeDescuentoPermitido { get; set; }
        public String grupo_codigoNombre { get; set; }
        public String cliente_codigoRazonSocial { get; set; }
        public int tipoCotizacion { get; set; }


    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cotizador.Models.DTOs
{
    public class FacturaDTO
    {

        public Guid idDocumentoVenta { get; set; }
        public String serieNumero { get; set; }
        public String pedido_numeroPedidoString { get; set; }
       
        public String usuario_nombre { get; set; }
        public DateTime? fechaEmision { get; set; }
        public String cliente_razonSocial { get; set; }
        public String cliente_ruc { get; set; }
        public String ciudad_nombre { get; set; }
        public Decimal total { get; set; }    
       
        public String estadoDocumentoSunatString { get; set; }
        public String comentarioSolicitudAnulacion { get; set; }
        public bool usuario_apruebaAnulaciones { get; set; }
        public bool usuario_creaNotasCredito { get; set; }
        
       
        public bool solicitadoAnulacion { get; set; }
        public  Model.DocumentoVenta.EstadosDocumentoSunat estadoDocumentoSunat { get; set; }
        public Model.NotaIngreso notaIngreso { get; set; }
        public Model.GuiaRemision guiaRemision { get; set; }

        /*
        public String guiaRemision_serieNumeroGuia { get; set; }
        public String notaIngreso_serieNumeroNotaIngreso { get; set; }
        */










    }
}
             


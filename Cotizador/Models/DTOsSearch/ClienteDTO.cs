using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cotizador.Models.DTOs
{
    public class ClienteDTO
    {
        public Guid idCliente { get; set; }
               
        public String codigo { get; set; }

        public String razonSocialSunat { get; set; }

        public String nombreComercial { get; set; }
        public String tipoDocumentoIdentidadToString { get; set; }

        public String ruc { get; set; }

        public String ciudad_nombre { get; set; }

        public String grupoCliente_nombre { get; set; }
        public String responsableComercial_descripcion { get; set; }
        public String supervisorComercial_descripcion { get; set; }
        public String asistenteServicioCliente_descripcion { get; set; }

        
              public String tipoPagoFacturaToString { get; set; }

        public String plazoCredito { get; set; }

        public decimal creditoAprobado{ get; set; }
        public bool bloqueado { get; set; }
        






    }
    
}
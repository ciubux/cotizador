using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cotizador.Models.DTOshow
{
    public class ClienteDTOshow
    {
        public String ciudad_nombre { get; set; }
        public String codigo { get; set; }
        public String ruc { get; set; }
        public String tipoDocumentoIdentidadToString { get; set; }        
        public String nombreComercial { get; set; }
        public String contacto1 { get; set; }
        public String correoEnvioFactura { get; set; }
        public bool bloqueado { get; set; }
        public String observaciones { get; set; }
        public String plazoCreditoSolicitadoToString { get; set; }
        public String tipoPagoFacturaToString { get; set; }
        public Decimal creditoSolicitado { get; set; }
        public Decimal creditoAprobado { get; set; }
        public Decimal sobreGiro { get; set; }
        public String observacionesCredito { get; set; }
        public String formaPagoFacturaToString { get; set; }
        public String razonSocialSunat { get; set; }
        public String nombreComercialSunat { get; set; }
        public String direccionDomicilioLegalSunat { get; set; }
        public String estadoContribuyente { get; set; }
        public String condicionContribuyente { get; set; }
           
        public String responsableComercial_descripcion { get; set; }
        public String supervisorComercial_descripcion { get; set; }
        
        public String asistenteServicioCliente_descripcion { get; set; }
        public String grupoCliente_nombre { get; set; }
        public bool perteneceCanalMultiregional { get; set; }
        public bool perteneceCanalLima { get; set; }
        public bool perteneceCanalProvincias { get; set; }
        public bool perteneceCanalPCP { get; set; }
        public bool esSubDistribuidor { get; set; }
        public bool negociacionMultiregional { get; set; }
        public bool sedePrincipal { get; set; }

        public String horaInicioPrimerTurnoEntregaFormat { get; set; }
        public String horaFinPrimerTurnoEntregaFormat { get; set; }
        public String horaInicioSegundoTurnoEntregaFormat { get; set; }
        public String horaFinSegundoTurnoEntregaFormat { get; set; }


       







    }
    
}
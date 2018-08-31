using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class Persona : Auditoria
    {
        [Display(Name = "Código MP:")]
        public String codigo { get; set; }
        public int codigoAlterno { get; set; }

        [Display(Name = "Razón Social:")]
        public String razonSocial { get; set; }


        [Display(Name = "Nombre Comercial / Nombres y Apellidos:")]
        public String nombreComercial { get; set; }

        [Display(Name = "RUC:")]
        public String ruc { get; set; }

        public String contacto1 { get; set; }

        public String contacto2 { get; set; }

        [Display(Name = "Sede MP:")]
        public Ciudad ciudad { get; set; }

        [Display(Name = "Domicilio Legal:")]
        public String domicilioLegal { get; set; }

        public List<DireccionEntrega> direccionEntregaList { get; set; }

        public List<Solicitante> solicitanteList { get; set; }

        [Display(Name = "Razón Social:")]
        public String razonSocialSunat { get; set; }

        [Display(Name = "Nombre Comercial:")]
        public String nombreComercialSunat { get; set; }

        [Display(Name = "Domicilio Legal:")]
        public String direccionDomicilioLegalSunat { get; set; }

        [Display(Name = "Estado Contribuyente:")]
        public String estadoContribuyente { get; set; }

        [Display(Name = "Condición Contribuyente:")]
        public String condicionContribuyente { get; set; }

        [Display(Name = "Correo Envío Factura:")]
        public String correoEnvioFactura { get; set; }


        public Ubigeo ubigeo { get; set; }

        [Display(Name = "Plazo de Crédito:")]
        public String plazoCredito { get; set; }

        [Display(Name = "Crédito Solicitado:")]
        public Decimal creditoSolicitado { get; set; }

        [Display(Name = "Crédito Aprobado:")]
        public Decimal creditoAprobado { get; set; }

        public override string ToString()
        {
            return "R. Social: " + this.razonSocial + "  -  N. Comercial: "
                + this.nombreComercial + " - Cod: " + this.codigo + " - RUC: " + this.ruc;
        }

        public string codigoRazonSocial
        {
            get { return this.codigo + " - " + this.razonSocial; }
        }

        [Display(Name = "Tipo de Pago Factura:")]
        public DocumentoVenta.TipoPago tipoPagoFactura { get; set; }

        [Display(Name = "Forma de Pago Factura:")]
        public DocumentoVenta.FormaPago formaPagoFactura { get; set; }

        public String tipoDocumento { get; set; }


    }
}
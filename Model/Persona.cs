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

        [Display(Name = "Contacto:")]
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

        [Display(Name = "Plazo de Crédito en días (Anterior al ZAS):")]
        public String plazoCredito { get; set; }

        [Display(Name = "Monto Crédito (Solicitado) S/:")]
        public Decimal creditoSolicitado { get; set; }

        [Display(Name = "Monto Crédito (Aprobado) S/:")]
        public Decimal creditoAprobado { get; set; }


        [Display(Name = "Sobre Giro S/:")]
        public Decimal sobreGiro { get; set; }


        public override string ToString()
        {
            return "R. Social: " + this.razonSocial + "  -  N. Comercial: "
                + this.nombreComercial + " - Cod: " + this.codigo + " - RUC: " + this.ruc;
        }

        public string codigoRazonSocial
        {
            get { return this.codigo + " - " + this.razonSocial; }
        }

        [Display(Name = "Plazo Crédito (Aprobado):")]
        public DocumentoVenta.TipoPago tipoPagoFactura { get; set; }


        [Display(Name = "Plazo Crédito (Solicitado):")]
        public DocumentoVenta.TipoPago tipoPagoSolicitado { get; set; }


        [Display(Name = "Sobre Plazo en días:")]
        public int sobrePlazo { get; set; }

     

        [Display(Name = "Forma de Pago Factura:")]
        public DocumentoVenta.FormaPago formaPagoFactura { get; set; }

        public String tipoDocumento { get; set; }

        [Display(Name = "Tipo Doc. Identidad:")]
        public DocumentoVenta.TiposDocumentoIdentidad tipoDocumentoIdentidad { get; set; }




    }
}
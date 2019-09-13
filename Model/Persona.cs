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

        [Display(Name = "Contacto Compras:")]
        public String contacto1 { get; set; }

        public String contacto2 { get; set; }

        [Display(Name = "Sede MP:")]
        public Ciudad ciudad { get; set; }

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

        [Display(Name = "E-mail Envío Factura:")]
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
            if (this.tipoDocumentoIdentidad != DocumentoVenta.TiposDocumentoIdentidad.RUC)
            {
                return "Nombres y Apellidos: " + this.razonSocial + " - COD: " + this.codigo + " - DOC: " + this.ruc;
            }
            else
            {
                return "R. Social: " + this.razonSocial + "  -  N. Comercial: "
                + this.nombreComercial + " - COD: " + this.codigo + " - RUC: " + this.ruc;
            }
        }

        public string ToStringRUC()
        {
            if (this.tipoDocumentoIdentidad == DocumentoVenta.TiposDocumentoIdentidad.RUC)
            {
                return "RUC: " + this.ruc + "  - R. Social: " + this.razonSocial;
            }

            return "";
        }


        public string codigoRazonSocial
        {
            get { return this.codigo + " - " + this.razonSocial; }
        }

        [Display(Name = "Plazo Crédito (Aprobado):")]
        public DocumentoVenta.TipoPago tipoPagoFactura { get; set; }


        public String tipoPagoFacturaToString
        {
            get
            {
                return EnumHelper<DocumentoVenta.TipoPago>.GetDisplayValue(this.tipoPagoFactura);
            }
        }

        [Display(Name = "Plazo Crédito (Solicitado):")]
        public DocumentoVenta.TipoPago plazoCreditoSolicitado { get; set; }

        public String plazoCreditoSolicitadoToString
        {
            get
            {
                return EnumHelper<DocumentoVenta.TipoPago>.GetDisplayValue(this.plazoCreditoSolicitado);
            }
        }


        [Display(Name = "Sobre Plazo en días:")]
        public int sobrePlazo { get; set; }

     

        [Display(Name = "Forma de Pago Factura:")]
        public DocumentoVenta.FormaPago formaPagoFactura { get; set; }

        [Display(Name = "Liberación Crediticia:")]
        public TipoLiberacionCrediticia tipoLiberacionCrediticia { get; set; }
        public enum TipoLiberacionCrediticia
        {
            [Display(Name = "Todos")]
            todos = -999,

            [Display(Name = "Requiere Liberación")]
            requiere = 0,
            [Display(Name = "Exonerado de Liberación")]
            exonerado = 1,
            [Display(Name = "Bloqueado")]
            bloqueado = -1,
        };
        public String tipoLiberacionCrediticiaString
        {
            get
            {
                return EnumHelper<TipoLiberacionCrediticia>.GetDisplayValue(this.tipoLiberacionCrediticia);
            }
        }


        public bool requiereLiberacionCrediticia { get { return this.tipoLiberacionCrediticia == TipoLiberacionCrediticia.requiere; } }
        public bool exoneradoLiberacionCrediticia { get { return this.tipoLiberacionCrediticia == TipoLiberacionCrediticia.exonerado; } }
        public bool bloqueadoLiberacionCrediticia { get { return this.tipoLiberacionCrediticia == TipoLiberacionCrediticia.bloqueado; } }

        public String formaPagoFacturaToString
        {
            get
            {
                return EnumHelper<DocumentoVenta.FormaPago>.GetDisplayValue(this.formaPagoFactura);
            }
        }

        public String tipoDocumento { get; set; }

        [Display(Name = "Tipo Doc. Identidad:")]
        public DocumentoVenta.TiposDocumentoIdentidad tipoDocumentoIdentidad { get; set; }

        public String tipoDocumentoIdentidadToString
        {
            get
            {
                return EnumHelper<DocumentoVenta.TiposDocumentoIdentidad>.GetDisplayValue(this.tipoDocumentoIdentidad);
            }
        }


        [Display(Name = "Vendedores Asignados:")]
        public Boolean vendedoresAsignados { get; set; }

        public Usuario usuario { get; set; }

        [Display(Name = "Observaciones:")]
        public String observaciones { get; set; }

        [Display(Name = "Telef. Contacto Compras:")]
        public String telefonoContacto1 { get; set; }

        [Display(Name = "E-mail Contacto Compras:")]
        public String emailContacto1 { get; set; }


        /*Domicilio Legal*/

        [Display(Name = "Domicilio Legal:")]
        public String domicilioLegal { get; set; }

        public List<DomicilioLegal> domicilioLegalList { get; set; }

        /*Dirección Entrega*/
        public List<DireccionEntrega> direccionEntregaList { get; set; }

    }
}
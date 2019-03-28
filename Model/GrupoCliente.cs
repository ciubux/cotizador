using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class GrupoCliente : Auditoria
    {

        public GrupoCliente()
        {
            this.plazoCreditoSolicitado = DocumentoVenta.TipoPago.NoAsignado;
            this.plazoCreditoAprobado = DocumentoVenta.TipoPago.NoAsignado;
            this.grupoClienteAdjuntoList = new List<GrupoClienteAdjunto>();
            this.miembros = new List<Cliente>();
        }

        public int idGrupoCliente { get; set; }

        [Display(Name = "Código:")]
        public String codigo { get; set;  }

        [Display(Name = "Nombre:")]
        public String nombre { get; set; }

        [Display(Name = "Contacto:")]
        public String contacto { get; set; }

        [Display(Name = "Telefono Contacto:")]
        public String telefonoContacto { get; set; }

        [Display(Name = "Observaciones:")]
        public String observaciones { get; set; }

        [Display(Name = "Email Contacto:")]
        public String emailContacto { get; set; }

        [Display(Name = "Sede MP (Principal):")]
        public Ciudad ciudad { get; set; }

        public override string ToString()
        {
            return "Grupo: " + this.nombre + " - Cod: " + this.codigo;
        }

        public string codigoNombre
        {
            get { return this.codigo + " - " + this.nombre; }
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

        [Display(Name = "Plazo Crédito (Aprobado):")]
        public DocumentoVenta.TipoPago plazoCreditoAprobado { get; set; }

        public String plazoCreditoAprobadoToString
        {
            get
            {
                return EnumHelper<DocumentoVenta.TipoPago>.GetDisplayValue(this.plazoCreditoAprobado);
            }
        }

        public Boolean existenCambiosCreditos { get; set; }

        public Boolean sinMontoCreditoAprobado { get; set; }

        public Boolean sinPlazoCreditoAprobado { get; set; }


        [Display(Name = "Monto Crédito (Solicitado) S/:")]
        public Decimal creditoSolicitado { get; set; }

        [Display(Name = "Monto Crédito (Aprobado) S/:")]
        public Decimal creditoAprobado { get; set; }

        [Display(Name = "Sobre Plazo en días:")]
        public int sobrePlazo { get; set; }

        [Display(Name = "Sobre Giro S/:")]
        public Decimal sobreGiro { get; set; }

        [Display(Name = "Observaciones Crédito:")]
        public String observacionesCredito { get; set; }

        public List<GrupoClienteAdjunto> grupoClienteAdjuntoList { get; set; }

        public List<Cliente> miembros { get; set; }
    }
}
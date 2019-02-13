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
        }

        public int idGrupoCliente { get; set; }
        public String codigo { get; set;  }
        public String nombre { get; set; }
        public String contacto { get; set; }
        public String telefonoContacto { get; set; }
        public String observaciones { get; set; }
        public String emailContacto { get; set; }
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

        [Display(Name = "Plazo Crédito (Aprobado):")]
        public DocumentoVenta.TipoPago plazoCreditoAprobado { get; set; }

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

        public String observacionesCredito { get; set; }

        public List<GrupoClienteAdjunto> grupoClienteAdjuntoList { get; set; }
    }
}
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
        }

        public int idGrupoCliente { get; set; }
        public String codigo { get; set;  }
        public String nombre { get; set; }
        public String contacto { get; set; }
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


    }
}
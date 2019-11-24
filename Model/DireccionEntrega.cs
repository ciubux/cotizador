using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class DireccionEntrega : Auditoria
    {
        public Guid idDireccionEntrega { get; set; }
        [Display(Name = "Dirección Establecimiento:")]
        public String descripcion { get; set; }
        [Display(Name = "Contacto:")]
        public String contacto { get; set; }
        [Display(Name = "Teléfono Contacto:")]
        public String telefono { get; set; }
        public Ubigeo ubigeo { get; set; }
        [Display(Name = "Código Cliente:")]
        public String codigoCliente { get; set; }
        [Display(Name = "Código MP:")]
        public String codigoMP { get; set; }
        [Display(Name = "Sede Cliente:")]
        public String nombre { get; set; }
        [Display(Name = "Observaciones:")]
        public String observaciones { get; set; }
        public String direccionDomicilioLegal { get; set; }

        public String direccionConSede { get {
                return descripcion +" "+ (nombre == null ? "" : "(" + nombre+ (codigoCliente == null ? "" : "/" + codigoCliente )+ ")");
            } }
        [Display(Name = "Email Recepción Facturas:")]
        public String emailRecepcionFacturas { get; set; }

        [Display(Name = "Domicilio Legal:")]
        public DomicilioLegal domicilioLegal { get; set; }


        [Display(Name = "Cliente:")]
        public Cliente cliente { get; set; }

        [Display(Name = "Codigo:")]
        public int codigo { get; set; }

        public DireccionEntrega direccionEntregaAlmacen { get; set; }

        public Decimal limitePresupuesto { get; set; }

        [Display(Name = "Dirección Entrega Proveedor:")]
        public Boolean esDireccionAcopio { get; set; }

    }
}
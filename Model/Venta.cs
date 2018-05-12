using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class Venta : Auditoria 
    {
        public Guid idVenta { get; set; }

        [Display(Name = "Número Venta:")]
        public Int64 numero { get; set; }

        [Display(Name = "Serie Venta:")]
        public Int64 serie { get; set; }

        [Display(Name = "Documento Venta:")]
        public DocumentoVenta documentoVenta { get; set; }

        [Display(Name = "Pedido:")]
        public Pedido pedido { get; set; }

        [Display(Name = "Guía Remisión:")]
        public GuiaRemision guiaRemision { get; set; }

        [Display(Name = "Cotización:")]
        public Cotizacion cotizacion { get; set; }

        [Display(Name = "Sede MP:")]
        public Ciudad ciudad { get; set; }

        [Display(Name = "Cliente:")]
        public Cliente cliente { get; set; }

        [Display(Name = "Tasa IGV:")]
        public Decimal tasaIGV { get; set; }

        [Display(Name = "Sub Total:")]
        public Decimal subTotal { get; set; }

        [Display(Name = "Total:")]
        public Decimal total { get; set; }


        [Display(Name = "IGV:")]
        public Decimal igv { get; set; }

        [Display(Name = "Observaciones:")]
        public String observaciones { get; set; }

        [Display(Name = "Está Anulado:")]
        public Boolean esAnulado { get; set; }

        [Display(Name = "Usuario:")]
        public Usuario usuario { get; set; }
       
        public List<VentaDetalle> ventaDetalleList { get; set; }
    }
}

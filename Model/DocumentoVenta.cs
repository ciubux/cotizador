using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class DocumentoVenta : Auditoria
    {
        public Guid idDocumentoVenta { get; set; }

        [Display(Name = "Número:")]
        public String numero { get; set; }

        [Display(Name = "Serie:")]
        public String serie { get; set; }

        public enum tipoDocumento
        {
            [Display(Name = "Todos")]
            Todos = -1,
            [Display(Name = "Factura")]
            Factura = 0,
            [Display(Name = "Boleta de Venta")]
            BoletaVenta = 1,
            [Display(Name = "Nota de Crédito")]
            NotaCrédito = 2,
            [Display(Name = "Nota de Débito")]
            NotaDébito = 3
        };        
        /*
        [Display(Name = "Pedido:")]
        public Pedido pedido { get; set; }

        [Display(Name = "Guía Remisión:")]
        public GuiaRemision guiaRemision { get; set; }

        [Display(Name = "Cotización:")]
        public Cotizacion cotizacion { get; set; }*/

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

        [Display(Name = "Observaciones:")]
        public String observaciones { get; set; }

        [Display(Name = "Está Anulado:")]
        public Boolean esAnulado { get; set; }

        [Display(Name = "Usuario:")]
        public Usuario usuario { get; set; }

        public List<VentaDetalle> ventaDetalleList { get; set; }
    }
}

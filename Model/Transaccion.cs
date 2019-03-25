using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class Transaccion : Auditoria
    {

        public Transaccion()
        {
            this.tipoNotaCredito = DocumentoVenta.TiposNotaCredito.AnulacionOperacion;
            this.tipoNotaDebito = DocumentoVenta.TiposNotaDebito.Penalidades;
        }

        public Guid idVenta { get; set; }

        [Display(Name = "Número Venta:")]
        public Int64 numero { get; set; }

        [Display(Name = "Serie Venta:")]
        public Int64 serie { get; set; }

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

        [Display(Name = "Sustento:")]
        public String sustento { get; set; }

        [Display(Name = "Documento Venta:")]
        public DocumentoVenta documentoVenta { get; set; }


        public String tipoDocumentoVentaString
        {
            get
            {
                return EnumHelper<DocumentoVenta.TipoDocumento>.GetDisplayValue(this.tipoDocumentoVenta);
            }
        }


        public DocumentoVenta.TipoDocumento tipoDocumentoVenta { get; set; }


        [Display(Name = "Tipo Nota Débito:")]
        public String tipoNotaDebitoString
        {
            get
            {
                return EnumHelper<DocumentoVenta.TiposNotaDebito>.GetDisplayValue(this.tipoNotaDebito);
            }
        }

        public DocumentoVenta.TiposNotaDebito tipoNotaDebito { get; set; }


        [Display(Name = "Tipo Nota Crédito:")]
        public String tipoNotaCreditoString
        {
            get
            {
                return EnumHelper<DocumentoVenta.TiposNotaCredito>.GetDisplayValue(this.tipoNotaCredito);
            }
        }

        public DocumentoVenta.TiposNotaCredito tipoNotaCredito { get; set; }

        public DocumentoReferencia documentoReferencia { get; set; }

        public Boolean permiteEliminarLineas
        {
            get
            {
                return this.tipoNotaCredito == DocumentoVenta.TiposNotaCredito.DescuentoItem
                   || this.tipoNotaCredito == DocumentoVenta.TiposNotaCredito.DevolucionItem
                   || this.tipoNotaCredito == DocumentoVenta.TiposNotaCredito.DisminucionValor;
            }
        }

        public Boolean permiteEditarCantidades
        {
            get
            {
                return this.tipoNotaCredito == DocumentoVenta.TiposNotaCredito.DevolucionItem; // || this.tipoNotaCredito == DocumentoVenta.TiposNotaCredito.AnulacionOperacion;

            }
        }

        public Boolean permiteEditarPrecios
        {
            get
            {
                return this.tipoNotaCredito == DocumentoVenta.TiposNotaCredito.DescuentoItem
                   || this.tipoNotaCredito == DocumentoVenta.TiposNotaCredito.DisminucionValor
                   || this.tipoNotaCredito == DocumentoVenta.TiposNotaCredito.DescuentoGlobal;
            }
        }



        public TiposErrorCrearTransaccion tipoErrorCrearTransaccion { get; set; }
        public String tipoErrorSolicitudAnulacionString
        {
            get
            {
                return EnumHelper<TiposErrorCrearTransaccion>.GetDisplayValue(this.tipoErrorCrearTransaccion);
            }
        }

        public enum TiposErrorCrearTransaccion
        {
            [Display(Name = "Ninguno")]
            NoExisteError = 0,
            [Display(Name = "La venta ya ha sido afectada por otra venta, no se puede continuar generando el documento de venta.")]
            ExisteVentaAfectacion = 1

        }

        public String descripcionError { get; set; }
    }
}

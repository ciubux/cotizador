using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class Cotizacion : IDocumento
    {
        public Cotizacion()
        {
            this.observacionesFijas = Constantes.COTIZACION_OBSERVACIONES_FIJAS;
        }

        public enum OpcionesConsiderarCantidades {
            [Display(Name = "Solo Observaciones")]
            Observaciones = 0,
            [Display(Name = "Solo Cantidades")]
            Cantidades = 1,
            [Display(Name = "Cantidades y Observaciones")]
            Ambos = 2 };

        public enum TiposCotizacion
        {
            [Display(Name = "Permanente")]
            Normal = 0,
            [Display(Name = "Puntual")]
            Transitoria = 1,
            [Display(Name = "Trivial (Sin variación de precios)")]
            Trivial = 2
        };

        public Guid idCotizacion { get; set; }

        public Guid idCotizacionAntecedente { get; set; }

        public bool productosInactivosRemovidos { get; set; }
        public Int64 codigoAntecedente { get; set; }

        [Display(Name = "Número Cotización:")]
        public Int64 codigo { get; set; }
        public DateTime fecha { get; set; }
        public Boolean fechaEsModificada { get; set; }

        [Display(Name = "Tipo Cotización:")]
        public TiposCotizacion tipoCotizacion { get; set; }

        /* 0 indica días, 1 indica fecha */
        public int mostrarValidezOfertaEnDias { get; set; }
        //plazo oferta dias
        public int validezOfertaEnDias { get; set; }

        public DateTime fechaLimiteValidezOferta { get; set; }

        public DateTime? fechaFinVigenciaPrecios { get; set; }
        public DateTime? fechaInicioVigenciaPrecios { get; set; }

        public int estadoExtendida { get; set; }
        public DateTime? fechaFinVigenciaPreciosExtendida { get; set; }

        public DateTime fechaModificacion { get; set; }

        public Usuario usuario { get; set; }

        public Boolean incluidoIGV { get; set; }
        public Decimal flete { get; set; }
        public String usuarioCreacion { get; set; }
        //  public Usuario usuario_aprobador { get; set; }
        public Decimal igv { get; set; }

        [Display(Name = "Moneda:")]
        public Moneda moneda { get; set; }

        [Display(Name = "Sede MP:")]
        public Ciudad ciudad { get; set; }
        public Cliente cliente { get; set; }
        public GrupoCliente grupo { get; set; }

        [Display(Name = "Promoción:")]
        public Promocion promocion { get; set; }
        public OpcionesConsiderarCantidades considerarCantidades { get; set; }
        public Boolean mostrarCodigoProveedor { get; set; }
        public Decimal montoSubTotal { get; set; }
        public Decimal montoIGV { get; set; }
        public Decimal montoTotal { get; set; }
        public String observaciones { get; set; }
        public String observacionesFijas { get; set; }
        public String contacto { get; set; }
        public List<CotizacionDetalle> cotizacionDetalleList { get; set; }
        public bool esRecotizacion { get; set; }
        /*0 pendiente, 1 aprobado, 2 rechazado*/
        public SeguimientoCotizacion seguimientoCotizacion { get; set; }

        public List<SeguimientoCotizacion> seguimientoCotizacionList { get; set; }

        public bool mostrarCosto { get; set; }
        public bool ajusteCalculoPrecios { get; set; }
        public bool considerarDescontinuados { get; set; }

        public Decimal maximoPorcentajeDescuentoPermitido { get; set; }

        public Decimal minimoMargen { get; set; }

        public bool buscarSedesGrupoCliente { get; set; }

        public bool buscarSoloCotizacionesGrupales { get; set; }
        /*Campos utilizados para búsqueda*/
        public Usuario usuarioBusqueda { get; set; }
        public DateTime fechaDesde { get; set; }
        public DateTime fechaHasta { get; set; }

        public DateTime fechaPrecios { get; set; }

        public Boolean aplicaSedes { get; set; }

        public Boolean esPagoContado { get; set; }

        [Display(Name = "Promociones:")]
        public List<Promocion> promociones { get; set; }
        public List<DocumentoDetalle> documentoDetalle
        {
            get {
                List<DocumentoDetalle> documentoDetalle = new List<DocumentoDetalle>();
                if (this.cotizacionDetalleList == null)
                    this.cotizacionDetalleList = new List<CotizacionDetalle>();
                foreach (CotizacionDetalle cotizacionDetalle in cotizacionDetalleList)
                {
                    documentoDetalle.Add(cotizacionDetalle);
                }
                return documentoDetalle;
                }
            set {
                this.cotizacionDetalleList = new List<CotizacionDetalle>();
                foreach (DocumentoDetalle documentoDetalle in value)
                {
                    cotizacionDetalleList.Add((CotizacionDetalle)documentoDetalle);
                }


            }
        }


        public String numeroCotizacionString
        {
            get { return this.codigo == 0 ? "" : this.codigo.ToString().PadLeft(Constantes.LONGITUD_NUMERO, Constantes.PAD); }
        }


        public String textoCondicionesPago
        {
            get
            {
                if (this.esPagoContado)
                {
                    return EnumHelper<DocumentoVenta.TipoPago>.GetDisplayValue(DocumentoVenta.TipoPago.Contado);
                } else {
                    if (this.cliente != null && this.cliente.idCliente != Guid.Empty)
                    {
                        /*Se evalua si el solicitado es al contado*/
                        if (this.cliente.plazoCreditoSolicitado == DocumentoVenta.TipoPago.Contado)
                        {
                            return EnumHelper<DocumentoVenta.TipoPago>.GetDisplayValue(this.cliente.plazoCreditoSolicitado) + ".";
                        }
                        /*Si no es al contado y el pago aprobado es no asignado se muestra el mensaje que indica que está sujeto a evaluación*/
                        else if (this.cliente.tipoPagoFactura == DocumentoVenta.TipoPago.NoAsignado)
                        {
                            return EnumHelper<DocumentoVenta.TipoPago>.GetDisplayValue(this.cliente.plazoCreditoSolicitado) + ", sujeto a evaluación crediticia (aprobación pendiente).";
                        }
                        /*Si no es un caso anterior se muestra el plazo de credito aprobado*/
                        else
                        {
                            return EnumHelper<DocumentoVenta.TipoPago>.GetDisplayValue(this.cliente.tipoPagoFactura) + ".";
                        }
                    }
                    else if (this.grupo != null && this.grupo.idGrupoCliente != 0)
                    {
                        if (this.grupo.plazoCreditoSolicitado == DocumentoVenta.TipoPago.Contado)
                        {
                            return EnumHelper<DocumentoVenta.TipoPago>.GetDisplayValue(this.grupo.plazoCreditoSolicitado) + ".";
                        }
                        /*Si no es al contado y el pago aprobado es no asignado se muestra el mensaje que indica que está sujeto a evaluación*/
                        else if (this.grupo.plazoCreditoAprobado == DocumentoVenta.TipoPago.NoAsignado)
                        {
                            return EnumHelper<DocumentoVenta.TipoPago>.GetDisplayValue(this.grupo.plazoCreditoSolicitado) + ", sujeto a evaluación crediticia (aprobación pendiente).";
                        }
                        /*Si no es un caso anterior se muestra el plazo de credito aprobado*/
                        else
                        {
                            return EnumHelper<DocumentoVenta.TipoPago>.GetDisplayValue(this.grupo.plazoCreditoAprobado) + ".";
                        }

                    }
                    else
                    {
                        return String.Empty;
                    }
                }
            }
         
        }

        public Decimal utilidadVisible
        {
            get
            {
                decimal utilidad = 0;
                
                foreach(CotizacionDetalle det in this.documentoDetalle)
                {
                    if (det.tieneCostoEspecial)
                    {
                        utilidad = utilidad + ((det.precioNeto - det.costoEspecialVisible) * det.cantidad);
                    } else
                    {
                        utilidad = utilidad + ((det.precioNeto - det.costoListaVisible) * det.cantidad);
                    }
                    
                }

                return utilidad;
            }
        }

        public Decimal utilidadFleteVisible
        {
            get
            {
                decimal utilidad = 0;
                foreach (CotizacionDetalle det in this.documentoDetalle)
                {
                    if (det.tieneCostoEspecial)
                    {
                        utilidad = utilidad + ((det.precioNeto - det.costoEspecialFleteVisible) * det.cantidad);
                    }
                    else
                    {
                        utilidad = utilidad + ((det.precioNeto - det.costoListaFleteVisible) * det.cantidad);
                    }
                }

                return utilidad;
            }
        }

        public Decimal margenVisible
        {
            get
            {
                decimal utilidad = this.utilidadVisible;
                decimal total = 0;

                foreach (CotizacionDetalle det in this.documentoDetalle)
                {
                    total = total + (det.precioNeto * det.cantidad);
                }

                if (total == 0)
                {
                    return 0;
                }

                return Decimal.Parse(String.Format(Constantes.formatoUnDecimal, (utilidad / total) * 100));
            }
        }

        public Decimal margenFleteVisible
        {
            get
            {
                decimal utilidad = this.utilidadFleteVisible;
                decimal total = 0;

                foreach (CotizacionDetalle det in this.documentoDetalle)
                {
                    total = total + (det.precioNeto * det.cantidad);
                }

                if (total == 0)
                {
                    return 0;
                }
                
                return Decimal.Parse(String.Format(Constantes.formatoUnDecimal, (utilidad / total) * 100));
            }
        }

        /**
         * Utilizado para búsqueda
         */
        [Display(Name = "SKU:")]
        public String sku { get; set; }

    }
}

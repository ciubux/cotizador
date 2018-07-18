using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class Pedido : Auditoria, IDocumento
    {
        public Pedido()
        {
            this.tipoPedido = tiposPedido.Venta;
        }

        public Guid idPedido { get; set; }
        [Display(Name = "Número Pedido:")]
        public Int64 numeroPedido { get; set; }
        [Display(Name = "Número Grupo Pedido:")]
        public Int64? numeroGrupoPedido { get; set; }

        public Cotizacion cotizacion { get; set; }
        [Display(Name = "Sede MP:")]
        public Ciudad ciudad { get; set; }

        [Display(Name = "Solicitar A:")]
        public Ciudad ciudadASolicitar { get; set; }

        [Display(Name = "Cliente:")]
        public Cliente cliente { get; set; }
        [Display(Name = "Orden de Compra N°:")]
        public String numeroReferenciaCliente { get; set; }


        [Display(Name = "Referencia Adicional Cliente:")]
        public String numeroReferenciaAdicional { get; set; }

        [Display(Name = "Otros Cargos (Flete):")]
        public Decimal otrosCargos { get; set; }
        public DireccionEntrega direccionEntrega { get; set; }
        [Display(Name = "Solicitado por:")]
        public Solicitante solicitante { get; set; }

        [Display(Name = "Fecha de Entrega:")]
        public DateTime? fechaEntregaDesde { get; set; }

        [Display(Name = "Fecha Máxima de Entrega:")]
        public DateTime? fechaEntregaHasta { get; set; }

        [Display(Name = "Hora de Entrega:")]
        public String horaEntregaDesde { get; set; }

        [Display(Name = "Fecha Máxima de Entrega:")]
        public String horaEntregaHasta { get; set; }



        [Display(Name = "Fecha Solicitud:")]
        public DateTime fechaSolicitud { get; set; }
        [Required(ErrorMessage = "Ingrese la Fecha de Entrega.")]

        [Display(Name = "Solicitado Por:")]
        public String contactoPedido { get; set; }
        [Display(Name = "Telefono Solicitante:")]
        public String telefonoContactoPedido { get; set; }
        [Display(Name = "Correo Solicitante:")]
        public String correoContactoPedido { get; set; }

        [Display(Name = "Telefono y Correo Solicitante:")]
        public String telefonoCorreoContactoPedido
        {
            get { return "Tef: " + telefonoContactoPedido + " Correo: " + correoContactoPedido; }
        }

        [Display(Name = "Observaciones para uso interno:")]
        public String observaciones { get; set; }

        [Display(Name = "Observaciones Guía Remisión:")]
        public String observacionesGuiaRemision { get; set; }

        [Display(Name = "Observaciones Factura:")]
        public String observacionesFactura { get; set; }

        [Display(Name = "Ciudad Entrega:")]
        public Ubigeo ubigeoEntrega { get; set; }



        [Display(Name = "Fechas de Entrega:")]
        public String rangoFechasEntrega {
            get
            {
                String rangoFechasEntrega = String.Empty;
                if (this.fechaEntregaDesde != null && this.fechaEntregaHasta != null)
                {
                    String entregaDesde = this.fechaEntregaDesde.Value.ToString(Constantes.formatoFecha);
                    String entregaHasta = this.fechaEntregaHasta.Value.ToString(Constantes.formatoFecha);

                    if (entregaDesde.Equals(entregaHasta))
                    {
                        return entregaDesde;
                    }
                    else
                    {
                        return "Desde: " + entregaDesde + " Hasta: " + entregaHasta;
                    }
                }
                else
                {
                    return "";
                }

            }
        }

        [Display(Name = "Horario de Entrega:")]
        public String rangoHoraEntrega
        {
            get
            {
                return "Desde: " + this.horaEntregaDesde + " Hasta: " + this.horaEntregaHasta;
            }
        }

        [Display(Name = "Fecha y Horario Entrega:")]
        public String fechaHorarioEntrega
        {

            get {
                return rangoFechasEntrega + " " + rangoHoraEntrega;
            }
        }

        public Boolean existeCambioDireccionEntrega { get; set; }

        public Boolean existeCambioSolicitante { get; set; }


        public String fechaHoraSolicitud
        {
            get { return this.fechaSolicitud.ToString("dd/MM/yyyy HH:mm"); }
        }

        public String fechaHoraRegistro
        {
            get { return this.FechaRegistro.ToString("dd/MM/yyyy HH:mm"); }
        }

        public String numeroGrupoPedidoString
        {
            get { return this.numeroGrupoPedido == 0 ? "" : this.numeroGrupoPedido.ToString().PadLeft(Constantes.LONGITUD_NUMERO, Constantes.PAD); }
        }

        public String numeroPedidoString
        {
            get { return this.numeroPedido.ToString().PadLeft(Constantes.LONGITUD_NUMERO, Constantes.PAD); }
        }

        public Usuario usuario { get; set; }
        public Boolean incluidoIGV { get; set; }
        //   public Decimal flete { get; set; }
        public String usuarioCreacion { get; set; }





        public DateTime fechaModificacion { get; set; }


        //  public Usuario usuario_aprobador { get; set; }
        // public Boolean mostrarCodigoProveedor { get; set; }
        public Decimal montoSubTotal { get; set; }
        public Decimal montoIGV { get; set; }
        public Decimal montoTotal { get; set; }


        public List<PedidoDetalle> pedidoDetalleList { get; set; }

        public List<PedidoAdjunto> pedidoAdjuntoList { get; set; }
        public bool esRePedido { get; set; }
        /*0 pendiente, 1 aprobado, 2 rechazado*/
        public SeguimientoPedido seguimientoPedido { get; set; }

        public List<SeguimientoPedido> seguimientoPedidoList { get; set; }

        public SeguimientoCrediticioPedido seguimientoCrediticioPedido { get; set; }

        public List<SeguimientoCrediticioPedido> seguimientoCrediticioPedidoList { get; set; }
        public bool mostrarCosto { get; set; }

        public bool considerarDescontinuados { get; set; }








        /*Campos utilizados para búsqueda*/
        public Usuario usuarioBusqueda { get; set; }
        [Display(Name = "Solicitado Desde:")]
        public DateTime fechaSolicitudDesde { get; set; }
        [Display(Name = "Solicitado Hasta:")]
        public DateTime fechaSolicitudHasta { get; set; }


        [Display(Name = "Fecha Programación:")]
        public DateTime? fechaProgramacion { get; set; }

        [Display(Name = "Comentario Programación:")]
        public String comentarioProgramacion { get; set; }

        public DateTime? fechaProgramacionDesde { get; set; }

        public DateTime? fechaProgramacionHasta { get; set; }

        public DateTime fechaPrecios { get; set; }


        public List<GuiaRemision> guiaRemisionList { get; set; }
    

        /*Implementación Interface*/
        public List<DocumentoDetalle> documentoDetalle
        {
            get
            {
                List<DocumentoDetalle> documentoDetalle = new List<DocumentoDetalle>();
                if (this.pedidoDetalleList == null)
                    this.pedidoDetalleList = new List<PedidoDetalle>();
                foreach (PedidoDetalle pedidoDetalle in pedidoDetalleList)
                {
                    documentoDetalle.Add(pedidoDetalle);
                }
                return documentoDetalle;
            }
            set
            {
                this.pedidoDetalleList = new List<PedidoDetalle>();
                foreach (DocumentoDetalle documentoDetalle in value)
                {
                    pedidoDetalleList.Add((PedidoDetalle)documentoDetalle);
                }
            }
        }

        //VENTA, TRASLADO INTERNO, COMODATO,  TRANSFERENCIA GRATUITA, PRESTAMO

        [Display(Name = "Tipo Pedido:")]
        public tiposPedido tipoPedido { get; set; }
        public enum tiposPedido
        {
            /*GENERAN GUIA REMISION*/
            [Display(Name = "Venta")]
            Venta = 'V', 
            [Display(Name = "Traslado Interno")]
            TrasladoInterno = 'T',
            [Display(Name = "Comodato a Entregar")]
            ComodatoEntregado = 'M',
            [Display(Name = "Transferencia Gratuita a Entregar")]
            TransferenciaGratuitaEntregada = 'G',
            [Display(Name = "Préstamo a Entregar")]
            PrestamoEntregado = 'P',
            [Display(Name = "Devolución de Compra")]
            DevolucionCompra = 'B',
            [Display(Name = "Devolución de Préstamo Recibido")]
            DevolucionPrestamoRecibido = 'E',
            [Display(Name = "Devolución de Comodato Recibido")]
            DevolucionComodatoRecibido = 'F',
            [Display(Name = "Devolución de Transferencia Gratuita Recibida")]
            DevolucionTransferenciaGratuitaRecibida = 'H',

            /*GENERAN NOTA INGRESO
            [Display(Name = "Compra")]
            Compra = 'C', 
            [Display(Name = "Comodato Recibido")]
            ComodatoRecibido = 'M',
            [Display(Name = "Transferencia Gratuita Recibida")]
            TransferenciaGratuitaRecibida = 'G',
            [Display(Name = "Préstamo Recibido")]
            PrestamoRecibido = 'P',
            [Display(Name = "Devolución de Venta")]
            DevolucionVenta = 'D', //GENERA NOTA DE CREDITO
            [Display(Name = "Devolución de Préstamo Entregado")]
            DevolucionPrestamoEntregado = 'E',
            [Display(Name = "Devolución de Comodato Entregado")]
            DevolucionComodatoEntregado = 'F',
            [Display(Name = "Devolución de Transferencia Gratuita Entregada")]
            DevolucionTransferenciaGratuitaEntregada = 'H',
            */
        }

        public String tiposPedidoString
        {
            get
            {
                return EnumHelper<tiposPedido>.GetDisplayValue(this.tipoPedido);
            }
        }

        public DocumentoVenta documentoVenta { get; set; }

      

    }
}

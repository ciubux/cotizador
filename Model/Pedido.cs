using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class Pedido : Auditoria , IDocumento 
    {
        public Guid idPedido { get; set; }
        [Display(Name = "Número Pedido:")]
        public Int64 numeroPedido { get; set; }
        [Display(Name = "Número Grupo Pedido:")]
        public Int64? numeroGrupoPedido { get; set; }

        public Cotizacion cotizacion { get; set; }
        [Display(Name = "Ciudad:")]
        public Ciudad ciudad { get; set; }
        [Display(Name = "Cliente:")]
        public Cliente cliente { get; set; }
        [Display(Name = "Número Documento Cliente (OC):")]
        public String numeroReferenciaCliente { get; set; }
        [Display(Name = "Dirección de Despacho:")]
        public String direccionEntrega { get; set; }
        [Display(Name = "Contacto de Entrega:")]
        public String contactoEntrega { get; set; }
        [Display(Name = "Telefono Contacto de Entrega:")]
        public String telefonoContactoEntrega { get; set; }
        [Display(Name = "Fecha Solicitud:")]
        public DateTime fechaSolicitud { get; set; }
        [Required(ErrorMessage = "Ingrese la Fecha de Entrega.")]
        [Display(Name = "Fecha de Entrega:")]
        public DateTime fechaMaximaEntrega { get; set; }
        [Display(Name = "Fecha Máxima de Entrega:")]
        public DateTime fechaEntrega { get; set; }
        [Display(Name = "Pedido Por:")]
        public String contactoPedido { get; set; }
        [Display(Name = "Telefono Pedido Por:")]
        public String telefonoContactoPedido { get; set; }
        [Display(Name = "Observaciones:")]
        public String observaciones { get; set; }


        public String rangoFechasEntrega {
            get
            {

                String rangoFechasEntrega = String.Empty;
                String entregaDesde = this.fechaEntrega.ToString(Constantes.formatoFecha);
                String entregaHasta = this.fechaMaximaEntrega.ToString(Constantes.formatoFecha);

                if (entregaDesde.Equals(entregaHasta))
                {
                    return entregaDesde;
                }
                else
                {
                    return "Desde: " + entregaDesde + " Hasta:" + entregaHasta;
                }
            }
        }

        public String fechaHoraSolicitud
        {
            get { return this.fechaSolicitud.ToString("dd/MM/yyyy hh:mm"); }
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
        public bool esRePedido { get; set; }
        /*0 pendiente, 1 aprobado, 2 rechazado*/
        public SeguimientoPedido seguimientoPedido { get; set; }

        public List<SeguimientoPedido> seguimientoPedidoList { get; set; }

        public bool mostrarCosto { get; set; }

        public bool considerarDescontinuados { get; set; }








        /*Campos utilizados para búsqueda*/
        public Usuario usuarioBusqueda { get; set; }
        [Display(Name = "Solicitado Desde:")]
        public DateTime fechaSolicitudDesde { get; set; }
        [Display(Name = "Solicitado Hasta:")]
        public DateTime fechaSolicitudHasta { get; set; }
        [Display(Name = "Entrega Desde:")]
        public DateTime fechaEntregaDesde { get; set; }
        [Display(Name = "Entrega Hasta:")]
        public DateTime fechaEntregaHasta { get; set; }
        public DateTime fechaPrecios { get; set; }

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
    }
}

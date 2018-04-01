using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class Pedido
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



        public Usuario usuario { get; set; }
        public Boolean incluidoIgv { get; set; }
        public Decimal flete { get; set; }
        public String usuarioCreacion { get; set; }

        
        
        

        public DateTime fechaModificacion { get; set; }


      //  public Usuario usuario_aprobador { get; set; }
        public Decimal tasaIGV { get; set; }
        
        public Boolean mostrarCodigoProveedor { get; set; }
        public Decimal montoSubTotal { get; set; }
        public Decimal montoIGV { get; set; }
        public Decimal montoTotal { get; set; }
       

        public List<PedidoDetalle> PedidoDetalleList { get; set; }
        public bool esRePedido { get; set; }
        /*0 pendiente, 1 aprobado, 2 rechazado*/
        public SeguimientoPedido seguimientoPedido { get; set; }

        public List<SeguimientoPedido> seguimientoPedidoList { get; set; }

        public bool mostrarCosto { get; set; }

        public bool considerarDescontinuados { get; set; }

        






        /*Campos utilizados para búsqueda*/
        public Usuario usuarioBusqueda { get; set; }
        public DateTime fechaDesde { get; set; }
        public DateTime fechaHasta { get; set; }

        public DateTime fechaPrecios { get; set; }
        
        
    }
}

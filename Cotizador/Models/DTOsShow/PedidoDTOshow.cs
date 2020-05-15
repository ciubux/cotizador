using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cotizador.Models.DTOshow
{
    public class PedidoDTOshow
    {
        
        public Guid idPedido { get; set; }
        public DateTime? fechaEntregaDesde { get; set; }
        public DateTime? fechaEntregaHasta { get; set; }
        public String numeroPedidoString { get; set; }
        public String numeroGrupoPedidoString { get; set; }
        public String cotizacion_numeroCotizacionString { get; set; }
        public String tiposPedidoString { get; set; }
        public String cliente_responsableComercial_codigoDescripcion { get; set; }
        public String cliente_responsableComercial_usuario_email { get; set; }
        public String cliente_supervisorComercial_codigoDescripcion { get; set; }
        public String cliente_supervisorComercial_usuario_email { get; set; }
        public String cliente_asistenteServicioCliente_codigoDescripcion { get; set; }
        public String cliente_asistenteServicioCliente_usuario_email { get; set; }
        public String fechaHorarioEntrega { get; set; }
        public String ciudad_nombre { get; set; }
        public Guid cliente_idCliente { get; set; }
        public String cliente_codigoRazonSocial { get; set; }
        public String numeroReferenciaCliente { get; set; }
        public String numeroReferenciaAdicional { get; set; }
        public String fechaEntregaExtendidaString { get; set; }
        public String direccionEntrega_descripcion { get; set; }
        public String direccionEntrega_telefono { get; set; }
        public String direccionEntrega_contacto { get; set; }
        public String usuario_nombre { get; set; }
        public String fechaHoraRegistro { get; set; }
        public Model.Ubigeo ubigeoEntrega { get; set; }
        public String contactoPedido { get; set; }
        public String telefonoCorreoContactoPedido { get; set; }
        public String fechaHoraSolicitud { get; set; }
        public String seguimientoPedido_estadoString { get; set; }
        public String seguimientoPedido_usuario_nombre { get; set; }
        public String seguimientoPedido_observacion { get; set; }
        public String seguimientoCrediticioPedido_estadoString { get; set; }
        public String seguimientoCrediticioPedido_usuario_nombre { get; set; }
        public String seguimientoCrediticioPedido_observacion { get; set; }
        public String observaciones { get; set; }
        public String observacionesFactura { get; set; }
        public String observacionesGuiaRemision { get; set; }
        public Decimal montoSubTotal { get; set; }
        public Decimal montoIGV { get; set; }
        public Decimal montoTotal { get; set; }
        public List<Model.PedidoAdjunto> pedidoAdjuntoList { get; set; }
        public List<Model.PedidoDetalle> pedidoDetalleList { get; set; }
        public String cliente_razonSocialSunat { get; set; }
        public String cliente_ruc { get; set; }
        public String cliente_direccionDomicilioLegalSunat { get; set; }
        public String cliente_codigo { get; set; }
        public String cliente_correoEnvioFactura { get; set; }
        public Model.Pedido.tiposPedido tipoPedido { get; set; }
        public List<Model.GuiaRemision> guiaRemisionList { get; set; }
        public Model.SeguimientoPedido.estadosSeguimientoPedido seguimientoPedido_estado { get; set; }
        public  Model.SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido seguimientoCrediticioPedido_estado { get; set; }     
        public String documentoVenta_numero { get; set; }
        public String textoCondicionesPago { get; set; }
        public List<PedidoGrupo> pedidoGrupoList { get; set; }
        public Int64? numeroGrupoPedido { get; set; }
        public String numeroRequerimiento { get; set; }
        public int cotizacion_tipoCotizacion { get; set; }

        public int truncado { get; set; }
        public String grupoCliente_nombre { get; set; }

    }
}
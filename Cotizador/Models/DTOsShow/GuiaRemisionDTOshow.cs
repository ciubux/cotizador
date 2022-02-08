using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model.CONFIGCLASSES;

namespace Cotizador.Models.DTOshow
{
    public class GuiaRemisionDTOshow
    {
        public Guid pedido_idPedido { get; set; }
        public Guid idMovimientoAlmacen { get; set; }
        public String ciudadOrigen_nombre { get; set; }
        public String ciudadOrigen_direccionPuntoPartida { get; set; }
        public DateTime? fechaTraslado { get; set; }
        public DateTime? fechaEmision { get; set; }
        public String serieNumeroGuia { get; set; }     
        public String pedido_numeroPedidoString { get; set; }
        public String pedido_cliente_razonSocial { get; set; }

        public long pedido_numeroGrupo { get; set; }
        public int pedido_facturaUnica { get; set; }

        public int guiaAtiendePedido { get; set; }

        public ClienteConfiguracion pedido_cliente_configuraciones { get; set; }
        public String pedido_numeroReferenciaCliente { get; set; }
        public String motivoTrasladoString { get; set; }
        public bool atencionParcial { get; set; }
        public Model.Ubigeo pedido_ubigeoEntrega { get; set; }
        public String pedido_direccionEntrega_descripcion { get; set; }
        public String transportista_descripcion { get; set; }
        public String transportista_ruc { get; set; }
        public String transportista_brevete { get; set; }
        public String transportista_direccion { get; set; }
        public String placaVehiculo { get; set; }
        public String observaciones { get; set; }
        public bool estaAnulado { get; set; }
        public String estadoDescripcion { get; set; }
        public Model.MovimientoAlmacen.TiposExtorno tipoExtorno { get; set; }
        public String tipoExtornoToString { get; set; }
        public Model.NotaIngreso notaIngresoAExtornar { get; set; }
        public String notaIngresoAExtornar_serieNumeroNotaIngreso { get; set; }
        public String motivoExtornoNotaIngresoToString { get; set; }
        public String sustentoExtorno { get; set; }
        public bool estaFacturado { get; set; }
        public List<Model.Transaccion> transaccionList { get; set; }
        public Model.GuiaRemision.motivosTraslado motivoTraslado { get; set; }
        public bool ingresado { get; set; }
        public List<Model.DocumentoDetalle> documentoDetalle { get; set; }
        
       public bool esGuiaDiferida { get; set; }

    }
}
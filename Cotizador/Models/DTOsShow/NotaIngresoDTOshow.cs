using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cotizador.Models.DTOshow
{
    public class NotaIngresoDTOshow
    {

        public Guid pedido_idPedido { get; set; }
        public Guid idMovimientoAlmacen { get; set; }
        public String ciudadDestino_nombre { get; set; }
        public String ciudadDestino_direccionPuntoLlegada { get; set; }
        public String direccionPuntoLlegada { get; set; }
        public DateTime? fechaTraslado { get; set; }
        public DateTime? fechaEmision { get; set; }
        public String serieNumeroNotaIngreso { get; set; }
        public String pedido_numeroPedidoString { get; set; }
        public String pedido_cliente_razonSocial { get; set; }
        public String pedido_numeroReferenciaCliente { get; set; }
        public String motivoTrasladoString { get; set; }
        public bool atencionParcial { get; set; }
        public String observaciones { get; set; }
        public String estadoDescripcion { get; set; }
        public bool estaAnulado { get; set; }
        public Model.MovimientoAlmacen.TiposExtorno tipoExtorno { get; set; }
        public String tipoExtornoToString { get; set; }
        public Model.GuiaRemision guiaRemisionAExtornar { get; set; }
        public Model.GuiaRemision guiaRemisionAIngresar { get; set; }
        public String serieGuiaReferencia { get; set; }
        public int numeroGuiaReferencia { get; set; }
        public String serieDocumentoVentaReferencia { get; set; }
        public int numeroDocumentoVentaReferencia { get; set; }
        public String tipoDocumentoVentaReferenciaString { get; set; }
        //public String guiaRemisionAIngresar_serieNumeroGuia { get; set; }
        //public String guiaRemisionAExtornar_serieNumeroGuia { get; set; }
        public String motivoExtornoGuiaRemisionToString { get; set; }
        public String sustentoExtorno { get; set; }
        public String usuario_nombre { get; set; }
        public Model.NotaIngreso.motivosTraslado motivoTraslado { get; set; }
        public bool estaFacturado { get; set; }
        public List<Model.DocumentoDetalle> documentoDetalle { get; set; }

    }
}
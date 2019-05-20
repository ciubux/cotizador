using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class PedidoCompra : Pedido, IDocumento
    {
        public PedidoCompra(ClasesPedido tipo)
        {
            this.clasePedido = tipo;
            this.tipoPedido = tiposPedido.Venta;
            this.tipoPedidoCompra = tiposPedidoCompra.Compra;
            this.tipoPedidoAlmacen = tiposPedidoAlmacen.TrasladoInterno;
            this.tipoPedidoVentaBusqueda = tiposPedidoVentaBusqueda.Todos;
            this.tipoPedidoCompraBusqueda = tiposPedidoCompraBusqueda.Todos;
            this.tipoPedidoAlmacenBusqueda = tiposPedidoAlmacenBusqueda.Todos;

            this.solicitante = new Solicitante();
            this.pedidoAdjuntoList = new List<PedidoAdjunto>();
            this.ciudadASolicitar = new Ciudad();

            this.idGrupoCliente = 0;
        }        

        public DocumentoCompra documentoCompra { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class PedidoVenta : Pedido, IDocumento
    {
        public PedidoVenta()
        {
            this.clasePedido = ClasesPedido.Venta;
        }



        /*

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
        */
        
    }
}

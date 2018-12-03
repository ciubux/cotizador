using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cotizador.Models.DTOsSearch
{
    public static class ParserDTOsSearch
    {
        public static List<PedidoDTO> PedidoToPedidoDTO(List<Pedido> pedidoList)
        {
            
            List<PedidoDTO> pedidoDTOList = new List<PedidoDTO>();
            foreach (Pedido pedidoTmp in pedidoList )
            {
                PedidoDTO pedidoDTO = new PedidoDTO();
                pedidoDTO.fechaProgramacion = pedidoTmp.fechaProgramacion;
                pedidoDTO.stockConfirmado = pedidoTmp.stockConfirmado;
                pedidoDTO.observaciones = pedidoTmp.observaciones;
                pedidoDTO.idPedido = pedidoTmp.idPedido;
                pedidoDTO.numeroPedido = pedidoTmp.numeroPedido;
                pedidoDTO.numeroPedidoNumeroGrupoString = pedidoTmp.numeroPedidoNumeroGrupoString;
                pedidoDTO.ciudad_nombre = pedidoTmp.ciudad.nombre;
                pedidoDTO.cliente_codigo = pedidoTmp.cliente.codigo;
                pedidoDTO.cliente_razonSocial = pedidoTmp.cliente.razonSocial;
                pedidoDTO.numeroReferenciaCliente = pedidoTmp.numeroReferenciaCliente;
                pedidoDTO.usuario_nombre = pedidoTmp.usuario.nombre;
                pedidoDTO.fechaHoraRegistro = pedidoTmp.fechaHoraRegistro;
                pedidoDTO.rangoFechasEntrega = pedidoTmp.rangoFechasEntrega;
                pedidoDTO.rangoHoraEntrega = pedidoTmp.rangoHoraEntrega;
                pedidoDTO.montoTotal = pedidoTmp.montoTotal;
                pedidoDTO.ubigeoEntrega_distrito = pedidoTmp.ubigeoEntrega.Distrito;
                pedidoDTO.seguimientoPedido_estadoString = pedidoTmp.seguimientoPedido.estadoString;
                pedidoDTO.seguimientoCrediticioPedido_estadoString = pedidoTmp.seguimientoCrediticioPedido.estadoString;
                pedidoDTOList.Add(pedidoDTO);
            }
            return pedidoDTOList;
        }


    }
}
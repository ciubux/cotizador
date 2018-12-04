using Cotizador.Models.DTOs;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cotizador.Models.DTOsSearch
{
    public class ParserDTOsSearch
    {

        public static List<ClienteDTO> ClienteToClienteDTO(List<Cliente> clienteList)
        {
            List<ClienteDTO> clienteDTOList = new List<ClienteDTO>();
            foreach (Cliente clienteTmp in clienteList)
            {
                ClienteDTO clienteDTO = new ClienteDTO();

                clienteDTO.idCliente = clienteTmp.idCliente;
                clienteDTO.codigo = clienteTmp.codigo;
                clienteDTO.razonSocialSunat = clienteTmp.razonSocialSunat;
                clienteDTO.nombreComercial = clienteTmp.nombreComercial;
                clienteDTO.tipoDocumentoIdentidadToString = clienteTmp.tipoDocumentoIdentidadToString;
                clienteDTO.ruc = clienteTmp.ruc;
                clienteDTO.ciudad_nombre = clienteTmp.ciudad.nombre;
                clienteDTO.grupoCliente_nombre = clienteTmp.grupoCliente.nombre;
                clienteDTO.responsableComercial_descripcion = clienteTmp.responsableComercial.descripcion;
                clienteDTO.supervisorComercial_descripcion = clienteTmp.supervisorComercial.descripcion;
                clienteDTO.asistenteServicioCliente_descripcion = clienteTmp.asistenteServicioCliente.descripcion;
                clienteDTO.creditoAprobado = clienteTmp.creditoAprobado;

                clienteDTO.tipoPagoFacturaToString = clienteTmp.tipoPagoFactura.ToString();

                clienteDTO.bloqueado = clienteTmp.bloqueado;

                clienteDTOList.Add(clienteDTO);
            }
            return clienteDTOList;

        }

        public static List<CotizacionDTO> CotizacionToCotizacionDTO(List<Cotizacion> cotizacionList)
        {
            List<CotizacionDTO> cotizacionDTOList = new List<CotizacionDTO>();
            foreach (Cotizacion cotizacionTmp in cotizacionList)
            {
                CotizacionDTO cotizacionDTO = new CotizacionDTO();

                cotizacionDTO.idCotizacion = cotizacionTmp.idCotizacion;
                cotizacionDTO.codigo = cotizacionTmp.codigo;
                cotizacionDTO.usuario_nombre = cotizacionTmp.usuario.nombre;
                cotizacionDTO.fecha = cotizacionTmp.fecha;
                cotizacionDTO.cliente_razonSocial = cotizacionTmp.cliente.razonSocial;
                cotizacionDTO.ciudad_nombre = cotizacionTmp.ciudad.nombre;
                cotizacionDTO.cliente_ruc = cotizacionTmp.cliente.ruc;
                cotizacionDTO.montoTotal = cotizacionTmp.montoTotal;
                cotizacionDTO.maximoPorcentajeDescuentoPermitido = cotizacionTmp.maximoPorcentajeDescuentoPermitido;
                cotizacionDTO.montoIGV = cotizacionTmp.montoIGV;
                cotizacionDTO.montoSubTotal = cotizacionTmp.montoSubTotal;
                cotizacionDTO.minimoMargen = cotizacionTmp.minimoMargen;
                cotizacionDTO.seguimientoCotizacion_estadoString = cotizacionTmp.seguimientoCotizacion.estadoString;
                cotizacionDTO.seguimientoCotizacion_usuario_nombre = cotizacionTmp.seguimientoCotizacion.usuario.nombre;
                cotizacionDTO.seguimientoCotizacion_observacion = cotizacionTmp.seguimientoCotizacion.observacion;
                cotizacionDTOList.Add(cotizacionDTO);
            }
            return cotizacionDTOList;
        }

        public static List<FacturaDTO> FacturaToFacturaDTO(List<DocumentoVenta> documentoVentaList)
        {
            List<FacturaDTO> facturaDTOList = new List<FacturaDTO>();
            foreach (DocumentoVenta documentoventaTmp in documentoVentaList)
            {
                FacturaDTO facturaDTO = new FacturaDTO();

                facturaDTO.idDocumentoVenta = documentoventaTmp.idDocumentoVenta;
                facturaDTO.serieNumero = documentoventaTmp.serieNumero;
                facturaDTO.pedido_numeroPedidoString = documentoventaTmp.pedido.numeroPedidoString;
                facturaDTO.usuario_nombre = documentoventaTmp.usuario.nombre;
                facturaDTO.fechaEmision = documentoventaTmp.fechaEmision;
                facturaDTO.cliente_razonSocial = documentoventaTmp.cliente.razonSocial;
                facturaDTO.cliente_ruc = documentoventaTmp.cliente.ruc;
                facturaDTO.ciudad_nombre = documentoventaTmp.ciudad.nombre;
                facturaDTO.total = documentoventaTmp.total;
                facturaDTO.estadoDocumentoSunatString = documentoventaTmp.estadoDocumentoSunatString;
                facturaDTO.comentarioSolicitudAnulacion = documentoventaTmp.comentarioAprobacionAnulacion;
                facturaDTO.usuario_apruebaAnulaciones = documentoventaTmp.usuario.apruebaAnulaciones;
                facturaDTO.solicitadoAnulacion = documentoventaTmp.solicitadoAnulacion;
                facturaDTO.estadoDocumentoSunat = documentoventaTmp.estadoDocumentoSunat;
                facturaDTO.notaIngreso = documentoventaTmp.notaIngreso;
                facturaDTO.guiaRemision = documentoventaTmp.guiaRemision;
                facturaDTO.usuario_creaNotasCredito = documentoventaTmp.usuario.creaNotasCredito;
                //facturaDTO.guiaRemision_serieNumeroGuia = documentoventaTmp.guiaRemision.serieNumeroGuia;
                //facturaDTO.notaIngreso_serieNumeroNotaIngreso = documentoventaTmp.notaIngreso.serieNumeroNotaIngreso;
                facturaDTOList.Add(facturaDTO);
            }
            return facturaDTOList;
        }

        public static List<GuiaRemisionDTO> GuiaRemisionToGuiaRemisionDTO(List<GuiaRemision> guiaRemisionList)
        {
            List<GuiaRemisionDTO> guiaremisionDTOList = new List<GuiaRemisionDTO>();
            foreach (GuiaRemision guiaremisionTmp in guiaRemisionList)
            {
                GuiaRemisionDTO guiaremisionDTO = new GuiaRemisionDTO();


                guiaremisionDTO.idMovimientoAlmacen = guiaremisionTmp.idMovimientoAlmacen;
                guiaremisionDTO.numeroDocumento = guiaremisionTmp.numeroDocumento;
                guiaremisionDTO.motivoTrasladoString = guiaremisionTmp.motivoTrasladoString;
                guiaremisionDTO.serieNumeroGuia = guiaremisionTmp.serieNumeroGuia;
                guiaremisionDTO.pedido_numeroPedidoString = guiaremisionTmp.pedido.numeroPedidoString;
                guiaremisionDTO.usuario_nombre = guiaremisionTmp.usuario.nombre;
                guiaremisionDTO.fechaEmision = guiaremisionTmp.fechaEmision;
                guiaremisionDTO.estaNoEntregado = guiaremisionTmp.estaNoEntregado;
                guiaremisionDTO.fechaTraslado = guiaremisionTmp.fechaTraslado;
                guiaremisionDTO.estaAnulado = guiaremisionTmp.estaAnulado;

                guiaremisionDTO.estaFacturado = guiaremisionTmp.estaFacturado;

                guiaremisionDTO.estadoDescripcion = guiaremisionTmp.estadoDescripcion;
                guiaremisionDTO.tipoExtornoToString = guiaremisionTmp.tipoExtornoToString;
                guiaremisionDTO.pedido_cliente_ruc = guiaremisionTmp.pedido.cliente.ruc;
                guiaremisionDTO.pedido_cliente_razonSocial = guiaremisionTmp.pedido.cliente.razonSocial;
                guiaremisionDTO.ciudadOrigen_nombre = guiaremisionTmp.ciudadOrigen.nombre;

                guiaremisionDTOList.Add(guiaremisionDTO);
            }

            return guiaremisionDTOList;
        }

        public static List<NotaIngresoDTO> NotaIngresoToNotaIngresoDTO(List<NotaIngreso> notaIngresoList)
        {

            List<NotaIngresoDTO> notaingresoDTOList = new List<NotaIngresoDTO>();

            foreach (NotaIngreso notaingresoTmp in notaIngresoList)
            {
                NotaIngresoDTO notaingresoDTO = new NotaIngresoDTO();


                notaingresoDTO.idMovimientoAlmacen = notaingresoTmp.idMovimientoAlmacen;
                notaingresoDTO.serieNumeroNotaIngreso = notaingresoTmp.serieNumeroNotaIngreso;
                notaingresoDTO.pedido_numeroPedidoString = notaingresoTmp.pedido.numeroPedidoString;
                notaingresoDTO.motivoTrasladoString = notaingresoTmp.motivoTrasladoString;
                notaingresoDTO.usuario_nombre = notaingresoTmp.usuario.nombre;
                notaingresoDTO.fechaEmision = notaingresoTmp.fechaEmision;
                notaingresoDTO.fechaTraslado = notaingresoTmp.fechaTraslado;
                notaingresoDTO.pedido_cliente_razonSocial = notaingresoTmp.pedido.cliente.razonSocial;
                notaingresoDTO.pedido_cliente_ruc = notaingresoTmp.pedido.cliente.ruc;
                notaingresoDTO.ciudadDestino_nombre = notaingresoTmp.ciudadDestino.nombre;
                notaingresoDTO.tipoExtornoToString = notaingresoTmp.tipoExtornoToString;
                notaingresoDTO.estadoDescripcion = notaingresoTmp.estadoDescripcion;
                notaingresoDTO.numeroDocumento = notaingresoTmp.numeroDocumento;
                notaingresoDTO.estaAnulado = notaingresoTmp.estaAnulado;
                notaingresoDTO.estaFacturado = notaingresoTmp.estaFacturado;

                notaingresoDTOList.Add(notaingresoDTO);


            }
            return notaingresoDTOList;
        }

        public static List<PedidoAlmacenDTO> PedidoAlmacenToPedidoAlmecenDTO(List<Pedido> pedidoList)
        {
            List<PedidoAlmacenDTO> pedidoalmacenDTOList = new List<PedidoAlmacenDTO>();
            foreach (Pedido pedidoalmacenTmp in pedidoList)
            {

                PedidoAlmacenDTO pedidoalmacenDTO = new PedidoAlmacenDTO();

                pedidoalmacenDTO.idPedido = pedidoalmacenTmp.idPedido;
                pedidoalmacenDTO.numeroPedidoString = pedidoalmacenTmp.numeroPedidoString;
                pedidoalmacenDTO.ciudad_nombre = pedidoalmacenTmp.ciudad.nombre;
                pedidoalmacenDTO.cliente_codigo = pedidoalmacenTmp.cliente.codigo;
                pedidoalmacenDTO.cliente_razonSocial = pedidoalmacenTmp.cliente.razonSocial;
                pedidoalmacenDTO.numeroReferenciaCliente = pedidoalmacenTmp.numeroReferenciaCliente;
                pedidoalmacenDTO.usuario_nombre = pedidoalmacenTmp.usuario.nombre;
                pedidoalmacenDTO.fechaHoraRegistro = pedidoalmacenTmp.fechaHoraRegistro;
                pedidoalmacenDTO.rangoFechasEntrega = pedidoalmacenTmp.rangoFechasEntrega;
                pedidoalmacenDTO.montoTotal = pedidoalmacenTmp.montoTotal;
                pedidoalmacenDTO.ubigeoEntrega_Distrito = pedidoalmacenTmp.ubigeoEntrega.Distrito;
                pedidoalmacenDTO.seguimientoPedido_estadoString = pedidoalmacenTmp.seguimientoPedido.estadoString;
                pedidoalmacenDTO.seguimientoCrediticioPedido_estadoString = pedidoalmacenTmp.seguimientoCrediticioPedido.estadoString;
                pedidoalmacenDTO.numeroPedido = pedidoalmacenTmp.numeroPedido;
                pedidoalmacenDTO.observaciones = pedidoalmacenTmp.observaciones;
                pedidoalmacenDTO.fechaProgramacion = pedidoalmacenTmp.fechaProgramacion;
                pedidoalmacenDTO.stockConfirmado = pedidoalmacenTmp.stockConfirmado;
                pedidoalmacenDTO.numeroPedido = pedidoalmacenTmp.numeroPedido;
                pedidoalmacenDTOList.Add(pedidoalmacenDTO);
            }
            return pedidoalmacenDTOList;
        }


        public static List<PedidoCompraDTO> PedidoCompraToPedidoCompraDTO(List<Pedido> pedidoList)
        {
            List<PedidoCompraDTO> pedidoalmacenDTOList = new List<PedidoCompraDTO>();
            foreach (Pedido pedidoalmacenTmp in pedidoList)
            {
                PedidoCompraDTO pedidocompraDTO = new PedidoCompraDTO();

                pedidocompraDTO.idPedido = pedidoalmacenTmp.idPedido;
                pedidocompraDTO.numeroPedidoString = pedidoalmacenTmp.numeroPedidoString;
                pedidocompraDTO.ciudad_nombre = pedidoalmacenTmp.ciudad.nombre;
                pedidocompraDTO.cliente_codigo = pedidoalmacenTmp.cliente.codigo;
                pedidocompraDTO.cliente_razonSocial = pedidoalmacenTmp.cliente.razonSocial;
                pedidocompraDTO.numeroReferenciaCliente = pedidoalmacenTmp.numeroReferenciaCliente;
                pedidocompraDTO.usuario_nombre = pedidoalmacenTmp.usuario.nombre;
                pedidocompraDTO.fechaHoraRegistro = pedidoalmacenTmp.fechaHoraRegistro;
                pedidocompraDTO.rangoFechasEntrega = pedidoalmacenTmp.rangoFechasEntrega;
                pedidocompraDTO.montoTotal = pedidoalmacenTmp.montoTotal;
                pedidocompraDTO.ubigeoEntrega_Distrito = pedidoalmacenTmp.ubigeoEntrega.Distrito;
                pedidocompraDTO.seguimientoPedido_estadoString = pedidoalmacenTmp.seguimientoPedido.estadoString;
                pedidocompraDTO.seguimientoCrediticioPedido_estadoString = pedidoalmacenTmp.seguimientoCrediticioPedido.estadoString;
                pedidocompraDTO.numeroPedido = pedidoalmacenTmp.numeroPedido;
                pedidocompraDTO.observaciones = pedidoalmacenTmp.observaciones;
                pedidocompraDTO.fechaProgramacion = pedidoalmacenTmp.fechaProgramacion;
                pedidoalmacenDTOList.Add(pedidocompraDTO);
            }
            return pedidoalmacenDTOList;
        }
    public static List<PedidoDTO> PedidoVentaToPedidoVentaDTO(List<Pedido> pedidoList)
        {

            List<PedidoDTO> pedidoDTOList = new List<PedidoDTO>();
            foreach (Pedido pedidoTmp in pedidoList)
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
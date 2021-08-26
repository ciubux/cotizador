using Cotizador.Models.DTOshow;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cotizador.Models.DTOsShow
{
    public class ParserDTOsShow
    {
        public static ClienteDTOshow ClienteToClienteDTO(Cliente cliente)
        {

            ClienteDTOshow clienteDTOshow = new ClienteDTOshow();
            clienteDTOshow.ciudad_nombre = cliente.ciudad.nombre;
            clienteDTOshow.codigo = cliente.codigo;
            clienteDTOshow.ruc = cliente.ruc;
            clienteDTOshow.tipoDocumentoIdentidadToString = cliente.tipoDocumentoIdentidadToString;
            clienteDTOshow.nombreComercial = cliente.nombreComercial;
            clienteDTOshow.contacto1 = cliente.contacto1;
            clienteDTOshow.correoEnvioFactura = cliente.correoEnvioFactura;
            clienteDTOshow.bloqueado = false; // cliente.bloqueado;
            clienteDTOshow.plazoCreditoSolicitadoToString = cliente.plazoCreditoSolicitadoToString;
            clienteDTOshow.tipoPagoFacturaToString = cliente.tipoPagoFacturaToString;
            clienteDTOshow.observaciones = cliente.observaciones;
            clienteDTOshow.observacionesCredito = cliente.observacionesCredito;
            clienteDTOshow.creditoSolicitado = cliente.creditoSolicitado;
            clienteDTOshow.creditoAprobado = cliente.creditoAprobado;
            clienteDTOshow.formaPagoFacturaToString = cliente.formaPagoFacturaToString;
            clienteDTOshow.razonSocialSunat = cliente.razonSocialSunat;
            clienteDTOshow.nombreComercialSunat = cliente.nombreComercialSunat;
            clienteDTOshow.direccionDomicilioLegalSunat = cliente.direccionDomicilioLegalSunat;
            clienteDTOshow.estadoContribuyente = cliente.estadoContribuyente;
            clienteDTOshow.condicionContribuyente = cliente.condicionContribuyente;
            clienteDTOshow.responsableComercial_descripcion = cliente.responsableComercial.descripcion;
            clienteDTOshow.supervisorComercial_descripcion = cliente.supervisorComercial.descripcion;
            clienteDTOshow.asistenteServicioCliente_descripcion = cliente.asistenteServicioCliente.descripcion;
            clienteDTOshow.grupoCliente_nombre = cliente.grupoCliente.nombre;
            clienteDTOshow.perteneceCanalMultiregional = cliente.perteneceCanalMultiregional;
            clienteDTOshow.perteneceCanalLima = cliente.perteneceCanalLima;
            clienteDTOshow.perteneceCanalProvincias = cliente.perteneceCanalProvincias;
            clienteDTOshow.perteneceCanalPCP = cliente.perteneceCanalPCP;
            clienteDTOshow.esSubDistribuidor = cliente.esSubDistribuidor;
            clienteDTOshow.negociacionMultiregional = cliente.negociacionMultiregional;
            clienteDTOshow.sedePrincipal= cliente.sedePrincipal;
            clienteDTOshow.horaInicioPrimerTurnoEntregaFormat = cliente.horaInicioPrimerTurnoEntregaFormat;
            clienteDTOshow.horaFinPrimerTurnoEntregaFormat = cliente.horaFinPrimerTurnoEntregaFormat;
            clienteDTOshow.horaInicioSegundoTurnoEntregaFormat = cliente.horaInicioSegundoTurnoEntregaFormat;
            clienteDTOshow.horaFinSegundoTurnoEntregaFormat = cliente.horaFinSegundoTurnoEntregaFormat;
            return clienteDTOshow;
             
        }
        public static CotizacionDTOshow CotizaciontoCotizacionDTO(Cotizacion cotizacion)
        {
            CotizacionDTOshow cotizacionDTOshow = new CotizacionDTOshow();
            cotizacionDTOshow.textoCondicionesPago = cotizacion.ciudad.nombre;
            cotizacionDTOshow.idCotizacion = cotizacion.idCotizacion;
            cotizacionDTOshow.cliente_idCliente = cotizacion.cliente.idCliente;
            cotizacionDTOshow.codigo = cotizacion.codigo;
            cotizacionDTOshow.codigoAntecedente = cotizacion.codigoAntecedente;
            cotizacionDTOshow.ciudad_nombre = cotizacion.ciudad.nombre;
            cotizacionDTOshow.ciudad_idCiudad = cotizacion.ciudad.idCiudad;
            cotizacionDTOshow.cliente_razonSocial = cotizacion.cliente.razonSocial;//no se muestra
            
            cotizacionDTOshow.monedaCodigo = cotizacion.moneda.codigo;
            cotizacionDTOshow.monedaSimbolo = cotizacion.moneda.simbolo;
            cotizacionDTOshow.monedaNombre = cotizacion.moneda.nombre;

            cotizacionDTOshow.fecha = cotizacion.fecha;
            cotizacionDTOshow.fechaLimiteValidezOferta = cotizacion.fechaLimiteValidezOferta;
            cotizacionDTOshow.fechaInicioVigenciaPrecios = cotizacion.fechaInicioVigenciaPrecios;
            cotizacionDTOshow.fechaFinVigenciaPrecios = cotizacion.fechaFinVigenciaPrecios;
            cotizacionDTOshow.textoCondicionesPago = cotizacion.textoCondicionesPago;
            cotizacionDTOshow.seguimientoCotizacion_estadoString = cotizacion.seguimientoCotizacion.estadoString;
            cotizacionDTOshow.seguimientoCotizacion_usuario_nombre = cotizacion.seguimientoCotizacion.usuario.nombre;
            cotizacionDTOshow.seguimientoCotizacion_observacion = cotizacion.seguimientoCotizacion.observacion;
            cotizacionDTOshow.considerarCantidades = cotizacion.considerarCantidades.ToString();//preguntar a cesar
            cotizacionDTOshow.observaciones = cotizacion.observaciones;
            cotizacionDTOshow.aplicaSedes = cotizacion.aplicaSedes;
            cotizacionDTOshow.cliente_sedeListWebString = cotizacion.cliente.sedeListWebString;
            cotizacionDTOshow.montoSubTotal = cotizacion.montoSubTotal;
            cotizacionDTOshow.montoIGV = cotizacion.montoIGV;
            cotizacionDTOshow.minimoMargen = cotizacion.minimoMargen;
            cotizacionDTOshow.cotizacionDetalleList = cotizacion.cotizacionDetalleList;
            cotizacionDTOshow.seguimientoCotizacion_usuario_idUsuario = cotizacion.seguimientoCotizacion.usuario.idUsuario;
            cotizacionDTOshow.montoTotal = cotizacion.montoTotal;
            cotizacionDTOshow.seguimientoCotizacion_estado = cotizacion.seguimientoCotizacion.estado;
            cotizacionDTOshow.maximoPorcentajeDescuentoPermitido = cotizacion.maximoPorcentajeDescuentoPermitido;
            cotizacionDTOshow.grupo_codigoNombre = cotizacion.grupo.codigoNombre;
            cotizacionDTOshow.cliente_codigoRazonSocial = cotizacion.cliente.codigoRazonSocial;

            if (cotizacion.cliente.grupoCliente != null && cotizacion.cliente.grupoCliente.idGrupoCliente > 0)
            {
                cotizacionDTOshow.cliente_codigoRazonSocial = cotizacionDTOshow.cliente_codigoRazonSocial + " (Grupo: " + cotizacion.cliente.grupoCliente.codigoNombre + ")";
            }

            cotizacionDTOshow.tipoCotizacion = (int)cotizacion.tipoCotizacion;
            cotizacionDTOshow.contacto = cotizacion.contacto;
            cotizacionDTOshow.cliente_esClienteLite = cotizacion.cliente.esClienteLite;
            cotizacionDTOshow.grupo_idGrupoCliente = cotizacion.grupo.idGrupoCliente;
            return cotizacionDTOshow;
        }

        public static FacturaDTOshow FacturatoFacturaDTO(DocumentoVenta documentoVenta)
        {
            FacturaDTOshow facturaDTOshow = new FacturaDTOshow();
            facturaDTOshow.solicitadoAnulacion = documentoVenta.solicitadoAnulacion;
            facturaDTOshow.estadoDocumentoSunat = documentoVenta.estadoDocumentoSunat;
            facturaDTOshow.tipoDocumento = documentoVenta.tipoDocumento;
            facturaDTOshow.tipoNotaCreditoString = documentoVenta.tipoNotaCreditoString;
            facturaDTOshow.tipoNotaDebitoString = documentoVenta.tipoNotaDebitoString;
            facturaDTOshow.cPE_CABECERA_BE_DES_MTVO_NC_ND = documentoVenta.cPE_CABECERA_BE.DES_MTVO_NC_ND;
            facturaDTOshow.idDocumentoVenta = documentoVenta.idDocumentoVenta;
            facturaDTOshow.cPE_CABECERA_BE_FEC_EMI = documentoVenta.cPE_CABECERA_BE.FEC_EMI;
            facturaDTOshow.cPE_CABECERA_BE_HOR_EMI = documentoVenta.cPE_CABECERA_BE.HOR_EMI;
            facturaDTOshow.cPE_CABECERA_BE_CORRELATIVO = documentoVenta.cPE_CABECERA_BE.CORRELATIVO;
            facturaDTOshow.cPE_CABECERA_BE_DIR_DES_RCT = documentoVenta.cPE_CABECERA_BE.DIR_DES_RCT;
            facturaDTOshow.cPE_CABECERA_BE_NRO_ORD_COM = documentoVenta.cPE_CABECERA_BE.NRO_ORD_COM;
            facturaDTOshow.cPE_CABECERA_BE_NRO_DOC_RCT = documentoVenta.cPE_CABECERA_BE.NRO_DOC_RCT;
            facturaDTOshow.cPE_CABECERA_BE_NRO_GRE = documentoVenta.cPE_CABECERA_BE.NRO_GRE;
            facturaDTOshow.cPE_CABECERA_BE_CORREO_ENVIO = documentoVenta.cPE_CABECERA_BE.CORREO_ENVIO;
            facturaDTOshow.tipoPagoString = documentoVenta.tipoPagoString;
            facturaDTOshow.cPE_CABECERA_BE_FEC_VCTO = documentoVenta.cPE_CABECERA_BE.FEC_VCTO;
            facturaDTOshow.cPE_DAT_ADIC_BEList = documentoVenta.cPE_DAT_ADIC_BEList;
            facturaDTOshow.cPE_CABECERA_BE_MNT_TOT_GRV = documentoVenta.cPE_CABECERA_BE.MNT_TOT_GRV;
            facturaDTOshow.cPE_DOC_REF_BEList = documentoVenta.cPE_DOC_REF_BEList;
            facturaDTOshow.cPE_CABECERA_BE_FEC_EMI = documentoVenta.cPE_CABECERA_BE.FEC_EMI;
            facturaDTOshow.cPE_CABECERA_BE_SERIE = documentoVenta.cPE_CABECERA_BE.SERIE;
            facturaDTOshow.cPE_CABECERA_BE_NOM_RCT = documentoVenta.cPE_CABECERA_BE.NOM_RCT;
            facturaDTOshow.cPE_CABECERA_BE_MNT_TOT_INF = documentoVenta.cPE_CABECERA_BE.MNT_TOT_INF;
            facturaDTOshow.cPE_CABECERA_BE_MNT_TOT_EXR = documentoVenta.cPE_CABECERA_BE.MNT_TOT_EXR;
            facturaDTOshow.cPE_CABECERA_BE_MNT_TOT_GRT = documentoVenta.cPE_CABECERA_BE.MNT_TOT_GRT;
            facturaDTOshow.cPE_CABECERA_BE_MNT_TOT_GRV_NAC = documentoVenta.cPE_CABECERA_BE.MNT_TOT_GRV_NAC;
            facturaDTOshow.cPE_CABECERA_BE_MNT_TOT_VAL_VTA = documentoVenta.cPE_CABECERA_BE.MNT_TOT_VAL_VTA;
            facturaDTOshow.cPE_CABECERA_BE_MNT_TOT_TRB_IGV = documentoVenta.cPE_CABECERA_BE.MNT_TOT_TRB_IGV;
            facturaDTOshow.cPE_CABECERA_BE_MNT_TOT_PRC_VTA = documentoVenta.cPE_CABECERA_BE.MNT_TOT_PRC_VTA;
            facturaDTOshow.cPE_DETALLE_BEList = documentoVenta.cPE_DETALLE_BEList;
                   

            return facturaDTOshow;
        }
        public static GuiaRemisionDTOshow GuiaRemisionToGuiaRemisionDTO(GuiaRemision guiaRemision)
        {

            GuiaRemisionDTOshow guiaRemisionDTOshow = new GuiaRemisionDTOshow();
            guiaRemisionDTOshow.pedido_idPedido = guiaRemision.pedido.idPedido;
            guiaRemisionDTOshow.idMovimientoAlmacen = guiaRemision.idMovimientoAlmacen;
            guiaRemisionDTOshow.ciudadOrigen_nombre = guiaRemision.ciudadOrigen.nombre;
            guiaRemisionDTOshow.ciudadOrigen_direccionPuntoPartida = guiaRemision.ciudadOrigen.direccionPuntoPartida;
            guiaRemisionDTOshow.fechaTraslado = guiaRemision.fechaTraslado;
            guiaRemisionDTOshow.fechaEmision = guiaRemision.fechaEmision;
            guiaRemisionDTOshow.serieNumeroGuia = guiaRemision.serieNumeroGuia;
            guiaRemisionDTOshow.pedido_numeroPedidoString = guiaRemision.pedido.numeroPedidoString;
            guiaRemisionDTOshow.pedido_cliente_razonSocial = guiaRemision.pedido.cliente.razonSocial;
            guiaRemisionDTOshow.pedido_numeroReferenciaCliente = guiaRemision.pedido.numeroReferenciaCliente;
            guiaRemisionDTOshow.pedido_cliente_razonSocial = guiaRemision.pedido.cliente.razonSocial;
            guiaRemisionDTOshow.pedido_cliente_configuraciones = guiaRemision.pedido.cliente.configuraciones;
            guiaRemisionDTOshow.pedido_numeroReferenciaCliente = guiaRemision.pedido.numeroReferenciaCliente;
            guiaRemisionDTOshow.motivoTrasladoString = guiaRemision.motivoTrasladoString;
            guiaRemisionDTOshow.atencionParcial = guiaRemision.atencionParcial;
            guiaRemisionDTOshow.pedido_ubigeoEntrega = guiaRemision.pedido.ubigeoEntrega;
            guiaRemisionDTOshow.pedido_direccionEntrega_descripcion = guiaRemision.pedido.direccionEntrega.descripcion;
            guiaRemisionDTOshow.transportista_descripcion = guiaRemision.transportista.descripcion;
            guiaRemisionDTOshow.transportista_ruc = guiaRemision.transportista.ruc;
            guiaRemisionDTOshow.transportista_brevete = guiaRemision.transportista.brevete;
            guiaRemisionDTOshow.transportista_direccion = guiaRemision.transportista.direccion;
            guiaRemisionDTOshow.placaVehiculo = guiaRemision.placaVehiculo;
            guiaRemisionDTOshow.observaciones = guiaRemision.observaciones;
            guiaRemisionDTOshow.estaAnulado = guiaRemision.estaAnulado;
            guiaRemisionDTOshow.estadoDescripcion = guiaRemision.estadoDescripcion;
            guiaRemisionDTOshow.tipoExtorno = guiaRemision.tipoExtorno;
            guiaRemisionDTOshow.tipoExtornoToString = guiaRemision.tipoExtornoToString;
            guiaRemisionDTOshow.notaIngresoAExtornar = guiaRemision.notaIngresoAExtornar;
            //guiaRemisionDTOshow.notaIngresoAExtornar_serieNumeroNotaIngreso = guiaRemision.notaIngresoAExtornar.serieNumeroNotaIngreso;
            guiaRemisionDTOshow.motivoExtornoNotaIngresoToString = guiaRemision.motivoExtornoNotaIngresoToString;
            guiaRemisionDTOshow.sustentoExtorno = guiaRemision.sustentoExtorno;
            guiaRemisionDTOshow.estaFacturado = guiaRemision.estaFacturado;
            guiaRemisionDTOshow.transaccionList = guiaRemision.transaccionList;
            guiaRemisionDTOshow.motivoTraslado = guiaRemision.motivoTraslado;
            guiaRemisionDTOshow.ingresado = guiaRemision.ingresado;
            guiaRemisionDTOshow.documentoDetalle = guiaRemision.documentoDetalle;
            guiaRemisionDTOshow.esGuiaDiferida = guiaRemision.esGuiaDiferida;
            return guiaRemisionDTOshow;
        }

        public static NotaIngresoDTOshow NotaIngresoToNotaIngreso(NotaIngreso notaIngreso)
        {
            NotaIngresoDTOshow notaingresoDTOshow = new NotaIngresoDTOshow();
            notaingresoDTOshow.pedido_idPedido = notaIngreso.pedido.idPedido;
            notaingresoDTOshow.idMovimientoAlmacen = notaIngreso.idMovimientoAlmacen;
            notaingresoDTOshow.ciudadDestino_nombre = notaIngreso.ciudadDestino.nombre;
            notaingresoDTOshow.ciudadDestino_direccionPuntoLlegada = notaIngreso.ciudadDestino.direccionPuntoLlegada;
            notaingresoDTOshow.fechaTraslado = notaIngreso.fechaTraslado;
            notaingresoDTOshow.fechaEmision = notaIngreso.fechaEmision;
            notaingresoDTOshow.serieNumeroNotaIngreso = notaIngreso.serieNumeroNotaIngreso;
            notaingresoDTOshow.pedido_numeroPedidoString = notaIngreso.pedido.numeroPedidoString;
            notaingresoDTOshow.pedido_cliente_razonSocial = notaIngreso.pedido.cliente.razonSocial;
            notaingresoDTOshow.pedido_numeroReferenciaCliente = notaIngreso.pedido.numeroReferenciaCliente;
            notaingresoDTOshow.motivoTrasladoString = notaIngreso.motivoTrasladoString;
            notaingresoDTOshow.atencionParcial = notaIngreso.atencionParcial;
            notaingresoDTOshow.observaciones = notaIngreso.observaciones;
            notaingresoDTOshow.estadoDescripcion = notaIngreso.estadoDescripcion;
            notaingresoDTOshow.estaAnulado = notaIngreso.estaAnulado;
            notaingresoDTOshow.tipoExtorno = notaIngreso.tipoExtorno;
            notaingresoDTOshow.tipoExtornoToString = notaIngreso.tipoExtornoToString;
            notaingresoDTOshow.guiaRemisionAExtornar = notaIngreso.guiaRemisionAExtornar;
            notaingresoDTOshow.guiaRemisionAIngresar = notaIngreso.guiaRemisionAIngresar;
            notaingresoDTOshow.serieGuiaReferencia = notaIngreso.serieGuiaReferencia;
            notaingresoDTOshow.numeroGuiaReferencia = notaIngreso.numeroGuiaReferencia;
            notaingresoDTOshow.serieDocumentoVentaReferencia = notaIngreso.serieDocumentoVentaReferencia;
            notaingresoDTOshow.numeroDocumentoVentaReferencia = notaIngreso.numeroDocumentoVentaReferencia;
            notaingresoDTOshow.tipoDocumentoVentaReferenciaString = notaIngreso.tipoDocumentoVentaReferenciaString;
            // notaingresoDTOshow.guiaRemisionAIngresar_serieNumeroGuia = notaIngreso.guiaRemisionAIngresar.serieNumeroGuia;
            // notaingresoDTOshow.guiaRemisionAExtornar_serieNumeroGuia = notaIngreso.guiaRemisionAExtornar.serieNumeroGuia;
            notaingresoDTOshow.motivoExtornoGuiaRemisionToString = notaIngreso.motivoExtornoGuiaRemisionToString;
            notaingresoDTOshow.sustentoExtorno = notaIngreso.sustentoExtorno;
            notaingresoDTOshow.usuario_nombre = notaIngreso.usuario.nombre;
            notaingresoDTOshow.estaFacturado = notaIngreso.estaFacturado;
            notaingresoDTOshow.documentoDetalle = notaIngreso.documentoDetalle;
            notaingresoDTOshow.motivoTraslado = notaIngreso.motivoTraslado;
            return notaingresoDTOshow;

        }

        public static PedidoAlmacenDTOshow PedidoAlmacenToPedidoAlmacen(Pedido pedido)
        {
            PedidoAlmacenDTOshow pedidoalmacenDTOshow = new PedidoAlmacenDTOshow();
            pedidoalmacenDTOshow.fechaEntregaDesde = pedido.fechaEntregaDesde;
            pedidoalmacenDTOshow.fechaEntregaHasta = pedido.fechaEntregaHasta;
            pedidoalmacenDTOshow.idPedido = pedido.idPedido;
            pedidoalmacenDTOshow.numeroPedidoString = pedido.numeroPedidoString;
            pedidoalmacenDTOshow.numeroGrupoPedidoString = pedido.numeroGrupoPedidoString;
            pedidoalmacenDTOshow.cotizacion_numeroCotizacionString = pedido.cotizacion.numeroCotizacionString;
            pedidoalmacenDTOshow.tiposPedidoAlmacenString = pedido.tiposPedidoAlmacenString;
            pedidoalmacenDTOshow.cliente_responsableComercial_codigoDescripcion = pedido.cliente.responsableComercial.codigoDescripcion;
            pedidoalmacenDTOshow.cliente_supervisorComercial_codigoDescripcion = pedido.cliente.supervisorComercial.codigoDescripcion;
            pedidoalmacenDTOshow.cliente_responsableComercial_usuario_email = pedido.cliente.responsableComercial.usuario.email;
            pedidoalmacenDTOshow.cliente_supervisorComercial_usuario_email = pedido.cliente.supervisorComercial.usuario.email;
            pedidoalmacenDTOshow.cliente_asistenteServicioCliente_codigoDescripcion = pedido.cliente.asistenteServicioCliente.codigoDescripcion;
            pedidoalmacenDTOshow.cliente_asistenteServicioCliente_usuario_email = pedido.cliente.asistenteServicioCliente.usuario.email;
            pedidoalmacenDTOshow.cliente_ciudad_nombre = pedido.cliente.ciudad.nombre;
            pedidoalmacenDTOshow.tipoPedidoAlmacen = pedido.tipoPedidoAlmacen;
            pedidoalmacenDTOshow.fechaHorarioEntrega = pedido.fechaHorarioEntrega;
            pedidoalmacenDTOshow.ciudad_nombre = pedido.ciudad.nombre;
            pedidoalmacenDTOshow.cliente_idCliente = pedido.cliente.idCliente;
            pedidoalmacenDTOshow.cliente_codigoRazonSocial = pedido.cliente.codigoRazonSocial;
            pedidoalmacenDTOshow.numeroReferenciaCliente = pedido.numeroReferenciaCliente;
            pedidoalmacenDTOshow.numeroReferenciaAdicional = pedido.numeroReferenciaAdicional;
            pedidoalmacenDTOshow.direccionEntrega_descripcion = pedido.direccionEntrega.descripcion;
            pedidoalmacenDTOshow.direccionEntrega_telefono = pedido.direccionEntrega.telefono;
            pedidoalmacenDTOshow.direccionEntrega_contacto = pedido.direccionEntrega.contacto;
            pedidoalmacenDTOshow.usuario_nombre = pedido.usuario.nombre;
            pedidoalmacenDTOshow.fechaHoraRegistro = pedido.fechaHoraRegistro;
            pedidoalmacenDTOshow.ubigeoEntrega = pedido.ubigeoEntrega;
            pedidoalmacenDTOshow.contactoPedido = pedido.contactoPedido;
            pedidoalmacenDTOshow.telefonoCorreoContactoPedido = pedido.telefonoCorreoContactoPedido;
            pedidoalmacenDTOshow.fechaHoraSolicitud = pedido.fechaHoraSolicitud;
            pedidoalmacenDTOshow.seguimientoPedido_estadoString = pedido.seguimientoPedido.estadoString;
            pedidoalmacenDTOshow.seguimientoPedido_usuario_nombre = pedido.seguimientoPedido.usuario.nombre;
            pedidoalmacenDTOshow.seguimientoPedido_observacion = pedido.seguimientoPedido.observacion;
            pedidoalmacenDTOshow.seguimientoCrediticioPedido_estadoString = pedido.seguimientoCrediticioPedido.estadoString;
            pedidoalmacenDTOshow.seguimientoCrediticioPedido_usuario_nombre = pedido.seguimientoCrediticioPedido.usuario.nombre;
            pedidoalmacenDTOshow.seguimientoCrediticioPedido_observacion = pedido.seguimientoCrediticioPedido.observacion;
            pedidoalmacenDTOshow.observaciones = pedido.observaciones;
            pedidoalmacenDTOshow.observacionesFactura = pedido.observacionesFactura;
            pedidoalmacenDTOshow.observacionesGuiaRemision = pedido.observacionesGuiaRemision;
            pedidoalmacenDTOshow.montoSubTotal = pedido.montoSubTotal;
            pedidoalmacenDTOshow.montoIGV = pedido.montoIGV;
            pedidoalmacenDTOshow.montoTotal = pedido.montoTotal;
            pedidoalmacenDTOshow.pedidoAdjuntoList = pedido.pedidoAdjuntoList;
            pedidoalmacenDTOshow.cliente_razonSocialSunat = pedido.cliente.razonSocialSunat;
            pedidoalmacenDTOshow.cliente_ruc = pedido.cliente.ruc;
            pedidoalmacenDTOshow.cliente_direccionDomicilioLegalSunat = pedido.cliente.direccionDomicilioLegalSunat;
            pedidoalmacenDTOshow.cliente_codigo = pedido.cliente.codigo;
            pedidoalmacenDTOshow.cliente_correoEnvioFactura = pedido.cliente.correoEnvioFactura;
            pedidoalmacenDTOshow.seguimientoPedido_estado = pedido.seguimientoPedido.estado;
            pedidoalmacenDTOshow.seguimientoCrediticioPedido_estado = pedido.seguimientoCrediticioPedido.estado;
            pedidoalmacenDTOshow.documentoVenta_numero = pedido.documentoVenta.numero;
            pedidoalmacenDTOshow.pedidoDetalleList = pedido.pedidoDetalleList;
            pedidoalmacenDTOshow.guiaRemisionList = pedido.guiaRemisionList;

            return pedidoalmacenDTOshow;
        }
        public static PedidoCompraDTOshow PedidoCompraToPedidoCompraDTO(Pedido pedido)
        {
            PedidoCompraDTOshow pedidocompraDTOshow = new PedidoCompraDTOshow();
            pedidocompraDTOshow.fechaEntregaDesde = pedido.fechaEntregaDesde;
            pedidocompraDTOshow.fechaEntregaHasta = pedido.fechaEntregaHasta;
            pedidocompraDTOshow.idPedido = pedido.idPedido;
            pedidocompraDTOshow.numeroPedidoString = pedido.numeroPedidoString;
            pedidocompraDTOshow.numeroGrupoPedidoString = pedido.numeroGrupoPedidoString;
            pedidocompraDTOshow.cotizacion_numeroCotizacionString = pedido.cotizacion.numeroCotizacionString;
            pedidocompraDTOshow.tiposPedidoCompraString = pedido.tiposPedidoCompraString;
            pedidocompraDTOshow.cliente_responsableComercial_codigoDescripcion = pedido.cliente.responsableComercial.codigoDescripcion;
            pedidocompraDTOshow.cliente_responsableComercial_usuario_email = pedido.cliente.responsableComercial.usuario.email;
            pedidocompraDTOshow.cliente_supervisorComercial_codigoDescripcion = pedido.cliente.supervisorComercial.codigoDescripcion;
            pedidocompraDTOshow.cliente_supervisorComercial_usuario_email = pedido.cliente.supervisorComercial.usuario.email;
            pedidocompraDTOshow.cliente_asistenteServicioCliente_codigoDescripcion = pedido.cliente.asistenteServicioCliente.codigoDescripcion;
            pedidocompraDTOshow.cliente_asistenteServicioCliente_usuario_email = pedido.cliente.asistenteServicioCliente.usuario.email;
            pedidocompraDTOshow.fechaHorarioEntrega = pedido.fechaHorarioEntrega;
            pedidocompraDTOshow.ciudad_nombre = pedido.ciudad.nombre;
            pedidocompraDTOshow.cliente_idCliente = pedido.cliente.idCliente;
            pedidocompraDTOshow.cliente_codigoRazonSocial = pedido.cliente.codigoRazonSocial;
            pedidocompraDTOshow.numeroReferenciaCliente = pedido.numeroReferenciaCliente;
            pedidocompraDTOshow.numeroReferenciaAdicional = pedido.numeroReferenciaAdicional;
            pedidocompraDTOshow.direccionEntrega_descripcion = pedido.direccionEntrega.descripcion;
            pedidocompraDTOshow.direccionEntrega_telefono = pedido.direccionEntrega.telefono;
            pedidocompraDTOshow.direccionEntrega_contacto = pedido.direccionEntrega.contacto;
            pedidocompraDTOshow.usuario_nombre = pedido.usuario.nombre;
            pedidocompraDTOshow.fechaHoraRegistro = pedido.fechaHoraRegistro;
            pedidocompraDTOshow.ubigeoEntrega = pedido.ubigeoEntrega;
            pedidocompraDTOshow.contactoPedido = pedido.contactoPedido;
            pedidocompraDTOshow.telefonoCorreoContactoPedido = pedido.telefonoCorreoContactoPedido;
            pedidocompraDTOshow.fechaHoraSolicitud = pedido.fechaHoraSolicitud;
            pedidocompraDTOshow.seguimientoPedido_estadoString = pedido.seguimientoPedido.estadoString;
            pedidocompraDTOshow.seguimientoPedido_usuario_nombre = pedido.seguimientoPedido.usuario.nombre;
            pedidocompraDTOshow.seguimientoPedido_observacion = pedido.seguimientoPedido.observacion;
            pedidocompraDTOshow.seguimientoCrediticioPedido_estadoString = pedido.seguimientoCrediticioPedido.estadoString;
            pedidocompraDTOshow.seguimientoCrediticioPedido_usuario_nombre = pedido.seguimientoCrediticioPedido.usuario.nombre;
            pedidocompraDTOshow.seguimientoCrediticioPedido_observacion = pedido.seguimientoCrediticioPedido.observacion;
            pedidocompraDTOshow.observaciones = pedido.observaciones;
            pedidocompraDTOshow.observacionesFactura = pedido.observacionesFactura;
            pedidocompraDTOshow.observacionesGuiaRemision = pedido.observacionesGuiaRemision;
            pedidocompraDTOshow.montoSubTotal = pedido.montoSubTotal;
            pedidocompraDTOshow.montoIGV = pedido.montoIGV;
            pedidocompraDTOshow.montoTotal = pedido.montoTotal;
            pedidocompraDTOshow.pedidoAdjuntoList = pedido.pedidoAdjuntoList;
            pedidocompraDTOshow.pedidoDetalleList = pedido.pedidoDetalleList;
            pedidocompraDTOshow.cliente_razonSocialSunat = pedido.cliente.razonSocialSunat;
            pedidocompraDTOshow.cliente_ruc = pedido.cliente.ruc;
            pedidocompraDTOshow.cliente_direccionDomicilioLegalSunat = pedido.cliente.direccionDomicilioLegalSunat;
            pedidocompraDTOshow.cliente_codigo = pedido.cliente.codigo;
            pedidocompraDTOshow.cliente_correoEnvioFactura = pedido.cliente.correoEnvioFactura;
            pedidocompraDTOshow.tipoPedidoCompra = pedido.tipoPedidoCompra;
            pedidocompraDTOshow.guiaRemisionList = pedido.guiaRemisionList;
            pedidocompraDTOshow.seguimientoPedido_estado = pedido.seguimientoPedido.estado;
            pedidocompraDTOshow.seguimientoCrediticioPedido_estado = pedido.seguimientoCrediticioPedido.estado;
            pedidocompraDTOshow.documentoVenta_numero = pedido.documentoVenta.numero;

            return pedidocompraDTOshow;
        }

        public static PedidoDTOshow PedidoVentaToPedidoVentaDTO(Pedido pedido)
        {

            PedidoDTOshow pedidoDTOshow = new PedidoDTOshow();
            pedidoDTOshow.idPedido = pedido.idPedido;
            pedidoDTOshow.truncado = pedido.truncado;
            pedidoDTOshow.fechaEntregaDesde = pedido.fechaEntregaDesde;
            pedidoDTOshow.fechaEntregaHasta = pedido.fechaEntregaHasta;
            pedidoDTOshow.numeroPedidoString = pedido.numeroPedidoString;
            pedidoDTOshow.numeroGrupoPedidoString = pedido.numeroGrupoPedidoString;
            pedidoDTOshow.cotizacion_numeroCotizacionString = pedido.cotizacion.numeroCotizacionString;
            pedidoDTOshow.tiposPedidoString = pedido.tiposPedidoString;
            pedidoDTOshow.cliente_responsableComercial_codigoDescripcion = pedido.cliente.responsableComercial.codigoDescripcion;
            pedidoDTOshow.cliente_responsableComercial_usuario_email = pedido.cliente.responsableComercial.usuario.email;
            pedidoDTOshow.numeroReferenciaCliente = pedido.numeroReferenciaCliente;
            pedidoDTOshow.cliente_supervisorComercial_usuario_email = pedido.cliente.supervisorComercial.usuario.email;
            pedidoDTOshow.cliente_supervisorComercial_codigoDescripcion = pedido.cliente.supervisorComercial.codigoDescripcion;
            pedidoDTOshow.cliente_asistenteServicioCliente_codigoDescripcion = pedido.cliente.asistenteServicioCliente.codigoDescripcion;
            pedidoDTOshow.cliente_asistenteServicioCliente_usuario_email = pedido.cliente.asistenteServicioCliente.usuario.email;
            pedidoDTOshow.textoCondicionesPago = pedido.textoCondicionesPago;
            pedidoDTOshow.fechaHorarioEntrega = pedido.fechaHorarioEntrega;
            pedidoDTOshow.ciudad_nombre = pedido.ciudad.nombre;
            pedidoDTOshow.ciudad_idCiudad = pedido.ciudad.idCiudad;

            pedidoDTOshow.cliente_idCliente = pedido.cliente.idCliente;
            pedidoDTOshow.cliente_codigoRazonSocial = pedido.cliente.codigoRazonSocial;
            pedidoDTOshow.numeroReferenciaCliente = pedido.numeroReferenciaCliente;
            pedidoDTOshow.numeroReferenciaAdicional = pedido.numeroReferenciaAdicional;
            pedidoDTOshow.fechaEntregaExtendidaString = pedido.fechaEntregaExtendidaString;
            pedidoDTOshow.direccionEntrega_descripcion = pedido.direccionEntrega.descripcion;
            pedidoDTOshow.direccionEntrega_telefono = pedido.direccionEntrega.telefono;
            pedidoDTOshow.direccionEntrega_contacto = pedido.direccionEntrega.contacto;
            pedidoDTOshow.usuario_nombre = pedido.usuario.nombre;
            pedidoDTOshow.fechaHoraRegistro = pedido.fechaHoraRegistro;
            pedidoDTOshow.ubigeoEntrega = pedido.ubigeoEntrega;
            pedidoDTOshow.contactoPedido = pedido.contactoPedido;
            pedidoDTOshow.telefonoCorreoContactoPedido = pedido.telefonoCorreoContactoPedido;
            pedidoDTOshow.fechaHoraSolicitud = pedido.fechaHoraSolicitud;
            pedidoDTOshow.seguimientoPedido_estadoString = pedido.seguimientoPedido.estadoString;
            pedidoDTOshow.seguimientoPedido_usuario_nombre = pedido.seguimientoPedido.usuario.nombre;
            pedidoDTOshow.seguimientoPedido_observacion = pedido.seguimientoPedido.observacion;
            pedidoDTOshow.seguimientoCrediticioPedido_estadoString = pedido.seguimientoCrediticioPedido.estadoString;
            pedidoDTOshow.seguimientoCrediticioPedido_usuario_nombre = pedido.seguimientoCrediticioPedido.usuario.nombre;
            pedidoDTOshow.seguimientoCrediticioPedido_observacion = pedido.seguimientoCrediticioPedido.observacion;
            pedidoDTOshow.observaciones = pedido.observaciones;
            pedidoDTOshow.observacionesAlmacen = pedido.observacionesAlmacen;
            pedidoDTOshow.observacionesFactura = pedido.observacionesFactura;
            pedidoDTOshow.observacionesGuiaRemision = pedido.observacionesGuiaRemision;
            pedidoDTOshow.montoSubTotal = pedido.montoSubTotal;
            pedidoDTOshow.montoIGV = pedido.montoIGV;
            pedidoDTOshow.montoTotal = pedido.montoTotal;
            pedidoDTOshow.pedidoAdjuntoList = pedido.pedidoAdjuntoList;
            pedidoDTOshow.pedidoDetalleList = pedido.pedidoDetalleList;
            pedidoDTOshow.cliente_razonSocialSunat = pedido.cliente.razonSocialSunat;
            pedidoDTOshow.cliente_ruc = pedido.cliente.ruc;
            pedidoDTOshow.cliente_direccionDomicilioLegalSunat = pedido.cliente.direccionDomicilioLegalSunat;
            pedidoDTOshow.cliente_codigo = pedido.cliente.codigo;
            pedidoDTOshow.cliente_correoEnvioFactura = pedido.cliente.correoEnvioFactura;
            pedidoDTOshow.tipoPedido = pedido.tipoPedido;
            pedidoDTOshow.guiaRemisionList = pedido.guiaRemisionList;
            pedidoDTOshow.seguimientoPedido_estado = pedido.seguimientoPedido.estado;
            pedidoDTOshow.seguimientoCrediticioPedido_estado = pedido.seguimientoCrediticioPedido.estado;
            pedidoDTOshow.documentoVenta_numero = pedido.documentoVenta.numero;
            pedidoDTOshow.pedidoGrupoList = pedido.pedidoGrupoList;
            pedidoDTOshow.numeroGrupoPedido = pedido.numeroGrupoPedido;
            pedidoDTOshow.cotizacion_tipoCotizacion = (int)pedido.cotizacion.tipoCotizacion;
            pedidoDTOshow.numeroRequerimiento = pedido.numeroRequerimiento;
            pedidoDTOshow.grupoCliente_nombre = pedido.cliente.grupoCliente.nombre;
            return pedidoDTOshow;
        }
    }
}

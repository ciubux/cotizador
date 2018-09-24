﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class Pedido : Auditoria, IDocumento
    {
        public Pedido()
        {
            this.tipoPedido = tiposPedido.Venta;
            this.tipoPedidoCompra = tiposPedidoCompra.Compra;
            this.tipoPedidoVentaBusqueda = tiposPedidoVentaBusqueda.Todos;
            this.tipoPedidoCompraBusqueda = tiposPedidoCompraBusqueda.Todos;

            this.solicitante = new Solicitante();
            this.pedidoAdjuntoList = new List<PedidoAdjunto>();
            this.ciudadASolicitar = new Ciudad();
        }

        public Guid idPedido { get; set; }
        [Display(Name = "Número Pedido:")]
        public Int64 numeroPedido { get; set; }
        [Display(Name = "Número Grupo Pedido:")]
        public Int64? numeroGrupoPedido { get; set; }

        public Cotizacion cotizacion { get; set; }
        [Display(Name = "Sede MP:")]
        public Ciudad ciudad { get; set; }

        [Display(Name = "Solicitar A:")]
        public Ciudad ciudadASolicitar { get; set; }

        [Display(Name = "Cliente:")]
        public Cliente cliente { get; set; }

        [Display(Name = "Proveedor:")]
        public Proveedor proveedor { get; set; }

        [Display(Name = "Orden de Compra N°:")]
        public String numeroReferenciaCliente { get; set; }


        [Display(Name = "Referencia Adicional Cliente:")]
        public String numeroReferenciaAdicional { get; set; }

        [Display(Name = "Otros Cargos (Flete):")]
        public Decimal otrosCargos { get; set; }
        public DireccionEntrega direccionEntrega { get; set; }
        [Display(Name = "Solicitado por:")]
        public Solicitante solicitante { get; set; }

        [Display(Name = "Fecha de Entrega:")]
        public DateTime? fechaEntregaDesde { get; set; }

        [Display(Name = "Fecha Máxima de Entrega:")]
        public DateTime? fechaEntregaHasta { get; set; }

        [Display(Name = "Hora de Entrega:")]
        public String horaEntregaDesde { get; set; }

        [Display(Name = "Fecha Máxima de Entrega:")]
        public String horaEntregaHasta { get; set; }



        [Display(Name = "Fecha Solicitud:")]
        public DateTime fechaSolicitud { get; set; }
        [Required(ErrorMessage = "Ingrese la Fecha de Entrega.")]

        [Display(Name = "Solicitado Por:")]
        public String contactoPedido { get; set; }
        [Display(Name = "Telefono Solicitante:")]
        public String telefonoContactoPedido { get; set; }
        [Display(Name = "Correo Solicitante:")]
        public String correoContactoPedido { get; set; }

        [Display(Name = "Telefono y Correo Solicitante:")]
        public String telefonoCorreoContactoPedido
        {
            get { return "Tef: " + telefonoContactoPedido + " Correo: " + correoContactoPedido; }
        }

        [Display(Name = "Observaciones para uso interno:")]
        public String observaciones { get; set; }

        [Display(Name = "Observaciones Guía Remisión:")]
        public String observacionesGuiaRemision { get; set; }

        [Display(Name = "Observaciones Factura:")]
        public String observacionesFactura { get; set; }

        [Display(Name = "Ciudad Entrega:")]
        public Ubigeo ubigeoEntrega { get; set; }



        [Display(Name = "Fechas de Entrega:")]
        public String rangoFechasEntrega {
            get
            {
                String rangoFechasEntrega = String.Empty;
                if (this.fechaEntregaDesde != null && this.fechaEntregaHasta != null)
                {
                    String entregaDesde = this.fechaEntregaDesde.Value.ToString(Constantes.formatoFecha);
                    String entregaHasta = this.fechaEntregaHasta.Value.ToString(Constantes.formatoFecha);

                    if (entregaDesde.Equals(entregaHasta))
                    {
                        return entregaDesde;
                    }
                    else
                    {
                        return "Desde: " + entregaDesde + " Hasta: " + entregaHasta;
                    }
                }
                else
                {
                    return "";
                }

            }
        }

        [Display(Name = "Horario de Entrega:")]
        public String rangoHoraEntrega
        {
            get
            {
                return "Desde: " + this.horaEntregaDesde + " Hasta: " + this.horaEntregaHasta;
            }
        }

        [Display(Name = "Fecha y Horario Entrega:")]
        public String fechaHorarioEntrega
        {

            get {
                return rangoFechasEntrega + " " + rangoHoraEntrega;
            }
        }

        public Boolean existeCambioDireccionEntrega { get; set; }

        public Boolean existeCambioSolicitante { get; set; }


        [Display(Name = "Fecha Hora Solicitud")]
        public String fechaHoraSolicitud
        {
            get { return this.fechaSolicitud.ToString("dd/MM/yyyy HH:mm"); }
        }


        [Display(Name = "Fecha Hora de Registro:")]
        public String fechaHoraRegistro
        {
            get { return this.FechaRegistro.ToString("dd/MM/yyyy HH:mm"); }
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


        [Display(Name = "Creado Por:")]
        public String usuarioCreacion { get; set; }



        public DocumentoVenta documentoVenta { get; set; }

        [Display(Name = "Stock Confirmado:")]
        public Boolean stockConfirmado { get; set; }


        public DateTime fechaModificacion { get; set; }


        //  public Usuario usuario_aprobador { get; set; }
        // public Boolean mostrarCodigoProveedor { get; set; }
        public Decimal montoSubTotal { get; set; }
        public Decimal montoIGV { get; set; }
        public Decimal montoTotal { get; set; }


        public List<PedidoDetalle> pedidoDetalleList { get; set; }

        public List<PedidoAdjunto> pedidoAdjuntoList { get; set; }
        public bool esRePedido { get; set; }
        /*0 pendiente, 1 aprobado, 2 rechazado*/
        public SeguimientoPedido seguimientoPedido { get; set; }

        public List<SeguimientoPedido> seguimientoPedidoList { get; set; }

        public SeguimientoCrediticioPedido seguimientoCrediticioPedido { get; set; }

        public List<SeguimientoCrediticioPedido> seguimientoCrediticioPedidoList { get; set; }
        public bool mostrarCosto { get; set; }

        public bool considerarDescontinuados { get; set; }

        /*Campos utilizados para búsqueda*/
        public Usuario usuarioBusqueda { get; set; }



        [Display(Name = "Fecha Programación:")]
        public DateTime? fechaProgramacion { get; set; }

        [Display(Name = "Comentario Programación:")]
        public String comentarioProgramacion { get; set; }


        public DateTime fechaPrecios { get; set; }


        public List<GuiaRemision> guiaRemisionList { get; set; }
    

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

        //VENTA, TRASLADO INTERNO, COMODATO,  TRANSFERENCIA GRATUITA, PRESTAMO

        public enum tipos
        {
            [Display(Name = "Venta")]
            Venta = 'V', 
            [Display(Name = "Compra")]
            Compra = 'C',
        }

        public tipos tipo { get; set; }



        [Display(Name = "Tipo Pedido:")]
        public tiposPedido tipoPedido { get; set; }
        public enum tiposPedido
        {
            /*GENERAN GUIA REMISION*/
            [Display(Name = "Venta")]
            Venta = 'V', /*PEDIDOS DE VENTA*/
            [Display(Name = "Traslado Interno a Entregar")]
            TrasladoInterno = 'T',  /*ALMACEN*/
            [Display(Name = "Comodato a Entregar")]
            ComodatoEntregado = 'M', /*PEDIDO DE VENTA*/
            [Display(Name = "Transferencia Gratuita a Entregar")]
            TransferenciaGratuitaEntregada = 'G', /*PEDIDO DE VENTA*/
            [Display(Name = "Préstamo a Entregar")]
            PrestamoEntregado = 'P', /*PEDIDO DE VENTA*/
            [Display(Name = "Devolución de Venta")]
            DevolucionVenta = 'D', /*PEDIDO DE VENTA*/  //GENERA NOTA DE CREDITO
            [Display(Name = "Devolución de Préstamo Entregado")]
            DevolucionPrestamoEntregado = 'E',  /*PEDIDO DE VENTA*/
            [Display(Name = "Devolución de Comodato Entregado")]
            DevolucionComodatoEntregado = 'F', /*PEDIDO DE VENTA*/
            [Display(Name = "Devolución de Transferencia Gratuita Entregada")]
            DevolucionTransferenciaGratuitaEntregada = 'H' /*PEDIDO DE VENTA*/
        }

        public String tiposPedidoString
        {
            get
            {
                    return EnumHelper<tiposPedido>.GetDisplayValue(this.tipoPedido);                
            }
        }







        [Display(Name = "Tipo Pedido:")]
        public tiposPedidoCompra tipoPedidoCompra { get; set; }
        public enum tiposPedidoCompra
        {
            //GENERAN NOTA INGRESO
            [Display(Name = "Compra")]
            Compra = 'C',  /*PEDIDO DE COMPRA*/
            [Display(Name = "Traslado Interno a Recibir")]
            TrasladoInternoRecibido = 'T',  /*ALMACEN*/
            [Display(Name = "Comodato a Recibir")]
            ComodatoRecibido = 'M', /*PEDIDO DE COMPRA*/
            [Display(Name = "Transferencia Gratuita a Recibir")]
            TransferenciaGratuitaRecibida = 'G', /*PEDIDO DE COMPRA*/
            [Display(Name = "Préstamo a Recibir")]
            PrestamoRecibido = 'P', /*PEDIDO DE COMPRA*/
            [Display(Name = "Devolución de Compra")]
            DevolucionCompra = 'B', /*PEDIDO DE COMPRA*/
            [Display(Name = "Devolución de Préstamo Recibido")]
            DevolucionPrestamoRecibido = 'E', /*PEDIDO DE COMPRA*/
            [Display(Name = "Devolución de Comodato Recibido")]
            DevolucionComodatoRecibido = 'F', /*PEDIDO DE COMPRA*/
            [Display(Name = "Devolución de Transferencia Gratuita Recibida")]
            DevolucionTransferenciaGratuitaRecibida = 'H', /*PEDIDO DE COMPRA*/
        }

        public String tiposPedidoCompraString
        {
            get
            {
                    return EnumHelper<tiposPedidoCompra>.GetDisplayValue(this.tipoPedidoCompra);
            }
        }














        #region Criterios de Búsqueda
        [Display(Name = "Tipo Pedido:")]
        public tiposPedidoVentaBusqueda tipoPedidoVentaBusqueda { get; set; }
        public enum tiposPedidoVentaBusqueda
        {
            Todos = '0',
            Venta = tiposPedido.Venta,
            TrasladoInterno = tiposPedido.TrasladoInterno,
            ComodatoEntregado = tiposPedido.ComodatoEntregado,
            TransferenciaGratuitaEntregada = tiposPedido.TransferenciaGratuitaEntregada,
            PrestamoEntregado = tiposPedido.PrestamoEntregado,
            DevolucionVenta = tiposPedido.DevolucionVenta,
            DevolucionPrestamoEntregado = tiposPedido.DevolucionPrestamoEntregado,
            DevolucionComodatoEntregado = tiposPedido.DevolucionComodatoEntregado,
            DevolucionTransferenciaGratuitaEntregada = tiposPedido.DevolucionTransferenciaGratuitaEntregada,
        }

        [Display(Name = "Tipo Pedido:")]
        public tiposPedidoCompraBusqueda tipoPedidoCompraBusqueda { get; set; }
        public enum tiposPedidoCompraBusqueda
        {
            Todos = '0',
            Compra = tiposPedidoCompra.Compra,
            TrasladoInternoRecibido = tiposPedidoCompra.TrasladoInternoRecibido,
            ComodatoRecibido = tiposPedidoCompra.ComodatoRecibido,
            TransferenciaGratuitaRecibida = tiposPedidoCompra.TransferenciaGratuitaRecibida,
            PrestamoRecibido = tiposPedidoCompra.PrestamoRecibido,
            DevolucionCompra = tiposPedidoCompra.DevolucionCompra,
            DevolucionPrestamoRecibido = tiposPedidoCompra.DevolucionPrestamoRecibido,
            DevolucionComodatoRecibido = tiposPedidoCompra.DevolucionComodatoRecibido,
            DevolucionTransferenciaGratuitaRecibida = tiposPedidoCompra.DevolucionTransferenciaGratuitaRecibida,
        }


        [Display(Name = "Solicitado Desde:")]
        public DateTime fechaSolicitudDesde { get; set; }
        [Display(Name = "Solicitado Hasta:")]
        public DateTime fechaSolicitudHasta { get; set; }
        [Display(Name = "Programado Desde:")]
        public DateTime? fechaProgramacionDesde { get; set; }
        [Display(Name = "Programado Hasta:")]
        public DateTime? fechaProgramacionHasta { get; set; }

        #endregion

    }
}

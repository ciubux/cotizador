using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class Pedido : Auditoria, IDocumento, ICloneable
    {
        public Pedido(ClasesPedido tipo)
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
            this.truncado = 0;

            this.entregaATerceros = false;
            this.entregaTerciarizada = false;

            this.idGrupoCliente = 0;
        }

        public Pedido()
        {
            this.tipoPedido = tiposPedido.Venta;
            this.tipoPedidoCompra = tiposPedidoCompra.Compra;
            this.tipoPedidoAlmacen = tiposPedidoAlmacen.TrasladoInterno;
            this.tipoPedidoVentaBusqueda = tiposPedidoVentaBusqueda.Todos;
            this.tipoPedidoCompraBusqueda = tiposPedidoCompraBusqueda.Todos;
            this.tipoPedidoAlmacenBusqueda = tiposPedidoAlmacenBusqueda.Todos;

            this.solicitante = new Solicitante();
            this.pedidoAdjuntoList = new List<PedidoAdjunto>();
            this.ciudadASolicitar = new Ciudad();
            this.truncado = 0;
            this.idGrupoCliente = 0;

            this.entregaATerceros = false;
            this.entregaTerciarizada = false;
        }


        [Display(Name = "Truncado:")]
        public int truncado { get; set; }

        [Display(Name = "Restringir a factura única:")]
        public Boolean facturaUnica { get; set; }
        public Boolean guardadoParcialmente { get; set; }
        public bool accion_truncar { get; set; }
        public bool accion_alertarNoAtendido { get; set; }
        public Guid idPedido { get; set; }
        [Display(Name = "Número Pedido:")]
        public Int64 numeroPedido { get; set; }
        [Display(Name = "Número Grupo Pedido:")]
        public Int64? numeroGrupoPedido { get; set; }

        public Int64 numeroPedidoRelacionado { get; set; }
        public string codigoEmpresaPedidoRelacionado { get; set; }


        public Guid idClienteTercero { get; set; }
        public Boolean entregaATerceros { get; set; }
        public Boolean entregaTerciarizada { get; set; }

        public int idGrupoCliente { get; set; }

        public Guid idMPPedido { get; set; }
        public Int64 numeroPedidoMP { get; set; }

        public bool buscarSedesGrupoCliente { get; set; }

        [Display(Name = "Moneda:")]
        public Moneda moneda { get; set; }
        public OrdenCompraCliente ordenCompracliente { get; set; }

        [Display(Name = "Promociones:")]
        public List<Promocion> promociones { get; set; }
        public Cotizacion cotizacion { get; set; }
        [Display(Name = "Sede MP:")]
        public Ciudad ciudad { get; set; }

        [Display(Name = "Solicitar A:")]
        public Ciudad ciudadASolicitar { get; set; }

        [Display(Name = "Cliente:")]
        public Cliente cliente { get; set; }

        [Display(Name = "Vendedor:")]
        public Vendedor responsableComercial { get; set; }

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

        [Display(Name = "Fecha de Entrega Extendida:")]
        public DateTime? fechaEntregaExtendida { get; set; }

        public string fechaEntregaExtendidaString => !fechaEntregaExtendida.HasValue ? "" : ((DateTime)fechaEntregaExtendida).ToString("dd/MM/yyyy");


        [Display(Name = "Hora de Entrega:")]
        public String horaEntregaDesde { get; set; }

        [Display(Name = "Fecha Máxima de Entrega:")]
        public String horaEntregaHasta { get; set; }

        [Display(Name = "Hora de Entrega 2:")]
        public String horaEntregaAdicionalDesde { get; set; }

        [Display(Name = "Max Hora de Entrega 2:")]
        public String horaEntregaAdicionalHasta { get; set; }

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

        [Display(Name = "Observaciones Almacén:")]
        public String observacionesAlmacen { get; set; }

        [Display(Name = "Observaciones Factura:")]
        public String observacionesFactura { get; set; }

        [Display(Name = "Promoción:")]
        public Promocion promocion { get; set; }

        [Display(Name = "Ciudad Entrega:")]
        public Ubigeo ubigeoEntrega { get; set; }

        [Display(Name = "Número Requerimiento:")]
        public String numeroRequerimiento { get; set; }

        [Display(Name = "Venta Indirecta:")]
        public bool esVentaIndirecta { get; set; }

        public bool esVentaIndirectaAnt { get; set; }

        [Display(Name = "Fechas de Entrega:")]
        public String rangoFechasEntrega {
            get
            {
                String rangoFechasEntrega = String.Empty;
                if (this.fechaEntregaDesde != null && this.fechaEntregaHasta != null)
                {
                    String entregaDesde = this.fechaEntregaDesde.Value.ToString(Constantes.formatoFecha);
                    String entregaHasta = this.fechaEntregaHasta.Value.ToString(Constantes.formatoFecha);

                    if (!fechaEntregaExtendida.HasValue)
                    {
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
                        entregaHasta = this.fechaEntregaExtendida.Value.ToString(Constantes.formatoFecha);
                        return "Desde: " + entregaDesde + " Hasta (Ext.): " + entregaHasta;
                    }

                }
                else
                {
                    return "";
                }

            }
        }

        [Display(Name = "Horarios de Entrega:")]
        public String rangoHoraEntrega
        {
            get
            {
                string texto = "1er Turno: " + this.horaEntregaDesde + " - " + this.horaEntregaHasta;
                if (this.horaEntregaAdicionalDesde != null && !this.horaEntregaAdicionalDesde.Equals("00:00:00"))
                {
                    texto = texto + "   2do Turno: " + this.horaEntregaAdicionalDesde + " - " + this.horaEntregaAdicionalHasta;
                }
                return texto;
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
            get { return this.numeroGrupoPedido == 0 || this.numeroGrupoPedido == null ? "" : this.numeroGrupoPedido.ToString().PadLeft(Constantes.LONGITUD_NUMERO_GRUPO, Constantes.PAD); }
        }

        public String numeroPedidoString
        {
            get { return this.numeroPedido == 0 ? "" : this.numeroPedido.ToString().PadLeft(Constantes.LONGITUD_NUMERO, Constantes.PAD); }
        }


        public String numeroPedidoNumeroGrupoString
        {
            get {

                String numeroPedido = this.numeroPedido == 0 ? "" : this.numeroPedido.ToString().PadLeft(Constantes.LONGITUD_NUMERO, Constantes.PAD);
                if (!numeroGrupoPedidoString.Equals(String.Empty))
                    numeroPedido = numeroPedido + " (" + numeroGrupoPedidoString + ")";
                return numeroPedido;

            }
        }




        public Usuario usuario { get; set; }
        public Boolean incluidoIGV { get; set; }
        //   public Decimal flete { get; set; }


        [Display(Name = "Creado Por:")]
        public String usuarioCreacion { get; set; }



        public DocumentoVenta documentoVenta { get; set; }

        public IDocumentoPago documentoPago { get; set; }

        [Display(Name = "Stock Confirmado:")]
        public int stockConfirmado { get; set; }


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


        public Almacen almacenOrigen { get; set; }
        public Almacen almacenDestino { get; set; }
        public int usaSerieTI { get; set; }

        public DateTime fechaPrecios { get; set; }

        public Boolean esPagoContado { get; set; }

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


        public enum ClasesPedido
        {
            [Display(Name = "Venta")]
            Venta = 'V',
            [Display(Name = "Compra")]
            Compra = 'C',
            [Display(Name = "Almacen")]
            Almacen = 'A'
        }
        
        public ClasesPedido clasePedido { get; set; }


        [Display(Name = "Tipo Pedido:")]
        public tiposPedido tipoPedido { get; set; }
        public enum tiposPedido
        {
            /*GENERAN GUIA REMISION*/
            [Display(Name = "Venta")]
            Venta = 'V', 
            [Display(Name = "Comodato a Entregar")]
            ComodatoEntregado = 'M', 
            [Display(Name = "Transferencia Gratuita a Entregar")]
            TransferenciaGratuitaEntregada = 'G', 

            /*GENERAN NOTAS DE INGRESO*/
 /*           [Display(Name = "Devolución de Venta")]
            DevolucionVenta = 'D',  //GENERA NOTA DE CREDITO
            [Display(Name = "Devolución de Comodato Entregado")]
            DevolucionComodatoEntregado = 'F', 
            [Display(Name = "Devolución de Transferencia Gratuita Entregada")]
            DevolucionTransferenciaGratuitaEntregada = 'H' */
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
            Compra = 'C',  
            [Display(Name = "Comodato a Recibir")]
            ComodatoRecibido = 'M', 
            [Display(Name = "Transferencia Gratuita a Recibir")]
            TransferenciaGratuitaRecibida = 'G',

            /*GENERAN NOTAS DE INGRESO*/
   /*         [Display(Name = "Devolución de Compra")]
            DevolucionCompra = 'B', 
            [Display(Name = "Devolución de Comodato Recibido")]
            DevolucionComodatoRecibido = 'F', 
            [Display(Name = "Devolución de Transferencia Gratuita Recibida")]
            DevolucionTransferenciaGratuitaRecibida = 'H', 
    */    }

        public String tiposPedidoCompraString
        {
            get
            {
                    return EnumHelper<tiposPedidoCompra>.GetDisplayValue(this.tipoPedidoCompra);
            }
        }



        [Display(Name = "Tipo Pedido:")]
        public tiposPedidoAlmacen tipoPedidoAlmacen { get; set; }
        public enum tiposPedidoAlmacen
        {
            [Display(Name = "Solicitud de Traslado")]
            TrasladoInterno = 'T', //GUIA REMISION
       //     [Display(Name = "Recepción de Traslado Interno")]
       //s     TrasladoInternoRecibido = 'I',  //NOTA INGRESO
            [Display(Name = "Préstamo a Entregar")]
            PrestamoEntregado = 'P',   //GUIA REMISION
    //        [Display(Name = "Devolución de Préstamo Entregado")]
    //        DevolucionPrestamoEntregado = 'D', //NOTA INGRESO
            [Display(Name = "Préstamo a Recibir")]
            PrestamoRecibido = 'R', //NOTA INGRESO
      /*      [Display(Name = "Devolución de Préstamo Recibido")]
            DevolucionPrestamoRecibido = 'E', //GUIA REMISION           
            [Display(Name = "Extorno de Guía Remisión")]
            ExtornoGuíaRemision = 'X',  //NOTA INGRESO
            */
        }

        public String textoCondicionesPago
        {
            get
            {
                if (this.esPagoContado)
                {
                    return EnumHelper<DocumentoVenta.TipoPago>.GetDisplayValue(DocumentoVenta.TipoPago.Contado);
                }
                else
                {
                    if (this.cliente != null)
                    {
                        /*Se evalua si el solicitado es al contado*/
                        if (this.cliente.plazoCreditoSolicitado == DocumentoVenta.TipoPago.Contado)
                        {
                            return EnumHelper<DocumentoVenta.TipoPago>.GetDisplayValue(this.cliente.plazoCreditoSolicitado) + ".";
                        }
                        /*Si no es al contado y el pago aprobado es no asignado se muestra el mensaje que indica que está sujeto a evaluación*/
                        else if (this.cliente.tipoPagoFactura == DocumentoVenta.TipoPago.NoAsignado)
                        {
                            return EnumHelper<DocumentoVenta.TipoPago>.GetDisplayValue(this.cliente.plazoCreditoSolicitado) + ", sujeto a evaluación crediticia (aprobación pendiente).";
                        }
                        /*Si no es un caso anterior se muestra el plazo de credito aprobado*/
                        else
                        {
                            return EnumHelper<DocumentoVenta.TipoPago>.GetDisplayValue(this.cliente.tipoPagoFactura) + ".";
                        }
                    }
                    else
                    {
                        return String.Empty;
                    }
                }
            }

        }

        public String tiposPedidoAlmacenString
        {
            get
            {
                return EnumHelper<tiposPedidoAlmacen>.GetDisplayValue(this.tipoPedidoAlmacen);
            }
        }

        public List<PedidoGrupo> pedidoGrupoList { get; set; }

        #region Criterios de Búsqueda
        [Display(Name = "Tipo Pedido:")]
        public tiposPedidoVentaBusqueda tipoPedidoVentaBusqueda { get; set; }
        public enum tiposPedidoVentaBusqueda
        {
            Todos = '0',
            [Display(Name = "Venta")]
            Venta = tiposPedido.Venta,
            [Display(Name = "Comodato a Entregar")]
            ComodatoEntregado = tiposPedido.ComodatoEntregado,
            [Display(Name = "Transferencia Gratuita a Entregar")]
            TransferenciaGratuitaEntregada = tiposPedido.TransferenciaGratuitaEntregada,
      /*      [Display(Name = "Devolución de Venta")]
            DevolucionVenta = tiposPedido.DevolucionVenta,
            [Display(Name = "Devolución de Comodato Entregado")]
            DevolucionComodatoEntregado = tiposPedido.DevolucionComodatoEntregado,
            [Display(Name = "Devolución de Transferencia Gratuita Entregada")]
            DevolucionTransferenciaGratuitaEntregada = tiposPedido.DevolucionTransferenciaGratuitaEntregada,
   */     }
        
        [Display(Name = "Tipo Pedido:")]
        public tiposPedidoCompraBusqueda tipoPedidoCompraBusqueda { get; set; }
        public enum tiposPedidoCompraBusqueda
        {
            Todos = '0',
            /*GENERAN NOTAS DE INGRESO*/
            [Display(Name = "Compra")]
            Compra = tiposPedidoCompra.Compra,
            [Display(Name = "Comodato a Recibir")]
            ComodatoRecibido = tiposPedidoCompra.ComodatoRecibido,
            [Display(Name = "Transferencia Gratuita a Recibir")]
            TransferenciaGratuitaRecibida = tiposPedidoCompra.TransferenciaGratuitaRecibida,
      /*      [Display(Name = "Devolución de Compra")]
            DevolucionCompra = tiposPedidoCompra.DevolucionCompra,
            [Display(Name = "Devolución de Comodato Recibido")]
            DevolucionComodatoRecibido = tiposPedidoCompra.DevolucionComodatoRecibido,
            [Display(Name = "Devolución de Transferencia Gratuita Recibida")]
            DevolucionTransferenciaGratuitaRecibida = tiposPedidoCompra.DevolucionTransferenciaGratuitaRecibida,
     */   }


        [Display(Name = "Tipo Pedido:")]
        public tiposPedidoAlmacenBusqueda tipoPedidoAlmacenBusqueda { get; set; }
        public enum tiposPedidoAlmacenBusqueda
        {
            Todos = '0',
            /*GENERAN GUÍA REMISIÓN DE INGRESO*/
            [Display(Name = "Traslado Interno a Entregar")]
            TrasladoInterno = tiposPedidoAlmacen.TrasladoInterno,
     //       [Display(Name = "Traslado Interno a Recibir")]
    //        TrasladoInternoRecibido = tiposPedidoAlmacen.TrasladoInternoRecibido,
            [Display(Name = "Préstamo a Entregar")]
            PrestamoEntregado = tiposPedidoAlmacen.PrestamoEntregado,
     //       [Display(Name = "Devolución de Préstamo Entregado")]
    //        DevolucionPrestamoEntregado = tiposPedidoAlmacen.DevolucionPrestamoEntregado,
            [Display(Name = "Préstamo a Recibir")]
            PrestamoRecibido = tiposPedidoAlmacen.PrestamoRecibido, 
   /*         [Display(Name = "Devolución de Préstamo Recibido")]
            DevolucionPrestamoRecibido = tiposPedidoAlmacen.DevolucionPrestamoRecibido,
            [Display(Name = "Extorno de Guía Remisión")]
            ExtornoGuíaRemision = tiposPedidoAlmacen.ExtornoGuíaRemision,
     */   }


        [Display(Name = "Solicitado Desde:")]
        public DateTime fechaSolicitudDesde { get; set; }
        [Display(Name = "Solicitado Hasta:")]
        public DateTime fechaSolicitudHasta { get; set; }
        [Display(Name = "Creado Desde:")]
        public DateTime fechaCreacionDesde { get; set; }
        [Display(Name = "Creado Hasta:")]
        public DateTime fechaCreacionHasta { get; set; }

        [Display(Name = "Programado Desde:")]
        public DateTime? fechaProgramacionDesde { get; set; }
        [Display(Name = "Programado Hasta:")]
        public DateTime? fechaProgramacionHasta { get; set; }

        #endregion


        public Decimal utilidadVisible
        {
            get
            {
                decimal utilidad = 0;

                foreach (PedidoDetalle det in this.documentoDetalle)
                {
                    if (det.tieneCostoEspecial)
                    {
                        utilidad = utilidad + ((det.precioNeto - det.costoEspecialVisible) * det.cantidad);
                    }
                    else
                    {
                        utilidad = utilidad + ((det.precioNeto - det.costoListaVisible) * det.cantidad);
                    }

                }

                return utilidad;
            }
        }

        public Decimal utilidadFleteVisible
        {
            get
            {
                decimal utilidad = 0;
                foreach (PedidoDetalle det in this.documentoDetalle)
                {
                    if (det.tieneCostoEspecial)
                    {
                        utilidad = utilidad + ((det.precioNeto - det.costoEspecialFleteVisible) * det.cantidad);
                    }
                    else
                    {
                        utilidad = utilidad + ((det.precioNeto - det.costoListaFleteVisible) * det.cantidad);
                    }
                }

                return utilidad;
            }
        }

        public Decimal margenVisible
        {
            get
            {
                decimal utilidad = this.utilidadVisible;
                decimal total = 0;

                foreach (PedidoDetalle det in this.documentoDetalle)
                {
                    total = total + (det.precioNeto * det.cantidad);
                }

                if (total == 0)
                {
                    return 0;
                }

                return Decimal.Parse(String.Format(Constantes.formatoUnDecimal, (utilidad / total) * 100));
            }
        }

        public Decimal margenFleteVisible
        {
            get
            {
                decimal utilidad = this.utilidadFleteVisible;
                decimal total = 0;

                foreach (PedidoDetalle det in this.documentoDetalle)
                {
                    total = total + (det.precioNeto * det.cantidad);
                }

                if (total == 0)
                {
                    return 0;
                }

                return Decimal.Parse(String.Format(Constantes.formatoUnDecimal, (utilidad / total) * 100));
            }
        }


        /**
         * Utilizado para búsqueda
         */
        [Display(Name = "SKU:")]
        public String sku { get; set; }


        public Vendedor vendedor { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

    }
}

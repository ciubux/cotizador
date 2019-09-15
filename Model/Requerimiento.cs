using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class Requerimiento : Auditoria, IDocumento
    {
        public Requerimiento(ClasesRequerimiento tipo)
        {
            this.claseRequerimiento = tipo;
            this.tipoRequerimiento = tiposRequerimiento.Venta;
            this.tipoRequerimientoCompra = tiposRequerimientoCompra.Compra;
            this.tipoRequerimientoAlmacen = tiposRequerimientoAlmacen.TrasladoInterno;
            this.tipoRequerimientoVentaBusqueda = tiposRequerimientoVentaBusqueda.Todos;
            this.tipoRequerimientoCompraBusqueda = tiposRequerimientoCompraBusqueda.Todos;
            this.tipoRequerimientoAlmacenBusqueda = tiposRequerimientoAlmacenBusqueda.Todos;
            this.estadoRequerimiento = estadosRequerimiento.Ingresado;
            this.periodo = new PeriodoSolicitud();
            this.solicitante = new Solicitante();
            this.ciudadASolicitar = new Ciudad();

            this.idGrupoCliente = 0;
        }

        public Requerimiento()
        {
            this.tipoRequerimiento = tiposRequerimiento.Venta;
            this.tipoRequerimientoCompra = tiposRequerimientoCompra.Compra;
            this.tipoRequerimientoAlmacen = tiposRequerimientoAlmacen.TrasladoInterno;
            this.tipoRequerimientoVentaBusqueda = tiposRequerimientoVentaBusqueda.Todos;
            this.tipoRequerimientoCompraBusqueda = tiposRequerimientoCompraBusqueda.Todos;
            this.tipoRequerimientoAlmacenBusqueda = tiposRequerimientoAlmacenBusqueda.Todos;

            this.solicitante = new Solicitante();
            this.ciudadASolicitar = new Ciudad();

            this.idGrupoCliente = 0;
        }
        [Display(Name = "Número Requerimiento:")]
        public Int32 idRequerimiento { get; set; }
        
        public int idGrupoCliente { get; set; }

        public bool buscarSedesGrupoCliente { get; set; }


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
        public String contactoRequerimiento { get; set; }
        [Display(Name = "Telefono Solicitante:")]
        public String telefonoContactoRequerimiento { get; set; }
        [Display(Name = "Correo Solicitante:")]
        public String correoContactoRequerimiento { get; set; }

        [Display(Name = "Telefono y Correo Solicitante:")]
        public String telefonoCorreoContactoRequerimiento
        {
            get { return "Tef: " + telefonoContactoRequerimiento + " Correo: " + correoContactoRequerimiento; }
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
        public String rangoFechasEntrega
        {
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

            get
            {
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

   

        public Usuario usuario { get; set; }
        public Boolean incluidoIGV { get; set; }
        //   public Decimal flete { get; set; }


        [Display(Name = "Creado Por:")]
        public String usuarioCreacion { get; set; }



        public DocumentoVenta documentoVenta { get; set; }

        public IDocumentoPago documentoPago { get; set; }

        [Display(Name = "Stock Confirmado:")]
        public Boolean stockConfirmado { get; set; }


        public DateTime fechaModificacion { get; set; }


        //  public Usuario usuario_aprobador { get; set; }
        // public Boolean mostrarCodigoProveedor { get; set; }
        public Decimal montoSubTotal { get; set; }
        public Decimal montoIGV { get; set; }
        public Decimal montoTotal { get; set; }


        public List<RequerimientoDetalle> requerimientoDetalleList { get; set; }

        public bool esReRequerimiento { get; set; }
        /*0 pendiente, 1 aprobado, 2 rechazado*/
       public bool mostrarCosto { get; set; }

        public bool considerarDescontinuados { get; set; }

        /*Campos utilizados para búsqueda*/
        public Usuario usuarioBusqueda { get; set; }



        [Display(Name = "Fecha Programación:")]
        public DateTime? fechaProgramacion { get; set; }

        [Display(Name = "Comentario Programación:")]
        public String comentarioProgramacion { get; set; }


        public DateTime fechaPrecios { get; set; }

        public Boolean esPagoContado { get; set; }

        public List<GuiaRemision> guiaRemisionList { get; set; }


        /*Implementación Interface*/
        public List<DocumentoDetalle> documentoDetalle
        {
            get
            {
                List<DocumentoDetalle> documentoDetalle = new List<DocumentoDetalle>();
                if (this.requerimientoDetalleList == null)
                    this.requerimientoDetalleList = new List<RequerimientoDetalle>();
                foreach (RequerimientoDetalle pedidoDetalle in requerimientoDetalleList)
                {
                    documentoDetalle.Add(pedidoDetalle);
                }
                return documentoDetalle;
            }
            set
            {
                this.requerimientoDetalleList = new List<RequerimientoDetalle>();
                foreach (DocumentoDetalle documentoDetalle in value)
                {
                    requerimientoDetalleList.Add((RequerimientoDetalle)documentoDetalle);
                }
            }
        }


        public enum ClasesRequerimiento
        {
            [Display(Name = "Venta")]
            Venta = 'V',
            [Display(Name = "Compra")]
            Compra = 'C',
            [Display(Name = "Almacen")]
            Almacen = 'A'
        }

        public ClasesRequerimiento claseRequerimiento { get; set; }


        [Display(Name = "Tipo Requerimiento:")]
        public tiposRequerimiento tipoRequerimiento { get; set; }
        public enum tiposRequerimiento
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

        public String tiposRequerimientoString
        {
            get
            {
                return EnumHelper<tiposRequerimiento>.GetDisplayValue(this.tipoRequerimiento);
            }
        }

        [Display(Name = "Tipo Requerimiento:")]
        public tiposRequerimientoCompra tipoRequerimientoCompra { get; set; }
        public enum tiposRequerimientoCompra
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
             */
        }

        public String tiposRequerimientoCompraString
        {
            get
            {
                return EnumHelper<tiposRequerimientoCompra>.GetDisplayValue(this.tipoRequerimientoCompra);
            }
        }



        [Display(Name = "Tipo Requerimiento:")]
        public tiposRequerimientoAlmacen tipoRequerimientoAlmacen { get; set; }
        public enum tiposRequerimientoAlmacen
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

        public String tiposRequerimientoAlmacenString
        {
            get
            {
                return EnumHelper<tiposRequerimientoAlmacen>.GetDisplayValue(this.tipoRequerimientoAlmacen);
            }
        }

        #region Criterios de Búsqueda
        [Display(Name = "Tipo Requerimiento:")]
        public tiposRequerimientoVentaBusqueda tipoRequerimientoVentaBusqueda { get; set; }
        public enum tiposRequerimientoVentaBusqueda
        {
            Todos = '0',
            [Display(Name = "Venta")]
            Venta = tiposRequerimiento.Venta,
            [Display(Name = "Comodato a Entregar")]
            ComodatoEntregado = tiposRequerimiento.ComodatoEntregado,
            [Display(Name = "Transferencia Gratuita a Entregar")]
            TransferenciaGratuitaEntregada = tiposRequerimiento.TransferenciaGratuitaEntregada,
            /*      [Display(Name = "Devolución de Venta")]
                  DevolucionVenta = tiposRequerimiento.DevolucionVenta,
                  [Display(Name = "Devolución de Comodato Entregado")]
                  DevolucionComodatoEntregado = tiposRequerimiento.DevolucionComodatoEntregado,
                  [Display(Name = "Devolución de Transferencia Gratuita Entregada")]
                  DevolucionTransferenciaGratuitaEntregada = tiposRequerimiento.DevolucionTransferenciaGratuitaEntregada,
         */
        }

        [Display(Name = "Tipo Requerimiento:")]
        public tiposRequerimientoCompraBusqueda tipoRequerimientoCompraBusqueda { get; set; }
        public enum tiposRequerimientoCompraBusqueda
        {
            Todos = '0',
            /*GENERAN NOTAS DE INGRESO*/
            [Display(Name = "Compra")]
            Compra = tiposRequerimientoCompra.Compra,
            [Display(Name = "Comodato a Recibir")]
            ComodatoRecibido = tiposRequerimientoCompra.ComodatoRecibido,
            [Display(Name = "Transferencia Gratuita a Recibir")]
            TransferenciaGratuitaRecibida = tiposRequerimientoCompra.TransferenciaGratuitaRecibida,
            /*      [Display(Name = "Devolución de Compra")]
                  DevolucionCompra = tiposRequerimientoCompra.DevolucionCompra,
                  [Display(Name = "Devolución de Comodato Recibido")]
                  DevolucionComodatoRecibido = tiposRequerimientoCompra.DevolucionComodatoRecibido,
                  [Display(Name = "Devolución de Transferencia Gratuita Recibida")]
                  DevolucionTransferenciaGratuitaRecibida = tiposRequerimientoCompra.DevolucionTransferenciaGratuitaRecibida,
           */
        }


        [Display(Name = "Tipo Requerimiento:")]
        public tiposRequerimientoAlmacenBusqueda tipoRequerimientoAlmacenBusqueda { get; set; }
        public enum tiposRequerimientoAlmacenBusqueda
        {
            Todos = '0',
            /*GENERAN GUÍA REMISIÓN DE INGRESO*/
            [Display(Name = "Traslado Interno a Entregar")]
            TrasladoInterno = tiposRequerimientoAlmacen.TrasladoInterno,
            //       [Display(Name = "Traslado Interno a Recibir")]
            //        TrasladoInternoRecibido = tiposRequerimientoAlmacen.TrasladoInternoRecibido,
            [Display(Name = "Préstamo a Entregar")]
            PrestamoEntregado = tiposRequerimientoAlmacen.PrestamoEntregado,
            //       [Display(Name = "Devolución de Préstamo Entregado")]
            //        DevolucionPrestamoEntregado = tiposRequerimientoAlmacen.DevolucionPrestamoEntregado,
            [Display(Name = "Préstamo a Recibir")]
            PrestamoRecibido = tiposRequerimientoAlmacen.PrestamoRecibido,
            /*         [Display(Name = "Devolución de Préstamo Recibido")]
                     DevolucionPrestamoRecibido = tiposRequerimientoAlmacen.DevolucionPrestamoRecibido,
                     [Display(Name = "Extorno de Guía Remisión")]
                     ExtornoGuíaRemision = tiposRequerimientoAlmacen.ExtornoGuíaRemision,
              */
        }


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


        /**
         * Utilizado para búsqueda
         */
        [Display(Name = "SKU:")]
        public String sku { get; set; }



        [Display(Name = "Estado Requerimiento:")]
        public estadosRequerimiento estadoRequerimiento { get; set; }
        public enum estadosRequerimiento
        {
            [Display(Name = "Ingresado")]
            Ingresado = 0,
            [Display(Name = "Aprobado")]
            Aprobado = 1,
            [Display(Name = "Liberada")]
            Liberada = 2
        }

        public String estadoRequerimientoString
        {
            get
            {
                return EnumHelper<estadosRequerimiento>.GetDisplayValue(this.estadoRequerimiento);
            }
        }

        public bool excedioPresupuesto { get; set; }
        public decimal topePresupuesto { get; set; }



     /*   [Display(Name = "Periodo:")]
        public periodos periodo { get; set; }
        public enum periodos
        {
            [Display(Name = "Abril-2019")]
            abril = 0,
            [Display(Name = "Mayo-2019")]
            mayo = 1,
            [Display(Name = "Junio-2019")]
            junio = 2,
            [Display(Name = "Julio-2019")]
            julio = 3,
            [Display(Name = "Agosto-2019")]
            agosto = 4
        }*/

  /*      public String periodoString
        {
            get
            {
                return EnumHelper<periodos>.GetDisplayValue(this.periodo);
            }
        }*/

        public Pedido pedido { get; set; }

        public String numeroRequerimientoString
        {
            get { return this.idRequerimiento == 0 ? "" : this.idRequerimiento.ToString().PadLeft(Constantes.LONGITUD_NUMERO, Constantes.PAD); }
        }

        public PeriodoSolicitud periodo { get; set; }
    }
}

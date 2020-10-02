using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class OrdenCompraCliente : Auditoria, IDocumento
    {
        public OrdenCompraCliente()
        {
            this.tipoOrdenCompraCliente = tiposOrdenCompraCliente.Venta;
            this.tipoOrdenCompraClienteCompra = tiposOrdenCompraClienteCompra.Compra;
            this.tipoOrdenCompraClienteAlmacen = tiposOrdenCompraClienteAlmacen.TrasladoInterno;
            this.tipoOrdenCompraClienteVentaBusqueda = tiposOrdenCompraClienteVentaBusqueda.Todos;
            this.tipoOrdenCompraClienteCompraBusqueda = tiposOrdenCompraClienteCompraBusqueda.Todos;
            this.tipoOrdenCompraClienteAlmacenBusqueda = tiposOrdenCompraClienteAlmacenBusqueda.Todos;

            this.solicitante = new Solicitante();
            this.AdjuntoList = new List<ArchivoAdjunto>();
            this.ciudadASolicitar = new Ciudad();

            this.incluidoIGV = false;
            this.idGrupoCliente = 0;
        }




        public Guid idOrdenCompraCliente { get; set; }
        [Display(Name = "Registro N°")]
        public Int64 numeroOrdenCompraCliente { get; set; }

        public int idGrupoCliente { get; set; }

        public bool buscarSedesGrupoCliente { get; set; }


        public Cotizacion cotizacion { get; set; }
        [Display(Name = "Sede:")]
        public Ciudad ciudad { get; set; }

        [Display(Name = "Solicitar A:")]
        public Ciudad ciudadASolicitar { get; set; }

        [Display(Name = "Cliente:")]
        public Cliente cliente { get; set; }

        [Display(Name = "Cliente:")]
        public ClienteSunat clienteSunat { get; set; }

        [Display(Name = "O/C N°:")]
        public String numeroReferenciaCliente { get; set; }


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

        [Display(Name = "Hora Máxima de Entrega:")]
        public String horaEntregaHasta { get; set; }

        [Display(Name = "Hora de Entrega 2:")]
        public String horaEntregaAdicionalDesde { get; set; }

        [Display(Name = "Max Hora de Entrega 2:")]
        public String horaEntregaAdicionalHasta { get; set; }

        [Display(Name = "Fecha Solicitud:")]
        public DateTime fechaSolicitud { get; set; }
        [Required(ErrorMessage = "Ingrese la Fecha de Entrega.")]

        [Display(Name = "Solicitado Por:")]
        public String contactoOrdenCompraCliente { get; set; }
        [Display(Name = "Telefono Solicitante:")]
        public String telefonoContactoOrdenCompraCliente { get; set; }
        [Display(Name = "Correo Solicitante:")]
        public String correoContactoOrdenCompraCliente { get; set; }

        [Display(Name = "Telefono y Correo Solicitante:")]
        public String telefonoCorreoContactoOrdenCompraCliente
        {
            get { return "Tef: " + telefonoContactoOrdenCompraCliente + " Correo: " + correoContactoOrdenCompraCliente; }
        }

        [Display(Name = "Observaciones para uso interno:")]
        public String observaciones { get; set; }

        [Display(Name = "Observaciones Guía Remisión:")]
        public String observacionesGuiaRemision { get; set; }

        [Display(Name = "Observaciones Factura:")]
        public String observacionesFactura { get; set; }

        [Display(Name = "Ciudad Entrega:")]
        public Ubigeo ubigeoEntrega { get; set; }

        [Display(Name = "Número Requerimiento:")]
        public String numeroRequerimiento { get; set; }


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


        public String numeroOrdenCompraClienteString
        {
            get { return this.numeroOrdenCompraCliente == 0 ? "" : this.numeroOrdenCompraCliente.ToString().PadLeft(Constantes.LONGITUD_NUMERO, Constantes.PAD); }
        }




        public Boolean incluidoIGV { get; set; }
        //   public Decimal flete { get; set; }


        [Display(Name = "Creado Por:")]
        public String usuarioCreacion { get; set; }



        
        //  public Usuario usuario_aprobador { get; set; }
        // public Boolean mostrarCodigoProveedor { get; set; }
        public Decimal montoSubTotal { get; set; }
        public Decimal montoIGV { get; set; }
        public Decimal montoTotal { get; set; }


        public List<OrdenCompraClienteDetalle> detalleList { get; set; }

        public List<ArchivoAdjunto> AdjuntoList { get; set; }
        public bool esReOrdenCompraCliente { get; set; }
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
                if (this.detalleList == null)
                    this.detalleList = new List<OrdenCompraClienteDetalle>();
                foreach (OrdenCompraClienteDetalle OrdenCompraClienteDetalle in detalleList)
                {
                    documentoDetalle.Add(OrdenCompraClienteDetalle);
                }
                return documentoDetalle;
            }
            set
            {
                this.detalleList = new List<OrdenCompraClienteDetalle>();
                foreach (DocumentoDetalle documentoDetalle in value)
                {
                    detalleList.Add((OrdenCompraClienteDetalle)documentoDetalle);
                }
            }
        }


        public enum ClasesOrdenCompraCliente
        {
            [Display(Name = "Venta")]
            Venta = 'V',
            [Display(Name = "Compra")]
            Compra = 'C',
            [Display(Name = "Almacen")]
            Almacen = 'A'
        }
        
        public ClasesOrdenCompraCliente claseOrdenCompraCliente { get; set; }


        [Display(Name = "Tipo OrdenCompraCliente:")]
        public tiposOrdenCompraCliente tipoOrdenCompraCliente { get; set; }
        public enum tiposOrdenCompraCliente
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

        public String tiposOrdenCompraClienteString
        {
            get
            {
                    return EnumHelper<tiposOrdenCompraCliente>.GetDisplayValue(this.tipoOrdenCompraCliente);                
            }
        }
        
        [Display(Name = "Tipo OrdenCompraCliente:")]
        public tiposOrdenCompraClienteCompra tipoOrdenCompraClienteCompra { get; set; }
        public enum tiposOrdenCompraClienteCompra
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

        public String tiposOrdenCompraClienteCompraString
        {
            get
            {
                    return EnumHelper<tiposOrdenCompraClienteCompra>.GetDisplayValue(this.tipoOrdenCompraClienteCompra);
            }
        }



        [Display(Name = "Tipo OrdenCompraCliente:")]
        public tiposOrdenCompraClienteAlmacen tipoOrdenCompraClienteAlmacen { get; set; }
        public enum tiposOrdenCompraClienteAlmacen
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

        public String tiposOrdenCompraClienteAlmacenString
        {
            get
            {
                return EnumHelper<tiposOrdenCompraClienteAlmacen>.GetDisplayValue(this.tipoOrdenCompraClienteAlmacen);
            }
        }


        #region Criterios de Búsqueda
        [Display(Name = "Tipo OrdenCompraCliente:")]
        public tiposOrdenCompraClienteVentaBusqueda tipoOrdenCompraClienteVentaBusqueda { get; set; }
        public enum tiposOrdenCompraClienteVentaBusqueda
        {
            Todos = '0',
            [Display(Name = "Venta")]
            Venta = tiposOrdenCompraCliente.Venta,
            [Display(Name = "Comodato a Entregar")]
            ComodatoEntregado = tiposOrdenCompraCliente.ComodatoEntregado,
            [Display(Name = "Transferencia Gratuita a Entregar")]
            TransferenciaGratuitaEntregada = tiposOrdenCompraCliente.TransferenciaGratuitaEntregada,
      /*      [Display(Name = "Devolución de Venta")]
            DevolucionVenta = tiposOrdenCompraCliente.DevolucionVenta,
            [Display(Name = "Devolución de Comodato Entregado")]
            DevolucionComodatoEntregado = tiposOrdenCompraCliente.DevolucionComodatoEntregado,
            [Display(Name = "Devolución de Transferencia Gratuita Entregada")]
            DevolucionTransferenciaGratuitaEntregada = tiposOrdenCompraCliente.DevolucionTransferenciaGratuitaEntregada,
   */     }
        
        [Display(Name = "Tipo OrdenCompraCliente:")]
        public tiposOrdenCompraClienteCompraBusqueda tipoOrdenCompraClienteCompraBusqueda { get; set; }
        public enum tiposOrdenCompraClienteCompraBusqueda
        {
            Todos = '0',
            /*GENERAN NOTAS DE INGRESO*/
            [Display(Name = "Compra")]
            Compra = tiposOrdenCompraClienteCompra.Compra,
            [Display(Name = "Comodato a Recibir")]
            ComodatoRecibido = tiposOrdenCompraClienteCompra.ComodatoRecibido,
            [Display(Name = "Transferencia Gratuita a Recibir")]
            TransferenciaGratuitaRecibida = tiposOrdenCompraClienteCompra.TransferenciaGratuitaRecibida,
      /*      [Display(Name = "Devolución de Compra")]
            DevolucionCompra = tiposOrdenCompraClienteCompra.DevolucionCompra,
            [Display(Name = "Devolución de Comodato Recibido")]
            DevolucionComodatoRecibido = tiposOrdenCompraClienteCompra.DevolucionComodatoRecibido,
            [Display(Name = "Devolución de Transferencia Gratuita Recibida")]
            DevolucionTransferenciaGratuitaRecibida = tiposOrdenCompraClienteCompra.DevolucionTransferenciaGratuitaRecibida,
     */   }


        [Display(Name = "Tipo OrdenCompraCliente:")]
        public tiposOrdenCompraClienteAlmacenBusqueda tipoOrdenCompraClienteAlmacenBusqueda { get; set; }
        public enum tiposOrdenCompraClienteAlmacenBusqueda
        {
            Todos = '0',
            /*GENERAN GUÍA REMISIÓN DE INGRESO*/
            [Display(Name = "Traslado Interno a Entregar")]
            TrasladoInterno = tiposOrdenCompraClienteAlmacen.TrasladoInterno,
     //       [Display(Name = "Traslado Interno a Recibir")]
    //        TrasladoInternoRecibido = tiposOrdenCompraClienteAlmacen.TrasladoInternoRecibido,
            [Display(Name = "Préstamo a Entregar")]
            PrestamoEntregado = tiposOrdenCompraClienteAlmacen.PrestamoEntregado,
     //       [Display(Name = "Devolución de Préstamo Entregado")]
    //        DevolucionPrestamoEntregado = tiposOrdenCompraClienteAlmacen.DevolucionPrestamoEntregado,
            [Display(Name = "Préstamo a Recibir")]
            PrestamoRecibido = tiposOrdenCompraClienteAlmacen.PrestamoRecibido, 
   /*         [Display(Name = "Devolución de Préstamo Recibido")]
            DevolucionPrestamoRecibido = tiposOrdenCompraClienteAlmacen.DevolucionPrestamoRecibido,
            [Display(Name = "Extorno de Guía Remisión")]
            ExtornoGuíaRemision = tiposOrdenCompraClienteAlmacen.ExtornoGuíaRemision,
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

        public List<Ciudad> sedesClienteSunat { get; set; }

        /**
         * Utilizado para búsqueda
         */
        [Display(Name = "SKU:")]
        public String sku { get; set; }

    }
}

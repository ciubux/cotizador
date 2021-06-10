using Model.ServiceReferencePSE;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class DocumentoVenta : Auditoria, IDocumentoPago
    {

        public DocumentoVenta()
        {
            this.tipoNotaCredito = TiposNotaCredito.AnulacionOperacion;
            this.tipoNotaDebito = TiposNotaDebito.Penalidades;
            this.tipoDocumentoIdentidad = TiposDocumentoIdentidad.RUC;
        }

        [Display(Name = "Fecha Emisión:")]
        public DateTime? fechaEmision { get; set; }

        [Display(Name = "Hora Emisión:")]
        public DateTime horaEmision { get; set; }

        [Display(Name = "Fecha Vencimiento:")]
        public DateTime fechaVencimiento { get; set; }

        [Display(Name = "Condición de Pago:")]
        public TipoPago tipoPago { get; set; }

        public String tipoPagoString
        {
            get
            {
                return EnumHelper<TipoPago>.GetDisplayValue(this.tipoPago);
            }
        }


        [Display(Name = "Forma de Pago:")]
        public FormaPago formaPago { get; set; }

        public enum TipoPago
        {
            [Display(Name = "NO ASIGNADO")]
            NoAsignado = 0,
            [Display(Name = "CONTADO")]
            Contado = 1,
            [Display(Name = "CRÉDITO A 1 DÍA")]
            Crédito1 = 101,
            [Display(Name = "CRÉDITO A 7 DÍAS")]
            Crédito7 = 2,
            [Display(Name = "CRÉDITO A 15 DÍAS")]
            Crédito15 = 3,
            [Display(Name = "CRÉDITO A 20 DÍAS")]
            Crédito20 = 8,
            [Display(Name = "CRÉDITO A 21 DÍAS")]
            Crédito21 = 10,
            [Display(Name = "CRÉDITO A 25 DÍAS")]
            Crédito25 = 11,
            [Display(Name = "CRÉDITO A 30 DÍAS")]
            Crédito30 = 4,
            [Display(Name = "CRÉDITO A 45 DÍAS")]
            Crédito45 = 9,
            [Display(Name = "CRÉDITO A 60 DÍAS")]
            Crédito60 = 5,
            [Display(Name = "CRÉDITO A 90 DÍAS")]
            Crédito90 = 6,
            [Display(Name = "CRÉDITO A 120 DÍAS")]
            Crédito120 = 7                   
        };

        public enum FormaPago
        {
            [Display(Name = "NO ASIGNADO")]
            NoAsignado = 0,
            [Display(Name = "EFECTIVO")]
            Efectivo = 1,
            [Display(Name = "CHEQUE")]
            Cheque = 2,
            [Display(Name = "LETRA")]
            Letra = 3,
            [Display(Name = "TARJETA DE CRÉDITO")]
            TarjetaCredito = 4,
            [Display(Name = "TARJETA DE DÉBITO")]
            TarjetaDebito = 5,
            [Display(Name = "DEPOSITO BANCARIO")]
            DepositoBancario = 6,
            [Display(Name = "TRANSFERENCIA INTERBANCARIA")]
            TransferenciaInterbancaria = 7
        };


        public TiposDocumentoIdentidad tipoDocumentoIdentidad { get; set; }
        public enum TiposDocumentoIdentidad
        {
            [Display(Name = "RUC")]
            RUC = 6,
            [Display(Name = "DNI")]
            DNI = 1,
            [Display(Name = "C.E.")]
            Carnet = 4,
        };
        public String tipoDocumentoIdentidadString
        {
            get
            {
                return EnumHelper<TiposDocumentoIdentidad>.GetDisplayValue(this.tipoDocumentoIdentidad);
            }
        }



        [Display(Name = "Correo Envío:")]
        public String correoEnvio { get; set; }

        [Display(Name = "Correo Copia:")]
        public String correoCopia { get; set; }

        [Display(Name = "Correo Oculato:")]
        public String correoOculto { get; set; }


        public String tipoDocumentoString
        {
            get
            {
                return EnumHelper<TipoDocumento>.GetDisplayValue(this.tipoDocumento);
            }
        }


        [Display(Name = "Tipo Documento:")]
        public TipoDocumento tipoDocumento { get; set; }


        public Venta venta { get; set; }

        public Guid idDocumentoVenta { get; set; }

        [Display(Name = "Número:")]
        public String numero { get; set; }

        [Display(Name = "Serie:")]
        public String serie { get; set; }

        [Display(Name = "Serie:")]
        public SerieDocumentoElectronico serieDocumentoElectronico { get; set; }

        [Display(Name = "Número Doc.:")]
        public String serieNumero { get { return this.serie + "-" + this.numero; } }


        public String descripcionError { get; set;  }

        public TiposErrorValidacion tiposErrorValidacion { get; set; }
        public String tiposErrorValidacionString
        {
            get
            {
                return EnumHelper<TiposErrorValidacion>.GetDisplayValue(this.tiposErrorValidacion);
            }
        }

        public enum TiposErrorValidacion
        {
            [Display(Name = "Ninguno")]
            NoExisteError = 0,
            [Display(Name = "Existen Documentos con Fecha Posterior")]
            ExisteDocumentoFechaPosterior = 1,
            [Display(Name = "Guía de Remisión ya se encuentra facturada")]
            ExisteGuiaRemision = 2,
            [Display(Name = "Usario no tiene permisos para crear facturas")]
            UsuarioNoAutorizado = 3
        }

        public TiposErrorSolicitudAnulacion tipoErrorSolicitudAnulacion { get; set; }
        public String tipoErrorSolicitudAnulacionString
        {
            get
            {
                return EnumHelper<TiposErrorSolicitudAnulacion>.GetDisplayValue(this.tipoErrorSolicitudAnulacion);
            }
        }

        public enum TiposErrorSolicitudAnulacion
        {
            [Display(Name = "Ninguno")]
            NoExisteError = 0,
            [Display(Name = "El documento de venta no permite anulación")]
            NoPermiteAnulacion = 1,
            [Display(Name = "Ya se ha solicitado anulación previamente")]
            ExisteSolicitudAnulacion = 2,
           
        }

        public enum TipoDocumento
        {
            [Display(Name = "Todos")]
            Todos = 0,
            [Display(Name = "Factura")]
            Factura = 1,
            [Display(Name = "Boleta de Venta")]
            BoletaVenta = 3,
            [Display(Name = "Nota de Crédito")]
            NotaCrédito = 7,
            [Display(Name = "Nota de Débito")]
            NotaDébito = 8
        };

       
        public CPE_CABECERA_BE cPE_CABECERA_BE;

        public List<CPE_DETALLE_BE> cPE_DETALLE_BEList;

        public List<CPE_DAT_ADIC_BE> cPE_DAT_ADIC_BEList;

        public List<CPE_DOC_REF_BE> cPE_DOC_REF_BEList;

        public List<CPE_ANTICIPO_BE> cPE_ANTICIPO_BEList;

        public List<CPE_FAC_GUIA_BE> cPE_FAC_GUIA_BEList;

        public List<CPE_DOC_ASOC_BE> cPE_DOC_ASOC_BEList;


        public CPE_RESPUESTA_BE cPE_RESPUESTA_BE { get; set; }


        public RPTA_DOC_TRIB_BE rPTA_DOC_TRIB_BE { get; set; }


        public RPTA_BE rPTA_BE { get; set; }

        


        public GlobalEnumTipoOnline globalEnumTipoOnline;
        


        [Display(Name = "Fecha Emisión Desde:")]
        public DateTime fechaEmisionDesde { get; set; }

        [Display(Name = "Fecha Emisión Hasta:")]
        public DateTime fechaEmisionHasta { get; set; }

        public Pedido pedido { get; set; }

        public GuiaRemision guiaRemision { get; set; }

        public NotaIngreso notaIngreso { get; set; }
        public String estadoDocumentoSunatString
        {
            get
            {
                return EnumHelper<EstadosDocumentoSunat>.GetDisplayValue(this.estadoDocumentoSunat);
            }
        }

        public EstadosDocumentoSunat estadoDocumentoSunat { get; set; }

        public enum EstadosDocumentoSunat
        {
            [Display(Name = "En Proceso")]
            EnProceso = 101,
            [Display(Name = "Aceptada")]
            Aceptado = 102,
            [Display(Name = "Aceptada con Obs.")]
            AceptadoConObs = 103,
            [Display(Name = "Rechazada")]
            Rechazado = 104,
            [Display(Name = "Anulada")]
            Anulado = 105,
            [Display(Name = "Exception")]
            Exception = 106,
            [Display(Name = "Anulación Solicitada (Solicitud de Baja)")]
            EnProcesoAnulacion = 108,
            [Display(Name = "No Identificado")]
            NoIdentificado = 0
        };


        public String estadoDocumentoSunatBusquedaString
        {
            get
            {
                return EnumHelper<EstadosDocumentoSunatBusqueda>.GetDisplayValue(this.estadoDocumentoSunatBusqueda);
            }
        }

        [Display(Name = "Estado:")]
        public EstadosDocumentoSunatBusqueda estadoDocumentoSunatBusqueda { get; set; }
        public enum EstadosDocumentoSunatBusqueda
        {
            [Display(Name = "Todos")]
            Todos = 0,
            [Display(Name = "En proceso")]
            EnProceso = 101,
            [Display(Name = "Aceptada")]
            TodosAceptados = 205,
            [Display(Name = "Rechazada")]
            Rechazado = 104,
            [Display(Name = "Anulación Solicitada (Solicitud de Baja)")]
            EnProcesoAnulacion = 108,
            [Display(Name = "Anulada")]
            Anulado = 105,
            [Display(Name = "Otros")]
            Otros = 1
        };






        public String tipoNotaCreditoString
        {
            get
            {
                return EnumHelper<TiposNotaCredito>.GetDisplayValue(this.tipoNotaCredito);
            }
        }

        [Display(Name = "Motivo Nota Crédito:")]
        public TiposNotaCredito tipoNotaCredito { get; set; }
        public enum TiposNotaCredito
        {
            [Display(Name = "ANULACIÓN DE LA OPERACIÓN")]
            AnulacionOperacion = 1,
            [Display(Name = "ANULACIÓN POR ERROR EN EL RUC")]
            AnulacionErrorRUC = 2,
            [Display(Name = "CORRECCIÓN POR ERROR EN LA DESCRIPCIÓN")]
            CorreccionErrorDescripcion = 3,
            [Display(Name = "DESCUENTO GLOBAL")]
            DescuentoGlobal = 4,
            [Display(Name = "DESCUENTO POR ITEM")]
            DescuentoItem = 5,
            [Display(Name = "DEVOLUCIÓN TOTAL")]
            DevolucionTotal = 6,
            [Display(Name = "DEVOLUCIÓN POR ITEM")]
            DevolucionItem = 7,
                /*,
            [Display(Name = "BONIFICACIÓN")]
            Bonificacion = 8,*/
            [Display(Name = "DISMINUCIÓN EN EL VALOR")]
            DisminucionValor = 9
                /*,
            [Display(Name = "OTROS CONCEPTOS")]
            OtrosConceptos = 10,*/
        };
        /*01 – Intereses por mora 02 – Aumento de valor 03 – Penalidades*/

        public String tipoNotaDebitoString
        {
            get
            {
                return EnumHelper<TiposNotaDebito>.GetDisplayValue(this.tipoNotaDebito);
            }
        }


        [Display(Name = "Motivo Nota Débito:")]
        public TiposNotaDebito tipoNotaDebito { get; set; }
        public enum TiposNotaDebito
        {

            /*01 – Intereses por mora 02 – Aumento de valor 03 – Penalidades*/
            [Display(Name = "INTERESES POR MORA")]
            Intereses = 1,
            [Display(Name = "AUMENTO EN EL VALOR")]
            AumentoValor = 2,
            [Display(Name = "PENALIDADES / OTROS CONCEPTOS")]
            Penalidades = 3
            /*,
        [Display(Name = "OTROS CONCEPTOS")]
        OtrosConceptos = 10,*/
        };



        public String comentarioAnulado { get; set; }





        public bool solicitadoAnulacion { get; set; }
        public String comentarioSolicitudAnulacion { get; set; }
        public String comentarioAprobacionAnulacion { get; set; }



        /*
        [Display(Name = "Pedido:")]
        public Pedido pedido { get; set; }

        [Display(Name = "Guía Remisión:")]
        public GuiaRemision guiaRemision { get; set; }

        [Display(Name = "Cotización:")]
        public Cotizacion cotizacion { get; set; }*/

        [Display(Name = "Sede MP:")]
        public Ciudad ciudad { get; set; }

        [Display(Name = "Cliente:")]
        public Cliente cliente { get; set; }

        [Display(Name = "Tasa IGV:")]
        public Decimal tasaIGV { get; set; }

        [Display(Name = "Sub Total:")]
        public Decimal subTotal { get; set; }

        [Display(Name = "Total:")]
        public Decimal total { get; set; }

        [Display(Name = "Observaciones Factura:")]
        public String observaciones { get; set; }

        [Display(Name = "Observaciones para uso interno:")]
        public String observacionesUsoInterno { get; set; }

        [Display(Name = "Está Anulado:")]
        public Boolean esAnulado { get; set; }

        [Display(Name = "Usuario:")]
        public Usuario usuario { get; set; }

        public MovimientoAlmacen movimientoAlmacen { get; set; }

        public List<VentaDetalle> ventaDetalleList { get; set; }

        [Display(Name = "N° Doc Referencia Adicional:")]
        public String documentoReferenciaAdicional { get; set; }



        public byte[] cpeFile { get; set; }

        public byte[] cdrFile { get; set; }

        public bool permiteAnulacion { get; set; }

        public bool tieneNotaCredito { get; set; }

        public bool tieneNotaDebito { get; set; }

        public int idGrupoCliente { get; set; }

        public bool buscarSedesGrupoCliente { get; set; }

        public Usuario usuarioSolicitudAnulacion { get; set; }

        /**
         * Utilizado para búsqueda
         */
        [Display(Name = "SKU:")]
        public String sku { get; set; }

        /***************************/

        public String razon_social { get; set; }
        public String usuario_solicitud { get; set; }
        public String nombre { get; set; }
        public String fecha_solicitud { get; set; }
        public String monto { get; set; }
        public String numero_factura { get; set; }
        public String ruc { get; set; }
        public String contacto { get; set; }
        public int estado_anulacion { get; set; }

        public string telefonoContacto { get; set; }
    }
}

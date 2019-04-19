﻿using Model.ServiceReferencePSE;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Model
{
    public class DocumentoCompra : Auditoria , IDocumentoPago
    {

        public DocumentoCompra()
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


        public Compra compra { get; set; }

        public int idDocumentoCompra { get; set; }

        [Display(Name = "Número:")]
        public String numero { get; set; }

        [Display(Name = "Serie:")]
        public String serie { get; set; }

        [Display(Name = "Serie:")]
        public SerieDocumentoElectronico serieDocumentoElectronico { get; set; }

        [Display(Name = "Número Doc.:")]
        public String serieNumero { get { return this.serie + "-" + this.numero; } }


        public String descripcionError { get; set; }

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


        public CPE_CABECERA_COMPRA cPE_CABECERA_COMPRA;

        public List<CPE_DETALLE_COMPRA> cPE_DETALLE_COMPRAList;

        public List<CPE_DAT_ADIC_COMPRA> cPE_DAT_ADIC_COMPRAList;

        public List<CPE_DOC_REF_COMPRA> cPE_DOC_REF_COMPRAList;

        public List<CPE_ANTICIPO_COMPRA> cPE_ANTICIPO_COMPRAList;

        public List<CPE_FAC_GUIA_COMPRA> cPE_FAC_GUIA_COMPRAList;

        public List<CPE_DOC_ASOC_COMPRA> cPE_DOC_ASOC_COMPRAList;


        public CPE_RESPUESTA_COMPRA cPE_RESPUESTA_COMPRA { get; set; }


        public RPTA_DOC_TRIB_COMPRA rPTA_DOC_TRIB_COMPRA { get; set; }


        public RPTA_COMPRA rPTA_COMPRA { get; set; }




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
        public Proveedor proveedor { get; set; }

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


        public int idGrupoCliente { get; set; }

        public bool buscarSedesGrupoCliente { get; set; }


        /**
         * Utilizado para búsqueda
         */
        [Display(Name = "SKU:")]
        public String sku { get; set; }
    }






    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "CPE_CABECERA_COMPRA", Namespace = "http://schemas.datacontract.org/2004/07/TT.EOL.Level.BE")]
    [System.SerializableAttribute()]
    public partial class CPE_CABECERA_COMPRA : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
    {

        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string IDField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string COD_GPOField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TIP_CPEField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string COD_OPE_LEYField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FEC_EMIField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string HOR_EMIField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string SERIEField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CORRELATIVOField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FEC_VCTOField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MONEDAField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string COD_TIP_OPEField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TIP_DOC_EMIField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NRO_DOC_EMIField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NOM_EMIField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NOM_COM_EMIField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string COD_LOC_EMIField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TIP_DOC_RCTField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NRO_DOC_RCTField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NOM_RCTField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DIR_DES_RCTField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string UBI_RCTField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NUM_REG_MTCField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_REFField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string COD_MND_REFField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string UBI_ENTField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DIR_DES_ENTField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DIR_URB_ENTField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DIR_PAI_ENTField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TIP_PAGField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FRM_PAGField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NRO_ORD_COMField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string COD_TIP_GREField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NRO_GREField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string COD_OPCField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FMT_IMPRField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string IMPRESORAField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_DCTO_GLBField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FAC_DCTO_GLBField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_CARG_GLBField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FAC_CARG_GLOBField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TIP_CARG_GLOBField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_PERField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TIP_PERField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FAC_PERField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_IMPField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_GRVField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_INFField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_EXRField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_GRTField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_EXPField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_ISCField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_TRB_IGVField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_TRB_ISCField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_TRB_OTRField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_VAL_VTAField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_PRC_VTAField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_DCTOField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_OTR_CGOField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOTField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_ANTCPField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TIP_CMBField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_DCTO_GLB_NACField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_CARG_GLB_NACField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_PER_NACField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_IMP_NACField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_GRV_NACField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_INF_NACField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_EXR_NACField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_GRT_NACField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_EXP_NACField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_ISC_NACField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_TRB_IGV_NACField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_TRB_ISC_NACField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_TRB_OTR_NACField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_VAL_VTA_NACField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_PRC_VTA_NACField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_DCTO_NACField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_OTR_CGO_NACField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_NACField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_ANTCP_NACField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_IMPTO_PERField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_MAS_PERField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string COD_TIP_DETRACCIONField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_DETRACCIONField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FAC_DETRACCIONField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string COD_SOF_FACTField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string COD_TIP_NCField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string COD_TIP_NDField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DES_MTVO_NC_NDField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CORREO_ENVIOField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CORREO_COPIAField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CORREO_OCULTOField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PTO_VTAField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FLG_TIP_CAMBIOField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string COD_PROCEDENCIAField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ID_EXT_RZNField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ETD_SNTField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DES_REF_CLTField;

        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ID
        {
            get
            {
                return this.IDField;
            }
            set
            {
                if ((object.ReferenceEquals(this.IDField, value) != true))
                {
                    this.IDField = value;
                    this.RaisePropertyChanged("ID");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 1)]
        public string COD_GPO
        {
            get
            {
                return this.COD_GPOField;
            }
            set
            {
                if ((object.ReferenceEquals(this.COD_GPOField, value) != true))
                {
                    this.COD_GPOField = value;
                    this.RaisePropertyChanged("COD_GPO");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 2)]
        public string TIP_CPE
        {
            get
            {
                return this.TIP_CPEField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TIP_CPEField, value) != true))
                {
                    this.TIP_CPEField = value;
                    this.RaisePropertyChanged("TIP_CPE");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 3)]
        public string COD_OPE_LEY
        {
            get
            {
                return this.COD_OPE_LEYField;
            }
            set
            {
                if ((object.ReferenceEquals(this.COD_OPE_LEYField, value) != true))
                {
                    this.COD_OPE_LEYField = value;
                    this.RaisePropertyChanged("COD_OPE_LEY");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
        public string FEC_EMI
        {
            get
            {
                return this.FEC_EMIField;
            }
            set
            {
                if ((object.ReferenceEquals(this.FEC_EMIField, value) != true))
                {
                    this.FEC_EMIField = value;
                    this.RaisePropertyChanged("FEC_EMI");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 5)]
        public string HOR_EMI
        {
            get
            {
                return this.HOR_EMIField;
            }
            set
            {
                if ((object.ReferenceEquals(this.HOR_EMIField, value) != true))
                {
                    this.HOR_EMIField = value;
                    this.RaisePropertyChanged("HOR_EMI");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 6)]
        public string SERIE
        {
            get
            {
                return this.SERIEField;
            }
            set
            {
                if ((object.ReferenceEquals(this.SERIEField, value) != true))
                {
                    this.SERIEField = value;
                    this.RaisePropertyChanged("SERIE");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 7)]
        public string CORRELATIVO
        {
            get
            {
                return this.CORRELATIVOField;
            }
            set
            {
                if ((object.ReferenceEquals(this.CORRELATIVOField, value) != true))
                {
                    this.CORRELATIVOField = value;
                    this.RaisePropertyChanged("CORRELATIVO");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 8)]
        public string FEC_VCTO
        {
            get
            {
                return this.FEC_VCTOField;
            }
            set
            {
                if ((object.ReferenceEquals(this.FEC_VCTOField, value) != true))
                {
                    this.FEC_VCTOField = value;
                    this.RaisePropertyChanged("FEC_VCTO");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 9)]
        public string MONEDA
        {
            get
            {
                return this.MONEDAField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MONEDAField, value) != true))
                {
                    this.MONEDAField = value;
                    this.RaisePropertyChanged("MONEDA");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 10)]
        public string COD_TIP_OPE
        {
            get
            {
                return this.COD_TIP_OPEField;
            }
            set
            {
                if ((object.ReferenceEquals(this.COD_TIP_OPEField, value) != true))
                {
                    this.COD_TIP_OPEField = value;
                    this.RaisePropertyChanged("COD_TIP_OPE");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 11)]
        public string TIP_DOC_EMI
        {
            get
            {
                return this.TIP_DOC_EMIField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TIP_DOC_EMIField, value) != true))
                {
                    this.TIP_DOC_EMIField = value;
                    this.RaisePropertyChanged("TIP_DOC_EMI");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 12)]
        public string NRO_DOC_EMI
        {
            get
            {
                return this.NRO_DOC_EMIField;
            }
            set
            {
                if ((object.ReferenceEquals(this.NRO_DOC_EMIField, value) != true))
                {
                    this.NRO_DOC_EMIField = value;
                    this.RaisePropertyChanged("NRO_DOC_EMI");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 13)]
        public string NOM_EMI
        {
            get
            {
                return this.NOM_EMIField;
            }
            set
            {
                if ((object.ReferenceEquals(this.NOM_EMIField, value) != true))
                {
                    this.NOM_EMIField = value;
                    this.RaisePropertyChanged("NOM_EMI");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 14)]
        public string NOM_COM_EMI
        {
            get
            {
                return this.NOM_COM_EMIField;
            }
            set
            {
                if ((object.ReferenceEquals(this.NOM_COM_EMIField, value) != true))
                {
                    this.NOM_COM_EMIField = value;
                    this.RaisePropertyChanged("NOM_COM_EMI");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 15)]
        public string COD_LOC_EMI
        {
            get
            {
                return this.COD_LOC_EMIField;
            }
            set
            {
                if ((object.ReferenceEquals(this.COD_LOC_EMIField, value) != true))
                {
                    this.COD_LOC_EMIField = value;
                    this.RaisePropertyChanged("COD_LOC_EMI");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 16)]
        public string TIP_DOC_RCT
        {
            get
            {
                return this.TIP_DOC_RCTField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TIP_DOC_RCTField, value) != true))
                {
                    this.TIP_DOC_RCTField = value;
                    this.RaisePropertyChanged("TIP_DOC_RCT");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 17)]
        public string NRO_DOC_RCT
        {
            get
            {
                return this.NRO_DOC_RCTField;
            }
            set
            {
                if ((object.ReferenceEquals(this.NRO_DOC_RCTField, value) != true))
                {
                    this.NRO_DOC_RCTField = value;
                    this.RaisePropertyChanged("NRO_DOC_RCT");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 18)]
        public string NOM_RCT
        {
            get
            {
                return this.NOM_RCTField;
            }
            set
            {
                if ((object.ReferenceEquals(this.NOM_RCTField, value) != true))
                {
                    this.NOM_RCTField = value;
                    this.RaisePropertyChanged("NOM_RCT");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 19)]
        public string DIR_DES_RCT
        {
            get
            {
                return this.DIR_DES_RCTField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DIR_DES_RCTField, value) != true))
                {
                    this.DIR_DES_RCTField = value;
                    this.RaisePropertyChanged("DIR_DES_RCT");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 20)]
        public string UBI_RCT
        {
            get
            {
                return this.UBI_RCTField;
            }
            set
            {
                if ((object.ReferenceEquals(this.UBI_RCTField, value) != true))
                {
                    this.UBI_RCTField = value;
                    this.RaisePropertyChanged("UBI_RCT");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 21)]
        public string NUM_REG_MTC
        {
            get
            {
                return this.NUM_REG_MTCField;
            }
            set
            {
                if ((object.ReferenceEquals(this.NUM_REG_MTCField, value) != true))
                {
                    this.NUM_REG_MTCField = value;
                    this.RaisePropertyChanged("NUM_REG_MTC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 22)]
        public string MNT_REF
        {
            get
            {
                return this.MNT_REFField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_REFField, value) != true))
                {
                    this.MNT_REFField = value;
                    this.RaisePropertyChanged("MNT_REF");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 23)]
        public string COD_MND_REF
        {
            get
            {
                return this.COD_MND_REFField;
            }
            set
            {
                if ((object.ReferenceEquals(this.COD_MND_REFField, value) != true))
                {
                    this.COD_MND_REFField = value;
                    this.RaisePropertyChanged("COD_MND_REF");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 24)]
        public string UBI_ENT
        {
            get
            {
                return this.UBI_ENTField;
            }
            set
            {
                if ((object.ReferenceEquals(this.UBI_ENTField, value) != true))
                {
                    this.UBI_ENTField = value;
                    this.RaisePropertyChanged("UBI_ENT");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 25)]
        public string DIR_DES_ENT
        {
            get
            {
                return this.DIR_DES_ENTField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DIR_DES_ENTField, value) != true))
                {
                    this.DIR_DES_ENTField = value;
                    this.RaisePropertyChanged("DIR_DES_ENT");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 26)]
        public string DIR_URB_ENT
        {
            get
            {
                return this.DIR_URB_ENTField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DIR_URB_ENTField, value) != true))
                {
                    this.DIR_URB_ENTField = value;
                    this.RaisePropertyChanged("DIR_URB_ENT");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 27)]
        public string DIR_PAI_ENT
        {
            get
            {
                return this.DIR_PAI_ENTField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DIR_PAI_ENTField, value) != true))
                {
                    this.DIR_PAI_ENTField = value;
                    this.RaisePropertyChanged("DIR_PAI_ENT");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 28)]
        public string TIP_PAG
        {
            get
            {
                return this.TIP_PAGField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TIP_PAGField, value) != true))
                {
                    this.TIP_PAGField = value;
                    this.RaisePropertyChanged("TIP_PAG");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 29)]
        public string FRM_PAG
        {
            get
            {
                return this.FRM_PAGField;
            }
            set
            {
                if ((object.ReferenceEquals(this.FRM_PAGField, value) != true))
                {
                    this.FRM_PAGField = value;
                    this.RaisePropertyChanged("FRM_PAG");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 30)]
        public string NRO_ORD_COM
        {
            get
            {
                return this.NRO_ORD_COMField;
            }
            set
            {
                if ((object.ReferenceEquals(this.NRO_ORD_COMField, value) != true))
                {
                    this.NRO_ORD_COMField = value;
                    this.RaisePropertyChanged("NRO_ORD_COM");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 31)]
        public string COD_TIP_GRE
        {
            get
            {
                return this.COD_TIP_GREField;
            }
            set
            {
                if ((object.ReferenceEquals(this.COD_TIP_GREField, value) != true))
                {
                    this.COD_TIP_GREField = value;
                    this.RaisePropertyChanged("COD_TIP_GRE");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 32)]
        public string NRO_GRE
        {
            get
            {
                return this.NRO_GREField;
            }
            set
            {
                if ((object.ReferenceEquals(this.NRO_GREField, value) != true))
                {
                    this.NRO_GREField = value;
                    this.RaisePropertyChanged("NRO_GRE");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 33)]
        public string COD_OPC
        {
            get
            {
                return this.COD_OPCField;
            }
            set
            {
                if ((object.ReferenceEquals(this.COD_OPCField, value) != true))
                {
                    this.COD_OPCField = value;
                    this.RaisePropertyChanged("COD_OPC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 34)]
        public string FMT_IMPR
        {
            get
            {
                return this.FMT_IMPRField;
            }
            set
            {
                if ((object.ReferenceEquals(this.FMT_IMPRField, value) != true))
                {
                    this.FMT_IMPRField = value;
                    this.RaisePropertyChanged("FMT_IMPR");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 35)]
        public string IMPRESORA
        {
            get
            {
                return this.IMPRESORAField;
            }
            set
            {
                if ((object.ReferenceEquals(this.IMPRESORAField, value) != true))
                {
                    this.IMPRESORAField = value;
                    this.RaisePropertyChanged("IMPRESORA");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 36)]
        public string MNT_DCTO_GLB
        {
            get
            {
                return this.MNT_DCTO_GLBField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_DCTO_GLBField, value) != true))
                {
                    this.MNT_DCTO_GLBField = value;
                    this.RaisePropertyChanged("MNT_DCTO_GLB");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 37)]
        public string FAC_DCTO_GLB
        {
            get
            {
                return this.FAC_DCTO_GLBField;
            }
            set
            {
                if ((object.ReferenceEquals(this.FAC_DCTO_GLBField, value) != true))
                {
                    this.FAC_DCTO_GLBField = value;
                    this.RaisePropertyChanged("FAC_DCTO_GLB");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 38)]
        public string MNT_CARG_GLB
        {
            get
            {
                return this.MNT_CARG_GLBField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_CARG_GLBField, value) != true))
                {
                    this.MNT_CARG_GLBField = value;
                    this.RaisePropertyChanged("MNT_CARG_GLB");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 39)]
        public string FAC_CARG_GLOB
        {
            get
            {
                return this.FAC_CARG_GLOBField;
            }
            set
            {
                if ((object.ReferenceEquals(this.FAC_CARG_GLOBField, value) != true))
                {
                    this.FAC_CARG_GLOBField = value;
                    this.RaisePropertyChanged("FAC_CARG_GLOB");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 40)]
        public string TIP_CARG_GLOB
        {
            get
            {
                return this.TIP_CARG_GLOBField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TIP_CARG_GLOBField, value) != true))
                {
                    this.TIP_CARG_GLOBField = value;
                    this.RaisePropertyChanged("TIP_CARG_GLOB");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 41)]
        public string MNT_TOT_PER
        {
            get
            {
                return this.MNT_TOT_PERField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_PERField, value) != true))
                {
                    this.MNT_TOT_PERField = value;
                    this.RaisePropertyChanged("MNT_TOT_PER");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 42)]
        public string TIP_PER
        {
            get
            {
                return this.TIP_PERField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TIP_PERField, value) != true))
                {
                    this.TIP_PERField = value;
                    this.RaisePropertyChanged("TIP_PER");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 43)]
        public string FAC_PER
        {
            get
            {
                return this.FAC_PERField;
            }
            set
            {
                if ((object.ReferenceEquals(this.FAC_PERField, value) != true))
                {
                    this.FAC_PERField = value;
                    this.RaisePropertyChanged("FAC_PER");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 44)]
        public string MNT_TOT_IMP
        {
            get
            {
                return this.MNT_TOT_IMPField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_IMPField, value) != true))
                {
                    this.MNT_TOT_IMPField = value;
                    this.RaisePropertyChanged("MNT_TOT_IMP");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 45)]
        public string MNT_TOT_GRV
        {
            get
            {
                return this.MNT_TOT_GRVField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_GRVField, value) != true))
                {
                    this.MNT_TOT_GRVField = value;
                    this.RaisePropertyChanged("MNT_TOT_GRV");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 46)]
        public string MNT_TOT_INF
        {
            get
            {
                return this.MNT_TOT_INFField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_INFField, value) != true))
                {
                    this.MNT_TOT_INFField = value;
                    this.RaisePropertyChanged("MNT_TOT_INF");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 47)]
        public string MNT_TOT_EXR
        {
            get
            {
                return this.MNT_TOT_EXRField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_EXRField, value) != true))
                {
                    this.MNT_TOT_EXRField = value;
                    this.RaisePropertyChanged("MNT_TOT_EXR");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 48)]
        public string MNT_TOT_GRT
        {
            get
            {
                return this.MNT_TOT_GRTField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_GRTField, value) != true))
                {
                    this.MNT_TOT_GRTField = value;
                    this.RaisePropertyChanged("MNT_TOT_GRT");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 49)]
        public string MNT_TOT_EXP
        {
            get
            {
                return this.MNT_TOT_EXPField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_EXPField, value) != true))
                {
                    this.MNT_TOT_EXPField = value;
                    this.RaisePropertyChanged("MNT_TOT_EXP");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 50)]
        public string MNT_TOT_ISC
        {
            get
            {
                return this.MNT_TOT_ISCField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_ISCField, value) != true))
                {
                    this.MNT_TOT_ISCField = value;
                    this.RaisePropertyChanged("MNT_TOT_ISC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 51)]
        public string MNT_TOT_TRB_IGV
        {
            get
            {
                return this.MNT_TOT_TRB_IGVField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_TRB_IGVField, value) != true))
                {
                    this.MNT_TOT_TRB_IGVField = value;
                    this.RaisePropertyChanged("MNT_TOT_TRB_IGV");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 52)]
        public string MNT_TOT_TRB_ISC
        {
            get
            {
                return this.MNT_TOT_TRB_ISCField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_TRB_ISCField, value) != true))
                {
                    this.MNT_TOT_TRB_ISCField = value;
                    this.RaisePropertyChanged("MNT_TOT_TRB_ISC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 53)]
        public string MNT_TOT_TRB_OTR
        {
            get
            {
                return this.MNT_TOT_TRB_OTRField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_TRB_OTRField, value) != true))
                {
                    this.MNT_TOT_TRB_OTRField = value;
                    this.RaisePropertyChanged("MNT_TOT_TRB_OTR");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 54)]
        public string MNT_TOT_VAL_VTA
        {
            get
            {
                return this.MNT_TOT_VAL_VTAField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_VAL_VTAField, value) != true))
                {
                    this.MNT_TOT_VAL_VTAField = value;
                    this.RaisePropertyChanged("MNT_TOT_VAL_VTA");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 55)]
        public string MNT_TOT_PRC_VTA
        {
            get
            {
                return this.MNT_TOT_PRC_VTAField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_PRC_VTAField, value) != true))
                {
                    this.MNT_TOT_PRC_VTAField = value;
                    this.RaisePropertyChanged("MNT_TOT_PRC_VTA");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 56)]
        public string MNT_TOT_DCTO
        {
            get
            {
                return this.MNT_TOT_DCTOField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_DCTOField, value) != true))
                {
                    this.MNT_TOT_DCTOField = value;
                    this.RaisePropertyChanged("MNT_TOT_DCTO");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 57)]
        public string MNT_TOT_OTR_CGO
        {
            get
            {
                return this.MNT_TOT_OTR_CGOField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_OTR_CGOField, value) != true))
                {
                    this.MNT_TOT_OTR_CGOField = value;
                    this.RaisePropertyChanged("MNT_TOT_OTR_CGO");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 58)]
        public string MNT_TOT
        {
            get
            {
                return this.MNT_TOTField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOTField, value) != true))
                {
                    this.MNT_TOTField = value;
                    this.RaisePropertyChanged("MNT_TOT");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 59)]
        public string MNT_TOT_ANTCP
        {
            get
            {
                return this.MNT_TOT_ANTCPField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_ANTCPField, value) != true))
                {
                    this.MNT_TOT_ANTCPField = value;
                    this.RaisePropertyChanged("MNT_TOT_ANTCP");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 60)]
        public string TIP_CMB
        {
            get
            {
                return this.TIP_CMBField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TIP_CMBField, value) != true))
                {
                    this.TIP_CMBField = value;
                    this.RaisePropertyChanged("TIP_CMB");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 61)]
        public string MNT_DCTO_GLB_NAC
        {
            get
            {
                return this.MNT_DCTO_GLB_NACField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_DCTO_GLB_NACField, value) != true))
                {
                    this.MNT_DCTO_GLB_NACField = value;
                    this.RaisePropertyChanged("MNT_DCTO_GLB_NAC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 62)]
        public string MNT_CARG_GLB_NAC
        {
            get
            {
                return this.MNT_CARG_GLB_NACField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_CARG_GLB_NACField, value) != true))
                {
                    this.MNT_CARG_GLB_NACField = value;
                    this.RaisePropertyChanged("MNT_CARG_GLB_NAC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 63)]
        public string MNT_TOT_PER_NAC
        {
            get
            {
                return this.MNT_TOT_PER_NACField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_PER_NACField, value) != true))
                {
                    this.MNT_TOT_PER_NACField = value;
                    this.RaisePropertyChanged("MNT_TOT_PER_NAC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 64)]
        public string MNT_TOT_IMP_NAC
        {
            get
            {
                return this.MNT_TOT_IMP_NACField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_IMP_NACField, value) != true))
                {
                    this.MNT_TOT_IMP_NACField = value;
                    this.RaisePropertyChanged("MNT_TOT_IMP_NAC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 65)]
        public string MNT_TOT_GRV_NAC
        {
            get
            {
                return this.MNT_TOT_GRV_NACField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_GRV_NACField, value) != true))
                {
                    this.MNT_TOT_GRV_NACField = value;
                    this.RaisePropertyChanged("MNT_TOT_GRV_NAC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 66)]
        public string MNT_TOT_INF_NAC
        {
            get
            {
                return this.MNT_TOT_INF_NACField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_INF_NACField, value) != true))
                {
                    this.MNT_TOT_INF_NACField = value;
                    this.RaisePropertyChanged("MNT_TOT_INF_NAC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 67)]
        public string MNT_TOT_EXR_NAC
        {
            get
            {
                return this.MNT_TOT_EXR_NACField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_EXR_NACField, value) != true))
                {
                    this.MNT_TOT_EXR_NACField = value;
                    this.RaisePropertyChanged("MNT_TOT_EXR_NAC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 68)]
        public string MNT_TOT_GRT_NAC
        {
            get
            {
                return this.MNT_TOT_GRT_NACField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_GRT_NACField, value) != true))
                {
                    this.MNT_TOT_GRT_NACField = value;
                    this.RaisePropertyChanged("MNT_TOT_GRT_NAC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 69)]
        public string MNT_TOT_EXP_NAC
        {
            get
            {
                return this.MNT_TOT_EXP_NACField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_EXP_NACField, value) != true))
                {
                    this.MNT_TOT_EXP_NACField = value;
                    this.RaisePropertyChanged("MNT_TOT_EXP_NAC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 70)]
        public string MNT_TOT_ISC_NAC
        {
            get
            {
                return this.MNT_TOT_ISC_NACField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_ISC_NACField, value) != true))
                {
                    this.MNT_TOT_ISC_NACField = value;
                    this.RaisePropertyChanged("MNT_TOT_ISC_NAC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 71)]
        public string MNT_TOT_TRB_IGV_NAC
        {
            get
            {
                return this.MNT_TOT_TRB_IGV_NACField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_TRB_IGV_NACField, value) != true))
                {
                    this.MNT_TOT_TRB_IGV_NACField = value;
                    this.RaisePropertyChanged("MNT_TOT_TRB_IGV_NAC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 72)]
        public string MNT_TOT_TRB_ISC_NAC
        {
            get
            {
                return this.MNT_TOT_TRB_ISC_NACField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_TRB_ISC_NACField, value) != true))
                {
                    this.MNT_TOT_TRB_ISC_NACField = value;
                    this.RaisePropertyChanged("MNT_TOT_TRB_ISC_NAC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 73)]
        public string MNT_TOT_TRB_OTR_NAC
        {
            get
            {
                return this.MNT_TOT_TRB_OTR_NACField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_TRB_OTR_NACField, value) != true))
                {
                    this.MNT_TOT_TRB_OTR_NACField = value;
                    this.RaisePropertyChanged("MNT_TOT_TRB_OTR_NAC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 74)]
        public string MNT_TOT_VAL_VTA_NAC
        {
            get
            {
                return this.MNT_TOT_VAL_VTA_NACField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_VAL_VTA_NACField, value) != true))
                {
                    this.MNT_TOT_VAL_VTA_NACField = value;
                    this.RaisePropertyChanged("MNT_TOT_VAL_VTA_NAC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 75)]
        public string MNT_TOT_PRC_VTA_NAC
        {
            get
            {
                return this.MNT_TOT_PRC_VTA_NACField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_PRC_VTA_NACField, value) != true))
                {
                    this.MNT_TOT_PRC_VTA_NACField = value;
                    this.RaisePropertyChanged("MNT_TOT_PRC_VTA_NAC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 76)]
        public string MNT_TOT_DCTO_NAC
        {
            get
            {
                return this.MNT_TOT_DCTO_NACField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_DCTO_NACField, value) != true))
                {
                    this.MNT_TOT_DCTO_NACField = value;
                    this.RaisePropertyChanged("MNT_TOT_DCTO_NAC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 77)]
        public string MNT_TOT_OTR_CGO_NAC
        {
            get
            {
                return this.MNT_TOT_OTR_CGO_NACField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_OTR_CGO_NACField, value) != true))
                {
                    this.MNT_TOT_OTR_CGO_NACField = value;
                    this.RaisePropertyChanged("MNT_TOT_OTR_CGO_NAC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 78)]
        public string MNT_TOT_NAC
        {
            get
            {
                return this.MNT_TOT_NACField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_NACField, value) != true))
                {
                    this.MNT_TOT_NACField = value;
                    this.RaisePropertyChanged("MNT_TOT_NAC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 79)]
        public string MNT_TOT_ANTCP_NAC
        {
            get
            {
                return this.MNT_TOT_ANTCP_NACField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_ANTCP_NACField, value) != true))
                {
                    this.MNT_TOT_ANTCP_NACField = value;
                    this.RaisePropertyChanged("MNT_TOT_ANTCP_NAC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 80)]
        public string MNT_IMPTO_PER
        {
            get
            {
                return this.MNT_IMPTO_PERField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_IMPTO_PERField, value) != true))
                {
                    this.MNT_IMPTO_PERField = value;
                    this.RaisePropertyChanged("MNT_IMPTO_PER");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 81)]
        public string MNT_TOT_MAS_PER
        {
            get
            {
                return this.MNT_TOT_MAS_PERField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_MAS_PERField, value) != true))
                {
                    this.MNT_TOT_MAS_PERField = value;
                    this.RaisePropertyChanged("MNT_TOT_MAS_PER");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 82)]
        public string COD_TIP_DETRACCION
        {
            get
            {
                return this.COD_TIP_DETRACCIONField;
            }
            set
            {
                if ((object.ReferenceEquals(this.COD_TIP_DETRACCIONField, value) != true))
                {
                    this.COD_TIP_DETRACCIONField = value;
                    this.RaisePropertyChanged("COD_TIP_DETRACCION");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 83)]
        public string MNT_TOT_DETRACCION
        {
            get
            {
                return this.MNT_TOT_DETRACCIONField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_DETRACCIONField, value) != true))
                {
                    this.MNT_TOT_DETRACCIONField = value;
                    this.RaisePropertyChanged("MNT_TOT_DETRACCION");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 84)]
        public string FAC_DETRACCION
        {
            get
            {
                return this.FAC_DETRACCIONField;
            }
            set
            {
                if ((object.ReferenceEquals(this.FAC_DETRACCIONField, value) != true))
                {
                    this.FAC_DETRACCIONField = value;
                    this.RaisePropertyChanged("FAC_DETRACCION");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 85)]
        public string COD_SOF_FACT
        {
            get
            {
                return this.COD_SOF_FACTField;
            }
            set
            {
                if ((object.ReferenceEquals(this.COD_SOF_FACTField, value) != true))
                {
                    this.COD_SOF_FACTField = value;
                    this.RaisePropertyChanged("COD_SOF_FACT");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 86)]
        public string COD_TIP_NC
        {
            get
            {
                return this.COD_TIP_NCField;
            }
            set
            {
                if ((object.ReferenceEquals(this.COD_TIP_NCField, value) != true))
                {
                    this.COD_TIP_NCField = value;
                    this.RaisePropertyChanged("COD_TIP_NC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 87)]
        public string COD_TIP_ND
        {
            get
            {
                return this.COD_TIP_NDField;
            }
            set
            {
                if ((object.ReferenceEquals(this.COD_TIP_NDField, value) != true))
                {
                    this.COD_TIP_NDField = value;
                    this.RaisePropertyChanged("COD_TIP_ND");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 88)]
        public string DES_MTVO_NC_ND
        {
            get
            {
                return this.DES_MTVO_NC_NDField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DES_MTVO_NC_NDField, value) != true))
                {
                    this.DES_MTVO_NC_NDField = value;
                    this.RaisePropertyChanged("DES_MTVO_NC_ND");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 89)]
        public string CORREO_ENVIO
        {
            get
            {
                return this.CORREO_ENVIOField;
            }
            set
            {
                if ((object.ReferenceEquals(this.CORREO_ENVIOField, value) != true))
                {
                    this.CORREO_ENVIOField = value;
                    this.RaisePropertyChanged("CORREO_ENVIO");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 90)]
        public string CORREO_COPIA
        {
            get
            {
                return this.CORREO_COPIAField;
            }
            set
            {
                if ((object.ReferenceEquals(this.CORREO_COPIAField, value) != true))
                {
                    this.CORREO_COPIAField = value;
                    this.RaisePropertyChanged("CORREO_COPIA");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 91)]
        public string CORREO_OCULTO
        {
            get
            {
                return this.CORREO_OCULTOField;
            }
            set
            {
                if ((object.ReferenceEquals(this.CORREO_OCULTOField, value) != true))
                {
                    this.CORREO_OCULTOField = value;
                    this.RaisePropertyChanged("CORREO_OCULTO");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 92)]
        public string PTO_VTA
        {
            get
            {
                return this.PTO_VTAField;
            }
            set
            {
                if ((object.ReferenceEquals(this.PTO_VTAField, value) != true))
                {
                    this.PTO_VTAField = value;
                    this.RaisePropertyChanged("PTO_VTA");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 93)]
        public string FLG_TIP_CAMBIO
        {
            get
            {
                return this.FLG_TIP_CAMBIOField;
            }
            set
            {
                if ((object.ReferenceEquals(this.FLG_TIP_CAMBIOField, value) != true))
                {
                    this.FLG_TIP_CAMBIOField = value;
                    this.RaisePropertyChanged("FLG_TIP_CAMBIO");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 94)]
        public string COD_PROCEDENCIA
        {
            get
            {
                return this.COD_PROCEDENCIAField;
            }
            set
            {
                if ((object.ReferenceEquals(this.COD_PROCEDENCIAField, value) != true))
                {
                    this.COD_PROCEDENCIAField = value;
                    this.RaisePropertyChanged("COD_PROCEDENCIA");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 95)]
        public string ID_EXT_RZN
        {
            get
            {
                return this.ID_EXT_RZNField;
            }
            set
            {
                if ((object.ReferenceEquals(this.ID_EXT_RZNField, value) != true))
                {
                    this.ID_EXT_RZNField = value;
                    this.RaisePropertyChanged("ID_EXT_RZN");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 96)]
        public string ETD_SNT
        {
            get
            {
                return this.ETD_SNTField;
            }
            set
            {
                if ((object.ReferenceEquals(this.ETD_SNTField, value) != true))
                {
                    this.ETD_SNTField = value;
                    this.RaisePropertyChanged("ETD_SNT");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 97)]
        public string DES_REF_CLT
        {
            get
            {
                return this.DES_REF_CLTField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DES_REF_CLTField, value) != true))
                {
                    this.DES_REF_CLTField = value;
                    this.RaisePropertyChanged("DES_REF_CLT");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "CPE_DETALLE_COMPRA", Namespace = "http://schemas.datacontract.org/2004/07/TT.EOL.Level.BE")]
    [System.SerializableAttribute()]
    public partial class CPE_DETALLE_COMPRA : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
    {

        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string LIN_ITMField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string COD_UND_ITMField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CANT_UND_ITMField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string VAL_VTA_ITMField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PRC_VTA_UND_ITMField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string VAL_UNIT_ITMField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_IGV_ITMField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string POR_IGV_ITMField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PRC_VTA_ITEMField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string VAL_VTA_BRT_ITEMField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string COD_TIP_AFECT_IGV_ITMField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_ISC_ITMField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string POR_ISC_ITMField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string COD_TIP_SIST_ISCField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PRECIO_SUGERIDO_ISCField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_DCTO_ITMField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FAC_DCTO_ITMField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_CARG_ITMField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FAC_CARG_ITMField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TIP_CARG_ITMField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_TOT_PER_ITMField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FAC_PER_ITMField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TIP_PER_ITMField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TXT_DES_ITMField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TXT_DES_ADIC_ITMField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string COD_ITMField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string COD_ITM_SUNATField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string HPJ_NUM_DOCField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string HPJ_TIP_DOCField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string HPJ_PAIS_EMI_PASPField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string HPJ_NOM_APE_RZN_SCLField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string HPJ_PAIS_RESField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string HPJ_NUM_DIAS_PERField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string HPJ_FEC_INGField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string HPJ_FEC_SALField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string HPJ_FEC_CSMField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PQT_NUM_DOCField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PQT_TIP_DOCField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PQT_NOM_DOCField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TPT_NUM_ASNTField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TPT_INF_MANField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TPT_FEC_INI_PROGField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TPT_HOR_INI_PROGField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TPT_NUM_DOC_PASJField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TPT_TIP_DOC_PASJField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TPT_NOM_APE_PASJField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TPT_UBIG_DESTField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TPT_UBIG_ORIGField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NUM_EXPEDIENTEField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string COD_UND_EJECUTORAField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NUM_CONTRATOField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NUM_PROCESOField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DRH_NOM_MTR_EMBField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DRH_CANT_ESP_MARField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DRH_LUG_DESField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DRH_FEC_DESField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DST_NUM_REG_MTCField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DST_CONF_VHCField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DST_PTO_ORGField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DST_PTO_DETField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DST_COD_MNDField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DST_VAL_REF_VJEField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DST_VAL_REF_VHCField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DST_VAL_REF_TM_VJEField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DST_CAR_EFV_VHCField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DST_CAR_EFV_VHC_UNDField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DST_CAR_UTL_VHC_VJEField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DST_CAR_UTL_VHC_VJE_UNDField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CDO_COD_UNI_CON_MINField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CDO_NUM_DEC_COMPField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CDO_REG_COM_OROField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CDO_RES_AUT_PLANField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CDO_LEY_MINField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string GDI_NUM_PLACAField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string GDI_NUM_CONTField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string GDI_FEC_CREDField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DET_VAL_ADIC01Field;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DET_VAL_ADIC02Field;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DET_VAL_ADIC03Field;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DET_VAL_ADIC04Field;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DET_VAL_ADIC05Field;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DET_VAL_ADIC06Field;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DET_VAL_ADIC07Field;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DET_VAL_ADIC08Field;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DET_VAL_ADIC09Field;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DET_VAL_ADIC10Field;

        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string LIN_ITM
        {
            get
            {
                return this.LIN_ITMField;
            }
            set
            {
                if ((object.ReferenceEquals(this.LIN_ITMField, value) != true))
                {
                    this.LIN_ITMField = value;
                    this.RaisePropertyChanged("LIN_ITM");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 1)]
        public string COD_UND_ITM
        {
            get
            {
                return this.COD_UND_ITMField;
            }
            set
            {
                if ((object.ReferenceEquals(this.COD_UND_ITMField, value) != true))
                {
                    this.COD_UND_ITMField = value;
                    this.RaisePropertyChanged("COD_UND_ITM");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 2)]
        public string CANT_UND_ITM
        {
            get
            {
                return this.CANT_UND_ITMField;
            }
            set
            {
                if ((object.ReferenceEquals(this.CANT_UND_ITMField, value) != true))
                {
                    this.CANT_UND_ITMField = value;
                    this.RaisePropertyChanged("CANT_UND_ITM");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 3)]
        public string VAL_VTA_ITM
        {
            get
            {
                return this.VAL_VTA_ITMField;
            }
            set
            {
                if ((object.ReferenceEquals(this.VAL_VTA_ITMField, value) != true))
                {
                    this.VAL_VTA_ITMField = value;
                    this.RaisePropertyChanged("VAL_VTA_ITM");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 4)]
        public string PRC_VTA_UND_ITM
        {
            get
            {
                return this.PRC_VTA_UND_ITMField;
            }
            set
            {
                if ((object.ReferenceEquals(this.PRC_VTA_UND_ITMField, value) != true))
                {
                    this.PRC_VTA_UND_ITMField = value;
                    this.RaisePropertyChanged("PRC_VTA_UND_ITM");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 5)]
        public string VAL_UNIT_ITM
        {
            get
            {
                return this.VAL_UNIT_ITMField;
            }
            set
            {
                if ((object.ReferenceEquals(this.VAL_UNIT_ITMField, value) != true))
                {
                    this.VAL_UNIT_ITMField = value;
                    this.RaisePropertyChanged("VAL_UNIT_ITM");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 6)]
        public string MNT_IGV_ITM
        {
            get
            {
                return this.MNT_IGV_ITMField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_IGV_ITMField, value) != true))
                {
                    this.MNT_IGV_ITMField = value;
                    this.RaisePropertyChanged("MNT_IGV_ITM");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 7)]
        public string POR_IGV_ITM
        {
            get
            {
                return this.POR_IGV_ITMField;
            }
            set
            {
                if ((object.ReferenceEquals(this.POR_IGV_ITMField, value) != true))
                {
                    this.POR_IGV_ITMField = value;
                    this.RaisePropertyChanged("POR_IGV_ITM");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 8)]
        public string PRC_VTA_ITEM
        {
            get
            {
                return this.PRC_VTA_ITEMField;
            }
            set
            {
                if ((object.ReferenceEquals(this.PRC_VTA_ITEMField, value) != true))
                {
                    this.PRC_VTA_ITEMField = value;
                    this.RaisePropertyChanged("PRC_VTA_ITEM");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 9)]
        public string VAL_VTA_BRT_ITEM
        {
            get
            {
                return this.VAL_VTA_BRT_ITEMField;
            }
            set
            {
                if ((object.ReferenceEquals(this.VAL_VTA_BRT_ITEMField, value) != true))
                {
                    this.VAL_VTA_BRT_ITEMField = value;
                    this.RaisePropertyChanged("VAL_VTA_BRT_ITEM");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 10)]
        public string COD_TIP_AFECT_IGV_ITM
        {
            get
            {
                return this.COD_TIP_AFECT_IGV_ITMField;
            }
            set
            {
                if ((object.ReferenceEquals(this.COD_TIP_AFECT_IGV_ITMField, value) != true))
                {
                    this.COD_TIP_AFECT_IGV_ITMField = value;
                    this.RaisePropertyChanged("COD_TIP_AFECT_IGV_ITM");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 11)]
        public string MNT_ISC_ITM
        {
            get
            {
                return this.MNT_ISC_ITMField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_ISC_ITMField, value) != true))
                {
                    this.MNT_ISC_ITMField = value;
                    this.RaisePropertyChanged("MNT_ISC_ITM");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 12)]
        public string POR_ISC_ITM
        {
            get
            {
                return this.POR_ISC_ITMField;
            }
            set
            {
                if ((object.ReferenceEquals(this.POR_ISC_ITMField, value) != true))
                {
                    this.POR_ISC_ITMField = value;
                    this.RaisePropertyChanged("POR_ISC_ITM");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 13)]
        public string COD_TIP_SIST_ISC
        {
            get
            {
                return this.COD_TIP_SIST_ISCField;
            }
            set
            {
                if ((object.ReferenceEquals(this.COD_TIP_SIST_ISCField, value) != true))
                {
                    this.COD_TIP_SIST_ISCField = value;
                    this.RaisePropertyChanged("COD_TIP_SIST_ISC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 14)]
        public string PRECIO_SUGERIDO_ISC
        {
            get
            {
                return this.PRECIO_SUGERIDO_ISCField;
            }
            set
            {
                if ((object.ReferenceEquals(this.PRECIO_SUGERIDO_ISCField, value) != true))
                {
                    this.PRECIO_SUGERIDO_ISCField = value;
                    this.RaisePropertyChanged("PRECIO_SUGERIDO_ISC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 15)]
        public string MNT_DCTO_ITM
        {
            get
            {
                return this.MNT_DCTO_ITMField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_DCTO_ITMField, value) != true))
                {
                    this.MNT_DCTO_ITMField = value;
                    this.RaisePropertyChanged("MNT_DCTO_ITM");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 16)]
        public string FAC_DCTO_ITM
        {
            get
            {
                return this.FAC_DCTO_ITMField;
            }
            set
            {
                if ((object.ReferenceEquals(this.FAC_DCTO_ITMField, value) != true))
                {
                    this.FAC_DCTO_ITMField = value;
                    this.RaisePropertyChanged("FAC_DCTO_ITM");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 17)]
        public string MNT_CARG_ITM
        {
            get
            {
                return this.MNT_CARG_ITMField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_CARG_ITMField, value) != true))
                {
                    this.MNT_CARG_ITMField = value;
                    this.RaisePropertyChanged("MNT_CARG_ITM");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 18)]
        public string FAC_CARG_ITM
        {
            get
            {
                return this.FAC_CARG_ITMField;
            }
            set
            {
                if ((object.ReferenceEquals(this.FAC_CARG_ITMField, value) != true))
                {
                    this.FAC_CARG_ITMField = value;
                    this.RaisePropertyChanged("FAC_CARG_ITM");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 19)]
        public string TIP_CARG_ITM
        {
            get
            {
                return this.TIP_CARG_ITMField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TIP_CARG_ITMField, value) != true))
                {
                    this.TIP_CARG_ITMField = value;
                    this.RaisePropertyChanged("TIP_CARG_ITM");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 20)]
        public string MNT_TOT_PER_ITM
        {
            get
            {
                return this.MNT_TOT_PER_ITMField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_TOT_PER_ITMField, value) != true))
                {
                    this.MNT_TOT_PER_ITMField = value;
                    this.RaisePropertyChanged("MNT_TOT_PER_ITM");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 21)]
        public string FAC_PER_ITM
        {
            get
            {
                return this.FAC_PER_ITMField;
            }
            set
            {
                if ((object.ReferenceEquals(this.FAC_PER_ITMField, value) != true))
                {
                    this.FAC_PER_ITMField = value;
                    this.RaisePropertyChanged("FAC_PER_ITM");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 22)]
        public string TIP_PER_ITM
        {
            get
            {
                return this.TIP_PER_ITMField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TIP_PER_ITMField, value) != true))
                {
                    this.TIP_PER_ITMField = value;
                    this.RaisePropertyChanged("TIP_PER_ITM");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 23)]
        public string TXT_DES_ITM
        {
            get
            {
                return this.TXT_DES_ITMField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TXT_DES_ITMField, value) != true))
                {
                    this.TXT_DES_ITMField = value;
                    this.RaisePropertyChanged("TXT_DES_ITM");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 24)]
        public string TXT_DES_ADIC_ITM
        {
            get
            {
                return this.TXT_DES_ADIC_ITMField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TXT_DES_ADIC_ITMField, value) != true))
                {
                    this.TXT_DES_ADIC_ITMField = value;
                    this.RaisePropertyChanged("TXT_DES_ADIC_ITM");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 25)]
        public string COD_ITM
        {
            get
            {
                return this.COD_ITMField;
            }
            set
            {
                if ((object.ReferenceEquals(this.COD_ITMField, value) != true))
                {
                    this.COD_ITMField = value;
                    this.RaisePropertyChanged("COD_ITM");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 26)]
        public string COD_ITM_SUNAT
        {
            get
            {
                return this.COD_ITM_SUNATField;
            }
            set
            {
                if ((object.ReferenceEquals(this.COD_ITM_SUNATField, value) != true))
                {
                    this.COD_ITM_SUNATField = value;
                    this.RaisePropertyChanged("COD_ITM_SUNAT");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 27)]
        public string HPJ_NUM_DOC
        {
            get
            {
                return this.HPJ_NUM_DOCField;
            }
            set
            {
                if ((object.ReferenceEquals(this.HPJ_NUM_DOCField, value) != true))
                {
                    this.HPJ_NUM_DOCField = value;
                    this.RaisePropertyChanged("HPJ_NUM_DOC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 28)]
        public string HPJ_TIP_DOC
        {
            get
            {
                return this.HPJ_TIP_DOCField;
            }
            set
            {
                if ((object.ReferenceEquals(this.HPJ_TIP_DOCField, value) != true))
                {
                    this.HPJ_TIP_DOCField = value;
                    this.RaisePropertyChanged("HPJ_TIP_DOC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 29)]
        public string HPJ_PAIS_EMI_PASP
        {
            get
            {
                return this.HPJ_PAIS_EMI_PASPField;
            }
            set
            {
                if ((object.ReferenceEquals(this.HPJ_PAIS_EMI_PASPField, value) != true))
                {
                    this.HPJ_PAIS_EMI_PASPField = value;
                    this.RaisePropertyChanged("HPJ_PAIS_EMI_PASP");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 30)]
        public string HPJ_NOM_APE_RZN_SCL
        {
            get
            {
                return this.HPJ_NOM_APE_RZN_SCLField;
            }
            set
            {
                if ((object.ReferenceEquals(this.HPJ_NOM_APE_RZN_SCLField, value) != true))
                {
                    this.HPJ_NOM_APE_RZN_SCLField = value;
                    this.RaisePropertyChanged("HPJ_NOM_APE_RZN_SCL");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 31)]
        public string HPJ_PAIS_RES
        {
            get
            {
                return this.HPJ_PAIS_RESField;
            }
            set
            {
                if ((object.ReferenceEquals(this.HPJ_PAIS_RESField, value) != true))
                {
                    this.HPJ_PAIS_RESField = value;
                    this.RaisePropertyChanged("HPJ_PAIS_RES");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 32)]
        public string HPJ_NUM_DIAS_PER
        {
            get
            {
                return this.HPJ_NUM_DIAS_PERField;
            }
            set
            {
                if ((object.ReferenceEquals(this.HPJ_NUM_DIAS_PERField, value) != true))
                {
                    this.HPJ_NUM_DIAS_PERField = value;
                    this.RaisePropertyChanged("HPJ_NUM_DIAS_PER");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 33)]
        public string HPJ_FEC_ING
        {
            get
            {
                return this.HPJ_FEC_INGField;
            }
            set
            {
                if ((object.ReferenceEquals(this.HPJ_FEC_INGField, value) != true))
                {
                    this.HPJ_FEC_INGField = value;
                    this.RaisePropertyChanged("HPJ_FEC_ING");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 34)]
        public string HPJ_FEC_SAL
        {
            get
            {
                return this.HPJ_FEC_SALField;
            }
            set
            {
                if ((object.ReferenceEquals(this.HPJ_FEC_SALField, value) != true))
                {
                    this.HPJ_FEC_SALField = value;
                    this.RaisePropertyChanged("HPJ_FEC_SAL");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 35)]
        public string HPJ_FEC_CSM
        {
            get
            {
                return this.HPJ_FEC_CSMField;
            }
            set
            {
                if ((object.ReferenceEquals(this.HPJ_FEC_CSMField, value) != true))
                {
                    this.HPJ_FEC_CSMField = value;
                    this.RaisePropertyChanged("HPJ_FEC_CSM");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 36)]
        public string PQT_NUM_DOC
        {
            get
            {
                return this.PQT_NUM_DOCField;
            }
            set
            {
                if ((object.ReferenceEquals(this.PQT_NUM_DOCField, value) != true))
                {
                    this.PQT_NUM_DOCField = value;
                    this.RaisePropertyChanged("PQT_NUM_DOC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 37)]
        public string PQT_TIP_DOC
        {
            get
            {
                return this.PQT_TIP_DOCField;
            }
            set
            {
                if ((object.ReferenceEquals(this.PQT_TIP_DOCField, value) != true))
                {
                    this.PQT_TIP_DOCField = value;
                    this.RaisePropertyChanged("PQT_TIP_DOC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 38)]
        public string PQT_NOM_DOC
        {
            get
            {
                return this.PQT_NOM_DOCField;
            }
            set
            {
                if ((object.ReferenceEquals(this.PQT_NOM_DOCField, value) != true))
                {
                    this.PQT_NOM_DOCField = value;
                    this.RaisePropertyChanged("PQT_NOM_DOC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 39)]
        public string TPT_NUM_ASNT
        {
            get
            {
                return this.TPT_NUM_ASNTField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TPT_NUM_ASNTField, value) != true))
                {
                    this.TPT_NUM_ASNTField = value;
                    this.RaisePropertyChanged("TPT_NUM_ASNT");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 40)]
        public string TPT_INF_MAN
        {
            get
            {
                return this.TPT_INF_MANField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TPT_INF_MANField, value) != true))
                {
                    this.TPT_INF_MANField = value;
                    this.RaisePropertyChanged("TPT_INF_MAN");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 41)]
        public string TPT_FEC_INI_PROG
        {
            get
            {
                return this.TPT_FEC_INI_PROGField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TPT_FEC_INI_PROGField, value) != true))
                {
                    this.TPT_FEC_INI_PROGField = value;
                    this.RaisePropertyChanged("TPT_FEC_INI_PROG");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 42)]
        public string TPT_HOR_INI_PROG
        {
            get
            {
                return this.TPT_HOR_INI_PROGField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TPT_HOR_INI_PROGField, value) != true))
                {
                    this.TPT_HOR_INI_PROGField = value;
                    this.RaisePropertyChanged("TPT_HOR_INI_PROG");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 43)]
        public string TPT_NUM_DOC_PASJ
        {
            get
            {
                return this.TPT_NUM_DOC_PASJField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TPT_NUM_DOC_PASJField, value) != true))
                {
                    this.TPT_NUM_DOC_PASJField = value;
                    this.RaisePropertyChanged("TPT_NUM_DOC_PASJ");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 44)]
        public string TPT_TIP_DOC_PASJ
        {
            get
            {
                return this.TPT_TIP_DOC_PASJField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TPT_TIP_DOC_PASJField, value) != true))
                {
                    this.TPT_TIP_DOC_PASJField = value;
                    this.RaisePropertyChanged("TPT_TIP_DOC_PASJ");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 45)]
        public string TPT_NOM_APE_PASJ
        {
            get
            {
                return this.TPT_NOM_APE_PASJField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TPT_NOM_APE_PASJField, value) != true))
                {
                    this.TPT_NOM_APE_PASJField = value;
                    this.RaisePropertyChanged("TPT_NOM_APE_PASJ");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 46)]
        public string TPT_UBIG_DEST
        {
            get
            {
                return this.TPT_UBIG_DESTField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TPT_UBIG_DESTField, value) != true))
                {
                    this.TPT_UBIG_DESTField = value;
                    this.RaisePropertyChanged("TPT_UBIG_DEST");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 47)]
        public string TPT_UBIG_ORIG
        {
            get
            {
                return this.TPT_UBIG_ORIGField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TPT_UBIG_ORIGField, value) != true))
                {
                    this.TPT_UBIG_ORIGField = value;
                    this.RaisePropertyChanged("TPT_UBIG_ORIG");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 48)]
        public string NUM_EXPEDIENTE
        {
            get
            {
                return this.NUM_EXPEDIENTEField;
            }
            set
            {
                if ((object.ReferenceEquals(this.NUM_EXPEDIENTEField, value) != true))
                {
                    this.NUM_EXPEDIENTEField = value;
                    this.RaisePropertyChanged("NUM_EXPEDIENTE");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 49)]
        public string COD_UND_EJECUTORA
        {
            get
            {
                return this.COD_UND_EJECUTORAField;
            }
            set
            {
                if ((object.ReferenceEquals(this.COD_UND_EJECUTORAField, value) != true))
                {
                    this.COD_UND_EJECUTORAField = value;
                    this.RaisePropertyChanged("COD_UND_EJECUTORA");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 50)]
        public string NUM_CONTRATO
        {
            get
            {
                return this.NUM_CONTRATOField;
            }
            set
            {
                if ((object.ReferenceEquals(this.NUM_CONTRATOField, value) != true))
                {
                    this.NUM_CONTRATOField = value;
                    this.RaisePropertyChanged("NUM_CONTRATO");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 51)]
        public string NUM_PROCESO
        {
            get
            {
                return this.NUM_PROCESOField;
            }
            set
            {
                if ((object.ReferenceEquals(this.NUM_PROCESOField, value) != true))
                {
                    this.NUM_PROCESOField = value;
                    this.RaisePropertyChanged("NUM_PROCESO");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 52)]
        public string DRH_NOM_MTR_EMB
        {
            get
            {
                return this.DRH_NOM_MTR_EMBField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DRH_NOM_MTR_EMBField, value) != true))
                {
                    this.DRH_NOM_MTR_EMBField = value;
                    this.RaisePropertyChanged("DRH_NOM_MTR_EMB");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 53)]
        public string DRH_CANT_ESP_MAR
        {
            get
            {
                return this.DRH_CANT_ESP_MARField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DRH_CANT_ESP_MARField, value) != true))
                {
                    this.DRH_CANT_ESP_MARField = value;
                    this.RaisePropertyChanged("DRH_CANT_ESP_MAR");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 54)]
        public string DRH_LUG_DES
        {
            get
            {
                return this.DRH_LUG_DESField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DRH_LUG_DESField, value) != true))
                {
                    this.DRH_LUG_DESField = value;
                    this.RaisePropertyChanged("DRH_LUG_DES");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 55)]
        public string DRH_FEC_DES
        {
            get
            {
                return this.DRH_FEC_DESField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DRH_FEC_DESField, value) != true))
                {
                    this.DRH_FEC_DESField = value;
                    this.RaisePropertyChanged("DRH_FEC_DES");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 56)]
        public string DST_NUM_REG_MTC
        {
            get
            {
                return this.DST_NUM_REG_MTCField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DST_NUM_REG_MTCField, value) != true))
                {
                    this.DST_NUM_REG_MTCField = value;
                    this.RaisePropertyChanged("DST_NUM_REG_MTC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 57)]
        public string DST_CONF_VHC
        {
            get
            {
                return this.DST_CONF_VHCField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DST_CONF_VHCField, value) != true))
                {
                    this.DST_CONF_VHCField = value;
                    this.RaisePropertyChanged("DST_CONF_VHC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 58)]
        public string DST_PTO_ORG
        {
            get
            {
                return this.DST_PTO_ORGField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DST_PTO_ORGField, value) != true))
                {
                    this.DST_PTO_ORGField = value;
                    this.RaisePropertyChanged("DST_PTO_ORG");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 59)]
        public string DST_PTO_DET
        {
            get
            {
                return this.DST_PTO_DETField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DST_PTO_DETField, value) != true))
                {
                    this.DST_PTO_DETField = value;
                    this.RaisePropertyChanged("DST_PTO_DET");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 60)]
        public string DST_COD_MND
        {
            get
            {
                return this.DST_COD_MNDField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DST_COD_MNDField, value) != true))
                {
                    this.DST_COD_MNDField = value;
                    this.RaisePropertyChanged("DST_COD_MND");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 61)]
        public string DST_VAL_REF_VJE
        {
            get
            {
                return this.DST_VAL_REF_VJEField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DST_VAL_REF_VJEField, value) != true))
                {
                    this.DST_VAL_REF_VJEField = value;
                    this.RaisePropertyChanged("DST_VAL_REF_VJE");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 62)]
        public string DST_VAL_REF_VHC
        {
            get
            {
                return this.DST_VAL_REF_VHCField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DST_VAL_REF_VHCField, value) != true))
                {
                    this.DST_VAL_REF_VHCField = value;
                    this.RaisePropertyChanged("DST_VAL_REF_VHC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 63)]
        public string DST_VAL_REF_TM_VJE
        {
            get
            {
                return this.DST_VAL_REF_TM_VJEField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DST_VAL_REF_TM_VJEField, value) != true))
                {
                    this.DST_VAL_REF_TM_VJEField = value;
                    this.RaisePropertyChanged("DST_VAL_REF_TM_VJE");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 64)]
        public string DST_CAR_EFV_VHC
        {
            get
            {
                return this.DST_CAR_EFV_VHCField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DST_CAR_EFV_VHCField, value) != true))
                {
                    this.DST_CAR_EFV_VHCField = value;
                    this.RaisePropertyChanged("DST_CAR_EFV_VHC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 65)]
        public string DST_CAR_EFV_VHC_UND
        {
            get
            {
                return this.DST_CAR_EFV_VHC_UNDField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DST_CAR_EFV_VHC_UNDField, value) != true))
                {
                    this.DST_CAR_EFV_VHC_UNDField = value;
                    this.RaisePropertyChanged("DST_CAR_EFV_VHC_UND");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 66)]
        public string DST_CAR_UTL_VHC_VJE
        {
            get
            {
                return this.DST_CAR_UTL_VHC_VJEField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DST_CAR_UTL_VHC_VJEField, value) != true))
                {
                    this.DST_CAR_UTL_VHC_VJEField = value;
                    this.RaisePropertyChanged("DST_CAR_UTL_VHC_VJE");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 67)]
        public string DST_CAR_UTL_VHC_VJE_UND
        {
            get
            {
                return this.DST_CAR_UTL_VHC_VJE_UNDField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DST_CAR_UTL_VHC_VJE_UNDField, value) != true))
                {
                    this.DST_CAR_UTL_VHC_VJE_UNDField = value;
                    this.RaisePropertyChanged("DST_CAR_UTL_VHC_VJE_UND");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 68)]
        public string CDO_COD_UNI_CON_MIN
        {
            get
            {
                return this.CDO_COD_UNI_CON_MINField;
            }
            set
            {
                if ((object.ReferenceEquals(this.CDO_COD_UNI_CON_MINField, value) != true))
                {
                    this.CDO_COD_UNI_CON_MINField = value;
                    this.RaisePropertyChanged("CDO_COD_UNI_CON_MIN");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 69)]
        public string CDO_NUM_DEC_COMP
        {
            get
            {
                return this.CDO_NUM_DEC_COMPField;
            }
            set
            {
                if ((object.ReferenceEquals(this.CDO_NUM_DEC_COMPField, value) != true))
                {
                    this.CDO_NUM_DEC_COMPField = value;
                    this.RaisePropertyChanged("CDO_NUM_DEC_COMP");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 70)]
        public string CDO_REG_COM_ORO
        {
            get
            {
                return this.CDO_REG_COM_OROField;
            }
            set
            {
                if ((object.ReferenceEquals(this.CDO_REG_COM_OROField, value) != true))
                {
                    this.CDO_REG_COM_OROField = value;
                    this.RaisePropertyChanged("CDO_REG_COM_ORO");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 71)]
        public string CDO_RES_AUT_PLAN
        {
            get
            {
                return this.CDO_RES_AUT_PLANField;
            }
            set
            {
                if ((object.ReferenceEquals(this.CDO_RES_AUT_PLANField, value) != true))
                {
                    this.CDO_RES_AUT_PLANField = value;
                    this.RaisePropertyChanged("CDO_RES_AUT_PLAN");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 72)]
        public string CDO_LEY_MIN
        {
            get
            {
                return this.CDO_LEY_MINField;
            }
            set
            {
                if ((object.ReferenceEquals(this.CDO_LEY_MINField, value) != true))
                {
                    this.CDO_LEY_MINField = value;
                    this.RaisePropertyChanged("CDO_LEY_MIN");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 73)]
        public string GDI_NUM_PLACA
        {
            get
            {
                return this.GDI_NUM_PLACAField;
            }
            set
            {
                if ((object.ReferenceEquals(this.GDI_NUM_PLACAField, value) != true))
                {
                    this.GDI_NUM_PLACAField = value;
                    this.RaisePropertyChanged("GDI_NUM_PLACA");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 74)]
        public string GDI_NUM_CONT
        {
            get
            {
                return this.GDI_NUM_CONTField;
            }
            set
            {
                if ((object.ReferenceEquals(this.GDI_NUM_CONTField, value) != true))
                {
                    this.GDI_NUM_CONTField = value;
                    this.RaisePropertyChanged("GDI_NUM_CONT");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 75)]
        public string GDI_FEC_CRED
        {
            get
            {
                return this.GDI_FEC_CREDField;
            }
            set
            {
                if ((object.ReferenceEquals(this.GDI_FEC_CREDField, value) != true))
                {
                    this.GDI_FEC_CREDField = value;
                    this.RaisePropertyChanged("GDI_FEC_CRED");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 76)]
        public string DET_VAL_ADIC01
        {
            get
            {
                return this.DET_VAL_ADIC01Field;
            }
            set
            {
                if ((object.ReferenceEquals(this.DET_VAL_ADIC01Field, value) != true))
                {
                    this.DET_VAL_ADIC01Field = value;
                    this.RaisePropertyChanged("DET_VAL_ADIC01");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 77)]
        public string DET_VAL_ADIC02
        {
            get
            {
                return this.DET_VAL_ADIC02Field;
            }
            set
            {
                if ((object.ReferenceEquals(this.DET_VAL_ADIC02Field, value) != true))
                {
                    this.DET_VAL_ADIC02Field = value;
                    this.RaisePropertyChanged("DET_VAL_ADIC02");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 78)]
        public string DET_VAL_ADIC03
        {
            get
            {
                return this.DET_VAL_ADIC03Field;
            }
            set
            {
                if ((object.ReferenceEquals(this.DET_VAL_ADIC03Field, value) != true))
                {
                    this.DET_VAL_ADIC03Field = value;
                    this.RaisePropertyChanged("DET_VAL_ADIC03");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 79)]
        public string DET_VAL_ADIC04
        {
            get
            {
                return this.DET_VAL_ADIC04Field;
            }
            set
            {
                if ((object.ReferenceEquals(this.DET_VAL_ADIC04Field, value) != true))
                {
                    this.DET_VAL_ADIC04Field = value;
                    this.RaisePropertyChanged("DET_VAL_ADIC04");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 80)]
        public string DET_VAL_ADIC05
        {
            get
            {
                return this.DET_VAL_ADIC05Field;
            }
            set
            {
                if ((object.ReferenceEquals(this.DET_VAL_ADIC05Field, value) != true))
                {
                    this.DET_VAL_ADIC05Field = value;
                    this.RaisePropertyChanged("DET_VAL_ADIC05");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 81)]
        public string DET_VAL_ADIC06
        {
            get
            {
                return this.DET_VAL_ADIC06Field;
            }
            set
            {
                if ((object.ReferenceEquals(this.DET_VAL_ADIC06Field, value) != true))
                {
                    this.DET_VAL_ADIC06Field = value;
                    this.RaisePropertyChanged("DET_VAL_ADIC06");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 82)]
        public string DET_VAL_ADIC07
        {
            get
            {
                return this.DET_VAL_ADIC07Field;
            }
            set
            {
                if ((object.ReferenceEquals(this.DET_VAL_ADIC07Field, value) != true))
                {
                    this.DET_VAL_ADIC07Field = value;
                    this.RaisePropertyChanged("DET_VAL_ADIC07");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 83)]
        public string DET_VAL_ADIC08
        {
            get
            {
                return this.DET_VAL_ADIC08Field;
            }
            set
            {
                if ((object.ReferenceEquals(this.DET_VAL_ADIC08Field, value) != true))
                {
                    this.DET_VAL_ADIC08Field = value;
                    this.RaisePropertyChanged("DET_VAL_ADIC08");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 84)]
        public string DET_VAL_ADIC09
        {
            get
            {
                return this.DET_VAL_ADIC09Field;
            }
            set
            {
                if ((object.ReferenceEquals(this.DET_VAL_ADIC09Field, value) != true))
                {
                    this.DET_VAL_ADIC09Field = value;
                    this.RaisePropertyChanged("DET_VAL_ADIC09");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(Order = 85)]
        public string DET_VAL_ADIC10
        {
            get
            {
                return this.DET_VAL_ADIC10Field;
            }
            set
            {
                if ((object.ReferenceEquals(this.DET_VAL_ADIC10Field, value) != true))
                {
                    this.DET_VAL_ADIC10Field = value;
                    this.RaisePropertyChanged("DET_VAL_ADIC10");
                }
            }
        }


        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }    

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "CPE_DAT_ADIC_COMPRA", Namespace = "http://schemas.datacontract.org/2004/07/TT.EOL.Level.BE")]
    [System.SerializableAttribute()]
    public partial class CPE_DAT_ADIC_COMPRA : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
    {

        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string COD_TIP_ADIC_SUNATField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NUM_LIN_ADIC_SUNATField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TXT_DESC_ADIC_SUNATField;

        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string COD_TIP_ADIC_SUNAT
        {
            get
            {
                return this.COD_TIP_ADIC_SUNATField;
            }
            set
            {
                if ((object.ReferenceEquals(this.COD_TIP_ADIC_SUNATField, value) != true))
                {
                    this.COD_TIP_ADIC_SUNATField = value;
                    this.RaisePropertyChanged("COD_TIP_ADIC_SUNAT");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NUM_LIN_ADIC_SUNAT
        {
            get
            {
                return this.NUM_LIN_ADIC_SUNATField;
            }
            set
            {
                if ((object.ReferenceEquals(this.NUM_LIN_ADIC_SUNATField, value) != true))
                {
                    this.NUM_LIN_ADIC_SUNATField = value;
                    this.RaisePropertyChanged("NUM_LIN_ADIC_SUNAT");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string TXT_DESC_ADIC_SUNAT
        {
            get
            {
                return this.TXT_DESC_ADIC_SUNATField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TXT_DESC_ADIC_SUNATField, value) != true))
                {
                    this.TXT_DESC_ADIC_SUNATField = value;
                    this.RaisePropertyChanged("TXT_DESC_ADIC_SUNAT");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "CPE_DOC_REF_COMPRA", Namespace = "http://schemas.datacontract.org/2004/07/TT.EOL.Level.BE")]
    [System.SerializableAttribute()]
    public partial class CPE_DOC_REF_COMPRA : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
    {

        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string COD_TIP_DOC_REFField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string COD_TIP_OTR_DOC_REFField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FEC_DOC_REFField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NUM_CORRE_CPE_REFField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NUM_LIN_REFField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NUM_OTR_DOC_REFField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NUM_SERIE_CPE_REFField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string SERIE_CORRE_CPE_REFField;

        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string COD_TIP_DOC_REF
        {
            get
            {
                return this.COD_TIP_DOC_REFField;
            }
            set
            {
                if ((object.ReferenceEquals(this.COD_TIP_DOC_REFField, value) != true))
                {
                    this.COD_TIP_DOC_REFField = value;
                    this.RaisePropertyChanged("COD_TIP_DOC_REF");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string COD_TIP_OTR_DOC_REF
        {
            get
            {
                return this.COD_TIP_OTR_DOC_REFField;
            }
            set
            {
                if ((object.ReferenceEquals(this.COD_TIP_OTR_DOC_REFField, value) != true))
                {
                    this.COD_TIP_OTR_DOC_REFField = value;
                    this.RaisePropertyChanged("COD_TIP_OTR_DOC_REF");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string FEC_DOC_REF
        {
            get
            {
                return this.FEC_DOC_REFField;
            }
            set
            {
                if ((object.ReferenceEquals(this.FEC_DOC_REFField, value) != true))
                {
                    this.FEC_DOC_REFField = value;
                    this.RaisePropertyChanged("FEC_DOC_REF");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NUM_CORRE_CPE_REF
        {
            get
            {
                return this.NUM_CORRE_CPE_REFField;
            }
            set
            {
                if ((object.ReferenceEquals(this.NUM_CORRE_CPE_REFField, value) != true))
                {
                    this.NUM_CORRE_CPE_REFField = value;
                    this.RaisePropertyChanged("NUM_CORRE_CPE_REF");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NUM_LIN_REF
        {
            get
            {
                return this.NUM_LIN_REFField;
            }
            set
            {
                if ((object.ReferenceEquals(this.NUM_LIN_REFField, value) != true))
                {
                    this.NUM_LIN_REFField = value;
                    this.RaisePropertyChanged("NUM_LIN_REF");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NUM_OTR_DOC_REF
        {
            get
            {
                return this.NUM_OTR_DOC_REFField;
            }
            set
            {
                if ((object.ReferenceEquals(this.NUM_OTR_DOC_REFField, value) != true))
                {
                    this.NUM_OTR_DOC_REFField = value;
                    this.RaisePropertyChanged("NUM_OTR_DOC_REF");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NUM_SERIE_CPE_REF
        {
            get
            {
                return this.NUM_SERIE_CPE_REFField;
            }
            set
            {
                if ((object.ReferenceEquals(this.NUM_SERIE_CPE_REFField, value) != true))
                {
                    this.NUM_SERIE_CPE_REFField = value;
                    this.RaisePropertyChanged("NUM_SERIE_CPE_REF");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string SERIE_CORRE_CPE_REF
        {
            get
            {
                return this.SERIE_CORRE_CPE_REFField;
            }
            set
            {
                if ((object.ReferenceEquals(this.SERIE_CORRE_CPE_REFField, value) != true))
                {
                    this.SERIE_CORRE_CPE_REFField = value;
                    this.RaisePropertyChanged("SERIE_CORRE_CPE_REF");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "CPE_ANTICIPO_COMPRA", Namespace = "http://schemas.datacontract.org/2004/07/TT.EOL.Level.BE")]
    [System.SerializableAttribute()]
    public partial class CPE_ANTICIPO_COMPRA : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
    {

        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string COD_TIP_DOC_ANTCPField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string LIN_ANTCPField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MNT_ANTCPField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NUM_CORRE_CPE_ANTCPField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NUM_SERIE_CPE_ANTCPField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string SERIE_CORRE_ANTCPField;

        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string COD_TIP_DOC_ANTCP
        {
            get
            {
                return this.COD_TIP_DOC_ANTCPField;
            }
            set
            {
                if ((object.ReferenceEquals(this.COD_TIP_DOC_ANTCPField, value) != true))
                {
                    this.COD_TIP_DOC_ANTCPField = value;
                    this.RaisePropertyChanged("COD_TIP_DOC_ANTCP");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string LIN_ANTCP
        {
            get
            {
                return this.LIN_ANTCPField;
            }
            set
            {
                if ((object.ReferenceEquals(this.LIN_ANTCPField, value) != true))
                {
                    this.LIN_ANTCPField = value;
                    this.RaisePropertyChanged("LIN_ANTCP");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string MNT_ANTCP
        {
            get
            {
                return this.MNT_ANTCPField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MNT_ANTCPField, value) != true))
                {
                    this.MNT_ANTCPField = value;
                    this.RaisePropertyChanged("MNT_ANTCP");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NUM_CORRE_CPE_ANTCP
        {
            get
            {
                return this.NUM_CORRE_CPE_ANTCPField;
            }
            set
            {
                if ((object.ReferenceEquals(this.NUM_CORRE_CPE_ANTCPField, value) != true))
                {
                    this.NUM_CORRE_CPE_ANTCPField = value;
                    this.RaisePropertyChanged("NUM_CORRE_CPE_ANTCP");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NUM_SERIE_CPE_ANTCP
        {
            get
            {
                return this.NUM_SERIE_CPE_ANTCPField;
            }
            set
            {
                if ((object.ReferenceEquals(this.NUM_SERIE_CPE_ANTCPField, value) != true))
                {
                    this.NUM_SERIE_CPE_ANTCPField = value;
                    this.RaisePropertyChanged("NUM_SERIE_CPE_ANTCP");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string SERIE_CORRE_ANTCP
        {
            get
            {
                return this.SERIE_CORRE_ANTCPField;
            }
            set
            {
                if ((object.ReferenceEquals(this.SERIE_CORRE_ANTCPField, value) != true))
                {
                    this.SERIE_CORRE_ANTCPField = value;
                    this.RaisePropertyChanged("SERIE_CORRE_ANTCP");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "CPE_FAC_GUIA_COMPRA", Namespace = "http://schemas.datacontract.org/2004/07/TT.EOL.Level.BE")]
    [System.SerializableAttribute()]
    public partial class CPE_FAC_GUIA_COMPRA : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
    {

        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CERT_AUTO_TRNSPField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string COD_MOD_TRNSPField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string COD_TIP_GREField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string COD_UND_MEDIDAField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DIR_LLEGADAField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DIR_PAI_LLEGADAField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DIR_PAI_PARTIDAField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DIR_PARTIDAField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DIR_UBI_LLEGADAField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DIR_UBI_PARTIDAField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FEC_INI_TRASLADOField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool IND_SUB_CONTRATAField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string LIC_COND_TRNSPField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MARCA_AUTO_TRNSPField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MTV_TRASLADOField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NOM_DESTINATARIOField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NRO_DOC_COND_TRASLADOField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NRO_DOC_DESTINATARIOField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NRO_GREField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NRO_PLACA_TRASLADOField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PLACA_AUTO_TRNSPField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string RUC_TRNSPField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string RZN_SCL_TRNSPField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TIP_DOC_COND_TRASLADOField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TIP_DOC_DESTINATARIOField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TOT_PSO_BRUTOField;

        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string CERT_AUTO_TRNSP
        {
            get
            {
                return this.CERT_AUTO_TRNSPField;
            }
            set
            {
                if ((object.ReferenceEquals(this.CERT_AUTO_TRNSPField, value) != true))
                {
                    this.CERT_AUTO_TRNSPField = value;
                    this.RaisePropertyChanged("CERT_AUTO_TRNSP");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string COD_MOD_TRNSP
        {
            get
            {
                return this.COD_MOD_TRNSPField;
            }
            set
            {
                if ((object.ReferenceEquals(this.COD_MOD_TRNSPField, value) != true))
                {
                    this.COD_MOD_TRNSPField = value;
                    this.RaisePropertyChanged("COD_MOD_TRNSP");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string COD_TIP_GRE
        {
            get
            {
                return this.COD_TIP_GREField;
            }
            set
            {
                if ((object.ReferenceEquals(this.COD_TIP_GREField, value) != true))
                {
                    this.COD_TIP_GREField = value;
                    this.RaisePropertyChanged("COD_TIP_GRE");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string COD_UND_MEDIDA
        {
            get
            {
                return this.COD_UND_MEDIDAField;
            }
            set
            {
                if ((object.ReferenceEquals(this.COD_UND_MEDIDAField, value) != true))
                {
                    this.COD_UND_MEDIDAField = value;
                    this.RaisePropertyChanged("COD_UND_MEDIDA");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DIR_LLEGADA
        {
            get
            {
                return this.DIR_LLEGADAField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DIR_LLEGADAField, value) != true))
                {
                    this.DIR_LLEGADAField = value;
                    this.RaisePropertyChanged("DIR_LLEGADA");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DIR_PAI_LLEGADA
        {
            get
            {
                return this.DIR_PAI_LLEGADAField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DIR_PAI_LLEGADAField, value) != true))
                {
                    this.DIR_PAI_LLEGADAField = value;
                    this.RaisePropertyChanged("DIR_PAI_LLEGADA");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DIR_PAI_PARTIDA
        {
            get
            {
                return this.DIR_PAI_PARTIDAField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DIR_PAI_PARTIDAField, value) != true))
                {
                    this.DIR_PAI_PARTIDAField = value;
                    this.RaisePropertyChanged("DIR_PAI_PARTIDA");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DIR_PARTIDA
        {
            get
            {
                return this.DIR_PARTIDAField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DIR_PARTIDAField, value) != true))
                {
                    this.DIR_PARTIDAField = value;
                    this.RaisePropertyChanged("DIR_PARTIDA");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DIR_UBI_LLEGADA
        {
            get
            {
                return this.DIR_UBI_LLEGADAField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DIR_UBI_LLEGADAField, value) != true))
                {
                    this.DIR_UBI_LLEGADAField = value;
                    this.RaisePropertyChanged("DIR_UBI_LLEGADA");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DIR_UBI_PARTIDA
        {
            get
            {
                return this.DIR_UBI_PARTIDAField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DIR_UBI_PARTIDAField, value) != true))
                {
                    this.DIR_UBI_PARTIDAField = value;
                    this.RaisePropertyChanged("DIR_UBI_PARTIDA");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string FEC_INI_TRASLADO
        {
            get
            {
                return this.FEC_INI_TRASLADOField;
            }
            set
            {
                if ((object.ReferenceEquals(this.FEC_INI_TRASLADOField, value) != true))
                {
                    this.FEC_INI_TRASLADOField = value;
                    this.RaisePropertyChanged("FEC_INI_TRASLADO");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IND_SUB_CONTRATA
        {
            get
            {
                return this.IND_SUB_CONTRATAField;
            }
            set
            {
                if ((this.IND_SUB_CONTRATAField.Equals(value) != true))
                {
                    this.IND_SUB_CONTRATAField = value;
                    this.RaisePropertyChanged("IND_SUB_CONTRATA");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string LIC_COND_TRNSP
        {
            get
            {
                return this.LIC_COND_TRNSPField;
            }
            set
            {
                if ((object.ReferenceEquals(this.LIC_COND_TRNSPField, value) != true))
                {
                    this.LIC_COND_TRNSPField = value;
                    this.RaisePropertyChanged("LIC_COND_TRNSP");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string MARCA_AUTO_TRNSP
        {
            get
            {
                return this.MARCA_AUTO_TRNSPField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MARCA_AUTO_TRNSPField, value) != true))
                {
                    this.MARCA_AUTO_TRNSPField = value;
                    this.RaisePropertyChanged("MARCA_AUTO_TRNSP");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string MTV_TRASLADO
        {
            get
            {
                return this.MTV_TRASLADOField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MTV_TRASLADOField, value) != true))
                {
                    this.MTV_TRASLADOField = value;
                    this.RaisePropertyChanged("MTV_TRASLADO");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NOM_DESTINATARIO
        {
            get
            {
                return this.NOM_DESTINATARIOField;
            }
            set
            {
                if ((object.ReferenceEquals(this.NOM_DESTINATARIOField, value) != true))
                {
                    this.NOM_DESTINATARIOField = value;
                    this.RaisePropertyChanged("NOM_DESTINATARIO");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NRO_DOC_COND_TRASLADO
        {
            get
            {
                return this.NRO_DOC_COND_TRASLADOField;
            }
            set
            {
                if ((object.ReferenceEquals(this.NRO_DOC_COND_TRASLADOField, value) != true))
                {
                    this.NRO_DOC_COND_TRASLADOField = value;
                    this.RaisePropertyChanged("NRO_DOC_COND_TRASLADO");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NRO_DOC_DESTINATARIO
        {
            get
            {
                return this.NRO_DOC_DESTINATARIOField;
            }
            set
            {
                if ((object.ReferenceEquals(this.NRO_DOC_DESTINATARIOField, value) != true))
                {
                    this.NRO_DOC_DESTINATARIOField = value;
                    this.RaisePropertyChanged("NRO_DOC_DESTINATARIO");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NRO_GRE
        {
            get
            {
                return this.NRO_GREField;
            }
            set
            {
                if ((object.ReferenceEquals(this.NRO_GREField, value) != true))
                {
                    this.NRO_GREField = value;
                    this.RaisePropertyChanged("NRO_GRE");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NRO_PLACA_TRASLADO
        {
            get
            {
                return this.NRO_PLACA_TRASLADOField;
            }
            set
            {
                if ((object.ReferenceEquals(this.NRO_PLACA_TRASLADOField, value) != true))
                {
                    this.NRO_PLACA_TRASLADOField = value;
                    this.RaisePropertyChanged("NRO_PLACA_TRASLADO");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PLACA_AUTO_TRNSP
        {
            get
            {
                return this.PLACA_AUTO_TRNSPField;
            }
            set
            {
                if ((object.ReferenceEquals(this.PLACA_AUTO_TRNSPField, value) != true))
                {
                    this.PLACA_AUTO_TRNSPField = value;
                    this.RaisePropertyChanged("PLACA_AUTO_TRNSP");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string RUC_TRNSP
        {
            get
            {
                return this.RUC_TRNSPField;
            }
            set
            {
                if ((object.ReferenceEquals(this.RUC_TRNSPField, value) != true))
                {
                    this.RUC_TRNSPField = value;
                    this.RaisePropertyChanged("RUC_TRNSP");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string RZN_SCL_TRNSP
        {
            get
            {
                return this.RZN_SCL_TRNSPField;
            }
            set
            {
                if ((object.ReferenceEquals(this.RZN_SCL_TRNSPField, value) != true))
                {
                    this.RZN_SCL_TRNSPField = value;
                    this.RaisePropertyChanged("RZN_SCL_TRNSP");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string TIP_DOC_COND_TRASLADO
        {
            get
            {
                return this.TIP_DOC_COND_TRASLADOField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TIP_DOC_COND_TRASLADOField, value) != true))
                {
                    this.TIP_DOC_COND_TRASLADOField = value;
                    this.RaisePropertyChanged("TIP_DOC_COND_TRASLADO");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string TIP_DOC_DESTINATARIO
        {
            get
            {
                return this.TIP_DOC_DESTINATARIOField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TIP_DOC_DESTINATARIOField, value) != true))
                {
                    this.TIP_DOC_DESTINATARIOField = value;
                    this.RaisePropertyChanged("TIP_DOC_DESTINATARIO");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string TOT_PSO_BRUTO
        {
            get
            {
                return this.TOT_PSO_BRUTOField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TOT_PSO_BRUTOField, value) != true))
                {
                    this.TOT_PSO_BRUTOField = value;
                    this.RaisePropertyChanged("TOT_PSO_BRUTO");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "CPE_DOC_ASOC_COMPRA", Namespace = "http://schemas.datacontract.org/2004/07/TT.EOL.Level.BE")]
    [System.SerializableAttribute()]
    public partial class CPE_DOC_ASOC_COMPRA : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
    {

        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private byte[] DOC_ASOCField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime FEC_CREAField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int FLG_ACTIVOField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NUM_CPEField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string SEQ_DOC_ASOCField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TXT_DOC_ASOCField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TXT_TAM_DOCField;

        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public byte[] DOC_ASOC
        {
            get
            {
                return this.DOC_ASOCField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DOC_ASOCField, value) != true))
                {
                    this.DOC_ASOCField = value;
                    this.RaisePropertyChanged("DOC_ASOC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime FEC_CREA
        {
            get
            {
                return this.FEC_CREAField;
            }
            set
            {
                if ((this.FEC_CREAField.Equals(value) != true))
                {
                    this.FEC_CREAField = value;
                    this.RaisePropertyChanged("FEC_CREA");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int FLG_ACTIVO
        {
            get
            {
                return this.FLG_ACTIVOField;
            }
            set
            {
                if ((this.FLG_ACTIVOField.Equals(value) != true))
                {
                    this.FLG_ACTIVOField = value;
                    this.RaisePropertyChanged("FLG_ACTIVO");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NUM_CPE
        {
            get
            {
                return this.NUM_CPEField;
            }
            set
            {
                if ((object.ReferenceEquals(this.NUM_CPEField, value) != true))
                {
                    this.NUM_CPEField = value;
                    this.RaisePropertyChanged("NUM_CPE");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string SEQ_DOC_ASOC
        {
            get
            {
                return this.SEQ_DOC_ASOCField;
            }
            set
            {
                if ((object.ReferenceEquals(this.SEQ_DOC_ASOCField, value) != true))
                {
                    this.SEQ_DOC_ASOCField = value;
                    this.RaisePropertyChanged("SEQ_DOC_ASOC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string TXT_DOC_ASOC
        {
            get
            {
                return this.TXT_DOC_ASOCField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TXT_DOC_ASOCField, value) != true))
                {
                    this.TXT_DOC_ASOCField = value;
                    this.RaisePropertyChanged("TXT_DOC_ASOC");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string TXT_TAM_DOC
        {
            get
            {
                return this.TXT_TAM_DOCField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TXT_TAM_DOCField, value) != true))
                {
                    this.TXT_TAM_DOCField = value;
                    this.RaisePropertyChanged("TXT_TAM_DOC");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

   
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "CPE_RESPUESTA_COMPRA", Namespace = "http://schemas.datacontract.org/2004/07/TT.EOL.Level.BE")]
    [System.SerializableAttribute()]
    public partial class CPE_RESPUESTA_COMPRA : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
    {

        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CODIGOField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string COD_ESTD_SUNATField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DESCRIPCIONField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DETALLEField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NUM_CPEField;

        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string CODIGO
        {
            get
            {
                return this.CODIGOField;
            }
            set
            {
                if ((object.ReferenceEquals(this.CODIGOField, value) != true))
                {
                    this.CODIGOField = value;
                    this.RaisePropertyChanged("CODIGO");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string COD_ESTD_SUNAT
        {
            get
            {
                return this.COD_ESTD_SUNATField;
            }
            set
            {
                if ((object.ReferenceEquals(this.COD_ESTD_SUNATField, value) != true))
                {
                    this.COD_ESTD_SUNATField = value;
                    this.RaisePropertyChanged("COD_ESTD_SUNAT");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DESCRIPCION
        {
            get
            {
                return this.DESCRIPCIONField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DESCRIPCIONField, value) != true))
                {
                    this.DESCRIPCIONField = value;
                    this.RaisePropertyChanged("DESCRIPCION");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DETALLE
        {
            get
            {
                return this.DETALLEField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DETALLEField, value) != true))
                {
                    this.DETALLEField = value;
                    this.RaisePropertyChanged("DETALLE");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NUM_CPE
        {
            get
            {
                return this.NUM_CPEField;
            }
            set
            {
                if ((object.ReferenceEquals(this.NUM_CPEField, value) != true))
                {
                    this.NUM_CPEField = value;
                    this.RaisePropertyChanged("NUM_CPE");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "RPTA_COMPRA", Namespace = "http://schemas.datacontract.org/2004/07/TT.EOL.Level.BE.RESPUESTA")]
    [System.SerializableAttribute()]
    public partial class RPTA_COMPRA : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
    {

        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CODIGOField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DESCRIPCIONField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DETALLEField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ESTADOField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NUM_OPEField;

        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string CODIGO
        {
            get
            {
                return this.CODIGOField;
            }
            set
            {
                if ((object.ReferenceEquals(this.CODIGOField, value) != true))
                {
                    this.CODIGOField = value;
                    this.RaisePropertyChanged("CODIGO");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DESCRIPCION
        {
            get
            {
                return this.DESCRIPCIONField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DESCRIPCIONField, value) != true))
                {
                    this.DESCRIPCIONField = value;
                    this.RaisePropertyChanged("DESCRIPCION");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DETALLE
        {
            get
            {
                return this.DETALLEField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DETALLEField, value) != true))
                {
                    this.DETALLEField = value;
                    this.RaisePropertyChanged("DETALLE");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ESTADO
        {
            get
            {
                return this.ESTADOField;
            }
            set
            {
                if ((object.ReferenceEquals(this.ESTADOField, value) != true))
                {
                    this.ESTADOField = value;
                    this.RaisePropertyChanged("ESTADO");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NUM_OPE
        {
            get
            {
                return this.NUM_OPEField;
            }
            set
            {
                if ((object.ReferenceEquals(this.NUM_OPEField, value) != true))
                {
                    this.NUM_OPEField = value;
                    this.RaisePropertyChanged("NUM_OPE");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "RPTA_DOC_TRIB_COMPRA", Namespace = "http://schemas.datacontract.org/2004/07/TT.EOL.Level.BE.RESPUESTA")]
    [System.SerializableAttribute()]
    public partial class RPTA_DOC_TRIB_COMPRA : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
    {

        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string COD_RPTAField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DESCRIPCIONField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DETALLEField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private byte[] DOC_TRIB_PDFField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DOC_TRIB_RPTAField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DOC_TRIB_XMLField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Nullable<long> NUM_OPEField;

        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string COD_RPTA
        {
            get
            {
                return this.COD_RPTAField;
            }
            set
            {
                if ((object.ReferenceEquals(this.COD_RPTAField, value) != true))
                {
                    this.COD_RPTAField = value;
                    this.RaisePropertyChanged("COD_RPTA");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DESCRIPCION
        {
            get
            {
                return this.DESCRIPCIONField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DESCRIPCIONField, value) != true))
                {
                    this.DESCRIPCIONField = value;
                    this.RaisePropertyChanged("DESCRIPCION");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DETALLE
        {
            get
            {
                return this.DETALLEField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DETALLEField, value) != true))
                {
                    this.DETALLEField = value;
                    this.RaisePropertyChanged("DETALLE");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public byte[] DOC_TRIB_PDF
        {
            get
            {
                return this.DOC_TRIB_PDFField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DOC_TRIB_PDFField, value) != true))
                {
                    this.DOC_TRIB_PDFField = value;
                    this.RaisePropertyChanged("DOC_TRIB_PDF");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DOC_TRIB_RPTA
        {
            get
            {
                return this.DOC_TRIB_RPTAField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DOC_TRIB_RPTAField, value) != true))
                {
                    this.DOC_TRIB_RPTAField = value;
                    this.RaisePropertyChanged("DOC_TRIB_RPTA");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DOC_TRIB_XML
        {
            get
            {
                return this.DOC_TRIB_XMLField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DOC_TRIB_XMLField, value) != true))
                {
                    this.DOC_TRIB_XMLField = value;
                    this.RaisePropertyChanged("DOC_TRIB_XML");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<long> NUM_OPE
        {
            get
            {
                return this.NUM_OPEField;
            }
            set
            {
                if ((this.NUM_OPEField.Equals(value) != true))
                {
                    this.NUM_OPEField = value;
                    this.RaisePropertyChanged("NUM_OPE");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "CPE_DOC_TRIBUTARO_COMPRA", Namespace = "http://schemas.datacontract.org/2004/07/TT.EOL.Level.BE")]
    [System.SerializableAttribute()]
    public partial class CPE_DOC_TRIBUTARO_COMPRA : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
    {

        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private byte[] DOC_TRIB_PDFField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DOC_TRIB_XML_ENVIOField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DOC_TRIB_XML_RPTAField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Nullable<long> NUM_CPEField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Nullable<long> NUM_RESUMField;

        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public byte[] DOC_TRIB_PDF
        {
            get
            {
                return this.DOC_TRIB_PDFField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DOC_TRIB_PDFField, value) != true))
                {
                    this.DOC_TRIB_PDFField = value;
                    this.RaisePropertyChanged("DOC_TRIB_PDF");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DOC_TRIB_XML_ENVIO
        {
            get
            {
                return this.DOC_TRIB_XML_ENVIOField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DOC_TRIB_XML_ENVIOField, value) != true))
                {
                    this.DOC_TRIB_XML_ENVIOField = value;
                    this.RaisePropertyChanged("DOC_TRIB_XML_ENVIO");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DOC_TRIB_XML_RPTA
        {
            get
            {
                return this.DOC_TRIB_XML_RPTAField;
            }
            set
            {
                if ((object.ReferenceEquals(this.DOC_TRIB_XML_RPTAField, value) != true))
                {
                    this.DOC_TRIB_XML_RPTAField = value;
                    this.RaisePropertyChanged("DOC_TRIB_XML_RPTA");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<long> NUM_CPE
        {
            get
            {
                return this.NUM_CPEField;
            }
            set
            {
                if ((this.NUM_CPEField.Equals(value) != true))
                {
                    this.NUM_CPEField = value;
                    this.RaisePropertyChanged("NUM_CPE");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<long> NUM_RESUM
        {
            get
            {
                return this.NUM_RESUMField;
            }
            set
            {
                if ((this.NUM_RESUMField.Equals(value) != true))
                {
                    this.NUM_RESUMField = value;
                    this.RaisePropertyChanged("NUM_RESUM");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    





}

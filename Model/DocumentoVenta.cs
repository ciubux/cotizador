using Model.ServiceReferencePSE;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class DocumentoVenta : Auditoria
    {

        [Display(Name = "Fecha Emisión:")]
        public DateTime fechaEmision { get; set; }

        [Display(Name = "Hora Emisión:")]
        public DateTime horaEmision { get; set; }

        [Display(Name = "Fecha Vencimiento:")]
        public DateTime fechaVencimiento { get; set; }

        [Display(Name = "Tipo Pago:")]
        public TipoPago tipoPago { get; set; }

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
            [Display(Name = "CRÉDITO A 30 DÍAS")]
            Crédito30 = 4,
            [Display(Name = "CRÉDITO A 60 DÍAS")]
            Crédito60 = 5,
            [Display(Name = "CRÉDITO A 90 DÍAS")]
            Crédito90 = 6,
            [Display(Name = "CRÉDITO A 120 DÍAS")]
            Crédito120 = 7,
            [Display(Name = "CRÉDITO A 20 DÍAS")]
            Crédito20 = 8,
            [Display(Name = "CRÉDITO A 45 DÍAS")]
            Crédito45 = 9
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

        [Display(Name = "Correo Envío:")]
        public String correoEnvio { get; set; }

        [Display(Name = "Correo Copia:")]
        public String correoCopia { get; set; }

        [Display(Name = "Correo Oculato:")]
        public String correoOculto { get; set; }

        public TipoDocumento tipoDocumento { get; set; }

        
        public Venta venta { get; set; }

        public Guid idDocumentoVenta { get; set; }

        [Display(Name = "Número:")]
        public String numero { get; set; }

        [Display(Name = "Serie:")]
        public String serie { get; set; }

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

        public bool estaAnulado { get; set; }


        public CPE_CABECERA_BE cPE_CABECERA_BE;

        public List<CPE_DETALLE_BE> cPE_DETALLE_BEList;

        public List<CPE_DAT_ADIC_BE> cPE_DAT_ADIC_BEList;

        public List<CPE_DOC_REF_BE> cPE_DOC_REF_BEList;

        public List<CPE_ANTICIPO_BE> cPE_ANTICIPO_BEList;

        public List<CPE_FAC_GUIA_BE> cPE_FAC_GUIA_BEList;

        public List<CPE_DOC_ASOC_BE> cPE_DOC_ASOC_BEList;


        public GlobalEnumTipoOnline globalEnumTipoOnline;



        [Display(Name = "Fecha Emisión Desde:")]
        public DateTime fechaEmisionDesde { get; set; }

        [Display(Name = "Fecha Emisión Hasta:")]
        public DateTime fechaEmisionHasta { get; set; }






















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

        [Display(Name = "Observaciones:")]
        public String observaciones { get; set; }

        [Display(Name = "Está Anulado:")]
        public Boolean esAnulado { get; set; }

        [Display(Name = "Usuario:")]
        public Usuario usuario { get; set; }

        public List<VentaDetalle> ventaDetalleList { get; set; }
    }
}

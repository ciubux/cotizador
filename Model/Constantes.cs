using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public static class Constantes
    {
        public static String formatoDosDecimales = "{0:0.00}";
        public static String formatoUnDecimal = "{0:0.0}";
        public static String formatoCuatroDecimales = "{0:0.0000}";
        public static String formatoSeisDecimales = "{0:0.000000}";
        public static String formatoOchoDecimales = "{0:0.00000000}";
        public static String formatoFecha = "dd/MM/yyyy";
        public static String formatoHora = "HH:mm";
        public static String MENSAJE_SI = "Sí";
        public static String MENSAJE_NO = "No";
        public static Char PAD = '0';
        public static int LONGITUD_NUMERO = 10;
        public static String UBIGEO_VACIO = "000000";

        public static String LABEL_DIRECCION_ENTREGA_VACIO = "Seleccione Dirección de Entrega";


        public static String VAR_SESSION_COTIZACION = "cotizacion";
        public static String VAR_SESSION_COTIZACION_BUSQUEDA = "cotizacionBusqueda";
        public static String VAR_SESSION_COTIZACION_LISTA = "cotizacionList";
        public static String VAR_SESSION_COTIZACION_VER = "cotizacionVer";

        public static String VAR_SESSION_PEDIDO = "pedido";
        public static String VAR_SESSION_PEDIDO_BUSQUEDA = "pedidoBusqueda";
        public static String VAR_SESSION_PEDIDO_LISTA = "pedidoList";
        public static String VAR_SESSION_PEDIDO_VER = "pedidoVer";

        public static String VAR_SESSION_CLIENTE = "cliente";

        public static String VAR_SESSION_GUIA = "guiaRemision";
        public static String VAR_SESSION_GUIA_BUSQUEDA = "guiaRemisionBusqueda";
        public static String VAR_SESSION_GUIA_LISTA = "guiaRemisionList";
        public static String VAR_SESSION_GUIA_VER = "guiaRemisionVer";

        public static String VAR_SESSION_VENTA = "venta";
        public static String VAR_SESSION_VENTA_BUSQUEDA = "ventaBusqueda";
        public static String VAR_SESSION_VENTA_LISTA = "ventaList";
        public static String VAR_SESSION_VENTA_VER = "ventaVer";

        public static String VAR_SESSION_FACTURA = "factura";
        public static String VAR_SESSION_FACTURA_BUSQUEDA = "facturaBusqueda";
        public static String VAR_SESSION_FACTURA_LISTA = "facturaList";
        public static String VAR_SESSION_FACTURA_VER = "facturaVer";

        public static String VAR_SESSION_BOLETA = "boleta";
        public static String VAR_SESSION_BOLETA_BUSQUEDA = "boletaBusqueda";
        public static String VAR_SESSION_BOLETA_LISTA = "boletaList";
        public static String VAR_SESSION_BOLETA_VER = "boletaVer";

        public static String VAR_SESSION_NOTA_CREDITO = "notaCredito";
        public static String VAR_SESSION_NOTA_CREDITO_BUSQUEDA = "notaCreditoBusqueda";
        public static String VAR_SESSION_NOTA_CREDITO_LISTA = "notaCreditoList";
        public static String VAR_SESSION_NOTA_CREDITO_VER = "notaCreditoVer";

        public static String VAR_SESSION_NOTA_DEBITO = "notaDebito";
        public static String VAR_SESSION_NOTA_DEBITO_BUSQUEDA = "notaDebitoBusqueda";
        public static String VAR_SESSION_NOTA_DEBITO_LISTA = "notaDebitoList";
        public static String VAR_SESSION_NOTA_DEBITO_VER = "notaDebitoVer";

        public static String VAR_SESSION_PAGINA = "pagina";
        public static String VAR_SESSION_USUARIO = "usuario";

        public static Decimal VARIACION_PRECIO_ITEM_PEDIDO = 0.01M;

        public static String USER_EOL_TEST = "";
        public static String PASSWORD_EOL_TEST = "";
        public static String USER_EOL_PROD = "";
        public static String PASSWORD_EOL_PROD = "";
        public static string RUC_MP = "20509411671";
        public static String AMBIENTE_EOL = "";

        public static String USER_EOL {
            get { return AMBIENTE_EOL.Equals("TEST") ? USER_EOL_TEST : USER_EOL_PROD; }
        }

        public static String PASSWORD_EOL
        {
            get { return AMBIENTE_EOL.Equals("TEST") ? PASSWORD_EOL_TEST : PASSWORD_EOL_PROD; }
        }

        public static bool ES_EOL_PRODUCCION
        {
            get { return AMBIENTE_EOL.Equals("PROD"); }
        }



        public static Decimal IGV = 0.18M;
        public static Decimal PORCENTAJE_MAX_APROBACION = 3.00M;
        public static String SIMBOLO_SOL = "S/";
        public static int PLAZO_OFERTA_DIAS = 15;
        public static int DEBUG = 1;
        public static int DIAS_MAX_BUSQUEDA_PRECIOS = 730;
        public static int MILISEGUNDOS_AUTOGUARDADO = 5000;
        public static int DIAS_MAX_VIGENCIA_PRECIOS_COTIZACION = 180;
        public static int DIAS_MAX_VIGENCIA_PRECIOS_PEDIDO = 365;
        public static String OBSERVACION = "* Condiciones de pago: al contado.\n" +
                                       "* Entrega en almacén del cliente, 48 horas luego de la recepción del pedido o la orden de compra.\n" +
                                       "* (para productos no stockeables o primeras compras, consultar plazo).\n";



        public static String EOL_CPE_RESPUESTA_BE_CODIGO_OK = "001";
        public static String EOL_CPE_RESPUESTA_BE_CODIGO_ERROR_DATA = "002";
        public static String EOL_CPE_RESPUESTA_BE_CODIGO_ERROR_TECNICO = "003";

        public static String EOL_RPTA_BE_CODIGO_CONSULTA_EXITOSA = "001";
        public static String EOL_RPTA_BE_CODIGO_ERROR_USUARIO_INVALIDO = "002";
        public static String EOL_RPTA_BE_CODIGO_ERROR_NO_EXISTE_COMPROBANTE = "003";
        public static String EOL_RPTA_BE_CODIGO_ERROR_CONSULTA = "004";
        public static String EOL_RPTA_BE_CODIGO_FALLO_CONEXION_EOL = "005";

        public static String EOL_RPTA_BE_CODIGO_CONSULTA_EXITOSA_STR = "Consulta Exitosa.";
        public static String EOL_RPTA_BE_CODIGO_ERROR_USUARIO_INVALIDO_STR = "Usuario y/o password inválidos.";
        public static String EOL_RPTA_BE_CODIGO_ERROR_NO_EXISTE_COMPROBANTE_STR = "No se encontró el Comprobante electrónico en el sistema.";
        public static String EOL_RPTA_BE_CODIGO_ERROR_CONSULTA_STR = "Error en el Proceso de Consulta.";
        public static String EOL_RPTA_BE_CODIGO_FALLO_CONEXION_EOL_STR = "Fallo conexión a EOL.";

      











        //Numero de Página

        public static int BUSQUEDA_COTIZACION = 0;
        public static int MANTENIMIENTO_COTIZACION = 1;
        public static int BUSQUEDA_PEDIDO = 2;
        public static int MANTENIMIENTO_PEDIDO = 3;
        public static int BUSQUEDA_GUIA_REMISION = 4;
        public static int MANTENIMIENTO_GUIA_REMISION = 5;



        public enum paginas {
            [Display(Name = "BUSQUEDA COTIZACION")]
            BusquedaCotizaciones = 0,
            [Display(Name = "MANTENIMIENTO COTIZACION")]
            MantenimientoCotizacion = 1,
            [Display(Name = "BUSQUEDA PEDIDO")]
            BusquedaPedidos = 2,
            [Display(Name = "MANTENIMIENTO PEDIDO")]
            MantenimientoPedido = 3,
            [Display(Name = "BUSQUEDA GUIA REMISION")]
            BusquedaGuiasRemision = 4,
            [Display(Name = "MANTENIMIENTO GUIA REMISION")]
            MantenimientoGuiaRemision = 5,

            [Display(Name = "BUSQUEDA FACTURAS")]
            BusquedaFacturas = 6,
            [Display(Name = "MANTENIMIENTO FACTURA")]
            MantenimientoFactura = 7,   
            
            [Display(Name = "BUSQUEDA BOLETAS")]
            BusquedaBoletas = 8,
            [Display(Name = "MANTENIMIENTO BOLETA")]
            MantenimientoBoleta = 9,

            [Display(Name = "BUSQUEDA NOTAS DE CRÉDITO")]
            BusquedaNotasCredito = 10,
            [Display(Name = "MANTENIMIENTO NOTA DE CRÉDITO")]
            MantenimientoNotaCredito = 11,

            [Display(Name = "BUSQUEDA NOTAS DE DÉBITO")]
            BusquedaNotasDebito = 12,
            [Display(Name = "MANTENIMIENTO NOTA DE DÉBITO")]
            MantenimientoNotaDebito = 13,



            [Display(Name = "Imprimir Guia Remisión")]
            ImprimirGuiaRemision = 14,


            [Display(Name = "BUSQUEDA VENTAS")]
            BusquedaVentas = 15,
            [Display(Name = "MANTENIMIENTO VENTAS")]
            MantenimientoVenta = 16,


            [Display(Name = "BUSQUEDA CLIENTES")]
            BusquedaClientes = 17,
            [Display(Name = "MANTENIMIENTO CLIENTE")]
            MantenimientoCliente = 18


        };


    }

}

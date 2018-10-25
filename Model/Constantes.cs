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
        /*FORMATOS*/
        public static String formatoDosDecimales = "{0:0.00}";
        public static String formatoUnDecimal = "{0:0.0}";
        public static String formatoCuatroDecimales = "{0:0.0000}";
        public static String formatoSeisDecimales = "{0:0.000000}";
        public static String formatoOchoDecimales = "{0:0.00000000}";
        public static String formatoFecha = "dd/MM/yyyy";
        public static String formatoFechaCPE = "yyyy-MM-dd";
        public static String formatoHora = "HH:mm";
        public static String MENSAJE_SI = "Sí";
        public static String MENSAJE_NO = "No";
        public static Char PAD = '0';
        public static int LONGITUD_NUMERO = 6;
        public static String UBIGEO_VACIO = "000000";
        public static int DIAS_DESDE_BUSQUEDA = 10;
        public static int ID_VENDEDOR_POR_ASIGNAR = 43;



        public static String LABEL_DIRECCION_ENTREGA_VACIO = "Seleccione Dirección de Entrega";

        public static String PREFIJO_NOTA_CREDITO_FACTURA = "FC";
        public static String PREFIJO_NOTA_CREDITO_BOLETA = "BC";
        public static String PREFIJO_NOTA_DEBITO_FACTURA = "BD";
        public static String PREFIJO_NOTA_DEBITO_BOLETA = "FD";

        /*Tipos Documentos Cliente*/
        public static String TIPO_DOCUMENTO_CLIENTE_RUC = "6";
        public static String TIPO_DOCUMENTO_CLIENTE_DNI = "1";
        public static String TIPO_DOCUMENTO_CLIENTE_CARNET_EXTRANJERIA = "4";


        public static int MOTIVO_EXTORNO_ANULACION_OPERACION = 1;
        public static int MOTIVO_EXTORNO_DEVOLUCION_TOTAL = 6;
        public static int MOTIVO_EXTORNO_DEVOLUCION_PARCIAL = 7;


        /*CONSTANTES PARA VARIABLES DE SESION*/

        public static String VAR_SESSION_COTIZACION = "cotizacion";
        public static String VAR_SESSION_COTIZACION_BUSQUEDA = "cotizacionBusqueda";
        public static String VAR_SESSION_COTIZACION_LISTA = "cotizacionList";
        public static String VAR_SESSION_COTIZACION_VER = "cotizacionVer";

        public static String VAR_SESSION_PEDIDO = "pedido";
        public static String VAR_SESSION_PEDIDO_BUSQUEDA = "pedidoBusqueda";
        public static String VAR_SESSION_PEDIDO_LISTA = "pedidoList";
        public static String VAR_SESSION_PEDIDO_VER = "pedidoVer";

        public static String VAR_SESSION_PEDIDO_COMPRA = "pedidoCompra";
        public static String VAR_SESSION_PEDIDO_COMPRA_BUSQUEDA = "pedidoCompraBusqueda";
        public static String VAR_SESSION_PEDIDO_COMPRA_LISTA = "pedidoCompraList";
        public static String VAR_SESSION_PEDIDO_COMPRA_VER = "pedidoCompraVer";

        public static String VAR_SESSION_PEDIDO_ALMACEN = "pedidoAlmacen";
        public static String VAR_SESSION_PEDIDO_ALMACEN_BUSQUEDA = "pedidoAlmacenBusqueda";
        public static String VAR_SESSION_PEDIDO_ALMACEN_LISTA = "pedidoAlmacenList";
        public static String VAR_SESSION_PEDIDO_ALMACEN_VER = "pedidoAlmacenVer";

        public static String VAR_SESSION_CLIENTE = "cliente";
        public static String VAR_SESSION_CLIENTE_BUSQUEDA = "clienteBusqueda";
        public static String VAR_SESSION_CLIENTE_LISTA = "clienteList";
        public static String VAR_SESSION_CLIENTE_VER = "clienteVer";

        public static String VAR_SESSION_PROVEEDOR = "proveedor";

        public static String VAR_SESSION_GUIA = "guiaRemision";
        public static String VAR_SESSION_GUIA_BUSQUEDA = "guiaRemisionBusqueda";
        public static String VAR_SESSION_GUIA_BUSQUEDA_FACTURA_CONSOLIDADA = "guiaRemisionBusquedaFacturaConsolidada";
        public static String VAR_SESSION_GUIA_BUSQUEDA_LISTA_IDS = "guiaRemisionBusquedaFacturaConsolidadaIDs";
        public static String VAR_SESSION_GUIA_LISTA = "guiaRemisionList";
        public static String VAR_SESSION_GUIA_LISTA_FACTURA_CONSOLIDADA = "guiaRemisionListFacturaConsolidada";
        public static String VAR_SESSION_GUIA_VER = "guiaRemisionVer";

        public static String VAR_SESSION_NOTA_INGRESO = "notaIngreso";
        public static String VAR_SESSION_NOTA_INGRESO_BUSQUEDA = "notaIngresoBusqueda";
        public static String VAR_SESSION_NOTA_INGRESO_LISTA = "notaIngresoLista";
        public static String VAR_SESSION_NOTA_INGRESO_VER = "notaIngresoVer";

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
        public static String ENDPOINT_ADDRESS_EOL_TEST = "";
        public static DateTime HORA_CORTE_CREDITOS_LIMA = DateTime.Now;
        public static String USER_EOL_PROD = "";
        public static String PASSWORD_EOL_PROD = "";
        public static String ENDPOINT_ADDRESS_EOL_PROD = "";
        public static string RUC_MP = "20509411671";
        public static String AMBIENTE_EOL = "";
        public static String CPE_CABECERA_BE_ID = "";
        public static String CPE_CABECERA_BE_COD_GPO = "";
        public static String MAIL_COMUNICACION_FACTURAS = "";
        public static String PASSWORD_MAIL_COMUNICACION_FACTURAS = "";   


        public static String USER_EOL {
            get { return AMBIENTE_EOL.Equals("TEST") ? USER_EOL_TEST : USER_EOL_PROD; }
        }

        public static String ENDPOINT_ADDRESS_EOL
        {
            get { return AMBIENTE_EOL.Equals("TEST") ? ENDPOINT_ADDRESS_EOL_TEST : ENDPOINT_ADDRESS_EOL_PROD; }
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
        public static int DESCARGAR_XML = 1;


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

              

        public enum paginas {
            /*COTIZACION*/
            [Display(Name = "BUSQUEDA COTIZACION")]
            BusquedaCotizaciones = 0,
            [Display(Name = "MANTENIMIENTO COTIZACION")]
            MantenimientoCotizacion = 1,
            /*PEDIDO VENTA*/
            [Display(Name = "BUSQUEDA PEDIDO")]
            BusquedaPedidos = 2,
            [Display(Name = "MANTENIMIENTO PEDIDO")]
            MantenimientoPedido = 3,
            /*GUIA REMISION*/
            [Display(Name = "BUSQUEDA GUIA REMISION")]
            BusquedaGuiasRemision = 4,
            [Display(Name = "MANTENIMIENTO GUIA REMISION")]
            MantenimientoGuiaRemision = 5,
            /*DOCUMENTOS DE VENTA*/
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
            /*IMPRESION GUIA REMISION*/
            [Display(Name = "IMPRIMIR GUÍA REMISIÓN")]
            ImprimirGuiaRemision = 14,
            /*VENTAS*/
            [Display(Name = "BUSQUEDA VENTAS")]
            BusquedaVentas = 15,
            [Display(Name = "MANTENIMIENTO VENTAS")]
            MantenimientoVenta = 16,
            /*CLIENTES*/
            [Display(Name = "BUSQUEDA CLIENTES")]
            BusquedaClientes = 17,
            [Display(Name = "MANTENIMIENTO CLIENTE")]
            MantenimientoCliente = 18,
            /*CONSOLIDAR GUÍAS*/
            [Display(Name = "BUSQUEDA GUIAS REMISION CONSOLIDAR FACTURA")]
            BusquedaGuiasRemisionConsolidarFactura = 19,
            /*NOTAS INGRESO*/
            [Display(Name = "BUSQUEDA NOTA INGRESO")]
            BusquedaNotasIngreso = 20,
            [Display(Name = "MANTENIMIENTO NOTA INGRESO")]
            MantenimientoNotaIngreso = 21,
            [Display(Name = "IMPRIMIR NOTA INGRESO")]
            ImprimirNotaIngreso = 22,
            /*PEDIDO COMPRA*/
            [Display(Name = "BUSQUEDA PEDIDO COMPRA")]
            BusquedaPedidosCompra = 23,
            [Display(Name = "MANTENIMIENTO PEDIDO COMPRA")]
            MantenimientoPedidoCompra = 24,
            /*PEDIDO ALMACEN*/
            [Display(Name = "BUSQUEDA PEDIDO ALMACEN")]
            BusquedaPedidosAlmacen = 25,
            [Display(Name = "MANTENIMIENTO PEDIDO ALMACEN")]
            MantenimientoPedidoAlmacen = 26,
            /*PRODUCTOS*/
            [Display(Name = "BUSQUEDA PRODUCTOS")]
            BusquedaProductos = 27,
            [Display(Name = "MANTENIMIENTO PRODUCTO")]
            MantenimientoProducto = 28,
        };


        #region PARAMETROS MAIL
        public static String SERVER_SMPTP = "smtp.office365.com";
        public static int PUERTO_SERVER_SMPTP = 587;


        #endregion


    }

}

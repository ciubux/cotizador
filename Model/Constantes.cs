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
        public static String formatoDecimalesPrecioNeto = "{0:0.00}";
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
        public static int LONGITUD_NUMERO_GRUPO = 4;
        public static String UBIGEO_VACIO = "000000";
        public static int DIAS_DESDE_BUSQUEDA = 10;
        public static int ID_VENDEDOR_POR_ASIGNAR = 43;
        public static int DIAS_MAX_COTIZACION_TRANSITORIA = 10;
        public static Guid ID_SEDE_LIMA = Guid.Parse("15526227-2108-4113-B46A-1C8AB5C0E581");

        public static String URL_VER_PEDIDO = "http://cotizadormp.azurewebsites.net/Pedido?idPedido=";



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
        public static String VAR_SESSION_COTIZACION_APROBADA = "cotizacionAprobada";
        public static String VAR_SESSION_COTIZACION_BUSQUEDA = "cotizacionBusqueda";
        public static String VAR_SESSION_COTIZACION_LISTA = "cotizacionList";
        public static String VAR_SESSION_COTIZACION_VER = "cotizacionVer";
        public static String VAR_SESSION_COTIZACION_GRUPAL = "cotizacionGrupal";

        public static String VAR_SESSION_PEDIDO = "pedido";
        public static String VAR_SESSION_PEDIDO_BUSQUEDA = "pedidoBusqueda";
        public static String VAR_SESSION_PEDIDO_APROBACION = "pedidoAprobacion";
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


        public static String VAR_SESSION_GRUPO_CLIENTE = "grupoCliente";
        public static String VAR_SESSION_GRUPO_CLIENTE_MIEMBROS = "grupoClienteMiembros";
        public static String VAR_SESSION_GRUPO_CLIENTE_BUSQUEDA = "grupoClienteBusqueda";
        public static String VAR_SESSION_GRUPO_CLIENTE_LISTA = "grupoClienteList";
        public static String VAR_SESSION_GRUPO_CLIENTE_VER = "grupoClienteVer";

        public static String VAR_SESSION_PRODUCTO = "producto";
        public static String VAR_SESSION_PRODUCTO_BUSQUEDA = "productoBusqueda";
        public static String VAR_SESSION_PRODUCTO_LISTA = "productoList";
        public static String VAR_SESSION_PRODUCTO_VER = "productoVer";

        public static String VAR_SESSION_ORIGEN = "origen";
        public static String VAR_SESSION_ORIGEN_BUSQUEDA = "origenBusqueda";
        public static String VAR_SESSION_ORIGEN_LISTA = "origenList";
        public static String VAR_SESSION_ORIGEN_VER = "origenVer";

        public static String VAR_SESSION_VENDEDOR = "vendedor";
        public static String VAR_SESSION_VENDEDOR_BUSQUEDA = "vendedorBusqueda";
        public static String VAR_SESSION_VENDEDOR_LISTA = "vendedorList";
        public static String VAR_SESSION_VENDEDOR_VER = "vendedorVer";

        public static String VAR_SESSION_LOGCAMBIO = "logcambio";
        public static String VAR_SESSION_LOGCAMBIO_BUSQUEDA = "logcambioBusqueda";
        public static String VAR_SESSION_LOGCAMBIO_LISTA = "logcambioList";
        public static String VAR_SESSION_LOGCAMBIO_VER = "logcambioVer";

        public static String VAR_SESSION_NOTIFICACIONDOCUMENTOVENTA = "notificacionDocumentoVenta";
        public static String VAR_SESSION_NOTIFICACIONDOCUMENTOVENTA_BUSQUEDA = "notificacionDocumentoVentaBusqueda";
        public static String VAR_SESSION_NOTIFICACIONDOCUMENTOVENTA_LISTA = "notificacionDocumentoVentaList";
        public static String VAR_SESSION_NOTIFICACIONDOCUMENTOVENTA_VER = "notificacionDocumentoVentaVer";

        public static String VAR_SESSION_SUBDISTRIBUIDOR = "subDistribuidor";
        public static String VAR_SESSION_SUBDISTRIBUIDOR_BUSQUEDA = "subDistribuidorBusqueda";
        public static String VAR_SESSION_SUBDISTRIBUIDOR_LISTA = "subDistribuidorList";
        public static String VAR_SESSION_SUBDISTRIBUIDOR_VER = "subDistribuidorVer";

        public static String VAR_SESSION_PROVEEDOR = "proveedor";

        public static String VAR_SESSION_GUIA = "guiaRemision";
        public static String VAR_SESSION_GUIA_BUSQUEDA = "guiaRemisionBusqueda";
        public static String VAR_SESSION_GUIA_BUSQUEDA_FACTURA_CONSOLIDADA = "guiaRemisionBusquedaFacturaConsolidada";
        public static String VAR_SESSION_GUIA_CONSOLIDADA = "guiaRemisionConsolidada";
        
        public static String VAR_SESSION_RESUMEN_CONSOLIDADO = "documentoVentaResumenConsolidado";
        public static String VAR_SESSION_GUIA_LISTA = "guiaRemisionList";
        public static String VAR_SESSION_GUIA_LISTA_FACTURA_CONSOLIDADA = "guiaRemisionListFacturaConsolidada";
        public static String VAR_SESSION_GUIA_VER = "guiaRemisionVer";
        public static String VAR_SESSION_GUIA_BUSQUEDA_LISTA_IDS = "guiaRemisionBusquedaFacturaConsolidadaIDs";

        public static String VAR_SESSION_NOTA_INGRESO = "notaIngreso";
        public static String VAR_SESSION_NOTA_INGRESO_BUSQUEDA = "notaIngresoBusqueda";
        public static String VAR_SESSION_NOTA_INGRESO_LISTA = "notaIngresoLista";
        public static String VAR_SESSION_NOTA_INGRESO_VER = "notaIngresoVer";

        public static String VAR_SESSION_VENTA = "venta";
        public static String VAR_SESSION_VENTA_BUSQUEDA = "ventaBusqueda";
        public static String VAR_SESSION_VENTA_LISTA = "ventaList";
        public static String VAR_SESSION_VENTA_VER = "ventaVer";

        public static String VAR_SESSION_COMPRA = "compra";
        public static String VAR_SESSION_COMPRA_BUSQUEDA = "compraBusqueda";
        public static String VAR_SESSION_COMPRA_LISTA = "compraList";
        public static String VAR_SESSION_COMPRA_VER = "compraVer";

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

        public static String VAR_SESSION_DOCUMENTO_COMPRA = "documentoCompra";
        public static String VAR_SESSION_DOCUMENTO_COMPRA_BUSQUEDA = "documentoCompraBusqueda";
        public static String VAR_SESSION_DOCUMENTO_COMPRA_LISTA = "documentoCompraList";
        public static String VAR_SESSION_DOCUMENTO_COMPRA_VER = "documentoCompraVer";

        public static String VAR_SESSION_NOTA_CREDITO_COMPRA = "notaCreditoCompra";
        public static String VAR_SESSION_NOTA_CREDITO_COMPRA_BUSQUEDA = "notaCreditoCompraBusqueda";
        public static String VAR_SESSION_NOTA_CREDITO_COMPRA_LISTA = "notaCreditoCompraList";
        public static String VAR_SESSION_NOTA_CREDITO_COMPRA_VER = "notaCreditoCompraVer";

        public static String VAR_SESSION_NOTA_DEBITO_COMPRA = "notaDebitoCompra";
        public static String VAR_SESSION_NOTA_DEBITO_COMPRA_BUSQUEDA = "notaDebitoCompraBusqueda";
        public static String VAR_SESSION_NOTA_DEBITO_COMPRA_LISTA = "notaDebitoCompraList";
        public static String VAR_SESSION_NOTA_DEBITO_COMPRA_VER = "notaDebitoCompraVer";
        
        public static String VAR_SESSION_ROL = "rol";
        public static String VAR_SESSION_ROL_BUSQUEDA = "rolBusqueda";
        public static String VAR_SESSION_ROL_LISTA = "rolList";
        public static String VAR_SESSION_ROL_VER = "rolVer";


        public static String VAR_SESSION_PAGINA = "pagina";
        

        public static String VAR_SESSION_USUARIO_LISTA = "usuarioList";
        public static String VAR_SESSION_USUARIO = "usuario";
        public static String VAR_SESSION_USUARIO_MANTENEDOR = "usuarioMantenedor";
        public static String VAR_SESSION_USUARIO_BUSQUEDA = "usuarioBusqueda";
        public static String VAR_SESSION_USUARIO_LISTA_MANTENEDOR = "usuarioMantenedorList";
        public static String VAR_SESSION_USUARIO_VER = "usuarioVer";


        public static String VAR_SESSION_PERMISO_LISTA = "permisoList";
        
        public static String VAR_SESSION_PERMISO = "permiso";
        public static String VAR_SESSION_PERMISO_MANTENEDOR = "permisoMantenedor";
        public static String VAR_SESSION_PERMISO_BUSQUEDA = "permisoBusqueda";
        public static String VAR_SESSION_PERMISO_LISTA_MANTENEDOR = "permisoMantenedorList";
        public static String VAR_SESSION_PERMISO_VER = "permisoVer";


        public static String VAR_SESSION_ASIGNACION_PERMISOS = "AsignacionPermisos";
        public static String VAR_SESSION_CAMBIO_ASIGNACION_PERMISOS = "CambioAsignacionPermisos";

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
        public static String MAIL_COMUNICACION_PEDIDOS_NO_ATENDIDOS = "";
        public static String PASSWORD_MAIL_COMUNICACION_PEDIDOS_NO_ATENDIDOS = "";

        public static List<Producto> DESCUENTOS_LIST;
        public static List<Producto> CARGOS_LIST;

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
        //public static 
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

        public static String ASUNTO_ANULACION_FACTURA = "MP INSTITUCIONAL S.A.C. / Anulación Facturación Electrónica ";




        /// <summary>
        /// Códigos de Permisos
        /// </summary>
        /// 
        public const String ADMINISTRA_PERMISOS = "P001";
        public const String APRUEBA_COTIZACIONES_LIMA = "P002";
        public const String APRUEBA_COTIZACIONES_PROVINCIAS = "P003";
        public const String APRUEBA_PEDIDOS_LIMA = "P004";
        public const String APRUEBA_PEDIDOS_PROVINCIAS = "P005";
        public const String CREA_GUIAS = "P006";
        public const String ADMINISTRA_GUIAS_LIMA = "P007";
        public const String ADMINISTRA_GUIAS_PROVINCIAS = "P008";
        public const String CREA_DOCUMENTOS_VENTA = "P009";
        public const String ADMINISTRA_DOCUMENTOS_VENTA_LIMA = "P010";
        public const String ADMINISTRA_DOCUMENTOS_VENTA_PROVINCIAS = "P011";
        public const String CREA_COTIZACIONES_PROVINCIAS = "P012";
        public const String TOMA_PEDIDOS_PROVINCIAS = "P013";
        public const String PROGRAMA_PEDIDOS = "P014";
        public const String MODIFICA_MAESTRO_CLIENTES = "P015";
        public const String MODIFICA_MAESTRO_PRODUCTOS = "P016";
        public const String VISUALIZA_DOCUMENTOS_VENTA = "P017";
        public const String VISUALIZA_PEDIDOS_LIMA = "P018";
        public const String VISUALIZA_GUIAS_REMISION = "P019";
        public const String VISUALIZA_COTIZACIONES = "P020";
        public const String LIBERA_PEDIDOS = "P021";
        public const String BLOQUEA_PEDIDOS = "P022";
        public const String APRUEBA_ANULACIONES = "P023";
        public const String CREA_NOTAS_CREDITO = "P024";
        public const String VISUALIZA_PEDIDOS_PROVINCIAS = "P025";
        public const String VISUALIZA_COSTOS = "P026";
        public const String CREA_NOTAS_DEBITO = "P027";
        public const String REALIZA_REFACTURACION = "P028";
        public const String VISUALIZA_MARGEN = "P029";
        public const String CONFIRMA_STOCK = "P030";
        public const String CREA_COTIZACIONES_LIMA = "P031";
        public const String TOMA_PEDIDOS_LIMA = "P032";
        public const String TOMA_PEDIDOS_COMPRA = "P033";
        public const String TOMA_PEDIDOS_ALMACEN = "P034";
        public const String DEFINE_PLAZO_CREDITO = "P035";
        public const String DEFINE_MONTO_CREDITO = "P036";
        public const String DEFINE_RESPONSABLE_COMERCIAL = "P037";
        public const String DEFINE_SUPERVISOR_COMERCIAL = "P038";
        public const String DEFINE_ASISTENTE_ATENCION_CLIENTE = "P039";
        public const String DEFINE_RESPONSABLE_PORTAFOLIO = "P040";
        public const String MODIFICA_PEDIDO_VENTA_FECHA_ENTREGA_HASTA = "P041";
        public const String BLOQUEA_CLIENTES = "P042";
        public const String MODIFICA_CANALES = "P043";
        public const String REALIZA_CARGA_MASIVA_PEDIDOS = "P044";
        public const String MODIFICA_PEDIDO_FECHA_ENTREGA_EXTENDIDA = "P045";
        public const String CREA_FACTURA_CONSOLIDADA_MULTIREGIONAL = "P046";
        public const String CREA_FACTURA_CONSOLIDADA_LOCAL = "P047";
        public const String VISUALIZA_GUIAS_PENDIENTES_FACTURACION = "P048";
        public const String MODIFICA_NEGOCIACION_MULTIREGIONAL = "P049";
        public const String BUSCA_SEDES_GRUPO_CLIENTE = "P050";
        public const String MODIFICA_PRODUCTO = "P051";
        public const String APRUEBA_PEDIDOS_COMPRA = "P052";
        public const String APRUEBA_PEDIDOS_ALMACEN = "P053";
        public const String CREA_COTIZACIONES_GRUPO_CLIENTE = "P054";
        public const String CREA_COTIZACIONES_GRUPALES = "P055";
        public const String APRUEBA_COTIZACIONES_GRUPALES = "P056";
        public const String MODIFICA_SUBDISTRIBUIDOR = "P057";
        public const String MODIFICA_ORIGEN = "P058";
        public const String REALIZA_CARGA_MASIVA_PRODUCTOS = "P059";
        public const String REALIZA_CARGA_MASIVA_CLIENTES = "P060";
        public const String MODIFICA_GRUPO_CLIENTES = "P061";
        public const String VISUALIZA_GRUPO_CLIENTES = "P062";
        public const String VISUALIZA_PRODUCTOS = "P063";
        public const String VISUALIZA_SUBDISTRIBUIDORES = "P064";
        public const String VISUALIZA_ORIGENES = "P065";
        public const String VISUALIZA_CLIENTES = "P066";
        public const String ELIMINA_COTIZACIONES_ACEPTADAS = "P067";
        public const String MODIFICA_CANASTA_CLIENTE = "P068";
        public const String MODIFICA_CANASTA_GRUPO_CLIENTE = "P069";
        public const String MODIFICA_MIEMBROS_GRUPO_CLIENTE = "P070";
        public const String ASIGNA_SUBDISTRIBUIDOR = "P071";
        public const String CREA_DOCUMENTOS_COMPRA = "P072";
        public const String VISUALIZA_DOCUMENTOS_COMPRA = "P073";        
        public const String MODIFICA_ROL = "P810";
        public const String VISUALIZA_ROLES = "P811";
        public const String MODIFICA_USUARIO = "P812";
        public const String VISUALIZA_USUARIOS = "P813";

        public const String MODIFICA_VENDEDORES = "P821";
        public const String VISUALIZA_VENDEDORES = "P822";
        public const String MODIFICA_LOGCAMBIO = "P823";
        public const String VISUALIZA_LOGCAMBIO = "P824";

        public const String VISUALIZA_DOCUMENTOVENTANOTIFICACION = "P420";

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
            CargaMasivaProductos = 28,
            [Display(Name = "CREATE/UPDATE PRODUCTO")]
            MantenimientoProductos = 29,
            [Display(Name = "BUSQUEDA ORIGEN")]
            BusquedaOrigenes = 30,
            [Display(Name = "CREATE/UPDATE ORIGEN")]
            MantenimientoOrigen= 31,
            [Display(Name = "BUSQUEDA SUBDISTRIBUIDOR")]
            BusquedaSubDistribuidores = 32,
            [Display(Name = "CREATE/UPDATE SUBDISTRIBUIDOR")]
            MantenimientoSubDistribuidor = 33,
            [Display(Name = "APROBAR PEDIDOS")]
            AprobarPedidos = 34,
            [Display(Name = "MANTENIMIENTO COTIZACION GRUPAL")]
            MantenimientoCotizacionGrupal = 35,
            /*GRUPOS CLIENTE*/
            [Display(Name = "BUSQUEDA GRUPOS CLIENTE")]
            BusquedaGrupoClientes = 36,
            [Display(Name = "MANTENIMIENTO GRUPOS CLIENTE")]
            MantenimientoGrupoCliente = 37,
            [Display(Name = "ASIGNACION PERMISOS")]
            AsignacionPermisos = 38,
            [Display(Name = "MIEMBROS GRUPO")]
            MiembrosGrupoCliente = 39,
            [Display(Name = "VALIDACIONES PENDIENTES")]
            AlertasValidacionPendientes = 40,
            /*DOCUMENTOS DE VENTA*/
            [Display(Name = "BUSQUEDA DOCUMENTOS COMPRA")]
            BusquedaDocumentosCompra = 41,

            [Display(Name = "MANTENIMIENTO COMPRA")]
            MantenimientoCompra = 42,

            /*BUSQUEDA DE VENDEDORES*/
            [Display(Name = "BUSQUEDA VENDEDORES")]
            BusquedaVendedores = 43,
            [Display(Name = "MANTENIMIENTO VENDEDORES")]
            MantenimientoVendedores = 44,

            /*BUSQUEDA DE LOGCAMBIO*/
            [Display(Name = "BUSQUEDA LOGCAMBIO")]
            BusquedaLogCambio = 45,
            [Display(Name = "MANTENIMIENTO LOGCAMBIO")]
            MantenimientoLogCambio = 46,


            /*NOTIFICACION DOCUMENTO DE VENTA*/
            [Display(Name = "BUSQUEDA DOCUMENTOVENTANOTIFICACION")]
            BusquedaDocumentoVentaNotificacion = 47,

            /*ROLES*/
            [Display(Name = "BUSQUEDA ROLES")]
            BusquedaRoles = 101,
            [Display(Name = "MANTENIMIENTO ROL")]
            MantenimientoRol = 102,

            /*USUARIOS*/
            [Display(Name = "BUSQUEDA USUARIOS")]
            BusquedaUsuarios = 103,

            [Display(Name = "MANTENIMIENTO USUARIO")]
            MantenimientoUsuario = 104,

            /*USUARIOS*/
            [Display(Name = "BUSQUEDA PERMISO")]
            BusquedaPermisos = 105,

            [Display(Name = "MANTENIMIENTO PERMISO")]
            MantenimientoPermisos = 106



        };


        #region PARAMETROS MAIL
        public static String SERVER_SMPTP = "smtp.office365.com";
        public static int PUERTO_SERVER_SMPTP = 587;
        public static String MAIL_ADMINISTRADOR = "c.cornejo@mpinstitucional.com";
        public static String MAIL_SOPORTE_TI = "soporte.ti@mpinstitucional.com";
        public static String MAIL_CREDITOS = "creditos@mpinstitucional.com";
        public static String MAIL_COBRANZAS = "cobranzas@mpinstitucional.com";


        #endregion

        public static string CUERPO_CORREO_SOLICITUD_DE_BAJA = @"<div>Estimado Cliente,</div>
        <div>&nbsp;</div>
        <div>Se le informa que se ha solicitado la ANULACIÓN de la factura electr&oacute;nica N&deg;&nbsp;{xxxx}-{xxxxxxxx}, enviada por correo electr&oacute;nico el d&iacute;a&nbsp;{xx/xx}&nbsp; por haberse detectado un error en la emisi&oacute;n.&nbsp; &nbsp;</div>
        <div>&nbsp;</div>
        <div>NOTA:&nbsp;<span style='font-size: small;'>Si desea cambiar el buz&oacute;n de correo para la recepci&oacute;n de documentos electr&oacute;nicos, por favor ingrese a </span><a id='LPlnk624034' href='http://mpinstitucional.com/buzon.facturas' target='_blank' rel='noopener noreferrer' data-auth='NotApplicable'><span style='font-size: small;'>http://mpinstitucional.com/buzon.facturas</span></a>
        <span style='font-size: small'>y complete el formulario.&nbsp;</span></div>
        <div>&nbsp;</div>
        <div>Agradeceremos confirmar la recepci&oacute;n del presente correo.</div>
        <div>&nbsp;</div>
        <div>Facturaci&oacute;n Electr&oacute;nica</div>
        <div><span style='font-family: Calibri,Helvetica,sans-serif,EmojiFont,Apple Color Emoji,Segoe UI Emoji,NotoColorEmoji,Segoe UI Symbol,Android Emoji,EmojiSymbols; font-size: medium;'><strong>MP INSTITUCIONAL S.A.C.</strong></span></div>";



        public static string CUERPO_CORREO_SOLICITUD_DE_BAJA2 = @"<html>
        <head>
            <meta http-equiv=Content-Type content='text/html; charset=windows-1252'>
            <meta name=Generator content='Microsoft Word 15 (filtered)'>
            <style>
                <!--
                /* Font Definitions */
                @font-face {
                    font-family: 'Cambria Math';
                    panose-1: 2 4 5 3 5 4 6 3 2 4;
                }

                @font-face {
                    font-family: Calibri;
                    panose-1: 2 15 5 2 2 2 4 3 2 4;
                }

                @font-face {
                    font-family: 'Segoe UI';
                    panose-1: 2 11 5 2 4 2 4 2 2 3;
                }
                /* Style Definitions */
                p.MsoNormal, li.MsoNormal, div.MsoNormal {
                    margin-top: 0cm;
                    margin-right: 0cm;
                    margin-bottom: 8.0pt;
                    margin-left: 0cm;
                    line-height: 107%;
                    font-size: 11.0pt;
                    font-family: 'Calibri',sans-serif;
                }

                a:link, span.MsoHyperlink {
                    color: blue;
                    text-decoration: underline;
                }

                a:visited, span.MsoHyperlinkFollowed {
                    color: #954F72;
                    text-decoration: underline;
                }

                .MsoPapDefault {
                    margin-bottom: 8.0pt;
                    line-height: 107%;
                }

                @page WordSection1 {
                    size: 595.3pt 841.9pt;
                    margin: 70.85pt 3.0cm 70.85pt 3.0cm;
                }

                div.WordSection1 {
                    page: WordSection1;
                }
                -->
            </style>

        </head>

        <body lang=ES link=blue vlink='#954F72'>

            <div class=WordSection1>

                <p class=MsoNormal style='margin-bottom:0cm;margin-bottom:.0001pt;line-height:
        normal;background:white'>
                    <span style='font-size:11.5pt;font-family:'Segoe UI',sans-serif;
        color:#212121'>Estimado Cliente,</span>
                </p>

                <p class=MsoNormal style='margin-bottom:0cm;margin-bottom:.0001pt;line-height:
        normal;background:white'>
                    <span style='font-size:11.5pt;font-family:'Segoe UI',sans-serif;
        color:#212121'>&nbsp;</span>
                </p>

                <p class=MsoNormal style='margin-bottom:0cm;margin-bottom:.0001pt;line-height:
        normal;background:white'>
                    <span style='font-size:11.5pt;font-family:'Segoe UI',sans-serif;
        color:#212121'>
                        Se le informa que la factura electrÃ³nica NÂ°&nbsp;<span >F{xxx}-{xxxxxxxx}</span>, enviada por correo
                        electrÃ³nico el dÃ­a&nbsp;<span>{xx/xx}</span>&nbsp;ha
                        sido ANULADA en SUNAT, por haberse detectado un error en la emisiÃ³n.&nbsp;
                        &nbsp;
                    </span>
                </p>

                <p class=MsoNormal style='margin-bottom:0cm;margin-bottom:.0001pt;line-height:
        normal;background:white'>
                    <span style='font-size:11.5pt;font-family:'Segoe UI',sans-serif;
        color:#212121'>&nbsp;</span>
                </p>

                <p class=MsoNormal style='margin-bottom:0cm;margin-bottom:.0001pt;line-height:
        normal;background:white'>
                    <span style='font-size:11.5pt;font-family:'Segoe UI',sans-serif;
        color:#212121'>NOTA:&nbsp;</span><span style='font-size:10.0pt;font-family:
        'Segoe UI',sans-serif;color:#212121'>
                        Si desea cambiar el buzÃ³n de correo para
                        la recepciÃ³n de documentos electrÃ³nicos, por favor ingrese a
                    </span><span style='font-size:11.5pt;font-family:'Segoe UI',sans-serif;color:#212121'>
                        <a href='http://mpinstitucional.com/buzon.facturas' target='_blank'>
                            <span style='font-size:10.0pt'>http://mpinstitucional.com/buzon.facturas</span>
                        </a>
                    </span>
                </p>

                <p class=MsoNormal style='margin-bottom:0cm;margin-bottom:.0001pt;line-height:
        normal;background:white'>
                    <span style='font-size:10.0pt;font-family:'Segoe UI',sans-serif;
        color:#212121'>y complete el formulario.&nbsp;</span>
                </p>

                <p class=MsoNormal style='margin-bottom:0cm;margin-bottom:.0001pt;line-height:
        normal;background:white'>
                    <span style='font-size:11.5pt;font-family:'Segoe UI',sans-serif;
        color:#212121'>&nbsp;</span>
                </p>

                <p class=MsoNormal style='margin-bottom:0cm;margin-bottom:.0001pt;line-height:
        normal;background:white'>
                    <span style='font-size:11.5pt;font-family:'Segoe UI',sans-serif;
        color:#212121'>Agradeceremos confirmar la recepciÃ³n del presente correo.</span>
                </p>

                <p class=MsoNormal style='margin-bottom:0cm;margin-bottom:.0001pt;line-height:
        normal;background:white'>
                    <span style='font-size:11.5pt;font-family:'Segoe UI',sans-serif;
        color:#212121'>&nbsp;</span>
                </p>

                <p class=MsoNormal style='margin-bottom:0cm;margin-bottom:.0001pt;line-height:
        normal;background:white'>
                    <span style='font-size:11.5pt;font-family:'Segoe UI',sans-serif;
        color:#212121'>FacturaciÃ³n ElectrÃ³nica</span>
                </p>

                <p class=MsoNormal style='margin-bottom:0cm;margin-bottom:.0001pt;line-height:
        normal;background:white'>
                    <b>
                        <span style='font-size:12.0pt;color:#212121'>
                            MP
                            INSTITUCIONAL S.A.C.
                        </span>
                    </b>
                </p>

                <p class=MsoNormal>&nbsp;</p>

            </div>

        </body>

        </html>
        ";

        }

}

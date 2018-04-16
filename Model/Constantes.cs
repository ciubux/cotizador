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

        public static String VAR_SESSION_GUIA = "guiaRemision";
        public static String VAR_SESSION_GUIA_BUSQUEDA = "guiaRemisionBusqueda";
        public static String VAR_SESSION_GUIA_LISTA = "guiaRemisionList";
        public static String VAR_SESSION_GUIA_VER = "guiaRemisionVer";

        public static String VAR_SESSION_VENTA = "venta";
        public static String VAR_SESSION_VENTA_BUSQUEDA = "ventaBusqueda";
        public static String VAR_SESSION_VENTA_LISTA = "ventaList";
        public static String VAR_SESSION_VENTA_VER = "ventaVer";

        public static String VAR_SESSION_DOC_VENTA = "documentoVenta";
        public static String VAR_SESSION_DOC_VENTA_BUSQUEDA = "documentoVentaBusqueda";
        public static String VAR_SESSION_DOC_VENTA_LISTA = "documentoVentaList";
        public static String VAR_SESSION_DOC_VENTA_VER = "documentoVentaVer";

        public static String VAR_SESSION_PAGINA = "pagina";
        public static String VAR_SESSION_USUARIO = "usuario";

      

        public static Decimal VARIACION_PRECIO_ITEM_PEDIDO = 0.01M;



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

        
        
        //Numero de Página
        public static int BUSQUEDA_COTIZACION = 0;
        public static int MANTENIMIENTO_COTIZACION = 1;
        public static int BUSQUEDA_PEDIDO = 2;
        public static int MANTENIMIENTO_PEDIDO = 3;
        public static int BUSQUEDA_GUIA_REMISION = 4;
        public static int MANTENIMIENTO_GUIA_REMISION = 5;
        



        public enum paginas {
            [Display(Name = "Mis Cotizaciones")]
            BusquedaCotizaciones = 0,
            [Display(Name = "Cotizacion")]
            MantenimientoCotizacion = 1,
            [Display(Name = "Mis Pedidos")]
            BusquedaPedidos = 2,
            [Display(Name = "Pedido")]
            MantenimientoPedido = 3,
            [Display(Name = "Mis Guías Remisión")]
            BusquedaGuiasRemision = 4,
            [Display(Name = "Guia Remisión")]
            MantenimientoGuiaRemision = 5,
            [Display(Name = "Mis Facturas")]
            BusquedaFacturas = 6,
            [Display(Name = "Factura")]
            MantenimientoFactura = 7,
            [Display(Name = "Mis Ventas")]
            BusquedaVentas = 8,
            [Display(Name = "Venta")]
            MantenimientoVenta = 9,
            [Display(Name = "Mis Boletas")]
            BusquedaBoletas = 10,
            [Display(Name = "Boletas")]
            MantenimientoBoleta = 11,
            [Display(Name = "Mis Notas de Crédito")]
            BusquedaNotasCredito = 12,
            [Display(Name = "Notas de Crédito")]
            MantenimientoNotaCredito = 13,
        };


    }

}

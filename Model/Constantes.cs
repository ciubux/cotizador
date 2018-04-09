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

        public static String VAR_SESSION_TRANSPORTISTAS = "transportistas";



        public static Decimal IGV = 0.18M;
        public static Decimal PORCENTAJE_MAX_APROBACION = 3.00M;
        public static String SIMBOLO_SOL = "S/";
        public static int PLAZO_OFERTA_DIAS = 15;
        public static int DEBUG = 0;
        public static int DIAS_MAX_BUSQUEDA_PRECIOS = 730;
        public static int MILISEGUNDOS_AUTOGUARDADO = 5000;
        public static String OBSERVACION = "* Condiciones de pago: al contado.\n" +
                                       "* Entrega en almacén del cliente, 48 horas luego de la recepción del pedido o la orden de compra.\n" +
                                       "* (para productos no stockeables o primeras compras, consultar plazo).\n";


        public static int BUSQUEDA_COTIZACION = 0;
        public static int MANTENIMIENTO_COTIZACION = 1;
        public static int BUSQUEDA_PEDIDO = 2;
        public static int MANTENIMIENTO_PEDIDO = 3;
        public static int BUSQUEDA_GUIA_REMISION = 4;
        public static int MANTENIMIENTO_GUIA_REMISION = 5;



        public enum paginas {
            [Display(Name = "Mis Cotizaciones")]
            misCotizaciones = 0,
            [Display(Name = "Cotizacion")]
            Cotizacion = 1,
            [Display(Name = "Mis Pedidos")]
            misPedidos = 2,
            [Display(Name = "Pedido")]
            Pedido = 3
        };


    }

}

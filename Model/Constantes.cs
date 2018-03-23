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
        public static String MENSAJE_SI = "Sí";
        public static String MENSAJE_NO = "No";


        public static Decimal IGV = 0.18M;
        public static Decimal PORCENTAJE_MAX_APROBACION = 3.00M;
        public static String SIMBOLO_SOL = "S/";
        public static int PLAZO_OFERTA_DIAS = 15;
        public static int DEBUG = 0;
        public static int DIAS_MAX_BUSQUEDA_PRECIOS = 730;
        public static String OBSERVACION = "* Condiciones de pago: al contado.\n" +
                                       "* Entrega en almacén del cliente, 48 horas luego de la recepción del pedido o la orden de compra.\n" +
                                       "* (para productos no stockeables o primeras compras, consultar plazo).\n";
        

        public enum paginas {
            [Display(Name = "Mis Cotizaciones")]
            misCotizaciones = 0,
            [Display(Name = "Solo Observaciones")]
            Cotizacion = 1
        };


    }

}

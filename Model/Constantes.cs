using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Constantes
    {
        public static Decimal IGV = 0.18M;
        public static String decimalFormat = "{0:0.00}";
        public static String unDecimalFormat = "{0:0.0}";
        public static String cuatroDecimalFormat = "{0:0.0000}";
        public static Decimal porcentajeLimiteSinAprobacion = 3.00M;
        public static String simboloMonedaSol = "S/";
        public static int plazoOfertaDias = 15;
        public static int debug = 0;
        public static String observacionesCotizacion = "* Condiciones de pago: al contado.\n" +
                                       "* Entrega en almacén del cliente, 48 horas luego de la recepción del pedido o la orden de compra.\n" +
                                       "* (para productos no stockeables o primeras compras, consultar plazo).\n";
    }
}

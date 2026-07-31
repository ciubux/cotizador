using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ClienteProductoReservadoMovimiento : Auditoria
    {
        public const string RECARGA_RESERVA = "RECARGA_RESERVA";
        public const string REDUCCION_RESERVA = "REDUCCION_RESERVA";
        public const string AGREGAR_REGISTRO_RESERVA = "AGREGAR_REGISTRO_RESERVA";
        public const string REDUCIR_REGISTRO_RESERVA = "REDUCIR_REGISTRO_RESERVA";
        public const string REGISTRO_INICIAL = "REGISTRO_INICIAL";
        public const string VENTA = "VENTA";
        public const string EXTORNO_VENTA = "EXTORNO_VENTA";
        public const string ANULACION_VENTA = "ANULACION_VENTA";

        public Guid idClienteProductoReservadoMovimiento{ get; set; }

        public string tipo { get; set; }

        public string tipoDescripcion { get { 
                string descripcion = string.Empty;

                switch (this.tipo)
                {
                    case ClienteProductoReservadoMovimiento.RECARGA_RESERVA: descripcion = "RECARGA RESERVA"; break;
                    case ClienteProductoReservadoMovimiento.REDUCCION_RESERVA: descripcion = "REDUCCIÓN RESERVA"; break;
                    case ClienteProductoReservadoMovimiento.AGREGAR_REGISTRO_RESERVA: descripcion = "ACTUALIZACIÓN RESERVA BASE"; break;
                    case ClienteProductoReservadoMovimiento.REDUCIR_REGISTRO_RESERVA: descripcion = "ACTUALIZACIÓN RESERVA BASE"; break;
                    case ClienteProductoReservadoMovimiento.REGISTRO_INICIAL: descripcion = "REGISTRO INICIAL"; break;
                    case ClienteProductoReservadoMovimiento.VENTA: descripcion = "VENTA"; break;
                    case ClienteProductoReservadoMovimiento.EXTORNO_VENTA: descripcion = "DEVOLUCIÓN VENTA"; break;
                    case ClienteProductoReservadoMovimiento.ANULACION_VENTA: descripcion = "ANULACIÓN VENTA"; break;
                }

                return descripcion;
            } 
        }

        public int cantidad { get; set; }
        public string informacionAdicional { get; set; }
        public DateTime fechaMovmiento { get; set; }

    }
}

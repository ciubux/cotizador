using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class MovimientoAlmacen : Auditoria
    {
        public int numero { get; set; }

        public enum tiposMovimiento {
            [Display(Name = "Ingreso")]
            Ingreso = 'I',
            [Display(Name = "Salida")]
            Salida = 'S',
        };

        public tiposMovimiento tipoMovimiento { get; set; }

        public enum estadosMovimiento {
            [Display(Name = "Anulado")]
            Anulado = 0,
            [Display(Name = "Activo")]
            Activo = 1,
        }

        public estadosMovimiento estadoMovimiento { get; set; }

        [Display(Name = "Fecha:")]
        public DateTime fechaMovimiento { get; set; }

        [Display(Name = "Serie:")]
        public int serieDocumento { get; set; }

        [Display(Name = "Número:")]
        public int numeroDocumento { get; set; }

    }
}

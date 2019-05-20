using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class SeguimientoMovimientoAlmacenEntrada
    {
        public SeguimientoMovimientoAlmacenEntrada()
        {
            this.estado = SeguimientoMovimientoAlmacenEntrada.estadosSeguimientoMovimientoAlmacenEntrada.Recibido;
        }


        public enum estadosSeguimientoMovimientoAlmacenEntrada
        {
            [Display(Name = "Todos")]
            Todos = -1,
            [Display(Name = "Anulado")]
            Anulado = 0,
            [Display(Name = "Recibido")]
            Recibido = 1
        };

        public estadosSeguimientoMovimientoAlmacenEntrada estado { get; set; }
        public String observacion { get; set; }

        public Usuario usuario { get; set; }

        public String estadoString {
            get
            {
                return EnumHelper<estadosSeguimientoMovimientoAlmacenEntrada>.GetDisplayValue(this.estado);
            }
        }

    }
}

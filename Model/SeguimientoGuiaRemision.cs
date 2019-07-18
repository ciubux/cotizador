using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class SeguimientoMovimientoAlmacenSalida
    {
        public SeguimientoMovimientoAlmacenSalida()
        {
            this.estado = SeguimientoMovimientoAlmacenSalida.estadosSeguimientoMovimientoAlmacenSalida.Enviado;
        }


        public enum estadosSeguimientoMovimientoAlmacenSalida
        {
            [Display(Name = "Todos")]
            Todos = -1,
            [Display(Name = "Anulado")]
            Anulado = 0,
            [Display(Name = "Enviado")]
            Enviado = 1,
            [Display(Name = "Entregado")]
            Entregado = 2
        };

        public estadosSeguimientoMovimientoAlmacenSalida estado { get; set; }
        public String observacion { get; set; }

        public Usuario usuario { get; set; }

        public String estadoString {
            get
            {
                return EnumHelper<estadosSeguimientoMovimientoAlmacenSalida>.GetDisplayValue(this.estado);
            }
        }

    }
}

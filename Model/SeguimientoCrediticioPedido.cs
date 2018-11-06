using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class SeguimientoCrediticioPedido : Auditoria
    {
        public SeguimientoCrediticioPedido()
        {
            this.estado = estadosSeguimientoCrediticioPedido.PendienteLiberación;
        }


        public enum estadosSeguimientoCrediticioPedido
        {
            [Display(Name = "Todos")]
            Todos = -1,
            [Display(Name = "Pendiente Liberación")]
            PendienteLiberación = 0,
            [Display(Name = "Liberado")]
            Liberado = 1,
            [Display(Name = "Bloqueado")]
            BLoqueado = 2
        };

        public estadosSeguimientoCrediticioPedido estado { get; set; }
        public String observacion { get; set; }

        public Usuario usuario { get; set; }

        public String estadoString {
            get
            {
                return EnumHelper<estadosSeguimientoCrediticioPedido>.GetDisplayValue(this.estado);
            }
        }

    }
}

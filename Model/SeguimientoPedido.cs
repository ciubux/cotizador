using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class SeguimientoPedido
    {
        public SeguimientoPedido()
        {
            this.estado = estadosSeguimientoPedido.PendienteEnvio;
        }


        public enum estadosSeguimientoPedido
        {
            [Display(Name = "Todos")]
            Todos = -1,
            [Display(Name = "Pendiente Envío")]
            PendienteEnvio = 0,
            [Display(Name = "Envío Denegado")]
            Denegado = 1,
            [Display(Name = "Pedido Enviado")]
            Enviado = 2,
            [Display(Name = "Pedido Enviado Parcialmente")]
            EnvioParcial = 3,
            [Display(Name = "Pedido Entregado")]
            Entregado = 4,
            [Display(Name = "Pedido Entregado Parcialmente")]
            EntregaParcial = 5,
            [Display(Name = "Pedido Rechazado")]
            Rechazado = 6,
            [Display(Name = "En Edición")]
            Edicion = 7,
            [Display(Name = "Anulado")]
            Anulado = 8
        };

        public estadosSeguimientoPedido estado { get; set; }
        public String observacion { get; set; }

        public Usuario usuario { get; set; }

        public String estadoString {
            get
            {
                return EnumHelper<estadosSeguimientoPedido>.GetDisplayValue(this.estado);
            }
        }

    }
}

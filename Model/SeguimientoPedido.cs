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
            this.estado = estadosSeguimientoPedido.Ingresado;
        }

        public enum estadosSeguimientoPedido
        {
            [Display(Name = "Todos")]
            Todos = -1,
            [Display(Name = "Pendiente Aprobación de Ingreso")]
            PendienteAprobacion = 0,
            [Display(Name = "Ingresado")]
            Ingresado = 1,
            [Display(Name = "Denegado")]
            Denegado = 2,
            [Display(Name = "Programado")]
            Programado = 3,
            [Display(Name = "Atendido")]
            Atendido = 4,
            [Display(Name = "Atendido Parcialmente")]
            AtendidoParcialmente = 5,
            [Display(Name = "En Edición")]
            Edicion = 6,
            [Display(Name = "Eliminado")]
            Eliminada = 7,
            [Display(Name = "Facturado")]
            Facturado = 8
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

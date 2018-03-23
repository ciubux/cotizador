﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class SeguimientoCotizacion
    {
        public SeguimientoCotizacion()
        {
            this.estado = estadosSeguimientoCotizacion.Aprobada;
        }


        public enum estadosSeguimientoCotizacion
        {
            [Display(Name = "Todos")]
            Todos = -1,
            [Display(Name = "Pendiente Aprobación")]
            Pendiente = 0,
            [Display(Name = "Aprobada")]
            Aprobada = 1,
            [Display(Name = "Denegada")]
            Denegada = 2,
            [Display(Name = "Aceptada")]
            Aceptada = 3,
            [Display(Name = "Rechazada")]
            Rechazada = 4,
            [Display(Name = "En Edición")]
            Edicion = 5
        };

        public estadosSeguimientoCotizacion estado { get; set; }
        public String observacion { get; set; }

        public Usuario usuario { get; set; }

        public String estadoString {
            get
            {
                return EnumHelper<estadosSeguimientoCotizacion>.GetDisplayValue(this.estado);
            }
        }

    }
}
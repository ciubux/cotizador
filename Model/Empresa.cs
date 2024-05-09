using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class Empresa : Auditoria
    {
        public int idEmpresa { get; set; }

        [Display(Name = "Código:")]
        public String codigo { get; set;  }
        [Display(Name = "Nombre:")]
        public String nombre { get; set; }

        [Display(Name = "Ruc:")]
        public String ruc { get; set; }

        [Display(Name = "Razon Social:")]
        public String razonSocial { get; set; }

        [Display(Name = "Url Web:")]
        public String urlWeb { get; set; }

        [Display(Name = "Factor Costo:")]
        public decimal factorCosto { get; set; }

        [Display(Name = "Porcentaje Margen Mínimo(Mk Down):")]
        public decimal porcentajeMargenMinimo { get; set; }

        [Display(Name = "Porcentaje Descuento Infra Margen(Mk Down):")]
        public decimal porcentajeDescuentoInframargen { get; set; }

        [Display(Name = "Porcentaje Ganancia Máxima(Mk Down):")]
        public decimal porcentajeMDGanaciaMax { get; set; }


        public bool atencionTerciarizada { get; set; }

        public override string ToString()
        {
            return this.codigo + " - " + this.nombre;
        }
    }
}
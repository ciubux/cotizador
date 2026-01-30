using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class Vehiculo : Auditoria
    {
        public int idVehiculo { get; set; }

        [Display(Name = "Placa:")]
        public String placa { get; set; }

        [Display(Name = "Marca:")]
        public String marca { get; set; }

        [Display(Name = "Modelo:")]
        public String modelo { get; set; }

        public Ciudad ciudad { get; set; }

        public override string ToString()
        {
            return this.placa;
        }
    }
}
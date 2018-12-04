using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class GuiaRemisionValidacion
    {

        public TiposErrorValidacion tipoErrorValidacion { get; set; }
        public String tipoErrorValidacionString
        {
            get
            {
                return EnumHelper<TiposErrorValidacion>.GetDisplayValue(this.tipoErrorValidacion);
            }
        }

        public enum TiposErrorValidacion
        {
            [Display(Name = "Ninguno")]
            NoExisteError = 0,
            [Display(Name = "Existen Guías de Remisión con Fecha Posterior")]
            ExisteDocumentoFechaPosterior = 1
        }

        public String descripcionError { get; set; }


    }
}

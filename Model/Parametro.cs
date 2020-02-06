using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Parametro
    {
        public Guid idParametro { get; set; }

        [Display(Name = "Codigo:")]
        public string codigo { get; set; }

        public string descripcion { get; set; }

        public string valor { get; set; }
    }
}

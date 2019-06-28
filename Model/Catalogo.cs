using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class Catalogo
    {

        public int catalogoId { get; set; }
        [Display(Name = "Estado:")]
        public int estado { get; set; }
        public string codigo { get; set; }
        public string nombre { get; set; }
        [Display(Name = "¿Persiste?:")]
        public int puede_persistir { get; set; }

        public int id_catalogo_campo { get; set; }

        public int tabla_referencia { get; set; }

        public int orden { get; set; }

        public int campo_referencia { get; set; }
    }
}

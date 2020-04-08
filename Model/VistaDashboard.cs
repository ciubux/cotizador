using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class VistaDashboard
    {        
        public int idVistaDashboard { get; set; }

        [Display(Name = "Tipo:")]
        public int idTipoVistaDashboard {get; set;}

        [Display(Name = "Código:")]
        public String codigo { get; set; }
        [Display(Name = "Nombre:")]
        public String nombre { get; set; }
        [Display(Name = "Descripción:")]
        public String descripcion  { get; set; }
        [Display(Name = "Ancho:")]
        public int bloquesAncho { get; set; }
        [Display(Name = "Alto:")]
        public int altoPx { get; set; }
        [Display(Name = "Estado:")]
        public int estado { get; set; }
       
        public TipoVistaDashboard tipoVistaDashboard{ get; set; }

    }
}

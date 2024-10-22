using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class Fabricante : Auditoria
    {
        public Fabricante() 
        {
            
        }

        public int idFabricante { get; set; }

        [Display(Name = "Código:")]
        public String codigo { get; set; }

        [Display(Name = "Nombre:")]
        public String nombreUsual { get; set; }

    }
}
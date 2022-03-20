using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class Area : Auditoria
    {
        public Area() 
        {
            
        }

        public int idArea { get; set; }

        [Display(Name = "Nombre:")]
        public String nombre { get; set; }

    }
}
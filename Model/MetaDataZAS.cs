using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class MetaDataZAS
    {
        [Display(Name = "Código:")]
        public String codigo { get; set; }

        [Display(Name = "Valor:")]
        public String valor { get; set; }


        [Display(Name = "Descripción Corta:")]
        public String descripcionCorta { get; set; }


    }
}
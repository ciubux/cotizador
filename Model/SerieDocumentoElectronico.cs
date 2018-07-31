﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class SerieDocumentoElectronico
    {
        public Guid idSerieDocumentoElectronico { get; set; }
        
        [Display(Name = "Serie:")]
        public String serie { get; set; }

        public Ciudad sedeMP { get; set; }

        public bool esPrincipal { get; set; }
        
    }
}
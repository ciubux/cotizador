﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class VentaDetalle : DocumentoDetalle
    {
        public Guid idVentaDetalle { get; set; }
        public Guid venta { get; set; }   
      
    }
}
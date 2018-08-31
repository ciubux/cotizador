using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class MovimientoDetalle : DocumentoDetalle
    {

        public Guid idMovimientoDetalle { get; set; }
        public Guid idMovimiento { get; set; }   
      
    }
}

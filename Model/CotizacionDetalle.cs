using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class CotizacionDetalle : DocumentoDetalle
    {
        public Guid idCotizacionDetalle { get; set; }
        public Guid idCotizacion { get; set; }

    }
}

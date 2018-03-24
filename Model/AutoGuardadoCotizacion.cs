using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class AutoGuardadoCotizacion : Auditoria
    {
        public Guid idAutoGuardadoCotizacion { get; set; }
        public Guid idCotizacion { get; set; }
        public String cotizacionSerializada { get; set; }
    }
}
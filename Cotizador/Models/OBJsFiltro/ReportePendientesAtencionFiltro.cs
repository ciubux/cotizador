using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cotizador.Models.OBJsFiltro
{
    public class ReportePendientesAtencionFiltro
    {
        public Guid idCiudad { get; set; }

        public String descripcion { get; set; }
        public String sku { get; set; }
        public String familia { get; set; }
        public String proveedor { get; set; }

        public DateTime fechaEntregaInicio { get; set; }
        public DateTime fechaEntregaFin { get; set; }

    }
    
}
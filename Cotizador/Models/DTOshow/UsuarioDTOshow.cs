using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cotizador.Models.DTOshow
{
    public class UsuarioDTOshow

    {

        public Guid idUsuario { get; set; }
        public bool apruebaCotizaciones { get; set; }
        public String maximoPorcentajeDescuentoAprobacion { get; set; }
        public String nombre { get; set; }

     
        
            
    }
}
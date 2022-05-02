using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cotizador.Models.DTOshow
{
    public class CierreStockDetalleDTOshow
    {
        
        public Guid idProducto { get; set; }
        
        public String producto_descripcion { get; set; }
        public String producto_sku { get; set; }
        public String unidadConteo { get; set; }
        public int cantidadConteo { get; set; }
        public int diferenciaCantidadValidacion { get; set; }
        public int stockValidable { get; set; }


    }
}


using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cotizador.Models.DTOshow
{
    public class CierreStockDTOshow
    {
        
        public Guid idArchivoAdjunto { get; set; }

        public String fechaDesc { get; set; }
        public String sede { get; set; }
        public String usuario { get; set; }
        public String usuarioRVS { get; set; }
        public String fechaRVSDesc { get; set; }

        public List<CierreStockDetalleDTOshow> detalles { get; set; }

    }
}
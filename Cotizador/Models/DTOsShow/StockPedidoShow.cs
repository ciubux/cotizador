using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cotizador.Models.DTOsShow
{
    public class StockPedidoShow
    {
        public String texto { get; set; }
        public String textoStock { get; set; }
        public int semaforoStock { get; set; }
        public bool esStock { get; set; }
    }
    
}
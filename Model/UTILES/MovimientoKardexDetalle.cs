using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.UTILES
{
    public class MovimientoKardexDetalle
    {
        public int cantidadConteo { get; set; }

        public int stockConteo { get; set; }
        public decimal stockUnidad { get; set; }

        public string unidadMovimiento { get; set; }

        public int cantidadMovimiento { get; set; }

        public DateTime fecha { get; set; }

        public int tipoMovimiento { get; set; }

        public int tipoDocumento { get; set; }
        public string serieDocumento { get; set; }
        public string numeroDocumento { get; set; }

        public string NumeroDocumentoDesc { 
            get {
                string nro = "";
                if (this.tipoDocumento == 1)
                {
                    nro += "G";
                }
                if (this.tipoDocumento == 2)
                {
                    nro += "NI";
                }

                return nro + serieDocumento + "-" + numeroDocumento;
            } 
        }

        public string FechaDesc
        {
            get { return fecha.ToString("dd/MM/yyyy"); }
        }

        public string TipoMovimientoDesc
        {
            get {
                string desc = "";
                switch (this.tipoMovimiento)
                {
                    case 1: desc = "Salida"; break;
                    case 2: desc = "Entrada"; break;
                    case 99: desc = "Toma de Inventario"; break;
                }

                return desc;
            }
        }
    }
    
}

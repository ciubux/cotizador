using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.UTILES
{
    public class ReporteMatriz
    {
        public string etiquetaFilas { get; set; }
        public List<ReporteMatrizCabecera> filas { get; set; }

        public string etiquetaColumnas { get; set; }
        public bool agrupaColumnas { get; set; }
        public bool tieneDescripcionColumnas { get; set; }

        public List<ReporteMatrizCabecera> columnas { get; set; }

        public List<ReporteMatrizDato> datos { get; set; }
        
        public bool concatenaValores { get; set; }

        public string concatenador { get; set; }
    }
}

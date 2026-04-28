using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ProductoControlStock : Auditoria
    {
        public Guid idProductoControlStock { get; set; } 
        public Empresa empresa { get; set; }
        public Producto producto { get; set; }
        public Ciudad ciudad { get; set; }

        public int cantidadMinima { get; set; }
        public int cantidadMaxima { get; set; }
        public int cantidadAlerta { get; set; }

        public DateTime fechaCs { get; set; }
        public int cantidadCs { get; set; }
        public string unidadCs { get; set; }

        public string FechaCsDesc
        {
            get { return fechaCs.ToString("dd/MM/yyyy HH:mm:ss"); }
        }
    }
}

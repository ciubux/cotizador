using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.UTILES
{
    public class RegistroCargaStock
    {

        public Producto producto { get; set; }

        public Ciudad ciudad { get; set; }

        public string sku { get; set; }

        public DateTime fecha { get; set; }

        public int cantidadProveedor { get; set; }
        public int cantidadMp { get; set; }
        public int cantidadAlternativa { get; set; }

        public decimal cantidadProveedorCalc { get; set; }
        public decimal cantidadMpCalc { get; set; }
        public decimal cantidadAlternativaCalc { get; set; }

        public bool registradoPeridoAplicable { get; set; }
        public bool tieneRegistroStock { get; set; }
        public bool stockNoDisponible { 
            get {
                return !(tieneRegistroStock || registradoPeridoAplicable);
            } 
        }


        public int cantidadConteo { get; set; }
    }
}

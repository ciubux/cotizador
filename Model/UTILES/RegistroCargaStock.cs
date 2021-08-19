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

        public int cantidadSeparadaConteo { get; set; }

        public decimal cantidadSeparadaProveedorCalc { get; set; }
        public decimal cantidadSeparadaMpCalc { get; set; }
        public decimal cantidadSeparadaAlternativaCalc { get; set; }

        public int cantidadTrasladosSumarConteo { get; set; }
        public decimal cantidadTrasladosSumarProveedorCalc { get; set; }
        public decimal cantidadTrasladosSumarMpCalc { get; set; }
        public decimal cantidadTrasladosSumarAlternativaCalc { get; set; }

        public int cantidadConTrasladosConteo { get { return cantidadConteo + cantidadTrasladosSumarConteo; } }
    }
}

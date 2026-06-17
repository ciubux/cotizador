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

        public int cantidadEsperadaConteo { get; set; }
        public decimal cantidadEsperadaProveedorCalc { get; set; }
        public decimal cantidadEsperadaMpCalc { get; set; }
        public decimal cantidadEsperadaAlternativaCalc { get; set; }

        public int cantidadTrasladosSumarConteo { get; set; }
        public decimal cantidadTrasladosSumarProveedorCalc { get; set; }
        public decimal cantidadTrasladosSumarMpCalc { get; set; }
        public decimal cantidadTrasladosSumarAlternativaCalc { get; set; }

        public int cantidadConTrasladosConteo { get { return cantidadConteo + cantidadTrasladosSumarConteo; } }

        public int cantidadReservadaEntregarConteo { get; set; }
        public decimal cantidadReservadaEntregarProveedorCalc { get; set; }
        public decimal cantidadReservadaEntregarMpCalc { get; set; }
        public decimal cantidadReservadaEntregarAlternativaCalc { get; set; }

        public int cantidadReservadaRecibirConteo { get; set; }
        public decimal cantidadReservadaRecibirProveedorCalc { get; set; }
        public decimal cantidadReservadaRecibirMpCalc { get; set; }
        public decimal cantidadReservadaRecibirAlternativaCalc { get; set; }

        public int cantidadDisponibleConteo { get { return cantidadConteo + cantidadReservadaRecibirConteo - cantidadReservadaEntregarConteo; } }
        public decimal cantidadDisponibleProveedorCalc { get { return cantidadProveedorCalc + cantidadReservadaRecibirProveedorCalc - cantidadReservadaEntregarProveedorCalc; } }
        public decimal cantidadDisponibleMPCalc { get { return cantidadMpCalc + cantidadReservadaRecibirMpCalc - cantidadReservadaEntregarMpCalc; } }
        public decimal cantidadDisponibleAlternativaCalc { get { return cantidadAlternativaCalc + cantidadReservadaRecibirAlternativaCalc - cantidadReservadaEntregarAlternativaCalc; } }
        
        public int estado { get; set; }
        
        public int diferenciaCantidadValidacion { get; set; }
        /* Mayor a  0: Stock Cargado es mayor al stock ZAS - EXCEDENTE
         * Menor a  0: Stock Cargado es menor al stock ZAS - FALTANTE
         */

        public int stockValidable { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class EscalaComision : Auditoria
    {
        public EscalaComision() 
        {
        }

        public int idEscalaComision { get; set; }
        [Display(Name = "Código:")]
        public String codigo { get; set; }
        [Display(Name = "Nombre:")]
        public String nombre { get; set; }

        public decimal margenDesde { get; set; }
        public String margenDesdeCondicion { get; set; }
        public decimal margenHasta { get; set; }
        public String margenHastaCondicion { get; set; }

        public static EscalaComision escalaPorMargen(decimal margen, List<EscalaComision> lista)
        {
            EscalaComision escala = new EscalaComision();
            bool validarDesde = false;
            bool validarHasta = false;

            foreach (EscalaComision item in lista)
            {
                validarDesde = false;
                validarHasta = false;

                if (item.margenDesdeCondicion.Equals("MAYOR") && item.margenDesde < margen)
                {
                    validarDesde = true;
                }

                if (item.margenDesdeCondicion.Equals("MAYORIGUAL") && item.margenDesde <= margen)
                {
                    validarDesde = true;
                }

                if (item.margenHastaCondicion.Equals("MENOR") && item.margenHasta > margen)
                {
                    validarHasta = true;
                }

                if (item.margenHastaCondicion.Equals("MENORIGUAL") && item.margenHasta >= margen)
                {
                    validarHasta = true;
                }

                if (validarDesde && validarHasta)
                {
                    escala = item;
                }
            }

            return escala;
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class Moneda : Auditoria
    {
        public Guid idMoneda { get; set; }

        public String codigo { get; set; }
        public String simbolo { get; set; }

        public String nombre { get; set; }

        public String caracter { get; set; }

        public static List<Moneda> ListaMonedas = new List<Moneda> {
            new Moneda { codigo = "PEN", simbolo = "S/", nombre = "SOL", caracter = "S"},
            new Moneda { codigo = "USD", simbolo = "US$", nombre = "DÓLAR", caracter = "D" }
        };

        public static List<Moneda> ListaMonedasFija = new List<Moneda> {
            new Moneda { codigo = "PEN", simbolo = "S/", nombre = "SOL", caracter = "S"},
            new Moneda { codigo = "USD", simbolo = "US$", nombre = "DÓLAR", caracter = "D" }
        };
    }
}
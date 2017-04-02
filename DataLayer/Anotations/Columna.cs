using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Anotations
{ 
    [System.AttributeUsage(System.AttributeTargets.Class |
                           System.AttributeTargets.Struct)
    ]
    public class Columna : System.Attribute
    {
        private string nombre;

        public Columna(string nombre)
        {
            this.nombre = nombre;
        }
    }
}
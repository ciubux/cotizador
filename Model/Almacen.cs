using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class Almacen : Auditoria
    {
        public Guid idAlmacen { get; set; }
        public Guid idCiudad { get; set; }
        public String codigo{ get; set; }
        public String nombre { get; set; }
        public String direccion { get; set; }
        public bool esPrincipal { get; set; }  

        public Ciudad ciudad { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class Usuario
    {
        public Guid idUsuario { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string nombre { get; set; }
        public string cargo { get; set; }
        public string contacto { get; set; }
        public bool esAprobador { get; set; }
        public Decimal maximoPorcentajeDescuentoAprobacion { get; set; }
        public List<Usuario> usuarioList { get; set; }
        public String cotizacionSerializada { get; set; }
    }
}

    
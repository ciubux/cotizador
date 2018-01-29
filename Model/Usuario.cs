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
        public string nombres { get; set; }
        public string apellidos { get; set; }
        public string password { get; set; }
        public string nombre_mostrar { get; set; }
        public string cargo { get; set; }
        public int anexo_empresa { get; set; }
        public int celular { get; set; }

        public bool esAprobador { get; set; }
    }
}

    
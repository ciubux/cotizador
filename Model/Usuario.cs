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
    }
}

    
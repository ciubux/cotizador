using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class Auditoria
    {
        public int Estado { get; set; }
        public DateTime FechaRegistro { get; set; }
        public Guid IdUsuarioRegistro { get; set; }
        public DateTime FechaEdicion { get; set; }
        public Guid IdUsuarioEdicion { get; set; }
        public Usuario UsuarioRegistro { get; set; }

        public string EstadoDesc => Estado == 1 ? "Activo" : "Inactivo";

        public bool EstadoCheck
        {
            get { return Estado == 1; }
            set
            {
                Estado = value ? 1 : 0;
            }
        }

        public string FechaRegistroDesc
        {
            get { return FechaRegistro.ToString("dd/MM/yyyy HH:mm:ss"); }
        }
    }
}

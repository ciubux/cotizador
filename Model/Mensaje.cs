using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Mensaje
    {
        public Guid id_mensaje { get; set; }

        public String usuario_creacion { get; set; }

        public DateTime fecha_creacion_mensaje { get; set; }

        public DateTime fechaVencimiento { get; set; }
        public String titulo { get; set; }

        public String mensaje { get; set; }

        public String importancia { get; set; }

        public int estado { get; set; }

        public Usuario user { get; set; }





        /*--------------ROL----------------------------*/

        public List<Rol> roles { get; set; }

        public int idRol { get; set; }

        public String nombreRol { get; set; }

        public String codigoRol { get; set; }



    }
}

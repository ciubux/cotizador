using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Mensaje
    {
        public Mensaje()
        {
            this.roles = new List<Rol>();
        }
        public Guid id_mensaje { get; set; }

        public String usuario_creacion { get; set; }
        [Display(Name = "Fecha de Creación:")]

        public DateTime fechaCreacionMensaje { get; set; }        
        public DateTime? fechaCreacionMensajeDesde { get; set; }
        public DateTime?  fechaCreacionMensajeHasta { get; set; }

        public DateTime? fechaInicioMensaje { get; set; }


        [Display(Name = "Fecha de Vencimiento:")]
        public DateTime? fechaVencimientoMensaje { get; set; }
        public DateTime? fechaVencimientoMensajeDesde { get; set; }
        public DateTime? fechaVencimientoMensajeHasta { get; set; }


        public DateTime fechaModificacionMensaje { get; set; }

        public String titulo { get; set; }

        public String mensaje { get; set; }

        public String importancia { get; set; }


        [Display(Name = "Estado:")]
        public int estado { get; set; }

        public Usuario user { get; set; }





        /*--------------ROL----------------------------*/

        public List<Rol> roles { get; set; }

        public List<Usuario> listUsuario { get; set; }
        public bool TieneRol(int idRol)
        {
            return this.roles.Where(item => item.idRol.Equals(idRol)).FirstOrDefault() != null;
        }
        /*
        public bool TieneUsuario(int idUsuario)
        {
            return this.listUsuario.Where(item => item.idUsuario.Equals(idUsuario)).FirstOrDefault() != null;
        }
        */

    }
}

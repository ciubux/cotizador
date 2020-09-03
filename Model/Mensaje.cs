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
        public DateTime? fechaCreacionMensajeHasta { get; set; }

        public DateTime? fechaInicioMensaje { get; set; }

        public DateTime fechaMensajeEntradaDesde { get; set; }
        public DateTime fechaMensajeEntradaHasta { get; set; }

        [Display(Name = "Fecha de Vencimiento:")]
        public DateTime? fechaVencimientoMensaje { get; set; }
        public DateTime? fechaVencimientoMensajeDesde { get; set; }
        public DateTime? fechaVencimientoMensajeHasta { get; set; }


        public DateTime fechaModificacionMensaje { get; set; }

        public String fechaVencimientoMensajeDesdeText { get {
                return fechaVencimientoMensajeDesde == null ? "" : fechaVencimientoMensajeDesde.Value.ToString();
            }
        }

        public String fechaVencimientoMensajeHastaText
        {
            get
            {
                return fechaVencimientoMensajeHasta == null ? "" : fechaVencimientoMensajeHasta.Value.ToString();
            }
        }

        public String fechaCreacionMensajeDesdeText
        {
            get
            {
                return fechaCreacionMensajeDesde == null ? "" : fechaCreacionMensajeDesde.Value.ToString();
            }
        }

        public String fechaCreacionMensajeHastaText
        {
            get
            {
                return fechaCreacionMensajeHasta == null ? "" : fechaCreacionMensajeHasta.Value.ToString();
            }
        }

        public String titulo { get; set; }


        public String mensaje { get; set; }

        public String importancia { get; set; }

        public String leido { get; set; }

        [Display(Name = "Estado:")]
        public int estado { get; set; }

        [Display(Name = "Bandeja:")]
        public int bandeja { get; set; }

        public Usuario user { get; set; }

        public Mensaje id_mensaje_hilo { get; set; }

        public List<Rol> roles { get; set; }

        public List<Usuario> listUsuario { get; set; }
        public bool TieneRol(int idRol)
        {
            return this.roles.Where(item => item.idRol.Equals(idRol)).FirstOrDefault() != null;
        }        

    }
}

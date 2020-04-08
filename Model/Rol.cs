using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class Rol : Auditoria
    {
        public Rol() 
        {
            this.permisos = new List<Permiso>();
            this.usuarios = new List<Usuario>();
            this.VistasDashboard = new List<VistaDashboard>();
        }

        public int idRol { get; set; }
        [Display(Name = "Código:")]
        public String codigo { get; set; }
        [Display(Name = "Nombre:")]
        public String nombre { get; set; }

        public List<Permiso> permisos { get; set; }


        public List<Usuario> usuarios { get; set; }
        public override string ToString()
        {
            return "Rol: " + this.nombre + " - Cod: " + this.codigo;
        }


        public bool TienePermiso(int idPermiso)
        {
            return this.permisos.Where(item => item.idPermiso.Equals(idPermiso)).FirstOrDefault() != null;
        }

        public List<VistaDashboard> VistasDashboard { get; set; }
        public bool TieneVistaDashboard(int idVistaDashboard)
        {
            return this.VistasDashboard.Where(item => item.idVistaDashboard.Equals(idVistaDashboard)).FirstOrDefault() != null;
        }

    }
}
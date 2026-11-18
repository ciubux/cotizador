using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class PortafolioProductos : Auditoria
    {
        public Guid idPortafolioProductos { get; set; }

        [Display(Name = "Nombre:")]
        public string nombre { get; set; }

        [Display(Name = "Comentarios:")]
        public string comentarios { get; set; }

        public List<PortafolioProductosItem> items { get; set; }
        public List<Usuario> usuarios { get; set; }
        public List<Rol> roles { get; set; }

        public PortafolioProductos()
        {
            this.items = new List<PortafolioProductosItem>();
            this.usuarios = new List<Usuario>();
            this.roles = new List<Rol>();
        }
    }
}
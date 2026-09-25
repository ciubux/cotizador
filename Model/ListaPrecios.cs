using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class ListaPrecios : Auditoria
    {
        public ListaPrecios() 
        {
            this.empresa = new Empresa();
            this.items = new List<ListaPreciosItem>();
        }

        public Guid idListaPrecios { get; set; }

        public Empresa empresa { get; set; }

        [Display(Name = "Nombre:")]
        public string nombre { get; set; }

        [Display(Name = "Comentarios:")]
        public string comentarios { get; set; }

        public List<ListaPreciosItem> items { get; set; }

    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class ListaPreciosItem : Auditoria
    {
        public Guid idListaPreciosItem { get; set; }

        public Guid idListaPrecios { get; set; }

        public Producto producto { get; set; }

        [Display(Name = "Unidad:")]
        public string unidad { get; set; }

        [Display(Name = "Presentación:")]
        public int idProductoPresentacion { get; set; }

        [Display(Name = "Precio:")]
        public decimal precio { get; set; }

        public ListaPreciosItem()
        {
            this.producto = new Producto();
        }

    }
}
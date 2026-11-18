using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class PortafolioProductosItem : Auditoria
    {
        public Guid idPortafolioProductosItem { get; set; }


        public Producto producto { get; set; }

        public PortafolioProductosItem()
        {
            this.producto = new Producto();
        }
    }
}
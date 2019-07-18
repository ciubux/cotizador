using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cotizador.Models
{
    public class ProductoMultipleViewModels
    {
        public List<string> ProductoSelectIds { get; set; }
        public string SelectedValue { get; set; }

        public List<Producto> Data { get; set; }

        public IEnumerable<SelectListItem> Productos
        {
            get
            {
                return Data.Select(c => new SelectListItem
                {
                    Value = c.idProducto.ToString(),
                    Text = c.sku + "-" + c.descripcion,
                    Selected = SelectedValue != null && SelectedValue == c.idProducto.ToString()
                });
            }
        }

        public Boolean Disabled { get; set; }
    }
}

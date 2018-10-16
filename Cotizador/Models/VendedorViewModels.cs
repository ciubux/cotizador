using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cotizador.Models
{
    public class VendedorViewModels
    {
        public string VendedorSelectId { get; set; }
        public string SelectedValue { get; set; }

        public List<Vendedor> Data { get; set; }

        public IEnumerable<SelectListItem> Vendedores
        {
            get
            {
                return Data.Select(c => new SelectListItem
                {
                    Value = c.idVendedor.ToString(),
                    Text = c.codigoDescripcion,
                    Selected = SelectedValue != null && SelectedValue == c.idVendedor.ToString()
                });
            }
        }

        public Boolean Disabled { get; set; }
    }
}

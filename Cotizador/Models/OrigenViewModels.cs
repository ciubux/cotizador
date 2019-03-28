using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cotizador.Models
{
    public class OrigenViewModels
    {
        public string OrigenSelectId { get; set; }
        public string SelectedValue { get; set; }

        public List<Origen> Data { get; set; }

        public IEnumerable<SelectListItem> Origenes
        {
            get
            {
                return Data.Select(c => new SelectListItem
                {
                    Value = c.idOrigen.ToString(),
                    Text = c.codigo + " - " + c.nombre,
                    Selected = SelectedValue != null && SelectedValue == c.idOrigen.ToString()
                });
            }
        }

        public Boolean Disabled { get; set; }
    }
}

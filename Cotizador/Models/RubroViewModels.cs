using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cotizador.Models
{
    public class RubroViewModels
    {
        public string RubroSelectId { get; set; }
        public string SelectedValue { get; set; }

        public List<Rubro> Data { get; set; }

        public IEnumerable<SelectListItem> Rubros
        {
            get
            {
                return Data.Select(c => new SelectListItem
                {
                    Value = c.idRubro.ToString(),
                    Text = c.nombre,
                    Selected = SelectedValue != null && SelectedValue == c.idRubro.ToString()
                });
            }
        }
    }
}

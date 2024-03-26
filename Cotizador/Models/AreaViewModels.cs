using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cotizador.Models
{
    public class AreaViewModels
    {
        public string AreaSelectId { get; set; }
        public string SelectedValue { get; set; }

        public bool incluirSeleccione { get; set; }
        public List<Area> Data { get; set; }

        public IEnumerable<SelectListItem> Areas
        {
            get
            {
                return Data.Select(a => new SelectListItem
                {
                    Value = a.idArea.ToString(),
                    Text = a.nombre,
                    Selected = SelectedValue != null && SelectedValue == a.idArea.ToString()
                });
            }
        }
    }
}

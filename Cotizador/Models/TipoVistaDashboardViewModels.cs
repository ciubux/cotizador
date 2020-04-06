using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cotizador.Models
{
    public class TipoVistaDashboardViewModels
    {
        public string TipoVistaDashboardSelectId { get; set; }
        public string SelectedValue { get; set; }

        public List<TipoVistaDashboard> Data { get; set; }

        public IEnumerable<SelectListItem> TiposVistaDashboard
        {
            get
            {
                return Data.Select(c => new SelectListItem
                {
                    Value = c.idTipoVistaDashboard.ToString(),
                    Text = c.idTipoVistaDashboard + " - " + c.nombre,
                    Selected = SelectedValue != null && SelectedValue == c.idTipoVistaDashboard.ToString()
                });
            }
        }
    }
}
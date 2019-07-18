using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cotizador.Models
{
    public class SubDistribuidorViewModels
    {
        public string SubDistribuidorSelectId { get; set; }
        public string SelectedValue { get; set; }

        public List<SubDistribuidor> Data { get; set; }

        public IEnumerable<SelectListItem> SubDistribuidores
        {
            get
            {
                return Data.Select(c => new SelectListItem
                {
                    Value = c.idSubDistribuidor.ToString(),
                    Text = c.codigo + " - " + c.nombre,
                    Selected = SelectedValue != null && SelectedValue == c.idSubDistribuidor.ToString()
                });
            }
        }

        public Boolean Disabled { get; set; }
    }
}

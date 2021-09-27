using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cotizador.Models
{
    public class PromocionViewModels
    {
        public string SelectId { get; set; }
        public string SelectedValue { get; set; }
        public List<Promocion> Data { get; set; }

        public IEnumerable<SelectListItem> promociones
        {
            get
            {
                return Data.Select(c => new SelectListItem
                {
                    Value = c.idPromocion.ToString(),
                    Text = c.codigo + " - " + c.titulo,
                    Selected = SelectedValue != null && SelectedValue == c.idPromocion.ToString()
                });
            }
        }
    }
}

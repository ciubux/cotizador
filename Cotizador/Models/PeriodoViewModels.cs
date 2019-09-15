using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cotizador.Models
{
    public class PeriodoViewModels
    {
        public string PeriodoSelectId { get; set; }
        public string SelectedValue { get; set; }

        public List<PeriodoSolicitud> Data { get; set; }

        public IEnumerable<SelectListItem> Periodos
        {
            get
            {
                return Data.Select(c => new SelectListItem
                {
                    Value = c.idPeriodoSolicitud.ToString(),
                    Text = c.nombre,
                    Selected = SelectedValue != null && SelectedValue == c.idPeriodoSolicitud.ToString()
                });
            }
        }
    }
}

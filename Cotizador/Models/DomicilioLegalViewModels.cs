using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cotizador.Models
{
    public class DomicilioLegalViewModels
    {
        public string DomicilioLegalSelectId { get; set; }
        public string SelectedValue { get; set; }

        public List<DomicilioLegal> Data { get; set; }

        public IEnumerable<SelectListItem> DomiciliosLegales
        {
            get
            {
                return Data.Select(c => new SelectListItem
                {
                    Value = c.idDomicilioLegal.ToString(),
                    Text = c.direccion + " " + c.ubigeo.Departamento + " - " + c.ubigeo.Provincia + " - " + c.ubigeo.Distrito,
                    Selected = SelectedValue != null && SelectedValue == c.idDomicilioLegal.ToString()
                });
            }
        }
    }
}

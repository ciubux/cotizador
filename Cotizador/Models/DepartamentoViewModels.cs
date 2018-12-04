using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cotizador.Models
{
    public class DepartamentoViewModels
    {
        public string DepartamentoSelectId { get; set; }
        public string ProvinciaSelectId { get; set; }
        public string DistritoSelectId { get; set; }
        public string ProvinciaDivId { get; set; }
        public string DistritoDivId { get; set; }
        public string UrlProvinciasPorDepartamento { get; set; }
        public string UrlDistritosPorProvincia { get; set; }
        public string SelectedValue { get; set; }

        public List<Ubigeo> Data { get; set; }

        public IEnumerable<SelectListItem> Departamentos
        {
            get
            {
                return Data.Select(d => new SelectListItem
                {
                    Value = d.Id,
                    Text = d.Descripcion,
                    Selected = SelectedValue != null && SelectedValue == d.Id
                });
            }
        }
    }
}

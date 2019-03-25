using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cotizador.Models
{
    public class DistritoViewModels
    {
        public string DistritoSelectId { get; set; }
        public string SelectedValue { get; set; }

        public List<Ubigeo> Data { get; set; }
        public Boolean Disabled { get; set; }

        public IEnumerable<SelectListItem> Distritos
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
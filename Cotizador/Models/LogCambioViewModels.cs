using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cotizador.Models
{
    public class LogCambioViewModels
    {
        public string CatalogoSelectId { get; set; }
        public string SelectedValue { get; set; }

        public List<LogCambio> Data { get; set; }

        public IEnumerable<SelectListItem> Catalogo
        {
            get
            {
                return Data.Select(c => new SelectListItem
                {
                    Value = c.catalogoId.ToString(),
                    Text = c.catalogoId + " - " + c.nombre,
                    Selected = SelectedValue != null && SelectedValue == c.catalogoId.ToString()
                });
            }
        }
    }
}
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cotizador.Models
{
    public class LogCampoViewModels
    {
        public string CatalogoSelectId { get; set; }
        public string SelectedValue { get; set; }

        public List<LogCampo> Data { get; set; }

        public IEnumerable<SelectListItem> Catalogo
        {
            get
            {
                return Data.Select(c => new SelectListItem
                {
                    Value = c.tablaId.ToString(),
                    Text = c.tablaId + " - " + c.nombreTabla,
                    Selected = SelectedValue != null && SelectedValue == c.tablaId.ToString()
                });
            }
        }
    }
}
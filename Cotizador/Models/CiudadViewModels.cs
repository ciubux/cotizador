using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cotizador.Models
{
    public class CiudadViewModels
    {
        public string CiudadSelectId { get; set; }
        public string SelectedValue { get; set; }

        public bool incluirSeleccione { get; set; }
        public List<Ciudad> Data { get; set; }

        public IEnumerable<SelectListItem> Ciudades
        {
            get
            {
                return Data.Select(c => new SelectListItem
                {
                    Value = c.idCiudad.ToString(),
                    Text = c.nombre + " (" + c.sede + ")",
                    Selected = SelectedValue != null && SelectedValue == c.idCiudad.ToString()
                });
            }
        }
    }
}

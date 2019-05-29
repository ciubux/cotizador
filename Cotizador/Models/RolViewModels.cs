using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cotizador.Models
{
    public class RolViewModels
    {
        public string RolSelectId { get; set; }
        public string SelectedValue { get; set; }

        public List<Rol> Data { get; set; }

        public IEnumerable<SelectListItem> Roles
        {
            get
            {
                return Data.Select(c => new SelectListItem
                {
                    Value = c.idRol.ToString(),
                    Text = c.codigo + " - " + c.nombre,
                    Selected = SelectedValue != null && SelectedValue == c.idRol.ToString()
                });
            }
        }

        public Boolean Disabled { get; set; }
    }
}

using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cotizador.Models
{
    public class GrupoClienteViewModels
    {
        public string GrupoClienteSelectId { get; set; }
        public string SelectedValue { get; set; }

        public List<GrupoCliente> Data { get; set; }

        public IEnumerable<SelectListItem> GruposCliente
        {
            get
            {
                return Data.Select(c => new SelectListItem
                {
                    Value = c.idGrupoCliente.ToString(),
                    Text = c.codigo + " - " + c.nombre,
                    Selected = SelectedValue != null && SelectedValue == c.idGrupoCliente.ToString()
                });
            }
        }
    }
}

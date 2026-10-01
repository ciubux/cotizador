using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cotizador.Models
{
    public class ListaPreciosViewModels
    {
        public string SelectId { get; set; }
        public string SelectedValue { get; set; }

        public bool incluirSeleccione { get; set; }
        public string textoSeleccione { get; set; }
        public List<ListaPrecios> Data { get; set; }

        public string disabled { get; set; }
        public IEnumerable<SelectListItem> Listas
        {
            get
            {
                return Data.Select(c => new SelectListItem
                {
                    Value = c.idListaPrecios.ToString(),
                    Text = c.nombre,
                    Selected = SelectedValue != null && SelectedValue == c.idListaPrecios.ToString()
                });
            }
        }
    }
}

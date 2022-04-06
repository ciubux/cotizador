using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cotizador.Models
{
    public class MotivoAjusteAlmacenViewModels
    {
        public string MotivoAjusteAlmacenSelectId { get; set; }
        public string SelectedValue { get; set; }

        public bool incluirSeleccione { get; set; }
        public List<MotivoAjusteAlmacen> Data { get; set; }

        public IEnumerable<SelectListItem> Motivos
        {
            get
            {
                return Data.Select(c => new SelectListItem
                {
                    Value = c.idMotivoAjusteAlmacen.ToString(),
                    Text = c.descripcion,
                    Selected = SelectedValue != null && SelectedValue == c.idMotivoAjusteAlmacen.ToString()
                });
            }
        }
    }
}

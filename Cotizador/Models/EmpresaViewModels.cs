using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cotizador.Models
{
    public class EmpresaViewModels
    {
        public string EmpresaSelectId { get; set; }
        public string SelectedValue { get; set; }

        public bool incluirSeleccione { get; set; }
        public List<Empresa> Data { get; set; }

        public IEnumerable<SelectListItem> Empresas
        {
            get
            {
                return Data.Select(e => new SelectListItem
                {
                    Value = e.idEmpresa.ToString(),
                    Text = e.nombre,
                    Selected = SelectedValue != null && SelectedValue == e.idEmpresa.ToString()
                });
            }
        }
    }
}

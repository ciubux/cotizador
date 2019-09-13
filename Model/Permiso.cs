using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Permiso : Auditoria
    {
        public int idPermiso { get; set; }

        [Display(Name = "Código:")]
        public String codigo { get; set; }

        [Display(Name = "Descripción Corta:")]
        public String descripcion_corta { get; set; }

        [Display(Name = "Descripción Completa:")]
        public String descripcion_larga { get; set; }

        [Display(Name = "Categoría:")]
        public CategoriaPermiso categoriaPermiso { get; set; }
        public List<Usuario> usuarioList { get; set; }
        public bool byRol { get; set; }
        public bool byUser { get; set; }
    }
}

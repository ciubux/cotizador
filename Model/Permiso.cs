using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Permiso : Auditoria
    {
        public int idPermiso { get; set; }
        public String codigo { get; set; }
        public String descripcion_corta { get; set; }
        public String descripcion_larga { get; set; }
        public CategoriaPermiso categoriaPermiso { get; set; }
        public List<Usuario> usuarioList { get; set; }

        public bool byRol { get; set; }
        public bool byUser { get; set; }
    }
}

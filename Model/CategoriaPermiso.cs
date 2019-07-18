using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class CategoriaPermiso : Auditoria
    {
        public int idCategoriaPermiso { get; set; }
        public String descripcion { get; set; }
        public List<Permiso> permisoList { get; set; }
    }
}

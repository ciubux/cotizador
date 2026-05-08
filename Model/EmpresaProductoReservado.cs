using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class EmpresaProductoReservado : Auditoria
    {
        public Guid idEmpresaProductoReservado { get; set; }

        public string unidadConteo { get; set; }

        public int cantidadReservada { get; set; }
        public int cantidadAtendida { get; set; }

        public Empresa empresa { get; set; }

        public Producto producto { get; set; }

    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class EmpresaProductoSeparado : Auditoria
    {
        public Guid idEmpresaProductoSeparado{ get; set; }

        public string unidadConteo { get; set; }

        public int cantidadSeparada { get; set; }

        public decimal cantidadSeparadaAlternativa { get { return (producto == null ? 0 : ((decimal)cantidadSeparada) / ((decimal)producto.equivalenciaUnidadAlternativaUnidadConteo)); } }
        public decimal cantidadSeparadaMp { get { return (producto == null ? 0 : ((decimal)cantidadSeparada) / ((decimal)producto.equivalenciaUnidadEstandarUnidadConteo)); } }
        public decimal cantidadSeparadaProveedor { get { return (producto == null ? 0 : ((decimal)cantidadSeparada) / ((decimal)producto.equivalenciaUnidadProveedorUnidadConteo)); } }
   
        public Empresa empresa { get; set; }

        public Producto producto { get; set; }

    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class EmpresaDescuento : Auditoria
    {
        public const string TIPO_DESCUENTO_PORCENTAJE = "PORCENTAJE";
        public const string TIPO_DESCUENTO_MONTO = "MONTO";
        public Guid idEmpresadescuento { get; set; }

        [Display(Name = "Tipo Descuento:")]
        public String tipoDescuento { get; set;  }
        
        [Display(Name = "Descuento:")]
        public decimal descuento { get; set; }

        public Empresa empresa { get; set; }
        public Ciudad ciudad { get; set; }

        public Producto producto { get; set; }

        public string descuentoDesc { get {
                string desc = descuento.ToString();
                switch(tipoDescuento)
                {
                    case TIPO_DESCUENTO_PORCENTAJE: desc = desc + " %"; break;
                    case TIPO_DESCUENTO_MONTO: desc = "S/ " + desc;  break;
                } 

                return desc;
            } 
        }
    }
}
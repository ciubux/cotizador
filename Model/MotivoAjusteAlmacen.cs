using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class MotivoAjusteAlmacen : Auditoria
    {
        public int idMotivoAjusteAlmacen { get; set; }

        [Display(Name = "Descripcion:")]
        public String descripcion { get; set; }


        [Display(Name = "Tipo:")]
        public int tipo { get; set; }

        public String tipoDesc { get {
                String desc = "";
                switch(this.tipo)
                {
                    case 1: desc = "Pérdida"; break;
                    case 2: desc = "Sobrante"; break;
                }

                return desc;
            } 
        }

        public override string ToString()
        {
            return this.descripcion;
        }
    }
}
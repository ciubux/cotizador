using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Ubigeo : Auditoria
    {
        public string Id { get; set; }

        public string Descripcion { get; set; }

        //  [Required(ErrorMessage = "Departamento es requerido")]
        [Display(Name = "Departamento:")]
        public string Departamento { get; set; }

        //  [Required(ErrorMessage = "Provincia es requerido")]
        [Display(Name = "Provincia:")]
        public string Provincia { get; set; }

        //  [Required(ErrorMessage = "Distrito es requerido")]
        [Display(Name = "Distrito:")]
        public string Distrito { get; set; }


        public new string ToString
        {
            get
            {
                String deptartamento = Departamento == null ? "" : Departamento;
                String provincia = Provincia == null ? "" : Provincia;
                String distrito = Distrito == null ? "" : Distrito;

                return deptartamento + " - " + provincia + " - " + distrito;
            }
        }

    }
}

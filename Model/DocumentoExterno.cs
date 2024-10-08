using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class DocumentoExterno : Auditoria
    {
        public const string TIPO_FACTURA_RELACIONADA = "FACTURA_RELACIONADA";
        public Guid idDocumentoExterno { get; set; }

        public Guid idRegistro { get; set; }

        [Display(Name = "Tipo:")]
        public String tipo { get; set;  }
        [Display(Name = "Nombre:")]
        public String nombre { get; set; }

        [Display(Name = "Serie:")]
        public String serie { get; set; }

        [Display(Name = "Correlativo:")]
        public String correlativo { get; set; }

        
        public String SerieCorrelativo()
        {
            return this.serie+ "-" + this.correlativo;
        }
    }
}
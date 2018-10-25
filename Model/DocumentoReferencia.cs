using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class DocumentoReferencia : Auditoria
    {

        public Guid idDocumentoReferenciaVenta { get; set; }

        [Display(Name = "Tipo Documento:")]
        public DocumentoVenta.TipoDocumento tipoDocumento { get; set; }

        [Display(Name = "Serie:")]
        public String serie { get; set; }

        [Display(Name = "Número:")]
        public String numero { get; set; }

        [Display(Name = "Número Documento:")]
        public String serieNumero { get {
                return serie + "-" + numero;
            } }

        [Display(Name = "Fecha Emisión:")]
        public DateTime fechaEmision { get; set; }


        [Display(Name = "Fecha Emisión:")]
        public String fechaEmisionFormat { get {
                return this.fechaEmision.ToString(Constantes.formatoFecha);
            } }

        [Display(Name = "Número Doc. Ref.:")]
        public DateTime numeroOtroDocumentoReferencia { get; set; }


    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Model.UTILES;

namespace Model
{
    public class CierreStock : Auditoria
    {
        public Guid idCierreStock { get; set; }

        public Ciudad ciudad { get; set; }

        public Usuario UsuarioReporteValidacion { get; set; }
        public DateTime fecha { get; set; }
        public ArchivoAdjunto archivo{ get; set; }

        public List<RegistroCargaStock> detalles { get; set; }

        public DateTime fechaReporteValidacion { get; set; }

        public String fechaDesc { get { return fechaReporteValidacion.ToString("dd/MM/yyyy"); } }

        public String fechaReporteValidacionDesc { get { return fechaReporteValidacion.ToString("dd/MM/yyyy HH:mm:ss"); } }
        public MovimientoAlmacen ajusteFaltante { get; set; }
        public MovimientoAlmacen ajusteExcedente { get; set; }
    }
}
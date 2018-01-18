using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class Cotizacion
    {
        public Guid idCotizacion { get; set; }
        public Int64 codigo { get; set; }
        public DateTime fecha { get; set; }
        public Boolean incluidoIgv { get; set; }
        public short mostrarCodProveedor { get; set; }
        public Decimal flete { get; set; }
        public String usuarioCreacion { get; set; }
        public Usuario usuario { get; set; }
        public Decimal igv { get; set; }
        public Moneda moneda { get; set; }
        public Ciudad ciudad { get; set; }
        public Cliente cliente { get; set; }
        public Boolean considerarCantidades { get; set; }
        public Decimal montoSubTotal { get; set; }
        public Decimal montoIGV { get; set; }
        public Decimal montoTotal { get; set; }
        public String observaciones { get; set; }
        public String contacto { get; set; }
        public List<CotizacionDetalle> cotizacionDetalleList { get; set; }

    }
}

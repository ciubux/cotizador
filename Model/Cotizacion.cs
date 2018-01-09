using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class Cotizacion
    {
        public Guid idCotizacion { get; set; }
        public DateTime fecha { get; set; }
        public Guid idCiudad { get; set; }
        public Guid idCliente { get; set; }
        public Guid idMoneda { get; set; }
        public Guid idTipoCambio { get; set; }
        public Guid idPrecio { get; set; }
        public short incluidoIgv { get; set; }
        public short mostrarCodProveedor { get; set; }
        public Decimal flete { get; set; }
        public String usuarioCreacion { get; set; }
        public Usuario usuario { get; set; }
        public Decimal subtotal { get; set; }
        public Decimal igv { get; set; }
        public Decimal total { get; set; }
        public Moneda moneda { get; set; }
        public Ciudad ciudad { get; set; }
        public Cliente cliente { get; set; }
        public List<CotizacionDetalle> detalles { get; set; }
        public Boolean considerarCantidades { get; set; }
        public Decimal montoSubTotal { get; set; }
        public Decimal montoIGV { get; set; }
        public Decimal montoTotal { get; set; }
        public Decimal montoFlete { get; set; }
        public Decimal montoTotalMasFlete { get; set; }
        public String observaciones { get; set; }

    }
}

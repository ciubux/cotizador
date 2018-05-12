using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class PedidoDetalle : DocumentoDetalle
    {
        public Guid idPedidoDetalle { get; set; }
        public Guid idPedido { get; set; }

        public new Decimal precioNeto
        {
            get
            {
                if (esPrecioAlternativo)
                    return Decimal.Parse(String.Format(Constantes.formatoCuatroDecimales, _precioNeto / producto.equivalencia));
                else
                    return _precioNeto;
            }

            set
            {
                this._precioNeto = value;
            }
        }

        public Decimal precioUnitarioVenta { get; set; }

        public Guid idVentaDetalle { get; set; }

        public new Decimal precioUnitario
        {
            get { return Decimal.Parse(String.Format(Constantes.formatoCuatroDecimales, flete + precioNeto)); }
        }

        public new Decimal subTotal
        {
            get { return Decimal.Parse(String.Format(Constantes.formatoDosDecimales, this.cantidad * this.precioUnitario)); }
        }
    }
}

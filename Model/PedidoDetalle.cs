using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class PedidoDetalle : DocumentoDetalle
    {
        public PedidoDetalle(Boolean visualizaCostos, Boolean visualizaMargen) 
        {
            this.visualizaCostos = visualizaCostos;
            this.visualizaMargen = visualizaMargen;
        }

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


        public Decimal precioNetoAlterno
        {
            set
            {
                Decimal.Parse(String.Format(Constantes.formatoCuatroDecimales, this._precioNeto = value * producto.equivalencia));
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

        public Decimal precioUnitarioOriginal
        {
            get;set;
        }

        public Boolean visualizaCostos { get; set; }

        public Decimal costoListaVisible
        {
            get
            {
                if (this.visualizaCostos)
                    return this.costoLista;
                else
                    return 0M;
            }
        }

        public Boolean visualizaMargen { get; set; }

        public Decimal margen
        {

            get
            {

                if (this.visualizaMargen)// && !this.usuario.esCliente)
                    return Decimal.Parse(String.Format(Constantes.formatoUnDecimal, (1 - costoLista / (precioNeto == 0 ? 1 : precioNeto)) * 100));
                else
                    return 0.0M;
            }
        }

    }
}

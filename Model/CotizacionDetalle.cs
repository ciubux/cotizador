using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class CotizacionDetalle : DocumentoDetalle
    {
        public CotizacionDetalle(Boolean visualizaCostos, Boolean visualizaMargen)
        {
            this.visualizaCostos = visualizaCostos;
            this.visualizaMargen = visualizaMargen;
        }

        public Guid idCotizacionDetalle { get; set; }
        public Guid idCotizacion { get; set; }

        private Decimal _precioAnterior;
        public Decimal precioNetoAnterior
        {
            get { return Decimal.Parse(String.Format(Constantes.formatoDosDecimales, _precioAnterior)); }
            set
            {
                _precioAnterior = value;
            }
        }


        //recupera la variacion del precio con respecto al precioAnterior para los calculos de recotizacion
        private Decimal _variacionPrecioAnterior;
        public Decimal variacionPrecioAnterior
        {

            get
            {


                if (precioNetoAnterior != 0)
                    _variacionPrecioAnterior = (this.precioNeto / this.precioNetoAnterior - 1) * 100;
                else
                    _variacionPrecioAnterior = 0;

                return Decimal.Parse(String.Format(Constantes.formatoUnDecimal, _variacionPrecioAnterior));

            }
        }
        public Decimal costoAnterior { get; set; }

        //recupera la variacion del costo con respecto al costoAnterior para los calculos de recotizacion
        private Decimal _variacionCosto;
        public Decimal variacionCosto
        {
            get
            {

                if (precioNetoAnterior != 0)
                    _variacionCosto = (this.costoLista / this.costoAnterior - 1) * 100;
                else
                    _variacionCosto = 0;

                return Decimal.Parse(String.Format(Constantes.formatoUnDecimal, _variacionCosto));
            }

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

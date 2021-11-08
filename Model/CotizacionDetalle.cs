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
            this.validar = true;
          //  this.ProductoPresentacion = new ProductoPresentacion();
        }

        public Guid idCotizacionDetalle { get; set; }
        public Guid idCotizacion { get; set; }

        private Decimal _precioAnterior;
        public Decimal precioNetoAnterior
        {
            get { return Decimal.Parse(String.Format(Constantes.formatoDecimalesPrecioNeto, _precioAnterior)); }
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



        private Decimal _variacionPrecioListaAnterior;
        public Decimal variacionPrecioListaAnterior
        {

            get
            {


                if (precioListaAnterior != 0)
                    _variacionPrecioListaAnterior = (this.precioLista / this.precioListaAnterior - 1) * 100;
                else
                    _variacionPrecioListaAnterior = 0;

                return Decimal.Parse(String.Format(Constantes.formatoUnDecimal, _variacionPrecioListaAnterior));

            }
        }



        //public Decimal costoAnterior { get; set; }

        //recupera la variacion del costo con respecto al costoAnterior para los calculos de recotizacion
        private Decimal _variacionCosto;
        public Decimal variacionCosto
        {
            get
            {

                if (costoListaAnterior != 0)
                    _variacionCosto = (this.costoLista / this.costoListaAnterior - 1) * 100;
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

        public Decimal costoListaFleteVisible
        {
            get
            {
                if (this.visualizaCostos)
                    return this.costoListaFlete;
                else
                    return 0M;
            }
        }

        public Decimal costoEspecialVisible
        {
            get
            {
                if (this.visualizaCostos)
                    return this.costoEspecialMostrar;
                else
                    return 0M;
            }
        }

        public Decimal costoEspecialFleteVisible
        {
            get
            {
                if (this.visualizaCostos)
                    return this.costoEspecialFlete;
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
                    if (tieneCostoEspecial)
                    {
                        return Decimal.Parse(String.Format(Constantes.formatoUnDecimal, (1 - costoEspecial / (precioNeto == 0 ? 1 : precioNeto)) * 100));
                    }
                    else
                    {
                        return Decimal.Parse(String.Format(Constantes.formatoUnDecimal, (1 - costoLista / (precioNeto == 0 ? 1 : precioNeto)) * 100));
                    }
                else
                    return 0.0M;
            }
        }

        public Decimal margenCostoFlete
        {
            get
            {
                if (this.visualizaMargen)// && !this.usuario.esCliente)
                    if (tieneCostoEspecial)
                    {
                        return Decimal.Parse(String.Format(Constantes.formatoUnDecimal, (1 - (costoEspecial + (producto.costoFleteProvincias / producto.equivalenciaProveedor)) / (precioNeto == 0 ? 1 : precioNeto)) * 100));
                    } else
                    {
                        return Decimal.Parse(String.Format(Constantes.formatoUnDecimal, (1 - (costoLista + (producto.costoFleteProvincias / producto.equivalenciaProveedor)) / (precioNeto == 0 ? 1 : precioNeto)) * 100));
                    }
                else
                    return 0.0M;
            }
        }

    }
}

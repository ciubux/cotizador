using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class CotizacionDetalle
    {
        public Guid idCotizacionDetalle { get; set; }
        public Guid idCotizacion { get; set; }
        public int cantidad { get; set; }
       
        private Decimal _porcentajeDescuento;
        public Decimal porcentajeDescuento
        {
            get
            {
                return Decimal.Parse(String.Format(Constantes.cuatroDecimalFormat, _porcentajeDescuento));
            }
            set { _porcentajeDescuento = value; }
        }



        public String unidad { get; set; }
        public Decimal subTotal { get
            {
                return this.cantidad * this.precioNeto;
            } }      
        public Producto producto { get; set; }

        //Es el precio definido por el usuario
        private Decimal _precioNeto;
        public Decimal precioNeto {
            get { if (esPrecioAlternativo)
                    return Decimal.Parse(String.Format(Constantes.decimalFormat, _precioNeto / producto.equivalencia));
                else
                    return _precioNeto;
            }
            set { this._precioNeto = value; }
        }

        //Se obtiene del producto
        public Decimal precioLista
        {
            get
            {
                if (esPrecioAlternativo)
                    return Decimal.Parse(String.Format(Constantes.decimalFormat, producto.precioLista / producto.equivalencia));
                else
                    return producto.precioLista;
            }
            
        }

        //Se obtienes desde el producto
        public Decimal costoLista
        {
            get
            {
                if (esPrecioAlternativo)
                    return Decimal.Parse(String.Format(Constantes.decimalFormat, producto.costoLista / producto.equivalencia));
                else
                    return producto.costoLista;
            }

        }

        //permite identificar si el precio es el alternativo y poder hacer las divisiones donde corresponda
        public Boolean esPrecioAlternativo { get; set; }
    

        public Usuario usuario { get; set; }

        //Obtiene el margen
        public Decimal margen {

            get {
                return 1 - Decimal.Parse(String.Format(Constantes.unDecimalFormat, costoLista / precioNeto));

            } }

        //Obtiene y define el precioAnterior para los calculos de recotizacion
        private Decimal _precioAnterior;
        public Decimal precioNetoAnterior {
            get { return Decimal.Parse(String.Format(Constantes.decimalFormat, _precioAnterior)); }
            set
            {
                _precioAnterior = value;
            }
        }


        //recupera la variacion del precio con respecto al precioAnterior para los calculos de recotizacion
        private Decimal _variacionPrecioAnterior;
        public Decimal variacionPrecioAnterior {

            get {


                if (precioNetoAnterior != 0)
                    _variacionPrecioAnterior = this.precioNeto / this.precioNetoAnterior - 1;
                else
                    _variacionPrecioAnterior = 0;

                return Decimal.Parse(String.Format(Constantes.unDecimalFormat, _variacionPrecioAnterior));

            } 
             }
        public Decimal costoAnterior { get; set; }

        //recupera la variacion del costo con respecto al costoAnterior para los calculos de recotizacion
        private Decimal _variacionCosto;
        public Decimal variacionCosto
        {
            get {

                if (precioNetoAnterior != 0)
                    _variacionCosto = this.costoLista / this.costoAnterior - 1;
                else
                    _variacionCosto = 0;

                return Decimal.Parse(String.Format(Constantes.unDecimalFormat, _variacionCosto)); }
            
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class DocumentoDetalle 
    {
      
        public int cantidad { get; set; }
        public Producto producto { get; set; }
        public String unidad { get; set; }


        public Boolean esPrecioAlternativo { get; set; }
        public Usuario usuario { get; set; }
        public String observacion { get; set; }
        public Decimal subTotal
        {
            get {  return this.cantidad * this.precioUnitario;  }
        }




        //permite identificar si el precio es el alternativo y poder hacer las divisiones donde corresponda
        private Decimal _porcentajeDescuento;
        //Recupera el porcentajeDescuento con la mayor cantidad de decimales definidos
        public Decimal porcentajeDescuento
        {
            get
            {
                return Decimal.Parse(String.Format(Constantes.formatoCuatroDecimales, _porcentajeDescuento));
            }
            set { _porcentajeDescuento = value; }
        }

        //Recupera el porcentajeDescuento con la cantidad de decimales que se desea ver en la interfaz
        public Decimal porcentajeDescuentoMostrar
        {
            get
            {
                return Decimal.Parse(String.Format(Constantes.formatoUnDecimal, _porcentajeDescuento));
            }
        }
                  
      

        //Es el precio definido por el usuario
    //    private Decimal _precioNeto;

        public Decimal precioNeto { get; set; }

        public Decimal precioNetoEquivalente {
            get { if (esPrecioAlternativo)
                    return Decimal.Parse(String.Format(Constantes.formatoDosDecimales, precioNeto / producto.equivalencia));
                else
                    return precioNeto;
            }
        }

        //Se obtiene del producto
        public Decimal precioLista
        {
            get
            {
                Decimal precioListaTmp = 0;
                if (esPrecioAlternativo)
                    precioListaTmp = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, producto.precioLista / producto.equivalencia));
                else
                    precioListaTmp = producto.precioLista;

                return precioListaTmp;
                
            }
            
        }

        //Se obtienes desde el producto
        public Decimal costoLista
        {
            get
            {
                Decimal costoListaTmp = 0;
                if (esPrecioAlternativo)
                    costoListaTmp = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, producto.costoLista / producto.equivalencia));
                else
                    costoListaTmp = producto.costoLista;

                return costoListaTmp;

            }

        }

        //Obtiene el margen
        public Decimal margen {

            get {
                return Decimal.Parse(String.Format(Constantes.formatoUnDecimal, (1 - costoLista / precioNetoEquivalente)*100));

            } }

        //Obtiene y define el precioAnterior para los calculos de recotizacion
        private Decimal _precioAnterior;
        public Decimal precioNetoAnterior {
            get { return Decimal.Parse(String.Format(Constantes.formatoDosDecimales, _precioAnterior)); }
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
                    _variacionPrecioAnterior = (this.precioNetoEquivalente / this.precioNetoAnterior - 1)*100;
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
            get {

                if (precioNetoAnterior != 0)
                    _variacionCosto = (this.costoLista / this.costoAnterior - 1)*100;
                else
                    _variacionCosto = 0;

                return Decimal.Parse(String.Format(Constantes.formatoUnDecimal, _variacionCosto)); }
            
        }

        private Decimal _flete;
        public Decimal flete
        {
            get { return Decimal.Parse(String.Format(Constantes.formatoDosDecimales, _flete)); }
            set { this._flete = value; }
        }

        public Decimal precioUnitario
        {
            get { return Decimal.Parse(String.Format(Constantes.formatoDosDecimales, flete + precioNetoEquivalente)); } 
        }

        

    }
}

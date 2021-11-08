using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
            indicadorAprobacion = IndicadorAprobacion.Aprobado;
            //this.ProductoPresentacion = new ProductoPresentacion();
        }

        public Guid idPedidoDetalle { get; set; }
        public Guid idPedido { get; set; }

        public new Decimal precioNeto
        {
            get
            {
                if (esPrecioAlternativo)
                    return Decimal.Parse(String.Format(Constantes.formatoCuatroDecimales, _precioNeto / ProductoPresentacion.Equivalencia));
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
                Decimal.Parse(String.Format(Constantes.formatoCuatroDecimales, this._precioNeto = value * ProductoPresentacion.Equivalencia));
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
            get {
                if (this._reverseSubTotal <= 0)
                {
                    return Decimal.Parse(String.Format(Constantes.formatoDosDecimales, this.cantidad * this.precioUnitario));
                } else
                {
                    return Decimal.Parse(String.Format(Constantes.formatoDosDecimales, this.reverseSubTotal));
                }
            }
        }

        public Decimal precioUnitarioOriginal
        {
            get; set;
        }

        public int cantidadOriginal
        {
            get; set;
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

        //public Decimal margen
        //{

        //    get
        //    {

        //        if (this.visualizaMargen)// && !this.usuario.esCliente)
        //            return Decimal.Parse(String.Format(Constantes.formatoUnDecimal, (1 - costoLista / (precioNeto == 0 ? 1 : precioNeto)) * 100));
        //        else
        //            return 0.0M;
        //    }
        //}

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
                        return Decimal.Parse(String.Format(Constantes.formatoUnDecimal, (1 - (costoEspecial + (producto.costoFleteProvincias / (producto.equivalenciaProveedor == 0 ? 1 : producto.equivalenciaProveedor))) / (precioNeto == 0 ? 1 : precioNeto)) * 100));
                    }
                    else
                    {
                        return Decimal.Parse(String.Format(Constantes.formatoUnDecimal, (1 - (costoLista + (producto.costoFleteProvincias / (producto.equivalenciaProveedor == 0 ? 1 : producto.equivalenciaProveedor))) / (precioNeto == 0 ? 1 : precioNeto)) * 100));
                    }
                else
                    return 0.0M;
            }
        }

        public IndicadorAprobacion indicadorAprobacion { get; set; }

        public enum IndicadorAprobacion
        {
            [Display(Name = "Aprobado")]
            Aprobado = 1,
            [Display(Name = "Pendiente Aprobación sin Vigencia")]
            RechazadoSinVigencia = 2,
            [Display(Name = "Pendiente Aprobación sin Precio")]
            RechazadoSinPrecio = 3
        }


    }
}

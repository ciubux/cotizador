using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class DocumentoDetalle 
    {

      
        public Guid idDocumentoDetalle { get; set; }
        
        public int cantidad { get; set; }

        public decimal cantidadDecimal { get; set; }

        public int cantidadPorAtender { get; set; }

        public int cantidadPendienteAtencion { get; set; }

        public int cantidadSolicitada { get { return cantidad; } }

        public Producto producto { get; set; }
        public String unidad { get; set; }

        public String unidadInternacional { get; set; }


        public Boolean esPrecioAlternativo { get; set; }
        //public Usuario usuario { get; set; }
        public String observacion { get; set; }
        public Decimal subTotal
        {
            get {  return this.cantidad * precioUnitario;  }
        }




        //permite identificar si el precio es el alternativo y poder hacer las divisiones donde corresponda
        private Decimal _porcentajeDescuento;
        //Recupera el porcentajeDescuento con la mayor cantidad de decimales definidos
        public Decimal porcentajeDescuento
        {
            get
            {
                if(precioLista == precioNeto)
                    return Decimal.Parse(String.Format(Constantes.formatoOchoDecimales, 0));
                else
                    return Decimal.Parse(String.Format(Constantes.formatoOchoDecimales, _porcentajeDescuento));
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
        protected Decimal _precioNeto;

     //   public Decimal precioNeto { get; set; }


        public Decimal precioNeto
        {
            get { if (esPrecioAlternativo)
                    return Decimal.Parse(String.Format(Constantes.formatoDecimalesPrecioNeto, _precioNeto / producto.equivalencia));
                else
                    return _precioNeto;
            }

            set {
                this._precioNeto = value;
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


        public Decimal precioListaAnterior
        {
            get
            {
                Decimal precioListaTmp = 0;
                if (esPrecioAlternativo)
                    precioListaTmp = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, producto.precioListaAnterior / producto.equivalencia));
                else
                    precioListaTmp = producto.precioListaAnterior;

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

       
        //Obtiene y define el precioAnterior para los calculos de recotizacion
       

        private Decimal _flete;
        public Decimal flete
        {
            get { return Decimal.Parse(String.Format(Constantes.formatoDecimalesPrecioNeto, _flete)); }
            set { this._flete = value; }
        }

        public Decimal precioUnitario
        {
            get { return Decimal.Parse(String.Format(Constantes.formatoDecimalesPrecioNeto, flete + precioNeto)); } 
        }
        
        public bool validar { get; set; }



        public PrecioClienteProducto precioCliente;


        private Decimal _porcentajeMargen;
        //Recupera el porcentajeDescuento con la mayor cantidad de decimales definidos
        public Decimal porcentajeMargen
        {
            get
            {
                return Decimal.Parse(String.Format(Constantes.formatoOchoDecimales, _porcentajeMargen));
            }
            set { _porcentajeMargen = value; }
        }

        public Decimal porcentajeMargenMostrar
        {
            get
            {
                return Decimal.Parse(String.Format(Constantes.formatoUnDecimal, _porcentajeMargen));
            }
        }

    }
}

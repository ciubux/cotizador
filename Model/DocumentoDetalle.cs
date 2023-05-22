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

        public int cantidadTotalAtencion { get; set; }
        public decimal cantidadDecimal { get; set; }

        
        public int cantidadPorAtender { get; set; }
        public int cantidadIngresada { get; set; }
        public int cantidadGuiada { get; set; }

        //public bool tienePrecioEspecial { get; set; }
        //public decimal precioEspecial { get; set; }

        public bool tieneCostoEspecial { get; set; }
        public decimal costoEspecial { get; set; }
        public int cantidadPorAtenderPermitida
        {
            get
            {
                int valor = cantidadPendienteAtencionPermitida - cantidadPorAtender;
                if (valor < 0) cantidadPorAtender = cantidadPendienteAtencionPermitida;
                
                return cantidadPorAtender;
            }
        }

        public int cantidadPendienteAtencionPermitida
        {
            get {
                int valor = cantidadPermitida - (cantidad - cantidadPendienteAtencion);
                if (valor < 0) valor = 0;
                return valor;
            }
        }

        public int cantidadPendienteAtencion { get; set; }

        public int cantidadPermitida { get; set; }

        public string observacionRestriccion { get; set; }

        public int cantidadSolicitada { get { return cantidad; } }

        public Boolean excluirVenta { get; set; }
        public int estadoVenta { get; set; }

        public Producto producto { get; set; }
        public String unidad { get; set; }

        public String unidadInternacional { get; set; }


        public Boolean esPrecioAlternativo { get; set; }
        //public Usuario usuario { get; set; }
        public String observacion { get; set; }
        public Decimal subTotal
        {
            get { return this.cantidad * precioUnitario; }
        }

        protected Decimal _reverseSubTotal;
        public Decimal reverseSubTotal
        { 
            get {
                return this._reverseSubTotal;
            }
            set {
                _reverseSubTotal = value;
            }
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

        public bool cumpleTopeDescuentoProducto
        {
            get
            {
                return this.producto.topeDescuento <= 0 || this.producto.topeDescuento > this.porcentajeDescuento;
            }
        }
        
        //Recupera el porcentajeDescuento con la cantidad de decimales que se desea ver en la interfaz
        public Decimal porcentajeDescuentoMostrar
        {
            get
            {
                return Decimal.Parse(String.Format(Constantes.formatoUnDecimal, this.porcentajeDescuento));
            }
        }
                  
      

        //Es el precio definido por el usuario
        protected Decimal _precioNeto;

     //   public Decimal precioNeto { get; set; }


        public Decimal precioNeto
        {
            get { if (esPrecioAlternativo)
                {
                    return Decimal.Parse(String.Format(Constantes.formatoDecimalesPrecioNeto, _precioNeto / ProductoPresentacion.Equivalencia));
                }
                else
                {
                    return _precioNeto;
                }
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
                {
                    precioListaTmp = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, producto.precioLista / ProductoPresentacion.Equivalencia));
                }
                else
                {
                    precioListaTmp = producto.precioLista;
                }

                return precioListaTmp;
                
            }
            
        }


        public Decimal precioListaAnterior
        {
            get
            {
                Decimal precioListaTmp = 0;
                if (esPrecioAlternativo)
                    precioListaTmp = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, producto.precioListaAnterior / ProductoPresentacion.Equivalencia));
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
                    costoListaTmp = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, producto.costoLista / ProductoPresentacion.Equivalencia));
                else
                    costoListaTmp = producto.costoLista;
                
                return costoListaTmp;
            }
        }

        public Decimal costoEspecialMostrar
        {
            get
            {
                Decimal costoTmp = 0;

                if (tieneCostoEspecial)
                {
                    if (esPrecioAlternativo)
                        costoTmp = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, this.costoEspecial / ProductoPresentacion.Equivalencia));
                    else
                        costoTmp = this.costoEspecial;
                }

                return costoTmp;
            }
        }

        public Decimal costoListaFlete
        {
            get
            {
                Decimal costoTmp = 0;
                if (producto.equivalenciaProveedor == 0)
                {
                    return 0;
                }

                if (esPrecioAlternativo)
                    costoTmp = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, (producto.costoLista / ProductoPresentacion.Equivalencia) + (producto.costoFleteProvincias / (producto.equivalenciaProveedor * ProductoPresentacion.Equivalencia))));
                else
                    costoTmp = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, producto.costoLista + (producto.costoFleteProvincias / producto.equivalenciaProveedor)));
                
                return costoTmp;
            }
        }

        public Decimal costoEspecialFlete
        {
            get
            {
                if (producto.equivalenciaProveedor == 0)
                {
                    return 0;
                }

                Decimal costoTmp = 0;
                if (esPrecioAlternativo)
                    costoTmp = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, (this.costoEspecial / ProductoPresentacion.Equivalencia) + (producto.costoFleteProvincias / (producto.equivalenciaProveedor * ProductoPresentacion.Equivalencia))));
                else
                    costoTmp = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, this.costoEspecial + (producto.costoFleteProvincias / producto.equivalenciaProveedor)));

                return costoTmp;
            }
        }

        public Decimal costoListaOriginal
        {
            get
            {
                Decimal costoListaTmp = 0;
                int factorEqProv = producto.equivalenciaProveedor <= 0 ? 1 : producto.equivalenciaProveedor;
                if (esPrecioAlternativo)
                    costoListaTmp = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, (producto.costoOriginal/ factorEqProv) / ProductoPresentacion.Equivalencia));
                else
                    costoListaTmp = (producto.costoOriginal/factorEqProv);

                return costoListaTmp;
            }
        }

        public Decimal costoListaAnterior
        {
            get
            {
                Decimal costoListaTmp = 0;
                if (esPrecioAlternativo)
                    costoListaTmp = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, producto.costoListaAnterior / ProductoPresentacion.Equivalencia));
                else
                    costoListaTmp = producto.costoListaAnterior;

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

       public ProductoPresentacion ProductoPresentacion { get; set; }

        public int idProductoPresentacion
        {
            get
            {
                return ProductoPresentacion == null ? 0 : ProductoPresentacion.IdProductoPresentacion;
            }
        }

        public bool tieneInfraMargenEmpresaExterna { get; set; }
    }


}

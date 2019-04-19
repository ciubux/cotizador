using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class Producto : Auditoria
    {
        public Producto()
        {
            this.tipoProducto = TipoProducto.Bien;
            this.ProductoPresentacionList = new List<ProductoPresentacion>();
        }


        public Guid idProducto { get; set; }

        [Display(Name = "Código (SKU):")]
        public String sku { get; set; }
        [Display(Name = "SKU Prov.:")]
        public String skuProveedor { get; set; }
        [Display(Name = "Descripción:")]
        public String descripcion { get; set; }
        public Byte[] image { get; set; }
        [Display(Name = "Proveedor:")]
        public String proveedor { get; set; }
        [Display(Name = "Familia:")]
        public String familia { get; set; }

        ///<summary>
        ///Identifica si el producto se encuentra exonerado de IGV
        ///</summary>
        ///<return>
        ///Retorna true si el producto se encuentra exonerado de IGV, false en caso contrario
        ///</return>
        [Display(Name = "Exonerado IGV:")]
        public bool exoneradoIgv { get; set; }

        ///<summary>
        ///Identifica si el producto se encuentra Inafecto de IGV
        ///</summary>
        ///<return>
        ///Retorna true si el producto se encuentra inafecto de IGV, false en caso contrario
        ///</return>
        [Display(Name = "Inafecto:")]
        public bool inafecto { get; set; }


        ///<summary>
        ///Unidad de Sunat correspondiente a la unidad de venta estándar
        ///</summary>
        ///<return>
        ///Retorna unidad de Sunat correspondiente a la unidad de venta estándar
        ///</return>
        [Display(Name = "Unidad SUNAT:")]
        public String unidadEstandarInternacional { get; set; }


        public Decimal precioLista { get; set; }
        public Decimal precioListaAnterior { get; set; }


        [Display(Name = "Unidad de Venta:")]
        ///<summary>
        ///Unidad estándar de venta
        ///</summary>
        ///<return>
        ///Retorna la unidad estándar de venta
        ///</return>
        public String unidad { get; set; }

        private String _unidad_alternativa;
        [Display(Name = "Unidad Alternativa:")]
        public String unidad_alternativa
        {
            //        get { return equivalencia == 1?String.Empty:_unidad_alternativa; }
            get { return _unidad_alternativa; }
            set { _unidad_alternativa = value; }
        }

        [Display(Name = "Unidad Proveedor:")]
        public String unidadProveedor { get; set; }


        /// <summary>
        /// El campo equivalencia puede contener la equivalencia con respecto a la unidad de proveedor o alternativa
        /// En caso que la unidad se la estandar entonces la equivalencia debe ser 1
        /// </summary>
   //     public decimal equivalencia { get; set; }


        // private int _equivalencia;
        [Display(Name = "Equivalencia Und. Alt.:")]
        public int equivalenciaAlternativa { get; set; }

        [Display(Name = "Equivalencia Und. Prov.:")]
        public int equivalenciaProveedor { get; set; }



        [Display(Name = "Precio Lima:")]
        public Decimal precioSinIgv { get; set; }


        [Display(Name = "Precio Provincias:")]
        public Decimal precioProvinciaSinIgv { get; set; }

        /*     [Display(Name = "Precio Alternativo:")]
             public Decimal precioAlternativoSinIgv
             {
                 get
                 {
                     return Decimal.Parse(String.Format(Constantes.formatoDecimalesPrecioNeto, precioSinIgv / (equivalencia==0?1:equivalencia )));
                 }
             }*/

        public List<PrecioClienteProducto> precioListaList { get; set; }




        [Display(Name = "Costo:")]
        ///<summary>
        ///Costo sin IGV del producto en la unidad estándar
        ///</summary>
        ///<return>
        ///Retorna el costo del producto en la unidad estándar
        ///</return>
        public Decimal costoSinIgv { get; set; }

        ///<summary>
        ///Costo sin IGV del producto calculado en la unidad alternativa, no permite setear el valor.
        ///</summary>
        ///<return>
        ///Retorna el costo del producto calculado en la unidad alternativa en formato de dos decimales.
        ///</return>
     /*   public Decimal costoAlternativoSinIgv
        {
            get
            {
                return Decimal.Parse(String.Format(Constantes.formatoDosDecimales, costoSinIgv / (equivalencia == 0 ? 1 : equivalencia)));
            }
        }*/

        ///<summary>
        ///Costo del producto en la unidad estándar calculado con IGV o sin IGV dependiendo de la opción seleccionada, se utiliza en DocumentoDetalle
        ///</summary>
        ///<return>
        ///Retorna el costo del producto en la unidad estándar calculado con IGV o sin IGV dependiendo de la opción seleccionada
        ///</return>
        public Decimal costoLista { get; set; }

        public Decimal costoListaAnterior { get; set; }


        public override string ToString()
        {
            return this.sku.Trim() + " - " + this.descripcion;
        }

        public PrecioClienteProducto precioClienteProducto { get; set; }


        public int tipoProductoVista { get; set; }

        [Display(Name = "Tipo:")]
        public TipoProducto tipoProducto { get; set; }
        public enum TipoProducto
        {
            [Display(Name = "Producto")]
            Bien = 1,
            [Display(Name = "Equipo de Comodato")]
            Comodato = 2,
            [Display(Name = "Servicio")]
            Servicio = 3,
            [Display(Name = "Cargo")]
            Cargo = 4,
            [Display(Name = "Descuento")]
            Descuento = 5
        }

        public String tipoProductoToString
        {
            get
            {
                return EnumHelper<TipoProducto>.GetDisplayValue(this.tipoProducto);
            }
        }

        public List<ProductoPresentacion> ProductoPresentacionList { get; set; }

        public ProductoPresentacion getProductoPresentacion(int idProductoPresentacion) {
            if (ProductoPresentacionList != null)
            {
                return ProductoPresentacionList.Where(p => p.IdProductoPresentacion == idProductoPresentacion).FirstOrDefault();
            }
            return null;
        }
        public String UnidadConteo { get; set; }
        public int EquivalenciaUnidadAlternativaUnidadConteo { get; set; }
        public int EquivalenciaUnidadStandarUnidadConteo { get; set; }
        public int EquivalenciaUnidadProveedorUnidadConteo { get; set; }
        public int Stock { get; set; }
    }
}
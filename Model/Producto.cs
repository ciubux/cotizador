using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class Producto : Auditoria
    {
        public Guid idProducto { get; set; }
        public Guid idFamilia { get; set; }
        public Guid idProveedor { get; set; }
        public Guid idUnidad { get; set; }
        [Display(Name = "Código:")]
        public String sku { get; set; }
        [Display(Name = "Código Proveedor:")]
        public String skuProveedor { get; set; }
        [Display(Name = "Descripción:")]
        public String descripcion { get; set; }
        public Byte[] image { get; set; }
        [Display(Name = "Unidad:")]
        public String unidad { get; set; }
        [Display(Name = "Proveedor:")]
        public String proveedor { get; set; }
        [Display(Name = "Familia:")]
        public String familia { get; set; }
        public String clase { get; set; }
        public String marca { get; set; }

        [Display(Name = "Exonerado IGV:")]
        public bool exoneradoIgv { get; set; }

        [Display(Name = "Inafecto:")]
        public bool inafecto { get; set; }

        [Display(Name = "Unidad Estandar Internacional:")]
        public String unidadEstandarInternacional { get; set; }

        public Decimal precioLista { get; set; }

        [Display(Name = "Precio Provincia:")]
        public Decimal precioProvinciaSinIgv { get; set; }
       

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
        // private int _equivalencia;
        [Display(Name = "Equivalencia:")]
        public int equivalencia { get; set; }

        [Display(Name = "Equivalencia Proveedor:")]
        public int equivalenciaProveedor { get; set; }

        // private Decimal _precioSinIgv;

        [Display(Name = "Precio:")]
        public Decimal precioSinIgv {      get;set;    }

        [Display(Name = "Precio Alternativo:")]
        public Decimal precioAlternativoSinIgv
        {
            get
            {
                return Decimal.Parse(String.Format(Constantes.formatoCuatroDecimales, precioSinIgv / (equivalencia==0?1:equivalencia )));
            }
        }

        public List<PrecioClienteProducto> precioListaList { get; set; }

        [Display(Name = "Costo:")]
        public Decimal costoSinIgv { get;set; }

        public Decimal costoAlternativoSinIgv
        {
            get
            {
                return Decimal.Parse(String.Format(Constantes.formatoDosDecimales, costoSinIgv / (equivalencia == 0 ? 1 : equivalencia)));
            }
        }

        public Decimal costoLista { get; set; }
        public override string ToString()
        {
            return this.sku.Trim() + " - " + this.descripcion;
        }

        public PrecioClienteProducto precioClienteProducto { get; set; }

        public Usuario usuario { get; set; }
    }
}
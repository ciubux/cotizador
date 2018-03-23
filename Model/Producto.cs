using System;
using System.Collections.Generic;
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
        public String sku { get; set; }
        public String skuProveedor { get; set; }
        public String descripcion { get; set; }
        public Byte[] image { get; set; }
        public String unidad { get; set; }
        public String proveedor { get; set; }
        public Decimal precioLista { get; set; }
        public Decimal precioProvinciaSinIgv { get; set; }
       // public Decimal valor { get; set; }
        public String familia { get; set; }
        public String clase { get; set; }
        public String marca { get; set; }

        private String _unidad_alternativa;
        public String unidad_alternativa
        {
            get { return equivalencia==1?String.Empty:_unidad_alternativa; }
            set { _unidad_alternativa = value; }
        }
        public int equivalencia { get; set; }

        public Decimal precioSinIgv { get; set; }

        public Decimal precioAlternativoSinIgv {
            get {
                return  Decimal.Parse(String.Format(Constantes.formatoDosDecimales, precioSinIgv / equivalencia));
            }
        }


        public List<PrecioClienteProducto> precioListaList { get; set; }

        public Decimal? precioNeto { get; set; }
        public Decimal costoSinIgv { get; set; }
        public Decimal costoAlternativoSinIgv {
            get
            {
                return Decimal.Parse(String.Format(Constantes.formatoDosDecimales, costoSinIgv / equivalencia));
            }
        }



        public Decimal costoLista { get; set; }
        public override string ToString()
        {
            return this.sku.Trim() + " - " + this.descripcion;
        }
    }
}
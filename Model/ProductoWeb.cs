using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class ProductoWeb : Auditoria
    {
        public ProductoWeb() 
        {
            
        }

        public Guid idProductoWeb { get; set; }
        public Producto producto { get; set; }
        
        public string sku { get; set; }
        public string nombre { get; set; }
        public string descripcionCorta { get; set; }
        public string descripcionCatalogo { get; set; }
        public string categoria { get; set; }
        public string subCategoria { get; set; }
        public int itemOrder{ get; set; }
        public string atributoTitulo1 { get; set; }
        public string atributoValor1 { get; set; }
        public string atributoTitulo2 { get; set; }
        public string atributoValor2 { get; set; }
        public string atributoTitulo3 { get; set; }
        public string atributoValor3 { get; set; }
        public string atributoTitulo4 { get; set; }
        public string atributoValor4 { get; set; }
        public string atributoTitulo5 { get; set; }
        public string atributoValor5 { get; set; }

        public string tagBusqueda { get; set; }
        public string tagPromociones { get; set; }
        public string seoTitulo { get; set; }
        public string seoPalabrasClave { get; set; }
        public string seoDescripcion { get; set; }
        public int cuotaWeb { get; set; }

        public decimal precio { get; set; }
        public decimal precioProvincia { get; set; }

        public List<String> codigoSedes { get; set; }
        public List<int> stocks { get; set; }
        public ProductoPresentacion presentacion { get; set; }
    }
}

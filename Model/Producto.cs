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
        public String descripcion { get; set; }
        public Byte[] image { get; set; }
        public String unidad { get; set; }
        public String proveedor { get; set; }
        public Decimal precio { get; set; }
        public Decimal precio_provincia { get; set; }
        public Decimal valor { get; set; }
        public String familia { get; set; }
        public String clase { get; set; }
        public String marca { get; set; }
        public String unidad_alternativa { get; set; }
        public Decimal equivalencia { get; set; }

        public override string ToString()
        {           
            return this.sku.Trim() + " - " + this.descripcion;
        }
    }
}
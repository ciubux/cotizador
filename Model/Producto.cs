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
        public Unidad unidad { get; set; }
    }
}
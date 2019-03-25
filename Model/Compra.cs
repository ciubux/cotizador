using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace Model
{
    public class Compra : Transaccion
    {
        public int idCompra { get; set; }
        public MovimientoAlmacen notaIngreso { get; set; }
        public DocumentoCompra documentoCompra { get; set; }
    }
}


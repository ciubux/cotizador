using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class ArchivoAdjunto 
    {
        public Guid idArchivoAdjunto { get; set; }
        public Usuario usuario { get; set; }
        public Byte[] adjunto { get; set; }
        [Display(Name = "Nombre:")]
        public String nombre { get; set; }
        public long checksum { get; set; }
        
        public DateTime fechaCreacion { get; set; }

        public int estado { get; set; }
        public List<ArchivoAdjunto> listaArchivosAdjuntos { get; set; }
        public enum origenes
        {            
            [Display(Name = "Factura")]
            Factura = 1,
            [Display(Name = "Pedido")]
            Pedido = 2,
            [Display(Name = "Guia de Remisión")]
            GuiaRemision = 3,
            [Display(Name = "Nota de Ingreso")]
            NotaIngreso= 4
        }


        [Display(Name = "Tipo Pedido:")]
        public origenesBusqueda origenBusqueda { get; set; }
        public enum origenesBusqueda
        {
            [Display(Name = "Seleccione una opción")]
            opcion = 0,
            [Display(Name = "Factura")]
            Factura = origenes.Factura,
            [Display(Name = "Pedido")]
            Pedido = origenes.Pedido,
            [Display(Name = "Guia de Remisión")]
            GuiaRemision = origenes.GuiaRemision,
            [Display(Name = "Nota de Ingreso")]
            NotaIngreso = origenes.NotaIngreso

        }

        public Guid idRegistro { get; set; }

        public String origen { get; set; }

        public String metaData { get; set; }

        public Guid idCliente { get; set; }

        public enum ArchivoAdjuntoOrigen
        {
            PRODUCTO=1,
            FACTURA=2,
            PEDIDO=3,
            GUIA_REMISION=4
        }
        
    }
}

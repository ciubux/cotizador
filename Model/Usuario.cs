using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class Usuario
    {
        public Guid idUsuario { get; set; }

        /*DATOS*/
        public string email { get; set; }
        public string password { get; set; }
        public string nombre { get; set; }
        public string cargo { get; set; }
        public string contacto { get; set; }


        /*ROLES COTIZACION*/
        public bool apruebaCotizaciones { get; set; }
        public bool creaCotizaciones { get; set; }
        public Decimal maximoPorcentajeDescuentoAprobacion { get; set; }

        /*ROLES PEDIDO*/
        public bool tomaPedidos { get; set; }
        public bool apruebaPedidos { get; set; }

        /*ROLES GUIA REMISION*/
        public bool creaGuias { get; set; }
        public bool administraGuiasLima { get; set; }
        public bool administraGuiasProvincia { get; set; }

        /*ROLES FACTURA ELECTRONICA*/
        public bool creaDocumentosVenta { get; set; }     
        public bool administraDocumentosVentaLima { get; set; }
        public bool administraDocumentosVentaProvincia { get; set; }




        public Ciudad sedeMP { get; set; }

        public List<Ciudad> sedesMP { get; set; }
        public List<Ciudad> sedesMPGuiasRemision { get; set; }

        public List<Ciudad> sedesMPDocumentosVenta { get; set; }

        public List<Usuario> usuarioCreaCotizacionList { get; set; }
        public List<Usuario> usuarioTomaPedidoList { get; set; }
        public String cotizacionSerializada { get; set; }
    }
}

    
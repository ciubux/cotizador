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
        public bool apruebaCotizaciones { get { return apruebaCotizacionesLima || apruebaCotizacionesProvincias;  } }
        public bool apruebaCotizacionesLima { get; set; }
        public bool apruebaCotizacionesProvincias { get; set; }
        public bool creaCotizaciones { get; set; }
        public Decimal maximoPorcentajeDescuentoAprobacion { get; set; }

        /*ROLES PEDIDO*/
        public bool tomaPedidos { get; set; }
        public bool apruebaPedidos { get { return apruebaPedidosLima || apruebaPedidosProvincias; } }
        public bool apruebaPedidosLima { get; set; }
        public bool apruebaPedidosProvincias { get; set; }

        /*ROLES GUIA REMISION*/
        public bool creaGuias { get; set; }

        public bool administraGuias { get { return administraGuiasLima || administraGuiasProvincias; } }
        public bool administraGuiasLima { get; set; }
        public bool administraGuiasProvincias { get; set; }

        /*ROLES FACTURA ELECTRONICA*/
        public bool creaDocumentosVenta { get; set; }

        public bool administraDocumentosVenta { get { return administraDocumentosVentaLima || administraDocumentosVentaProvincias; } }

        public bool administraDocumentosVentaLima { get; set; }
        public bool administraDocumentosVentaProvincias { get; set; }




        public Ciudad sedeMP { get; set; }

        public List<Ciudad> sedesMP { get; set; }
        public List<Ciudad> sedesMPGuiasRemision { get; set; }

        public List<Ciudad> sedesMPDocumentosVenta { get; set; }

        public List<Ciudad> sedesMPCotizaciones { get; set; }

        public List<Ciudad> sedesMPPedidos { get; set; }





        public List<Usuario> usuarioCreaCotizacionList { get; set; }
        public List<Usuario> usuarioTomaPedidoList { get; set; }

        public List<Usuario> usuarioCreaGuiaList { get; set; }
        public List<Usuario> usuarioCreaDocumentoVentaList { get; set; }
        public String cotizacionSerializada { get; set; }
    }
}

    
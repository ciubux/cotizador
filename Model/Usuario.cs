using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class Usuario
    {
        public Usuario()
        {
            this.vendedorList = new List<Vendedor>();
        }


        public Guid idUsuario { get; set; }

        /*DATOS*/
        public string email { get; set; }
        public string password { get; set; }

        [Display(Name = "Nombre:")]
        public string nombre { get; set; }
        public string cargo { get; set; }
        public string contacto { get; set; }






        /*PERMISOS COTIZACION*/
        public bool apruebaCotizaciones { get { return apruebaCotizacionesLima || apruebaCotizacionesProvincias; } }
        public bool apruebaCotizacionesLima { get; set; }
        public bool apruebaCotizacionesProvincias { get; set; }
        public bool creaCotizaciones { get; set; }
        public Decimal maximoPorcentajeDescuentoAprobacion { get; set; }
        public bool visualizaCotizaciones { get; set; }

        /*PERMISOS PEDIDO*/
        public bool tomaPedidos { get; set; }
        public bool apruebaPedidos { get { return apruebaPedidosLima || apruebaPedidosProvincias; } }
        public bool apruebaPedidosLima { get; set; }
        public bool apruebaPedidosProvincias { get; set; }
        public bool visualizaPedidosLima { get; set; }

        public bool visualizaPedidosProvincias { get; set; }

        public bool visualizaCostos { get; set; }

        public bool visualizaPedidos { get { return visualizaPedidosLima || visualizaPedidosProvincias; } }

        public bool bloqueaPedidos { get; set; }
        public bool liberaPedidos { get; set; }



        /*PERMISOS GUIA REMISION*/
        public bool creaGuias { get; set; }
        public bool administraGuias { get { return administraGuiasLima || administraGuiasProvincias; } }
        public bool administraGuiasLima { get; set; }
        public bool administraGuiasProvincias { get; set; }
        public bool visualizaGuias { get; set; }

        /*PERMISOS FACTURA ELECTRONICA*/
        public bool creaDocumentosVenta { get; set; }

        public bool administraDocumentosVenta { get { return administraDocumentosVentaLima || administraDocumentosVentaProvincias; } }

        public bool apruebaAnulaciones { get; set; }

        public bool administraDocumentosVentaLima { get; set; }
        public bool administraDocumentosVentaProvincias { get; set; }
        public bool visualizaDocumentosVenta { get; set; }


        public Ciudad sedeMP { get; set; }

        public List<Ciudad> sedesMP { get; set; }
        public List<Ciudad> sedesMPGuiasRemision { get; set; }

        public List<Ciudad> sedesMPDocumentosVenta { get; set; }

        public List<Ciudad> sedesMPCotizaciones { get; set; }

        public List<Ciudad> sedesMPPedidos { get; set; }


        public List<Cliente> clienteList { get; set; }


        public List<Usuario> usuarioCreaCotizacionList { get; set; }
        public List<Usuario> usuarioTomaPedidoList { get; set; }

        public List<Usuario> usuarioCreaGuiaList { get; set; }
        public List<Usuario> usuarioCreaDocumentoVentaList { get; set; }
        public String cotizacionSerializada { get; set; }
        public String pedidoSerializado { get; set; }


        public bool creaCotizacionesProvincias { get; set; }
        public bool tomaPedidosProvincias { get; set; }
        public bool programaPedidos { get; set; }
        public bool modificaMaestroClientes { get; set; }
        public bool modificaMaestroProductos { get; set; }
        public bool creaNotasCredito { get; set; }
        public bool creaNotasDebito { get; set; }
        public bool realizaRefacturacion { get; set; }
        public bool esCliente { get; set; }
        public bool visualizaMargen { get; set; }
        public bool confirmaStock { get; set; }
        public bool tomaPedidosCompra { get; set; }
        public bool tomaPedidosAlmacen { get; set; }
        public bool apruebaPlazoCredito { get; set; }
        public bool apruebaMontoCredito { get; set; }
        public bool defineResponsableComercial { get; set; }
        public bool defineSupervisorComercial { get; set; }
        public bool defiineAsistenteAtencionCliente { get; set; }
        public bool defineResponsablePortafolio { get; set; }
        public bool modificaPedidoVentaFechaEntregaHasta { get; set; }


        public List<Vendedor> vendedorList { get; set; }


        public List<Vendedor>  responsableComercialList {
            get { return this.vendedorList.Where(v => v.esResponsableComercial).ToList(); }
        }

        public List<Vendedor> asistenteServicioClienteList
        {
            get { return this.vendedorList.Where(v => v.esAsistenteServicioCliente).ToList(); }
        }

        public List<Vendedor> responsablePortafolioList
        {
            get { return this.vendedorList.Where(v => v.esResponsablePortafolio).ToList(); }
        }

        public List<Vendedor> supervisorComercialList
        {
            get { return this.vendedorList.Where(v => v.esSupervisorComercial).ToList(); }
        }

    }
}

    
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class Usuario : Auditoria
    {
        public Usuario()
        {
            this.vendedorList = new List<Vendedor>();
            this.permisoList = new List<Permiso>();
        }

        public Guid idUsuario { get; set; }
        /*DATOS*/
        [Display(Name = "Email:")]
        public string email { get; set; }
        public string password { get; set; }
        [Display(Name = "Nombre:")]
        public string nombre { get; set; }
        [Display(Name = "Cargo:")]
        public string cargo { get; set; }
        [Display(Name = "Contacto:")]
        public string contacto { get; set; }
        public Decimal maximoPorcentajeDescuentoAprobacion { get; set; }

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

        public List<AlertaValidacion> alertasList { get; set; }

        public String cotizacionSerializada { get; set; }
        public String pedidoSerializado { get; set; }


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

        public bool esCliente { get; set; }

        public bool esVendedor { get; set; }


        /// <summary>
        /// PERMISOS
        /// </summary>

        public List<Permiso> permisoList { get; set; }

        public bool TienePermiso(int idPermiso)
        {
            return this.permisoList.Where(item => item.idPermiso.Equals(idPermiso)).FirstOrDefault() != null;
        }

        public bool PermisoPorRol(int idPermiso)
        {
            try
            {
                return this.permisoList.Where(item => item.idPermiso.Equals(idPermiso)).FirstOrDefault().byRol;
            } catch (Exception ex)
            {
                return false;
            }
        }

        /*PERMISOS COTIZACION*/
        public bool apruebaCotizaciones { get { return apruebaCotizacionesLima || apruebaCotizacionesProvincias; } }
        public bool apruebaCotizacionesLima { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.APRUEBA_COTIZACIONES_LIMA)).FirstOrDefault() != null;  } }
        public bool apruebaCotizacionesProvincias { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.APRUEBA_COTIZACIONES_PROVINCIAS)).FirstOrDefault() != null; } }
        public bool creaCotizaciones { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.CREA_COTIZACIONES_LIMA)).FirstOrDefault() != null; } }
        public bool visualizaCotizaciones { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_COTIZACIONES)).FirstOrDefault() != null; } }
        public bool eliminaCotizacionesAceptadas { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.ELIMINA_COTIZACIONES_ACEPTADAS)).FirstOrDefault() != null; } }
        public bool visualizaCostos { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_COSTOS)).FirstOrDefault() != null; } }
        public bool creaCotizacionesGrupales { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.CREA_COTIZACIONES_GRUPALES)).FirstOrDefault() != null; } }
        public bool apruebaCotizacionesGrupales { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.APRUEBA_COTIZACIONES_GRUPALES)).FirstOrDefault() != null; } }
        public bool creaCotizacionesProvincias { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.CREA_COTIZACIONES_PROVINCIAS)).FirstOrDefault() != null; } }
        public bool visualizaMargen { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_MARGEN)).FirstOrDefault() != null; } }

        /*PERMISOS PEDIDO*/
        public bool tomaPedidos { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.TOMA_PEDIDOS_LIMA)).FirstOrDefault() != null; } }
        public bool modificaPedidoFechaEntregaExtendida { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.MODIFICA_PEDIDO_FECHA_ENTREGA_EXTENDIDA)).FirstOrDefault() != null; } }
        public bool realizaCargaMasivaPedidos { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.REALIZA_CARGA_MASIVA_PEDIDOS)).FirstOrDefault() != null; } }
        public bool apruebaPedidos { get { return apruebaPedidosLima || apruebaPedidosProvincias; } }
        public bool apruebaPedidosCompra { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.APRUEBA_PEDIDOS_COMPRA)).FirstOrDefault() != null; } }
        public bool apruebaPedidosAlmacen { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.APRUEBA_PEDIDOS_ALMACEN)).FirstOrDefault() != null; } }
        public bool apruebaPedidosLima { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.APRUEBA_PEDIDOS_LIMA)).FirstOrDefault() != null; } }
        public bool apruebaPedidosProvincias { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.APRUEBA_PEDIDOS_PROVINCIAS)).FirstOrDefault() != null; } }
        public bool visualizaPedidosLima { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_PEDIDOS_LIMA)).FirstOrDefault() != null; } }
        public bool visualizaPedidosProvincias { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_PEDIDOS_PROVINCIAS)).FirstOrDefault() != null; } }
        public bool visualizaPedidos { get { return visualizaPedidosLima || visualizaPedidosProvincias; } }
        public bool bloqueaPedidos { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.BLOQUEA_PEDIDOS)).FirstOrDefault() != null; } }
        public bool liberaPedidos { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.LIBERA_PEDIDOS)).FirstOrDefault() != null; } }
        public bool tomaPedidosProvincias { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.TOMA_PEDIDOS_PROVINCIAS)).FirstOrDefault() != null; } }
        public bool programaPedidos { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.PROGRAMA_PEDIDOS)).FirstOrDefault() != null; } }
        public bool confirmaStock { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.CONFIRMA_STOCK)).FirstOrDefault() != null; } }
        public bool tomaPedidosCompra { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.TOMA_PEDIDOS_COMPRA)).FirstOrDefault() != null; } }
        public bool tomaPedidosAlmacen { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.TOMA_PEDIDOS_ALMACEN)).FirstOrDefault() != null; } }
        public bool modificaPedidoVentaFechaEntregaHasta { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.MODIFICA_PEDIDO_VENTA_FECHA_ENTREGA_HASTA)).FirstOrDefault() != null; } }

        /*PERMISOS GUIA REMISION*/
        public bool creaGuias { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.CREA_GUIAS)).FirstOrDefault() != null; } }
        public bool administraGuias { get { return administraGuiasLima || administraGuiasProvincias; } }
        public bool administraGuiasLima { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.ADMINISTRA_GUIAS_LIMA)).FirstOrDefault() != null; } }
        public bool administraGuiasProvincias { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.ADMINISTRA_GUIAS_PROVINCIAS)).FirstOrDefault() != null; } }
        public bool visualizaGuias { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_GUIAS_REMISION)).FirstOrDefault() != null; } }

        /*PERMISOS FACTURA ELECTRONICA*/
        public bool creaDocumentosVenta { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.CREA_DOCUMENTOS_VENTA)).FirstOrDefault() != null; } }
        public bool creaDocumentosCompra { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.CREA_DOCUMENTOS_COMPRA)).FirstOrDefault() != null; } }
        public bool creaFacturaConsolidadaMultiregional { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.CREA_FACTURA_CONSOLIDADA_MULTIREGIONAL)).FirstOrDefault() != null; } }
        public bool visualizaGuiasPendienteFacturacion { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_GUIAS_PENDIENTES_FACTURACION)).FirstOrDefault() != null; } }
        public bool creaFacturaConsolidadaLocal { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.CREA_FACTURA_CONSOLIDADA_LOCAL)).FirstOrDefault() != null; } }
        public bool administraDocumentosVenta { get { return administraDocumentosVentaLima || administraDocumentosVentaProvincias; } }
        public bool apruebaAnulaciones { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.APRUEBA_ANULACIONES)).FirstOrDefault() != null; } }
        public bool administraDocumentosVentaLima { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.ADMINISTRA_DOCUMENTOS_VENTA_LIMA)).FirstOrDefault() != null; } }
        public bool administraDocumentosVentaProvincias { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.ADMINISTRA_DOCUMENTOS_VENTA_PROVINCIAS)).FirstOrDefault() != null; } }
        public bool visualizaDocumentosVenta { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_DOCUMENTOS_VENTA)).FirstOrDefault() != null; } }
        public bool visualizaDocumentosCompra { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_DOCUMENTOS_COMPRA)).FirstOrDefault() != null; } }
        public bool creaNotasCredito { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.CREA_NOTAS_CREDITO)).FirstOrDefault() != null; } }
        public bool creaNotasDebito { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.CREA_NOTAS_DEBITO)).FirstOrDefault() != null; } }
        public bool realizaRefacturacion { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.REALIZA_REFACTURACION)).FirstOrDefault() != null; } }
        

        /*Grupo Clientes*/
        public bool modificaGrupoClientes { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.MODIFICA_GRUPO_CLIENTES)).FirstOrDefault() != null; } }
        public bool visualizaGrupoClientes { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_GRUPO_CLIENTES)).FirstOrDefault() != null; } }
        public bool buscaSedesGrupoCliente { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.BUSCA_SEDES_GRUPO_CLIENTE)).FirstOrDefault() != null; } }
        public bool modificaCanastaGrupoCliente { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.MODIFICA_CANASTA_GRUPO_CLIENTE)).FirstOrDefault() != null; } }
        public bool modificaMiembrosGrupoCliente { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.MODIFICA_MIEMBROS_GRUPO_CLIENTE)).FirstOrDefault() != null; } }

        /*Clientes*/

        public bool modificaMaestroClientes { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.MODIFICA_MAESTRO_CLIENTES)).FirstOrDefault() != null; } }
        public bool bloqueaClientes { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.BLOQUEA_CLIENTES)).FirstOrDefault() != null; } }
        public bool modificaCanales { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.MODIFICA_CANALES)).FirstOrDefault() != null; } }
        public bool asignaSubdistribuidor { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.ASIGNA_SUBDISTRIBUIDOR)).FirstOrDefault() != null; } }
        public bool modificaNegociacionMultiregional { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.MODIFICA_NEGOCIACION_MULTIREGIONAL)).FirstOrDefault() != null; } }
        public bool apruebaPlazoCredito { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.DEFINE_PLAZO_CREDITO)).FirstOrDefault() != null; } }
        public bool apruebaMontoCredito { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.DEFINE_MONTO_CREDITO)).FirstOrDefault() != null; } }
        public bool modificaResponsableComercial { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.DEFINE_RESPONSABLE_COMERCIAL)).FirstOrDefault() != null; } }
        public bool modificaSupervisorComercial { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.DEFINE_SUPERVISOR_COMERCIAL)).FirstOrDefault() != null; } }
        public bool modificaAsistenteAtencionCliente { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.DEFINE_ASISTENTE_ATENCION_CLIENTE)).FirstOrDefault() != null; } }
        public bool modificaResponsablePortafolio { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.DEFINE_RESPONSABLE_PORTAFOLIO)).FirstOrDefault() != null; } }
        public bool realizaCargaMasivaCliente { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.REALIZA_CARGA_MASIVA_CLIENTES)).FirstOrDefault() != null; } }
        public bool visualizaClientes { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_CLIENTES)).FirstOrDefault() != null; } }
        public bool modificaCanastaCliente { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.MODIFICA_CANASTA_CLIENTE)).FirstOrDefault() != null; } }

        /*Productos*/
        public bool visualizaProductos { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_PRODUCTOS)).FirstOrDefault() != null; } }
        public bool modificaMaestroProductos { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.MODIFICA_MAESTRO_PRODUCTOS)).FirstOrDefault() != null; } }
        //public bool modificaProducto { get; set; }
        public bool realizaCargaMasivaProductos { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.REALIZA_CARGA_MASIVA_PRODUCTOS)).FirstOrDefault() != null; } }

        /*SubDistribuidores*/
        public bool modificaSubDistribuidor { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.MODIFICA_SUBDISTRIBUIDOR)).FirstOrDefault() != null; } }
        public bool visualizaSubDistribuidores { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_SUBDISTRIBUIDORES)).FirstOrDefault() != null; } }

        /*Origenes*/
        public bool modificaOrigen { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.MODIFICA_ORIGEN)).FirstOrDefault() != null; } }
        public bool visualizaOrigenes { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_ORIGENES)).FirstOrDefault() != null; } }

        /*Vendedores*/
        public bool modificaVendedor { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.MODIFICA_VENDEDORES)).FirstOrDefault() != null; } }
        public bool visualizaVendedor { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_VENDEDORES)).FirstOrDefault() != null; } }

        /*LogCambio*/
        public bool modificaLogCambio { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.MODIFICA_LOGCAMBIO)).FirstOrDefault() != null; } }
        public bool visualizaLogCambio { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_LOGCAMBIO)).FirstOrDefault() != null; } }

        /*Notificacion de Documento de Venta*/        
        public bool visualizaDocumentoVentaNotificacion { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_DOCUMENTOVENTANOTIFICACION)).FirstOrDefault() != null; } }


        /*Administra Permisos*/
        public bool administraPermisos { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.ADMINISTRA_PERMISOS)).FirstOrDefault() != null; } }

        public bool modificaDireccionEntrega { get; set; }
        
        /*Roles*/
        public bool modificaRol { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.MODIFICA_ROL)).FirstOrDefault() != null; } }
        public bool visualizaRoles { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_ROLES)).FirstOrDefault() != null; } }

        public bool modificaUsuario { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.MODIFICA_USUARIO)).FirstOrDefault() != null; } }
        public bool visualizaUsuarios { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_USUARIOS)).FirstOrDefault() != null; } }
    }
}

    
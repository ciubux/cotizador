using System;
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

        public int idEmpresa { get; set; }
        public string codigoEmpresa { get; set; }
        public string razonSocialEmpresa { get; set; }
        public string urlEmpresa { get; set; }
        public decimal factorEmpresa { get; set; }
        public decimal pMargenMinimo { get; set; }
        public decimal pDescuentoInfraMargen { get; set; }
        
        public Area area { get; set; }
        public Guid idUsuario { get; set; }
        /*DATOS*/
        [Display(Name = "Email:")]
        public string email { get; set; }
        [Display(Name = "Contraseña:")]
        public string password { get; set; }
        [Display(Name = "Nombre:")]
        public string nombre { get; set; }
        [Display(Name = "Cargo:")]
        public string cargo { get; set; }
        [Display(Name = "Contacto:")]
        public string contacto { get; set; }

        [Display(Name = "Descuento Maximo (%):")]
        public Decimal maximoPorcentajeDescuentoAprobacion { get; set; }

        public Byte[] firmaImagen { get; set; }
        public Guid idUsuarioModificacion { get; set; }
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
        public List<Vendedor> responsableComercialList
        {
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

        [Display(Name = "Cliente:")]
        public bool esCliente { get; set; }

        public bool esVendedor { get; set; }

        public string ipAddress { get; set; }
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
                Permiso p = this.permisoList.Where(item => item.idPermiso.Equals(idPermiso)).FirstOrDefault();
                return p == null ? false : p.byRol;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /*PERMISOS COTIZACION*/
        public bool apruebaCotizaciones { get { return apruebaCotizacionesLima || apruebaCotizacionesProvincias; } }
        public bool apruebaCotizacionesLima { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.APRUEBA_COTIZACIONES_LIMA)).FirstOrDefault() != null; } }
        public bool apruebaCotizacionesProvincias { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.APRUEBA_COTIZACIONES_PROVINCIAS)).FirstOrDefault() != null; } }
        public bool apruebaCotizacionesVentaRestringida { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.APRUEBA_COTIZACIONES_VENTA_RESTRINGIDA)).FirstOrDefault() != null; } }
        public bool creaCotizaciones { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.CREA_COTIZACIONES_LIMA)).FirstOrDefault() != null; } }
        public bool visualizaCotizaciones { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_COTIZACIONES)).FirstOrDefault() != null; } }
        public bool eliminaCotizacionesAceptadas { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.ELIMINA_COTIZACIONES_ACEPTADAS)).FirstOrDefault() != null; } }
        public bool visualizaCostos { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_COSTOS)).FirstOrDefault() != null; } }
        public bool creaCotizacionesGrupales { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.CREA_COTIZACIONES_GRUPALES)).FirstOrDefault() != null; } }
        public bool apruebaCotizacionesGrupales { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.APRUEBA_COTIZACIONES_GRUPALES)).FirstOrDefault() != null; } }
        public bool creaCotizacionesProvincias { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.CREA_COTIZACIONES_PROVINCIAS)).FirstOrDefault() != null; } }
        public bool visualizaMargen { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_MARGEN)).FirstOrDefault() != null; } }

        public bool fijaVigenciaPreciosCotizacion { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.FIJA_VIGENCIA_PRECIOS_COTIZACION)).FirstOrDefault() != null; } }

        

        /*PERMISOS PEDIDO*/
        public bool tomaPedidos { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.TOMA_PEDIDOS_LIMA)).FirstOrDefault() != null; } }
        public bool modificaPedidoFechaEntregaExtendida { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.MODIFICA_PEDIDO_FECHA_ENTREGA_EXTENDIDA)).FirstOrDefault() != null; } }
        public bool realizaCargaMasivaPedidos { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.REALIZA_CARGA_MASIVA_PEDIDOS)).FirstOrDefault() != null; } }
        public bool pedidoRegistraObservacionAlmacen { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.PEDIDO_REGISTRA_OBSERVACION_ALMACEN)).FirstOrDefault() != null; } }
        public bool apruebaPedidos { get { return apruebaPedidosLima || apruebaPedidosProvincias; } }
        public bool apruebaPedidosCompra { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.APRUEBA_PEDIDOS_COMPRA)).FirstOrDefault() != null; } }
        public bool apruebaPedidosAlmacen { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.APRUEBA_PEDIDOS_ALMACEN)).FirstOrDefault() != null; } }
        public bool apruebaPedidosLima { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.APRUEBA_PEDIDOS_LIMA)).FirstOrDefault() != null; } }
        public bool apruebaPedidosProvincias { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.APRUEBA_PEDIDOS_PROVINCIAS)).FirstOrDefault() != null; } }
        public bool apruebaPedidosVentaRestringida { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.APRUEBA_PEDIDOS_VENTA_RESTRINGIDA)).FirstOrDefault() != null; } }

        public bool truncaPedidos { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.TRUNCA_PEDIDOS)).FirstOrDefault() != null; } }
        public bool revierteTruncadoPedidos { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.REVIERTE_TRUNCADO_PEDIDOS)).FirstOrDefault() != null; } }
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
        public bool creaOrdenesCompraCliente { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.CREA_ORDENES_COMPRA_CLIENTE)).FirstOrDefault() != null; } }
        public bool marcaPedidoVentaIndirecta { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.MARCA_PEDIDO_VENTA_INDIRECTA)).FirstOrDefault() != null; } }

        

        /*PERMISOS GUIA REMISION*/
        public bool creaGuias { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.CREA_GUIAS)).FirstOrDefault() != null; } }
        public bool creaGuiasDiferidas { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.CREA_GUIAS_DIFERIDAS)).FirstOrDefault() != null; } }
        public bool administraGuias { get { return administraGuiasLima || administraGuiasProvincias; } }
        public bool administraGuiasLima { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.ADMINISTRA_GUIAS_LIMA)).FirstOrDefault() != null; } }
        public bool administraGuiasProvincias { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.ADMINISTRA_GUIAS_PROVINCIAS)).FirstOrDefault() != null; } }
        public bool visualizaGuias { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_GUIAS_REMISION)).FirstOrDefault() != null; } }

        public bool anulaGuias { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.ANULA_GUIAS_REMISION)).FirstOrDefault() != null; } }

        
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
        public bool cambiaClienteFactura { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.CAMBIA_CLIENTE_FACTURA)).FirstOrDefault() != null; } }
        public bool creaFacturaCompleja { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.CREA_FACTURA_COMPLEJA)).FirstOrDefault() != null; } }

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
        public bool activaClienteFacturaCompleja { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.ACTIVA_CLIENTE_FACTURA_COMPLEJA)).FirstOrDefault() != null; } }

        public bool reasignaCarteraCliente { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.REASIGNA_CARTERA_CLIENTE)).FirstOrDefault() != null; } }

        public bool modificaPreciosEspeciales { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.MODIFICA_PRECIOS_ESPECIALES)).FirstOrDefault() != null; } }


        /*Productos*/
        public bool visualizaProductos { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_PRODUCTOS)).FirstOrDefault() != null; } }
        public bool modificaMaestroProductos { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.MODIFICA_MAESTRO_PRODUCTOS)).FirstOrDefault() != null; } }

        public bool modificaRestriccionVentaProducto { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.MODIFICA_RESTRICCION_VENTA_PRODUCTO)).FirstOrDefault() != null; } }
        
        //public bool modificaProducto { get; set; }
        public bool realizaCargaMasivaProductos { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.REALIZA_CARGA_MASIVA_PRODUCTOS)).FirstOrDefault() != null; } }

        public bool realizaCargaMasivaStock { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.CARGA_MASIVA_STOCK)).FirstOrDefault() != null; } }
        public bool visualizaReporteGlobalStock { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_REPORTE_GLOBAL_STOCK)).FirstOrDefault() != null; } }

        public bool registraAjusteStock { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.REGISTRA_AJUSTE_STOCK)).FirstOrDefault() != null; } }
        public bool apruebaAjusteStock { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.APRUEBA_AJUSTE_STOCK)).FirstOrDefault() != null; } }

        public bool saltaValidacionStock { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.SALTA_VALIDACION_STOCK)).FirstOrDefault() != null; } }

        //public bool modificaProducto { get; set; }
        public bool realizaCargaProductosWEB { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.REALIZA_CARGA_MASIVA_PRODUCTOS)).FirstOrDefault() != null; } }


        /*SubDistribuidores*/
        public bool modificaSubDistribuidor { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.MODIFICA_SUBDISTRIBUIDOR)).FirstOrDefault() != null; } }
        public bool visualizaSubDistribuidores { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_SUBDISTRIBUIDORES)).FirstOrDefault() != null; } }

        /*Origenes*/
        public bool modificaOrigen { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.MODIFICA_ORIGEN)).FirstOrDefault() != null; } }
        public bool visualizaOrigenes { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_ORIGENES)).FirstOrDefault() != null; } }


        /*Promociones*/
        public bool modificaPromocion { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.MODIFICA_PROMOCION)).FirstOrDefault() != null; } }

        /*Rubros*/
        public bool modificaRubro { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.MODIFICA_RUBRO)).FirstOrDefault() != null; } }
        public bool visualizaRubros { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_RUBROS)).FirstOrDefault() != null; } }

        /*Clietne Contacto Tipo*/
        public bool modificaClienteContactoTipo { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.MODIFICA_CLIENTE_CONTACTO_TIPO)).FirstOrDefault() != null; } }

        /*Vendedores*/
        public bool modificaVendedor { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.MODIFICA_VENDEDORES)).FirstOrDefault() != null; } }
        public bool visualizaVendedor { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_VENDEDORES)).FirstOrDefault() != null; } }

        /*LogCambio*/
        public bool modificaLogCampo { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.MODIFICA_LOGCAMPO)).FirstOrDefault() != null; } }
        public bool visualizaLogCampo { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_LOGCAMPO)).FirstOrDefault() != null; } }
        public bool modificaLogDatoHistorico { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.MODIFICA_LOGDATOHISTORICO)).FirstOrDefault() != null; } }

        /*Mensaje*/
        public bool modificaMensaje { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.MODIFICA_MENSAJE)).FirstOrDefault() != null; } }

        public bool visualizaMensaje { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.LISTA_MENSAJE)).FirstOrDefault() != null; } }

        public bool enviaMensaje { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.ENVIA_MENSAJE)).FirstOrDefault() != null; } }

        /*Dashboard*/
        public bool modificaVistaDashboard { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.MODIFICA_VISTA_DASHBOARD)).FirstOrDefault() != null; } }

        /*Parametro*/
        public bool modificaParametro { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.MODIFICA_PARAMETRO)).FirstOrDefault() != null; } }

        /*Archivos*/
        public bool verArchivos { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VER_ARCHIVO)).FirstOrDefault() != null; } }

        /*Exporta Ventas Contabilidad*/
        public bool exportarVentasContabilidad { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.EXPORTAR_VENTAS_CONTABILIDAD)).FirstOrDefault() != null; } }

        /*Notificacion de Documento de Venta*/
        public bool visualizaDocumentoVentaNotificacion { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_DOCUMENTOVENTANOTIFICACION)).FirstOrDefault() != null; } }

        /*Venta*/
        public bool visualizaVentas { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_VENTAS)).FirstOrDefault() != null; } }
        public bool rectificarVenta { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.RECTIFICAR_VENTA)).FirstOrDefault() != null; } }

        /*KPI*/
        public bool NivelKpiMultiUsuario { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.NIVEL_KPI_MULTI_USUARIO)).FirstOrDefault() != null; } }
        public bool NivelKpiMultiArea { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.NIVEL_KPI_MULTI_AREA)).FirstOrDefault() != null; } }

        /*Administra Permisos*/
        public bool administraPermisos { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.ADMINISTRA_PERMISOS)).FirstOrDefault() != null; } }

        public bool modificaDireccionEntrega { get; set; }

        public bool multiEmpresa { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.MULTI_EMPRESA)).FirstOrDefault() != null; } }

        /*Roles*/
        public bool modificaRol { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.MODIFICA_ROL)).FirstOrDefault() != null; } }
        public bool visualizaRoles { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_ROLES)).FirstOrDefault() != null; } }

        public bool modificaUsuario { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.MODIFICA_USUARIO)).FirstOrDefault() != null; } }
        public bool visualizaUsuarios { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_USUARIOS)).FirstOrDefault() != null; } }

        public bool buscaNotasIngresoTodasSedes { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.BUSCA_NOTAS_INGRESO_TODAS_SEDES)).FirstOrDefault() != null; } }
        public bool validaReponsablescomercialesAsignados { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VALIDA_RESPONSABLES_COMERCIALES_ASIGNADOS)).FirstOrDefault() != null; } }

        /* REPORTES */
        public bool visualizaReporteSellOutPersonalizado{ get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_REPORTE_SELLOUT_PERSONALIZADO)).FirstOrDefault() != null; } }
        public bool visualizaReporteProductosPendientesAtencion { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_REPORTE_PRODUCTOS_PENDIENTES_ATENCION)).FirstOrDefault() != null; } }

        public bool visualizaReporteSellOutVendedores { get { return this.permisoList.Where(u => u.codigo.Equals(Constantes.VISUALIZA_REPORTE_SELLOUT_VENDEDORES)).FirstOrDefault() != null; } }
    }
}

 
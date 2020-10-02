using BusinessLayer;
using Cotizador.ExcelExport;
using Model;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Cotizador.Models.DTOsSearch;
using NLog;
using Cotizador.Models.DTOsShow;

namespace Cotizador.Controllers
{
    public class OrdenCompraClienteController : ParentController
    {

        private OrdenCompraCliente OrdenCompraClienteSession {
            get {

                OrdenCompraCliente occ = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaOrdenCompraClientes: occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE_BUSQUEDA]; break;
                    case Constantes.paginas.MantenimientoOrdenCompraCliente: occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE]; break;
                }
                return occ;
            }
            set {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaOrdenCompraClientes: this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE_BUSQUEDA] = value; break;
                    case Constantes.paginas.MantenimientoOrdenCompraCliente: this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE] = value; break;
                }
            }
        }

        // GET: OrdenCompraCliente

        private void instanciarOrdenCompraClienteBusqueda()
        {
            try
            {
                OrdenCompraCliente occTmp = new OrdenCompraCliente();
                DateTime fechaDesde = DateTime.Now.AddDays(-Constantes.DIAS_DESDE_BUSQUEDA);
                DateTime fechaHasta = DateTime.Now.AddDays(1);
                occTmp.cotizacion = new Cotizacion();
                occTmp.solicitante = new Solicitante();
                occTmp.direccionEntrega = new DireccionEntrega();

                occTmp.fechaCreacionDesde = new DateTime(fechaDesde.Year, fechaDesde.Month, fechaDesde.Day, 0, 0, 0);
                occTmp.fechaCreacionHasta = fechaHasta;

                occTmp.fechaEntregaDesde = null;// new DateTime(fechaDesde.Year, fechaDesde.Month, fechaDesde.Day, 0, 0, 0);
                occTmp.fechaEntregaHasta = null;// DateTime.Now.AddDays(Constantes.DIAS_DESDE_BUSQUEDA);

                occTmp.fechaProgramacionDesde = null;// new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                occTmp.fechaProgramacionHasta = null;// new DateTime(fechaHasta.Year, fechaHasta.Month, fechaHasta.Day, 23, 59, 59);

                occTmp.buscarSedesGrupoCliente = false;

                occTmp.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                occTmp.ciudad = new Ciudad();
                occTmp.cliente = new Cliente();
                occTmp.clienteSunat = new ClienteSunat();
                occTmp.solicitante = new Solicitante();


                occTmp.detalleList = new List<OrdenCompraClienteDetalle>();

                occTmp.usuarioBusqueda = new Usuario { idUsuario = Guid.Empty };

                this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE_BUSQUEDA] = occTmp;
                this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE_LISTA] = new List<OrdenCompraCliente>();
            }
            catch (Exception e)
            {
                logger.Error(e, agregarUsuarioAlMensaje(e.Message));
                throw e;
            }
        }


        public ActionResult Index(Guid? idOrdenCompraCliente = null)
        {
            try
            {
                this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaOrdenCompraClientes;

                if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                    if (!usuario.creaOrdenesCompraCliente && !usuario.creaOrdenesCompraCliente)
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }

                if (this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE_BUSQUEDA] == null)
                {
                    instanciarOrdenCompraClienteBusqueda();
                }

                OrdenCompraCliente occSearch = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE_BUSQUEDA];

                //Si existe cotizacion se debe verificar si no existe cliente
                if (this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE] != null)
                {
                    OrdenCompraCliente occEdicion = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE];
                    if (occEdicion.ciudad == null || occEdicion.ciudad.idCiudad == null
                        || occEdicion.ciudad.idCiudad == Guid.Empty)
                    {
                        this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE] = null;
                    }

                }

                ViewBag.fechaCreacionDesde = occSearch.fechaCreacionDesde.ToString(Constantes.formatoFecha);
                ViewBag.fechaCreacionHasta = occSearch.fechaCreacionHasta.ToString(Constantes.formatoFecha);


                if (this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE_LISTA] == null)
                {
                    this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE_LISTA] = new List<OrdenCompraCliente>();
                }


                int existeCliente = 0;
                //  if (cotizacion.cliente.idCliente != Guid.Empty || cotizacion.grupo.idGrupo != Guid.Empty)
                if (occSearch.cliente.idCliente != Guid.Empty)
                {
                    existeCliente = 1;
                }

                GuiaRemision guiaRemision = new GuiaRemision();
                guiaRemision.motivoTraslado = GuiaRemision.motivosTraslado.Venta;

                ViewBag.guiaRemision = guiaRemision;

                ViewBag.occ = occSearch;
                DocumentoVenta documentoVenta = new DocumentoVenta();
                documentoVenta.tipoPago = DocumentoVenta.TipoPago.NoAsignado;
                documentoVenta.formaPago = DocumentoVenta.FormaPago.NoAsignado;
                documentoVenta.fechaEmision = DateTime.Now;
                ViewBag.documentoVenta = documentoVenta;
                ViewBag.fechaEmision = documentoVenta.fechaEmision.Value.ToString(Constantes.formatoFecha);
                ViewBag.horaEmision = documentoVenta.fechaEmision.Value.ToString(Constantes.formatoHora);
                ViewBag.lista = this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE_LISTA];
                ViewBag.existeCliente = existeCliente;

                ViewBag.pagina = (int)Constantes.paginas.BusquedaOrdenCompraClientes;

                ViewBag.idOrdenCompraCliente = idOrdenCompraCliente;
                return View();
            }
            catch (Exception e)
            {
                logger.Error(e, agregarUsuarioAlMensaje(e.Message));
                throw e;
            }
        }


        public ActionResult CargarOrdenCompraClientes()
        {
            try
            {
                this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.MantenimientoOrdenCompraCliente;

                //Si no hay usuario, se dirige el logueo
                if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                    if (!usuario.esCliente)
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }

                ViewBag.debug = Constantes.DEBUG;
                ViewBag.Si = Constantes.MENSAJE_SI;
                ViewBag.No = Constantes.MENSAJE_NO;
                ViewBag.IGV = Constantes.IGV;


                //Si no se está trabajando con una cotización se crea una y se agrega a la sesion

                if (this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE] == null)
                {

                    instanciarOrdenCompraCliente();
                }
                OrdenCompraCliente occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE];


                int existeCliente = 0;
                if (occ.cliente.idCliente != Guid.Empty)
                {
                    existeCliente = 1;
                }

                ViewBag.existeCliente = existeCliente;
                ViewBag.idClienteGrupo = occ.cliente.idCliente;
                ViewBag.clienteGrupo = occ.cliente.ToString();

                ViewBag.fechaSolicitud = occ.fechaSolicitud.ToString(Constantes.formatoFecha);
                ViewBag.horaSolicitud = occ.fechaSolicitud.ToString(Constantes.formatoHora);

                ViewBag.fechaEntregaDesde = occ.fechaEntregaDesde == null ? "" : occ.fechaEntregaDesde.Value.ToString(Constantes.formatoFecha);
                ViewBag.fechaEntregaHasta = occ.fechaEntregaHasta == null ? "" : occ.fechaEntregaHasta.Value.ToString(Constantes.formatoFecha);



                ViewBag.occ = occ;
                ViewBag.VARIACION_PRECIO_ITEM_ORDEN_COMPRA_CLIENTE = Constantes.VARIACION_PRECIO_ITEM_PEDIDO;

                ViewBag.fechaPrecios = DateTime.Now.AddDays(-Constantes.DIAS_MAX_BUSQUEDA_PRECIOS).ToString(Constantes.formatoFecha);
                //   ViewBag.fechaPrecios = occ.fechaPrecios.ToString(Constantes.formatoFecha);

                ViewBag.pagina = (int)Constantes.paginas.MantenimientoOrdenCompraCliente;
                return View();
            }
            catch (Exception e)
            {
                logger.Error(e, agregarUsuarioAlMensaje(e.Message));
                throw e;
            }
        }


        public ActionResult Editar()
        {
            try
            {
                this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.MantenimientoOrdenCompraCliente;

                //Si no hay usuario, se dirige el logueo
                if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                    if (!usuario.creaOrdenesCompraCliente)
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }

                ViewBag.debug = Constantes.DEBUG;
                ViewBag.Si = Constantes.MENSAJE_SI;
                ViewBag.No = Constantes.MENSAJE_NO;
                ViewBag.IGV = Constantes.IGV;


                //Si no se está trabajando con una cotización se crea una y se agrega a la sesion

                if (this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE] == null)
                {

                    instanciarOrdenCompraCliente();
                }
                OrdenCompraCliente occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE];


                int existeCliente = 0;
                if (occ.cliente.idCliente != Guid.Empty)
                {
                    existeCliente = 1;
                }
                    
                ViewBag.existeCliente = existeCliente;
                ViewBag.idClienteGrupo = occ.cliente.idCliente;
                ViewBag.clienteGrupo = occ.cliente.ToString();
                
                ViewBag.fechaSolicitud = occ.fechaSolicitud.ToString(Constantes.formatoFecha);
                ViewBag.horaSolicitud = occ.fechaSolicitud.ToString(Constantes.formatoHora);

                ViewBag.fechaEntregaDesde = occ.fechaEntregaDesde ==null?"": occ.fechaEntregaDesde.Value.ToString(Constantes.formatoFecha);
                ViewBag.fechaEntregaHasta = occ.fechaEntregaHasta == null?"": occ.fechaEntregaHasta.Value.ToString(Constantes.formatoFecha);

       

                ViewBag.occ = occ;
                ViewBag.VARIACION_PRECIO_ITEM_ORDEN_COMPRA_CLIENTE = Constantes.VARIACION_PRECIO_ITEM_PEDIDO;

                ViewBag.fechaPrecios = DateTime.Now.AddDays(-Constantes.DIAS_MAX_BUSQUEDA_PRECIOS).ToString(Constantes.formatoFecha);

                ViewBag.busquedaProductosIncluyeDescontinuados = 0;
                if (this.Session[Constantes.VAR_SESSION_PEDIDO_SEARCH_PRODUCTO_PARAM + "incluyeDescontinuados"] != null)
                {
                    ViewBag.busquedaProductosIncluyeDescontinuados = int.Parse(this.Session[Constantes.VAR_SESSION_PEDIDO_SEARCH_PRODUCTO_PARAM + "incluyeDescontinuados"].ToString());
                }

                ViewBag.pagina = (int)Constantes.paginas.MantenimientoOrdenCompraCliente;
                return View();
            }
            catch (Exception e)
            {
                logger.Error(e, agregarUsuarioAlMensaje(e.Message));
                throw e;
            }
        }

        /*
        public void iniciarEdicionOrdenCompraClienteDesdeCotizacion()
        {
            //try
            //{
                if (this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE] == null)
                {

                    instanciarOrdenCompraCliente();
                }
                OrdenCompraCliente occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE];

                Cotizacion cotizacion = (Cotizacion)this.Session[Constantes.VAR_SESSION_COTIZACION_VER];
                occ.cotizacion = new Cotizacion();
                occ.cotizacion.idCotizacion = cotizacion.idCotizacion;
                occ.cotizacion.codigo = cotizacion.codigo;
                occ.ciudad = cotizacion.ciudad;
                occ.cliente = cotizacion.cliente;
                occ.esPagoContado = cotizacion.esPagoContado;


                if (cotizacion.cliente.horaInicioPrimerTurnoEntrega != null && !cotizacion.cliente.horaInicioPrimerTurnoEntrega.Equals("00:00:00"))
                {
                    occ.horaEntregaDesde = cotizacion.cliente.horaInicioPrimerTurnoEntregaFormat;
                }
                if (cotizacion.cliente.horaFinPrimerTurnoEntrega != null && !cotizacion.cliente.horaFinPrimerTurnoEntrega.Equals("00:00:00"))
                {
                    occ.horaEntregaHasta = cotizacion.cliente.horaFinPrimerTurnoEntregaFormat;
                }

                if (cotizacion.cliente.horaInicioSegundoTurnoEntrega != null && !cotizacion.cliente.horaInicioSegundoTurnoEntrega.Equals("00:00:00"))
                {
                    occ.horaEntregaAdicionalDesde = cotizacion.cliente.horaInicioSegundoTurnoEntregaFormat;
                }
                if (cotizacion.cliente.horaFinSegundoTurnoEntrega != null && !cotizacion.cliente.horaFinSegundoTurnoEntrega.Equals("00:00:00"))
                {
                    occ.horaEntregaAdicionalHasta = cotizacion.cliente.horaFinSegundoTurnoEntregaFormat;
                }


                SolicitanteBL solicitanteBL = new SolicitanteBL();
                occ.cliente.solicitanteList = solicitanteBL.getSolicitantes(cotizacion.cliente.idCliente);

                DireccionEntregaBL direccionEntregaBL = new DireccionEntregaBL();
                occ.cliente.direccionEntregaList = direccionEntregaBL.getDireccionesEntrega(cotizacion.cliente.idCliente);
                occ.direccionEntrega = new DireccionEntrega();

                occ.observaciones = String.Empty;

                occ.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                occ.seguimientoOrdenCompraCliente = new SeguimientoOrdenCompraCliente();

                occ.occDetalleList = new List<OrdenCompraClienteDetalle>();
                foreach (DocumentoDetalle documentoDetalle in  cotizacion.documentoDetalle)
                {
                    OrdenCompraClienteDetalle occDetalle = new OrdenCompraClienteDetalle(occ.usuario.visualizaCostos, occ.usuario.visualizaMargen);
                    occDetalle.cantidad = documentoDetalle.cantidad;
                    if (documentoDetalle.cantidad == 0)
                        occDetalle.cantidad = 1;

                    //occDetalle.costoAnterior = documentoDetalle.costoAnterior;
                    occDetalle.esPrecioAlternativo = documentoDetalle.esPrecioAlternativo;
                    occDetalle.flete = documentoDetalle.flete;
                    occDetalle.observacion = documentoDetalle.observacion;


          
                   // occDetalle.porcentajeDescuento = documentoDetalle.porcentajeDescuento;
                

                    occDetalle.producto = documentoDetalle.producto;
                    
                    


                    if (cotizacion.tipoCotizacion == Cotizacion.TiposCotizacion.Transitoria)
                    {
                        if (occDetalle.producto.precioClienteProducto == null)
                        {
                            occDetalle.producto.precioClienteProducto = new PrecioClienteProducto();
                        }
                        //Se le asigna un id temporal solo para que no se rechaza el precio en la validación
                        occDetalle.producto.precioClienteProducto.idPrecioClienteProducto = Guid.NewGuid();
                        occDetalle.producto.precioClienteProducto.fechaInicioVigencia = cotizacion.fechaInicioVigenciaPrecios;
                        occDetalle.producto.precioClienteProducto.fechaFinVigencia = cotizacion.fechaFinVigenciaPrecios.HasValue ? cotizacion.fechaFinVigenciaPrecios.Value : cotizacion.fechaInicioVigenciaPrecios.HasValue ? cotizacion.fechaInicioVigenciaPrecios.Value.AddDays(Constantes.DIAS_MAX_COTIZACION_TRANSITORIA) : DateTime.Now.AddDays(10);
                        //occDetalle.producto.precioClienteProducto.cliente.idCliente = cotizacion.cliente.idCliente;
                        occDetalle.producto.precioClienteProducto.precioUnitario = documentoDetalle.precioUnitario;
                        occDetalle.producto.precioClienteProducto.precioNeto = documentoDetalle.precioNeto;
                        occDetalle.producto.precioClienteProducto.esUnidadAlternativa = documentoDetalle.esPrecioAlternativo;
                      
                    }
                    else if (cotizacion.tipoCotizacion == Cotizacion.TiposCotizacion.Trivial)
                    {
                        //Se le asigna un id temporal solo para que no se rechaza el precio en la validación
                        occDetalle.producto.precioClienteProducto.idPrecioClienteProducto = Guid.NewGuid();
                        //occDetalle.producto.precioClienteProducto.fechaInicioVigencia = cotizacion.fechaInicioVigenciaPrecios;
                        //occDetalle.producto.precioClienteProducto.fechaFinVigencia = cotizacion.fechaFinVigenciaPrecios.HasValue ? cotizacion.fechaFinVigenciaPrecios.Value : cotizacion.fechaInicioVigenciaPrecios.Value.AddDays(Constantes.DIAS_MAX_COTIZACION_TRANSITORIA);
                        //occDetalle.producto.precioClienteProducto.cliente.idCliente = cotizacion.cliente.idCliente;
                        occDetalle.producto.precioClienteProducto.precioUnitario = documentoDetalle.precioUnitario;
                        occDetalle.producto.precioClienteProducto.precioNeto = documentoDetalle.precioNeto;
                        occDetalle.producto.precioClienteProducto.esUnidadAlternativa = documentoDetalle.esPrecioAlternativo;
                    }
                    else
                    {
                        if(occDetalle.producto.precioClienteProducto.esUnidadAlternativa)
                        {
                            occDetalle.producto.precioClienteProducto.precioUnitario = occDetalle.producto.precioClienteProducto.precioUnitario / occDetalle.producto.precioClienteProducto.equivalencia;
                        }


                    }

                    if (documentoDetalle.esPrecioAlternativo)
                    {
                        occDetalle.precioNeto = documentoDetalle.precioNeto * documentoDetalle.ProductoPresentacion.Equivalencia;
                        occDetalle.ProductoPresentacion = documentoDetalle.ProductoPresentacion;
                    }
                    else
                    {
                        occDetalle.precioNeto = documentoDetalle.precioNeto;
                    }
                    occDetalle.unidad = documentoDetalle.unidad;

                    occDetalle.porcentajeDescuento = 100 - (occDetalle.precioNeto * 100 / occDetalle.precioLista);

                    occ.occDetalleList.Add(occDetalle);
                }
                occ.fechaPrecios = occ.fechaSolicitud.AddDays(Constantes.DIAS_MAX_BUSQUEDA_PRECIOS * -1);
                OrdenCompraClienteBL occBL = new OrdenCompraClienteBL();
                occBL.calcularMontosTotales(occ);
                this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE] = occ;
            //}
            //catch (Exception e)
            //{
            //    logger.Error(e, agregarUsuarioAlMensaje(e.Message));
            //    throw e;
            //}

        }
        */

        private void instanciarOrdenCompraCliente()
        {
            try
            {
                OrdenCompraCliente occ = new OrdenCompraCliente();
                occ.idOrdenCompraCliente = Guid.Empty;
                occ.numeroOrdenCompraCliente = 0;
                occ.cotizacion = new Cotizacion();
                occ.ubigeoEntrega = new Ubigeo();
                occ.ubigeoEntrega.Id = "000000";
                occ.ciudad = new Ciudad();
                occ.cliente = new Cliente();
                occ.esPagoContado = false;

                occ.clienteSunat = new ClienteSunat();

                occ.tipoOrdenCompraCliente = OrdenCompraCliente.tiposOrdenCompraCliente.Venta;
                occ.ciudadASolicitar = new Ciudad();

                occ.numeroReferenciaCliente = null;
                occ.direccionEntrega = new DireccionEntrega();
                occ.solicitante = new Solicitante();
                occ.fechaSolicitud = DateTime.Now;
                occ.fechaEntregaDesde = null;
                occ.fechaEntregaHasta = null;
                occ.horaEntregaDesde = "09:00";
                occ.horaEntregaHasta = "18:00";
                occ.contactoOrdenCompraCliente = String.Empty;
                occ.telefonoContactoOrdenCompraCliente = String.Empty;
                occ.incluidoIGV = false;
              //  occ.tasaIGV = Constantes.IGV;
                //occ.flete = 0;
               // occ.mostrarCodigoProveedor = true;
                occ.observaciones = String.Empty;

                occ.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                occ.ciudad = occ.usuario.sedeMP;
                occ.sedesClienteSunat = new List<Ciudad>();
                occ.detalleList = new List<OrdenCompraClienteDetalle>();
                occ.AdjuntoList = new List<ArchivoAdjunto>();
                occ.fechaPrecios = occ.fechaSolicitud.AddDays(Constantes.DIAS_MAX_BUSQUEDA_PRECIOS * -1);

                this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE] = occ;
            }
            catch (Exception e)
            {
                logger.Error(e, agregarUsuarioAlMensaje(e.Message));
                throw e;
            }
        }

        public void iniciarEdicionOrdenCompraCliente()
        {
            //try
            //{
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                OrdenCompraCliente occVer = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE_VER];
                OrdenCompraClienteBL occBL = new OrdenCompraClienteBL();
                OrdenCompraCliente occ = new OrdenCompraCliente();
                ClienteBL clienteBl = new ClienteBL();
                occ.idOrdenCompraCliente = occVer.idOrdenCompraCliente;
                //    occ.fechaModificacion = cotizacionVer.fechaModificacion;
                occ.usuario = (Usuario)this.Session["usuario"];
                //Se cambia el estado de la cotizacion a Edición
                //Se obtiene los datos de la cotización ya modificada
                occ = occVer;
                occ.cliente = new Cliente();
                occ.sedesClienteSunat = clienteBl.getCiudadesCliente(occ.clienteSunat.idClienteSunat);
                this.evaluarCiudadSedeOCC(occ);

                //occ = occBL.GetOrdenCompraClienteParaEditar(occ,usuario);
                //Temporal
                occ.ciudadASolicitar = new Ciudad();
           
              /*  if (occ.tipoOrdenCompraCliente == OrdenCompraCliente.tiposOrdenCompraCliente.TrasladoInterno)
                {
                    occ.ciudadASolicitar = new Ciudad { idCiudad = occ.ciudad.idCiudad,
                                                            nombre = occ.ciudad.nombre,
                                                            esProvincia = occ.ciudad.esProvincia };

                    occ.ciudad = occ.cliente.ciudad;
                }*/

                this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE] = occ;
            //}
            //catch (Exception e)
            //{
            //    logger.Error(e, agregarUsuarioAlMensaje(e.Message));
            //    throw e;
            //}
        }

        
        [HttpGet]
        public ActionResult CargarProductosCanasta(int tipo)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            OrdenCompraCliente occ = this.OrdenCompraClienteSession;
            OrdenCompraClienteBL occBL = new OrdenCompraClienteBL();

            occBL.obtenerProductosAPartirdePreciosCotizados(occ, tipo==2, usuario);

            return RedirectToAction("Pedir", "OrdenCompraCliente");
        }

        public String SearchProductos()
        {
            try
            {
                String texto_busqueda = this.Request.Params["data[q]"];
                ProductoBL bl = new ProductoBL();

                int incluyeDescontinuados = 0;
                if (this.Session[Constantes.VAR_SESSION_PEDIDO_SEARCH_PRODUCTO_PARAM + "incluyeDescontinuados"] != null)
                {
                    incluyeDescontinuados = int.Parse(this.Session[Constantes.VAR_SESSION_PEDIDO_SEARCH_PRODUCTO_PARAM + "incluyeDescontinuados"].ToString());
                }

                OrdenCompraCliente occ = this.OrdenCompraClienteSession;
                String resultado = bl.getProductosBusqueda(texto_busqueda, false, this.Session["proveedor"] != null ? (String)this.Session["proveedor"] : "Todos", this.Session["familia"] != null ? (String)this.Session["familia"] : "Todas", Pedido.tiposPedido.Venta, incluyeDescontinuados);
                return resultado;
            }
            catch (Exception e)
            {
                logger.Error(e,agregarUsuarioAlMensaje(e.Message));
                throw e;
            }
        }

        public void SetSearchProductParam()
        {
            String parametro = this.Request.Params["parametro"];
            String valor = this.Request.Params["valor"];
            this.Session[Constantes.VAR_SESSION_PEDIDO_SEARCH_PRODUCTO_PARAM + parametro] = valor;
        }


        public ActionResult CancelarCreacionOrdenCompraCliente()
        {
            try
            {
                this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE] = null;
                UsuarioBL usuarioBL = new UsuarioBL();
                Usuario usuario = (Usuario)this.Session["usuario"];

                HttpSessionStateBase sessionParams = this.Session;
                var arcBL = new ArchivoAdjuntoBL();
                arcBL.limpiarAsociarAchivoRegistro(sessionParams, ArchivoAdjunto.ArchivoAdjuntoOrigen.OC_OCCLIENTE);


                return RedirectToAction("Index", "OrdenCompraCliente");
            }
            catch (Exception e)
            {
                logger.Error(e, agregarUsuarioAlMensaje(e.Message));
                throw e;
            }
        }



        #region CONTROLES CHOOSEN

        public String SearchClientes()
        {
            try
            {
                String data = this.Request.Params["data[q]"];
                ClienteBL clienteBL = new ClienteBL();
                OrdenCompraCliente occ = this.OrdenCompraClienteSession;
                return clienteBL.getClientesSunatBusqueda(data);
            }
            catch (Exception e)
            {
                logger.Error(e, agregarUsuarioAlMensaje(e.Message));
                throw e;
            }
        }



        public String GetCliente()
        {
            try
            {

                OrdenCompraCliente occ = this.OrdenCompraClienteSession;
                int idCliente = int.Parse(Request["idCliente"].ToString());
                ClienteBL clienteBl = new ClienteBL();
                occ.clienteSunat = clienteBl.getClienteSunat(idCliente);
                occ.sedesClienteSunat = clienteBl.getCiudadesCliente(idCliente);

                

                this.evaluarCiudadSedeOCC(occ);

                this.OrdenCompraClienteSession = occ;
                String resultado = JsonConvert.SerializeObject(occ.clienteSunat);
                return resultado;
            }
            catch (Exception e)
            {
                logger.Error(e, agregarUsuarioAlMensaje(e.Message));
                throw e;
            }
        }

        #endregion



        #region AGREGAR PRODUCTO


        public String GetProducto()
        {
            try
            {
                OrdenCompraCliente occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE];
                //Se recupera el producto y se guarda en Session

                Guid idProducto = Guid.Parse(this.Request.Params["idProducto"].ToString());
                //Guid idProducto = Guid.Parse(Request["idProducto"].ToString());
                this.Session["idProducto"] = idProducto.ToString();

                //Para recuperar el producto se envia si la sede seleccionada es provincia o no
                ProductoBL bl = new ProductoBL();
                Producto producto = bl.getProducto(idProducto, occ.ciudad.esProvincia, occ.incluidoIGV, occ.cliente.idCliente);

                Decimal fleteDetalle = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, producto.costoLista * (0) / 100));
                Decimal precioUnitario = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, fleteDetalle + producto.precioLista));

                //Se calcula el porcentaje de descuento
                Decimal porcentajeDescuento = 0;
                if (producto.precioClienteProducto.idPrecioClienteProducto != Guid.Empty)
                {
                    fleteDetalle = producto.precioClienteProducto.flete;
                    //Solo en caso de que el precioNetoEquivalente sea distinto a 0 se calcula el porcentaje de descuento
                    //si no se obtiene precioNetoEquivalente quiere decir que no hay precioRegistrado
                    if (producto.precioLista == 0)
                        porcentajeDescuento = 100;
                    else
                        porcentajeDescuento = 100 - (producto.precioClienteProducto.precioNeto * 100 / producto.precioLista);
                }

                String jsonPrecioLista = JsonConvert.SerializeObject(producto.precioListaList);
                String jsonProductoPresentacion = JsonConvert.SerializeObject(producto.ProductoPresentacionList);





                String resultado = "{" +
                    "\"id\":\"" + producto.idProducto + "\"," +
                    "\"nombre\":\"" + producto.descripcion + "\"," +
                    "\"image\":\"data:image/png;base64, " + Convert.ToBase64String(producto.image) + "\"," +
                    "\"unidad\":\"" + producto.unidad + "\"," +
                    "\"unidad_alternativa\":\"" + producto.unidad_alternativa + "\"," +
                    "\"proveedor\":\"" + producto.proveedor + "\"," +
                    "\"familia\":\"" + producto.familia + "\"," +
                    "\"precioUnitarioSinIGV\":\"" + producto.precioSinIgv + "\"," +
                    //       "\"precioUnitarioAlternativoSinIGV\":\"" + producto.precioAlternativoSinIgv + "\"," +
                    "\"precioLista\":\"" + producto.precioLista + "\"," +
                    "\"costoSinIGV\":\"" + producto.costoSinIgv + "\"," +
                    //       "\"costoAlternativoSinIGV\":\"" + producto.costoAlternativoSinIgv + "\"," +
                    "\"fleteDetalle\":\"" + fleteDetalle + "\"," +
                    "\"precioUnitario\":\"" + precioUnitario + "\"," +
                    "\"porcentajeDescuento\":\"" + porcentajeDescuento + "\"," +
                    "\"descontinuado\":\"" + producto.descontinuado.ToString() + "\"," +
                    "\"motivoRestriccion\":\"" + producto.motivoRestriccion + "\"," +
                    "\"precioListaList\":" + jsonPrecioLista + "," +
                    "\"productoPresentacionList\":" + jsonProductoPresentacion + "," +
                    "\"costoLista\":\"" + producto.costoLista + "\"" +
                    "}";
                return resultado;
            }
            catch (Exception e)
            {
                logger.Error(e, agregarUsuarioAlMensaje(e.Message));
                throw e;
            }

        }


        public String AddProducto()
        {
            try
            {
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                OrdenCompraCliente occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE];
                Guid idProducto = Guid.Parse(this.Session["idProducto"].ToString());
                OrdenCompraClienteDetalle occDetalle = occ.detalleList.Where(p => p.producto.idProducto == idProducto).FirstOrDefault();
                if (occDetalle != null)
                {
                    String mensajeError = "Producto ya se encuentra en la lista.";
                    logger.Error(agregarUsuarioAlMensaje(mensajeError));
                    throw new System.Exception(mensajeError);
                }

                OrdenCompraClienteDetalle detalle = new OrdenCompraClienteDetalle(occ.usuario.visualizaCostos, occ.usuario.visualizaMargen);
                ProductoBL productoBL = new ProductoBL();
                Producto producto = productoBL.getProducto(idProducto, occ.ciudad.esProvincia, occ.incluidoIGV, occ.cliente.idCliente);

                if (occ.claseOrdenCompraCliente == OrdenCompraCliente.ClasesOrdenCompraCliente.Venta)
                {
                    String mensajeError = "Tipo de Producto no concuerda con el tipo de OrdenCompraCliente.";
                    if ((occ.tipoOrdenCompraCliente == OrdenCompraCliente.tiposOrdenCompraCliente.Venta || occ.tipoOrdenCompraCliente == OrdenCompraCliente.tiposOrdenCompraCliente.TransferenciaGratuitaEntregada) && producto.tipoProducto == Producto.TipoProducto.Comodato)
                    {
                        logger.Error(agregarUsuarioAlMensaje(mensajeError));
                        throw new System.Exception(mensajeError);
                    }
                    if (occ.tipoOrdenCompraCliente == OrdenCompraCliente.tiposOrdenCompraCliente.ComodatoEntregado && producto.tipoProducto != Producto.TipoProducto.Comodato)
                    {
                        logger.Error(agregarUsuarioAlMensaje(mensajeError));
                        throw new System.Exception(mensajeError);
                    }
                }


                detalle.producto = producto;

                detalle.cantidad = Int32.Parse(Request["cantidad"].ToString());
                detalle.porcentajeDescuento = Decimal.Parse(Request["porcentajeDescuento"].ToString());
                detalle.esPrecioAlternativo = Int16.Parse(Request["esPrecioAlternativo"].ToString()) == 1;

                int idProductoPresentacion = Int16.Parse(Request["idProductoPresentacion"].ToString());

                detalle.observacion = Request["observacion"].ToString();
                decimal precioNeto = Decimal.Parse(Request["precio"].ToString());
                decimal costo = Decimal.Parse(Request["costo"].ToString());
                decimal flete = Decimal.Parse(Request["flete"].ToString());


                detalle.unidad = detalle.producto.unidad;

                if (detalle.esPrecioAlternativo)
                {
                    ProductoPresentacion productoPresentacion = producto.getProductoPresentacion(idProductoPresentacion);
                    detalle.unidad = productoPresentacion.Presentacion;
                    detalle.ProductoPresentacion = productoPresentacion;
                    //Si es el precio Alternativo se multiplica por la equivalencia para que se registre el precio estandar
                    //dado que cuando se hace get al precioNetoEquivalente se recupera diviendo entre la equivalencia
                    detalle.precioNeto = Decimal.Parse(String.Format(Constantes.formatoCuatroDecimales, precioNeto * detalle.ProductoPresentacion.Equivalencia));

                    //Si es el precio Alternativo se debe modificar el precio_cliente_producto para que compare con el precio
                    //de la unidad alternativa en lugar del precio de la unidad estandar
                    //detalle.producto.precioClienteProducto.precioUnitario =
                    //  detalle.producto.precioClienteProducto.precioUnitario / producto.equivalencia;
                    if (detalle.producto.precioClienteProducto.idPrecioClienteProducto != Guid.Empty)
                    {
                        detalle.producto.precioClienteProducto.precioUnitario =
                       detalle.producto.precioClienteProducto.precioUnitario / detalle.ProductoPresentacion.Equivalencia;
                    }

                }
                else
                {
                    detalle.precioNeto = precioNeto;
                }
                detalle.flete = flete;
                occ.detalleList.Add(detalle);

                //CotizacionDetalle cotizacionDetalle = (CotizacionDetalle)Convert.ChangeType(occ, typeof(CotizacionDetalle));

                //Calcula los montos totales de la cabecera de la cotizacion
                OrdenCompraClienteBL occBL = new OrdenCompraClienteBL();
                occBL.calcularMontosTotales(occ);




                var nombreProducto = detalle.producto.descripcion;
               /* if (occ.mostrarCodigoProveedor)
                {*/
                    nombreProducto = detalle.producto.skuProveedor + " - " + detalle.producto.descripcion;
                //    }

                /*  if (occ.considerarCantidades == Cotizacion.OpcionesConsiderarCantidades.Ambos)
                  {*/
                //      nombreProducto = nombreProducto + "\\n" + detalle.observacion;
                //}

                /* String resultado = "{" +
                     "\"idProducto\":\"" + detalle.producto.idProducto + "\"," +
                     "\"codigoProducto\":\"" + detalle.producto.sku + "\"," +
                     "\"nombreProducto\":\"" + nombreProducto + "\"," +
                     "\"unidad\":\"" + detalle.unidad + "\"," +
                     "\"igv\":\"" + occ.montoIGV.ToString() + "\", " +
                     "\"subTotal\":\"" + occ.montoSubTotal.ToString() + "\", " +
                     "\"margen\":\"" + detalle.margen + "\", " +
                     "\"precioUnitario\":\"" + detalle.precioUnitario + "\", " +
                     "\"observacion\":\"" + detalle.observacion + "\", " +
                     "\"total\":\"" + occ.montoTotal.ToString() + "\"}";*/

                Decimal precioUnitarioRegistrado = detalle.producto.precioClienteProducto.precioUnitario;
            /*    if (precioUnitarioRegistrado == 0)
                {
                    precioUnitarioRegistrado = detalle.producto.precioLista;
                }*/


                var v = new
                {
                    idProducto = detalle.producto.idProducto,
                    codigoProducto = detalle.producto.sku,
                    nombreProducto = nombreProducto,
                    unidad = detalle.unidad,
                    igv = occ.montoIGV.ToString(),
                    subTotal = occ.montoSubTotal.ToString(),
                    margen = detalle.margen,
                    precioUnitario = detalle.precioUnitario,
                    descontinuado = detalle.producto.descontinuado,
                    motivoRestriccion = detalle.producto.motivoRestriccion,
                    observacion = detalle.observacion,
                    total = occ.montoTotal.ToString(),
                    precioUnitarioRegistrado = precioUnitarioRegistrado

                };

                this.OrdenCompraClienteSession = occ;
                String resultado = JsonConvert.SerializeObject(v);
                return resultado;
            }
            catch (Exception e)
            {
                logger.Error(e, agregarUsuarioAlMensaje(e.Message));
                throw e;
            }

        }

        #endregion





        /*Actualización de Campos*/
        #region ACTUALIZACION DE CAMPOS FORMULARIO

        public void ChangeInputString()
        {
            OrdenCompraCliente occ = this.OrdenCompraClienteSession;
            PropertyInfo propertyInfo = occ.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(occ, this.Request.Params["valor"]);
            this.OrdenCompraClienteSession = occ;
        }

        public void ChangeUbigeoEntrega()
        {
            OrdenCompraCliente occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE];
            occ.ubigeoEntrega.Id = this.Request.Params["ubigeoEntregaId"];
            this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE] = occ;
        }
      

        public void ChangeNumeroReferenciaCliente()
        {
            OrdenCompraCliente occ = this.OrdenCompraClienteSession;
            occ.numeroReferenciaCliente = this.Request.Params["numeroReferenciaCliente"];
            this.OrdenCompraClienteSession = occ;
        }

        public void ChangeOtrosCargos()
        {
            OrdenCompraCliente occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE];
            occ.otrosCargos = Decimal.Parse(this.Request.Params["otrosCargos"]);
            this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE] = occ;
        }
        

        public string ChangeDireccionEntrega()
        {
            OrdenCompraCliente occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE];

            if (this.Request.Params["idDireccionEntrega"] == null || this.Request.Params["idDireccionEntrega"].Equals(String.Empty))
            {
                occ.direccionEntrega = new DireccionEntrega();
            }
            else
            { 
                Guid idDireccionEntrega = Guid.Parse(this.Request.Params["idDireccionEntrega"]);
                occ.direccionEntrega = occ.cliente.direccionEntregaList.Where(d => d.idDireccionEntrega == idDireccionEntrega).FirstOrDefault();
                occ.ubigeoEntrega = occ.direccionEntrega.ubigeo;

            }

            occ.existeCambioDireccionEntrega = false;
            this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE] = occ;
            return JsonConvert.SerializeObject(occ.direccionEntrega);
        }

        public void ChangeDireccionEntregaDescripcion()
        {
            OrdenCompraCliente occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE];
            occ.direccionEntrega.descripcion = this.Request.Params["direccionEntregaDescripcion"];
            occ.existeCambioDireccionEntrega = true;
            this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE] = occ;
        }

        public void ChangeDireccionEntregaContacto()
        {
            OrdenCompraCliente occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE];
            occ.direccionEntrega.contacto = this.Request.Params["direccionEntregaContacto"];
            occ.existeCambioDireccionEntrega = true;
            this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE] = occ;
        }

        public void ChangeDireccionEntregaTelefono()
        {
            OrdenCompraCliente occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE];
            occ.direccionEntrega.telefono = this.Request.Params["direccionEntregaTelefono"];
            occ.existeCambioDireccionEntrega = true;
            this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE] = occ;
        }



        public string ChangeSolicitante()
        {
            OrdenCompraCliente occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE];

            if (this.Request.Params["idSolicitante"] == null || this.Request.Params["idSolicitante"].Equals(String.Empty))
            {
                occ.solicitante = new Solicitante();
            }
            else
            {
                Guid idSolicitante = Guid.Parse(this.Request.Params["idSolicitante"]);
                occ.solicitante = occ.clienteSunat.solicitanteList.Where(d => d.idSolicitante == idSolicitante).FirstOrDefault();
            }

            occ.existeCambioSolicitante = false;
            this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE] = occ;
            return JsonConvert.SerializeObject(occ.solicitante);
        }

        public void ChangeSolicitanteNombre()
        {
            OrdenCompraCliente occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE];
            occ.solicitante.nombre = this.Request.Params["solicitanteNombre"];
            occ.existeCambioSolicitante = true;
            this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE] = occ;
        }

        public void ChangeSolicitanteTelefono()
        {
            OrdenCompraCliente occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE];
            occ.solicitante.telefono = this.Request.Params["solicitanteTelefono"];
            occ.existeCambioSolicitante = true;
            this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE] = occ;
        }

        public void ChangeSolicitanteCorreo()
        {
            OrdenCompraCliente occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE];
            occ.solicitante.correo = this.Request.Params["solicitanteCorreo"];
            occ.existeCambioSolicitante = true;
            this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE] = occ;
        }


        public void ChangeFechaSolicitud()
        {
            OrdenCompraCliente occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE];
            String[] fechaSolicitud = this.Request.Params["fechaSolicitud"].Split('/');
            occ.fechaSolicitud = new DateTime(Int32.Parse(fechaSolicitud[2]), Int32.Parse(fechaSolicitud[1]), Int32.Parse(fechaSolicitud[0]), 0, 0, 0);
            this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE] = occ;
        }

        public void ChangeFechaEntregaDesde()
        {
            OrdenCompraCliente occ = this.OrdenCompraClienteSession;
            String[] ftmp = this.Request.Params["fechaEntregaDesde"].Split('/');
            occ.fechaEntregaDesde = new DateTime(Int32.Parse(ftmp[2]), Int32.Parse(ftmp[1]), Int32.Parse(ftmp[0]));
            this.OrdenCompraClienteSession = occ;
        }

        public void ChangeFechaEntregaHasta()
        {
            OrdenCompraCliente occ = this.OrdenCompraClienteSession;
            String[] ftmp = this.Request.Params["fechaEntregaHasta"].Split('/');
            occ.fechaEntregaHasta = new DateTime(Int32.Parse(ftmp[2]), Int32.Parse(ftmp[1]), Int32.Parse(ftmp[0]));
            this.OrdenCompraClienteSession = occ;
        }

        public void ChangeContactoOrdenCompraCliente()
        {
            OrdenCompraCliente occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE];
            occ.contactoOrdenCompraCliente = this.Request.Params["contactoOrdenCompraCliente"];
            this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE] = occ;
        }

        public void ChangeTelefonoContactoOrdenCompraCliente()
        {
            OrdenCompraCliente occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE];
            occ.telefonoContactoOrdenCompraCliente = this.Request.Params["telefonoContactoOrdenCompraCliente"];
            this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE] = occ;
        }


        public void ChangeObservaciones()
        {
            OrdenCompraCliente occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE];

            occ.GetType().GetProperty("observaciones").SetValue(occ, this.Request.Params["observaciones"]);

            this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE] = occ;
        }


        public void ChangeFechaSolicitudDesde()
        {
            OrdenCompraCliente occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE_BUSQUEDA];
            String[] solDesde = this.Request.Params["fechaSolicitudDesde"].Split('/');
            occ.fechaSolicitudDesde = new DateTime(Int32.Parse(solDesde[2]), Int32.Parse(solDesde[1]), Int32.Parse(solDesde[0]));
            this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE_BUSQUEDA] = occ;
        }

        public void ChangeFechaSolicitudHasta()
        {
            OrdenCompraCliente occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE_BUSQUEDA];
            String[] solHasta = this.Request.Params["fechaSolicitudHasta"].Split('/');
            occ.fechaSolicitudHasta = new DateTime(Int32.Parse(solHasta[2]), Int32.Parse(solHasta[1]), Int32.Parse(solHasta[0]), 23, 59, 59);
            this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE_BUSQUEDA] = occ;
        }

        public void ChangeNumero()
        {
            OrdenCompraCliente occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE_BUSQUEDA];
            if (this.Request.Params["numero"] == null || this.Request.Params["numero"].Trim().Length == 0)
            {
                occ.numeroOrdenCompraCliente = 0;
            }
            else
            {
                occ.numeroOrdenCompraCliente = long.Parse(this.Request.Params["numero"]);
            }
            this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE_BUSQUEDA] = occ;
        }

       

        public void ChangeIdGrupoCliente()
        {
            OrdenCompraCliente occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE_BUSQUEDA];
            if (this.Request.Params["idGrupoCliente"] == null || this.Request.Params["idGrupoCliente"].Trim().Length == 0)
            {
                occ.idGrupoCliente = 0;
            }
            else
            {
                occ.idGrupoCliente = int.Parse(this.Request.Params["idGrupoCliente"]);
            }
            this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE_BUSQUEDA] = occ;
        }

        public void ChangeBuscarSedesGrupoCliente()
        {
            OrdenCompraCliente occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE_BUSQUEDA];
            if (this.Request.Params["buscarSedesGrupoCliente"] != null && Int32.Parse(this.Request.Params["buscarSedesGrupoCliente"]) == 1)
            {
                occ.buscarSedesGrupoCliente = true;
            }
            else
            {
                occ.buscarSedesGrupoCliente = false;
            }
            this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE_BUSQUEDA] = occ;
        }

    

        public String ChangeIdCiudad()
        {
            OrdenCompraCliente occ = this.OrdenCompraClienteSession;
            
            Guid idCiudad = Guid.Empty;
            if (this.Request.Params["idCiudad"] != null && !this.Request.Params["idCiudad"].Equals(""))
            {
                idCiudad = Guid.Parse(this.Request.Params["idCiudad"]);
            }
            //Para realizar el cambio de ciudad ningun producto debe estar agregado
            if (occ.detalleList != null && occ.detalleList.Count > 0)
            {
                // throw new Exception("No se puede cambiar de ciudad");
                return "No se puede cambiar de ciudad";
            }
            else 
            {
                CiudadBL ciudadBL = new CiudadBL();
                Ciudad ciudadNueva = ciudadBL.getCiudad(idCiudad);
                occ.ciudad = ciudadNueva;

                evaluarCiudadSedeOCC(occ);

                this.OrdenCompraClienteSession = occ;
                return "{\"idCiudad\": \"" + idCiudad + "\"}";
            }
        }

        private void evaluarCiudadSedeOCC(OrdenCompraCliente occ)
        {
            SolicitanteBL solicitanteBL = new SolicitanteBL();
            occ.clienteSunat.solicitanteList = new List<Solicitante>();

            occ.cliente.idCliente = Guid.Empty;
            foreach (Ciudad sede in occ.sedesClienteSunat)
            {
                if (occ.ciudad.idCiudad == sede.idCiudad)
                {
                    occ.cliente.idCliente = sede.idClienteRelacionado;
                }
            }

            if (occ.cliente.idCliente != Guid.Empty)
            {
                occ.clienteSunat.solicitanteList = solicitanteBL.getSolicitantes(occ.cliente.idCliente);
            }
        }

        public void ChangeTipoOrdenCompraCliente()
        {
            Char tipoOrdenCompraCliente = Convert.ToChar(Int32.Parse(this.Request.Params["tipoOrdenCompraCliente"]));
            this.OrdenCompraClienteSession.tipoOrdenCompraCliente = (OrdenCompraCliente.tiposOrdenCompraCliente)tipoOrdenCompraCliente;
        }

        public void ChangeTipoOrdenCompraClienteBusqueda()
        {
            Char tipoOrdenCompraClienteBusqueda = Convert.ToChar(Int32.Parse(this.Request.Params["tipoOrdenCompraClienteBusqueda"]));
            this.OrdenCompraClienteSession.tipoOrdenCompraClienteVentaBusqueda = (OrdenCompraCliente.tiposOrdenCompraClienteVentaBusqueda)tipoOrdenCompraClienteBusqueda;
        }


        public String ChangeIdCiudadASolicitar()
        {
            OrdenCompraCliente occ = this.OrdenCompraClienteSession;
            Guid idCiudad = Guid.Empty;
            if (this.Request.Params["idCiudadASolicitar"] != null && !this.Request.Params["idCiudadASolicitar"].Equals(""))
            {
                idCiudad = Guid.Parse(this.Request.Params["idCiudadASolicitar"]);
            }
            //Para realizar el cambio de ciudad ningun producto debe estar agregado
        /*    if (occ.occDetalleList != null && occ.occDetalleList.Count > 0)
            {
                // throw new Exception("No se puede cambiar de ciudad");
                return "No se puede cambiar de ciudad";
            }
            else
            {*/
            CiudadBL ciudadBL = new CiudadBL();
            Ciudad ciudadASolicitar = ciudadBL.getCiudad(idCiudad);
            occ.ciudadASolicitar = ciudadASolicitar;
            this.OrdenCompraClienteSession = occ;
            return "{\"idCiudad\": \"" + idCiudad + "\"}";
          //  }
        }

        [HttpPost]
        public String ChangeDetalle(List<DocumentoDetalleJson> cotizacionDetalleJsonList)
        {
            IDocumento documento = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE];
            List<DocumentoDetalle> documentoDetalle = HelperDocumento.updateDocumentoDetalle(documento, cotizacionDetalleJsonList);
            documento.documentoDetalle = documentoDetalle;
            OrdenCompraClienteBL occBL = new OrdenCompraClienteBL();
            occBL.calcularMontosTotales((OrdenCompraCliente)documento);
            this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE] = documento;
            return "{\"cantidad\":\"" + documento.documentoDetalle.Count + "\"}";
        }


        public String ChangeEsPagoContado()
        {
            OrdenCompraCliente occ = this.OrdenCompraClienteSession;
            try
            {
                occ.esPagoContado = Int32.Parse(this.Request.Params["esPagoContado"]) == 1;
            }
            catch (Exception ex)
            {
            }
            this.OrdenCompraClienteSession = occ;

            return "{\"textoCondicionesPago\":\"" + occ.textoCondicionesPago + "\"}";
        }


        #endregion



        #region CREAR/ACTUALIZAR PEDIDO

        
        #region carga de imagenes

        public void ChangeFiles(List<HttpPostedFileBase> files)
        {
           
            OrdenCompraCliente occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE];

            if((int)this.Session[Constantes.VAR_SESSION_PAGINA] == (int)Constantes.paginas.BusquedaOrdenCompraClientes )
                occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE_VER];


            foreach (var file in files)
            {
                if (file != null && file.ContentLength > 0)
                {
                    if (occ.AdjuntoList.Where(p => p.nombre.Equals(file.FileName) ).FirstOrDefault() != null)
                    {
                        continue;
                    }
                    

                    ArchivoAdjunto occAdjunto = new ArchivoAdjunto();
                    using (Stream inputStream = file.InputStream)
                    {
                        MemoryStream memoryStream = inputStream as MemoryStream;
                        if (memoryStream == null)
                        {
                            memoryStream = new MemoryStream();
                            inputStream.CopyTo(memoryStream);
                        }
                        occAdjunto.nombre = file.FileName;
                        occAdjunto.adjunto = memoryStream.ToArray();
                    }
                    occ.AdjuntoList.Add(occAdjunto);
                }
            }

        }


        public String DescartarArchivos()
        {
            OrdenCompraCliente occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE];
            if ((int)this.Session[Constantes.VAR_SESSION_PAGINA] == (int)Constantes.paginas.BusquedaOrdenCompraClientes)
                occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE_VER];


            String nombreArchivo = Request["nombreArchivo"].ToString();

            List<ArchivoAdjunto> occAdjuntoList = new List<ArchivoAdjunto>();
            foreach (ArchivoAdjunto occAdjunto in occ.AdjuntoList )
            {
                if(!occAdjunto.nombre.Equals(nombreArchivo))
                    occAdjuntoList.Add(occAdjunto);
            }

            occ.AdjuntoList = occAdjuntoList;

            return JsonConvert.SerializeObject(occ.AdjuntoList);
        }

        public String Descargar()
        {
            String nombreArchivo = Request["nombreArchivo"].ToString();
            OrdenCompraCliente occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE];

            if ((int)this.Session[Constantes.VAR_SESSION_PAGINA] == (int)Constantes.paginas.BusquedaOrdenCompraClientes)
            {
                occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE_VER];

            }

            ArchivoAdjunto archivoAdjunto  = occ.AdjuntoList.Where(p => p.nombre.Equals(nombreArchivo)).FirstOrDefault();
            
            if (archivoAdjunto != null)
            {
                ArchivoAdjuntoBL archivoAdjuntoBL = new ArchivoAdjuntoBL();
                archivoAdjunto = archivoAdjuntoBL.GetArchivoAdjunto(archivoAdjunto);         
                return JsonConvert.SerializeObject(archivoAdjunto);
            }
            else
            {
                return null;
            }
            
        }

        #endregion



        public String Create()
        {

            //RUC_MP
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            int continuarLuego = int.Parse(Request["continuarLuego"].ToString());
            OrdenCompraCliente occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE];
            occ.usuario = usuario;
            OrdenCompraClienteBL occBL = new OrdenCompraClienteBL();

            if (occ.idOrdenCompraCliente != Guid.Empty || occ.numeroOrdenCompraCliente > 0)
            {
                throw new System.Exception("OrdenCompraCliente ya se encuentra creado");
            }

            occBL.InsertOrdenCompraCliente(occ);
            long numeroOrdenCompraCliente = occ.numeroOrdenCompraCliente;
            String numeroOrdenCompraClienteString = occ.numeroOrdenCompraClienteString;
            Guid idOrdenCompraCliente = occ.idOrdenCompraCliente;

            HttpSessionStateBase sessionParams = this.Session;
            var arcBL = new ArchivoAdjuntoBL();
            arcBL.asociarAchivoRegistro(sessionParams, occ.idOrdenCompraCliente, ArchivoAdjunto.ArchivoAdjuntoOrigen.OC_OCCLIENTE);

            // occ = null;
            this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE] = null;// occ;// null;



            var v = new { numeroOrdenCompraCliente = numeroOrdenCompraClienteString, idOrdenCompraCliente = idOrdenCompraCliente };
            String resultado = JsonConvert.SerializeObject(v);

           // String resultado = "{ \"codigo\":\"" + numeroOrdenCompraCliente + "\", \"estado\":\"" + estado + "\", \"observacion\":\"" + observacion + "\" }";
            return resultado;
        }





        public String Update()
        {
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            int continuarLuego = int.Parse(Request["continuarLuego"].ToString());
            OrdenCompraCliente occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE];
            occ.usuario = usuario;
            OrdenCompraClienteBL bl = new OrdenCompraClienteBL();
            bl.UpdateOrdenCompraCliente(occ);
            long numeroOrdenCompraCliente = occ.numeroOrdenCompraCliente;
            String numeroOrdenCompraClienteString = occ.numeroOrdenCompraClienteString;
            Guid idOrdenCompraCliente = occ.idOrdenCompraCliente;

            // occ = null;
            this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE] = null;// occ;

            var v = new { numeroOrdenCompraCliente = numeroOrdenCompraClienteString, idOrdenCompraCliente = idOrdenCompraCliente };
            String resultado = JsonConvert.SerializeObject(v);
            
            //String resultado = "{ \"codigo\":\"" + numeroOrdenCompraCliente + "\", \"estado\":\"" + estado + "\", \"observacion\":\"" + observacion + "\" }";
            return resultado;
        }


        #endregion

        public void CleanBusqueda()
        {
            instanciarOrdenCompraClienteBusqueda();
            //Se retorna la cantidad de elementos encontrados
            //List<OrdenCompraCliente> cotizacionList = (List<Cotizacion>)this.Session[Constantes.VAR_SESSION_COTIZACION_LISTA];
            //return cotizacionList.Count();
        }

        /*
        [HttpGet]
        public ActionResult ExportLastSearchExcel()
        {
            List<OrdenCompraCliente> list = (List<OrdenCompraCliente>) this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE_LISTA];

            //OrdenCompraClienteSearch excel = new OrdenCompraClienteSearch();
           return excel.generateExcel(list);
        }
        */

        public String Search()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaOrdenCompraClientes;
            //Se recupera el occ Búsqueda de la session
            OrdenCompraCliente occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE_BUSQUEDA];

            String[] solDesde = this.Request.Params["fechaCreacionDesde"].Split('/');
            occ.fechaCreacionDesde = new DateTime(Int32.Parse(solDesde[2]), Int32.Parse(solDesde[1]), Int32.Parse(solDesde[0]));

            String[] solHasta = this.Request.Params["fechaCreacionHasta"].Split('/');
            occ.fechaCreacionHasta = new DateTime(Int32.Parse(solHasta[2]), Int32.Parse(solHasta[1]), Int32.Parse(solHasta[0]),23,59,59);



            if (this.Request.Params["numero"] == null || this.Request.Params["numero"].Trim().Length == 0)
            {
                occ.numeroOrdenCompraCliente = 0;
            }
            else
            {
                occ.numeroOrdenCompraCliente = long.Parse(this.Request.Params["numero"]);
            }


            OrdenCompraClienteBL occBL = new OrdenCompraClienteBL();
            List<OrdenCompraCliente> occList = occBL.GetOrdenCompraClientes(occ);
            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE_LISTA] = occList;
            this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE_BUSQUEDA] = occ;

            String occListString = JsonConvert.SerializeObject(ParserDTOsSearch.OrdenCompraClienteToOrdenCompraClienteVentaDTO(occList));
            return occListString;
            //return occList.Count();
        }


        public String ConsultarSiExisteOrdenCompraCliente()
        {
            OrdenCompraCliente occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE];
            if (occ == null)
                return "{\"existe\":\"false\",\"numero\":\"0\"}";
            else
                return "{\"existe\":\"true\",\"numero\":\"" + occ.numeroOrdenCompraCliente + "\"}";
        }

        public String Show()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            OrdenCompraClienteBL occBL = new OrdenCompraClienteBL();
            ProductoBL productoBL = new ProductoBL();

            OrdenCompraCliente occ = new OrdenCompraCliente();
            occ.idOrdenCompraCliente = Guid.Parse(Request["idOrdenCompraCliente"].ToString());
            occ = occBL.GetOrdenCompraCliente(occ,usuario);
            this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE_VER] = occ;
            
            string jsonUsuario = JsonConvert.SerializeObject(usuario);


            Pedido pedidoGenerar = new Pedido(Pedido.ClasesPedido.Venta);

            pedidoGenerar.ordenCompracliente = occ;
            pedidoGenerar.pedidoDetalleList = new List<PedidoDetalle>();
            pedidoGenerar.numeroReferenciaCliente = occ.numeroReferenciaCliente;
            foreach (OrdenCompraClienteDetalle det in occ.detalleList)
            {
                det.producto = productoBL.getProducto(det.producto.idProducto, occ.ciudad.esProvincia, occ.incluidoIGV, occ.ciudad.idClienteRelacionado);

                DocumentoDetalle docdet = det;
                PedidoDetalle pedDet =  JsonConvert.DeserializeObject<PedidoDetalle>(JsonConvert.SerializeObject(docdet));

                pedDet.cantidad = 0;

                pedDet.precioNeto = det.precioNeto;
                pedDet.precioUnitarioVenta = det.precioUnitarioVenta;
                pedDet.precioUnitarioOriginal = det.precioUnitarioOriginal;
                pedDet.visualizaCostos = det.visualizaCostos;
                pedDet.visualizaMargen = det.visualizaMargen;

                pedidoGenerar.pedidoDetalleList.Add(pedDet);

                
            }

            this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE_PEDIDO_GENERAR] = pedidoGenerar;

            string jsonOrdenCompraCliente = JsonConvert.SerializeObject(occ);

            String json = "{\"occ\":" + jsonOrdenCompraCliente + ", \"usuario\":" + jsonUsuario + "}";
            return json;
        }

        public void autoGuardarOrdenCompraCliente()
        {
            if (this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE] != null)
            {
                OrdenCompraCliente occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE];
                UsuarioBL usuarioBL = new UsuarioBL();
                Usuario usuario = (Usuario)this.Session["usuario"];

                String occSerializado = JsonConvert.SerializeObject(occ);

            }

        }

        public void updateUsuario()
        {
            OrdenCompraCliente occ = this.OrdenCompraClienteSession;
            occ.usuarioBusqueda = new Usuario { idUsuario = Guid.Parse(this.Request.Params["usuario"]) };
            this.OrdenCompraClienteSession = occ;
        }

        public String CreateDireccionTemporal()
        {
            OrdenCompraCliente occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE];
            DireccionEntrega direccionEntrega = new DireccionEntrega();
            direccionEntrega.descripcion = Request["direccion"];
            direccionEntrega.contacto = Request["contacto"];
            direccionEntrega.telefono = Request["telefono"];
            direccionEntrega.idDireccionEntrega = Guid.Empty;
            occ.cliente.direccionEntregaList.Add(direccionEntrega);
            occ.direccionEntrega = direccionEntrega;
            this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE] = occ;
            return JsonConvert.SerializeObject(direccionEntrega);
        }

        public String CreateSolicitanteTemporal()
        {
            OrdenCompraCliente occ = (OrdenCompraCliente)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE];
            Solicitante solicitante = new Solicitante();
            solicitante.nombre = Request["nombre"];
            solicitante.telefono = Request["telefono"];
            solicitante.correo = Request["correo"];
            solicitante.idSolicitante = Guid.Empty;
            occ.cliente.solicitanteList.Add(solicitante);
            occ.solicitante = solicitante;
            this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE] = occ;
            return JsonConvert.SerializeObject(solicitante);
        }



        public void SetDetalleGenerarPedido()
        {
            Guid idProducto = Guid.Parse(Request["idProducto"].ToString());
            int cantidad = int.Parse(Request["cantidad"].ToString());
            int idProductoPresentacion = int.Parse(Request["productoPresentacion"].ToString());
            String comentario = Request["comentario"].ToString();

            Pedido pedidoGenerar = (Pedido) this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE_PEDIDO_GENERAR];

            foreach (PedidoDetalle det in pedidoGenerar.pedidoDetalleList)
            {
                if (det.producto.idProducto == idProducto)
                {
                    
                    switch(idProductoPresentacion)
                    {
                        case 1: // Alternativa
                            det.ProductoPresentacion = det.producto.ProductoPresentacionList.Where(o => o.IdProductoPresentacion == idProductoPresentacion).FirstOrDefault();
                            det.esPrecioAlternativo = true;
                            det.unidad = det.producto.unidad_alternativa;
                            break;
                        case 2: // proveedor
                            det.ProductoPresentacion = det.producto.ProductoPresentacionList.Where(o => o.IdProductoPresentacion == idProductoPresentacion).FirstOrDefault();
                            det.esPrecioAlternativo = true;
                            det.unidad = det.producto.unidadProveedor;
                            break;
                        default:
                            det.esPrecioAlternativo = false;
                            det.ProductoPresentacion = null;
                            det.unidad = det.producto.unidad;
                            break;
                    }

                    det.cantidad = cantidad;
                    det.observacion = comentario;
                }
            }

            this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE_PEDIDO_GENERAR] = pedidoGenerar;
        }

        public void ChangeIdSedePedidoGenerar()
        {
            Guid idCliente = Guid.Parse(Request["idCliente"].ToString());

            Pedido pedidoGenerar = (Pedido)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE_PEDIDO_GENERAR];

            Cliente cliente = new Cliente();
            cliente.idCliente = idCliente;

            pedidoGenerar.cliente = cliente;

            this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE_PEDIDO_GENERAR] = pedidoGenerar;
        }

        public void changeMostrarCosto()
        {
            OrdenCompraCliente occ = this.OrdenCompraClienteSession;
            occ.mostrarCosto = Boolean.Parse(this.Request.Params["mostrarCosto"]);
            this.OrdenCompraClienteSession = occ;
        }
    }
}
 
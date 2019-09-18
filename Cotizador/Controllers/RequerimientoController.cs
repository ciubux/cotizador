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
    public class RequerimientoController : ParentController
    {

        private Requerimiento RequerimientoSession
        {
            get
            {

                Requerimiento requerimiento = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaRequerimientos: requerimiento = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_BUSQUEDA]; break;
                    case Constantes.paginas.MantenimientoRequerimientos: requerimiento = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO]; break;
                   // case Constantes.paginas.AprobarPedidos: requerimiento = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_APROBACION]; break;
                }
                return requerimiento;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaPedidos: this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_BUSQUEDA] = value; break;
                    case Constantes.paginas.MantenimientoPedido: this.Session[Constantes.VAR_SESSION_REQUERIMIENTO] = value; break;
                   // case Constantes.paginas.AprobarPedidos: this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_APROBACION] = value; break;
                }
            }
        }

        // GET: Pedido

        private void instanciarPedidoBusqueda()
        {
            try
            {
                Requerimiento requerimientoTmp = new Requerimiento(Requerimiento.ClasesRequerimiento.Venta);
                DateTime fechaDesde = DateTime.Now.AddDays(-Constantes.DIAS_DESDE_BUSQUEDA);
                DateTime fechaHasta = DateTime.Now.AddDays(1);
                requerimientoTmp.cotizacion = new Cotizacion();
                requerimientoTmp.solicitante = new Solicitante();
                requerimientoTmp.direccionEntrega = new DireccionEntrega();

                requerimientoTmp.fechaCreacionDesde = new DateTime(fechaDesde.Year, fechaDesde.Month, fechaDesde.Day, 0, 0, 0);
                requerimientoTmp.fechaCreacionHasta = fechaHasta;

                requerimientoTmp.fechaEntregaDesde = null;// new DateTime(fechaDesde.Year, fechaDesde.Month, fechaDesde.Day, 0, 0, 0);
                requerimientoTmp.fechaEntregaHasta = null;// DateTime.Now.AddDays(Constantes.DIAS_DESDE_BUSQUEDA);

                requerimientoTmp.fechaProgramacionDesde = null;// new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                requerimientoTmp.fechaProgramacionHasta = null;// new DateTime(fechaHasta.Year, fechaHasta.Month, fechaHasta.Day, 23, 59, 59);

                requerimientoTmp.buscarSedesGrupoCliente = false;

                requerimientoTmp.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                requerimientoTmp.ciudad = new Ciudad();
                requerimientoTmp.cliente = new Cliente();
           

                //Si es un coordinador cargarán por defecto todos los pedidos pendientes de atención
          
                requerimientoTmp.requerimientoDetalleList = new List<RequerimientoDetalle>();

                requerimientoTmp.usuarioBusqueda = new Usuario { idUsuario = Guid.Empty };

                this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_BUSQUEDA] = requerimientoTmp;
                this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_LISTA] = new List<Pedido>();
            }
            catch (Exception e)
            {
                logger.Error(e, agregarUsuarioAlMensaje(e.Message));
                throw e;
            }
        }


        private void instanciarRequerimientoBusquedaAprobacion()
        {
            try
            {
                Requerimiento requerimientoTmp = new Requerimiento(Requerimiento.ClasesRequerimiento.Venta);
                DateTime fechaDesde = DateTime.Now.AddDays(-Constantes.DIAS_DESDE_BUSQUEDA);
                DateTime fechaHasta = DateTime.Now.AddDays(1);
                requerimientoTmp.cotizacion = new Cotizacion();
                requerimientoTmp.solicitante = new Solicitante();
                requerimientoTmp.direccionEntrega = new DireccionEntrega();

                requerimientoTmp.fechaCreacionDesde = new DateTime(fechaDesde.Year, fechaDesde.Month, fechaDesde.Day, 0, 0, 0);
                requerimientoTmp.fechaCreacionHasta = fechaHasta;

                requerimientoTmp.fechaEntregaDesde = null;// new DateTime(fechaDesde.Year, fechaDesde.Month, fechaDesde.Day, 0, 0, 0);
                requerimientoTmp.fechaEntregaHasta = null;// DateTime.Now.AddDays(Constantes.DIAS_DESDE_BUSQUEDA);

                requerimientoTmp.fechaProgramacionDesde = null;// new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                requerimientoTmp.fechaProgramacionHasta = null;// new DateTime(fechaHasta.Year, fechaHasta.Month, fechaHasta.Day, 23, 59, 59);

                requerimientoTmp.buscarSedesGrupoCliente = false;

                requerimientoTmp.ciudad = new Ciudad();
                requerimientoTmp.cliente = new Cliente();
          
                requerimientoTmp.requerimientoDetalleList = new List<RequerimientoDetalle>();
                requerimientoTmp.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                requerimientoTmp.usuarioBusqueda = new Usuario { idUsuario = Guid.Empty };

                this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_BUSQUEDA] = requerimientoTmp;
                this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_LISTA] = new List<Requerimiento>();
            }
            catch (Exception e)
            {
                logger.Error(e, agregarUsuarioAlMensaje(e.Message));
                throw e;
            }
        }


        public ActionResult Index(Guid? idRequerimiento = null)
        {
            try
            {
                this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaPedidos;

                if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                    if (!usuario.tomaPedidos && !usuario.apruebaPedidos && !usuario.visualizaPedidos)
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }

                if (this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_BUSQUEDA] == null)
                {
                    instanciarPedidoBusqueda();
                }

                Requerimiento requerimientoSearch = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_BUSQUEDA];

                //Si existe cotizacion se debe verificar si no existe cliente
                if (this.Session[Constantes.VAR_SESSION_REQUERIMIENTO] != null)
                {
                    Requerimiento requerimientoEdicion = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO];
                    if (requerimientoEdicion.ciudad == null || requerimientoEdicion.ciudad.idCiudad == null
                        || requerimientoEdicion.ciudad.idCiudad == Guid.Empty)
                    {
                        this.Session[Constantes.VAR_SESSION_REQUERIMIENTO] = null;
                    }

                }

                ViewBag.fechaCreacionDesde = requerimientoSearch.fechaCreacionDesde.ToString(Constantes.formatoFecha);
                ViewBag.fechaCreacionHasta = requerimientoSearch.fechaCreacionHasta.ToString(Constantes.formatoFecha);

                if (requerimientoSearch.fechaEntregaDesde != null)
                    ViewBag.fechaEntregaDesde = requerimientoSearch.fechaEntregaDesde.Value.ToString(Constantes.formatoFecha);
                else
                    ViewBag.fechaEntregaDesde = null;

                if (requerimientoSearch.fechaEntregaHasta != null)
                    ViewBag.fechaEntregaHasta = requerimientoSearch.fechaEntregaHasta.Value.ToString(Constantes.formatoFecha);
                else
                    ViewBag.fechaEntregaHasta = null;


                if (requerimientoSearch.fechaProgramacionDesde != null)
                    ViewBag.fechaProgramacionDesde = requerimientoSearch.fechaProgramacionDesde.Value.ToString(Constantes.formatoFecha);
                else
                    ViewBag.fechaProgramacionDesde = null;

                if (requerimientoSearch.fechaProgramacionHasta != null)
                    ViewBag.fechaProgramacionHasta = requerimientoSearch.fechaProgramacionHasta.Value.ToString(Constantes.formatoFecha);
                else
                    ViewBag.fechaProgramacionHasta = null;




                if (this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_LISTA] == null)
                {
                    this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_LISTA] = new List<Pedido>();
                }


                int existeCliente = 0;
                //  if (cotizacion.cliente.idCliente != Guid.Empty || cotizacion.grupo.idGrupo != Guid.Empty)
                if (requerimientoSearch.cliente.idCliente != Guid.Empty)
                {
                    existeCliente = 1;
                }

                GuiaRemision guiaRemision = new GuiaRemision();
                guiaRemision.motivoTraslado = GuiaRemision.motivosTraslado.Venta;

                ViewBag.guiaRemision = guiaRemision;

                ViewBag.pedido = requerimientoSearch;
                DocumentoVenta documentoVenta = new DocumentoVenta();
                documentoVenta.tipoPago = DocumentoVenta.TipoPago.NoAsignado;
                documentoVenta.formaPago = DocumentoVenta.FormaPago.NoAsignado;
                documentoVenta.fechaEmision = DateTime.Now;
                ViewBag.documentoVenta = documentoVenta;
                ViewBag.fechaEmision = documentoVenta.fechaEmision.Value.ToString(Constantes.formatoFecha);
                ViewBag.horaEmision = documentoVenta.fechaEmision.Value.ToString(Constantes.formatoHora);
                ViewBag.pedidoList = this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_LISTA];
                ViewBag.existeCliente = existeCliente;

                ViewBag.pagina = (int)Constantes.paginas.BusquedaPedidos;

                ViewBag.idRequerimiento = idRequerimiento;
                return View();
            }
            catch (Exception e)
            {
                logger.Error(e, agregarUsuarioAlMensaje(e.Message));
                throw e;
            }
        }


        public ActionResult Aprobar(Guid? idRequerimiento = null)

        {
            try
            {
                this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaRequerimientos;
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];


                if (this.Session[Constantes.VAR_SESSION_USUARIO] == null && !usuario.tomaPedidos && !usuario.apruebaPedidos && !usuario.visualizaPedidos)
                {
                    return RedirectToAction("Login", "Account");
                }
                

                if (this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_BUSQUEDA] == null)
                {
                    instanciarRequerimientoBusquedaAprobacion();
                }


                Requerimiento requerimientoSearch = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_BUSQUEDA];

                //Si existe cotizacion se debe verificar si no existe cliente
                if (this.Session[Constantes.VAR_SESSION_REQUERIMIENTO] != null)
                {
                    Requerimiento requerimientoEdicion = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO];
                    if (requerimientoEdicion.ciudad == null || requerimientoEdicion.ciudad.idCiudad == null
                        || requerimientoEdicion.ciudad.idCiudad == Guid.Empty)
                    {
                        this.Session[Constantes.VAR_SESSION_REQUERIMIENTO] = null;
                    }                    
                }

                
                ViewBag.fechaCreacionDesde = requerimientoSearch.fechaCreacionDesde.ToString(Constantes.formatoFecha);
                ViewBag.fechaCreacionHasta = requerimientoSearch.fechaCreacionHasta.ToString(Constantes.formatoFecha);

                if (requerimientoSearch.fechaEntregaDesde != null)
                    ViewBag.fechaEntregaDesde = requerimientoSearch.fechaEntregaDesde.Value.ToString(Constantes.formatoFecha);
                else

                    

                    ViewBag.fechaEntregaDesde = null;


                if (requerimientoSearch.fechaEntregaHasta != null)
                    ViewBag.fechaEntregaHasta = requerimientoSearch.fechaEntregaHasta.Value.ToString(Constantes.formatoFecha);
                else
                    ViewBag.fechaEntregaHasta = null;
                

                if (requerimientoSearch.fechaProgramacionDesde != null)
                    ViewBag.fechaProgramacionDesde = requerimientoSearch.fechaProgramacionDesde.Value.ToString(Constantes.formatoFecha);
                else

                    

                    ViewBag.fechaProgramacionDesde = null;


                if (requerimientoSearch.fechaProgramacionHasta != null)
                    ViewBag.fechaProgramacionHasta = requerimientoSearch.fechaProgramacionHasta.Value.ToString(Constantes.formatoFecha);
                else
                    ViewBag.fechaProgramacionHasta = null;

                



                if (this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_LISTA] == null)
                {
                    this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_LISTA] = new List<Pedido>();
                }

                


                int existeCliente = 0;
                //  if (cotizacion.cliente.idCliente != Guid.Empty || cotizacion.grupo.idGrupo != Guid.Empty)
                if (requerimientoSearch.cliente.idCliente != Guid.Empty)
                {
                    existeCliente = 1;
                }


              
                        
               // requerimientoSearch.usuario = usuario;
                RequerimientoBL requerimientoBL = new RequerimientoBL();
                
                requerimientoSearch.fechaPrecios = DateTime.Now.AddDays(-Constantes.DIAS_MAX_BUSQUEDA_PRECIOS);
                String proveedor = "Todos";
                String familia = "Todas";

                //requerimiento.cliente.idCliente, requerimiento.fechaPrecios, requerimiento.ciudad.esProvincia,
                requerimientoSearch.cliente = usuario.clienteList.Where(c => c.sedePrincipal).FirstOrDefault();

                requerimientoSearch = requerimientoBL.obtenerProductosAPartirdePreciosRegistrados(requerimientoSearch, familia, proveedor, usuario);
                this.RequerimientoSession = requerimientoSearch;

                ViewBag.requerimiento = requerimientoSearch;
                DocumentoVenta documentoVenta = new DocumentoVenta();
                documentoVenta.tipoPago = DocumentoVenta.TipoPago.NoAsignado;
                documentoVenta.formaPago = DocumentoVenta.FormaPago.NoAsignado;
                documentoVenta.fechaEmision = DateTime.Now;
                ViewBag.documentoVenta = documentoVenta;
                ViewBag.fechaEmision = documentoVenta.fechaEmision.Value.ToString(Constantes.formatoFecha);
                ViewBag.horaEmision = documentoVenta.fechaEmision.Value.ToString(Constantes.formatoHora);
                ViewBag.requerimientoList = this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_LISTA];
                ViewBag.existeCliente = existeCliente;

                ViewBag.pagina = (int)Constantes.paginas.BusquedaPedidos;






                ViewBag.idRequerimiento = idRequerimiento;
                return View();
            }
            catch (Exception e)
            {
                logger.Error(e, agregarUsuarioAlMensaje(e.Message));
                throw e;
            }



        }






        public ActionResult CargarPedidos()
        {
            try
            {
                this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.MantenimientoPedido;

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

                if (this.Session[Constantes.VAR_SESSION_REQUERIMIENTO] == null)
                {

                    instanciarRequerimiento();
                }
                Requerimiento requerimiento = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO];


                int existeCliente = 0;
                if (requerimiento.cliente.idCliente != Guid.Empty)
                {
                    existeCliente = 1;
                }

                ViewBag.existeCliente = existeCliente;
                ViewBag.idClienteGrupo = requerimiento.cliente.idCliente;
                ViewBag.clienteGrupo = requerimiento.cliente.ToString();

                ViewBag.fechaSolicitud = requerimiento.fechaSolicitud.ToString(Constantes.formatoFecha);
                ViewBag.horaSolicitud = requerimiento.fechaSolicitud.ToString(Constantes.formatoHora);

                ViewBag.fechaEntregaDesde = requerimiento.fechaEntregaDesde == null ? "" : requerimiento.fechaEntregaDesde.Value.ToString(Constantes.formatoFecha);
                ViewBag.fechaEntregaHasta = requerimiento.fechaEntregaHasta == null ? "" : requerimiento.fechaEntregaHasta.Value.ToString(Constantes.formatoFecha);



                ViewBag.pedido = requerimiento;
                ViewBag.VARIACION_PRECIO_ITEM_REQUERIMIENTO = 0;// Constantes.VARIACION_PRECIO_ITEM_REQUERIMIENTO;

                ViewBag.fechaPrecios = DateTime.Now.AddDays(-Constantes.DIAS_MAX_BUSQUEDA_PRECIOS).ToString(Constantes.formatoFecha);
                //   ViewBag.fechaPrecios = requerimiento.fechaPrecios.ToString(Constantes.formatoFecha);

                ViewBag.pagina = (int)Constantes.paginas.MantenimientoPedido;
                return View();
            }
            catch (Exception e)
            {
                logger.Error(e, agregarUsuarioAlMensaje(e.Message));
                throw e;
            }
        }


        public ActionResult Pedir()
        {
            try
            {
                
                this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.MantenimientoRequerimientos;

                //Si no hay usuario, se dirige el logueo
                if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                    if (!usuario.tomaPedidos)
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }

                ViewBag.debug = Constantes.DEBUG;
                ViewBag.Si = Constantes.MENSAJE_SI;
                ViewBag.No = Constantes.MENSAJE_NO;
                ViewBag.IGV = Constantes.IGV;


                //Si no se está trabajando con una cotización se crea una y se agrega a la sesion

                if (this.Session[Constantes.VAR_SESSION_REQUERIMIENTO] == null)
                {

                    instanciarRequerimiento();
                }
                Requerimiento requerimiento = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO];


                int existeCliente = 0;
                if (requerimiento.cliente.idCliente != Guid.Empty)
                {
                    existeCliente = 1;
                }

                ViewBag.existeCliente = existeCliente;
                ViewBag.idClienteGrupo = requerimiento.cliente.idCliente;
                ViewBag.clienteGrupo = requerimiento.cliente.ToString();

                ViewBag.fechaSolicitud = requerimiento.fechaSolicitud.ToString(Constantes.formatoFecha);
                ViewBag.horaSolicitud = requerimiento.fechaSolicitud.ToString(Constantes.formatoHora);

                ViewBag.fechaEntregaDesde = requerimiento.fechaEntregaDesde == null ? "" : requerimiento.fechaEntregaDesde.Value.ToString(Constantes.formatoFecha);
                ViewBag.fechaEntregaHasta = requerimiento.fechaEntregaHasta == null ? "" : requerimiento.fechaEntregaHasta.Value.ToString(Constantes.formatoFecha);



                ViewBag.requerimiento = requerimiento;
                ViewBag.VARIACION_PRECIO_ITEM_REQUERIMIENTO = 0;// Constantes.VARIACION_PRECIO_ITEM_REQUERIMIENTO;

                ViewBag.fechaPrecios = DateTime.Now.AddDays(-Constantes.DIAS_MAX_BUSQUEDA_PRECIOS).ToString(Constantes.formatoFecha);

                ViewBag.pagina = (int)Constantes.paginas.MantenimientoPedido;
                return View();
            }
            catch (Exception e)
            {
                logger.Error(e, agregarUsuarioAlMensaje(e.Message));
                throw e;
            }
        }

        public void iniciarEdicionPedidoDesdeCotizacion()
        {
            try
            {
                if (this.Session[Constantes.VAR_SESSION_REQUERIMIENTO] == null)
                {

                    instanciarRequerimiento();
                }
                Requerimiento requerimiento = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO];

                Cotizacion cotizacion = (Cotizacion)this.Session[Constantes.VAR_SESSION_COTIZACION_VER];
                requerimiento.cotizacion = new Cotizacion();
                requerimiento.cotizacion.idCotizacion = cotizacion.idCotizacion;
                requerimiento.cotizacion.codigo = cotizacion.codigo;
                requerimiento.ciudad = cotizacion.ciudad;
                requerimiento.cliente = cotizacion.cliente;
                requerimiento.esPagoContado = cotizacion.esPagoContado;


                if (cotizacion.cliente.horaInicioPrimerTurnoEntrega != null && !cotizacion.cliente.horaInicioPrimerTurnoEntrega.Equals("00:00:00"))
                {
                    requerimiento.horaEntregaDesde = cotizacion.cliente.horaInicioPrimerTurnoEntregaFormat;
                }
                if (cotizacion.cliente.horaFinPrimerTurnoEntrega != null && !cotizacion.cliente.horaFinPrimerTurnoEntrega.Equals("00:00:00"))
                {
                    requerimiento.horaEntregaHasta = cotizacion.cliente.horaFinPrimerTurnoEntregaFormat;
                }

                if (cotizacion.cliente.horaInicioSegundoTurnoEntrega != null && !cotizacion.cliente.horaInicioSegundoTurnoEntrega.Equals("00:00:00"))
                {
                    requerimiento.horaEntregaAdicionalDesde = cotizacion.cliente.horaInicioSegundoTurnoEntregaFormat;
                }
                if (cotizacion.cliente.horaFinSegundoTurnoEntrega != null && !cotizacion.cliente.horaFinSegundoTurnoEntrega.Equals("00:00:00"))
                {
                    requerimiento.horaEntregaAdicionalHasta = cotizacion.cliente.horaFinSegundoTurnoEntregaFormat;
                }


                SolicitanteBL solicitanteBL = new SolicitanteBL();
                requerimiento.cliente.solicitanteList = solicitanteBL.getSolicitantes(cotizacion.cliente.idCliente);

                DireccionEntregaBL direccionEntregaBL = new DireccionEntregaBL();
                requerimiento.cliente.direccionEntregaList = direccionEntregaBL.getDireccionesEntrega(cotizacion.cliente.idCliente);
                requerimiento.direccionEntrega = new DireccionEntrega();

                requerimiento.observaciones = String.Empty;

                requerimiento.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            
                requerimiento.requerimientoDetalleList = new List<RequerimientoDetalle>();
                foreach (DocumentoDetalle documentoDetalle in cotizacion.documentoDetalle)
                {
                    RequerimientoDetalle pedidoDetalle = new RequerimientoDetalle(requerimiento.usuario.visualizaCostos, requerimiento.usuario.visualizaMargen);
                    pedidoDetalle.cantidad = documentoDetalle.cantidad;
                    if (documentoDetalle.cantidad == 0)
                        pedidoDetalle.cantidad = 1;

                    //pedidoDetalle.costoAnterior = documentoDetalle.costoAnterior;
                    pedidoDetalle.esPrecioAlternativo = documentoDetalle.esPrecioAlternativo;
                    pedidoDetalle.flete = documentoDetalle.flete;
                    pedidoDetalle.observacion = documentoDetalle.observacion;



                    // pedidoDetalle.porcentajeDescuento = documentoDetalle.porcentajeDescuento;


                    pedidoDetalle.producto = documentoDetalle.producto;




                    if (cotizacion.tipoCotizacion == Cotizacion.TiposCotizacion.Transitoria)
                    {
                        if (pedidoDetalle.producto.precioClienteProducto == null)
                        {
                            pedidoDetalle.producto.precioClienteProducto = new PrecioClienteProducto();
                        }
                        //Se le asigna un id temporal solo para que no se rechaza el precio en la validación
                        pedidoDetalle.producto.precioClienteProducto.idPrecioClienteProducto = Guid.NewGuid();
                        pedidoDetalle.producto.precioClienteProducto.fechaInicioVigencia = cotizacion.fechaInicioVigenciaPrecios;
                        pedidoDetalle.producto.precioClienteProducto.fechaFinVigencia = cotizacion.fechaFinVigenciaPrecios.HasValue ? cotizacion.fechaFinVigenciaPrecios.Value : cotizacion.fechaInicioVigenciaPrecios.Value.AddDays(Constantes.DIAS_MAX_COTIZACION_TRANSITORIA);
                        //pedidoDetalle.producto.precioClienteProducto.cliente.idCliente = cotizacion.cliente.idCliente;
                        pedidoDetalle.producto.precioClienteProducto.precioUnitario = documentoDetalle.precioUnitario;
                        pedidoDetalle.producto.precioClienteProducto.precioNeto = documentoDetalle.precioNeto;
                        pedidoDetalle.producto.precioClienteProducto.esUnidadAlternativa = documentoDetalle.esPrecioAlternativo;

                    }
                    else if (cotizacion.tipoCotizacion == Cotizacion.TiposCotizacion.Trivial)
                    {
                        //Se le asigna un id temporal solo para que no se rechaza el precio en la validación
                        pedidoDetalle.producto.precioClienteProducto.idPrecioClienteProducto = Guid.NewGuid();
                        //pedidoDetalle.producto.precioClienteProducto.fechaInicioVigencia = cotizacion.fechaInicioVigenciaPrecios;
                        //pedidoDetalle.producto.precioClienteProducto.fechaFinVigencia = cotizacion.fechaFinVigenciaPrecios.HasValue ? cotizacion.fechaFinVigenciaPrecios.Value : cotizacion.fechaInicioVigenciaPrecios.Value.AddDays(Constantes.DIAS_MAX_COTIZACION_TRANSITORIA);
                        //pedidoDetalle.producto.precioClienteProducto.cliente.idCliente = cotizacion.cliente.idCliente;
                        pedidoDetalle.producto.precioClienteProducto.precioUnitario = documentoDetalle.precioUnitario;
                        pedidoDetalle.producto.precioClienteProducto.precioNeto = documentoDetalle.precioNeto;
                        pedidoDetalle.producto.precioClienteProducto.esUnidadAlternativa = documentoDetalle.esPrecioAlternativo;
                    }
                    else
                    {
                        if (pedidoDetalle.producto.precioClienteProducto.esUnidadAlternativa)
                        {
                            pedidoDetalle.producto.precioClienteProducto.precioUnitario = pedidoDetalle.producto.precioClienteProducto.precioUnitario / pedidoDetalle.producto.precioClienteProducto.equivalencia;
                        }


                    }

                    if (documentoDetalle.esPrecioAlternativo)
                    {
                        pedidoDetalle.precioNeto = documentoDetalle.precioNeto * documentoDetalle.ProductoPresentacion.Equivalencia;
                        pedidoDetalle.ProductoPresentacion = documentoDetalle.ProductoPresentacion;
                    }
                    else
                    {
                        pedidoDetalle.precioNeto = documentoDetalle.precioNeto;
                    }
                    pedidoDetalle.unidad = documentoDetalle.unidad;

                    pedidoDetalle.porcentajeDescuento = 100 - (pedidoDetalle.precioNeto * 100 / pedidoDetalle.precioLista);

                    requerimiento.requerimientoDetalleList.Add(pedidoDetalle);
                }
                requerimiento.fechaPrecios = requerimiento.fechaSolicitud.AddDays(Constantes.DIAS_MAX_BUSQUEDA_PRECIOS * -1);
                RequerimientoBL requerimientoBL = new RequerimientoBL();
                requerimientoBL.calcularMontosTotales(requerimiento);
                this.Session[Constantes.VAR_SESSION_REQUERIMIENTO] = requerimiento;
            }
            catch (Exception e)
            {
                logger.Error(e, agregarUsuarioAlMensaje(e.Message));
                throw e;
            }

        }

        private void instanciarRequerimiento()
        {
            try
            {
                Requerimiento requerimiento = new Requerimiento(Requerimiento.ClasesRequerimiento.Venta);
                requerimiento.idRequerimiento = 0;
                requerimiento.cotizacion = new Cotizacion();
                requerimiento.ubigeoEntrega = new Ubigeo();
                requerimiento.ubigeoEntrega.Id = "000000";
                requerimiento.ciudad = new Ciudad();
                requerimiento.cliente = new Cliente();
                requerimiento.esPagoContado = false;

                requerimiento.tipoRequerimiento = Requerimiento.tiposRequerimiento.Venta;
                requerimiento.ciudadASolicitar = new Ciudad();

                requerimiento.numeroReferenciaCliente = null;
                requerimiento.direccionEntrega = new DireccionEntrega();
                requerimiento.solicitante = new Solicitante();
                requerimiento.fechaSolicitud = DateTime.Now;
                requerimiento.fechaEntregaDesde = null;
                requerimiento.fechaEntregaHasta = null;
                //Se considera como fecha de entrega la misma fecha en que se realiza el registro (solo debe ser para Interbank)
                requerimiento.fechaEntregaDesde = DateTime.Now;
                requerimiento.fechaEntregaHasta = DateTime.Now;

                requerimiento.horaEntregaDesde = "09:00";
                requerimiento.horaEntregaHasta = "18:00";
                requerimiento.contactoRequerimiento = String.Empty;
                requerimiento.telefonoContactoRequerimiento = String.Empty;
                requerimiento.incluidoIGV = false;
                //  requerimiento.tasaIGV = Constantes.IGV;
                //requerimiento.flete = 0;
                // requerimiento.mostrarCodigoProveedor = true;
                requerimiento.observaciones = String.Empty;

                requerimiento.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                requerimiento.requerimientoDetalleList = new List<RequerimientoDetalle>();
                requerimiento.fechaPrecios = requerimiento.fechaSolicitud.AddDays(Constantes.DIAS_MAX_BUSQUEDA_PRECIOS * -1);
                requerimiento.cliente.direccionEntregaList = requerimiento.usuario.direccionEntregaList;
                this.Session[Constantes.VAR_SESSION_REQUERIMIENTO] = requerimiento;
            }
            catch (Exception e)
            {
                logger.Error(e, agregarUsuarioAlMensaje(e.Message));
                throw e;
            }
        }

        public void iniciarEdicionRequerimiento()
        {
            try
            {
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                Requerimiento requerimientoVer = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_VER];
                RequerimientoBL requerimientoBL = new RequerimientoBL();
                Requerimiento requerimiento = new Requerimiento(Requerimiento.ClasesRequerimiento.Venta);
                requerimiento.idRequerimiento = requerimientoVer.idRequerimiento;
                //    requerimiento.fechaModificacion = cotizacionVer.fechaModificacion;
                requerimiento.usuario = (Usuario)this.Session["usuario"];
                //Se cambia el estado de la cotizacion a Edición
                //requerimientoBL.cambiarEstadoRe(pedido);
                //Se obtiene los datos de la cotización ya modificada
                requerimiento = requerimientoBL.GetRequerimientoParaEditar(requerimiento, usuario);

                requerimiento.cliente.direccionEntregaList = usuario.direccionEntregaList;
                //Temporal
                requerimiento.ciudadASolicitar = new Ciudad();

                /*  if (requerimiento.tipoRequerimiento == Requerimiento.tiposRequerimiento.TrasladoInterno)
                  {
                      requerimiento.ciudadASolicitar = new Ciudad { idCiudad = requerimiento.ciudad.idCiudad,
                                                              nombre = requerimiento.ciudad.nombre,
                                                              esProvincia = requerimiento.ciudad.esProvincia };

                      requerimiento.ciudad = requerimiento.cliente.ciudad;
                  }*/

                this.Session[Constantes.VAR_SESSION_REQUERIMIENTO] = requerimiento;
            }
            catch (Exception e)
            {
                logger.Error(e, agregarUsuarioAlMensaje(e.Message));
                throw e;
            }
        }

        public void obtenerProductosAPartirdePreciosRegistrados()
        {
            try
            {
                Requerimiento requerimiento = this.RequerimientoSession;
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                requerimiento.usuario = usuario;
                RequerimientoBL requerimientoBL = new RequerimientoBL();

                String[] fechaPrecios = this.Request.Params["fecha"].Split('/');
                requerimiento.fechaPrecios = new DateTime(Int32.Parse(fechaPrecios[2]), Int32.Parse(fechaPrecios[1]), Int32.Parse(fechaPrecios[0]), 0, 0, 0);

                String proveedor = "Todos";
                String familia = "Todas";
                if (this.Session["proveedor"] != null)
                {
                    proveedor = (String)this.Session["proveedor"];
                }

                if (this.Session["familia"] != null)
                {
                    familia = (String)this.Session["familia"];
                }

                requerimiento = requerimientoBL.obtenerProductosAPartirdePreciosRegistrados(requerimiento, familia, proveedor, usuario);
                requerimientoBL.calcularMontosTotales(requerimiento);
                this.RequerimientoSession = requerimiento;
            }
            catch (Exception e)
            {
                logger.Error(e, agregarUsuarioAlMensaje(e.Message));
                throw e;
            }
        }


        public String SearchProductos()
        {
            try
            {
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                String texto_busqueda = this.Request.Params["data[q]"];
                ProductoBL bl = new ProductoBL();
                Requerimiento requerimiento = this.RequerimientoSession;
                String resultado = bl.getProductosBusqueda(texto_busqueda, false, this.Session["proveedor"] != null ? (String)this.Session["proveedor"] : "Todos", this.Session["familia"] != null ? (String)this.Session["familia"] : "Todas", null, usuario.idClienteSunat);
                return resultado;
            }
            catch (Exception e)
            {
                logger.Error(e, agregarUsuarioAlMensaje(e.Message));
                throw e;
            }
        }


        public ActionResult CancelarCreacionRequerimiento()
        {
            try
            {
                this.Session[Constantes.VAR_SESSION_REQUERIMIENTO] = null;
                UsuarioBL usuarioBL = new UsuarioBL();
                Usuario usuario = (Usuario)this.Session["usuario"];
                return RedirectToAction("Index", "Pedido");
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
                Requerimiento requerimiento = this.RequerimientoSession;
                return clienteBL.getCLientesBusqueda(data, requerimiento.ciudad.idCiudad);
            }
            catch (Exception e)
            {
                logger.Error(e, agregarUsuarioAlMensaje(e.Message));
                throw e;
            }
        }



        public void GetCliente(Guid idCliente, Guid idSolicitante)
        {
            try
            {

                Requerimiento requerimiento = this.RequerimientoSession;
                //Guid idCliente = Guid.Parse(Request["idCliente"].ToString());
                ClienteBL clienteBl = new ClienteBL();
                requerimiento.cliente = clienteBl.getCliente(idCliente);

                if (requerimiento.cliente.correoEnvioFactura == null || (requerimiento.cliente.correoEnvioFactura != null && requerimiento.cliente.correoEnvioFactura.Trim().Length == 0))
                {
                    requerimiento.cliente = new Cliente();
                }
                else
                {

                    //Se obtiene la lista de direccioines de entrega registradas para el cliente
                    DireccionEntregaBL direccionEntregaBL = new DireccionEntregaBL();
                    requerimiento.cliente.direccionEntregaList = requerimiento.usuario.direccionEntregaList;//. direccionEntregaBL.getDireccionesEntrega(idCliente);

                    SolicitanteBL solicitanteBL = new SolicitanteBL();
                    requerimiento.cliente.solicitanteList = solicitanteBL.getSolicitantes(idCliente);

                    //requerimiento.direccionEntrega = new DireccionEntrega();
                    //requerimiento.horaEntregaDesde = requerimiento.cliente.horaInicioPrimerTurnoEntrega;
                    //requerimiento.horaEntregaHasta = requerimiento.cliente.horaFinPrimerTurnoEntrega;
                    //requerimiento.horaEntregaAdicionalDesde = requerimiento.cliente.horaInicioSegundoTurnoEntrega;
                    //requerimiento.horaEntregaAdicionalHasta = requerimiento.cliente.horaFinSegundoTurnoEntrega;

                    //Se limpia el ubigeo de entrega
                    //requerimiento.ubigeoEntrega = new Ubigeo();
                    //requerimiento.ubigeoEntrega.Id = Constantes.UBIGEO_VACIO;


                }

                requerimiento.solicitante = requerimiento.cliente.solicitanteList.Where(d => d.idSolicitante == idSolicitante).FirstOrDefault();

                this.RequerimientoSession = requerimiento;

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
                Requerimiento requerimiento = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO];
                //Se recupera el producto y se guarda en Session

                Guid idProducto = Guid.Parse(this.Request.Params["idProducto"].ToString());
                //Guid idProducto = Guid.Parse(Request["idProducto"].ToString());
                this.Session["idProducto"] = idProducto.ToString();

                //Para recuperar el producto se envia si la sede seleccionada es provincia o no
                ProductoBL bl = new ProductoBL();
                Producto producto = bl.getProducto(idProducto, requerimiento.ciudad.esProvincia, requerimiento.incluidoIGV, requerimiento.cliente.idCliente);

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
                Requerimiento requerimiento = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO];
                Guid idProducto = Guid.Parse(this.Session["idProducto"].ToString());
                RequerimientoDetalle pedidoDetalle = requerimiento.requerimientoDetalleList.Where(p => p.producto.idProducto == idProducto).FirstOrDefault();
                if (pedidoDetalle != null)
                {
                    String mensajeError = "Producto ya se encuentra en la lista.";
                    logger.Error(agregarUsuarioAlMensaje(mensajeError));
                    throw new System.Exception(mensajeError);
                }

                RequerimientoDetalle detalle = new RequerimientoDetalle(requerimiento.usuario.visualizaCostos, requerimiento.usuario.visualizaMargen);
                ProductoBL productoBL = new ProductoBL();
                Producto producto = productoBL.getProducto(idProducto, requerimiento.ciudad.esProvincia, requerimiento.incluidoIGV, requerimiento.cliente.idCliente);

                if (requerimiento.claseRequerimiento == Requerimiento.ClasesRequerimiento.Venta)
                {
                    String mensajeError = "Tipo de Producto no concuerda con el tipo de Requerimiento.";
                    if ((requerimiento.tipoRequerimiento == Requerimiento.tiposRequerimiento.Venta || requerimiento.tipoRequerimiento == Requerimiento.tiposRequerimiento.TransferenciaGratuitaEntregada) && producto.tipoProducto == Producto.TipoProducto.Comodato)
                    {
                        logger.Error(agregarUsuarioAlMensaje(mensajeError));
                        throw new System.Exception(mensajeError);
                    }
                    if (requerimiento.tipoRequerimiento == Requerimiento.tiposRequerimiento.ComodatoEntregado && producto.tipoProducto != Producto.TipoProducto.Comodato)
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
                requerimiento.requerimientoDetalleList.Add(detalle);

                //CotizacionDetalle cotizacionDetalle = (CotizacionDetalle)Convert.ChangeType(pedido, typeof(CotizacionDetalle));

                //Calcula los montos totales de la cabecera de la cotizacion
                RequerimientoBL requerimientoBL = new RequerimientoBL();
                requerimientoBL.calcularMontosTotales(requerimiento);




                var nombreProducto = detalle.producto.descripcion;
                /* if (requerimiento.mostrarCodigoProveedor)
                 {*/
                nombreProducto = detalle.producto.skuProveedor + " - " + detalle.producto.descripcion;
                //    }

                /*  if (requerimiento.considerarCantidades == Cotizacion.OpcionesConsiderarCantidades.Ambos)
                  {*/
                //      nombreProducto = nombreProducto + "\\n" + detalle.observacion;
                //}

                /* String resultado = "{" +
                     "\"idProducto\":\"" + detalle.producto.idProducto + "\"," +
                     "\"codigoProducto\":\"" + detalle.producto.sku + "\"," +
                     "\"nombreProducto\":\"" + nombreProducto + "\"," +
                     "\"unidad\":\"" + detalle.unidad + "\"," +
                     "\"igv\":\"" + requerimiento.montoIGV.ToString() + "\", " +
                     "\"subTotal\":\"" + requerimiento.montoSubTotal.ToString() + "\", " +
                     "\"margen\":\"" + detalle.margen + "\", " +
                     "\"precioUnitario\":\"" + detalle.precioUnitario + "\", " +
                     "\"observacion\":\"" + detalle.observacion + "\", " +
                     "\"total\":\"" + requerimiento.montoTotal.ToString() + "\"}";*/

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
                    igv = requerimiento.montoIGV.ToString(),
                    subTotal = requerimiento.montoSubTotal.ToString(),
                    margen = detalle.margen,
                    precioUnitario = detalle.precioUnitario,
                    observacion = detalle.observacion,
                    total = requerimiento.montoTotal.ToString(),
                    precioUnitarioRegistrado = precioUnitarioRegistrado

                };

                this.RequerimientoSession = requerimiento;
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
            Requerimiento requerimiento = this.RequerimientoSession;
            PropertyInfo propertyInfo = requerimiento.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(requerimiento, this.Request.Params["valor"]);
            this.RequerimientoSession = requerimiento;
        }

        public void ChangeUbigeoEntrega()
        {
            Requerimiento requerimiento = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO];
            requerimiento.ubigeoEntrega.Id = this.Request.Params["ubigeoEntregaId"];
            this.Session[Constantes.VAR_SESSION_REQUERIMIENTO] = requerimiento;
        }


        public void ChangeNumeroReferenciaCliente()
        {
            Requerimiento requerimiento = this.RequerimientoSession;
            requerimiento.numeroReferenciaCliente = this.Request.Params["numeroReferenciaCliente"];
            this.RequerimientoSession = requerimiento;
        }

        public void ChangeOtrosCargos()
        {
            Requerimiento requerimiento = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO];
            requerimiento.otrosCargos = Decimal.Parse(this.Request.Params["otrosCargos"]);
            this.Session[Constantes.VAR_SESSION_REQUERIMIENTO] = requerimiento;
        }


        public string ChangeDireccionEntrega()
        {
            Requerimiento requerimiento = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO];

            if (this.Request.Params["idDireccionEntrega"] == null || this.Request.Params["idDireccionEntrega"].Equals(String.Empty))
            {
                requerimiento.direccionEntrega = new DireccionEntrega();
                requerimiento.ciudad = new Ciudad();
                requerimiento.cliente = new Cliente();
            }
            else
            {
                Guid idDireccionEntrega = Guid.Parse(this.Request.Params["idDireccionEntrega"]);
                requerimiento.direccionEntrega = requerimiento.cliente.direccionEntregaList.Where(d => d.idDireccionEntrega == idDireccionEntrega).FirstOrDefault();
                requerimiento.ubigeoEntrega = requerimiento.direccionEntrega.ubigeo;
                requerimiento.ciudad = requerimiento.direccionEntrega.cliente.ciudad;
                GetCliente(requerimiento.direccionEntrega.cliente.idCliente, requerimiento.usuario.solicitante.idSolicitante);
            }

            requerimiento.existeCambioDireccionEntrega = false;
            this.Session[Constantes.VAR_SESSION_REQUERIMIENTO] = requerimiento;
            return JsonConvert.SerializeObject(requerimiento.direccionEntrega);
        }

        public void ChangeDireccionEntregaDescripcion()
        {
            Requerimiento requerimiento = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO];
            requerimiento.direccionEntrega.descripcion = this.Request.Params["direccionEntregaDescripcion"];
            requerimiento.existeCambioDireccionEntrega = true;
            this.Session[Constantes.VAR_SESSION_REQUERIMIENTO] = requerimiento;
        }

        public void ChangeDireccionEntregaContacto()
        {
            Requerimiento requerimiento = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO];
            requerimiento.direccionEntrega.contacto = this.Request.Params["direccionEntregaContacto"];
            requerimiento.existeCambioDireccionEntrega = true;
            this.Session[Constantes.VAR_SESSION_REQUERIMIENTO] = requerimiento;
        }

        public void ChangeDireccionEntregaTelefono()
        {
            Requerimiento requerimiento = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO];
            requerimiento.direccionEntrega.telefono = this.Request.Params["direccionEntregaTelefono"];
            requerimiento.existeCambioDireccionEntrega = true;
            this.Session[Constantes.VAR_SESSION_REQUERIMIENTO] = requerimiento;
        }



        public string ChangeSolicitante()
        {
            Requerimiento requerimiento = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO];

            if (this.Request.Params["idSolicitante"] == null || this.Request.Params["idSolicitante"].Equals(String.Empty))
            {
                requerimiento.solicitante = new Solicitante();
            }
            else
            {
                Guid idSolicitante = Guid.Parse(this.Request.Params["idSolicitante"]);
                requerimiento.solicitante = requerimiento.cliente.solicitanteList.Where(d => d.idSolicitante == idSolicitante).FirstOrDefault();
            }

            requerimiento.existeCambioSolicitante = false;
            this.Session[Constantes.VAR_SESSION_REQUERIMIENTO] = requerimiento;
            return JsonConvert.SerializeObject(requerimiento.solicitante);
        }

        public void ChangeSolicitanteNombre()
        {
            Requerimiento requerimiento = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO];
            requerimiento.solicitante.nombre = this.Request.Params["solicitanteNombre"];
            requerimiento.existeCambioSolicitante = true;
            this.Session[Constantes.VAR_SESSION_REQUERIMIENTO] = requerimiento;
        }

        public void ChangeSolicitanteTelefono()
        {
            Requerimiento requerimiento = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO];
            requerimiento.solicitante.telefono = this.Request.Params["solicitanteTelefono"];
            requerimiento.existeCambioSolicitante = true;
            this.Session[Constantes.VAR_SESSION_REQUERIMIENTO] = requerimiento;
        }

        public void ChangeSolicitanteCorreo()
        {
            Requerimiento requerimiento = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO];
            requerimiento.solicitante.correo = this.Request.Params["solicitanteCorreo"];
            requerimiento.existeCambioSolicitante = true;
            this.Session[Constantes.VAR_SESSION_REQUERIMIENTO] = requerimiento;
        }


        public void ChangeFechaSolicitud()
        {
            Requerimiento requerimiento = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO];
            String[] fechaSolicitud = this.Request.Params["fechaSolicitud"].Split('/');
            String[] horaSolicitud = this.Request.Params["horaSolicitud"].Split(':');
            requerimiento.fechaSolicitud = new DateTime(Int32.Parse(fechaSolicitud[2]), Int32.Parse(fechaSolicitud[1]), Int32.Parse(fechaSolicitud[0]), Int32.Parse(horaSolicitud[0]), Int32.Parse(horaSolicitud[1]), 0);
            this.Session[Constantes.VAR_SESSION_REQUERIMIENTO] = requerimiento;
        }

        public void ChangeFechaEntregaDesde()
        {
            Requerimiento requerimiento = this.RequerimientoSession;
            String[] ftmp = this.Request.Params["fechaEntregaDesde"].Split('/');
            requerimiento.fechaEntregaDesde = new DateTime(Int32.Parse(ftmp[2]), Int32.Parse(ftmp[1]), Int32.Parse(ftmp[0]));
            this.RequerimientoSession = requerimiento;
        }

        public void ChangeFechaEntregaHasta()
        {
            Requerimiento requerimiento = this.RequerimientoSession;
            String[] ftmp = this.Request.Params["fechaEntregaHasta"].Split('/');
            requerimiento.fechaEntregaHasta = new DateTime(Int32.Parse(ftmp[2]), Int32.Parse(ftmp[1]), Int32.Parse(ftmp[0]));
            this.RequerimientoSession = requerimiento;
        }

        public void ChangeContactoRequerimiento()
        {
            Requerimiento requerimiento = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO];
            requerimiento.contactoRequerimiento = this.Request.Params["contactoPedido"];
            this.Session[Constantes.VAR_SESSION_REQUERIMIENTO] = requerimiento;
        }

        public void ChangeTelefonoContactoRequerimiento()
        {
            Requerimiento requerimiento = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO];
            requerimiento.telefonoContactoRequerimiento = this.Request.Params["telefonoContactoPedido"];
            this.Session[Constantes.VAR_SESSION_REQUERIMIENTO] = requerimiento;
        }


        public void ChangeObservaciones()
        {
            Requerimiento requerimiento = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO];

            requerimiento.GetType().GetProperty("observaciones").SetValue(requerimiento, this.Request.Params["observaciones"]);

            this.Session[Constantes.VAR_SESSION_REQUERIMIENTO] = requerimiento;
        }

/*
        public void ChangeFechaSolicitudDesde()
        {
            Requerimiento requerimiento = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_BUSQUEDA];
            String[] solDesde = this.Request.Params["fechaSolicitudDesde"].Split('/');
            requerimiento.fechaSolicitudDesde = new DateTime(Int32.Parse(solDesde[2]), Int32.Parse(solDesde[1]), Int32.Parse(solDesde[0]));
            this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_BUSQUEDA] = requerimiento;
        }

        public void ChangeFechaSolicitudHasta()
        {
            Requerimiento requerimiento = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_BUSQUEDA];
            String[] solHasta = this.Request.Params["fechaSolicitudHasta"].Split('/');
            requerimiento.fechaSolicitudHasta = new DateTime(Int32.Parse(solHasta[2]), Int32.Parse(solHasta[1]), Int32.Parse(solHasta[0]), 23, 59, 59);
            this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_BUSQUEDA] = requerimiento;
        }*/

        public void ChangeNumero()
        {
            Requerimiento requerimiento = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_BUSQUEDA];
            if (this.Request.Params["numero"] == null || this.Request.Params["numero"].Trim().Length == 0)
            {
                requerimiento.idRequerimiento = 0;
            }
            else
            {
                requerimiento.idRequerimiento = int.Parse(this.Request.Params["numero"]);
            }
            this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_BUSQUEDA] = requerimiento;
        }



        public void ChangeIdPeriodo()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            Requerimiento requerimiento = this.RequerimientoSession;

            requerimiento.periodo.idPeriodoSolicitud = Guid.Parse(this.Request.Params["idPeriodo"]);

            this.RequerimientoSession = requerimiento;
        }

        public String ChangeIdCiudad()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            Requerimiento requerimiento = this.RequerimientoSession;
            requerimiento.cliente = new Cliente();
            Guid idCiudad = Guid.Empty;
            List<Cliente> clienteList = (List<Cliente>)this.Session["clienteList"];
            if (this.Request.Params["idCiudad"] != null && !this.Request.Params["idCiudad"].Equals(""))
            {
                idCiudad = Guid.Parse(this.Request.Params["idCiudad"]);



            }
            //Para realizar el cambio de ciudad ningun producto debe estar agregado
            if (requerimiento.requerimientoDetalleList != null && requerimiento.requerimientoDetalleList.Count > 0)
            {
                // throw new Exception("No se puede cambiar de ciudad");
                return "No se puede cambiar de ciudad";
            }
            else
            {
                CiudadBL ciudadBL = new CiudadBL();
                Ciudad ciudadNueva = ciudadBL.getCiudad(idCiudad);
                requerimiento.ciudad = ciudadNueva;
                requerimiento.cliente = clienteList.Where(c => c.ciudad.idCiudad == idCiudad).FirstOrDefault();
                if (requerimiento.cliente == null)
                {
                    requerimiento.cliente = new Cliente();
                }
                else
                {
                    GetCliente(requerimiento.cliente.idCliente, usuario.solicitante.idSolicitante);
                }




                //this.PedidoSession = requerimiento;
                return "{\"idCiudad\": \"" + idCiudad + "\"}";
            }
        }

     /*   public void ChangeTipoRequerimiento()
        {
            Char tipoRequerimiento = Convert.ToChar(Int32.Parse(this.Request.Params["tipoPedido"]));
            this.RequerimientoSession.tipoRequerimiento = (Requerimiento.tiposPedido)tipoPedido;
        }

        public void ChangeTipoPedidoBusqueda()
        {
            Char tipoPedidoBusqueda = Convert.ToChar(Int32.Parse(this.Request.Params["tipoPedidoBusqueda"]));
            this.RequerimientoSession.tipoPedidoVentaBusqueda = (Requerimiento.tiposPedidoVentaBusqueda)tipoPedidoBusqueda;
        }*/

        public void changePeriodo()
        {
   //         int periodo = Int32.Parse(this.Request.Params["periodo"]);
   //         this.RequerimientoSession.periodo = (Requerimiento.periodos)periodo;
        }


        public String ChangeIdCiudadASolicitar()
        {
            Requerimiento requerimiento = this.RequerimientoSession;
            Guid idCiudad = Guid.Empty;
            if (this.Request.Params["idCiudadASolicitar"] != null && !this.Request.Params["idCiudadASolicitar"].Equals(""))
            {
                idCiudad = Guid.Parse(this.Request.Params["idCiudadASolicitar"]);
            }
            //Para realizar el cambio de ciudad ningun producto debe estar agregado
            /*    if (requerimiento.requerimientoDetalleList != null && requerimiento.requerimientoDetalleList.Count > 0)
                {
                    // throw new Exception("No se puede cambiar de ciudad");
                    return "No se puede cambiar de ciudad";
                }
                else
                {*/
            CiudadBL ciudadBL = new CiudadBL();
            Ciudad ciudadASolicitar = ciudadBL.getCiudad(idCiudad);
            requerimiento.ciudadASolicitar = ciudadASolicitar;
            this.RequerimientoSession = requerimiento;
            return "{\"idCiudad\": \"" + idCiudad + "\"}";
            //  }
        }

        [HttpPost]
        public String ChangeDetalle(List<DocumentoDetalleJson> cotizacionDetalleJsonList)
        {
            IDocumento documento = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO];
            List<DocumentoDetalle> documentoDetalle = HelperDocumento.updateDocumentoDetalle(documento, cotizacionDetalleJsonList);
            documento.documentoDetalle = documentoDetalle;
            RequerimientoBL requerimientoBL = new RequerimientoBL();
            requerimientoBL.calcularMontosTotales((Requerimiento)documento);
            this.Session[Constantes.VAR_SESSION_REQUERIMIENTO] = documento;
            return "{\"cantidad\":\"" + documento.documentoDetalle.Count + "\"}";
        }


        public String ChangeEsPagoContado()
        {
            Requerimiento requerimiento = this.RequerimientoSession;
            try
            {
                requerimiento.esPagoContado = Int32.Parse(this.Request.Params["esPagoContado"]) == 1;
            }
            catch (Exception ex)
            {
            }
            this.RequerimientoSession = requerimiento;

            return "{\"textoCondicionesPago\":\"" + requerimiento.textoCondicionesPago + "\"}";
        }


        #endregion


        #region CAMBIOS CAMPOS DE FORMULARIO BUSQUEDA



        /*    public void ChangeEstado()
            {
                Requerimiento requerimiento = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_BUSQUEDA];
                requerimiento.seguimientoRequerimiento.estado = (SeguimientoRequerimiento.estadosSeguimientoPedido)Int32.Parse(this.Request.Params["estado"]);
                this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_BUSQUEDA] = requerimiento;
            }*/




        #endregion




        #region CREAR/ACTUALIZAR PEDIDO

       

        #endregion




        public String Create()
        {

            //RUC_MP
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            Requerimiento requerimiento = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO];
            requerimiento.usuario = usuario;
            RequerimientoBL requerimientoBL = new RequerimientoBL();

            if (requerimiento.idRequerimiento > 0)
            {
                throw new System.Exception("Requerimiento ya se encuentra creado");
            }

            requerimientoBL.InsertRequerimiento(requerimiento);
            //long numeroRequerimiento = requerimiento.idRequerimiento;
            String numeroRequerimientoString = requerimiento.numeroRequerimientoString;
            Int32 idRequerimiento = requerimiento.idRequerimiento;


          
            // pedido = null;
            this.Session[Constantes.VAR_SESSION_REQUERIMIENTO] = null;// requerimiento;// null;


            usuarioBL.updatePedidoSerializado(usuario, null);

            var v = new { numeroRequerimiento = numeroRequerimientoString, idRequerimiento = idRequerimiento };
            String resultado = JsonConvert.SerializeObject(v);

            // String resultado = "{ \"codigo\":\"" + numeroRequerimiento + "\", \"estado\":\"" + estado + "\", \"observacion\":\"" + observacion + "\" }";
            return resultado;
        }


        public String Update()
        {

            //RUC_MP
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            Requerimiento requerimiento = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO];
            requerimiento.usuario = usuario;
            RequerimientoBL requerimientoBL = new RequerimientoBL();

       /*     if (requerimiento.idRequerimiento > 0)
            {
                throw new System.Exception("Requerimiento ya se encuentra creado");
            }*/

            requerimientoBL.UpdateRequerimiento(requerimiento);
            //long numeroRequerimiento = requerimiento.idRequerimiento;
            String numeroRequerimientoString = requerimiento.numeroRequerimientoString;
            Int32 idRequerimiento = requerimiento.idRequerimiento;


            // pedido = null;
            this.Session[Constantes.VAR_SESSION_REQUERIMIENTO] = null;// requerimiento;// null;


            usuarioBL.updatePedidoSerializado(usuario, null);

            var v = new { numeroRequerimiento = numeroRequerimientoString, idRequerimiento = idRequerimiento };
            String resultado = JsonConvert.SerializeObject(v);

            // String resultado = "{ \"codigo\":\"" + numeroRequerimiento + "\", \"estado\":\"" + estado + "\", \"observacion\":\"" + observacion + "\" }";
            return resultado;
        }













        public void CleanBusqueda()
        {
            instanciarPedidoBusqueda();
            //Se retorna la cantidad de elementos encontrados
            //List<Pedido> cotizacionList = (List<Cotizacion>)this.Session[Constantes.VAR_SESSION_COTIZACION_LISTA];
            //return cotizacionList.Count();
        }

        [HttpGet]
        public ActionResult ExportLastSearchExcel()
        {
            List<Pedido> list = (List<Pedido>)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_LISTA];

            PedidoSearch excel = new PedidoSearch();
            return excel.generateExcel(list);
        }

        [HttpGet]
        public ActionResult ExportLastSearchRequerimientosExcel()
        {
            List<Pedido> list = (List<Pedido>)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_LISTA];

            PedidoSearch excel = new PedidoSearch();
            return excel.generateExcelRequerimientos(list);
        }

        public String Search()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaRequerimientos;
            //Se recupera el pedido Búsqueda de la session
            Requerimiento requerimiento = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_BUSQUEDA];

            String[] solDesde = this.Request.Params["fechaCreacionDesde"].Split('/');
            requerimiento.fechaCreacionDesde = new DateTime(Int32.Parse(solDesde[2]), Int32.Parse(solDesde[1]), Int32.Parse(solDesde[0]));

            String[] solHasta = this.Request.Params["fechaCreacionHasta"].Split('/');
            requerimiento.fechaCreacionHasta = new DateTime(Int32.Parse(solHasta[2]), Int32.Parse(solHasta[1]), Int32.Parse(solHasta[0]), 23, 59, 59);


            if (this.Request.Params["fechaEntregaDesde"] == null || this.Request.Params["fechaEntregaDesde"].Equals(""))
            {
                requerimiento.fechaEntregaDesde = null;
            }
            else
            {
                String[] entregaDesde = this.Request.Params["fechaEntregaDesde"].Split('/');
                requerimiento.fechaEntregaDesde = new DateTime(Int32.Parse(entregaDesde[2]), Int32.Parse(entregaDesde[1]), Int32.Parse(entregaDesde[0]));
            }


            if (this.Request.Params["fechaEntregaHasta"] == null || this.Request.Params["fechaEntregaHasta"].Equals(""))
            {
                requerimiento.fechaEntregaHasta = null;
            }
            else
            {
                String[] entregaHasta = this.Request.Params["fechaEntregaHasta"].Split('/');
                requerimiento.fechaEntregaHasta = new DateTime(Int32.Parse(entregaHasta[2]), Int32.Parse(entregaHasta[1]), Int32.Parse(entregaHasta[0]), 23, 59, 59);
            }

            if (this.Request.Params["fechaProgramacionDesde"] == null || this.Request.Params["fechaProgramacionDesde"].Equals(""))
            {
                requerimiento.fechaProgramacionDesde = null;
            }
            else
            {
                String[] programacionDesde = this.Request.Params["fechaProgramacionDesde"].Split('/');
                requerimiento.fechaProgramacionDesde = new DateTime(Int32.Parse(programacionDesde[2]), Int32.Parse(programacionDesde[1]), Int32.Parse(programacionDesde[0]));
            }


            if (this.Request.Params["fechaProgramacionHasta"] == null || this.Request.Params["fechaProgramacionHasta"].Equals(""))
            {
                requerimiento.fechaProgramacionHasta = null;
            }
            else
            {
                String[] programacionHasta = this.Request.Params["fechaProgramacionDesde"].Split('/');
                requerimiento.fechaProgramacionHasta = new DateTime(Int32.Parse(programacionHasta[2]), Int32.Parse(programacionHasta[1]), Int32.Parse(programacionHasta[0]));
            }




            if (this.Request.Params["numero"] == null || this.Request.Params["numero"].Trim().Length == 0)
            {
                requerimiento.idRequerimiento = 0;
            }
            else
            {
                requerimiento.idRequerimiento = int.Parse(this.Request.Params["numero"]);
            }

           
            if (this.Request.Params["idGrupoCliente"] == null || this.Request.Params["idGrupoCliente"].Trim().Length == 0)
            {
                requerimiento.idGrupoCliente = 0;
            }
            else
            {
                requerimiento.idGrupoCliente = int.Parse(this.Request.Params["idGrupoCliente"]);
            }

  

            RequerimientoBL requerimientoBL = new RequerimientoBL();
            List<Requerimiento> requerimientoList = requerimientoBL.GetRequerimientos(requerimiento);
            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_LISTA] = requerimientoList;
            this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_BUSQUEDA] = requerimiento;

            String requerimientoListString = JsonConvert.SerializeObject(ParserDTOsSearch.RequerimientoToRequerimientoDTO(requerimientoList));
            //String requerimientoString = JsonConvert.SerializeObject(ParserDTOsShow.RequerimientoToRequerimientoDTO(requerimiento));
            //return "{\"requerimientoList\":" + requerimientoListString + ",\"requerimiento\":" + requerimientoString + " }";
            return requerimientoListString;
            //return pedidoList.Count();
        }

        public String SearchRequerimientos()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.AprobarPedidos;
            //Se recupera el pedido Búsqueda de la session
            Requerimiento requerimiento = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_BUSQUEDA];

            String[] solDesde = this.Request.Params["fechaCreacionDesde"].Split('/');
            requerimiento.fechaCreacionDesde = new DateTime(Int32.Parse(solDesde[2]), Int32.Parse(solDesde[1]), Int32.Parse(solDesde[0]));

            String[] solHasta = this.Request.Params["fechaCreacionHasta"].Split('/');
            requerimiento.fechaCreacionHasta = new DateTime(Int32.Parse(solHasta[2]), Int32.Parse(solHasta[1]), Int32.Parse(solHasta[0]), 23, 59, 59);


            if (this.Request.Params["fechaEntregaDesde"] == null || this.Request.Params["fechaEntregaDesde"].Equals(""))
            {
                requerimiento.fechaEntregaDesde = null;
            }
            else
            {
                String[] entregaDesde = this.Request.Params["fechaEntregaDesde"].Split('/');
                requerimiento.fechaEntregaDesde = new DateTime(Int32.Parse(entregaDesde[2]), Int32.Parse(entregaDesde[1]), Int32.Parse(entregaDesde[0]));
            }


            if (this.Request.Params["fechaEntregaHasta"] == null || this.Request.Params["fechaEntregaHasta"].Equals(""))
            {
                requerimiento.fechaEntregaHasta = null;
            }
            else
            {
                String[] entregaHasta = this.Request.Params["fechaEntregaHasta"].Split('/');
                requerimiento.fechaEntregaHasta = new DateTime(Int32.Parse(entregaHasta[2]), Int32.Parse(entregaHasta[1]), Int32.Parse(entregaHasta[0]), 23, 59, 59);
            }

            if (this.Request.Params["fechaProgramacionDesde"] == null || this.Request.Params["fechaProgramacionDesde"].Equals(""))
            {
                requerimiento.fechaProgramacionDesde = null;
            }
            else
            {
                String[] programacionDesde = this.Request.Params["fechaProgramacionDesde"].Split('/');
                requerimiento.fechaProgramacionDesde = new DateTime(Int32.Parse(programacionDesde[2]), Int32.Parse(programacionDesde[1]), Int32.Parse(programacionDesde[0]));
            }


            if (this.Request.Params["fechaProgramacionHasta"] == null || this.Request.Params["fechaProgramacionHasta"].Equals(""))
            {
                requerimiento.fechaProgramacionHasta = null;
            }
            else
            {
                String[] programacionHasta = this.Request.Params["fechaProgramacionDesde"].Split('/');
                requerimiento.fechaProgramacionHasta = new DateTime(Int32.Parse(programacionHasta[2]), Int32.Parse(programacionHasta[1]), Int32.Parse(programacionHasta[0]));
            }




            if (this.Request.Params["numero"] == null || this.Request.Params["numero"].Trim().Length == 0)
            {
                requerimiento.idRequerimiento = 0;
            }
            else
            {
                requerimiento.idRequerimiento = int.Parse(this.Request.Params["numero"]);
            }

          

            if (this.Request.Params["idGrupoCliente"] == null || this.Request.Params["idGrupoCliente"].Trim().Length == 0)
            {
                requerimiento.idGrupoCliente = 0;
            }
            else
            {
                requerimiento.idGrupoCliente = int.Parse(this.Request.Params["idGrupoCliente"]);
            }

   
            RequerimientoBL requerimientoBL = new RequerimientoBL();
            List<Requerimiento> requerimientoList = requerimientoBL.GetRequerimientos(requerimiento);
            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_LISTA] = requerimientoList;
            this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_BUSQUEDA] = requerimiento;

            String pedidoListString = JsonConvert.SerializeObject(ParserDTOsSearch.RequerimientoToRequerimientoDTO(requerimientoList));
            return pedidoListString;
            //return pedidoList.Count();
        }

        public String AprobarTodosPreview()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            List<Requerimiento> pedidoList = (List<Requerimiento>)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_LISTA];


            pedidoList = pedidoList.Where(p => p.estadoRequerimiento == Requerimiento.estadosRequerimiento.Ingresado).ToList();

            List<Requerimiento> finalList = new List<Requerimiento>();
            RequerimientoBL requerimientoBL = new RequerimientoBL();
            Requerimiento requerimientoSearch = new Requerimiento(Requerimiento.ClasesRequerimiento.Venta);
            bool esNuevo = true;
            bool nuevoDetalle = true;
            foreach (Requerimiento req in pedidoList)
            {
                requerimientoSearch.idRequerimiento = req.idRequerimiento;
                Requerimiento reqTotal = requerimientoBL.GetRequerimiento(requerimientoSearch, usuario);

                Requerimiento item = null;
                esNuevo = true;
                foreach (Requerimiento ped in finalList)
                {
                    if (ped.direccionEntrega.idDireccionEntrega == reqTotal.direccionEntrega.direccionEntregaAlmacen.idDireccionEntrega)
                    {
                        esNuevo = false;
                        item = ped;
                    }
                }

                if (esNuevo)
                {
                    item = new Requerimiento(Requerimiento.ClasesRequerimiento.Venta);

                    item.direccionEntrega = reqTotal.direccionEntrega.direccionEntregaAlmacen;
                    item.ciudad = reqTotal.ciudad;
                    item.cliente = reqTotal.cliente;
                    item.requerimientoDetalleList = new List<RequerimientoDetalle>();
                    finalList.Add(item);
                }


                foreach (RequerimientoDetalle detReq in reqTotal.requerimientoDetalleList)
                {
                    nuevoDetalle = true;
                    RequerimientoDetalle det = null;
                    foreach (RequerimientoDetalle itemDet in item.requerimientoDetalleList)
                    {
                        if (detReq.producto.sku.Equals(itemDet.producto.sku))
                        {
                            nuevoDetalle = false;
                            det = itemDet;
                        }
                    }

                    if (nuevoDetalle)
                    {
                        det = JsonConvert.DeserializeObject<RequerimientoDetalle>(JsonConvert.SerializeObject(detReq));
                        det.idDocumentoDetalle = Guid.Empty;
                        item.requerimientoDetalleList.Add(det);

                    }
                    else
                    {
                        det.cantidad = det.cantidad + detReq.cantidad;
                    }
                }
            }

            decimal subTotal = 0;

            foreach (Requerimiento ped in finalList)
            {
                HelperDocumento.calcularMontosTotales(ped);
                subTotal = subTotal + ped.montoSubTotal;
            }

            decimal igv = subTotal * ((decimal)0.18);
            decimal total = igv + subTotal;

            this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_CONSOLIDADO] = finalList;

            String pedidoListString = JsonConvert.SerializeObject(finalList);

            return "{\"subTotal\":" + subTotal + ",\"igv\":" + igv + ",\"total\":" + total + ",\"requerimientoList\":" + pedidoListString + "}";
        }


        public String CreatePedidos()
        {


            List<Requerimiento> requerimientoList = (List<Requerimiento>)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_CONSOLIDADO];

            Requerimiento requerimientoBusqueda = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_BUSQUEDA];
            List<Requerimiento> requerimientoBusquedaList = (List<Requerimiento>)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_LISTA];
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            PedidoBL pedidoBL = new PedidoBL();
            List<Pedido> pedidoList = new List<Pedido>();
            Int64 numeroGrupo = pedidoBL.GetSiguienteNumeroGrupoPedido();
            foreach (Requerimiento requerimiento in  requerimientoList)
            {
                foreach (RequerimientoDetalle requerimientoDetalle in requerimiento.documentoDetalle)
                {
                    Pedido pedido = new Pedido();
                    pedido.usuario = usuario;
                    pedido.numeroGrupoPedido = numeroGrupo;

                    pedido.pedidoDetalleList = new List<PedidoDetalle>();

                    PedidoDetalle pedidoDetalle = new PedidoDetalle(false, false);
                    pedidoDetalle.producto = requerimientoDetalle.producto;
                    pedidoDetalle.cantidad = requerimientoDetalle.cantidad;
                    pedidoDetalle.producto.precioSinIgv = requerimientoDetalle.producto.precioSinIgv;
                    pedidoDetalle.producto.costoSinIgv = requerimientoDetalle.producto.costoSinIgv;
                    pedidoDetalle.esPrecioAlternativo = requerimientoDetalle.esPrecioAlternativo;
                    if (pedidoDetalle.esPrecioAlternativo)
                    {
                        pedidoDetalle.ProductoPresentacion.Equivalencia = requerimientoDetalle.ProductoPresentacion.Equivalencia;
                        pedidoDetalle.ProductoPresentacion.IdProductoPresentacion = requerimientoDetalle.ProductoPresentacion.IdProductoPresentacion;
                    }


                    pedidoDetalle.unidad = requerimientoDetalle.unidad;
                    pedidoDetalle.porcentajeDescuento = requerimientoDetalle.porcentajeDescuento;
                    pedidoDetalle.precioNeto = requerimientoDetalle.precioNeto;
                    pedidoDetalle.flete = requerimientoDetalle.flete;
                    pedidoDetalle.observacion = requerimientoDetalle.observacion;

                    //pedidoDetalle.indicadorAprobacion = requerimientoDetalle.indicadorAprobacion;
                    pedido.pedidoDetalleList.Add(pedidoDetalle);
                    

                    //revisar
                    pedido.ciudad = requerimiento.ciudad;
                    pedido.cliente = requerimiento.cliente;
                    //pedido.numeroReferenciaCliente = requerimiento.numeroReferenciaCliente;

                    pedido.direccionEntrega = requerimiento.direccionEntrega;
                    pedido.direccionEntrega.idDireccionEntrega = requerimiento.direccionEntrega.idDireccionEntrega;
                    pedido.direccionEntrega.descripcion = requerimiento.direccionEntrega.descripcion;
                    pedido.direccionEntrega.telefono = requerimiento.direccionEntrega.telefono;
                    pedido.direccionEntrega.codigoCliente = requerimiento.direccionEntrega.codigoCliente;
                    pedido.direccionEntrega.codigoMP = requerimiento.direccionEntrega.codigoMP;
                    pedido.direccionEntrega.nombre = requerimiento.direccionEntrega.nombre;
                    pedido.direccionEntrega.observaciones = requerimiento.direccionEntrega.observaciones;
                    pedido.fechaSolicitud = DateTime.Now;

                    //OK
                    String[] ftmp = this.Request.Params["fechaEntregaDesde"].Split('/');
                    pedido.fechaEntregaDesde = new DateTime(Int32.Parse(ftmp[2]), Int32.Parse(ftmp[1]), Int32.Parse(ftmp[0]));
                    ftmp = this.Request.Params["fechaEntregaHasta"].Split('/');
                    pedido.fechaEntregaHasta = new DateTime(Int32.Parse(ftmp[2]), Int32.Parse(ftmp[1]), Int32.Parse(ftmp[0]));
                    pedido.esPagoContado = false;
                    DateTime dtTmp = DateTime.Now;
                    String[] horaEntregaDesdeArray = requerimientoBusqueda.horaEntregaDesde.Split(':');
                    DateTime horaEntregaDesde = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaEntregaDesdeArray[0]), Int32.Parse(horaEntregaDesdeArray[1]), 0);

                    String[] horaEntregaHastaArray = requerimientoBusqueda.horaEntregaHasta.Split(':');
                    DateTime horaEntregaHasta = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaEntregaHastaArray[0]), Int32.Parse(horaEntregaHastaArray[1]), 0);

                    pedido.horaEntregaDesde = requerimientoBusqueda.horaEntregaDesde;
                    pedido.horaEntregaHasta = requerimientoBusqueda.horaEntregaHasta;


                    if (requerimientoBusqueda.horaEntregaAdicionalDesde != null && !requerimientoBusqueda.horaEntregaAdicionalDesde.Equals("")
                    && requerimientoBusqueda.horaEntregaAdicionalDesde != null && !requerimientoBusqueda.horaEntregaAdicionalDesde.Equals(""))
                    {
                        String[] horaEntregaAdicionalDesdeArray = requerimientoBusqueda.horaEntregaAdicionalDesde.Split(':');
                        DateTime horaEntregaAdicionalDesde = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaEntregaAdicionalDesdeArray[0]), Int32.Parse(horaEntregaAdicionalDesdeArray[1]), 0);
                        String[] horaEntregaAdicionalHastaArray = requerimientoBusqueda.horaEntregaAdicionalHasta.Split(':');
                        DateTime horaEntregaAdicionalHasta = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaEntregaAdicionalHastaArray[0]), Int32.Parse(horaEntregaAdicionalHastaArray[1]), 0);
                    }



                    //Revisar
                    pedido.solicitante.idSolicitante = requerimiento.solicitante.idSolicitante;
                    pedido.solicitante.nombre = requerimiento.solicitante.nombre;
                    pedido.solicitante.telefono = requerimiento.solicitante.telefono;
                    pedido.solicitante.correo = requerimiento.solicitante.correo;

                    pedido.montoIGV = requerimiento.montoIGV;
                    pedido.montoTotal = requerimiento.montoTotal;
                    pedido.observaciones = requerimiento.observaciones;
                    pedido.ubigeoEntrega = new Ubigeo();
                    pedido.ubigeoEntrega.Id = requerimiento.direccionEntrega.ubigeo.Id;

                    pedido.seguimientoPedido = new SeguimientoPedido();
                    pedido.seguimientoCrediticioPedido = new SeguimientoCrediticioPedido();
                    pedido.cotizacion = new Cotizacion();
                    pedido.tipoPedido = Pedido.tiposPedido.Venta;
                    pedido.clasePedido = Pedido.ClasesPedido.Venta;
                    pedidoBL.calcularMontosTotales(pedido);
                    pedidoBL.InsertPedido(pedido);
                    /*
                    long numeroPedido = pedido.numeroPedido;
                    String numeroPedidoString = pedido.numeroPedidoString;
                    Guid idPedido = pedido.idPedido;
                    int estado = (int)pedido.seguimientoPedido.estado;
                    String observacion = pedido.seguimientoPedido.observacion;*/
                    pedidoList.Add(pedido);
                }

                





            }
            //var v = new { numeroPedido = numeroPedidoString, estado = estado, observacion = observacion, idPedido = idPedido };


            RequerimientoBL requerimientoBL = new RequerimientoBL();
            requerimientoBL.AprobarRequerimientos(requerimientoBusquedaList, numeroGrupo);


            String resultado = JsonConvert.SerializeObject(pedidoList);
            return resultado;
        }




        public String ConsultarSiExisteRequerimiento()
        {
            Requerimiento requerimiento = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO];
            if (requerimiento == null)
                return "{\"existe\":\"false\",\"numero\":\"0\"}";
            else
                return "{\"existe\":\"true\",\"numero\":\"" + requerimiento.idRequerimiento + "\"}";
        }

        public String Show()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            RequerimientoBL requerimientoBL = new RequerimientoBL();

            Requerimiento requerimiento = new Requerimiento(Requerimiento.ClasesRequerimiento.Venta);
            requerimiento.idRequerimiento = Int32.Parse(Request["idRequerimiento"].ToString());
            requerimiento = requerimientoBL.GetRequerimiento(requerimiento, usuario);
            this.Session[Constantes.VAR_SESSION_REQUERIMIENTO_VER] = requerimiento;

            string jsonUsuario = JsonConvert.SerializeObject(usuario);

            string jsonRequerimiento = JsonConvert.SerializeObject(ParserDTOsShow.RequerimientoToRequerimientoDTO(requerimiento));

            Ciudad ciudad = usuario.sedesMPPedidos.Where(s => s.idCiudad == requerimiento.ciudad.idCiudad).FirstOrDefault();

            string jsonSeries = "[]";
            if (ciudad != null)
            {
                var serieDocumentoElectronicoList = ciudad.serieDocumentoElectronicoList.OrderByDescending(x => x.esPrincipal).ToList();
                jsonSeries = JsonConvert.SerializeObject(serieDocumentoElectronicoList);

            }


            String json = "{\"serieDocumentoElectronicoList\":" + jsonSeries + ", \"requerimiento\":" + jsonRequerimiento + "}";
            return json;
        }

        public void autoGuardarRequerimiento()
        {
            if (this.Session[Constantes.VAR_SESSION_REQUERIMIENTO] != null)
            {
                Requerimiento requerimiento = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO];
                UsuarioBL usuarioBL = new UsuarioBL();
                Usuario usuario = (Usuario)this.Session["usuario"];

                String pedidoSerializado = JsonConvert.SerializeObject(requerimiento);

                usuarioBL.updatePedidoSerializado(usuario, pedidoSerializado);
            }

        }

        public void updateUsuario()
        {
            Requerimiento requerimiento = this.RequerimientoSession;
            requerimiento.usuarioBusqueda = new Usuario { idUsuario = Guid.Parse(this.Request.Params["usuario"]) };
            this.RequerimientoSession = requerimiento;
        }

        public String CreateDireccionTemporal()
        {
            Requerimiento requerimiento = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO];
            DireccionEntrega direccionEntrega = new DireccionEntrega();
            direccionEntrega.descripcion = Request["direccion"];
            direccionEntrega.contacto = Request["contacto"];
            direccionEntrega.telefono = Request["telefono"];
            direccionEntrega.idDireccionEntrega = Guid.Empty;
            requerimiento.cliente.direccionEntregaList.Add(direccionEntrega);
            requerimiento.direccionEntrega = direccionEntrega;
            this.Session[Constantes.VAR_SESSION_REQUERIMIENTO] = requerimiento;
            return JsonConvert.SerializeObject(direccionEntrega);
        }

        public String CreateSolicitanteTemporal()
        {
            Requerimiento requerimiento = (Requerimiento)this.Session[Constantes.VAR_SESSION_REQUERIMIENTO];
            Solicitante solicitante = new Solicitante();
            solicitante.nombre = Request["nombre"];
            solicitante.telefono = Request["telefono"];
            solicitante.correo = Request["correo"];
            solicitante.idSolicitante = Guid.Empty;
            requerimiento.cliente.solicitanteList.Add(solicitante);
            requerimiento.solicitante = solicitante;
            this.Session[Constantes.VAR_SESSION_REQUERIMIENTO] = requerimiento;
            return JsonConvert.SerializeObject(solicitante);
        }




      
    
        public void changeMostrarCosto()
        {
            Requerimiento requerimiento = this.RequerimientoSession;
            requerimiento.mostrarCosto = Boolean.Parse(this.Request.Params["mostrarCosto"]);
            this.RequerimientoSession = requerimiento;
        }
    }
}

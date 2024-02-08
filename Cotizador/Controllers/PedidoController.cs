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
using Model.UTILES;

namespace Cotizador.Controllers
{
    public class PedidoController : ParentController
    {

        private Pedido PedidoSession {
            get {

                Pedido pedido = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaPedidos: pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA]; break;
                    case Constantes.paginas.MantenimientoPedido: pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO]; break;
                    case Constantes.paginas.AprobarPedidos: pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO]; break;
                }
                return pedido;
            }
            set {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaPedidos: this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA] = value; break;
                    case Constantes.paginas.MantenimientoPedido: this.Session[Constantes.VAR_SESSION_PEDIDO] = value; break;
                    case Constantes.paginas.AprobarPedidos: this.Session[Constantes.VAR_SESSION_PEDIDO] = value; break;
                }
            }
        }

        // GET: Pedido

        private void instanciarPedidoBusqueda()
        {
            try
            {
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

                Pedido pedidoTmp = new Pedido(Pedido.ClasesPedido.Venta);
                DateTime fechaDesde = DateTime.Now.AddDays(-Constantes.DIAS_DESDE_BUSQUEDA);
                DateTime fechaHasta = DateTime.Now.AddDays(1);
                pedidoTmp.cotizacion = new Cotizacion();
                pedidoTmp.solicitante = new Solicitante();
                pedidoTmp.direccionEntrega = new DireccionEntrega();

                pedidoTmp.fechaCreacionDesde = new DateTime(fechaDesde.Year, fechaDesde.Month, fechaDesde.Day, 0, 0, 0);
                pedidoTmp.fechaCreacionHasta = fechaHasta;

                pedidoTmp.fechaEntregaDesde = null;// new DateTime(fechaDesde.Year, fechaDesde.Month, fechaDesde.Day, 0, 0, 0);
                pedidoTmp.fechaEntregaHasta = null;// DateTime.Now.AddDays(Constantes.DIAS_DESDE_BUSQUEDA);

                pedidoTmp.fechaProgramacionDesde = null;// new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                pedidoTmp.fechaProgramacionHasta = null;// new DateTime(fechaHasta.Year, fechaHasta.Month, fechaHasta.Day, 23, 59, 59);

                pedidoTmp.buscarSedesGrupoCliente = false;
                pedidoTmp.truncado = 0;

                pedidoTmp.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                pedidoTmp.ciudad = new Ciudad();
                pedidoTmp.cliente = new Cliente();
                pedidoTmp.seguimientoPedido = new SeguimientoPedido();
                pedidoTmp.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.Todos;

                pedidoTmp.responsableComercial = new Vendedor();

                if (usuario.esResponsableComercial && !usuario.modificaFiltroVendedor)
                {
                    pedidoTmp.responsableComercial = usuario.vendedor;
                }

                //Si es un coordinador cargarán por defecto todos los pedidos pendientes de atención
                if (pedidoTmp.usuario.creaGuias)
                {
                    pedidoTmp.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.NoAtendidos;
                }

                pedidoTmp.seguimientoCrediticioPedido = new SeguimientoCrediticioPedido();
                pedidoTmp.seguimientoCrediticioPedido.estado = SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido.Todos;

                pedidoTmp.pedidoDetalleList = new List<PedidoDetalle>();

                pedidoTmp.usuarioBusqueda = new Usuario { idUsuario = Guid.Empty };

                this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA] = pedidoTmp;
                this.Session[Constantes.VAR_SESSION_PEDIDO_LISTA] = new List<Pedido>();
            }
            catch (Exception e)
            {
                logger.Error(e, agregarUsuarioAlMensaje(e.Message));
                throw e;
            }
        }


        private void instanciarPedidoBusquedaAprobacion()
        {
            try
            {
                Pedido pedidoTmp = new Pedido(Pedido.ClasesPedido.Venta);
                DateTime fechaDesde = DateTime.Now.AddDays(-Constantes.DIAS_DESDE_BUSQUEDA);
                DateTime fechaHasta = DateTime.Now.AddDays(1);
                pedidoTmp.cotizacion = new Cotizacion();
                pedidoTmp.solicitante = new Solicitante();
                pedidoTmp.direccionEntrega = new DireccionEntrega();

                pedidoTmp.fechaCreacionDesde = new DateTime(fechaDesde.Year, fechaDesde.Month, fechaDesde.Day, 0, 0, 0);
                pedidoTmp.fechaCreacionHasta = fechaHasta;

                pedidoTmp.fechaEntregaDesde = null;// new DateTime(fechaDesde.Year, fechaDesde.Month, fechaDesde.Day, 0, 0, 0);
                pedidoTmp.fechaEntregaHasta = null;// DateTime.Now.AddDays(Constantes.DIAS_DESDE_BUSQUEDA);

                pedidoTmp.fechaProgramacionDesde = null;// new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                pedidoTmp.fechaProgramacionHasta = null;// new DateTime(fechaHasta.Year, fechaHasta.Month, fechaHasta.Day, 23, 59, 59);

                pedidoTmp.buscarSedesGrupoCliente = false;

                pedidoTmp.ciudad = new Ciudad();
                pedidoTmp.cliente = new Cliente();
                pedidoTmp.seguimientoPedido = new SeguimientoPedido();
                pedidoTmp.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.PendienteAprobacion;
                pedidoTmp.seguimientoCrediticioPedido = new SeguimientoCrediticioPedido();
                pedidoTmp.seguimientoCrediticioPedido.estado = SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido.Todos;

                pedidoTmp.pedidoDetalleList = new List<PedidoDetalle>();
                pedidoTmp.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                pedidoTmp.usuarioBusqueda = new Usuario { idUsuario = Guid.Empty };

                this.Session[Constantes.VAR_SESSION_PEDIDO_APROBACION] = pedidoTmp;
                this.Session[Constantes.VAR_SESSION_PEDIDO_LISTA] = new List<Pedido>();
            }
            catch (Exception e)
            {
                logger.Error(e, agregarUsuarioAlMensaje(e.Message));
                throw e;
            }
        }


        public ActionResult Index(Guid? idPedido = null)
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

                if (this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA] == null)
                {
                    instanciarPedidoBusqueda();
                }

                Pedido pedidoSearch = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA];

                //Si existe cotizacion se debe verificar si no existe cliente
                if (this.Session[Constantes.VAR_SESSION_PEDIDO] != null)
                {
                    Pedido pedidoEdicion = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
                    if (pedidoEdicion.ciudad == null || pedidoEdicion.ciudad.idCiudad == null
                        || pedidoEdicion.ciudad.idCiudad == Guid.Empty)
                    {
                        this.Session[Constantes.VAR_SESSION_PEDIDO] = null;
                    }

                }

                ViewBag.fechaCreacionDesde = pedidoSearch.fechaCreacionDesde.ToString(Constantes.formatoFecha);
                ViewBag.fechaCreacionHasta = pedidoSearch.fechaCreacionHasta.ToString(Constantes.formatoFecha);

                if (pedidoSearch.fechaEntregaDesde != null)
                    ViewBag.fechaEntregaDesde = pedidoSearch.fechaEntregaDesde.Value.ToString(Constantes.formatoFecha);
                else
                    ViewBag.fechaEntregaDesde = null;

                if (pedidoSearch.fechaEntregaHasta != null)
                    ViewBag.fechaEntregaHasta = pedidoSearch.fechaEntregaHasta.Value.ToString(Constantes.formatoFecha);
                else
                    ViewBag.fechaEntregaHasta = null;


                if (pedidoSearch.fechaProgramacionDesde != null)
                    ViewBag.fechaProgramacionDesde = pedidoSearch.fechaProgramacionDesde.Value.ToString(Constantes.formatoFecha);
                else
                    ViewBag.fechaProgramacionDesde = null;

                if (pedidoSearch.fechaProgramacionHasta != null)
                    ViewBag.fechaProgramacionHasta = pedidoSearch.fechaProgramacionHasta.Value.ToString(Constantes.formatoFecha);
                else
                    ViewBag.fechaProgramacionHasta = null;




                if (this.Session[Constantes.VAR_SESSION_PEDIDO_LISTA] == null)
                {
                    this.Session[Constantes.VAR_SESSION_PEDIDO_LISTA] = new List<Pedido>();
                }


                int existeCliente = 0;
                //  if (cotizacion.cliente.idCliente != Guid.Empty || cotizacion.grupo.idGrupo != Guid.Empty)
                if (pedidoSearch.cliente.idCliente != Guid.Empty)
                {
                    existeCliente = 1;
                }

                GuiaRemision guiaRemision = new GuiaRemision();
                guiaRemision.motivoTraslado = GuiaRemision.motivosTraslado.Venta;

                ViewBag.guiaRemision = guiaRemision;

                ViewBag.pedido = pedidoSearch;
                DocumentoVenta documentoVenta = new DocumentoVenta();
                documentoVenta.tipoPago = DocumentoVenta.TipoPago.NoAsignado;
                documentoVenta.formaPago = DocumentoVenta.FormaPago.NoAsignado;
                documentoVenta.fechaEmision = DateTime.Now;
                ViewBag.documentoVenta = documentoVenta;
                ViewBag.fechaEmision = documentoVenta.fechaEmision.Value.ToString(Constantes.formatoFecha);
                ViewBag.horaEmision = documentoVenta.fechaEmision.Value.ToString(Constantes.formatoHora);
                ViewBag.pedidoList = this.Session[Constantes.VAR_SESSION_PEDIDO_LISTA];
                ViewBag.existeCliente = existeCliente;
                ViewBag.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                ViewBag.vistaAgregadoPedidoModals = true;
                ViewBag.pagina = (int)Constantes.paginas.BusquedaPedidos;

                ViewBag.idPedido = idPedido;
                return View();
            }
            catch (Exception e)
            {
                logger.Error(e, agregarUsuarioAlMensaje(e.Message));
                throw e;
            }
        }




        public ActionResult AprobarPedidos(Guid? idPedido = null)
        {
            try
            {
                this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.AprobarPedidos;

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

                if (this.Session[Constantes.VAR_SESSION_PEDIDO_APROBACION] == null)
                {
                    instanciarPedidoBusquedaAprobacion();
                }

                Pedido pedidoSearch = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_APROBACION];

                //Si existe cotizacion se debe verificar si no existe cliente
                if (this.Session[Constantes.VAR_SESSION_PEDIDO] != null)
                {
                    Pedido pedidoEdicion = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
                    if (pedidoEdicion.ciudad == null || pedidoEdicion.ciudad.idCiudad == null
                        || pedidoEdicion.ciudad.idCiudad == Guid.Empty)
                    {
                        this.Session[Constantes.VAR_SESSION_PEDIDO] = null;
                    }

                }

                ViewBag.fechaCreacionDesde = pedidoSearch.fechaCreacionDesde.ToString(Constantes.formatoFecha);
                ViewBag.fechaCreacionHasta = pedidoSearch.fechaCreacionHasta.ToString(Constantes.formatoFecha);

                if (pedidoSearch.fechaEntregaDesde != null)
                    ViewBag.fechaEntregaDesde = pedidoSearch.fechaEntregaDesde.Value.ToString(Constantes.formatoFecha);
                else
                    ViewBag.fechaEntregaDesde = null;

                if (pedidoSearch.fechaEntregaHasta != null)
                    ViewBag.fechaEntregaHasta = pedidoSearch.fechaEntregaHasta.Value.ToString(Constantes.formatoFecha);
                else
                    ViewBag.fechaEntregaHasta = null;


                if (pedidoSearch.fechaProgramacionDesde != null)
                    ViewBag.fechaProgramacionDesde = pedidoSearch.fechaProgramacionDesde.Value.ToString(Constantes.formatoFecha);
                else
                    ViewBag.fechaProgramacionDesde = null;

                if (pedidoSearch.fechaProgramacionHasta != null)
                    ViewBag.fechaProgramacionHasta = pedidoSearch.fechaProgramacionHasta.Value.ToString(Constantes.formatoFecha);
                else
                    ViewBag.fechaProgramacionHasta = null;




                if (this.Session[Constantes.VAR_SESSION_PEDIDO_LISTA] == null)
                {
                    this.Session[Constantes.VAR_SESSION_PEDIDO_LISTA] = new List<Pedido>();
                }


                int existeCliente = 0;
                //  if (cotizacion.cliente.idCliente != Guid.Empty || cotizacion.grupo.idGrupo != Guid.Empty)
                if (pedidoSearch.cliente.idCliente != Guid.Empty)
                {
                    existeCliente = 1;
                }

                GuiaRemision guiaRemision = new GuiaRemision();
                guiaRemision.motivoTraslado = GuiaRemision.motivosTraslado.Venta;

                ViewBag.guiaRemision = guiaRemision;

                ViewBag.pedido = pedidoSearch;
                DocumentoVenta documentoVenta = new DocumentoVenta();
                documentoVenta.tipoPago = DocumentoVenta.TipoPago.NoAsignado;
                documentoVenta.formaPago = DocumentoVenta.FormaPago.NoAsignado;
                documentoVenta.fechaEmision = DateTime.Now;
                ViewBag.documentoVenta = documentoVenta;
                ViewBag.fechaEmision = documentoVenta.fechaEmision.Value.ToString(Constantes.formatoFecha);
                ViewBag.horaEmision = documentoVenta.fechaEmision.Value.ToString(Constantes.formatoHora);
                ViewBag.pedidoList = this.Session[Constantes.VAR_SESSION_PEDIDO_LISTA];
                ViewBag.existeCliente = existeCliente;

                ViewBag.pagina = (int)Constantes.paginas.BusquedaPedidos;

                ViewBag.idPedido = idPedido;
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

                if (this.Session[Constantes.VAR_SESSION_PEDIDO] == null)
                {

                    instanciarPedido();
                }
                Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];


                int existeCliente = 0;
                if (pedido.cliente.idCliente != Guid.Empty)
                {
                    existeCliente = 1;
                }

                ViewBag.existeCliente = existeCliente;
                ViewBag.idClienteGrupo = pedido.cliente.idCliente;
                ViewBag.clienteGrupo = pedido.cliente.ToString();

                ViewBag.fechaSolicitud = pedido.fechaSolicitud.ToString(Constantes.formatoFecha);
                ViewBag.horaSolicitud = pedido.fechaSolicitud.ToString(Constantes.formatoHora);

                ViewBag.fechaEntregaDesde = pedido.fechaEntregaDesde == null ? "" : pedido.fechaEntregaDesde.Value.ToString(Constantes.formatoFecha);
                ViewBag.fechaEntregaHasta = pedido.fechaEntregaHasta == null ? "" : pedido.fechaEntregaHasta.Value.ToString(Constantes.formatoFecha);



                ViewBag.pedido = pedido;
                ViewBag.VARIACION_PRECIO_ITEM_PEDIDO = Constantes.VARIACION_PRECIO_ITEM_PEDIDO;

                ViewBag.fechaPrecios = DateTime.Now.AddDays(-Constantes.DIAS_MAX_BUSQUEDA_PRECIOS).ToString(Constantes.formatoFecha);
                //   ViewBag.fechaPrecios = pedido.fechaPrecios.ToString(Constantes.formatoFecha);

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
                this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.MantenimientoPedido;

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

                if (this.Session[Constantes.VAR_SESSION_PEDIDO] == null)
                {

                    instanciarPedido();
                }
                Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];


                int existeCliente = 0;
                if (pedido.cliente.idCliente != Guid.Empty)
                {
                    existeCliente = 1;
                }

                if(!pedido.idPedido.Equals(Guid.Empty))
                {
                    bool existeDireccionEntrega = false;
                    foreach (DireccionEntrega dir in pedido.cliente.direccionEntregaList)
                    {
                        if (pedido.direccionEntrega.idDireccionEntrega.Equals(dir.idDireccionEntrega))
                        {
                            existeDireccionEntrega = true;
                        }
                    }

                    if (!existeDireccionEntrega)
                    {
                        pedido.cliente.direccionEntregaList.Add(pedido.direccionEntrega);
                    }

                    bool existeSolicitante = false;
                    foreach (Solicitante sol in pedido.cliente.solicitanteList)
                    {
                        if (pedido.solicitante.idSolicitante.Equals(sol.idSolicitante))
                        {
                            existeSolicitante = true;
                        }
                    }

                    if (!existeSolicitante)
                    {
                        pedido.cliente.solicitanteList.Add(pedido.solicitante);
                    }
                }

                ViewBag.existeCliente = existeCliente;
                ViewBag.idClienteGrupo = pedido.cliente.idCliente;
                ViewBag.clienteGrupo = pedido.cliente.ToString();

                ViewBag.fechaSolicitud = pedido.fechaSolicitud.ToString(Constantes.formatoFecha);
                ViewBag.horaSolicitud = pedido.fechaSolicitud.ToString(Constantes.formatoHora);

                ViewBag.fechaEntregaDesde = pedido.fechaEntregaDesde == null ? "" : pedido.fechaEntregaDesde.Value.ToString(Constantes.formatoFecha);
                ViewBag.fechaEntregaHasta = pedido.fechaEntregaHasta == null ? "" : pedido.fechaEntregaHasta.Value.ToString(Constantes.formatoFecha);



                ViewBag.pedido = pedido;
                ViewBag.VARIACION_PRECIO_ITEM_PEDIDO = Constantes.VARIACION_PRECIO_ITEM_PEDIDO;

                ViewBag.fechaPrecios = DateTime.Now.AddDays(-Constantes.DIAS_MAX_BUSQUEDA_PRECIOS).ToString(Constantes.formatoFecha);

                ViewBag.busquedaProductosIncluyeDescontinuados = 0;
                if (this.Session[Constantes.VAR_SESSION_PRODUCTO_SEARCH_PARAM + "incluyeDescontinuados"] != null)
                {
                    ViewBag.busquedaProductosIncluyeDescontinuados = int.Parse(this.Session[Constantes.VAR_SESSION_PRODUCTO_SEARCH_PARAM + "incluyeDescontinuados"].ToString());
                }

                ViewBag.pagina = (int)Constantes.paginas.MantenimientoPedido;
                ViewBag.promocionSelect = pedido.promocion;

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
            //try
            //{
            if (this.Session[Constantes.VAR_SESSION_PEDIDO] == null)
            {

                instanciarPedido();
            }
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];

            Cotizacion cotizacion = (Cotizacion)this.Session[Constantes.VAR_SESSION_COTIZACION_VER];

            if (cotizacion.tipoCotizacion != Cotizacion.TiposCotizacion.Transitoria || !cotizacion.estaVencida)
            {
                pedido.cotizacion = new Cotizacion();
                pedido.cotizacion.idCotizacion = cotizacion.idCotizacion;
                pedido.cotizacion.codigo = cotizacion.codigo;
            }

            pedido.ciudad = cotizacion.ciudad;
            pedido.cliente = cotizacion.cliente;
            pedido.esPagoContado = cotizacion.esPagoContado;
            pedido.moneda = cotizacion.moneda;

            if (cotizacion.cliente.horaInicioPrimerTurnoEntrega != null && !cotizacion.cliente.horaInicioPrimerTurnoEntrega.Equals("00:00:00"))
            {
                pedido.horaEntregaDesde = cotizacion.cliente.horaInicioPrimerTurnoEntregaFormat;
            }
            if (cotizacion.cliente.horaFinPrimerTurnoEntrega != null && !cotizacion.cliente.horaFinPrimerTurnoEntrega.Equals("00:00:00"))
            {
                pedido.horaEntregaHasta = cotizacion.cliente.horaFinPrimerTurnoEntregaFormat;
            }

            if (cotizacion.cliente.horaInicioSegundoTurnoEntrega != null && !cotizacion.cliente.horaInicioSegundoTurnoEntrega.Equals("00:00:00"))
            {
                pedido.horaEntregaAdicionalDesde = cotizacion.cliente.horaInicioSegundoTurnoEntregaFormat;
            }
            if (cotizacion.cliente.horaFinSegundoTurnoEntrega != null && !cotizacion.cliente.horaFinSegundoTurnoEntrega.Equals("00:00:00"))
            {
                pedido.horaEntregaAdicionalHasta = cotizacion.cliente.horaFinSegundoTurnoEntregaFormat;
            }

            //if(cotizacion.promocion != null && !cotizacion.promocion.idPromocion.Equals(Guid.Empty))
            //{
            //    pedido.promocion = cotizacion.promocion;
            //}

            pedido.promociones = cotizacion.promociones;

            SolicitanteBL solicitanteBL = new SolicitanteBL();
            pedido.cliente.solicitanteList = solicitanteBL.getSolicitantes(cotizacion.cliente.idCliente);

            DireccionEntregaBL direccionEntregaBL = new DireccionEntregaBL();
            pedido.cliente.direccionEntregaList = direccionEntregaBL.getDireccionesEntrega(cotizacion.cliente.idCliente);
            pedido.direccionEntrega = new DireccionEntrega();

            pedido.observaciones = "Pedido generado desde la cotización " + cotizacion.codigo + ".";

            pedido.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            pedido.seguimientoPedido = new SeguimientoPedido();

            pedido.pedidoDetalleList = new List<PedidoDetalle>();
            foreach (DocumentoDetalle documentoDetalle in cotizacion.documentoDetalle)
            {
                PedidoDetalle pedidoDetalle = new PedidoDetalle(pedido.usuario.visualizaCostos, pedido.usuario.visualizaMargen);
                pedidoDetalle.cantidad = documentoDetalle.cantidad;
                if (documentoDetalle.cantidad == 0)
                    pedidoDetalle.cantidad = 1;

                //pedidoDetalle.costoAnterior = documentoDetalle.costoAnterior;
                pedidoDetalle.esPrecioAlternativo = documentoDetalle.esPrecioAlternativo;
                pedidoDetalle.flete = documentoDetalle.flete;
                pedidoDetalle.observacion = documentoDetalle.observacion;



                // pedidoDetalle.porcentajeDescuento = documentoDetalle.porcentajeDescuento;


                pedidoDetalle.producto = documentoDetalle.producto;


                ProductoBL productoBl = new ProductoBL();

                if (cotizacion.tipoCotizacion == Cotizacion.TiposCotizacion.Transitoria)
                {
                    if (!cotizacion.estaVencida) {
                        if (pedidoDetalle.producto.precioClienteProducto == null)
                        {
                            pedidoDetalle.producto.precioClienteProducto = new PrecioClienteProducto();
                        }
                        //Se le asigna un id temporal solo para que no se rechaza el precio en la validación
                        pedidoDetalle.producto.precioClienteProducto.idPrecioClienteProducto = Guid.NewGuid();
                        pedidoDetalle.producto.precioClienteProducto.fechaInicioVigencia = cotizacion.fechaInicioVigenciaPrecios;
                        pedidoDetalle.producto.precioClienteProducto.fechaFinVigencia =
                            cotizacion.fechaFinVigenciaPrecios.HasValue ?
                                cotizacion.fechaFinVigenciaPrecios.Value :
                                cotizacion.fechaInicioVigenciaPrecios.HasValue ?
                                    cotizacion.fechaInicioVigenciaPrecios.Value.AddDays(Constantes.DIAS_MAX_VALIDEZ_OFERTA_COTIZACION_PUNTUAL) :
                                    DateTime.Now.AddDays(Constantes.DIAS_MAX_VALIDEZ_OFERTA_COTIZACION_PUNTUAL);
                        //pedidoDetalle.producto.precioClienteProducto.cliente.idCliente = cotizacion.cliente.idCliente;
                        pedidoDetalle.producto.precioClienteProducto.precioUnitario = documentoDetalle.precioUnitario;
                        pedidoDetalle.producto.precioClienteProducto.precioNeto = documentoDetalle.precioNeto;
                        pedidoDetalle.producto.precioClienteProducto.esUnidadAlternativa = documentoDetalle.esPrecioAlternativo;

                        int diaFinVigenciaCot = int.Parse(pedidoDetalle.producto.precioClienteProducto.fechaFinVigencia.Value.ToString("yyyyMMdd"));
                        int diaFechaHoy = int.Parse(DateTime.Now.ToString("yyyyMMdd"));

                        if (diaFinVigenciaCot < diaFechaHoy)
                        {
                            ProductoBL productoBL = new ProductoBL();
                            Producto producto = productoBL.getProducto(pedidoDetalle.producto.idProducto, pedido.ciudad.esProvincia, pedido.incluidoIGV, pedido.cliente.idCliente);

                            int diaFinVigProd = 0;
                            if (producto.precioClienteProducto != null && producto.precioClienteProducto.fechaInicioVigencia != null)
                            {
                                if (producto.precioClienteProducto.fechaFinVigencia == null)
                                {
                                    diaFinVigProd = int.Parse(producto.precioClienteProducto.fechaInicioVigencia.Value.AddDays(Constantes.DIAS_MAX_VIGENCIA_PRECIOS_COTIZACION).ToString("yyyyMMdd"));
                                } else {
                                    diaFinVigProd = int.Parse(producto.precioClienteProducto.fechaFinVigencia.Value.ToString("yyyyMMdd"));
                                }
                                //decimal precioProd = producto.precioClienteProducto.precioNeto * producto.precioClienteProducto.equivalencia;
                                //decimal precioCot = pedidoDetalle.producto.precioClienteProducto.precioNeto * pedidoDetalle.producto.precioClienteProducto.equivalencia;

                                if (diaFinVigProd > diaFinVigenciaCot)
                                {
                                    pedidoDetalle.producto = producto;
                                }

                                if (pedidoDetalle.esPrecioAlternativo)
                                {
                                    pedidoDetalle.producto.precioClienteProducto.precioUnitario = pedidoDetalle.producto.precioClienteProducto.precioUnitario / pedidoDetalle.producto.precioClienteProducto.equivalencia;
                                }
                            }
                        }
                    } else
                    {
                        Producto producto = productoBl.getProducto(documentoDetalle.producto.idProducto, pedido.ciudad.esProvincia, pedido.incluidoIGV, pedido.cliente.idCliente);
                        pedidoDetalle.producto = producto;
                        if (documentoDetalle.esPrecioAlternativo)
                        {
                            pedidoDetalle.producto.precioClienteProducto.precioUnitario = pedidoDetalle.producto.precioClienteProducto.precioUnitario / documentoDetalle.ProductoPresentacion.Equivalencia;
                        }
                    }
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
                        pedidoDetalle.producto.precioClienteProducto.precioUnitario = pedidoDetalle.producto.precioClienteProducto.precioUnitario / documentoDetalle.ProductoPresentacion.Equivalencia;
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

                pedido.pedidoDetalleList.Add(pedidoDetalle);
            }
            pedido.fechaPrecios = pedido.fechaSolicitud.AddDays(Constantes.DIAS_MAX_BUSQUEDA_PRECIOS * -1);
            PedidoBL pedidoBL = new PedidoBL();
            pedidoBL.calcularMontosTotales(pedido);
            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
            //}
            //catch (Exception e)
            //{
            //    logger.Error(e, agregarUsuarioAlMensaje(e.Message));
            //    throw e;
            //}

        }

        
        public void iniciarEdicionPedidoDesdeOC ()
        {
            //try
            //{

            ClienteBL clienteBl = new ClienteBL();

            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_ORDEN_COMPRA_CLIENTE_PEDIDO_GENERAR];


            pedido.cliente = clienteBl.getCliente(pedido.cliente.idCliente);

            pedido.ciudad = pedido.cliente.ciudad;
            pedido.idPedido = Guid.Empty;
            pedido.numeroPedido = 0;
            pedido.numeroGrupoPedido = null;
            pedido.cotizacion = new Cotizacion();
            pedido.ubigeoEntrega = new Ubigeo();
            pedido.ubigeoEntrega.Id = "000000";
            pedido.esPagoContado = false;

            pedido.tipoPedido = Pedido.tiposPedido.Venta;
            pedido.ciudadASolicitar = new Ciudad();

            pedido.direccionEntrega = new DireccionEntrega();
            pedido.solicitante = new Solicitante();
            pedido.fechaSolicitud = DateTime.Now;
            pedido.fechaEntregaDesde = null;
            pedido.fechaEntregaHasta = null;
            pedido.horaEntregaDesde = "09:00";
            pedido.horaEntregaHasta = "18:00";
            pedido.contactoPedido = String.Empty;
            pedido.telefonoContactoPedido = String.Empty;
            pedido.incluidoIGV = false;
            //  pedido.tasaIGV = Constantes.IGV;
            //pedido.flete = 0;
            // pedido.mostrarCodigoProveedor = true;
            pedido.observaciones = String.Empty;

            pedido.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            pedido.seguimientoPedido = new SeguimientoPedido();
            pedido.seguimientoCrediticioPedido = new SeguimientoCrediticioPedido();
            pedido.pedidoAdjuntoList = new List<PedidoAdjunto>();
            pedido.fechaPrecios = pedido.fechaSolicitud.AddDays(Constantes.DIAS_MAX_BUSQUEDA_PRECIOS * -1);


            if (pedido.cliente.horaInicioPrimerTurnoEntrega != null && !pedido.cliente.horaInicioPrimerTurnoEntrega.Equals("00:00:00"))
            {
                pedido.horaEntregaDesde = pedido.cliente.horaInicioPrimerTurnoEntregaFormat;
            } 
            if (pedido.cliente.horaFinPrimerTurnoEntrega != null && !pedido.cliente.horaFinPrimerTurnoEntrega.Equals("00:00:00"))
            {
                pedido.horaEntregaHasta = pedido.cliente.horaFinPrimerTurnoEntregaFormat;
            } 

            if (pedido.cliente.horaInicioSegundoTurnoEntrega != null && !pedido.cliente.horaInicioSegundoTurnoEntrega.Equals("00:00:00"))
            {
                pedido.horaEntregaAdicionalDesde = pedido.cliente.horaInicioSegundoTurnoEntregaFormat;
            }
            if (pedido.cliente.horaFinSegundoTurnoEntrega != null && !pedido.cliente.horaFinSegundoTurnoEntrega.Equals("00:00:00"))
            {
                pedido.horaEntregaAdicionalHasta = pedido.cliente.horaFinSegundoTurnoEntregaFormat;
            }


            SolicitanteBL solicitanteBL = new SolicitanteBL();
            pedido.cliente.solicitanteList = solicitanteBL.getSolicitantes(pedido.cliente.idCliente);

            DireccionEntregaBL direccionEntregaBL = new DireccionEntregaBL();
            pedido.cliente.direccionEntregaList = direccionEntregaBL.getDireccionesEntrega(pedido.cliente.idCliente);
            pedido.direccionEntrega = new DireccionEntrega();

            pedido.observaciones = String.Empty;

            pedido.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            pedido.seguimientoPedido = new SeguimientoPedido();

            List<PedidoDetalle> detalles = new List<PedidoDetalle>();

            foreach (PedidoDetalle pedidoDetalle in pedido.pedidoDetalleList)
            {
                if (pedidoDetalle.cantidad > 0)
                {
                    if (pedidoDetalle.producto.precioClienteProducto.esUnidadAlternativa)
                    {
                        pedidoDetalle.producto.precioClienteProducto.precioUnitario = pedidoDetalle.producto.precioClienteProducto.precioUnitario / pedidoDetalle.producto.precioClienteProducto.equivalencia;
                    }

                    pedidoDetalle.porcentajeDescuento = 100 - (pedidoDetalle.precioNeto * 100 / pedidoDetalle.precioLista);

                    detalles.Add(pedidoDetalle);
                }
            }

            pedido.pedidoDetalleList = detalles;
            PedidoBL pedidoBL = new PedidoBL();
            pedidoBL.calcularMontosTotales(pedido);
            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;

            //}
            //catch (Exception e)
            //{
            //    logger.Error(e, agregarUsuarioAlMensaje(e.Message));
            //    throw e;
            //}

        }

        private void instanciarPedido()
        {
            try
            {
                Pedido pedido = new Pedido(Pedido.ClasesPedido.Venta);
                pedido.idPedido = Guid.Empty;
                pedido.numeroPedido = 0;
                pedido.numeroGrupoPedido = null;
                pedido.cotizacion = new Cotizacion();
                pedido.ubigeoEntrega = new Ubigeo();
                pedido.ubigeoEntrega.Id = "000000";
                pedido.ciudad = new Ciudad();
                pedido.cliente = new Cliente();
                pedido.esPagoContado = false;
                pedido.esVentaIndirecta = false;

                pedido.promocion = new Promocion();
                pedido.promocion.idPromocion = Guid.Empty;

                pedido.promociones = new List<Promocion>();

                pedido.tipoPedido = Pedido.tiposPedido.Venta;
                pedido.ciudadASolicitar = new Ciudad();
                
                pedido.moneda = Moneda.ListaMonedas.Where(m => m.codigo.Equals("PEN")).FirstOrDefault();

                pedido.numeroReferenciaCliente = null;
                pedido.direccionEntrega = new DireccionEntrega();
                pedido.solicitante = new Solicitante();
                pedido.fechaSolicitud = DateTime.Now;
                pedido.fechaEntregaDesde = null;
                pedido.fechaEntregaHasta = null;
                pedido.horaEntregaDesde = "09:00";
                pedido.horaEntregaHasta = "18:00";
                pedido.contactoPedido = String.Empty;
                pedido.telefonoContactoPedido = String.Empty;
                pedido.incluidoIGV = false;
              //  pedido.tasaIGV = Constantes.IGV;
                //pedido.flete = 0;
               // pedido.mostrarCodigoProveedor = true;
                pedido.observaciones = String.Empty;

                pedido.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                pedido.seguimientoPedido = new SeguimientoPedido();
                pedido.seguimientoCrediticioPedido = new SeguimientoCrediticioPedido();
                pedido.pedidoDetalleList = new List<PedidoDetalle>();
                pedido.pedidoAdjuntoList = new List<PedidoAdjunto>();
                pedido.fechaPrecios = pedido.fechaSolicitud.AddDays(Constantes.DIAS_MAX_BUSQUEDA_PRECIOS * -1);

                this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
            }
            catch (Exception e)
            {
                logger.Error(e, agregarUsuarioAlMensaje(e.Message));
                throw e;
            }
        }

        public void iniciarEdicionPedido()
        {
            try
            {
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                Pedido pedidoVer = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_VER];
                PedidoBL pedidoBL = new PedidoBL();
                Pedido pedido = new Pedido(Pedido.ClasesPedido.Venta);
                pedido.idPedido = pedidoVer.idPedido;
                //    pedido.fechaModificacion = cotizacionVer.fechaModificacion;
                pedido.seguimientoPedido = new SeguimientoPedido();
                pedido.usuario = (Usuario)this.Session["usuario"];
                //Se cambia el estado de la cotizacion a Edición
                pedido.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.Edicion;
                pedidoBL.cambiarEstadoPedido(pedido);
                //Se obtiene los datos de la cotización ya modificada
                pedido = pedidoBL.GetPedidoParaEditar(pedido,usuario);
                //Temporal
                pedido.ciudadASolicitar = new Ciudad();
           
              /*  if (pedido.tipoPedido == Pedido.tiposPedido.TrasladoInterno)
                {
                    pedido.ciudadASolicitar = new Ciudad { idCiudad = pedido.ciudad.idCiudad,
                                                            nombre = pedido.ciudad.nombre,
                                                            esProvincia = pedido.ciudad.esProvincia };

                    pedido.ciudad = pedido.cliente.ciudad;
                }*/

                this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
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
                Pedido pedido = this.PedidoSession;
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                pedido.usuario = usuario;
                PedidoBL pedidoBL = new PedidoBL();

                String[] fechaPrecios = this.Request.Params["fecha"].Split('/');
                pedido.fechaPrecios = new DateTime(Int32.Parse(fechaPrecios[2]), Int32.Parse(fechaPrecios[1]), Int32.Parse(fechaPrecios[0]), 0, 0, 0);

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

                pedido = pedidoBL.obtenerProductosAPartirdePreciosRegistrados(pedido, familia, proveedor,usuario);
                pedidoBL.calcularMontosTotales(pedido);
                this.PedidoSession = pedido;
            }
            catch (Exception e)
            {
                logger.Error(e, agregarUsuarioAlMensaje(e.Message));
                throw e;
            }
        }

        [HttpGet]
        public ActionResult CargarProductosCanasta(int tipo)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            Pedido pedido = this.PedidoSession;
            PedidoBL pedidoBL = new PedidoBL();

            pedidoBL.obtenerProductosAPartirdePreciosCotizados(pedido, tipo==2, usuario);

            return RedirectToAction("Pedir", "Pedido");
        }

        public String SearchProductos()
        {
            try
            {
                String texto_busqueda = this.Request.Params["data[q]"];
                ProductoBL bl = new ProductoBL();

                int incluyeDescontinuados = 0;
                if (this.Session[Constantes.VAR_SESSION_PRODUCTO_SEARCH_PARAM + "incluyeDescontinuados"] != null)
                {
                    incluyeDescontinuados = int.Parse(this.Session[Constantes.VAR_SESSION_PRODUCTO_SEARCH_PARAM + "incluyeDescontinuados"].ToString());
                }

                Pedido pedido = this.PedidoSession;
                String resultado = bl.getProductosBusqueda(texto_busqueda, false, this.Session["proveedor"] != null ? (String)this.Session["proveedor"] : "Todos", this.Session["familia"] != null ? (String)this.Session["familia"] : "Todas", pedido.tipoPedido, incluyeDescontinuados);
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
            this.Session[Constantes.VAR_SESSION_PRODUCTO_SEARCH_PARAM + parametro] = valor;
        }


        public ActionResult CancelarCreacionPedido()
        {
            try
            {
                this.Session[Constantes.VAR_SESSION_PEDIDO] = null;
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
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

                ClienteBL clienteBL = new ClienteBL();
                Pedido pedido = this.PedidoSession;
                return clienteBL.getCLientesBusqueda(data, pedido.ciudad.idCiudad, usuario.idUsuario, true);
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

                Pedido pedido = this.PedidoSession; 
                Guid idCliente = Guid.Parse(Request["idCliente"].ToString());
                ClienteBL clienteBl = new ClienteBL();
                pedido.cliente = clienteBl.getCliente(idCliente);

                if (pedido.cliente.correoEnvioFactura == null || (pedido.cliente.correoEnvioFactura != null && pedido.cliente.correoEnvioFactura.Trim().Length == 0))
                {
                    pedido.cliente = new Cliente();
                }
                else
                {
                    //Se obtiene la lista de direccioines de entrega registradas para el cliente
                    DireccionEntregaBL direccionEntregaBL = new DireccionEntregaBL();
                    pedido.cliente.direccionEntregaList = direccionEntregaBL.getDireccionesEntrega(idCliente);

                    SolicitanteBL solicitanteBL = new SolicitanteBL();
                    pedido.cliente.solicitanteList = solicitanteBL.getSolicitantes(idCliente);

                    pedido.direccionEntrega = new DireccionEntrega();
                    pedido.horaEntregaDesde = pedido.cliente.horaInicioPrimerTurnoEntrega;
                    pedido.horaEntregaHasta = pedido.cliente.horaFinPrimerTurnoEntrega;
                    pedido.horaEntregaAdicionalDesde = pedido.cliente.horaInicioSegundoTurnoEntrega;
                    pedido.horaEntregaAdicionalHasta = pedido.cliente.horaFinSegundoTurnoEntrega;

                    //Se limpia el ubigeo de entrega
                    pedido.ubigeoEntrega = new Ubigeo();
                    pedido.ubigeoEntrega.Id = Constantes.UBIGEO_VACIO;

                    pedido.facturaUnica = pedido.cliente.facturaUnica;
                
                }
                this.PedidoSession = pedido;
                String resultado = JsonConvert.SerializeObject(pedido.cliente);
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
                Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
                //Se recupera el producto y se guarda en Session

                Guid idProducto = Guid.Parse(this.Request.Params["idProducto"].ToString());
                //Guid idProducto = Guid.Parse(Request["idProducto"].ToString());
                this.Session["idProducto"] = idProducto.ToString();

                TipoCambioSunatBL tcBl = new TipoCambioSunatBL();
                TipoCambioSunat tc = tcBl.GetTipoCambioHoy();

                //Para recuperar el producto se envia si la sede seleccionada es provincia o no
                ProductoBL bl = new ProductoBL();
                Producto producto = bl.getProducto(idProducto, pedido.ciudad.esProvincia, pedido.incluidoIGV, pedido.cliente.idCliente, false, pedido.moneda.codigo, tc);

                ParametroBL parametroBL = new ParametroBL();
                string activaUnidProvSub = parametroBL.getParametro("PEDIDO_UNID_PROV_SUBDISTRIBUIDOR");

                int selectUnidadProv = 0;
                if (pedido.cliente != null && pedido.cliente.esSubDistribuidor && activaUnidProvSub.ToUpper().Equals("SI"))
                {
                    selectUnidadProv = 1;
                }

                Decimal fleteDetalle = Decimal.Parse(String.Format(Constantes.formatoCuatroDecimales, producto.costoLista * (0) / 100));
                Decimal precioUnitario = Decimal.Parse(String.Format(Constantes.formatoCuatroDecimales, fleteDetalle + producto.precioLista));

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
                    "\"sku\":\"" + producto.sku + "\"," +
                    "\"image\":\"data:image/png;base64, " + Convert.ToBase64String(producto.image) + "\"," +
                    "\"unidad\":\"" + producto.unidad + "\"," +
                    "\"unidad_alternativa\":\"" + producto.unidad_alternativa + "\"," +
                    "\"selectUnidadProv\":" + selectUnidadProv + "," +
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
                Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
                Guid idProducto = Guid.Parse(this.Session["idProducto"].ToString());
                PedidoDetalle pedidoDetalle = pedido.pedidoDetalleList.Where(p => p.producto.idProducto == idProducto).FirstOrDefault();
                if (pedidoDetalle != null)
                {
                    String mensajeError = "Producto ya se encuentra en la lista.";
                    logger.Error(agregarUsuarioAlMensaje(mensajeError));
                    throw new System.Exception(mensajeError);
                }

                PedidoDetalle detalle = new PedidoDetalle(pedido.usuario.visualizaCostos, pedido.usuario.visualizaMargen);
                ProductoBL productoBL = new ProductoBL();
                Producto producto = productoBL.getProducto(idProducto, pedido.ciudad.esProvincia, pedido.incluidoIGV, pedido.cliente.idCliente);

                if (pedido.clasePedido == Pedido.ClasesPedido.Venta)
                {
                    String mensajeError = "Tipo de Producto no concuerda con el tipo de Pedido.";
                    if ((pedido.tipoPedido == Pedido.tiposPedido.Venta || pedido.tipoPedido == Pedido.tiposPedido.TransferenciaGratuitaEntregada) && producto.tipoProducto == Producto.TipoProducto.Comodato)
                    {
                        logger.Error(agregarUsuarioAlMensaje(mensajeError));
                        throw new System.Exception(mensajeError);
                    }
                    if (pedido.tipoPedido == Pedido.tiposPedido.ComodatoEntregado && producto.tipoProducto != Producto.TipoProducto.Comodato)
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
                pedido.pedidoDetalleList.Add(detalle);

                //CotizacionDetalle cotizacionDetalle = (CotizacionDetalle)Convert.ChangeType(pedido, typeof(CotizacionDetalle));

                //Calcula los montos totales de la cabecera de la cotizacion
                PedidoBL pedidoBL = new PedidoBL();
                pedidoBL.calcularMontosTotales(pedido);




                var nombreProducto = detalle.producto.descripcion;
               /* if (pedido.mostrarCodigoProveedor)
                {*/
                    nombreProducto = detalle.producto.skuProveedor + " - " + detalle.producto.descripcion;
                //    }

                /*  if (pedido.considerarCantidades == Cotizacion.OpcionesConsiderarCantidades.Ambos)
                  {*/
                //      nombreProducto = nombreProducto + "\\n" + detalle.observacion;
                //}

                /* String resultado = "{" +
                     "\"idProducto\":\"" + detalle.producto.idProducto + "\"," +
                     "\"codigoProducto\":\"" + detalle.producto.sku + "\"," +
                     "\"nombreProducto\":\"" + nombreProducto + "\"," +
                     "\"unidad\":\"" + detalle.unidad + "\"," +
                     "\"igv\":\"" + pedido.montoIGV.ToString() + "\", " +
                     "\"subTotal\":\"" + pedido.montoSubTotal.ToString() + "\", " +
                     "\"margen\":\"" + detalle.margen + "\", " +
                     "\"precioUnitario\":\"" + detalle.precioUnitario + "\", " +
                     "\"observacion\":\"" + detalle.observacion + "\", " +
                     "\"total\":\"" + pedido.montoTotal.ToString() + "\"}";*/

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
                    descripcionLarga = detalle.producto.descripcionLarga,
                    unidad = detalle.unidad,
                    igv = pedido.montoIGV.ToString(),
                    subTotal = pedido.montoSubTotal.ToString(),
                    margen = detalle.margen,
                    precioUnitario = detalle.precioUnitario,
                    descontinuado = detalle.producto.descontinuado,
                    motivoRestriccion = detalle.producto.motivoRestriccion,
                    observacion = detalle.observacion,
                    total = pedido.montoTotal.ToString(),
                    idProductoPresentacion = detalle.esPrecioAlternativo ? detalle.ProductoPresentacion.IdProductoPresentacion : 0,
                    precioUnitarioRegistrado = precioUnitarioRegistrado

                };

                this.PedidoSession = pedido;
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

        public string ChangeMoneda()
        {
            Pedido pedido = (Pedido) this.PedidoSession; 
            string moneda = this.Request.Params["moneda"];

            pedido.moneda = Moneda.ListaMonedas.Where(m => m.codigo.Equals(moneda)).FirstOrDefault();
            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA] = pedido;


            return "{\"simbolo\":\"" + (pedido.moneda != null ? pedido.moneda.simbolo : "") + "\"}";
        }

        public void ChangeInputString()
        {
            Pedido pedido = this.PedidoSession;
            PropertyInfo propertyInfo = pedido.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(pedido, this.Request.Params["valor"]);
            this.PedidoSession = pedido;
        }

        public void ChangeInputInt()
        {
            Pedido pedido = this.PedidoSession;
            PropertyInfo propertyInfo = pedido.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(pedido, Int32.Parse(this.Request.Params["valor"]));
            this.PedidoSession = pedido;
        }

        public void ChangeInputBool()
        {
            Pedido pedido = this.PedidoSession;
            PropertyInfo propertyInfo = pedido.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(pedido, Int32.Parse(this.Request.Params["valor"]) == 1 ? true : false);
            this.PedidoSession = pedido;
        }

        public void ChangeUbigeoEntrega()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            pedido.ubigeoEntrega.Id = this.Request.Params["ubigeoEntregaId"];
            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
        }
      

        public void ChangeNumeroReferenciaCliente()
        {
            Pedido pedido = this.PedidoSession;
            pedido.numeroReferenciaCliente = this.Request.Params["numeroReferenciaCliente"];
            this.PedidoSession = pedido;
        }

        public void ChangeOtrosCargos()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            pedido.otrosCargos = Decimal.Parse(this.Request.Params["otrosCargos"]);
            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
        }
        

        public string ChangeDireccionEntrega()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];

            if (this.Request.Params["idDireccionEntrega"] == null || this.Request.Params["idDireccionEntrega"].Equals(String.Empty))
            {
                pedido.direccionEntrega = new DireccionEntrega();
            }
            else
            { 
                Guid idDireccionEntrega = Guid.Parse(this.Request.Params["idDireccionEntrega"]);
                pedido.direccionEntrega = pedido.cliente.direccionEntregaList.Where(d => d.idDireccionEntrega == idDireccionEntrega).FirstOrDefault();
                pedido.ubigeoEntrega = pedido.direccionEntrega.ubigeo;

            }

            pedido.existeCambioDireccionEntrega = false;
            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
            return JsonConvert.SerializeObject(pedido.direccionEntrega);
        }

        public void ChangeDireccionEntregaDescripcion()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            pedido.direccionEntrega.descripcion = this.Request.Params["direccionEntregaDescripcion"];
            pedido.existeCambioDireccionEntrega = true;
            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
        }

        public void ChangeDireccionEntregaContacto()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            pedido.direccionEntrega.contacto = this.Request.Params["direccionEntregaContacto"];
            pedido.existeCambioDireccionEntrega = true;
            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
        }

        public void ChangeDireccionEntregaTelefono()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            pedido.direccionEntrega.telefono = this.Request.Params["direccionEntregaTelefono"];
            pedido.existeCambioDireccionEntrega = true;
            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
        }



        public string ChangeSolicitante()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];

            if (this.Request.Params["idSolicitante"] == null || this.Request.Params["idSolicitante"].Equals(String.Empty))
            {
                pedido.solicitante = new Solicitante();
            }
            else
            {
                Guid idSolicitante = Guid.Parse(this.Request.Params["idSolicitante"]);
                pedido.solicitante = pedido.cliente.solicitanteList.Where(d => d.idSolicitante == idSolicitante).FirstOrDefault();
            }

            pedido.existeCambioSolicitante = false;
            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
            return JsonConvert.SerializeObject(pedido.solicitante);
        }

        public void ChangeSolicitanteNombre()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            pedido.solicitante.nombre = this.Request.Params["solicitanteNombre"];
            pedido.existeCambioSolicitante = true;
            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
        }

        public void ChangeSolicitanteTelefono()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            pedido.solicitante.telefono = this.Request.Params["solicitanteTelefono"];
            pedido.existeCambioSolicitante = true;
            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
        }

        public void ChangeSolicitanteCorreo()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            pedido.solicitante.correo = this.Request.Params["solicitanteCorreo"];
            pedido.existeCambioSolicitante = true;
            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
        }


        public void ChangeFechaSolicitud()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            String[] fechaSolicitud = this.Request.Params["fechaSolicitud"].Split('/');
            String[] horaSolicitud = this.Request.Params["horaSolicitud"].Split(':');
            pedido.fechaSolicitud = new DateTime(Int32.Parse(fechaSolicitud[2]), Int32.Parse(fechaSolicitud[1]), Int32.Parse(fechaSolicitud[0]), Int32.Parse(horaSolicitud[0]), Int32.Parse(horaSolicitud[1]), 0);
            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
        }

        public void ChangeFechaEntregaDesde()
        {
            Pedido pedido = this.PedidoSession;
            String[] ftmp = this.Request.Params["fechaEntregaDesde"].Split('/');
            pedido.fechaEntregaDesde = new DateTime(Int32.Parse(ftmp[2]), Int32.Parse(ftmp[1]), Int32.Parse(ftmp[0]));
            this.PedidoSession = pedido;
        }

        public void ChangeFechaEntregaHasta()
        {
            Pedido pedido = this.PedidoSession;
            String[] ftmp = this.Request.Params["fechaEntregaHasta"].Split('/');
            pedido.fechaEntregaHasta = new DateTime(Int32.Parse(ftmp[2]), Int32.Parse(ftmp[1]), Int32.Parse(ftmp[0]));
            this.PedidoSession = pedido;
        }

        public void ChangeContactoPedido()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            pedido.contactoPedido = this.Request.Params["contactoPedido"];
            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
        }

        public void ChangeTelefonoContactoPedido()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            pedido.telefonoContactoPedido = this.Request.Params["telefonoContactoPedido"];
            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
        }


        public void ChangeObservaciones()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];

            pedido.GetType().GetProperty("observaciones").SetValue(pedido, this.Request.Params["observaciones"]);

            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
        }


        public void ChangeFechaSolicitudDesde()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA];
            String[] solDesde = this.Request.Params["fechaSolicitudDesde"].Split('/');
            pedido.fechaSolicitudDesde = new DateTime(Int32.Parse(solDesde[2]), Int32.Parse(solDesde[1]), Int32.Parse(solDesde[0]));
            this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA] = pedido;
        }

        public void ChangeFechaSolicitudHasta()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA];
            String[] solHasta = this.Request.Params["fechaSolicitudHasta"].Split('/');
            pedido.fechaSolicitudHasta = new DateTime(Int32.Parse(solHasta[2]), Int32.Parse(solHasta[1]), Int32.Parse(solHasta[0]), 23, 59, 59);
            this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA] = pedido;
        }

        public void ChangeNumero()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA];
            if (this.Request.Params["numero"] == null || this.Request.Params["numero"].Trim().Length == 0)
            {
                pedido.numeroPedido = 0;
            }
            else
            {
                pedido.numeroPedido = long.Parse(this.Request.Params["numero"]);
            }
            this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA] = pedido;
        }

        public void ChangeNumeroGrupo()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA];
            if (this.Request.Params["numeroGrupo"] == null || this.Request.Params["numeroGrupo"].Trim().Length == 0)
            {
                pedido.numeroGrupoPedido = 0;
            }
            else
            {
                pedido.numeroGrupoPedido = long.Parse(this.Request.Params["numeroGrupo"]);
            }
            this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA] = pedido;
        }


        public void ChangeIdGrupoCliente()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA];
            if (this.Request.Params["idGrupoCliente"] == null || this.Request.Params["idGrupoCliente"].Trim().Length == 0)
            {
                pedido.idGrupoCliente = 0;
            }
            else
            {
                pedido.idGrupoCliente = int.Parse(this.Request.Params["idGrupoCliente"]);
            }
            this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA] = pedido;
        }

        public void ChangeBuscarSedesGrupoCliente()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA];
            if (this.Request.Params["buscarSedesGrupoCliente"] != null && Int32.Parse(this.Request.Params["buscarSedesGrupoCliente"]) == 1)
            {
                pedido.buscarSedesGrupoCliente = true;
            }
            else
            {
                pedido.buscarSedesGrupoCliente = false;
            }
            this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA] = pedido;
        }

    

        public String ChangeIdCiudad()
        {
            Pedido pedido = this.PedidoSession;
            pedido.cliente = new Cliente();
            Guid idCiudad = Guid.Empty;
            if (this.Request.Params["idCiudad"] != null && !this.Request.Params["idCiudad"].Equals(""))
            {
                idCiudad = Guid.Parse(this.Request.Params["idCiudad"]);
            }
            //Para realizar el cambio de ciudad ningun producto debe estar agregado
            if (pedido.pedidoDetalleList != null && pedido.pedidoDetalleList.Count > 0)
            {
                // throw new Exception("No se puede cambiar de ciudad");
                return "No se puede cambiar de ciudad";
            }
            else
            {
                CiudadBL ciudadBL = new CiudadBL();
                Ciudad ciudadNueva = ciudadBL.getCiudad(idCiudad);
                pedido.ciudad = ciudadNueva;
                this.PedidoSession = pedido;
                return "{\"idCiudad\": \"" + idCiudad + "\"}";
            }
        }

        public String ChangeIdPromocion()
        {
            Pedido pedido = this.PedidoSession;
            Guid idPromocion = Guid.Empty;
            if (this.Request.Params["idPromocion"] != null && !this.Request.Params["idPromocion"].Equals(""))
            {
                idPromocion = Guid.Parse(this.Request.Params["idPromocion"]);
                PromocionBL bL = new PromocionBL();
                pedido.promocion = bL.getPromocion(idPromocion);
            } else
            {
                pedido.promocion = new Promocion();
                pedido.promocion.idPromocion = Guid.Empty;
            }

            this.PedidoSession = pedido;
            return "{\"idPromocion\": \"" + idPromocion + "\"}";
        }

        public String AddPromocion()
        {
            Pedido pedido = this.PedidoSession;
            Guid idPromocion = Guid.Empty;
            int success = 0;
            string errorMessage = "";
            Promocion prom = null;

            if (this.Request.Params["idPromocion"] != null && !this.Request.Params["idPromocion"].Equals(""))
            {
                idPromocion = Guid.Parse(this.Request.Params["idPromocion"]);
                prom = pedido.promociones.Where(p => p.idPromocion.Equals(idPromocion)).FirstOrDefault();

                if (prom == null)
                {
                    PromocionBL bL = new PromocionBL();
                    prom = bL.getPromocion(idPromocion);
                    pedido.promociones.Add(prom);
                    success = 1;
                }
                else
                {
                    errorMessage = "La promoción ya está agregada.";
                }
            }
            else
            {
                errorMessage = "La promoción no es válida.";
            }

            this.PedidoSession = pedido;
            var obj = new
            {
                promocion = prom,
                success = success,
                errorMessage = errorMessage
            };

            return JsonConvert.SerializeObject(obj);
        }

        public String RemovePromocion()
        {
            Pedido pedido = this.PedidoSession;
            Guid idPromocion = Guid.Empty;
            int success = 0;
            string errorMessage = "";
            Promocion prom = null;

            if (this.Request.Params["idPromocion"] != null && !this.Request.Params["idPromocion"].Equals(""))
            {
                idPromocion = Guid.Parse(this.Request.Params["idPromocion"]);
                prom = pedido.promociones.Where(p => p.idPromocion.Equals(idPromocion)).FirstOrDefault();

                if (prom != null)
                {
                    pedido.promociones.Remove(prom);
                    success = 1;
                }
            }
            else
            {
                errorMessage = "Promoción no válida.";
            }

            this.PedidoSession = pedido;
            var obj = new
            {
                promocion = prom,
                success = success,
                errorMessage = errorMessage
            };

            return JsonConvert.SerializeObject(obj);
        }


        public void ChangeTipoPedido()
        {
            Char tipoPedido = Convert.ToChar(Int32.Parse(this.Request.Params["tipoPedido"]));
            this.PedidoSession.tipoPedido = (Pedido.tiposPedido)tipoPedido;
        }

        public void ChangeTipoPedidoBusqueda()
        {
            Char tipoPedidoBusqueda = Convert.ToChar(Int32.Parse(this.Request.Params["tipoPedidoBusqueda"]));
            this.PedidoSession.tipoPedidoVentaBusqueda = (Pedido.tiposPedidoVentaBusqueda)tipoPedidoBusqueda;
        }


        public String ChangeIdCiudadASolicitar()
        {
            Pedido pedido = this.PedidoSession;
            Guid idCiudad = Guid.Empty;
            if (this.Request.Params["idCiudadASolicitar"] != null && !this.Request.Params["idCiudadASolicitar"].Equals(""))
            {
                idCiudad = Guid.Parse(this.Request.Params["idCiudadASolicitar"]);
            }
            //Para realizar el cambio de ciudad ningun producto debe estar agregado
        /*    if (pedido.pedidoDetalleList != null && pedido.pedidoDetalleList.Count > 0)
            {
                // throw new Exception("No se puede cambiar de ciudad");
                return "No se puede cambiar de ciudad";
            }
            else
            {*/
            CiudadBL ciudadBL = new CiudadBL();
            Ciudad ciudadASolicitar = ciudadBL.getCiudad(idCiudad);
            pedido.ciudadASolicitar = ciudadASolicitar;
            this.PedidoSession = pedido;
            return "{\"idCiudad\": \"" + idCiudad + "\"}";
          //  }
        }

        [HttpPost]
        public String ChangeDetalle(List<DocumentoDetalleJson> cotizacionDetalleJsonList)
        {
            IDocumento documento = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            List<DocumentoDetalle> documentoDetalle = HelperDocumento.updateDocumentoDetalle(documento, cotizacionDetalleJsonList);
            documento.documentoDetalle = documentoDetalle;
            PedidoBL pedidoBL = new PedidoBL();
            pedidoBL.calcularMontosTotales((Pedido)documento);
            this.Session[Constantes.VAR_SESSION_PEDIDO] = documento;
            return "{\"cantidad\":\"" + documento.documentoDetalle.Count + "\"}";
        }


        public String ChangeEsPagoContado()
        {
            Pedido pedido = this.PedidoSession;
            try
            {
                pedido.esPagoContado = Int32.Parse(this.Request.Params["esPagoContado"]) == 1;
            }
            catch (Exception ex)
            {
            }
            this.PedidoSession = pedido;

            return "{\"textoCondicionesPago\":\"" + pedido.textoCondicionesPago + "\"}";
        }

        public String ChangeFacturaUnica()
        {
            Pedido pedido = this.PedidoSession;
            try
            {
                pedido.facturaUnica = Int32.Parse(this.Request.Params["facturaUnica"]) == 1;
            }
            catch (Exception ex)
            {
            }
            this.PedidoSession = pedido;

            return "{\"success\":\" 1\"}";
        }


        #endregion


        #region CAMBIOS CAMPOS DE FORMULARIO BUSQUEDA



        public void ChangeEstado()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA];
            pedido.seguimientoPedido.estado = (SeguimientoPedido.estadosSeguimientoPedido)Int32.Parse(this.Request.Params["estado"]);
            this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA] = pedido;
        }

        public void ChangeEstadoCrediticio()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA];
            pedido.seguimientoCrediticioPedido.estado = (SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido)Int32.Parse(this.Request.Params["estadoCrediticio"]);
            this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA] = pedido;
        }


        #endregion




        #region CREAR/ACTUALIZAR PEDIDO

        public String UpdatePost()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_VER];

            //pedido.
            PedidoBL pedidoBL = new PedidoBL();
            pedido.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            pedido.numeroReferenciaAdicional = this.Request.Params["numeroReferenciaAdicional"];
            pedido.numeroReferenciaCliente = this.Request.Params["numeroReferenciaCliente"];
            pedido.observaciones = this.Request.Params["observaciones"];

            if (pedido.usuario.pedidoRegistraObservacionAlmacen)
            {
                pedido.observacionesAlmacen = this.Request.Params["observacionesAlmacen"];
            }

            pedido.observacionesGuiaRemision = this.Request.Params["observacionesGuiaRemision"];
            pedido.observacionesFactura = this.Request.Params["observacionesFactura"];
            pedido.numeroGrupoPedido = Int32.Parse(this.Request.Params["pedidoNumeroGrupo"]);
            pedido.facturaUnica = Int32.Parse(this.Request.Params["facturaUnica"]) == 1 ? true : false;

            if (Logueado.modificaPedidoFechaEntregaExtendida) { 
                if (this.Request.Params["fechaEntregaExtendida"] == null || this.Request.Params["fechaEntregaExtendida"].Equals(""))
                {
                    pedido.fechaEntregaExtendida = null;
                }
                else
                {
                    String[] entregaExtendida = this.Request.Params["fechaEntregaExtendida"].Split('/');
                    pedido.fechaEntregaExtendida = new DateTime(Int32.Parse(entregaExtendida[2]), Int32.Parse(entregaExtendida[1]), Int32.Parse(entregaExtendida[0]), 23, 59, 59);
                }
            }

            pedidoBL.ActualizarPedido(pedido);
            long numeroPedido = pedido.numeroPedido;
            String numeroPedidoString = pedido.numeroPedidoString;
            Guid idPedido = pedido.idPedido;
            int estado = (int)pedido.seguimientoPedido.estado;







          
            var v = new { numeroPedido = numeroPedidoString, estado = estado, idPedido = idPedido };
            String resultado = JsonConvert.SerializeObject(v);
            return resultado;
        }

        #region carga de imagenes

        public void ChangeFiles(List<HttpPostedFileBase> files)
        {
           
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];

            if((int)this.Session[Constantes.VAR_SESSION_PAGINA] == (int)Constantes.paginas.BusquedaPedidos )
                pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_VER];


            foreach (var file in files)
            {
                if (file != null && file.ContentLength > 0)
                {
                    if (pedido.pedidoAdjuntoList.Where(p => p.nombre.Equals(file.FileName) ).FirstOrDefault() != null)
                    {
                        continue;
                    }
                    

                    PedidoAdjunto pedidoAdjunto = new PedidoAdjunto();
                    using (Stream inputStream = file.InputStream)
                    {
                        MemoryStream memoryStream = inputStream as MemoryStream;
                        if (memoryStream == null)
                        {
                            memoryStream = new MemoryStream();
                            inputStream.CopyTo(memoryStream);
                        }
                        pedidoAdjunto.nombre = file.FileName;
                        pedidoAdjunto.adjunto = memoryStream.ToArray();
                    }
                    pedido.pedidoAdjuntoList.Add(pedidoAdjunto);
                }
            }

        }


        public String DescartarArchivos()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            if ((int)this.Session[Constantes.VAR_SESSION_PAGINA] == (int)Constantes.paginas.BusquedaPedidos)
                pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_VER];


            String nombreArchivo = Request["nombreArchivo"].ToString();

            List<PedidoAdjunto> pedidoAdjuntoList = new List<PedidoAdjunto>();
            foreach (PedidoAdjunto pedidoAdjunto in pedido.pedidoAdjuntoList )
            {
                if(!pedidoAdjunto.nombre.Equals(nombreArchivo))
                    pedidoAdjuntoList.Add(pedidoAdjunto);
            }

            pedido.pedidoAdjuntoList = pedidoAdjuntoList;

            return JsonConvert.SerializeObject(pedido.pedidoAdjuntoList);
        }

        public String Descargar()
        {
            String nombreArchivo = Request["nombreArchivo"].ToString();
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];

            if ((int)this.Session[Constantes.VAR_SESSION_PAGINA] == (int)Constantes.paginas.BusquedaPedidos)
            {
                pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_VER];

            }

            ArchivoAdjunto archivoAdjunto  = pedido.pedidoAdjuntoList.Where(p => p.nombre.Equals(nombreArchivo)).FirstOrDefault();
            
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
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            pedido.guardadoParcialmente = continuarLuego == 1 ? true : false;
            pedido.usuario = usuario;
            pedido.UsuarioRegistro = usuario;
            PedidoBL pedidoBL = new PedidoBL();

            if (pedido.idPedido != Guid.Empty || pedido.numeroPedido > 0)
            {
                throw new System.Exception("Pedido ya se encuentra creado");
            }

            pedidoBL.InsertPedido(pedido);
            long numeroPedido = pedido.numeroPedido;
            String numeroPedidoString = pedido.numeroPedidoString;
            Guid idPedido = pedido.idPedido;
            int estado = (int)pedido.seguimientoPedido.estado;
            String observacion = pedido.seguimientoPedido.observacion;
            if (continuarLuego == 1)
            {
                SeguimientoPedido.estadosSeguimientoPedido estadosSeguimientoPedido = SeguimientoPedido.estadosSeguimientoPedido.Edicion;
                estado = (int)estadosSeguimientoPedido;
                 observacion = "Se continuará editando luego";
                updateEstadoSeguimientoPedido(idPedido, estadosSeguimientoPedido, observacion);
            }
            // pedido = null;
            this.Session[Constantes.VAR_SESSION_PEDIDO] = null;// pedido;// null;


            usuarioBL.updatePedidoSerializado(usuario, null);

            var v = new { numeroPedido = numeroPedidoString, estado = estado, observacion = observacion, idPedido = idPedido };
            String resultado = JsonConvert.SerializeObject(v);

           // String resultado = "{ \"codigo\":\"" + numeroPedido + "\", \"estado\":\"" + estado + "\", \"observacion\":\"" + observacion + "\" }";
            return resultado;
        }





        public String Update()
        {
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            int continuarLuego = int.Parse(Request["continuarLuego"].ToString());
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            pedido.guardadoParcialmente = continuarLuego == 1 ? true : false;
            pedido.usuario = usuario;
            pedido.UsuarioRegistro = usuario;
            PedidoBL bl = new PedidoBL();
            bl.UpdatePedido(pedido);
            long numeroPedido = pedido.numeroPedido;
            String numeroPedidoString = pedido.numeroPedidoString;
            Guid idPedido = pedido.idPedido;
            int estado = (int)pedido.seguimientoPedido.estado;
            String observacion = pedido.seguimientoPedido.observacion;
            if (continuarLuego == 1)
            {
                SeguimientoPedido.estadosSeguimientoPedido estadosSeguimientoPedido = SeguimientoPedido.estadosSeguimientoPedido.Edicion;
                estado = (int)estadosSeguimientoPedido;
                observacion = "Se continuará editando luego";
                updateEstadoSeguimientoPedido(idPedido, estadosSeguimientoPedido, observacion);
            }
            // pedido = null;
            this.Session[Constantes.VAR_SESSION_PEDIDO] = null;// pedido;

            usuarioBL.updatePedidoSerializado(usuario, null);

            var v = new { numeroPedido = numeroPedidoString, estado = estado, observacion = observacion, idPedido = idPedido };
            String resultado = JsonConvert.SerializeObject(v);
            
            //String resultado = "{ \"codigo\":\"" + numeroPedido + "\", \"estado\":\"" + estado + "\", \"observacion\":\"" + observacion + "\" }";
            return resultado;
        }



        public void ChangeResponsableComercialPedidoBusqueda()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA];

            try
            {
                string idVendedor = this.Request.Params["idVendedor"].ToString();
                if (idVendedor.Equals("")) idVendedor = "0";

                if (usuario.modificaFiltroVendedor)
                {
                    pedido.responsableComercial.idVendedor = int.Parse(idVendedor);
                }
                else
                {
                    if (usuario.esResponsableComercial)
                    {
                        pedido.responsableComercial = usuario.vendedor;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA] = pedido;
        }




        public void updateEstadoPedido()
        {
            /*Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_VER];
            SeguimientoPedido.estadosSeguimientoPedido estadosSeguimientoPedido = (SeguimientoPedido.estadosSeguimientoPedido)Int32.Parse(Request["estado"].ToString());
            String observacion = Request["observacion"].ToString();*/

            Guid idPedido = Guid.Parse(Request["idPedido"].ToString());
            SeguimientoPedido.estadosSeguimientoPedido estadosSeguimientoPedido = (SeguimientoPedido.estadosSeguimientoPedido)Int32.Parse(Request["estado"].ToString());
            String observacion = Request["observacion"].ToString();
            updateEstadoSeguimientoPedido(idPedido, estadosSeguimientoPedido, observacion);

            if (estadosSeguimientoPedido == SeguimientoPedido.estadosSeguimientoPedido.Ingresado)
            {
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

                PedidoBL pedidoBL = new PedidoBL();

                List<Guid> idDetalles = (List<Guid>)this.Session["pedidoDRIds"];
                List<int> cantidades = (List<int>)this.Session["pedidoDRCantidades"];
                List<String> comentarios = (List<String>)this.Session["pedidoDRComentarios"];

                pedidoBL.UpdateDetallesRestriccion(idPedido, idDetalles, cantidades, comentarios, usuario.idUsuario);

                Pedido pedido = new Pedido(Pedido.ClasesPedido.Venta);
                pedido.idPedido = idPedido;
                pedido = pedidoBL.GetPedido(pedido, usuario);
                pedido.usuario = usuario;

                if (!pedido.usuario.codigoEmpresa.Equals(Constantes.EMPRESA_CODIGO_MP) && 
                    pedido.seguimientoPedido.estado == SeguimientoPedido.estadosSeguimientoPedido.Ingresado && 
                    pedido.seguimientoCrediticioPedido.estado == SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido.Liberado)
                {
                    pedidoBL.ProcesarPedidoAprobadoTecnica(pedido);
                }
            }
        }

        public void updateEstadoPedidoCrediticio()
        {
            Guid idPedido = Guid.Parse(Request["idPedido"].ToString());
            SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido estadosSeguimientoCrediticioPedido = (SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido)Int32.Parse(Request["estado"].ToString());
            String observacion = Request["observacion"].ToString();
            updateEstadoSeguimientoCrediticioPedido(idPedido, estadosSeguimientoCrediticioPedido, observacion);

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            PedidoBL pedidoBL = new PedidoBL();

            Pedido pedido = new Pedido(Pedido.ClasesPedido.Venta);
            pedido.idPedido = idPedido;
            pedido = pedidoBL.GetPedido(pedido, usuario);
            pedido.usuario = usuario;

            if (!pedido.usuario.codigoEmpresa.Equals(Constantes.EMPRESA_CODIGO_MP) &&
                pedido.seguimientoPedido.estado == SeguimientoPedido.estadosSeguimientoPedido.Ingresado &&
                pedido.seguimientoCrediticioPedido.estado == SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido.Liberado)
            {
                pedidoBL.ProcesarPedidoAprobadoTecnica(pedido);
            }
            
        }


        public String Programar()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_VER];
            String[] fechaProgramacion = this.Request.Params["fechaProgramacion"].Split('/');
            pedido.fechaProgramacion = new DateTime(Int32.Parse(fechaProgramacion[2]), Int32.Parse(fechaProgramacion[1]), Int32.Parse(fechaProgramacion[0]));

            pedido.comentarioProgramacion = this.Request.Params["comentarioProgramacion"];
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            PedidoBL pedidoBL = new PedidoBL();
            pedidoBL.ProgramarPedido(pedido, usuario);

            String resultado = pedido.numeroPedido.ToString();
            return resultado;
        }

        



        private void updateEstadoSeguimientoPedido(Guid idPedido, SeguimientoPedido.estadosSeguimientoPedido estado, String observacion)
        {
           // Pedido cotizacionSession = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            PedidoBL pedidoBL = new PedidoBL();
            Pedido pedido = new Pedido(Pedido.ClasesPedido.Venta);
            pedido.idPedido = idPedido;
            //REVISAR
            pedido.fechaModificacion = DateTime.Now;// cotizacionSession.fechaModificacion;
            pedido.seguimientoPedido = new SeguimientoPedido();
            pedido.seguimientoPedido.estado = estado;
            pedido.seguimientoPedido.observacion = observacion;
            pedido.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
           //FALTA
            pedidoBL.cambiarEstadoPedido(pedido);
        }

        private void updateEstadoSeguimientoCrediticioPedido(Guid idPedido, SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido estado, String observacion)
        {
            Pedido cotizacionSession = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            PedidoBL pedidoBL = new PedidoBL();
            Pedido pedido = new Pedido(Pedido.ClasesPedido.Venta);
            pedido.idPedido = idPedido;
            //REVISAR
            pedido.fechaModificacion = DateTime.Now;// cotizacionSession.fechaModificacion;
            pedido.seguimientoCrediticioPedido = new SeguimientoCrediticioPedido();
            pedido.seguimientoCrediticioPedido.estado = estado;
            pedido.seguimientoCrediticioPedido.observacion = observacion;
            pedido.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            //FALTA
            pedidoBL.cambiarEstadoCrediticioPedido(pedido);
        }
        #endregion

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
            List<Pedido> list = (List<Pedido>) this.Session[Constantes.VAR_SESSION_PEDIDO_LISTA];

            PedidoSearch excel = new PedidoSearch();
            return excel.generateExcel(list);
        }

        public String Search()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaPedidos;
            //Se recupera el pedido Búsqueda de la session
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA];

            String[] solDesde = this.Request.Params["fechaCreacionDesde"].Split('/');
            pedido.fechaCreacionDesde = new DateTime(Int32.Parse(solDesde[2]), Int32.Parse(solDesde[1]), Int32.Parse(solDesde[0]));

            String[] solHasta = this.Request.Params["fechaCreacionHasta"].Split('/');
            pedido.fechaCreacionHasta = new DateTime(Int32.Parse(solHasta[2]), Int32.Parse(solHasta[1]), Int32.Parse(solHasta[0]),23,59,59);


            if (this.Request.Params["fechaEntregaDesde"] == null || this.Request.Params["fechaEntregaDesde"].Equals(""))
            {
                pedido.fechaEntregaDesde = null;
            }
            else
            { 
                String[] entregaDesde = this.Request.Params["fechaEntregaDesde"].Split('/');
                pedido.fechaEntregaDesde = new DateTime(Int32.Parse(entregaDesde[2]), Int32.Parse(entregaDesde[1]), Int32.Parse(entregaDesde[0]));
            }


            if (this.Request.Params["fechaEntregaHasta"] == null || this.Request.Params["fechaEntregaHasta"].Equals(""))
            {
                pedido.fechaEntregaHasta = null;
            }
            else
            {
                String[] entregaHasta = this.Request.Params["fechaEntregaHasta"].Split('/');
                pedido.fechaEntregaHasta = new DateTime(Int32.Parse(entregaHasta[2]), Int32.Parse(entregaHasta[1]), Int32.Parse(entregaHasta[0]), 23, 59, 59);
            }

            if (this.Request.Params["fechaProgramacionDesde"] == null || this.Request.Params["fechaProgramacionDesde"].Equals(""))
            {
                pedido.fechaProgramacionDesde = null;
            }
            else
            {
                String[] programacionDesde = this.Request.Params["fechaProgramacionDesde"].Split('/');
                pedido.fechaProgramacionDesde = new DateTime(Int32.Parse(programacionDesde[2]), Int32.Parse(programacionDesde[1]), Int32.Parse(programacionDesde[0]));
            }


            if (this.Request.Params["fechaProgramacionHasta"] == null || this.Request.Params["fechaProgramacionHasta"].Equals(""))
            {
                pedido.fechaProgramacionHasta = null;
            }
            else
            {
                String[] programacionHasta = this.Request.Params["fechaProgramacionDesde"].Split('/');
                pedido.fechaProgramacionHasta = new DateTime(Int32.Parse(programacionHasta[2]), Int32.Parse(programacionHasta[1]), Int32.Parse(programacionHasta[0]));
            }




            if (this.Request.Params["numero"] == null || this.Request.Params["numero"].Trim().Length == 0)
            {
                pedido.numeroPedido = 0;
            }
            else
            {
                pedido.numeroPedido = long.Parse(this.Request.Params["numero"]);
            }

            if (this.Request.Params["numeroGrupo"] == null || this.Request.Params["numeroGrupo"].Trim().Length == 0)
            {
                pedido.numeroGrupoPedido = 0;
            }
            else
            {
                pedido.numeroGrupoPedido = long.Parse(this.Request.Params["numeroGrupo"]);
            }

            if (this.Request.Params["idGrupoCliente"] == null || this.Request.Params["idGrupoCliente"].Trim().Length == 0)
            {
                pedido.idGrupoCliente = 0;
            }
            else
            {
                pedido.idGrupoCliente = int.Parse(this.Request.Params["idGrupoCliente"]);
            }

          //  pedido.seguimientoPedido.estado = (SeguimientoPedido.estadosSeguimientoPedido) Int32.Parse(this.Request.Params["estado"]);
            pedido.seguimientoCrediticioPedido.estado = (SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido)Int32.Parse(this.Request.Params["estadoCrediticio"]);


            PedidoBL pedidoBL = new PedidoBL();
            List<Pedido> pedidoList = pedidoBL.GetPedidos(pedido);
            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_PEDIDO_LISTA] = pedidoList;
            this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA] = pedido;

             String pedidoListString = JsonConvert.SerializeObject(ParserDTOsSearch.PedidoVentaToPedidoVentaDTO(pedidoList));
             return pedidoListString;
            //return pedidoList.Count();
        }

        public String SearchGrupo()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaPedidos;
            //Se recupera el pedido Búsqueda de la session
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA];

            if (this.Request.Params["numeroGrupo"] == null || this.Request.Params["numeroGrupo"].Trim().Length == 0)
            {
                pedido.numeroGrupoPedido = 0;
            }
            else
            {
                pedido.numeroGrupoPedido = long.Parse(this.Request.Params["numeroGrupo"]);
            }

            int tipoOrdenamiento = int.Parse(this.Request.Params["tipoOrdenamiento"]);

            PedidoBL pedidoBL = new PedidoBL();
            List<Pedido> pedidoList = pedidoBL.GetPedidosGrupo(pedido, tipoOrdenamiento);
            
            String pedidoListString = JsonConvert.SerializeObject(ParserDTOsSearch.PedidoVentaToPedidoVentaDTO(pedidoList));
            return pedidoListString;
        }

        public String SearchColectivo(Guid[] idsPedido, int tipoOrdenamiento /* 1:aprobar, 2:liberar*/)
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaPedidos;
            //Se recupera el pedido Búsqueda de la session
            
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            List<Guid> idsPedidoList = new List<Guid>(idsPedido);

            PedidoBL pedidoBL = new PedidoBL();

            if (tipoOrdenamiento == 1)
            {
                idsPedidoList = pedidoBL.soloPedidosAApropbar(idsPedidoList, usuario.idUsuario);
            }

            if (tipoOrdenamiento == 2)
            {
                idsPedidoList = pedidoBL.soloPedidosALiberar(idsPedidoList, usuario.idUsuario);
            }

            List<Pedido> pedidoList = pedidoBL.SelectPedidosByIds(idsPedidoList, tipoOrdenamiento);
            List<List<String>> resumenVentaCliente = pedidoBL.totalesRazonSocial(idsPedidoList, usuario.idUsuario);
            List<List<String>> resumenVentaProducto = pedidoBL.totalesProductos(idsPedidoList, usuario.idUsuario);

            var resultado = new
            {
                pedidos  = ParserDTOsSearch.PedidoVentaToPedidoVentaDTO(pedidoList),
                resumenVentaCliente = resumenVentaCliente, 
                resumenVentaProducto = resumenVentaProducto,
                idsPedidos = idsPedidoList
            };

            return JsonConvert.SerializeObject(resultado);
        }

        public String LiberarPedidosGrupo()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaPedidos;
            long nroGrupoPedido = 0;
            
            if (this.Request.Params["numeroGrupo"] == null || this.Request.Params["numeroGrupo"].Trim().Length == 0)
            {
                nroGrupoPedido = 0;
            }
            else
            {
                nroGrupoPedido = long.Parse(this.Request.Params["numeroGrupo"]);
            }

            int success = 0;
            int cantidadGrupo = 0;
            int cantidadAplicados = 0;

            if (usuario.liberaPedidos)
            {
                PedidoBL pedidoBL = new PedidoBL();
                List<int> results = pedidoBL.LiberarPedidosGrupo(nroGrupoPedido, usuario.idUsuario);
                cantidadGrupo = results.ElementAt(0);
                cantidadAplicados = results.ElementAt(1);
                success = 1;
            }

            var v = new { success = success, cantidadGrupo = cantidadGrupo, cantidadAplicados = cantidadAplicados };
            String resultado = JsonConvert.SerializeObject(v);

            return resultado;
        }

        public String AprobarPedidosColectivo(Guid[] idsPedido)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaPedidos;

            List<Guid> idsPedidoList = new List<Guid>(idsPedido);

            int success = 0;
            int cantidadGrupo = 0;
            int cantidadAplicados = 0;

            if (usuario.apruebaPedidos)
            {
                PedidoBL pedidoBL = new PedidoBL();
                List<int> results = pedidoBL.AprobarPedidos(idsPedidoList, usuario.idUsuario);
                cantidadGrupo = results.ElementAt(0);
                cantidadAplicados = results.ElementAt(1);
                success = 1;
            }

            var v = new { success = success, cantidadGrupo = cantidadGrupo, cantidadAplicados = cantidadAplicados };
            String resultado = JsonConvert.SerializeObject(v);

            return resultado;
        }

        public String LiberarPedidosColectivo(Guid[] idsPedido)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaPedidos;
            List<Guid> idsPedidoList = new List<Guid>(idsPedido);

            int success = 0;
            int cantidadGrupo = 0;
            int cantidadAplicados = 0;

            if (usuario.liberaPedidos)
            {
                PedidoBL pedidoBL = new PedidoBL();
                List<int> results = pedidoBL.LiberarPedidos(idsPedidoList, usuario.idUsuario);
                cantidadGrupo = results.ElementAt(0);
                cantidadAplicados = results.ElementAt(1);
                success = 1;
            }

            var v = new { success = success, cantidadGrupo = cantidadGrupo, cantidadAplicados = cantidadAplicados };
            String resultado = JsonConvert.SerializeObject(v);

            return resultado;
        }

        public String AprobarPedidosGrupo()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaPedidos;
            long nroGrupoPedido = 0;
            if (this.Request.Params["numeroGrupo"] == null || this.Request.Params["numeroGrupo"].Trim().Length == 0)
            {
                nroGrupoPedido = 0;
            }
            else
            {
                nroGrupoPedido = long.Parse(this.Request.Params["numeroGrupo"]);
            }

            int success = 0;
            int cantidadGrupo = 0;
            int cantidadAplicados = 0;

            if (usuario.apruebaPedidos)
            {
                PedidoBL pedidoBL = new PedidoBL();
                List<int> results = pedidoBL.AprobarPedidosgrupo(nroGrupoPedido, usuario.idUsuario);
                cantidadGrupo = results.ElementAt(0);
                cantidadAplicados = results.ElementAt(1);
                success = 1;
            }

            var v = new { success = success, cantidadGrupo = cantidadGrupo, cantidadAplicados = cantidadAplicados };
            String resultado = JsonConvert.SerializeObject(v);

            return resultado;
        }



        public String ConsultarSiExistePedido()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            if (pedido == null)
                return "{\"existe\":\"false\",\"numero\":\"0\"}";
            else
                return "{\"existe\":\"true\",\"numero\":\"" + pedido.numeroPedido + "\"}";
        }

        public String Show(int soloLectura = 0)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            PedidoBL pedidoBL = new PedidoBL();

            Pedido pedido = new Pedido(Pedido.ClasesPedido.Venta);
            pedido.idPedido = Guid.Parse(Request["idPedido"].ToString());
            pedido = pedidoBL.GetPedido(pedido,usuario);

            if (soloLectura == 0) { 
                this.Session[Constantes.VAR_SESSION_PEDIDO_VER] = pedido;
            }

            ClienteBL clienteBl = new ClienteBL();
            Cliente cliente = clienteBl.getCliente(pedido.cliente.idCliente);
            cliente.usuario = usuario;

            string jsonUsuario = JsonConvert.SerializeObject(usuario);

            string jsonPedido = JsonConvert.SerializeObject(ParserDTOsShow.PedidoVentaToPedidoVentaDTO(pedido));

            Ciudad ciudad = usuario.sedesMPPedidos.Where(s => s.idCiudad == pedido.ciudad.idCiudad).FirstOrDefault();
          
            string jsonSeries = "[]";
            if (ciudad != null)
            {
                var serieDocumentoElectronicoList = ciudad.serieDocumentoElectronicoList.OrderByDescending(x => x.esPrincipal).ToList();
                jsonSeries = JsonConvert.SerializeObject(serieDocumentoElectronicoList);

            }

            if (soloLectura == 0)
            {

                this.Session["pedidoDRIds"] = new List<Guid>();
                this.Session["pedidoDRCantidades"] = new List<int>();
                this.Session["pedidoDRComentarios"] = new List<String>();
            }

            String json = "{\"serieDocumentoElectronicoList\":" + jsonSeries + ", \"pedido\":" + jsonPedido + ", \"usuario\":" + jsonUsuario + ", \"isOwner\":" + (cliente.isOwner || pedido.usuario.idUsuario == usuario.idUsuario ? "1" : "0") + "}";
            return json;
        }

        [HttpPost]
        public String GetStockProductos()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_VER];
            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null || pedido == null)
            {
                return "";
            }
            
            List<Guid> idProductos = new List<Guid>();

            foreach (DocumentoDetalle det in pedido.documentoDetalle)
            {
                idProductos.Add(det.producto.idProducto);
            }

            ProductoBL bl = new ProductoBL();
            List<RegistroCargaStock> stocks = bl.StockProductosSede(idProductos, pedido.ciudad.idCiudad, usuario.idUsuario);

            return JsonConvert.SerializeObject(stocks);
        }


        public void autoGuardarPedido()
        {
            if (this.Session[Constantes.VAR_SESSION_PEDIDO] != null)
            {
                Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
                UsuarioBL usuarioBL = new UsuarioBL();
                Usuario usuario = (Usuario)this.Session["usuario"];

                String pedidoSerializado = JsonConvert.SerializeObject(pedido);

                usuarioBL.updatePedidoSerializado(usuario, pedidoSerializado);
            }

        }

        public void updateUsuario()
        {
            Pedido pedido = this.PedidoSession;
            pedido.usuarioBusqueda = new Usuario { idUsuario = Guid.Parse(this.Request.Params["usuario"]) };
            this.PedidoSession = pedido;
        }

        public String CreateDireccionTemporal()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            DireccionEntrega direccionEntrega = new DireccionEntrega();
            direccionEntrega.descripcion = Request["direccion"];
            direccionEntrega.contacto = Request["contacto"];
            direccionEntrega.telefono = Request["telefono"];
            direccionEntrega.idDireccionEntrega = Guid.Empty;
            pedido.cliente.direccionEntregaList.Add(direccionEntrega);
            pedido.direccionEntrega = direccionEntrega;
            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
            return JsonConvert.SerializeObject(direccionEntrega);
        }

        public String CreateSolicitanteTemporal()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            Solicitante solicitante = new Solicitante();
            solicitante.nombre = Request["nombre"];
            solicitante.telefono = Request["telefono"];
            solicitante.correo = Request["correo"];
            solicitante.idSolicitante = Guid.Empty;
            pedido.cliente.solicitanteList.Add(solicitante);
            pedido.solicitante = solicitante;
            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
            return JsonConvert.SerializeObject(solicitante);
        }


        public string UpdateStockConfirmado()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            string success = "0";
            string message = "";

            if (usuario.confirmaStock)
            {
                Pedido pedido = new Pedido(Pedido.ClasesPedido.Venta);
                pedido.idPedido = Guid.Parse(this.Request.Params["idPedido"]);

                PedidoBL pedidoBL = new PedidoBL();
                pedido = pedidoBL.GetPedido(pedido, usuario);

                pedido.stockConfirmado = Int32.Parse(this.Request.Params["stockConfirmado"]);
                pedido.usuario = usuario;

                if (pedido.ciudad.idCiudad == usuario.sedeMP.idCiudad)
                {
                    success = "1";
                    pedidoBL.UpdateStockConfirmado(pedido);
                }
                else
                {
                    message = "Debe pertenecer a la sede del pedido para poder editar la confirmación de Stock";
                }
            }

            return "{\"success\": " + success + ",\"message\":\"" + message + "\"}";
        }




        public void TruncarPedido()
        {
            if (this.Session[Constantes.VAR_SESSION_USUARIO] != null)
            {
                Pedido pedido = new Pedido(Pedido.ClasesPedido.Venta);
                pedido.idPedido = Guid.Parse(this.Request.Params["idPedido"]);
                pedido.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

                PedidoBL pedidoBL = new PedidoBL();
                pedidoBL.TruncarPedido(pedido);

            }
        }

        public void DestruncarPedido()
        {
            if (this.Session[Constantes.VAR_SESSION_USUARIO] != null)
            {
                Pedido pedido = new Pedido(Pedido.ClasesPedido.Venta);
                pedido.idPedido = Guid.Parse(this.Request.Params["idPedido"]);
                pedido.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

                PedidoBL pedidoBL = new PedidoBL();
                pedidoBL.DestruncarPedido(pedido);

            }
        }


        [HttpGet]
        public ActionResult Load()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                if (!usuario.realizaCargaMasivaPedidos)
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            ViewBag.numerosPedido = String.Empty;


            //List<Guid> idsProducto = new List<Guid>();
            //List<Pedido> pedidosSede = new List<Pedido>();
            //List<Pedido> pedidoList = new List<Pedido>();
            //PedidoBL bl = new PedidoBL();

            //Pedido ped = new Pedido();
            
            //ped.idPedido = Guid.Parse("CA45607A-9BD3-4BEF-9D09-06CD300B42A6");
            //pedidoList.Add(bl.GetPedido(ped, usuario));

            //ped = new Pedido();
            //ped.idPedido = Guid.Parse("F5661003-0FCE-43A4-8E31-1181D64B666C");
            //pedidoList.Add(bl.GetPedido(ped, usuario));

            //ped = new Pedido();
            //ped.idPedido = Guid.Parse("843FB29D-902C-4946-8590-1A3D826F9C63");
            //pedidoList.Add(bl.GetPedido(ped, usuario));

            //ped = new Pedido();
            //ped.idPedido = Guid.Parse("2F65C2C9-BC1A-4179-A8AB-2451DB352EE2");
            //pedidoList.Add(bl.GetPedido(ped, usuario));

            //ped = new Pedido();
            //ped.idPedido = Guid.Parse("1A1C3D4B-7C59-4DCD-997A-25AC1A2C37B9");
            //pedidoList.Add(bl.GetPedido(ped, usuario));

            //ped = new Pedido();
            //ped.idPedido = Guid.Parse("86972B6B-FB92-41C2-91AE-2C1064C9CFC5");
            //pedidoList.Add(bl.GetPedido(ped, usuario));

            //ped = new Pedido();
            //ped.idPedido = Guid.Parse("6EA83190-8462-448E-8D52-72391EFFC9E4");
            //pedidoList.Add(bl.GetPedido(ped, usuario));

            //ped = new Pedido();
            //ped.idPedido = Guid.Parse("036A1918-5902-4B92-B2F2-770BCDE29817");
            //pedidoList.Add(bl.GetPedido(ped, usuario));

            //ped = new Pedido();
            //ped.idPedido = Guid.Parse("311A9DB5-E172-4D6A-ADCF-852F024D6B11");
            //pedidoList.Add(bl.GetPedido(ped, usuario));

            //ped = new Pedido();
            //ped.idPedido = Guid.Parse("9A035C68-9F2A-488A-B3B0-E40E439C6632");
            //pedidoList.Add(bl.GetPedido(ped, usuario));

            //ped = new Pedido();
            //ped.idPedido = Guid.Parse("AF794817-E1F9-4FF9-8FA5-E630EEC22584");
            //pedidoList.Add(bl.GetPedido(ped, usuario));

            //ped = new Pedido();
            //ped.idPedido = Guid.Parse("E35FA87B-AF6B-4401-A5BB-E8023AD8A91C");
            //pedidoList.Add(bl.GetPedido(ped, usuario));

            //ped = new Pedido();
            //ped.idPedido = Guid.Parse("7EC025C9-F069-4718-AF94-ED5BA2CB0D69");
            //pedidoList.Add(bl.GetPedido(ped, usuario));

            //ped = new Pedido();
            //ped.idPedido = Guid.Parse("E39A3CDD-0498-441D-81A4-FBE7AD3C3698");
            //pedidoList.Add(bl.GetPedido(ped, usuario));

            //ped = new Pedido();
            //ped.idPedido = Guid.Parse("72C5E8E8-7CEB-42FF-9AE3-B46C5F8D0162");
            //pedidoList.Add(bl.GetPedido(ped, usuario));


            //foreach (Pedido pedido in pedidoList)
            //{
            //    Pedido pedExists = pedidosSede.Where(p => p.ciudad.idCiudad.Equals(pedido.ciudad.idCiudad)).FirstOrDefault();
            //    if (pedExists == null)
            //    {
            //        pedidosSede.Add(pedido);
            //        foreach (PedidoDetalle det in pedido.pedidoDetalleList)
            //        {
            //            idsProducto.Add(det.producto.idProducto);
            //        }
            //    }
            //    else
            //    {

            //        foreach (PedidoDetalle det in pedido.pedidoDetalleList)
            //        {
            //            PedidoDetalle detExists = pedExists.pedidoDetalleList.Where(d => d.producto.idProducto.Equals(det.producto.idProducto)).FirstOrDefault();
            //            if (detExists == null)
            //            {
            //                detExists = det;
            //                pedExists.pedidoDetalleList.Add(det);
            //                idsProducto.Add(det.producto.idProducto);
            //            }
            //            else
            //            {
            //                detExists.cantidad = detExists.cantidad + det.cantidad;
            //            }
            //        }
            //    }
            //}

            //idsProducto = idsProducto.Distinct().ToList();

            //ProductoBL productoBl = new ProductoBL();
            //List<RegistroCargaStock> stocks = productoBl.StockProductosCadaSede(idsProducto, usuario.idUsuario);

            //List<Ciudad> sedes = new List<Ciudad>();
            //List<List<StockPedidoShow>> tableStocks = new List<List<StockPedidoShow>>();
            //List<StockPedidoShow> rowStocks = null;
            //RegistroCargaStock anterior = null;
            //StockPedidoShow cabezaFila = null;
            //string unidadFila = "";

            //foreach (RegistroCargaStock reg in stocks)
            //{
            //    if (stocks.ElementAt(0).producto.idProducto.Equals(reg.producto.idProducto))
            //    {
            //        sedes.Add(reg.ciudad);
            //    }

            //    if (anterior == null || !anterior.producto.idProducto.Equals(reg.producto.idProducto))
            //    {
            //        if (rowStocks != null)
            //        {
            //            cabezaFila.texto = cabezaFila.texto + " - " + unidadFila;
            //            tableStocks.Add(rowStocks);
            //        }
            //        rowStocks = new List<StockPedidoShow>();
            //        cabezaFila = new StockPedidoShow { texto = reg.producto.sku + " - " + reg.producto.descripcion, esStock = false };
            //        rowStocks.Add(cabezaFila);
            //    }


            //    Pedido pedi = pedidosSede.Where(p => p.ciudad.idCiudad.Equals(reg.ciudad.idCiudad)).FirstOrDefault();
            //    PedidoDetalle det = null;
            //    if (pedi != null)
            //    {
            //        det = pedi.pedidoDetalleList.Where(d => d.producto.idProducto.Equals(reg.producto.idProducto)).FirstOrDefault();
            //    }

            //    int semaforoStock = 0;

            //    string stockNro = "";

            //    if (det != null)
            //    {
            //        unidadFila = det.unidad;
            //        if (reg.stockNoDisponible)
            //        {
            //            stockNro = "No Registrado";
            //        }
            //        else
            //        {
            //            decimal stockPresentacion = 0;
            //            decimal separadaPresentacion = 0;
            //            decimal stockSeparado = 0;
            //            switch (det.idProductoPresentacion)
            //            {
            //                case 0:
            //                    stockPresentacion = reg.cantidadMpCalc;
            //                    separadaPresentacion = reg.cantidadSeparadaMpCalc;
            //                    break;
            //                case 1:
            //                    stockPresentacion = reg.cantidadAlternativaCalc;
            //                    separadaPresentacion = reg.cantidadSeparadaAlternativaCalc;
            //                    break;
            //                case 2:
            //                    stockPresentacion = reg.cantidadProveedorCalc;
            //                    separadaPresentacion = reg.cantidadSeparadaProveedorCalc;
            //                    break;
            //            }

            //            stockNro = stockPresentacion.ToString();
            //            stockSeparado = separadaPresentacion - det.cantidad;
            //            if (stockSeparado > 0)
            //            {
            //                stockNro = stockNro + " [" + stockSeparado.ToString() + "]";
            //            }

            //            if (stockPresentacion >= det.cantidad)
            //            {
            //                if (stockPresentacion >= separadaPresentacion)
            //                {
            //                    semaforoStock = 1;
            //                }
            //                else
            //                {
            //                    semaforoStock = 2;
            //                }
            //            }
            //            else
            //            {
            //                semaforoStock = 3;
            //            }
            //        }

            //    }

            //    rowStocks.Add(new StockPedidoShow
            //    {
            //        texto = det != null ? det.cantidad.ToString() : "-",
            //        esStock = det != null ? true : false,
            //        textoStock = "STOCK: " + stockNro,
            //        semaforoStock = semaforoStock
            //    });

            //    anterior = reg;
            //}

            //if (rowStocks != null)
            //{
            //    cabezaFila.texto = cabezaFila.texto + " - " + unidadFila;
            //    tableStocks.Add(rowStocks);
            //}

            //ViewBag.numerosPedido = "423";
            //ViewBag.numeroGrupo = 24;
            //ViewBag.tableStocks = tableStocks;
            //ViewBag.sedes = sedes;

            return View();
        }


        private void addProductoCargaMasiva(Pedido pedido, String SKU, int esUnidadAlternativa, int cantidad, Decimal precioNeto)
        {

            PedidoDetalle detalle = new PedidoDetalle(pedido.usuario.visualizaCostos, pedido.usuario.visualizaMargen);
            ProductoBL productoBL = new ProductoBL();

            Guid idProducto = productoBL.getProductoId(SKU);
            Producto producto = productoBL.getProducto(idProducto, pedido.ciudad.esProvincia, pedido.incluidoIGV, pedido.cliente.idCliente);
            detalle.producto = producto;


            /*SE REQUIERE CALCULAR*/
            detalle.porcentajeDescuento = 0;// Decimal.Parse(Request["porcentajeDescuento"].ToString());
           

            detalle.cantidad = cantidad;          
            detalle.esPrecioAlternativo = esUnidadAlternativa == 1;


            detalle.observacion = String.Empty;
            //decimal precioNeto = Decimal.Parse(Request["precio"].ToString());
           // decimal costo = Decimal.Parse(Request["costo"].ToString());
            decimal flete = 0;// Decimal.Parse(Request["flete"].ToString());
            if (detalle.esPrecioAlternativo)
            {
                detalle.ProductoPresentacion = new ProductoPresentacion();
                detalle.ProductoPresentacion.Equivalencia = detalle.producto.ProductoPresentacionList[0].Equivalencia;

                if (detalle.ProductoPresentacion.Equivalencia > 1)
                {
                    detalle.ProductoPresentacion.IdProductoPresentacion = 1;
                }

                if (detalle.ProductoPresentacion.Equivalencia < 1)
                {
                    detalle.ProductoPresentacion.IdProductoPresentacion = 2;
                }

                //Si es el precio Alternativo se multiplica por la equivalencia para que se registre el precio estandar
                //dado que cuando se hace get al precioNetoEquivalente se recupera diviendo entre la equivalencia
                detalle.precioNeto = Decimal.Parse(String.Format(Constantes.formatoCuatroDecimales, precioNeto * detalle.ProductoPresentacion.Equivalencia));

                //Si es el precio Alternativo se debe modificar el precio_cliente_producto para que compare con el precio
                //de la unidad alternativa en lugar del precio de la unidad estandar
                //detalle.producto.precioClienteProducto.precioUnitario =
                //  detalle.producto.precioClienteProducto.precioUnitario / producto.equivalencia;
                detalle.producto.precioClienteProducto.precioUnitario =
                detalle.producto.precioClienteProducto.precioUnitario / detalle.ProductoPresentacion.Equivalencia;

                
            }
            else
            {
                detalle.precioNeto = precioNeto;
            }
            detalle.porcentajeDescuento = 100 - (precioNeto * 100 / detalle.precioLista);

            detalle.flete = flete;
            pedido.pedidoDetalleList.Add(detalle);

            //CotizacionDetalle cotizacionDetalle = (CotizacionDetalle)Convert.ChangeType(pedido, typeof(CotizacionDetalle));
            //Calcula los montos totales de la cabecera de la cotizacion
            PedidoBL pedidoBL = new PedidoBL();
            pedidoBL.calcularMontosTotales(pedido);


            detalle.unidad = detalle.producto.unidad;
            //si esPrecioAlternativo  se mostrará la unidad alternativa
            if (detalle.esPrecioAlternativo)
            {
                detalle.unidad = detalle.producto.unidad_alternativa;
            }

            var nombreProducto = detalle.producto.descripcion;
            /* if (pedido.mostrarCodigoProveedor)
             {*/
            nombreProducto = detalle.producto.skuProveedor + " - " + detalle.producto.descripcion;

            Decimal precioUnitarioRegistrado = detalle.producto.precioClienteProducto.precioUnitario;


        }


        private Pedido instanciarPedidoCargaMasiva(String ruc, String nombreSolicitante, 
            String numeroRequerimiento,
            DireccionEntrega direccionEntrega, String observaciones, 
            DateTime? fechaEntregaDesde, DateTime? fechaEntregaHasta,
            String inicioPrimerTurnoEntrega, String finPrimerTurnoEntrega, String inicioSegundoTurnoEntrega, String finSegundoTurnoEntrega,
            String numeroOrdenCompra, Pedido.ClasesPedido tipoPedido, Pedido.tiposPedido tipoPedidoVenta, Pedido.tiposPedidoAlmacen tipoPedidoAlmacen, String facturaConsolidada,
            Int64 numeroGrupo
            )
        {
            Pedido pedido = new Pedido(Pedido.ClasesPedido.Venta);
            pedido.tipoPedido = tipoPedidoVenta;
            pedido.idPedido = Guid.Empty;
            pedido.numeroPedido = 0;
            pedido.numeroGrupoPedido = numeroGrupo;
            pedido.cotizacion = new Cotizacion();
            pedido.observaciones = "";
            pedido.observacionesGuiaRemision = observaciones;
          //  pedido.ubigeoEntrega = new Ubigeo();
           // pedido.ubigeoEntrega.Id = ubigeo;

            //ClienteBL clienteBL = new ClienteBL();

       /*     Guid idCliente = clienteBL.getClienteId(ruc, sedeMP);
            pedido.cliente = clienteBL.getCliente(idCliente);
            pedido.ciudad = pedido.cliente.ciudad;*/
            
            pedido.ciudadASolicitar = new Ciudad();
            pedido.numeroReferenciaCliente = numeroOrdenCompra;

            DireccionEntregaBL direccionEntregaBL = new DireccionEntregaBL();
            pedido.direccionEntrega = direccionEntregaBL.getDireccionEntregaPorCodigo(direccionEntrega.codigo);
            
            pedido.ubigeoEntrega = pedido.direccionEntrega.ubigeo;
            pedido.contactoPedido = pedido.direccionEntrega.contacto;
            pedido.telefonoContactoPedido = pedido.direccionEntrega.telefono;
            pedido.cliente = pedido.direccionEntrega.cliente;
            pedido.ciudad = pedido.direccionEntrega.cliente.ciudad;


            pedido.numeroRequerimiento = numeroRequerimiento;
            pedido.solicitante = new Solicitante();
            pedido.solicitante.nombre = nombreSolicitante;         

            pedido.fechaSolicitud = DateTime.Now;
            pedido.fechaEntregaDesde = fechaEntregaDesde.Value;
            pedido.fechaEntregaHasta = fechaEntregaHasta.Value;
            pedido.horaEntregaDesde = inicioPrimerTurnoEntrega;
            pedido.horaEntregaHasta = finPrimerTurnoEntrega;
            pedido.contactoPedido = String.Empty;
            pedido.telefonoContactoPedido = String.Empty;
            pedido.incluidoIGV = false;
            pedido.promociones = new List<Promocion>();

            pedido.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            pedido.seguimientoPedido = new SeguimientoPedido();
            pedido.seguimientoCrediticioPedido = new SeguimientoCrediticioPedido();
            pedido.pedidoDetalleList = new List<PedidoDetalle>();
            pedido.pedidoAdjuntoList = new List<PedidoAdjunto>();
            pedido.fechaPrecios = pedido.fechaSolicitud.AddDays(Constantes.DIAS_MAX_BUSQUEDA_PRECIOS * -1);

            return pedido;

        }


        [HttpPost]
        public ActionResult Load(HttpPostedFileBase file)
        {
            /*try
            {*/
                PedidoBL pedidoBL = new PedidoBL();
                //Se obtiene el número de Grupo
                Int64 numeroGrupo =  pedidoBL.GetSiguienteNumeroGrupoPedido();

                XSSFWorkbook hssfwb = new XSSFWorkbook(file.InputStream);
                ISheet sheet = hssfwb.GetSheetAt(0);
                int row = 1;
                //Si se ha ingresado la última fila  entonces  se considera ese valor como límite
                int ultimaFila = Int32.Parse(Request["ultimaFila"].ToString());
                if (ultimaFila == 0)
                {
                    ultimaFila = sheet.LastRowNum;
                }
                else
                {
                    ultimaFila--;
                }


                /*Datos de Cliente y Solicitud*/
                DateTime? fechaSolicitud = UtilesHelper.getValorCeldaDate(sheet, 2, "B");
                String razonSocial = UtilesHelper.getValorCelda(sheet, 3, "B");
                String ruc = UtilesHelper.getValorCelda(sheet,4,"B");
                String nombreComercial = UtilesHelper.getValorCelda(sheet, 5, "B");
                String pedidoContado = UtilesHelper.getValorCelda(sheet, 6, "B"); //Se integra con implementación pendiente de Yrving
                DateTime? fechaEntregaDesdeGeneral = UtilesHelper.getValorCeldaDate(sheet, 7, "B");
                DateTime? fechaEntregaHastaGeneral = UtilesHelper.getValorCeldaDate(sheet, 8, "B");
                String inicioPrimerTurnoEntrega = UtilesHelper.getValorCelda(sheet, 9, "B");
                String finPrimerTurnoEntrega = UtilesHelper.getValorCelda(sheet, 9, "C");
                String inicioSegundoTurnoEntrega = UtilesHelper.getValorCelda(sheet, 10, "B");
                String finSegundoTurnoEntrega = UtilesHelper.getValorCelda(sheet, 10, "C");
                String numeroOrdenCompra = UtilesHelper.getValorCelda(sheet, 11, "B");
                String facturaConsolidada = UtilesHelper.getValorCelda(sheet, 12, "B");

                String tipoPedidoString = UtilesHelper.getValorCelda(sheet, 3, "E");
                Pedido.ClasesPedido tipoPedido;
                Pedido.tiposPedido tipoPedidoVenta = Pedido.tiposPedido.Venta;
                Pedido.tiposPedidoAlmacen tipoPedidoTrasladoInterno = Pedido.tiposPedidoAlmacen.TrasladoInterno;
                if (tipoPedidoString.Equals("Venta"))
                {
                    tipoPedido = Pedido.ClasesPedido.Venta;
                    tipoPedidoVenta = Pedido.tiposPedido.Venta;
                }
                else if (tipoPedidoString.Equals("Transferencia Gratuita"))
                {
                    tipoPedido = Pedido.ClasesPedido.Venta;
                    tipoPedidoVenta = Pedido.tiposPedido.TransferenciaGratuitaEntregada;
                }
                else
                {
                    tipoPedido = Pedido.ClasesPedido.Almacen;
                    tipoPedidoTrasladoInterno = Pedido.tiposPedidoAlmacen.TrasladoInterno;
                }

                String sedeSolicitante = String.Empty;
                if (tipoPedido == Pedido.ClasesPedido.Almacen)
                {
                    sedeSolicitante = UtilesHelper.getValorCelda(sheet, 4, "E");
                }

                String solicitante = UtilesHelper.getValorCelda(sheet, 5, "E");

                List<Decimal> subTotales = new List<Decimal>();
                //List<String> ubigeos = new List<String>();
                List<DireccionEntrega> direccionEntregaList = new List<DireccionEntrega>();

                List<Pedido> pedidoList = new List<Pedido>();
                Pedido ultimoPedido = new Pedido();

                List<Exception> exceptionList = new List<Exception>();

                //Se considera la ultimafila más uno porque estamos trabajando con las posiciones físicas.
                for (row = 16; row <= ultimaFila + 1; row++)
                {
                   /* try
                    {*/
                        //Se identifica el clasePedido de fila
                        String tipoCabecera = UtilesHelper.getValorCelda(sheet, row, "D");
                        if (tipoCabecera.Equals("C"))
                        {
                            //Si es cabecera se instancia el pedido
                            DireccionEntrega direccionEntrega = new DireccionEntrega();
                            direccionEntrega.codigo = UtilesHelper.getValorCeldaInt(sheet, row, "A");

                            /*String codigoCentroCostosCliente = UtilesHelper.getValorCelda(sheet, row, "A");
                            String codigoCentroCostosMP = UtilesHelper.getValorCelda(sheet, row, "B");
                            String nombreCentroCostos = UtilesHelper.getValorCelda(sheet, row, "D");
                            String direccionEntrega = UtilesHelper.getValorCelda(sheet, row, "E");
                            */
                            String numeroRequerimiento = UtilesHelper.getValorCelda(sheet, row, "C");
                            String observaciones = UtilesHelper.getValorCelda(sheet, row, "G");
                            

                            /*String sedeMP = UtilesHelper.getValorCelda(sheet, row, "G");
                            String ubigeo = UtilesHelper.getValorCelda(sheet, row, "H");*/

                            DateTime? fechaEntregaDesde = UtilesHelper.getValorCeldaDate(sheet, row, "N");
                            DateTime? fechaEntregaHasta = UtilesHelper.getValorCeldaDate(sheet, row, "O");

                            if (fechaEntregaDesde == null || fechaEntregaDesde == default(DateTime))
                            {
                                fechaEntregaDesde = fechaEntregaDesdeGeneral;
                            }
                            if (fechaEntregaHasta == null || fechaEntregaHasta == default(DateTime))
                            {
                                fechaEntregaHasta = fechaEntregaHastaGeneral;
                            }
                            //Se agrega el subTotal solo para realizar una validación
                            Decimal subtotal = UtilesHelper.getValorCeldaDecimal(sheet, row, "J");
                            subTotales.Add(subtotal);
                            //fechaEntregaHasta = fechaEntregaHastaGeneral;
                            //  ubigeos.Add(ubigeo);
                            direccionEntregaList.Add(direccionEntrega);

                            /*String inicioPrimerTurnoEntrega = UtilesHelper.getValorCelda(sheet, 9, "B");
                            String finPrimerTurnoEntrega = UtilesHelper.getValorCelda(sheet, 9, "C");
                            String inicioSegundoTurnoEntrega = UtilesHelper.getValorCelda(sheet, 10, "B");
                            String finSegundoTurnoEntrega = UtilesHelper.getValorCelda(sheet, 10, "C");*/

                            Pedido pedido = this.instanciarPedidoCargaMasiva(ruc, solicitante,
                                numeroRequerimiento,
                                direccionEntrega, observaciones, fechaEntregaDesdeGeneral, fechaEntregaHasta,
                                inicioPrimerTurnoEntrega, finPrimerTurnoEntrega, inicioSegundoTurnoEntrega, finSegundoTurnoEntrega,
                                numeroOrdenCompra, tipoPedido, tipoPedidoVenta, tipoPedidoTrasladoInterno, facturaConsolidada,
                                numeroGrupo);
                            ultimoPedido = pedido;
                            pedidoList.Add(pedido);
                        }
                        else if(tipoCabecera.Equals("D"))
                        {

                            String skuMP = UtilesHelper.getValorCelda(sheet, row, "E");
                            int unidadAlternativa = UtilesHelper.getValorCeldaInt(sheet, row, "G");
                            int cantidad = UtilesHelper.getValorCeldaInt(sheet, row, "H");
                            decimal precioNeto = UtilesHelper.getValorCeldaDecimal(sheet, row, "I");

                            if (cantidad > 0 && precioNeto > 0)
                            {
                                //Si es detalle se agrega el producto al último pedido
                                addProductoCargaMasiva(ultimoPedido, skuMP, unidadAlternativa, cantidad, precioNeto);
                            }
                        }                
                    /*}
                    catch (Exception ex)
                    {

                        Usuario usuario = (Usuario)this.Session["usuario"];
                        Log log = new Log(ex.ToString() + " paso:", TipoLog.Error, usuario);
                        LogBL logBL = new LogBL();
                        logBL.insertLog(log);
                        throw ex;
                    }*/
                }

                String numerosPedido = String.Empty;
                int contadorPedidos = 0;

                List<Guid> idsProducto = new List<Guid>();
                List<Pedido> pedidosSede = new List<Pedido>();
                
                foreach (Pedido pedido in pedidoList)
                {
                    pedidoBL.InsertPedido(pedido);
                    numerosPedido = numerosPedido + pedido.numeroPedidoString;
                    if (pedido.montoSubTotal != subTotales[contadorPedidos])
                    {
                        numerosPedido = numerosPedido + "[Diferencia subtotal] ";
                    }

                    Pedido pedExists = pedidosSede.Where(p => p.ciudad.idCiudad.Equals(pedido.ciudad.idCiudad)).FirstOrDefault();
                    if (pedExists == null)
                    {
                        pedidosSede.Add(pedido);
                        foreach (PedidoDetalle det in pedido.pedidoDetalleList)
                        {
                            idsProducto.Add(det.producto.idProducto);
                        }
                    }
                    else
                    {

                        foreach (PedidoDetalle det in pedido.pedidoDetalleList)
                        {
                            PedidoDetalle detExists = pedExists.pedidoDetalleList.Where(d => d.producto.idProducto.Equals(det.producto.idProducto)).FirstOrDefault();
                            if (detExists == null)
                            {
                                detExists = det;
                                pedExists.pedidoDetalleList.Add(det);
                                idsProducto.Add(det.producto.idProducto);
                            }
                            else
                            {
                                detExists.cantidad = detExists.cantidad + det.cantidad;
                            }
                        }
                    }
                    numerosPedido = numerosPedido + ",";

                    contadorPedidos++;

                }

                idsProducto = idsProducto.Distinct().ToList();

                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                
                ProductoBL productoBl = new ProductoBL();
                List<RegistroCargaStock> stocks = productoBl.StockProductosCadaSede(idsProducto, usuario.idUsuario);

            List<Ciudad> sedes = new List<Ciudad>();
            List<List<StockPedidoShow>> tableStocks = new List<List<StockPedidoShow>>();
            List<StockPedidoShow> rowStocks = null;
            RegistroCargaStock anterior = null;
            StockPedidoShow cabezaFila = null;
            string unidadFila = "";

            foreach (RegistroCargaStock reg in stocks)
            {
                if (stocks.ElementAt(0).producto.idProducto.Equals(reg.producto.idProducto))
                {
                    sedes.Add(reg.ciudad);
                }

                if (anterior == null || !anterior.producto.idProducto.Equals(reg.producto.idProducto))
                {
                    if (rowStocks != null)
                    {
                        cabezaFila.texto = cabezaFila.texto + " - " + unidadFila;
                        tableStocks.Add(rowStocks);
                    }
                    rowStocks = new List<StockPedidoShow>();
                    cabezaFila = new StockPedidoShow { texto = reg.producto.sku + " - " + reg.producto.descripcion, esStock = false };
                    rowStocks.Add(cabezaFila);
                }


                Pedido pedi = pedidosSede.Where(p => p.ciudad.idCiudad.Equals(reg.ciudad.idCiudad)).FirstOrDefault();
                PedidoDetalle det = null;
                if (pedi != null)
                {
                    det = pedi.pedidoDetalleList.Where(d => d.producto.idProducto.Equals(reg.producto.idProducto)).FirstOrDefault();
                }

                int semaforoStock = 0;

                string stockNro = "";

                if (det != null)
                {
                    unidadFila = det.unidad;
                    if (reg.stockNoDisponible)
                    {
                        stockNro = "No Registrado";
                    }
                    else
                    {
                        decimal stockPresentacion = 0;
                        decimal separadaPresentacion = 0;
                        decimal stockSeparado = 0;
                        switch (det.idProductoPresentacion)
                        {
                            case 0:
                                stockPresentacion = reg.cantidadMpCalc;
                                separadaPresentacion = reg.cantidadSeparadaMpCalc;
                                break;
                            case 1:
                                stockPresentacion = reg.cantidadAlternativaCalc;
                                separadaPresentacion = reg.cantidadSeparadaAlternativaCalc;
                                break;
                            case 2:
                                stockPresentacion = reg.cantidadProveedorCalc;
                                separadaPresentacion = reg.cantidadSeparadaProveedorCalc;
                                break;
                        }

                        stockNro = stockPresentacion.ToString();
                        stockSeparado = separadaPresentacion - det.cantidad;
                        if (stockSeparado > 0)
                        {
                            stockNro = stockNro + " [" + stockSeparado.ToString() + "]";
                        }

                        if (stockPresentacion >= det.cantidad)
                        {
                            if (stockPresentacion >= separadaPresentacion)
                            {
                                semaforoStock = 1;
                            }
                            else
                            {
                                semaforoStock = 2;
                            }
                        }
                        else
                        {
                            semaforoStock = 3;
                        }
                    }

                }

                rowStocks.Add(new StockPedidoShow
                {
                    texto = det != null ? det.cantidad.ToString() : "-",
                    esStock = det != null ? true : false,
                    textoStock = "STOCK: " + stockNro,
                    semaforoStock = semaforoStock
                });

                anterior = reg;
            }

            if (rowStocks != null)
            {
                cabezaFila.texto = cabezaFila.texto + " - " + unidadFila;
                tableStocks.Add(rowStocks);
            }

            ViewBag.numerosPedido = numerosPedido.Substring(0, numerosPedido.Length - 1);
                ViewBag.numeroGrupo = numeroGrupo;
                ViewBag.tableStocks = tableStocks;
                ViewBag.sedes = sedes;
            /*}
            catch (Exception ex)
            {
                Usuario usuario = (Usuario)this.Session["usuario"];
                Log log = new Log(ex.ToString(), TipoLog.Error, usuario);
                LogBL logBL = new LogBL();
                logBL.insertLog(log);
                throw ex;
                //return View("CargaIncorrecta");
            }*/
            return View();
        }

        public void UpdateDetallesRestriccion()
        {
            Guid idPedido = Guid.Parse(Request["idPedido"].ToString());

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (usuario.apruebaPedidos)
            {
                PedidoBL pedidoBL = new PedidoBL();

                List<Guid> idDetalles = (List<Guid>)this.Session["pedidoDRIds"];
                List<int> cantidades = (List<int>)this.Session["pedidoDRCantidades"];
                List<String> comentarios = (List<String>)this.Session["pedidoDRComentarios"];

                pedidoBL.UpdateDetallesRestriccion(idPedido, idDetalles, cantidades, comentarios, usuario.idUsuario);
            }
        }

        public String GetHistorial()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            PedidoBL pedidoBL = new PedidoBL();
            List<SeguimientoPedido> historial = new List<SeguimientoPedido>();
            Guid idPedido = Guid.Parse(Request["id"].ToString());
            historial = pedidoBL.GetHistorialSeguimiento(idPedido);

            String json = "";

            foreach (SeguimientoPedido seg in historial)
            {
                string jsonItem = JsonConvert.SerializeObject(seg);
                if (json.Equals(""))
                {
                    json = jsonItem;
                }
                else
                {
                    json = json + "," + jsonItem;
                }
            }

            json = "{\"result\": [" + json + "]}";
            return json;
        }

        public String GetHistorialCrediticio()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            PedidoBL pedidoBL = new PedidoBL();
            List<SeguimientoCrediticioPedido> historial = new List<SeguimientoCrediticioPedido>();
            Guid idPedido = Guid.Parse(Request["id"].ToString());
            historial = pedidoBL.GetHistorialSeguimientoCrediticio(idPedido);

            String json = "";

            foreach (SeguimientoCrediticioPedido seg in historial)
            {
                string jsonItem = JsonConvert.SerializeObject(seg);
                if (json.Equals(""))
                {
                    json = jsonItem;
                }
                else
                {
                    json = json + "," + jsonItem;
                }
            }

            json = "{\"result\": [" + json + "]}";
            return json;
        }

        public void changeMostrarCosto()
        {
            Pedido pedido = this.PedidoSession;
            pedido.mostrarCosto = Boolean.Parse(this.Request.Params["mostrarCosto"]);
            this.PedidoSession = pedido;
        }


        public void SetDetalleRestriccion()
        {
            List<Guid> idDetalles = (List<Guid>)this.Session["pedidoDRIds"];
            List<int> cantidades = (List<int>)this.Session["pedidoDRCantidades"];
            List<String> comentarios = (List<String>)this.Session["pedidoDRComentarios"];

            Guid idDetallePedido = Guid.Parse(Request["idDetalle"].ToString());
            int cantidad = int.Parse(Request["cantidad"].ToString());
            String comentario = Request["comentario"].ToString();

            int idx = idDetalles.IndexOf(idDetallePedido);

            if (idx >= 0)
            {
                cantidades[idx] = cantidad;
                comentarios[idx] = comentario;
            } else
            {
                idDetalles.Add(idDetallePedido);
                cantidades.Add(cantidad);
                comentarios.Add(comentario);
            }

            this.Session["pedidoDRIds"] = idDetalles;
            this.Session["pedidoDRCantidades"] = cantidades;
            this.Session["pedidoDRComentarios"] = comentarios;
        }
    }
}
 
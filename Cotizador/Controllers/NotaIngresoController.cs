using BusinessLayer;
using Cotizador.ExcelExport;
using Cotizador.Models.DTOsSearch;
using Cotizador.Models.DTOsShow;
using Model;
using Model.EXCEPTION;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Cotizador.Controllers
{
    public class NotaIngresoController : Controller
    {

        private NotaIngreso NotaIngresoSession
        {
            get
            {

                NotaIngreso notaIngreso = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaNotasIngreso: notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_BUSQUEDA]; break;
                    case Constantes.paginas.MantenimientoNotaIngreso: notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO]; break;
                        //       case Constantes.paginas.BusquedaGuiasRemisionConsolidarFactura: notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_BUSQUEDA_FACTURA_CONSOLIDADA]; break;
                }
                return notaIngreso;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaNotasIngreso: this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_BUSQUEDA] = value; break;
                    case Constantes.paginas.MantenimientoNotaIngreso: this.Session[Constantes.VAR_SESSION_NOTA_INGRESO] = value; break;
                        //      case Constantes.paginas.BusquedaGuiasRemisionConsolidarFactura: this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_BUSQUEDA_FACTURA_CONSOLIDADA] = value; break;
                }
            }
        }

        public String SearchClientes()
        {
            String data = this.Request.Params["data[q]"];
            ClienteBL clienteBL = new ClienteBL();
            NotaIngreso notaIngreso = this.NotaIngresoSession;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            return clienteBL.getCLientesBusqueda(data, notaIngreso.ciudadDestino.idCiudad, usuario.idUsuario);
        }

        public String GetCliente()
        {
            NotaIngreso notaIngreso = this.NotaIngresoSession;
            Guid idCliente = Guid.Parse(Request["idCliente"].ToString());
            ClienteBL clienteBl = new ClienteBL();
            notaIngreso.pedido.cliente = clienteBl.getCliente(idCliente);
            String resultado = JsonConvert.SerializeObject(notaIngreso.pedido.cliente);
            this.NotaIngresoSession = notaIngreso;
            return resultado;
        }

        public String GetGrupoCliente()
        {
            NotaIngreso notaIngreso = this.NotaIngresoSession;
            int idGrupoCliente = int.Parse(Request["idGrupoCliente"].ToString());

            notaIngreso.pedido.idGrupoCliente = idGrupoCliente;

            this.NotaIngresoSession = notaIngreso;
            return "";
        }


        #region Busqueda Guias

        private void instanciarNotaIngresoBusqueda()
        {
            NotaIngreso notaIngreso = new NotaIngreso();
            notaIngreso.motivoTrasladoBusqueda = NotaIngreso.motivosTrasladoBusqueda.Todos;
            notaIngreso.seguimientoMovimientoAlmacenSalida = new SeguimientoMovimientoAlmacenSalida();
            notaIngreso.seguimientoMovimientoAlmacenSalida.estado = SeguimientoMovimientoAlmacenSalida.estadosSeguimientoMovimientoAlmacenSalida.Enviado;
            notaIngreso.ciudadDestino = new Ciudad();
            notaIngreso.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            notaIngreso.estadoFiltro = NotaIngreso.EstadoFiltro.Todos;

            notaIngreso.pedido = new Pedido();
            notaIngreso.pedido.cliente = new Cliente();
            notaIngreso.pedido.cliente.idCliente = Guid.Empty;
            notaIngreso.pedido.buscarSedesGrupoCliente = false;

            notaIngreso.fechaTrasladoDesde = DateTime.Now.AddDays(-Constantes.DIAS_DESDE_BUSQUEDA);
            notaIngreso.fechaTrasladoHasta = DateTime.Now.AddDays(0);
            notaIngreso.fechaEmisionDesde = DateTime.Now.AddDays(-Constantes.DIAS_DESDE_BUSQUEDA);
            notaIngreso.fechaEmisionHasta = DateTime.Now.AddDays(0);
            notaIngreso.estaFacturado = true;

            this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_BUSQUEDA] = notaIngreso;
        }

        public void CleanBusqueda()
        {
            instanciarNotaIngresoBusqueda();
        }

        public ActionResult Index(Guid? idMovimientoAlmacen = null)
        {


            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaGuiasRemision;

            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_BUSQUEDA] == null)
            {
                instanciarNotaIngresoBusqueda();
            }

            if (this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_LISTA] == null)
            {
                this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_LISTA] = new List<NotaIngreso>();
            }

            NotaIngreso notaIngresoSearch = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_BUSQUEDA];

            ViewBag.notaIngreso = notaIngresoSearch;
            ViewBag.notaIngresoList = this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_LISTA];
            ViewBag.pagina = (int)Constantes.paginas.BusquedaGuiasRemision;

            ViewBag.fechaEmisionDesde = notaIngresoSearch.fechaEmisionDesde.ToString(Constantes.formatoFecha);
            ViewBag.fechaEmisionHasta = notaIngresoSearch.fechaEmisionHasta.ToString(Constantes.formatoFecha);
            ViewBag.Si = Constantes.MENSAJE_SI;
            ViewBag.No = Constantes.MENSAJE_NO;

            int existeCliente = 0;
            if (notaIngresoSearch.pedido.cliente.idCliente != Guid.Empty)
            {
                existeCliente = 1;
            }

            Pedido pedido = new Pedido();

            ViewBag.pedido = pedido;

            DocumentoVenta documentoVenta = new DocumentoVenta();
            ViewBag.documentoVenta = documentoVenta;

            ViewBag.existeCliente = existeCliente;


            /*     int mostrarGuia = 0;
                 if (idMovimientoAlmacen != null)
                 {
                     mostrarGuia = 1;
                 }

                 ViewBag.mostrarGuia = mostrarGuia;*/
            ViewBag.idMovimientoAlmacen = idMovimientoAlmacen;

            return View();
        }

        [HttpGet]
        public ActionResult ExportLastSearchExcel()
        {
            List<NotaIngreso> list = (List<NotaIngreso>)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_LISTA];

            NotaIngresoSearch excel = new NotaIngresoSearch();
            return excel.generateExcel(list);
        }

        public String Search()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaNotasIngreso;

            //Se recupera el pedido Búsqueda de la session
            NotaIngreso notaIngreso = this.NotaIngresoSession;

            String[] movDesde = this.Request.Params["fechaEmisionDesde"].Split('/');
            notaIngreso.fechaEmisionDesde = new DateTime(Int32.Parse(movDesde[2]), Int32.Parse(movDesde[1]), Int32.Parse(movDesde[0]));

            String[] movHasta = this.Request.Params["fechaEmisionHasta"].Split('/');
            notaIngreso.fechaEmisionHasta = new DateTime(Int32.Parse(movHasta[2]), Int32.Parse(movHasta[1]), Int32.Parse(movHasta[0]), 23, 59, 59);


            if (this.Request.Params["numeroDocumento"] == null || this.Request.Params["numeroDocumento"].Trim().Length == 0)
            {
                notaIngreso.numeroDocumento = 0;
            }
            else
            {
                notaIngreso.numeroDocumento = int.Parse(this.Request.Params["numeroDocumento"]);
            }

            if (this.Request.Params["numeroPedido"] == null || this.Request.Params["numeroPedido"].Trim().Length == 0)
            {
                notaIngreso.pedido.numeroPedido = 0;
            }
            else
            {
                notaIngreso.pedido.numeroPedido = int.Parse(this.Request.Params["numeroPedido"]);
            }

            if (this.Request.Params["numeroGuiaReferencia"] == null || this.Request.Params["numeroGuiaReferencia"].Trim().Length == 0)
            {
                notaIngreso.numeroGuiaReferencia = 0;
            }
            else
            {
                notaIngreso.numeroGuiaReferencia = int.Parse(this.Request.Params["numeroGuiaReferencia"]);
            }
            

            MovimientoAlmacenBL movimientoAlmacenBL = new MovimientoAlmacenBL();


            List<NotaIngreso> notaIngresoList = movimientoAlmacenBL.GetNotasIngreso(notaIngreso);
            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_LISTA] = notaIngresoList;
            this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_BUSQUEDA] = notaIngreso;
            //Se retorna la cantidad de elementos encontrados
            return JsonConvert.SerializeObject(ParserDTOsSearch.NotaIngresoToNotaIngresoDTO(notaIngresoList));
            //return JsonConvert.SerializeObject(notaIngresoList);


            //return pedidoList.Count();
        }

        #endregion


        #region Crear Guia

        public Boolean ConsultarSiExisteNotaIngreso()
        {
            NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO];
            if (notaIngreso == null)
            {
                return false;
            }
            else
                return true;
        }

        private void instanciarNotaIngreso(Ciudad ciudad)
        {
            NotaIngreso notaIngreso = new NotaIngreso();
            notaIngreso.fechaEmision = DateTime.Now;
            notaIngreso.fechaTraslado = DateTime.Now;
            notaIngreso.motivoTraslado = NotaIngreso.motivosTraslado.Compra;
            notaIngreso.transportista = new Transportista();
            notaIngreso.ciudadDestino = ciudad;
            notaIngreso.pedido = new Pedido();
            notaIngreso.pedido.pedidoDetalleList = new List<PedidoDetalle>();
            notaIngreso.pedido.ciudad = new Ciudad();
            notaIngreso.pedido.ubigeoEntrega = new Ubigeo();
            notaIngreso.ciudadDestino.transportistaList = new List<Transportista>();
            notaIngreso.seguimientoMovimientoAlmacenEntrada = new SeguimientoMovimientoAlmacenEntrada();
            //  notaIngreso.certificadoInscripcion = ".";

            SerieDocumentoBL serieDocumentoBL = new SerieDocumentoBL();
            notaIngreso.ciudadDestino.serieDocumentoElectronicoList = serieDocumentoBL.getSeriesDocumento(ciudad.idCiudad);
            notaIngreso.serieDocumentoElectronico = notaIngreso.ciudadDestino.serieDocumentoElectronicoList[0];
            notaIngreso.serieDocumento = notaIngreso.serieDocumentoElectronico.serie;
            notaIngreso.numeroDocumento = notaIngreso.serieDocumentoElectronico.siguienteNumeroNotaIngreso;
        

            this.Session[Constantes.VAR_SESSION_NOTA_INGRESO] = notaIngreso;

        }


        public void iniciarAtencionDesdePedido()
        {
            try
            {
                Pedido pedido = null;
                if ((Pedido.ClasesPedido)Char.Parse(Request.Params["tipo"]) == Pedido.ClasesPedido.Venta)
                {
                    pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_VER];
                }
                else if ((Pedido.ClasesPedido)Char.Parse(Request.Params["tipo"]) == Pedido.ClasesPedido.Compra)
                {
                    pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_VER];
                }
                else if ((Pedido.ClasesPedido)Char.Parse(Request.Params["tipo"]) == Pedido.ClasesPedido.Almacen)
                {
                    pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_ALMACEN_VER];
                }



                if (this.Session[Constantes.VAR_SESSION_NOTA_INGRESO] == null)
                {
                    instanciarNotaIngreso(pedido.ciudad);
                }
                NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO];
                notaIngreso.pedido = pedido;



                if ((Pedido.ClasesPedido)Char.Parse(Request.Params["tipo"]) == Pedido.ClasesPedido.Venta)
                {
                    notaIngreso.motivoTraslado = (NotaIngreso.motivosTraslado)(char)pedido.tipoPedido;
                }
                else if ((Pedido.ClasesPedido)Char.Parse(Request.Params["tipo"]) == Pedido.ClasesPedido.Compra)
                {
                    notaIngreso.motivoTraslado = (NotaIngreso.motivosTraslado)(char)pedido.tipoPedidoCompra;

                    if (pedido.almacenOrigen != null && !pedido.almacenOrigen.idAlmacen.Equals(Guid.Empty))
                    {
                        notaIngreso.idAlmacen = pedido.almacenOrigen.idAlmacen;
                        notaIngreso.direccionLlegada = pedido.almacenOrigen.direccion;
                    }
                }
                else if ((Pedido.ClasesPedido)Char.Parse(Request.Params["tipo"]) == Pedido.ClasesPedido.Almacen)
                {
                    notaIngreso.motivoTraslado = (NotaIngreso.motivosTraslado)(char)pedido.tipoPedidoAlmacen;
                }


                notaIngreso.transportista = new Transportista();

                notaIngreso.observaciones = String.Empty;
                if (pedido.numeroReferenciaCliente != null && !pedido.numeroReferenciaCliente.Trim().Equals(String.Empty))
                {
                    if (pedido.numeroReferenciaCliente.Contains("O/C"))
                    {
                        notaIngreso.observaciones = pedido.numeroReferenciaCliente.Trim() + " ";
                    }
                    else
                    {
                        notaIngreso.observaciones = "O/C N° " + pedido.numeroReferenciaCliente.Trim() + " ";
                    }
                }
                notaIngreso.observaciones = notaIngreso.observaciones + pedido.observacionesGuiaRemision;

                CiudadBL ciudadBL = new CiudadBL();


                Ciudad ciudadDestino = ciudadBL.getCiudad(notaIngreso.ciudadDestino.idCiudad);
                notaIngreso.ciudadDestino.nombre = ciudadDestino.nombre;
                notaIngreso.ciudadDestino.direccionPuntoLlegada = ciudadDestino.direccionPuntoPartida;

               // notaIngreso.ciudadDestino = ciudadDestino;

                notaIngreso.transportista = new Transportista();
                TransportistaBL transportistaBL = new TransportistaBL();
                notaIngreso.ciudadDestino.transportistaList = transportistaBL.getTransportistas(pedido.ciudad.idCiudad);

                notaIngreso.documentoDetalle = notaIngreso.pedido.documentoDetalle;

            }
            catch (Exception ex)
            {
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                Log log = new Log(ex.ToString(), TipoLog.Error, usuario);
                LogBL logBL = new LogBL();
                logBL.insertLog(log);
            }

        }

        public void iniciarIngresoDesdeGuiaRemision()
        {
            try
            {   /*IMPORTANTE: Se debe identificar si la guia esta facturada para ver si se genera nota de crédito*/

                GuiaRemision guiaRemisionAExtornar = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA_VER];
                instanciarNotaIngreso(guiaRemisionAExtornar.ciudadOrigen);
                NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO];

                notaIngreso.motivoExtornoGuiaRemision = (NotaIngreso.MotivosExtornoGuiaRemision)Int32.Parse(Request.Params["motivoExtornoGuiaRemision"]);
                notaIngreso.guiaRemisionAExtornar = guiaRemisionAExtornar;

                if (guiaRemisionAExtornar.almacen != null && !guiaRemisionAExtornar.almacen.idAlmacen.Equals(Guid.Empty))
                {
                    notaIngreso.idAlmacen = guiaRemisionAExtornar.almacen.idAlmacen;
                    notaIngreso.direccionLlegada = guiaRemisionAExtornar.almacen.direccion;
                }

                /*Si el motivo del extorno es devolución parcial se activa atención parcial para poder editar el detalle*/
                notaIngreso.documentoDetalle = notaIngreso.guiaRemisionAExtornar.documentoDetalle;

                if (notaIngreso.motivoExtornoGuiaRemision == NotaIngreso.MotivosExtornoGuiaRemision.DevolucionItem)
                {
                    notaIngreso.atencionParcial = true;
                    /*Si ya ha sido extornado se debe recuperar las cantidades extornadas*/
                    if (notaIngreso.guiaRemisionAExtornar.tipoExtorno != GuiaRemision.TiposExtorno.SinExtorno)
                    {
                        MovimientoAlmacenBL movimientoAlmacenBL = new MovimientoAlmacenBL();
                        movimientoAlmacenBL.obtenerCantidadesPorExtornar(notaIngreso.guiaRemisionAExtornar);
                    }
                    else
                    {
                        foreach (DocumentoDetalle documentoDetalle in notaIngreso.documentoDetalle)
                        {
                            documentoDetalle.cantidadPendienteAtencion = documentoDetalle.cantidadSolicitada;
                            documentoDetalle.cantidadPorAtender = documentoDetalle.cantidadSolicitada;
                        }
                    }
                }
                else
                {
                 

                    foreach (DocumentoDetalle documentoDetalle in notaIngreso.documentoDetalle)
                    {
                        documentoDetalle.cantidadPendienteAtencion = documentoDetalle.cantidadSolicitada;
                        documentoDetalle.cantidadPorAtender = documentoDetalle.cantidadSolicitada;
                    }

                    notaIngreso.atencionParcial = false;
                }
               
                notaIngreso.pedido = notaIngreso.guiaRemisionAExtornar.pedido;


                if (notaIngreso.guiaRemisionAExtornar.motivoTraslado == GuiaRemision.motivosTraslado.Venta)
                {
                    notaIngreso.pedido.clasePedido = Pedido.ClasesPedido.Venta;
                    notaIngreso.pedido.tipoPedido = Pedido.tiposPedido.Venta;
                    notaIngreso.motivoTraslado = NotaIngreso.motivosTraslado.DevolucionVenta;
                }
                else if (notaIngreso.guiaRemisionAExtornar.motivoTraslado == GuiaRemision.motivosTraslado.ComodatoEntregado)
                {
                    notaIngreso.pedido.clasePedido = Pedido.ClasesPedido.Venta;
                    notaIngreso.pedido.tipoPedido = Pedido.tiposPedido.ComodatoEntregado;
                    notaIngreso.motivoTraslado = NotaIngreso.motivosTraslado.DevolucionComodatoEntregado;
                }
                else if (notaIngreso.guiaRemisionAExtornar.motivoTraslado == GuiaRemision.motivosTraslado.TransferenciaGratuitaEntregada)
                {
                    notaIngreso.pedido.clasePedido = Pedido.ClasesPedido.Venta;
                    notaIngreso.pedido.tipoPedido = Pedido.tiposPedido.TransferenciaGratuitaEntregada;
                    notaIngreso.motivoTraslado = NotaIngreso.motivosTraslado.DevolucionTransferenciaGratuitaEntregada;
                }
                else if (notaIngreso.guiaRemisionAExtornar.motivoTraslado == GuiaRemision.motivosTraslado.PrestamoEntregado)
                {
                    notaIngreso.pedido.clasePedido = Pedido.ClasesPedido.Almacen;
                    notaIngreso.pedido.tipoPedidoAlmacen = Pedido.tiposPedidoAlmacen.PrestamoEntregado;
                    notaIngreso.motivoTraslado = NotaIngreso.motivosTraslado.DevolucionPrestamoEntregado;
                }

                

                notaIngreso.observaciones = String.Empty;
               
                       
                CiudadBL ciudadBL = new CiudadBL();
                Ciudad ciudadDestino = ciudadBL.getCiudad(notaIngreso.ciudadDestino.idCiudad);
                notaIngreso.ciudadDestino.nombre = ciudadDestino.nombre;
                notaIngreso.ciudadDestino.direccionPuntoLlegada = ciudadDestino.direccionPuntoPartida;

                notaIngreso.transportista = new Transportista();
                TransportistaBL transportistaBL = new TransportistaBL();
                notaIngreso.ciudadDestino.transportistaList = transportistaBL.getTransportistas(notaIngreso.guiaRemisionAExtornar.ciudadOrigen.idCiudad);

                this.Session[Constantes.VAR_SESSION_NOTA_INGRESO] = notaIngreso;
            }
            catch (Exception ex)
            {
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                Log log = new Log(ex.ToString(), TipoLog.Error, usuario);
                LogBL logBL = new LogBL();
                logBL.insertLog(log);
            }
        }

        public void iniciarIngresoDesdeGuiaRemisionTrasladoInterno()
        {
            try
            {   /*IMPORTANTE: Se debe identificar si la guia esta facturada para ver si se genera nota de crédito*/

                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                GuiaRemision guiaRemisionAIngresar = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA_VER];


                instanciarNotaIngreso(usuario.sedeMP);

                NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO];

                //notaIngreso.motivoExtornoGuiaRemision = (NotaIngreso.MotivosExtornoGuiaRemision)Int32.Parse(Request.Params["motivoExtornoGuiaRemision"]);
                notaIngreso.guiaRemisionAIngresar = guiaRemisionAIngresar;

                /*Si el motivo del extorno es devolución parcial se activa atención parcial para poder editar el detalle*/
                notaIngreso.documentoDetalle = notaIngreso.guiaRemisionAIngresar.documentoDetalle;

                

                foreach (DocumentoDetalle documentoDetalle in notaIngreso.documentoDetalle)
                {
                    documentoDetalle.cantidadPendienteAtencion = documentoDetalle.cantidadSolicitada;
                    documentoDetalle.cantidadPorAtender = documentoDetalle.cantidadSolicitada;
                }

                notaIngreso.atencionParcial = false;
                

                notaIngreso.pedido = notaIngreso.guiaRemisionAIngresar.pedido;

                PedidoBL pedidoBL = new PedidoBL();

                notaIngreso.pedido = pedidoBL.GetPedido(notaIngreso.pedido, usuario);

                if (notaIngreso.pedido.almacenDestino != null && !notaIngreso.pedido.almacenDestino.idAlmacen.Equals(Guid.Empty))
                {
                    notaIngreso.idAlmacen = notaIngreso.pedido.almacenDestino.idAlmacen;
                    notaIngreso.direccionLlegada = notaIngreso.pedido.almacenDestino.direccion;
                }

                notaIngreso.pedido.clasePedido = Pedido.ClasesPedido.Almacen;
                notaIngreso.pedido.tipoPedidoAlmacen = Pedido.tiposPedidoAlmacen.TrasladoInterno;
                notaIngreso.motivoTraslado = NotaIngreso.motivosTraslado.TrasladoInterno;

                //Cambiar a serie traslado interno si el traslado es entre almacenes de la misma sede
                if (notaIngreso.pedido.almacenOrigen.idCiudad.Equals(notaIngreso.pedido.almacenDestino.idCiudad))
                {
                    this.Session["seAtiendeTrasladoInterno"] = true;
                }

                notaIngreso.observaciones = String.Empty;


                CiudadBL ciudadBL = new CiudadBL();
                /*Por Revisar*/
                Ciudad ciudadDestino = ciudadBL.getCiudad(notaIngreso.ciudadDestino.idCiudad);
                notaIngreso.ciudadDestino.nombre = ciudadDestino.nombre;
                notaIngreso.ciudadDestino.direccionPuntoLlegada = ciudadDestino.direccionPuntoPartida;

                notaIngreso.transportista = new Transportista();
                TransportistaBL transportistaBL = new TransportistaBL();
                notaIngreso.ciudadDestino.transportistaList = transportistaBL.getTransportistas(notaIngreso.guiaRemisionAIngresar.ciudadOrigen.idCiudad);

                this.Session[Constantes.VAR_SESSION_NOTA_INGRESO] = notaIngreso;
            }
            catch (Exception ex)
            {
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                Log log = new Log(ex.ToString(), TipoLog.Error, usuario);
                LogBL logBL = new LogBL();
                logBL.insertLog(log);
            }
        }


        public ActionResult Ingresar()
        {
            //this.Session[Constantes.VAR_SESSION_NOTA_INGRESO] = null;
            //return RedirectToAction("Index", "Pedido");

            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.MantenimientoNotaIngreso;

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (usuario == null || !usuario.creaGuias)
            {
                return RedirectToAction("Login", "Account");
            }



            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.MantenimientoNotaIngreso;
            try
            {
                if (this.Session[Constantes.VAR_SESSION_NOTA_INGRESO] == null)
                {
                    return View("IngresarEmpty");
                    //instanciarNotaIngreso();
                }
                NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO];

                ViewBag.fechaTrasladotmp = notaIngreso.fechaTraslado.ToString(Constantes.formatoFecha);
                ViewBag.fechaEmisiontmp = notaIngreso.fechaEmision.ToString(Constantes.formatoFecha);

                if (notaIngreso.almacenes == null || notaIngreso.almacenes.Count == 0)
                {
                    AlmacenBL almacenBl = new AlmacenBL();
                    List<Almacen> almacenes = almacenBl.getAlmacenesSedes(notaIngreso.ciudadDestino.idCiudad);
                    notaIngreso.almacenes = almacenes;
                }

                if (notaIngreso.idAlmacen == null || notaIngreso.idAlmacen == Guid.Empty)
                {
                    foreach (Almacen item in notaIngreso.almacenes)
                    {
                        if (item.esPrincipal)
                        {
                            notaIngreso.idAlmacen = item.idAlmacen;
                            notaIngreso.direccionLlegada = item.direccion;
                        }
                    }
                }


                
                ViewBag.notaIngreso = notaIngreso;

                this.Session[Constantes.VAR_SESSION_NOTA_INGRESO] = notaIngreso;
                //   ViewBag.serieDocumentoElectronicoList = ciudad.serieDocumentoElectronicoList;

            }
            catch (Exception ex)
            {
                usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                Log log = new Log(ex.ToString(), TipoLog.Error, usuario);
                LogBL logBL = new LogBL();
                logBL.insertLog(log);
            }

            bool seAtiendeTrasladoInterno = this.Session["seAtiendeTrasladoInterno"] != null ? (bool)this.Session["seAtiendeTrasladoInterno"] : false;

            if (seAtiendeTrasladoInterno)
            {
                this.CambiarASerieTrasladoInterno();
                this.Session["seAtiendeTrasladoInterno"] = false;
            }

            ViewBag.pagina = (int)Constantes.paginas.MantenimientoNotaIngreso;
            return View();
        }

        public String CambiarASerieTrasladoInterno()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            String response = "{\"success\": 0}";

            SerieDocumentoBL serieBL = new SerieDocumentoBL();
            NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO];

            SerieDocumentoElectronico serie = serieBL.getSerieDocumento("TRASLADOINTERNO", notaIngreso.ciudadDestino.idCiudad);

            if (serie.sedeMP != null)
            {
                notaIngreso.serieDocumento = serie.serie;
                notaIngreso.numeroDocumento = serie.siguienteNumeroNotaIngreso;
                notaIngreso.serieDocumentoElectronico = serie;
                this.Session[Constantes.VAR_SESSION_NOTA_INGRESO] = notaIngreso;

                response = "{\"success\": 1,  \"serie\":\"" + serie.serie + "\", \"numero\":\"" + serie.siguienteNumeroNotaIngreso.ToString() + "\", \"serieNumeroString\":\"" + notaIngreso.serieNumeroNotaIngreso + "\"}";
            }

            return response;
        }


        public String Create()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.MantenimientoNotaIngreso;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            NotaIngreso notaIngreso = this.NotaIngresoSession;
            notaIngreso.usuario = usuario;

            String error = String.Empty;
            MovimientoAlmacenBL movimientoAlmacenBL = new MovimientoAlmacenBL();
            try
            {
                movimientoAlmacenBL.InsertMovimientoAlmacenEntrada(notaIngreso);
            }
            catch (DuplicateNumberDocumentException ex)
            {
                error = ex.Message;
            }

            long numeroNotaIngreso = notaIngreso.numero;
            Guid idNotaIngreso = notaIngreso.idMovimientoAlmacen;
            String serieNumeroNotaIngreso = notaIngreso.serieNumeroNotaIngreso;

            int estado = (int)notaIngreso.seguimientoMovimientoAlmacenEntrada.estado;

            String jsonNotaIngresoValidacion = JsonConvert.SerializeObject(notaIngreso.notaIngresoValidacion);

            string generarNotaCredito = "false";
            if (notaIngreso.notaIngresoValidacion.tipoErrorValidacion == NotaIngresoValidacion.TiposErrorValidacion.NoExisteError)
            {
                if (notaIngreso.guiaRemisionAExtornar != null)
                {
                    //Si la guia a extornar se encuentra facturada se debe generar nota de crédito
                    if (notaIngreso.guiaRemisionAExtornar.estaFacturado)
                    {
                        generarNotaCredito = "true";
                    }
                    else
                    {
                        this.NotaIngresoSession = null;
                    }
                }
                else
                {
                    this.NotaIngresoSession = null;
                }
            }


            String resultado = "{ \"serieNumeroNotaIngreso\":\"" + serieNumeroNotaIngreso + "\", \"idNotaIngreso\":\"" + idNotaIngreso + "\", \"error\":\"" + error + "\",     \"notaIngresoValidacion\": " + jsonNotaIngresoValidacion + ",     \"generarNotaCredito\": " + generarNotaCredito + "  }";
            return resultado;
        }

        public String CreateTransportistaTemporal()
        {
            NotaIngreso notaIngreso = this.NotaIngresoSession;
            Transportista transportista = new Transportista();
            transportista.descripcion = Request["descripcion"];
            transportista.direccion = Request["direccion"];
            transportista.ruc = Request["ruc"];
            transportista.telefono = Request["telefono"];
            transportista.idTransportista = Guid.Empty;
            NotaIngresoSession.ciudadDestino.transportistaList.Add(transportista);
            NotaIngresoSession.transportista = transportista;
            this.NotaIngresoSession = notaIngreso;
            return JsonConvert.SerializeObject(transportista);
        }

        [HttpPost]
        public String ChangeDetalle(List<DocumentoDetalleJson> documentoDetalleList)
        {
            NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO];

            foreach (DocumentoDetalleJson documentoDetalleJson in documentoDetalleList)
            {
                DocumentoDetalle documentoDetalle = notaIngreso.documentoDetalle.Where(d => d.producto.idProducto == Guid.Parse(documentoDetalleJson.idProducto)).FirstOrDefault();
                documentoDetalle.cantidadPorAtender = documentoDetalleJson.cantidad;
            }
            this.Session[Constantes.VAR_SESSION_NOTA_INGRESO] = notaIngreso;
            return "{\"cantidad\":\"" + notaIngreso.documentoDetalle.Count + "\"}";
        }
        #endregion

        public String ChangeIdAlmacen()
        {
            NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO];
            Guid idAlmacen = Guid.Empty;
            if (this.Request.Params["valor"] != null && !this.Request.Params["valor"].Equals(""))
            {
                idAlmacen = Guid.Parse(this.Request.Params["valor"]);
            }

            Almacen selected = new Almacen();
            foreach (Almacen item in notaIngreso.almacenes)
            {
                notaIngreso.idAlmacen = item.idAlmacen;
                notaIngreso.direccionLlegada = item.direccion;
                selected = item;
            }

            this.Session[Constantes.VAR_SESSION_NOTA_INGRESO] = notaIngreso;
            return JsonConvert.SerializeObject(selected);
        }

        #region Acciones en Guia

        public String Show()
        {
            MovimientoAlmacenBL movimientoAlmacenBL = new MovimientoAlmacenBL();
            NotaIngreso notaIngreso = new NotaIngreso();
            notaIngreso.idMovimientoAlmacen = Guid.Parse(Request["idMovimientoAlmacen"].ToString());
            notaIngreso = movimientoAlmacenBL.GetNotaIngreso(notaIngreso);
            this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_VER] = notaIngreso;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            string jsonUsuario = JsonConvert.SerializeObject(usuario);
            //string jsonNotaIngreso = JsonConvert.SerializeObject(notaIngreso);
            string jsonNotaIngreso = JsonConvert.SerializeObject(ParserDTOsShow.NotaIngresoToNotaIngreso(notaIngreso));
            String json = "{\"usuario\":" + jsonUsuario + ", \"notaIngreso\":" + jsonNotaIngreso + "}";
            return json;
        }

        public ActionResult Print()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.ImprimirNotaIngreso;

            NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_VER];

            ViewBag.notaIngreso = notaIngreso;
            ViewBag.pagina = this.Session[Constantes.VAR_SESSION_PAGINA];

            return View();

        }

        public String Anular()
        {
            NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_VER];
            notaIngreso.comentarioAnulado = Request["comentarioAnulado"];
            MovimientoAlmacenBL movimientoAlmacenBL = new MovimientoAlmacenBL();
            movimientoAlmacenBL.AnularMovimientoAlmacen(notaIngreso);
            return JsonConvert.SerializeObject(notaIngreso);
        }



        #endregion


        #region Changes

        public String UpdateSerieDocumento()
        {
            int serieDocumento = int.Parse(this.Request.Params["serieDocumento"]);
            NotaIngreso notaIngreso = this.NotaIngresoSession;
            SerieDocumentoElectronico serieDocumentoElectronico = notaIngreso.ciudadDestino.serieDocumentoElectronicoList.Where(s => int.Parse(s.serie) == serieDocumento).FirstOrDefault();
            notaIngreso.serieDocumento = serieDocumentoElectronico.serie;
            notaIngreso.numeroDocumento = serieDocumentoElectronico.siguienteNumeroNotaIngreso;
            notaIngreso.serieDocumentoElectronico = serieDocumentoElectronico;
            return notaIngreso.numeroDocumentoString;
        }

        public void ChangeEstaAnulado()
        {
            this.NotaIngresoSession.estaAnulado = Int32.Parse(this.Request.Params["estaAnulado"]) == 1;
        }

        public void ChangeEstaFacturado()
        {
            this.NotaIngresoSession.estaFacturado = Int32.Parse(this.Request.Params["estaFacturado"]) == 1;
        }

        public void ChangeFechaEmisionDesde()
        {
            String[] fec = this.Request.Params["fecha"].Split('/');
            this.NotaIngresoSession.fechaEmisionDesde = new DateTime(Int32.Parse(fec[2]), Int32.Parse(fec[1]), Int32.Parse(fec[0]));
        }

        public void ChangeFechaEmisionHasta()
        {
            String[] fec = this.Request.Params["fecha"].Split('/');
            this.NotaIngresoSession.fechaEmisionHasta = new DateTime(Int32.Parse(fec[2]), Int32.Parse(fec[1]), Int32.Parse(fec[0]));
        }

        public void ChangeFechaTrasladoDesde()
        {
            String[] movDesde = this.Request.Params["fechaTrasladoDesde"].Split('/');
            this.NotaIngresoSession.fechaTrasladoDesde = new DateTime(Int32.Parse(movDesde[2]), Int32.Parse(movDesde[1]), Int32.Parse(movDesde[0]));
        }

        public void ChangeFechaTraslado()
        {
            String[] movHasta = this.Request.Params["fechaTraslado"].Split('/');
            this.NotaIngresoSession.fechaTraslado = new DateTime(Int32.Parse(movHasta[2]), Int32.Parse(movHasta[1]), Int32.Parse(movHasta[0]), 23, 59, 59);
        }

        public void ChangeFechaEmision()
        {
            String[] movDesde = this.Request.Params["fechaEmision"].Split('/');
            this.NotaIngresoSession.fechaEmision = new DateTime(Int32.Parse(movDesde[2]), Int32.Parse(movDesde[1]), Int32.Parse(movDesde[0]));
        }

        public void ChangeFechaTrasladoHasta()
        {
            String[] movHasta = this.Request.Params["fechaTrasladoHasta"].Split('/');
            this.NotaIngresoSession.fechaTrasladoHasta = new DateTime(Int32.Parse(movHasta[2]), Int32.Parse(movHasta[1]), Int32.Parse(movHasta[0]), 23, 59, 59);
        }


        public String ChangeIdCiudad()
        {
            NotaIngreso notaIngreso = this.NotaIngresoSession;
            Guid idCiudad = Guid.Empty;
            if (this.Request.Params["idCiudad"] != null && !this.Request.Params["idCiudad"].Equals(""))
            {
                idCiudad = Guid.Parse(this.Request.Params["idCiudad"]);
            }
            CiudadBL ciudadBL = new CiudadBL();
            Ciudad ciudadDestino = ciudadBL.getCiudad(idCiudad);
            notaIngreso.transportista = new Transportista();
            TransportistaBL transportistaBL = new TransportistaBL();
            ciudadDestino.transportistaList = transportistaBL.getTransportistas(idCiudad);
            notaIngreso.ciudadDestino = ciudadDestino;
            this.NotaIngresoSession = notaIngreso;
            return JsonConvert.SerializeObject(notaIngreso.ciudadDestino);
        }


        public void ChangeBuscarSedesGrupoCliente()
        {
            NotaIngreso notaIngreso = this.NotaIngresoSession;
            if (this.Request.Params["buscarSedesGrupoCliente"] != null && Int32.Parse(this.Request.Params["buscarSedesGrupoCliente"]) == 1)
            {
                notaIngreso.pedido.buscarSedesGrupoCliente = true;
            }
            else
            {
                notaIngreso.pedido.buscarSedesGrupoCliente = false;
            }
            this.NotaIngresoSession = notaIngreso;
        }

        public void ChangeInputString()
        {
            NotaIngreso notaIngreso = this.NotaIngresoSession;
            PropertyInfo propertyInfo = notaIngreso.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(notaIngreso, this.Request.Params["valor"]);
            this.NotaIngresoSession = notaIngreso;
        }

        public void ChangeInputInt()
        {
            NotaIngreso notaIngreso = this.NotaIngresoSession;
            PropertyInfo propertyInfo = notaIngreso.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(notaIngreso, Int32.Parse( this.Request.Params["valor"]));
            this.NotaIngresoSession = notaIngreso;
        }

        public void ChangeTipoDocumentoVentaReferencia()
        {
            NotaIngreso notaIngreso = this.NotaIngresoSession;
            notaIngreso.tipoDocumentoVentaReferencia = (NotaIngreso.TiposDocumentoVentaReferencia)int.Parse(this.Request.Params["tipoDocumentoVentaReferencia"]);
            this.NotaIngresoSession = notaIngreso;
        }


        public void ChangeEstadoFiltro()
        {
            NotaIngreso notaIngreso = this.NotaIngresoSession;
            notaIngreso.estadoFiltro = (NotaIngreso.EstadoFiltro)int.Parse(this.Request.Params["estado"]);
            this.NotaIngresoSession = notaIngreso;
        }

        

        public void ChangeInputStringTransportista()
        {
            NotaIngreso notaIngreso = this.NotaIngresoSession;
            PropertyInfo propertyInfo = notaIngreso.transportista.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(notaIngreso.transportista, this.Request.Params["valor"]);
            this.NotaIngresoSession = notaIngreso;
        }

        public void ChangeAtencionParcial()
        {
            NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO];
            notaIngreso.atencionParcial = Int32.Parse( this.Request.Params["atencionParcial"])==1;

            if (!notaIngreso.atencionParcial)
            {
                foreach (DocumentoDetalle documentoDetalle in notaIngreso.documentoDetalle)
                {
                    documentoDetalle.cantidadPorAtender = documentoDetalle.cantidadPendienteAtencion;
                }

            }
            this.Session[Constantes.VAR_SESSION_NOTA_INGRESO] = notaIngreso;
        }

        /*
        public void ChangeUltimaAtencionParcial()
        {
            NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO];
            notaIngreso.ultimaAtencionParcial = Int32.Parse(this.Request.Params["ultimaAtencionParcial"]) == 1;


            if (!notaIngreso.atencionParcial && notaIngreso.ultimaAtencionParcial)
            {
                foreach (DocumentoDetalle documentoDetalle in notaIngreso.documentoDetalle)
                {
                    documentoDetalle.cantidadPorAtender = documentoDetalle.cantidadPendienteAtencion;
                }

            }

            this.Session[Constantes.VAR_SESSION_NOTA_INGRESO] = notaIngreso;
        }

        */


        public String ChangeTransportista()
        {
            NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO];
         
            if (this.Request.Params["idTransportista"] == null || this.Request.Params["idTransportista"].Equals(String.Empty))
            {
                notaIngreso.transportista = new Transportista();
            }
            else
            {
                Guid idTransportista = Guid.Parse(this.Request.Params["idTransportista"]);
                notaIngreso.transportista = notaIngreso.ciudadDestino.transportistaList.Where(t => t.idTransportista == idTransportista).FirstOrDefault();
            }
           
            this.Session[Constantes.VAR_SESSION_NOTA_INGRESO] = notaIngreso;
            String jsonTransportista = JsonConvert.SerializeObject(notaIngreso.transportista);
            return jsonTransportista;
        }

        public void ChangeMotivoTraslado()
        {
            NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_BUSQUEDA];
            notaIngreso.motivoTrasladoBusqueda = (NotaIngreso.motivosTrasladoBusqueda) (Char)Int32.Parse(this.Request.Params["motivoTraslado"]);
            this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_BUSQUEDA] = notaIngreso;
        }


        public ActionResult CancelarCreacionNotaIngreso()
        {
            this.Session[Constantes.VAR_SESSION_NOTA_INGRESO] = null;
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            //   usuarioBL.updateCotizacionSerializada(usuario, null);
            return RedirectToAction("Index");
        }


        public String Descargar()
        {
            String nombreArchivo = Request["nombreArchivo"].ToString();
            NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO];
            if(notaIngreso == null)
                notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_VER];

            Pedido pedido = notaIngreso.pedido; ;

            //      pedido.pedidoAdjuntoList = new List<PedidoAdjunto>();
            PedidoAdjunto pedidoAdjunto = pedido.pedidoAdjuntoList.Where(p => p.nombre.Equals(nombreArchivo)).FirstOrDefault();

            if (pedidoAdjunto != null)
            {
                return JsonConvert.SerializeObject(pedidoAdjunto);
            }
            else
            {
                return null;
            }

        }



        #endregion



        public String GetMovimientosAlmacenExtornantes()
        {
            MovimientoAlmacen movimientoAlmacen = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_VER];
            MovimientoAlmacenBL movimientoAlmacenBL = new MovimientoAlmacenBL();
            List<MovimientoAlmacen> movimientoAlmacenExtornanteList = movimientoAlmacenBL.GetMovimientosAlmacenExtornantes(movimientoAlmacen);
            String resultado = JsonConvert.SerializeObject(movimientoAlmacenExtornanteList);
            return resultado;
        }
    }
}
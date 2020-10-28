using BusinessLayer;
using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using cotizadorPDF;
using Cotizador.ExcelExport;

namespace Cotizador.Controllers
{
    public class PedidoCompraController : Controller
    {
        private Pedido PedidoSession {
            get {

                Pedido pedido = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaPedidos: pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_BUSQUEDA]; break;
                    case Constantes.paginas.MantenimientoPedido: pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA]; break;
                }
                return pedido;
            }
            set {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaPedidos: this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_BUSQUEDA] = value; break;
                    case Constantes.paginas.MantenimientoPedido: this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA] = value; break;
                }
            }
        }

        // GET: Pedido

        private void instanciarPedidoBusqueda()
        {
            Pedido pedidoTmp = new Pedido(Pedido.ClasesPedido.Compra);
            DateTime fechaDesde = DateTime.Now.AddDays(-Constantes.DIAS_DESDE_BUSQUEDA);
            DateTime fechaHasta = DateTime.Now.AddDays(1);
            pedidoTmp.cotizacion = new Cotizacion();
            pedidoTmp.solicitante = new Solicitante();
            pedidoTmp.direccionEntrega = new DireccionEntrega();

           /* pedidoTmp.fechaSolicitudDesde = new DateTime(fechaDesde.Year, fechaDesde.Month, fechaDesde.Day, 0, 0, 0);
            pedidoTmp.fechaSolicitudHasta = fechaHasta;*/

            pedidoTmp.fechaCreacionDesde = new DateTime(fechaDesde.Year, fechaDesde.Month, fechaDesde.Day, 0, 0, 0);
            pedidoTmp.fechaCreacionHasta = fechaHasta;

            pedidoTmp.fechaEntregaDesde = null;
            pedidoTmp.fechaEntregaHasta = null;

            pedidoTmp.fechaProgramacionDesde = null;// new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            pedidoTmp.fechaProgramacionHasta = null;// new DateTime(fechaHasta.Year, fechaHasta.Month, fechaHasta.Day, 23, 59, 59);

            pedidoTmp.ciudad = new Ciudad();
            pedidoTmp.cliente = new Cliente();
            pedidoTmp.seguimientoPedido = new SeguimientoPedido();
            pedidoTmp.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.Todos;
            pedidoTmp.seguimientoCrediticioPedido = new SeguimientoCrediticioPedido();
            pedidoTmp.seguimientoCrediticioPedido.estado = SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido.Todos;

            pedidoTmp.pedidoDetalleList = new List<PedidoDetalle>();
            pedidoTmp.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            pedidoTmp.usuarioBusqueda = new Usuario { idUsuario = Guid.Empty };

            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_BUSQUEDA] = pedidoTmp;
            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_LISTA] = new List<Pedido>();
        }

      
        public ActionResult Index()
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

            if (this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_BUSQUEDA] == null)
            {
                instanciarPedidoBusqueda();
            }

            Pedido pedidoSearch = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_BUSQUEDA];

            //Si existe cotizacion se debe verificar si no existe cliente
            if (this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA] != null)
            {
                Pedido pedidoEdicion = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA];
                if (pedidoEdicion.ciudad == null || pedidoEdicion.ciudad.idCiudad == null
                    || pedidoEdicion.ciudad.idCiudad == Guid.Empty)
                {
                    this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA] = null;
                }

            }


            ViewBag.fechaCreacionDesde = pedidoSearch.fechaCreacionDesde.ToString(Constantes.formatoFecha);
            ViewBag.fechaCreacionHasta = pedidoSearch.fechaCreacionHasta.ToString(Constantes.formatoFecha);
            /*
            ViewBag.fechaSolicitudDesde = pedidoSearch.fechaSolicitudDesde.ToString(Constantes.formatoFecha);
            ViewBag.fechaSolicitudHasta = pedidoSearch.fechaSolicitudHasta.ToString(Constantes.formatoFecha);*/

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

            if (this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_LISTA] == null)
            {
                this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_LISTA] = new List<Pedido>();
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
            ViewBag.pedidoList = this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_LISTA];
            ViewBag.existeCliente = existeCliente;
            ViewBag.pagina = (int)Constantes.paginas.BusquedaPedidosCompra;
            return View();
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

                if (this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA] == null)
                {

                    instanciarPedido();
                }
                Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA];


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

            }
            catch (Exception ex)
            {
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                Log log = new Log(ex.ToString(), TipoLog.Error, usuario);
                LogBL logBL = new LogBL();
                logBL.insertLog(log);
            }

            ViewBag.pagina = (int)Constantes.paginas.MantenimientoPedido;
            return View();
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
                ViewBag.pagina = (int) Constantes.paginas.MantenimientoPedidoCompra;

                //Si no se está trabajando con una cotización se crea una y se agrega a la sesion

                if (this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA] == null)
                {

                    instanciarPedido();
                }
                Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA];


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

                ViewBag.fechaEntregaDesde = pedido.fechaEntregaDesde ==null?"": pedido.fechaEntregaDesde.Value.ToString(Constantes.formatoFecha);
                ViewBag.fechaEntregaHasta = pedido.fechaEntregaHasta == null?"": pedido.fechaEntregaHasta.Value.ToString(Constantes.formatoFecha);

       

                ViewBag.pedido = pedido;
                ViewBag.VARIACION_PRECIO_ITEM_PEDIDO = Constantes.VARIACION_PRECIO_ITEM_PEDIDO;

                ViewBag.fechaPrecios = DateTime.Now.AddDays(-Constantes.DIAS_MAX_BUSQUEDA_PRECIOS).ToString(Constantes.formatoFecha);
             //   ViewBag.fechaPrecios = pedido.fechaPrecios.ToString(Constantes.formatoFecha);
                
            }
            catch (Exception ex)
            {
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                Log log = new Log(ex.ToString(), TipoLog.Error, usuario);
                LogBL logBL = new LogBL();
                logBL.insertLog(log);
            }

            return View();
        }

        public void iniciarEdicionPedidoDesdeCotizacion()
        {
            if (this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA] == null)
            {

                instanciarPedido();
            }
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA];

            Cotizacion cotizacion = (Cotizacion)this.Session[Constantes.VAR_SESSION_COTIZACION_VER];
            pedido.cotizacion = new Cotizacion();
            pedido.cotizacion.idCotizacion = cotizacion.idCotizacion;
            pedido.cotizacion.codigo = cotizacion.codigo;
            pedido.ciudad = cotizacion.ciudad;
            pedido.cliente = cotizacion.cliente;          
            
          
            

            DireccionEntregaBL direccionEntregaBL = new DireccionEntregaBL();
            pedido.cliente.direccionEntregaList = direccionEntregaBL.getDireccionesEntrega(cotizacion.cliente.idCliente);
            pedido.direccionEntrega = new DireccionEntrega();

            pedido.observaciones = String.Empty;

            pedido.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            pedido.seguimientoPedido = new SeguimientoPedido();

            pedido.pedidoDetalleList = new List<PedidoDetalle>();
            foreach (DocumentoDetalle documentoDetalle in  cotizacion.documentoDetalle)
            {
                PedidoDetalle pedidoDetalle = new PedidoDetalle(usuario.visualizaCostos, usuario.visualizaMargen);
                pedidoDetalle.cantidad = documentoDetalle.cantidad;
                if (documentoDetalle.cantidad == 0)
                    pedidoDetalle.cantidad = 1;

               // pedidoDetalle.costoAnterior = documentoDetalle.costoAnterior;
                pedidoDetalle.esPrecioAlternativo = documentoDetalle.esPrecioAlternativo;
                pedidoDetalle.flete = documentoDetalle.flete;
                pedidoDetalle.observacion = documentoDetalle.observacion;
                pedidoDetalle.porcentajeDescuento = documentoDetalle.porcentajeDescuento;
                if(documentoDetalle.esPrecioAlternativo)
                    pedidoDetalle.precioNeto = documentoDetalle.precioNeto * documentoDetalle.ProductoPresentacion.Equivalencia;
                else
                    pedidoDetalle.precioNeto = documentoDetalle.precioNeto;
               // pedidoDetalle.precioNetoAnterior = documentoDetalle.precioNetoAnterior;
                pedidoDetalle.producto = documentoDetalle.producto;
                pedidoDetalle.unidad = documentoDetalle.unidad;
                pedido.pedidoDetalleList.Add(pedidoDetalle);
            }
            pedido.fechaPrecios = pedido.fechaSolicitud.AddDays(Constantes.DIAS_MAX_BUSQUEDA_PRECIOS * -1);
            PedidoBL pedidoBL = new PedidoBL();
            pedidoBL.calcularMontosTotales(pedido);
            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA] = pedido;


        }

        private void instanciarPedido()
        {
            Pedido pedido = new Pedido(Pedido.ClasesPedido.Compra);
            pedido.idPedido = Guid.Empty;
            pedido.numeroPedido = 0;
            pedido.numeroGrupoPedido = null;
            pedido.cotizacion = new Cotizacion();
            pedido.ubigeoEntrega = new Ubigeo();
            pedido.ubigeoEntrega.Id = "000000";
            pedido.ciudad = new Ciudad();
            pedido.cliente = new Cliente();

            pedido.tipoPedido = Pedido.tiposPedido.Venta;
            pedido.ciudadASolicitar = new Ciudad();

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

            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA] = pedido;
        }

        public void iniciarEdicionPedido()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            Pedido pedidoVer = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_VER];
            PedidoBL pedidoBL = new PedidoBL();
            Pedido pedido = new Pedido(Pedido.ClasesPedido.Compra);
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

            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA] = pedido;
        }

        public void obtenerProductosAPartirdePreciosRegistrados()
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

            pedido = pedidoBL.obtenerProductosAPartirdePreciosRegistrados(pedido, familia, proveedor, usuario);
            pedidoBL.calcularMontosTotales(pedido);
            this.PedidoSession = pedido;
        }


        [HttpGet]
        public ActionResult ExportLastViewExcel()
        {
            Pedido obj = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_VER];

            PedidoOCExcel excel = new PedidoOCExcel();
            return excel.generateExcel(obj);
        }

        [HttpGet]
        public void ExportLastViewCSVKC()
        {
            Pedido obj = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_VER];


            Response.Clear();
            Response.AddHeader("Content-Disposition", "attachment; filename=OC_" + obj.numeroPedido + ".csv");
            Response.ContentType = "text/csv";

            int count = 0;
            foreach (PedidoDetalle pd in obj.pedidoDetalleList)
            {
                if (count > 0)
                {
                    Response.Write("\n");
                }
                Response.Write(String.Format("{0},{1},{2}", pd.producto.skuProveedor, pd.cantidad, "cj"));
                count++;
            }

            

            Response.End();
            
        }

        [HttpPost]
        public String GenerarPDF()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            Int64 codigo = Int64.Parse(this.Request.Params["codigo"].ToString());

            PedidoBL bl = new PedidoBL();
            Pedido obj = new Pedido();
            obj.numeroPedido = codigo;
            obj = bl.GetPedido(obj, usuario);
            GeneradorOrdenCompraPDF gen = new GeneradorOrdenCompraPDF();
            String nombreArchivo = gen.generarPDFExtended(obj);
            return nombreArchivo;
        }


        public ActionResult CancelarCreacionPedido()
        {
            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA] = null;
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session["usuario"];
         //   usuarioBL.updateCotizacionSerializada(usuario, null);
            return RedirectToAction("Index", "PedidoCompra");
        }



        #region CONTROLES CHOOSEN

        public String SearchClientes()
        {
            String data = this.Request.Params["data[q]"];
            /*ProveedorBL proveedorBL = new ProveedorBL();
            Pedido pedido = this.PedidoSession;
           return proveedorBL.getProveedoresBusqueda(data, pedido.ciudad.idCiudad);*/

            ClienteBL clienteBL = new ClienteBL();
            Pedido pedido = this.PedidoSession;
            return clienteBL.getCLientesBusqueda(data, pedido.ciudad.idCiudad);
        }



        public String GetCliente()
        {


            Pedido pedido = this.PedidoSession; 
            Guid idCliente = Guid.Parse(Request["idCliente"].ToString());
            ClienteBL clienteBl = new ClienteBL();
            pedido.cliente = clienteBl.getCliente(idCliente);

            //Se obtiene la lista de direccioines de entrega registradas para el cliente
            DireccionEntregaBL direccionEntregaBL = new DireccionEntregaBL();
            pedido.cliente.direccionEntregaList = direccionEntregaBL.getDireccionesEntrega(idCliente);

            SolicitanteBL solicitanteBL = new SolicitanteBL();
            pedido.cliente.solicitanteList = solicitanteBL.getSolicitantes(idCliente);

            pedido.direccionEntrega = new DireccionEntrega();

            //Se limpia el ubigeo de entrega
            pedido.ubigeoEntrega = new Ubigeo();
            pedido.ubigeoEntrega.Id = Constantes.UBIGEO_VACIO;

            String resultado = JsonConvert.SerializeObject(pedido.cliente);

            this.PedidoSession = pedido;
            return resultado;
        }

        #endregion



        #region AGREGAR PRODUCTO


        public String GetProducto()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA];
            //Se recupera el producto y se guarda en Session
            Guid idProducto = Guid.Parse(Request["idProducto"].ToString());
            this.Session["idProducto"] = idProducto.ToString();

            //Para recuperar el producto se envia si la sede seleccionada es provincia o no
            ProductoBL bl = new ProductoBL();
            Producto producto = bl.getProducto(idProducto, pedido.ciudad.esProvincia, pedido.incluidoIGV, pedido.cliente.idCliente, true);

            Decimal fleteDetalle = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, producto.costoLista * (0) / 100));
            Decimal precioUnitario = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, fleteDetalle + producto.precioLista));

            //Se calcula el porcentaje de descuento
            Decimal porcentajeDescuento = 0;
            if (producto.precioClienteProducto.idPrecioClienteProducto != Guid.Empty)
            {
                //Solo en caso de que el precioNetoEquivalente sea distinto a 0 se calcula el porcentaje de descuento
                //si no se obtiene precioNetoEquivalente quiere decir que no hay precioRegistrado
                porcentajeDescuento = 100 - (producto.precioClienteProducto.precioNeto * 100 / producto.precioLista);
            }

            String jsonPrecioLista = JsonConvert.SerializeObject(producto.precioListaList);
            String jsonProductoPresentacion = JsonConvert.SerializeObject(producto.ProductoPresentacionList);
            Decimal costoOriginal = producto.costoOriginal / producto.equivalenciaProveedor;

            String resultado = "{" +
                "\"id\":\"" + producto.idProducto + "\"," +
                "\"nombre\":\"" + producto.descripcion + "\"," +
                "\"image\":\"data:image/png;base64, " + Convert.ToBase64String(producto.image) + "\"," +
                "\"unidad\":\"" + producto.unidad + "\"," +
                "\"unidad_alternativa\":\"" + producto.unidad_alternativa + "\"," +
                "\"proveedor\":\"" + producto.proveedor + "\"," +
                "\"familia\":\"" + producto.familia + "\"," +
                "\"monedaProveedor\":\"" + producto.monedaProveedor + "\"," +
                "\"precioUnitarioSinIGV\":\"" + producto.precioSinIgv + "\"," +
             //   "\"precioUnitarioAlternativoSinIGV\":\"" + producto.precioAlternativoSinIgv + "\"," +
                "\"precioLista\":\"" + producto.precioLista + "\"," +
                "\"costoSinIGV\":\"" + producto.costoSinIgv + "\"," +
                "\"costoOriginalSinIGV\":\"" + costoOriginal + "\"," +
                //     "\"costoAlternativoSinIGV\":\"" + producto.costoAlternativoSinIgv + "\"," +
                "\"fleteDetalle\":\"" + fleteDetalle + "\"," +
                "\"precioUnitario\":\"" + precioUnitario + "\"," +
                "\"porcentajeDescuento\":\"" + porcentajeDescuento + "\"," +
                "\"precioListaList\":" + jsonPrecioLista + "," +
                "\"productoPresentacionList\":" + jsonProductoPresentacion + "," +
                "\"costoLista\":\"" + producto.costoLista + "\"" +
                "}";
            return resultado;


        }


        public String AddProducto()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA];
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            Guid idProducto = Guid.Parse(this.Session["idProducto"].ToString());
            PedidoDetalle pedidoDetalle = pedido.pedidoDetalleList.Where(p => p.producto.idProducto == idProducto).FirstOrDefault();
            if (pedidoDetalle != null)
            {
                throw new System.Exception("Producto ya se encuentra en la lista");
            }

            PedidoDetalle detalle = new PedidoDetalle(usuario.visualizaCostos,usuario.visualizaMargen);
            ProductoBL productoBL = new ProductoBL();
            Producto producto = productoBL.getProducto(idProducto, pedido.ciudad.esProvincia, pedido.incluidoIGV, pedido.cliente.idCliente);
            detalle.producto = producto;

            detalle.cantidad = Int32.Parse(Request["cantidad"].ToString());
            detalle.porcentajeDescuento = Decimal.Parse(Request["porcentajeDescuento"].ToString());
            detalle.esPrecioAlternativo = Int16.Parse(Request["esPrecioAlternativo"].ToString()) == 1;
            int idProductoPresentacion = Int16.Parse(Request["idProductoPresentacion"].ToString());


            detalle.observacion = Request["observacion"].ToString();
            decimal precioNeto = Decimal.Parse(Request["precio"].ToString());
            decimal costo = Decimal.Parse(Request["costo"].ToString());
            decimal flete = Decimal.Parse(Request["flete"].ToString());
            if (detalle.esPrecioAlternativo)
            {
                //Si es el precio Alternativo se multiplica por la equivalencia para que se registre el precio estandar
                //dado que cuando se hace get al precioNetoEquivalente se recupera diviendo entre la equivalencia
                ProductoPresentacion productoPresentacion = producto.getProductoPresentacion(idProductoPresentacion);
                detalle.unidad = productoPresentacion.Presentacion;
                detalle.ProductoPresentacion = productoPresentacion;
                detalle.precioNeto = Decimal.Parse(String.Format(Constantes.formatoCuatroDecimales, precioNeto * detalle.ProductoPresentacion.Equivalencia));

                //Si es el precio Alternativo se debe modificar el precio_cliente_producto para que compare con el precio
                //de la unidad alternativa en lugar del precio de la unidad estandar
                detalle.producto.precioClienteProducto.precioUnitario =
                    detalle.producto.precioClienteProducto.precioUnitario / detalle.ProductoPresentacion.Equivalencia;


            }
            else
            {
                detalle.unidad = detalle.producto.unidad;
                detalle.precioNeto = precioNeto;
            }
            detalle.flete = flete;
            pedido.pedidoDetalleList.Add(detalle);

            //CotizacionDetalle cotizacionDetalle = (CotizacionDetalle)Convert.ChangeType(pedido, typeof(CotizacionDetalle));

            //Calcula los montos totales de la cabecera de la cotizacion
            PedidoBL pedidoBL = new PedidoBL();
            pedidoBL.calcularMontosTotales(pedido);


            /*detalle.unidad = detalle.producto.unidad;
            //si esPrecioAlternativo  se mostrará la unidad alternativa
            if (detalle.esPrecioAlternativo)
            {
                detalle.unidad = detalle.producto.unidad_alternativa;
            }
            */

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
                unidad = detalle.unidad,
                igv = pedido.montoIGV.ToString(),
                subTotal = pedido.montoSubTotal.ToString(),
                margen = detalle.margen,
                precioUnitario = detalle.precioUnitario,
                observacion = detalle.observacion,
                total = pedido.montoTotal.ToString(),
                precioUnitarioRegistrado = precioUnitarioRegistrado

            };

            this.PedidoSession = pedido;
            String resultado = JsonConvert.SerializeObject(v);
            return resultado;


        }

        #endregion





        /*Actualización de Campos*/
        #region ACTUALIZACION DE CAMPOS FORMULARIO

        public void ChangeInputString()
        {
            Pedido pedido = this.PedidoSession;
            PropertyInfo propertyInfo = pedido.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(pedido, this.Request.Params["valor"]);
            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA] = pedido;
        }

        public void ChangeUbigeoEntrega()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA];
            pedido.ubigeoEntrega.Id = this.Request.Params["ubigeoEntregaId"];
            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA] = pedido;
        }

        public string ChangeMoneda()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA];
            string moneda = this.Request.Params["moneda"];

            pedido.moneda = Moneda.ListaMonedas.Where(m => m.codigo.Equals(moneda)).FirstOrDefault();
            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA] = pedido;


            return "{\"simbolo\":\"" + (pedido.moneda != null ? pedido.moneda.simbolo : "") + "\"}";
        }

        public void ChangeNumeroReferenciaCliente()
        {
            Pedido pedido = this.PedidoSession;
            pedido.numeroReferenciaCliente = this.Request.Params["numeroReferenciaCliente"];
            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA] = pedido;
        }

        public void ChangeOtrosCargos()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA];
            pedido.otrosCargos = Decimal.Parse(this.Request.Params["otrosCargos"]);
            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA] = pedido;
        }
        

        public string ChangeDireccionEntrega()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA];

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
            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA] = pedido;
            return JsonConvert.SerializeObject(pedido.direccionEntrega);
        }

        public void ChangeDireccionEntregaDescripcion()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA];
            pedido.direccionEntrega.descripcion = this.Request.Params["direccionEntregaDescripcion"];
            pedido.existeCambioDireccionEntrega = true;
            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA] = pedido;
        }

        public void ChangeDireccionEntregaContacto()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA];
            pedido.direccionEntrega.contacto = this.Request.Params["direccionEntregaContacto"];
            pedido.existeCambioDireccionEntrega = true;
            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA] = pedido;
        }

        public void ChangeDireccionEntregaTelefono()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA];
            pedido.direccionEntrega.telefono = this.Request.Params["direccionEntregaTelefono"];
            pedido.existeCambioDireccionEntrega = true;
            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA] = pedido;
        }



        public string ChangeSolicitante()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA];

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
            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA] = pedido;
            return JsonConvert.SerializeObject(pedido.solicitante);
        }

        public void ChangeSolicitanteNombre()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA];
            pedido.solicitante.nombre = this.Request.Params["solicitanteNombre"];
            pedido.existeCambioSolicitante = true;
            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA] = pedido;
        }

        public void ChangeSolicitanteTelefono()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA];
            pedido.solicitante.telefono = this.Request.Params["solicitanteTelefono"];
            pedido.existeCambioSolicitante = true;
            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA] = pedido;
        }

        public void ChangeSolicitanteCorreo()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA];
            pedido.solicitante.correo = this.Request.Params["solicitanteCorreo"];
            pedido.existeCambioSolicitante = true;
            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA] = pedido;
        }


        public void ChangeFechaSolicitud()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA];
            String[] fechaSolicitud = this.Request.Params["fechaSolicitud"].Split('/');
            String[] horaSolicitud = this.Request.Params["horaSolicitud"].Split(':');
            pedido.fechaSolicitud = new DateTime(Int32.Parse(fechaSolicitud[2]), Int32.Parse(fechaSolicitud[1]), Int32.Parse(fechaSolicitud[0]), Int32.Parse(horaSolicitud[0]), Int32.Parse(horaSolicitud[1]), 0);
            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA] = pedido;
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
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA];
            pedido.contactoPedido = this.Request.Params["contactoPedido"];
            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA] = pedido;
        }

        public void ChangeTelefonoContactoPedido()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA];
            pedido.telefonoContactoPedido = this.Request.Params["telefonoContactoPedido"];
            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA] = pedido;
        }


        public void ChangeObservaciones()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA];

            pedido.GetType().GetProperty("observaciones").SetValue(pedido, this.Request.Params["observaciones"]);

            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA] = pedido;
        }


        public void ChangeFechaSolicitudDesde()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_BUSQUEDA];
            String[] solDesde = this.Request.Params["fechaSolicitudDesde"].Split('/');
            pedido.fechaSolicitudDesde = new DateTime(Int32.Parse(solDesde[2]), Int32.Parse(solDesde[1]), Int32.Parse(solDesde[0]));
            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_BUSQUEDA] = pedido;
        }

        public void ChangeFechaSolicitudHasta()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_BUSQUEDA];
            String[] solHasta = this.Request.Params["fechaSolicitudHasta"].Split('/');
            pedido.fechaSolicitudHasta = new DateTime(Int32.Parse(solHasta[2]), Int32.Parse(solHasta[1]), Int32.Parse(solHasta[0]), 23, 59, 59);
            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_BUSQUEDA] = pedido;
        }

        public void ChangeNumero()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_BUSQUEDA];
            if (this.Request.Params["numero"] == null || this.Request.Params["numero"].Trim().Length == 0)
            {
                pedido.numeroPedido = 0;
            }
            else
            {
                pedido.numeroPedido = long.Parse(this.Request.Params["numero"]);
            }
            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_BUSQUEDA] = pedido;
        }

        public void ChangeNumeroGrupo()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_BUSQUEDA];
            if (this.Request.Params["numeroGrupo"] == null || this.Request.Params["numeroGrupo"].Trim().Length == 0)
            {
                pedido.numeroGrupoPedido = 0;
            }
            else
            {
                pedido.numeroGrupoPedido = long.Parse(this.Request.Params["numeroGrupo"]);
            }
            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_BUSQUEDA] = pedido;
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

        public void ChangeTipoPedido()
        {
            Char tipoPedidoCompra = Convert.ToChar(Int32.Parse(this.Request.Params["tipoPedido"]));
            this.PedidoSession.tipoPedidoCompra = (Pedido.tiposPedidoCompra)tipoPedidoCompra;
        }

        public void ChangeTipoPedidoBusqueda()
        {
            Char tipoPedidoBusqueda = Convert.ToChar(Int32.Parse(this.Request.Params["tipoPedidoBusqueda"]));
            this.PedidoSession.tipoPedidoCompraBusqueda = (Pedido.tiposPedidoCompraBusqueda)tipoPedidoBusqueda;
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
            IDocumento documento = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA];
            List<DocumentoDetalle> documentoDetalle = HelperDocumento.updateDocumentoDetalle(documento, cotizacionDetalleJsonList);
            documento.documentoDetalle = documentoDetalle;
            PedidoBL pedidoBL = new PedidoBL();
            pedidoBL.calcularMontosTotales((Pedido)documento);
            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA] = documento;
            return "{\"cantidad\":\"" + documento.documentoDetalle.Count + "\"}";
        }



        #endregion


        #region CAMBIOS CAMPOS DE FORMULARIO BUSQUEDA

    

        public void ChangeEstado()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_BUSQUEDA];
            pedido.seguimientoPedido.estado = (SeguimientoPedido.estadosSeguimientoPedido)Int32.Parse(this.Request.Params["estado"]);
            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_BUSQUEDA] = pedido;
        }

        public void ChangeEstadoCrediticio()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_BUSQUEDA];
            pedido.seguimientoCrediticioPedido.estado = (SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido)Int32.Parse(this.Request.Params["estadoCrediticio"]);
            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_BUSQUEDA] = pedido;
        }


        #endregion




        #region CREAR/ACTUALIZAR PEDIDO

        public String UpdatePost()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_VER];
            pedido.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            //pedido.
            PedidoBL pedidoBL = new PedidoBL();
            pedido.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            pedido.numeroReferenciaAdicional = this.Request.Params["numeroReferenciaAdicional"];
            pedido.numeroReferenciaCliente = this.Request.Params["numeroReferenciaCliente"];
            pedido.observaciones = this.Request.Params["observaciones"];
            pedido.observacionesFactura = this.Request.Params["observacionesFactura"];




            pedidoBL.ActualizarPedido(pedido);
            long numeroPedido = pedido.numeroPedido;
            String numeroPedidoString = pedido.numeroPedidoString;
            Guid idPedido = pedido.idPedido;
            int estado = (int)pedido.seguimientoPedido.estado;







          
            var v = new { numeroPedido = numeroPedidoString, estado = estado, idPedido = idPedido };
            String resultado = JsonConvert.SerializeObject(v);
            return resultado;
        }

        public void ChangeFiles(List<HttpPostedFileBase> files)
        {
           
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA];

            if((int)this.Session[Constantes.VAR_SESSION_PAGINA] == (int)Constantes.paginas.BusquedaPedidos )
                pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_VER];


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
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA];
            if ((int)this.Session[Constantes.VAR_SESSION_PAGINA] == (int)Constantes.paginas.BusquedaPedidos)
                pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_VER];


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
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA];

            if ((int)this.Session[Constantes.VAR_SESSION_PAGINA] == (int)Constantes.paginas.BusquedaPedidos)
            {
                pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_VER];

            }

            ArchivoAdjunto archivoAdjunto = pedido.pedidoAdjuntoList.Where(p => p.nombre.Equals(nombreArchivo)).FirstOrDefault();

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




        public String Create()
        {

            //RUC_MP
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            int continuarLuego = int.Parse(Request["continuarLuego"].ToString());
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA];
            pedido.usuario = usuario;
            PedidoBL pedidoBL = new PedidoBL();

            if (pedido.idPedido != Guid.Empty || pedido.numeroPedido > 0)
            {
                throw new System.Exception("Pedido ya se encuentra creado");
            }

            pedidoBL.InsertPedidoCompra(pedido);
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
            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA] = null;// pedido;// null;


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
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA];
            pedido.usuario = usuario;
            PedidoBL bl = new PedidoBL();
            bl.UpdatePedidoCompra(pedido);
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
            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA] = null;// pedido;

            usuarioBL.updatePedidoSerializado(usuario, null);

            var v = new { numeroPedido = numeroPedidoString, estado = estado, observacion = observacion, idPedido = idPedido };
            String resultado = JsonConvert.SerializeObject(v);
            
            //String resultado = "{ \"codigo\":\"" + numeroPedido + "\", \"estado\":\"" + estado + "\", \"observacion\":\"" + observacion + "\" }";
            return resultado;
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





        public void updateEstadoPedido()
        {
            /*Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_ENTRADA_VER];
            SeguimientoPedido.estadosSeguimientoPedido estadosSeguimientoPedido = (SeguimientoPedido.estadosSeguimientoPedido)Int32.Parse(Request["estado"].ToString());
            String observacion = Request["observacion"].ToString();*/

            Guid idPedido = Guid.Parse(Request["idPedido"].ToString());
            SeguimientoPedido.estadosSeguimientoPedido estadosSeguimientoPedido = (SeguimientoPedido.estadosSeguimientoPedido)Int32.Parse(Request["estado"].ToString());
            String observacion = Request["observacion"].ToString();
            updateEstadoSeguimientoPedido(idPedido, estadosSeguimientoPedido, observacion);
        }

        public void updateEstadoPedidoCrediticio()
        {
            Guid idPedido = Guid.Parse(Request["idPedido"].ToString());
            SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido estadosSeguimientoCrediticioPedido = (SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido)Int32.Parse(Request["estado"].ToString());
            String observacion = Request["observacion"].ToString();
            updateEstadoSeguimientoCrediticioPedido(idPedido, estadosSeguimientoCrediticioPedido, observacion);
        }


        public String Programar()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_VER];
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
           // Pedido cotizacionSession = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_ENTRADA];
            PedidoBL pedidoBL = new PedidoBL();
            Pedido pedido = new Pedido(Pedido.ClasesPedido.Compra);
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
            Pedido cotizacionSession = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA];
            PedidoBL pedidoBL = new PedidoBL();
            Pedido pedido = new Pedido(Pedido.ClasesPedido.Compra);
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

        public String Search()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaPedidos;
            //Se recupera el pedido Búsqueda de la session
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_BUSQUEDA];

            String[] solDesde = this.Request.Params["fechaCreacionDesde"].Split('/');
            pedido.fechaCreacionDesde = new DateTime(Int32.Parse(solDesde[2]), Int32.Parse(solDesde[1]), Int32.Parse(solDesde[0]));

            String[] solHasta = this.Request.Params["fechaCreacionHasta"].Split('/');
            pedido.fechaCreacionHasta = new DateTime(Int32.Parse(solHasta[2]), Int32.Parse(solHasta[1]), Int32.Parse(solHasta[0]), 23, 59, 59);

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


            pedido.seguimientoPedido.estado = (SeguimientoPedido.estadosSeguimientoPedido) Int32.Parse(this.Request.Params["estado"]);
            pedido.seguimientoCrediticioPedido.estado = (SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido)Int32.Parse(this.Request.Params["estadoCrediticio"]);

            PedidoBL pedidoBL = new PedidoBL();
            List<Pedido> pedidoList = pedidoBL.GetPedidosCompra(pedido);
            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_LISTA] = pedidoList;
            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_BUSQUEDA] = pedido;
            //Se retorna la cantidad de elementos encontrados
            return JsonConvert.SerializeObject(pedidoList);
            //return pedidoList.Count();
        }


        public String ConsultarSiExistePedido()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA];
            if (pedido == null)
                return "{\"existe\":\"false\",\"numero\":\"0\"}";
            else
                return "{\"existe\":\"true\",\"numero\":\"" + pedido.numeroPedido + "\"}";
        }

        public String Show()
        {
            PedidoBL pedidoBL = new PedidoBL();
            Pedido pedido = new Pedido(Pedido.ClasesPedido.Compra);
            pedido.idPedido = Guid.Parse(Request["idPedido"].ToString());
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            pedido = pedidoBL.GetPedido(pedido,usuario);
            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_VER] = pedido;
            string jsonUsuario = JsonConvert.SerializeObject(usuario);
            string jsonPedido = JsonConvert.SerializeObject(pedido);

  

            Ciudad ciudad = usuario.sedesMPPedidos.Where(s => s.idCiudad == pedido.ciudad.idCiudad).FirstOrDefault();
            /*

                        var seriesDocumentosElectronicosTmp = from s in ciudad.serieDocumentoElectronicoList
                                                              group s.tipoDocumento by new { s.serie, s.esPrincipal }  into g
                        select new { serie = g.Key, tiposDocumento = g.ToList() };

                        List<SerieDocumentoElectronico> serieDocumentoElectronicoList = new List<SerieDocumentoElectronico>();
                        foreach (var serieDocumentosElectronicoTmp  in seriesDocumentosElectronicosTmp)
                        {
                            SerieDocumentoElectronico serieDocumentosElectronico = new SerieDocumentoElectronico { serie = serieDocumentosElectronicoTmp.serie.serie, esPrincipal = serieDocumentosElectronicoTmp.serie.esPrincipal };
                            serieDocumentoElectronicoList.Add(serieDocumentosElectronico);
                        }

                */
            string jsonSeries = "[]";
            if (ciudad != null)
            {
                var serieDocumentoElectronicoList = ciudad.serieDocumentoElectronicoList.OrderByDescending(x => x.esPrincipal).ToList();
                jsonSeries = JsonConvert.SerializeObject(serieDocumentoElectronicoList);

            }

          
            String json = "{\"serieDocumentoElectronicoList\":" + jsonSeries + ", \"pedido\":" + jsonPedido + "}";
            return json;
        }

        public void autoGuardarPedido()
        {
            if (this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA] != null)
            {
                Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA];
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
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA];
            DireccionEntrega direccionEntrega = new DireccionEntrega();
            direccionEntrega.descripcion = Request["direccion"];
            direccionEntrega.contacto = Request["contacto"];
            direccionEntrega.telefono = Request["telefono"];
            direccionEntrega.idDireccionEntrega = Guid.Empty;
            pedido.cliente.direccionEntregaList.Add(direccionEntrega);
            pedido.direccionEntrega = direccionEntrega;
            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA] = pedido;
            return JsonConvert.SerializeObject(direccionEntrega);
        }

        public String CreateSolicitanteTemporal()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA];
            Solicitante solicitante = new Solicitante();
            solicitante.nombre = Request["nombre"];
            solicitante.telefono = Request["telefono"];
            solicitante.correo = Request["correo"];
            solicitante.idSolicitante = Guid.Empty;
            pedido.cliente.solicitanteList.Add(solicitante);
            pedido.solicitante = solicitante;
            this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA] = pedido;
            return JsonConvert.SerializeObject(solicitante);
        }


    }
}
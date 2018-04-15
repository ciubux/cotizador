using BusinessLayer;
using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Cotizador.Controllers
{
    public class PedidoController : Controller
    {

        private Pedido PedidoSession {
            get {

                Pedido pedido = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaPedidos: pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA]; break;
                    case Constantes.paginas.MantenimientoPedido: pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO]; break;
                }
                return pedido;
            }
            set {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaPedidos: this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA] = value; break;
                    case Constantes.paginas.MantenimientoPedido: this.Session[Constantes.VAR_SESSION_PEDIDO] = value; break;
                }
            }
        }
        
        // GET: Pedido
        public ActionResult Index()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.BUSQUEDA_PEDIDO;

            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA] == null)
            {
                Pedido pedidoTmp = new Pedido();
                pedidoTmp.fechaSolicitudDesde = DateTime.Now.AddDays(-10);
                DateTime fechaHasta = DateTime.Now;
                pedidoTmp.fechaSolicitudHasta = new DateTime(fechaHasta.Year, fechaHasta.Month, fechaHasta.Day, 23, 59, 59);
                pedidoTmp.fechaEntregaDesde = DateTime.Now.AddDays(-10);
                DateTime fechaEntregaHasta = DateTime.Now;
                pedidoTmp.fechaEntregaHasta = new DateTime(fechaEntregaHasta.Year, fechaEntregaHasta.Month, fechaEntregaHasta.Day, 23, 59, 59);

                pedidoTmp.ciudad = new Ciudad();
                pedidoTmp.cliente = new Cliente();
                pedidoTmp.seguimientoPedido = new SeguimientoPedido();
                pedidoTmp.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.Todos;
                // cotizacionTmp.cotizacionDetalleList = new List<CotizacionDetalle>();
                pedidoTmp.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                pedidoTmp.usuarioBusqueda = pedidoTmp.usuario;
                this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA] = pedidoTmp;
            }

            Pedido pedidoSearch = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA];


            ViewBag.fechaSolicitudDesde = pedidoSearch.fechaSolicitudDesde.ToString(Constantes.formatoFecha);
            ViewBag.fechaSolicitudHasta = pedidoSearch.fechaSolicitudHasta.ToString(Constantes.formatoFecha);
            ViewBag.fechaEntregaDesde = pedidoSearch.fechaEntregaDesde.Value.ToString(Constantes.formatoFecha);
            ViewBag.fechaEntregaHasta = pedidoSearch.fechaEntregaHasta.Value.ToString(Constantes.formatoFecha);

            if (this.Session[Constantes.VAR_SESSION_PEDIDO_LISTA] == null)
            {
                this.Session[Constantes.VAR_SESSION_PEDIDO_LISTA] = new List<Pedido>();
            }

            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA];
            int existeCliente = 0;
            //  if (cotizacion.cliente.idCliente != Guid.Empty || cotizacion.grupo.idGrupo != Guid.Empty)
            if (pedido.cliente.idCliente != Guid.Empty)
            {
                existeCliente = 1;
            }

            GuiaRemision guiaRemision = new GuiaRemision();
            guiaRemision.motivoTraslado = GuiaRemision.motivosTraslado.Venta;

            ViewBag.guiaRemision = guiaRemision;

            ViewBag.pedido = pedido;

            ViewBag.pedidoList = this.Session[Constantes.VAR_SESSION_PEDIDO_LISTA];
            ViewBag.existeCliente = existeCliente;

            return View();
        }


        public ActionResult Pedir()
        {
            try
            {
                this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.MANTENIMIENTO_PEDIDO;

                //Si no hay usuario, se dirige el logueo
                if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
                {
                    return RedirectToAction("Login", "Account");
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

                ViewBag.fechaEntregaDesde = pedido.fechaEntregaDesde ==null?"": pedido.fechaEntregaDesde.Value.ToString(Constantes.formatoFecha);
                ViewBag.fechaEntregaHasta = pedido.fechaEntregaHasta == null?"": pedido.fechaEntregaHasta.Value.ToString(Constantes.formatoFecha);

       

                ViewBag.pedido = pedido;


                ViewBag.fechaPrecios = pedido.fechaPrecios.ToString(Constantes.formatoFecha);
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
            Pedido pedido = new Pedido();
            pedido.idPedido = Guid.Empty;
            pedido.numeroPedido = 0;
            pedido.numeroGrupoPedido = null;
            Cotizacion cotizacion = (Cotizacion)this.Session[Constantes.VAR_SESSION_COTIZACION_VER];
            pedido.cotizacion = new Cotizacion();
            pedido.cotizacion.idCotizacion = cotizacion.idCotizacion;
            pedido.cotizacion.codigo = cotizacion.codigo;

          /*  pedido.montoIGV = cotizacion.montoIGV;
            pedido.montoSubTotal = cotizacion.montoSubTotal;
            pedido.montoTotal = cotizacion.montoTotal;
            pedido.montoIGV = cotizacion.montoIGV;*/
            pedido.ciudad = cotizacion.ciudad;
            pedido.cliente = cotizacion.cliente;
            pedido.numeroReferenciaCliente = null;
            pedido.direccionEntrega = new DireccionEntrega();
            pedido.fechaSolicitud = DateTime.Now;
            pedido.fechaEntregaDesde = DateTime.Now;
            pedido.fechaEntregaHasta = DateTime.Now;
            pedido.contactoPedido = String.Empty;
            pedido.telefonoContactoPedido = String.Empty;
            pedido.incluidoIGV = false;
            //  pedido.tasaIGV = Constantes.IGV;
            //pedido.flete = 0;
            // pedido.mostrarCodigoProveedor = true;
            pedido.observaciones = String.Empty;

            pedido.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            pedido.seguimientoPedido = new SeguimientoPedido();

            pedido.pedidoDetalleList = new List<PedidoDetalle>();
            foreach (DocumentoDetalle documentoDetalle in  cotizacion.documentoDetalle)
            {
                PedidoDetalle pedidoDetalle = new PedidoDetalle();
                pedidoDetalle.cantidad = documentoDetalle.cantidad;
                if (documentoDetalle.cantidad == 0)
                    pedidoDetalle.cantidad = 1;
                pedidoDetalle.costoAnterior = documentoDetalle.costoAnterior;
                pedidoDetalle.esPrecioAlternativo = documentoDetalle.esPrecioAlternativo;
                pedidoDetalle.flete = documentoDetalle.flete;
                pedidoDetalle.observacion = documentoDetalle.observacion;
                pedidoDetalle.porcentajeDescuento = documentoDetalle.porcentajeDescuento;             
                pedidoDetalle.precioNeto = documentoDetalle.precioNeto;
                pedidoDetalle.precioNetoAnterior = documentoDetalle.precioNetoAnterior;
                pedidoDetalle.producto = documentoDetalle.producto;
                pedidoDetalle.unidad = documentoDetalle.unidad;
                pedido.pedidoDetalleList.Add(pedidoDetalle);
            }
            pedido.fechaPrecios = pedido.fechaSolicitud.AddDays(Constantes.DIAS_MAX_BUSQUEDA_PRECIOS * -1);
            HelperDocumento.calcularMontosTotales(pedido);
            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;


        }

        private void instanciarPedido()
        {
            Pedido pedido = new Pedido();
            pedido.idPedido = Guid.Empty;
            pedido.numeroPedido = 0;
            pedido.numeroGrupoPedido = null;
            pedido.cotizacion = new Cotizacion();
            pedido.ubigeoEntrega = new Ubigeo();
            pedido.ubigeoEntrega.Id = "000000";
            pedido.ciudad = new Ciudad();
            pedido.cliente = new Cliente();
           

            pedido.numeroReferenciaCliente = null;
            pedido.direccionEntrega = new DireccionEntrega(); 
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
            pedido.pedidoDetalleList = new List<PedidoDetalle>();
            pedido.fechaPrecios = pedido.fechaSolicitud.AddDays(Constantes.DIAS_MAX_BUSQUEDA_PRECIOS * -1);

            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
        }

        public void iniciarEdicionPedido()
        {
            Pedido pedidoVer = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_VER];
            PedidoBL pedidoBL = new PedidoBL();
            Pedido pedido = new Pedido();
            pedido.idPedido = pedidoVer.idPedido;
            //    pedido.fechaModificacion = cotizacionVer.fechaModificacion;
            pedido.seguimientoPedido = new SeguimientoPedido();
            pedido.usuario = (Usuario)this.Session["usuario"];
            //Se cambia el estado de la cotizacion a Edición
            pedido.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.Edicion;
            pedidoBL.cambiarEstadoPedido(pedido);
            //Se obtiene los datos de la cotización ya modificada
            pedido = pedidoBL.GetPedido(pedido);

            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
        }


        public ActionResult CancelarCreacionPedido()
        {
            this.Session[Constantes.VAR_SESSION_PEDIDO] = null;
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session["usuario"];
         //   usuarioBL.updateCotizacionSerializada(usuario, null);
            return RedirectToAction("Index", "Pedido");
        }



        #region CONTROLES CHOOSEN

        public String SearchClientes()
        {
            String data = this.Request.Params["data[q]"];
            ClienteBL clienteBL = new ClienteBL();
            Pedido pedido = this.PedidoSession;

            List<Cliente> clienteList = clienteBL.getCLientesBusqueda(data, pedido.ciudad.idCiudad);
            String resultado = "{\"q\":\"" + data + "\",\"results\":[";
            Boolean existeCliente = false;
            foreach (Cliente cliente in clienteList)
            {
                resultado += "{\"id\":\"" + cliente.idCliente + "\",\"text\":\"" + cliente.ToString() + "\"},";
                existeCliente = true;
            }
            if (existeCliente)
                resultado = resultado.Substring(0, resultado.Length - 1) + "]}";
            else
                resultado = resultado.Substring(0, resultado.Length) + "]}";
            return resultado;
        }



        public String GetCliente()
        {


            Pedido pedido = this.PedidoSession; 
            Guid idCliente = Guid.Parse(Request["idCliente"].ToString());
            ClienteBL clienteBl = new ClienteBL();
            pedido.cliente = clienteBl.getCliente(idCliente);

            //Se obtiene la lista de direccioines de entrega registradas para el cliente
            DireccionEntregaBL direccionEntregaBL = new DireccionEntregaBL();
        /*    pedido.cliente.direccionEntregaList = new List<DireccionEntrega>();
            DireccionEntrega seleccioneDireccionEntrega = new DireccionEntrega();
            seleccioneDireccionEntrega.descripcion = Constantes.LABEL_DIRECCION_ENTREGA_VACIO;
            seleccioneDireccionEntrega.idDireccionEntrega = Guid.Empty;

            pedido.cliente.direccionEntregaList.Add(seleccioneDireccionEntrega);
            List<DireccionEntrega> direccionEntregaList = direccionEntregaBL.getDireccionesEntrega(idCliente);
            foreach (DireccionEntrega direccionEntrega in direccionEntregaList)
            {
                pedido.cliente.direccionEntregaList.Add(direccionEntrega);
            }*/

            pedido.cliente.direccionEntregaList = direccionEntregaBL.getDireccionesEntrega(idCliente);
            pedido.direccionEntrega = new DireccionEntrega();


            //Se limpia el ubigeo de entrega
            pedido.ubigeoEntrega.Id = Constantes.UBIGEO_VACIO;

            String resultado = JsonConvert.SerializeObject(pedido.cliente);

            this.PedidoSession = pedido;
            return resultado;
        }

        #endregion



        #region AGREGAR PRODUCTO


        public String GetProducto()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            //Se recupera el producto y se guarda en Session
            Guid idProducto = Guid.Parse(Request["idProducto"].ToString());
            this.Session["idProducto"] = idProducto.ToString();

            //Para recuperar el producto se envia si la sede seleccionada es provincia o no
            ProductoBL bl = new ProductoBL();
            Producto producto = bl.getProducto(idProducto, pedido.ciudad.esProvincia, pedido.incluidoIGV, pedido.cliente.idCliente);

            Decimal fleteDetalle = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, producto.costoLista * (0) / 100));
            Decimal precioUnitario = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, fleteDetalle + producto.precioLista));

            Decimal porcentajeDescuento = 0;
            if (producto.precioNeto != null)
            {
                porcentajeDescuento = 100 - (producto.precioNeto.Value * 100 / producto.precioLista);
            }

            String jsonPrecioLista = JsonConvert.SerializeObject(producto.precioListaList);

            String resultado = "{" +
                "\"id\":\"" + producto.idProducto + "\"," +
                "\"nombre\":\"" + producto.descripcion + "\"," +
                "\"image\":\"data:image/png;base64, " + Convert.ToBase64String(producto.image) + "\"," +
                "\"unidad\":\"" + producto.unidad + "\"," +
                "\"unidad_alternativa\":\"" + producto.unidad_alternativa + "\"," +
                "\"proveedor\":\"" + producto.proveedor + "\"," +
                "\"familia\":\"" + producto.familia + "\"," +
                "\"precioUnitarioSinIGV\":\"" + producto.precioSinIgv + "\"," +
                "\"precioUnitarioAlternativoSinIGV\":\"" + producto.precioAlternativoSinIgv + "\"," +
                "\"precioLista\":\"" + producto.precioLista + "\"," +
                "\"costoSinIGV\":\"" + producto.costoSinIgv + "\"," +
                "\"costoAlternativoSinIGV\":\"" + producto.costoAlternativoSinIgv + "\"," +
                "\"fleteDetalle\":\"" + fleteDetalle + "\"," +
                "\"precioUnitario\":\"" + precioUnitario + "\"," +
                "\"porcentajeDescuento\":\"" + porcentajeDescuento + "\"," +
                "\"precioListaList\":" + jsonPrecioLista + "," +

                "\"costoLista\":\"" + producto.costoLista + "\"" +
                "}";
            return resultado;


        }


        public String AddProducto()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            Guid idProducto = Guid.Parse(this.Session["idProducto"].ToString());
            PedidoDetalle pedidoDetalle = pedido.pedidoDetalleList.Where(p => p.producto.idProducto == idProducto).FirstOrDefault();
            if (pedidoDetalle != null)
            {
                throw new System.Exception("Producto ya se encuentra en la lista");
            }

            PedidoDetalle detalle = new PedidoDetalle();
            ProductoBL productoBL = new ProductoBL();
            Producto producto = productoBL.getProducto(idProducto, pedido.ciudad.esProvincia, pedido.incluidoIGV, pedido.cliente.idCliente);
            detalle.producto = producto;

            detalle.cantidad = Int32.Parse(Request["cantidad"].ToString());
            detalle.porcentajeDescuento = Decimal.Parse(Request["porcentajeDescuento"].ToString());
            detalle.esPrecioAlternativo = Int16.Parse(Request["esPrecioAlternativo"].ToString()) == 1;
            detalle.observacion = Request["observacion"].ToString();
            decimal precioNeto = Decimal.Parse(Request["precio"].ToString());
            decimal costo = Decimal.Parse(Request["costo"].ToString());
            decimal flete = Decimal.Parse(Request["flete"].ToString());
            if (detalle.esPrecioAlternativo)
            {
                //Si es el precio Alternativo se multiplica por la equivalencia para que se registre el precio estandar
                //dado que cuando se hace get al precioNeto se recupera diviendo entre la equivalencia
                detalle.precioNeto = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, precioNeto * producto.equivalencia));
            }
            else
            {
                detalle.precioNeto = precioNeto;
            }
            detalle.flete = flete;
            pedido.pedidoDetalleList.Add(detalle);

            //CotizacionDetalle cotizacionDetalle = (CotizacionDetalle)Convert.ChangeType(pedido, typeof(CotizacionDetalle));

            //Calcula los montos totales de la cabecera de la cotizacion
            HelperDocumento.calcularMontosTotales(pedido);


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
        //    }

          /*  if (pedido.considerarCantidades == Cotizacion.OpcionesConsiderarCantidades.Ambos)
            {*/
          //      nombreProducto = nombreProducto + "\\n" + detalle.observacion;
            //}

            String resultado = "{" +
                "\"idProducto\":\"" + detalle.producto.idProducto + "\"," +
                "\"codigoProducto\":\"" + detalle.producto.sku + "\"," +
                "\"nombreProducto\":\"" + nombreProducto + "\"," +
                "\"unidad\":\"" + detalle.unidad + "\"," +
                "\"igv\":\"" + pedido.montoIGV.ToString() + "\", " +
                "\"subTotal\":\"" + pedido.montoSubTotal.ToString() + "\", " +
                "\"margen\":\"" + detalle.margen + "\", " +
                "\"precioUnitario\":\"" + detalle.precioUnitario + "\", " +
                "\"observacion\":\"" + detalle.observacion + "\", " +
                "\"total\":\"" + pedido.montoTotal.ToString() + "\"}";

            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
            return resultado;


        }

        #endregion





        /*Actualización de Campos*/
        #region ACTUALIZACION DE CAMPOS FORMULARIO

        public void ChangeInputString()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            PropertyInfo propertyInfo = pedido.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(pedido, this.Request.Params["valor"]);
            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
        }

        public void ChangeUbigeoEntrega()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            pedido.ubigeoEntrega.Id = this.Request.Params["ubigeoEntregaId"];
            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
        }
      

        public void ChangeNumeroReferenciaCliente()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            pedido.numeroReferenciaCliente = this.Request.Params["numeroReferenciaCliente"];
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


        [HttpPost]
        public String ChangeDetalle(List<DocumentoDetalleJson> cotizacionDetalleJsonList)
        {
            IDocumento documento = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            List<DocumentoDetalle> documentoDetalle = HelperDocumento.updateDocumentoDetalle(documento, cotizacionDetalleJsonList);
            documento.documentoDetalle = documentoDetalle;
            HelperDocumento.calcularMontosTotales(documento);
            this.Session[Constantes.VAR_SESSION_PEDIDO] = documento;
            return "{\"cantidad\":\"" + documento.documentoDetalle.Count + "\"}";
        }

        #endregion


        #region CAMBIOS CAMPOS DE FORMULARIO BUSQUEDA

    

        public void ChangeEstado()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA];
            pedido.seguimientoPedido.estado = (SeguimientoPedido.estadosSeguimientoPedido)Int32.Parse(this.Request.Params["estado"]);
            this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA] = pedido;
        }
        

        #endregion




        #region CREAR/ACTUALIZAR PEDIDO

        public String Create()
        {
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            int continuarLuego = int.Parse(Request["continuarLuego"].ToString());
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            pedido.usuario = usuario;
            PedidoBL pedidoBL = new PedidoBL();
            pedidoBL.InsertPedido(pedido);
            long numeroPedido = pedido.numeroPedido;
            Guid idPedido = pedido.idPedido;
            int estado = (int)pedido.seguimientoPedido.estado;
            if (continuarLuego == 1)
            {
                SeguimientoPedido.estadosSeguimientoPedido estadosSeguimientoPedido = SeguimientoPedido.estadosSeguimientoPedido.Edicion;
                estado = (int)estadosSeguimientoPedido;
                String observacion = "Se continuará editando luego";
                updateEstadoSeguimientoPedido(idPedido, estadosSeguimientoPedido, observacion);
            }
            pedido = null;
            this.Session[Constantes.VAR_SESSION_PEDIDO] = null;


            usuarioBL.updatePedidoSerializado(usuario, null);
            String resultado = "{ \"codigo\":\"" + numeroPedido + "\", \"estado\":\"" + estado + "\" }";
            return resultado;
        }





        public String Update()
        {
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            int continuarLuego = int.Parse(Request["continuarLuego"].ToString());
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            pedido.usuario = usuario;
            PedidoBL bl = new PedidoBL();
            bl.UpdatePedido(pedido);
            long codigo = pedido.numeroPedido;
            Guid idPedido = pedido.idPedido;
            int estado = (int)pedido.seguimientoPedido.estado;
            if (continuarLuego == 1)
            {
                SeguimientoPedido.estadosSeguimientoPedido estadosSeguimientoPedido = SeguimientoPedido.estadosSeguimientoPedido.Edicion;
                estado = (int)estadosSeguimientoPedido;
                String observacion = "Se continuará editando luego";
                updateEstadoSeguimientoPedido(idPedido, estadosSeguimientoPedido, observacion);
            }
            pedido = null;
            this.Session[Constantes.VAR_SESSION_PEDIDO] = null;


            usuarioBL.updateCotizacionSerializada(usuario, null);
            String resultado = "{ \"codigo\":\"" + codigo + "\", \"estado\":\"" + estado + "\" }";
            return resultado;
        }



        private void updateEstadoSeguimientoPedido(Guid idPedido, SeguimientoPedido.estadosSeguimientoPedido estado, String observacion)
        {
            Pedido cotizacionSession = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            PedidoBL pedidoBL = new PedidoBL();
            Pedido pedido = new Pedido();
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
        #endregion

        public int Search()
        {
            //Se recupera el pedido Búsqueda de la session
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA];

            String[] solDesde = this.Request.Params["fechaSolicitudDesde"].Split('/');
            pedido.fechaSolicitudDesde = new DateTime(Int32.Parse(solDesde[2]), Int32.Parse(solDesde[1]), Int32.Parse(solDesde[0]));

            String[] solHasta = this.Request.Params["fechaSolicitudHasta"].Split('/');
            pedido.fechaSolicitudHasta = new DateTime(Int32.Parse(solHasta[2]), Int32.Parse(solHasta[1]), Int32.Parse(solHasta[0]),23,59,59);

            String[] entregaDesde = this.Request.Params["fechaEntregaDesde"].Split('/');
            pedido.fechaEntregaDesde = new DateTime(Int32.Parse(entregaDesde[2]), Int32.Parse(entregaDesde[1]), Int32.Parse(entregaDesde[0]));

            String[] entregaHasta = this.Request.Params["fechaEntregaHasta"].Split('/');
            pedido.fechaEntregaHasta = new DateTime(Int32.Parse(entregaHasta[2]), Int32.Parse(entregaHasta[1]), Int32.Parse(entregaHasta[0]), 23, 59, 59);


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

            PedidoBL pedidoBL = new PedidoBL();
            List<Pedido> pedidoList = pedidoBL.GetPedidos(pedido);
            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_PEDIDO_LISTA] = pedidoList;
            this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA] = pedido;
            //Se retorna la cantidad de elementos encontrados
            return pedidoList.Count();
        }


        public Boolean ConsultarSiExistePedido()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            if (pedido == null)
                return false;
            else
                return true;
        }

        public String Show()
        {
            PedidoBL pedidoBL = new PedidoBL();
            Pedido pedido = new Pedido();
            pedido.idPedido = Guid.Parse(Request["idPedido"].ToString());
            pedido = pedidoBL.GetPedido(pedido);
            this.Session[Constantes.VAR_SESSION_PEDIDO_VER] = pedido;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            string jsonUsuario = JsonConvert.SerializeObject(usuario);
            string jsonPedido = JsonConvert.SerializeObject(pedido);
            String json = "{\"usuario\":" + jsonUsuario + ", \"pedido\":" + jsonPedido + "}";
            return json;
        }

        public void autoGuardarCotizacion()
        {
            if (this.Session[Constantes.VAR_SESSION_PEDIDO] != null)
            {
                Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
                UsuarioBL usuarioBL = new UsuarioBL();
                Usuario usuario = (Usuario)this.Session["usuario"];

                String cotizacionSerializada = JsonConvert.SerializeObject(pedido);

                usuarioBL.updateCotizacionSerializada(usuario, cotizacionSerializada);
            }

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


    }
}